using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using KKManager.Properties;
using KKManager.Util;
using KKManager.Windows;

namespace KKManager
{
    internal static class Program
    {
        public static SynchronizationContext MainSynchronizationContext { get; internal set; }

        private static string _assemblyLocation;
        public static string ProgramLocation => Path.GetDirectoryName(_assemblyLocation);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            _assemblyLocation = typeof(Program).Assembly.Location;
            using (SimpleFileLogger.SetupLogging(_assemblyLocation))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
                Settings.Default.Save();
            }
        }
    }
}
