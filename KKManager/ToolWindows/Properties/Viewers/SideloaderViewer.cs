using System;
using System.Diagnostics;
using System.Linq;
using KKManager.Sideloader.Data;

namespace KKManager.ToolWindows.Properties.Viewers
{
    public partial class SideloaderViewerBase : PropertyViewerBase
    {
        private int _currentImageId;
        private SideloaderMod _currentObject;

        public SideloaderViewerBase()
        {
            InitializeComponent();

            SupportedTypes = new[] {typeof(SideloaderMod)};
        }

        private SideloaderMod CurrentObject
        {
            get => _currentObject;
            set
            {
                _currentObject = value;

                propertyGrid1.SelectedObject = _currentObject;
                pictureBox1.Image = _currentObject?.Images.FirstOrDefault();
                _currentImageId = 0;
            }
        }
        
        public override void DisplayObjectProperties(object obj)
        {
            Debug.Assert(obj is SideloaderMod);
            CurrentObject = (SideloaderMod) obj;
        }

        private void buttonImageRight_Click(object sender, EventArgs e)
        {
            var images = CurrentObject?.Images;
            if (images == null || images.Count == 0) return;

            _currentImageId = Math.Min(images.Count - 1, _currentImageId + 1);
            pictureBox1.Image = images[_currentImageId];
        }

        private void buttonImageLeft_Click(object sender, EventArgs e)
        {
            var images = CurrentObject?.Images;
            if (images == null || images.Count == 0) return;

            _currentImageId = Math.Max(0, _currentImageId - 1);
            pictureBox1.Image = images[_currentImageId];
        }

        private void toolStripButtonWebsite_Click(object sender, EventArgs e)
        {
            var uri = CurrentObject?.TryGetWebsiteUri();
            if (uri != null) Process.Start(uri.ToString());
        }
    }
}