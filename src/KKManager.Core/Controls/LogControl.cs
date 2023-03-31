using KKManager.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace KKManager.Controls
{
    public partial class LogControl : UserControl
    {
        public static LogWriter Log { get; set; }

        public LogControl()
        {
            InitializeComponent();

            if (Log != null)
            {
                // Read log written up to this point
                var initialLog = Log.GetLog();
                textBox1.AppendText(initialLog);

                // Subscribe to future log events
                var listener = new LogViewerWriter(textBox1);
                Log.AddListener(listener);
                Disposed += (sender, args) =>
                {
                    Log.RemoveListener(listener);
                    listener.Dispose();
                };
            }
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Log window cleared\r\n";
        }

        private void toolStripButtonRead_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Flush();
                Process.Start(Log.LogFilePath);
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
