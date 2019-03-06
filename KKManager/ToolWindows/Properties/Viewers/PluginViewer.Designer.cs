namespace KKManager.ToolWindows.Properties.Viewers
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStripButtonWebsite = new System.Windows.Forms.ToolStripButton();
            this.groupBoxRefs = new System.Windows.Forms.GroupBox();
            this.listViewRefs = new System.Windows.Forms.ListView();
            this.groupBoxDeps = new System.Windows.Forms.GroupBox();
            this.listViewDeps = new System.Windows.Forms.ListView();
            this.groupBox1.SuspendLayout();
            this.groupBoxRefs.SuspendLayout();
            this.groupBoxDeps.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 193);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Properties";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 16);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(371, 174);
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
            this.groupBoxRefs.Text = "Plugins that depend on this one";
            // 
            // listViewRefs
            // 
            this.listViewRefs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRefs.FullRowSelect = true;
            this.listViewRefs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
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
            this.groupBoxDeps.Text = "Dependens on these plugins";
            // 
            // listViewDeps
            // 
            this.listViewDeps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDeps.FullRowSelect = true;
            this.listViewDeps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewDeps.Location = new System.Drawing.Point(3, 16);
            this.listViewDeps.Name = "listViewDeps";
            this.listViewDeps.ShowGroups = false;
            this.listViewDeps.Size = new System.Drawing.Size(371, 121);
            this.listViewDeps.TabIndex = 0;
            this.listViewDeps.UseCompatibleStateImageBehavior = false;
            this.listViewDeps.View = System.Windows.Forms.View.List;
            this.listViewDeps.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewDeps_MouseClick);
            // 
            // PluginViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxRefs);
            this.Controls.Add(this.groupBoxDeps);
            this.Controls.Add(this.groupBox1);
            this.Name = "PluginViewer";
            this.Size = new System.Drawing.Size(377, 741);
            this.groupBox1.ResumeLayout(false);
            this.groupBoxRefs.ResumeLayout(false);
            this.groupBoxDeps.ResumeLayout(false);
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
    }
}
