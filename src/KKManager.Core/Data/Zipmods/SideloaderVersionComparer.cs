using System;
using System.Collections.Generic;
using System.Linq;

namespace KKManager.Data.Zipmods
{
    public class SideloaderVersionComparer : Comparer<string>
    {
        public override int Compare(string x, string y)
        {
            return CompareVersions(x, y);
        }

        public static int CompareVersions(string firstVer, string secondVer)
        {
            firstVer = firstVer?.Trim().TrimStart('v', 'V', 'r', 'R', ' ');
            if (string.IsNullOrEmpty(firstVer)) firstVer = "0";
            secondVer = secondVer?.Trim().TrimStart('v', 'V', 'r', 'R', ' ');
            if (string.IsNullOrEmpty(secondVer)) secondVer = "0";

            if (firstVer == secondVer) return 0;

            var version = new { First = Tokenize(firstVer), Second = Tokenize(secondVer) };
            var limit = Math.Max(version.First.Count, version.Second.Count);
            for (var i = 0; i < limit; i++)
            {
                var first = version.First.ElementAtOrDefault(i) ?? 0;
                var second = version.Second.ElementAtOrDefault(i) ?? 0;
                try
                {
                    var result = first.CompareTo(second);
                    if (result != 0)
                        return result;
                }
                catch (ArgumentException)
                {
                    var s1 = first.ToString();
                    var s2 = second.ToString();
                    
                    if (s1 == "0" && s2 != "0")
                        return -1;
                    
                    if (s2 == "0" && s1 != "0")
                        return 1;
                    
                    // Handle invalid characters in strings by comparing them byte by byte
                    var result = string.CompareOrdinal(s1, s2);
                    if (result != 0)
                        return result;
                }
            }

            return 0;
        }

        private static ICollection<IComparable> Tokenize(string version)
        {
            var results = new List<IComparable>(2);
            foreach (var part in version.Trim().Split('.', ' ', '-', ',', '_'))
            {
                if (part.Length == 0)
                {
                    results.Add(0);
                    continue;
                }

                // Handle mixed digit + letter parts by splitting them (1.0a -> 1 0 a)
                var isDigit = char.IsDigit(part[0]);
                var current = part[0].ToString();
                for (int i = 1; i < part.Length; i++)
                {
                    if (isDigit == char.IsDigit(part[i]))
                    {
                        current += part[i];
                    }
                    else
                    {
                        results.Add(Parse(current));
                        current = part[i].ToString();
                        isDigit = !isDigit;
                    }
                }

                results.Add(Parse(current));
            }

            return results;
        }

        private static IComparable Parse(string version) => int.TryParse(version, out var result) ? (IComparable)result : version;
    }
}