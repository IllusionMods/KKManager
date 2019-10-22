using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using Mono.Cecil;

namespace KKManager.Data.Plugins
{
    internal static class PluginLoader
    {
        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="pluginDirectory">Directory containing the plugins to gather info from. Usually BepInEx directory inside game root.</param>
        /// <param name="cancellationToken">Token used to abort the search</param>
        public static IObservable<PluginInfo> TryLoadPlugins(string pluginDirectory, CancellationToken cancellationToken)
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
                    var files = Directory.EnumerateFiles(pluginDirectory, "*.*", SearchOption.TopDirectoryOnly);

                    var bep5PluginsDir = Path.Combine(pluginDirectory, "plugins");
                    if (Directory.Exists(bep5PluginsDir))
                        files = files.Concat(Directory.EnumerateFiles(bep5PluginsDir, "*.*", SearchOption.AllDirectories));

                    foreach (var file in files)
                    {
                        try
                        {
                            var ext = Path.GetExtension(file);
                            if (!IsValidPluginExtension(ext)) continue;

                            foreach (var pluginInfo in LoadFromFile(file))
                                subject.OnNext(pluginInfo);
                        }
                        catch (SystemException ex)
                        {
                            Console.WriteLine(ex);
                        }

                        if (cancellationToken.IsCancellationRequested) break;
                    }

                    subject.OnCompleted();
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Could not load information about plugins because access to the plugins folder was denied. Check the permissions of your plugins folder and try again.\n\n" + ex.Message,
                        "Load plugins", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    Console.WriteLine(ex);
                    subject.OnError(ex);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show("Could not load information about plugins because access to the plugins folder was denied. Check the permissions of your plugins folder and try again.\n\n" + ex.Message,
                        "Load plugins", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    Console.WriteLine(ex);
                    subject.OnError(ex);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    subject.OnError(ex);
                }
            }

            try { Task.Run((Action)ReadPluginsAsync, cancellationToken); }
            catch (TaskCanceledException) { }

            return subject;
        }

        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">Access to fileName is denied. </exception>
        public static IEnumerable<PluginInfo> LoadFromFile(string dllFile)
        {
            var location = new FileInfo(dllFile);

            using (var md = ModuleDefinition.ReadModule(dllFile, new ReaderParameters { AssemblyResolver = GetResolver(location) }))
            {
                var assRefs = md.AssemblyReferences.Select(x => x.FullName).ToArray();
                var classes = md.Types.Where(x => x.HasCustomAttributes && x.IsClass).ToList();

                foreach (var c in classes)
                {
                    var bp = c.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "BepInEx.BepInPlugin");
                    if (bp == null) continue;

                    var depAttributes = c.CustomAttributes
                        .Where(x => x.AttributeType.FullName == "BepInEx.BepInDependency");
                    var deps = depAttributes
                        .Select(x => x.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();

                    yield return new PluginInfo(
                        bp.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? location.Name,
                        bp.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString() ?? "Error while loading",
                        bp.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? "Error while loading",
                        location,
                        deps,
                        assRefs);
                }
            }
        }

        public static bool IsValidPluginExtension(string extension)
        {
            return extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) || extension.Equals(".dl_", StringComparison.OrdinalIgnoreCase);
        }

        private static DefaultAssemblyResolver GetResolver(FileInfo location)
        {
            var resolver = new DefaultAssemblyResolver();
            if (location.Directory != null)
            {
                var coreDir = Path.Combine(InstallDirectoryHelper.GetPluginPath(), "core");
                if (Directory.Exists(coreDir))
                    resolver.AddSearchDirectory(coreDir);
            }

            return resolver;
        }

        /// <summary>
        /// Check which of the <see cref="referencesToTest"/> depend on the supplied <see cref="plugins"/>
        /// </summary>
        /// <param name="plugins">Plugins to be tested against</param>
        /// <param name="referencesToTest">Plugins to check if they depended on the target plugin list</param>
        /// <returns>Filtered list of references (only what the plugins reference)</returns>
        public static IEnumerable<PluginInfo> CheckReferences(IList<PluginInfo> plugins, IList<PluginInfo> referencesToTest)
        {
            var deps = referencesToTest.SelectMany(reference => reference.Dependancies.Select(d => new { reference, depGuid = d.ToLower() }));
            var existingDeps = deps.Join(plugins, s => s.depGuid, info => info.Guid.ToLower(), (s, info) => s.reference);

            var refs = referencesToTest.SelectMany(reference => reference.GetAssemblyReferencesNames().Select(d => new { reference, dllName = d.ToLower() }));
            var existingRefs = refs.Join(plugins, s => s.dllName, info => info.FileNameWithoutExtension.ToLower(), (s, info) => s.reference);

            var topLevelDependancies = existingDeps.Concat(existingRefs).Distinct();

            return topLevelDependancies;
        }

        /// <summary>
        /// Check the supplied <see cref="plugins"/> for being dependent on other plugins in <see cref="dependenciesToTest"/>
        /// </summary>
        /// <param name="plugins">Plugins to be tested for depending on others</param>
        /// <param name="dependenciesToTest">Plugins to check if they are depended on by the target plugin list</param>
        /// <returns>Filtered list of references (only what the plugins reference)</returns>
        public static IEnumerable<PluginInfo> CheckDependencies(IList<PluginInfo> plugins, IList<PluginInfo> dependenciesToTest)
        {
            var deps = plugins.SelectMany(reference => reference.Dependancies.Select(d => new { reference, depGuid = d.ToLower() }));
            var existingDeps = deps.Join(dependenciesToTest, s => s.depGuid, info => info.Guid.ToLower(), (s, info) => info);

            var refs = plugins.SelectMany(reference => reference.GetAssemblyReferencesNames().Select(d => new { reference, dllName = d.ToLower() }));
            var existingRefs = refs.Join(dependenciesToTest, s => s.dllName, info => info.FileNameWithoutExtension.ToLower(), (s, info) => info);

            var topLevelDependancies = existingDeps.Concat(existingRefs).Distinct();

            return topLevelDependancies;
        }
    }
}