using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.AC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGameParameter_AC
    {
        public enum ErogenousZoneType
        {
            None,
            Mouth,
            Breast,
            Nipple,
            Groin,
            Anal,
            Hip
        }

        [IgnoreMember] public static readonly string BlockName = "GameParameter_AC";
        [IgnoreMember] public static readonly Version Version = new Version("0.0.0");

        [Browsable(false)] public byte[] imageData;
        public Version version;

        public Characteristics characteristics;
        public byte clubActivities;
        public ErogenousZoneType erogenousZone;
        public Hobby hobby;
        public bool[] individuality;

        [MessagePackObject(true)]
        [Union(0, typeof(Characteristics))]
        [Union(1, typeof(Hobby))]
        [ReadOnly(true)]
        public abstract class AnswerBase
        {
            [IgnoreMember] public const int None = -1;

            public int[] answer;

            public override string ToString()
            {
                return answer == null ? "null" : string.Join(", ", answer);
            }
        }

        [MessagePackObject(true)]
        [ReadOnly(true)]
        public class Characteristics : AnswerBase { }

        [MessagePackObject(true)]
        [ReadOnly(true)]
        public class Hobby : AnswerBase { }
    }
}
