using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using BrightIdeasSoftware;
using KKManager.Properties;
using Microsoft.VisualBasic.FileIO;
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
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException("wtf " + path));
                    extractor.WriteEntryTo(path);
                }
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

        public static IEnumerable<TOut> Attempt<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, TOut> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                TOut output;
                try
                {
                    output = action(item);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    continue;
                }
                yield return output;
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var knownKeys = new HashSet<TKey>();
            foreach (var item in source)
            {
                if (knownKeys.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

        public static string GetNameWithoutExtension(this FileSystemInfo fsi)
        {
            if (fsi == null) throw new ArgumentNullException(nameof(fsi));
            return Path.GetFileNameWithoutExtension(fsi.Name);
        }

        public static string GetFullNameWithDifferentExtension(this FileInfo fi, string newExtension)
        {
            if (fi == null) throw new ArgumentNullException(nameof(fi));
            if (newExtension == null) throw new ArgumentNullException(nameof(newExtension));

            return Path.Combine(fi.DirectoryName ?? throw new InvalidOperationException("DirectoryName null for " + fi),
                                fi.GetNameWithoutExtension() + newExtension);
        }

        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static XElement GetOrAddElement(this XElement e, string name)
        {
            var result = e.Element(name);
            if (result == null)
            {
                result = new XElement(name);
                e.Add(result);
            }
            return result;
        }

        public static void AddObjects<T>(this ObjectListView olv, IList<T> modelObjects) => olv.AddObjects((ICollection)modelObjects);
        public static void RefreshObjects<T>(this ObjectListView olv, IList<T> modelObjects) => olv.RefreshObjects((IList)modelObjects);

        public static async Task SafeDelete(this FileSystemInfo info)
        {
            if (info == null) return;

            var toRecycleBin = Settings.Default.DeleteToRecycleBin;

            try
            {
                if (info.Exists)
                {
                    // Prevent issues removing readonly files
                    info.Attributes = FileAttributes.Normal;

                    if (info is DirectoryInfo dir)
                    {
                        try
                        {
                            if (toRecycleBin)
                                FileSystem.DeleteDirectory(dir.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            else
                                dir.Delete(true);
                        }
                        catch
                        {
                            await Task.Delay(100);
                            dir.Delete(true);
                        }
                    }
                    else if (info is FileInfo file)
                    {
                        try
                        {
                            if (toRecycleBin)
                                FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            else
                                file.Delete();
                        }
                        catch
                        {
                            await Task.Delay(100);
                            file.Delete();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("wtf is" + info.GetType().AssemblyQualifiedName);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is AggregateException ae && ae.InnerExceptions.Count == 1)
                    ex = ae.InnerExceptions[0];

                Console.WriteLine($"Failed to delete [{info.FullName}] because of error: {ex.ToStringDemystified()}");

                throw;
            }
        }
        
        public static Task SafeDelete(this string filename)
        {
            if (File.Exists(filename))
                return new FileInfo(filename).SafeDelete();
            else if (Directory.Exists(filename))
                return new DirectoryInfo(filename).SafeDelete();
            else
                return Task.CompletedTask;
        }
    }
}
