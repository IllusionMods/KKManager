using System;
using System.Collections.Generic;
using System.Linq;

namespace KKManager.Util
{
    public struct FileSize
    {
        public enum SizeRange
        {
            None = 0,
            Kb,
            Mb,
            Gb,
            Tb
        }

        public static FileSize SumFileSizes(IEnumerable<FileSize> sizes)
        {
            return FromKilobytes(sizes.Sum(x => x.GetRawSize(false)));
        }

        public static readonly FileSize Empty = new FileSize(0);
        private readonly long _sizeInKb;

        public FileSize(long kiloBytes)
        {
            _sizeInKb = kiloBytes;
        }

        public static bool operator !=(FileSize a, FileSize b)
        {
            return a._sizeInKb != b._sizeInKb;
        }

        public static FileSize operator +(FileSize a, FileSize b)
        {
            return new FileSize(a._sizeInKb + b._sizeInKb);
        }

        public static FileSize operator -(FileSize a, FileSize b)
        {
            return new FileSize(a._sizeInKb - b._sizeInKb);
        }

        public static bool operator ==(FileSize a, FileSize b)
        {
            return a._sizeInKb == b._sizeInKb;
        }

        public static FileSize FromBytes(long bytes)
        {
            if (bytes < 0)
                bytes = 0;
            return new FileSize(bytes / 1024);
        }

        public static FileSize FromKilobytes(long kiloBytes)
        {
            if (kiloBytes < 0)
                kiloBytes = 0;

            return new FileSize(kiloBytes);
        }

        public override bool Equals(object obj)
        {
            return (obj is FileSize) && _sizeInKb.Equals(((FileSize)obj)._sizeInKb);
        }

        public override int GetHashCode()
        {
            return _sizeInKb.GetHashCode();
        }

        public long GetRawSize()
        {
            return _sizeInKb;
        }

        /// <summary>
        /// Empty items return long.MaxValue. For use in sorting
        /// </summary>
        public long GetRawSize(bool treatEmptyAsLargest)
        {
            if (treatEmptyAsLargest && _sizeInKb <= 0)
                return long.MaxValue;
            return _sizeInKb;
        }

        public float GetCompactSize(out SizeRange sizeRange)
        {
            sizeRange = SizeRange.None;

            if (_sizeInKb <= 0)
                return 0;

            sizeRange = SizeRange.Kb;
            var tempSize = (double)_sizeInKb;
            if (tempSize < 1024)
                return (float)tempSize;

            sizeRange = SizeRange.Mb;
            tempSize = tempSize / 1024;
            if (tempSize < 1024)
                return (float)tempSize;

            sizeRange = SizeRange.Gb;
            tempSize = tempSize / 1024;
            if (tempSize < 1024)
                return (float)tempSize;

            sizeRange = SizeRange.Tb;
            tempSize = tempSize / 1024;
            return (float)tempSize;
        }

        /// <summary>
        /// Returns string representation of the filesize in format "Number.Decimal ShortName"
        /// (eg. 32,51 MB, 1021 KB)
        /// </summary>
        public override string ToString()
        {
            var value = GetCompactSize(out var range);
            
            return $"{Math.Round(value, 2)} {GetRangeString(range)}";
        }

        private static string GetRangeString(SizeRange range)
        {
            if (range == SizeRange.None)
                return "B";

            string rangeName;
            switch (range)
            {
                case SizeRange.Tb:
                    rangeName = "TB";
                    break;
                case SizeRange.Gb:
                    rangeName = "GB";
                    break;
                case SizeRange.Mb:
                    rangeName = "MB";
                    break;
                case SizeRange.Kb:
                    rangeName = "KB";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(range), range, "Unknown range");
            }

            return rangeName;
        }
    }
}
