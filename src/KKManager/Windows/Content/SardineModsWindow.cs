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
using KKManager.Data;
using KKManager.Data.Sardines;
using KKManager.Functions;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.Content
{
    public sealed partial class SardineModsWindow : DockContent, IContentWindow
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TypedObjectListView<SardineModInfo> _listView;

        public SardineModsWindow()
        {
            InitializeComponent();
            _listView = new TypedObjectListView<SardineModInfo>(objectListView1);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);
            objectListView1.EmptyListMsg = "No sardine mods were found";

            objectListView1.FormatRow += ObjectListView1_FormatRow;

            objectListView1.PrimarySortColumn = olvColumnName;
            objectListView1.SecondarySortColumn = olvColumnPath;
            objectListView1.PrimarySortOrder = SortOrder.Ascending;
            objectListView1.SecondarySortOrder = SortOrder.Ascending;

            ListTools.SetUpSearchBox(objectListView1, toolStripTextBoxSearch);
        }

        public void DeserializeContent(string contentString)
        {
            if (!string.IsNullOrEmpty(contentString))
            {
                try { objectListView1.RestoreState(Convert.FromBase64String(contentString)); }
                catch { /* safe to ignore */ }
            }
        }

        protected override string GetPersistString()
        {
            return base.GetPersistString() + "|||" + Convert.ToBase64String(objectListView1.SaveState());
        }
        
        private void ObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            var plug = (SardineModInfo)e.Model;
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
            UseWaitCursor = true;

            objectListView1.ClearObjects();

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            SardineModLoader.Sardines
                            .Buffer(TimeSpan.FromSeconds(3), ThreadPoolScheduler.Instance)
                            .ObserveOn(Program.MainSynchronizationContext)
                            .Subscribe(list => objectListView1.AddObjects(list),
                                       () =>
                                       {
                                           objectListView1.FastAutoResizeColumns();
                                           UseWaitCursor = false;
                                           MainWindow.SetStatusText("Done loading sardines");
                                       }, token);
        }

        private void SideloaderModsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelRefreshing();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SardineModLoader.StartReload();
            RefreshList();
        }

        private void toolStripButtonEnable_Click(object sender, EventArgs e)
        {
            SetSardineEnabled(true, _listView.SelectedObjects);
        }

        private void SetSardineEnabled(bool enabled, IEnumerable<SardineModInfo> toChange)
        {
            var targets = toChange.ToList();
            foreach (var obj in targets)
            {
                try
                {
                    obj.SetEnabled(enabled);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to toggle active state of " + obj.Name + "\n\n" + ex.Message, "Enable/Disable sardines", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            objectListView1.RefreshObjects(targets);
        }

        private void toolStripButtonDisable_Click(object sender, EventArgs e)
        {
            SetSardineEnabled(false, _listView.SelectedObjects);
        }

        private async void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will permanently delete all selected sardine mods, are you sure you want to continue?",
                    "Delete sardines", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            foreach (var obj in _listView.SelectedObjects.ToList())
            {
                try
                {
                    await obj.Location.SafeDelete();
                    objectListView1.RemoveObject(obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to delete " + obj.Name + "\n\n" + ex.Message, "Delete sardines", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripButtonOpenModsDir_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InstallDirectoryHelper.ModsPath.FullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CancelRefreshing()
        {
            _cancellationTokenSource?.Cancel();
            UseWaitCursor = false;
        }
    }
}
