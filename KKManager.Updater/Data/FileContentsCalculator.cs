using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Crc;
using MessagePack;

namespace KKManager.Updater.Data
{
    public static class FileContentsCalculator
    {
        [ThreadStatic]
        private static CRC32 _crc;

        public static int GetFileHash(FileInfo file)
        {
            // todo calculate once and cache?

            if (!file.Exists) return -1;

            if (SB3UGS.SB3UGS_Utils.FileIsAssetBundle(file))
            {
                try
                {
                    return SB3UGS.SB3UGS_Utils.HashFileContents(file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get content hash with SB3UGS, falling back to full hash. File: " + file.FullName + " Exception: " + ex.Message);
                }
            }

            if (_crc == null) _crc = new CRC32();
            else _crc.Reset();

            using (var rs = file.OpenRead())
                return _crc.GetCrc32(rs);
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

        public static byte[] SerializeHashes(Dictionary<string, int> hashes)
        {
            return MessagePackSerializer.Serialize(hashes, MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block));
        }

        public static Dictionary<string, int> DeserializeHashes(byte[] data)
        {
            return MessagePackSerializer.Deserialize<Dictionary<string, int>>(data);
        }
    }
}
