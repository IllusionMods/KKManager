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
    public class KoiCoordCard : Card
    {
        public static Card ParseKoiClothes(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            //if (0 > ChaFileDefine.ChaFileClothesVersion.CompareTo(this.loadVersion))
            //{
            //    this.lastLoadErrorCode = -2;
            //    flag = false;
            //}
            //else
            {
                var coordinateName = reader.ReadString();
                int num = reader.ReadInt32();
                //load clothes and accs from the bytes array
                byte[] array = reader.ReadBytes(num);

                var extended = TryReadExtData(reader);
                
                return new KoiCoordCard(file, gameType, extended, loadVersion, coordinateName);
            }
        }
        private static Dictionary<string, PluginData> TryReadExtData(BinaryReader br)
        {
            try
            {
                string marker = br.ReadString();
                int version = br.ReadInt32();

                int length = br.ReadInt32();

                if (marker == "KKEx" /*&& version == DataVersion*/ && length > 0)
                {
                    byte[] bytes = br.ReadBytes(length);
                    return MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(bytes);
                }
            }
            catch
            {
                // Incomplete/non-existant data
            }

            return null;
        }

        public KoiCoordCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, Version version, string coordinateName) : base(cardFile, type, extended, version)
        {
            Name = coordinateName;
        }
        [Browsable(false)] public override CharaSex Sex => CharaSex.Unknown;
        [Browsable(false)] public override string PersonalityName => null;
        public override string Name { get; }
        public override Image GetCardFaceImage() => null;
    }
}
