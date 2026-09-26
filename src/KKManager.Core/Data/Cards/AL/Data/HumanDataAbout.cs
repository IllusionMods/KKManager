using MessagePack;
using System;
using System.ComponentModel;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataAbout
    {
        internal static readonly string BlockName = "About";

        public string dataID { get; set; }

		public int language { get; set; }

        public string userID { get; set; }

        public Version version { get; set; }
    }
}
