using System;
using System.Linq;
using System.Windows.Forms;
using KKManager.Functions;

namespace KKManager.Windows.Dialogs
{
    public sealed partial class GameSelectionDialog : Form
    {
        private string _selectedPath;
        public string SelectedPath => _selectedPath;

        public GameSelectionDialog(string currentPath)
        {
            InitializeComponent();
            LoadDetectedGames(currentPath);
        }

        private void LoadDetectedGames(string currentPath)
        {
            try
            {
                var detectedGames = InstallDirectoryHelper.FindInstalledGames();

                if (detectedGames.Any())
                {
                    foreach (var gamePath in detectedGames)
                    {
                        var item = new ListViewItem(gamePath);
                        if (gamePath.Equals(currentPath, StringComparison.OrdinalIgnoreCase))
                            item.Selected = true;
                        gameListView.Items.Add(item);
                    }

                    gameListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
                else
                {
                    messageLabel.Text = "No installed games detected.";
                    messageLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                messageLabel.Text = $"Error detecting games: {ex.Message}";
                messageLabel.Visible = true;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var currentPath = _selectedPath ?? Program.ProgramLocation;

            using (var fb = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog())
            {
                fb.IsFolderPicker = true;
                fb.InitialDirectory = currentPath;
                fb.AllowNonFileSystemItems = false;
                fb.AddToMostRecentlyUsedList = false;
                fb.EnsurePathExists = true;
                fb.EnsureFileExists = true;
                fb.Multiselect = false;
                fb.Title = "Select the install directory of your game";
                
                if (fb.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                {
                    var path = fb.FileName;
                    if (!InstallDirectoryHelper.IsValidGamePath(path))
                    {
                        if (MessageBox.Show(
                                "The selected directory doesn't seem to contain the game. Make sure the directory is correct and try again.",
                                "Select install directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                        {
                            BrowseButton_Click(sender, e);
                        }
                    }
                    else
                    {
                        _selectedPath = path;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
        }

        private void GameListView_ItemActivate(object sender, EventArgs e)
        {
            if (gameListView.SelectedItems.Count > 0)
            {
                _selectedPath = gameListView.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (gameListView.SelectedItems.Count > 0)
            {
                _selectedPath = gameListView.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a game from the list or use the Browse button.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
