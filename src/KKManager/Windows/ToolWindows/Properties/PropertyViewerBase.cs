using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.ToolWindows.Properties
{
    public class PropertyViewerBase : UserControl
    {
        public virtual void DisplayObjectProperties(object obj, DockContent source) { }
        public IReadOnlyCollection<Type> SupportedTypes { get; protected set; }
    }
}