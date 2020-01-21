using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.SB3UGS;
using KKManager.Updater.Utils;
using MessagePack;

namespace KKManager.Updater.Data
{
    public static class FileContentsCalculator
    {
        private static readonly Dictionary<string, Tuple<long, int>> _hashCache = new Dictionary<string, Tuple<long, int>>();

        [ThreadStatic]
        private static CRC32 _crc;

        static FileContentsCalculator()
        {
            var hashCachePath = Path.Combine(Path.GetDirectoryName(typeof(FileContentsCalculator).Assembly.Location), "FileContentsHashCache.bin");
            var serializerOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block).WithOmitAssemblyVersion(true);

            Application.ApplicationExit += (sender, args) =>
                {
                    try
                    {
                        Console.WriteLine("Saving hash cache to " + hashCachePath);
                        File.WriteAllBytes(hashCachePath, MessagePackSerializer.Serialize(_hashCache, serializerOptions));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to save hash cache: " + e);
                    }
                };

            if (File.Exists(hashCachePath))
            {
                try
                {
                    Console.WriteLine("Loading hash cache from " + hashCachePath);
                    _hashCache = MessagePackSerializer.Deserialize<Dictionary<string, Tuple<long, int>>>(File.OpenRead(hashCachePath), serializerOptions);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to load hash cache: " + e);
                }
            }
        }

        public static int GetFileHash(FileInfo file)
        {
            var identifier = file.LastWriteTimeUtc.Ticks ^ file.Length;
            var cacheKey = file.FullName.ToLower();

            _hashCache.TryGetValue(cacheKey, out var cacheEntry);
            if (cacheEntry != null && identifier == cacheEntry.Item1)
                return cacheEntry.Item2;

            var result = GetFileHashImpl(file);

            _hashCache[cacheKey] = new Tuple<long, int>(identifier, result);

            return result;
        }

        public static Dictionary<FileInfo, int> GetFileHashes(IEnumerable<FileInfo> files)
        {
            return files.ToDictionary(x => x, GetFileHash);
        }

        public static Dictionary<FileInfo, int> GetFileHashes(DirectoryInfo directory, bool recursive)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (!directory.Exists) return new Dictionary<FileInfo, int>();
            return GetFileHashes(directory.GetFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
        }

        private static int GetFileHashImpl(FileInfo file)
        {
            if (!file.Exists) return -1;

            if (SB3UGS_Utils.FileIsAssetBundle(file))
            {
                try
                {
                    return SB3UGS_Utils.HashFileContents(file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get content hash with SB3UGS, falling back to full hash. File: " + file.FullName + " Exception: " + ex.Message);
                }
            }

            if (_crc == null) _crc = new CRC32();
            else _crc.Reset();

            using (var rs = file.OpenRead())
            {
                return _crc.GetCrc32(rs);
            }
        }
    }
}
