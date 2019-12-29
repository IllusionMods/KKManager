using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Windows
{
    public partial class ModUpdateProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private IUpdateSource[] _updaters;
        private FileSize _overallSize;
        private FileSize _completedSize;

        private ModUpdateProgressDialog()
        {
            InitializeComponent();
        }

        public bool Silent { get; set; }

        public static void StartUpdateDialog(Form owner, params IUpdateSource[] updaters)
        {
            using (var w = CreateUpdateDialog(updaters))
            {
                w.Icon = owner.Icon;
                w.StartPosition = FormStartPosition.CenterParent;
                w.ShowDialog(owner);
            }
        }

        public static ModUpdateProgressDialog CreateUpdateDialog(params IUpdateSource[] updaters)
        {
            if (updaters == null || updaters.Length == 0) throw new ArgumentException("Need at least one update source.", nameof(updaters));

            var w = new ModUpdateProgressDialog();
            w._updaters = updaters;
            return w;
        }

        private async void ModUpdateProgress_Shown(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Value = 0;
                progressBar1.Maximum = 1;

                labelProgress.Text = "N/A";
                labelPercent.Text = "";

                SetStatus("Searching for mod updates...");
                var updateTasks = await UpdateSourceManager.GetUpdates(_cancelToken.Token, _updaters);

                _cancelToken.Token.ThrowIfCancellationRequested();

                progressBar1.Style = ProgressBarStyle.Blocks;

                if (updateTasks.All(x => x.UpToDate))
                {
                    SetStatus("Everything is up to date!");
                    progressBar1.Value = progressBar1.Maximum;
                    _cancelToken.Cancel();
                    return;
                }

                if (!Silent)
                {
                    SetStatus($"Found {updateTasks.Count} updates, waiting for user confirmation.");
                    updateTasks = ModUpdateSelectDialog.ShowWindow(this, updateTasks);
                }
                else
                {
                    var skipped = updateTasks.RemoveAll(x => x.UpToDate || !x.EnableByDefault);
                    SetStatus($"Found {updateTasks.Count} updates in silent mode, skipped {skipped}.", true, true);
                }

                if (updateTasks == null)
                    throw new OperationCanceledException();

                _overallSize = FileSize.SumFileSizes(updateTasks.Select(x => x.TotalUpdateSize));
                _completedSize = FileSize.Empty;

                var allItems = updateTasks.SelectMany(x => x.GetUpdateItems())
                    // Try items with a single source first since they are the most risky
                    .OrderBy(sources => sources.Count())
                    .ToList();

                SetStatus($"{allItems.Count(items => items.Count() > 1)} out of {allItems.Count} items have more than 1 source", false, true);

                progressBar1.Maximum = allItems.Count;

                for (var index = 0; index < allItems.Count; index++)
                {
                    _cancelToken.Token.ThrowIfCancellationRequested();

                    var task = allItems[index];

                    labelProgress.Text = (index + 1) + " / " + allItems.Count;
                    SetStatus("Downloading " + task.First().Item2.TargetPath.Name);

                    if (await UpdateSingleItem(task))
                        _completedSize += task.First().Item2.ItemSize;

                    progressBar1.Value = index + 1;
                }

                var s = $"Successfully updated/removed {allItems.Count} files from {updateTasks.Count} tasks.";
                if (_failedItems.Any())
                {
                    s += $"\n\nFailed to update {_failedItems.Count} files because some of the sources raised errors.";
                    if (_failedExceptions.Any())
                        s += " Reason(s) for failing:\n" + string.Join("\n", _failedExceptions.Select(x => x.Message).Distinct());
                }
                SetStatus(s, true, true);
                MessageBox.Show(s, "Finished updating", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                SetStatus("Operation was cancelled by the user.", true, true);
            }
            catch (Exception ex)
            {
                var exceptions = ex is AggregateException aex ? aex.Flatten().InnerExceptions : (ICollection<Exception>)new[] { ex };

                SetStatus("Crash while updating mods, aborting.", true, true);
                SetStatus(string.Join("\n---\n", exceptions), false, true);
                MessageBox.Show("Something unexpected happened and the update could not be completed. Make sure that your internet connection is stable, " +
                                "and that you did not hit your download limit on Mega, then try again.\n\nError message (check log for more):\n" + string.Join("\n", exceptions.Select(x => x.Message)),
                                "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cancelToken.Cancel();

                labelPercent.Text = _completedSize == FileSize.Empty ? "" : $"Downloaded {_completedSize} out of {_overallSize}.";

                progressBar1.Style = ProgressBarStyle.Blocks;
                button1.Enabled = true;
                button1.Text = "OK";
            }
        }

        private readonly List<UpdateInfo> _badUpdateSources = new List<UpdateInfo>();
        private readonly List<IGrouping<string, Tuple<UpdateInfo, IUpdateItem>>> _failedItems = new List<IGrouping<string, Tuple<UpdateInfo, IUpdateItem>>>();
        private readonly List<Exception> _failedExceptions = new List<Exception>();

        private async Task<bool> UpdateSingleItem(IGrouping<string, Tuple<UpdateInfo, IUpdateItem>> task)
        {
            var firstItem = task.First().Item2;
            var progress = new Progress<double>(d => labelPercent.Text = $"Downloaded {d:F1}% of {firstItem.ItemSize}.  Overall: {_completedSize} / {_overallSize}.");

            SetStatus($"Updating {firstItem.TargetPath.Name}");
            SetStatus($"Updating {InstallDirectoryHelper.GetRelativePath(firstItem.TargetPath)}", false, true);

            // todo move logging to update methods
            if (firstItem.TargetPath.Exists)
            {
                SetStatus($"Deleting old file {firstItem.TargetPath.FullName}", false, true);
                firstItem.TargetPath.Delete();
            }

            var sourcesToAttempt = task.Where(x => !_badUpdateSources.Contains(x.Item1)).OrderByDescending(x => x.Item1.SourcePriority).ToList();
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
                    SetStatus($"Attempting download from source {source.Item1.Origin}", false, true);

                    await RetryHelper.RetryOnExceptionAsync(() => source.Item2.Update(progress, _cancelToken.Token), 3, TimeSpan.FromSeconds(3), _cancelToken.Token);

                    ex = null;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Marking source {source.Item1.Origin} as broken because of exception: {e}");

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

            SetStatus($"Download OK {firstItem.ItemSize}", false, true);
            return true;
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
                _cancelToken.Cancel();
                button1.Enabled = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cancelToken.Cancel();
            base.OnClosed(e);
        }
    }
}
