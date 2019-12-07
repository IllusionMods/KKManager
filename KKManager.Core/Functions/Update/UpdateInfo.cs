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

        /// <summary>
        /// Local path relative to game root to downlaod the mod files into
        /// </summary>
        public DirectoryInfo ClientPath { get; private set; }
        /// <summary>
        /// Relative server path to download the mod files from
        /// </summary>
        public string ServerPath { get; private set; }

        /// <summary>
        /// Should the files be downloaded recursively from specified server path. Directory structure is maintained.
        /// </summary>
        public bool Recursive { get; private set; }
        /// <summary>
        /// Should files existing in <see cref="ClientPath"/> but not in <see cref="ServerPath"/> be removed from client.
        /// </summary>
        public bool RemoveExtraClientFiles { get; private set; }
        /// <summary>
        /// Should the mod be always selected to be installed by default.
        /// </summary>
        public bool AlwaysInstall { get; private set; }
        /// <summary>
        /// How the file versions are compared to decide if they should be updated.
        /// </summary>
        public VersioningMode Versioning { get; private set; }

        /// <summary>
        /// Display name of the mod.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Identifier of the mod to be used when resolving conflicts between multiple sources.
        /// </summary>
        public string Guid { get; private set; }

        public static IEnumerable<UpdateInfo> ParseUpdateManifest(Stream str, string origin, int priority)
        {
            var doc = XDocument.Load(str);

            foreach (var updateInfoElement in doc.Element("Updates").Elements("UpdateInfo"))
            {
                var name = updateInfoElement.Element("Name")?.Value;
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name), "The Name element is missing or empty");

                var guid = updateInfoElement.Element("GUID")?.Value;
                if (string.IsNullOrEmpty(guid)) throw new ArgumentNullException(nameof(guid), "The GUID element is missing or empty in " + name);

                var remotePath = updateInfoElement.Element("ServerPath")?.Value;
                if (string.IsNullOrEmpty(remotePath)) throw new ArgumentNullException(nameof(remotePath), "The ServerPath element is missing or empty in " + name);

                var localPath = updateInfoElement.Element("ClientPath")?.Value;
                if (string.IsNullOrEmpty(localPath)) throw new ArgumentNullException(nameof(localPath), "The ClientPath element is missing or empty in " + name);
                var local = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, localPath));
                if (!local.FullName.StartsWith(InstallDirectoryHelper.KoikatuDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                    throw new SecurityException("ClientPath points to a directory outside the game folder - " + localPath);

                if (!bool.TryParse(updateInfoElement.Element("Recursive")?.Value, out var recursive)) recursive = false;
                if (!bool.TryParse(updateInfoElement.Element("RemoveExtraClientFiles")?.Value, out var removeExtras)) removeExtras = false;
                if (!bool.TryParse(updateInfoElement.Element("AlwaysInstall")?.Value, out var alwaysInstall)) alwaysInstall = false;

                if (!Enum.TryParse<VersioningMode>(updateInfoElement.Element("VersioningMode")?.Value, true, out var versioningMode)) versioningMode = VersioningMode.Size;

                yield return new UpdateInfo
                {
                    ClientPath = local,
                    ServerPath = remotePath,
                    Recursive = recursive,
                    RemoveExtraClientFiles = removeExtras,
                    Name = name,
                    Guid = guid,
                    AlwaysInstall = alwaysInstall,
                    Origin = origin,
                    SourcePriority = priority,
                    Versioning = versioningMode
                };
            }
        }

        /// <summary>
        /// Where the update came from
        /// </summary>
        public string Origin { get; private set; }

        /// <summary>
        /// Priority of the source if multiple sources for a download are available. Higher will be attempted to be downloaded first.
        /// </summary>
        public int SourcePriority { get; private set; }

        /// <summary>
        /// Should the mod be selected to be installed by default.
        /// </summary>
        public bool IsEnabledByDefault()
        {
            return AlwaysInstall || ClientPath.Exists;
        }

        public enum VersioningMode
        {
            Size,
            Date
        }
    }
}
