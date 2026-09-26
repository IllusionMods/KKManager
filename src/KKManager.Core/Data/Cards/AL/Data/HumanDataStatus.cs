using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataStatus
    {
        internal static readonly string BlockName = "Status";

        public int backCoordinateType;

		public byte[] clothesState { get; set; }

		public float[] ClothTransparencies { get; set; }

		public float[] ClothWets { get; set; }

		public int coordinateType { get; set; }

		public float?[,] disableBustShapeMask { get; set; }

		public bool disableMouthShapeMask { get; set; }

		public bool[] enableShapeHand { get; set; }

		public float eyebrowOpenMax { get; set; }

		public int eyebrowPtn { get; set; }

		public bool eyesBlink { get; set; }

		public int eyesLookPtn { get; set; }

		public float eyesOpenMax { get; set; }

		public int eyesPtn { get; set; }

		public float eyesTargetAngle { get; set; }

		public float eyesTargetRange { get; set; }

		public float eyesTargetRate { get; set; }

		public int eyesTargetType { get; set; }

		public bool eyesYure { get; set; }

		public HumanDataStatus.FluidFlowDataStatus FFCloth { get; set; }

		public HumanDataStatus.FluidFlowDataStatus FFSkin { get; set; }

		public bool hideEyesHighlight { get; set; }

		public float hohoAkaRate { get; set; }

		public bool mouthAdjustWidth { get; set; }

		public bool mouthFixed { get; set; }

		public float mouthOpenMax { get; set; }

		public float mouthOpenMin { get; set; }

		public int mouthPtn { get; set; }

		public int neckLookPtn { get; set; }

		public float neckTargetAngle { get; set; }

		public float neckTargetRange { get; set; }

		public float neckTargetRate { get; set; }

		public int neckTargetType { get; set; }

		public float nipStandRate { get; set; }

		public float[] shapeHandBlendValue { get; set; }

		public int[,] shapeHandPtn { get; set; }

		public bool[] showAccessory { get; set; }

		public Color simpleColor { get; set; }

		public float siriAkaRate { get; set; }

		public byte[] siruLv { get; set; }

		public float skinTuyaRate { get; set; }

		public float sweatRate { get; set; }

		public byte tearsLv { get; set; }

		public byte tongueState { get; set; }

		public Version version { get; set; }

		public bool visibleBodyAlways { get; set; }

		public bool visibleGomu { get; set; }

		public bool visibleHeadAlways { get; set; }

		public bool visibleSimple { get; set; }

		public bool visibleSon { get; set; }

		public bool visibleSonAlways { get; set; }

		public float wetRate { get; set; }

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class FluidFlowDataStatus
		{
			public float ColorScatter { get; set; }

			public float HighlightPower { get; set; }

			public float HighlightSize { get; set; }

			public float MinColorScale { get; set; }

			public float NoiseAmount { get; set; }

			public float NoiseScale { get; set; }

			public float NormalScale { get; set; }

			public float Scale { get; set; }

			public float ShadowPower { get; set; }

			public float ShadowSaturation { get; set; }

			public float ShadowSize { get; set; }

			public float Transmittance { get; set; }
		}
	}
}
