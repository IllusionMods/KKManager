using System.Linq;
using KKManager.Windows.ToolWindows.Properties;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.ModpackTool
{
    public partial class ModpackToolManifestEditor : PropertyViewerBase
    {
        public ModpackToolManifestEditor()
        {
            InitializeComponent();
            SupportedTypes = new[] { typeof(ZipmodEntry) };

            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Author), textBoxAuthor, labelAuthor, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Description), textBoxDescr, labelDescr, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Games), textBoxGames, labelGames, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Guid), textBoxGuid, labelGuid, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Name), textBoxName, labelName, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Newfilename), textBoxNewfilename, labelNewfilename, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Version), textBoxVersion, labelVersion, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Website), textBoxWebsite, labelWebsite, DataSourceUpdateMode.OnValidation);

            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.OutputSubdirectory), comboBoxOutput, labelOutput, DataSourceUpdateMode.OnValidation);

            textBoxOldfilename.DataBindings.Add(nameof(TextBox.Text), zipmodEntryBindingSource, nameof(ZipmodEntry.OriginalFilename), false, DataSourceUpdateMode.Never);
            labelContents.DataBindings.Add(nameof(TextBox.Text), zipmodEntryBindingSource, nameof(ZipmodEntry.Info) + "." + nameof(ZipmodEntry.Info.ContentsKind), false, DataSourceUpdateMode.Never);
            checkBoxCompress.DataBindings.Add(nameof(CheckBox.Checked), zipmodEntryBindingSource, nameof(ZipmodEntry.Recompress), true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxNotes.DataBindings.Add(nameof(CheckBox.Text), zipmodEntryBindingSource, nameof(ZipmodEntry.Notes), true, DataSourceUpdateMode.OnValidation);
        }

        //todo
        // private BrightIdeasSoftware.TreeListView treeListViewManifest;

        public ZipmodEntry CurrentObject
        {
            get => (ZipmodEntry)zipmodEntryBindingSource.DataSource;
            set => zipmodEntryBindingSource.DataSource = value;
        }

        public override void DisplayObjectProperties(object obj, DockContent source)
        {
            CurrentObject = (ZipmodEntry)obj;

            comboBoxOutput.Items.Clear();
            comboBoxOutput.Items.AddRange(ModpackToolConfiguration.Instance.ContentsHandlingPolicies.Select(x => x.OutputSubfolder.Value).Distinct().Cast<object>().ToArray());
            var shortNames = ModpackToolConfiguration.Instance.AllAcceptableGameShortNames.ToList();
            for (int i = 0; i < shortNames.Count; i++)
            {
                for (int j = 1; j < shortNames.Count - i + 1; j++)
                {
                    var str = ModpackToolConfiguration.Instance.GameOutputSubfolder.Value + " " + string.Join(" ", shortNames.Skip(i).Take(j));
                    comboBoxOutput.Items.Add(str);
                }
            }
        }
        

        private void buttonPrev_Click(object sender, System.EventArgs e) => ModpackToolWindow.SelectZipmodEntry(CurrentObject, SortOrder.Descending, false);
        private void buttonNextIssue_Click(object sender, System.EventArgs e) => ModpackToolWindow.SelectZipmodEntry(CurrentObject, SortOrder.Ascending, true);
        private void buttonNext_Click(object sender, System.EventArgs e) => ModpackToolWindow.SelectZipmodEntry(CurrentObject, SortOrder.Ascending, false);
        private void buttonFail_Click(object sender, System.EventArgs e) => CurrentObject.Status = ZipmodEntry.ZipmodEntryStatus.FAIL;
        private void buttonReprocess_Click(object sender, System.EventArgs e) => CurrentObject.Status = CurrentObject.IsValid() ? ZipmodEntry.ZipmodEntryStatus.NeedsProcessing : ZipmodEntry.ZipmodEntryStatus.ManifestIssue;
    }
}
