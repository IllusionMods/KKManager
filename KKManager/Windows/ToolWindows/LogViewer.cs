using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Program.Logger.OnLogWrite += Logger_OnLogWrite;
        }

        private void Logger_OnLogWrite(object sender, Util.LogWriter.LogEventArgs e)
        {
            textBox1.AppendText(e.Message);
            textBox1.AppendText("\r\n");
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Log window cleared";
        }

        private void toolStripButtonRead_Click(object sender, EventArgs e)
        {
            try { Process.Start(Program.Logger.LogFilePath); }
            catch (Exception ex) { Console.WriteLine(ex); }
            //try
            //{
            //    Program.Logger.Flush();
            //    textBox1.Text = File.ReadAllText(Program.Logger.LogFilePath);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Failed to read log file - " + ex.Message);
            //}
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    Program.Logger.Flush();
            //    File.Delete(Program.Logger.LogFilePath);
            //    textBox1.Text = Program.Logger.LogFilePath + " has been deleted";
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Failed to delete log file - " + ex.Message);
            //}
        }
    }
}
