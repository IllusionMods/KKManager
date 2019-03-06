using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKManager.Util
{
    class StringTools
    {
        private static readonly char[] PathTrimChars = {
            '\\',
            '/',
            '"',
            // SPACE 
            '\u0020',
            // NO-BREAK SPACE 
            '\u00A0',
            // OGHAM SPACE MARK 
            '\u1680',
            // EN QUAD 
            '\u2000',
            // EM QUAD 
            '\u2001',
            // EN SPACE 
            '\u2002',
            // EM SPACE 
            '\u2003',
            // THREE-PER-EM SPACE 
            '\u2004',
            // FOUR-PER-EM SPACE 
            '\u2005',
            // SIX-PER-EM SPACE 
            '\u2006',
            // FIGURE SPACE 
            '\u2007',
            // PUNCTUATION SPACE 
            '\u2008',
            // THIN SPACE 
            '\u2009',
            // HAIR SPACE 
            '\u200A',
            // NARROW NO-BREAK SPACE 
            '\u202F',
            // MEDIUM MATHEMATICAL SPACE 
            '\u205F',
            // and IDEOGRAPHIC SPACE 
            '\u3000',

            // LINE SEPARATOR 
            '\u2028',

            // PARAGRAPH SEPARATOR  
            '\u2029',

            // CHARACTER TABULATION 
            '\u0009',
            // LINE FEED 
            '\u000A',
            // LINE TABULATION 
            '\u000B',
            // FORM FEED 
            '\u000C',
            // CARRIAGE RETURN 
            '\u000D',
            // NEXT LINE 
            '\u0085'
        };

        public static bool PathsEqual(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                return false;

            path1 = NormalizePath(path1);
            path2 = NormalizePath(path2);
            return path1.Equals(path2, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public static string NormalizePath(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return path.Trim(PathTrimChars);
        }

        public static bool PathsEqual(FileSystemInfo path1, FileSystemInfo path2)
        {
            if (path1 == null || path2 == null)
                return false;

            return PathsEqual(path1.FullName, path2.FullName);
        }
    }
}
