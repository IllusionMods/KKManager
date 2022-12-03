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
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.hiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.configInputBox = new System.Windows.Forms.ToolStripComboBox();
            this.configBrowseBtn = new System.Windows.Forms.ToolStripButton();
            this.configSaveBtn = new System.Windows.Forms.ToolStripButton();
            this.configLoadBtn = new System.Windows.Forms.ToolStripButton();
            this.configDeleteBtn = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.objectListViewMain = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnNo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnOrigName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnManifest = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnOutputDir = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnStatus = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBoxInput = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1Read = new System.Windows.Forms.Button();
            this.button2Fillin = new System.Windows.Forms.Button();
            this.button3Verify = new System.Windows.Forms.Button();
            this.button4CopyToOut = new System.Windows.Forms.Button();
            this.button5Export = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.toolConfigurationEditor1 = new KKManager.ModpackTool.ToolConfigurationEditor();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewMain)).BeginInit();
            this.groupBoxInput.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.configInputBox,
            this.configBrowseBtn,
            this.configSaveBtn,
            this.configLoadBtn,
            this.configDeleteBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(905, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hiToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton1.Text = "Options";
            // 
            // hiToolStripMenuItem
            // 
            this.hiToolStripMenuItem.Name = "hiToolStripMenuItem";
            this.hiToolStripMenuItem.Size = new System.Drawing.Size(84, 22);
            this.hiToolStripMenuItem.Text = "hi";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
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
            this.configBrowseBtn.Size = new System.Drawing.Size(23, 22);
            this.configBrowseBtn.Text = "...";
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(905, 482);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.toolConfigurationEditor1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(897, 456);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Configuration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBoxInput);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(897, 456);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Ingest tool";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.objectListViewMain);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 84);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(891, 369);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Processing";
            // 
            // objectListViewMain
            // 
            this.objectListViewMain.AllColumns.Add(this.olvColumnNo);
            this.objectListViewMain.AllColumns.Add(this.olvColumnOrigName);
            this.objectListViewMain.AllColumns.Add(this.olvColumnManifest);
            this.objectListViewMain.AllColumns.Add(this.olvColumnOutputDir);
            this.objectListViewMain.AllColumns.Add(this.olvColumnStatus);
            this.objectListViewMain.CellEditUseWholeCell = false;
            this.objectListViewMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnNo,
            this.olvColumnOrigName,
            this.olvColumnManifest,
            this.olvColumnOutputDir,
            this.olvColumnStatus});
            this.objectListViewMain.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListViewMain.FullRowSelect = true;
            this.objectListViewMain.GridLines = true;
            this.objectListViewMain.HideSelection = false;
            this.objectListViewMain.Location = new System.Drawing.Point(3, 16);
            this.objectListViewMain.Name = "objectListViewMain";
            this.objectListViewMain.ShowGroups = false;
            this.objectListViewMain.Size = new System.Drawing.Size(885, 350);
            this.objectListViewMain.TabIndex = 0;
            this.objectListViewMain.UseCompatibleStateImageBehavior = false;
            this.objectListViewMain.View = System.Windows.Forms.View.Details;
            this.objectListViewMain.SelectionChanged += new System.EventHandler(this.objectListViewMain_SelectionChanged);
            // 
            // olvColumnNo
            // 
            this.olvColumnNo.Text = "#";
            // 
            // olvColumnOrigName
            // 
            this.olvColumnOrigName.Text = "Original Filename";
            this.olvColumnOrigName.Width = 185;
            // 
            // olvColumnManifest
            // 
            this.olvColumnManifest.Text = "Has manifest";
            this.olvColumnManifest.Width = 108;
            // 
            // olvColumnOutputDir
            // 
            this.olvColumnOutputDir.Text = "Output folder";
            this.olvColumnOutputDir.Width = 224;
            // 
            // olvColumnStatus
            // 
            this.olvColumnStatus.Text = "Status";
            this.olvColumnStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            this.flowLayoutPanel1.Controls.Add(this.button1Read);
            this.flowLayoutPanel1.Controls.Add(this.button2Fillin);
            this.flowLayoutPanel1.Controls.Add(this.button3Verify);
            this.flowLayoutPanel1.Controls.Add(this.button4CopyToOut);
            this.flowLayoutPanel1.Controls.Add(this.button5Export);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(885, 58);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // button1Read
            // 
            this.button1Read.AutoSize = true;
            this.button1Read.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1Read.Location = new System.Drawing.Point(3, 3);
            this.button1Read.Name = "button1Read";
            this.button1Read.Size = new System.Drawing.Size(200, 23);
            this.button1Read.TabIndex = 0;
            this.button1Read.Text = "1 - Read from ingest and output folders";
            this.button1Read.UseVisualStyleBackColor = true;
            this.button1Read.Click += new System.EventHandler(this.button1Read_Click);
            // 
            // button2Fillin
            // 
            this.button2Fillin.AutoSize = true;
            this.button2Fillin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2Fillin.Location = new System.Drawing.Point(209, 3);
            this.button2Fillin.Name = "button2Fillin";
            this.button2Fillin.Size = new System.Drawing.Size(292, 23);
            this.button2Fillin.TabIndex = 0;
            this.button2Fillin.Text = "2 - Fill in missing information (select on list for single editing)";
            this.button2Fillin.UseVisualStyleBackColor = true;
            this.button2Fillin.Click += new System.EventHandler(this.button3_Click);
            // 
            // button3Verify
            // 
            this.button3Verify.AutoSize = true;
            this.button3Verify.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button3Verify.Location = new System.Drawing.Point(507, 3);
            this.button3Verify.Name = "button3Verify";
            this.button3Verify.Size = new System.Drawing.Size(199, 23);
            this.button3Verify.TabIndex = 0;
            this.button3Verify.Text = "3 - Verify all/selected (if ready to verify)";
            this.button3Verify.UseVisualStyleBackColor = true;
            // 
            // button4CopyToOut
            // 
            this.button4CopyToOut.AutoSize = true;
            this.button4CopyToOut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button4CopyToOut.Location = new System.Drawing.Point(712, 3);
            this.button4CopyToOut.Name = "button4CopyToOut";
            this.button4CopyToOut.Size = new System.Drawing.Size(135, 23);
            this.button4CopyToOut.TabIndex = 0;
            this.button4CopyToOut.Text = "4 - Copy to output folders";
            this.button4CopyToOut.UseVisualStyleBackColor = true;
            // 
            // button5Export
            // 
            this.button5Export.AutoSize = true;
            this.button5Export.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button5Export.Location = new System.Drawing.Point(3, 32);
            this.button5Export.Name = "button5Export";
            this.button5Export.Size = new System.Drawing.Size(92, 23);
            this.button5Export.TabIndex = 0;
            this.button5Export.Text = "5 - Export report";
            this.button5Export.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(897, 456);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "History";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // toolConfigurationEditor1
            // 
            this.toolConfigurationEditor1.AutoSize = true;
            this.toolConfigurationEditor1.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolConfigurationEditor1.Location = new System.Drawing.Point(3, 3);
            this.toolConfigurationEditor1.Name = "toolConfigurationEditor1";
            this.toolConfigurationEditor1.Size = new System.Drawing.Size(891, 149);
            this.toolConfigurationEditor1.TabIndex = 2;
            // 
            // ModpackToolWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 507);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ModpackToolWindow";
            this.Text = "ModpackTool";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewMain)).EndInit();
            this.groupBoxInput.ResumeLayout(false);
            this.groupBoxInput.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton configSaveBtn;
        private System.Windows.Forms.ToolStripButton configLoadBtn;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem hiToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton configBrowseBtn;
        private System.Windows.Forms.ToolStripButton configDeleteBtn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private BrightIdeasSoftware.ObjectListView objectListViewMain;
        private BrightIdeasSoftware.OLVColumn olvColumnNo;
        private BrightIdeasSoftware.OLVColumn olvColumnOrigName;
        private BrightIdeasSoftware.OLVColumn olvColumnManifest;
        private BrightIdeasSoftware.OLVColumn olvColumnStatus;
        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button1Read;
        private BrightIdeasSoftware.OLVColumn olvColumnOutputDir;
        private System.Windows.Forms.Button button2Fillin;
        private System.Windows.Forms.Button button3Verify;
        private System.Windows.Forms.Button button4CopyToOut;
        private System.Windows.Forms.Button button5Export;
        private System.Windows.Forms.ToolStripComboBox configInputBox;
        private ToolConfigurationEditor toolConfigurationEditor1;
    }
}