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
        private int _currentImageId;

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
            propertyGrid1.SelectedObject = null;

            olvColumnName.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            olvColumnAuthor.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            olvColumnGuid.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            olvColumnVersion.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = objectListView1.SelectedObject;
            pictureBox1.Image = (objectListView1.SelectedObject as SideloaderMod)?.Images.FirstOrDefault();
            _currentImageId = 0;
        }

        private void buttonImageRight_Click(object sender, EventArgs e)
        {
            var images = (objectListView1.SelectedObject as SideloaderMod)?.Images;
            if(images == null || images.Count == 0) return;

            _currentImageId = Math.Min(images.Count - 1, _currentImageId + 1);
            pictureBox1.Image = images[_currentImageId];
        }

        private void buttonImageLeft_Click(object sender, EventArgs e)
        {
            var images = (objectListView1.SelectedObject as SideloaderMod)?.Images;
            if (images == null || images.Count == 0) return;

            _currentImageId = Math.Max(0, _currentImageId - 1);
            pictureBox1.Image = images[_currentImageId];
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var uri = (objectListView1.SelectedObject as SideloaderMod)?.TryGetWebsiteUri();
            if (uri != null) Process.Start(uri.ToString());
        }
    }
}
