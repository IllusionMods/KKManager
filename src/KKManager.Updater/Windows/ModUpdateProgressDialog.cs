using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;
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
        private readonly List<KeyValuePair<FileInfo, string>> _reEnabledMods = new List<KeyValuePair<FileInfo, string>>();
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
            //w._updaters = w._updaters.Where(x => x.Origin.Contains("hf.honeyselect2.com")).ToArray(); //todo remove or improve
#endif
            w._autoInstallGuids = autoInstallGuids;
            return w;
        }

        private async void ModUpdateProgress_Shown(object sender, EventArgs e)
        {
            var averageDownloadSpeed = new MovingAverage(25);
            var averageDownloadSpeedFast = new MovingAverage(4);
            var downloadStartTime = DateTime.Now;
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
                averageDownloadSpeedFast.Sample(downloadedSinceLast.GetKbSize());
                var etaSeconds = (_overallSize - _completedSize).GetKbSize() / (double)averageDownloadSpeed.GetAverage();
                var eta = double.IsNaN(etaSeconds) || etaSeconds < 0 || etaSeconds > TimeSpan.MaxValue.TotalSeconds
                    ? KKManager.Properties.Resources.Unknown
                    : TimeSpan.FromSeconds(etaSeconds).GetReadableTimespan();

                var text = $"Overall: {totalPercent:F1}% done  ({_completedSize} out of {_overallSize})";

                var uploadSpeed = TorrentUpdater.GetCurrentUpload();
                if (uploadSpeed.HasValue) text += $" | Seeding: {FileSize.FromBytes(uploadSpeed.Value)}/s";

                text += $"\r\nSpeed: {FileSize.FromKilobytes(averageDownloadSpeedFast.GetAverage())}/s  (ETA: {eta})";

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

                SetStatus(Resources.ModUpdateProgress_Preparing);
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir() == false)
                    throw new OperationCanceledException();

                #region Find and select updates

                SetStatus(Resources.ModUpdateProgress_Searching);
                labelPercent.Text = Resources.ModUpdateProgress_PleaseWait;
                if (KKManager.Properties.Settings.Default.P2P_Enabled)
                    labelPercent.Text += '\n' + Resources.ModUpdateProgress_PleaseWait_P2P;

                progressBar1.Maximum = 1000;
                progressBar1.Style = ProgressBarStyle.Blocks;

                var progress = new Progress<float>(p => this.SafeInvoke(() => progressBar1.Value = (int)Math.Round(p * 1000)));

                // Re-enable any disabled zipmods and plugins before searching for updates so that they don't get treated as missing
                foreach (var fileInfo in InstallDirectoryHelper.PluginPath.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (PluginLoader.IsDisabledPlugin(fileInfo.Extension, out var enabledExtension))
                        AddReEnabledMod(fileInfo, enabledExtension);
                }
                foreach (var fileInfo in InstallDirectoryHelper.ModsPath.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (SideloaderModLoader.IsDisabledZipmod(fileInfo.Extension, out var enabledExtension))
                        AddReEnabledMod(fileInfo, enabledExtension);
                }

                var updateTasks = await Task.Run(() => UpdateSourceManager.GetUpdates(_cancelToken.Token, _updaters, _autoInstallGuids, false, progress));

                progressBar1.Value = 0;

                _cancelToken.Token.ThrowIfCancellationRequested();

                // If no update tasks are found (empty collection) or all tasks are already up-to-date,
                // treat this as all tasks being up-to-date and proceed accordingly.
                if (updateTasks.Count == 0 || updateTasks.All(x => x.UpToDate))
                {
                    SetStatus(Resources.ModUpdateProgress_AllUpToDate);
                    progressBar1.Value = progressBar1.Maximum;
                    await TorrentUpdater.Start();
                    return;
                }

                var skipped = updateTasks.RemoveAll(x => x.UpToDate);
                var isAutoInstall = _autoInstallGuids != null && _autoInstallGuids.Length > 0;
                if (!isAutoInstall)
                {
                    SetStatus(string.Format(Resources.ModUpdateProgress_UpdatesFoundConfirmation, updateTasks.Count, skipped));
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

                SetStatus(Resources.ModUpdateProgress_Downloading, true, true);

                await downloader.RunUpdate();

                updateTimer.Stop();

                _cancelToken.Token.ThrowIfCancellationRequested();

                #endregion

                #region Show finish messages

                var failedItems = downloadItems.Where(x => x.Status == UpdateDownloadStatus.Failed).ToList();
                var unfinishedCount = downloadItems.Count(x => x.Status != UpdateDownloadStatus.Finished);

                var s = string.Format(Resources.ModUpdateProgress_Finished_Main, downloadItems.Count - unfinishedCount, updateTasks.Count);
                if (failedItems.Any())
                    s += '\n' + string.Format(Resources.ModUpdateProgress_Finished_Fails, failedItems.Count);

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

                    var failDetails = Resources.ModUpdateProgress_Finished_FailReasons + "\n" + string.Join("\n", exceptionMessages);
                    Console.WriteLine(failDetails);
                    s += " " + failDetails;
                }

                // Sleep before showing a messagebox since the box will block until user clicks ok
                SleepIfNecessary();

                WindowUtils.FlashWindow(Handle);

                MessageBox.Show(s, Resources.ModUpdateProgress_Finished_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);

                #endregion
            }
            catch (OutdatedVersionException ex)
            {
                SetStatus(Resources.ModUpdateProgress_Failed_Outdated, true, true);

                await TorrentUpdater.Stop();

                WindowUtils.FlashWindow(Handle);

                ex.ShowKkmanOutdatedMessage();
            }
            catch (OperationCanceledException)
            {
                SetStatus(Resources.ModUpdateProgress_Failed_CancelledByUser, true, true);
            }
            catch (Exception ex)
            {
                var exceptions = ex is AggregateException aex ? aex.Flatten().InnerExceptions : (ICollection<Exception>)new[] { ex };

                SetStatus(Resources.ModUpdateProgress_Failed_Unexpected, true, true);
                SetStatus(string.Join("\n---\n", exceptions), false, true);

                await TorrentUpdater.Stop();

                if (!exceptions.Any(x => x is OperationCanceledException))
                    SleepIfNecessary();

                WindowUtils.FlashWindow(Handle);

                MessageBox.Show(string.Format(Resources.ModUpdateProgress_Failed_Unexpected_Message, string.Join("\n", exceptions.Select(x => x.Message))),
                                Resources.ModUpdateProgress_Failed_Unexpected_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                updateTimer.Tick -= DoStatusLabelUpdate;
                updateTimer.Stop();

                checkBoxSleep.Enabled = false;

                fastObjectListView1.EmptyListMsg = Resources.ModUpdateProgress_NothingWasDownloaded;

                _cancelToken.Cancel();

                labelPercent.Text = "";

                string topText;
                if (_completedSize != FileSize.Empty)
                {
                    topText = string.Format(Resources.ModUpdateProgress_DownloadFinishedStats, _completedSize, _overallSize, (DateTime.Now - downloadStartTime).GetReadableTimespan());
                }
                else
                {
                    topText = Resources.ModUpdateProgress_NothingWasDownloaded;
                }

                if (TorrentUpdater.GetCurrentUpload() != null)
                {
                    updateTimer.Tick += (o, args) =>
                    {
                        var uploadSpeed = TorrentUpdater.GetCurrentUpload();
                        labelPercent.Text = uploadSpeed.HasValue
                            ? topText + '\n' + string.Format(Resources.ModUpdateProgress_DownloadFinished_Seeding, FileSize.FromBytes(uploadSpeed.Value), TorrentUpdater.GetPeerCount())
                            : topText;
                    };
                    updateTimer.Start();
                }
                else
                {
                    var averageDlSpeed = averageDownloadSpeed.GetAverage();
                    if (averageDlSpeed > 0)
                        topText += $"\n" + string.Format(Resources.ModUpdateProgress_DownloadFinished_Average, new FileSize(averageDlSpeed));
                }

                labelPercent.Text = topText;

                progressBar1.Style = ProgressBarStyle.Blocks;
                buttonCancelClose.Enabled = true;
                buttonCancelClose.Text = Resources.ModUpdateProgress_OKbutton;

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

        private void AddReEnabledMod(FileInfo fileInfo, string enabledExtension)
        {
            try
            {
                var enabledPath = fileInfo.GetFullNameWithDifferentExtension(enabledExtension);
                if (File.Exists(enabledPath)) return;
                var originalPath = fileInfo.FullName;
                Console.WriteLine($"Temporarily re-enabling mod: {originalPath}");
                fileInfo.MoveTo(enabledPath);
                _reEnabledMods.Add(new KeyValuePair<FileInfo, string>(fileInfo, originalPath));
            }
            catch (Exception e)
            {
                // Safe to ignore the error since last line it can throw at is the MoveTo, so it doesnt get moved and doesn't get added to the list.
                Console.WriteLine(e);
            }
        }
        private void UndoReEnabledMods()
        {
            foreach (var disabledMod in _reEnabledMods)
            {
                try
                {
                    var fileInfo = disabledMod.Key;
                    var originalPath = disabledMod.Value;
                    if (File.Exists(originalPath)) continue;
                    Console.WriteLine($"Re-disabling mod: {originalPath}");
                    fileInfo.MoveTo(originalPath);
                }
                catch (Exception e)
                {
                    // This shouldn't happen
                    Console.WriteLine(e);
                }
            }
            _reEnabledMods.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_cancelToken.IsCancellationRequested)
            {
                UseWaitCursor = true;
                SetStatus(Resources.ModUpdateProgress_Finishing);
                labelPercent.Text = Resources.ModUpdateProgress_ThisCouldTakeAMinute;
                Application.DoEvents();
                Close();
            }
            else
            {
                buttonCancelClose.Enabled = false;
                _cancelToken.Cancel();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Task.Run(Finish).GetAwaiter().GetResult();

            UseWaitCursor = false;

            _logPopup?.Dispose();

            base.OnClosed(e);
        }

        private async Task Finish()
        {
            _cancelToken.Cancel();

            await TorrentUpdater.Stop();
            await Task.Delay(200);

            await RemoveTempDownloadDirectory();

            UndoReEnabledMods();
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

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private LogPopup _logPopup;
        private void buttonViewLog_Click(object sender, EventArgs e)
        {
            if (_logPopup != null && _logPopup.Visible)
            {
                if (_logPopup.WindowState == FormWindowState.Minimized)
                    _logPopup.WindowState = FormWindowState.Normal;

                _logPopup.BringToFront();
                _logPopup.Focus();
                return;
            }

            _logPopup?.Dispose();

            _logPopup = new LogPopup();
            _logPopup.Icon = Icon;
            _logPopup.Show();
        }
    }
}
