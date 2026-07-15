using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Data.Cards;
using KKManager.Data.ExtData.Deserializers;
using KKManager.Data.Zipmods;
using MessagePack;

namespace KKManager.Data.Scenes
{
    public static class SceneLoader
    {
        private static readonly byte[] SideloaderKey = System.Text.Encoding.UTF8.GetBytes(SideloaderExtDataDeserializer.UARExtID);

        public static IObservable<Scene> ReadScenes(DirectoryInfo path, SearchOption searchOption, CancellationToken token)
        {
            var result = new ReplaySubject<Scene>();
            if (!path.Exists) { result.OnCompleted(); return result; }

            var readTask = Task.Run(() =>
            {
                var scenes = new List<Scene>();
                try
                {
                    Parallel.ForEach(path.EnumerateFiles("*.png", searchOption), new ParallelOptions { CancellationToken = token }, file =>
                    {
                        if (TryParseScene(file, out var scene)) lock (scenes) scenes.Add(scene);
                    });
                }
                catch (OperationCanceledException) { }
                catch (Exception ex) { Console.WriteLine("Failed to load scenes: " + ex); }
                return scenes;
            }, token);

            Task.WhenAll(readTask, SideloaderModLoader.Zipmods.ToTask(token)).ContinueWith(_ =>
            {
                try
                {
                    var installed = ZipmodRequirementChecker.GetInstalledGuids(SideloaderModLoader.Zipmods.ToEnumerable());
                    foreach (var scene in readTask.Result)
                    {
                        scene.MissingZipmods = ZipmodRequirementChecker.GetMissing(scene.UsedZipmods, installed);
                        result.OnNext(scene);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex) { Console.WriteLine("Failed to check scene zipmods: " + ex); }
                finally { result.OnCompleted(); }
            }, token);

            return result;
        }

        public static bool TryParseScene(FileInfo file, out Scene scene)
        {
            scene = null;
            try
            {
                var bytes = File.ReadAllBytes(file.FullName);
                var guids = ReadUniversalAutoResolverGuids(bytes);
                scene = new Scene(file, guids);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse scene [{file.FullName}]: {ex.Message}");
                return false;
            }
        }

        internal static string[] ReadUniversalAutoResolverGuids(byte[] bytes)
        {
            var keyOffset = Find(bytes, SideloaderKey);
            if (keyOffset < 0) return Array.Empty<string>();

            // The ExtendedSaveFormat map stores plugin data directly after its MessagePack string key.
            // Deserialize only this value; MessagePack accepts the remaining scene data after it.
            var valueOffset = keyOffset + SideloaderKey.Length;
            var data = MessagePackSerializer.Deserialize<PluginData>(new ReadOnlyMemory<byte>(bytes, valueOffset, bytes.Length - valueOffset));
            if (data?.data == null || !data.data.TryGetValue("info", out var infos) || !(infos is IEnumerable<object> rawInfos))
                return Array.Empty<string>();

            var guids = new List<string>();
            foreach (var raw in rawInfos.OfType<byte[]>())
            {
                var info = MessagePackSerializer.Deserialize<SideloaderExtDataDeserializer.ResolveInfo>(raw);
                if (!string.IsNullOrWhiteSpace(info?.GUID)) guids.Add(info.GUID);
            }
            return guids.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private static int Find(byte[] haystack, byte[] needle)
        {
            for (var i = 0; i <= haystack.Length - needle.Length; i++)
            {
                var found = true;
                for (var j = 0; j < needle.Length; j++)
                    if (haystack[i + j] != needle[j]) { found = false; break; }
                if (found) return i;
            }
            return -1;
        }
    }
}
