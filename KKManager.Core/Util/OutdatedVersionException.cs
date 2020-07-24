using System;
using System.Windows.Forms;

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
            if (MessageBox.Show(
                "There's a newer version of KK Manager available. Mod updates will not work until you update." +
                "\n\nDo you want to open the download page of the latest version of KKManager?",
                "KK Manager is outdated", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                ProcessTools.SafeStartProcess(Constants.LatestReleaseLink);
            }
        }
    }
}