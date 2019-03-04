using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly Bitmap _emptyImage;

        private readonly TypedObjectListView<Card> _typedListView;
        private CancellationTokenSource _cancellationTokenSource;

        private DirectoryInfo _currentDirectory;

        private bool _listLoadIsRunning;
        private CancellationTokenSource _thumbnailCancellationTokenSource;
        private CharacterRange _previousLoadedItemRange = new CharacterRange();

        public CardWindow()
        {
            _emptyImage = new Bitmap(1, 1);
            using (var gr = Graphics.FromImage(_emptyImage))
                gr.Clear(Color.FromKnownColor(KnownColor.Transparent));

            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;

            SetupDragAndDrop();
            SetupImageLists();

            olvColumnName.AspectGetter = rowObject => (rowObject as Card)?.GetCharaName();
            olvColumnFilename.AspectGetter = rowObject => (rowObject as Card)?.CardFile.Name;
            olvColumnModDate.AspectGetter = rowObject => (rowObject as Card)?.CardFile.LastWriteTime;
            olvColumnSex.AspectGetter = rowObject => (rowObject as Card)?.Parameter.sex == 0 ? "Male" : "Female";
            olvColumnPersonality.AspectGetter = rowObject => Utility.GetPersonalityName((rowObject as Card)?.Parameter.personality ?? -1);
            olvColumnExtended.AspectGetter = rowObject => (rowObject as Card)?.Extended?.Count.ToString() ?? "-";

            Details(this, EventArgs.Empty);

            ((OLVColumn)listView.Columns[listView.Columns.Count - 1]).FillsFreeSpace = true;

            _typedListView = new TypedObjectListView<Card>(listView);

            listView.CacheVirtualItems += ListView_CacheVirtualItems;
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
                Text = CurrentDirectory?.Name ?? "Card viewer";
                ToolTipText = CurrentDirectory?.FullName ?? string.Empty;
            }
        }

        public void OpenCardDirectory(DirectoryInfo directory)
        {
            StartNewLoadProcess();

            addressBar.Text = directory?.FullName ?? string.Empty;
            CurrentDirectory = directory;

            RefreshCurrentFolder();
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
            var parts = ps.Split(new[] { "|||" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                if (parts[0] == typeof(CardWindow).ToString())
                {
                    var cardWindow = new CardWindow();
                    cardWindow.OpenCardDirectory(new DirectoryInfo(parts[1]));
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
            return base.GetPersistString() + "|||" + _currentDirectory.FullName;
        }

        private void addressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TryOpenCardDirectory(addressBar.Text);
        }

        private void CardWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            StartNewLoadProcess();
        }

        private void femaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCardDirectory(FemaleCardDir);
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            listView.Sort(olvColumnModDate, SortOrder.Descending);
        }

        private void Details(object sender, EventArgs e)
        {
            var refresh = listView.View == View.LargeIcon;
            listView.View = View.Details;
            if (refresh) RefreshThumbnails();

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
            OpenCardDirectory(MaleCardDir);
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
                MainWindow.Instance.DisplayInPropertyViewer(listView.SelectedObject);
        }

        private void RefreshCurrentFolder()
        {
            _listLoadIsRunning = true;
            var cancellationToken = StartNewLoadProcess();

            listView.ClearObjects();
            listView.SmallImageList.Images.Clear();
            listView.LargeImageList.Images.Clear();

            if (CurrentDirectory == null)
                return;

            if (!CurrentDirectory.Exists)
            {
                OpenCardDirectory(null);
                return;
            }

            var cardLoadObservable = CardLoader.ReadCards(CurrentDirectory, cancellationToken);

            var processedCount = 0;
            cardLoadObservable
                .Buffer(TimeSpan.FromSeconds(1))
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
                        try { listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent); }
                        catch (Exception ex) { Console.WriteLine(ex); }

                        _listLoadIsRunning = false;
                        RefreshThumbnails(true);

                        MainWindow.SetStatusText("Done loading cards");
                    },
                    cancellationToken);
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
                            return !imageList.Images.ContainsKey(x.CardFile.FullName);
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
                        var key = card.CardFile.FullName;
                        using (var img = large ? card.GetCardImage() : card.GetCardFaceImage())
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
                .Buffer(TimeSpan.FromSeconds(1))
                .ObserveOn(this)
                .Subscribe(list => listView.RefreshObjects((IList)list), token);
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
                        var key = card.CardFile.FullName;
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
            if (e.DataObject is DataObject dataObject)
            {
                var files = dataObject.GetFileDropList();

                var filesChanged = false;
                foreach (var file in files)
                {
                    if (_typedListView.Objects.Any(y => y.CardFile.FullName == file)) continue;

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

        private CancellationToken StartNewLoadProcess()
        {
            lock (this)
            {
                CancelCardLoad();
                CancelThumbnailRefresh();

                _cancellationTokenSource = new CancellationTokenSource();

                return _cancellationTokenSource.Token;
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
            RefreshCurrentFolder();
        }

        private void toolStripButtonSegregate_Click(object sender, EventArgs e)
        {
            var sexes = _typedListView.Objects.GroupBy(x => x.Parameter.sex).ToList();

            if (sexes.Count < 2)
            {
                MessageBox.Show("All cards are of the same sex, no need to sort", "Card sort", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var sex in sexes)
            {
                var dirpath = Path.Combine(_currentDirectory.FullName, sex.Key == 0 ? "male" : "female");
                Directory.CreateDirectory(dirpath);
                foreach (var card in sex)
                    card.CardFile.MoveTo(Path.Combine(dirpath, card.CardFile.Name));
            }

            OpenCardDirectory(_currentDirectory.GetDirectories().First());
        }
    }
}
