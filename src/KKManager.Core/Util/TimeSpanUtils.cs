using System;
using System.Collections.Generic;
using System.Linq;

namespace KKManager.Util
{
    public static class TimeSpanUtils
    {
        /// <summary>
        /// Get a human readable version of the timespan
        /// https://stackoverflow.com/a/21649465/4309247
        /// </summary>
        public static string GetReadableTimespan(this TimeSpan ts)
        {
            // formats and its cutoffs based on totalseconds
            var cutoff = new SortedList<long, string>
            {
                {59, "{3:S}"},
                {60, "{2:M}"},
                {60 * 60 - 1, "{2:M}, {3:S}"},
                {60 * 60, "{1:H}"},
                {24 * 60 * 60 - 1, "{1:H}, {2:M}"},
                {24 * 60 * 60, "{0:D}"},
                {long.MaxValue, "{0:D}, {1:H}"}
            };

            // find nearest best match
            var find = cutoff.Keys.ToList().BinarySearch((long)ts.TotalSeconds);
            // negative values indicate a nearest match
            var near = find < 0 ? Math.Abs(find) - 1 : find;
            // use custom formatter to get the string
            return string.Format(
                new HMSFormatter(),
                cutoff[cutoff.Keys[near]],
                ts.Days,
                ts.Hours,
                ts.Minutes,
                ts.Seconds);
        }

        // formatter for forms of
        // seconds/hours/day
        public class HMSFormatter : ICustomFormatter, IFormatProvider
        {
            // list of Formats, with a P customformat for pluralization
            private static readonly Dictionary<string, string> timeformats = new Dictionary<string, string>
            {
                {"S", "{0:P:Seconds:Second}"},
                {"M", "{0:P:Minutes:Minute}"},
                {"H", "{0:P:Hours:Hour}"},
                {"D", "{0:P:Days:Day}"}
            };

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                return string.Format(new PluralFormatter(), timeformats[format], arg);
            }

            public object GetFormat(Type formatType)
            {
                return formatType == typeof(ICustomFormatter) ? this : null;
            }
        }

        // formats a numeric value based on a format P:Plural:Singular
        public class PluralFormatter : ICustomFormatter, IFormatProvider
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg != null)
                {
                    var parts = format.Split(':'); // ["P", "Plural", "Singular"]

                    if (parts[0] == "P") // correct format?
                    {
                        // which index postion to use
                        var partIndex = arg.ToString() == "1" ? 2 : 1;
                        // pick string (safe guard for array bounds) and format
                        return string.Format("{0} {1}", arg, parts.Length > partIndex ? parts[partIndex] : "");
                    }
                }

                return string.Format(format, arg);
            }

            public object GetFormat(Type formatType)
            {
                return formatType == typeof(ICustomFormatter) ? this : null;
            }
        }
    }
}