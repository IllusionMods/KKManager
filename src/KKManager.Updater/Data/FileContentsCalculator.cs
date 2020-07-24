using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.SB3UGS;
using KKManager.Updater.Utils;
using MessagePack;
using MessagePack.Resolvers;

namespace KKManager.Updater.Data
{
    public static class FileContentsCalculator
    {
        private static readonly Dictionary<string, HashInfo> _hashCache = new Dictionary<string, HashInfo>();


        static FileContentsCalculator()
        {
            var hashCachePath = Path.Combine(Path.GetDirectoryName(typeof(FileContentsCalculator).Assembly.Location),
                "FileContentsHashCache.bin");
            var serializerOptions = MessagePackSerializerOptions.Standard
                .WithCompression(MessagePackCompression.Lz4Block).WithOmitAssemblyVersion(true)
                .WithResolver(StandardResolverAllowPrivate.Instance);

            Application.ApplicationExit += (sender, args) =>
            {
                try
                {
                    Console.WriteLine("Saving hash cache to " + hashCachePath);
                    File.WriteAllBytes(hashCachePath,
                        MessagePackSerializer.Serialize(_hashCache.Values.ToList(), serializerOptions));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to save hash cache: " + e.Message);
                }
            };

            if (File.Exists(hashCachePath))
            {
                try
                {
                    Console.WriteLine("Loading hash cache from " + hashCachePath);
                    _hashCache = MessagePackSerializer.Deserialize<List<HashInfo>>(File.OpenRead(hashCachePath), serializerOptions).ToDictionary(x => x.CacheKey);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to load hash cache: " + e.Message);
                }
            }
        }

        public static HashInfo GetFileHash(FileInfo file)
        {
            if (!file.Exists) return null;

            var identifier = file.LastWriteTimeUtc.Ticks ^ file.Length;
            var cacheKey = file.FullName.ToLower();

            _hashCache.TryGetValue(cacheKey, out var cacheEntry);
            if (cacheEntry != null && identifier == cacheEntry.Identifier)
                return cacheEntry;

            var result = new HashInfo { CacheKey = cacheKey, Identifier = identifier, File = file };
            _hashCache[cacheKey] = result;

            return result;
        }

        public static Dictionary<FileInfo, HashInfo> GetFileHashes(IEnumerable<FileInfo> files)
        {
            return files.ToDictionary(x => x, GetFileHash);
        }

        public static Dictionary<FileInfo, HashInfo> GetFileHashes(DirectoryInfo directory, bool recursive)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (!directory.Exists) return new Dictionary<FileInfo, HashInfo>();
            return GetFileHashes(directory.GetFiles("*",
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
        }

        [MessagePackObject]
        public sealed class HashInfo
        {
            [ThreadStatic] private static CRC32 _crc;

            [IgnoreMember] private FileInfo _file;

            [Key(2)] private int _fileHash;

            [Key(1)] private int _sb3UHash;

            [Key(0)] public long Identifier { get; set; }

            [Key(3)] public string CacheKey { get; set; }

            [IgnoreMember]
            public FileInfo File
            {
                get => _file ?? (_file = new FileInfo(CacheKey));
                set => _file = value;
            }

            [IgnoreMember]
            public int FileHash
            {
                get
                {
                    if (_fileHash == 0)
                        RecalculateFileHash();
                    return _fileHash;
                }
                set => _fileHash = value;
            }

            [IgnoreMember]
            public int SB3UHash
            {
                get
                {
                    if (_sb3UHash == 0)
                        RecalculateSB3UHash();
                    return _sb3UHash;
                }
                set => _sb3UHash = value;
            }

            public void RecalculateSB3UHash()
            {
                _sb3UHash = 0;
                try
                {
                    if (SB3UGS_Utils.FileIsAssetBundle(File))
                        _sb3UHash = SB3UGS_Utils.HashFileContents(File.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get content hash with SB3UGS, falling back to full hash. File: " +
                                      File.FullName + " Exception: " + ex.Message);
                }
            }

            public void RecalculateFileHash()
            {
                _fileHash = 0;
                if (!System.IO.File.Exists(CacheKey))
                    return;

                if (_crc == null) _crc = new CRC32();
                else _crc.Reset();

                using (var rs = System.IO.File.OpenRead(CacheKey))
                    _fileHash = _crc.GetCrc32(rs);
            }
        }
    }
}