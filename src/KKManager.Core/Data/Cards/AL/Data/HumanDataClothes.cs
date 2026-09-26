using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataClothes
	{
        //[CompilerGenerated]
		//public HumanDataClothes.CoverInfo[] coverInfos;

		public bool[] hideBraOpt { get; set; }

		public bool[] hideShortsOpt { get; set; }

		public bool isSteamLimited { get; set; }

		public HumanDataClothes.PartsInfo[] parts { get; set; }

		public byte shirtType { get; set; }

		public int[] subPartsId { get; set; }

		public Version version { get; set; }

		[MessagePackObject(true)]
		public class CoverInfo
		{
			public Dictionary<int, float> infoTable { get; set; }

			public bool use { get; set; }
		}

		[MessagePackObject(true)]
		public class PartsInfo
		{
			public HumanDataClothes.PartsInfo.ColorInfo[] colorInfo { get; set; }

			public float DentPower { get; set; }

			public int emblemeId { get; set; }

			public int emblemeId2 { get; set; }

			public int hideCategory { get; set; }

			public bool[] hideOpt { get; set; }

			public int id { get; set; }

			public HumanDataPaintInfo[] paintInfos { get; set; }

			public int sleevesType { get; set; }

			[MessagePackObject(true)]
			public class ColorInfo
			{
				public Color baseColor { get; set; }

				public float gloss { get; set; }

				public float metallic { get; set; }

				public HumanDataClothes.PartsInfo.PatternInfo patternInfo { get; set; }
			}

			[MessagePackObject(true)]
			public class PatternInfo
			{
				public Vector2 offset { get; set; }

				public int pattern { get; set; }

				public Color patternColor { get; set; }

				public float rotate { get; set; }

				public Vector2 tiling { get; set; }
			}
		}
	}
}
