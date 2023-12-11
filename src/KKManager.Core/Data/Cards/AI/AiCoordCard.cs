using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using MessagePack;

namespace KKManager.Data.Cards.KK
{
    /// <summary>
    /// Supports all versions of Koikatu/Koikatsu Party and Koikatsu Sunshine
    /// </summary>
    public sealed class AiCoordCard : Card
    {
        public static Card ParseAiClothes(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            var language = reader.ReadInt32();
            var coordinateName = reader.ReadString();
            var num = reader.ReadInt32();
            //load clothes and accs from the bytes array
            var _ = reader.ReadBytes(num);

            var extended = TryReadExtData(reader);

            return new AiCoordCard(file, gameType, extended, loadVersion, coordinateName, language);
        }
        private static Dictionary<string, PluginData> TryReadExtData(BinaryReader br)
        {
            try
            {
                var marker = br.ReadString();
                var version = br.ReadInt32();

                var length = br.ReadInt32();

                if (marker == "KKEx" /*&& version == DataVersion*/ && length > 0)
                {
                    var bytes = br.ReadBytes(length);
                    return MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(bytes);
                }
            }
            catch
            {
                // Incomplete/non-existant data
            }

            return null;
        }

        public AiCoordCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, Version version, string coordinateName, int language) : base(cardFile, type, extended, version)
        {
            Name = coordinateName;
            Language = language;
        }
        [Browsable(false)] public override CharaSex Sex => CharaSex.Unknown;
        [Browsable(false)] public override string PersonalityName => null;
        public override string Name { get; }
        public override Image GetCardFaceImage() => null;
    }
}
