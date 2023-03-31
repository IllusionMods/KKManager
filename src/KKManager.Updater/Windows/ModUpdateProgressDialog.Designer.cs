namespace KKManager.Updater.Windows
{
    partial class ModUpdateProgressDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModUpdateProgressDialog));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxSleep = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.buttonLog = new System.Windows.Forms.Button();
            this.buttonMinimize = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonCancelClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.fastObjectListView1 = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnNo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnProgress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnStatus = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel5 = new System.Windows.Forms.Panel();
            this.labelPercent = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListView1)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Value = 33;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.label1);
            this.panel1.Name = "panel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxSleep
            // 
            resources.ApplyResources(this.checkBoxSleep, "checkBoxSleep");
            this.checkBoxSleep.Name = "checkBoxSleep";
            this.checkBoxSleep.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.buttonLog);
            this.panel2.Controls.Add(this.buttonMinimize);
            this.panel2.Controls.Add(this.checkBoxSleep);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.buttonCancelClose);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel6
            // 
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // buttonLog
            // 
            resources.ApplyResources(this.buttonLog, "buttonLog");
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonViewLog_Click);
            // 
            // buttonMinimize
            // 
            resources.ApplyResources(this.buttonMinimize, "buttonMinimize");
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.UseVisualStyleBackColor = true;
            this.buttonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // buttonCancelClose
            // 
            this.buttonCancelClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancelClose, "buttonCancelClose");
            this.buttonCancelClose.Name = "buttonCancelClose";
            this.buttonCancelClose.UseVisualStyleBackColor = true;
            this.buttonCancelClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::KKManager.Updater.Properties.Resources.chikajump;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.fastObjectListView1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // fastObjectListView1
            // 
            this.fastObjectListView1.AllColumns.Add(this.olvColumnNo);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnName);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnSize);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnProgress);
            this.fastObjectListView1.AllColumns.Add(this.olvColumnStatus);
            this.fastObjectListView1.AllowColumnReorder = true;
            this.fastObjectListView1.CellEditUseWholeCell = false;
            this.fastObjectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnNo,
            this.olvColumnName,
            this.olvColumnSize,
            this.olvColumnProgress,
            this.olvColumnStatus});
            this.fastObjectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.fastObjectListView1, "fastObjectListView1");
            this.fastObjectListView1.FullRowSelect = true;
            this.fastObjectListView1.HideSelection = false;
            this.fastObjectListView1.MultiSelect = false;
            this.fastObjectListView1.Name = "fastObjectListView1";
            this.fastObjectListView1.ShowGroups = false;
            this.fastObjectListView1.ShowItemToolTips = true;
            this.fastObjectListView1.UseCompatibleStateImageBehavior = false;
            this.fastObjectListView1.View = System.Windows.Forms.View.Details;
            this.fastObjectListView1.VirtualMode = true;
            // 
            // olvColumnNo
            // 
            this.olvColumnNo.MinimumWidth = 20;
            resources.ApplyResources(this.olvColumnNo, "olvColumnNo");
            // 
            // olvColumnName
            // 
            this.olvColumnName.FillsFreeSpace = true;
            resources.ApplyResources(this.olvColumnName, "olvColumnName");
            // 
            // olvColumnSize
            // 
            this.olvColumnSize.MinimumWidth = 70;
            resources.ApplyResources(this.olvColumnSize, "olvColumnSize");
            // 
            // olvColumnProgress
            // 
            this.olvColumnProgress.MinimumWidth = 70;
            resources.ApplyResources(this.olvColumnProgress, "olvColumnProgress");
            // 
            // olvColumnStatus
            // 
            this.olvColumnStatus.MinimumWidth = 90;
            resources.ApplyResources(this.olvColumnStatus, "olvColumnStatus");
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Controls.Add(this.labelPercent);
            this.panel5.Controls.Add(this.labelStatus);
            this.panel5.Name = "panel5";
            // 
            // labelPercent
            // 
            resources.ApplyResources(this.labelPercent, "labelPercent");
            this.labelPercent.Name = "labelPercent";
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.Name = "labelStatus";
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 1000;
            // 
            // ModUpdateProgressDialog
            // 
            this.AcceptButton = this.buttonCancelClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModUpdateProgressDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.ModUpdateProgress_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListView1)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonCancelClose;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBoxSleep;
        private System.Windows.Forms.Panel panel4;
        private BrightIdeasSoftware.FastObjectListView fastObjectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnProgress;
        private BrightIdeasSoftware.OLVColumn olvColumnSize;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.Label labelStatus;
        private BrightIdeasSoftware.OLVColumn olvColumnStatus;
        private System.Windows.Forms.Timer updateTimer;
        private BrightIdeasSoftware.OLVColumn olvColumnNo;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.Button buttonMinimize;
    }
}