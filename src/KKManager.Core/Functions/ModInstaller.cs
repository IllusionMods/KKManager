using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;

namespace KKManager.Functions
{
    public static class ModInstaller
    {
        public static void InstallFromUnknownFile(string fileName)
        {
            var file = new FileInfo(fileName);
            Exception toThrow = null;

            if (SideloaderModLoader.IsValidZipmodExtension(file.Extension))
            {
                try
                {
                    // This will throw NotSupportedException on hardmods so no need to check it later
                    InstallZipmod(fileName);
                    return;
                }
                catch (InvalidDataException ex)
                {
                    // Not a zipmod
                    toThrow = ex;
                }
            }

            if (PluginLoader.IsValidPluginExtension(file.Extension))
            {
                // If it throws, there's a good reason
                InstallPlugin(fileName);
                return;
            }

            /* todo fix and reenable
             * if (ZipFile.IsZipFile(fileName))
            {
                InstallFromArchive(fileName);
                return;
            }*/

            if (toThrow != null)
                throw toThrow;

            throw new InvalidDataException("The file format is not supported, or the file is broken");
        }

        /*private static void InstallFromArchive(string fileName)
        {
            using (var archive = SharpCompress.Archives.ArchiveFactory.Open(fileName))
            {
                var any = false;
                var bepin = archive.Entries.FirstOrDefault(x => x.IsDirectory && PathTools.PathsEqual(x.Key, "bepinex"));
                if (bepin != null)
                {
                    // refuses to work, throws access denied when trying to extract over existing bepinex
                    bepin.Extract(InstallDirectoryHelper.KoikatuDirectory.FullName, ExtractExistingFileAction.OverwriteSilently);
                    any = true;
                    // Alternative way? Go file per file and create dirs as needed and delete old files as needed
                    using (ZipFile zip1 = ZipFile.Read(fileName))
                    {
                        var selection = (from e in zip1.Entries
                                            where (e.FileName).StartsWith("CSS/")
                                            select e);


                        Directory.CreateDirectory(outputDirectory);

                        foreach (var e in selection)
                        {
                            e.Extract(outputDirectory);
                        }
                    }
                }

                var mods = archive.Entries.FirstOrDefault(x => x.IsDirectory && PathTools.PathsEqual(x.Key, "mods"));
                if (mods != null)
                {
                    mods.Extract(InstallDirectoryHelper.KoikatuDirectory.FullName, ExtractExistingFileAction.OverwriteSilently);
                    any = true;
                }

                if (!any)
                    throw new InvalidDataException("The archive is in an unknown format. If it contains a mod extract it and try again.");

                if (!any)
                {
                    var entries = archive.Where(x => !x.IsDirectory)
                        .Select(x => new { entry = x, ext = Path.GetExtension(x.FileName) })
                        .Where(x => !string.IsNullOrEmpty(x.ext))
                        .ToList();

                    var plugs = new Dictionary<ZipEntry, PluginInfo>();
                    var anyNonPluginDlls = false;
                    foreach (var entry in entries.Where(x => PluginLoader.IsValidPluginExtension(x.ext)))
                    {
                        var tempFilename = Path.GetTempFileName() + ".dll";
                        entry.entry.Extract(tempFilename);
                        try
                        {
                            foreach (var pluginInfo in InstallPlugin(tempFilename))
                                plugs.Add(entry.entry, pluginInfo);
                        }
                        catch
                        {
                            anyNonPluginDlls = true;
                        }
                    }
                    if (plugs.Any())
                    {
                        var groups = plugs.GroupBy(x => Path.GetDirectoryName(x.Key.FileName)).ToList();
                        if (groups.Count == 1)
                        {
                            var dir = groups.Single().Key;
                            var container = archive.Entries.First(x => x.IsDirectory && x.FileName.Equals(dir, StringComparison.OrdinalIgnoreCase));
                            container.Extract();
                        }
                    }
                }
            }
        }*/

        private static void InstallPlugin(string fileName)
        {
            List<PluginInfo> newPlugs;
            try
            {
                newPlugs = PluginLoader.LoadFromFile(fileName).ToList();
            }
            catch (SystemException ex)
            {
                throw new InvalidDataException("The file is not a plugin or is broken - " + ex.Message, ex);
            }

            var oldPlugs = PluginLoader.Plugins.ToList().Wait();

            var relations = newPlugs.Join(oldPlugs, p => p.Guid, p => p.Guid,
                (newPlug, oldPlug) => new { newPlug, oldPlug })
                .ToList();

            if (!relations.Any() || relations.Any(x => new Version(x.newPlug.Version) > new Version(x.oldPlug.Version)))
            {
                var missing = new List<string>();
                foreach (var newPlug in newPlugs)
                {
                    missing.AddRange(newPlug.Dependancies.Where(plugDependancy => oldPlugs.Concat(newPlugs).All(x => x.Guid != plugDependancy)));
                }
                if (missing.Any())
                {
                    throw new InvalidOperationException("You are missing plugins required for this mod to work. Install the following and try again:\n\n"
                        + string.Join("\n", missing.Distinct().OrderBy(x => x)));
                }

                foreach (var relation in relations)
                    relation.oldPlug.Location.Delete();
                var file = new FileInfo(fileName);
                file.CopyTo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, file.Name));
            }
            else if (relations.Any(x => new Version(x.newPlug.Version) == new Version(x.oldPlug.Version)))
            {
                throw new InvalidOperationException("You already have this plugin installed");
            }
            else
            {
                throw new InvalidOperationException("You already have a newer version of this plugin installed");
            }
        }

        private static void InstallZipmod(string fileName)
        {
            SideloaderModInfo newMod;
            try
            {
                newMod = SideloaderModLoader.LoadFromFile(fileName);
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                throw new InvalidDataException("The file is not a zipmod or is broken - " + ex.Message, ex);
            }

            if (newMod != null)
            {
                try
                {
                    var modDirectory = InstallDirectoryHelper.ModsPath.FullName;

                    var oldMod = SideloaderModLoader.Zipmods.ToList().Wait().FirstOrDefault(x => x.Guid == newMod.Guid);

                    if (oldMod != null)
                    {
                        switch (SideloaderVersionComparer.CompareVersions(newMod.Version, oldMod.Version))
                        {
                            case -1:
                                throw new InvalidOperationException("You already have a newer version of this zipmod installed");
                            case 0:
                                throw new InvalidOperationException("You already have this zipmod installed");
                            case 1:
                                oldMod.Location.Delete();
                                var newPath = oldMod.Location.Directory?.FullName;
                                if (!string.IsNullOrWhiteSpace(newPath))
                                {
                                    newMod.Location.CopyTo(Path.Combine(newPath, newMod.Location.Name));
                                    return;
                                }
                                break;
                        }
                    }

                    var installModDir = Path.Combine(modDirectory, "Installed by KKManager");
                    Directory.CreateDirectory(installModDir);
                    newMod.Location.CopyTo(Path.Combine(installModDir, newMod.Location.Name));
                    return;
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                    throw new InvalidOperationException("Failed to install zipmod file - " + e.Message, e);
                }
            }

            throw new InvalidOperationException("Failed to install zipmod file for an unknown reason");
        }
    }
}