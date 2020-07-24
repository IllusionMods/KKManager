using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CG.Web.MegaApiClient;

namespace KKManager.Updater.Windows
{
    public partial class MegaLoginWindow : Form
    {
        private MegaApiClient _client;
        private MegaApiClient.AuthInfos _authInfos;
        private MegaApiClient.LogonSessionToken _token;

        public MegaLoginWindow()
        {
            InitializeComponent();
        }

        public static Tuple<MegaApiClient.AuthInfos, MegaApiClient.LogonSessionToken> ShowDialog(string currentLogin, MegaApiClient client)
        {
            using (var w = new MegaLoginWindow())
            {
                w.textBoxLogin.Text = currentLogin ?? "";
                w._client = client;

                switch (w.ShowDialog(ActiveForm))
                {
                    case DialogResult.OK:
                        return new Tuple<MegaApiClient.AuthInfos, MegaApiClient.LogonSessionToken>(w.checkBoxRemember.Checked ? w._authInfos : null, w._token);
                    case DialogResult.Ignore:
                        return new Tuple<MegaApiClient.AuthInfos, MegaApiClient.LogonSessionToken>(null, null);
                    default:
                        return null;
                }
            }
        }

        private void buttonAcc_Click(object sender, EventArgs e)
        {
            try
            {
                if (_client.IsLoggedIn)
                    _client.Logout();

                _authInfos = _client.GenerateAuthInfos(textBoxLogin.Text, textBoxPassw.Text);
                _token = _client.Login(_authInfos);

                if (!_client.IsLoggedIn) throw new IOException("Client did not log in");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to log in with the specified login and password. Please make sure that they are correct and you are connected to the internet, then try again.\n\nError message: " + ex.Message, 
                    "Log in to mega.nz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void buttonAnon_Click(object sender, EventArgs e)
        {
            try
            {
                if (_client.IsLoggedIn)
                    _client.Logout();

                _authInfos = null;
                _token = null;
                _client.LoginAnonymous();

                if (!_client.IsLoggedIn) throw new IOException("Client did not log in");

                DialogResult = DialogResult.Ignore;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to log in as an anonymous user. Please make sure that you are connected to the internet, then try again.\n\nError message: " + ex.Message, 
                    "Log in to mega.nz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxes_TextChanged(object sender, EventArgs e)
        {
            var isValidEmail = IsValidEmail(textBoxLogin.Text);
            var isValidPass = !string.IsNullOrEmpty(textBoxPassw.Text);
            buttonAcc.Enabled = isValidEmail && isValidPass;
            labelEmail.ForeColor = isValidEmail ? DefaultForeColor : Color.Red;
            labelPass.ForeColor = isValidPass ? DefaultForeColor : Color.Red;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var _ = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
