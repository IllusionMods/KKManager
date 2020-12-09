using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager;
using KKManager.Functions;
using KKManager.Updater;
using KKManager.Updater.Sources;
using KKManager.Updater.Windows;
using KKManager.Util;

namespace StandaloneUpdater
{
    internal static class Program
    {
        private static readonly string _exeLocation = typeof(Program).Assembly.Location;
        private static readonly string _exeDirectory = Path.GetDirectoryName(_exeLocation);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, arg) =>
            {
                Console.WriteLine("UNHANDLED EXCEPTION: " + arg.ExceptionObject);
                NBug.Handler.UnhandledException(sender, arg);
            };

            using (LogWriter.StartLogging())
            {
                var currentCulture = LanguageManager.CurrentCulture;
                CultureInfo.CurrentCulture = currentCulture;
                CultureInfo.CurrentUICulture = currentCulture;
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = currentCulture;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Console.WriteLine("Arguments: " + string.Join("   ", args));

                const string guidValueName = "-guid:";
                var silentInstallGuids = args.Where(x => x.StartsWith(guidValueName)).Select(x => x.Substring(guidValueName.Length).Trim(' ', '"')).ToArray();

                args = args.Where(x => !x.StartsWith("-")).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                if (args.Length == 0)
                {
                    ShowArgError("Not enough arguments - the following arguments are supported:\n" +
                                 "Path to game root directory. This is mandatory and has to be the 1st argument.\n" +
                                 "One or more links to update sources. If no update source links are provided then UpdateSources file is used.\n\n" +
                                 "You can also add -guid:UPDATE_GUID arguments to not show the mod selection screen, and instead automatically install mods with the specified GUIDs.");
                    return;
                }

                try
                {
                    var gameDir = new DirectoryInfo(args[0]);
                    if (!gameDir.Exists)
                        throw new IOException("Directory doesn't exist");
                    InstallDirectoryHelper.Initialize(gameDir);
                }
                catch (Exception e)
                {
                    ShowArgError($"Error opening game directory: {args[0]}\n\n{e}");
                    return;
                }

                if (SelfUpdater.CheckForUpdatesAndShowDialog().Result == true)
                    return;

                var updateSources = GetUpdateSources(args.Skip(1).ToArray());

                if (updateSources == null || !updateSources.Any())
                    return;

                var window = ModUpdateProgressDialog.CreateUpdateDialog(updateSources, silentInstallGuids);
                window.Icon = Icon.ExtractAssociatedIcon(typeof(Program).Assembly.Location);
                Application.Run(window);
            }
        }

        private static UpdateSourceBase[] GetUpdateSources(string[] sourceArgs)
        {
            if (sourceArgs.Length > 0)
                return UpdateSourceManager.GetUpdateSources(sourceArgs);

            var updateSources = UpdateSourceManager.FindUpdateSources(_exeDirectory);
            if (updateSources == null || updateSources.Length == 0)
                ShowArgError("No links to update sources have been provided in arguments and the UpdateSources file is missing or has no valid sources.\n\n" +
                             "Add one or more links to update sources as arguments or edit the UpdateSources file.");
            return updateSources;
        }

        private static void ShowArgError(string msg)
        {
            Console.WriteLine(msg.Replace("\n\n", "\n"));
            MessageBox.Show(msg, "Invalid arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
