using System;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.AI
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChaFileParameter
    {
        public Version version { get; set; } = new Version("0.0.0");

		public byte sex { get; set; }

        public string fullname { get; set; } = string.Empty;

		public int personality { get; set; }

        public byte birthMonth { get; set; } = 1;

		public byte birthDay { get; set; } = 1;

		public float voiceRate { get; set; } = 0.5f;

		[TypeConverter(typeof(CollectionConverter))]
        public HashSet<int> hsWish { get; set; } = new HashSet<int>();

		public bool futanari { get; set; }
        
        [IgnoreMember]
        public static readonly string BlockName = "Parameter";

		public override string ToString() => "AIS Chara Parameters";
	}

	[MessagePackObject(true)]
	[ReadOnly(true)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ChaFileParameter2
	{
		public Version version { get; set; } = new Version("0.0.0");

		public int personality { get; set; }

		public float voiceRate { get; set; } = 0.5f;

		public byte trait { get; set; }

		public byte mind { get; set; }

		public byte hAttribute { get; set; }

		[IgnoreMember]
		public static readonly string BlockName = "Parameter2";

		public override string ToString() => "HS2 Chara Parameters";
	}

	[MessagePackObject(true)]
	[ReadOnly(true)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ChaFileGameInfo
	{
		public Version version { get; set; } = new Version("0.0.0");

		public bool gameRegistration { get; set; }

		public MinMaxInfo tempBound { get; set; } = new MinMaxInfo();

		public MinMaxInfo moodBound { get; set; } = new MinMaxInfo();

		//public Dictionary<int, int> flavorState { get; set; } = new Dictionary<int, int>();

		public int totalFlavor { get; set; }

		//public Dictionary<int, float> desireDefVal { get; set; } = new Dictionary<int, float>();

		//public Dictionary<int, float> desireBuffVal { get; set; } = new Dictionary<int, float>();

		public int phase { get; set; }

		//public Dictionary<int, int> normalSkill { get; set; } = new Dictionary<int, int>();

		//public Dictionary<int, int> hSkill { get; set; } = new Dictionary<int, int>();

		public int favoritePlace { get; set; } = -1;

		public int lifestyle { get; set; } = -1;

		public int morality { get; set; }

		public int motivation { get; set; }

		public int immoral { get; set; }

		public bool isHAddTaii0 { get; set; }

		public bool isHAddTaii1 { get; set; }

		public ChaFileGameInfo()
		{
			//for (int i = 0; i < 8; i++)
			//{
			//	flavorState[i] = 0;
			//}
			//for (int j = 0; j < 16; j++)
			//{
			//	desireDefVal[j] = 0f;
			//	desireBuffVal[j] = 0f;
			//}
			//for (int k = 0; k < 5; k++)
			//{
			//	normalSkill[k] = -1;
			//	hSkill[k] = -1;
			//}
		}

		[IgnoreMember]
		public static readonly string BlockName = "GameInfo";

		public override string ToString() => "AIS Game Progress";

		[MessagePackObject(true)]
		[ReadOnly(true)]
		//[TypeConverter(typeof(ExpandableObjectConverter))]
		public class MinMaxInfo
		{
			public float lower { get; set; } = 20f;

			public float upper { get; set; } = 80f;

			public override string ToString() => $"From {lower} to {upper}";
		}
	}

	[MessagePackObject(true)]
	[ReadOnly(true)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ChaFileGameInfo2
	{
		public Version version { get; set; } = new Version("0.0.0");

		public int Favor{ get; set; }

		public int Enjoyment{ get; set; }

		public int Aversion{ get; set; }

		public int Slavery{ get; set; }

		public int Broken{ get; set; }

		public int Dependence{ get; set; }

		public State nowState{ get; set; }

		public State nowDrawState{ get; set; }

		public bool lockNowState{ get; set; }

		public bool lockBroken{ get; set; }

		public bool lockDependence{ get; set; }

		public int Dirty{ get; set; }

		public int Tiredness{ get; set; }

		public int Toilet{ get; set; }

		public int Libido{ get; set; }

		public int alertness{ get; set; }

		public State calcState{ get; set; }

		public byte escapeFlag{ get; set; }

		public bool escapeExperienced{ get; set; }

		public bool firstHFlag{ get; set; }

		//public bool[][] genericVoice = new bool[2][]{ get; set; }

		public bool genericBrokenVoice{ get; set; }

		public bool genericDependencepVoice{ get; set; }

		public bool genericAnalVoice{ get; set; }

		public bool genericPainVoice{ get; set; }

		public bool genericFlag{ get; set; }

		public bool genericBefore{ get; set; }

		//public bool[] inviteVoice = new bool[5]{ get; set; }

		public int hCount{ get; set; }

		//public HashSet<int> map = new HashSet<int>(){ get; set; }

		public bool arriveRoom50{ get; set; }

		public bool arriveRoom80{ get; set; }

		public bool arriveRoomHAfter{ get; set; }

		public int resistH{ get; set; }

		public int resistPain{ get; set; }

		public int resistAnal{ get; set; }

		public int usedItem{ get; set; }

		public bool isChangeParameter{ get; set; }

		public bool isConcierge{ get; set; }

		[IgnoreMember]
		public static readonly string BlockName = "GameInfo2";

		public override string ToString() => "HS2 Game Progress";

		public enum State
		{
			Blank,
			Favor,
			Enjoyment,
			Aversion,
			Slavery,
			Broken,
			Dependence
		}
	}
}