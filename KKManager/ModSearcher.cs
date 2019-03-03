using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KKManager.Plugins.Data;
using KKManager.Sideloader.Data;

namespace KKManager
{
    internal static class ModSearcher
    {
        private static readonly BehaviorSubject<IReadOnlyCollection<PluginInfo>> _plugins;
        private static readonly BehaviorSubject<IReadOnlyCollection<SideloaderModInfo>> _sideloaderMods;

        private static bool _refreshRunning;

        static ModSearcher()
        {
            _sideloaderMods = new BehaviorSubject<IReadOnlyCollection<SideloaderModInfo>>(new SideloaderModInfo[] { });
            _plugins = new BehaviorSubject<IReadOnlyCollection<PluginInfo>>(new PluginInfo[] { });
        }

        public static IObservable<IReadOnlyCollection<PluginInfo>> Plugins => _plugins.ObserveOn(Program.MainSynchronizationContext);

        public static IObservable<IReadOnlyCollection<SideloaderModInfo>> SideloaderMods => _sideloaderMods.ObserveOn(Program.MainSynchronizationContext);

        public static void StartModsRefresh()
        {
            lock (_sideloaderMods)
            {
                if (!_refreshRunning)
                {
                    _refreshRunning = true;
                    Task.Run((Action) RefreshMods);
                }
            }
        }

        private static void LoadPlugins()
        {
            var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "bepinex");
            var plugins = PluginLoader.TryLoadPlugins(modDir);

            _plugins.OnNext(plugins);
        }

        private static void LoadSideloaderMods()
        {
            var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "mods");
            var mods = SideloaderModLoader.TryReadSideloaderMods(modDir);

            _sideloaderMods.OnNext(mods);
        }

        private static void RefreshMods()
        {
            var sideloaderTask = Task.Run((Action) LoadSideloaderMods);
            var pluinsTask = Task.Run((Action) LoadPlugins);

            Task.WaitAll(sideloaderTask, pluinsTask);

            _refreshRunning = false;
        }
    }
}
