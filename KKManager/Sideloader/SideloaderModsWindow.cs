using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Sideloader.Data;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Sideloader
{
    public sealed partial class SideloaderModsWindow : DockContent
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TypedObjectListView<SideloaderModInfo> _listView;

        public SideloaderModsWindow()
        {
            InitializeComponent();
            _listView = new TypedObjectListView<SideloaderModInfo>(objectListView1);

            olvColumnName.GroupKeyGetter = rowObject => ListTools.GetFirstCharacter(((SideloaderModInfo)rowObject).Name);
            olvColumnGuid.GroupKeyGetter = rowObject => ListTools.GetGuidGroupKey(((SideloaderModInfo)rowObject).Guid);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);

            objectListView1.EmptyListMsg = "No mods were found";

            objectListView1.FormatRow += ObjectListView1_FormatRow;

            objectListView1.PrimarySortColumn = olvColumnName;
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
            ReloadList();
        }

        private void ReloadList()
        {
            CancelListReload();
            objectListView1.ClearObjects();

            _cancellationTokenSource = new CancellationTokenSource();

            var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "mods");
            var token = _cancellationTokenSource.Token;
            var observable = SideloaderModLoader.TryReadSideloaderMods(modDir, token);

            observable
                .Buffer(TimeSpan.FromSeconds(0.5))
                .ObserveOn(this)
                .Subscribe(list => objectListView1.AddObjects((ICollection)list),
                    () =>
                    {
                        try { objectListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent); }
                        catch (Exception ex) { Console.WriteLine(ex); }
                        
                        MainWindow.SetStatusText("Done loading zipmods");
                    }, token);
        }

        private void CancelListReload()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void SideloaderModsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelListReload();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadList();
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
    }
}
