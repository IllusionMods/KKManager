using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Mono.Cecil;

namespace KKManager.Plugins.Data
{
    internal static class PluginLoader
    {
        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="pluginDirectory">Directory containing the plugins to gather info from. Usually BepInEx directory inside game root.</param>
        /// <param name="cancellationToken">Token used to abort the search</param>
        /// <param name="searchOption">Where to search</param>
        public static IObservable<PluginInfo> TryLoadPlugins(string pluginDirectory, CancellationToken cancellationToken, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var subject = new ReplaySubject<PluginInfo>();

            if (!Directory.Exists(pluginDirectory) || cancellationToken.IsCancellationRequested)
            {
                subject.OnCompleted();
                return subject;
            }

            void ReadPluginsAsync()
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(pluginDirectory, "*.dll", searchOption))
                    {
                        try
                        {
                            foreach (var pluginInfo in LoadFromFile(file))
                                subject.OnNext(pluginInfo);
                        }
                        catch (SystemException ex)
                        {
                            Console.WriteLine(ex);
                            /*MessageBox.Show(
                                        $"Failed to load mod from \"{file}\" with error: {ex.Message}",
                                        "Load sideloader mods", MessageBoxButtons.OK, MessageBoxIcon.Warning);*/
                        }

                        if (cancellationToken.IsCancellationRequested) break;
                    }

                    subject.OnCompleted();
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    subject.OnError(ex);
                }
            }

            Task.Run((Action)ReadPluginsAsync, cancellationToken);

            return subject;
        }

        public static IEnumerable<PluginInfo> LoadFromFile(string dllFile)
        {
            using (var md = ModuleDefinition.ReadModule(dllFile))
            {
                var attribs = md.Types.Where(x => x.HasCustomAttributes && x.IsClass)
                    .SelectMany(x => x.CustomAttributes)
                    .Where(x => x.AttributeType.FullName == "BepInEx.BepInPlugin")
                    .ToList();

                var location = new FileInfo(dllFile);
                foreach (var attrib in attribs)
                {
                    yield return new PluginInfo(
                            attrib.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? location.Name,
                            attrib.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString() ?? "Error while loading",
                            attrib.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? "Error while loading",
                            location);
                }
            }
        }
    }
}