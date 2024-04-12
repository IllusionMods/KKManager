using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace KKManager.Data.Plugins
{
    public class PluginInfo : ModInfoBase
    {
        public PluginInfo(
            string name,
            string version,
            string guid,
            IEnumerable<string> extDataGuidCandidates,
            FileInfo location,
            string[] dependancies,
            string[] assemblyReferences,
            string author,
            string description,
            string website,
            IReadOnlyList<string> processes,
            FileInfo configFile)
            : base(location, guid, name, version, author, description, website, processes)
        {
            var extension = location.Extension;
            if (!PluginLoader.IsValidPluginExtension(extension))
                throw new InvalidOperationException("Invalid extension for a plugin: " + Location.Extension);

            ExtDataGuidCandidates = extDataGuidCandidates?.ToArray() ?? Array.Empty<string>();
            Dependancies = dependancies;
            AssemblyReferences = assemblyReferences;
            ConfigFile = configFile;
        }


        [Browsable(false)]
        public string[] Dependancies { get; }
        public string[] AssemblyReferences { get; }

#if !DEBUG
        [Browsable(false)]
#endif
        public string[] ExtDataGuidCandidates { get; }

        public FileInfo ConfigFile { get; }

        public IEnumerable<string> GetAssemblyReferencesNames()
        {
            return AssemblyReferences.Select(x => x.Split(',').First());
        }

        public override bool Enabled => Location.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase);

        public override void SetEnabled(bool value)
        {
            if (Enabled != value)
            {
                Location.MoveTo(EnabledLocation(Location, value).FullName);
            }
        }

        public static FileInfo EnabledLocation(FileInfo location, bool enable = true)
        {
            var ext = location.Extension.ToCharArray();
            ext[3] = enable ? 'l' : '_';
            return new FileInfo(location.FullName.Substring(0, location.FullName.Length - ext.Length) + new string(ext));
        }
    }
}
