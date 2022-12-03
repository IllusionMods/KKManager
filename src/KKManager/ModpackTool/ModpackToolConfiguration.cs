using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using KKManager.Data.Zipmods;
using static KKManager.ModpackTool.ModpackToolConfiguration;

namespace KKManager.ModpackTool;

public class ValidatedString : ValidatedStringWrapper
{
    public ValidatedString(Func<string, bool> verifyValue) : this(string.Empty, verifyValue)
    { }

    public ValidatedString(string initialValue, Func<string, bool> verifyValue) : base(verifyValue)
    {
        _value = initialValue ?? throw new ArgumentNullException(nameof(initialValue));
        _isValid = verifyValue(initialValue);
    }

    private string _value;
    private bool _isValid;

    public override event PropertyChangedEventHandler PropertyChanged;

    public override string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;

                var newValid = VerifyValue(value);
                if (_isValid != newValid)
                {
                    _isValid = newValid;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }

    public override bool IsValid
    {
        get => _isValid;
    }

    public static void Bind(BindingSource bindTarget, string stringPropName, TextBox inputTextbox, Label passfailLabel, DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged)
    {
        if (bindTarget == null) throw new ArgumentNullException(nameof(bindTarget));
        if (stringPropName == null) throw new ArgumentNullException(nameof(stringPropName));
        if (inputTextbox == null) throw new ArgumentNullException(nameof(inputTextbox));
        if (passfailLabel == null) throw new ArgumentNullException(nameof(passfailLabel));

        inputTextbox.DataBindings.Add(nameof(TextBox.Text), bindTarget, stringPropName + ".Value", false, updateMode);

        var binding = new Binding(nameof(Label.Text), bindTarget, stringPropName + ".IsValid", true, DataSourceUpdateMode.Never);
        binding.Format += (sender, args) => args.Value = (bool)args.Value ? "PASS" : "FAIL";
        passfailLabel.DataBindings.Add(binding);
        var binding1 = new Binding(nameof(Label.BackColor), bindTarget, stringPropName + ".IsValid", true, DataSourceUpdateMode.Never);
        binding1.Format += (sender, args) => args.Value = (bool)args.Value ? Color.DarkGreen : Color.DarkRed;
        passfailLabel.DataBindings.Add(binding1);
        passfailLabel.ForeColor = Color.White;
    }
}

public class ValidatedStringWrapper : INotifyPropertyChanged
{
    public static implicit operator string(ValidatedStringWrapper w) => w.Value;

    protected ValidatedStringWrapper(Func<string, bool> verifyValue)
    {
        VerifyValue = verifyValue ?? throw new ArgumentNullException(nameof(verifyValue));
    }
    public ValidatedStringWrapper(Action<string> set, Func<string> get, Func<string, bool> verifyValue) : this(verifyValue)
    {
        _set = set ?? throw new ArgumentNullException(nameof(set));
        _get = get ?? throw new ArgumentNullException(nameof(get));
    }

    private readonly Action<string> _set;
    private readonly Func<string> _get;
    protected readonly Func<string, bool> VerifyValue;

    public virtual event PropertyChangedEventHandler PropertyChanged;

    public virtual string Value
    {
        get => _get();
        set
        {
            //if (Value != value)
            {
                var prevValid = VerifyValue(value);
                _set(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                if (prevValid != IsValid)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            }
        }
    }

    public virtual bool IsValid => VerifyValue(_get());//todo handle needs check values

    public override string ToString() => Value ?? "";
}

public class ModpackToolConfiguration : IXmlSerializable
{
    public static ModpackToolConfiguration Instance { get; } = new();

    public ValidatedString IngestFolder { get; } = new(Directory.Exists);
    public ValidatedString OutputFolder { get; } = new(Directory.Exists);
    public ValidatedString TestGameFolder { get; } = new(s => Directory.Exists(s) && Directory.Exists(Path.Combine(s, "mods")));
    public ValidatedString FailFolder { get; } = new(Directory.Exists);
    public ValidatedString BackupFolder { get; } = new(Directory.Exists);

    public ValidatedString Game1Short { get; } = new(s => s.All(x => char.IsLetter(x) && char.IsUpper(x)));
    public ValidatedStringWrapper Game1Longs { get; } = new(s => Game1LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game1LongsList), ZipmodEntry.GameNamesVerifierLoose);
    public static IReadOnlyCollection<string> Game1LongsList { get; private set; } = Array.Empty<string>();

    public ValidatedString Game2Short { get; } = new(s => s.All(x => char.IsLetter(x) && char.IsUpper(x)));
    public ValidatedStringWrapper Game2Longs { get; } = new(s => Game2LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game2LongsList), ZipmodEntry.GameNamesVerifierLoose);
    public static IReadOnlyCollection<string> Game2LongsList { get; private set; } = Array.Empty<string>();

    public ValidatedString Game3Short { get; } = new(s => s.All(x => char.IsLetter(x) && char.IsUpper(x)));
    public ValidatedStringWrapper Game3Longs { get; } = new(s => Game3LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game3LongsList), ZipmodEntry.GameNamesVerifierLoose);
    public static IReadOnlyCollection<string> Game3LongsList { get; private set; } = Array.Empty<string>();

    public IEnumerable<string> AllAcceptableGameLongNames => Game1LongsList.Concat(Game2LongsList).Concat(Game3LongsList);
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

    public ValidatedString GameOutputSubfolder { get; } = new("Sideloader Modpack - Exclusive", VerifySubfolderName);
    private static bool VerifySubfolderName(string s)
    {
        var invalidFileNameChars = Path.GetInvalidFileNameChars();
        return !string.IsNullOrWhiteSpace(s) && !s.StartsWith(" ") && !s.EndsWith(" ") && s.All(c => !invalidFileNameChars.Contains(c));
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
        public ValidatedString OutputSubfolder { get; } = new(VerifySubfolderName);

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

    public event EventHandler ContentsHandlingPoliciesChanged;
    private void OnContentsHandlingPoliciesChanged()
    {
        ContentsHandlingPoliciesChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool AllValid() => this.AllValidatedStringsAreValid();

    #region Serialization

    public void Serialize(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Create);
        new XmlSerializer(typeof(ModpackToolConfiguration)).Serialize(fileStream, this);
    }

    public static ModpackToolConfiguration FromFile(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Open);
        return (ModpackToolConfiguration)new XmlSerializer(typeof(ModpackToolConfiguration)).Deserialize(fileStream);
    }
    public void Deserialize(string path)
    {
        var result = FromFile(path);
        CopyValuesFrom(result);
    }

    public void CopyValuesFrom(ModpackToolConfiguration other)
    {
        foreach (var field in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.PropertyType)))
        {
            var our = (ValidatedStringWrapper)field.GetValue(this);
            var their = (ValidatedStringWrapper)field.GetValue(other);
            our.Value = their.Value;
        }
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        var allFields = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.PropertyType)).ToList();

        reader.ReadStartElement("ModpackToolConfiguration");

        while (reader.NodeType == XmlNodeType.Element)
        {
            var f = allFields.FirstOrDefault(x => x.Name == reader.Name);
            if (f != null)
            {
                ((ValidatedStringWrapper)f.GetValue(this)).Value = reader.ReadElementContentAsString();
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
                            var kind = reader.GetAttribute("Kind");
                            var subf = reader.GetAttribute("OutputSubfolder");
                            var canCompr = reader.GetAttribute("CanCompress");
                            var neverSpecific = reader.GetAttribute("NeverPutInsideGameSpecific");

                            var policy = ContentsHandlingPolicies.Find(policy => policy.ContentsKind.ToString() == kind);
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

        OnContentsHandlingPoliciesChanged();

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
        foreach (var field in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.PropertyType)))
            writer.WriteElementString(field.Name, ((ValidatedStringWrapper)field.GetValue(this)).Value);

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
}
