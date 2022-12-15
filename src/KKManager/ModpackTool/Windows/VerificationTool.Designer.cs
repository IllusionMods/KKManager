namespace KKManager.ModpackTool.Windows
{
    partial class VerificationTool
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonOpenTestdir = new System.Windows.Forms.Button();
            this.buttonStartGame = new System.Windows.Forms.Button();
            this.buttonStartStudio = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonStartVerify = new System.Windows.Forms.Button();
            this.buttonPass = new System.Windows.Forms.Button();
            this.buttonFail = new System.Windows.Forms.Button();
            this.buttonReverify = new System.Windows.Forms.Button();
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnIndex = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnStatus = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnContent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 381);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(580, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shortcuts";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.buttonOpenTestdir);
            this.flowLayoutPanel1.Controls.Add(this.buttonStartGame);
            this.flowLayoutPanel1.Controls.Add(this.buttonStartStudio);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(574, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // buttonOpenTestdir
            // 
            this.buttonOpenTestdir.AutoSize = true;
            this.buttonOpenTestdir.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonOpenTestdir.Location = new System.Drawing.Point(3, 3);
            this.buttonOpenTestdir.Name = "buttonOpenTestdir";
            this.buttonOpenTestdir.Size = new System.Drawing.Size(143, 23);
            this.buttonOpenTestdir.TabIndex = 0;
            this.buttonOpenTestdir.Text = "Open test folder in explorer";
            this.buttonOpenTestdir.UseVisualStyleBackColor = true;
            this.buttonOpenTestdir.Click += new System.EventHandler(this.buttonOpenTestdir_Click);
            // 
            // buttonStartGame
            // 
            this.buttonStartGame.AutoSize = true;
            this.buttonStartGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonStartGame.Location = new System.Drawing.Point(152, 3);
            this.buttonStartGame.Name = "buttonStartGame";
            this.buttonStartGame.Size = new System.Drawing.Size(156, 23);
            this.buttonStartGame.TabIndex = 1;
            this.buttonStartGame.Text = "Start the game (for test folder)";
            this.buttonStartGame.UseVisualStyleBackColor = true;
            this.buttonStartGame.Click += new System.EventHandler(this.buttonStartGame_Click);
            // 
            // buttonStartStudio
            // 
            this.buttonStartStudio.AutoSize = true;
            this.buttonStartStudio.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonStartStudio.Location = new System.Drawing.Point(314, 3);
            this.buttonStartStudio.Name = "buttonStartStudio";
            this.buttonStartStudio.Size = new System.Drawing.Size(140, 23);
            this.buttonStartStudio.TabIndex = 1;
            this.buttonStartStudio.Text = "Start studio (for test folder)";
            this.buttonStartStudio.UseVisualStyleBackColor = true;
            this.buttonStartStudio.Click += new System.EventHandler(this.buttonStartStudio_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox3.Controls.Add(this.flowLayoutPanel2);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(0, 304);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(580, 77);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Selected mods:";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.buttonStartVerify);
            this.flowLayoutPanel2.Controls.Add(this.buttonPass);
            this.flowLayoutPanel2.Controls.Add(this.buttonFail);
            this.flowLayoutPanel2.Controls.Add(this.buttonReverify);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(574, 58);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // buttonStartVerify
            // 
            this.buttonStartVerify.AutoSize = true;
            this.buttonStartVerify.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.SetFlowBreak(this.buttonStartVerify, true);
            this.buttonStartVerify.Location = new System.Drawing.Point(3, 3);
            this.buttonStartVerify.Name = "buttonStartVerify";
            this.buttonStartVerify.Size = new System.Drawing.Size(396, 23);
            this.buttonStartVerify.TabIndex = 1;
            this.buttonStartVerify.Text = "Start verification / Copy to test folder (removes all existing files in /mods/_te" +
    "sting)";
            this.buttonStartVerify.UseVisualStyleBackColor = true;
            this.buttonStartVerify.Click += new System.EventHandler(this.buttonStartVerify_Click);
            // 
            // buttonPass
            // 
            this.buttonPass.BackColor = System.Drawing.Color.LimeGreen;
            this.buttonPass.Location = new System.Drawing.Point(3, 32);
            this.buttonPass.Name = "buttonPass";
            this.buttonPass.Size = new System.Drawing.Size(75, 23);
            this.buttonPass.TabIndex = 2;
            this.buttonPass.Text = "PASS";
            this.buttonPass.UseVisualStyleBackColor = false;
            this.buttonPass.Click += new System.EventHandler(this.buttonPass_Click);
            // 
            // buttonFail
            // 
            this.buttonFail.BackColor = System.Drawing.Color.Salmon;
            this.buttonFail.Location = new System.Drawing.Point(84, 32);
            this.buttonFail.Name = "buttonFail";
            this.buttonFail.Size = new System.Drawing.Size(75, 23);
            this.buttonFail.TabIndex = 3;
            this.buttonFail.Text = "FAIL";
            this.buttonFail.UseVisualStyleBackColor = false;
            this.buttonFail.Click += new System.EventHandler(this.buttonFail_Click);
            // 
            // buttonReverify
            // 
            this.buttonReverify.Location = new System.Drawing.Point(165, 32);
            this.buttonReverify.Name = "buttonReverify";
            this.buttonReverify.Size = new System.Drawing.Size(75, 23);
            this.buttonReverify.TabIndex = 0;
            this.buttonReverify.Text = "Reverify";
            this.buttonReverify.UseVisualStyleBackColor = true;
            this.buttonReverify.Click += new System.EventHandler(this.buttonReverify_Click);
            // 
            // objectListView
            // 
            this.objectListView.AllColumns.Add(this.olvColumnIndex);
            this.objectListView.AllColumns.Add(this.olvColumnStatus);
            this.objectListView.AllColumns.Add(this.olvColumnName);
            this.objectListView.AllColumns.Add(this.olvColumnContent);
            this.objectListView.CellEditUseWholeCell = false;
            this.objectListView.CheckBoxes = true;
            this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnIndex,
            this.olvColumnStatus,
            this.olvColumnName,
            this.olvColumnContent});
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView.EmptyListMsg = "Nothing to show here. Only mods with statuses NeedsVerify, Verify, PASS and FAIL " +
    "will appear here.";
            this.objectListView.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectListView.FullRowSelect = true;
            this.objectListView.GridLines = true;
            this.objectListView.HideSelection = false;
            this.objectListView.Location = new System.Drawing.Point(0, 0);
            this.objectListView.Name = "objectListView";
            this.objectListView.ShowGroups = false;
            this.objectListView.Size = new System.Drawing.Size(580, 304);
            this.objectListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.objectListView.TabIndex = 3;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.UseFilterIndicator = true;
            this.objectListView.UseFiltering = true;
            this.objectListView.UseNotifyPropertyChanged = true;
            this.objectListView.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnIndex
            // 
            this.olvColumnIndex.Text = "#";
            this.olvColumnIndex.Width = 47;
            // 
            // olvColumnStatus
            // 
            this.olvColumnStatus.Text = "Status";
            this.olvColumnStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumnStatus.Width = 89;
            // 
            // olvColumnName
            // 
            this.olvColumnName.Text = "Filename (new)";
            this.olvColumnName.Width = 215;
            // 
            // olvColumnContent
            // 
            this.olvColumnContent.Text = "Content type";
            this.olvColumnContent.Width = 162;
            // 
            // VerificationTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.objectListView);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "VerificationTool";
            this.Size = new System.Drawing.Size(580, 429);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonOpenTestdir;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button buttonStartVerify;
        private System.Windows.Forms.Button buttonPass;
        private System.Windows.Forms.Button buttonFail;
        private System.Windows.Forms.Button buttonReverify;
        private BrightIdeasSoftware.ObjectListView objectListView;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnContent;
        private BrightIdeasSoftware.OLVColumn olvColumnStatus;
        private BrightIdeasSoftware.OLVColumn olvColumnIndex;
        private System.Windows.Forms.Button buttonStartGame;
        private System.Windows.Forms.Button buttonStartStudio;
    }
}
