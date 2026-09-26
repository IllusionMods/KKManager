using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataHair
	{
		public int glossId { get; set; }

		public float glossSize { get; set; }

		public int kind { get; set; }

		public HumanDataHair.PartsInfo[] parts { get; set; }

		public Version version { get; set; }

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class PartsInfo
		{
			public Color[] acsColor { get; set; }

			public Color baseColor { get; set; }

			public int bundleId { get; set; }

			public Dictionary<int, HumanDataHair.PartsInfo.BundleInfo> dictBundle { get; set; }

			public Dictionary<int, bool> dictOption { get; set; }

			public Color endColor { get; set; }

			public Color glossColor { get; set; }

			public Color glossColor2 { get; set; }

			public int id { get; set; }

			public Color innerColor { get; set; }

			public bool IsOtherNoShake { get; set; }

			public Color meshColor { get; set; }

			public Color outlineColor { get; set; }

			public Vector3 pos { get; set; }

			public Vector3 rot { get; set; }

			public Vector3 scl { get; set; }

			public Color shadowColor { get; set; }

			public Color startColor { get; set; }

			public bool useInner { get; set; }

			public bool useMesh { get; set; }

			[MessagePackObject(true)]
            [ReadOnly(true)]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public class BundleInfo
			{
				public Vector3 moveRate { get; set; }

				public bool noShake { get; set; }

				public Vector3 rotRate { get; set; }
			}
		}
	}
}
