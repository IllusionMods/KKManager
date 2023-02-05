using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KKManager.Data.Zipmods;

namespace KKManager.Windows
{
    public static class ZipmodTools
    {
        private static bool _simulate;

        public static void RemoveDuplicateZipmodsInDir(DirectoryInfo modsPath, bool simulate)
        {
            _simulate = simulate;

            try
            {
                Console.WriteLine($"Removing duplicate zipmods in directory: " + modsPath.FullName);

                var ld = modsPath.FullName;
                if (!Directory.Exists(ld))
                {
                    Console.WriteLine($"Directory doesn't exist!");
                    return;
                }

                var allMods = (from file in Directory.GetFiles(ld, "*", SearchOption.AllDirectories)
                    where file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                          || file.EndsWith(".zi_", StringComparison.OrdinalIgnoreCase)
                          || FileHasZipmodExtension(file)
                    select file).ToList();

                SideloaderCleanupByManifest(allMods);
                SideloaderCleanupByFilename(allMods.Where(File.Exists));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to remove duplicate zipmods: " + e.ToStringDemystified());
            }
            finally
            {
                Console.WriteLine($"Finished removing duplicate zipmods");
            }
        }

        private static void SideloaderCleanupByManifest(IEnumerable<string> allMods)
        {
            try
            {
                var mods = new List<SideloaderModInfo>();

                foreach (var mod in allMods)
                {
                    try
                    {
                        mods.Add(SideloaderModLoader.LoadFromFile(mod));
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine($"Deleting zipmod file: {mod} | Reason: {ex.Message}");
                        // Kill it with fire
                        SafeFileDelete(mod);
                    }
                }

                foreach (var modGroup in mods.GroupBy(x => x.Guid))
                {
                    //todo don't prioritize mods inside the modpacks?
                    var orderedMods = modGroup.All(x => !string.IsNullOrWhiteSpace(x.Version))
                        ? modGroup.OrderByDescending(x => x.Location.FullName.ToLower().Contains("mods\\sideloader modpack")).ThenByDescending(x => x.Version, new SideloaderVersionComparer())
                        : modGroup.OrderByDescending(x => x.Location.FullName.ToLower().Contains("mods\\sideloader modpack")).ThenByDescending(x => x.Location.LastWriteTimeUtc);

                    // Prefer .zipmod extension and then longer paths (so the mod has either longer name or is arranged in a subdirectory)
                    orderedMods = orderedMods.ThenByDescending(x => FileHasZipmodExtension(x.FileName)).ThenByDescending(x => x.Location.FullName.Length);

                    var orederedModsList = orderedMods.ToList();
                    foreach (var oldMod in orederedModsList.Skip(1))
                    {
                        Console.WriteLine($"Deleting zipmod file: {oldMod.Location.FullName} | Reason: Duplicate GUID, older than {orederedModsList.First().Location.FullName}");
                        SafeFileDelete(oldMod.Location.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SideloaderCleanupByFilename(IEnumerable<string> allMods)
        {
            var modDuplicates = allMods.GroupBy(Path.GetFileNameWithoutExtension);

            foreach (var modVersions in modDuplicates)
            {
                if (modVersions.Count() <= 1) continue;

                // Figure out the newest mod and remove all others. Favor .zipmod versions if both have the same creation date
                var orderedVersions = modVersions.OrderByDescending(File.GetLastWriteTime)
                    .ThenByDescending(FileHasZipmodExtension)
                    // Prefer non-disabled mods
                    .ThenByDescending(x => !Path.GetExtension(x).Contains("_"));
                var orederedModsList = orderedVersions.ToList();
                foreach (var oldModPath in orederedModsList.Skip(1))
                {
                    Console.WriteLine($"Deleting zipmod file: {oldModPath} | Reason: Same filename, but later last write date than {orederedModsList.First()}");
                    SafeFileDelete(oldModPath);
                }
            }
        }

        private static bool FileHasZipmodExtension(string fileName)
        {
            return fileName.EndsWith(".zipmod", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".zi_mod", StringComparison.OrdinalIgnoreCase);
        }

        private static void SafeFileDelete(string file)
        {
            try
            {
                if (!_simulate)
                    File.Delete(file);
            }
            catch (SystemException)
            {
                // Nom nom nom
            }
        }
    }
}