// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
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
        public static SortedDictionary<string, object> blockNames = new SortedDictionary<string, object>
        {
            // HS2/AI/KK/KKS/EC possibly RG
            { "ChaFileCustom", null },
            { "ChaFileCoordinate", null },
            { "ChaFileParameter", null },
            { "ChaFileStatus", null }
        };
        public static SortedDictionary<string, object> HS2_AI_blockNames = new SortedDictionary<string, object>
        {
            { "ChaFileGameInfo", null },
            { "ChaFileParameter2", null },
            { "ChaFileGameInfo2", null }
        };
        public static SortedDictionary<string, object> KKS_blockNames = new SortedDictionary<string, object>
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
            return blockLongNames[blockName] + "Version";
        }

        public static SortedDictionary<string, object> GetValidBlockNames(GameName gameName)
        {
            SortedDictionary<string, object> combinedDict = new SortedDictionary<string, object>();
            foreach (var item in blockNames)
            {
                combinedDict.Add(item.Key, item.Value);
            }
            if (gameName.Equals("HoneySelect2") || gameName.Equals(new GameName("AI")))
            {
                foreach (var item in HS2_AI_blockNames)
                {
                    combinedDict.Add(item.Key, item.Value);
                }
            }
            else if (gameName.Equals(new GameName("KKS")))
            {
                foreach (var item in KKS_blockNames)
                {
                    combinedDict.Add(item.Key, item.Value);
                }
            }
            return combinedDict;
        }
    }
}
