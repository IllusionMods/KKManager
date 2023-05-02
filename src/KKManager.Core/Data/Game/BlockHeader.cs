// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace KKManager.Data.Game
{
    [MessagePackObject(true)]
    public class BlockHeader
    {
        public List<Info> lstInfo { get; set; } = new List<Info>();

        public Info SearchInfo(string name)
        {
            return lstInfo.Find(x => x.name == name);
        }

        [MessagePackObject(true)]
        public class Info
        {
            public string name { get; set; }

            public string version { get; set; }

            public long pos { get; set; }

            public long size { get; set; }
        }
    }
    public static class BlockHelper
    {
        // chara blocks.
        public static OrderedDictionary blockNames = new OrderedDictionary
        {
            // HS2/AI/KK/KKS/EC possibly RG
            { "ChaFileCustom", null },
            { "ChaFileCoordinate", null },
            { "ChaFileParameter", null },
            { "ChaFileStatus", null }
        };
        public static OrderedDictionary HS2_AI_blockNames = new OrderedDictionary
        {
            { "ChaFileGameInfo", null },
            { "ChaFileParameter2", null },
            { "ChaFileGameInfo2", null }
        };
        public static OrderedDictionary KKS_blockNames = new OrderedDictionary
        {
            { "ChaFileAbout", null }
        };
        public static HashSet<string> otherAssets = new HashSet<string>()
        {
            { "ChaFile" },
            { "ChaFileDefine" }
        };

        public static Dictionary<string, string> blockMethods = new Dictionary<string, string>
        {
            { "Custom", "SetCustomBytes" },
            { "Coordinate", "SetCoordinateBytes" },
            { "Parameter", "SetParameterBytes" },
            { "Status", "SetStatusBytes" },
            { "GameInfo", "SetGameInfoBytes" },
            { "Parameter2", "SetParameter2Bytes" },
            { "GameInfo2", "SetGameInfo2Bytes" },
            { "About", "SetAboutBytes" }
        };

        public static Dictionary<string, string> blockMethodsNeedVersion = new Dictionary<string, string>
        {
            { "Custom", "SetCustomBytes" },
            { "Coordinate", "SetCoordinateBytes" },
        };

        public static Dictionary<string, string> blockShortNames = new Dictionary<string, string>
        {
            { "ChaFileCustom", "Custom" },
            { "ChaFileCoordinate", "Coordinate" },
            { "ChaFileParameter", "Parameter" },
            { "ChaFileStatus", "Status" },
            { "ChaFileGameInfo", "GameInfo" },
            { "ChaFileParameter2", "Parameter2" },
            { "ChaFileGameInfo2", "GameInfo2" },
            { "ChaFileAbout", "About" }
        };

        public static Dictionary<string, string> blockLongNames = new Dictionary<string, string>
        {
            { "Custom", "ChaFileCustom" },
            { "Coordinate", "ChaFileCoordinate" },
            { "Parameter", "ChaFileParameter" },
            { "Status", "ChaFileStatus" },
            { "GameInfo", "ChaFileGameInfo" },
            { "Parameter2", "ChaFileParameter2" },
            { "GameInfo2", "ChaFileGameInfo2" },
            { "About", "ChaFileAbout" }
        };

        // Key a block name, value is the ChaFile pointer ref. i.e. ChaFileGameInfo2 ChaFile.gameinfo2;
        public static Dictionary<string, string> chaFileBlockNames = new Dictionary<string, string>
        {
            { "ChaFileCustom", "custom" },
            { "ChaFileCoordinate", "coordinate" },
            { "ChaFileParameter", "parameter" },
            { "ChaFileStatus", "status" },
            { "ChaFileGameInfo", "gameinfo" },
            { "ChaFileParameter2", "parameter2" },
            { "ChaFileGameInfo2", "gameinfo2" },
            { "ChaFileAbout", "about" }
        };

        public static string blockVersionMethodName(string blockName)
        {
            return blockName + "Version";
        }

        public static OrderedDictionary GetValidBlockNames(GameName gameName)
        {
            OrderedDictionary combinedDict = new OrderedDictionary();
            foreach (DictionaryEntry item in blockNames)
            {
                combinedDict.Add(item.Key, (object)item.Value);
            }
            if (gameName.Equals("HoneySelect2") || gameName.Equals(new GameName("AI")))
            {
                foreach (DictionaryEntry item in HS2_AI_blockNames)
                {
                    combinedDict.Add(item.Key, (object)item.Value);
                }
            }
            else if (gameName.Equals(new GameName("KKS")))
            {
                foreach (DictionaryEntry item in KKS_blockNames)
                {
                    combinedDict.Add(item.Key, (object)item.Value);
                }
            }
            return combinedDict;
        }
    }
}
