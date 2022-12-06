namespace KKManager.ModpackTool
{
    partial class ModpackToolWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModpackToolWindow));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.configInputBox = new System.Windows.Forms.ToolStripComboBox();
            this.configBrowseBtn = new System.Windows.Forms.ToolStripButton();
            this.configSaveBtn = new System.Windows.Forms.ToolStripButton();
            this.configLoadBtn = new System.Windows.Forms.ToolStripButton();
            this.configDeleteBtn = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageConfiguration = new System.Windows.Forms.TabPage();
            this.toolConfigurationEditor1 = new KKManager.ModpackTool.ToolConfigurationEditor();
            this.tabPageIngest = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.objectListViewMain = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnNo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnOrigName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnStatus = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnOutputPath = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBoxInput = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button0Read = new System.Windows.Forms.Button();
            this.button1Fillin = new System.Windows.Forms.Button();
            this.button2Process = new System.Windows.Forms.Button();
            this.button3Verify = new System.Windows.Forms.Button();
            this.button4CopyToOut = new System.Windows.Forms.Button();
            this.button9Export = new System.Windows.Forms.Button();
            this.tabPageVerify = new System.Windows.Forms.TabPage();
            this.verificationTool1 = new KKManager.ModpackTool.Windows.VerificationTool();
            this.tabPageHistory = new System.Windows.Forms.TabPage();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageConfiguration.SuspendLayout();
            this.tabPageIngest.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewMain)).BeginInit();
            this.groupBoxInput.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabPageVerify.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.configInputBox,
            this.configSaveBtn,
            this.configLoadBtn,
            this.configDeleteBtn,
            this.configBrowseBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(905, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(46, 22);
            this.toolStripLabel1.Text = "Config:";
            // 
            // configInputBox
            // 
            this.configInputBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.configInputBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.configInputBox.Name = "configInputBox";
            this.configInputBox.Size = new System.Drawing.Size(250, 25);
            this.configInputBox.Text = "test.xml";
            // 
            // configBrowseBtn
            // 
            this.configBrowseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.configBrowseBtn.Image = ((System.Drawing.Image)(resources.GetObject("configBrowseBtn.Image")));
            this.configBrowseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.configBrowseBtn.Name = "configBrowseBtn";
            this.configBrowseBtn.Size = new System.Drawing.Size(94, 22);
            this.configBrowseBtn.Text = "Open config dir";
            this.configBrowseBtn.Click += new System.EventHandler(this.configBrowseBtn_Click);
            // 
            // configSaveBtn
            // 
            this.configSaveBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.configSaveBtn.Image = ((System.Drawing.Image)(resources.GetObject("configSaveBtn.Image")));
            this.configSaveBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.configSaveBtn.Name = "configSaveBtn";
            this.configSaveBtn.Size = new System.Drawing.Size(35, 22);
            this.configSaveBtn.Text = "Save";
            this.configSaveBtn.Click += new System.EventHandler(this.configSaveBtn_Click);
            // 
            // configLoadBtn
            // 
            this.configLoadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.configLoadBtn.Image = ((System.Drawing.Image)(resources.GetObject("configLoadBtn.Image")));
            this.configLoadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.configLoadBtn.Name = "configLoadBtn";
            this.configLoadBtn.Size = new System.Drawing.Size(37, 22);
            this.configLoadBtn.Text = "Load";
            this.configLoadBtn.Click += new System.EventHandler(this.configLoadBtn_Click);
            // 
            // configDeleteBtn
            // 
            this.configDeleteBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.configDeleteBtn.Image = ((System.Drawing.Image)(resources.GetObject("configDeleteBtn.Image")));
            this.configDeleteBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.configDeleteBtn.Name = "configDeleteBtn";
            this.configDeleteBtn.Size = new System.Drawing.Size(44, 22);
            this.configDeleteBtn.Text = "Delete";
            this.configDeleteBtn.Click += new System.EventHandler(this.configDeleteBtn_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageConfiguration);
            this.tabControl1.Controls.Add(this.tabPageIngest);
            this.tabControl1.Controls.Add(this.tabPageVerify);
            this.tabControl1.Controls.Add(this.tabPageHistory);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(905, 506);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageConfiguration
            // 
            this.tabPageConfiguration.AutoScroll = true;
            this.tabPageConfiguration.Controls.Add(this.toolConfigurationEditor1);
            this.tabPageConfiguration.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfiguration.Name = "tabPageConfiguration";
            this.tabPageConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfiguration.Size = new System.Drawing.Size(897, 480);
            this.tabPageConfiguration.TabIndex = 0;
            this.tabPageConfiguration.Text = "Configuration";
            this.tabPageConfiguration.UseVisualStyleBackColor = true;
            // 
            // toolConfigurationEditor1
            // 
            this.toolConfigurationEditor1.AutoSize = true;
            this.toolConfigurationEditor1.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolConfigurationEditor1.Location = new System.Drawing.Point(3, 3);
            this.toolConfigurationEditor1.Name = "toolConfigurationEditor1";
            this.toolConfigurationEditor1.Size = new System.Drawing.Size(874, 688);
            this.toolConfigurationEditor1.TabIndex = 2;
            // 
            // tabPageIngest
            // 
            this.tabPageIngest.Controls.Add(this.groupBox3);
            this.tabPageIngest.Controls.Add(this.groupBoxInput);
            this.tabPageIngest.Location = new System.Drawing.Point(4, 22);
            this.tabPageIngest.Name = "tabPageIngest";
            this.tabPageIngest.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIngest.Size = new System.Drawing.Size(897, 480);
            this.tabPageIngest.TabIndex = 1;
            this.tabPageIngest.Text = "Ingest tool";
            this.tabPageIngest.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.objectListViewMain);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 84);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(891, 393);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Processing";
            // 
            // objectListViewMain
            // 
            this.objectListViewMain.AllColumns.Add(this.olvColumnNo);
            this.objectListViewMain.AllColumns.Add(this.olvColumnOrigName);
            this.objectListViewMain.AllColumns.Add(this.olvColumnStatus);
            this.objectListViewMain.AllColumns.Add(this.olvColumnOutputPath);
            this.objectListViewMain.CellEditUseWholeCell = false;
            this.objectListViewMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnNo,
            this.olvColumnOrigName,
            this.olvColumnStatus,
            this.olvColumnOutputPath});
            this.objectListViewMain.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListViewMain.FullRowSelect = true;
            this.objectListViewMain.GridLines = true;
            this.objectListViewMain.HideSelection = false;
            this.objectListViewMain.Location = new System.Drawing.Point(3, 16);
            this.objectListViewMain.Name = "objectListViewMain";
            this.objectListViewMain.ShowGroups = false;
            this.objectListViewMain.Size = new System.Drawing.Size(885, 374);
            this.objectListViewMain.TabIndex = 0;
            this.objectListViewMain.UseCompatibleStateImageBehavior = false;
            this.objectListViewMain.UseNotifyPropertyChanged = true;
            this.objectListViewMain.View = System.Windows.Forms.View.Details;
            this.objectListViewMain.SelectionChanged += new System.EventHandler(this.objectListViewMain_SelectionChanged);
            // 
            // olvColumnNo
            // 
            this.olvColumnNo.Text = "#";
            this.olvColumnNo.Width = 40;
            // 
            // olvColumnOrigName
            // 
            this.olvColumnOrigName.Text = "Original Filename";
            this.olvColumnOrigName.Width = 185;
            // 
            // olvColumnStatus
            // 
            this.olvColumnStatus.Text = "Status";
            this.olvColumnStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumnStatus.Width = 96;
            // 
            // olvColumnOutputPath
            // 
            this.olvColumnOutputPath.AspectName = "";
            this.olvColumnOutputPath.Text = "Output path (relative)";
            this.olvColumnOutputPath.Width = 400;
            // 
            // groupBoxInput
            // 
            this.groupBoxInput.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxInput.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxInput.Location = new System.Drawing.Point(3, 3);
            this.groupBoxInput.Name = "groupBoxInput";
            this.groupBoxInput.Size = new System.Drawing.Size(891, 81);
            this.groupBoxInput.TabIndex = 0;
            this.groupBoxInput.TabStop = false;
            this.groupBoxInput.Text = "Input";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.button0Read);
            this.flowLayoutPanel1.Controls.Add(this.button1Fillin);
            this.flowLayoutPanel1.Controls.Add(this.button2Process);
            this.flowLayoutPanel1.Controls.Add(this.button3Verify);
            this.flowLayoutPanel1.Controls.Add(this.button4CopyToOut);
            this.flowLayoutPanel1.Controls.Add(this.button9Export);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(885, 58);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // button0Read
            // 
            this.button0Read.AutoSize = true;
            this.button0Read.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button0Read.Location = new System.Drawing.Point(3, 3);
            this.button0Read.Name = "button0Read";
            this.button0Read.Size = new System.Drawing.Size(200, 23);
            this.button0Read.TabIndex = 0;
            this.button0Read.Text = "0 - Read from ingest and output folders";
            this.button0Read.UseVisualStyleBackColor = true;
            this.button0Read.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // button1Fillin
            // 
            this.button1Fillin.AutoSize = true;
            this.button1Fillin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1Fillin.Location = new System.Drawing.Point(209, 3);
            this.button1Fillin.Name = "button1Fillin";
            this.button1Fillin.Size = new System.Drawing.Size(292, 23);
            this.button1Fillin.TabIndex = 0;
            this.button1Fillin.Text = "1 - Fill in missing information (select on list for single editing)";
            this.button1Fillin.UseVisualStyleBackColor = true;
            this.button1Fillin.Click += new System.EventHandler(this.buttonSelectInvalid_Click);
            // 
            // button2Process
            // 
            this.button2Process.AutoSize = true;
            this.button2Process.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2Process.Location = new System.Drawing.Point(507, 3);
            this.button2Process.Name = "button2Process";
            this.button2Process.Size = new System.Drawing.Size(228, 23);
            this.button2Process.TabIndex = 0;
            this.button2Process.Text = "2 - Process all/selected (if NeedsProcessing)";
            this.button2Process.UseVisualStyleBackColor = true;
            this.button2Process.Click += new System.EventHandler(this.button2Process_Click);
            // 
            // button3Verify
            // 
            this.button3Verify.AutoSize = true;
            this.button3Verify.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button3Verify.Location = new System.Drawing.Point(741, 3);
            this.button3Verify.Name = "button3Verify";
            this.button3Verify.Size = new System.Drawing.Size(132, 23);
            this.button3Verify.TabIndex = 0;
            this.button3Verify.Text = "3 - Verify (if NeedsVerify)";
            this.button3Verify.UseVisualStyleBackColor = true;
            this.button3Verify.Click += new System.EventHandler(this.button3Verify_Click);
            // 
            // button4CopyToOut
            // 
            this.button4CopyToOut.AutoSize = true;
            this.button4CopyToOut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button4CopyToOut.Location = new System.Drawing.Point(3, 32);
            this.button4CopyToOut.Name = "button4CopyToOut";
            this.button4CopyToOut.Size = new System.Drawing.Size(217, 23);
            this.button4CopyToOut.TabIndex = 0;
            this.button4CopyToOut.Text = "4 - Copy to output folders (if PASS or FAIL)";
            this.button4CopyToOut.UseVisualStyleBackColor = true;
            this.button4CopyToOut.Click += new System.EventHandler(this.button4CopyToOut_Click);
            // 
            // button9Export
            // 
            this.button9Export.AutoSize = true;
            this.button9Export.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button9Export.Location = new System.Drawing.Point(226, 32);
            this.button9Export.Name = "button9Export";
            this.button9Export.Size = new System.Drawing.Size(92, 23);
            this.button9Export.TabIndex = 0;
            this.button9Export.Text = "9 - Export report";
            this.button9Export.UseVisualStyleBackColor = true;
            // 
            // tabPageVerify
            // 
            this.tabPageVerify.Controls.Add(this.verificationTool1);
            this.tabPageVerify.Location = new System.Drawing.Point(4, 22);
            this.tabPageVerify.Name = "tabPageVerify";
            this.tabPageVerify.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageVerify.Size = new System.Drawing.Size(897, 480);
            this.tabPageVerify.TabIndex = 3;
            this.tabPageVerify.Text = "Verification tool";
            this.tabPageVerify.UseVisualStyleBackColor = true;
            // 
            // verificationTool1
            // 
            this.verificationTool1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verificationTool1.Location = new System.Drawing.Point(3, 3);
            this.verificationTool1.Name = "verificationTool1";
            this.verificationTool1.Size = new System.Drawing.Size(891, 474);
            this.verificationTool1.TabIndex = 0;
            // 
            // tabPageHistory
            // 
            this.tabPageHistory.Location = new System.Drawing.Point(4, 22);
            this.tabPageHistory.Name = "tabPageHistory";
            this.tabPageHistory.Size = new System.Drawing.Size(897, 480);
            this.tabPageHistory.TabIndex = 2;
            this.tabPageHistory.Text = "History";
            this.tabPageHistory.UseVisualStyleBackColor = true;
            // 
            // ModpackToolWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 531);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ModpackToolWindow";
            this.Text = "ModpackTool";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageConfiguration.ResumeLayout(false);
            this.tabPageConfiguration.PerformLayout();
            this.tabPageIngest.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewMain)).EndInit();
            this.groupBoxInput.ResumeLayout(false);
            this.groupBoxInput.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabPageVerify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton configSaveBtn;
        private System.Windows.Forms.ToolStripButton configLoadBtn;
        private System.Windows.Forms.ToolStripButton configBrowseBtn;
        private System.Windows.Forms.ToolStripButton configDeleteBtn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageConfiguration;
        private System.Windows.Forms.TabPage tabPageIngest;
        private System.Windows.Forms.TabPage tabPageHistory;
        private System.Windows.Forms.GroupBox groupBox3;
        private BrightIdeasSoftware.ObjectListView objectListViewMain;
        private BrightIdeasSoftware.OLVColumn olvColumnNo;
        private BrightIdeasSoftware.OLVColumn olvColumnOrigName;
        private BrightIdeasSoftware.OLVColumn olvColumnStatus;
        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button0Read;
        private BrightIdeasSoftware.OLVColumn olvColumnOutputPath;
        private System.Windows.Forms.Button button1Fillin;
        private System.Windows.Forms.Button button3Verify;
        private System.Windows.Forms.Button button4CopyToOut;
        private System.Windows.Forms.Button button9Export;
        private System.Windows.Forms.ToolStripComboBox configInputBox;
        private ToolConfigurationEditor toolConfigurationEditor1;
        private System.Windows.Forms.Button button2Process;
        private System.Windows.Forms.TabPage tabPageVerify;
        private Windows.VerificationTool verificationTool1;
    }
}