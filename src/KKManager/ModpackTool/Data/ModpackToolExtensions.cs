using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;

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
    public static void SubscribeToAllINotifyPropertyChangedMembers(this object obj, PropertyChangedEventHandler handler)
    {
        var type = obj.GetType();
        var objs = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                       .Where(x => typeof(INotifyPropertyChanged).IsAssignableFrom(x.FieldType))
                       .Select(x => x.GetValue(obj))
                       .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                   .Where(x => typeof(INotifyPropertyChanged).IsAssignableFrom(x.PropertyType))
                                   .Select(x => x.GetValue(obj, null)))
                       .Cast<INotifyPropertyChanged>();
        foreach (var x in objs) x.PropertyChanged += handler;
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

    /// <summary>
    /// Waits asynchronously for the process to exit.
    /// </summary>
    /// <param name="process">The process to wait for cancellation.</param>
    /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
    /// immediately as canceled.</param>
    /// <returns>A Task representing waiting for the process to end.</returns>
    public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (process.HasExited) return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => tcs.TrySetResult(null);
        if (cancellationToken != default(CancellationToken))
            cancellationToken.Register(() => tcs.SetCanceled());

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }

    public static async Task<bool> SafeDelete(this FileSystemInfo info)
    {
        try
        {
            if (info.Exists)
            {
                if (info is DirectoryInfo di)
                    di.Delete(true);
                else
                    info.Delete();
            }
        }
        catch
        {
            await Task.Delay(100);
            try
            {
                info.Refresh();
                if (info.Exists)
                {
                    if (info is DirectoryInfo di)
                        di.Delete(true);
                    else
                        info.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete [{info.FullName}] because of error: {ex}");
                return false;
            }
        }

        return true;
    }

    public static void FormatAsModpackToolEntryStatus(this OLVColumn columnStatus)
    {
        var listView = (ObjectListView)columnStatus.ListView;
        listView.UseCellFormatEvents = true;
        listView.FormatCell += FormatCell;

        void FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Column == columnStatus)
            {
                var m = (ZipmodEntry)e.Model;
                e.SubItem.BackColor = m.Status switch
                {
                    ZipmodEntry.ZipmodEntryStatus.Ingested => Color.DimGray,
                    ZipmodEntry.ZipmodEntryStatus.ManifestIssue => Color.DarkMagenta,
                    ZipmodEntry.ZipmodEntryStatus.NeedsProcessing => Color.DarkBlue,
                    ZipmodEntry.ZipmodEntryStatus.Processing => Color.DarkSlateBlue,
                    ZipmodEntry.ZipmodEntryStatus.NeedsVerify => Color.DarkCyan,
                    ZipmodEntry.ZipmodEntryStatus.Verifying => Color.DarkTurquoise,
                    ZipmodEntry.ZipmodEntryStatus.PASS => Color.DarkGreen,
                    ZipmodEntry.ZipmodEntryStatus.FAIL => Color.DarkRed,
                    ZipmodEntry.ZipmodEntryStatus.Outputted => Color.DarkSeaGreen,
                    _ => Color.DimGray
                };
                e.SubItem.ForeColor = Color.White;
            }
        }
    }
}
