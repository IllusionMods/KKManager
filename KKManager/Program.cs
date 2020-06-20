using System;
using System.Globalization;
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
        public static LogWriter Logger { get; private set; }
        public static SynchronizationContext MainSynchronizationContext { get; internal set; }

        public static string ProgramLocation => Path.GetDirectoryName(typeof(Program).Assembly.Location);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Console.WriteLine("UNHANDLED EXCEPTION: " + args.ExceptionObject);
                NBug.Handler.UnhandledException(sender, args);
            };

            Logger = LogWriter.StartLogging();
            using (Logger)
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
                Settings.Default.Save();
            }
        }
    }
}
