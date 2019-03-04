using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using KKManager.Sideloader.Data;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Sideloader
{
    public sealed partial class SideloaderModsWindow : DockContent
    {
        private CancellationTokenSource _cancellationTokenSource;

        public SideloaderModsWindow()
        {
            InitializeComponent();

            olvColumnName.GroupKeyGetter = rowObject => ListTools.GetFirstCharacter(((SideloaderModInfo)rowObject).Name);
            olvColumnGuid.GroupKeyGetter = rowObject => ListTools.GetGuidGroupKey(((SideloaderModInfo)rowObject).Guid);

            objectListView1.EmptyListMsgFont = new Font(Font.FontFamily, 24);

            objectListView1.EmptyListMsg = "No mods were found";
        }
        
        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayInPropertyViewer(objectListView1.SelectedObject);
        }

        private void SideloaderModsWindow_Shown(object sender, EventArgs e)
        {
            ReloadList();
        }

        private void ReloadList()
        {
            CancelListReload();

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
    }
}
