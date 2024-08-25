using System;
using System.Windows.Forms;
using KKManager.Properties;

namespace KKManager.Updater.Windows
{
    public partial class P2PSettingsDialog : Form
    {
        public P2PSettingsDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            var s = Settings.Default;
            checkBoxEnable.Checked = s.P2P_Enabled;
            numericUpDown1.Value = s.P2P_Port;
            checkBoxForward.Checked = s.P2P_PortForward;
            checkBoxLogVerbose.Checked = s.P2P_VerboseLogging;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var s = Settings.Default;
            s.P2P_Enabled = checkBoxEnable.Checked;
            s.P2P_Port = (int)numericUpDown1.Value;
            s.P2P_PortForward = checkBoxForward.Checked;
            s.P2P_VerboseLogging = checkBoxLogVerbose.Checked;
            s.P2P_SettingsShown = true;
            Close();
        }
    }
}
