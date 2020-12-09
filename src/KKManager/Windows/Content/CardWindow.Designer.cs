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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardWindow));
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
            this.toolStripTextBoxSearch = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            resources.ApplyResources(this.listView, "listView");
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
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Name = "listView";
            this.listView.OverlayText.Text = resources.GetString("resource.Text");
            this.listView.ShowGroups = false;
            this.listView.ShowItemToolTips = true;
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
            resources.ApplyResources(this.olvColumnName, "olvColumnName");
            this.olvColumnName.MinimumWidth = 150;
            this.olvColumnName.UseFiltering = false;
            // 
            // olvColumnSex
            // 
            resources.ApplyResources(this.olvColumnSex, "olvColumnSex");
            this.olvColumnSex.MinimumWidth = 35;
            // 
            // olvColumnPersonality
            // 
            resources.ApplyResources(this.olvColumnPersonality, "olvColumnPersonality");
            this.olvColumnPersonality.MinimumWidth = 70;
            // 
            // olvColumnExtended
            // 
            resources.ApplyResources(this.olvColumnExtended, "olvColumnExtended");
            this.olvColumnExtended.MinimumWidth = 40;
            this.olvColumnExtended.Searchable = false;
            // 
            // olvColumnModDate
            // 
            resources.ApplyResources(this.olvColumnModDate, "olvColumnModDate");
            this.olvColumnModDate.MinimumWidth = 60;
            this.olvColumnModDate.UseFiltering = false;
            // 
            // olvColumnFilename
            // 
            resources.ApplyResources(this.olvColumnFilename, "olvColumnFilename");
            this.olvColumnFilename.MinimumWidth = 60;
            this.olvColumnFilename.UseFiltering = false;
            // 
            // toolStrip
            // 
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addressBar,
            this.toolStripButtonGo,
            this.toolStripButtonRefresh,
            this.toolStripSeparator3,
            this.toolStripOpenDropdown,
            this.toolStripSeparator1,
            this.toolStripViewSelect,
            this.toolStripDropDownButtonTools,
            this.toolStripTextBoxSearch});
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Resize += new System.EventHandler(this.OnResizeToolstip);
            // 
            // addressBar
            // 
            resources.ApplyResources(this.addressBar, "addressBar");
            this.addressBar.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.addressBar.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.addressBar.Name = "addressBar";
            this.addressBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addressBar_KeyDown);
            // 
            // toolStripButtonGo
            // 
            resources.ApplyResources(this.toolStripButtonGo, "toolStripButtonGo");
            this.toolStripButtonGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonGo.Name = "toolStripButtonGo";
            this.toolStripButtonGo.Click += new System.EventHandler(this.toolStripButtonGo_Click);
            // 
            // toolStripButtonRefresh
            // 
            resources.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripOpenDropdown
            // 
            resources.ApplyResources(this.toolStripOpenDropdown, "toolStripOpenDropdown");
            this.toolStripOpenDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripOpenDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.femaleCardFolderToolStripMenuItem,
            this.maleCardFolderToolStripMenuItem,
            this.toolStripSeparator2});
            this.toolStripOpenDropdown.Name = "toolStripOpenDropdown";
            this.toolStripOpenDropdown.ButtonClick += new System.EventHandler(this.ShowOpenFolderDialog);
            // 
            // femaleCardFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.femaleCardFolderToolStripMenuItem, "femaleCardFolderToolStripMenuItem");
            this.femaleCardFolderToolStripMenuItem.Name = "femaleCardFolderToolStripMenuItem";
            this.femaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.femaleCardFolderToolStripMenuItem_Click);
            // 
            // maleCardFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.maleCardFolderToolStripMenuItem, "maleCardFolderToolStripMenuItem");
            this.maleCardFolderToolStripMenuItem.Name = "maleCardFolderToolStripMenuItem";
            this.maleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.maleCardFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripViewSelect
            // 
            resources.ApplyResources(this.toolStripViewSelect, "toolStripViewSelect");
            this.toolStripViewSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripViewSelect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem});
            this.toolStripViewSelect.Name = "toolStripViewSelect";
            // 
            // detailsToolStripMenuItem
            // 
            resources.ApplyResources(this.detailsToolStripMenuItem, "detailsToolStripMenuItem");
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.Details);
            // 
            // smallIconsToolStripMenuItem
            // 
            resources.ApplyResources(this.smallIconsToolStripMenuItem, "smallIconsToolStripMenuItem");
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.SmallIcons);
            // 
            // largeIconsToolStripMenuItem
            // 
            resources.ApplyResources(this.largeIconsToolStripMenuItem, "largeIconsToolStripMenuItem");
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.LargeIcons);
            // 
            // toolStripDropDownButtonTools
            // 
            resources.ApplyResources(this.toolStripDropDownButtonTools, "toolStripDropDownButtonTools");
            this.toolStripDropDownButtonTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.segregateBySexToolStripMenuItem,
            this.renameCardsToolStripMenuItem});
            this.toolStripDropDownButtonTools.Name = "toolStripDropDownButtonTools";
            // 
            // segregateBySexToolStripMenuItem
            // 
            resources.ApplyResources(this.segregateBySexToolStripMenuItem, "segregateBySexToolStripMenuItem");
            this.segregateBySexToolStripMenuItem.Name = "segregateBySexToolStripMenuItem";
            this.segregateBySexToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonSegregate_Click);
            // 
            // renameCardsToolStripMenuItem
            // 
            resources.ApplyResources(this.renameCardsToolStripMenuItem, "renameCardsToolStripMenuItem");
            this.renameCardsToolStripMenuItem.Name = "renameCardsToolStripMenuItem";
            this.renameCardsToolStripMenuItem.Click += new System.EventHandler(this.renameCardsToolStripMenuItem_Click);
            // 
            // toolStripTextBoxSearch
            // 
            resources.ApplyResources(this.toolStripTextBoxSearch, "toolStripTextBoxSearch");
            this.toolStripTextBoxSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripTextBoxSearch.Name = "toolStripTextBoxSearch";
            this.toolStripTextBoxSearch.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            // 
            // CardWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip);
            this.Name = "CardWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CardWindow_FormClosed);
            this.Load += new System.EventHandler(this.formMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
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
        private ToolStripTextBox toolStripTextBoxSearch;
    }
}

