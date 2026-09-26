using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KKManager.Windows.Dialogs
{
    internal sealed class CancellableProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private readonly Label _status = new Label { Dock = DockStyle.Fill, AutoEllipsis = true };
        private readonly ProgressBar _progress = new ProgressBar { Dock = DockStyle.Fill, Style = ProgressBarStyle.Marquee };
        private readonly Button _cancelButton = new Button { Text = "Cancel", AutoSize = true };
        private readonly Func<CancellationToken, IProgress<string>, IProgress<int>, Task> _work;
        public Exception Error { get; private set; }

        private CancellableProgressDialog(string title, Func<CancellationToken, IProgress<string>, IProgress<int>, Task> work)
        {
            Text = title; Size = new Size(540, 150); StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; ControlBox = false; _work = work;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(_status, 0, 0); layout.Controls.Add(_progress, 0, 1); layout.Controls.Add(_cancelButton, 0, 2); _cancelButton.Anchor = AnchorStyles.Right; _cancelButton.Click += (s, e) => { _cancelButton.Enabled = false; _cancel.Cancel(); _status.Text = "Cancelling..."; }; Controls.Add(layout);
        }
        public static void Run(IWin32Window owner, string title, Func<CancellationToken, IProgress<string>, IProgress<int>, Task> work)
        {
            using var dialog = new CancellableProgressDialog(title, work);
            dialog.Shown += async (s, e) =>
            {
                var text = new Progress<string>(x => dialog._status.Text = x);
                var percent = new Progress<int>(x => { dialog._progress.Style = ProgressBarStyle.Continuous; dialog._progress.Value = Math.Max(0, Math.Min(100, x)); });
                try { await dialog._work(dialog._cancel.Token, text, percent); } catch (Exception ex) { dialog.Error = ex; } finally { dialog.Close(); }
            };
            dialog.ShowDialog(owner);
            if (dialog.Error != null && !(dialog.Error is OperationCanceledException)) throw dialog.Error;
        }
    }
}
