using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.AC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Graphic
    {
        [IgnoreMember] public static readonly string BlockName = "Graphic";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        public Version version { get; set; }

        public int RampID { get; set; }

        public float ShadowDepth { get; set; }

        public float LineWidth { get; set; }
    }
}
