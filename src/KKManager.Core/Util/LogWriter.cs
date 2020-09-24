using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace KKManager.Util
{
    public sealed class LogWriter : StreamWriter
    {
        private readonly LogStreamSplitter _logStream;

        private LogWriter(string logFilePath) : this(logFilePath, Encoding.UTF8)
        {
        }

        private LogWriter(string logFilePath, Encoding encoding) : this(encoding,
            CreateBaseLogStream(logFilePath, encoding))
        {
        }

        private LogWriter(Encoding encoding, LogStreamSplitter logStream) : base(logStream, encoding, 1024, false)
        {
            _logStream = logStream;
            LogFilePath = logStream.BaseStream.Name;
            AutoFlush = true;
        }

        public string LogFilePath { get; }


        public long CurrentLogStartPosition { get; set; }

        public void AddListener(TextWriter listener)
        {
            _logStream.CopyWriters.Add(listener);
        }

        public void RemoveListener(TextWriter listener)
        {
            _logStream.CopyWriters.Remove(listener);
        }

        private static string CreateLogFilenameForAssembly(Assembly assembly)
        {
            var location = assembly.Location;
            if (location.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                location = location.Remove(location.Length - 4);
            location += ".log";
            return location;
        }

        private static LogStreamSplitter CreateBaseLogStream(string logFilePath, Encoding encoding)
        {
            return new LogStreamSplitter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read),
                encoding);
        }

        /// <summary>
        /// Start logging to a file reflecting the calling assembly name.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
        public static LogWriter StartLogging(Assembly entryAssembly = null)
        {
            if (entryAssembly == null) entryAssembly = Assembly.GetCallingAssembly();
            var location = CreateLogFilenameForAssembly(entryAssembly);
            return StartLogging(location, entryAssembly.GetName());
        }

        /// <summary>
        /// Start logging to a file.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
        public static LogWriter StartLogging(string logPath, AssemblyName appName)
        {
            try
            {
                // Limit log size to 100 kb
                var fileInfo = new FileInfo(logPath);
                if (fileInfo.Exists && fileInfo.Length > 1024 * 100)
                    fileInfo.Delete();

                // Create new log writer
                var logWriter = new LogWriter(logPath);

                // Make sure we can write to the file
                logWriter.WriteSeparator();
                logWriter.Flush();
                logWriter.CurrentLogStartPosition = logWriter.BaseStream.Position;

                logWriter.AddListener(Console.Out);
                Console.SetOut(logWriter);
                Console.SetError(logWriter);

                logWriter.WriteLine($"Application startup - {appName.Name} v{appName.Version} {appName.ProcessorArchitecture}");
                logWriter.Flush();

                Debug.Listeners.Add(new TextWriterTraceListener(logWriter));

                return logWriter;
            }
            catch (Exception ex)
            {
                // Ignore logging errors
                Console.WriteLine(ex);
                return null;
            }
        }

        public void WriteSeparator()
        {
            base.WriteLine("--------------------------------------------------");
        }

        public override void WriteLine(object value)
        {
            if (value is Exception ex)
                WriteLine(ex.ToStringDemystified());
            else
                base.WriteLine(value);
        }

        public override void Write(object value)
        {
            if (value is Exception ex)
                base.Write(ex.ToStringDemystified());
            else
                base.Write(value);
        }

        public override void WriteLine(string value)
        {
            var fullEntry = "[" + DateTime.Now.ToString("T", CultureInfo.InvariantCulture) + "] " + value;
            base.WriteLine(fullEntry);
        }

        public string GetLog()
        {
            Flush();
            using (var reader =
                new StreamReader(File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                    Encoding.UTF8, true, 1024, false))
            {
                reader.BaseStream.Seek(CurrentLogStartPosition, SeekOrigin.Begin);
                return reader.ReadToEnd();
            }
        }

        private class LogStreamSplitter : Stream
        {
            public readonly FileStream BaseStream;
            public readonly List<TextWriter> CopyWriters = new List<TextWriter>(2);
            public readonly Encoding Encoding;

            public LogStreamSplitter(FileStream baseStream, Encoding encoding)
            {
                BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
                Encoding = encoding;
            }

            public override bool CanRead => BaseStream.CanRead;

            public override bool CanSeek => BaseStream.CanSeek;

            public override bool CanWrite => BaseStream.CanWrite;

            public override long Length => BaseStream.Length;

            public override long Position
            {
                get => BaseStream.Position;
                set => BaseStream.Position = value;
            }

            public override void Flush()
            {
                BaseStream.Flush();
                foreach (var writer in CopyWriters) writer.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return BaseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                BaseStream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return BaseStream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                BaseStream.Write(buffer, offset, count);

                if (CopyWriters.Count > 0)
                {
                    var str = Encoding.GetString(buffer, offset, count);
                    foreach (var writer in CopyWriters) writer.Write(str);
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                BaseStream.Dispose();
                foreach (var writer in CopyWriters) writer.Dispose();
                CopyWriters.Clear();
            }
        }
    }
}