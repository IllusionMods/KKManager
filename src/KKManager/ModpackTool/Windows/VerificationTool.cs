using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KKManager.ModpackTool.Windows
{
    public partial class VerificationTool : UserControl
    {
        public VerificationTool()
        {
            InitializeComponent();

            olvColumnIndex.AspectName = nameof(ZipmodEntry.Index);
            olvColumnName.AspectName = nameof(ZipmodEntry.Newfilename);
            olvColumnStatus.AspectName = nameof(ZipmodEntry.Status);
            olvColumnStatus.FormatAsModpackToolEntryStatus();
            olvColumnContent.AspectName = $"{nameof(ZipmodEntry.Info)}.{nameof(ZipmodEntry.Info.ContentsKind)}";

            objectListView.PrimarySortColumn = olvColumnStatus;
            objectListView.PrimarySortOrder = SortOrder.Ascending;
            objectListView.SecondarySortColumn = olvColumnIndex;
            objectListView.SecondarySortOrder = SortOrder.Ascending;
            objectListView.ShowSortIndicator();
        }

        private static string GetTestDirPath() => Path.Combine(ModpackToolConfiguration.Instance.TestGameFolder.Value, "mods\\_testing");
        private static string GetTestPathForEntry(ZipmodEntry entry) => Path.Combine(GetTestDirPath(), entry.Newfilename);

        private List<ZipmodEntry> GetSelectedObjects(bool warnOnEmpty = true)
        {
            var result = objectListView.CheckedObjects?.Cast<ZipmodEntry>().ToList() ?? new List<ZipmodEntry>();
            if (warnOnEmpty && result.Count == 0)
                Console.WriteLine("WARNING: Nothing is selected! Select some entries by ticking the checkboxes on left and try again.");
            return result;
        }

        public void SetObjects(IEnumerable<ZipmodEntry> entries)
        {
            var zipmodEntries = entries == null ? new List<ZipmodEntry>() : entries.Where(x => x.Status is >= ZipmodEntry.ZipmodEntryStatus.NeedsVerify and < ZipmodEntry.ZipmodEntryStatus.Outputted).ToList();

            // Avoid firing events if the entries didn't actually change
            if (entries == null || objectListView.Objects == null || !zipmodEntries.OrderBy(x => x.Index).SequenceEqual(objectListView.Objects.Cast<ZipmodEntry>().OrderBy(x => x.Index)))
            {
                objectListView.SetObjects(zipmodEntries, true);
                objectListView.Sort();
            }
            else
            {
                if (objectListView.Visible)
                    objectListView.Refresh();
            }
        }

        private async void buttonStartVerify_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;

                var testDir = new DirectoryInfo(GetTestDirPath());
                await testDir.SafeDelete();

                testDir.Create();

                foreach (var entry in GetSelectedObjects())
                {
                    if (entry.Status == ZipmodEntry.ZipmodEntryStatus.NeedsVerify)
                    {
                        var outputFile = new FileInfo(entry.GetTempOutputFilePath());
                        if (!outputFile.Exists)
                        {
                            Console.WriteLine($"ERROR: Can not start verifying {entry.Name} because it wasn't processed or the resulting zipmod file got removed. Setting status to NeedsProcessing.");
                            entry.Status = ZipmodEntry.ZipmodEntryStatus.NeedsProcessing;
                            // No longer can be verified
                            objectListView.RemoveObject(entry);
                        }
                        else
                        {
                            var targetFilePath = GetTestPathForEntry(entry);
                            outputFile.CopyTo(targetFilePath);
                            Console.WriteLine($"Copied {outputFile.Name} to {targetFilePath}");
                            entry.Status = ZipmodEntry.ZipmodEntryStatus.Verifying;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Can not start verifying {entry.Name} because it was already verified (only NeedsVerify status works, click Reverify and try again).");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to start verification: " + ex);
                foreach (var entry in GetSelectedObjects(false))
                {
                    if (entry.Status == ZipmodEntry.ZipmodEntryStatus.Verifying)
                    {
                        await new FileInfo(GetTestPathForEntry(entry)).SafeDelete();
                        entry.Status = ZipmodEntry.ZipmodEntryStatus.NeedsVerify;
                    }
                }
            }
            finally
            {
                Enabled = true;
            }
        }

        private async void buttonPass_Click(object sender, EventArgs e)
        {
            foreach (var entry in GetSelectedObjects())
            {
                if (entry.Status == ZipmodEntry.ZipmodEntryStatus.Verifying)
                {
                    await new FileInfo(GetTestPathForEntry(entry)).SafeDelete();
                    entry.Status = ZipmodEntry.ZipmodEntryStatus.PASS;
                }
                else
                {
                    Console.WriteLine($"Can not set {entry.Name} as PASS because it isn't being verified (only Verifying status works, click Start and try again).");
                }
            }
        }

        private async void buttonFail_Click(object sender, EventArgs e)
        {
            foreach (var entry in GetSelectedObjects())
            {
                if (entry.Status == ZipmodEntry.ZipmodEntryStatus.Verifying)
                {
                    await new FileInfo(GetTestPathForEntry(entry)).SafeDelete();
                    entry.Status = ZipmodEntry.ZipmodEntryStatus.FAIL;
                }
                else
                {
                    Console.WriteLine($"Can not set {entry.Name} as FAIL because it isn't being verified (only Verifying status works, click Start and try again).");
                }
            }
        }

        private async void buttonReverify_Click(object sender, EventArgs e)
        {
            foreach (var entry in GetSelectedObjects())
            {
                await new FileInfo(GetTestPathForEntry(entry)).SafeDelete();
                entry.Status = ZipmodEntry.ZipmodEntryStatus.NeedsVerify;
            }
        }

        private void buttonOpenTestdir_Click(object sender, EventArgs e)
        {
            try
            {
                var testDir = new DirectoryInfo(GetTestDirPath());
                testDir.Create();
                Process.Start(testDir.FullName);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void buttonStartGame_Click(object sender, EventArgs e)
        {
            try
            {
                var names = new[]
                {
                    "Koikatsu Party.exe",
                    "Koikatu.exe",
                    "KoikatsuSunshine.exe",
                    "EmotionCreators.exe",
                    "AI-Syoujyo.exe",
                    "HoneySelect2.exe",
                    "RoomGirl.exe"
                };
                var testGameRoot = ModpackToolConfiguration.Instance.TestGameFolder.Value;
                var gamePath = names.Select(x => Path.Combine(testGameRoot, x)).Where(File.Exists).FirstOrDefault();
                if (gamePath != null)
                {
                    Console.WriteLine("Starting: " + gamePath);
                    Process.Start(gamePath);
                }
                else
                {
                    Console.WriteLine("ERROR: Could not find any game exe inside of " + testGameRoot);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void buttonStartStudio_Click(object sender, EventArgs e)
        {
            try
            {
                var names = new[]
                {
                    "StudioNEOV2.exe",
                    "CharaStudio.exe"
                };
                var testGameRoot = ModpackToolConfiguration.Instance.TestGameFolder.Value;
                var studioPath = names.Select(x => Path.Combine(testGameRoot, x)).Where(File.Exists).FirstOrDefault() ??
                                 Directory.GetFiles(testGameRoot, "*.exe", SearchOption.TopDirectoryOnly).FirstOrDefault(x => x.Contains("Studio"));
                if (studioPath != null)
                {
                    Console.WriteLine("Starting: " + studioPath);
                    Process.Start(studioPath);
                }
                else
                {
                    Console.WriteLine("ERROR: Could not find any studio exe inside of " + testGameRoot);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
