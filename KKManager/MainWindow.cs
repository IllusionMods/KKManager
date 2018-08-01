using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Cards;
using KKManager.Sideloader;
using KKManager.ToolWindows.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager
{
    public partial class MainWindow : Form
    {
        public static MainWindow Instance { get; private set; }

        /// <summary>
        /// Get already existing dockable window or open a new instance of it if none are present.
        /// </summary>
        /// <typeparam name="T">Type of the window to open</typeparam>
        /// <param name="createNew">Create new instance if none are present?</param>
        public T GetOrCreateWindow<T>(bool createNew = true) where T : DockContent, new()
        {
            var w = dockPanel1.Contents.OfType<T>().Concat(dockPanel1.FloatWindows.OfType<T>()).FirstOrDefault();

            if (w == null && createNew)
            {
                w = new T();
                w.Show(dockPanel1);
            }

            return w;
        }

        public PropertyViewerBase DisplayInPropertyViewer(object obj, bool forceOpen = false)
        {
            var viewer = GetOrCreateWindow<PropertiesToolWindow>(forceOpen);
            return viewer?.ShowProperties(obj);
        }

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            GetOrCreateWindow<SideloaderModsWindow>();

            cardManagerToolStripMenuItem_Click(this, EventArgs.Empty);

            dockPanel1.DockRightPortion = 400;
            PropertiesToolWindow propertiesToolWindow = GetOrCreateWindow<PropertiesToolWindow>();
            propertiesToolWindow.DockState = DockState.DockRight;

        }

        private void cardManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CardWindow cardWindow = GetOrCreateWindow<CardWindow>();
            cardWindow.Show(dockPanel1, DockState.Document);
            cardWindow.OpenCardDirectory(new DirectoryInfo(Path.Combine(Program.KoikatuDirectory.FullName, @"UserData\chara\female")));
        }

        private void sideloaderModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<SideloaderModsWindow>().Show(dockPanel1, DockState.Document);
        }
    }
}
