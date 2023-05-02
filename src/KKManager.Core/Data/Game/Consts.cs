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
            { "HoneySelect2", @"SOFTWARE\illusion\HoneySelect2\HoneySelect2" },
            { "AI-Syoujyo", @"SOFTWARE\illusion\AI-Syoujyo\AI-Syoujyo" },
            { "Koikatu", @"SOFTWARE\illusion\Koikatu\koikatu" },
            { "KoikatsuSunshine", @"SOFTWARE\illusion\KoikatsuSunshine\KoikatsuSunshine" },
            { "EmotionCreations", @"SOFTWARE\illusion\TBD\TBD" }, // todo
            { "RoomGirl", @"SOFTWARE\illusion\TBD\TBD" }, // todo
            { "PlayHome", @"SOFTWARE\illusion\TBD\TBD" } // todo
        };

        public static Dictionary<string, GameName> gameNameByChunkDict = new Dictionary<string, GameName>
        {
            { "\u3010AIS_Chara\u3011", new GameName("HS2") },
            { "\u3010HoneySelectCharaFemale\u3011", new GameName("HS1") }, // todo
            { "\u3010KoiKatuChara\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaS\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaSP\u3011", new GameName("KK") },
            { "\u3010KoiKatuCharaSun\u3011", new GameName("KKS") },
            { "\u3010KStudio\u3011", new GameName("KK") }, // todo
            { "\u3010EroMakeChara\u3011", new GameName("EC") },
            { "\u3010RG_Chara\u3011", new GameName("RG") } // todo
        };

        public static string GetInstallPath(GameName gameName)
        {
            if (registryPathMapping.TryGetValue(gameName.LongName, out string registryPath))
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(registryPath))
                {
                    if (key != null && key.GetValue("INSTALLDIR") is string installPath && Directory.Exists(installPath))
                    {
                        return installPath;
                    }
                }
            }
            return "";
        }

    }
}
