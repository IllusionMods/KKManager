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
        public IReadOnlyList<Image> Images { get; internal set; }

        [Browsable(false)]
        public IReadOnlyList<string> Contents { get; internal set; }

        public Uri TryGetWebsiteUri()
        {
            return Uri.IsWellFormedUriString(Website, UriKind.RelativeOrAbsolute) ? new Uri(Website) : null;
        }
    }
}
