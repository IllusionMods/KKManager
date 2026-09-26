using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataBody
	{
		public float areolaSize { get; set; }

		public float bellySoftness { get; set; }

		public float bustSoftness { get; set; }

		public float bustWeight { get; set; }

		public int detailId { get; set; }

		public float detailPower { get; set; }

		public bool drawAddLine { get; set; }

		public bool drawBustShadow { get; set; }

		public float hipSoftness { get; set; }

		public bool isSteamLimited { get; set; }

		public Color nipColor { get; set; }

		public Color nipColor2 { get; set; }

		public float nipGlossPower { get; set; }

		public int nipId { get; set; }

		public float[] shapeValueBody { get; set; }

		public Color[] skinDetailColors { get; set; }

		public Color skinHighlightColor { get; set; }

		public int skinId { get; set; }

		public Color skinMainColor { get; set; }

		public Color skinShadowColor { get; set; }

		public int skinShineId { get; set; }

		public float skinShinePower { get; set; }

		public Color sunburnColor { get; set; }

		public int sunburnDownId { get; set; }

		public int sunburnUpId { get; set; }

		public float thighSoftness { get; set; }

		public Color underhairColor { get; set; }

		public int underhairId { get; set; }

		public float upperArmSoftness { get; set; }

		public Version version { get; set; }

		public float waistSoftness { get; set; }
	}
}
