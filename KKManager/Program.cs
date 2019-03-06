using System;
using System.Threading;
using System.Windows.Forms;
using KKManager.Properties;
using KKManager.Util;
using KKManager.Windows;

namespace KKManager
{
    internal static class Program
    {
        public static SynchronizationContext MainSynchronizationContext { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (SimpleFileLogger.SetupLogging(typeof(Program).Assembly.Location))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
                Settings.Default.Save();
            }
        }
    }
}
