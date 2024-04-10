using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Zipmods;
using KKManager.Functions;
using KKManager.Util;
using Mono.Cecil;

namespace KKManager.Data.Plugins
{
    public static class PluginLoader
    {
        public static IObservable<PluginInfo> Plugins => _plugins ?? StartReload();

        private static readonly object _Lock = new object();
        private static ReplaySubject<PluginInfo> _plugins;

        public static IObservable<PluginInfo> StartReload()
        {
            lock (_Lock)
            {
                if (_plugins == null || _currentTask == null || _currentTask.IsCompleted)
                {
                    _plugins = new ReplaySubject<PluginInfo>();
                    _cancelSource?.Dispose();
                    _cancelSource = new CancellationTokenSource();
                    _currentTask = TryLoadPlugins(InstallDirectoryHelper.PluginPath.FullName, _plugins);
                }
            }
            return _plugins;
        }

        private static CancellationTokenSource _cancelSource;
        private static Task _currentTask;

        public static void CancelReload()
        {
            _cancelSource?.Cancel();
        }

        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="pluginDirectory">Directory containing the plugins to gather info from. Usually BepInEx directory inside game root.</param>
        /// <param name="subject">Output stream</param>
        private static Task TryLoadPlugins(string pluginDirectory, ReplaySubject<PluginInfo> subject)
        {
            Console.WriteLine($"Start loading plugins from [{pluginDirectory}]");

            _cancelSource?.Dispose();
            _cancelSource = new CancellationTokenSource();
            var token = _cancelSource.Token;

            void ReadPluginsAsync()
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    if (!Directory.Exists(pluginDirectory))
                    {
                        subject.OnCompleted();
                        Console.WriteLine("No plugin folder detected");
                        return;
                    }

                    var files = Directory.EnumerateFiles(pluginDirectory, "*.*", SearchOption.TopDirectoryOnly);

                    var bep5PluginsDir = Path.Combine(pluginDirectory, "plugins");
                    if (Directory.Exists(bep5PluginsDir))
                        files = files.Concat(Directory.EnumerateFiles(bep5PluginsDir, "*.*", SearchOption.AllDirectories));

                    var bep5PatchersDir = Path.Combine(pluginDirectory, "patchers");
                    if (Directory.Exists(bep5PatchersDir))
                        files = files.Concat(Directory.EnumerateFiles(bep5PatchersDir, "*.*", SearchOption.AllDirectories));

                    token.ThrowIfCancellationRequested();

                    var configDir = new DirectoryInfo(Path.Combine(pluginDirectory, "config"));
                    var configFiles = configDir.Exists ? configDir.GetFiles("*.cfg", SearchOption.TopDirectoryOnly) : Array.Empty<FileInfo>();

                    Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 6, CancellationToken = token}, file =>
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();

                            if (!IsValidPluginExtension(Path.GetExtension(file))) return;

                            foreach (var pluginInfo in LoadFromFile(file, configFiles))
                                subject.OnNext(pluginInfo);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load plugin from \"{file}\" with error: {ex.ToStringDemystified()}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException aggr)
                        ex = aggr.Flatten().InnerExceptions.First();

                    if (ex is OperationCanceledException)
                        return;

                    if (ex is SecurityException || ex is UnauthorizedAccessException)
                        MessageBox.Show("Could not load information about plugins because access to the plugins folder was denied. Check the permissions of your plugins folder and try again.\n\n" + ex.Message,
                            "Load plugins", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    Console.WriteLine("Crash when loading plugins: " + ex.ToStringDemystified());
                    subject.OnError(ex);
                }
                finally
                {
                    Console.WriteLine($"Finished loading plugins from [{pluginDirectory}] in {sw.ElapsedMilliseconds}ms");
                    subject.OnCompleted();
                }
            }

            try
            {
                var task = new Task(ReadPluginsAsync, token, TaskCreationOptions.LongRunning);
                task.Start();
                return task;
            }
            catch (OperationCanceledException)
            {
                return Task.FromCanceled(token);
            }
        }

        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">Access to fileName is denied. </exception>
        public static IEnumerable<PluginInfo> LoadFromFile(string dllFile, FileInfo[] configFiles = null)
        {
            var location = new FileInfo(dllFile);

            using (var md = ModuleDefinition.ReadModule(dllFile, new ReaderParameters { AssemblyResolver = GetResolver(location) }))
            {
                var classes = md.Types.Where(x => x.IsClass).ToList();

                var guidFields = classes.SelectMany(x => x.Fields)
                                        .Where(x => x.IsLiteral && x.IsStatic && x.HasConstant  // const
                                                 && x.FieldType.FullName == "System.String"
                                                 && x.Name.ContainsAny(StringComparison.OrdinalIgnoreCase, "GUID", "extdata", "extsave", "ExtID", "PluginName"))
                                        .Select(f => f.Constant?.ToString())
                                        .Where(x => !string.IsNullOrWhiteSpace(x))
                                        .ToList();

                var pluginClasses = classes.Where(x => x.HasCustomAttributes).Select(c => new
                {
                    c,
                    bp = c.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "BepInEx.BepInPlugin" ||
                                                                x.AttributeType.FullName == "BepInEx.Preloader.Core.Patching.PatcherPluginInfoAttribute")
                }).Where(x => x.bp != null).ToList();

                if (pluginClasses.Count != 0)
                {
                    var assRefs = md.AssemblyReferences.Select(x => x.FullName).ToArray();

                    var f = FileVersionInfo.GetVersionInfo(dllFile);
                    var author = f.CompanyName;
                    var m = Regex.Match(author, @"^https://*.github.com/([^/]+).*?$");
                    if (m.Success) author = m.Groups[1].Value;
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
                        guidFields.Add(guid);

                        var config = configFiles?.FirstOrDefault(x => string.Equals(x.Name.Substring(0, x.Name.Length - 4), guid, StringComparison.Ordinal));

                        yield return new PluginInfo(
                            pc.bp.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? location.Name,
                            pc.bp.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString() ?? "Error while loading",
                            guid,
                            guidFields.Distinct().OrderBy(x => x),
                            location,
                            deps,
                            assRefs,
                            author,
                            description,
                            url,
                            null, //Needed to handle additional parameter returned by PluginInfo, placeholder for future reading plugin BepInDependency application name
                            config);
                    }
                }
                else if (md.AssemblyReferences.Any(x => x.Name == "Mono.Cecil"))
                {
                    // This could be a patcher
                    var patcherClasses = classes.Any(x =>
                    {
                        return x.Methods.Any(m => m.IsStatic && m.Name == "Patch" && m.Parameters.Count == 1) && x.Properties.Any(p => p.Name == "TargetDLLs" && p.GetMethod.IsStatic);
                    });

                    if (patcherClasses)
                    {
                        var f = FileVersionInfo.GetVersionInfo(dllFile);
                        var author = f.CompanyName;
                        var m = Regex.Match(author, @"^https://*.github.com/([^/]+).*?$");
                        if (m.Success) author = m.Groups[1].Value;
                        var description = f.Comments;
                        var fileUrl = new[] { f.CompanyName, f.FileDescription, f.Comments, f.LegalCopyright, f.LegalTrademarks }.FirstOrDefault(x => x.StartsWith("http", StringComparison.OrdinalIgnoreCase));

                        var assRefs = md.AssemblyReferences.Select(x => x.FullName).ToArray();

                        yield return new PluginInfo(Path.GetFileNameWithoutExtension(dllFile), string.IsNullOrWhiteSpace(f.FileVersion) ? f.ProductVersion : f.FileVersion, "< PATCHER PLUGIN >", null, location,
                                                    Array.Empty<string>(), assRefs, author, description, fileUrl, null, null); //for future reading plugin BepInDependency application name replace the first "null" with a proper name
                    }
                }
            }
        }

        public static bool IsValidPluginExtension(string extension)
        {
            return extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) || extension.Equals(".dl_", StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsDisabledPlugin(string extension, out string enabledExtension)
        {
            if (extension.Equals(".dl_", StringComparison.OrdinalIgnoreCase))
            {
                enabledExtension = ".dll";
                return true;
            }
            enabledExtension = null;
            return false;
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
