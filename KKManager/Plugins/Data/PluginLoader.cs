using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace KKManager.Plugins.Data
{
    internal static class PluginLoader
    {
        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="pluginDirectory">Directory containing the plugins to gather info from. Usually BepInEx directory inside game root.</param>
        /// <param name="searchOption">Where to search</param>
        public static List<PluginInfo> TryLoadPlugins(string pluginDirectory, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var plugins = new List<PluginInfo>();

            if (Directory.Exists(pluginDirectory))
            {
                var dlls = Directory.GetFiles(pluginDirectory, "*.dll", searchOption);
                foreach (var dll in dlls)
                {
                    try
                    {
                        plugins.AddRange(LoadFromFile(dll));
                    }
                    catch { }
                }
            }

            return plugins;
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