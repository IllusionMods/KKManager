using System;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.HC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataGameInfo_HC
    {
        [IgnoreMember] public static readonly string[] BlockName = { "GameInfo_HC", "GameInfo_HCP" };
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        public int alertness { get; set; }
        public bool arriveRoom50 { get; set; }
        public bool arriveRoom80 { get; set; }
        public bool arriveRoomHAfter { get; set; }
        //public int aversion { get; set; }
        public int Aversion { get; set; }
        public int broken { get; set; }
        public ChaFileDefine.State calcState { get; set; }
        //public int dependence { get; set; }
        public int Dependence { get; set; }
        //public int dirty { get; set; }
        public int Dirty { get; set; }
        //public int enjoyment { get; set; }
        public int Enjoyment { get; set; }
        public bool escapeExperienced { get; set; }
        public byte escapeFlag { get; set; }
        //public int favor { get; set; }
        public int Favor { get; set; }
        public bool firstHFlag { get; set; }
        public bool genericAnalVoice { get; set; }
        public bool genericBefore { get; set; }
        public bool genericBrokenVoice { get; set; }
        public bool genericDependencepVoice { get; set; }
        public bool genericFlag { get; set; }
        public bool genericPainVoice { get; set; }
        //public bool[] genericVoice { get; set; } //todo arrays break messagepack
        public int hCount { get; set; }
        //public bool[] inviteVoice { get; set; } //todo arrays break messagepack
        public bool isChangeParameter { get; set; }
        public bool isConcierge { get; set; }
        //public int libido { get; set; }
        public int Libido { get; set; }
        public bool lockBroken { get; set; }
        public bool lockDependence { get; set; }
        public bool lockNowState { get; set; }
        [TypeConverter(typeof(CollectionConverter))] //todo doesn't work
        public HashSet<int> map { get; set; }
        public ChaFileDefine.State nowDrawState { get; set; }
        public ChaFileDefine.State nowState { get; set; }
        public int resistAnal { get; set; }
        public bool ResistedAnal { get; set; }
        public bool ResistedH { get; set; }
        public bool ResistedPain { get; set; }
        public int resistH { get; set; }
        public int resistPain { get; set; }
        //public int slavery { get; set; }
        public int Slavery { get; set; }
        public bool TalkFlag { get; set; }
        //public int tiredness { get; set; }
        public int Tiredness { get; set; }
        //public int toilet { get; set; }
        public int Toilet { get; set; }
        public int usedItem { get; set; }
        public Version version { get; set; }
    }
}