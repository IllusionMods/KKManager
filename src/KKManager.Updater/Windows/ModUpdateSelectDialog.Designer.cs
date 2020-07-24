namespace KKManager.Updater.Windows
{
	partial class ModUpdateSelectDialog
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
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelDownload = new System.Windows.Forms.Label();
            this.buttonNone = new System.Windows.Forms.Button();
            this.buttonAll = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.objectListView2 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnFileName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFileDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFileSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView2)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnName);
            this.objectListView1.AllColumns.Add(this.olvColumnDate);
            this.objectListView1.AllColumns.Add(this.olvColumnSize);
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.CheckBoxes = true;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnDate,
            this.olvColumnSize});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.GridLines = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.Location = new System.Drawing.Point(0, 17);
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowGroups = false;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.Size = new System.Drawing.Size(622, 181);
            this.objectListView1.TabIndex = 1;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.objectListView1_ItemChecked);
            this.objectListView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.objectListView1_ItemSelectionChanged);
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "TaskName";
            this.olvColumnName.FillsFreeSpace = true;
            this.olvColumnName.Hideable = false;
            this.olvColumnName.MinimumWidth = 60;
            this.olvColumnName.Text = "Task name";
            this.olvColumnName.Width = 332;
            // 
            // olvColumnDate
            // 
            this.olvColumnDate.AspectName = "ModifiedTime";
            this.olvColumnDate.Hideable = false;
            this.olvColumnDate.MinimumWidth = 60;
            this.olvColumnDate.Text = "Update date";
            this.olvColumnDate.Width = 132;
            // 
            // olvColumnSize
            // 
            this.olvColumnSize.AspectName = "TotalUpdateSize";
            this.olvColumnSize.Hideable = false;
            this.olvColumnSize.MinimumWidth = 60;
            this.olvColumnSize.Text = "Update Size";
            this.olvColumnSize.Width = 82;
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonCancel.Location = new System.Drawing.Point(553, 6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.buttonCancel.Size = new System.Drawing.Size(75, 29);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonAccept
            // 
            this.buttonAccept.AutoSize = true;
            this.buttonAccept.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonAccept.Location = new System.Drawing.Point(420, 6);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.buttonAccept.Size = new System.Drawing.Size(133, 29);
            this.buttonAccept.TabIndex = 3;
            this.buttonAccept.Text = "Update selected mods";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelDownload);
            this.panel1.Controls.Add(this.buttonNone);
            this.panel1.Controls.Add(this.buttonAll);
            this.panel1.Controls.Add(this.buttonAccept);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 420);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(6);
            this.panel1.Size = new System.Drawing.Size(634, 41);
            this.panel1.TabIndex = 2;
            // 
            // labelDownload
            // 
            this.labelDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDownload.Location = new System.Drawing.Point(160, 6);
            this.labelDownload.Name = "labelDownload";
            this.labelDownload.Size = new System.Drawing.Size(260, 29);
            this.labelDownload.TabIndex = 5;
            this.labelDownload.Text = "123 MB to download";
            this.labelDownload.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonNone
            // 
            this.buttonNone.AutoSize = true;
            this.buttonNone.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonNone.Location = new System.Drawing.Point(76, 6);
            this.buttonNone.Name = "buttonNone";
            this.buttonNone.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.buttonNone.Size = new System.Drawing.Size(84, 29);
            this.buttonNone.TabIndex = 1;
            this.buttonNone.Text = "Select none";
            this.buttonNone.UseVisualStyleBackColor = true;
            this.buttonNone.Click += new System.EventHandler(this.SelectNone);
            // 
            // buttonAll
            // 
            this.buttonAll.AutoSize = true;
            this.buttonAll.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonAll.Location = new System.Drawing.Point(6, 6);
            this.buttonAll.Name = "buttonAll";
            this.buttonAll.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.buttonAll.Size = new System.Drawing.Size(70, 29);
            this.buttonAll.TabIndex = 0;
            this.buttonAll.Text = "Select all";
            this.buttonAll.UseVisualStyleBackColor = true;
            this.buttonAll.Click += new System.EventHandler(this.SelectAll);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(6);
            this.panel2.Size = new System.Drawing.Size(634, 420);
            this.panel2.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(6, 6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer1.Panel1Collapsed = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(622, 408);
            this.splitContainer1.SplitterDistance = 86;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.objectListView1);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.objectListView2);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new System.Drawing.Size(622, 408);
            this.splitContainer2.SplitterDistance = 198;
            this.splitContainer2.SplitterWidth = 7;
            this.splitContainer2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label1.Size = new System.Drawing.Size(526, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mods that can be updated. Select which updates to download and install, then clic" +
    "k the Update button below.";
            // 
            // objectListView2
            // 
            this.objectListView2.AllColumns.Add(this.olvColumnFileName);
            this.objectListView2.AllColumns.Add(this.olvColumnFileDate);
            this.objectListView2.AllColumns.Add(this.olvColumnFileSize);
            this.objectListView2.CellEditUseWholeCell = false;
            this.objectListView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnFileName,
            this.olvColumnFileDate,
            this.olvColumnFileSize});
            this.objectListView2.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView2.FullRowSelect = true;
            this.objectListView2.GridLines = true;
            this.objectListView2.HideSelection = false;
            this.objectListView2.Location = new System.Drawing.Point(0, 30);
            this.objectListView2.MultiSelect = false;
            this.objectListView2.Name = "objectListView2";
            this.objectListView2.ShowGroups = false;
            this.objectListView2.ShowItemToolTips = true;
            this.objectListView2.Size = new System.Drawing.Size(622, 173);
            this.objectListView2.TabIndex = 2;
            this.objectListView2.UseCompatibleStateImageBehavior = false;
            this.objectListView2.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnFileName
            // 
            this.olvColumnFileName.AspectName = "";
            this.olvColumnFileName.FillsFreeSpace = true;
            this.olvColumnFileName.Hideable = false;
            this.olvColumnFileName.Text = "File name";
            this.olvColumnFileName.Width = 333;
            // 
            // olvColumnFileDate
            // 
            this.olvColumnFileDate.AspectName = "";
            this.olvColumnFileDate.Hideable = false;
            this.olvColumnFileDate.MinimumWidth = 60;
            this.olvColumnFileDate.Text = "Update date";
            this.olvColumnFileDate.Width = 119;
            // 
            // olvColumnFileSize
            // 
            this.olvColumnFileSize.AspectName = "";
            this.olvColumnFileSize.Hideable = false;
            this.olvColumnFileSize.MinimumWidth = 60;
            this.olvColumnFileSize.Text = "File size";
            this.olvColumnFileSize.Width = 79;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label2.Size = new System.Drawing.Size(480, 30);
            this.label2.TabIndex = 3;
            this.label2.Text = "Contents of the selected update. All paths are relative to the game\'s root direct" +
    "ory.\r\nFiles in green are completely new additions. Files in red are no longer ne" +
    "cessary and will be removed.";
            // 
            // ModUpdateSelectDialog
            // 
            this.AcceptButton = this.buttonAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(634, 461);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ModUpdateSelectDialog";
            this.ShowIcon = false;
            this.Text = "Select which mods to update";
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView2)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView objectListView1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Panel panel1;
		private BrightIdeasSoftware.OLVColumn olvColumnName;
		private BrightIdeasSoftware.OLVColumn olvColumnDate;
		private System.Windows.Forms.Button buttonNone;
		private System.Windows.Forms.Button buttonAll;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BrightIdeasSoftware.OLVColumn olvColumnSize;
        private System.Windows.Forms.Label labelDownload;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.ObjectListView objectListView2;
        private System.Windows.Forms.Label label2;
        private BrightIdeasSoftware.OLVColumn olvColumnFileName;
        private BrightIdeasSoftware.OLVColumn olvColumnFileDate;
        private BrightIdeasSoftware.OLVColumn olvColumnFileSize;
    }
}