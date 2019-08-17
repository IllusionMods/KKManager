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

        public static string ProgramLocation => Path.GetDirectoryName(typeof(Program).Assembly.Location);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Console.WriteLine("UNHANDLED EXCEPTION: " + args.ExceptionObject);

            using (LogWriter.StartLogging())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
                Settings.Default.Save();
            }
        }
    }
}
