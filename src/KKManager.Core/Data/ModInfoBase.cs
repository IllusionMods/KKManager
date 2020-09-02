using System;
using System.ComponentModel;
using System.IO;

namespace KKManager.Data
{
    public abstract class ModInfoBase : IFileInfoBase
    {
        protected ModInfoBase(FileInfo location, string guid, string name, string version, string author, string description, string website)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Guid = guid ?? throw new ArgumentNullException(nameof(guid));
            Name = name;// ?? throw new ArgumentNullException(nameof(name));
            Version = version;// ?? throw new ArgumentNullException(nameof(version));
            Author = author;
            Description = description;
            Website = website;
        }

        public FileInfo Location { get; }
        public string Name { get; }
        public string Version { get; }
        public string Guid { get; }

        public string Author { get; }
        public string Description { get; }
        public string Website { get; }

        [Browsable(false)]
        public string FileName => Location.Name;

        [Browsable(false)]
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

        public abstract bool Enabled { get; }
        public abstract void SetEnabled(bool value);
    }
}
