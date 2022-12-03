using System;
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
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.ModpackTool
{
    public partial class ModpackToolWindow : DockContent
    {
        public ModpackToolWindow()
        {
            InitializeComponent();

            RefreshConfigSuggestions();

            _listView = new TypedObjectListView<ZipmodEntry>(objectListViewMain);
            //_listView.
            olvColumnNo.AspectName = nameof(ZipmodEntry.Index);
            olvColumnOrigName.AspectName = nameof(ZipmodEntry.OriginalFilename);
            olvColumnStatus.AspectGetter = rowObject => ((ZipmodEntry)rowObject).Status;
            _listView.ListView.UseCellFormatEvents = true;
            _listView.ListView.FormatCell += ListView_FormatCell;
            
            //todo load last loaded config
        }

        private void ListView_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Column == olvColumnStatus)
            {
                var m = (ZipmodEntry)e.Model;
                e.SubItem.BackColor = m.Status switch
                {
                    ZipmodEntry.ZipmodEntryStatus.Ingested => Color.LightGray,
                    ZipmodEntry.ZipmodEntryStatus.ManifestIssue => Color.Peru,
                    ZipmodEntry.ZipmodEntryStatus.NeedsProcessing => Color.SteelBlue,
                    ZipmodEntry.ZipmodEntryStatus.Processing => Color.SteelBlue,
                    ZipmodEntry.ZipmodEntryStatus.NeedsVerify => Color.Aqua,
                    ZipmodEntry.ZipmodEntryStatus.Verifying => Color.Aqua,
                    ZipmodEntry.ZipmodEntryStatus.PASS => Color.SpringGreen,
                    ZipmodEntry.ZipmodEntryStatus.FAIL => Color.PaleVioletRed,
                    ZipmodEntry.ZipmodEntryStatus.Outputted => Color.LawnGreen,
                    _ => Color.White
                };
            }
        }

        private TypedObjectListView<ZipmodEntry> _listView;

        private static string ModpackToolRootDir = Path.Combine(Program.ProgramLocation, "ModpackTool");
        internal static string ModpackToolTempDir = Path.Combine(ModpackToolRootDir, "Temp");

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            groupBoxInput.Height = groupBoxInput.Padding.Top + groupBoxInput.Padding.Bottom + flowLayoutPanel1.Height + (groupBoxInput.Height - groupBoxInput.DisplayRectangle.Height);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var nextToEdit = _listView.Objects.FirstOrDefault(x => !x.AllValidatedStringsAreValid()); //todo 
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

        private void button1Read_Click(object sender, EventArgs e)
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
    }
}
