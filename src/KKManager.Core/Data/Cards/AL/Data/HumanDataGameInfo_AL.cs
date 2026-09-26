using MessagePack;
using System;
using System.ComponentModel;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGameInfo_AL
    {
        internal static readonly string BlockName = "GameInfo_AL";

        public Version version { get; set; }
	}
}
