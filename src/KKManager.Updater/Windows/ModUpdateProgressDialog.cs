using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Functions;
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
            var averageDownloadSpeed = new MovingAverage(20);
            var downloadStartTime = DateTime.MinValue;
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

                olvColumnProgress.Renderer = new BarRenderer(0, 100);
                olvColumnProgress.AspectGetter = rowObject => (int)Math.Round(((UpdateDownloadItem)rowObject).FinishPercent);
                olvColumnSize.AspectGetter = rowObject => ((UpdateDownloadItem)rowObject).TotalSize;
                //olvColumnDownloaded.AspectGetter = rowObject => ((UpdateDownloadItem)rowObject).GetDownloadedSize();
                olvColumnStatus.AspectGetter = rowObject =>
                {
                    var item = (UpdateDownloadItem)rowObject;
                    return item.Exceptions.Count == 0
                        ? item.Status.ToString()
                        : item.Status + " - " + string.Join("; ", item.Exceptions.Select(x => x.Message));
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
                labelPercent.Text = "Please wait, this might take a couple of minutes.";
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

                #endregion

                SleepControls.PreventSleepOrShutdown(Handle, "Update is in progress");

                #region Set up update downloader and start downloading

                downloadStartTime = DateTime.Now;

                var downloader = UpdateDownloadCoordinator.Create(updateTasks);
                var downloadItems = downloader.UpdateItems;

                SetStatus($"{downloadItems.Count(items => items.DownloadSources.Count > 1)} out of {downloadItems.Count} items have more than 1 source", false, true);

                fastObjectListView1.Objects = downloadItems;

                progressBar1.Maximum = 1000;
                progressBar1.Value = 0;
                checkBoxSleep.Enabled = true;

                _overallSize = FileSize.SumFileSizes(downloadItems.Select(x => x.TotalSize));

                var lastCompletedSize = FileSize.Empty;
                updateTimer.Tick += (o, args) =>
                {
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

                    var totalPercent = (double)_completedSize.GetKbSize() / (double)_overallSize.GetKbSize() * 100d;
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

                    labelPercent.Text =
                        $"Overall: {totalPercent:F1}% done  ({_completedSize} out of {_overallSize})\r\n" +
                        $"Speed: {downloadedSinceLast}/s  (ETA: {eta})";
                    //$"Speed: {downloadedSinceLast:F1}KB/s";

                    progressBar1.Value = Math.Min((int)Math.Round(totalPercent * 10), progressBar1.Maximum);
                };
                updateTimer.Start();

                SetStatus("Downloading updates...", true, true);

                await downloader.RunUpdate(_cancelToken.Token);

                _cancelToken.Token.ThrowIfCancellationRequested();

                #endregion

                #region Show finish messages

                var failedItems = downloadItems.Where(x => x.Status == UpdateDownloadStatus.Failed).ToList();
                var unfinishedCount = downloadItems.Count(x => x.Status != UpdateDownloadStatus.Finished);

                var s = $"Successfully updated/removed {downloadItems.Count - unfinishedCount} files from {updateTasks.Count} tasks.";
                if (failedItems.Any())
                    s += $"\nFailed to update {failedItems.Count} files because some sources crashed. Check log for details.";

                SetStatus(s, true, true);

                updateTimer.Stop();
                progressBar1.Value = progressBar1.Maximum;
                labelPercent.Text = "";

                if (failedItems.Any(x => x.Exceptions.Count > 0))
                {
                    var exceptionMessages = failedItems
                        .SelectMany(x => x.Exceptions)
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
            }
            finally
            {
                updateTimer.Stop();
                checkBoxSleep.Enabled = false;

                fastObjectListView1.EmptyListMsg = "Nothing was downloaded";

                _cancelToken.Cancel();

                labelPercent.Text = "";
                if (_completedSize != FileSize.Empty)
                {
                    labelPercent.Text += $"Downloaded {_completedSize} out of {_overallSize}";
                    if (downloadStartTime != DateTime.MinValue)
                    {
                        var timeSpent = DateTime.Now - downloadStartTime;
                        labelPercent.Text += $" in {timeSpent.GetReadableTimespan()}";
                    }
                    labelPercent.Text += "\n";
                }
                var averageDlSpeed = averageDownloadSpeed.GetAverage();
                if (averageDlSpeed > 0)
                    labelPercent.Text += $"Average download speed: {new FileSize(averageDlSpeed)}/s";

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
