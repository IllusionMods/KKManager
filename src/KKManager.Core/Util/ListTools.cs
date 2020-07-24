using System;
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
    }
}
