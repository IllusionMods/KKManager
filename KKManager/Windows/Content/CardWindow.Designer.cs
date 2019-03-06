using System.Windows.Forms;

namespace KKManager.Windows.Content
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
            this.ctxCards = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeGenderToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fermaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSex = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnPersonality = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnExtended = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnModDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.addressBar = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonGo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripOpenDropdown = new System.Windows.Forms.ToolStripSplitButton();
            this.femaleCardFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maleCardFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripViewSelect = new System.Windows.Forms.ToolStripDropDownButton();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.segregateBySexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
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
            this.listView.AllColumns.Add(this.olvColumnSex);
            this.listView.AllColumns.Add(this.olvColumnPersonality);
            this.listView.AllColumns.Add(this.olvColumnExtended);
            this.listView.AllColumns.Add(this.olvColumnModDate);
            this.listView.AllColumns.Add(this.olvColumnFilename);
            this.listView.CellEditUseWholeCell = false;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnSex,
            this.olvColumnPersonality,
            this.olvColumnExtended,
            this.olvColumnModDate,
            this.olvColumnFilename});
            this.listView.ContextMenuStrip = this.ctxCards;
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(1024, 488);
            this.listView.TabIndex = 3;
            this.listView.TileSize = new System.Drawing.Size(200, 200);
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.UseFilterIndicator = true;
            this.listView.UseFiltering = true;
            this.listView.View = System.Windows.Forms.View.LargeIcon;
            this.listView.VirtualMode = true;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // olvColumnName
            // 
            this.olvColumnName.Text = "Name";
            this.olvColumnName.UseFiltering = false;
            // 
            // olvColumnSex
            // 
            this.olvColumnSex.Text = "Sex";
            // 
            // olvColumnPersonality
            // 
            this.olvColumnPersonality.Text = "Personality";
            // 
            // olvColumnExtended
            // 
            this.olvColumnExtended.Searchable = false;
            this.olvColumnExtended.Text = "Extended data count";
            // 
            // olvColumnModDate
            // 
            this.olvColumnModDate.Text = "Modified";
            this.olvColumnModDate.UseFiltering = false;
            // 
            // olvColumnFilename
            // 
            this.olvColumnFilename.Text = "Filename";
            this.olvColumnFilename.UseFiltering = false;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addressBar,
            this.toolStripButtonGo,
            this.toolStripButtonRefresh,
            this.toolStripSeparator3,
            this.toolStripOpenDropdown,
            this.toolStripSeparator1,
            this.toolStripViewSelect,
            this.toolStripDropDownButtonTools});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1024, 25);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            this.toolStrip.Resize += new System.EventHandler(this.OnResizeToolstip);
            // 
            // addressBar
            // 
            this.addressBar.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.addressBar.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.addressBar.AutoSize = false;
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(200, 23);
            this.addressBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addressBar_KeyDown);
            // 
            // toolStripButtonGo
            // 
            this.toolStripButtonGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonGo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGo.Image")));
            this.toolStripButtonGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGo.Name = "toolStripButtonGo";
            this.toolStripButtonGo.Size = new System.Drawing.Size(26, 22);
            this.toolStripButtonGo.Text = "Go";
            this.toolStripButtonGo.Click += new System.EventHandler(this.toolStripButtonGo_Click);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(50, 22);
            this.toolStripButtonRefresh.Text = "Refresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripOpenDropdown
            // 
            this.toolStripOpenDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripOpenDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.femaleCardFolderToolStripMenuItem,
            this.maleCardFolderToolStripMenuItem,
            this.toolStripSeparator2});
            this.toolStripOpenDropdown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripOpenDropdown.Image")));
            this.toolStripOpenDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenDropdown.Name = "toolStripOpenDropdown";
            this.toolStripOpenDropdown.Size = new System.Drawing.Size(61, 22);
            this.toolStripOpenDropdown.Text = "Open...";
            this.toolStripOpenDropdown.ButtonClick += new System.EventHandler(this.ShowOpenFolderDialog);
            // 
            // femaleCardFolderToolStripMenuItem
            // 
            this.femaleCardFolderToolStripMenuItem.Name = "femaleCardFolderToolStripMenuItem";
            this.femaleCardFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.femaleCardFolderToolStripMenuItem.Text = "Female card folder";
            this.femaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.femaleCardFolderToolStripMenuItem_Click);
            // 
            // maleCardFolderToolStripMenuItem
            // 
            this.maleCardFolderToolStripMenuItem.Name = "maleCardFolderToolStripMenuItem";
            this.maleCardFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.maleCardFolderToolStripMenuItem.Text = "Male card folder";
            this.maleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.maleCardFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripViewSelect
            // 
            this.toolStripViewSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripViewSelect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem});
            this.toolStripViewSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripViewSelect.Image")));
            this.toolStripViewSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripViewSelect.Name = "toolStripViewSelect";
            this.toolStripViewSelect.Size = new System.Drawing.Size(45, 22);
            this.toolStripViewSelect.Text = "View";
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.Details);
            // 
            // smallIconsToolStripMenuItem
            // 
            this.smallIconsToolStripMenuItem.Enabled = false;
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.smallIconsToolStripMenuItem.Text = "Small icons";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.SmallIcons);
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.largeIconsToolStripMenuItem.Text = "Large icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.LargeIcons);
            // 
            // toolStripDropDownButtonTools
            // 
            this.toolStripDropDownButtonTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.segregateBySexToolStripMenuItem,
            this.renameCardsToolStripMenuItem});
            this.toolStripDropDownButtonTools.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonTools.Image")));
            this.toolStripDropDownButtonTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonTools.Name = "toolStripDropDownButtonTools";
            this.toolStripDropDownButtonTools.Size = new System.Drawing.Size(48, 22);
            this.toolStripDropDownButtonTools.Text = "Tools";
            this.toolStripDropDownButtonTools.ToolTipText = "Tools";
            // 
            // segregateBySexToolStripMenuItem
            // 
            this.segregateBySexToolStripMenuItem.Name = "segregateBySexToolStripMenuItem";
            this.segregateBySexToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.segregateBySexToolStripMenuItem.Text = "Segregate selected by sex";
            this.segregateBySexToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonSegregate_Click);
            // 
            // renameCardsToolStripMenuItem
            // 
            this.renameCardsToolStripMenuItem.Enabled = false;
            this.renameCardsToolStripMenuItem.Name = "renameCardsToolStripMenuItem";
            this.renameCardsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.renameCardsToolStripMenuItem.Text = "Rename selected card files...";
            this.renameCardsToolStripMenuItem.Click += new System.EventHandler(this.renameCardsToolStripMenuItem_Click);
            // 
            // CardWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 513);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip);
            this.Name = "CardWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.Text = "Cards";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CardWindow_FormClosed);
            this.Load += new System.EventHandler(this.formMain_Load);
            this.ctxCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ContextMenuStrip ctxCards;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeGenderToToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem maleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fermaleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private BrightIdeasSoftware.FastObjectListView listView;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnModDate;
        private BrightIdeasSoftware.OLVColumn olvColumnFilename;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripViewSelect;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox addressBar;
        private System.Windows.Forms.ToolStripSplitButton toolStripOpenDropdown;
        private System.Windows.Forms.ToolStripMenuItem femaleCardFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maleCardFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonGo;
        private ToolStripButton toolStripButtonRefresh;
        private ToolStripSeparator toolStripSeparator3;
        private BrightIdeasSoftware.OLVColumn olvColumnPersonality;
        private BrightIdeasSoftware.OLVColumn olvColumnSex;
        private BrightIdeasSoftware.OLVColumn olvColumnExtended;
        private ToolStripDropDownButton toolStripDropDownButtonTools;
        private ToolStripMenuItem segregateBySexToolStripMenuItem;
        private ToolStripMenuItem renameCardsToolStripMenuItem;
    }
}

