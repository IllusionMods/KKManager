namespace KKManager.Updater.Windows
{
    partial class LogPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogPopup));
            this.logControl1 = new KKManager.Controls.LogControl();
            this.SuspendLayout();
            // 
            // logControl1
            // 
            resources.ApplyResources(this.logControl1, "logControl1");
            this.logControl1.Name = "logControl1";
            // 
            // LogPopup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logControl1);
            this.Name = "LogPopup";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.LogControl logControl1;
    }
}