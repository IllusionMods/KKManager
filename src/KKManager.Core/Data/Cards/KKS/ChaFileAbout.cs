using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.KKS
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChaFileAbout
    {
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");
        [IgnoreMember] public static readonly string BlockName = "About";

        public ChaFileAbout()
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
            language = 0;
            userID = "";
            dataID = "";
        }

        public string dataID { get; set; }
        public int language { get; set; }
        public string userID { get; set; }
        public Version version { get; set; }
    }
}
