using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using KKManager.Util;
using MessagePack;
using KKManager.Data.Cards.AL.Data;

namespace KKManager.Data.Cards.AL
{
    public class AmanatsuCard : Card
    {
        [ReadOnly(true)] public HumanDataAbout About { get; }
        [ReadOnly(true)] public HumanDataCoordinate Coordinate { get; }
        [ReadOnly(true)] public HumanDataCustom Custom { get; }
        [ReadOnly(true)] public HumanDataGameInfo_AL GameInfoAl { get; }
        [ReadOnly(true)] public HumanDataGameParameter_AL GameParameterAl { get; }
        [ReadOnly(true)] public HumanDataGraphic Graphic { get; }
        [ReadOnly(true)] public HumanDataParameter HumanDataParameter { get; }
        [ReadOnly(true)] public HumanDataStatus Status { get; }
        [ReadOnly(true)] public HumanDataThumbParameter ThumbParameter { get; }

        public override string Name
        {
            get
            {
                var name = HumanDataParameter.firstname + " " + HumanDataParameter.lastname;
                name = name.Trim();
                return name.Length > 0 ? name : base.Name;
            }
        }

        public override CharaSex Sex => HumanDataParameter == null ? CharaSex.Unknown : HumanDataParameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetProfession(HumanDataParameter?.personality ?? -1, Sex);
        public string Birthday => $"{GetBirthMonth(HumanDataParameter.birthMonth)} {HumanDataParameter.birthDay}";

        private AmanatsuCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended,
            FileSize extendedSize, HumanDataAbout about, HumanDataCoordinate coordinate, HumanDataCustom version,
            HumanDataGameInfo_AL gameInfoAl, HumanDataGameParameter_AL gameParameterAl, HumanDataGraphic graphic,
            HumanDataParameter humanDataParameter, HumanDataStatus status, HumanDataThumbParameter thumbParameter,
            Version loadVersion) : base(cardFile, type, extended, extendedSize, loadVersion)
        {
            About = about;
            Coordinate = coordinate;
            Custom = version;
            GameInfoAl = gameInfoAl;
            GameParameterAl = gameParameterAl;
            Graphic = graphic;
            HumanDataParameter = humanDataParameter;
            Status = status;
            ThumbParameter = thumbParameter;
        }

        public static AmanatsuCard ParseAlChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            //if (0 > new Version("1.0.1").CompareTo(loadVersion))
            //    return null;

            //TODO Is this actually correct? The face is now serialized inside Parameter it seems
            // the next int32 contains a byte-offset value from the beginning of the file
            // to the end of the 2nd PNG file, which is where the juicy metadata is
            var faceLength = reader.ReadInt32();
            if (faceLength > 0)
            {
                //this.facePngData = reader.ReadBytes(num);
                // fast forward to the end of the 2nd PNG file
                reader.BaseStream.Seek(faceLength, SeekOrigin.Current);
            }

            // Get the BlockHeader
            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);
            var deserializeOptions = MessagePackSerializerOptions.Standard.WithSecurity(MessagePackSecurity.UntrustedData);
            var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes, deserializeOptions);

            // Some sort of ID, ignore
            _ = reader.ReadInt64();

            // Position used to calculate where the blocks are by adding offset from header infos
            var basePosition = reader.BaseStream.Position;

            //blockHeader.DumpBlocksToJson(reader, basePosition, new DirectoryInfo("E:\\block_dump"));

            // ---------- blocks ------------
            blockHeader.TryGetObject<HumanDataAbout>(reader, basePosition, HumanDataAbout.BlockName, out var about, out _);
            // TODO doesn't work blockHeader.TryGetObject<HumanDataCoordinate>(reader, basePosition, HumanDataCoordinate.BlockName, out var coordinate, out _);
            // TODO doesn't work blockHeader.TryGetObject<HumanDataCustom>(reader, basePosition, HumanDataCustom.BlockName, out var custom, out _);
            blockHeader.TryGetObject<HumanDataGameInfo_AL>(reader, basePosition, HumanDataGameInfo_AL.BlockName, out var gameInfoAl, out _);
            blockHeader.TryGetObject<HumanDataGameParameter_AL>(reader, basePosition, HumanDataGameParameter_AL.BlockName, out var parameterAl, out _);
            blockHeader.TryGetObject<HumanDataGraphic>(reader, basePosition, HumanDataGraphic.BlockName, out var graphic, out _);
            blockHeader.TryGetObject<HumanDataParameter>(reader, basePosition, HumanDataParameter.BlockName, out var parameter, out _);
            blockHeader.TryGetObject<HumanDataStatus>(reader, basePosition, HumanDataStatus.BlockName, out var status, out _);
            blockHeader.TryGetObject<HumanDataThumbParameter>(reader, basePosition, HumanDataThumbParameter.BlockName, out var thumbParameter, out _);

            // Modded
            blockHeader.TryGetObject<Dictionary<string, PluginData>>(reader, basePosition, ChaFileExtended.BlockName, out var extData, out var info);
            var extendedSize = info.GetSize();
            // ---------- end blocks ----------

            var card = new AmanatsuCard(file, gameType, extData, extendedSize, about, null, null, gameInfoAl, parameterAl, graphic, parameter, status, thumbParameter, loadVersion)
            {
                Language = about?.language ?? -1,
                UserID = about?.userID,
                DataID = about?.dataID,
            };

            return card;
        }

        public override Image GetCardFaceImage()
        {
            if (GameParameterAl?.imageData != null)
            {
                using (var str = new MemoryStream(GameParameterAl.imageData))
                    return Image.FromStream(str);
            }

            // Imported HC cards don't have a face image
            return null;
        }

        public static string GetProfession(int profession, CharaSex sex)
        {
            switch (sex)
            {
                case CharaSex.Female:
                    var femalePersonalities = new[]
                    {
                        "Ordinary",           //c00
                        "Responsible",        //c01
                        "Energetic",          //c02
                        "Cool",               //c03
                        "Caring",             //c04
                        "Gyaru",              //c05
                        "Boyish",             //c06
                        "Big-sisterly",       //c07
                        "Neat and clean",     //c08
                        "Strong and healthy", //c09
                        "Ojousama",           //c10
                        "Gentle and quiet",   //c11
                    };
                    return GetValue(profession, femalePersonalities);

                case CharaSex.Male:
                    return GetValue(profession, new[] { "Default" });

                default:
                    return Properties.Resources.Unknown;
            }

            string GetValue(int idx, string[] values)
            {
                if (idx < 0) return "Invalid";
                if (idx >= values.Length) return Properties.Resources.Unknown;
                return values[idx];
            }
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