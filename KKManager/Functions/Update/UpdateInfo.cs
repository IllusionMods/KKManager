using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml.Linq;

namespace KKManager.Functions.Update {
    public sealed class UpdateInfo
    {
        public static IEnumerable<UpdateInfo> ParseUpdateManifest(Stream str)
        {
            var doc = XDocument.Load(str);

            foreach (var updateInfoElement in doc.Element("Updates").Elements("UpdateInfo"))
            {
                var remotePath = updateInfoElement.Element("ServerPath")?.Value;
                if (remotePath == null) throw new ArgumentNullException(nameof(remotePath));

                var localPath = updateInfoElement.Element("ClientPath")?.Value;
                if (localPath == null) throw new ArgumentNullException(nameof(localPath));
                var local = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, localPath));
                if (!local.FullName.StartsWith(InstallDirectoryHelper.KoikatuDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                    throw new SecurityException("ClientPath points to a directory outside the game folder - " + localPath);

                var recursive = String.Equals(updateInfoElement.Element("Recursive")?.Value, "true", StringComparison.OrdinalIgnoreCase);
                var removeExtras = String.Equals(updateInfoElement.Element("RemoveExtraClientFiles")?.Value, "true", StringComparison.OrdinalIgnoreCase);

                yield return new UpdateInfo { ClientPath = local, ServerPath = remotePath, Recursive = recursive, RemoveExtraClientFiles = removeExtras };
            }
        }

        public string ServerPath;
        public DirectoryInfo ClientPath;
        public bool Recursive;
        public bool RemoveExtraClientFiles;
        public const string UpdateFileName = "Updates.xml";
    }
}