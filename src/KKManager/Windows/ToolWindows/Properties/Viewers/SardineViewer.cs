﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using KKManager.Data.Sardines;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.ToolWindows.Properties.Viewers
{
    public partial class SardineViewer : PropertyViewerBase
    {
        private int _currentImageId;
        private SardineModInfo _currentObject;

        public SardineViewer()
        {
            InitializeComponent();

            SupportedTypes = new[] { typeof(SardineModInfo) };
        }

        private SardineModInfo CurrentObject
        {
            get => _currentObject;
            set
            {
                _currentObject = value;

                pictureBox1.Image = _currentObject?.Images.FirstOrDefault();
                _currentImageId = 0;
                
                listView1.Items.Clear();

                if (_currentObject != null)
                {
                    listView1.Items.AddRange(_currentObject.Contents
                        .Where(x => !x.EndsWith("\\"))
                        .OrderBy(x => x)
                        .Select(x => new ListViewItem(x))
                        .ToArray());

                    columnHeaderPath.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }
        }

        public override void DisplayObjectProperties(object obj, DockContent source)
        {
            Debug.Assert(obj is SardineModInfo);
            CurrentObject = (SardineModInfo)obj;
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
    }
}