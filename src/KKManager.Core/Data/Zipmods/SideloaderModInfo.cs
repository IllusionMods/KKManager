using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Sideloader;

namespace KKManager.Data.Zipmods
{
    public class SideloaderModInfo : ModInfoBase
    {
        public SideloaderModInfo(FileInfo location, Manifest manifest, IReadOnlyList<Image> images, IReadOnlyList<string> contents)
            : base(location, manifest.GUID, manifest.Name, manifest.Version, manifest.Author, manifest.Description, manifest.Website)
        {
            var extension = location.Extension;
            if (!SideloaderModLoader.IsValidZipmodExtension(extension))
                throw new InvalidOperationException("Zipmod has invalid extension: " + Location.Extension);

            Manifest = manifest;
            Images = images;
            Contents = contents;
            ContentsKind = CheckContentsKinds(contents, manifest);
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
        public Manifest Manifest { get; }

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

        private static ZipmodContentsKind CheckContentsKinds(IReadOnlyList<string> contents, Manifest manifest)
        {
            var kind = ZipmodContentsKind.Unknown;

            if (manifest.ManifestDocumentRoot.Element("AnimationLoader") != null)
                kind |= ZipmodContentsKind.AnimationFreeH;

            if (manifest.ManifestDocumentRoot.Element("KK_UncensorSelector") != null)
                kind |= ZipmodContentsKind.UncensorSelector;

            if (manifest.ManifestDocumentRoot.Element("MaterialEditor") != null)
                kind |= ZipmodContentsKind.MaterialEditor;

            foreach (var filePath in contents)
            {
                if (filePath.StartsWith(@"abdata\map\", StringComparison.OrdinalIgnoreCase))
                    kind |= ZipmodContentsKind.MapFreeH;
                else if (filePath.StartsWith(@"abdata\h\anim\", StringComparison.OrdinalIgnoreCase))
                    kind |= ZipmodContentsKind.AnimationFreeH;
                else if (filePath.StartsWith(@"abdata\list\characustom\", StringComparison.OrdinalIgnoreCase))
                    kind |= ZipmodContentsKind.Character;
                else if (filePath.StartsWith(@"abdata\studio\info\", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = Path.GetFileName(filePath);
                    if (fileName.StartsWith("Map_", StringComparison.OrdinalIgnoreCase))
                        kind |= ZipmodContentsKind.MapStudio;
                    else if (Regex.IsMatch(fileName, @"^(H)?Anime(Category|Group)?_", RegexOptions.IgnoreCase))
                        kind |= ZipmodContentsKind.AnimationStudio;
                    else
                        kind |= ZipmodContentsKind.Studio;
                }
            }

            return kind;
        }

        [Flags]
        public enum ZipmodContentsKind
        { //todo draggable grid list, order matters, save as separate xml elems, each has output subfolder name, compressbydefault, place in author subfolders
            Unknown = 0,
            MapFreeH = 1 << 0,
            MapStudio = 1 << 1,
            AnimationFreeH = 1 << 2,
            AnimationStudio = 1 << 3,
            Studio = 1 << 4,
            Character = 1 << 5,
            UncensorSelector = 1 << 6,
            MaterialEditor = 1 << 7,
        }

        public ZipmodContentsKind ContentsKind { get; }
    }
}
