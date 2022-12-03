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

            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Author), textBoxAuthor, labelAuthor);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Description), textBoxDescr, labelDescr);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Games), textBoxGames, labelGames, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Guid), textBoxGuid, labelGuid);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Name), textBoxName, labelName);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Newfilename), textBoxNewfilename, labelNewfilename);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Version), textBoxVersion, labelVersion);
            ValidatedString.Bind(zipmodEntryBindingSource, nameof(ZipmodEntry.Website), textBoxWebsite, labelWebsite);

            textBoxOldfilename.DataBindings.Add(nameof(TextBox.Text), zipmodEntryBindingSource, nameof(ZipmodEntry.OriginalFilename), false, DataSourceUpdateMode.Never);
            labelContents.DataBindings.Add(nameof(TextBox.Text), zipmodEntryBindingSource, nameof(ZipmodEntry.Info) + "." + nameof(ZipmodEntry.Info.ContentsKind), false, DataSourceUpdateMode.Never);
            checkBoxCompress.DataBindings.Add(nameof(CheckBox.Checked), zipmodEntryBindingSource, nameof(ZipmodEntry.Recompress), true, DataSourceUpdateMode.OnPropertyChanged);
        }

        //todo
        // private System.Windows.Forms.ComboBox comboBoxOutput;
        // private System.Windows.Forms.CheckBox checkBoxCompress;
        //
        // private BrightIdeasSoftware.TreeListView treeListViewManifest;
        //

        public ZipmodEntry CurrentObject
        {
            get => (ZipmodEntry)zipmodEntryBindingSource.DataSource;
            set => zipmodEntryBindingSource.DataSource = value;
        }

        public override void DisplayObjectProperties(object obj, DockContent source)
        {
            CurrentObject = (ZipmodEntry)obj;
        }
    }
}
