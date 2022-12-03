using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace KKManager.ModpackTool;

public static class ModpackToolExtensions
{
    public static void SetStatusLabel(this Label l, string text, Color color)
    {
        l.TextAlign = ContentAlignment.MiddleCenter;
        l.BackColor = color;
        l.Text = text;
    }
    public static void SetStatusLabelOK(this Label l) => SetStatusLabel(l, "OK", Color.DarkGreen);
    public static void SetStatusLabelFail(this Label l) => SetStatusLabel(l, "FAIL", Color.DarkRed);
    public static void SetStatusLabelWarn(this Label l) => SetStatusLabel(l, "WARN", Color.Peru);
    public static void SetStatusLabel(this Label l, bool status)
    {
        if (status) SetStatusLabelOK(l);
        else SetStatusLabelFail(l);
    }

    public static bool AllValidatedStringsAreValid(this object obj)
    {
        var type = obj.GetType();
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                   .Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.FieldType))
                   .Select(x => x.GetValue(obj))
                   .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(x => typeof(ValidatedStringWrapper).IsAssignableFrom(x.PropertyType))
                               .Select(x => x.GetValue(obj, null)))
                   .Cast<ValidatedStringWrapper>()
                   .All(x => x.IsValid);
    }

    public static void Bind(this ValidatedStringWrapper bindTarget, TextBox inputTextbox, Label passfailLabel)
    {
        inputTextbox.DataBindings.Add(nameof(TextBox.Text), bindTarget, nameof(bindTarget.Value), false, DataSourceUpdateMode.OnPropertyChanged);

        var binding = new Binding(nameof(Label.Text), bindTarget, nameof(bindTarget.IsValid), true, DataSourceUpdateMode.Never);
        binding.Format += (sender, args) => args.Value = (bool)args.Value ? "PASS" : "FAIL";
        passfailLabel.DataBindings.Add(binding);
        var binding1 = new Binding(nameof(Label.BackColor), bindTarget, nameof(bindTarget.IsValid), true, DataSourceUpdateMode.Never);
        binding1.Format += (sender, args) => args.Value = (bool)args.Value ? Color.DarkGreen : Color.DarkRed;
        passfailLabel.DataBindings.Add(binding1);
        passfailLabel.ForeColor = Color.White;
    }

    /// <summary>
    /// Remove unnecessary .0 at the end, but leave at least 1 dot (e.g. 1.0.0.0 -> 1.0)
    /// </summary>
    public static string TrimEmptyVersionParts(this string verStr)
    {
        while (verStr.Count(c => c == '.') > 1 && verStr.EndsWith(".0")) verStr = verStr.Substring(0, verStr.Length - 2);
        return verStr;
    }
    public static int IndexOf<T>(this IEnumerable<T> source, T value)
    {
        int index = 0;
        var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
        foreach (T item in source)
        {
            if (comparer.Equals(item, value)) return index;
            index++;
        }
        return -1;
    }
}
