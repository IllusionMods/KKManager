using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions.Update;

namespace KKManager.Windows.Dialogs
{
    public partial class ModUpdateProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private MegaUpdater _megaUpdater;

        private ModUpdateProgressDialog()
        {
            InitializeComponent();
        }

        public static void StartUpdateDialog(Form owner, MegaUpdater updater)
        {
            using (var w = new ModUpdateProgressDialog())
            {
                w.Icon = owner.Icon;
                w.StartPosition = FormStartPosition.CenterParent;
                w._megaUpdater = updater;
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

                SetStatus("Searching for mod updates...");

                var updateTasks = await _megaUpdater.GetUpdateTasksAsync();

                SetStatus($"Found {updateTasks.Count} updates, waiting for user confirmation.");
                updateTasks = ModUpdateSelectDialog.ShowWindow(this, updateTasks);

                progressBar1.Style = ProgressBarStyle.Blocks;

                if (updateTasks == null)
                {
                    _cancelToken.Cancel();
                }
                else
                {
                    progressBar1.Maximum = updateTasks.Count;

                    for (var index = 0; index < updateTasks.Count; index++)
                    {
                        _cancelToken.Token.ThrowIfCancellationRequested();

                        var task = updateTasks[index];

                        labelProgress.Text = (index + 1) + " / " + updateTasks.Count;

                        await UpdateSingleItem(task);

                        progressBar1.Value = index + 1;
                    }
                }

                var s = $"Successfully updated/removed {updateTasks?.Count ?? 0} mods!";
                SetStatus(s, true, true);
                MessageBox.Show(s, "Finished updating", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException)
            {
                SetStatus("Operation was cancelled by the user.", true, true);
            }
            catch (Exception ex)
            {
                progressBar1.Style = ProgressBarStyle.Blocks;

                var exceptions = ex is AggregateException aex ? aex.Flatten().InnerExceptions : (ICollection<Exception>)new[] { ex };

                SetStatus("Crash while updating mods, aborting.", true, true);
                SetStatus(string.Join("\n---\n", exceptions), false, true);
                MessageBox.Show("Something unexpected happened and the update could not be completed. Make sure that your internet connection is stable, " +
                                "and that you did not hit your download limit on Mega, then try again.\n\nError message (check log for more):\n" + string.Join("\n", exceptions.Select(x => x.Message)),
                                "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            button1.Enabled = true;
            button1.Text = "OK";
        }

        private async Task UpdateSingleItem(SideloaderUpdateItem task)
        {
            var progress = new Progress<double>(d => SetStatus($"{d:F1}% - Downloading {task.Name}"));

            SetStatus($"Preparing to download {task.Name}");

            if (task.LocalExists)
            {
                SetStatus($"Deleting old file {task.RelativePath}", false, true);
                task.LocalFile.Delete();
            }

            if (task.RemoteExists)
            {
                task.LocalFile.Directory?.Create();

                SetStatus($"Downloading {task.RemoteFile}\nto {task.RelativePath}", false, true);

                await _megaUpdater.DownloadNodeAsync(task, progress, _cancelToken.Token);

                SetStatus("Finished", false, true);
            }
        }

        private void SetStatus(string status, bool writeToUi = true, bool writeToLog = false)
        {
            if (writeToUi)
                labelStatus.Text = status;
            if (writeToLog)
                Console.WriteLine(status);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _cancelToken.Cancel();
            button1.Enabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            _cancelToken.Cancel();
            base.OnClosed(e);
        }
    }
}
