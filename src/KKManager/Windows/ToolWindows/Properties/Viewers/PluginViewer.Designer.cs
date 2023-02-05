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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLocation = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConf = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUrl = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonWebsite = new System.Windows.Forms.ToolStripButton();
            this.groupBoxRefs = new System.Windows.Forms.GroupBox();
            this.listViewRefs = new System.Windows.Forms.ListView();
            this.groupBoxDeps = new System.Windows.Forms.GroupBox();
            this.listViewDeps = new System.Windows.Forms.ListView();
            this.describedTaskRenderer1 = new BrightIdeasSoftware.DescribedTaskRenderer();
            this.describedTaskRenderer2 = new BrightIdeasSoftware.DescribedTaskRenderer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBoxRefs.SuspendLayout();
            this.groupBoxDeps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Controls.Add(this.toolStrip1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.propertyGrid1, "propertyGrid1");
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLocation,
            this.toolStripButtonConf,
            this.toolStripButtonUrl});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonLocation
            // 
            this.toolStripButtonLocation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonLocation, "toolStripButtonLocation");
            this.toolStripButtonLocation.Name = "toolStripButtonLocation";
            this.toolStripButtonLocation.Click += new System.EventHandler(this.toolStripButtonLocation_Click);
            // 
            // toolStripButtonConf
            // 
            this.toolStripButtonConf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonConf, "toolStripButtonConf");
            this.toolStripButtonConf.Name = "toolStripButtonConf";
            this.toolStripButtonConf.Click += new System.EventHandler(this.toolStripButtonConf_Click);
            // 
            // toolStripButtonUrl
            // 
            this.toolStripButtonUrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonUrl, "toolStripButtonUrl");
            this.toolStripButtonUrl.Name = "toolStripButtonUrl";
            this.toolStripButtonUrl.Click += new System.EventHandler(this.toolStripButtonUrl_Click);
            // 
            // toolStripButtonWebsite
            // 
            this.toolStripButtonWebsite.Name = "toolStripButtonWebsite";
            resources.ApplyResources(this.toolStripButtonWebsite, "toolStripButtonWebsite");
            // 
            // groupBoxRefs
            // 
            this.groupBoxRefs.Controls.Add(this.listViewRefs);
            resources.ApplyResources(this.groupBoxRefs, "groupBoxRefs");
            this.groupBoxRefs.Name = "groupBoxRefs";
            this.groupBoxRefs.TabStop = false;
            // 
            // listViewRefs
            // 
            resources.ApplyResources(this.listViewRefs, "listViewRefs");
            this.listViewRefs.FullRowSelect = true;
            this.listViewRefs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewRefs.HideSelection = false;
            this.listViewRefs.Name = "listViewRefs";
            this.listViewRefs.ShowGroups = false;
            this.listViewRefs.UseCompatibleStateImageBehavior = false;
            this.listViewRefs.View = System.Windows.Forms.View.List;
            this.listViewRefs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewRefs_MouseClick);
            // 
            // groupBoxDeps
            // 
            this.groupBoxDeps.Controls.Add(this.listViewDeps);
            resources.ApplyResources(this.groupBoxDeps, "groupBoxDeps");
            this.groupBoxDeps.Name = "groupBoxDeps";
            this.groupBoxDeps.TabStop = false;
            // 
            // listViewDeps
            // 
            resources.ApplyResources(this.listViewDeps, "listViewDeps");
            this.listViewDeps.FullRowSelect = true;
            this.listViewDeps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewDeps.HideSelection = false;
            this.listViewDeps.Name = "listViewDeps";
            this.listViewDeps.ShowGroups = false;
            this.listViewDeps.UseCompatibleStateImageBehavior = false;
            this.listViewDeps.View = System.Windows.Forms.View.List;
            this.listViewDeps.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewDeps_MouseClick);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxDeps);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxRefs);
            // 
            // PluginViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Name = "PluginViewer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBoxRefs.ResumeLayout(false);
            this.groupBoxDeps.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
