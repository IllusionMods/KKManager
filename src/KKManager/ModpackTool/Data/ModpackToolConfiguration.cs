using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using KKManager.Data.Zipmods;

namespace KKManager.ModpackTool
{
    public class ModpackToolConfiguration : IXmlSerializable, INotifyPropertyChanged
    {
        private static ModpackToolConfiguration s_instance;
        private bool _compressPNGs = true;
        private bool _randomizeCABs = true;

        public static ModpackToolConfiguration Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ModpackToolConfiguration();
                }
                return s_instance;
            }
        }


        public ValidatedString IngestFolder { get; } = new ValidatedString(Directory.Exists);

        public ValidatedString OutputFolder { get; } = new ValidatedString(Directory.Exists);
        public ValidatedString TestGameFolder { get; } = new ValidatedString(s => Directory.Exists(s) && Directory.Exists(Path.Combine(s, "mods")));
        public ValidatedString FailFolder { get; } = new ValidatedString(Directory.Exists);
        public ValidatedString BackupFolder { get; } = new ValidatedString(Directory.Exists);
        public ValidatedString LooseFilesFolder { get; } = new ValidatedString(Directory.Exists);

        public ValidatedString Game1Short { get; } = new ValidatedString(s => s.All(x => char.IsUpper(x) || char.IsDigit(x)));
        public ValidatedStringWrapper Game1Longs { get; } = new ValidatedStringWrapper(s => Game1LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game1LongsList), ZipmodEntry.GameNamesVerifierLoose);
        public static IReadOnlyCollection<string> Game1LongsList { get; private set; } = Array.Empty<string>();

        public ValidatedString Game2Short { get; } = new ValidatedString(s => s.All(x => char.IsUpper(x) || char.IsDigit(x)));
        public ValidatedStringWrapper Game2Longs { get; } = new ValidatedStringWrapper(s => Game2LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game2LongsList), ZipmodEntry.GameNamesVerifierLoose);
        public static IReadOnlyCollection<string> Game2LongsList { get; private set; } = Array.Empty<string>();

        public ValidatedString Game3Short { get; } = new ValidatedString(s => s.All(x => char.IsUpper(x) || char.IsDigit(x)));
        public ValidatedStringWrapper Game3Longs { get; } = new ValidatedStringWrapper(s => Game3LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game3LongsList), ZipmodEntry.GameNamesVerifierLoose);
        public static IReadOnlyCollection<string> Game3LongsList { get; private set; } = Array.Empty<string>();

        public static IEnumerable<string> AllAcceptableGameLongNames => Game1LongsList.Concat(Game2LongsList).Concat(Game3LongsList);
        public IEnumerable<string> AllAcceptableGameShortNames => new[] { Game1Short.Value, Game2Short.Value, Game3Short.Value }.Where(x => !string.IsNullOrEmpty(x));
        public string GameLongNameToShortName(string longGameName)
        {
            if (Game1LongsList.Contains(longGameName, StringComparer.OrdinalIgnoreCase)) return Game1Short.Value;
            if (Game2LongsList.Contains(longGameName, StringComparer.OrdinalIgnoreCase)) return Game2Short.Value;
            if (Game3LongsList.Contains(longGameName, StringComparer.OrdinalIgnoreCase)) return Game3Short.Value;
            return null;
        }
        public string GameShortNameToLongName(string shortGameName)
        {
            if (shortGameName == Game1Short.Value) return Game1LongsList.FirstOrDefault();
            if (shortGameName == Game2Short.Value) return Game2LongsList.FirstOrDefault();
            if (shortGameName == Game3Short.Value) return Game3LongsList.FirstOrDefault();
            return null;
        }

        public ValidatedString GameOutputSubfolder { get; } = new ValidatedString("Sideloader Modpack - Exclusive", VerifySubfolderName);
        private static bool VerifySubfolderName(string s)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            return !string.IsNullOrWhiteSpace(s) && !s.StartsWith(" ") && !s.EndsWith(" ") && s.All(c => !invalidFileNameChars.Contains(c));
        }

        public bool CompressPNGs
        {
            get => _compressPNGs;
            set
            {
                if (value == _compressPNGs) return;
                _compressPNGs = value;
                OnPropertyChanged();
            }
        }

        public bool RandomizeCABs
        {
            get => _randomizeCABs;
            set
            {
                if (value == _randomizeCABs) return;
                _randomizeCABs = value;
                OnPropertyChanged();
            }
        }

        public sealed class ModContentsHandlingPolicy : INotifyPropertyChanged
        {
            private bool _neverPutInsideGameSpecific;
            private bool _canCompress;
            public ModContentsHandlingPolicy(SideloaderModInfo.ZipmodContentsKind contentsKind, string outputSubfolder) : this(contentsKind, outputSubfolder, true, false) { }
            public ModContentsHandlingPolicy(SideloaderModInfo.ZipmodContentsKind contentsKind, string outputSubfolder, bool canCompress, bool neverPutInsideGameSpecific)
            {
                ContentsKind = contentsKind;
                CanCompress = canCompress;
                NeverPutInsideGameSpecific = neverPutInsideGameSpecific;
                OutputSubfolder.Value = outputSubfolder;
            }
            public SideloaderModInfo.ZipmodContentsKind ContentsKind { get; }
            public ValidatedString OutputSubfolder { get; } = new ValidatedString(VerifySubfolderName);

            public bool CanCompress
            {
                get => _canCompress;
                set
                {
                    if (value == _canCompress) return;
                    _canCompress = value;
                    OnPropertyChanged();
                }
            }

            public bool NeverPutInsideGameSpecific
            {
                get => _neverPutInsideGameSpecific;
                set
                {
                    if (value == _neverPutInsideGameSpecific) return;
                    _neverPutInsideGameSpecific = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public List<ModContentsHandlingPolicy> ContentsHandlingPolicies { get; } =
            Enum.GetValues(typeof(SideloaderModInfo.ZipmodContentsKind)).Cast<SideloaderModInfo.ZipmodContentsKind>().Select(x => new ModContentsHandlingPolicy(x, "")).ToList();

        public event EventHandler ContentsChanged;
        private void OnContentsChanged()
        {
            ContentsChanged?.Invoke(this, EventArgs.Empty);
        }
        public bool AllValid() => this.AllValidatedStringsAreValid();

        #region Serialization

        public void Serialize(string path)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Create);
                new XmlSerializer(typeof(ModpackToolConfiguration)).Serialize(fileStream, this);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }

        public static ModpackToolConfiguration FromFile(string path)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Open);
                return (ModpackToolConfiguration)new XmlSerializer(typeof(ModpackToolConfiguration)).Deserialize(fileStream);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }
        public void Deserialize(string path)
        {
            var result = FromFile(path);
            CopyValuesFrom(result);

            OnContentsChanged();
        }

        public void CopyValuesFrom(ModpackToolConfiguration other)
        {
            foreach (var field in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(ValidatedStringWrapper).IsAssignableFrom(field.PropertyType))
                {
                    var our = (ValidatedStringWrapper)field.GetValue(this);
                    var their = (ValidatedStringWrapper)field.GetValue(other);
                    our.Value = their.Value;
                }
                else if (typeof(bool).IsAssignableFrom(field.PropertyType))
                {
                    field.SetValue(this, field.GetValue(other));
                }
            }

            ContentsHandlingPolicies.Clear();
            ContentsHandlingPolicies.AddRange(other.ContentsHandlingPolicies);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var allFields = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            reader.ReadStartElement("ModpackToolConfiguration");

            while (reader.NodeType == XmlNodeType.Element)
            {
                PropertyInfo f = allFields.FirstOrDefault(x => x.Name == reader.Name);
                if (f != null)
                {
                    if (typeof(ValidatedStringWrapper).IsAssignableFrom(f.PropertyType))
                        ((ValidatedStringWrapper)f.GetValue(this)).Value = reader.ReadElementContentAsString();
                    else if (typeof(bool).IsAssignableFrom(f.PropertyType))
                        f.SetValue(this, string.Equals(reader.ReadElementContentAsString(), "True", StringComparison.OrdinalIgnoreCase));
                    //reader.ReadEndElement();
                }
                else
                {
                    if (reader.Name == "ContentPolicies")
                    {
                        int index = 0;
                        reader.ReadStartElement();
                        if (reader.Name == "ContentPolicy")
                        {
                            do
                            {
                                string kind = reader.GetAttribute("Kind");
                                string subf = reader.GetAttribute("OutputSubfolder");
                                string canCompr = reader.GetAttribute("CanCompress");
                                string neverSpecific = reader.GetAttribute("NeverPutInsideGameSpecific");

                                ModContentsHandlingPolicy policy = ContentsHandlingPolicies.Find(p => p.ContentsKind.ToString() == kind);
                                policy.OutputSubfolder.Value = subf;
                                policy.CanCompress = string.Equals(canCompr, "True", StringComparison.OrdinalIgnoreCase);
                                policy.NeverPutInsideGameSpecific = string.Equals(neverSpecific, "True", StringComparison.OrdinalIgnoreCase);
                                ContentsHandlingPolicies.Remove(policy);
                                ContentsHandlingPolicies.Insert(index, policy);
                                index++;
                            } while (reader.ReadToNextSibling("ContentPolicy"));
                        }

                        reader.ReadEndElement();
                    }
                }
            }
            reader.ReadEndElement();

            //while (!reader.EOF && reader.LocalName != "ContentPolicies")
            //{
            //    var f = allFields.FirstOrDefault(x => x.Name == reader.Name);
            //    if (f != null)
            //    {
            //        ((ValidatedStringWrapper)f.GetValue(this)).Value = reader.ReadElementContentAsString();
            //    }
            //    else
            //        reader.Read();
            //}
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var field in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(ValidatedStringWrapper).IsAssignableFrom(field.PropertyType))
                {
                    writer.WriteElementString(field.Name, ((ValidatedStringWrapper)field.GetValue(this)).Value);
                }
                else if (typeof(bool).IsAssignableFrom(field.PropertyType))
                {
                    writer.WriteElementString(field.Name, ((bool)field.GetValue(this)).ToString(CultureInfo.InvariantCulture));
                }
            }

            writer.WriteStartElement("ContentPolicies");
            foreach (var modContentsHandlingPolicy in ContentsHandlingPolicies)
            {
                writer.WriteStartElement("ContentPolicy");
                writer.WriteAttributeString("Kind", modContentsHandlingPolicy.ContentsKind.ToString());
                writer.WriteAttributeString("OutputSubfolder", modContentsHandlingPolicy.OutputSubfolder.Value);
                writer.WriteAttributeString("CanCompress", modContentsHandlingPolicy.CanCompress.ToString());
                writer.WriteAttributeString("NeverPutInsideGameSpecific", modContentsHandlingPolicy.NeverPutInsideGameSpecific.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
