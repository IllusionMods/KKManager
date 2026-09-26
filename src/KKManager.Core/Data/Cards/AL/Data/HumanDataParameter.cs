using MessagePack;
using System;
using System.ComponentModel;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataParameter
    {
        internal static readonly string BlockName = "Parameter";

        public byte birthDay { get; set; }

		public byte birthMonth { get; set; }

		public byte bloodType { get; set; }

		public string firstname { get; set; }

		public string lastname { get; set; }

		public string nickname { get; set; }

		public int personality { get; set; }

		public byte sex { get; set; }

		public Version version { get; set; }

		public float voiceRate { get; set; }
	}
}
