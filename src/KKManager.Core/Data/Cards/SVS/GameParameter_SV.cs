using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.SVS
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GameParameter_SV
    {
        [IgnoreMember] public static readonly string BlockName = "GameParameter_SV";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        public Version version { get; set; }

        [Browsable(false)] public byte[] imageData { get; set; } // Profile picture

        public int job { get; set; }
        public int sexualTarget { get; set; }
        public int lvChastity { get; set; }
        public int lvSociability { get; set; }
        public int lvTalk { get; set; }
        public int lvStudy { get; set; }
        public int lvLiving { get; set; }
        public int lvPhysical { get; set; }
        public int lvDefeat { get; set; }
        public int[] belongings { get; set; }
        public bool isVirgin { get; set; }
        public bool isAnalVirgin { get; set; }
        public bool isMaleVirgin { get; set; }
        public bool isMaleAnalVirgin { get; set; }
        public Individuality individuality { get; set; }
        public PreferenceH preferenceH { get; set; }

        [MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Individuality
        {
            public int[] answer { get; set; }
        }

        [MessagePackObject(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class PreferenceH
        {
            public int[] answer { get; set; }
        }
    }
}