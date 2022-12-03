using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using KKManager.Data.Zipmods;

namespace KKManager.ModpackTool;

public class ZipmodEntry
{

    public static ZipmodEntry FromEntry(SideloaderModInfo info, int index)
    {
        var result = new ZipmodEntry(info);
        result.Index = index;
        return result;
    }

    public SideloaderModInfo Info { get; private set; }

    public FileInfo FullPath => Info.Location;
    public int Index { get; private set; }
    public string OriginalFilename => FullPath.Name;
    public ZipmodEntryStatus Status { get; set; }
    public string GetTempDir() => Path.Combine(ModpackToolWindow.ModpackToolTempDir, Path.GetFileNameWithoutExtension(OriginalFilename));

    public ValidatedStringWrapper Author { get; }
    public ValidatedStringWrapper Description { get; }
    public ValidatedStringWrapper Games { get; }
    public ValidatedStringWrapper Guid { get; }
    public ValidatedStringWrapper Name { get; }
    public ValidatedStringWrapper Newfilename { get; }
    public ValidatedStringWrapper Output { get; }
    public ValidatedStringWrapper Version { get; }
    public ValidatedStringWrapper Website { get; }
    public bool Recompress { get; set; }

    public ZipmodEntry(SideloaderModInfo sideloaderModInfo)
    {
        Info = sideloaderModInfo ?? throw new ArgumentNullException(nameof(sideloaderModInfo));

        Recompress = CanRecompress(sideloaderModInfo);

        Author = new ValidatedStringWrapper(s => Info.Manifest.Author = s, () => Info.Manifest.Author, s => !string.IsNullOrWhiteSpace(s) && s != "IllusionMods");
        Description = new ValidatedStringWrapper(s => Info.Manifest.Description = s, () => Info.Manifest.Description, s => string.IsNullOrEmpty(s) || s.Any(char.IsLetter));
        Games = new ValidatedStringWrapper(s => Info.Manifest.Games = CleanUpManifestGameTags(GameNamesStrToList(s)).ToList(), () => GameNamesListToStr(CleanUpManifestGameTags(Info.Manifest.Games)), GameNamesVerifier);
        Guid = new ValidatedStringWrapper(s => Info.Manifest.GUID = s, () => Info.Manifest.GUID, s => !string.IsNullOrWhiteSpace(s) && !s.EndsWith("Example"));
        Name = new ValidatedStringWrapper(s => Info.Manifest.Name = s, () => Info.Manifest.Name, s => !string.IsNullOrWhiteSpace(s) && s.IndexOf("Name of your mod pack", StringComparison.OrdinalIgnoreCase) < 0 && !s.EndsWith(" Example"));
        Version = new ValidatedStringWrapper(s => Info.Manifest.Version = s, () => Info.Manifest.Version, s => !string.IsNullOrWhiteSpace(s) && System.Version.TryParse(s, out var _));
        Website = new ValidatedStringWrapper(s => Info.Manifest.Website = s, () => Info.Manifest.Website, s => string.IsNullOrWhiteSpace(s) || Uri.TryCreate(s, UriKind.Absolute, out var uri) && !uri.IsFile && !uri.IsLoopback);

        // Clean up the manifest
        try
        {
            Description.Value = Description.Value?.Trim() ?? string.Empty;
            Website.Value = Website.Value?.Trim() ?? string.Empty;
            Author.Value = CleanUpManifestEntry(Author.Value, Info.FileNameWithoutExtension, @"^\[(.+)\]");
            Name.Value = CleanUpManifestEntry(Name.Value, Info.FileNameWithoutExtension, @"^\[.+\](.*) [vr]?\d");
            Version.Value = CleanUpManifestVersion(Version.Value, Info.FileNameWithoutExtension);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to clean up manifest of {Info.FileNameWithoutExtension} because of error: {e}");
        }

        //todo
        Output = new ValidatedString(s => !string.IsNullOrWhiteSpace(s) && s.IndexOfAny(Path.GetInvalidPathChars()) == -1);

        // Auto generate new filename but allow it to be overriden
        Newfilename = new ValidatedString(s => !string.IsNullOrWhiteSpace(s) && s.IndexOfAny(Path.GetInvalidFileNameChars()) == -1);
        void UpdateNewfilename(object sender, PropertyChangedEventArgs e) => Newfilename.Value = $"[{Author}] {Name} v{Version}.zipmod";
        UpdateNewfilename(null, null);
        Author.PropertyChanged += UpdateNewfilename;
        //Description.PropertyChanged += UpdateOutput;
        //Games.PropertyChanged += UpdateOutput;
        //Guid.PropertyChanged += UpdateOutput;
        Name.PropertyChanged += UpdateNewfilename;
        Version.PropertyChanged += UpdateNewfilename;
        //Website.PropertyChanged += UpdateOutput;
    }

    private static string CleanUpManifestEntry(string original, string fileName, string filenameRegex)
    {
        var author = original?.Trim() ?? string.Empty;
        if (author.Length == 0)
        {
            // Try extracting from filename
            var m = Regex.Match(fileName, filenameRegex);
            if (m.Success)
                author = m.Groups[1].Value.Trim();
        }
        return author;
    }

    private static string CleanUpManifestVersion(string original, string fileName)
    {
        // Try to clean up original version string
        if (!string.IsNullOrWhiteSpace(original))
        {
            if (System.Version.TryParse(original.Replace(',', '.').Trim().TrimStart('v', 'V', 'r', 'R'), out var v))
                return v.ToString().TrimEmptyVersionParts();
        }

        // Try extracting version from filename
        var m = Regex.Match(fileName, @" [vr]?(\d+([\.,]\d+)+)", RegexOptions.IgnoreCase);
        if (m.Success)
            return m.Groups[1].Value.Replace(',', '.').TrimEmptyVersionParts();

        // Clean up old version string and have user edit it
        return original?.Trim() ?? string.Empty;
    }

    internal static string GameNamesListToStr(IEnumerable<string> e) => string.Join(", ", e);

    private static IEnumerable<string> CleanUpManifestGameTags(IEnumerable<string> e)
    {
        return e.Select(x => ModpackToolConfiguration.Instance.GameLongNameToShortName(x) ?? x).Distinct().OrderBy(x => ModpackToolConfiguration.Instance.AllAcceptableGameShortNames.IndexOf(x)).Select(x => ModpackToolConfiguration.Instance.GameShortNameToLongName(x) ?? x);
    }

    internal static List<string> GameNamesStrToList(string s) => s.Split(',').Select(x => x.Trim()).ToList();
    internal static bool GameNamesVerifierLoose(string s) => s.Split(',').All(x => x.All(c => char.IsLetter(c) || c == ' '));
    private static bool GameNamesVerifier(string s) => s == "" || s.Split(',').All(x => x.All(c => char.IsLetter(c) || c == ' ') && ModpackToolConfiguration.Instance.AllAcceptableGameLongNames.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase));

    private static bool CanRecompress(SideloaderModInfo sideloaderModInfo)
    {
        //todo check based on config
        //sideloaderModInfo.
        return false;
    }

    public enum ZipmodEntryStatus
    {
        Ingested,
        ManifestIssue,
        NeedsProcessing,
        Processing,
        NeedsVerify,
        Verifying,
        PASS,
        FAIL,
        Outputted
    }
}
