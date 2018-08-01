using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.Sideloader.Data;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Sideloader
{
    public partial class SideloaderModsWindow : DockContent
    {
        public SideloaderModsWindow()
        {
            InitializeComponent();
            ReloadMods();
        }

        private void ReloadMods()
        {
            var modDir = Path.Combine(Program.KoikatuDirectory.FullName, "mods");

            if (!Directory.Exists(modDir))
                Directory.CreateDirectory(modDir);

            var items = SideloaderModLoader.ReadSideloaderMods(modDir);

            objectListView1.SetObjects(items);

            olvColumnName.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            olvColumnAuthor.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            olvColumnGuid.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            olvColumnVersion.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayInPropertyViewer(objectListView1.SelectedObject);
        }
    }
}
