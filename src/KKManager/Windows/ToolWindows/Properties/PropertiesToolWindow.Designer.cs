namespace KKManager.Windows.ToolWindows.Properties
{
    partial class PropertiesToolWindow
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
            this.propViewerContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // propViewerContainer
            // 
            this.propViewerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propViewerContainer.Location = new System.Drawing.Point(0, 0);
            this.propViewerContainer.Name = "propViewerContainer";
            this.propViewerContainer.Size = new System.Drawing.Size(341, 321);
            this.propViewerContainer.TabIndex = 0;
            // 
            // PropertiesToolWindow
            // 
            this.ClientSize = new System.Drawing.Size(341, 321);
            this.Controls.Add(this.propViewerContainer);
            this.Name = "PropertiesToolWindow";
            this.Text = "Properties";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel propViewerContainer;
    }
}