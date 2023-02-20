using System;
using System.Windows.Forms;
using KKManager.Properties;

namespace KKManager.Util
{
    /// <summary>
    /// Thrown if KK Manager needs to be updated
    /// </summary>
    public sealed class OutdatedVersionException : Exception
    {
        public OutdatedVersionException(string message) : base(message)
        {
        }

        public void ShowKkmanOutdatedMessage()
        {
            if (MessageBox.Show(Resources.KKManagerOutdatedMessageBody, Resources.KKManagerOutdatedMessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                ProcessTools.SafeStartProcess(Constants.LatestReleaseLink);
            }
        }
    }
}