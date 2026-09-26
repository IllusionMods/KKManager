using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataAccessory
	{
		public HumanDataAccessory.PartsInfo[] parts { get; set; }

		public Version version { get; set; }

		[MessagePackObject(true)]
		public class PartsInfo
		{
			public Vector3[,] addMove { get; set; }

			public Color[] color { get; set; }

			public HumanDataAccessory.PartsInfo.ColorInfo[] colorInfo { get; set; }

			public HumanDataAccessory.PartsInfo.FKInfo fkInfo { get; set; }

			public float[] gloss { get; set; }

			public int hideCategory { get; set; }

			public int hideCategoryClothes { get; set; }

			public int id { get; set; }

			public float[] metallic { get; set; }

			public bool noShake { get; set; }

			public int parentKeyType { get; set; }

			public const int PatternLength = 3;

			public int type { get; set; }

			public bool[] visibleTimings { get; set; }

			[MessagePackObject(true)]
			public class ColorInfo
			{
				public Vector2 offset { get; set; }

				public int pattern { get; set; }

				public Color patternColor { get; set; }

				public float rotate { get; set; }

				public Vector2 tiling { get; set; }
			}

			[MessagePackObject(true)]
			public class FKInfo
			{
				public Vector3[] bones { get; set; }

				public bool use { get; set; }
			}
		}
	}
}
