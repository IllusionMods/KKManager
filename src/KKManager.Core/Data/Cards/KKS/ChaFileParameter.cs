using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.KKS
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChaFileParameter
    {
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.6");
        [IgnoreMember] public static readonly string BlockName = "Parameter";

        public ChaFileParameter()
        {
            MemberInit();
        }

        public void ComplementWithVersion()
        {
            version = (Version)CurrentVersion.Clone();
        }

        public void MemberInit()
        {
            ComplementWithVersion();
            sex = 0;
            exType = 0;
            lastname = "";
            firstname = "";
            nickname = "";
            callType = -1;
            personality = 0;
            bloodType = 0;
            birthMonth = 1;
            birthDay = 1;
            clubActivities = 0;
            voiceRate = 0.5f;
            awnser = new Awnser();
            weakPoint = -1;
            denial = new Denial();
            attribute = new Attribute();
            aggressive = 0;
            diligence = 0;
            kindness = 0;
            interest = new Interest();
            interest.MemberInit();
        }

        public int aggressive { get; set; }

        public Attribute attribute { get; set; }

        public Awnser awnser { get; set; }

        public byte birthDay { get; set; }

        public byte birthMonth { get; set; }

        public byte bloodType { get; set; }

        public int callType { get; set; }

        public byte clubActivities { get; set; }

        public Denial denial { get; set; }

        public int diligence { get; set; }

        public int exType { get; set; }

        public string firstname { get; set; }

        [IgnoreMember]
        public string fullname => lastname + " " + firstname;

        public Interest interest { get; set; }

        public int kindness { get; set; }

        public string lastname { get; set; }

        public string nickname { get; set; }

        public int personality { get; set; }

        public byte sex { get; set; }

        public Version version { get; set; }

        public float voiceRate { get; set; }

        public int weakPoint { get; set; }

        [MessagePackObject(true)]
        public class Attribute
        {
            public bool active { get; set; }

            public bool bitch { get; set; }

            public bool choroi { get; set; }

            public bool dokusyo { get; set; }

            public bool friendly { get; set; }

            public bool harapeko { get; set; }

            public bool hinnyo { get; set; }

            public bool hitori { get; set; }

            public bool info { get; set; }

            public bool kireizuki { get; set; }

            public bool likeGirls { get; set; }

            public bool lonely { get; set; }

            public bool love { get; set; }

            public bool majime { get; set; }

            public bool mutturi { get; set; }

            public bool nakama { get; set; }

            public bool nonbiri { get; set; }

            public bool okute { get; set; }

            public bool ongaku { get; set; }

            public bool sinsyutu { get; set; }

            public bool talk { get; set; }

            public bool ukemi { get; set; }

            public bool undo { get; set; }
        }

        [MessagePackObject(true)]
        public class Awnser
        {
            public bool animal { get; set; }

            public bool blackCoffee { get; set; }

            public bool cook { get; set; }

            public bool eat { get; set; }

            public bool exercise { get; set; }

            public bool fashionable { get; set; }

            public bool spicy { get; set; }

            public bool study { get; set; }

            public bool sweet { get; set; }
        }

        [MessagePackObject(true)]
        public class Denial
        {
            public bool aibu { get; set; }

            public bool anal { get; set; }

            public bool kiss { get; set; }

            public bool massage { get; set; }

            public bool notCondom { get; set; }
        }

        [MessagePackObject(true)]
        public class Interest
        {
            public Interest()
            {
                MemberInit();
            }

            public void MemberInit()
            {
                answer = new int[AnswerMax];
                for (int i = 0; i < answer.Length; i++)
                {
                    answer[i] = None;
                }
            }

            public int[] answer { get; set; }

            [IgnoreMember] public static int AnswerMax { get; } = 2;

            [IgnoreMember] public static int None { get; } = -1;
        }
    }
}