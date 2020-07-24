using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.ToolWindows.Properties
{
    public partial class PropertiesToolWindow : DockContent
    {
        private PropertyViewerBase _defaultPropertyViewer;
        private Dictionary<Type, PropertyViewerBase> _propertyViewers;

        public PropertiesToolWindow()
        {
            InitializeComponent();

            CreateAllPropertyViewers();
        }

        private void CreateAllPropertyViewers()
        {
            _propertyViewers = new Dictionary<Type, PropertyViewerBase>();

            foreach (var propViewerType in FindAllPropertyViewerTypes())
            {
                var propViewer = TryCreatePropertyViewer(propViewerType);
                if (propViewer == null) continue;

                propViewerContainer.Controls.Add(propViewer);
                propViewer.Dock = DockStyle.Fill;
                propViewer.Enabled = false;
                propViewer.Visible = false;

                if (propViewer.SupportedTypes != null)
                {
                    foreach (var supportedType in propViewer.SupportedTypes)
                    {
                        Debug.Assert(!_propertyViewers.ContainsKey(supportedType));
                        _propertyViewers[supportedType] = propViewer;
                    }
                }
                else
                {
                    _defaultPropertyViewer = propViewer;
                }
            }
        }

        private static PropertyViewerBase TryCreatePropertyViewer(Type propViewerType)
        {
            try
            {
                return (PropertyViewerBase)Activator.CreateInstance(propViewerType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static IEnumerable<Type> FindAllPropertyViewerTypes()
        {
            var iType = typeof(PropertyViewerBase);
            var results = new List<Type>();

            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    results.AddRange(ass.GetTypes().Where(x => x != iType && iType.IsAssignableFrom(x)));
                }
                catch (Exception e)
                {
                    if (e is ReflectionTypeLoadException re)
                    {
                        foreach (var loaderException in re.LoaderExceptions)
                            Console.WriteLine("LoaderException: " + loaderException);
                    }

                    Console.WriteLine(e);
                }
            }

            return results;
        }

        public PropertyViewerBase ShowProperties(object targetObject, DockContent source)
        {
            if (targetObject != null)
            {
                var targetType = targetObject.GetType();
                while (targetType != null)
                {
                    _propertyViewers.TryGetValue(targetType, out var targetViewer);
                    if (targetViewer != null)
                    {
                        ShowViewer(targetViewer);
                        targetViewer.DisplayObjectProperties(targetObject, source);
                        return targetViewer;
                    }

                    targetType = targetType.BaseType;
                }

                return _defaultPropertyViewer;
            }
            return null;
        }

        private IEnumerable<PropertyViewerBase> GetAllPropertyViewers()
        {
            return _propertyViewers.Values.Concat(new[] { _defaultPropertyViewer });
        }

        private void ShowViewer(PropertyViewerBase targetViewerBase)
        {
            var s = Size;

            foreach (var propertyViewer in GetAllPropertyViewers())
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
