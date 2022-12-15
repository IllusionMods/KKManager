using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Data.Zipmods;
using KKManager.Util;
using KKManager.Windows;

namespace KKManager.ModpackTool
{
    public class ZipmodEntry : INotifyPropertyChanged
    {
        public static ZipmodEntry FromEntry(SideloaderModInfo info, int index)
        {
            var result = new ZipmodEntry(info);
            result.Index = index;
            return result;
        }

        public SideloaderModInfo Info { get; }

        public FileInfo FullPath => Info.Location;
        public int Index { get; private set; }
        public string OriginalFilename => FullPath.Name;

        private ZipmodEntryStatus _status = ZipmodEntryStatus.Ingested;
        public ZipmodEntryStatus Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public string GetTempDir() => Path.Combine(ModpackToolWindow.ModpackToolTempDir, Path.GetFileNameWithoutExtension(OriginalFilename));
        public string GetTempOutputFilePath() => Path.Combine(ModpackToolWindow.ModpackToolTempDir, "__out__", Newfilename);

        public ValidatedStringWrapper Author { get; }
        public ValidatedStringWrapper Description { get; }
        public ValidatedStringWrapper Games { get; }
        public ValidatedStringWrapper Guid { get; }
        public ValidatedStringWrapper Name { get; }
        public ValidatedStringWrapper Newfilename { get; }
        public ValidatedStringWrapper OutputSubdirectory { get; }
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
            Version = new ValidatedStringWrapper(s => Info.Manifest.Version = s, () => Info.Manifest.Version, s => !string.IsNullOrWhiteSpace(s) && System.Version.TryParse(s, out var _) &&
                                                                                                                   ExistingInOutput.Select(x => x.Version).All(v => SideloaderVersionComparer.CompareVersions(s, v) > 0));
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

            // Auto generate new filename but allow it to be overriden
            Newfilename = new ValidatedString(s => !string.IsNullOrWhiteSpace(s) && s.IndexOfAny(Path.GetInvalidFileNameChars()) == -1);
            void UpdateNewfilename(object sender, PropertyChangedEventArgs e) => Newfilename.Value = $"[{Author.Value.Replace('\\', '_').Replace('/', '_')}] {Name} v{Version}.zipmod";
            UpdateNewfilename(null, null);
            Author.PropertyChanged += UpdateNewfilename;
            //Description.PropertyChanged += UpdateOutput;
            //Games.PropertyChanged += UpdateOutput;
            //Guid.PropertyChanged += UpdateOutput;
            Name.PropertyChanged += UpdateNewfilename;
            Version.PropertyChanged += UpdateNewfilename;
            //Website.PropertyChanged += UpdateOutput;

            OutputSubdirectory = new ValidatedString(s => !string.IsNullOrWhiteSpace(s) && s.IndexOfAny(Path.GetInvalidPathChars()) == -1);
            void UpdateOutput(object sender, PropertyChangedEventArgs e) => OutputSubdirectory.Value = GetOutputSubfolder();
            UpdateOutput(null, null);
            Newfilename.PropertyChanged += UpdateOutput;

            void CheckManifestIssueStatus(object sender, PropertyChangedEventArgs args)
            {
                if (!IsValid())
                    Status = ZipmodEntryStatus.ManifestIssue;
                else if (Status < ZipmodEntryStatus.NeedsProcessing)
                    Status = ZipmodEntryStatus.NeedsProcessing;
            }
            CheckManifestIssueStatus(null, null);
            this.SubscribeToAllINotifyPropertyChangedMembers(CheckManifestIssueStatus);

            OutputSubdirectory.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(OutputSubdirectory)));
            Newfilename.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Newfilename)));

            // Never comrpess head mods since it breaks them
            if (Info.Contents.Any(x => x.EndsWith("head.unity3d", StringComparison.OrdinalIgnoreCase) && x.IndexOf("chara", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                Console.WriteLine($"{Info.FileName} looks to be a headmod, disabling recompression");
                Recompress = false;
            }
        }

        public string RelativeOutputPath => Path.Combine(OutputSubdirectory.Value, Newfilename.Value);
        public string Notes { get; set; } = string.Empty;

        public bool IsValid()
        {
            return this.AllValidatedStringsAreValid();
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
        internal static bool GameNamesVerifierLoose(string s) => s.Split(',').All(x => x.All(c => char.IsLetterOrDigit(c) || c == ' '));
        private static bool GameNamesVerifier(string s) => s == "" || s.Split(',').All(x => x.All(c => char.IsLetterOrDigit(c) || c == ' ') && ModpackToolConfiguration.Instance.AllAcceptableGameLongNames.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase));

        private static bool CanRecompress(SideloaderModInfo sideloaderModInfo)
        {
            if (sideloaderModInfo.ContentsKind == SideloaderModInfo.ZipmodContentsKind.Unknown)
                return ModpackToolConfiguration.Instance.ContentsHandlingPolicies.Single(x => x.ContentsKind == SideloaderModInfo.ZipmodContentsKind.Unknown).CanCompress;

            var canCompress = true;
            foreach (var policy in ModpackToolConfiguration.Instance.ContentsHandlingPolicies)
            {
                if ((policy.ContentsKind & sideloaderModInfo.ContentsKind) != 0)
                    canCompress = canCompress && policy.CanCompress;
            }
            return canCompress;
        }

        private string GetOutputSubfolder()
        {
            return GetOutputSubfolderWithoutAuthor().Trim('\\', '/') + "\\" + Author.Value.Replace('\\', '_').Replace('/', '_');
        }

        private string GetOutputSubfolderWithoutAuthor()
        {
            foreach (var policy in ModpackToolConfiguration.Instance.ContentsHandlingPolicies)
            {
                if ((policy.ContentsKind & Info.ContentsKind) != 0)
                {
                    if (!policy.NeverPutInsideGameSpecific && Info.Manifest.Games.Any())
                        return $"{ModpackToolConfiguration.Instance.GameOutputSubfolder.Value} {string.Join(" ", Info.Manifest.Games.Select(ModpackToolConfiguration.Instance.GameLongNameToShortName))}";
                    return policy.OutputSubfolder.Value;
                }
            }

            return ModpackToolConfiguration.Instance.ContentsHandlingPolicies.Single(x => x.ContentsKind == SideloaderModInfo.ZipmodContentsKind.Unknown).OutputSubfolder.Value;
        }

        public enum ZipmodEntryStatus
        {
            Ingested = 0,
            ManifestIssue,
            NeedsProcessing,
            Processing, //todo either lock in manifest changes or make it necessary to redo some steps?
            NeedsVerify,
            Verifying,
            PASS,
            FAIL,
            Outputted
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Prevent issues if values are changed outside main thread
            MainWindow.Instance.SafeInvoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        public void ReprocessIfPossible()
        {
            if (Status == ZipmodEntryStatus.Outputted)
            {
                if (!File.Exists(FullPath.FullName))
                {
                    if (File.Exists(GetBackupFullName()))
                        File.Move(GetBackupFullName(), FullPath.FullName);
                    else
                    {
                        Console.WriteLine($"Can't reprocess {OriginalFilename} because the file seems to be gone from both ingest and backup");
                        return;
                    }
                }
            }

            Status = IsValid() ? ZipmodEntry.ZipmodEntryStatus.NeedsProcessing : ZipmodEntry.ZipmodEntryStatus.ManifestIssue;
        }

        public void OutputIfPossible()
        {
            if (Status == ZipmodEntry.ZipmodEntryStatus.PASS)
            {
                var outputDirectory = Path.Combine(ModpackToolConfiguration.Instance.OutputFolder.Value, OutputSubdirectory.Value);
                var outputPath = Path.Combine(outputDirectory, Newfilename.Value);
                var finishedFile = new FileInfo(GetTempOutputFilePath());
                if (!finishedFile.Exists)
                {
                    Console.WriteLine($"ERROR: Can not output [{Name}] because it wasn't processed or the resulting zipmod file got removed. Setting status to NeedsProcessing.");
                    Status = ZipmodEntry.ZipmodEntryStatus.NeedsProcessing;
                }
                else
                {
                    Console.WriteLine($"Copying [{Name}] ({Status}) to {outputPath}");
                    if (File.Exists(outputPath))
                    {
                        Console.WriteLine($"WARNING: File already existed and was overwritten! {outputPath}");
                    }
                    else if (!Directory.Exists(outputDirectory))
                    {
                        Console.WriteLine($"INFO: Output directory didn't exist and was created: {outputDirectory}");
                        Directory.CreateDirectory(outputDirectory);
                    }

                    finishedFile.CopyTo(outputPath, true);

                    var backupPath = GetBackupFullName();
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                    File.Move(FullPath.FullName, backupPath);
                    Status = ZipmodEntry.ZipmodEntryStatus.Outputted;
                }
            }
            else if (Status == ZipmodEntry.ZipmodEntryStatus.FAIL)
            {
                var outputDirectory = ModpackToolConfiguration.Instance.FailFolder.Value;
                var outputPath = Path.Combine(outputDirectory, OriginalFilename);
                Console.WriteLine($"Copying [{Name}] ({Status}) to {outputPath}");
                if (File.Exists(outputPath))
                {
                    Console.WriteLine($"WARNING: File already existed and was overwritten! {outputPath}");
                }
                else if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine($"INFO: Fail directory didn't exist and was created: {outputDirectory}");
                    Directory.CreateDirectory(outputDirectory);
                }

                FullPath.CopyTo(outputPath, true);
                var backupPath = GetBackupFullName();
                if (File.Exists(backupPath)) File.Delete(backupPath);
                File.Move(FullPath.FullName, backupPath);
                Status = ZipmodEntry.ZipmodEntryStatus.Outputted;
            }
        }

        private string GetBackupFullName()
        {
            return Path.Combine(ModpackToolConfiguration.Instance.BackupFolder.Value, OriginalFilename);
        }

        public static async Task<List<ZipmodEntry>> ReadAllZipmodEntries()
        {
            var inputR = new ReplaySubject<SideloaderModInfo>();
            var inputT = SideloaderModLoader.TryReadSideloaderMods(ModpackToolConfiguration.Instance.IngestFolder, inputR, CancellationToken.None);

            var outputR = new ReplaySubject<SideloaderModInfo>();
            var outputT = SideloaderModLoader.TryReadSideloaderMods(ModpackToolConfiguration.Instance.OutputFolder, outputR, CancellationToken.None);

            await inputT;
            var allInputs = inputR.ToEnumerable().Select(FromEntry).ToList();

            await outputT;
            var existingLookup = outputR.ToEnumerable()
                                        .Where(x => x.Location.FullName.IndexOf("Sideloader Modpack - Bleeding Edge", StringComparison.OrdinalIgnoreCase) < 0 &&
                                                    x.Location.FullName.IndexOf("SideloaderModpack-BleedingEdge", StringComparison.OrdinalIgnoreCase) < 0)
                                        .ToLookup(x => x.Manifest.GUID);

            foreach (var input in allInputs)
            {
                var existing = existingLookup[input.Guid];
                input.ExistingInOutput.AddRange(existing.OrderByDescending(x => x.Manifest.Version, SideloaderVersionComparer.Default));

                input.FillInFromExisting();
            }

            return allInputs;
        }

        public void FillInFromExisting()
        {
            if (ExistingInOutput.Count == 0) return;

            FillInFromExisting(Author, x => x.Manifest.Author);
            FillInFromExisting(Name, x => x.Manifest.Name);
            FillInFromExisting(Website, x => x.Manifest.Website);
            FillInFromExisting(Description, x => x.Manifest.Description);

            Notes = "EXISTS IN OUTPUT: " + string.Join(" ; ", ExistingInOutput.Select(x => x.Manifest.Version));

            // update IsValid
            Version.Value = Version;
        }

        private void FillInFromExisting(ValidatedStringWrapper stringWrapper, Func<SideloaderModInfo, string> manifestSelector)
        {
            var currentIsValid = !string.IsNullOrWhiteSpace(stringWrapper.Value) && stringWrapper.IsValid;
            if (currentIsValid)
                return;

            var bestInExisting = ExistingInOutput.Select(manifestSelector).FirstOrDefault(x => !string.IsNullOrEmpty(x) && stringWrapper.VerifyString(x));
            if (bestInExisting != null)
                stringWrapper.Value = bestInExisting;
        }

        public List<SideloaderModInfo> ExistingInOutput { get; } = new();
    }
}