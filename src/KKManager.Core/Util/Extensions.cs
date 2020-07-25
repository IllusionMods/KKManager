using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace KKManager.Util
{
    public static class Extensions
    {
        public static void SafeInvoke(this Control obj, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!obj.IsDisposed && !obj.Disposing)
            {
                if (obj.InvokeRequired)
                {
                    try
                    {
                        obj.Invoke(action);
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                    catch (InvalidAsynchronousStateException) { }
                }
                else
                {
                    action();
                }
            }
        }

        public static void ExtractArchiveToDirectory(this IArchive archive, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
            var extractor = archive.ExtractAllEntries();
            while (extractor.MoveToNextEntry())
            {
                var path = Path.Combine(targetDirectory, extractor.Entry.Key);
                if (extractor.Entry.IsDirectory) Directory.CreateDirectory(path);
                else extractor.WriteEntryTo(path);
            }
        }

        /// <summary>
        /// Returns true if the task finished in time, false if the task timed out.
        /// </summary>
        public static async Task<bool> WithTimeout(this Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var result = await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) == task;
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }
    }
}
