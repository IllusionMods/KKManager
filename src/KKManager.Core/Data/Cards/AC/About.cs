using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.AC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class About
    {
        [IgnoreMember] public static readonly string BlockName = "About";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        public Version version { get; set; }

        public string dataID { get; set; }

        public int language { get; set; }

        public string userID { get; set; }
    }
}
