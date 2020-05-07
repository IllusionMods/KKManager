using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MessagePack;

namespace KKManager.Data.Cards.AI
{
    public class AiCard : Card
    {
        public override string Name => Parameter == null ? base.Name : $"{Parameter.fullname}";
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetPersonalityName(Parameter?.personality ?? -1);

        public ChaFileParameter Parameter { get; }

        public override Image GetCardFaceImage()
        {
            return null;
        }

        public static AiCard ParseAiChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            if (0 > new Version("0.0.0").CompareTo(loadVersion))
            {
                //return null;
            }
            
            var language = reader.ReadInt32();
            var userID = reader.ReadString();
            var dataID = reader.ReadString();
            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);
            var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes);
            var num = reader.ReadInt64();
            var position = reader.BaseStream.Position;

            ChaFileParameter parameter = null;
            var info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
            if (info != null)
            {
                var value = new Version(info.version);
                reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);

                parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(parameterBytes);
                parameter.version = value;
            }

            Dictionary<string, PluginData> extData = null;
            info = blockHeader.SearchInfo(ChaFileExtended.BlockName);
            if (info != null)
            {
                reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);

                extData = MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(parameterBytes);
            }

            var card = new AiCard(file, gameType, extData, parameter);

            return card;
        }

        public string GetPersonalityName(int personality)
        {
            if (Sex == CharaSex.Male) return "Male";

            string[] personalityLookup =
            {
                "Emotionless and stoic",
                "Friendly and gentle",
                "Confident and aware",
                "Selfish and spoiled",
                "Lazy and sluggish",
                "Positive and cheerful"
            };

            if (personality < 0 || personality > 90) return "Invalid";

            if (personalityLookup.Length > personality)
                return personalityLookup[personality];

            return "Unknown";
        }

        public AiCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, ChaFileParameter parameter) : base(cardFile, type, extended)
        {
            Parameter = parameter;
        }
    }
}