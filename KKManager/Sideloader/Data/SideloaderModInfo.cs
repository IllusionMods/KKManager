using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace KKManager.Sideloader.Data
{
    public class SideloaderModInfo
    {
        internal SideloaderModInfo() { }

        public FileInfo Location { get; internal set; }
        public string Name { get; internal set; }
        public string Version { get; internal set; }
        public string Author { get; internal set; }
        public string Description { get; internal set; }
        public string Website { get; internal set; }
        public string Guid { get; internal set; }

        [Browsable(false)]
        public string FileName => Location.Name;

        [Browsable(false)]
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

        [Browsable(false)]
        public IReadOnlyList<Image> Images { get; internal set; }

        [Browsable(false)]
        public IReadOnlyList<string> Contents { get; internal set; }

        public Uri TryGetWebsiteUri()
        {
            return Uri.IsWellFormedUriString(Website, UriKind.RelativeOrAbsolute) ? new Uri(Website) : null;
        }

        public bool Enabled => Location.Extension.StartsWith(".zip", StringComparison.OrdinalIgnoreCase);

        public void SetEnabled(bool value)
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
