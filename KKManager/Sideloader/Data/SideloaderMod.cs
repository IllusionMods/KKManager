using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace KKManager.Sideloader.Data
{
    public class SideloaderMod
    {
        internal SideloaderMod() { }

        public FileInfo Location { get; internal set; }
        public string Name { get; internal set; }
        public string Version { get; internal set; }
        public string Author { get; internal set; }
        public string Description { get; internal set; }
        public string Website { get; internal set; }
        public string Guid { get; internal set; }

        public List<Image> Images { get; } = new List<Image>();

        public Uri TryGetWebsiteUri()
        {
            return Uri.IsWellFormedUriString(Website, UriKind.RelativeOrAbsolute) ? new Uri(Website) : null;
        }
    }
}
