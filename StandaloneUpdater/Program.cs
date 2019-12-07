using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Functions.Update;
using KKManager.Util;
using KKManager.Windows;

namespace StandaloneUpdater
{
    internal static class Program
    {
        public static string ProgramLocation => Path.GetDirectoryName(typeof(Program).Assembly.Location);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, arg) => Console.WriteLine("UNHANDLED EXCEPTION: " + arg.ExceptionObject);
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;

            using (LogWriter.StartLogging())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var isSilent = args.Any(s => string.Equals(s, "-silent", StringComparison.OrdinalIgnoreCase));

                args = args.Where(x => !x.StartsWith("-")).ToArray();

                if (args.Length == 0)
                {
                    MessageBox.Show("Not enough arguments - the following arguments are required in this order:\nPath to game root directory\nOne or more links to update sources\n\nYou can also add -silent argument to not show the mod selection screen. If no update source links are provided then UpdateSources file is used.", "Invalid arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    var gameDir = new DirectoryInfo(args[0]);
                    if (!gameDir.Exists)
                        throw new IOException("Directory doesn't exist");
                    InstallDirectoryHelper.KoikatuDirectory = gameDir;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error opening game directory: {args[0]}\n\n{e}", "Invalid arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(e);
                    return;
                }

                var updateSources = GetUpdateSources(args.Skip(1).ToArray());

                if (updateSources == null || !updateSources.Any())
                    return;

                var window = ModUpdateProgressDialog.CreateUpdateDialog(updateSources);
                window.Silent = isSilent;
                Application.Run(window);
            }
        }

        private static IUpdateSource[] GetUpdateSources(string[] sourceArgs)
        {
            if (sourceArgs.Length == 0)
            {
                var updateSources = UpdateSourceManager.GetUpdateSources(ProgramLocation);
                if (updateSources == null || updateSources.Length == 0)
                    MessageBox.Show("No links to update sources have been provided in arguments and the UpdateSources file is missing or has no valid sources.\n\nAdd one or more links to update sources as arguments or edit the UpdateSources file.", "Invalid arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return updateSources;
            }

            var results = new List<IUpdateSource>();
            foreach (var source in sourceArgs)
            {
                try
                {
                    var link = new Uri(source);
                    results.Add(UpdateSourceManager.GetUpdater(link));
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error opening update source link: {source}\n\n{e}", "Invalid arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(e);
                    return null;
                }
            }
            return results.ToArray();
        }
    }
}
