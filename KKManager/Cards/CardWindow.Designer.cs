namespace KKManager.Cards
{
	partial class CardWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ctxCards = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeGenderToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fermaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnModDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripViewSelect = new System.Windows.Forms.ToolStripDropDownButton();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.ctxCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 491);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(29, 17);
            this.toolStripStatusLabel1.Text = "Idle.";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(980, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "Showing (x / x)";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ctxCards
            // 
            this.ctxCards.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.changeGenderToToolStripMenuItem,
            this.moveToolStripMenuItem});
            this.ctxCards.Name = "ctxCards";
            this.ctxCards.Size = new System.Drawing.Size(179, 92);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // changeGenderToToolStripMenuItem
            // 
            this.changeGenderToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maleToolStripMenuItem,
            this.fermaleToolStripMenuItem});
            this.changeGenderToToolStripMenuItem.Name = "changeGenderToToolStripMenuItem";
            this.changeGenderToToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.changeGenderToToolStripMenuItem.Text = "Change gender to...";
            // 
            // maleToolStripMenuItem
            // 
            this.maleToolStripMenuItem.Name = "maleToolStripMenuItem";
            this.maleToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.maleToolStripMenuItem.Text = "Male";
            // 
            // fermaleToolStripMenuItem
            // 
            this.fermaleToolStripMenuItem.Name = "fermaleToolStripMenuItem";
            this.fermaleToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.fermaleToolStripMenuItem.Text = "Fermale";
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.moveToolStripMenuItem.Text = "Move";
            // 
            // listView
            // 
            this.listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView.AllColumns.Add(this.olvColumnName);
            this.listView.AllColumns.Add(this.olvColumnModDate);
            this.listView.AllColumns.Add(this.olvColumnFilename);
            this.listView.CellEditUseWholeCell = false;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnModDate,
            this.olvColumnFilename});
            this.listView.ContextMenuStrip = this.ctxCards;
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(1024, 466);
            this.listView.TabIndex = 3;
            this.listView.TileSize = new System.Drawing.Size(200, 200);
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.LargeIcon;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.fastObjectListView1_SelectedIndexChanged);
            // 
            // olvColumnName
            // 
            this.olvColumnName.Text = "Name";
            // 
            // olvColumnModDate
            // 
            this.olvColumnModDate.Text = "Modified";
            // 
            // olvColumnFilename
            // 
            this.olvColumnFilename.Text = "Filename";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripViewSelect});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1024, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripViewSelect
            // 
            this.toolStripViewSelect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem});
            this.toolStripViewSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripViewSelect.Image")));
            this.toolStripViewSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripViewSelect.Name = "toolStripViewSelect";
            this.toolStripViewSelect.Size = new System.Drawing.Size(94, 22);
            this.toolStripViewSelect.Text = "View select";
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.Details);
            // 
            // smallIconsToolStripMenuItem
            // 
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.smallIconsToolStripMenuItem.Text = "Small icons";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.SmallIcons);
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.largeIconsToolStripMenuItem.Text = "Large icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.LargeIcons);
            // 
            // CardWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 513);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "CardWindow";
            this.Text = "Cards";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ctxCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ContextMenuStrip ctxCards;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeGenderToToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem maleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fermaleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private BrightIdeasSoftware.ObjectListView listView;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnModDate;
        private BrightIdeasSoftware.OLVColumn olvColumnFilename;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripViewSelect;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
    }
}

