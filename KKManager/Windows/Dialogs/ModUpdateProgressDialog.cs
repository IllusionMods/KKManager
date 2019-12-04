using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions.Update;
using KKManager.Util;

namespace KKManager.Windows.Dialogs
{
    public partial class ModUpdateProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private IUpdateSource _updater;
        private FileSize _overallSize;
        private FileSize _completedSize;

        private ModUpdateProgressDialog()
        {
            InitializeComponent();
        }

        public static void StartUpdateDialog(Form owner, IUpdateSource updater)
        {
            using (var w = new ModUpdateProgressDialog())
            {
                w.Icon = owner.Icon;
                w.StartPosition = FormStartPosition.CenterParent;
                w._updater = updater;
                w.ShowDialog(owner);
            }
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
                
                var updateTasks = await _updater.GetUpdateItems(_cancelToken.Token);

                _cancelToken.Token.ThrowIfCancellationRequested();

                progressBar1.Style = ProgressBarStyle.Blocks;

                if (updateTasks.All(x => x.UpToDate))
                {
                    SetStatus("Everything is up to date!");
                    progressBar1.Value = progressBar1.Maximum;
                    _cancelToken.Cancel();
                    return;
                }

                SetStatus($"Found {updateTasks.Count} updates, waiting for user confirmation.");
                updateTasks = ModUpdateSelectDialog.ShowWindow(this, updateTasks);

                if (updateTasks == null)
                    throw new OperationCanceledException();

                _overallSize = FileSize.SumFileSizes(updateTasks.Select(x => x.Items.Select(i=>i.)));
                _completedSize = FileSize.Empty;

                progressBar1.Maximum = updateTasks.Count;

                for (var index = 0; index < updateTasks.Count; index++)
                {
                    _cancelToken.Token.ThrowIfCancellationRequested();

                    var task = updateTasks[index];

                    labelProgress.Text = (index + 1) + " / " + updateTasks.Count;
                    SetStatus("Downloading " + task.Name);

                    await UpdateSingleItem(task);

                    _completedSize += task.Size;
                    progressBar1.Value = index + 1;
                }

                var s = $"Successfully updated/removed {updateTasks.Count} mods!";
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

        private async Task UpdateSingleItem(SideloaderUpdateItem task)
        {
            var progress = new Progress<double>(d => labelPercent.Text = $"Downloaded {d:F1}% of {task.Size}.  Overall: {_completedSize} / {_overallSize}.");

            SetStatus($"Updating {task.Name}");

            if (task.LocalExists)
            {
                SetStatus($"Deleting old file {task.RelativePath}", false, true);
                task.LocalFile.Delete();
            }

            if (task.RemoteExists)
            {
                task.LocalFile.Directory?.Create();

                SetStatus($"Downloading to {task.RelativePath}", false, true);

                await _updater.DownloadNodeAsync(task, progress, _cancelToken.Token);

                SetStatus($"Download OK {task.Size}", false, true);
            }
        }

        private void SetStatus(string status, bool writeToUi = true, bool writeToLog = false)
        {
            if (writeToUi)
                labelStatus.Text = status;
            if (writeToLog)
                Console.WriteLine("[Updater]" + status);
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
