using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.Cards;
using KKManager.Sideloader;
using KKManager.ToolWindows.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            GetOrCreateWindow<SideloaderModsWindow>();

            var fc = OpenOrGetCardWindow(CardWindow.FemaleCardDir);
            OpenOrGetCardWindow(CardWindow.MaleCardDir).Show(fc.Pane, DockAlignment.Bottom, 0.4);

            dockPanel.DockRightPortion = 400;
            var propertiesToolWindow = GetOrCreateWindow<PropertiesToolWindow>();
            propertiesToolWindow.DockState = DockState.DockRight;
        }

        public static MainWindow Instance { get; private set; }

        /// <summary>
        ///     Get already existing dockable window or open a new instance of it if none are present.
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

        public PropertyViewerBase DisplayInPropertyViewer(object obj, bool forceOpen = false)
        {
            var viewer = GetOrCreateWindow<PropertiesToolWindow>(forceOpen);
            return viewer?.ShowProperties(obj);
        }

        public CardWindow OpenOrGetCardWindow(DirectoryInfo targetDir)
        {
            return OpenOrGetCardWindow(targetDir.FullName);
        }

        private void sideloaderModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<SideloaderModsWindow>().Show(dockPanel, DockState.Document);
        }

        private void openFemaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(CardWindow.FemaleCardDir);
        }

        private void openMaleCardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrGetCardWindow(CardWindow.MaleCardDir);
        }

        private CardWindow OpenOrGetCardWindow(string targetDir)
        {
            var existing = GetWindows<CardWindow>().FirstOrDefault(x => string.Equals(
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
    }
}