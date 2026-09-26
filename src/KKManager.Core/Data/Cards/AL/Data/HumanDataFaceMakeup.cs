using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataFaceMakeup
	{
		public Color cheekColor { get; set; }

		public Color cheekHighlightColor { get; set; }

		public int cheekId { get; set; }

		public Vector2 cheekPos { get; set; }

		public float cheekRotation { get; set; }

		public float cheekSize { get; set; }

		public Color eyeshadowColor { get; set; }

		public int eyeshadowId { get; set; }

		public Color lipColor { get; set; }

		public Color lipHighlightColor { get; set; }

		public int lipId { get; set; }

		public HumanDataPresetPaintInfo[] paintInfos { get; set; }

		public Version version { get; set; }
	}
}
