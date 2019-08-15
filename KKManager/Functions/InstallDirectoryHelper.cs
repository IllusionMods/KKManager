using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using KKManager.Properties;
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
                if(!IsValidGamePath(value.FullName)) throw new ArgumentException("The directory is not a valid install directory");
                _koikatuDirectory = value;
            }
        }

        public static string GetPluginPath()
        {
            return Path.Combine(KoikatuDirectory.FullName, "BepInEx");
        }

        public static string GetModsPath()
        {
            return Path.Combine(KoikatuDirectory.FullName, "mods");
        }

        public static bool IsValidGamePath(string path)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(path) && (File.Exists(Path.Combine(path, "Koikatu.exe")) || Directory.Exists(Path.Combine(path, "bepinex")));
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

            if (!IsValidGamePath(path))
            {
                MessageBox.Show(
                    "Koikatu is either not registered properly or its install directory is missing or inaccessible.",
                    "Failed to get Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Debug.Assert(path != null, nameof(path) + " != null");
            return new DirectoryInfo(path);
        }
    }
}
