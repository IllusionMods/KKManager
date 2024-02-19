using System.Windows.Forms;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGameLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.changeGameInstallDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
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
            this.lookForModUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openIndividualDownloadWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useSystemProxyServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.p2PDownloaderSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixFileAndFolderPermissionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressGameFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpDuplicateZipmodsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.developersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModpackToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateContentsOfUpdatexmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressBundlesAndRandomizeCABsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tryToDeleteToRecycleBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.readmeAndSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.websiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LanguagesToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.startTheGameToolStripMenuItem,
            this.openToolStripMenuItem,
            this.installANewModToolStripMenuItem,
            this.updateSideloaderModpackToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ShowItemToolTips = true;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGameLogToolStripMenuItem,
            this.toolStripSeparator7,
            this.changeGameInstallDirectoryToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openGameLogToolStripMenuItem
            // 
            this.openGameLogToolStripMenuItem.Name = "openGameLogToolStripMenuItem";
            resources.ApplyResources(this.openGameLogToolStripMenuItem, "openGameLogToolStripMenuItem");
            this.openGameLogToolStripMenuItem.Click += new System.EventHandler(this.openGameLogToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // changeGameInstallDirectoryToolStripMenuItem
            // 
            this.changeGameInstallDirectoryToolStripMenuItem.Name = "changeGameInstallDirectoryToolStripMenuItem";
            resources.ApplyResources(this.changeGameInstallDirectoryToolStripMenuItem, "changeGameInstallDirectoryToolStripMenuItem");
            this.changeGameInstallDirectoryToolStripMenuItem.Click += new System.EventHandler(this.changeGameInstallDirectoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCardBrowserToolStripMenuItem,
            this.sideloaderModsToolStripMenuItem,
            this.openPluginBrowserToolStripMenuItem,
            this.openPropertiesToolStripMenuItem,
            this.openLogViewerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // openCardBrowserToolStripMenuItem
            // 
            this.openCardBrowserToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFemaleCardFolderToolStripMenuItem,
            this.openMaleCardFolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.otherToolStripMenuItem});
            this.openCardBrowserToolStripMenuItem.Name = "openCardBrowserToolStripMenuItem";
            resources.ApplyResources(this.openCardBrowserToolStripMenuItem, "openCardBrowserToolStripMenuItem");
            // 
            // openFemaleCardFolderToolStripMenuItem
            // 
            this.openFemaleCardFolderToolStripMenuItem.Name = "openFemaleCardFolderToolStripMenuItem";
            resources.ApplyResources(this.openFemaleCardFolderToolStripMenuItem, "openFemaleCardFolderToolStripMenuItem");
            this.openFemaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.openFemaleCardFolderToolStripMenuItem_Click);
            // 
            // openMaleCardFolderToolStripMenuItem
            // 
            this.openMaleCardFolderToolStripMenuItem.Name = "openMaleCardFolderToolStripMenuItem";
            resources.ApplyResources(this.openMaleCardFolderToolStripMenuItem, "openMaleCardFolderToolStripMenuItem");
            this.openMaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.openMaleCardFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // otherToolStripMenuItem
            // 
            this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
            resources.ApplyResources(this.otherToolStripMenuItem, "otherToolStripMenuItem");
            this.otherToolStripMenuItem.Click += new System.EventHandler(this.otherToolStripMenuItem_Click);
            // 
            // sideloaderModsToolStripMenuItem
            // 
            this.sideloaderModsToolStripMenuItem.Name = "sideloaderModsToolStripMenuItem";
            resources.ApplyResources(this.sideloaderModsToolStripMenuItem, "sideloaderModsToolStripMenuItem");
            this.sideloaderModsToolStripMenuItem.Click += new System.EventHandler(this.sideloaderModsToolStripMenuItem_Click);
            // 
            // openPluginBrowserToolStripMenuItem
            // 
            this.openPluginBrowserToolStripMenuItem.Name = "openPluginBrowserToolStripMenuItem";
            resources.ApplyResources(this.openPluginBrowserToolStripMenuItem, "openPluginBrowserToolStripMenuItem");
            this.openPluginBrowserToolStripMenuItem.Click += new System.EventHandler(this.openPluginBrowserToolStripMenuItem_Click);
            // 
            // openPropertiesToolStripMenuItem
            // 
            this.openPropertiesToolStripMenuItem.Name = "openPropertiesToolStripMenuItem";
            resources.ApplyResources(this.openPropertiesToolStripMenuItem, "openPropertiesToolStripMenuItem");
            this.openPropertiesToolStripMenuItem.Click += new System.EventHandler(this.openPropertiesToolStripMenuItem_Click);
            // 
            // openLogViewerToolStripMenuItem
            // 
            this.openLogViewerToolStripMenuItem.Name = "openLogViewerToolStripMenuItem";
            resources.ApplyResources(this.openLogViewerToolStripMenuItem, "openLogViewerToolStripMenuItem");
            this.openLogViewerToolStripMenuItem.Click += new System.EventHandler(this.openLogViewerToolStripMenuItem_Click);
            // 
            // startTheGameToolStripMenuItem
            // 
            this.startTheGameToolStripMenuItem.Name = "startTheGameToolStripMenuItem";
            resources.ApplyResources(this.startTheGameToolStripMenuItem, "startTheGameToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installDirectoryToolStripMenuItem,
            this.screenshotsToolStripMenuItem,
            this.charactersToolStripMenuItem,
            this.scenesToolStripMenuItem,
            this.toolStripSeparator2,
            this.kKManagerToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            // 
            // installDirectoryToolStripMenuItem
            // 
            this.installDirectoryToolStripMenuItem.Name = "installDirectoryToolStripMenuItem";
            resources.ApplyResources(this.installDirectoryToolStripMenuItem, "installDirectoryToolStripMenuItem");
            this.installDirectoryToolStripMenuItem.Click += new System.EventHandler(this.installDirectoryToolStripMenuItem_Click);
            // 
            // screenshotsToolStripMenuItem
            // 
            this.screenshotsToolStripMenuItem.Name = "screenshotsToolStripMenuItem";
            resources.ApplyResources(this.screenshotsToolStripMenuItem, "screenshotsToolStripMenuItem");
            this.screenshotsToolStripMenuItem.Click += new System.EventHandler(this.screenshotsToolStripMenuItem_Click);
            // 
            // charactersToolStripMenuItem
            // 
            this.charactersToolStripMenuItem.Name = "charactersToolStripMenuItem";
            resources.ApplyResources(this.charactersToolStripMenuItem, "charactersToolStripMenuItem");
            this.charactersToolStripMenuItem.Click += new System.EventHandler(this.charactersToolStripMenuItem_Click);
            // 
            // scenesToolStripMenuItem
            // 
            this.scenesToolStripMenuItem.Name = "scenesToolStripMenuItem";
            resources.ApplyResources(this.scenesToolStripMenuItem, "scenesToolStripMenuItem");
            this.scenesToolStripMenuItem.Click += new System.EventHandler(this.scenesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // kKManagerToolStripMenuItem
            // 
            this.kKManagerToolStripMenuItem.Name = "kKManagerToolStripMenuItem";
            resources.ApplyResources(this.kKManagerToolStripMenuItem, "kKManagerToolStripMenuItem");
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
            this.updateSideloaderModpackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lookForModUpdatesToolStripMenuItem,
            this.openIndividualDownloadWebsiteToolStripMenuItem,
            this.toolStripSeparator6,
            this.checkForUpdatesOnStartupToolStripMenuItem,
            this.useSystemProxyServerToolStripMenuItem,
            this.p2PDownloaderSettingsToolStripMenuItem});
            this.updateSideloaderModpackToolStripMenuItem.Name = "updateSideloaderModpackToolStripMenuItem";
            resources.ApplyResources(this.updateSideloaderModpackToolStripMenuItem, "updateSideloaderModpackToolStripMenuItem");
            // 
            // lookForModUpdatesToolStripMenuItem
            // 
            this.lookForModUpdatesToolStripMenuItem.Name = "lookForModUpdatesToolStripMenuItem";
            resources.ApplyResources(this.lookForModUpdatesToolStripMenuItem, "lookForModUpdatesToolStripMenuItem");
            this.lookForModUpdatesToolStripMenuItem.Click += new System.EventHandler(this.updateSideloaderModpackToolStripMenuItem_Click);
            // 
            // openIndividualDownloadWebsiteToolStripMenuItem
            // 
            this.openIndividualDownloadWebsiteToolStripMenuItem.Name = "openIndividualDownloadWebsiteToolStripMenuItem";
            resources.ApplyResources(this.openIndividualDownloadWebsiteToolStripMenuItem, "openIndividualDownloadWebsiteToolStripMenuItem");
            this.openIndividualDownloadWebsiteToolStripMenuItem.Click += new System.EventHandler(this.openIndividualDownloadWebsiteToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // checkForUpdatesOnStartupToolStripMenuItem
            // 
            this.checkForUpdatesOnStartupToolStripMenuItem.Name = "checkForUpdatesOnStartupToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesOnStartupToolStripMenuItem, "checkForUpdatesOnStartupToolStripMenuItem");
            // 
            // useSystemProxyServerToolStripMenuItem
            // 
            this.useSystemProxyServerToolStripMenuItem.Name = "useSystemProxyServerToolStripMenuItem";
            resources.ApplyResources(this.useSystemProxyServerToolStripMenuItem, "useSystemProxyServerToolStripMenuItem");
            // 
            // p2PDownloaderSettingsToolStripMenuItem
            // 
            this.p2PDownloaderSettingsToolStripMenuItem.Name = "p2PDownloaderSettingsToolStripMenuItem";
            resources.ApplyResources(this.p2PDownloaderSettingsToolStripMenuItem, "p2PDownloaderSettingsToolStripMenuItem");
            this.p2PDownloaderSettingsToolStripMenuItem.Click += new System.EventHandler(this.p2PDownloaderSettingsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fixFileAndFolderPermissionsToolStripMenuItem,
            this.compressGameFilesToolStripMenuItem,
            this.cleanUpDuplicateZipmodsToolStripMenuItem,
            this.toolStripSeparator3,
            this.developersToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // fixFileAndFolderPermissionsToolStripMenuItem
            // 
            this.fixFileAndFolderPermissionsToolStripMenuItem.Name = "fixFileAndFolderPermissionsToolStripMenuItem";
            resources.ApplyResources(this.fixFileAndFolderPermissionsToolStripMenuItem, "fixFileAndFolderPermissionsToolStripMenuItem");
            this.fixFileAndFolderPermissionsToolStripMenuItem.Click += new System.EventHandler(this.fixFileAndFolderPermissionsToolStripMenuItem_Click);
            // 
            // compressGameFilesToolStripMenuItem
            // 
            this.compressGameFilesToolStripMenuItem.Name = "compressGameFilesToolStripMenuItem";
            resources.ApplyResources(this.compressGameFilesToolStripMenuItem, "compressGameFilesToolStripMenuItem");
            this.compressGameFilesToolStripMenuItem.Click += new System.EventHandler(this.compressGameFilesToolStripMenuItem_Click);
            // 
            // cleanUpDuplicateZipmodsToolStripMenuItem
            // 
            this.cleanUpDuplicateZipmodsToolStripMenuItem.Name = "cleanUpDuplicateZipmodsToolStripMenuItem";
            resources.ApplyResources(this.cleanUpDuplicateZipmodsToolStripMenuItem, "cleanUpDuplicateZipmodsToolStripMenuItem");
            this.cleanUpDuplicateZipmodsToolStripMenuItem.Click += new System.EventHandler(this.cleanUpDuplicateZipmodsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // developersToolStripMenuItem
            // 
            this.developersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModpackToolToolStripMenuItem,
            this.generateContentsOfUpdatexmlToolStripMenuItem,
            this.compressBundlesAndRandomizeCABsToolStripMenuItem,
            this.cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem});
            this.developersToolStripMenuItem.Name = "developersToolStripMenuItem";
            resources.ApplyResources(this.developersToolStripMenuItem, "developersToolStripMenuItem");
            // 
            // openModpackToolToolStripMenuItem
            // 
            this.openModpackToolToolStripMenuItem.Name = "openModpackToolToolStripMenuItem";
            resources.ApplyResources(this.openModpackToolToolStripMenuItem, "openModpackToolToolStripMenuItem");
            this.openModpackToolToolStripMenuItem.Click += new System.EventHandler(this.openModpackToolToolStripMenuItem_Click);
            // 
            // generateContentsOfUpdatexmlToolStripMenuItem
            // 
            this.generateContentsOfUpdatexmlToolStripMenuItem.Name = "generateContentsOfUpdatexmlToolStripMenuItem";
            resources.ApplyResources(this.generateContentsOfUpdatexmlToolStripMenuItem, "generateContentsOfUpdatexmlToolStripMenuItem");
            this.generateContentsOfUpdatexmlToolStripMenuItem.Click += new System.EventHandler(this.generateContentsOfUpdatexmlToolStripMenuItem_Click);
            // 
            // compressBundlesAndRandomizeCABsToolStripMenuItem
            // 
            this.compressBundlesAndRandomizeCABsToolStripMenuItem.Name = "compressBundlesAndRandomizeCABsToolStripMenuItem";
            resources.ApplyResources(this.compressBundlesAndRandomizeCABsToolStripMenuItem, "compressBundlesAndRandomizeCABsToolStripMenuItem");
            this.compressBundlesAndRandomizeCABsToolStripMenuItem.Click += new System.EventHandler(this.compressBundlesAndRandomizeCABsToolStripMenuItem_Click);
            // 
            // cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem
            // 
            this.cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem.Name = "cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem";
            resources.ApplyResources(this.cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem, "cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem");
            this.cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem.Click += new System.EventHandler(this.cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languagesToolStripMenuItem,
            this.tryToDeleteToRecycleBinToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.settingsToolStripMenuItem_DropDownOpening);
            // 
            // languagesToolStripMenuItem
            // 
            this.languagesToolStripMenuItem.Name = "languagesToolStripMenuItem";
            resources.ApplyResources(this.languagesToolStripMenuItem, "languagesToolStripMenuItem");
            // 
            // tryToDeleteToRecycleBinToolStripMenuItem
            // 
            this.tryToDeleteToRecycleBinToolStripMenuItem.Name = "tryToDeleteToRecycleBinToolStripMenuItem";
            resources.ApplyResources(this.tryToDeleteToRecycleBinToolStripMenuItem, "tryToDeleteToRecycleBinToolStripMenuItem");
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripSeparator4,
            this.readmeAndSourceCodeToolStripMenuItem,
            this.licenseToolStripMenuItem,
            this.websiteToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // readmeAndSourceCodeToolStripMenuItem
            // 
            this.readmeAndSourceCodeToolStripMenuItem.Name = "readmeAndSourceCodeToolStripMenuItem";
            resources.ApplyResources(this.readmeAndSourceCodeToolStripMenuItem, "readmeAndSourceCodeToolStripMenuItem");
            this.readmeAndSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.readmeAndSourceCodeToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            resources.ApplyResources(this.licenseToolStripMenuItem, "licenseToolStripMenuItem");
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // websiteToolStripMenuItem
            // 
            this.websiteToolStripMenuItem.Name = "websiteToolStripMenuItem";
            resources.ApplyResources(this.websiteToolStripMenuItem, "websiteToolStripMenuItem");
            this.websiteToolStripMenuItem.Click += new System.EventHandler(this.websiteToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // LanguagesToolStripComboBox
            // 
            this.LanguagesToolStripComboBox.Name = "LanguagesToolStripComboBox";
            resources.ApplyResources(this.LanguagesToolStripComboBox, "LanguagesToolStripComboBox");
            // 
            // dockPanel
            // 
            resources.ApplyResources(this.dockPanel, "dockPanel");
            this.dockPanel.Name = "dockPanel";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelStatus});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            this.toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            resources.ApplyResources(this.toolStripStatusLabelStatus, "toolStripStatusLabelStatus");
            this.toolStripStatusLabelStatus.Click += new System.EventHandler(this.toolStripStatusLabelStatus_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 200;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ShowAlways = true;
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
        private System.Windows.Forms.ToolStripMenuItem fixFileAndFolderPermissionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem developersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateContentsOfUpdatexmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressGameFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressBundlesAndRandomizeCABsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox LanguagesToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem licenseToolStripMenuItem;
        private ToolStripMenuItem websiteToolStripMenuItem;
        private ToolStripMenuItem cleanUpDuplicateAndInvalidZipmodsToolStripMenuItem;
        private ToolStripMenuItem cleanUpDuplicateZipmodsToolStripMenuItem;
        private ToolStripMenuItem openModpackToolToolStripMenuItem;
        private ToolStripMenuItem lookForModUpdatesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem checkForUpdatesOnStartupToolStripMenuItem;
        private ToolStripMenuItem useSystemProxyServerToolStripMenuItem;
        private ToolStripMenuItem p2PDownloaderSettingsToolStripMenuItem;
        private ToolStripMenuItem changeGameInstallDirectoryToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem openIndividualDownloadWebsiteToolStripMenuItem;
        private ToolStripMenuItem openGameLogToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem tryToDeleteToRecycleBinToolStripMenuItem;
        private ToolTip toolTip1;
    }
}