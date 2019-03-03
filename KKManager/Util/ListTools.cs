using System;
using System.Linq;

namespace KKManager.Util
{
    internal static class ListTools
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
    }
}
