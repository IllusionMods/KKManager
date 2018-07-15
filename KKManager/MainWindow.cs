using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Cards;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Get already existing dockable window or open a new instance of it if none are present.
        /// </summary>
        /// <typeparam name="T">Type of the window to open</typeparam>
        /// <param name="createNew">Create new instance if none are present?</param>
        private T GetOrCreateWindow<T>(bool createNew = true) where T : DockContent, new()
        {
            var w = dockPanel1.Documents.OfType<T>().FirstOrDefault();

            if (w == null && createNew)
            {
                w = new T();
                w.Show(dockPanel1);
            }

            return w;
        }

        public MainWindow()
        {
            InitializeComponent();

            GetOrCreateWindow<CardWindow>();
        }

        private void cardManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetOrCreateWindow<CardWindow>().Show(dockPanel1, DockState.Document);
        }
    }
}
