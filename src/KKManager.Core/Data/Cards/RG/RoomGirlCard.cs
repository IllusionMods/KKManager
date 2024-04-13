using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards.RG
{
    public class RoomGirlCard : Card
    {
        public override string Name => Parameter?.fullname ?? base.Name;
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetProfession(Parameter?.personality ?? -1);
        public string Birthday => $"{GetBirthMonth(Parameter.birthMonth)} {Parameter.birthDay}";
        public string Fetishes => String.Join(", ", GetFetishes(Parameter.propensity));
        public string Traits => String.Join(", ", GetTraits(Parameter.features));
        [ReadOnly(true)] public ChaFileParameter Parameter { get; }

        private RoomGirlCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, FileSize extendedSize, ChaFileParameter parameter, Version version) : base(cardFile, type, extended, extendedSize, version)
        {
            Parameter = parameter;
        }

        public static RoomGirlCard ParseRGChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            // check version number from the "【RG_Chara】1.0.1" string in the middle of the file
            if (0 > new Version("1.0.1").CompareTo(loadVersion))
            {
                //return null;
            }

            var language = reader.ReadInt32();

            // two GUIDs, one is a meme, the other is unique to the card
            string userID = reader.ReadString(); // this is always 'illusion-2022-0825-xxxx-roomgirlocha'
            string dataID = reader.ReadString();

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

            reader.ReadInt64();
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
                    parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(parameterBytes, deserializeOptions);
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

            var card = new RoomGirlCard(file, gameType, extData, extendedSize, parameter, loadVersion)
            {
                Language = language,
                UserID = userID,
                DataID = dataID,
            };

            return card;
        }

        public List<string> GetTraits(List<byte> features)
        {
            var traits = new List<string>();

            string[] traitLookup =
            {
                "Positive",
                "Chatty",
                "Smooth Talker",
                "Gambler",
                "Energetic",
                "Playful",
                "Cautious",
                "Weak Bladder",
                "Quality Sleep",
                "Likes People",
                "Genius",
                "Multi-Hobby",
                "Friendly",
                "Romantic",
                "Pheromone",
                "Friendliness"
            };

            foreach (var attribute in features)
            {
                if (attribute > 15) traits.Add("Invalid");
                else if (traitLookup.Length > attribute) traits.Add(traitLookup[attribute]);
                else traits.Add(KKManager.Properties.Resources.Unknown);
            }
            return traits;
        }

        public List<string> GetFetishes(List<byte> propensity)
        {
            var fetishes = new List<string>();
        
            string[] fetishLookup =
            {
                "Kissing",
                "Submissive",
                "Tittyfucking",
                "Cunnilingus",
                "Anal Sex",
                "Nymphomaniac",
                "Sensitive",
                "Sex-positive",
                "Breeding",
                "Bukkake"
            };

            foreach (var kink in propensity)
            {
                if (kink > 12) fetishes.Add("Invalid");
                else if (fetishLookup.Length > kink) fetishes.Add(fetishLookup[kink]);
                else fetishes.Add(KKManager.Properties.Resources.Unknown);
            }
            return fetishes;
        }

        // personality is actually profession lol
        public static string GetProfession(int profession)
        {
            string[] professionLookup =
            {
                "Office Lady Type A",
                "Office Lady Type B",
                "Nurse Type A",
                "Nurse Type B",
                "Student Type A",
                "Student Type B",
                "Idol Type A",
                "Idol Type B",
                "Casino Dealer Type A",
                "Casino Dealer Type B",
                "NEET Type A",
                "NEET Type B"
            };

            if (profession < 0 || profession > 12) return "Invalid";
            else if (professionLookup.Length > profession) return professionLookup[profession];
            else return KKManager.Properties.Resources.Unknown;
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