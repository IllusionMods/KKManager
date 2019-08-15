using System;
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
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows
{
    public sealed partial class MainWindow : Form
    {
        private MegaUpdater _megaUpdater;

        public MainWindow()
        {
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            Program.MainSynchronizationContext = SynchronizationContext.Current;

            Instance = this;

            InitializeComponent();

            SetupTabs();

            Text = $"{Text} - {Assembly.GetExecutingAssembly().GetName().Version}";

            Task.Run((Action)PopulateStartMenu);

            _megaUpdater = new MegaUpdater();
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
                item.Click += (o, args) => { SafeStartProcess(file.FullName); };
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
                    targetDir, x.CurrentDirectory.FullName, StringComparison.InvariantCultureIgnoreCase));

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

            OpenOrGetCardWindow(CardWindow.MaleCardDir);
            OpenOrGetCardWindow(CardWindow.FemaleCardDir);

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
            OpenOrGetCardWindow(CardWindow.FemaleCardDir);
        }

        private void openMaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(CardWindow.MaleCardDir);
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
            Application.Exit();
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
            SafeStartProcess(InstallDirectoryHelper.KoikatuDirectory.FullName);
        }

        private void screenshotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SafeStartProcess(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "UserData\\cap"));
        }

        private void charactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SafeStartProcess(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "UserData\\chara"));
        }

        private void scenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SafeStartProcess(Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "UserData\\Studio\\scene"));
        }

        private static void SafeStartProcess(string fileFullName)
        {
            try
            {
                if (File.Exists(fileFullName))
                    Process.Start(new ProcessStartInfo(fileFullName) { WorkingDirectory = Path.GetDirectoryName(fileFullName) ?? fileFullName });
                else
                    Process.Start(fileFullName);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Failed to start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            ModUpdateProgressDialog.StartUpdateDialog(this, _megaUpdater);

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
                var results = await _megaUpdater.GetUpdateTasksAsync(_checkForUpdatesCancel.Token);
                var updates = results.Count(item => !item.UpToDate);

                _checkForUpdatesCancel.Token.ThrowIfCancellationRequested();

                if (updates > 0)
                {
                    SetStatusText($"Found {updates} mod updates!");
                    updateSideloaderModpackToolStripMenuItem.BackColor = Color.Lime;
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
