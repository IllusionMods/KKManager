using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Functions.Update;
using KKManager.Util;

namespace KKManager.Windows
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

                var allItems = updateTasks.SelectMany(x => x.Items).ToList();

                progressBar1.Maximum = allItems.Count;

                for (var index = 0; index < allItems.Count; index++)
                {
                    _cancelToken.Token.ThrowIfCancellationRequested();

                    var task = allItems[index];

                    labelProgress.Text = (index + 1) + " / " + allItems.Count;
                    SetStatus("Downloading " + task.TargetPath.Name);

                    await UpdateSingleItem(task);

                    _completedSize += task.ItemSize;
                    progressBar1.Value = index + 1;
                }

                var s = $"Successfully updated/removed {allItems.Count} files from {updateTasks.Count} tasks!";
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

        private async Task UpdateSingleItem(IUpdateItem task)
        {
            var progress = new Progress<double>(d => labelPercent.Text = $"Downloaded {d:F1}% of {task.ItemSize}.  Overall: {_completedSize} / {_overallSize}.");

            SetStatus($"Updating {InstallDirectoryHelper.GetRelativePath(task.TargetPath)}");

            // todo move logging to update methods
            if (task.TargetPath.Exists)
            {
                SetStatus($"Deleting old file {task.TargetPath.FullName}", false, true);
                task.TargetPath.Delete();
            }

            SetStatus($"Updating {InstallDirectoryHelper.GetRelativePath(task.TargetPath)}", false, true);

            await RetryHelper.RetryOnExceptionAsync(() => task.Update(progress, _cancelToken.Token), 3, TimeSpan.FromSeconds(3), _cancelToken.Token);

            SetStatus($"Download OK {task.ItemSize}", false, true);
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
