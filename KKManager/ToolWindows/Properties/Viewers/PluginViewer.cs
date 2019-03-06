using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KKManager.Plugins;
using KKManager.Plugins.Data;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.ToolWindows.Properties.Viewers
{
    public partial class PluginViewer : PropertyViewerBase
    {
        private PluginInfo _currentPlugin;
        private PluginsWindow _source;

        public PluginViewer()
        {
            InitializeComponent();

            SupportedTypes = new[] { typeof(PluginInfo) };
        }

        public PluginInfo CurrentPlugin
        {
            get => _currentPlugin;
            set
            {
                _currentPlugin = value;

                propertyGrid1.SelectedObject = _currentPlugin;

                BuildDepLists(value);
            }
        }

        private void BuildDepLists(PluginInfo value)
        {
            listViewDeps.Items.Clear();
            listViewRefs.Items.Clear();
            if (value == null) return;

            var allPlugins = GetPluginsWindow().AllPlugins.ToList();

            var refs = PluginLoader.CheckReferences(new List<PluginInfo> { value }, allPlugins);
            listViewRefs.Items.AddRange(refs.Select(x => x.Name).OrderBy(x => x).Select(x => new ListViewItem(x)).ToArray());

            var deps = PluginLoader.CheckDependencies(new List<PluginInfo> { value }, allPlugins);
            listViewDeps.Items.AddRange(deps.Select(x => x.Name).OrderBy(x => x).Select(x => new ListViewItem(x)).ToArray());
        }

        private PluginsWindow GetPluginsWindow()
        {
            return _source ?? MainWindow.Instance.GetWindows<PluginsWindow>().FirstOrDefault();
        }

        public override void DisplayObjectProperties(object obj, DockContent source)
        {
            _source = source as PluginsWindow;
            CurrentPlugin = (PluginInfo)obj;
        }

        private void listViewDeps_MouseClick(object sender, MouseEventArgs e)
        {
            //var p = listViewDeps.PointToClient(e.Location);
            var i = listViewDeps.GetItemAt(e.X, e.Y);
            if (i == null) return;

            var pw = GetPluginsWindow();
            var toSelect = pw?.AllPlugins.FirstOrDefault(x => x.Name == i.Text);
            if (toSelect != null) pw.SelectPlugin(toSelect);
        }

        private void listViewRefs_MouseClick(object sender, MouseEventArgs e)
        {
            //var p = listViewRefs.PointToClient(e.Location);
            var i = listViewRefs.GetItemAt(e.X, e.Y);
            if (i == null) return;

            var pw = GetPluginsWindow();
            var toSelect = pw?.AllPlugins.FirstOrDefault(x => x.Name == i.Text);
            if (toSelect != null) pw.SelectPlugin(toSelect);
        }
    }
}