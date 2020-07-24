using System.IO;

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
    }
}
