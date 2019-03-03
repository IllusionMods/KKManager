namespace KKManager.ToolWindows.Properties.Viewers
{
    public partial class GenericViewer : PropertyViewerBase
    {
        private object _currentObject;

        public GenericViewer()
        {
            InitializeComponent();

            SupportedTypes = null;
        }

        private object CurrentObject
        {
            get => _currentObject;
            set
            {
                _currentObject = value;

                propertyGrid1.SelectedObject = _currentObject;
            }
        }

        public override void DisplayObjectProperties(object obj)
        {
            CurrentObject = obj;
        }
    }
}