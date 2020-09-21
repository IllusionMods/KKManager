using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using Mono.Cecil;

namespace KKManager.Data.Plugins
{
    public static class PluginLoader
    {
        public static IObservable<PluginInfo> Plugins
        {
            get => _plugins ?? StartReload();
            private set => _plugins = value;
        }

        private static bool _isUpdating = false;
        private static readonly object _lock = new object();
        private static IObservable<PluginInfo> _plugins;

        public static IObservable<PluginInfo> StartReload()
        {
            lock (_lock)
            {
                if (!_isUpdating) Plugins = TryLoadPlugins(InstallDirectoryHelper.PluginPath.FullName);
                return Plugins;
            }
        }

        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="pluginDirectory">Directory containing the plugins to gather info from. Usually BepInEx directory inside game root.</param>
        private static IObservable<PluginInfo> TryLoadPlugins(string pluginDirectory)
        {
            _isUpdating = true;

            var subject = new ReplaySubject<PluginInfo>();

            if (!Directory.Exists(pluginDirectory))
            {
                subject.OnCompleted();
                _isUpdating = false;
                Console.WriteLine("No plugin folder detected");
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

                    var configDir = new DirectoryInfo(Path.Combine(pluginDirectory, "config"));
                    var configFiles = configDir.Exists ? configDir.GetFiles("*.cfg", SearchOption.TopDirectoryOnly) : new FileInfo[0];

                    foreach (var file in files)
                    {
                        try
                        {
                            var ext = Path.GetExtension(file);
                            if (!IsValidPluginExtension(ext)) continue;

                            foreach (var pluginInfo in LoadFromFile(file, configFiles))
                                subject.OnNext(pluginInfo);
                        }
                        catch (SystemException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }

                    subject.OnCompleted();
                    Console.WriteLine("Finished loading plugins");
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
                finally
                {
                    _isUpdating = false;
                }
            }

            try { Task.Run(ReadPluginsAsync); }
            catch (OperationCanceledException) { }

            return subject;
        }

        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">Access to fileName is denied. </exception>
        public static IEnumerable<PluginInfo> LoadFromFile(string dllFile, FileInfo[] configFiles = null)
        {
            var location = new FileInfo(dllFile);

            using (var md = ModuleDefinition.ReadModule(dllFile, new ReaderParameters { AssemblyResolver = GetResolver(location) }))
            {
                var classes = md.Types.Where(x => x.HasCustomAttributes && x.IsClass).ToList();

                var pluginClasses = classes.Select(c => new
                {
                    c,
                    bp = c.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "BepInEx.BepInPlugin")
                }).Where(x => x.bp != null).ToList();

                if (pluginClasses.Count == 0) yield break;

                var assRefs = md.AssemblyReferences.Select(x => x.FullName).ToArray();

                var f = FileVersionInfo.GetVersionInfo(dllFile);
                var author = f.CompanyName;
                var description = f.Comments;
                var fileUrl = new[] { f.CompanyName, f.FileDescription, f.Comments, f.LegalCopyright, f.LegalTrademarks }.FirstOrDefault(x => x.StartsWith("http", StringComparison.OrdinalIgnoreCase));

                foreach (var pc in pluginClasses)
                {
                    var depAttributes = pc.c.CustomAttributes
                        .Where(x => x.AttributeType.FullName == "BepInEx.BepInDependency");
                    var deps = depAttributes
                        .Select(x => x.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();

                    var url = pc.c.CustomAttributes
                        .Where(x => x.AttributeType.FullName == "UnityEngine.HelpURLAttribute")
                        .Select(x => x.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString())
                        .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                    if (string.IsNullOrEmpty(url))
                        url = fileUrl;

                    var guid = pc.bp.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? "Error while loading";

                    var config = configFiles?.FirstOrDefault(x => string.Equals(x.Name.Substring(0, x.Name.Length - 4), guid, StringComparison.Ordinal));

                    yield return new PluginInfo(
                        pc.bp.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? location.Name,
                        pc.bp.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString() ?? "Error while loading",
                        guid,
                        location,
                        deps,
                        assRefs,
                        author,
                        description,
                        url,
                        config);
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
                var coreDir = Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "core");
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