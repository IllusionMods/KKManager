using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Updater.Properties;
using KKManager.Util;

namespace KKManager.Updater.Windows
{
    public partial class ModUpdateSelectDialog : Form
    {
        private List<UpdateTask> _updateTasks;
        private List<UpdateTask> _selectedItems;

        private ModUpdateSelectDialog()
        {
            InitializeComponent();

            objectListView1.EmptyListMsg = Resources.ModUpdateSelect_AllUpToDate;
            olvColumnDate.AspectToStringConverter = value =>
            {
                if (value is DateTime dt)
                    return dt == DateTime.MinValue ? KKManager.Properties.Resources.Unknown : dt.ToShortDateString();
                if (value == null)
                    return KKManager.Properties.Resources.Unknown;
                return value.ToString();
            };

            objectListView2.EmptyListMsg = Resources.ModUpdateSelect_SelectTaskToView;
            objectListView2.FormatRow += ObjectListView2_FormatRow;
            olvColumnFileName.AspectName = "FileName";
            olvColumnFileDate.AspectGetter = rowObject =>
            {
                var date = ((UpdateItemListData)rowObject).Modified;
                if (date == null || date == DateTime.MinValue)
                    return Resources.ModUpdateSelect_WillBeRemoved;
                return date.Value.ToShortDateString();
            };
            olvColumnFileSize.AspectName = "Size";
        }

        public static List<UpdateTask> ShowWindow(ModUpdateProgressDialog owner, List<UpdateTask> updateTasks)
        {
            try
            {
                using (var w = new ModUpdateSelectDialog())
                {
                    if (owner.Icon != null)
                        w.Icon = owner.Icon;
                    w.StartPosition = FormStartPosition.CenterParent;
                    w._updateTasks = updateTasks.OrderBy(x => x.UpToDate).ThenBy(x => x.TaskName).ToList();
                    w.ShowDialog(owner);

                    return w._selectedItems;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToStringDemystified(), Resources.ModUpdateSelect_FailedMessage_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateListObjects();

            objectListView1.UncheckAll();
            objectListView1.CheckObjects(_updateTasks.Where(x => !x.UpToDate && x.EnableByDefault));

            objectListView1.AutoResizeColumns();

            UpdateDownloadSizeLabel();

            WindowUtils.FlashWindow(Handle);
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            _selectedItems = objectListView1.CheckedObjects.Cast<UpdateTask>().Where(x => !x.UpToDate).ToList();

            Close();
        }

        private static void ObjectListView2_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (e.Model is UpdateItemListData item)
            {
                if (item.IsDeleted)
                    e.Item.ForeColor = Color.DarkRed;
                else if (item.IsNew)
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
            UpdateDownloadSizeLabel();
        }

        private void objectListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (objectListView1.SelectedObject is UpdateTask selection)
            {
                var listItems = UpdateItemListData.ToListItems(selection.Items);
                objectListView2.SetObjects(listItems);
            }
            else
            {
                objectListView2.ClearObjects();
            }

            if (objectListView2.GetItemCount() > 0)
                objectListView2.AutoResizeColumns();
        }

        private void UpdateDownloadSizeLabel()
        {
            var sumFileSizes = FileSize.SumFileSizes(objectListView1.CheckedObjects.Cast<UpdateTask>().Select(x => x.TotalUpdateSize));
            labelDownload.Text = sumFileSizes == FileSize.Empty ? Resources.ModUpdateSelect_SizeStatus_Nothing : string.Format(Resources.ModUpdateSelect_SizeStatus_BytesToDownload, sumFileSizes);
        }

        private sealed class UpdateItemListData
        {
            public static List<UpdateItemListData> ToListItems(List<UpdateItem> allUpdates)
            {
                // The goal is to correctly display if a zipmod is actually removed or just updated with a different filename
                // Currently: strip the version number from the filename and group by the name, then do dirty work on the grouped UpdateItems
                // TODO Do this properly by detecting these pairs in the UpdateItem factory, somehow, maybe

                var allUpdatesWithVersions = allUpdates.Attempt(item =>
                {
                    var match = Regex.Match(item.RemoteFile?.Name ?? item.TargetPath.Name, @"^(.+?)[_ -]v(\d+(\.\d+)*)\.zipmod$", RegexOptions.IgnoreCase);

                    var hasVersion = match.Success;

                    // Version extraction if it's ever needed in the future
                    //Version version = null;
                    //if (hasVersion)
                    //{
                    //    var versionString = match.Groups[2].Value;
                    //    // If version is just a single number, add .0 to the end so `new Version()` doesn't crash
                    //    if (!match.Groups[3].Success) versionString += ".0";
                    //    version = new Version(versionString);
                    //}

                    return new { item, name = hasVersion ? match.Groups[1].Value : null };
                }, (item, exception) => Console.WriteLine($@"[Updater] WARN: Failed to parse filename of '{item}' - {exception.Message}")).ToList();

                var results = new List<UpdateItemListData>(allUpdates.Count);

                foreach (var group in allUpdatesWithVersions.GroupBy(x => x.name))
                {
                    if (group.Key == null)
                        results.AddRange(group.Select(x => new UpdateItemListData(new[] { x.item })));
                    else
                        results.Add(new UpdateItemListData(group.Select(x => x.item).ToList()));
                }

                results.Sort((data1, data2) => string.Compare(data1.FileName, data2.FileName, StringComparison.CurrentCultureIgnoreCase));

                return results;
            }

            private UpdateItemListData(ICollection<UpdateItem> items)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));
                if (items.Count == 0) throw new ArgumentException(@"Value cannot be an empty collection.", nameof(items));

                Size = items.Select(x => x.GetDownloadSize()).OrderByDescending(x => x).First();
                IsNew = items.All(x => x.TargetPath?.Exists != true);

                var latestTargetPath = items.OrderByDescending(x => x.RemoteFile != null).Select(x => x.TargetPath).Where(x => x != null).OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).First();
                FileName = latestTargetPath.FullName.Substring(InstallDirectoryHelper.GameDirectory.FullName.Length);

                var remote = items.Select(x => x.RemoteFile).Where(x => x != null).OrderByDescending(x => x.ModifiedTime).FirstOrDefault();
                Modified = remote?.ModifiedTime;
                IsDeleted = remote == null;
            }

            public string FileName { get; }
            public DateTime? Modified { get; }
            public FileSize Size { get; }
            public bool IsNew { get; }
            public bool IsDeleted { get; }
        }
    }
}
