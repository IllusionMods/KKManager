using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KKManager.Sideloader.Data;

namespace KKManager
{
    internal static class ModSearcher
    {
        private static readonly BehaviorSubject<IReadOnlyCollection<SideloaderMod>> _sideloaderMods;

        static ModSearcher()
        {
            _sideloaderMods = new BehaviorSubject<IReadOnlyCollection<SideloaderMod>>(new SideloaderMod[] { });
        }

        public static IObservable<IReadOnlyCollection<SideloaderMod>> SideloaderMods => _sideloaderMods.ObserveOn(Program.MainSc);

        public static void StartSideloaderModsRefresh()
        {
            lock (_sideloaderMods)
            {
                if (!_refreshRunning)
                {
                    _refreshRunning = true;
                    Task.Run((Action)ReloadSideloaderMods);
                }
            }
        }

        private static bool _refreshRunning;

        private static void ReloadSideloaderMods()
        {
            var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "mods");
            var mods = SideloaderModLoader.TryReadSideloaderMods(modDir);

            _sideloaderMods.OnNext(mods);
            _refreshRunning = false;
        }
    }
}