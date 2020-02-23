using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using KKManager.Util;
using SB3Utility;
using SharpCompress.Archives;

namespace KKManager.SB3UGS
{
    public class SB3UGS_Initializer
    {
        private static DirectoryInfo _pluginDirName;
        private static Dictionary<string, FileInfo> _sb3uDlls;

        private static void Initialize()
        {
            if (_pluginDirName != null) return;

            var assLocation = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var directoryName = new DirectoryInfo(Path.Combine(assLocation.FullName, "SB3UGS"));
            _pluginDirName = new DirectoryInfo(Path.Combine(directoryName.FullName, "plugins"));

            // bug this can sometimes fail to extract or to load the dlls
            if (!_pluginDirName.Exists)
            {
                if (directoryName.Exists)
                    directoryName.Delete(true);

                Console.WriteLine("Could not find a valid SB3UGS install, attempting to install a fresh one");

                var target = assLocation.GetFiles("SB3UGS*.7z").Where(x => !x.Name.Contains("_src")).OrderByDescending(x => x.Length).FirstOrDefault();
                if (target != null)
                {
                    Console.WriteLine("Found " + target.Name + " - attempting to extract");

                    var extr = ArchiveFactory.Open(target);

                    if (!extr.IsComplete)
                        throw new IOException("Archive " + target.Name + " is not valid or is corrupted");

                    directoryName.Create();
                    extr.ExtractArchiveToDirectory(directoryName.FullName);

                    _pluginDirName.Refresh();
                    if (!_pluginDirName.Exists)
                        throw new IOException("Archive " + target.Name + " did not contain the correct files or it failed to extract");
                }
            }

            _sb3uDlls = _pluginDirName.GetFiles("*.dll").Concat(directoryName.GetFiles("*.dll")).ToDictionary(x => Path.GetFileNameWithoutExtension(x.Name), x => x);

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
                throw new NotSupportedException("SB3UGS is not available");
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