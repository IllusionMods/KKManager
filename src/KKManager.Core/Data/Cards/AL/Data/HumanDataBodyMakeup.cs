using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataBodyMakeup
	{
		public HumanDataBodyMakeup.NailInfo nailInfo { get; set; }

		public HumanDataBodyMakeup.NailInfo nailLegInfo { get; set; }

		public HumanDataPresetPaintInfo[] paintInfos { get; set; }

		public Version version { get; set; }

		[MessagePackObject(true)]
		public class NailInfo
		{
			public Color[] colors { get; set; }

			public float glossPower { get; set; }

			public int ID { get; set; }

			public HumanDataPresetPaintInfo[] paintInfos { get; set; }
		}
	}
}
