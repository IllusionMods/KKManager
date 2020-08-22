using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using MessagePack;

namespace KKManager.Data.Cards.EC
{
    [ReadOnly(true)]
    public class EmoCard : Card
    {
        public override string Name => Parameter == null ? base.Name : Parameter.fullname;
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetPersonalityName(Parameter?.personality ?? -1);

        public int Language { get; private set; }
        public string UserID { get; private set; }
        public string DataID { get; private set; }

        public ChaFileParameter Parameter { get; }

        private EmoCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, ChaFileParameter parameter) : base(cardFile, type, extended)
        {
            Parameter = parameter;
        }

        public static EmoCard ParseEmoChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            if (0 > new Version("0.0.0").CompareTo(loadVersion))
            {
                //return null;
            }


            var language = 0;
            if (loadVersion > new Version("0.0.0"))
            {
                language = reader.ReadInt32();
            }
            var userID = reader.ReadString();
            var dataID = reader.ReadString();
            int num = reader.ReadInt32();
            var hsPackage = new HashSet<int>();
            for (int i = 0; i < num; i++)
            {
                hsPackage.Add(reader.ReadInt32());
            }


            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);
            var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes);
            var num2 = reader.ReadInt64();
            var position = reader.BaseStream.Position;

            ChaFileParameter parameter = null;
            var info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
            if (info != null)
            {
                reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);

                parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(parameterBytes);
                parameter.ComplementWithVersion();
            }

            Dictionary<string, PluginData> extData = null;
            info = blockHeader.SearchInfo(ChaFileExtended.BlockName);
            if (info != null)
            {
                reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);

                extData = MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(parameterBytes);
            }

            var card = new EmoCard(file, gameType, extData, parameter);
            card.Language = language;
            card.DataID = dataID;
            card.UserID = userID;
            return card;
        }

        public override Image GetCardFaceImage()
        {
            return null;
        }

        public static string GetPersonalityName(int personality)
        {
            string[] personalityLookup =
            {
                "Cheerful"   ,
                "Lively"     ,
                "Hesitant"   ,
                "Gentle"     ,
                "Deliquent"  ,
                "Quiet"      ,
                "Ladylike"   ,
                "Bewitching" ,
                "Diligent"   ,
            };

            if (personality < 0) return "Invalid";

            if (personalityLookup.Length > personality) return personalityLookup[personality];

            return "Unknown";
        }
    }
}