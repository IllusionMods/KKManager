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
using KKManager.Data.Zipmods;
using KKManager.Functions;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.Content
{
    public sealed partial class SideloaderModsWindow : DockContent, IContentWindow
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TypedObjectListView<SideloaderModInfo> _listView;

        public SideloaderModsWindow()
        {
            InitializeComponent();
            _listView = new TypedObjectListView<SideloaderModInfo>(objectListView1);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);
            objectListView1.EmptyListMsg = "No mods were found";

            objectListView1.FormatRow += ObjectListView1_FormatRow;

            objectListView1.PrimarySortColumn = olvColumnName;

            ListTools.SetUpSearchBox(objectListView1, toolStripTextBoxSearch);
        }

        public IEnumerable<SideloaderModInfo> AllPlugins => _listView.Objects;

        private void ObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            var plug = (SideloaderModInfo)e.Model;
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

            SideloaderModLoader.Zipmods
                .Buffer(TimeSpan.FromSeconds(3), ThreadPoolScheduler.Instance)
                .ObserveOn(this)
                .Subscribe(list => objectListView1.AddObjects((ICollection)list),
                    () =>
                    {
                        objectListView1.FastAutoResizeColumns();
                        MainWindow.SetStatusText("Done loading zipmods");
                    }, token);
        }

        private void SideloaderModsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelRefreshing();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SideloaderModLoader.StartReload();
            RefreshList();
        }

        private void toolStripButtonEnable_Click(object sender, EventArgs e)
        {
            SetZipmodEnabled(true, _listView.SelectedObjects);
        }

        private void SetZipmodEnabled(bool enabled, IEnumerable<SideloaderModInfo> toChange)
        {
            var targets = toChange.ToList();
            foreach (var obj in targets)
            {
                try
                {
                    obj.SetEnabled(enabled);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to toggle active state of " + obj.Name + "\n\n" + ex.Message, "Enable/Disable zipmods", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            objectListView1.RefreshObjects(targets);
        }

        private void toolStripButtonDisable_Click(object sender, EventArgs e)
        {
            SetZipmodEnabled(false, _listView.SelectedObjects);
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will permanently delete all selected zipmods, are you sure you want to continue?",
                    "Delete zipmods", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            foreach (var obj in _listView.SelectedObjects.ToList())
            {
                try
                {
                    obj.Location.Delete();
                    objectListView1.RemoveObject(obj);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to delete " + obj.Name + "\n\n" + ex.Message, "Delete zipmods", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripButtonOpenModsDir_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InstallDirectoryHelper.ModsPath.FullName);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Failed to start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CancelRefreshing()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
