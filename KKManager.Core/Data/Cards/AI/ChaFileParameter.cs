using System;
using System.Collections.Generic;
using System.ComponentModel;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards.AI
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(SimpleExpandTypeConverter<ChaFileParameter>))]
    public class ChaFileParameter
    {
        public ChaFileParameter()
        {
            version = new Version(1, 0);
            sex = 0;
            fullname = string.Empty;
            personality = 0;
            birthMonth = 1;
            birthDay = 1;
            voiceRate = 0.5f;
            hsWish = new HashSet<int>();
            futanari = false;
        }

        public Version version { get; set; }

        public byte sex { get; set; }

        public string fullname { get; set; }

        public int personality { get; set; }

        public byte birthMonth { get; set; }

        public byte birthDay { get; set; }

        public float voiceRate { get; set; }

        [TypeConverter(typeof(CollectionConverter))]
        public HashSet<int> hsWish { get; set; }

        public bool futanari { get; set; }
        
        [IgnoreMember]
        public static readonly string BlockName = "Parameter";
    }
}