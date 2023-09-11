using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.HC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGameParameter_HC
    {
        [IgnoreMember] public static readonly string[] BlockName = { "GameParameter_HC", "GameParameter_HCP" };
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        public ChaFileDefine.Kink hAttribute { get; set; } //todo decode flags
        public ChaFileDefine.Mood mind { get; set; } //todo decode flags
        public ChaFileDefine.Quirk trait { get; set; } //todo decode flags
        public Version version { get; set; }
    }
}