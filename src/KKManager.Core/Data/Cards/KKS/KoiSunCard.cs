using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards.KKS
{
    public class KoiSunCard : Card
    {
        public override string Name => Parameter == null ? base.Name : $"{Parameter.lastname} {Parameter.firstname}";
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetPersonalityName(Parameter?.personality ?? -1);

        [ReadOnly(true)] public ChaFileParameter Parameter { get; }
        [ReadOnly(true), Browsable(false)] public ChaFileAbout About { get; }

        public override int Language => About.language;
        public override string UserID => About.userID;
        public override string DataID => About.dataID;

        private KoiSunCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, FileSize extendedSize, ChaFileParameter parameter, ChaFileAbout about, Version version) :
            base(cardFile, type, extended, extendedSize, about?.version ?? version)
        {
            Parameter = parameter;
            About = about ?? new ChaFileAbout();
        }

        public static KoiSunCard ParseKoiChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            if (0 > new Version("0.0.0").CompareTo(loadVersion))
            {
                //return null;
            }

            var faceLength = reader.ReadInt32();
            if (faceLength > 0)
            {
                //this.facePngData = reader.ReadBytes(num);
                reader.BaseStream.Seek(faceLength, SeekOrigin.Current);
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
                var value = new Version(info.version);
                if (0 <= ChaFileParameter.CurrentVersion.CompareTo(value))
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                    var parameterBytes = reader.ReadBytes((int)info.size);

                    parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(parameterBytes);
                    parameter.ComplementWithVersion();
                }
            }

            ChaFileAbout about = null;
            info = blockHeader.SearchInfo(ChaFileAbout.BlockName);
            if (info != null)
            {
                var value = new Version(info.version);
                if (0 <= ChaFileAbout.CurrentVersion.CompareTo(value))
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                    var parameterBytes = reader.ReadBytes((int)info.size);

                    about = MessagePackSerializer.Deserialize<ChaFileAbout>(parameterBytes);
                    about.ComplementWithVersion();
                }
            }

            Dictionary<string, PluginData> extData = null;
            info = blockHeader.SearchInfo(ChaFileExtended.BlockName);
            if (info != null)
            {
                reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);

                extData = MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(parameterBytes);
            }
            var extendedSize = info != null ? Util.FileSize.FromBytes((int)info.size) : Util.FileSize.Empty;

            var card = new KoiSunCard(file, gameType, extData, extendedSize, parameter, about, loadVersion);

            return card;
        }

        public static string GetPersonalityName(int personality)
        {
            string[] personalityLookup =
            {
                "Sexy",
                "Ojousama",
                "Snobby",
                "Kouhai",
                "Mysterious",
                "Weirdo",
                "Yamato Nadeshiko",
                "Tomboy",
                "Pure",
                "Simple",
                "Delusional",
                "Motherly",
                "Big Sisterly",
                "Gyaru",
                "Delinquent",
                "Wild",
                "Wannabe",
                "Reluctant",
                "Jinxed",
                "Bookish",
                "Timid",
                "Typical Schoolgirl",
                "Trendy",
                "Otaku",
                "Yandere",
                "Lazy",
                "Quiet",
                "Stubborn",
                "Old-Fashioned",
                "Humble",
                "Friendly",
                "Willful",
                "Honest",
                "Glamorous",
                "Returnee",
                "Slangy",
                "Sadistic",
                "Emotionless",
                "Perfectionist",
                "Island Girl"
            };

            if (personality < 0 || personality > 90) return "Invalid";

            if (personalityLookup.Length > personality) return personalityLookup[personality];

            if (personality >= 80 && personality <= 86) return "Story-only " + personality;

            return KKManager.Properties.Resources.Unknown;
        }
    }
}