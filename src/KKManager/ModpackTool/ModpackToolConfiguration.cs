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
            if (Value != value)
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
    public ValidatedString IngestFolder { get; } = new(Directory.Exists);
    public ValidatedString OutputFolder { get; } = new(Directory.Exists);
    public ValidatedString TestGameFolder { get; } = new(s => Directory.Exists(s) && Directory.Exists(Path.Combine(s, "mods")));
    public ValidatedString FailFolder { get; } = new(Directory.Exists);
    public ValidatedString BackupFolder { get; } = new(Directory.Exists);

    public ValidatedString Game1Short { get; } = new(s => s.All(x => char.IsLetter(x) && char.IsUpper(x)));
    public ValidatedStringWrapper Game1Longs { get; } = new(s => Game1LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game1LongsList), ZipmodEntry.GameNamesVerifier);
    public static IReadOnlyCollection<string> Game1LongsList { get; private set; } = Array.Empty<string>();

    public ValidatedString Game2Short { get; } = new(s => s.All(x => char.IsLetter(x) && char.IsUpper(x)));
    public ValidatedStringWrapper Game2Longs { get; } = new(s => Game2LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game2LongsList), ZipmodEntry.GameNamesVerifier);
    public static IReadOnlyCollection<string> Game2LongsList { get; private set; } = Array.Empty<string>();

    public ValidatedString Game3Short { get; } = new(s => s.All(x => char.IsLetter(x) && char.IsUpper(x)));
    public ValidatedStringWrapper Game3Longs { get; } = new(s => Game3LongsList = ZipmodEntry.GameNamesStrToList(s), () => ZipmodEntry.GameNamesListToStr(Game3LongsList), ZipmodEntry.GameNamesVerifier);
    public static IReadOnlyCollection<string> Game3LongsList { get; private set; } = Array.Empty<string>();

    public bool AllValid() => this.AllValidatedStringsAreValid();

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
        IngestFolder.Value = other.IngestFolder.Value;
        OutputFolder.Value = other.OutputFolder.Value;
        TestGameFolder.Value = other.TestGameFolder.Value;
        FailFolder.Value = other.FailFolder.Value;
        BackupFolder.Value = other.BackupFolder.Value;
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {

        var allFields = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.PropertyType)).ToList();
        while (!reader.EOF)
        {
            var f = allFields.FirstOrDefault(x => x.Name == reader.Name);
            if (f != null)
            {
                ((ValidatedStringWrapper)f.GetValue(this)).Value = reader.ReadElementContentAsString();
            }
            else
                reader.Read();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        foreach (var field in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.PropertyType)))
            writer.WriteElementString(field.Name, ((ValidatedStringWrapper)field.GetValue(this)).Value);
    }
}
