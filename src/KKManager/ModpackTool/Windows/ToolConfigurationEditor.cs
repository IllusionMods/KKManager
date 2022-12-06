using System;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace KKManager.ModpackTool
{
    public partial class ToolConfigurationEditor : UserControl
    {
        public ToolConfigurationEditor()
        {
            InitializeComponent();

            var configuration = ModpackToolConfiguration.Instance;
            modpackToolConfigurationBindingSource.DataSource = configuration;

            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.IngestFolder), folderIngesttextBox, folderIngestOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.OutputFolder), folderOutputtextBox, folderOutputOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.TestGameFolder), folderTestGametextBox, folderTestGameOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.FailFolder), folderFailtextBox, folderFailOk);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.BackupFolder), folderBackuptextBox, folderBackupOk);

            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.Game1Short), textBoxG1ID, labelG1ID);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.Game1Longs), textBoxG1Tag, labelG1Tag, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.Game2Short), textBoxG2ID, labelG2ID);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.Game2Longs), textBoxG2Tag, labelG2Tag, DataSourceUpdateMode.OnValidation);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.Game3Short), textBoxG3ID, labelG3ID);
            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.Game3Longs), textBoxG3Tag, labelG3Tag, DataSourceUpdateMode.OnValidation);

            ValidatedString.Bind(modpackToolConfigurationBindingSource, nameof(configuration.GameOutputSubfolder), textBoxGameOutputSubfolder, labelGameOutputSubfolder);

            //checkBoxPngCompress.DataBindings.Add(nameof(CheckBox.Checked), modpackToolConfigurationBindingSource, nameof(configuration.CompressPNGs));
            //checkBoxRandomizeCab.DataBindings.Add(nameof(CheckBox.Checked), modpackToolConfigurationBindingSource, nameof(configuration.RandomizeCABs));

            objectListView1.UseCellFormatEvents = true;
            objectListView1.FormatCell += (sender, args) =>
            {
                if (args.ColumnIndex == olvColumnValid.Index)
                {
                    args.SubItem.BackColor = bool.TryParse(args.CellValue?.ToString(), out var b) && b ? Color.DarkGreen : Color.DarkRed;
                    args.SubItem.ForeColor = Color.White;
                }
            };

            configuration.ContentsChanged += (sender, args) => objectListView1.Objects = ModpackToolConfiguration.Instance.ContentsHandlingPolicies;
            objectListView1.Objects = configuration.ContentsHandlingPolicies;

            #region Drag and drop item reordering

            var simpleDropSink = new SimpleDropSink
            {
                CanDropBetween = true,
                CanDropOnBackground = false,
                CanDropOnItem = false,
                CanDropOnSubItem = false,
                AcceptExternal = false,
            };
            simpleDropSink.ModelCanDrop += (sender, args) => args.Effect = DragDropEffects.Move;
            simpleDropSink.ModelDropped += (sender, args) =>
            {
                var droppedItem = (ModpackToolConfiguration.ModContentsHandlingPolicy)args.SourceModels[0];
                var targetItem = (ModpackToolConfiguration.ModContentsHandlingPolicy)args.TargetModel;

                configuration.ContentsHandlingPolicies.Remove(droppedItem);

                var targetIndex = configuration.ContentsHandlingPolicies.IndexOf(targetItem);
                if (args.DropTargetLocation == DropTargetLocation.BelowItem)
                    targetIndex++;

                configuration.ContentsHandlingPolicies.Insert(targetIndex, droppedItem);

                objectListView1.Objects = configuration.ContentsHandlingPolicies;
            };
            objectListView1.DropSink = simpleDropSink;

            #endregion
        }
    }
}
