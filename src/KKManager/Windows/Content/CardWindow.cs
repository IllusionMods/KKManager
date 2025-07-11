﻿using BrightIdeasSoftware;
using KKManager.Data.Cards;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;
using KKManager.Functions;
using KKManager.Util;
using KKManager.Windows.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.Content
{
    public sealed partial class CardWindow : DockContent, IContentWindow
    {
        private readonly Bitmap _emptyImage;

        private readonly TypedObjectListView<Card> _typedListView;
        private CancellationTokenSource _cancellationTokenSource;

        private DirectoryInfo _currentDirectory;

        private CancellationTokenSource _thumbnailCancellationTokenSource;
        private CharacterRange _previousLoadedItemRange;
        private SearchOption DirectorySearchMode
        {
            get => toolStripButtonSubdirs.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            set => toolStripButtonSubdirs.Checked = value == SearchOption.AllDirectories;
        }
        private bool ShowInvalid
        {
            get => showUnknowninvalidCardsToolStripMenuItem.Checked;
            set
            {
                showUnknowninvalidCardsToolStripMenuItem.Checked = value;
                RefreshList();
            }
        }

        public CardWindow()
        {
            _emptyImage = new Bitmap(1, 1);
            using (var gr = Graphics.FromImage(_emptyImage))
                gr.Clear(Color.FromKnownColor(KnownColor.Transparent));

            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;

            UniversalDragAndDrop.SetupDragAndDrop(listView, SimpleDropSink_Dropped, SimpleDropSink_CanDrop, (sender, args) => RefreshList());
            SetupImageLists();

            Details(this, EventArgs.Empty);

            //((OLVColumn)listView.Columns[listView.Columns.Count - 1]).FillsFreeSpace = true;

            _typedListView = new TypedObjectListView<Card>(listView);

            listView.CacheVirtualItems += ListView_CacheVirtualItems;

            listView.EmptyListMsgFont = new Font(Font.FontFamily, 24);
            listView.EmptyListMsg = "No cards were found";

            listView.FormatRow += (sender, args) =>
            {
                if (args.Model is Card card)
                {
                    if (card is UnknownCard)
                        args.Item.BackColor = Color.LightGray;
                    else if (card.MissingPlugins?.Length > 0 || card.MissingZipmods?.Length > 0)
                        args.Item.BackColor = Color.MistyRose;
                }
            };

            olvColumnRelativeFilename.AspectGetter = rowObject => rowObject is Card card ? card.Location.FullName.Substring(_currentDirectory.FullName.Length).TrimStart('/', '\\') : rowObject;
            olvColumnMissingMods.AspectGetter = rowObject => rowObject is Card card ? card.MissingPlugins?.Length ?? 0 + card.MissingZipmods?.Length ?? 0 : 0;

#if DEBUG
            foreach (var column in listView.AllColumns)
            {
                if (string.IsNullOrEmpty(column.ToolTipText))
                    Debug.Fail(column.ToolTipText);
            }
#endif

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

        public bool TryOpenCardDirectory(string path)
        {
            try
            {
                OpenCardDirectory(new DirectoryInfo(path));
                return true;
            }
            catch (Exception ex)
            {
                ShowFailedToLoadDirError(ex);
                return false;
            }
        }

        public void DeserializeContent(string contentString)
        {
            var parts = contentString.Split(new[] { "|||" }, StringSplitOptions.None);
            if (parts.Length >= 1)
            {
                OpenCardDirectory(new DirectoryInfo(parts[0]));

                if (parts.Length >= 3)
                {
                    try
                    {
                        DirectorySearchMode = (SearchOption)Enum.Parse(typeof(SearchOption), parts[1]);
                        listView.RestoreState(Convert.FromBase64String(parts[1]));
                    }
                    catch { /* safe to ignore */ }
                }
                if (parts.Length >= 4)
                {
                    bool showInvalid;
                    if (bool.TryParse(parts[3], out showInvalid))
                    {
                        ShowInvalid = showInvalid;
                        showUnknowninvalidCardsToolStripMenuItem.Checked = showInvalid;
                    }
                }
            }
        }

        protected override string GetPersistString()
        {
            return base.GetPersistString() + "|||" + _currentDirectory?.FullName + "|||" + DirectorySearchMode + "|||" + Convert.ToBase64String(listView.SaveState()) + "|||" + ShowInvalid;
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

            UseWaitCursor = true;

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

            var prevEmptyListMsg = listView.EmptyListMsg;
            listView.EmptyListMsg = "Loading...";

            var cardLoadObservable = CardLoader.ReadCards(CurrentDirectory, DirectorySearchMode, _cancellationTokenSource.Token);

            var processedCount = 0;
            cardLoadObservable
                .Buffer(TimeSpan.FromSeconds(4), ThreadPoolScheduler.Instance)
                .ObserveOn(Program.MainSynchronizationContext)
                .Subscribe(
                    list =>
                    {
                        MainWindow.SetStatusText($"Loading cards in progress, {processedCount += list.Count} loaded so far...");
                        listView.AddObjects(ShowInvalid ? list : list.Where(x => x is not UnknownCard).ToList());
                        //RefreshThumbnails(true);
                    },
                    ShowFailedToLoadDirError,
                    () =>
                    {
                        listView.FastAutoResizeColumns();
                        RefreshThumbnails(true);

                        UseWaitCursor = false;

                        MainWindow.SetStatusText("Done loading cards");
                        listView.EmptyListMsg = prevEmptyListMsg;
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
                foreach (Image image in listView.SmallImageList.Images) image.Dispose();
                listView.SmallImageList.Images.Clear();
                foreach (Image image in listView.LargeImageList.Images) image.Dispose();
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
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
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

            Task.Run(CardThumbLoader, token);

            updateSubject
                .Buffer(TimeSpan.FromSeconds(3))
                .ObserveOn(Program.MainSynchronizationContext)
                .Subscribe(list => listView.RefreshObjects(list), token);
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
                    catch (Exception e) { Console.WriteLine(e); }
                }

                return null;
            };
        }

        private void ShowFailedToLoadDirError(Exception exception)
        {
            Console.WriteLine(exception);
            MessageBox.Show(exception.Message, "Failed to open folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UseWaitCursor = false;
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

                            default:
                                return;
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

        private async void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            var selectedObjects = _typedListView.SelectedObjects;
            if (!selectedObjects.Any()) return;

            if (MessageBox.Show($"Are you sure you want to delete {selectedObjects.Count} card(s)? This cannot be undone.", "Delete cards", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            foreach (var selectedObject in selectedObjects)
            {
                try
                {
                    await selectedObject.Location.SafeDelete();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Failed to delete card {selectedObject.Location?.Name} - " + exception.ToStringDemystified());
                }
            }

            RefreshList();
        }

        private void exportAListOfMissingModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedObjects = _typedListView.SelectedObjects;
            if (!selectedObjects.Any()) selectedObjects = _typedListView.Objects;

            var cardsWithMissingMods = selectedObjects.Where(x => (x.MissingPlugins?.Length ?? 0 + x.MissingPluginsMaybe?.Length ?? 0 + x.MissingZipmods?.Length ?? 0) > 0).ToList();
            if (cardsWithMissingMods.Count == 0)
            {
                MessageBox.Show("None of the selected cards are using mods or plugins that are missing. Make sure that you selected the cards you want to export in the card list.",
                                "Nothing to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Text file|*.txt",
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Export a list of missing mods",
                RestoreDirectory = true,
                //InitialDirectory = InstallDirectoryHelper.GameDirectory.FullName,
                DereferenceLinks = true,
                FileName = "Missing mod export",
            })
            {
                try
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (var writer = new StreamWriter(sfd.FileName, false, Encoding.Unicode))
                        {
                            writer.WriteLine("# All missing plugins:");
                            foreach (var missingPlugin in cardsWithMissingMods.Where(x => x.MissingPlugins != null).SelectMany(x => x.MissingPlugins).Distinct().OrderBy(x => x))
                                writer.WriteLine(missingPlugin);
                            foreach (var missingPlugin in cardsWithMissingMods.Where(x => x.MissingPluginsMaybe != null).SelectMany(x => x.MissingPluginsMaybe).Distinct().OrderBy(x => x))
                                writer.WriteLine(missingPlugin + " (maybe)");

                            writer.WriteLine();

                            writer.WriteLine("# All missing zipmods:");
                            foreach (var missingZipmod in cardsWithMissingMods.Where(x => x.MissingZipmods != null).SelectMany(x => x.MissingZipmods).Distinct().OrderBy(x => x))
                                writer.WriteLine(missingZipmod);

                            writer.WriteLine();


                            foreach (var cardWithMissingMods in cardsWithMissingMods)
                            {
                                writer.WriteLine("----------------------------------------------------------");

                                writer.WriteLine(cardWithMissingMods.Location.FullName);

                                writer.WriteLine("# Missing plugins:");
                                if (cardWithMissingMods.MissingPlugins != null)
                                {
                                    foreach (var missingPlugin in cardWithMissingMods.MissingPlugins)
                                        writer.WriteLine(missingPlugin);
                                }
                                if (cardWithMissingMods.MissingPluginsMaybe != null)
                                {
                                    foreach (var missingPlugin in cardWithMissingMods.MissingPluginsMaybe)
                                        writer.WriteLine(missingPlugin + " (maybe)");
                                }

                                writer.WriteLine("# Missing zipmods:");
                                if (cardWithMissingMods.MissingZipmods != null)
                                {
                                    foreach (var missingZipmod in cardWithMissingMods.MissingZipmods)
                                        writer.WriteLine(missingZipmod);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);

                    MessageBox.Show($"Failed to export: {exception.Message}\n\nTry saving to a different location. If the error persists, report it on GitHub together with the log file.",
                                    "Failed to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripButtonSubdirs_CheckedChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private static void ExportModCsv(ICollection<Card> cards, bool includeUnused, bool plugins, bool zipmods)
        {
            using var sfd = CreateCsvSaveDialog();

            try
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var writer = new StreamWriter(sfd.FileName, false, Encoding.Unicode))
                {
                    if (zipmods)
                    {
                        writer.WriteLine($"\"# Chara zipmods used by selected cards\",\"Card count:\",\"{cards.Count}\"");
                        writer.WriteLine("\"GUID\",\"Cards with usages\",\"Is installed\",\"Zipmod filename\"");

                        var usedZipmods = cards.SelectMany(x => x.Extended.Values.SelectMany(y => y?.RequiredZipmodGUIDs ?? new List<string>(0)).Distinct())
                                               .GroupBy(x => x)
                                               .Select(x => new Tuple<string, int>(x.Key, x.Count()))
                                               .ToList();
                        if (includeUnused)
                            usedZipmods.AddRange(SideloaderModLoader.Zipmods.Where(x => x.ContentsKind.HasFlag(SideloaderModInfo.ZipmodContentsKind.Character)).Select(x => x.Guid).ToEnumerable().Except(usedZipmods.Select(x => x.Item1)).Select(x => new Tuple<string, int>(x, 0)));

                        foreach (var zipmodGuid in usedZipmods.OrderByDescending(x => x.Item2).ThenBy(x => x.Item1))
                        {
                            var zipmod = SideloaderModLoader.Zipmods.FirstOrDefaultAsync(x => x.Guid == zipmodGuid.Item1).Wait();
                            var zipmodInstalled = zipmod != null;
                            writer.WriteLine($"\"{zipmodGuid.Item1}\",\"{zipmodGuid.Item2}\",\"{(zipmodInstalled ? "Yes" : "No")}\",\"{zipmod?.FileName}\"");
                        }

                        writer.WriteLine();
                    }
                    if (plugins)
                    {
                        writer.WriteLine($"\"# Plugins used by the cards\",\"{cards.Count} cards\"");
                        writer.WriteLine("\"GUID\",\"Cards with usages\",\"Is installed\"");

                        var usedPlugins = cards.SelectMany(x => x.Extended.SelectMany(y => y.Value?.RequiredPluginGUIDs?.Count > 0
                                                                                          ? y.Value.RequiredPluginGUIDs.Select(z => new Tuple<string, bool>(z, true))
                                                                                          : new List<Tuple<string, bool>> { new Tuple<string, bool>(y.Key, false) })
                                                                 .DistinctBy(c => c.Item1))
                                               .GroupBy(x => x.Item1)
                                               .Select(x => new Tuple<string, int, bool>(x.Key, x.Count(), x.First().Item2))
                                               .ToList();

                        if (includeUnused)
                            usedPlugins.AddRange(PluginLoader.Plugins.Select(x => x.Guid).ToEnumerable().Except(usedPlugins.Select(x => x.Item1)).Select(a => new Tuple<string, int, bool>(a, 0, true)));

                        foreach (var pluginGuid in usedPlugins.OrderByDescending(x => x.Item2).ThenByDescending(x => x.Item3).ThenBy(x => x.Item1))
                        {
                            var pluginInstalled = pluginGuid.Item2 == 0 || PluginLoader.Plugins.Any(p => p.Guid == pluginGuid.Item1).Wait() || PluginLoader.Plugins.SelectMany(x => x.ExtDataGuidCandidates).Any(p => p == pluginGuid.Item1).Wait();
                            writer.WriteLine($"\"{pluginGuid.Item1}\",\"{pluginGuid.Item2}\",\"{(pluginInstalled ? "Yes" : pluginGuid.Item3 ? "No" : "Maybe")}\"");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                MessageBox.Show($"Failed to export: {exception.Message}\n\nTry saving to a different location. If the error persists, report it on GitHub together with the log file.",
                                "Failed to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static SaveFileDialog CreateCsvSaveDialog()
        {
            return new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "csv file|*.csv",
                OverwritePrompt = true,
                ValidateNames = true,
                Title = "Export...",
                RestoreDirectory = true,
                //InitialDirectory = InstallDirectoryHelper.GameDirectory.FullName,
                DereferenceLinks = true,
                FileName = "KKManager data export",
            };
        }

        private void usedZipmodsAndPluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedObjects = _typedListView.SelectedObjects;
            if (!selectedObjects.Any()) selectedObjects = _typedListView.Objects;

            ExportModCsv(selectedObjects, false, true, true);
        }

        private void zipmodUsageincludingUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedObjects = _typedListView.SelectedObjects;
            if (!selectedObjects.Any()) selectedObjects = _typedListView.Objects;

            ExportModCsv(selectedObjects, true, false, true);
        }

        private void pluginUsageincludingUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedObjects = _typedListView.SelectedObjects;
            if (!selectedObjects.Any()) selectedObjects = _typedListView.Objects;

            ExportModCsv(selectedObjects, true, true, false);
        }

        private void cardMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var sfd = CreateCsvSaveDialog();

            try
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var writer = new StreamWriter(sfd.FileName, false, Encoding.Unicode))
                {
                    writer.WriteLine("\"FileName\",\"Size\",\"CardType\",\"CharacterName\",\"Sex\",\"Personality\",\"CreatorID\",\"DataID\",\"Version\",\"ExtenedDataCount\",\"ExtendedSize\",\"MissingZipmods\",\"MissingPlugins\",\"MissingPluginsMaybe\"");

                    foreach (var card in _typedListView.SelectedObjects)
                    {
                        var fileName = card.Location?.FullName ?? "";
                        var fileSize = card.FileSize ?? "";
                        var cardType = card.Type.ToString();
                        var characterName = card.Name ?? "";
                        var sex = card.Sex.ToString();
                        var personality = card.PersonalityName ?? "";
                        var extendedDataCount = card.Extended?.Count ?? 0;
                        var creatorId = card.UserID ?? "";
                        var dataId = card.DataID ?? "";
                        var version = card.Version?.ToString() ?? "";
                        var extendedSize = card.ExtendedSize.ToString();
                        var missingZipmods = card.MissingZipmods != null ? string.Join(";", card.MissingZipmods) : "";
                        var missingPlugins = card.MissingPlugins != null ? string.Join(";", card.MissingPlugins) : "";
                        var missingPluginsMaybe = card.MissingPluginsMaybe != null ? string.Join(";", card.MissingPluginsMaybe) : "";

                        writer.WriteLine($"\"{fileName}\",\"{fileSize}\",\"{cardType}\",\"{characterName}\",\"{sex}\",\"{personality}\",\"{creatorId}\",\"{dataId}\",\"{version}\",\"{extendedDataCount}\",\"{extendedSize}\",\"{missingZipmods}\",\"{missingPlugins}\",\"{missingPluginsMaybe}\"");
                    }

                    writer.WriteLine();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                MessageBox.Show($"Failed to export: {exception.Message}\n\nTry saving to a different location. If the error persists, report it on GitHub together with the log file.",
                                "Failed to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(_currentDirectory.FullName);
        }

        private void showUnknowninvalidCardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showUnknowninvalidCardsToolStripMenuItem.Checked = !showUnknowninvalidCardsToolStripMenuItem.Checked;
            ShowInvalid = showUnknowninvalidCardsToolStripMenuItem.Checked;
        }
    }
}
