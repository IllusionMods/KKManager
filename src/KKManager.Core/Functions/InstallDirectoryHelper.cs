using KKManager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.Util;

namespace KKManager.Functions
{
    public static class InstallDirectoryHelper
    {
        private static DirectoryInfo _gameDirectory;
        private static DirectoryInfo _pluginPath;
        private static DirectoryInfo _femaleCardDir;
        private static DirectoryInfo _maleCardDir;
        private static DirectoryInfo _zipmodsPath;
        private static DirectoryInfo _sardinesPath;
        private static DirectoryInfo _tempPath;
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
        public static DirectoryInfo ZipmodsPath
        {
            get
            {
                ThrowIfNotInitialized();
                return _zipmodsPath;
            }
            private set => _zipmodsPath = value;
        }
        public static DirectoryInfo SardinesPath
        {
            get
            {
                ThrowIfNotInitialized();
                return _sardinesPath;
            }
            private set => _sardinesPath = value;
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
        public static DirectoryInfo TempDir
        {
            get
            {
                ThrowIfNotInitialized();
                return _tempPath;
            }
            private set => _tempPath = value;
        }

        public static string ScreenshotDir => Path.Combine(GameDirectory.FullName, "UserData\\cap");
        public static string CardDir => Path.Combine(GameDirectory.FullName, "UserData\\chara");
        public static string SceneDir => Path.Combine(GameDirectory.FullName, "UserData\\Studio\\scene");

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
                new Tuple<string, GameType>("HoneySelect2.exe", GameType.HoneySelect2),
                new Tuple<string, GameType>("KoikatsuSunshine.exe", GameType.KoikatsuSunshine),
                new Tuple<string, GameType>("RoomGirl.exe", GameType.RoomGirl),
                new Tuple<string, GameType>("HoneyCome.exe", GameType.HoneyCome),
                new Tuple<string, GameType>("HoneyComeccp.exe", GameType.HoneyComeSteam),
                new Tuple<string, GameType>("SamabakeScramble.exe", GameType.SamabakeScramble),
                new Tuple<string, GameType>("Aicomi.exe", GameType.Aicomi),
            };

            GameType = gameCheck.FirstOrDefault(x => File.Exists(Path.Combine(path, x.Item1)))?.Item2 ?? GameType.Unknown;
            MaleCardDir = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, @"UserData\chara\male"));
            FemaleCardDir = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, @"UserData\chara\female"));
            ZipmodsPath = new DirectoryInfo(Path.Combine(GameDirectory.FullName, "mods"));
            SardinesPath = new DirectoryInfo(Path.Combine(GameDirectory.FullName, "sardines"));
            PluginPath = Directory.CreateDirectory(Path.Combine(GameDirectory.FullName, "BepInEx"));
            TempDir = new DirectoryInfo(Path.Combine(GameDirectory.FullName, "temp"));
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
                var abdataExist = File.Exists(Path.Combine(path, "abdata/abdata")) || File.Exists(Path.Combine(path, "abdata/sv_abdata")) || Directory.Exists(Path.Combine(path, "lib/chara"));

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
                case GameType.KoikatsuSunshine: return "KoikatsuSunshine";
                case GameType.RoomGirl: return "Room Girl";
                case GameType.HoneyCome: return "HoneyCome";
                case GameType.HoneyComeSteam: return "HoneyCome come come party";
                case GameType.SamabakeScramble: return "Summer Vacation Scramble";
                case GameType.Aicomi: return "Aicomi";
                default: throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }

        /// <summary>
        /// Figure out where the log file is written to and open it.
        /// </summary>
        public static void OpenLog()
        {
            try
            {
                var candidates = FindLogFiles(true);

                if (!candidates.Any())
                    throw new FileNotFoundException("No log files were found");

                foreach (var candidate in candidates.Take(3))
                    Process.Start(candidate.FullName);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(string.Format(Resources.OpenGameLogFailedMessage, exception.Message), Resources.OpenGameLogMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static readonly Dictionary<GameType, string> _AppdataRelativePaths = new Dictionary<GameType, string>
        {
            { GameType.Koikatsu , @"illusion__Koikatu\Koikatu"},
            { GameType.KoikatsuSteam , @"illusion__Koikatsu\Koikatsu Party"},
            { GameType.KoikatsuSunshine , @"illusion__KoikatsuSunshine\KoikatsuSunshine"},
            { GameType.EmotionCreators , @"illusion__EmotionCreators\EmotionCreators"},

            { GameType.AiShoujo ,@"illusion__AI-Syoujyo\AI-Syoujyo"},
            { GameType.AiShoujoSteam ,@"illusion__AI-Shoujo\AI-Shoujo"},
            { GameType.HoneySelect2 , @"illusion__HoneySelect2\HoneySelect2"}, //TODO: illusion__HoneySelect2_Steam\HoneySelect2_Steam
            
            { GameType.RoomGirl, @"illusion__RoomGirl\RoomGirl" },
            // VR_Kanojo ILLUSION\VR_Kanojo

            { GameType.HoneyCome ,@"ILLGAMES\HoneyCome"},
            { GameType.HoneyComeSteam ,@"ILLGAMES\HoneyComeccp"},
            { GameType.SamabakeScramble , @"ILLGAMES\SamabakeScramble"},
            { GameType.Aicomi, @"ILLGAMES\Aicomi" },
        };

        /// <summary>
        /// Finds log files for the current game.
        /// </summary>
        /// <param name="filter">Whether to filter the log files to only include the latest ones. If true, results are ordered by last write time (descending).</param>
        /// <returns>A list of log files for the current game.</returns>
        public static List<FileInfo> FindLogFiles(bool filter)
        {
            var candidates = new List<FileInfo>();
            // BepInEx 5.x log file, can be "LogOutput.log.1" or higher if multiple game instances run.
            candidates.AddRange(GameDirectory.GetFiles("LogOutput.log*", SearchOption.AllDirectories));
            // Unity built-in log file, by default inside _Data dir, disabled, or somewhere in appdata. Can be moved to game root by BepInEx preloader if configured.
            candidates.AddRange(GameDirectory.GetFiles("output_log.txt", SearchOption.AllDirectories));
            // Check for log files stored in AppData/LocalLow in case they were not redirected by preloader config
            if (_AppdataRelativePaths.TryGetValue(GameType, out var relativePath))
            {
                var localLow = PathTools.GetAppDataLocalLowPath();
                var appdataLogPath = Path.Combine(localLow, relativePath);
                // It can be either Player.log or output_log.txt depending on Unity version
                candidates.AddRange(Directory.GetFiles(appdataLogPath, "Player.log", SearchOption.AllDirectories).Select(x => new FileInfo(x)));
                candidates.AddRange(Directory.GetFiles(appdataLogPath, "output_log.txt", SearchOption.AllDirectories).Select(x => new FileInfo(x)));
            }

            if (filter)
            {
                candidates = candidates
                             // Only keep the latest log file for each name (e.g. in case output_log.txt is both in game root and in appdata)
                             .GroupBy(x => x.Name).Select(z => z.OrderByDescending(x => x.LastWriteTimeUtc).ThenByDescending(x => x.CreationTimeUtc).First())
                             // Order by last write time to have the most recent log on top (e.g. in case output_log.txt and LogOutput.log both exist)
                             .OrderByDescending(x => x.LastWriteTimeUtc).ThenByDescending(x => x.CreationTimeUtc)
                             .ToList();
            }

            return candidates;
        }
    }
}