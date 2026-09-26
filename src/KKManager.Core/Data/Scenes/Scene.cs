using System;
using System.ComponentModel;
using System.IO;
using KKManager.Util;

namespace KKManager.Data.Scenes
{
    public sealed class Scene : IFileInfoBase
    {
        public Scene(FileInfo location, string[] usedZipmods)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            UsedZipmods = usedZipmods ?? Array.Empty<string>();
            FileSize = FileSize.FromBytes(location.Length);
        }

        public string Name => Location.GetNameWithoutExtension();
        public FileInfo Location { get; }
        public FileSize FileSize { get; }
        [ReadOnly(true), TypeConverter(typeof(ReadOnlyStringCollectionConverterWithPreview))]
        public string[] UsedZipmods { get; }
        [ReadOnly(true), TypeConverter(typeof(ReadOnlyStringCollectionConverterWithPreview))]
        public string[] MissingZipmods { get; set; }
    }
}
