using Sideloader.AutoResolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MessagePack;
using SharpCompress.Common.Zip;
using KKManager.Util;
using SharpCompress.Archives;
#if AI || HS2
using AIChara;
#endif

#pragma warning disable CS0618
namespace Sideloader
{
    /// <summary>
    /// Contains data about the loaded manifest.xml
    /// </summary>
    public class Manifest
    {
        /// <summary>
        /// Version of this manifest.
        /// </summary>
        public int SchemaVer { get; } = 1;
        /// <summary>
        /// GUID of the mod.
        /// </summary>
        public string GUID
        {
            get => ManifestDocumentRoot.Element("guid")?.Value;
            set => ManifestDocumentRoot.GetOrAddElement("guid").Value = value;
        }


        /// <summary>
        /// Name of the mod. Only used for display the name of the mod when mods are loaded.
        /// </summary>
        public string Name
        {
            get => ManifestDocumentRoot.Element("name")?.Value.Trim();
            set => ManifestDocumentRoot.GetOrAddElement("name").Value = value;
        }

        /// <summary>
        /// Version of the mod.
        /// </summary>
        public string Version
        {
            get => ManifestDocumentRoot.Element("version")?.Value.Trim();
            set => ManifestDocumentRoot.GetOrAddElement("version").Value = value;
        }
        /// <summary>
        /// Author of the mod. Not currently used for anything.
        /// </summary>
        public string Author
        {
            get => ManifestDocumentRoot.Element("author")?.Value.Trim();
            set => ManifestDocumentRoot.GetOrAddElement("author").Value = value;
        }
        /// <summary>
        /// Website of the mod. Not currently used for anything.
        /// </summary>
        public string Website
        {
            get => ManifestDocumentRoot.Element("website")?.Value.Trim();
            set => ManifestDocumentRoot.GetOrAddElement("website").Value = value;
        }
        /// <summary>
        /// Description of the mod. Not currently used for anything.
        /// </summary>
        public string Description
        {
            get => ManifestDocumentRoot.Element("description")?.Value.Trim();
            set => ManifestDocumentRoot.GetOrAddElement("description").Value = value;
        }

        /// <summary>
        /// Parsed contents of the manifest.xml.
        /// </summary>
        [Obsolete("Use ManifestDocument instead")]
        public readonly XDocument manifestDocument;
        /// <summary>
        /// Parsed contents of the manifest.xml.
        /// </summary>
        public XDocument ManifestDocument => manifestDocument;
        public XElement ManifestDocumentRoot => ManifestDocument.Root;
        // /// <summary>
        // /// Raw contents of the manifest.xml.
        // /// </summary>
        // public string ManifestString { get; }

        // /// <summary>
        // /// Game the mod is made for. If specified, the mod will only load for that game. If not specified will load on any game.
        // /// </summary>
        // [IgnoreMember]
        // [Obsolete("Use Games instead")]
        // // OrderByDescending to make sure if a mod supports multiple games, this property will always show the tag for the currently running game
        // public string Game => Games.OrderByDescending(x => Array.IndexOf(Sideloader.GameNameList, x.ToLowerInvariant())).FirstOrDefault();
        /// <summary>
        /// Games the mod is made for. If specified, the mod will only load for those games. If not specified will load on any game.
        /// </summary>
        public List<string> Games
        {
            get => ManifestDocumentRoot.Elements().Where(x => x.Name.ToString().Equals("game", StringComparison.OrdinalIgnoreCase)).Select(x => x.Value.Trim()).Where(x => x.Length > 0).ToList();
            set
            {
                foreach (var xElement in ManifestDocumentRoot.Elements().Where(x => x.Name.ToString().Equals("game", StringComparison.OrdinalIgnoreCase)).ToList()) xElement.Remove();
                foreach (var gameTag in value) ManifestDocumentRoot.Add(new XElement("game", gameTag));
            }
        }

        /// <summary>
        /// List of all migration info for this mod
        /// </summary>
        public List<MigrationInfo> MigrationList { get; }

#if AI || HS2
        [Key(10)] public List<HeadPresetInfo> HeadPresetList { get; }
        [Key(11)] public List<FaceSkinInfo> FaceSkinList { get; }
#endif

        public Manifest(Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
                manifestDocument = XDocument.Load(reader);

            if (ManifestDocumentRoot == null)
                throw new OperationCanceledException("Manifest.xml is in an invalid format");

            var schemaVer = ManifestDocumentRoot.Attribute("schema-ver")?.Value;
            if (schemaVer != SchemaVer.ToString())
                throw new OperationCanceledException($"Manifest.xml is in an unknown version: {schemaVer} (supported: {SchemaVer})");

            //ManifestString = manifestDocument.ToString(SaveOptions.DisableFormatting);
            //todo move to property get and add set

            MigrationList = new List<MigrationInfo>();
#if AI || HS2
            HeadPresetList = new List<HeadPresetInfo>();
            FaceSkinList = new List<FaceSkinInfo>();
#endif
        }

        /*[SerializationConstructor]
        public Manifest(int schemaVer, string guid, string name, string version, string author, string website, string description, string manifestString, List<string> games, List<MigrationInfo> migrationList
#if AI || HS2
                        , List<HeadPresetInfo> headPresetList, List<FaceSkinInfo> faceSkinList
#endif
                        )
        {
            SchemaVer = schemaVer;
            GUID = guid;
            Name = name;
            Version = version;
            Author = author;
            Website = website;
            Description = description;
            MigrationList = migrationList ?? throw new ArgumentNullException(nameof(migrationList));
#if AI || HS2
            HeadPresetList = headPresetList ?? throw new ArgumentNullException(nameof(headPresetList));
            FaceSkinList = faceSkinList ?? throw new ArgumentNullException(nameof(faceSkinList));
#endif
            Games = games ?? throw new ArgumentNullException(nameof(games));

            ManifestString = manifestString ?? throw new ArgumentNullException(nameof(manifestString));
            // todo This adds ~200ms when reading ~3k mods from cache
            using (var reader = new StringReader(manifestString))
                manifestDocument = XDocument.Load(reader);
        }*/

        private void LoadMigrationInfo()
        {
            var migrationInfoElement = manifestDocument?.Root?.Element("migrationInfo");
            if (migrationInfoElement == null) return;

            foreach (var info in migrationInfoElement.Elements("info"))
            {
                try
                {
                    MigrationType migrationType;
                    if (info.Attribute("migrationType")?.Value == null || info.Attribute("migrationType").Value.IsNullOrWhiteSpace())
                        migrationType = MigrationType.Migrate;
                    else
                        migrationType = (MigrationType)Enum.Parse(typeof(MigrationType), info.Attribute("migrationType").Value);

                    string guidOld = info.Attribute("guidOld")?.Value;
                    string guidNew = info.Attribute("guidNew")?.Value;
                    if (guidNew.IsNullOrWhiteSpace())
                        guidNew = GUID;

                    if (guidOld.IsNullOrEmpty())
                        throw new Exception("guidOld must be specified for migration.");
                    if (guidNew.IsNullOrEmpty() && migrationType == MigrationType.Migrate)
                        throw new Exception("guidNew must be specified for migration.");

                    if (migrationType == MigrationType.MigrateAll || migrationType == MigrationType.StripAll)
                    {
                        MigrationList.Add(new MigrationInfo(migrationType, guidOld, guidNew));
                        continue;
                    }

                    if (info.Attribute("category")?.Value == null)
                        throw new Exception("Category must be specified for migration.");

                    //ChaListDefine.CategoryNo category = (ChaListDefine.CategoryNo)Enum.Parse(typeof(ChaListDefine.CategoryNo), info.Attribute("category").Value);
                    string category = info.Attribute("category").Value;

                    if (!int.TryParse(info.Attribute("idOld").Value, out int idOld) && migrationType == MigrationType.Migrate)
                        throw new Exception("ID must be specified for migration.");
                    if (!int.TryParse(info.Attribute("idNew").Value, out int idNew) && migrationType == MigrationType.Migrate)
                        throw new Exception("ID must be specified for migration.");

                    MigrationList.Add(new MigrationInfo(migrationType, category, guidOld, guidNew, idOld, idNew));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not load migration data for {GUID}, skipping line. Error: " + ex);
                }
            }
        }

        internal static bool TryLoadFromZip(IArchive zip, out Manifest manifest)
        {
            manifest = null;
            try
            {
                manifest = LoadFromZip(zip);
                return true;
            }
            catch (SystemException ex)
            {
                Console.WriteLine($"Cannot load {zip} - {ex.Message}.");
                if (!(ex is OperationCanceledException))
                    Console.WriteLine("Error details: " + ex);
                return false;
            }
        }

        internal static Manifest LoadFromZip(IArchive zip)
        {
            var entry = zip.Entries.FirstOrDefault(x => !x.IsDirectory && x.Key == "manifest.xml");

            if (entry == null)
                throw new OperationCanceledException("Manifest.xml is missing, make sure this is a zipmod");

            var manifest = new Manifest(entry.OpenEntryStream());

            if (manifest.GUID == null)
                throw new OperationCanceledException("Manifest.xml is missing the GUID");

            manifest.LoadMigrationInfo();
#if AI || HS2
            manifest.LoadHeadPresetInfo();
            manifest.LoadFaceSkinInfo();
#endif
            return manifest;
        }

#if AI || HS2
        private void LoadHeadPresetInfo()
        {
            if (manifestDocument?.Root == null) return;

            foreach (var info in ManifestDocumentRoot.Elements("headPresetInfo"))
            {
                try
                {
                    string preset = info.Attribute("preset")?.Value;
                    string headID = info.Element("headID")?.Value;
                    string headGUID = info.Element("headGUID")?.Value;
                    string skinGUID = info.Element("skinGUID")?.Value;
                    string detailGUID = info.Element("detailGUID")?.Value;
                    string eyebrowGUID = info.Element("eyebrowGUID")?.Value;
                    string pupil1GUID = info.Element("pupil1GUID")?.Value;
                    string pupil2GUID = info.Element("pupil2GUID")?.Value;
                    string black1GUID = info.Element("black1GUID")?.Value;
                    string black2GUID = info.Element("black2GUID")?.Value;
                    string hlGUID = info.Element("hlGUID")?.Value;
                    string eyelashesGUID = info.Element("eyelashesGUID")?.Value;
                    string moleGUID = info.Element("moleGUID")?.Value;
                    string eyeshadowGUID = info.Element("eyeshadowGUID")?.Value;
                    string cheekGUID = info.Element("cheekGUID")?.Value;
                    string lipGUID = info.Element("lipGUID")?.Value;
                    string paint1GUID = info.Element("paint1GUID")?.Value;
                    string paint2GUID = info.Element("paint2GUID")?.Value;
                    string layout1GUID = info.Element("layout1GUID")?.Value;
                    string layout2GUID = info.Element("layout2GUID")?.Value;

                    HeadPresetInfo headPresetInfo = new HeadPresetInfo();

                    if (preset.IsNullOrWhiteSpace())
                        throw new Exception("Preset must be specified.");
                    if (!int.TryParse(headID, out int headIDInt))
                        throw new Exception("HeadID must be specified.");
                    headPresetInfo.Preset = preset;
                    headPresetInfo.HeadID = headIDInt;
                    headPresetInfo.HeadGUID = headGUID.IsNullOrWhiteSpace() ? null : headGUID;
                    headPresetInfo.SkinGUID = skinGUID.IsNullOrWhiteSpace() ? null : skinGUID;
                    headPresetInfo.DetailGUID = detailGUID.IsNullOrWhiteSpace() ? null : detailGUID;
                    headPresetInfo.EyebrowGUID = eyebrowGUID.IsNullOrWhiteSpace() ? null : eyebrowGUID;
                    headPresetInfo.Pupil1GUID = pupil1GUID.IsNullOrWhiteSpace() ? null : pupil1GUID;
                    headPresetInfo.Pupil2GUID = pupil2GUID.IsNullOrWhiteSpace() ? null : pupil2GUID;
                    headPresetInfo.Black1GUID = black1GUID.IsNullOrWhiteSpace() ? null : black1GUID;
                    headPresetInfo.Black2GUID = black2GUID.IsNullOrWhiteSpace() ? null : black2GUID;
                    headPresetInfo.HlGUID = hlGUID.IsNullOrWhiteSpace() ? null : hlGUID;
                    headPresetInfo.EyelashesGUID = eyelashesGUID.IsNullOrWhiteSpace() ? null : eyelashesGUID;
                    headPresetInfo.MoleGUID = moleGUID.IsNullOrWhiteSpace() ? null : moleGUID;
                    headPresetInfo.EyeshadowGUID = eyeshadowGUID.IsNullOrWhiteSpace() ? null : eyeshadowGUID;
                    headPresetInfo.CheekGUID = cheekGUID.IsNullOrWhiteSpace() ? null : cheekGUID;
                    headPresetInfo.LipGUID = lipGUID.IsNullOrWhiteSpace() ? null : lipGUID;
                    headPresetInfo.Paint1GUID = paint1GUID.IsNullOrWhiteSpace() ? null : paint1GUID;
                    headPresetInfo.Paint2GUID = paint2GUID.IsNullOrWhiteSpace() ? null : paint2GUID;
                    headPresetInfo.Layout1GUID = layout1GUID.IsNullOrWhiteSpace() ? null : layout1GUID;
                    headPresetInfo.Layout2GUID = layout2GUID.IsNullOrWhiteSpace() ? null : layout2GUID;
                    headPresetInfo.Init();
                    HeadPresetList.Add(headPresetInfo);
                }
                catch (Exception ex)
                {
                    Sideloader.Logger.LogError($"Could not load head preset data for {GUID}, skipping line.");
                    Sideloader.Logger.LogError(ex);
                }
            }
        }

        private void LoadFaceSkinInfo()
        {
            if (manifestDocument?.Root == null) return;

            foreach (var info in ManifestDocumentRoot.Elements("faceSkinInfo"))
            {
                try
                {
                    string skinID = info.Attribute("skinID")?.Value;
                    string headID = info.Attribute("headID")?.Value;
                    string headGUID = info.Attribute("headGUID")?.Value;

                    if (!int.TryParse(skinID, out int skinIDInt))
                        throw new Exception("SkinID must be specified.");
                    if (!int.TryParse(headID, out int headIDInt))
                        throw new Exception("HeadID must be specified.");
                    if (headGUID.IsNullOrWhiteSpace())
                        throw new Exception("HeadGUID must be specified.");

                    FaceSkinInfo faceSkinInfo = new FaceSkinInfo(skinIDInt, GUID, headIDInt, headGUID);
                    FaceSkinList.Add(faceSkinInfo);
                }
                catch (Exception ex)
                {
                    Sideloader.Logger.LogError($"Could not load face skin data for {GUID}, skipping line.");
                    Sideloader.Logger.LogError(ex);
                }
            }
        }
#endif
    }
}
