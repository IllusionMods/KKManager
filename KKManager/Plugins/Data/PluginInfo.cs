using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace KKManager.Plugins.Data
{
    public class PluginInfo
    {
        public PluginInfo(string name, string version, string guid, FileInfo location, string[] dependancies, string[] assemblyReferences)
        {
            var extension = location.Extension;
            if (!PluginLoader.IsValidPluginExtension(extension))
                throw new InvalidOperationException("Invalid extension for a plugin: " + Location.Extension);

            Location = location;
            Dependancies = dependancies;
            AssemblyReferences = assemblyReferences;
            Name = name;
            Version = version;
            Guid = guid;
        }

        public FileInfo Location { get; }
        public string Name { get; }
        public string Version { get; }
        public string Guid { get; }

        public string[] Dependancies { get; }
        public string[] AssemblyReferences { get; }

        public IEnumerable<string> GetAssemblyReferencesNames()
        {
            return AssemblyReferences.Select(x => x.Split(',').First());
        }

        /// <summary>
        /// Can have extension either dll or dl_
        /// Use FileNameWithoutExtension instead to strip it
        /// </summary>
        [Browsable(false)]
        public string FileName => Location.Name;

        [Browsable(false)]
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

        public bool Enabled => Location.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase);

        public void SetEnabled(bool value)
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
