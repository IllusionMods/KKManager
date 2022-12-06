﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Data.Zipmods;
using KKManager.Windows;
using KKManager.Windows.Content;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.ModpackTool
{
    public partial class ModpackToolWindow : DockContent, IContentWindow
    {
        public ModpackToolWindow()
        {
            InitializeComponent();

            RefreshConfigSuggestions();

            _listView = new TypedObjectListView<ZipmodEntry>(objectListViewMain);
            //_listView.
            olvColumnNo.AspectName = nameof(ZipmodEntry.Index);
            olvColumnOrigName.AspectName = nameof(ZipmodEntry.OriginalFilename);
            olvColumnStatus.AspectName = nameof(ZipmodEntry.Status);
            olvColumnOutputPath.AspectName = nameof(ZipmodEntry.RelativeOutputPath);
            olvColumnStatus.FormatAsModpackToolEntryStatus();
            //todo load last loaded config
        }

        private static TypedObjectListView<ZipmodEntry> _listView;

        internal static string ModpackToolRootDir = Path.Combine(Program.ProgramLocation, "ModpackTool");
        internal static string ModpackToolTempDir = Path.Combine(ModpackToolRootDir, "Temp");

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            groupBoxInput.Height = groupBoxInput.Padding.Top + groupBoxInput.Padding.Bottom + flowLayoutPanel1.Height + (groupBoxInput.Height - groupBoxInput.DisplayRectangle.Height);
        }

        private void buttonSelectInvalid_Click(object sender, EventArgs e)
        {
            var nextToEdit = _listView.Objects.FirstOrDefault(x => !x.IsValid());
            _listView.SelectedObject = nextToEdit;
            if (nextToEdit == null)
                Console.WriteLine("All manifests appear to PASS.");
            else
                Console.WriteLine("Opening in editor");
        }

        #region Config

        private static string ConfigDirectoryPath = Path.GetFullPath(Path.Combine(ModpackToolRootDir, "Config"));
        private string GetFullPathToConfigFile() => Path.GetFullPath(Path.Combine(ConfigDirectoryPath, configInputBox.Text));

        private void RefreshConfigSuggestions()
        {
            try
            {
                configInputBox.Items.Clear();

                if (Directory.Exists(ConfigDirectoryPath))
                    configInputBox.Items.AddRange(Directory.GetFiles(ConfigDirectoryPath, "*.xml", SearchOption.AllDirectories)
                                                           .Select(Path.GetFullPath)
                                                           .Select(x => (object)x.Substring(ConfigDirectoryPath.Length + 1))
                                                           .ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to update config file list: " + e.Message);
            }
        }

        private void configSaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
                var fullName = GetFullPathToConfigFile();
                ModpackToolConfiguration.Instance.Serialize(fullName);
                Console.WriteLine("Serialized config to " + fullName);
                RefreshConfigSuggestions();
            }
            catch (Exception ex) { Console.WriteLine("Failed to serialize: " + ex.Message); }
        }

        private void configLoadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var fullName = GetFullPathToConfigFile();
                ModpackToolConfiguration.Instance.Deserialize(fullName);
                Console.WriteLine("Deserialized config from " + fullName);

                RefreshConfigSuggestions();
            }
            catch (Exception ex) { Console.WriteLine("Failed to deserialize: " + ex.Message); }
        }

        private void configDeleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(GetFullPathToConfigFile());
            }
            catch (Exception ex) { Console.WriteLine("Failed to delete config: " + ex.Message); }
        }

        #endregion

        private void buttonRead_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ModpackToolConfiguration.Instance.AllValid())
                    throw new Exception("Configuration looks to be invalid, check if all folder paths are correct and the folders exist.");

                try { Directory.Delete(ModpackToolTempDir, true); } catch (IOException) { }
                Directory.CreateDirectory(ModpackToolTempDir);

                var rb = new ReplaySubject<SideloaderModInfo>();
                SideloaderModLoader.TryReadSideloaderMods(ModpackToolConfiguration.Instance.IngestFolder, rb, CancellationToken.None).Wait();

                var all = rb.ToEnumerable().ToList();


                //var ingestZipmods = Directory.GetFiles(ModpackToolConfiguration.Instance.IngestFolder, "*.zipmod", SearchOption.AllDirectories);
                //var existingZipmods = Directory.GetFiles(ModpackToolConfiguration.Instance.OutputFolder, "*.zipmod", SearchOption.AllDirectories);

                _listView.Objects = all.Select(ZipmodEntry.FromEntry).ToList();

                _listView.ListView.PrimarySortColumn = olvColumnStatus;
                _listView.ListView.PrimarySortOrder = SortOrder.Ascending;
                _listView.ListView.SecondarySortColumn = olvColumnNo;
                _listView.ListView.SecondarySortOrder = SortOrder.Ascending;
                _listView.ListView.Sort();
                _listView.ListView.ShowSortIndicator();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR, can't proceed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex);
            }

        }

        private void objectListViewMain_SelectionChanged(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayInPropertyViewer(objectListViewMain.SelectedObject, this, objectListViewMain.SelectedObject != null);
        }

        public static void SelectZipmodEntry(ZipmodEntry entry, SortOrder selectionTarget, bool onlyInvalid)
        {
            if (_listView == null) return;

            switch (selectionTarget)
            {
                case SortOrder.None:
                    _listView.SelectedObject = onlyInvalid && entry.Status != ZipmodEntry.ZipmodEntryStatus.ManifestIssue ? null : entry;
                    break;
                case SortOrder.Ascending:
                    {
                        var i = _listView.Objects.IndexOf(entry);
                        var target = _listView.Objects.Skip(i + 1).FirstOrDefault(x => x.Status == ZipmodEntry.ZipmodEntryStatus.ManifestIssue || !onlyInvalid);
                        _listView.SelectedObject = target;
                    }
                    break;
                case SortOrder.Descending:
                    {
                        var i = _listView.Objects.IndexOf(entry);
                        var target = _listView.Objects.Reverse().Skip(i < 0 ? 0 : _listView.Objects.Count - i).FirstOrDefault(x => x.Status == ZipmodEntry.ZipmodEntryStatus.ManifestIssue || !onlyInvalid);
                        _listView.SelectedObject = target;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectionTarget), selectionTarget, null);
            }
        }

        private void button2Process_Click(object sender, EventArgs e)
        {
            var targets = (objectListViewMain.SelectedObjects.Count == 0 ? objectListViewMain.Objects : objectListViewMain.SelectedObjects).Cast<ZipmodEntry>().ToList();
            var filteredTargets = targets.Where(x => x.Status == ZipmodEntry.ZipmodEntryStatus.NeedsProcessing).ToList();
            Console.WriteLine($"Starting processing of {filteredTargets.Count} out of {targets.Count} zipmods (only with NeedsProcessing status).");
            ZipmodProcessor.ProcessZipmods(filteredTargets);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            verificationTool1.SetObjects(objectListViewMain.Objects?.Cast<ZipmodEntry>());
        }

        private void button3Verify_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void button4CopyToOut_Click(object sender, EventArgs e)
        {
            var targets = (objectListViewMain.SelectedObjects.Count == 0 ? objectListViewMain.Objects : objectListViewMain.SelectedObjects).Cast<ZipmodEntry>().ToList();
            foreach (var entry in targets)
            {
                try
                {
                    if (entry.Status == ZipmodEntry.ZipmodEntryStatus.PASS)
                    {
                        var outputDirectory = Path.Combine(ModpackToolConfiguration.Instance.OutputFolder.Value, entry.OutputSubdirectory.Value);
                        var outputPath = Path.Combine(outputDirectory, entry.Newfilename.Value);
                        var finishedFile = new FileInfo(entry.GetTempOutputFilePath());
                        if (!finishedFile.Exists)
                        {
                            Console.WriteLine($"ERROR: Can not output [{entry.Name}] because it wasn't processed or the resulting zipmod file got removed. Setting status to NeedsProcessing.");
                            entry.Status = ZipmodEntry.ZipmodEntryStatus.NeedsProcessing;
                        }
                        else
                        {
                            Console.WriteLine($"Copying [{entry.Name}] ({entry.Status}) to {outputPath}");
                            if (File.Exists(outputPath))
                            {
                                Console.WriteLine($"WARNING: File already existed and was overwritten! {outputPath}");
                            }
                            else if (!Directory.Exists(outputDirectory))
                            {
                                Console.WriteLine($"INFO: Output directory didn't exist and was created: {outputDirectory}");
                                Directory.CreateDirectory(outputDirectory);
                            }
                            finishedFile.CopyTo(outputPath, true);
                            entry.Status = ZipmodEntry.ZipmodEntryStatus.Outputted;
                        }
                    }
                    else if (entry.Status == ZipmodEntry.ZipmodEntryStatus.FAIL)
                    {
                        var outputDirectory = ModpackToolConfiguration.Instance.FailFolder.Value;
                        var outputPath = Path.Combine(outputDirectory, entry.FullPath.FullName);
                        Console.WriteLine($"Copying [{entry.Name}] ({entry.Status}) to {outputPath}");
                        if (File.Exists(outputPath))
                        {
                            Console.WriteLine($"WARNING: File already existed and was overwritten! {outputPath}");
                        }
                        else if (!Directory.Exists(outputDirectory))
                        {
                            Console.WriteLine($"INFO: Fail directory didn't exist and was created: {outputDirectory}");
                            Directory.CreateDirectory(outputDirectory);
                        }

                        entry.FullPath.CopyTo(outputPath, true);
                        entry.Status = ZipmodEntry.ZipmodEntryStatus.Outputted;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"ERROR: Failed to output [{entry.Name}] - " + exception);
                }
            }
        }

        private void configBrowseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
                Process.Start(ConfigDirectoryPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void RefreshList() { }
        public void CancelRefreshing() { }
        public void DeserializeContent(string contentString)
        {
            try
            {
                var fullPath = Path.GetFullPath(contentString);
                if (fullPath.StartsWith(ConfigDirectoryPath, StringComparison.OrdinalIgnoreCase))
                {
                    configInputBox.Text = fullPath.Substring(ConfigDirectoryPath.Length).TrimStart('/', '\\');
                    configLoadBtn_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        protected override string GetPersistString()
        {
            return base.GetPersistString() + "|||" + GetFullPathToConfigFile();
        }
    }
}
