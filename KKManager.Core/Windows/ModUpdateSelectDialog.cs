using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Functions;
using KKManager.Functions.Update;
using KKManager.Util;

namespace KKManager.Windows
{
    public partial class ModUpdateSelectDialog : Form
    {
        private List<UpdateTask> _updateTasks;
        private List<UpdateTask> _selectedItems;

        private ModUpdateSelectDialog()
        {
            InitializeComponent();

            objectListView1.EmptyListMsg = "All mods are up to date!";

            string DateConverter(object value)
            {
                if (value is DateTime dt)
                    return dt == DateTime.MinValue ? "Will be removed" : dt.ToShortDateString();
                if (value == null)
                    return "Will be removed";
                return value.ToString();
            }
            olvColumnDate.AspectToStringConverter = DateConverter;
            olvColumnFileDate.AspectToStringConverter = DateConverter;

            objectListView2.EmptyListMsg = "Select a task to view its details.";
            objectListView2.FormatRow += ObjectListView2_FormatRow;
            olvColumnFileName.AspectGetter = rowObject => ((IUpdateItem)rowObject).TargetPath.FullName.Substring(InstallDirectoryHelper.KoikatuDirectory.FullName.Length);
        }

        public static List<UpdateTask> ShowWindow(ModUpdateProgressDialog owner, List<UpdateTask> updateTasks)
        {
            try
            {
                using (var w = new ModUpdateSelectDialog())
                {
                    w.Icon = owner.Icon;
                    w.StartPosition = FormStartPosition.CenterParent;
                    w._updateTasks = updateTasks.OrderBy(x => x.UpToDate).ThenBy(x => x.TaskName).ToList();
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

            objectListView1.UncheckAll();
            objectListView1.CheckObjects(_updateTasks.Where(x => !x.UpToDate && x.EnableByDefault));
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            _selectedItems = objectListView1.CheckedObjects.Cast<UpdateTask>().Where(x => !x.UpToDate).ToList();

            Close();
        }

        private static void ObjectListView2_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (e.Model is IUpdateItem task)
            {
                if (task is DeleteFileUpdateItem)
                    e.Item.ForeColor = Color.DarkRed;
                else if (!task.TargetPath.Exists)
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
            var sumFileSizes = FileSize.SumFileSizes(objectListView1.CheckedObjects.Cast<UpdateTask>().Select(x => x.TotalUpdateSize));
            labelDownload.Text = (sumFileSizes == FileSize.Empty ? "Nothing" : sumFileSizes.ToString()) + " to download";
        }

        private void objectListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var selection = objectListView1.SelectedObject as UpdateTask;
            objectListView2.SetObjects(selection?.Items);
        }
    }
}
