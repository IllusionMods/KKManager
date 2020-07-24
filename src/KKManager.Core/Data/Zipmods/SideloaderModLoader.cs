using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using KKManager.Util;

namespace KKManager.Data.Zipmods
{
    public static class SideloaderModLoader
    {
        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="modDirectory">Directory containing the zipmods to gather info from. Usually mods directory inside game root.</param>
        /// <param name="cancellationToken">Token used to abort the search</param>
        /// <param name="searchOption">Where to search</param>
        public static IObservable<SideloaderModInfo> TryReadSideloaderMods(string modDirectory, CancellationToken cancellationToken, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var subject = new ReplaySubject<SideloaderModInfo>();

            if (!Directory.Exists(modDirectory) || cancellationToken.IsCancellationRequested)
            {
                subject.OnCompleted();
                return subject;
            }

            void ReadSideloaderModsAsync()
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(modDirectory, "*.*", searchOption))
                    {
                        try
                        {
                            if (!IsValidZipmodExtension(Path.GetExtension(file))) continue;

                            subject.OnNext(LoadFromFile(file));
                        }
                        catch (SystemException ex)
                        {
                            Console.WriteLine(ex);
                            /*MessageBox.Show(
                                        $"Failed to load mod from \"{file}\" with error: {ex.Message}",
                                        "Load sideloader mods", MessageBoxButtons.OK, MessageBoxIcon.Warning);*/
                        }

                        if (cancellationToken.IsCancellationRequested) break;
                    }

                    subject.OnCompleted();
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    subject.OnError(ex);
                }
            }

            Task.Run((Action)ReadSideloaderModsAsync, cancellationToken);

            return subject;
        }

        public static SideloaderModInfo LoadFromFile(string filename)
        {
            var location = new FileInfo(filename);

            if (!IsValidZipmodExtension(location.Extension))
                throw new ArgumentException($"The file {filename} has an invalid extension and can't be a zipmod", nameof(filename));

            using (var zf = SharpCompress.Archives.ArchiveFactory.Open(location))
            {
                var manifestEntry = zf.Entries.FirstOrDefault(x => PathTools.PathsEqual(x.Key, "manifest.xml"));

                if (manifestEntry == null)
                {
                    if (zf.Entries.Any(x => x.IsDirectory && PathTools.PathsEqual(x.Key, "abdata")))
                        throw new NotSupportedException("The file is a hardmod and cannot be installed automatically. It's recommeded to look for a sideloader version.");

                    throw new InvalidDataException("manifest.xml was not found in the mod archive. Make sure this is a zipmod.");
                }

                using (var fileStream = manifestEntry.OpenEntryStream())
                {
                    var manifest = XDocument.Load(fileStream, LoadOptions.None);

                    if (manifest.Root?.Element("guid")?.IsEmpty != false)
                        throw new InvalidDataException("The manifest.xml file is in an invalid format");

                    var guid = manifest.Root.Element("guid")?.Value;
                    var version = manifest.Root.Element("version")?.Value;
                    var name = manifest.Root.Element("name")?.Value ?? location.Name;
                    var author = manifest.Root.Element("author")?.Value;
                    var description = manifest.Root.Element("description")?.Value;
                    var website = manifest.Root.Element("website")?.Value;

                    var images = new List<Image>();
                    // TODO load from drive instead of caching to ram
                    foreach (var imageFile in zf.Entries
                        .Where(x => ".jpg".Equals(Path.GetExtension(x.Key), StringComparison.OrdinalIgnoreCase) ||
                                    ".png".Equals(Path.GetExtension(x.Key), StringComparison.OrdinalIgnoreCase))
                        .OrderBy(x => x.Key).Take(3))
                    {
                        try
                        {
                            using (var stream = imageFile.OpenEntryStream())
                            using (var img = Image.FromStream(stream))
                            {
                                images.Add(img.GetThumbnailImage(200, 200, null, IntPtr.Zero));
                            }
                        }
                        catch (SystemException ex)
                        {
                            Console.WriteLine($"Failed to load image \"{imageFile.Key}\" from mod archive \"{location.Name}\" with error: {ex.Message}");
                        }
                    }

                    var contents = zf.Entries.Where(x => !x.IsDirectory).Select(x => x.Key.Replace('/', '\\')).ToList();

                    return new SideloaderModInfo(location, guid, name, version, author, description, website, images, contents);
                }
            }
        }

        public static bool IsValidZipmodExtension(string extension)
        {
            var exts = new[]
            {
                ".zip",
                ".zi_",
                ".zipmod",
                ".zi_mod",
            };

            return exts.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
    }
}