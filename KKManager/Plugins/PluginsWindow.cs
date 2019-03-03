using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KKManager.Plugins.Data;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Plugins
{
    public sealed partial class PluginsWindow : DockContent
    {
        private IDisposable _subscription;

        public PluginsWindow()
        {
            InitializeComponent();

            olvColumnName.GroupKeyGetter = rowObject => ListTools.GetFirstCharacter(((PluginInfo)rowObject).Name);
            olvColumnGuid.GroupKeyGetter = rowObject => ListTools.GetGuidGroupKey(((PluginInfo)rowObject).Guid);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);
        }

        private void ReloadMods(IReadOnlyCollection<PluginInfo> mods)
        {
            if (mods.Count == 0)
            {
                objectListView1.EmptyListMsg = "No plugins were found";
            }
            else
            {
                objectListView1.EmptyListMsg = "No plugins match your filters";
                objectListView1.SetObjects(mods);

                UpdateColumnSizes();
            }
        }

        private void UpdateColumnSizes()
        {
            foreach (var column in objectListView1.AllColumns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (column.Width > objectListView1.Width / objectListView1.AllColumns.Count - 1)
                    column.Width = objectListView1.Width / objectListView1.AllColumns.Count - 1;
            }
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayInPropertyViewer(objectListView1.SelectedObject);
        }

        private void SideloaderModsWindow_Shown(object sender, EventArgs e)
        {
            ModSearcher.StartModsRefresh();
            _subscription = ModSearcher.Plugins.Subscribe(ReloadMods);
            objectListView1.EmptyListMsg = "Loading plugins, please wait...";
        }

        private void SideloaderModsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _subscription.Dispose();
        }
    }
}
