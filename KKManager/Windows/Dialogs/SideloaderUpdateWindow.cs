using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Functions;

namespace KKManager.Windows.Dialogs
{
	public partial class SideloaderUpdateWindow : Form
	{
		private SideloaderModpackUpdater _updater;
		private List<SideloaderUpdateTask> _updateTasks;

		public SideloaderUpdateWindow()
		{
			InitializeComponent();

			objectListView1.EmptyListMsg = "All mods are up to date!";
			objectListView1.FormatRow += ObjectListView1_FormatRow;
		}

		public static void ShowWindow()
		{
			if (MessageBox.Show("Will now connect to the mega folder and compare it to local files, this can take a while", "hello", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK) return;

			try
			{
				var updater = SideloaderModpackUpdater.ConnectNew();

				var updateTasks = updater.GetUpdateTasks()
					.OrderBy(x => x.UpToDate)
					.ThenBy(x => x.DisplayName)
					.ToList();

				using (var w = new SideloaderUpdateWindow())
				{
					w._updateTasks = updateTasks;
					w._updater = updater;
					w.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Failed to get updates", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			_updater?.Dispose();
			base.OnClosed(e);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			UpdateListObjects(this, e);
			SelectAll(this, e);
		}

		private void buttonAccept_Click(object sender, EventArgs e)
		{
			Enabled = false;
			Application.DoEvents();

			var targets = objectListView1.CheckedObjects.Cast<SideloaderUpdateTask>().Where(x => !x.UpToDate).ToList();

			foreach (var task in targets)
			{
				if (task.LocalExists)
				{
					Console.WriteLine($"Deleting old file {task.DisplayName}");
					task.LocalFile.Delete();
				}

				if (!task.RemoteExists) continue;

				task.LocalFile.Directory.Create();

				Console.WriteLine($"Downloading {task.RemoteFile}\nto {task.DisplayName}");

				try
				{
					_updater.DownloadNode(task.LocalFile.FullName, task.RemoteFile);

					Console.WriteLine($"Finished");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error while downloading " + ex);
					MessageBox.Show(ex.ToString(), "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			MessageBox.Show($"Updated/removed {targets.Count} mods", "Finished updating", MessageBoxButtons.OK, MessageBoxIcon.Information);

			Close();
		}

		private void ObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
		{
			var task = e.Model as SideloaderUpdateTask;
			if (task == null) return;

			if (!task.RemoteExists)
				e.Item.ForeColor = Color.DarkRed;
			else if (!task.LocalExists)
				e.Item.ForeColor = Color.Green;
		}

		private void UpdateListObjects(object sender, EventArgs e)
		{
			if (_updateTasks == null)
				objectListView1.ClearObjects();
			else if (checkBoxShowDone.Checked)
			{
				objectListView1.SetObjects(_updateTasks);
				objectListView1.DisableObjects(_updateTasks.Where(x => x.UpToDate));
			}
			else
				objectListView1.SetObjects(_updateTasks.Where(x => !x.UpToDate).ToList());
		}

		private void SelectAll(object sender, EventArgs e)
		{
			objectListView1.UncheckAll();
			objectListView1.CheckObjects(_updateTasks.Where(x => !x.UpToDate));
		}

		private void SelectNone(object sender, EventArgs e)
		{
			objectListView1.UncheckAll();
		}
	}
}
