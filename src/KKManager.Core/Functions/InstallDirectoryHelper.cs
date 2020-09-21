using System;
using System.IO;
using System.Linq;

namespace KKManager.Functions
{
    public static class InstallDirectoryHelper
    {
        private static DirectoryInfo _gameDirectory;
        private static DirectoryInfo _pluginPath;
        private static DirectoryInfo _femaleCardDir;
        private static DirectoryInfo _maleCardDir;
        private static DirectoryInfo _modsPath;
        public static DirectoryInfo GameDirectory
        {
            get
            {
                ThrowIfNotInitialized();
                return _gameDirectory;
            }
            private set => _gameDirectory = value;
        }
        public static DirectoryInfo PluginPath
        {
            get
            {
                ThrowIfNotInitialized();
                return _pluginPath;
            }
            private set => _pluginPath = value;
        }
        public static DirectoryInfo ModsPath
        {
            get
            {
                ThrowIfNotInitialized();
                return _modsPath;
            }
            private set => _modsPath = value;
        }
        public static DirectoryInfo MaleCardDir
        {
            get
            {
                ThrowIfNotInitialized();
                return _maleCardDir;
            }
            private set => _maleCardDir = value;
        }
        public static DirectoryInfo FemaleCardDir
        {
            get
            {
                ThrowIfNotInitialized();
                return _femaleCardDir;
            }
            private set => _femaleCardDir = value;
        }

        public static GameType GameType { get; private set; } = GameType.Unknown;

        public static void Initialize(DirectoryInfo gameDirectory)
        {
            GameDirectory = gameDirectory ?? throw new ArgumentNullException(nameof(gameDirectory));

            var path = GameDirectory.FullName;
            var gameCheck = new[]
            {
                new Tuple<string, GameType>("AI-Syoujyo.exe", GameType.AiShoujo),
                new Tuple<string, GameType>("AI-Shoujo.exe", GameType.AiShoujoSteam),
                new Tuple<string, GameType>("Koikatu.exe", GameType.Koikatsu),
                new Tuple<string, GameType>("Koikatsu Party.exe", GameType.KoikatsuSteam),
                new Tuple<string, GameType>("EmotionCreators.exe", GameType.EmotionCreators),
                new Tuple<string, GameType>("PlayHome64bit.exe", GameType.PlayHome),
                new Tuple<string, GameType>("HoneySelect2.exe", GameType.HoneySelect2)
            };

            GameType = gameCheck.FirstOrDefault(x => File.Exists(Path.Combine(path, x.Item1)))?.Item2 ?? GameType.Unknown;
            MaleCardDir = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, @"UserData\chara\male"));
            FemaleCardDir = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, @"UserData\chara\female"));
            ModsPath = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, "mods"));
            PluginPath = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, "BepInEx"));
        }

        /// <summary>
        /// Get relative path based on game root directory
        /// </summary>
        public static string GetRelativePath(string fullPath)
        {
            ThrowIfNotInitialized();
            var gamePath = GameDirectory.FullName;
            return fullPath.StartsWith(gamePath, StringComparison.OrdinalIgnoreCase)
                ? fullPath.Substring(gamePath.Length)
                : fullPath;
        }

        private static void ThrowIfNotInitialized()
        {
            if (_gameDirectory == null) throw new InvalidOperationException("Game directory was not initialized yet");
        }

        /// <summary>
        /// Get relative path based on game root directory
        /// </summary>
        public static string GetRelativePath(FileSystemInfo fullPath)
        {
            return GetRelativePath(fullPath.FullName);
        }

        public static bool IsValidGamePath(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;

                //var exeExist = File.Exists(Path.Combine(path, "Koikatu.exe")) ||
                //               File.Exists(Path.Combine(path, "Koikatsu Party.exe")) ||
                //               File.Exists(Path.Combine(path, "CharaStudio.exe"));

                var anyDatas = Directory.GetDirectories(path)
                    .Any(x => x.EndsWith("_Data", StringComparison.OrdinalIgnoreCase));
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
    }
}