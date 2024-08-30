using System;
using System.IO;
using KKManager.Util;
using MessagePack;
using Newtonsoft.Json;

namespace KKManager.Data.Cards
{
    public static class Utility
    {
        public static long SearchForSequence(Stream stream, byte[] sequence)
        {
            const int bufferSize = 4096;
            var origPos = stream.Position;

            var buffer = new byte[bufferSize];
            int read;

            var scanByte = sequence[0];

            while ((read = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (var i = 0; i < read; i++)
                {
                    if (buffer[i] != scanByte)
                        continue;

                    var flag = true;

                    for (var x = 1; x < sequence.Length; x++)
                    {
                        i++;

                        if (i >= bufferSize)
                        {
                            if ((read = stream.Read(buffer, 0, bufferSize)) < bufferSize)
                                return -1;

                            i = 0;
                        }

                        if (buffer[i] != sequence[x])
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        var result = (stream.Position + 1) - (bufferSize - i) - sequence.Length;
                        stream.Position = origPos;
                        return result;
                    }
                }
            }

            return -1;
        }

        private static readonly byte[] _pngEndChunk = { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
        public static long SearchForPngEnd(Stream stream)
        {
            var result = SearchForSequence(stream, _pngEndChunk);
            if (result >= 0) result += _pngEndChunk.Length;
            return result;
        }

        private static readonly byte[] _pngStartChunk = { 0x89, 0x50, 0x4E, 0x47, 0x0D };
        public static long SearchForPngStart(Stream stream)
        {
            return SearchForSequence(stream, _pngStartChunk);
        }

        public static bool TryGetObject<TObj>(this BlockHeader blockHeader, BinaryReader reader, long basePosition, string blockName, out TObj blockInfo, out BlockHeader.Info info)
        {
            if (blockHeader == null) throw new ArgumentNullException(nameof(blockHeader));
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (blockName == null) throw new ArgumentNullException(nameof(blockName));

            if (!reader.BaseStream.CanRead) throw new ArgumentException(@"stream is not readable", nameof(reader));
            if (basePosition < 0 || reader.BaseStream.Length <= basePosition) throw new ArgumentOutOfRangeException(nameof(basePosition), basePosition, @"position must be inside the stream");

            blockInfo = default;
            info = blockHeader.SearchInfo(blockName);
            if (info != null)
            {
                reader.BaseStream.Seek(basePosition + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);

                try
                {
                    blockInfo = MessagePackSerializer.Deserialize<TObj>(parameterBytes);

                    if (blockInfo != null)
                        return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($@"Failed to deserialize block ""{blockName}"" from BlockHeader - {e}");
                }
            }
            return false;
        }

        public static FileSize GetSize(this BlockHeader.Info info)
        {
            return info != null ? FileSize.FromBytes((int)info.size) : FileSize.Empty;
        }

        public static void DumpBlocksToJson(this BlockHeader blockHeader, BinaryReader reader, long basePosition, DirectoryInfo targetDirectory)
        {
            if (blockHeader == null) throw new ArgumentNullException(nameof(blockHeader));
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (targetDirectory == null) throw new ArgumentNullException(nameof(targetDirectory));

            targetDirectory.Create();

            foreach (var blockInfo in blockHeader.lstInfo)
            {
                reader.BaseStream.Seek(basePosition + blockInfo.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)blockInfo.size);

                var jsn = MessagePackSerializer.ConvertToJson(parameterBytes);

                dynamic parsedJson = JsonConvert.DeserializeObject(jsn);
                jsn = JsonConvert.SerializeObject((object)parsedJson, Formatting.Indented);

                File.WriteAllText(Path.Combine(targetDirectory.FullName, $@"BLOCK_{blockInfo.name}.json"), jsn);
            }
        }
    }
}
