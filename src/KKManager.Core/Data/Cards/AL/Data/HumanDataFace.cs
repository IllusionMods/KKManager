using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataFace
	{
		public Color detailColor { get; set; }

		public int detailId { get; set; }

		public float detailPower { get; set; }

		public bool doubleTooth { get; set; }

		public Color eyebrowColor { get; set; }

		public Color eyebrowColor2 { get; set; }

		public float eyebrowHeight { get; set; }

		public int eyebrowId { get; set; }

		public float eyebrowWidth { get; set; }

		public float eyeHeight { get; set; }

		public Color eyelidColor { get; set; }

		public int eyelidId { get; set; }

		public Vector2 eyelidPos { get; set; }

		public float eyelidRotation { get; set; }

		public float eyelidSize { get; set; }

		public Color eyelineColor { get; set; }

		public Color eyelineColor2 { get; set; }

		public Color eyelineColor3 { get; set; }

		public int eyelineDownId { get; set; }

		public int eyelineUpId { get; set; }

		public float eyelineUpWeight { get; set; }

		public float eyeWidth { get; set; }

		public float eyeX { get; set; }

		public float eyeY { get; set; }

		public byte foregroundEyebrow { get; set; }

		public byte foregroundEyes { get; set; }

		public float hairTransparency { get; set; }

		public int headId { get; set; }

		public int lipLineId { get; set; }

		public HumanDataPresetPaintInfo moleInfo { get; set; }

		public Color noseColor { get; set; }

		public float noseGlossIntensity { get; set; }

		public int noseId { get; set; }

		public HumanDataFace.PupilInfo[] pupil { get; set; }

		public byte pupilEditType { get; set; }

		public float pupilHeight { get; set; }

		public float pupilWidth { get; set; }

		public float[] shapeValueFace { get; set; }

		public int skinId { get; set; }

		public Version version { get; set; }

		public Color whiteBaseColor { get; set; }

		public int whiteId { get; set; }

		public Color whiteSub2Color { get; set; }

		public Color whiteSubColor { get; set; }

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class HighlightInfo
		{
			public Color color { get; set; }

			public Color color2 { get; set; }

			public float height { get; set; }

			public int id { get; set; }

			public float rotation { get; set; }

			public float width { get; set; }

			public float x { get; set; }

			public float y { get; set; }
		}

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class PupilInfo
		{
			public Color eye01Color { get; set; }

			public Color eye02Color { get; set; }

			public Color eye03Color { get; set; }

			public Color eyeGradColor { get; set; }

			public float eyeGradPosition { get; set; }

			public float eyeGradSize { get; set; }

			public int gradMaskId { get; set; }

			public HumanDataFace.HighlightInfo[] highlightInfos { get; set; }

			public int id { get; set; }

			public int overId { get; set; }

			public Color pupil01Color { get; set; }

			public Color pupil02Color { get; set; }

			public Color pupil03Color { get; set; }
		}
	}
}
