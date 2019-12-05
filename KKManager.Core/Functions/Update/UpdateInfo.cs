using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml.Linq;

namespace KKManager.Functions.Update
{
    public sealed class UpdateInfo
    {
        public static readonly string UpdateFileName = "Updates.xml";
        public DirectoryInfo ClientPath;
        public bool Recursive;
        public bool RemoveExtraClientFiles;

        public string ServerPath;

        public string Name;
        public string Guid;

        public static IEnumerable<UpdateInfo> ParseUpdateManifest(Stream str)
        {
            var doc = XDocument.Load(str);

            // todo relax checks
            foreach (var updateInfoElement in doc.Element("Updates").Elements("UpdateInfo"))
            {
                var name = updateInfoElement.Element("Name")?.Value;
                if (name == null) throw new ArgumentNullException(nameof(name));

                var guid = updateInfoElement.Element("GUID")?.Value;
                if (guid == null) throw new ArgumentNullException(nameof(guid));

                var remotePath = updateInfoElement.Element("ServerPath")?.Value;
                if (remotePath == null) throw new ArgumentNullException(nameof(remotePath));

                var localPath = updateInfoElement.Element("ClientPath")?.Value;
                if (localPath == null) throw new ArgumentNullException(nameof(localPath));
                var local = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, localPath));
                if (!local.FullName.StartsWith(InstallDirectoryHelper.KoikatuDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                    throw new SecurityException("ClientPath points to a directory outside the game folder - " + localPath);

                var recursive = string.Equals(updateInfoElement.Element("Recursive")?.Value, "true", StringComparison.OrdinalIgnoreCase);
                var removeExtras = string.Equals(updateInfoElement.Element("RemoveExtraClientFiles")?.Value, "true", StringComparison.OrdinalIgnoreCase);

                yield return new UpdateInfo { ClientPath = local, ServerPath = remotePath, Recursive = recursive, RemoveExtraClientFiles = removeExtras, Name = name, Guid = guid };
            }
        }
    }
}
