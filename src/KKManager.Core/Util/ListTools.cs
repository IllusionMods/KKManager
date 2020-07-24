using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace KKManager.Util
{
    public static class ListTools
    {
        public static object GetFirstCharacter(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? '\0' : char.ToUpperInvariant(name.Trim().FirstOrDefault());
        }

        public static object GetGuidGroupKey(string guid)
        {
            guid = guid ?? string.Empty;
            var i = guid.LastIndexOf(".", StringComparison.Ordinal);
            return i < 0 ? guid : guid.Substring(0, i).Trim().ToLowerInvariant();
        }

        public static void FastAutoResizeColumns(this ObjectListView olv)
        {
            try
            {
                olv.BeginUpdate();
                olv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                try
                {
                    olv.EndUpdate();
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void SetUpSearchBox(ObjectListView listView, ToolStripTextBox searchBox)
        {
            const string searchText = "Search...";
            void PutSearchText()
            {
                searchBox.Text = searchText;
                searchBox.ForeColor = Color.Gray;
            }
            PutSearchText();

            bool SearchStringIsEmpty()
            {
                return string.IsNullOrWhiteSpace(searchBox.Text) || searchBox.Text == searchText;
            }
            searchBox.GotFocus += (sender, args) =>
            {
                if (SearchStringIsEmpty())
                    searchBox.Text = string.Empty;
                searchBox.ForeColor = Control.DefaultForeColor;
            };
            searchBox.LostFocus += (sender, args) =>
            {
                if (SearchStringIsEmpty())
                    PutSearchText();
            };

            var textMatchFilter = new TextMatchFilter(listView);
            textMatchFilter.StringComparison = StringComparison.OrdinalIgnoreCase;
            listView.AdditionalFilter = textMatchFilter;
            listView.UseFiltering = true;
            searchBox.TextChanged += (sender, args) =>
            {
                if (SearchStringIsEmpty())
                    textMatchFilter.ContainsStrings = null;
                else
                    textMatchFilter.ContainsStrings = new[] { searchBox.Text.Trim() };
                listView.UpdateColumnFiltering();
            };
        }
    }
}
