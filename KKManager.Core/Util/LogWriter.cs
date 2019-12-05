using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace KKManager.Util
{
    public sealed class LogWriter : StreamWriter
    {
        private static string CreateLogFilenameForAssembly(Assembly assembly)
        {
            var location = assembly.Location;
            if (location.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                location = location.Remove(location.Length - 4);
            location += ".log";
            return location;
        }

        private LogWriter(string path) : base(path, true, Encoding.UTF8)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleOut?.Dispose();
            if (disposing)
                Disposed = true;
        }

        private bool Disposed { get; set; }

        /// <summary>
        /// Start logging to a file reflecting the calling assembly name.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
        public static LogWriter StartLogging()
        {
            var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());
            return StartLogging(location);
        }

        /// <summary>
        /// Start logging to a file.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
        public static LogWriter StartLogging(string logPath)
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
                logWriter.WriteLine("Application startup");
                logWriter.Flush();

                logWriter.ConsoleOut = Console.Out;

                Console.SetOut(logWriter);
                Console.SetError(logWriter);

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

        private TextWriter ConsoleOut { get; set; }

        public void WriteSeparator()
        {
            if (Disposed) return;
            base.WriteLine("--------------------------------------------------");
        }

        public override void WriteLine(string value)
        {
            if (Disposed) return;

            ConsoleOut?.WriteLine(value);

            base.AutoFlush = false;
            base.Write('[');
            base.Write(DateTime.UtcNow.ToString("T", CultureInfo.InvariantCulture));
            base.Write(']');
            base.WriteLine(value);
            base.AutoFlush = true;
        }
    }
}
