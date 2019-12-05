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
            : base(location, guid, name, version)
        {
            Author = author;
            Description = description;
            Website = website;
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

        public string Author { get; }
        public string Description { get; }
        public string Website { get; }

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
            if (Location.Extension.StartsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (!value)
                {
                    var newExt = Location.Extension.ToCharArray();
                    newExt[3] = '_';

                    var newName = Location.FullName.Substring(0, Location.FullName.Length - newExt.Length) + new string(newExt);
                    Location.MoveTo(newName);
                }
            }
            else if (Location.Extension.StartsWith(".zi_", StringComparison.OrdinalIgnoreCase))
            {
                if (value)
                {
                    var newExt = Location.Extension.ToCharArray();
                    newExt[3] = 'p';

                    var newName = Location.FullName.Substring(0, Location.FullName.Length - newExt.Length) + new string(newExt);
                    Location.MoveTo(newName);
                }
            }
            else
            {
                throw new InvalidOperationException("Zipmod has invalid extension: " + Location.Extension);
            }
        }
    }
}
