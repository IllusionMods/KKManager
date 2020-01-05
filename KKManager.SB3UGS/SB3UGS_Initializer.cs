using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SB3Utility;

namespace KKManager.SB3UGS
{
    public class SB3UGS_Initializer
    {
        private static DirectoryInfo _directoryName;
        private static DirectoryInfo _pluginDirName;
        private static Dictionary<string, FileInfo> _sb3uDlls;

        private static void Initialize()
        {
            if (_pluginDirName != null && _pluginDirName.Exists) return;

            var assLocation = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            _directoryName = new DirectoryInfo(Path.Combine(assLocation.FullName, "SB3UGS"));
            _pluginDirName = new DirectoryInfo(Path.Combine(_directoryName.FullName, "plugins"));

            if (!_directoryName.Exists || !_pluginDirName.Exists)
            {
                try { _directoryName.Delete(true); }
                catch (Exception ex) { Console.WriteLine(ex); }

                Console.WriteLine("Could not find a valid SB3UGS install, attempting to install a fresh one");

                var target = assLocation.GetFiles("SB3UGS*.7z").Where(x => !x.Name.Contains("_src")).OrderByDescending(x => x.Length).FirstOrDefault();
                if (target != null)
                {
                    Console.WriteLine("Found " + target.Name + " - attempting to extract");

                    var extr = new SevenZipNET.SevenZipExtractor(target.FullName);

                    if (!extr.TestArchive())
                        throw new IOException("Archive " + target.Name + " is not valid or is corrupted");

                    extr.ExtractAll(_directoryName.FullName, true, true);

                    _directoryName.Refresh();
                    _pluginDirName.Refresh();
                    if (!_directoryName.Exists || !_pluginDirName.Exists)
                        throw new IOException("Archive " + target.Name + " did not contain the correct files or it failed to extract");
                }
            }

            _sb3uDlls = _pluginDirName.GetFiles("*.dll").Concat(_directoryName.GetFiles("*.dll")).ToDictionary(x => Path.GetFileNameWithoutExtension(x.Name), x => x);

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            LoadPlugin(Path.Combine(_pluginDirName.FullName, "SB3UtilityPlugins.dll"));
        }

        public static bool CheckIsAvailable()
        {
            if (_pluginDirName == null)
            {
                try
                {
                    Initialize();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return _sb3uDlls != null;
        }

        public static void LoadPlugin(string pluginName)
        {
            ThrowIfNotAvailable();

            PluginManager.LoadPlugin(Path.Combine(_pluginDirName.FullName, pluginName));
        }

        public static void ThrowIfNotAvailable()
        {
            if (!CheckIsAvailable())
                throw new IOException("SB3UGS is not available");
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            var name = new AssemblyName(e.Name).Name;
            if (_sb3uDlls.TryGetValue(name, out var file) && file.Exists)
                return Assembly.LoadFrom(file.FullName);
            return null;
        }
    }
}