using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards.SVS
{
    public class SamabakeCard : Card
    {
        public override string Name => Parameter?.fullname ?? base.Name;
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetProfession(Parameter?.personality ?? -1);
        public string Birthday => $"{GetBirthMonth(Parameter.birthMonth)} {Parameter.birthDay}";
        [ReadOnly(true)] public Parameter Parameter { get; }
        [ReadOnly(true)] public Graphic Graphic { get; }
        [ReadOnly(true)] public GameInfo_SV GameInfo { get; }
        [ReadOnly(true)] public GameParameter_SV GameParameter { get; }

        private SamabakeCard(
            FileInfo cardFile,
            CardType type,
            Dictionary<string, PluginData> extended,
            FileSize extendedSize,
            Parameter parameter,
            Graphic graphic,
            Version version,
            GameInfo_SV gameInfo,
            GameParameter_SV gameParameter) : base(cardFile, type, extended, extendedSize, version)
        {
            Parameter = parameter;
            Graphic = graphic;
            GameInfo = gameInfo;
            GameParameter = gameParameter;
        }

        public static SamabakeCard ParseSvsChara(FileInfo file, BinaryReader reader, CardType gameType)
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
            //Custom
            //Coordinate
            blockHeader.TryGetObject<Parameter>(reader, basePosition, Parameter.BlockName, out var parameter, out _);
            //blockHeader.TryGetObject<Status>(reader, position, Status.BlockName, out var status, out _); //TODO broken
            blockHeader.TryGetObject<Graphic>(reader, basePosition, Graphic.BlockName, out var graphic, out _);
            blockHeader.TryGetObject<About>(reader, basePosition, About.BlockName, out var about, out _);
            blockHeader.TryGetObject<GameParameter_SV>(reader, basePosition, GameParameter_SV.BlockName, out var gameParameter, out _);
            blockHeader.TryGetObject<GameInfo_SV>(reader, basePosition, GameInfo_SV.BlockName, out var gameInfo, out _);
            // Modded
            blockHeader.TryGetObject<Dictionary<string, PluginData>>(reader, basePosition, ChaFileExtended.BlockName, out var extData, out var info);
            var extendedSize = info.GetSize();
            // ---------- end blocks ----------

            var card = new SamabakeCard(file, gameType, extData, extendedSize, parameter, graphic, loadVersion, gameInfo, gameParameter)
            {
                Language = about?.language ?? -1,
                UserID = about?.userID,
                DataID = about?.dataID,
            };

            return card;
        }

        public override Image GetCardFaceImage()
        {
            if (GameParameter?.imageData != null)
            {
                using (var str = new MemoryStream(GameParameter.imageData))
                    return Image.FromStream(str);
            }

            // Imported HC cards don't have a face image
            return null;
        }

        public static string GetProfession(int profession)
        {
            if (profession < 100)
            {
                var femalePersonalities = new[] { "Ordinary", "Cheeky", "Cool", "Gyaru", "Brave", "Bewitching", "Innocent", "Motherly", "Devilish", "Yankee" };
                return GetValue(profession, femalePersonalities);
            }
            else
            {
                var malePersonalities = new[] { "Gentle", "Bold" };
                return GetValue(profession - 100, malePersonalities);
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