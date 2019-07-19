namespace KKManager.Windows.Dialogs
{
	partial class SideloaderUpdateWindow
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
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkBoxShowDone = new System.Windows.Forms.CheckBox();
			this.buttonNone = new System.Windows.Forms.Button();
			this.buttonAll = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// objectListView1
			// 
			this.objectListView1.AllColumns.Add(this.olvColumnName);
			this.objectListView1.AllColumns.Add(this.olvColumnDate);
			this.objectListView1.CellEditUseWholeCell = false;
			this.objectListView1.CheckBoxes = true;
			this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnDate});
			this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
			this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objectListView1.FullRowSelect = true;
			this.objectListView1.GridLines = true;
			this.objectListView1.Location = new System.Drawing.Point(0, 21);
			this.objectListView1.Name = "objectListView1";
			this.objectListView1.ShowGroups = false;
			this.objectListView1.Size = new System.Drawing.Size(800, 388);
			this.objectListView1.TabIndex = 1;
			this.objectListView1.UseCompatibleStateImageBehavior = false;
			this.objectListView1.View = System.Windows.Forms.View.Details;
			// 
			// olvColumnName
			// 
			this.olvColumnName.AspectName = "DisplayName";
			this.olvColumnName.Text = "Filename";
			this.olvColumnName.Width = 515;
			// 
			// olvColumnDate
			// 
			this.olvColumnDate.AspectName = "UpdateDate";
			this.olvColumnDate.Text = "UpdateDate";
			this.olvColumnDate.Width = 132;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(4);
			this.label1.Size = new System.Drawing.Size(721, 21);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select which files to update. Files in gray are already up to date. Files in gree" +
    "n are new mods. Files in red are no longer necessary and will be removed.";
			// 
			// buttonCancel
			// 
			this.buttonCancel.AutoSize = true;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonCancel.Location = new System.Drawing.Point(719, 6);
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
			this.buttonAccept.Location = new System.Drawing.Point(614, 6);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.buttonAccept.Size = new System.Drawing.Size(105, 29);
			this.buttonAccept.TabIndex = 3;
			this.buttonAccept.Text = "Update selected";
			this.buttonAccept.UseVisualStyleBackColor = true;
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.checkBoxShowDone);
			this.panel1.Controls.Add(this.buttonNone);
			this.panel1.Controls.Add(this.buttonAll);
			this.panel1.Controls.Add(this.buttonAccept);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 409);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(6);
			this.panel1.Size = new System.Drawing.Size(800, 41);
			this.panel1.TabIndex = 2;
			// 
			// checkBoxShowDone
			// 
			this.checkBoxShowDone.AutoSize = true;
			this.checkBoxShowDone.Dock = System.Windows.Forms.DockStyle.Left;
			this.checkBoxShowDone.Location = new System.Drawing.Point(160, 6);
			this.checkBoxShowDone.Name = "checkBoxShowDone";
			this.checkBoxShowDone.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
			this.checkBoxShowDone.Size = new System.Drawing.Size(143, 29);
			this.checkBoxShowDone.TabIndex = 2;
			this.checkBoxShowDone.Text = "Show up-to-date mods";
			this.checkBoxShowDone.UseVisualStyleBackColor = true;
			this.checkBoxShowDone.CheckedChanged += new System.EventHandler(this.UpdateListObjects);
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
			// SideloaderUpdateWindow
			// 
			this.AcceptButton = this.buttonAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.objectListView1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label1);
			this.Name = "SideloaderUpdateWindow";
			this.Text = "SideloaderUpdateWindow";
			((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView objectListView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Panel panel1;
		private BrightIdeasSoftware.OLVColumn olvColumnName;
		private BrightIdeasSoftware.OLVColumn olvColumnDate;
		private System.Windows.Forms.CheckBox checkBoxShowDone;
		private System.Windows.Forms.Button buttonNone;
		private System.Windows.Forms.Button buttonAll;
	}
}