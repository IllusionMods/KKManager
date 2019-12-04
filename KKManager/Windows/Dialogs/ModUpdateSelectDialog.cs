using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Functions.Update;
using KKManager.Util;

namespace KKManager.Windows.Dialogs
{
    public partial class ModUpdateSelectDialog : Form
    {
        private List<SideloaderUpdateItem> _updateTasks;
        private List<SideloaderUpdateItem> _selectedItems;

        private ModUpdateSelectDialog()
        {
            InitializeComponent();

            objectListView1.EmptyListMsg = "All mods are up to date!";
            objectListView1.FormatRow += ObjectListView1_FormatRow;

            olvColumnDate.AspectToStringConverter = value =>
            {
                if (value is DateTime dt)
                    return dt == DateTime.MinValue ? "Will be removed" : dt.ToShortDateString();
                return value?.ToString();
            };
        }

        public static List<SideloaderUpdateItem> ShowWindow(ModUpdateProgressDialog owner, List<UpdateTask> updateTasks)
        {
            try
            {
                using (var w = new ModUpdateSelectDialog())
                {
                    w.Icon = owner.Icon;
                    w.StartPosition = FormStartPosition.CenterParent;
                    w._updateTasks = updateTasks.OrderBy(x => x.UpToDate).ThenBy(x => x.RelativePath).ToList();
                    w.ShowDialog();

                    return w._selectedItems;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to get updates", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateListObjects();
            SelectAll(this, e);
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            _selectedItems = objectListView1.CheckedObjects.Cast<SideloaderUpdateItem>().Where(x => !x.UpToDate).ToList();

            Close();
        }

        private static void ObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (e.Model is SideloaderUpdateItem task)
            {
                if (!task.RemoteExists)
                    e.Item.ForeColor = Color.DarkRed;
                else if (!task.LocalExists)
                    e.Item.ForeColor = Color.Green;
            }
        }

        private void UpdateListObjects()
        {
            objectListView1.BeginUpdate();
            objectListView1.SuspendLayout();

            if (_updateTasks == null)
                objectListView1.ClearObjects();
            else
                objectListView1.SetObjects(_updateTasks.Where(x => !x.UpToDate).ToList());

            objectListView1.ResumeLayout();
            objectListView1.EndUpdate();
        }

        private void SelectAll(object sender, EventArgs e)
        {
            objectListView1.UncheckAll();
            objectListView1.CheckObjects(_updateTasks.Where(x => !x.UpToDate));
        }

        private void SelectNone(object sender, EventArgs e)
        {
            objectListView1.UncheckAll();
        }

        private void objectListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var sumFileSizes = FileSize.SumFileSizes(objectListView1.CheckedObjects.Cast<SideloaderUpdateItem>().Select(x => x.Size));
            labelDownload.Text = (sumFileSizes == FileSize.Empty ? "Nothing" : sumFileSizes.ToString()) + " to download";
        }
    }
}
