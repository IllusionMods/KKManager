using MessagePack;
using System;
using System.ComponentModel;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGameParameter_AL
    {
        internal static readonly string BlockName = "GameParameter_AL";

        public byte characteristic { get; set; }

		[Browsable(false)]
		public byte[] imageData { get; set; }

		public HumanDataGameParameter_AL.PreferenceH preferenceH { get; set; }

		public Version version { get; set; }

		[MessagePackObject(true)]
		[Union(0, typeof(HumanDataGameParameter_AL.PreferenceH))]
		public abstract class AnswerBase
		{
			public int[] answer { get; set; }

            public override string ToString()
            {
                return answer == null ? "None" : string.Join(", ", answer);
            }
        }

		public struct ImageDataBackup
		{
			private HumanDataGameParameter_AL _gameParameter { get; set; }

			private byte[] _imageData { get; set; }
		}

		[MessagePackObject(true)]
		public class PreferenceH : HumanDataGameParameter_AL.AnswerBase
		{
		}
	}
}
