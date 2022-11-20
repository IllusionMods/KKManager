using System;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.RG
{
    [MessagePackObject]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChaFileParameter
    {
        [IgnoreMember] public static readonly string BlockName = "Parameter";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.3");
        [Key("version")] public Version version { get; set; }
        [Key("sex")] public byte sex { get; set; } // 00 male, 01 female
        [Key("fullname")] public string fullname { get; set; }
        [Key("personality")] public byte personality { get; set; }
        [Key("birthMonth")] public byte birthMonth { get; set; } // 0x01 - 0x0C
        [Key("birthDay")] public byte birthDay { get; set; } // 0x00 - 0x1F
        [Key("voiceRate")] public float voiceRate { get; set; } // float32
        [Key("property")] public byte property { get; set; }
        [Key("features")] public List<byte> features { get; set; }
        [Key("propensity")] public List<byte> propensity { get; set; }
        [Key("futanari")] public bool futanari { get; set; }
        [IgnoreMember] [Key("Preset")] public byte Preset { get; set; } // ??? idk what this is
    }
}