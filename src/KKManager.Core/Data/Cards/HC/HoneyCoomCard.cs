using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MessagePack;
using System.Linq;

namespace KKManager.Data.Cards.HC
{
    public class HoneyCoomCard : Card
    {
        public override string Name => Parameter?.fullname ?? base.Name;
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetProfession(Parameter?.personality ?? -1);
        public string Birthday => $"{GetBirthMonth(Parameter.birthMonth)} {Parameter.birthDay}";
        //public string Fetishes => String.Join(", ", GetFetishes(Parameter.propensity));
        //public string Traits => String.Join(", ", GetTraits(Parameter.features));
        [ReadOnly(true)] public HumanDataParameter Parameter { get; }
        [ReadOnly(true)] public HumanDataGameInfo_HC GameInfo { get; }
        [ReadOnly(true)] public HumanDataGameParameter_HC GameParameter { get; }

        private HoneyCoomCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, HumanDataParameter parameter, Version version, HumanDataGameInfo_HC gameInfo, HumanDataGameParameter_HC gameParameter) : base(cardFile, type, extended, version)
        {
            Parameter = parameter;
            GameInfo = gameInfo;
            GameParameter = gameParameter;
        }

        public static HoneyCoomCard ParseHCPChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            // check version number from the "【RG_Chara】1.0.1" string in the middle of the file
            if (0 > new Version("1.0.1").CompareTo(loadVersion))
            {
                //return null;
            }

            //var language = reader.ReadInt32();

            // two GUIDs, one is a meme, the other is unique to the card
            //string userID = reader.ReadString(); // this is always 'illusion-2022-0825-xxxx-roomgirlocha'
            //string dataID = reader.ReadString();

            // the next int32 contains a byte-offset value from the beginning of the file
            // to the end of the 2nd PNG file, which is where the juicy metadata is
            var faceLength = reader.ReadInt32();
            if (faceLength > 0)
            {
                //this.facePngData = reader.ReadBytes(num);
                // fast forward to the end of the 2nd PNG file
                reader.BaseStream.Seek(faceLength, SeekOrigin.Current);
            }

            // ingest the next chunk of data as raw bytes
            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);

            var deserializeOptions = MessagePackSerializerOptions.Standard.WithSecurity(MessagePackSecurity.UntrustedData);

            // deserialize messagepack json value array defined in RG/BlockHeader.cs
            var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes, deserializeOptions);
            var sds = blockHeader.lstInfo.ToDictionary(x => x.name, x => x.version);
            reader.ReadInt64();
            var position = reader.BaseStream.Position;

            HumanDataParameter parameter = null;
            var info = blockHeader.SearchInfo(HumanDataParameter.BlockName);
            if (info != null)
            {
                var value = new Version(info.version);
                if (0 <= HumanDataParameter.CurrentVersion.CompareTo(value))
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                    var parameterBytes = reader.ReadBytes((int)info.size);
                    parameter = MessagePackSerializer.Deserialize<HumanDataParameter>(parameterBytes, deserializeOptions);
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

            HumanDataAbout about = new HumanDataAbout();
            info = blockHeader.SearchInfo(HumanDataAbout.BlockName);
            if (info != null)
            {
                var value = new Version(info.version);
                if (0 <= HumanDataAbout.CurrentVersion.CompareTo(value))
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                    var parameterBytes = reader.ReadBytes((int)info.size);

                    about = MessagePackSerializer.Deserialize<HumanDataAbout>(parameterBytes);
                }
            }

            HumanDataGameInfo_HC gameInfo = null;
            info = HumanDataGameInfo_HC.BlockName.Select(x => blockHeader.SearchInfo(x)).FirstOrDefault(x => x != null);
            if (info != null)
            {
                var value = new Version(info.version);
                if (0 <= HumanDataGameInfo_HC.CurrentVersion.CompareTo(value))
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                    var parameterBytes = reader.ReadBytes((int)info.size);

                    gameInfo = MessagePackSerializer.Deserialize<HumanDataGameInfo_HC>(parameterBytes);
                }
            }

            HumanDataGameParameter_HC gameParameter = null;
            info = HumanDataGameParameter_HC.BlockName.Select(x => blockHeader.SearchInfo(x)).FirstOrDefault(x => x != null);
            if (info != null)
            {
                var value = new Version(info.version);
                if (0 <= HumanDataGameParameter_HC.CurrentVersion.CompareTo(value))
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                    var parameterBytes = reader.ReadBytes((int)info.size);

                    gameParameter = MessagePackSerializer.Deserialize<HumanDataGameParameter_HC>(parameterBytes);
                }
            }

            var card = new HoneyCoomCard(file, gameType, extData, parameter, loadVersion, gameInfo, gameParameter)
            {
                Language = about.language,
                UserID = about.userID,
                DataID = about.dataID,
            };

            return card;
        }
        
        public static string GetProfession(int profession)
        {
            return GetValue(profession, new[]
            {
                "Cheerful",
                "Cool",
                "Mother Figure",
                "Scaredy Cat",
                "Boyinsh",
                "Innocence"
            });
        }
        private static string GetValue(int profession, string[] values)
        {
            if (profession < 0) return "Invalid";
            if (profession >= values.Length) return Properties.Resources.Unknown;
            return values[profession];
        }

        public static string GetBirthMonth(int birthMonth)
        {
            string[] birthMonthLookup =
            {
                "Invalid",
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            };

            if (birthMonth < 1 || birthMonth > 12) return "Invalid";
            else if (birthMonthLookup.Length > birthMonth) return birthMonthLookup[birthMonth];
            else return KKManager.Properties.Resources.Unknown;
        }
    }
}