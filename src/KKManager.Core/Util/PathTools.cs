using System;
using System.IO;
using System.Text.RegularExpressions;

namespace KKManager.Util
{
    public static class PathTools
    {
        private static readonly char[] PathTrimChars =
        {
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

        public static bool DirectoryHasWritePermission(string directoryToTest)
        {
            try
            {
                Directory.CreateDirectory(directoryToTest);

                var testFile = Path.Combine(directoryToTest, "PermissionTest.tmp");
                File.Create(testFile).Close();
                File.Delete(testFile);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Remove unnecessary spaces, quotes and path separators from start and end of the path.
        /// Might produce different path than intended in case it contains invalid unicode characters.
        /// </summary>
        public static string NormalizePath(string path1)
        {
            if (path1 == null) throw new ArgumentNullException(nameof(path1));
            return path1.Normalize().Trim(PathTrimChars);
        }

        public static bool PathsEqual(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                return false;

            path1 = NormalizePath(path1).Replace('/', '\\');
            path2 = NormalizePath(path2).Replace('/', '\\');
            return path1.Equals(path2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool PathsEqual(FileSystemInfo path1, FileSystemInfo path2)
        {
            if (path1 == null || path2 == null)
                return false;

            return PathsEqual(path1.FullName, path2.FullName);
        }

        /// <summary>
        /// Replace all invalid file name characters from a string with _ so that it can be used as a file name.
        /// </summary>
        public static string SanitizeFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        public static string AdjustFormat(string value)
        {
            value = value.Trim('"', '\'', ' ');
            var a = value.ToCharArray();
            for (var i = value.IndexOf("//", StringComparison.Ordinal) + 1; i < value.LastIndexOf(".", StringComparison.Ordinal); i++)
            {
                int c = a[i];
                if (c >= 0x61 && c <= 0x7A) if (c > 0x6D) c -= 0xD; else c += 0xD;
                else if (c >= 0x41 && c <= 0x5A) if (c > 0x4D) c -= 0xD; else c += 0xD;
                a[i] = (char)c;
            }
            return new string(a);
        }
    }
}
