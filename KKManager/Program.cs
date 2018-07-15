using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace KKManager
{
    internal static class Program
    {
        static Program()
        {
            KoikatuDirectory = GetKoikatuDirectory();
        }

        public static DirectoryInfo KoikatuDirectory { get; }

        private static DirectoryInfo GetKoikatuDirectory()
        {
            string path = null;
            try
            {
                path = Registry.CurrentUser.OpenSubKey(@"Software\Illusion\Koikatu\koikatu")
                    ?.GetValue("INSTALLDIR") as string;
            }
            catch (SystemException ex)
            {
                MessageBox.Show("Failed to get Koikatu install directory with an error: " + ex.Message,
                    "Failed to get Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (!Directory.Exists(path))
            {
                MessageBox.Show("Koikatu is either not registered properly or its install directory is missing or inaccessible.",
                    "Failed to get Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            return new DirectoryInfo(path);
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}