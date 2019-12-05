using System;
using System.Collections.Generic;
using System.Linq;

namespace KKManager.Data.Zipmods
{
    public class SideloaderVersionComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return CompareVersions(x, y);
            }

            public static int CompareVersions(string firstVer, string secondVer)
            {
                if (firstVer == secondVer) return 0;
                var version = new { First = GetVersion(firstVer), Second = GetVersion(secondVer) };
                var limit = Math.Max(version.First.Length, version.Second.Length);
                for (var i = 0; i < limit; i++)
                {
                    var first = version.First.ElementAtOrDefault(i) ?? string.Empty;
                    var second = version.Second.ElementAtOrDefault(i) ?? string.Empty;
                    try
                    {
                        var result = first.CompareTo(second);
                        if (result != 0)
                            return result;
                    }
                    catch (ArgumentException)
                    {
                        if (first is string s1 && second is string s2)
                        {
                            // Handle invalid characters in strings by comparing them byte by byte
                            var result = string.CompareOrdinal(s1, s2);
                            if (result != 0)
                                return result;
                        }
                    }
                }
                return version.First.Length.CompareTo(version.Second.Length);
            }

            private static IComparable[] GetVersion(string version)
            {
                return (from part in version.Trim().Split('.', ' ', '-', ',', '_')
                        select Parse(part)).ToArray();
            }

            private static IComparable Parse(string version)
            {
                if (int.TryParse(version, out var result))
                    return result;
                return version;
            }
        }
}