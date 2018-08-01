using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.ToolWindows.Properties
{
    public partial class PropertiesToolWindow : DockContent
    {
        private readonly Dictionary<Type, PropertyViewerBase> _propertyViewers;

        public PropertiesToolWindow()
        {
            InitializeComponent();

            var iType = typeof(PropertyViewerBase);
            var propViewers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x != iType && iType.IsAssignableFrom(x))
                .Select(Activator.CreateInstance)
                .Cast<PropertyViewerBase>();

            _propertyViewers = new Dictionary<Type, PropertyViewerBase>();
            foreach (var propViewer in propViewers)
            {
                propViewerContainer.Controls.Add(propViewer);
                propViewer.Dock = DockStyle.Fill;
                propViewer.Enabled = false;
                propViewer.Visible = false;

                foreach (var supportedType in propViewer.SupportedTypes)
                {
                    Debug.Assert(!_propertyViewers.ContainsKey(supportedType));
                    _propertyViewers[supportedType] = propViewer;
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }

        public PropertyViewerBase ShowProperties(object targetObject)
        {
            if (targetObject != null)
            {
                if (_propertyViewers.TryGetValue(targetObject.GetType(), out var targetViewer))
                {
                    ShowViewer(targetViewer);
                    targetViewer.DisplayObjectProperties(targetObject);
                    return targetViewer;
                }
            }
            return null;
        }

        private void ShowViewer(PropertyViewerBase targetViewerBase)
        {
            var s = Size;
            foreach (var propertyViewer in _propertyViewers.Values)
            {
                var isVisible = targetViewerBase == propertyViewer;
                propertyViewer.Enabled = isVisible;
                propertyViewer.Visible = isVisible;
            }
            // Prevent propertyViewer from changing size of the window when it's shown
            Size = s;
        }
    }
}
