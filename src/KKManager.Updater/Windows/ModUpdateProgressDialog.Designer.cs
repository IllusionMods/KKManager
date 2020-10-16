namespace KKManager.Updater.Windows
{
    partial class ModUpdateProgressDialog
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelPercent = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBoxSleep = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(9, 9);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(370, 35);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Value = 33;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.checkBoxSleep);
            this.panel1.Controls.Add(this.labelPercent);
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 210);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(474, 120);
            this.panel1.TabIndex = 1;
            // 
            // labelPercent
            // 
            this.labelPercent.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPercent.Location = new System.Drawing.Point(0, 64);
            this.labelPercent.MaximumSize = new System.Drawing.Size(1337, 56);
            this.labelPercent.MinimumSize = new System.Drawing.Size(0, 56);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Padding = new System.Windows.Forms.Padding(8, 2, 8, 6);
            this.labelPercent.Size = new System.Drawing.Size(474, 56);
            this.labelPercent.TabIndex = 2;
            this.labelPercent.Text = "This item: 70% done (1MB / 20 MB)\r\nOverall: 50% done (111MB / 1221 MB)\r\nSpeed: 12" +
    "34KB/s (average 1111KB/s)";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(0, 40);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Padding = new System.Windows.Forms.Padding(8, 2, 8, 6);
            this.labelStatus.Size = new System.Drawing.Size(131, 24);
            this.labelStatus.TabIndex = 1;
            this.labelStatus.Text = "Downloading XYZ";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(4, 6, 4, 0);
            this.label1.Size = new System.Drawing.Size(474, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hold tight while the selected updates are being downloaded and installed by our c" +
    "ertified technician, Chikarin! Please avoid accessing the game files until after" +
    " the update is finished.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 330);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(9);
            this.panel2.Size = new System.Drawing.Size(474, 53);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(379, 9);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(11, 35);
            this.panel3.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(390, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::KKManager.Updater.Properties.Resources.chikajump;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(474, 210);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // checkBoxSleep
            // 
            this.checkBoxSleep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSleep.AutoSize = true;
            this.checkBoxSleep.Location = new System.Drawing.Point(362, 97);
            this.checkBoxSleep.Name = "checkBoxSleep";
            this.checkBoxSleep.Size = new System.Drawing.Size(109, 17);
            this.checkBoxSleep.TabIndex = 3;
            this.checkBoxSleep.Text = "Sleep when done";
            this.checkBoxSleep.UseVisualStyleBackColor = true;
            // 
            // ModUpdateProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(474, 383);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(490, 99999);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(490, 200);
            this.Name = "ModUpdateProgressDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mods are being updated";
            this.Shown += new System.EventHandler(this.ModUpdateProgress_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBoxSleep;
    }
}