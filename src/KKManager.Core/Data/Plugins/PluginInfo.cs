using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace KKManager.Data.Plugins
{
    public class PluginInfo : ModInfoBase
    {
        public PluginInfo(string name, string version, string guid, FileInfo location, string[] dependancies,
            string[] assemblyReferences, string author, string description, string website, FileInfo configFile) 
            : base(location, guid, name, version, author, description, website)
        {
            var extension = location.Extension;
            if (!PluginLoader.IsValidPluginExtension(extension))
                throw new InvalidOperationException("Invalid extension for a plugin: " + Location.Extension);

            Dependancies = dependancies;
            AssemblyReferences = assemblyReferences;
            ConfigFile = configFile;
        }

        [Browsable(false)]
        public string[] Dependancies { get; }
        public string[] AssemblyReferences { get; }

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
