using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KKManager.Sideloader.Data;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Sideloader
{
    public sealed partial class SideloaderModsWindow : DockContent
    {
        private IDisposable _subscription;
        
        public SideloaderModsWindow()
        {
            InitializeComponent();

            olvColumnName.GroupKeyGetter = rowObject => ListTools.GetFirstCharacter(((SideloaderModInfo)rowObject).Name);
            olvColumnGuid.GroupKeyGetter = rowObject => ListTools.GetGuidGroupKey(((SideloaderModInfo)rowObject).Guid);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);
        }

        private void ReloadMods(IReadOnlyCollection<SideloaderModInfo> mods)
        {
            if (mods.Count == 0)
            {
                objectListView1.EmptyListMsg = "No mods were found";
            }
            else
            {
                objectListView1.EmptyListMsg = "No mods match your filters";
                objectListView1.SetObjects(mods);

                UpdateColumnSizes();
            }
        }

        private void UpdateColumnSizes()
        {
            foreach (var column in objectListView1.AllColumns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (column.Width > objectListView1.Width / 5)
                    column.Width = objectListView1.Width / 5;
            }
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayInPropertyViewer(objectListView1.SelectedObject);
        }

        private void SideloaderModsWindow_Shown(object sender, EventArgs e)
        {
            ModSearcher.StartModsRefresh();
            _subscription = ModSearcher.SideloaderMods.Subscribe(ReloadMods);
            objectListView1.EmptyListMsg = "Loading mods, please wait...";
        }

        private void SideloaderModsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _subscription.Dispose();
        }
    }
}
