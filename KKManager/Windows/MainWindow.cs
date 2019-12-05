﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Functions.Update;
using KKManager.Properties;
using KKManager.Util;
using KKManager.Windows.Content;
using KKManager.Windows.Dialogs;
using KKManager.Windows.ToolWindows.Properties;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows
{
    public sealed partial class MainWindow : Form
    {
        private readonly IUpdateSource _updateSource;

        public MainWindow()
        {
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            Program.MainSynchronizationContext = SynchronizationContext.Current;

            Instance = this;

            InitializeComponent();

            InstallDirectoryHelper.KoikatuDirectory = GetKoikatuDirectory();

            SetupTabs();

            Text = $"{Text} - {Assembly.GetExecutingAssembly().GetName().Version}";

            Task.Run((Action)PopulateStartMenu);

            try
            {
                var link = new Uri(@"D:\test\test.zip");
                _updateSource = GetUpdater(link);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Failed to open update source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(e);
            }
        }

        private static DirectoryInfo GetKoikatuDirectory()
        {
            var path = Settings.Default.GamePath;
            if (!InstallDirectoryHelper.IsValidGamePath(path))
            {
                try
                {
                    path = Registry.CurrentUser.OpenSubKey(@"Software\Illusion\Koikatu\koikatu")
                        ?.GetValue("INSTALLDIR") as string;
                }
                catch (SystemException) { }

                if (!InstallDirectoryHelper.IsValidGamePath(path))
                {
                    MessageBox.Show(
                        "Koikatu is either not registered properly or its install directory is missing or inaccessible.\n\nYou will have to select game directory manually.",
                        "Failed to find Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    var fb = new FolderBrowserDialog { Description = "Select Koikatsu! game directory" };
                    retryFolderSelect:
                    if (fb.ShowDialog() == DialogResult.OK)
                    {
                        path = fb.SelectedPath;
                        if (!InstallDirectoryHelper.IsValidGamePath(path))
                        {
                            if (MessageBox.Show(
                                    "The selected directory doesn't seem to contain the game. Make sure the directory is correct and try again.",
                                    "Failed to find Koikatu install directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                                goto retryFolderSelect;
                        }
                    }
                }

                Settings.Default.GamePath = path;
            }

            if (string.IsNullOrEmpty(path) || !InstallDirectoryHelper.IsValidGamePath(path))
            {
                MessageBox.Show(
                    "Koikatu is either not registered properly or its install directory is missing or inaccessible.",
                    "Failed to get Koikatu install directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (!PathTools.DirectoryHasWritePermission(path) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "mods")) ||
                !PathTools.DirectoryHasWritePermission(Path.Combine(path, "userdata")))
            {
                if (MessageBox.Show("KK Manager doesn't have write permissions to the game directory! This can cause issues for both KK Manager and the game itself.\n\nDo you want KK Manager to fix permissions of the entire Koikatu folder?",
                        "No write access to game directory", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ProcessTools.FixPermissions(path)?.WaitForExit();
            }

            return new DirectoryInfo(path);
        }

        private IUpdateSource GetUpdater(Uri link)
        {
            switch (link.Scheme)
            {
                case "file":
                    return new ZipUpdater(new FileInfo(link.LocalPath));
                case "ftp":
                    return new FtpUpdater(link);
                case "https":
                    return new MegaUpdater(link, null);
                default:
                    throw new NotSupportedException("Link format is not supported as an update source:" + link.Scheme);
            }
        }

        private void PopulateStartMenu()
        {
            var toAdd = new List<ToolStripItem>();
            var pluginPath = InstallDirectoryHelper.GetPluginPath();
            var allExes = InstallDirectoryHelper.KoikatuDirectory.GetFiles("*.exe", SearchOption.AllDirectories);
            var filteredExes = allExes.Where(x => !x.Name.Equals("bepinex.patcher.exe", StringComparison.OrdinalIgnoreCase) && !x.FullName.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase));
            foreach (var file in filteredExes.OrderBy(x => x.Name))
            {
                var item = new ToolStripMenuItem(file.Name);
                item.AutoToolTip = false;
                item.ToolTipText = file.FullName;
                item.Click += (o, args) => { ProcessTools.SafeStartProcess(file.FullName); };
                toAdd.Add(item);
            }
            this.SafeInvoke(() => startTheGameToolStripMenuItem.DropDownItems.AddRange(toAdd.ToArray()));
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
            return dockPanel.Contents.OfType<T>().Concat(dockPanel.FloatWindows.OfType<T>());
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
            catch (Exception ex) { Console.WriteLine("Failed to read opened tabs from config: " + ex); }

            OpenOrGetCardWindow(InstallDirectoryHelper.GetMaleCardDir());
            OpenOrGetCardWindow(InstallDirectoryHelper.GetFemaleCardDir());

            GetOrCreateWindow<SideloaderModsWindow>();
            GetOrCreateWindow<PluginsWindow>();

            dockPanel.DockRightPortion = 400;
            var propertiesToolWindow = GetOrCreateWindow<PropertiesToolWindow>();
            propertiesToolWindow.DockState = DockState.DockRight;
        }

        private static IDockContent DeserializeTab(string persistString)
        {
            var cw = CardWindow.TryLoadFromPersistString(persistString);
            if (cw != null) return cw;

            var t = Type.GetType(persistString, false, true);
            if (t == null || !typeof(IDockContent).IsAssignableFrom(t))
                throw new InvalidDataException(persistString + " points to an invalid type");

            return (IDockContent)Activator.CreateInstance(t);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _checkForUpdatesCancel.Cancel();

            using (var s = new MemoryStream())
            {
                dockPanel.SaveAsXml(s, Encoding.Unicode);
                Settings.Default.DockState = Encoding.Unicode.GetString(s.ToArray());
            }
        }

        private void openFemaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(InstallDirectoryHelper.GetFemaleCardDir());
        }

        private void openMaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(InstallDirectoryHelper.GetMaleCardDir());
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
            foreach (var window in GetWindows<DockContent>())
            {
                if (window is PluginsWindow pw)
                {
                    if (plugins)
                        pw.ReloadList();
                }
                else if (window is SideloaderModsWindow sm)
                {
                    if (sideloader)
                        sm.ReloadList();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ab = new AboutBox())
                ab.Show(this);
        }

        private void installDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(InstallDirectoryHelper.KoikatuDirectory.FullName);
        }

        private void screenshotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "UserData\\cap"));
        }

        private void charactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "UserData\\chara"));
        }

        private void scenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "UserData\\Studio\\scene"));
        }

        private void kKManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.SafeStartProcess(Program.ProgramLocation);
        }

        private void updateSideloaderModpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowModUpdateDialog();
        }

        private void ShowModUpdateDialog()
        {
            _checkForUpdatesCancel.Cancel();

            var sideWindows = GetWindows<DockContent>().OfType<SideloaderModsWindow>().ToList();
            foreach (var window in sideWindows)
                window.CancelListReload();

            ModUpdateProgressDialog.StartUpdateDialog(this, _updateSource);

            foreach (var window in sideWindows)
                window.ReloadList();

            updateSideloaderModpackToolStripMenuItem.BackColor = DefaultBackColor;
        }

        private readonly CancellationTokenSource _checkForUpdatesCancel = new CancellationTokenSource();

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private async void CheckForUpdates()
        {
            try
            {
                var results = await _updateSource.GetUpdateItems(_checkForUpdatesCancel.Token);
                var updates = results.Count(item => !item.UpToDate);

                _checkForUpdatesCancel.Token.ThrowIfCancellationRequested();

                if (updates > 0)
                {
                    SetStatusText($"Found {updates} mod updates!");
                    updateSideloaderModpackToolStripMenuItem.BackColor = Color.Lime;
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private void fixFileAndFolderPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTools.FixPermissions(InstallDirectoryHelper.KoikatuDirectory.FullName)?.WaitForExit();
        }
    }
}
