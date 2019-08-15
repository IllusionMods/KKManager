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
            this.startTheGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.charactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installANewModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSideloaderModpackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readmeAndSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.startTheGameToolStripMenuItem,
            this.openToolStripMenuItem,
            this.installANewModToolStripMenuItem,
            this.updateSideloaderModpackToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1012, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCardBrowserToolStripMenuItem,
            this.sideloaderModsToolStripMenuItem,
            this.openPluginBrowserToolStripMenuItem,
            this.openPropertiesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // openCardBrowserToolStripMenuItem
            // 
            this.openCardBrowserToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFemaleCardFolderToolStripMenuItem,
            this.openMaleCardFolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.otherToolStripMenuItem});
            this.openCardBrowserToolStripMenuItem.Name = "openCardBrowserToolStripMenuItem";
            this.openCardBrowserToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openCardBrowserToolStripMenuItem.Text = "Open card browser";
            // 
            // openFemaleCardFolderToolStripMenuItem
            // 
            this.openFemaleCardFolderToolStripMenuItem.Name = "openFemaleCardFolderToolStripMenuItem";
            this.openFemaleCardFolderToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.openFemaleCardFolderToolStripMenuItem.Text = "Open female card folder";
            this.openFemaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.openFemaleCardFolderToolStripMenuItem_Click);
            // 
            // openMaleCardFolderToolStripMenuItem
            // 
            this.openMaleCardFolderToolStripMenuItem.Name = "openMaleCardFolderToolStripMenuItem";
            this.openMaleCardFolderToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.openMaleCardFolderToolStripMenuItem.Text = "Open male card folder";
            this.openMaleCardFolderToolStripMenuItem.Click += new System.EventHandler(this.openMaleCardFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // otherToolStripMenuItem
            // 
            this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
            this.otherToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.otherToolStripMenuItem.Text = "Open other folder...";
            this.otherToolStripMenuItem.Click += new System.EventHandler(this.otherToolStripMenuItem_Click);
            // 
            // sideloaderModsToolStripMenuItem
            // 
            this.sideloaderModsToolStripMenuItem.Name = "sideloaderModsToolStripMenuItem";
            this.sideloaderModsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.sideloaderModsToolStripMenuItem.Text = "Open sideloader mod browser";
            this.sideloaderModsToolStripMenuItem.Click += new System.EventHandler(this.sideloaderModsToolStripMenuItem_Click);
            // 
            // openPluginBrowserToolStripMenuItem
            // 
            this.openPluginBrowserToolStripMenuItem.Name = "openPluginBrowserToolStripMenuItem";
            this.openPluginBrowserToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openPluginBrowserToolStripMenuItem.Text = "Open plugin browser";
            this.openPluginBrowserToolStripMenuItem.Click += new System.EventHandler(this.openPluginBrowserToolStripMenuItem_Click);
            // 
            // openPropertiesToolStripMenuItem
            // 
            this.openPropertiesToolStripMenuItem.Name = "openPropertiesToolStripMenuItem";
            this.openPropertiesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openPropertiesToolStripMenuItem.Text = "Open properties";
            this.openPropertiesToolStripMenuItem.Click += new System.EventHandler(this.openPropertiesToolStripMenuItem_Click);
            // 
            // startTheGameToolStripMenuItem
            // 
            this.startTheGameToolStripMenuItem.Name = "startTheGameToolStripMenuItem";
            this.startTheGameToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.startTheGameToolStripMenuItem.Text = "Start";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installDirectoryToolStripMenuItem,
            this.screenshotsToolStripMenuItem,
            this.charactersToolStripMenuItem,
            this.scenesToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.openToolStripMenuItem.Text = "Open directory";
            // 
            // installDirectoryToolStripMenuItem
            // 
            this.installDirectoryToolStripMenuItem.Name = "installDirectoryToolStripMenuItem";
            this.installDirectoryToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.installDirectoryToolStripMenuItem.Text = "Install directory";
            this.installDirectoryToolStripMenuItem.Click += new System.EventHandler(this.installDirectoryToolStripMenuItem_Click);
            // 
            // screenshotsToolStripMenuItem
            // 
            this.screenshotsToolStripMenuItem.Name = "screenshotsToolStripMenuItem";
            this.screenshotsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.screenshotsToolStripMenuItem.Text = "Screenshots";
            this.screenshotsToolStripMenuItem.Click += new System.EventHandler(this.screenshotsToolStripMenuItem_Click);
            // 
            // charactersToolStripMenuItem
            // 
            this.charactersToolStripMenuItem.Name = "charactersToolStripMenuItem";
            this.charactersToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.charactersToolStripMenuItem.Text = "Characters";
            this.charactersToolStripMenuItem.Click += new System.EventHandler(this.charactersToolStripMenuItem_Click);
            // 
            // scenesToolStripMenuItem
            // 
            this.scenesToolStripMenuItem.Name = "scenesToolStripMenuItem";
            this.scenesToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.scenesToolStripMenuItem.Text = "Scenes";
            this.scenesToolStripMenuItem.Click += new System.EventHandler(this.scenesToolStripMenuItem_Click);
            // 
            // installANewModToolStripMenuItem
            // 
            this.installANewModToolStripMenuItem.Name = "installANewModToolStripMenuItem";
            this.installANewModToolStripMenuItem.Size = new System.Drawing.Size(121, 20);
            this.installANewModToolStripMenuItem.Text = "Install a new mod...";
            this.installANewModToolStripMenuItem.Click += new System.EventHandler(this.installANewModToolStripMenuItem_Click);
            // 
            // updateSideloaderModpackToolStripMenuItem
            // 
            this.updateSideloaderModpackToolStripMenuItem.Name = "updateSideloaderModpackToolStripMenuItem";
            this.updateSideloaderModpackToolStripMenuItem.Size = new System.Drawing.Size(176, 20);
            this.updateSideloaderModpackToolStripMenuItem.Text = "Update sideloader modpack...";
            this.updateSideloaderModpackToolStripMenuItem.Click += new System.EventHandler(this.updateSideloaderModpackToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readmeAndSourceCodeToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // readmeAndSourceCodeToolStripMenuItem
            // 
            this.readmeAndSourceCodeToolStripMenuItem.Name = "readmeAndSourceCodeToolStripMenuItem";
            this.readmeAndSourceCodeToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.readmeAndSourceCodeToolStripMenuItem.Text = "Readme and source code";
            this.readmeAndSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.readmeAndSourceCodeToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 24);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1012, 633);
            this.dockPanel.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 657);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1012, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            this.toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            this.toolStripStatusLabelStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 679);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "KK Manager - Chikarin technician edition";
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
	}
}