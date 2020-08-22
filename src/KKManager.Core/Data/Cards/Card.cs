using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using KKManager.Util;

namespace KKManager.Data.Cards
{
    public abstract class Card : IFileInfoBase
    {
        public virtual string Name => Path.GetFileNameWithoutExtension(Location.Name);
        [DisplayName("Filename")]
        public FileInfo Location { get; }
        public CardType Type { get; }

        public abstract CharaSex Sex { get; }
        public abstract string PersonalityName { get; }

        //[Browsable(false)]
        [DisplayName("Extended Data (plugins)")]
        [TypeConverter(typeof(DictionaryTypeConverter<string, PluginData>))]
        public Dictionary<string, PluginData> Extended { get; }

        public virtual Image GetCardImage()
        {
            using (var stream = Location.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return Image.FromStream(stream);
        }

        public virtual Image GetCardFaceImage()
        {
            using (var stream = Location.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Skip the first PNG and ignore it
                stream.Position = Utility.SearchForPngEnd(stream);
                // Find the second PNG and skip to it
                stream.Position = Utility.SearchForPngStart(stream);

                var imgSize = (int)(Utility.SearchForPngEnd(stream) - stream.Position);
                if (imgSize <= 0) return null;

                // This is necessary because Image.FromStream ignores stream.Position and always reads from start of stream
                using (var reader = new BinaryReader(stream))
                using (var str = new MemoryStream(reader.ReadBytes(imgSize)))
                {
                    return Image.FromStream(str);
                }
            }
        }

        internal Card(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended)
        {
            Location = cardFile ?? throw new ArgumentNullException(nameof(cardFile));
            Type = type;
            Extended = extended ?? new Dictionary<string, PluginData>();
        }
    }
}
