using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KKManager.ToolWindows.Properties
{
    public class PropertyViewerBase : UserControl
    {
        public virtual void DisplayObjectProperties(object obj) { }
        public IReadOnlyCollection<Type> SupportedTypes { get; protected set; }
    }
}