namespace KKManager.Windows.ToolWindows.Properties.Viewers
{
    partial class PluginViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginViewer));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStripButtonWebsite = new System.Windows.Forms.ToolStripButton();
            this.groupBoxRefs = new System.Windows.Forms.GroupBox();
            this.listViewRefs = new System.Windows.Forms.ListView();
            this.groupBoxDeps = new System.Windows.Forms.GroupBox();
            this.listViewDeps = new System.Windows.Forms.ListView();
            this.describedTaskRenderer1 = new BrightIdeasSoftware.DescribedTaskRenderer();
            this.describedTaskRenderer2 = new BrightIdeasSoftware.DescribedTaskRenderer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonConf = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUrl = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLocation = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBoxRefs.SuspendLayout();
            this.groupBoxDeps.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 193);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 41);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid1.Size = new System.Drawing.Size(371, 149);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // toolStripButtonWebsite
            // 
            this.toolStripButtonWebsite.Name = "toolStripButtonWebsite";
            this.toolStripButtonWebsite.Size = new System.Drawing.Size(23, 23);
            // 
            // groupBoxRefs
            // 
            this.groupBoxRefs.Controls.Add(this.listViewRefs);
            this.groupBoxRefs.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxRefs.Location = new System.Drawing.Point(0, 333);
            this.groupBoxRefs.Name = "groupBoxRefs";
            this.groupBoxRefs.Size = new System.Drawing.Size(377, 140);
            this.groupBoxRefs.TabIndex = 5;
            this.groupBoxRefs.TabStop = false;
            this.groupBoxRefs.Text = "These plugins require this plugin:";
            // 
            // listViewRefs
            // 
            this.listViewRefs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRefs.FullRowSelect = true;
            this.listViewRefs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewRefs.HideSelection = false;
            this.listViewRefs.Location = new System.Drawing.Point(3, 16);
            this.listViewRefs.Name = "listViewRefs";
            this.listViewRefs.ShowGroups = false;
            this.listViewRefs.Size = new System.Drawing.Size(371, 121);
            this.listViewRefs.TabIndex = 0;
            this.listViewRefs.UseCompatibleStateImageBehavior = false;
            this.listViewRefs.View = System.Windows.Forms.View.List;
            this.listViewRefs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewRefs_MouseClick);
            // 
            // groupBoxDeps
            // 
            this.groupBoxDeps.Controls.Add(this.listViewDeps);
            this.groupBoxDeps.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxDeps.Location = new System.Drawing.Point(0, 193);
            this.groupBoxDeps.Name = "groupBoxDeps";
            this.groupBoxDeps.Size = new System.Drawing.Size(377, 140);
            this.groupBoxDeps.TabIndex = 6;
            this.groupBoxDeps.TabStop = false;
            this.groupBoxDeps.Text = "This plugin requires the following plugins:";
            // 
            // listViewDeps
            // 
            this.listViewDeps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDeps.FullRowSelect = true;
            this.listViewDeps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewDeps.HideSelection = false;
            this.listViewDeps.Location = new System.Drawing.Point(3, 16);
            this.listViewDeps.Name = "listViewDeps";
            this.listViewDeps.ShowGroups = false;
            this.listViewDeps.Size = new System.Drawing.Size(371, 121);
            this.listViewDeps.TabIndex = 0;
            this.listViewDeps.UseCompatibleStateImageBehavior = false;
            this.listViewDeps.View = System.Windows.Forms.View.List;
            this.listViewDeps.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewDeps_MouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLocation,
            this.toolStripButtonConf,
            this.toolStripButtonUrl});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(371, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonConf
            // 
            this.toolStripButtonConf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonConf.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConf.Image")));
            this.toolStripButtonConf.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConf.Name = "toolStripButtonConf";
            this.toolStripButtonConf.Size = new System.Drawing.Size(96, 22);
            this.toolStripButtonConf.Text = "Open config file";
            this.toolStripButtonConf.Click += new System.EventHandler(this.toolStripButtonConf_Click);
            // 
            // toolStripButtonUrl
            // 
            this.toolStripButtonUrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonUrl.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUrl.Image")));
            this.toolStripButtonUrl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUrl.Name = "toolStripButtonUrl";
            this.toolStripButtonUrl.Size = new System.Drawing.Size(83, 22);
            this.toolStripButtonUrl.Text = "Open website";
            this.toolStripButtonUrl.Click += new System.EventHandler(this.toolStripButtonUrl_Click);
            // 
            // toolStripButtonLocation
            // 
            this.toolStripButtonLocation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonLocation.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLocation.Image")));
            this.toolStripButtonLocation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLocation.Name = "toolStripButtonLocation";
            this.toolStripButtonLocation.Size = new System.Drawing.Size(120, 22);
            this.toolStripButtonLocation.Text = "Open install location";
            this.toolStripButtonLocation.Click += new System.EventHandler(this.toolStripButtonLocation_Click);
            // 
            // PluginViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.groupBoxRefs);
            this.Controls.Add(this.groupBoxDeps);
            this.Controls.Add(this.groupBox1);
            this.Name = "PluginViewer";
            this.Size = new System.Drawing.Size(377, 741);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxRefs.ResumeLayout(false);
            this.groupBoxDeps.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripButton toolStripButtonWebsite;
        private System.Windows.Forms.GroupBox groupBoxRefs;
        private System.Windows.Forms.ListView listViewRefs;
        private System.Windows.Forms.GroupBox groupBoxDeps;
        private System.Windows.Forms.ListView listViewDeps;
        private BrightIdeasSoftware.DescribedTaskRenderer describedTaskRenderer1;
        private BrightIdeasSoftware.DescribedTaskRenderer describedTaskRenderer2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonConf;
        private System.Windows.Forms.ToolStripButton toolStripButtonUrl;
        private System.Windows.Forms.ToolStripButton toolStripButtonLocation;
    }
}
