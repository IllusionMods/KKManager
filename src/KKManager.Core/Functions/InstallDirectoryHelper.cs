using System;
using System.IO;
using System.Linq;

namespace KKManager.Functions
{
    public static class InstallDirectoryHelper
    {
        public static DirectoryInfo KoikatuDirectory { get; set; }

        public static string GetRelativePath(string fullPath)
        {
            return fullPath.Substring(KoikatuDirectory.FullName.Length);
        }
        public static string GetRelativePath(FileSystemInfo fullPath)
        {
            return GetRelativePath(fullPath.FullName);
        }

        public static string GetPluginPath()
        {
            return Path.Combine(KoikatuDirectory.FullName, "BepInEx");
        }

        public static DirectoryInfo GetModsPath()
        {
            var path = Path.Combine(KoikatuDirectory.FullName, "mods");
            return Directory.CreateDirectory(path);
        }

        public static bool IsValidGamePath(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;

                //var exeExist = File.Exists(Path.Combine(path, "Koikatu.exe")) ||
                //               File.Exists(Path.Combine(path, "Koikatsu Party.exe")) ||
                //               File.Exists(Path.Combine(path, "CharaStudio.exe"));

                var anyDatas = Directory.GetDirectories(path).Any(x => x.EndsWith("_Data", StringComparison.OrdinalIgnoreCase));
                var abdataExist = File.Exists(Path.Combine(path, "abdata/abdata"));

                // todo use this to offer to install bepinex and other mods / run update wizzard
                //var modsExist = Directory.Exists(Path.Combine(path, "bepinex")) && Directory.Exists(Path.Combine(path, "mods"));

                return anyDatas && abdataExist;
            }
            catch
            {
                return false;
            }
        }

        public static GameType GetGameType()
        {
            var path = KoikatuDirectory.FullName;
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (File.Exists(Path.Combine(path, "AI-Syoujyo.exe"))) return GameType.AiShoujo;
                if (File.Exists(Path.Combine(path, "AI-Shoujo.exe"))) return GameType.AiShoujoSteam;
                if (File.Exists(Path.Combine(path, "Koikatu.exe"))) return GameType.Koikatsu;
                if (File.Exists(Path.Combine(path, "Koikatsu Party.exe"))) return GameType.KoikatsuSteam;
                if (File.Exists(Path.Combine(path, "EmotionCreators.exe"))) return GameType.EmotionCreators;
                if (File.Exists(Path.Combine(path, "PlayHome64bit.exe"))) return GameType.PlayHome;
                if (File.Exists(Path.Combine(path, "HoneySelect2.exe"))) return GameType.HoneySelect2;
            }
            return GameType.Unknown;
        }

        public static string GetFancyGameName(this GameType gameType)
        {
            switch (gameType)
            {
                case GameType.Unknown: return "UNKNOWN GAME";
                case GameType.Koikatsu: return "Koikatu!";
                case GameType.KoikatsuSteam: return "Koikatsu Party";
                case GameType.AiShoujo: return "AI-Syoujyo";
                case GameType.AiShoujoSteam: return "AI-Shoujo";
                case GameType.PlayHome: return "PlayHome";
                case GameType.EmotionCreators: return "EmotionCreators";
                case GameType.HoneySelect2: return "HoneySelect2";
                default: throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }

        public static DirectoryInfo GetMaleCardDir()
        {
            var path = Path.Combine(KoikatuDirectory.FullName, @"UserData\chara\male");
            return Directory.CreateDirectory(path);
        }

        public static DirectoryInfo GetFemaleCardDir()
        {
            var path = Path.Combine(KoikatuDirectory.FullName, @"UserData\chara\female");
            return Directory.CreateDirectory(path);
        }
    }
}
