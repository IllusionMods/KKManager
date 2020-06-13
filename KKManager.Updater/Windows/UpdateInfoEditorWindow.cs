using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.Updater.Data;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace KKManager.Updater.Windows
{
    public partial class UpdateInfoEditorWindow : Form
    {
        public UpdateInfoEditorWindow()
        {
            InitializeComponent();
            OpenFile(string.Empty);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new CommonOpenFileDialog
            {
                DefaultExtension = ".xml",
                EnsureFileExists = false,
                IsFolderPicker = false,
                EnsureValidNames = true,
                Multiselect = false,
                Title = "Select UpdateInfo.xml file to edit or create a new one",
                Filters = { new CommonFileDialogFilter("xml file", ".xml") }
            })
            {
                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                    OpenFile(ofd.FileName);
            }
        }

        private void OpenFile(string filename)
        {
            textBox1.Text = filename;
            if (File.Exists(filename))
            {
                try
                {
                    using (var file = File.OpenRead(filename))
                    {
                        var deserialized = UpdateInfo.Deserialize(file);
                        CurrentUpdateInfos = deserialized;
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Failed to read the file - " + ex.Message, "Open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            CurrentUpdateInfos = new UpdateInfo.Updates();
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            OpenFile(textBox1.Text);
        }

        public UpdateInfo.Updates CurrentUpdateInfos
        {
            get => (UpdateInfo.Updates)propertyGrid1.SelectedObject;
            set => propertyGrid1.SelectedObject = value;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var file = new FileStream(textBox1.Text, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    foreach (var info in CurrentUpdateInfos.UpdateInfos)
                        UpdateInfo.TestConstraints(info);

                    CurrentUpdateInfos.Version = UpdateInfo.Updates.CurrentUpdateInfoVersion;
                    propertyGrid1.Refresh();
                    UpdateInfo.Serialize(file, CurrentUpdateInfos);
                }

                MessageBox.Show("Saved sccessfully to " + textBox1.Text, "Save file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("Failed to save to file - " + ex.Message, "Save file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonHash_Click(object sender, EventArgs e)
        {
            if (!(propertyGrid1.SelectedGridItem?.Value is UpdateInfo uinfo))
            {
                MessageBox.Show("Select which UpdateInfo to apply the new hash values to in the UpdateInfos list first", "Generate file hashes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var fb = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                AllowNonFileSystemItems = false,
                AddToMostRecentlyUsedList = false,
                EnsurePathExists = true,
                EnsureFileExists = true,
                Multiselect = false,
                Title = "Select root-level directory with files to hash."
            })
            {
                if (fb.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    try
                    {
                        var directory = new DirectoryInfo(fb.FileName);
                        var fileHashes = FileContentsCalculator.GetFileHashes(directory, true);
                        var results = fileHashes.Select(x => new UpdateInfo.ContentHash
                        {
                            RelativeFileName = x.Key.FullName.Substring(directory.FullName.Length).Replace('\\', '/').TrimStart('/'), 
                            SB3UHash = x.Value.SB3UHash,
                            FileHash = x.Value.FileHash
                        }).ToList();
                        uinfo.ContentHashes = results;
                        propertyGrid1.Refresh();
                        MessageBox.Show("File hashes have been added to currently opened object", "Generate file hashes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        MessageBox.Show("Failed to gather hashes: " + ex, "Generate file hashes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            propertyGrid1.Refresh();
        }
    }
}
