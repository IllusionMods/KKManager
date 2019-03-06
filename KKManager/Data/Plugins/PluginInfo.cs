using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace KKManager.Data.Plugins
{

    public class PluginInfo : ModInfoBase
    {
        public PluginInfo(string name, string version, string guid, FileInfo location, string[] dependancies, string[] assemblyReferences) : base(location, guid, name, version)
        {
            var extension = location.Extension;
            if (!PluginLoader.IsValidPluginExtension(extension))
                throw new InvalidOperationException("Invalid extension for a plugin: " + Location.Extension);
            
            Dependancies = dependancies;
            AssemblyReferences = assemblyReferences;
        }

        [Browsable(false)]
        public string[] Dependancies { get; }
        public string[] AssemblyReferences { get; }

        public IEnumerable<string> GetAssemblyReferencesNames()
        {
            return AssemblyReferences.Select(x => x.Split(',').First());
        }

        public override bool Enabled => Location.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase);

        public override void SetEnabled(bool value)
        {
            if (Location.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                if (!value)
                {
                    var newName = Location.FullName.Substring(0, Location.FullName.Length - 1) + '_';
                    Location.MoveTo(newName);
                }
            }
            else if (Location.Extension.Equals(".dl_", StringComparison.OrdinalIgnoreCase))
            {
                if (value)
                {
                    var newName = Location.FullName.Substring(0, Location.FullName.Length - 1) + 'l';
                    Location.MoveTo(newName);
                }
            }
            else
            {
                throw new InvalidOperationException("Plugin has invalid extension: " + Location.Extension);
            }
        }
    }
}
