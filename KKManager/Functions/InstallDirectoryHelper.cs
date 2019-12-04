using System;
using System.IO;
using System.Windows.Forms;
using KKManager.Properties;
using KKManager.Util;
using Microsoft.Win32;

namespace KKManager.Functions
{
    internal static class InstallDirectoryHelper
    {
        private static DirectoryInfo _koikatuDirectory;

        public static DirectoryInfo KoikatuDirectory
        {
            get => _koikatuDirectory ?? (_koikatuDirectory = GetKoikatuDirectory());
            set
            {
                if (!IsValidGamePath(value.FullName)) throw new ArgumentException("The directory is not a valid install directory");
                _koikatuDirectory = value;
            }
        }

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

        private static DirectoryInfo GetKoikatuDirectory()
        {
            var path = Settings.Default.GamePath;
            if (!IsValidGamePath(path))
            {
                try
                {
                    path = Registry.CurrentUser.OpenSubKey(@"Software\Illusion\Koikatu\koikatu")
                        ?.GetValue("INSTALLDIR") as string;
                }
                catch (SystemException) { }

                if (!IsValidGamePath(path))
                {
                    MessageBox.Show(
                        "Koikatu is either not registered properly or its install directory is missing or inaccessible.\n\nYou will have to select game directory manually.",
                        "Failed to find Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    var fb = new FolderBrowserDialog { Description = "Select Koikatsu! game directory" };
                    retryFolderSelect:
                    if (fb.ShowDialog() == DialogResult.OK)
                    {
                        path = fb.SelectedPath;
                        if (!IsValidGamePath(path))
                        {
                            if (MessageBox.Show(
                                    "The selected directory doesn't seem to contain the game. Make sure the directory is correct and try again.",
                                    "Failed to find Koikatu install directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                                goto retryFolderSelect;
                        }
                    }
                }

                Settings.Default.GamePath = path;
            }

            if (string.IsNullOrEmpty(path) || !IsValidGamePath(path))
            {
                MessageBox.Show(
                    "Koikatu is either not registered properly or its install directory is missing or inaccessible.",
                    "Failed to get Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (!PathTools.DirectoryHasWritePermission(path) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "mods")) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "userdata")))
            {
                if (MessageBox.Show("KK Manager doesn't have write permissions to the game directory! This can cause issues for both KK Manager and the game itself.\n\nDo you want KK Manager to fix permissions of the entire Koikatu folder?",
                        "No write access to game directory", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ProcessTools.FixPermissions(path)?.WaitForExit();
            }

            return new DirectoryInfo(path);
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
