using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Updater.Properties;
using KKManager.Updater.Sources;
using KKManager.Util;
using KKManager.Util.ProcessWaiter;

namespace KKManager.Updater.Windows
{
    public partial class ModUpdateProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private UpdateSourceBase[] _updaters;
        private string[] _autoInstallGuids;
        private FileSize _overallSize;
        private FileSize _completedSize;

        private ModUpdateProgressDialog()
        {
            InitializeComponent();

            switch (InstallDirectoryHelper.GameType)
            {
                case GameType.PlayHome:
                case GameType.AiShoujoSteam:
                case GameType.AiShoujo:
                case GameType.HoneySelect2:
                    pictureBox1.Image = Resources.aichika;
                    break;

                case GameType.Koikatsu:
                case GameType.KoikatsuSteam:
                case GameType.EmotionCreators:
                    pictureBox1.Image = Resources.chikajump;
                    break;

                case GameType.Unknown:
                    break;
            }
        }

        public static void StartUpdateDialog(Form owner, UpdateSourceBase[] updaters, string[] autoInstallGuids = null)
        {
            using (var w = CreateUpdateDialog(updaters, autoInstallGuids))
            {
                w.Icon = owner.Icon;
                w.StartPosition = FormStartPosition.CenterParent;
                w.ShowDialog(owner);
            }
        }

        public static ModUpdateProgressDialog CreateUpdateDialog(UpdateSourceBase[] updaters, string[] autoInstallGuids = null)
        {
            if (updaters == null || updaters.Length == 0) throw new ArgumentException("Need at least one update source.", nameof(updaters));

            var w = new ModUpdateProgressDialog();
            w._updaters = updaters;
            w._autoInstallGuids = autoInstallGuids;
            return w;
        }

        private async void ModUpdateProgress_Shown(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Value = 0;
                progressBar1.Maximum = 1;

                labelPercent.Text = "";

                checkBoxSleep.Enabled = false;

                var random = new Random();
                if (random.Next(0, 10) >= 9)
                {
                    var offset = random.Next(0, 136);
                    var offsetStr = new string(Enumerable.Repeat(' ', offset).ToArray());
                    labelPercent.Text = offsetStr + " ( )  ( )\n";
                    labelPercent.Text += offsetStr + "( o . o)";
                }

                SetStatus("Preparing...");
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir() == false)
                    throw new OperationCanceledException();

                SetStatus("Searching for mod updates...");
                var updateTasks = await UpdateSourceManager.GetUpdates(_cancelToken.Token, _updaters, _autoInstallGuids);

                _cancelToken.Token.ThrowIfCancellationRequested();

                progressBar1.Style = ProgressBarStyle.Blocks;

                if (updateTasks.All(x => x.UpToDate))
                {
                    SetStatus("Everything is up to date!");
                    progressBar1.Value = progressBar1.Maximum;
                    _cancelToken.Cancel();
                    return;
                }

                var isAutoInstall = _autoInstallGuids != null && _autoInstallGuids.Length > 0;
                if (!isAutoInstall)
                {
                    SetStatus($"Found {updateTasks.Count} updates, waiting for user confirmation.");
                    updateTasks = ModUpdateSelectDialog.ShowWindow(this, updateTasks);
                }
                else
                {
                    var skipped = updateTasks.RemoveAll(x => x.UpToDate);
                    SetStatus($"Found {updateTasks.Count} update tasks in silent mode, {skipped} are already up-to-date.", true, true);
                }

                if (updateTasks == null)
                    throw new OperationCanceledException();
                
                SleepControls.PreventSleepOrShutdown(Handle, "Update is in progress");

                _overallSize = FileSize.SumFileSizes(updateTasks.Select(x => x.TotalUpdateSize));
                _completedSize = FileSize.Empty;

                var allItems = updateTasks.SelectMany(x => x.GetUpdateItems())
                    // Remove unnecessary to avoid potential conflicts if the update is aborted midway and a newer version is added
                    .OrderByDescending(sources => sources.Any(x => x.Item2 is DeleteFileUpdateItem))
                    // Try items with a single source first since they are the most risky
                    .ThenBy(sources => sources.Count())
                    .ThenBy(sources => sources.FirstOrDefault()?.Item2.TargetPath.FullName)
                    .ToList();

                SetStatus($"{allItems.Count(items => items.Count() > 1)} out of {allItems.Count} items have more than 1 source", false, true);

                progressBar1.Maximum = 1000;

                checkBoxSleep.Enabled = true;

                for (var index = 0; index < allItems.Count; index++)
                {
                    _cancelToken.Token.ThrowIfCancellationRequested();

                    var task = allItems[index];

                    SetStatus("Downloading " + task.First().Item2.TargetPath.Name);

                    await UpdateSingleItem(task);
                }

                var s = $"Successfully updated/removed {allItems.Count - _failedItems.Count} files from {updateTasks.Count} tasks.";
                if (_failedItems.Any())
                    s += $"\nFailed to update {_failedItems.Count} files because some sources crashed. Check log for details.";
                SetStatus(s, true, true);
                if (_failedExceptions.Any())
                {
                    var failDetails = "Reason(s) for failing:\n" + string.Join("\n", _failedExceptions.Select(x => x.Message).Distinct());
                    Console.WriteLine(failDetails);
                    s += " " + failDetails;
                }

                SleepIfNecessary();

                MessageBox.Show(s, "Finished updating", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PerformAutoScale();
            }
            catch (OutdatedVersionException ex)
            {
                SetStatus("KK Manager needs to be updated to get updates.", true, true);
                ex.ShowKkmanOutdatedMessage();
            }
            catch (OperationCanceledException)
            {
                SetStatus("Operation was cancelled by the user.", true, true);
            }
            catch (Exception ex)
            {
                var exceptions = ex is AggregateException aex ? aex.Flatten().InnerExceptions : (ICollection<Exception>)new[] { ex };

                if (!exceptions.Any(x => x is OperationCanceledException))
                    SleepIfNecessary();

                SetStatus("Crash while updating mods, aborting.", true, true);
                SetStatus(string.Join("\n---\n", exceptions), false, true);
                MessageBox.Show("Something unexpected happened and the update could not be completed. Make sure that your internet connection is stable, " +
                                "and that you did not hit your download limits, then try again.\n\nError message (check log for more):\n" + string.Join("\n", exceptions.Select(x => x.Message)),
                                "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                checkBoxSleep.Enabled = false;

                var wasCancelled = _cancelToken.IsCancellationRequested;
                _cancelToken.Cancel();

                labelPercent.Text = wasCancelled ? "Update was cancelled" : "Update finished";

                if (_completedSize != FileSize.Empty)
                    labelPercent.Text += $"\nSuccessfully downloaded {_completedSize} out of {_overallSize}.";

                progressBar1.Style = ProgressBarStyle.Blocks;
                button1.Enabled = true;
                button1.Text = "OK";

                if (_autoInstallGuids != null && _autoInstallGuids.Length > 0) Close();

                SleepControls.AllowSleepOrShutdown(Handle);
            }
        }

        private void SleepIfNecessary()
        {
            SleepControls.AllowSleepOrShutdown(Handle);

            if (checkBoxSleep.Checked)
            {
                if (!SleepControls.PutToSleep())
                    MessageBox.Show("Sleep when done", "Could not put the PC to sleep for some reason :(",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            checkBoxSleep.Enabled = false;
        }

        private readonly List<UpdateInfo> _badUpdateSources = new List<UpdateInfo>();
        private readonly List<IGrouping<string, Tuple<UpdateInfo, UpdateItem>>> _failedItems = new List<IGrouping<string, Tuple<UpdateInfo, UpdateItem>>>();
        private readonly List<Exception> _failedExceptions = new List<Exception>();

        private async Task<bool> UpdateSingleItem(IGrouping<string, Tuple<UpdateInfo, UpdateItem>> task)
        {
            var firstItem = task.First().Item2;
            var itemSize = firstItem.GetDownloadSize();

            var lastTimestamp = DateTime.UtcNow;
            var lastDownloadedKBytes = 0l;

            var progress = new Progress<double>(thisPercent =>
            {
                var timeNow = DateTime.UtcNow;
                var secondsSinceLastUpdate = (timeNow - lastTimestamp).TotalSeconds;

                if (secondsSinceLastUpdate < 1 && thisPercent < 100) return;

                //This item: 70% done (1MB / 20MB)
                //Overall: 50% done (111MB / 1221MB)
                //Speed: 1234KB/s (average 1111KB/s)

                var downloadedKBytes = (long)(itemSize.GetRawSize() * (thisPercent / 100d));
                var downloadedSize = FileSize.FromKilobytes(downloadedKBytes);
                var totalDownloadedSize = _completedSize + downloadedSize;
                var totalPercent = ((double)totalDownloadedSize.GetRawSize() / (double)_overallSize.GetRawSize()) * 100d;

                var speed = (downloadedKBytes - lastDownloadedKBytes) / secondsSinceLastUpdate;
                if (double.IsNaN(speed)) speed = 0;
                lastDownloadedKBytes = downloadedKBytes;
                lastTimestamp = timeNow;

                labelPercent.Text =
$@"This item: {thisPercent:F1}% done ({downloadedSize} / {itemSize})
Overall: {totalPercent:F1}% done ({totalDownloadedSize} / {_overallSize})
Speed: {speed:F1}KB/s";

                progressBar1.Value = Math.Min((int)(totalPercent * 10), progressBar1.Maximum);
            });

            SetStatus($"Updating {firstItem.TargetPath.Name}");
            SetStatus($"Updating {InstallDirectoryHelper.GetRelativePath(firstItem.TargetPath)}", false, true);

            var sourcesToAttempt = task.Where(x => !_badUpdateSources.Contains(x.Item1)).OrderBy(x => GetPing(x.Item1)).ToList();
            if (sourcesToAttempt.Count == 0)
            {
                Console.WriteLine("There are no working sources to download from. Check the log for reasons why the sources failed.");

                _failedItems.Add(task);
                return false;
            }

            Exception ex = null;
            foreach (var source in sourcesToAttempt)
            {
                try
                {
                    // Needed because ZipUpdater doesn't support progress
                    if (source.Item2.RemoteFile is ZipUpdater.ArchiveItem)
                        labelPercent.Text = $"Extracting... Overall progress: {_completedSize} / {_overallSize}.";

                    await RetryHelper.RetryOnExceptionAsync(() => source.Item2.Update(progress, _cancelToken.Token), 3,
                        TimeSpan.FromSeconds(3), _cancelToken.Token);

                    _completedSize += source.Item2.GetDownloadSize();
                    ex = null;
                    break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Marking source {source.Item1.Source.Origin} as broken because of exception: {e.ToStringDemystified()}");

                    ex = e;
                    _badUpdateSources.Add(source.Item1);
                }
            }
            // Check if all sources failed
            if (ex != null)
            {
                Console.WriteLine("There are no working sources to download from. Check the log for reasons why the sources failed.");

                _failedItems.Add(task);
                _failedExceptions.Add(ex);
                return false;
            }

            return true;
        }

        private static long GetPing(UpdateInfo info)
        {
            if (info.Source is ZipUpdater) return -1;
            try
            {
                var p = new Ping();
                var reply = p.Send(info.Source.Origin, 4);
                if (reply != null)
                {
                    var result = reply.RoundtripTime;
                    Console.WriteLine($"Ping {info.Source.Origin} in {reply.RoundtripTime}");
                    if (reply.Status == IPStatus.Success) return result;
                }
            }
            catch (Exception exc) { Console.WriteLine(exc); }
            return long.MaxValue;
            //return x.Item1.Source.DownloadPriority;
        }

        private void SetStatus(string status, bool writeToUi = true, bool writeToLog = false)
        {
            if (writeToUi)
                labelStatus.Text = status;
            if (writeToLog)
                Console.WriteLine("[Updater] " + status);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_cancelToken.IsCancellationRequested)
            {
                Close();
            }
            else
            {
                button1.Enabled = false;
                _cancelToken.Cancel();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cancelToken.Cancel();
            base.OnClosed(e);
        }
    }
}
