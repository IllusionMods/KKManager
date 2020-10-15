using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Data.Plugins;
using KKManager.Functions;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.Content
{
    public sealed partial class PluginsWindow : DockContent, IContentWindow
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TypedObjectListView<PluginInfo> _listView;

        public PluginsWindow()
        {
            InitializeComponent();

            _listView = new TypedObjectListView<PluginInfo>(objectListView1);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);
            objectListView1.EmptyListMsg = "No plugins were found";

            objectListView1.FormatRow += ObjectListView1_FormatRow;

            objectListView1.PrimarySortColumn = olvColumnName;

            ListTools.SetUpSearchBox(objectListView1, toolStripTextBoxSearch);
        }

        public IEnumerable<PluginInfo> AllPlugins => _listView.Objects;

        private void ObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            var plug = (PluginInfo)e.Model;
            e.Item.ForeColor = plug.Enabled ? objectListView1.ForeColor : Color.Gray;
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayInPropertyViewer(objectListView1.SelectedObject, this);
        }

        private void SideloaderModsWindow_Shown(object sender, EventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            objectListView1.ClearObjects();

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            PluginLoader.Plugins
                .Buffer(TimeSpan.FromSeconds(3), ThreadPoolScheduler.Instance)
                .ObserveOn(this)
                .Subscribe(list => objectListView1.AddObjects((ICollection)list),
                    () =>
                    {
                        objectListView1.FastAutoResizeColumns();
                        MainWindow.SetStatusText("Done loading plugins");
                    }, token);
        }

        public void CancelRefreshing()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void SideloaderModsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelRefreshing();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will permanently delete all selected plugins, are you sure you want to continue?",
                "Delete plugins", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            var refs = PluginLoader.CheckReferences(_listView.SelectedObjects, _listView.Objects.Except(_listView.SelectedObjects).ToList()).ToList();
            if (refs.Any())
            {
                if (MessageBox.Show("The following depend on the plugin(s) that you are about to delete and will most likely stop working:\n" +
                    string.Join("\n", refs.Select(x => x.Name).OrderBy(x => x)) + "\n\nDo you want to continue anyways?",
                    "Delete plugins", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }

            foreach (var plug in _listView.SelectedObjects.ToList())
            {
                try
                {
                    plug.Location.Delete();
                    objectListView1.RemoveObject(plug);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to remove plugin " + plug.Name + "\n\n" + ex.Message, "Delete plugins", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Need to do full reload since multiple plugins can be in a single dll
            PluginLoader.StartReload();
            RefreshList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            PluginLoader.StartReload();
            RefreshList();
        }

        private void toolStripButtonEnable_Click(object sender, EventArgs e)
        {
            SetPluginsEnabled(_listView.SelectedObjects, true);
        }

        private void SetPluginsEnabled(IList<PluginInfo> plugins, bool enabled)
        {
            if (!enabled)
            {
                var filtered = PluginLoader.CheckReferences(_listView.SelectedObjects, _listView.Objects.Except(_listView.SelectedObjects).ToList()).ToList();
                if (filtered.Any())
                {
                    if (MessageBox.Show("The following depend on the plugin(s) that you are about to disable and will most likely stop working:\n\n" +
                                        string.Join("\n", filtered.Select(x => x.Name).OrderBy(x => x)) + "\n\nDo you want to continue?",
                            "Disable plugins", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                        return;
                }
            }

            foreach (var plug in plugins)
            {
                try
                {
                    plug.SetEnabled(enabled);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to toggle active state of " + plug.Name + "\n\n" + ex.Message, "Enable/Disable plugins", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Need to do this since multiple mods can be in a single dll
            RefreshView();
        }

        private void RefreshView()
        {
            objectListView1.RefreshObjects((IList)_listView.Objects);
        }

        private void toolStripButtonDisable_Click(object sender, EventArgs e)
        {
            SetPluginsEnabled(_listView.SelectedObjects, false);
        }

        public void SelectPlugin(PluginInfo toSelect, bool focus = true, bool exclusive = true)
        {
            if (exclusive)
                objectListView1.DeselectAll();

            objectListView1.SelectObject(toSelect, focus);

            if (focus)
                objectListView1.EnsureModelVisible(toSelect);
        }

        private void toolStripButtonOpenDir_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InstallDirectoryHelper.PluginPath.FullName);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Failed to start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
