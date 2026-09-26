using MessagePack;
using System;
using System.ComponentModel;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGraphic
    {
        internal static readonly string BlockName = "Graphic";

        public float LineWidth { get; set; }

		public int RampID { get; set; }

		public float ShadowDepth { get; set; }

		public Version version { get; set; }
	}
}
