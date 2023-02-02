using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using KKManager.Functions;
using KKManager.Updater.Sources;
using KKManager.Util;

namespace KKManager.Updater.Data
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class UpdateInfo
    {
        public static readonly string UpdateFileName = "Updates.xml";
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Updates));

        private string _clientPath;
        private DirectoryInfo _clientPathInfo;

        /// <summary>
        /// Local path relative to game root to downlaod the mod files into
        /// </summary>
        public string ClientPath
        {
            get => _clientPath;
            set
            {
                if (_clientPath == value) return;
                _clientPath = value;
                _clientPathInfo = null;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public DirectoryInfo ClientPathInfo
        {
            get
            {
                if (string.IsNullOrEmpty(ClientPath)) return null;

                if (_clientPathInfo == null)
                {
                    var local = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, ClientPath));
                    if (!local.FullName.StartsWith(InstallDirectoryHelper.GameDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                        throw new SecurityException("ClientPath points to a directory outside the game folder - " + ClientPath);
                    _clientPathInfo = local;
                }

                return _clientPathInfo;
            }
        }

        [TypeConverter(typeof(ListTypeConverter))]
        public List<Condition> Conditions { get; set; } = new List<Condition>();

        [TypeConverter(typeof(ListTypeConverter))]
        public List<ContentHash> ContentHashes { get; set; } = new List<ContentHash>();

        [TypeConverter(typeof(ListTypeConverter))]
        public List<GameType> SupportedGames { get; set; } = new List<GameType>();

        /// <summary>
        /// Marks this update info as being an expansion for another update info.
        /// </summary>
        public string ExpandsGUID { get; set; }

        /// <summary>
        /// Identifier of the mod to be used when resolving conflicts between multiple sources.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Should the mod be always selected to be installed by default.
        /// </summary>
        public InstallByDefaultMode InstallByDefault { get; set; }

        /// <summary>
        /// Display name of the mod.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Where the update came from
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public UpdateSourceBase Source { get; private set; }

        /// <summary>
        /// Should the files be downloaded recursively from specified server path. Directory structure is maintained.
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Should files existing in <see cref="ClientPath"/> but not in <see cref="ServerPath"/> be removed from client.
        /// </summary>
        public bool RemoveExtraClientFiles { get; set; }

        /// <summary>
        /// Relative server path to download the mod files from
        /// </summary>
        public string ServerPath { get; set; }

        /// <summary>
        /// How the file versions are compared to decide if they should be updated.
        /// </summary>
        public VersioningMode Versioning { get; set; }

        /// <summary>
        /// Relative server filename of an optional .torrent file that serves as a mirror of this update.
        /// </summary>
        public string TorrentFileName { get; set; }
        /// <summary>
        /// Relative server path to create the torrent file from. If null, ServerPath is used instead.
        /// </summary>
        public string TorrentServerPath { get; set; }

        public bool IsGameSupported(GameType gameType) => SupportedGames == null || SupportedGames.Count == 0 || SupportedGames.Contains(gameType);

        public bool CheckConditions()
        {
            if (!IsGameSupported(InstallDirectoryHelper.GameType)) return false;

            foreach (var condition in Conditions)
            {
                switch (condition.Type)
                {
                    case Condition.ConditionType.ClientFileExists:
                        if (!File.Exists(Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, condition.Operand)))
                            return false;
                        break;
                    case Condition.ConditionType.ClientFileNotExists:
                        if (File.Exists(Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, condition.Operand)))
                            return false;
                        break;
                    case Condition.ConditionType.ClientFileContains:
                        var split = condition.Operand.Split(new[] { '|' }, 2);
                        if (split.Length != 2) break;
                        var path = split[0].Trim();
                        var value = split[1].Trim();
                        var fullPath = Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, path);
                        if (!File.Exists(fullPath) || !File.ReadAllText(fullPath).Contains(value))
                            return false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        /// <summary>
        /// Should the mod be selected to be installed by default.
        /// </summary>
        public bool IsEnabledByDefault()
        {
            return InstallByDefault == InstallByDefaultMode.Always || InstallByDefault == InstallByDefaultMode.IfExists && ClientPathInfo.Exists;
        }

        public static IEnumerable<UpdateInfo> ParseUpdateManifest(Stream str, UpdateSourceBase origin)
        {
            var updateInfos = Deserialize(str);

            foreach (var deserialized in updateInfos.UpdateInfos)
            {
                TestConstraints(deserialized);
                deserialized.Source = origin;
                yield return deserialized;
            }
        }

        public static void Serialize(Stream stream, Updates updateInfos)
        {
            _serializer.Serialize(stream, updateInfos);
        }

        public static void Serialize(XmlWriter writer, UpdateInfo updateInfo)
        {
            new XmlSerializer(typeof(UpdateInfo)).Serialize(writer, updateInfo);
        }

        public static void TestConstraints(UpdateInfo deserialized)
        {
            if (string.IsNullOrEmpty(deserialized.Name)) throw new ArgumentNullException(nameof(Name), "The Name element is missing or empty in " + deserialized.GUID);
            if (string.IsNullOrEmpty(deserialized.GUID)) throw new ArgumentNullException(nameof(GUID), "The GUID element is missing or empty in " + deserialized.Name);
            if (string.IsNullOrEmpty(deserialized.ServerPath) && string.IsNullOrEmpty(deserialized.TorrentFileName)) throw new ArgumentException("Both ServerPath and TorrentFileName elements are missing or empty in " + deserialized.GUID);
            if (string.IsNullOrEmpty(deserialized.ClientPath)) throw new ArgumentNullException(nameof(ClientPath), "The ClientPath element is missing or empty in " + deserialized.GUID);
            if (deserialized.Versioning == VersioningMode.Contents && !deserialized.ContentHashes.Any()) throw new ArgumentException("ContentHashes are empty when VersioningMode is set to Contents", nameof(ContentHashes));
        }

        public static Updates Deserialize(Stream stream)
        {
            void CheckVersion(int ver)
            {
                if (ver > Updates.CurrentUpdateInfoVersion)
                    throw new OutdatedVersionException("The update data is in a new format that is not supported by this version of KK Manager. Please update KK Manager and try again.");
            }

            try
            {
                var updates = (Updates)_serializer.Deserialize(stream);
                CheckVersion(updates.Version);
                return updates;
            }
            catch (OutdatedVersionException)
            {
                throw;
            }
            catch
            {
                // Check if the issue isn't because we are outdated
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                    var doc = XDocument.Load(stream);
                    var value = doc.Root?.Element("Version")?.Value;
                    if (value != null && int.TryParse(value, out var ver))
                        CheckVersion(ver);
                }
                throw;
            }
        }

        public static UpdateInfo Deserialize(XmlReader reader)
        {
            return (UpdateInfo)new XmlSerializer(typeof(UpdateInfo)).Deserialize(reader);
        }

        public UpdateInfo Copy(UpdateSourceBase newSource)
        {
            using (var tw = new StringWriter())
            using (var xw = new XmlTextWriter(tw))
            {
                Serialize(xw, this);
                using (var r = new StringReader(tw.ToString()))
                using (var xr = new XmlTextReader(r))
                {
                    var copy = Deserialize(xr);
                    copy.Source = newSource;
                    return copy;
                }
            }
        }

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(GUID) ? "NO GUID" : GUID)} - {(string.IsNullOrEmpty(Name) ? "NO NAME" : Name)} ({Conditions?.Count} conditions, {ContentHashes?.Count} hashes. Supported games: {string.Join(",", SupportedGames?.Select(x => x.ToString()) ?? new[] { GameType.Unknown.ToString() })})";
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public sealed class Condition
        {
            public enum ConditionType
            {
                ClientFileExists,
                ClientFileNotExists,
                ClientFileContains
                //todo FileContains - true if file exists and contains string - operand is path | str trimmed
            }

            [XmlAttribute]
            public string Operand { get; set; }

            [XmlAttribute]
            public ConditionType Type { get; set; }

            public override string ToString()
            {
                return Type + " - " + Operand;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public sealed class ContentHash
        {
            [XmlAttribute]
            [Obsolete]
            [Browsable(false)]
            public int Hash
            {
                get => SB3UHash != 0 ? SB3UHash : FileHash;
                set
                {
                    if (SB3UHash == 0)
                        SB3UHash = value;
                    if (FileHash == 0)
                        FileHash = value;
                }
            }

            [XmlAttribute]
            public int SB3UHash { get; set; }

            [XmlAttribute]
            public int FileHash { get; set; }

            [XmlAttribute]
            public string RelativeFileName { get; set; }

            public override string ToString()
            {
                return $"{RelativeFileName} SB3UHash={SB3UHash} FileHash={FileHash}";
            }
        }

        [XmlRoot("Updates")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Updates
        {
            public const int CurrentUpdateInfoVersion = 5;

            [XmlElement("UpdateInfo")]
            [TypeConverter(typeof(ListTypeConverter))]
            public List<UpdateInfo> UpdateInfos { get; set; } = new List<UpdateInfo>();

            [ReadOnly(true)]
            public int Version { get; set; } = CurrentUpdateInfoVersion;
        }

        public enum InstallByDefaultMode
        {
            /// <summary>
            /// Never install by default, user has to select every time
            /// </summary>
            Never,

            /// <summary>
            /// Always select to install by default
            /// </summary>
            Always,

            /// <summary>
            /// Only select to install by default if the mod's local install directory already exists
            /// </summary>
            IfExists
        }

        public enum VersioningMode
        {
            /// <summary>
            /// Update if file sizes differ, or if local file doesn't exist.
            /// </summary>
            Size,

            /// <summary>
            /// Update if remote file modify/create date is newer than the local one, or if local file doesn't exist.
            /// </summary>
            Date,

            /// <summary>
            /// Update if file contents differ, or if local file doesn't exist.
            /// </summary>
            Contents
        }
    }
}
