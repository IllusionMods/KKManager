using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Util;

namespace KKManager.Windows.Dialogs
{
    internal sealed class MissingZipmodInstallDialog : Form
    {
        private readonly CheckedListBox _matches = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true };
        private readonly TextBox _missing = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, WordWrap = false };

        private MissingZipmodInstallDialog(IReadOnlyList<ZipmodCatalogEntry> matches, IReadOnlyList<string> missing)
        {
            Text = "Install missing zipmods"; StartPosition = FormStartPosition.CenterParent; Size = new Size(800, 580); MinimumSize = new Size(680, 480);
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 5 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 55)); layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(new Label { AutoSize = true, Text = $"Exact GUID matches in the BetterRepack catalog ({matches.Count}):" }, 0, 0);
            foreach (var entry in matches.OrderBy(x => x.FileName, StringComparer.CurrentCultureIgnoreCase)) _matches.Items.Add(new MatchItem(entry), true);
            _matches.DisplayMember = nameof(MatchItem.Display); layout.Controls.Add(_matches, 0, 1);
            layout.Controls.Add(new Label { AutoSize = true, Text = $"Not found in the local catalog ({missing.Count}):" }, 0, 2);
            _missing.Text = string.Join(Environment.NewLine, missing); layout.Controls.Add(_missing, 0, 3);
            var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
            var install = new Button { Text = "Install selected", DialogResult = DialogResult.OK, AutoSize = true, Enabled = matches.Count > 0 };
            buttons.Controls.Add(cancel); buttons.Controls.Add(install); layout.Controls.Add(buttons, 0, 4); AcceptButton = install; CancelButton = cancel; Controls.Add(layout);
        }

        public static ZipmodCatalogEntry[] ShowDialog(IWin32Window owner, IReadOnlyList<ZipmodCatalogEntry> matches, IReadOnlyList<string> missing)
        {
            using var dialog = new MissingZipmodInstallDialog(matches, missing);
            return dialog.ShowDialog(owner) == DialogResult.OK ? dialog._matches.CheckedItems.Cast<MatchItem>().Select(x => x.Entry).ToArray() : null;
        }
        private sealed class MatchItem { public MatchItem(ZipmodCatalogEntry entry) => Entry = entry; public ZipmodCatalogEntry Entry { get; } public string Display => $"{Entry.FileName}  [{Entry.Guid}]  ({FileSize.FromBytes(Entry.Size)})"; }
    }
}
