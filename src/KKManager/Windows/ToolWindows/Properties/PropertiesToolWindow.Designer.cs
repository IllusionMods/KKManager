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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesToolWindow));
            this.propViewerContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // propViewerContainer
            // 
            resources.ApplyResources(this.propViewerContainer, "propViewerContainer");
            this.propViewerContainer.Name = "propViewerContainer";
            // 
            // PropertiesToolWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.propViewerContainer);
            this.Name = "PropertiesToolWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel propViewerContainer;
    }
}