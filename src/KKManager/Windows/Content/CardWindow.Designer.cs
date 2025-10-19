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
            this.olvColumnFileSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnRelativeFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnCardType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnUserID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDataID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnMissingMods = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnVersion = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripOpenDropdown = new System.Windows.Forms.ToolStripSplitButton();
            this.femaleCardFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maleCardFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addressBar = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonGo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSubdirs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripViewSelect = new System.Windows.Forms.ToolStripDropDownButton();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.segregateBySexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAListOfMissingModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cardMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usedZipmodsAndPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zipmodUsageincludingUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginUsageincludingUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxSearch = new System.Windows.Forms.ToolStripTextBox();
            this.showUnknowninvalidCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.AllColumns.Add(this.olvColumnName);
            this.listView.AllColumns.Add(this.olvColumnSex);
            this.listView.AllColumns.Add(this.olvColumnPersonality);
            this.listView.AllColumns.Add(this.olvColumnExtended);
            this.listView.AllColumns.Add(this.olvColumnModDate);
            this.listView.AllColumns.Add(this.olvColumnFileSize);
            this.listView.AllColumns.Add(this.olvColumnRelativeFilename);
            this.listView.AllColumns.Add(this.olvColumnFilename);
            this.listView.AllColumns.Add(this.olvColumnCardType);
            this.listView.AllColumns.Add(this.olvColumnUserID);
            this.listView.AllColumns.Add(this.olvColumnDataID);
            this.listView.AllColumns.Add(this.olvColumnMissingMods);
            this.listView.AllColumns.Add(this.olvColumnVersion);
            this.listView.AllowColumnReorder = true;
            this.listView.CellEditUseWholeCell = false;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnSex,
            this.olvColumnPersonality,
            this.olvColumnExtended,
            this.olvColumnModDate,
            this.olvColumnFileSize,
            this.olvColumnRelativeFilename,
            this.olvColumnCardType,
            this.olvColumnUserID});
            resources.ApplyResources(this.listView, "listView");
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Name = "listView";
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
            this.olvColumnName.AspectName = "Name";
            this.olvColumnName.MinimumWidth = 150;
            resources.ApplyResources(this.olvColumnName, "olvColumnName");
            this.olvColumnName.UseFiltering = false;
            // 
            // olvColumnSex
            // 
            this.olvColumnSex.AspectName = "Sex";
            this.olvColumnSex.MinimumWidth = 35;
            resources.ApplyResources(this.olvColumnSex, "olvColumnSex");
            // 
            // olvColumnPersonality
            // 
            this.olvColumnPersonality.AspectName = "PersonalityName";
            this.olvColumnPersonality.MinimumWidth = 70;
            resources.ApplyResources(this.olvColumnPersonality, "olvColumnPersonality");
            // 
            // olvColumnExtended
            // 
            this.olvColumnExtended.AspectName = "Extended.Count";
            this.olvColumnExtended.MinimumWidth = 40;
            this.olvColumnExtended.Searchable = false;
            resources.ApplyResources(this.olvColumnExtended, "olvColumnExtended");
            // 
            // olvColumnModDate
            // 
            this.olvColumnModDate.AspectName = "Location.LastWriteTime";
            this.olvColumnModDate.MinimumWidth = 60;
            resources.ApplyResources(this.olvColumnModDate, "olvColumnModDate");
            this.olvColumnModDate.UseFiltering = false;
            // 
            // olvColumnFileSize
            // 
            this.olvColumnFileSize.AspectName = "FileSize";
            resources.ApplyResources(this.olvColumnFileSize, "olvColumnFileSize");
            // 
            // olvColumnRelativeFilename
            // 
            this.olvColumnRelativeFilename.MinimumWidth = 60;
            resources.ApplyResources(this.olvColumnRelativeFilename, "olvColumnRelativeFilename");
            // 
            // olvColumnFilename
            // 
            this.olvColumnFilename.AspectName = "Location.Name";
            resources.ApplyResources(this.olvColumnFilename, "olvColumnFilename");
            this.olvColumnFilename.IsVisible = false;
            this.olvColumnFilename.MinimumWidth = 60;
            this.olvColumnFilename.UseFiltering = false;
            // 
            // olvColumnCardType
            // 
            this.olvColumnCardType.AspectName = "Type";
            resources.ApplyResources(this.olvColumnCardType, "olvColumnCardType");
            // 
            // olvColumnUserID
            // 
            this.olvColumnUserID.AspectName = "UserID";
            resources.ApplyResources(this.olvColumnUserID, "olvColumnUserID");
            // 
            // olvColumnDataID
            // 
            this.olvColumnDataID.AspectName = "DataID";
            resources.ApplyResources(this.olvColumnDataID, "olvColumnDataID");
            this.olvColumnDataID.IsVisible = false;
            // 
            // olvColumnMissingMods
            // 
            resources.ApplyResources(this.olvColumnMissingMods, "olvColumnMissingMods");
            this.olvColumnMissingMods.IsVisible = false;
            // 
            // olvColumnVersion
            // 
            this.olvColumnVersion.AspectName = "Version";
            resources.ApplyResources(this.olvColumnVersion, "olvColumnVersion");
            this.olvColumnVersion.IsVisible = false;
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpenDropdown,
            this.addressBar,
            this.toolStripButtonGo,
            this.toolStripButtonRefresh,
            this.toolStripButtonSubdirs,
            this.toolStripSeparator3,
            this.toolStripButtonDelete,
            this.toolStripSeparator4,
            this.toolStripViewSelect,
            this.toolStripDropDownButtonTools,
            this.toolStripTextBoxSearch});
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Resize += new System.EventHandler(this.OnResizeToolstip);
            // 
            // toolStripOpenDropdown
            // 
            this.toolStripOpenDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripOpenDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.femaleCardFolderToolStripMenuItem,
            this.maleCardFolderToolStripMenuItem,
            this.toolStripSeparator2});
            resources.ApplyResources(this.toolStripOpenDropdown, "toolStripOpenDropdown");
            this.toolStripOpenDropdown.Name = "toolStripOpenDropdown";
            this.toolStripOpenDropdown.ButtonClick += new System.EventHandler(this.ShowOpenFolderDialog);
            // 
            // femaleCardFolderToolStripMenuItem
            // 
            this.femaleCardFolderToolStripMenuItem.Name = "femaleCardFolderToolStripMenuItem";
            resources.ApplyResources(this.femaleCardFolderToolStripMenuItem, "femaleCardFolderToolStripMenuItem");
            this.femaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.femaleCardFolderToolStripMenuItem_Click);
            // 
            // maleCardFolderToolStripMenuItem
            // 
            this.maleCardFolderToolStripMenuItem.Name = "maleCardFolderToolStripMenuItem";
            resources.ApplyResources(this.maleCardFolderToolStripMenuItem, "maleCardFolderToolStripMenuItem");
            this.maleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.maleCardFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // addressBar
            // 
            this.addressBar.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.addressBar.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            resources.ApplyResources(this.addressBar, "addressBar");
            this.addressBar.Name = "addressBar";
            this.addressBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addressBar_KeyDown);
            // 
            // toolStripButtonGo
            // 
            this.toolStripButtonGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonGo, "toolStripButtonGo");
            this.toolStripButtonGo.Name = "toolStripButtonGo";
            this.toolStripButtonGo.Click += new System.EventHandler(this.toolStripButtonGo_Click);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripButtonSubdirs
            // 
            this.toolStripButtonSubdirs.CheckOnClick = true;
            this.toolStripButtonSubdirs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonSubdirs, "toolStripButtonSubdirs");
            this.toolStripButtonSubdirs.Name = "toolStripButtonSubdirs";
            this.toolStripButtonSubdirs.CheckedChanged += new System.EventHandler(this.toolStripButtonSubdirs_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonDelete, "toolStripButtonDelete");
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripViewSelect
            // 
            this.toolStripViewSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripViewSelect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem,
            this.toolStripSeparator1,
            this.showUnknowninvalidCardsToolStripMenuItem,
            this.toolStripSeparator5,
            this.openInExplorerToolStripMenuItem});
            resources.ApplyResources(this.toolStripViewSelect, "toolStripViewSelect");
            this.toolStripViewSelect.Name = "toolStripViewSelect";
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            resources.ApplyResources(this.detailsToolStripMenuItem, "detailsToolStripMenuItem");
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
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            resources.ApplyResources(this.largeIconsToolStripMenuItem, "largeIconsToolStripMenuItem");
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.LargeIcons);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // openInExplorerToolStripMenuItem
            // 
            this.openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            resources.ApplyResources(this.openInExplorerToolStripMenuItem, "openInExplorerToolStripMenuItem");
            this.openInExplorerToolStripMenuItem.Click += new System.EventHandler(this.openInExplorerToolStripMenuItem_Click);
            // 
            // toolStripDropDownButtonTools
            // 
            this.toolStripDropDownButtonTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.segregateBySexToolStripMenuItem,
            this.renameCardsToolStripMenuItem,
            this.exportAListOfMissingModsToolStripMenuItem,
            this.exportToCsvToolStripMenuItem});
            resources.ApplyResources(this.toolStripDropDownButtonTools, "toolStripDropDownButtonTools");
            this.toolStripDropDownButtonTools.Name = "toolStripDropDownButtonTools";
            // 
            // segregateBySexToolStripMenuItem
            // 
            this.segregateBySexToolStripMenuItem.Name = "segregateBySexToolStripMenuItem";
            resources.ApplyResources(this.segregateBySexToolStripMenuItem, "segregateBySexToolStripMenuItem");
            this.segregateBySexToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonSegregate_Click);
            // 
            // renameCardsToolStripMenuItem
            // 
            resources.ApplyResources(this.renameCardsToolStripMenuItem, "renameCardsToolStripMenuItem");
            this.renameCardsToolStripMenuItem.Name = "renameCardsToolStripMenuItem";
            this.renameCardsToolStripMenuItem.Click += new System.EventHandler(this.renameCardsToolStripMenuItem_Click);
            // 
            // exportAListOfMissingModsToolStripMenuItem
            // 
            this.exportAListOfMissingModsToolStripMenuItem.Name = "exportAListOfMissingModsToolStripMenuItem";
            resources.ApplyResources(this.exportAListOfMissingModsToolStripMenuItem, "exportAListOfMissingModsToolStripMenuItem");
            this.exportAListOfMissingModsToolStripMenuItem.Click += new System.EventHandler(this.exportAListOfMissingModsToolStripMenuItem_Click);
            // 
            // exportToCsvToolStripMenuItem
            // 
            this.exportToCsvToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cardMetadataToolStripMenuItem,
            this.usedZipmodsAndPluginsToolStripMenuItem,
            this.zipmodUsageincludingUnusedToolStripMenuItem,
            this.pluginUsageincludingUnusedToolStripMenuItem});
            this.exportToCsvToolStripMenuItem.Name = "exportToCsvToolStripMenuItem";
            resources.ApplyResources(this.exportToCsvToolStripMenuItem, "exportToCsvToolStripMenuItem");
            // 
            // cardMetadataToolStripMenuItem
            // 
            this.cardMetadataToolStripMenuItem.Name = "cardMetadataToolStripMenuItem";
            resources.ApplyResources(this.cardMetadataToolStripMenuItem, "cardMetadataToolStripMenuItem");
            this.cardMetadataToolStripMenuItem.Click += new System.EventHandler(this.cardMetadataToolStripMenuItem_Click);
            // 
            // usedZipmodsAndPluginsToolStripMenuItem
            // 
            this.usedZipmodsAndPluginsToolStripMenuItem.Name = "usedZipmodsAndPluginsToolStripMenuItem";
            resources.ApplyResources(this.usedZipmodsAndPluginsToolStripMenuItem, "usedZipmodsAndPluginsToolStripMenuItem");
            this.usedZipmodsAndPluginsToolStripMenuItem.Click += new System.EventHandler(this.usedZipmodsAndPluginsToolStripMenuItem_Click);
            // 
            // zipmodUsageincludingUnusedToolStripMenuItem
            // 
            this.zipmodUsageincludingUnusedToolStripMenuItem.Name = "zipmodUsageincludingUnusedToolStripMenuItem";
            resources.ApplyResources(this.zipmodUsageincludingUnusedToolStripMenuItem, "zipmodUsageincludingUnusedToolStripMenuItem");
            this.zipmodUsageincludingUnusedToolStripMenuItem.Click += new System.EventHandler(this.zipmodUsageincludingUnusedToolStripMenuItem_Click);
            // 
            // pluginUsageincludingUnusedToolStripMenuItem
            // 
            this.pluginUsageincludingUnusedToolStripMenuItem.Name = "pluginUsageincludingUnusedToolStripMenuItem";
            resources.ApplyResources(this.pluginUsageincludingUnusedToolStripMenuItem, "pluginUsageincludingUnusedToolStripMenuItem");
            this.pluginUsageincludingUnusedToolStripMenuItem.Click += new System.EventHandler(this.pluginUsageincludingUnusedToolStripMenuItem_Click);
            // 
            // toolStripTextBoxSearch
            // 
            this.toolStripTextBoxSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripTextBoxSearch, "toolStripTextBoxSearch");
            this.toolStripTextBoxSearch.Name = "toolStripTextBoxSearch";
            this.toolStripTextBoxSearch.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            // 
            // showUnknowninvalidCardsToolStripMenuItem
            // 
            this.showUnknowninvalidCardsToolStripMenuItem.Checked = true;
            this.showUnknowninvalidCardsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showUnknowninvalidCardsToolStripMenuItem.Name = "showUnknowninvalidCardsToolStripMenuItem";
            resources.ApplyResources(this.showUnknowninvalidCardsToolStripMenuItem, "showUnknowninvalidCardsToolStripMenuItem");
            this.showUnknowninvalidCardsToolStripMenuItem.Click += new System.EventHandler(this.showUnknowninvalidCardsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
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
        private ToolStripButton toolStripButtonDelete;
        private ToolStripSeparator toolStripSeparator4;
        private BrightIdeasSoftware.OLVColumn olvColumnUserID;
        private BrightIdeasSoftware.OLVColumn olvColumnFileSize;
        private BrightIdeasSoftware.OLVColumn olvColumnCardType;
        private ToolStripMenuItem exportAListOfMissingModsToolStripMenuItem;
        private ToolStripButton toolStripButtonSubdirs;
        private BrightIdeasSoftware.OLVColumn olvColumnRelativeFilename;
        private BrightIdeasSoftware.OLVColumn olvColumnDataID;
        private BrightIdeasSoftware.OLVColumn olvColumnMissingMods;
        private BrightIdeasSoftware.OLVColumn olvColumnVersion;
        private ToolStripMenuItem exportToCsvToolStripMenuItem;
        private ToolStripMenuItem usedZipmodsAndPluginsToolStripMenuItem;
        private ToolStripMenuItem zipmodUsageincludingUnusedToolStripMenuItem;
        private ToolStripMenuItem pluginUsageincludingUnusedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem openInExplorerToolStripMenuItem;
        private ToolStripMenuItem cardMetadataToolStripMenuItem;
        private ToolStripMenuItem showUnknowninvalidCardsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
    }
}

