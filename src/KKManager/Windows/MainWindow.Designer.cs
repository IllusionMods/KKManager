namespace KKManager.Windows
{
    sealed partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCardBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFemaleCardFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMaleCardFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.otherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sideloaderModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPluginBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTheGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.charactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.kKManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installANewModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSideloaderModpackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.fixesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixFileAndFolderPermissionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeGameInstallDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressGameFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.developersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateContentsOfUpdatexmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressBundlesAndRandomizeCABsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageEnglishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageJapaneseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageSChineseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageTChineseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageRussianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageGermanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguageFrenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readmeAndSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguagesToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.startTheGameToolStripMenuItem,
            this.openToolStripMenuItem,
            this.installANewModToolStripMenuItem,
            this.updateSideloaderModpackToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.languagesToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCardBrowserToolStripMenuItem,
            this.sideloaderModsToolStripMenuItem,
            this.openPluginBrowserToolStripMenuItem,
            this.openPropertiesToolStripMenuItem,
            this.openLogViewerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            // 
            // openCardBrowserToolStripMenuItem
            // 
            resources.ApplyResources(this.openCardBrowserToolStripMenuItem, "openCardBrowserToolStripMenuItem");
            this.openCardBrowserToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFemaleCardFolderToolStripMenuItem,
            this.openMaleCardFolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.otherToolStripMenuItem});
            this.openCardBrowserToolStripMenuItem.Name = "openCardBrowserToolStripMenuItem";
            // 
            // openFemaleCardFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.openFemaleCardFolderToolStripMenuItem, "openFemaleCardFolderToolStripMenuItem");
            this.openFemaleCardFolderToolStripMenuItem.Name = "openFemaleCardFolderToolStripMenuItem";
            this.openFemaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.openFemaleCardFolderToolStripMenuItem_Click);
            // 
            // openMaleCardFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.openMaleCardFolderToolStripMenuItem, "openMaleCardFolderToolStripMenuItem");
            this.openMaleCardFolderToolStripMenuItem.Name = "openMaleCardFolderToolStripMenuItem";
            this.openMaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.openMaleCardFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // otherToolStripMenuItem
            // 
            resources.ApplyResources(this.otherToolStripMenuItem, "otherToolStripMenuItem");
            this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
            this.otherToolStripMenuItem.Click += new System.EventHandler(this.otherToolStripMenuItem_Click);
            // 
            // sideloaderModsToolStripMenuItem
            // 
            resources.ApplyResources(this.sideloaderModsToolStripMenuItem, "sideloaderModsToolStripMenuItem");
            this.sideloaderModsToolStripMenuItem.Name = "sideloaderModsToolStripMenuItem";
            this.sideloaderModsToolStripMenuItem.Click += new System.EventHandler(this.sideloaderModsToolStripMenuItem_Click);
            // 
            // openPluginBrowserToolStripMenuItem
            // 
            resources.ApplyResources(this.openPluginBrowserToolStripMenuItem, "openPluginBrowserToolStripMenuItem");
            this.openPluginBrowserToolStripMenuItem.Name = "openPluginBrowserToolStripMenuItem";
            this.openPluginBrowserToolStripMenuItem.Click += new System.EventHandler(this.openPluginBrowserToolStripMenuItem_Click);
            // 
            // openPropertiesToolStripMenuItem
            // 
            resources.ApplyResources(this.openPropertiesToolStripMenuItem, "openPropertiesToolStripMenuItem");
            this.openPropertiesToolStripMenuItem.Name = "openPropertiesToolStripMenuItem";
            this.openPropertiesToolStripMenuItem.Click += new System.EventHandler(this.openPropertiesToolStripMenuItem_Click);
            // 
            // openLogViewerToolStripMenuItem
            // 
            resources.ApplyResources(this.openLogViewerToolStripMenuItem, "openLogViewerToolStripMenuItem");
            this.openLogViewerToolStripMenuItem.Name = "openLogViewerToolStripMenuItem";
            this.openLogViewerToolStripMenuItem.Click += new System.EventHandler(this.openLogViewerToolStripMenuItem_Click);
            // 
            // startTheGameToolStripMenuItem
            // 
            resources.ApplyResources(this.startTheGameToolStripMenuItem, "startTheGameToolStripMenuItem");
            this.startTheGameToolStripMenuItem.Name = "startTheGameToolStripMenuItem";
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installDirectoryToolStripMenuItem,
            this.screenshotsToolStripMenuItem,
            this.charactersToolStripMenuItem,
            this.scenesToolStripMenuItem,
            this.toolStripSeparator2,
            this.kKManagerToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            // 
            // installDirectoryToolStripMenuItem
            // 
            resources.ApplyResources(this.installDirectoryToolStripMenuItem, "installDirectoryToolStripMenuItem");
            this.installDirectoryToolStripMenuItem.Name = "installDirectoryToolStripMenuItem";
            this.installDirectoryToolStripMenuItem.Click += new System.EventHandler(this.installDirectoryToolStripMenuItem_Click);
            // 
            // screenshotsToolStripMenuItem
            // 
            resources.ApplyResources(this.screenshotsToolStripMenuItem, "screenshotsToolStripMenuItem");
            this.screenshotsToolStripMenuItem.Name = "screenshotsToolStripMenuItem";
            this.screenshotsToolStripMenuItem.Click += new System.EventHandler(this.screenshotsToolStripMenuItem_Click);
            // 
            // charactersToolStripMenuItem
            // 
            resources.ApplyResources(this.charactersToolStripMenuItem, "charactersToolStripMenuItem");
            this.charactersToolStripMenuItem.Name = "charactersToolStripMenuItem";
            this.charactersToolStripMenuItem.Click += new System.EventHandler(this.charactersToolStripMenuItem_Click);
            // 
            // scenesToolStripMenuItem
            // 
            resources.ApplyResources(this.scenesToolStripMenuItem, "scenesToolStripMenuItem");
            this.scenesToolStripMenuItem.Name = "scenesToolStripMenuItem";
            this.scenesToolStripMenuItem.Click += new System.EventHandler(this.scenesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // kKManagerToolStripMenuItem
            // 
            resources.ApplyResources(this.kKManagerToolStripMenuItem, "kKManagerToolStripMenuItem");
            this.kKManagerToolStripMenuItem.Name = "kKManagerToolStripMenuItem";
            this.kKManagerToolStripMenuItem.Click += new System.EventHandler(this.kKManagerToolStripMenuItem_Click);
            // 
            // installANewModToolStripMenuItem
            // 
            resources.ApplyResources(this.installANewModToolStripMenuItem, "installANewModToolStripMenuItem");
            this.installANewModToolStripMenuItem.Name = "installANewModToolStripMenuItem";
            this.installANewModToolStripMenuItem.Click += new System.EventHandler(this.installANewModToolStripMenuItem_Click);
            // 
            // updateSideloaderModpackToolStripMenuItem
            // 
            resources.ApplyResources(this.updateSideloaderModpackToolStripMenuItem, "updateSideloaderModpackToolStripMenuItem");
            this.updateSideloaderModpackToolStripMenuItem.Name = "updateSideloaderModpackToolStripMenuItem";
            this.updateSideloaderModpackToolStripMenuItem.Click += new System.EventHandler(this.updateSideloaderModpackToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesOnStartupToolStripMenuItem,
            this.toolStripSeparator4,
            this.fixesToolStripMenuItem,
            this.changeGameInstallDirectoryToolStripMenuItem,
            this.compressGameFilesToolStripMenuItem,
            this.toolStripSeparator3,
            this.developersToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            // 
            // checkForUpdatesOnStartupToolStripMenuItem
            // 
            resources.ApplyResources(this.checkForUpdatesOnStartupToolStripMenuItem, "checkForUpdatesOnStartupToolStripMenuItem");
            this.checkForUpdatesOnStartupToolStripMenuItem.Name = "checkForUpdatesOnStartupToolStripMenuItem";
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // fixesToolStripMenuItem
            // 
            resources.ApplyResources(this.fixesToolStripMenuItem, "fixesToolStripMenuItem");
            this.fixesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fixFileAndFolderPermissionsToolStripMenuItem});
            this.fixesToolStripMenuItem.Name = "fixesToolStripMenuItem";
            // 
            // fixFileAndFolderPermissionsToolStripMenuItem
            // 
            resources.ApplyResources(this.fixFileAndFolderPermissionsToolStripMenuItem, "fixFileAndFolderPermissionsToolStripMenuItem");
            this.fixFileAndFolderPermissionsToolStripMenuItem.Name = "fixFileAndFolderPermissionsToolStripMenuItem";
            this.fixFileAndFolderPermissionsToolStripMenuItem.Click += new System.EventHandler(this.fixFileAndFolderPermissionsToolStripMenuItem_Click);
            // 
            // changeGameInstallDirectoryToolStripMenuItem
            // 
            resources.ApplyResources(this.changeGameInstallDirectoryToolStripMenuItem, "changeGameInstallDirectoryToolStripMenuItem");
            this.changeGameInstallDirectoryToolStripMenuItem.Name = "changeGameInstallDirectoryToolStripMenuItem";
            this.changeGameInstallDirectoryToolStripMenuItem.Click += new System.EventHandler(this.changeGameInstallDirectoryToolStripMenuItem_Click);
            // 
            // compressGameFilesToolStripMenuItem
            // 
            resources.ApplyResources(this.compressGameFilesToolStripMenuItem, "compressGameFilesToolStripMenuItem");
            this.compressGameFilesToolStripMenuItem.Name = "compressGameFilesToolStripMenuItem";
            this.compressGameFilesToolStripMenuItem.Click += new System.EventHandler(this.compressGameFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // developersToolStripMenuItem
            // 
            resources.ApplyResources(this.developersToolStripMenuItem, "developersToolStripMenuItem");
            this.developersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateContentsOfUpdatexmlToolStripMenuItem,
            this.compressBundlesAndRandomizeCABsToolStripMenuItem});
            this.developersToolStripMenuItem.Name = "developersToolStripMenuItem";
            // 
            // generateContentsOfUpdatexmlToolStripMenuItem
            // 
            resources.ApplyResources(this.generateContentsOfUpdatexmlToolStripMenuItem, "generateContentsOfUpdatexmlToolStripMenuItem");
            this.generateContentsOfUpdatexmlToolStripMenuItem.Name = "generateContentsOfUpdatexmlToolStripMenuItem";
            this.generateContentsOfUpdatexmlToolStripMenuItem.Click += new System.EventHandler(this.generateContentsOfUpdatexmlToolStripMenuItem_Click);
            // 
            // compressBundlesAndRandomizeCABsToolStripMenuItem
            // 
            resources.ApplyResources(this.compressBundlesAndRandomizeCABsToolStripMenuItem, "compressBundlesAndRandomizeCABsToolStripMenuItem");
            this.compressBundlesAndRandomizeCABsToolStripMenuItem.Name = "compressBundlesAndRandomizeCABsToolStripMenuItem";
            this.compressBundlesAndRandomizeCABsToolStripMenuItem.Click += new System.EventHandler(this.compressBundlesAndRandomizeCABsToolStripMenuItem_Click);
            // 
            // languagesToolStripMenuItem
            // 
            resources.ApplyResources(this.languagesToolStripMenuItem, "languagesToolStripMenuItem");
            this.languagesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LanguageEnglishToolStripMenuItem,
            this.LanguageJapaneseToolStripMenuItem,
            this.LanguageSChineseToolStripMenuItem,
            this.LanguageTChineseToolStripMenuItem,
            this.LanguageRussianToolStripMenuItem,
            this.LanguageGermanToolStripMenuItem,
            this.LanguageFrenchToolStripMenuItem});
            this.languagesToolStripMenuItem.Name = "languagesToolStripMenuItem";
            // 
            // LanguageEnglishToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageEnglishToolStripMenuItem, "LanguageEnglishToolStripMenuItem");
            this.LanguageEnglishToolStripMenuItem.Name = "LanguageEnglishToolStripMenuItem";
            this.LanguageEnglishToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // LanguageJapaneseToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageJapaneseToolStripMenuItem, "LanguageJapaneseToolStripMenuItem");
            this.LanguageJapaneseToolStripMenuItem.Name = "LanguageJapaneseToolStripMenuItem";
            this.LanguageJapaneseToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // LanguageSChineseToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageSChineseToolStripMenuItem, "LanguageSChineseToolStripMenuItem");
            this.LanguageSChineseToolStripMenuItem.Name = "LanguageSChineseToolStripMenuItem";
            this.LanguageSChineseToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // LanguageTChineseToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageTChineseToolStripMenuItem, "LanguageTChineseToolStripMenuItem");
            this.LanguageTChineseToolStripMenuItem.Name = "LanguageTChineseToolStripMenuItem";
            this.LanguageTChineseToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // LanguageRussianToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageRussianToolStripMenuItem, "LanguageRussianToolStripMenuItem");
            this.LanguageRussianToolStripMenuItem.Name = "LanguageRussianToolStripMenuItem";
            this.LanguageRussianToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // LanguageGermanToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageGermanToolStripMenuItem, "LanguageGermanToolStripMenuItem");
            this.LanguageGermanToolStripMenuItem.Name = "LanguageGermanToolStripMenuItem";
            this.LanguageGermanToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // LanguageFrenchToolStripMenuItem
            // 
            resources.ApplyResources(this.LanguageFrenchToolStripMenuItem, "LanguageFrenchToolStripMenuItem");
            this.LanguageFrenchToolStripMenuItem.Name = "LanguageFrenchToolStripMenuItem";
            this.LanguageFrenchToolStripMenuItem.Click += new System.EventHandler(this.LanguageToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readmeAndSourceCodeToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            // 
            // readmeAndSourceCodeToolStripMenuItem
            // 
            resources.ApplyResources(this.readmeAndSourceCodeToolStripMenuItem, "readmeAndSourceCodeToolStripMenuItem");
            this.readmeAndSourceCodeToolStripMenuItem.Name = "readmeAndSourceCodeToolStripMenuItem";
            this.readmeAndSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.readmeAndSourceCodeToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // LanguagesToolStripComboBox
            // 
            resources.ApplyResources(this.LanguagesToolStripComboBox, "LanguagesToolStripComboBox");
            this.LanguagesToolStripComboBox.Name = "LanguagesToolStripComboBox";
            // 
            // dockPanel
            // 
            resources.ApplyResources(this.dockPanel, "dockPanel");
            this.dockPanel.Name = "dockPanel";
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelStatus});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            resources.ApplyResources(this.toolStripStatusLabelStatus, "toolStripStatusLabelStatus");
            this.toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sideloaderModsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCardBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFemaleCardFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMaleCardFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem otherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPluginBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelStatus;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readmeAndSourceCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTheGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installANewModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenshotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem charactersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scenesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateSideloaderModpackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kKManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixFileAndFolderPermissionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem changeGameInstallDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem developersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateContentsOfUpdatexmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressGameFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressBundlesAndRandomizeCABsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnStartupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LanguageEnglishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LanguageSChineseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LanguageTChineseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LanguageJapaneseToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox LanguagesToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem LanguageRussianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LanguageGermanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LanguageFrenchToolStripMenuItem;
    }
}