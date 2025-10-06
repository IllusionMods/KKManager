using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.AC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGameInfo_AC
    {
        [IgnoreMember] public static readonly string BlockName = "GameInfo_AC";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");

        public Version version { get; set; }
    }
}