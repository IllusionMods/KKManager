using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KKManager.ModpackTool
{
    public partial class ToolConfigurationEditor : UserControl
    {
        public ToolConfigurationEditor()
        {
            InitializeComponent();

            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.IngestFolder), folderIngesttextBox, folderIngestOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.OutputFolder), folderOutputtextBox, folderOutputOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.TestGameFolder), folderTestGametextBox, folderTestGameOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.FailFolder), folderFailtextBox, folderFailOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.BackupFolder), folderBackuptextBox, folderBackupOk);

            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.Game1Short), textBoxG1ID, labelG1ID);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.Game1Longs), textBoxG1Tag, labelG1Tag, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.Game2Short), textBoxG2ID, labelG2ID);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.Game2Longs), textBoxG2Tag, labelG2Tag, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.Game3Short), textBoxG3ID, labelG3ID);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(CurrentConfiguration.Game3Longs), textBoxG3Tag, labelG3Tag, DataSourceUpdateMode.OnValidation);
        }

        public ModpackToolConfiguration CurrentConfiguration
        {
            get => (ModpackToolConfiguration)modpackToolConfigurationBindingSource.DataSource;
            set => modpackToolConfigurationBindingSource.DataSource = value;
        }
    }
}
