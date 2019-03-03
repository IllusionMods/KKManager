using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KKManager.Plugins.Data;
using KKManager.Sideloader.Data;
using Mono.Cecil;

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
                    var dlls = Directory.GetFiles(modDir, "*.dll", SearchOption.AllDirectories);
                    
                    var plugins = new List<PluginInfo>();
                    foreach (var dll in dlls)
                    {
                        try
                        {
                            var md = ModuleDefinition.ReadModule(dll);
                            var attribs = md.Types
                                .Where(x => x.HasCustomAttributes && x.IsClass)
                                .SelectMany(x => x.CustomAttributes)
                                .Where(x => x.AttributeType.FullName == "BepInEx.BepInPlugin")
                                .ToList();

                            var location = new FileInfo(dll);
                            foreach (var attrib in attribs)
                            {
                                plugins.Add(new PluginInfo(
                                    attrib.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? location.Name,
                                    attrib.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString() ?? "Error while loading",
                                    attrib.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? "Error while loading",
                                    location));
                            }
                        }
                        catch { }
                    }

                    _plugins.OnNext(plugins);
                });

            Task.WaitAll(sideloaderTask, pluinsTask);

            _refreshRunning = false;
        }
    }
}