using System;
using System.IO;

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

                var exeExist = File.Exists(Path.Combine(path, "Koikatu.exe")) ||
                               File.Exists(Path.Combine(path, "Koikatsu Party.exe")) ||
                               File.Exists(Path.Combine(path, "CharaStudio.exe"));

                // todo use this to offer to install bepinex and other mods / run update wizzard
                var modsExist = Directory.Exists(Path.Combine(path, "bepinex")) && Directory.Exists(Path.Combine(path, "mods"));

                if (!exeExist && !modsExist) return false;

                return true;
            }
            catch
            {
                return false;
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
