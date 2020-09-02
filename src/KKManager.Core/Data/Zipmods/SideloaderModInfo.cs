using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace KKManager.Data.Zipmods
{
    public class SideloaderModInfo : ModInfoBase
    {
        public SideloaderModInfo(FileInfo location, string guid, string name, string version,
            string author, string description, string website, IReadOnlyList<Image> images, IReadOnlyList<string> contents)
            : base(location, guid, name, version,author,description,website)
        {
            var extension = location.Extension;
            if (!SideloaderModLoader.IsValidZipmodExtension(extension))
                throw new InvalidOperationException("Zipmod has invalid extension: " + Location.Extension);

            Images = images;
            Contents = contents;
        }

        ~SideloaderModInfo()
        {
            // todo handle properly or move images out of here
            if (Images != null)
            {
                foreach (var image in Images)
                    image.Dispose();
            }
        }

        [Browsable(false)]
        public IReadOnlyList<Image> Images { get; }

        [Browsable(false)]
        public IReadOnlyList<string> Contents { get; }

        public Uri TryGetWebsiteUri()
        {
            return Uri.IsWellFormedUriString(Website, UriKind.RelativeOrAbsolute) ? new Uri(Website) : null;
        }

        public override bool Enabled => Location.Extension.StartsWith(".zip", StringComparison.OrdinalIgnoreCase);

        public override void SetEnabled(bool value)
        {
            if (Enabled != value)
            {
                var newPath = EnabledLocation(Location, value).FullName;
                if (!string.Equals(newPath, Location.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(newPath);
                    Location.MoveTo(newPath);
                }
            }
        }

        public static FileInfo EnabledLocation(FileInfo location, bool enable = true)
        {
            var ext = location.Extension.ToCharArray();
            ext[3] = enable ? 'p' : '_';
            return new FileInfo(location.FullName.Substring(0, location.FullName.Length - ext.Length) + new string(ext));
        }
    }
}
