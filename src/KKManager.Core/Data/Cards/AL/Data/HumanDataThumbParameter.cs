using MessagePack;
using System;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataThumbParameter
    {
        internal static readonly string BlockName = "ThumbParameter";

        public HumanDataThumbParameter.CameraData Camera { get; set; }

		public HumanDataThumbParameter.ExpData Exp { get; set; }

		public HumanDataThumbParameter.LightData Light { get; set; }

		public HumanDataThumbParameter.PoseData Pose { get; set; }

		public Version version { get; set; }

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CameraData
		{
			public Vector3 Dir { get; set; }

			public float Fov { get; set; }

			public Vector3 Pos { get; set; }

			public Vector3 Rot { get; set; }
		}

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ExpData
		{
			public HumanDataThumbParameter.IDCheck Eye { get; set; }

			public HumanDataThumbParameter.IDCheck Eyebrow { get; set; }

			public float EyeOpenMax { get; set; }

			public HumanDataThumbParameter.IDCheck Mouth { get; set; }

			public float MouthOpenMax { get; set; }
		}

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class IDCheck
		{
			public int ID { get; set; }

			public int Index { get; set; }
		}

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class LightData
		{
			public Color Color { get; set; }

			public Vector2 Dir { get; set; }

			public float Power { get; set; }
		}

		[MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class PoseData
		{
			public int Eyes { get; set; }

			public int[] Hands { get; set; }

			public int Neck { get; set; }

			public float NormalizedTime { get; set; }

			public HumanDataThumbParameter.IDCheck Pose { get; set; }
		}
	}
}
