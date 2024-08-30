using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.SVS
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Parameter
    {
        [IgnoreMember] public static readonly string BlockName = "Parameter";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        [IgnoreMember] public string fullname => lastname + " " + firstname;
        public byte birthDay { get; set; }
        public byte birthMonth { get; set; }
        public byte bloodType { get; set; }
        public string firstname { get; set; }
        public bool isFutanari { get; set; }
        public string lastname { get; set; }
        public int personality { get; set; }
        public byte sex { get; set; }
        public Version version { get; set; }
        public float voicePitch { get; set; }
        public float voiceRate { get; set; }
    }
}