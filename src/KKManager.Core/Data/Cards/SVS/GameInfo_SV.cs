using System;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.SVS
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GameInfo_SV
    {
        [IgnoreMember] public static readonly string BlockName = "GameInfo_SV";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");

        public Version version { get; set; }

        // TODO Empty class? Check contents once metadata is available
    }
}