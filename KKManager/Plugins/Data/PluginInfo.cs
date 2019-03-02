using System.ComponentModel;
using System.IO;

namespace KKManager.Plugins.Data
{
    public class PluginInfo
    {
        public PluginInfo(string name, string version, string guid, FileInfo location)
        {
            Location = location;
            Name = name;
            Version = version;
            Guid = guid;
        }

        public FileInfo Location { get; }
        public string Name { get; }
        public string Version { get; }
        public string Guid { get; }

        [Browsable(false)]
        public string FileName => Location.Name;
    }
}