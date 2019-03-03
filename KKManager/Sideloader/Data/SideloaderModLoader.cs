using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Ionic.Zip;

namespace KKManager.Sideloader.Data
{
    public static class SideloaderModLoader
    {
        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="modDirectory">Directory containing the zipmods to gather info from. Usually mods directory inside game root.</param>
        /// <param name="searchOption">Where to search</param>
        public static List<SideloaderModInfo> TryReadSideloaderMods(string modDirectory, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var results = new List<SideloaderModInfo>();

            if (Directory.Exists(modDirectory))
            {
                foreach (var file in Directory.GetFiles(modDirectory, "*.zip", searchOption))
                {
                    try
                    {
                        results.Add(LoadFromFile(file));
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine(ex);
                        MessageBox.Show($"Failed to load mod from \"{file}\" with error: {ex.Message}",
                            "Load sideloader mods", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            return results;
        }

        public static SideloaderModInfo LoadFromFile(string filename)
        {
            var m = new SideloaderModInfo();
            m.Location = new FileInfo(filename);

            using (var zf = ZipFile.Read(m.Location.FullName))
            {
                var manifestEntry = zf.Entries.FirstOrDefault(x =>
                    x.FileName.Equals("manifest.xml", StringComparison.OrdinalIgnoreCase));

                if (manifestEntry == null)
                    throw new FileNotFoundException("manifest.xml was not found in the mod archive. Make sure this is a sideloader mod.", "manifest.xml");

                using (var fileStream = manifestEntry.OpenReader())
                {
                    var manifest = XDocument.Load(fileStream, LoadOptions.None);

                    if (manifest.Root == null || !manifest.Root.HasElements)
                        throw new InvalidDataException("The manifest.xml file is in an invalid format");

                    m.Guid = manifest.Root.Element("guid")?.Value;
                    m.Version = manifest.Root.Element("version")?.Value;
                    m.Name = manifest.Root.Element("name")?.Value ?? m.Location.Name;
                    m.Author = manifest.Root.Element("author")?.Value;
                    m.Description = manifest.Root.Element("description")?.Value;
                    m.Website = manifest.Root.Element("website")?.Value;
                }

                var images = new List<Image>();
                // TODO load from drive instead of caching to ram
                foreach (var imageFile in zf.Entries
                    .Where(x => ".jpg".Equals(Path.GetExtension(x.FileName), StringComparison.OrdinalIgnoreCase) ||
                                ".png".Equals(Path.GetExtension(x.FileName), StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.FileName).Take(3))
                {
                    try
                    {
                        using (var stream = imageFile.OpenReader())
                        using (var img = Image.FromStream(stream))
                        {
                            images.Add(img.GetThumbnailImage(200, 200, null, IntPtr.Zero));
                        }
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine($"Failed to load image \"{imageFile.FileName}\" from mod archive \"{zf.Name}\" with error: {ex.Message}");
                    }
                }
                m.Images = images;

                m.Contents = zf.EntryFileNames.Select(x => x.Replace('/', '\\')).ToList();
            }

            return m;
        }
    }
}