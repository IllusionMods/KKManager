using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Data.Scenes;
using KKManager.Functions;
using KKManager.Util;
using KKManager.Windows.Dialogs;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.Content
{
    /// <summary>Browser for Studio scenes and their Sideloader requirements.</summary>
    public sealed class SceneWindow : DockContent, IContentWindow
    {
        private readonly FastObjectListView _list = new FastObjectListView();
        private readonly ToolStripButton _install = new ToolStripButton("Install missing zipmods") { DisplayStyle = ToolStripItemDisplayStyle.Text, Enabled = false };
        private readonly ToolStripButton _checkboxes = new ToolStripButton("Checkboxes") { DisplayStyle = ToolStripItemDisplayStyle.Text, CheckOnClick = true };
        private readonly OLVColumn _relativePath = new OLVColumn("Path", "Location.FullName") { MinimumWidth = 180 };
        private CancellationTokenSource _cancel;

        public SceneWindow()
        {
            Text = "Scenes";
            ToolTipText = InstallDirectoryHelper.SceneDir;
            ShowHint = DockState.Document;

            var name = new OLVColumn("Scene", "Name") { MinimumWidth = 180 };
            var modified = new OLVColumn("Modified", "Location.LastWriteTime") { MinimumWidth = 120 };
            var size = new OLVColumn("Size", "FileSize") { MinimumWidth = 80 };
            var used = new OLVColumn("Zipmods", "UsedZipmods.Length") { MinimumWidth = 75 };
            var missing = new OLVColumn("Missing", "MissingZipmods.Length") { MinimumWidth = 75 };
            _list.AllColumns.AddRange(new[] { name, modified, size, used, missing, _relativePath });
            _list.Columns.AddRange(new ColumnHeader[] { name, modified, size, used, missing, _relativePath });
            _list.Dock = DockStyle.Fill;
            _list.FullRowSelect = true;
            _list.GridLines = true;
            _list.HideSelection = false;
            _list.ShowGroups = false;
            _list.UseFiltering = true;
            _list.EmptyListMsg = "No Studio scenes were found";
            _list.EmptyListMsgFont = new Font(Font.FontFamily, 18);
            _list.SelectedIndexChanged += (_, _) => { ShowSelectedProperties(); UpdateInstallButton(); };
            _list.ItemChecked += (_, _) => UpdateInstallButton();
            _list.FormatRow += (_, args) =>
            {
                if (args.Model is Scene scene && scene.MissingZipmods?.Length > 0)
                    args.Item.BackColor = Color.MistyRose;
            };
            _relativePath.AspectGetter = row => row is Scene scene
                ? scene.Location.FullName.Substring(InstallDirectoryHelper.SceneDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                : string.Empty;

            var toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };
            var refresh = new ToolStripButton("Refresh") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            refresh.Click += (_, _) => RefreshList();
            _checkboxes.CheckedChanged += (_, _) => { _list.CheckBoxes = _checkboxes.Checked; UpdateInstallButton(); };
            _install.ToolTipText = "Find and install missing zipmods for selected or checked scenes";
            _install.Click += InstallMissingZipmods;
            var openFolder = new ToolStripButton("Open folder") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            openFolder.Click += (_, _) => ProcessTools.SafeStartProcess(InstallDirectoryHelper.SceneDir);
            var search = new ToolStripTextBox { Alignment = ToolStripItemAlignment.Right, Width = 220 };
            toolbar.Items.AddRange(new ToolStripItem[] { refresh, _checkboxes, _install, openFolder, search });
            ListTools.SetUpSearchBox(_list, search);

            Controls.Add(_list);
            Controls.Add(toolbar);
            toolbar.Dock = DockStyle.Top;
            FormClosed += (_, _) => CancelRefreshing();
            Shown += (_, _) => RefreshList();
        }

        public void DeserializeContent(string contentString) { }

        public void CancelRefreshing()
        {
            _cancel?.Cancel();
            _cancel?.Dispose();
            _cancel = null;
        }

        public void RefreshList()
        {
            CancelRefreshing();
            _cancel = new CancellationTokenSource();
            _list.ClearObjects();
            _list.EmptyListMsg = "Loading scenes...";
            UseWaitCursor = true;
            var loaded = 0;
            SceneLoader.ReadScenes(new DirectoryInfo(InstallDirectoryHelper.SceneDir), SearchOption.AllDirectories, _cancel.Token)
                .ObserveOn(Program.MainSynchronizationContext)
                .Subscribe(
                    scene => { _list.AddObject(scene); MainWindow.SetStatusText($"Loading scenes: {++loaded}"); },
                    ex => FinishLoading("Failed to load scenes: " + ex.Message),
                    () => FinishLoading($"Done loading {loaded} scene(s)"));
        }

        private void FinishLoading(string status)
        {
            if (IsDisposed) return;
            _list.EmptyListMsg = "No Studio scenes were found";
            _list.FastAutoResizeColumns();
            UseWaitCursor = false;
            MainWindow.SetStatusText(status);
            UpdateInstallButton();
        }

        private List<Scene> EffectiveSelection()
        {
            if (_list.CheckBoxes && _list.CheckedObjects?.Count > 0)
                return _list.CheckedObjects.Cast<Scene>().ToList();
            return _list.SelectedObjects?.Cast<Scene>().ToList() ?? new List<Scene>();
        }

        private void UpdateInstallButton() => _install.Enabled = EffectiveSelection().Any(x => x.MissingZipmods?.Length > 0);

        private void ShowSelectedProperties()
        {
            if (_list.SelectedObject != null)
                MainWindow.Instance.DisplayInPropertyViewer(_list.SelectedObject, this);
        }

        private void InstallMissingZipmods(object sender, EventArgs e)
        {
            var guids = EffectiveSelection().SelectMany(x => x.MissingZipmods ?? Array.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (guids.Length == 0) { UpdateInstallButton(); return; }

            try
            {
                var catalog = new BetterRepackZipmodCatalog(Path.Combine(Program.ProgramLocation, "ZipmodCatalog", "AISHS2.json"));
                if (catalog.Load().Entries.Count == 0)
                {
                    MessageBox.Show("Build the BetterRepack zipmod catalog from the Cards window before installing scene zipmods.", "Zipmod catalog", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var matches = catalog.Find(guids);
                var absent = guids.Except(matches.Select(x => x.Guid), StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray();
                var selected = MissingZipmodInstallDialog.ShowDialog(this, matches, absent);
                if (selected == null || selected.Length == 0) return;

                _install.Enabled = false;
                CancellableProgressDialog.Run(this, "Installing zipmods", (token, text, percent) =>
                    System.Threading.Tasks.Task.Run(() => BetterRepackZipmodCatalog.InstallEntries(selected, token, text, percent), token));
                MainWindow.Instance.RefreshContents(false, true, false);
                MainWindow.SetStatusText("Finished installing selected missing zipmods");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("Failed to look up or install missing zipmods.\n\n" + ex.Message, "Install missing zipmods", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { UpdateInstallButton(); }
        }
    }
}
