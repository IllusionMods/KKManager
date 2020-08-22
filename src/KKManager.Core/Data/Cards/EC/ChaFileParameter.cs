using System;
using System.ComponentModel;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards.EC
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChaFileParameter
    {
        [IgnoreMember] public static readonly string BlockName = "Parameter";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");

        public ChaFileParameter()
        {
            version = (Version)CurrentVersion.Clone();
            sex = 0;
            exType = 0;
            fullname = string.Empty;
            personality = 0;
            bloodType = 0;
            birthMonth = 1;
            birthDay = 1;
        }

        public Version version { get; set; }

        public byte sex { get; set; }

        public int exType { get; set; }

        public string fullname { get; set; }

        public int personality { get; set; }

        public byte bloodType { get; set; }

        public byte birthMonth { get; set; }

        public byte birthDay { get; set; }

        public void ComplementWithVersion()
        {
            version = (Version)CurrentVersion.Clone();
        }
    }
}