using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using KKManager.Plugins.Data;
using KKManager.Sideloader.Data;

namespace KKManager
{
    internal static class ModSearcher
    {
        private static readonly BehaviorSubject<IReadOnlyCollection<SideloaderModInfo>> _sideloaderMods;
        private static readonly BehaviorSubject<IReadOnlyCollection<PluginInfo>> _plugins;

        static ModSearcher()
        {
            _sideloaderMods = new BehaviorSubject<IReadOnlyCollection<SideloaderModInfo>>(new SideloaderModInfo[] { });
            _plugins = new BehaviorSubject<IReadOnlyCollection<PluginInfo>>(new PluginInfo[] { });
        }

        public static IObservable<IReadOnlyCollection<SideloaderModInfo>> SideloaderMods => _sideloaderMods.ObserveOn(Program.MainSc);
        public static IObservable<IReadOnlyCollection<PluginInfo>> Plugins => _plugins.ObserveOn(Program.MainSc);

        public static void StartModsRefresh()
        {
            lock (_sideloaderMods)
            {
                if (!_refreshRunning)
                {
                    _refreshRunning = true;
                    Task.Run((Action)RefreshMods);
                }
            }
        }

        private static bool _refreshRunning;

        private static void RefreshMods()
        {
            var sideloaderTask = Task.Run(
                () =>
                {
                    var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "mods");
                    var mods = SideloaderModLoader.TryReadSideloaderMods(modDir);

                    _sideloaderMods.OnNext(mods);
                });

            var pluinsTask = Task.Run(
                () =>
                {
                    var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "bepinex");
                    string dependancyDir = Path.Combine(Program.KoikatuDirectory.FullName, "Koikatu_Data");
                    string[] dependancies = Directory.GetFiles(dependancyDir, "*.dll", SearchOption.AllDirectories);
                    var dlls = Directory.GetFiles(modDir, "*.dll", SearchOption.AllDirectories);

                    foreach (var dll in dependancies)
                    {
                        try
                        {
                            Assembly.ReflectionOnlyLoadFrom(dll);
                        }
                        catch { }
                    }

                    var loadedAsses = new List<Assembly>();
                    foreach (var dll in dlls)
                    {
                        try
                        {
                            loadedAsses.Add(Assembly.ReflectionOnlyLoadFrom(dll));
                        }
                        catch { }
                    }

                    var plugins = new List<PluginInfo>();
                    foreach (var ass in loadedAsses)
                    {
                        try
                        {
                            // BUG: Assembly.ReflectionOnlyLoad Ignores Assembly Binding Redirects, plugins compiled against different version of BepInEx fail to load
                            // http://blogs.microsoft.co.il/sasha/2010/06/09/assemblyreflectiononlyload-ignores-assembly-binding-redirects/
                            var assAttribs = ass.GetTypes()
                                .Where(x => x.IsClass)
                                .SelectMany(x => x.CustomAttributes.Where(u => u.AttributeType.FullName == "BepInEx.BepInPlugin"))
                                .ToList();

                            if (assAttribs.Count == 0) continue;

                            FileInfo location = new FileInfo(ass.Location);

                            plugins.AddRange(assAttribs.Select(x => new PluginInfo(x.ConstructorArguments[1].Value.ToString(), x.ConstructorArguments[2].Value.ToString(), x.ConstructorArguments[0].Value.ToString(), location)));
                        }
                        catch
                        {
                            FileInfo location = new FileInfo(ass.Location);
                            var ver = FileVersionInfo.GetVersionInfo(location.FullName);
                            string version = ver.FileVersion ?? ver.ProductVersion;
                            if (version == "1.0.0.0") version = string.Empty;
                            plugins.Add(new PluginInfo(ver.ProductName ?? ver.FileName, version, "ERROR: Failed to load assembly", location));
                        }
                    }

                    _plugins.OnNext(plugins);
                });

            Task.WaitAll(sideloaderTask, pluinsTask);

            _refreshRunning = false;
        }
    }
}