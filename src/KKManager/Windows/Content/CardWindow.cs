using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Data.Cards;
using KKManager.Functions;
using KKManager.Util;
using KKManager.Windows.Dialogs;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.Content
{
    public partial class CardWindow : DockContent, IContentWindow
    {
        private readonly Bitmap _emptyImage;

        private readonly TypedObjectListView<Card> _typedListView;
        private CancellationTokenSource _cancellationTokenSource;

        private DirectoryInfo _currentDirectory;

        private CancellationTokenSource _thumbnailCancellationTokenSource;
        private CharacterRange _previousLoadedItemRange;

        public CardWindow()
        {
            _emptyImage = new Bitmap(1, 1);
            using (var gr = Graphics.FromImage(_emptyImage))
                gr.Clear(Color.FromKnownColor(KnownColor.Transparent));

            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;

            UniversalDragAndDrop.SetupDragAndDrop(listView, SimpleDropSink_Dropped, SimpleDropSink_CanDrop, (sender, args) => RefreshList());
            SetupImageLists();

            olvColumnName.AspectGetter = rowObject => (rowObject as Card)?.Name;
            olvColumnFilename.AspectGetter = rowObject => (rowObject as Card)?.Location.Name;
            olvColumnModDate.AspectGetter = rowObject => (rowObject as Card)?.Location.LastWriteTime;
            olvColumnSex.AspectGetter = rowObject => (rowObject as Card)?.Sex;
            olvColumnPersonality.AspectGetter = rowObject => (rowObject as Card)?.PersonalityName;
            olvColumnExtended.AspectGetter = rowObject => (rowObject as Card)?.Extended?.Count.ToString() ?? "-";

            Details(this, EventArgs.Empty);

            ((OLVColumn)listView.Columns[listView.Columns.Count - 1]).FillsFreeSpace = true;

            _typedListView = new TypedObjectListView<Card>(listView);

            listView.CacheVirtualItems += ListView_CacheVirtualItems;

            listView.EmptyListMsgFont = new Font(Font.FontFamily, 24);
            listView.EmptyListMsg = "No cards were found";

            ListTools.SetUpSearchBox(listView, toolStripTextBoxSearch);
        }

        /// <summary>
        /// Use <see cref="OpenCardDirectory"/> to change
        /// </summary>
        public DirectoryInfo CurrentDirectory
        {
            get => _currentDirectory;
            private set
            {
                _currentDirectory = value;
                Text = value?.Name ?? "Card viewer";
                var fullName = value?.FullName ?? string.Empty;
                ToolTipText = fullName;
                addressBar.Text = fullName;
            }
        }

        public void OpenCardDirectory(DirectoryInfo directory)
        {
            CurrentDirectory = directory;

            if (Visible)
                RefreshList();
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

        public static CardWindow TryLoadFromPersistString(string ps)
        {
            var parts = ps.Split(new[] { "|||" }, 3, StringSplitOptions.None);
            if (parts.Length >= 2 && parts[1].Length > 0)
            {
                if (parts[0] == typeof(CardWindow).ToString())
                {
                    var cardWindow = new CardWindow();
                    cardWindow.OpenCardDirectory(new DirectoryInfo(parts[1]));

                    if (parts.Length >= 3)
                    {
                        try { cardWindow.listView.RestoreState(Convert.FromBase64String(parts[2])); }
                        catch { }
                    }

                    return cardWindow;
                }
            }
            return null;
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
                ShowFailedToLoadDirError(ex);
                return false;
            }
        }

        protected override string GetPersistString()
        {
            return base.GetPersistString() + "|||" + _currentDirectory?.FullName + "|||" + Convert.ToBase64String(listView.SaveState());
        }

        private void addressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TryOpenCardDirectory(addressBar.Text);
        }

        private void CardWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelRefreshing();
        }

        private void femaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCardDirectory(InstallDirectoryHelper.FemaleCardDir);
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            listView.Sort(olvColumnModDate, SortOrder.Descending);
            RefreshList();
        }

        private void Details(object sender, EventArgs e)
        {
            var refresh = listView.View == View.LargeIcon;
            listView.View = View.Details;
            if (refresh) RefreshThumbnails();

            listView.FastAutoResizeColumns();
        }

        private void LargeIcons(object sender, EventArgs e)
        {
            var refresh = listView.View != View.LargeIcon;
            listView.View = View.LargeIcon;
            if (refresh) RefreshThumbnails();
        }

        private void SmallIcons(object sender, EventArgs e)
        {
            var refresh = listView.View == View.LargeIcon;
            listView.View = View.SmallIcon;
            if (refresh) RefreshThumbnails();
        }

        private void ListView_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (_previousLoadedItemRange.First == e.StartIndex &&
                _previousLoadedItemRange.First + _previousLoadedItemRange.Length == e.EndIndex + 1)
                return;

            _previousLoadedItemRange.First = e.StartIndex;
            _previousLoadedItemRange.Length = e.EndIndex - e.StartIndex + 1;

            RefreshThumbnails(true, _previousLoadedItemRange);
        }

        private void maleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCardDirectory(InstallDirectoryHelper.MaleCardDir);
        }

        private void OnResizeToolstip(object sender, EventArgs e)
        {
            var otherWidth = toolStrip.Items.Cast<ToolStripItem>().Where(x => x.Name != "addressBar").Sum(x => x.Width);
            var fillWidth = toolStrip.Width - otherWidth - 20;
            addressBar.Width = fillWidth;
            if (fillWidth > 0)
                addressBar.DropDownWidth = fillWidth;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedObject != null)
                MainWindow.Instance.DisplayInPropertyViewer(listView.SelectedObject, this);
        }

        public void RefreshList()
        {
            CancelRefreshing();

            listView.ClearObjects();
            listView.SmallImageList.Images.Clear();
            listView.LargeImageList.Images.Clear();

            if (CurrentDirectory == null)
            {
                listView.Enabled = false;
                return;
            }

            listView.Enabled = true;

            if (!CurrentDirectory.Exists)
            {
                OpenCardDirectory(null);
                return;
            }

            var cardLoadObservable = CardLoader.ReadCards(CurrentDirectory, _cancellationTokenSource.Token);

            var processedCount = 0;
            cardLoadObservable
                .Buffer(TimeSpan.FromSeconds(4), ThreadPoolScheduler.Instance)
                .ObserveOn(this)
                .Subscribe(
                    list =>
                    {
                        MainWindow.SetStatusText($"Loading cards in progress, {processedCount += list.Count} loaded so far...");
                        listView.AddObjects((ICollection)list);
                        //RefreshThumbnails(true);
                    },
                    ShowFailedToLoadDirError,
                    () =>
                    {
                        listView.FastAutoResizeColumns();
                        RefreshThumbnails(true);

                        MainWindow.SetStatusText("Done loading cards");
                    },
                    _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Cancels previous thumbnail refresh if any and starts a new one
        /// </summary>
        private void RefreshThumbnails(bool additive = false, CharacterRange refreshRange = default)
        {
            CancelThumbnailRefresh();
            _thumbnailCancellationTokenSource = new CancellationTokenSource();

            if (listView.GetItemCount() <= 0) return;

            if (refreshRange == default)
            {
                refreshRange.First = listView.LowLevelScrollPosition.X;
                var visibleItems = (int)Math.Ceiling(listView.Height / (double)listView.RowHeightEffective);
                refreshRange.Length = visibleItems;
            }

            if (!additive)
            {
                listView.SmallImageList.Images.Clear();
                listView.LargeImageList.Images.Clear();
            }

            var token = _thumbnailCancellationTokenSource.Token;

            if (token.IsCancellationRequested) return;

            var large = listView.View == View.LargeIcon;
            var imageList = large ? listView.LargeImageList : listView.SmallImageList;

            var targetCards = _typedListView.Objects.Skip(refreshRange.First).Take(refreshRange.Length);
            if (additive)
            {
                targetCards = targetCards.Where(
                    x =>
                    {
                        try
                        {
                            return !imageList.Images.ContainsKey(x.Location.FullName);
                        }
                        catch (SystemException)
                        {
                            return false;
                        }
                    });
            }

            var cardsToProcess = targetCards.ToList();

            if (cardsToProcess.Count == 0) return;

            var width = imageList.ImageSize.Width;
            var height = imageList.ImageSize.Height;

            var updateSubject = new ReplaySubject<Card>();

            void CardThumbLoader()
            {
                foreach (var card in cardsToProcess)
                {
                    if (token.IsCancellationRequested) return;
                    try
                    {
                        var key = card.Location.FullName;
                        using (var img = large ? card.GetCardImage() : card.GetCardFaceImage() ?? card.GetCardImage())
                        {
                            var thumb = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                            imageList.Images.Add(key, thumb);
                            updateSubject.OnNext(card);
                        }

                        // Need to keep SmallImageList keys in sync when using LargeImageList
                        if (large)
                            listView.SmallImageList.Images.Add(key, _emptyImage);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                updateSubject.OnCompleted();
            }

            Task.Run((Action)CardThumbLoader, token);

            updateSubject
                .Buffer(TimeSpan.FromSeconds(3))
                .ObserveOn(this)
                .Subscribe(list => listView.RefreshObjects((IList)list), token);
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
                    try
                    {
                        var key = card.Location.FullName;
                        if (listView.View == View.LargeIcon)
                        {
                            // Need to use index to fix large images not showing in large icon view of a vritual list
                            var index = listView.LargeImageList.Images.IndexOfKey(key);
                            if (index >= 0) return index;
                        }
                        else
                        {
                            if (listView.SmallImageList.Images.ContainsKey(key))
                                return key;
                        }
                    }
                    catch (SystemException) { }
                }

                return null;
            };
        }

        private static void ShowFailedToLoadDirError(Exception exception)
        {
            Console.WriteLine(exception);
            MessageBox.Show(exception.Message, "Failed to open folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowOpenFolderDialog(object sender, EventArgs e)
        {
            var d = ShowCardFolderBrowseDialog(this);
            if (d != null) TryOpenCardDirectory(d);
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
            if (CurrentDirectory == null) return;

            if (e.DataObject is DataObject dataObject)
            {
                try
                {
                    var files = dataObject.GetFileDropList();

                    var filesChanged = false;
                    foreach (var file in files)
                    {
                        if (_typedListView.Objects.Any(y => y.Location.FullName == file)) continue;

                        switch (e.Effect)
                        {
                            case DragDropEffects.Link:
                                TryOpenCardDirectory(file);
                                return;
                            case DragDropEffects.Copy:
                                File.Copy(file, Path.Combine(CurrentDirectory.FullName, Path.GetFileName(file) ?? throw new InvalidOperationException(file + " is not a valid path")));
                                filesChanged = true;
                                break;
                            case DragDropEffects.Move:
                                File.Move(file, Path.Combine(CurrentDirectory.FullName, Path.GetFileName(file) ?? throw new InvalidOperationException(file + " is not a valid path")));
                                filesChanged = true;
                                break;
                        }
                    }

                    if (filesChanged)
                        RefreshList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void CancelRefreshing()
        {
            lock (this)
            {
                CancelCardLoad();
                CancelThumbnailRefresh();

                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private void CancelCardLoad()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void CancelThumbnailRefresh()
        {
            if (_thumbnailCancellationTokenSource != null)
            {
                _thumbnailCancellationTokenSource.Cancel();
                _thumbnailCancellationTokenSource.Dispose();
                _thumbnailCancellationTokenSource = null;
            }
        }

        private void toolStripButtonGo_Click(object sender, EventArgs e)
        {
            TryOpenCardDirectory(addressBar.Text);
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void toolStripButtonSegregate_Click(object sender, EventArgs e)
        {
            var sexes = _typedListView.SelectedObjects.GroupBy(x => x.Sex).ToList();

            if (sexes.Count < 2)
            {
                MessageBox.Show("All cards are of the same sex, no need to sort", "Card sort", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var sex in sexes)
            {
                var dirpath = Path.Combine(_currentDirectory.FullName, sex.Key.ToString());
                Directory.CreateDirectory(dirpath);
                foreach (var card in sex)
                    card.Location.MoveTo(Path.Combine(dirpath, card.Location.Name));
            }

            OpenCardDirectory(_currentDirectory.GetDirectories().First());
        }

        private void renameCardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameCards.ShowDialog(this, _typedListView.SelectedObjects.ToArray());
        }
    }
}
