using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using KKManager.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.ToolWindows
{
    public partial class LogViewer : DockContent
    {
        public LogViewer()
        {
            InitializeComponent();

            // Read log written up to this point
            var initialLog = Program.Logger.GetLog();
            textBox1.AppendText(initialLog);

            // Subscribe to future log events
            var listener = new LogViewerWriter(textBox1);
            Program.Logger.AddListener(listener);
            Disposed += (sender, args) =>
            {
                Program.Logger.RemoveListener(listener);
                listener.Dispose();
            };
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Log window cleared\r\n";
        }

        private void toolStripButtonRead_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Logger.Flush();
                Process.Start(Program.Logger.LogFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private class LogViewerWriter : TextWriter
        {
            private readonly TextBox _target;

            public LogViewerWriter(TextBox target)
            {
                _target = target;
            }

            public override Encoding Encoding { get; } = Encoding.Unicode;

            public override void Write(string value)
            {
                _target.BeginInvoke(new Action(() => _target.AppendText(value)));
            }
        }
    }
}