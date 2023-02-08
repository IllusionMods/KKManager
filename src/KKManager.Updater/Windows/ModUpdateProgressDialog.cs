using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Updater.Downloader;
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
                case GameType.RoomGirl:
                    pictureBox1.Image = Resources.aichika;
                    break;

                case GameType.Koikatsu:
                case GameType.KoikatsuSteam:
                case GameType.EmotionCreators:
                case GameType.KoikatsuSunshine:
                    pictureBox1.Image = Resources.chikajump;
                    break;

                default:
                    Debug.Fail("Unhandled game type: " + InstallDirectoryHelper.GameType);
                    Console.WriteLine("WARNING: Unhandled game type: " + InstallDirectoryHelper.GameType);
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
#if DEBUG
            w._updaters = w._updaters.Where(x => x.Origin.Contains("hf.honeyselect2.com")).ToArray(); //todo remove or improve
#endif
            w._autoInstallGuids = autoInstallGuids;
            return w;
        }

        private async void ModUpdateProgress_Shown(object sender, EventArgs e)
        {
            var averageDownloadSpeed = new MovingAverage(20);
            var downloadStartTime = DateTime.MinValue;
            UpdateDownloadCoordinator downloader = null;

            IReadOnlyList<UpdateDownloadItem> downloadItems = null;
            var lastCompletedSize = FileSize.Empty;
            void DoStatusLabelUpdate(object o, EventArgs args)
            {
                if (downloadItems == null) throw new ArgumentNullException(nameof(downloadItems));

                var itemCount = fastObjectListView1.GetItemCount();
                if (itemCount > 0)
                {
                    fastObjectListView1.BeginUpdate();
                    fastObjectListView1.RedrawItems(0, itemCount - 1, true);
                    // Needed if user changes sorting column
                    //fastObjectListView1.SecondarySortColumn = olvColumnNo;
                    fastObjectListView1.Sort();
                    fastObjectListView1.EndUpdate();
                }

                _completedSize = FileSize.SumFileSizes(downloadItems.Select(x => x.GetDownloadedSize()));

                var totalPercent = (double)_completedSize.GetKbSize() / _overallSize.GetKbSize() * 100d;
                if (double.IsNaN(totalPercent)) totalPercent = 0;

                // Download speed calc
                var secondsPassed = updateTimer.Interval / 1000d;
                var downloadedSinceLast = FileSize.FromKilobytes((long)((_completedSize - lastCompletedSize).GetKbSize() / secondsPassed));
                lastCompletedSize = _completedSize;
                averageDownloadSpeed.Sample(downloadedSinceLast.GetKbSize());
                var etaSeconds = (_overallSize - _completedSize).GetKbSize() / (double)averageDownloadSpeed.GetAverage();
                var eta = double.IsNaN(etaSeconds) || etaSeconds < 0 || etaSeconds > TimeSpan.MaxValue.TotalSeconds
                    ? "Unknown"
                    : TimeSpan.FromSeconds(etaSeconds).GetReadableTimespan();

                var text = $"Overall: {totalPercent:F1}% done  ({_completedSize} out of {_overallSize})";

                var uploadSpeed = TorrentUpdater.GetCurrentUpload();
                if (uploadSpeed.HasValue) labelPercent.Text += $" / Seeding: {FileSize.FromBytes(uploadSpeed.Value)}/s";

                text += $"\r\nSpeed: {downloadedSinceLast}/s  (ETA: {eta})";

                labelPercent.Text = text;

                progressBar1.Value = Math.Min((int)Math.Round(totalPercent * 10), progressBar1.Maximum);
            }

            try
            {
                #region Initialize UI

                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Value = 0;
                progressBar1.Maximum = 1;

                labelPercent.Text = "";

                checkBoxSleep.Enabled = false;

                var random = new Random();
                if (random.Next(0, 10) >= 8)
                {
                    var offset = random.Next(20, 80);
                    var offsetStr = new string(Enumerable.Repeat(' ', offset).ToArray());
                    labelPercent.Text = offsetStr + " ( )  ( )\n";
                    labelPercent.Text += offsetStr + "( o . o)";
                }

                if (!KKManager.Properties.Settings.Default.P2P_SettingsShown)
                {
                    using (var settingsDialog = new P2PSettingsDialog())
                        settingsDialog.ShowDialog(this);
                }

                olvColumnProgress.Renderer = new BarRenderer(0, 100);
                olvColumnProgress.AspectGetter = rowObject => (int)Math.Round(((UpdateDownloadItem)rowObject).FinishPercent);
                olvColumnSize.AspectGetter = rowObject => ((UpdateDownloadItem)rowObject).TotalSize;
                //olvColumnDownloaded.AspectGetter = rowObject => ((UpdateDownloadItem)rowObject).GetDownloadedSize();
                olvColumnStatus.AspectGetter = rowObject =>
                {
                    var item = (UpdateDownloadItem)rowObject;
                    return item.Status == UpdateDownloadStatus.Cancelled || item.Exceptions.Count == 0
                        ? item.Status.ToString()
                        : item.Status + " - " + string.Join("\n", item.GetFlattenedExceptions().Select(x => x.Message));
                };
                olvColumnName.AspectGetter = rowObject => ((UpdateDownloadItem)rowObject).DownloadPath.Name;
                olvColumnNo.AspectGetter = rowObject => ((UpdateDownloadItem)rowObject).Order;

                fastObjectListView1.PrimarySortColumn = olvColumnStatus;
                fastObjectListView1.SecondarySortColumn = olvColumnNo;
                fastObjectListView1.Sorting = SortOrder.Ascending;
                fastObjectListView1.PrimarySortOrder = SortOrder.Ascending;
                fastObjectListView1.SecondarySortOrder = SortOrder.Ascending;
                fastObjectListView1.ShowSortIndicators = true;
                fastObjectListView1.Sort();
                fastObjectListView1.ShowSortIndicator();

                _overallSize = _completedSize = FileSize.Empty;

                #endregion

                SetStatus("Preparing...");
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir() == false)
                    throw new OperationCanceledException();

                #region Find and select updates

                SetStatus("Searching for mod updates...");
                labelPercent.Text = "Please wait, this might take a few minutes.";
                if (KKManager.Properties.Settings.Default.P2P_Enabled)
                    labelPercent.Text += "\nIt can take over 10 minutes when P2P is enabled.";

                progressBar1.Maximum = 1000;
                progressBar1.Style = ProgressBarStyle.Blocks;

                var progress = new Progress<float>(p => progressBar1.Value = (int)Math.Round(p * 1000));

                var updateTasks = await UpdateSourceManager.GetUpdates(_cancelToken.Token, _updaters, _autoInstallGuids, false, progress);

                progressBar1.Value = 0;

                _cancelToken.Token.ThrowIfCancellationRequested();

                if (updateTasks.All(x => x.UpToDate))
                {
                    SetStatus("Everything is up to date!");
                    progressBar1.Value = progressBar1.Maximum;
                    _cancelToken.Cancel();
                    await TorrentUpdater.Start();
                    return;
                }

                var skipped = updateTasks.RemoveAll(x => x.UpToDate);
                var isAutoInstall = _autoInstallGuids != null && _autoInstallGuids.Length > 0;
                if (!isAutoInstall)
                {
                    SetStatus($"Found {updateTasks.Count} update tasks ({skipped} were already up-to-date), waiting for user confirmation.");
                    updateTasks = ModUpdateSelectDialog.ShowWindow(this, updateTasks);
                }
                else
                {
                    SetStatus($"Found {updateTasks.Count} update tasks in silent mode ({skipped} were already up-to-date).", true, true);
                }

                if (updateTasks == null)
                    throw new OperationCanceledException();

                #endregion

                await TorrentUpdater.Start();

                SleepControls.PreventSleepOrShutdown(Handle, "Update is in progress");

                #region Set up update downloader and start downloading

                downloadStartTime = DateTime.Now;

                downloader = UpdateDownloadCoordinator.Create(updateTasks, _cancelToken.Token);
                downloadItems = downloader.UpdateItems;

                SetStatus($"{downloadItems.Count(items => items.DownloadSources.Count > 1)} out of {downloadItems.Count} items have more than 1 source", false, true);

                fastObjectListView1.Objects = downloadItems;

                progressBar1.Maximum = 1000;
                progressBar1.Value = 0;
                checkBoxSleep.Enabled = true;

                _overallSize = FileSize.SumFileSizes(downloadItems.Select(x => x.TotalSize));


                updateTimer.Tick += DoStatusLabelUpdate;
                updateTimer.Start();

                SetStatus("Downloading updates...", true, true);

                await downloader.RunUpdate();

                updateTimer.Stop();

                _cancelToken.Token.ThrowIfCancellationRequested();

                #endregion

                #region Show finish messages

                var failedItems = downloadItems.Where(x => x.Status == UpdateDownloadStatus.Failed).ToList();
                var unfinishedCount = downloadItems.Count(x => x.Status != UpdateDownloadStatus.Finished);

                var s = $"Successfully updated/removed {downloadItems.Count - unfinishedCount} files from {updateTasks.Count} tasks.";
                if (failedItems.Any())
                    s += $"\nFailed to update {failedItems.Count} files because some sources crashed. Check log for details.";

                SetStatus(s, true, true);

                progressBar1.Value = progressBar1.Maximum;
                labelPercent.Text = "";

                if (failedItems.Any(x => x.Exceptions.Count > 0))
                {
                    var exceptionMessages = failedItems
                        .SelectMany(x => x.GetFlattenedExceptions())
                        .Where(y => !(y is DownloadSourceCrashedException))
                        // Deal with wrapped exceptions
                        .Select(y => y.Message.Contains("InnerException") && y.InnerException != null ? y.InnerException.Message : y.Message)
                        .Distinct();

                    var failDetails = "Reason(s) for failing:\n" + string.Join("\n", exceptionMessages);
                    Console.WriteLine(failDetails);
                    s += " " + failDetails;
                }

                // Sleep before showing a messagebox since the box will block until user clicks ok
                SleepIfNecessary();

                MessageBox.Show(s, "Finished updating", MessageBoxButtons.OK, MessageBoxIcon.Information);

                #endregion
            }
            catch (OutdatedVersionException ex)
            {
                SetStatus("KK Manager needs to be updated to get updates.", true, true);
                await TorrentUpdater.Stop();
                ex.ShowKkmanOutdatedMessage();
            }
            catch (OperationCanceledException)
            {
                SetStatus("Update was cancelled by the user.", true, true);
            }
            catch (Exception ex)
            {
                var exceptions = ex is AggregateException aex ? aex.Flatten().InnerExceptions : (ICollection<Exception>)new[] { ex };

                if (!exceptions.Any(x => x is OperationCanceledException))
                    SleepIfNecessary();

                SetStatus("Unexpected crash while updating mods, aborting.", true, true);
                SetStatus(string.Join("\n---\n", exceptions), false, true);
                MessageBox.Show("Something unexpected happened and the update could not be completed. Make sure that your internet connection is stable, " +
                                "and that you did not hit your download limits, then try again.\n\nError message (check log for more):\n" + string.Join("\n", exceptions.Select(x => x.Message)),
                                "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TorrentUpdater.Stop();
            }
            finally
            {
                updateTimer.Tick -= DoStatusLabelUpdate;
                updateTimer.Stop();

                checkBoxSleep.Enabled = false;

                fastObjectListView1.EmptyListMsg = "Nothing was downloaded";

                _cancelToken.Cancel();

                labelPercent.Text = "";

                string topText;
                if (_completedSize != FileSize.Empty)
                {
                    topText = $"Downloaded {_completedSize} out of {_overallSize}";
                    if (downloadStartTime != DateTime.MinValue)
                    {
                        var timeSpent = DateTime.Now - downloadStartTime;
                        topText += $" in {timeSpent.GetReadableTimespan()}";
                    }
                }
                else
                {
                    topText = "Nothing was downloaded";
                }

                if (TorrentUpdater.GetCurrentUpload() != null)
                {
                    updateTimer.Tick += (o, args) =>
                    {
                        labelPercent.Text = topText;
                        var uploadSpeed = TorrentUpdater.GetCurrentUpload();
                        if (uploadSpeed.HasValue) labelPercent.Text += $"\nSeeding {FileSize.FromBytes(uploadSpeed.Value)}/s to {TorrentUpdater.GetPeerCount()} peers (Close this to stop)";
                    };
                    updateTimer.Start();
                }
                else
                {
                    var averageDlSpeed = averageDownloadSpeed.GetAverage();
                    if (averageDlSpeed > 0)
                        topText += $"\nAverage download speed: {new FileSize(averageDlSpeed)}/s)";
                }

                labelPercent.Text = topText;

                progressBar1.Style = ProgressBarStyle.Blocks;
                button1.Enabled = true;
                button1.Text = "OK";

                downloader?.Dispose();

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

        protected override async void OnClosed(EventArgs e)
        {
            _cancelToken.Cancel();

            await TorrentUpdater.Stop();
            await Task.Delay(200);

            await RemoveTempDownloadDirectory();

            base.OnClosed(e);
        }

        private static async Task RemoveTempDownloadDirectory()
        {
            var downloadDirectory = UpdateItem.GetTempDownloadDirectory();
            try
            {
                if (Directory.Exists(downloadDirectory))
                    Directory.Delete(downloadDirectory, true);
            }
            catch
            {
                await Task.Delay(500);
                try
                {
                    if (Directory.Exists(downloadDirectory))
                        Directory.Delete(downloadDirectory, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
