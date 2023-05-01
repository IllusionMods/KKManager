// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Dynamic;
using Microsoft.Win32;
using MessagePack;
using System.Collections.Specialized;

namespace KKManager.Data.Game
{
    public static class Consts
    {
        public static Dictionary<string, string> registryPathMapping = new Dictionary<string, string>()
        {
            { "HoneySelect2", @"HKEY_CURRENT_USER\SOFTWARE\illusion\HoneySelect2\HoneySelect2" },
            { "AI-Syoujyo", @"HKEY_CURRENT_USER\SOFTWARE\illusion\AI-Syoujyo\AI-Syoujyo" },
            { "Koikatu", @"HKEY_CURRENT_USER\SOFTWARE\illusion\Koikatu\koikatu" },
            { "KoikatsuSunshine", @"HKEY_CURRENT_USER\SOFTWARE\illusion\KoikatsuSunshine\KoikatsuSunshine" },
            { "EmotionCreations", @"HKEY_CURRENT_USER\SOFTWARE\illusion\TBD\TBD" },
            { "RoomGirl", @"HKEY_CURRENT_USER\SOFTWARE\illusion\TBD\TBD" },
            { "PlayHome", @"HKEY_CURRENT_USER\SOFTWARE\illusion\TBD\TBD" }
        };

        public static Dictionary<string, GameName> gameNameByChunkDict = new Dictionary<string, GameName>
        {
            { "\u3010AIS_Chara\u3011", new GameName("HS2") },
            { "\u3010HoneySelectCharaFemale\u3011", new GameName("HS1") }, // todo
            { "\u3010KoiKatuChara\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaS\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaSP\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaSun\u3011", new GameName("KKS") },
            { "\u3010KStudio\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaS\u3011", new GameName("KK") },
            { "\u3010EroMakeChara\u3011", new GameName("EC") },
            { "\u3010RG_Chara\u3011", new GameName("RG") }, // todo
            { "\u3010KStudio\u3011", new GameName("EC") }, // todo
        };

        public static string GetInstallPath(GameName gameName)
        {
            string gameInstallPath = "";
            foreach (var thisGameReg in registryPathMapping)
            {
                if (gameName.Equals(thisGameReg))
                {
                    gameInstallPath = thisGameReg.Value;

                    if (Directory.Exists(gameInstallPath))
                    {
                        return gameInstallPath;
                    }
                }
            }
            return gameInstallPath;
        }
    }
}
