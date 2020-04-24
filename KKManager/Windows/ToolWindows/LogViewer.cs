using System;
using System.Diagnostics;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.ToolWindows
{
    // Removing and reading the log file doesn't work because the file is locked
    public partial class LogViewer : DockContent
    {
        public LogViewer()
        {
            InitializeComponent();
            Program.Logger.OnLogWrite += Logger_OnLogWrite;
        }

        private void Logger_OnLogWrite(object sender, LogWriter.LogEventArgs e)
        {
            textBox1.SafeInvoke(() =>
            {
                textBox1.AppendText(e.Message);
                textBox1.AppendText("\r\n");
            });
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Log window cleared";
        }

        private void toolStripButtonRead_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Logger.Flush();
                Process.Start(Program.Logger.LogFilePath);
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
    }
}
