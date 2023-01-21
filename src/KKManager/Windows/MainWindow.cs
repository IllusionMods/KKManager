﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;
using KKManager.Functions;
using KKManager.ModpackTool;
using KKManager.Properties;
using KKManager.SB3UGS;
using KKManager.Updater;
using KKManager.Updater.Sources;
using KKManager.Updater.Windows;
using KKManager.Util;
using KKManager.Windows.Content;
using KKManager.Windows.ToolWindows;
using KKManager.Windows.ToolWindows.Properties;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows
{
    public sealed partial class MainWindow : Form
    {
        private UpdateSourceBase[] _updateSources;
        public UpdateSourceBase[] GetUpdateSources() => _updateSources ?? (_updateSources = UpdateSourceManager.FindUpdateSources(Program.ProgramLocation));

        public MainWindow()
        {
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            Program.MainSynchronizationContext = SynchronizationContext.Current;

            Instance = this;

            InitializeComponent();
            dockPanel.Theme = new VS2015LightTheme();

            InstallDirectoryHelper.Initialize(GetGameDirectory());

            SetupTabs();

            Task.Run(PopulateStartMenu);

#if DEBUG
            var version = Assembly.GetExecutingAssembly().GetName().Version;
#else
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
#endif
            var gameName = InstallDirectoryHelper.GameType.GetFancyGameName();
            var installDir = InstallDirectoryHelper.GameDirectory.FullName;
            Text = $"KK Manager {version} (New downloader edition) - [{gameName}] in {installDir}";
            Console.WriteLine($"Game: {gameName}   Path: {installDir}");

            Settings.Default.Binder.BindControl(checkForUpdatesOnStartupToolStripMenuItem, settings => settings.AutoUpdateSearch, this);
            Settings.Default.Binder.BindControl(useSystemProxyServerToolStripMenuItem, settings => settings.UseProxy, this);
            Settings.Default.Binder.SendUpdates(this);

            // Before using the window location, check if isn't the default value and that it's actually visible on the screen
            if (Settings.Default.WindowLocation != new Point(-1, -1) &&
                Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(Settings.Default.WindowLocation, Settings.Default.WindowSize))))
            {
                StartPosition = FormStartPosition.Manual;
                Location = Settings.Default.WindowLocation;
            }
            if (!Settings.Default.WindowSize.IsEmpty) Size = Settings.Default.WindowSize;
            if (Settings.Default.WindowMaximized) WindowState = FormWindowState.Maximized;
        }

        private static DirectoryInfo GetGameDirectory()
        {
            var path = Settings.Default.GamePath;
            if (!InstallDirectoryHelper.IsValidGamePath(path))
            {
                for (var dir = new DirectoryInfo(Program.ProgramLocation); dir.Exists && dir.Parent != null; dir = dir.Parent)
                {
                    if (InstallDirectoryHelper.IsValidGamePath(dir.FullName))
                    {
                        path = dir.FullName;
                        break;
                    }
                }

                if (!InstallDirectoryHelper.IsValidGamePath(path))
                {
                    MessageBox.Show(
                        "Your game's install directory could not be detected or is inaccessible.\n\nYou will have to select the game directory manually.",
                        "Failed to find game install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (string.IsNullOrWhiteSpace(path))
                    {
                        // Get a plausible initial path
                        try
                        {
                            path = Registry.CurrentUser.OpenSubKey(@"Software\Illusion\Koikatu\koikatu")
                                ?.GetValue("INSTALLDIR") as string;
                        }
                        catch (SystemException) { }
                    }

                    path = ShowInstallDirectoryDialog(path);
                }

                if (!InstallDirectoryHelper.IsValidGamePath(path))
                {
                    MessageBox.Show(
                        "Your game is either not registered properly or its install directory is missing or inaccessible.",
                        "Failed to find game install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }

                Settings.Default.GamePath = path;

                if (path.Length > 100)
                {
                    MessageBox.Show(
                        "Your game path is very long and is likely to cause issues with running the game and/or KK Manager.\n\nPlease consider moving your game to a simpler directory like \"D:\\Games\\Koikatsu\" to avoid potential issues. ",
                        "Game directory warning", MessageBoxButtons.OK);
                }
                else if (path.Any(x => x > 127))
                {
                    MessageBox.Show(
                        "Your game path contains special characters that might cause issues with running the game.\n\nPlease consider moving your game to a simpler directory like \"D:\\Games\\Koikatsu\" to avoid potential issues. ",
                        "Game directory warning", MessageBoxButtons.OK);
                }
            }

            CheckInstallPathPermissions(path);

            return new DirectoryInfo(path);
        }

        private static string ShowInstallDirectoryDialog(string currentPath)
        {
            if (string.IsNullOrWhiteSpace(currentPath))
                currentPath = Program.ProgramLocation;

            using (var fb = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = currentPath,
                AllowNonFileSystemItems = false,
                AddToMostRecentlyUsedList = false,
                EnsurePathExists = true,
                EnsureFileExists = true,
                Multiselect = false,
                Title = "Select the install directory of your game"
            })
            {
            retryFolderSelect:
                if (fb.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var path = fb.FileName;
                    if (!InstallDirectoryHelper.IsValidGamePath(path))
                    {
                        if (MessageBox.Show(
                                "The selected directory doesn't seem to contain the game. Make sure the directory is correct and try again.",
                                "Select install directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                            goto retryFolderSelect;
                    }
                    return path;
                }

                return null;
            }
        }

        private static void CheckInstallPathPermissions(string path)
        {
            if (!PathTools.DirectoryHasWritePermission(path) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "BepInEx/config")) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "mods")) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "UserData")))
            {
                if (MessageBox.Show("KK Manager doesn't have write permissions to the game directory! This can cause issues for both KK Manager and the game itself.\n\nDo you want KK Manager to fix permissions of the entire Koikatu folder?",
                        "No write access to game directory", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ProcessTools.FixPermissions(path)?.WaitForExit();
            }
        }

        private void PopulateStartMenu()
        {
            var toAdd = new List<ToolStripItem>();
            var pluginPath = InstallDirectoryHelper.PluginPath.FullName;
            var allExes = InstallDirectoryHelper.GameDirectory.GetFiles("*.exe", SearchOption.AllDirectories);
            var exeBlacklist = new[] { "UnityCrashHandler64.exe", "bepinex.patcher.exe", "Unity3DCompressor.exe", "KKManager.exe", "StandaloneUpdater.exe" };
            var filteredExes = allExes.Where(x => !exeBlacklist.Contains(x.Name, StringComparer.OrdinalIgnoreCase) && !x.FullName.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase));

            var first = true;
            foreach (var folder in filteredExes.GroupBy(x => x.DirectoryName, StringComparer.OrdinalIgnoreCase).OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (!first) toAdd.Add(new ToolStripSeparator());
                first = false;

                foreach (var file in folder.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
                {
                    // Trim .exe but leave other extensions
                    var trimmedName = file.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? file.Name.Substring(0, file.Name.Length - 4) : file.Name;
                    var item = new ToolStripMenuItem(trimmedName);

                    item.AutoToolTip = false;
                    item.ToolTipText = file.FullName;

                    item.Click += (_, _) => { ProcessTools.SafeStartProcess(file.FullName); };

                    try { item.Image = Icon.ExtractAssociatedIcon(file.FullName)?.ToBitmap(); }
                    catch { item.Image = null; }

                    toAdd.Add(item);
                }
            }

            this.SafeInvoke(() =>
            {
                foreach (var item in toAdd)
                    startTheGameToolStripMenuItem.DropDownItems.Add(item);
            });
        }

        public static MainWindow Instance { get; private set; }

        public static void SetStatusText(string text)
        {
            Instance?.SafeInvoke(() => Instance.toolStripStatusLabelStatus.Text = text);
        }

        public PropertyViewerBase DisplayInPropertyViewer(object obj, DockContent source, bool forceOpen = false)
        {
            var viewer = GetOrCreateWindow<PropertiesToolWindow>(forceOpen);
            return viewer?.ShowProperties(obj, source);
        }

        /// <summary>
        /// Get already existing dockable window or open a new instance of it if none are present.
        /// </summary>
        /// <typeparam name="T">Type of the window to open</typeparam>
        /// <param name="createNew">Create new instance if none are present?</param>
        public T GetOrCreateWindow<T>(bool createNew = true) where T : DockContent, new()
        {
            var w = GetWindows<T>().FirstOrDefault();

            if (w == null && createNew)
            {
                w = new T();
                w.Show(dockPanel);
            }

            return w;
        }

        public IEnumerable<T> GetWindows<T>() where T : DockContent, new()
        {
            return dockPanel.Contents.OfType<T>();
        }

        public CardWindow OpenOrGetCardWindow(DirectoryInfo targetDir)
        {
            return OpenOrGetCardWindow(targetDir.FullName);
        }

        private CardWindow OpenOrGetCardWindow(string targetDir)
        {
            var existing = GetWindows<CardWindow>()
                .FirstOrDefault(x => string.Equals(
                    targetDir, x.CurrentDirectory?.FullName, StringComparison.InvariantCultureIgnoreCase));

            if (existing != null)
            {
                existing.Focus();
                return existing;
            }

            var cardWindow = new CardWindow();
            cardWindow.Show(dockPanel, DockState.Document);
            cardWindow.TryOpenCardDirectory(targetDir);
            return cardWindow;
        }

        private void SetupTabs()
        {
            // Try to load saved state first, if that fails load defaults
            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.Default.DockState))
                {
                    using (var s = new MemoryStream(Encoding.Unicode.GetBytes(Settings.Default.DockState)))
                    {
                        dockPanel.LoadFromXml(s, DeserializeTab);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read opened tabs from config: " + ex.ToStringDemystified());
                foreach (var content in dockPanel.Contents.ToList()) content.DockHandler.Close();
                dockPanel.ResumeLayout(true, true);
            }

            // Load defaults
            OpenOrGetCardWindow(InstallDirectoryHelper.MaleCardDir);
            OpenOrGetCardWindow(InstallDirectoryHelper.FemaleCardDir);

            GetOrCreateWindow<SideloaderModsWindow>();
            GetOrCreateWindow<PluginsWindow>();

            dockPanel.DockRightPortion = 400;
            GetOrCreateWindow<PropertiesToolWindow>().Show(dockPanel, DockState.DockRight);

            GetOrCreateWindow<LogViewer>().Show(dockPanel, DockState.DockBottomAutoHide);
        }

        private static IDockContent DeserializeTab(string persistString)
        {
            var parts = persistString.Split(new[] { "|||" }, 2, StringSplitOptions.None);

            var t = Type.GetType(parts[0], false, true);

            if (t == null || !typeof(IDockContent).IsAssignableFrom(t))
                throw new InvalidDataException(persistString + " points to an invalid type");

            var content = (IDockContent)Activator.CreateInstance(t);

            if (content is IContentWindow contentWindow)
                contentWindow.DeserializeContent(parts.Length == 2 ? parts[1] : "");

            return content;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _checkForUpdatesCancel.Cancel();

            using (var s = new MemoryStream())
            {
                dockPanel.SaveAsXml(s, Encoding.Unicode);
                Settings.Default.DockState = Encoding.Unicode.GetString(s.ToArray());
            }

            Settings.Default.WindowMaximized = WindowState is FormWindowState.Maximized;

            if (WindowState is FormWindowState.Normal)
                Settings.Default.WindowSize = Size;
            if (WindowState is FormWindowState.Normal or FormWindowState.Maximized)
                Settings.Default.WindowLocation = Location;
        }

        private void openFemaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(InstallDirectoryHelper.FemaleCardDir);
        }

        private void openMaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(InstallDirectoryHelper.MaleCardDir);
        }

        private void openPluginBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<PluginsWindow>().Show(dockPanel, DockState.Document);
        }

        private void openPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<PropertiesToolWindow>().Show(dockPanel, DockState.DockRight);
        }

        private void otherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dir = CardWindow.ShowCardFolderBrowseDialog(this);
            if (dir != null)
            {
                var w = new CardWindow();
                w.Show(dockPanel, DockState.Document);
                w.TryOpenCardDirectory(dir);
            }
        }

        private void sideloaderModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<SideloaderModsWindow>().Show(dockPanel, DockState.Document);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void readmeAndSourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var readmePath = Path.Combine(Program.ProgramLocation, "README.md");
            if (File.Exists(readmePath))
                Process.Start("notepad.exe", readmePath);
            else
                Process.Start("https://github.com/bbepis/KKManager");
        }

        private void installANewModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                DereferenceLinks = true,
                ValidateNames = true,
                AutoUpgradeEnabled = true,
                Title = "Choose a .dll file or an archive with the mod to install",
                Filter = "Supported mod files(*.dll;*.zipmod;*.zip)|*.dll;*.zipmod;*.zip"
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ModInstaller.InstallFromUnknownFile(dialog.FileName);
                        RefreshContents(true, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        MessageBox.Show("Failed to install the selected mod.\n\n" + ex.Message, "Failed to install mod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RefreshContents(bool plugins, bool sideloader)
        {
            if (plugins) PluginLoader.StartReload();
            if (sideloader) SideloaderModLoader.StartReload();

            foreach (var window in GetWindows<DockContent>().OfType<IContentWindow>())
            {
                switch (window)
                {
                    case PluginsWindow pw:
                        if (plugins) pw.RefreshList();
                        break;
                    case SideloaderModsWindow sm:
                        if (sideloader) sm.RefreshList();
                        break;
                    default:
                        window.RefreshList();
                        break;
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ab = new AboutBox())
                ab.ShowDialog(this);
        }

        private void installDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(InstallDirectoryHelper.GameDirectory.FullName);
        }

        private void screenshotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, "UserData\\cap"));
        }

        private void charactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, "UserData\\chara"));
        }

        private void scenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Path.Combine(InstallDirectoryHelper.GameDirectory.FullName, "UserData\\Studio\\scene"));
        }

        private void kKManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Program.ProgramLocation);
        }

        private async void updateSideloaderModpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ShowModUpdateDialog();
        }

        private async Task ShowModUpdateDialog()
        {
            try
            {
                try
                {
                    Enabled = false;

                    _checkForUpdatesCancel?.Cancel();

                    PluginLoader.CancelReload();
                    SideloaderModLoader.CancelReload();

                    await PluginLoader.Plugins.LastOrDefaultAsync().Timeout(TimeSpan.FromSeconds(30));
                    await SideloaderModLoader.Zipmods.LastOrDefaultAsync().Timeout(TimeSpan.FromSeconds(30));

                    var updateSources = GetUpdateSources();
                    if (!updateSources.Any()) throw new IOException("No update sources are available");
                    ModUpdateProgressDialog.StartUpdateDialog(this, updateSources);
                }
                catch (Exception ex)
                {
                    var errorMsg = "Failed to start update - " + ex.ToStringDemystified();
                    Console.WriteLine(errorMsg);
                    MessageBox.Show(errorMsg, "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _ = SideloaderModLoader.StartReload();
                _ = PluginLoader.StartReload();

                var contentWindows = GetWindows<DockContent>().OfType<IContentWindow>().ToList();
                foreach (var window in contentWindows) window.RefreshList();

                updateSideloaderModpackToolStripMenuItem.BackColor = DefaultBackColor;
                updateSideloaderModpackToolStripMenuItem.ForeColor = DefaultForeColor;
            }
            finally
            {
                Enabled = true;
            }
        }

        private readonly CancellationTokenSource _checkForUpdatesCancel = new();

        private async void MainWindow_Shown(object sender, EventArgs e)
        {
            if (Settings.Default.AutoUpdateSearch)
                await CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            try
            {
                var _ = SelfUpdater.CheckForUpdatesAndShowDialog();

                var updateSources = GetUpdateSources();
                if (updateSources.Any())
                {
                    var results = await UpdateSourceManager.GetUpdates(_checkForUpdatesCancel.Token, updateSources);
                    var updates = results.Count(item => !item.UpToDate);

                    _checkForUpdatesCancel.Token.ThrowIfCancellationRequested();

                    if (updates > 0)
                    {
                        SetStatusText($"Found {updates} mod updates!");
                        updateSideloaderModpackToolStripMenuItem.BackColor = Color.Lime;
                    }
                    else
                    {
                        SetStatusText("No mod updates were found");
                        updateSideloaderModpackToolStripMenuItem.ForeColor = Color.Gray;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (OutdatedVersionException ex)
            {
                ex.ShowKkmanOutdatedMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Crash during update check: {ex.Message}\nat {ex.Demystify().TargetSite}");
            }
        }

        private void fixFileAndFolderPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.FixPermissions(InstallDirectoryHelper.GameDirectory.FullName)?.WaitForExit();
        }

        private void changeGameInstallDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folder = ShowInstallDirectoryDialog(Settings.Default.GamePath);
            if (folder == null) return;

            Settings.Default.GamePath = folder;
            Settings.Default.Save();
            MessageBox.Show("Install directory has been changed successfully. KKManager has to be restarted for the changes to take effect.", "Change install directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void generateContentsOfUpdatexmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var w = new UpdateInfoEditorWindow())
            {
                w.ShowDialog(this);
            }
        }

        private void compressGameFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "This will compress all of your game files in order to greatly reduce their size on disk and potentially slightly improve the loading times.\n\nThis process can take a very long time depending on your CPU and drive speeds. If some or all game files are already compressed then the size reduction might be low.\n\nWARNING: Backup your abdata folder before using this feature, especially in KoikatsuSunshine! Compressing some files might break them! If you encounter a file that breaks because of compression, please report this on KKManager github or on the Koikatsu discord server in the #mod-programming channel.",
                    "Compress files", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                var rootDirectory = InstallDirectoryHelper.GameDirectory;
                var directories = rootDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly)
                    .Where(directory => directory.Name.EndsWith("_Data", StringComparison.OrdinalIgnoreCase) ||
                                        directory.Name.Equals("abdata", StringComparison.OrdinalIgnoreCase))
                    .ToList();


                var files = directories.SelectMany(dir => dir.GetFiles("*", SearchOption.AllDirectories))
                    .Where(file => FileCanBeCompressed(file, rootDirectory))
                    .ToList();

                CompressFiles(files, false);
            }
        }

        private bool FileCanBeCompressed(FileInfo x, DirectoryInfo rootDirectory)
        {
            if (!SB3UGS_Utils.FileIsAssetBundle(x)) return false;

            // Files inside StreamingAssets are hash-checked so they can't be changed
            var isStreamingAsset = x.FullName.Substring(rootDirectory.FullName.Length)
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Contains("StreamingAssets", StringComparer.OrdinalIgnoreCase);
            if (isStreamingAsset)
            {
                Console.WriteLine($"Skipping file {x.FullName} - Files inside StreamingAssets folder are hash-checked and can't be modified.");
                return false;
            }

            return true;
        }

        private void CompressFiles(IReadOnlyList<FileInfo> files, bool randomizeCab)
        {
            if (!SB3UGS_Initializer.CheckIsAvailable())
            {
                MessageBox.Show(
                    "SB3UGS has not been found in KK Manager directory or it failed to be loaded. Reinstall KK Manager and try again.",
                    "Compress files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadingDialog.ShowDialog(this, "Compressing asset bundle files", dialogInterface =>
            {
                dialogInterface.SetMaximum(files.Count);

                var excs = new ConcurrentBag<Exception>();
                long totalSizeSaved = 0;
                var count = 0;

                Parallel.ForEach(files, file =>
                {
                    dialogInterface.SetProgress(count++, "Compressing " + file.Name);

                    try
                    {
                        var origSize = file.Length;
                        SB3UGS_Utils.CompressBundle(file.FullName, randomizeCab);
                        file.Refresh();
                        totalSizeSaved += origSize - file.Length;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to compress file {file.FullName} - {ex.Message}");
                        excs.Add(ex);
                    }
                });

                if (excs.Any())
                    MessageBox.Show($"Successfully compressed {files.Count - excs.Count} out of {files.Count} files, see log for details. Saved {FileSize.FromBytes(totalSizeSaved).ToString()}.", "Compress files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show($"Successfully compressed {files.Count} files. Saved {FileSize.FromBytes(totalSizeSaved).ToString()}.", "Compress files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void compressBundlesAndRandomizeCABsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new CommonOpenFileDialog("Compress bundles and randomize CABs")
            {
                IsFolderPicker = true,
                EnsurePathExists = true,
                EnsureFileExists = true
            })
            {
                if (d.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var rootDirectory = new DirectoryInfo(d.FileName);
                    var files = rootDirectory.GetFiles("*", SearchOption.AllDirectories)
                        .Where(file => FileCanBeCompressed(file, rootDirectory))
                        .ToList();

                    var randomize = MessageBox.Show("Do you want to randomize CABs of the compressed files? Click No to keep the original CAB strings.",
                                        "Compress bundles in a folder...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                                    DialogResult.Yes;

                    CompressFiles(files, randomize);
                }
            }
        }

        private void openLogViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<LogViewer>().Show(dockPanel, DockState.DockBottom);
        }

        private async void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (await SelfUpdater.CheckForUpdatesAndShowDialog() == null)
                MessageBox.Show("No KKManager updates were found. Check log for more information.", "Check for updates",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void settingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // Populate language dropdown during first run
            if (languagesToolStripMenuItem.DropDownItems.Count == 0)
            {
                var spaceWidth = TextRenderer.MeasureText(" ", Font, Size).Width;
                ToolStripItem CreateLanguageToggle(CultureInfo x)
                {
                    var textWidth = TextRenderer.MeasureText(x.NativeName, Font, Size).Width;
                    return new ToolStripMenuItem(
                            $"{x.NativeName.PadRight(50 - textWidth / spaceWidth)} {x.EnglishName}",
                            null,
                            (obj, _) =>
                            {
                                LanguageManager.CurrentCulture = (CultureInfo)((ToolStripMenuItem)obj).Tag;
                                LanguageManager.ApplyCurrentCulture(this);
                                MessageBox.Show(
                                    "You might need to restart KKManager to fully change the laguage.",
                                    "Language change", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            })
                    { Tag = x };
                }

                languagesToolStripMenuItem.DropDownItems.AddRange(LanguageManager.SupportedLanguages.Select(CreateLanguageToggle).ToArray());
            }

            // Select current language in the language dropdown
            var currentLang = LanguageManager.CurrentCulture;
            foreach (ToolStripMenuItem langItem in languagesToolStripMenuItem.DropDownItems)
            {
                var lang = (CultureInfo)langItem.Tag;
                langItem.Checked = lang.Equals(currentLang);
            }
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var licensePath = Path.Combine(Program.ProgramLocation, "LICENSE");
            if (File.Exists(licensePath))
                Process.Start("notepad.exe", licensePath);
            else
                Process.Start("https://github.com/bbepis/KKManager");
        }

        private void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/bbepis/KKManager");
        }

        private void cleanUpDuplicateZipmodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZipmodTools.RemoveDuplicateZipmodsInDir(InstallDirectoryHelper.ModsPath, false);
        }

        private void cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new CommonOpenFileDialog("Remove duplicate and broken zipmods")
            {
                IsFolderPicker = true,
                EnsurePathExists = true,
                EnsureFileExists = true
            })
            {
                if (d.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var rootDirectory = new DirectoryInfo(d.FileName);
                    var simulate = MessageBox.Show("Do you want to actually remove files? Click No to only log which files would be removed but not actually remove anything.", "Cleanup zipmods", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
                    ZipmodTools.RemoveDuplicateZipmodsInDir(rootDirectory, simulate);
                }
            }
        }

        private void openModpackToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<ModpackToolWindow>().Show(dockPanel, DockState.Document);
        }

        private void p2PDownloaderSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new P2PSettingsDialog())
                dialog.ShowDialog(this);
        }
    }
}
