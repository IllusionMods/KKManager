using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Cards.Data;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Cards
{
    public partial class CardWindow : DockContent
    {
        public static readonly DirectoryInfo FemaleCardDir =
            new DirectoryInfo(Path.Combine(Program.KoikatuDirectory.FullName, @"UserData\chara\female"));

        public static readonly DirectoryInfo MaleCardDir =
            new DirectoryInfo(Path.Combine(Program.KoikatuDirectory.FullName, @"UserData\chara\male"));

        private readonly CardLoader _loader;
        private DirectoryInfo _currentDirectory;

        public CardWindow()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;

            _loader = new CardLoader();
            _loader.CardsChanged += Loader_CardsChanged;

            SetupDragAndDrop();
            SetupImageLists();

            olvColumnName.AspectGetter = rowObject => (rowObject as Card)?.GetCharaName();
            olvColumnFilename.AspectGetter = rowObject => (rowObject as Card)?.CardFile.Name;
            olvColumnModDate.AspectGetter = rowObject => (rowObject as Card)?.CardFile.LastWriteTime;

            Details(this, EventArgs.Empty);

            ((OLVColumn)listView.Columns[listView.Columns.Count - 1]).FillsFreeSpace = true;
        }

        public DirectoryInfo CurrentDirectory
        {
            get { return _currentDirectory; }
            private set
            {
                _currentDirectory = value;
                Text = CurrentDirectory?.Name ?? "Card viewer";
                ToolTipText = CurrentDirectory?.FullName ?? string.Empty;
            }
        }

        private void SetupImageLists()
        {
            listView.LargeImageList = new ImageList
            {
                ImageSize = new Size(183, 256),
                ColorDepth = ColorDepth.Depth24Bit
            };

            listView.SmallImageList = new ImageList
            {
                ImageSize = new Size(38, 51),
                ColorDepth = ColorDepth.Depth24Bit
            };

            olvColumnName.ImageGetter = delegate (object rowObject)
            {
                if (rowObject is Card card)
                {
                    var key = card.CardFile.FullName;
                    if (!listView.LargeImageList.Images.ContainsKey(key))
                    {
                        listView.SmallImageList.Images.Add(key, card.CardFaceImage);
                        listView.LargeImageList.Images.Add(key, card.CardImage);
                    }
                    return key;
                }

                return null;
            };
        }

        private void SetupDragAndDrop()
        {
            var simpleDropSink = new SimpleDropSink
            {
                AcceptExternal = true,
                EnableFeedback = false,
                UseDefaultCursors = true
            };
            simpleDropSink.Dropped += SimpleDropSink_Dropped;
            simpleDropSink.CanDrop += SimpleDropSink_CanDrop;
            listView.DropSink = simpleDropSink;
            listView.AllowDrop = true;

            var fileDragSource = new FileDragSource();
            fileDragSource.AfterDrag += (sender, args) => RefreshCurrentFolder();
            listView.DragSource = fileDragSource;
        }

        private void SimpleDropSink_CanDrop(object sender, OlvDropEventArgs e)
        {
            void SetDropAllow(SimpleDropSink s, bool can)
            {
                s.CanDropOnBackground = can;
                s.CanDropOnItem = can;
                s.CanDropOnSubItem = can;
            }

            SetDropAllow(e.DropSink, false);

            if (e.DataObject is DataObject dataObject)
            {
                var files = dataObject.GetFileDropList();
                foreach (var file in files)
                {
                    if (Directory.Exists(file))
                    {
                        SetDropAllow(e.DropSink, true);
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                    if (CurrentDirectory != null && CurrentDirectory.Exists && File.Exists(file))
                    {
                        SetDropAllow(e.DropSink, true);
                        e.Effect = ModifierKeys.HasFlag(Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
                    }
                }
            }
        }

        private void SimpleDropSink_Dropped(object sender, OlvDropEventArgs e)
        {
            if (e.DataObject is DataObject dataObject)
            {
                var files = dataObject.GetFileDropList();

                var filesChanged = false;
                foreach (var file in files)
                {
                    if (_loader.Cards.Any(y => y.CardFile.FullName == file)) continue;

                    switch (e.Effect)
                    {
                        case DragDropEffects.Link:
                            TryOpenCardDirectory(file);
                            return;
                        case DragDropEffects.Copy:
                            File.Copy(file, Path.Combine(CurrentDirectory.FullName, Path.GetFileName(file)));
                            filesChanged = true;
                            break;
                        case DragDropEffects.Move:
                            File.Move(file, Path.Combine(CurrentDirectory.FullName, Path.GetFileName(file)));
                            filesChanged = true;
                            break;
                    }
                }

                if (filesChanged)
                    RefreshCurrentFolder();
            }
        }

        private void RefreshCurrentFolder()
        {
            if (CurrentDirectory != null && CurrentDirectory.Exists)
            {
                _loader.Read(CurrentDirectory);
            }
            else
            {
                CurrentDirectory = null;
                _loader.Clear();
            }
        }

        private void Loader_CardsChanged(object sender, EventArgs e)
        {
            listView.SmallImageList?.Images.Clear();
            listView.LargeImageList?.Images.Clear();

            listView.SetObjects(_loader.Cards);
            listView.AutoResizeColumns();
        }

        public void OpenCardDirectory(DirectoryInfo directory)
        {
            _loader.Read(directory);

            addressBar.Text = directory.FullName;
            CurrentDirectory = directory;
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            listView.Sort(olvColumnModDate, SortOrder.Descending);
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedObject != null)
                MainWindow.Instance.DisplayInPropertyViewer(listView.SelectedObject);
        }

        private void Details(object sender, EventArgs e)
        {
            listView.View = View.Details;
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void SmallIcons(object sender, EventArgs e)
        {
            listView.View = View.SmallIcon;
        }

        private void LargeIcons(object sender, EventArgs e)
        {
            listView.View = View.LargeIcon;
        }

        private void OnResizeToolstip(object sender, EventArgs e)
        {
            var otherWidth = toolStrip.Items.Cast<ToolStripItem>().Where(x => x.Name != "addressBar").Sum(x => x.Width);
            var fillWidth = toolStrip.Width - otherWidth - 20;
            addressBar.Width = fillWidth;
            if (fillWidth > 0)
                addressBar.DropDownWidth = fillWidth;
        }

        private void ShowOpenFolderDialog(object sender, EventArgs e)
        {
            var d = ShowCardFolderBrowseDialog(this);
            if (d != null) TryOpenCardDirectory(d);
        }

        public static string ShowCardFolderBrowseDialog(IWin32Window owner)
        {
            using (var d = new FolderBrowserDialog())
            {
                if (d.ShowDialog(owner) != DialogResult.OK)
                    return null;
                return d.SelectedPath;
            }
        }

        public bool TryOpenCardDirectory(string path)
        {
            try
            {
                OpenCardDirectory(new DirectoryInfo(path));
                return true;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Failed to open folder", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private void femaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCardDirectory(FemaleCardDir);
        }

        private void maleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCardDirectory(MaleCardDir);
        }

        private void addressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TryOpenCardDirectory(addressBar.Text);
        }

        private void toolStripButtonGo_Click(object sender, EventArgs e)
        {
            TryOpenCardDirectory(addressBar.Text);
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshCurrentFolder();
        }
    }
}