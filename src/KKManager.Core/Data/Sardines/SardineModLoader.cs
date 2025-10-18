using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using SharpCompress.Archives.Zip;
using Sideloader;

namespace KKManager.Data.Sardines
{
    public static class SardineModLoader
    {
        public static IObservable<SardineModInfo> Sardines => _sardines ?? StartReload();

        private static readonly object _lock = new object();
        private static ReplaySubject<SardineModInfo> _sardines;

        public static IObservable<SardineModInfo> StartReload()
        {
            lock (_lock)
            {
                if (_sardines == null || _currentTask == null || _currentTask.IsCompleted)
                {
                    _sardines = new ReplaySubject<SardineModInfo>();
                    _cancelSource?.Dispose();
                    _cancelSource = new CancellationTokenSource();
                    _currentTask = TryReadSardineMods(InstallDirectoryHelper..FullName, _sardines, _cancelSource.Token);
                }
            }
            return _sardines;
        }

        private static CancellationTokenSource _cancelSource;
        private static Task _currentTask;

        public static void CancelReload()
        {
            _cancelSource?.Cancel();
        }

        /// <summary>
        /// Gather information about valid plugins inside the selected directory
        /// </summary>
        /// <param name="modDirectory">Directory containing the zipmods to gather info from. Usually mods directory inside game root.</param>
        /// <param name="subject"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="searchOption">Where to search</param>
        public static Task TryReadSardineMods(string modDirectory, ReplaySubject<SardineModInfo> subject, CancellationToken cancellationToken, SearchOption searchOption = SearchOption.AllDirectories)
        {
            Console.WriteLine($"Start loading sardines from [{modDirectory}]");

            var token = cancellationToken;

            void ReadSideloaderModsAsync()
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    if (!Directory.Exists(modDirectory))
                    {
                        subject.OnCompleted();
                        Console.WriteLine("No sardines folder detected");
                        return;
                    }

                    var files = Directory.EnumerateFiles(modDirectory, "*.*", searchOption);
                    Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = cancellationToken }, file =>
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();

                            if (!IsValidZipmodExtension(Path.GetExtension(file))) return;

                            subject.OnNext(LoadFromFile(file));
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load zipmod from \"{file}\" with error: {ex.ToStringDemystified()}");
                        }
                    });

                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException aggr)
                        ex = aggr.Flatten().InnerExceptions.First();

                    if (ex is OperationCanceledException)
                        return;

                    if (ex is SecurityException || ex is UnauthorizedAccessException)
                        MessageBox.Show("Could not load information about zipmods because access to the plugins folder was denied. Check the permissions of your mods folder and try again.\n\n" + ex.Message,
                            "Load zipmods", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    Console.WriteLine("Crash when loading zipmods: " + ex.ToStringDemystified());
                    subject.OnError(ex);
                }
                finally
                {
                    Console.WriteLine($"Finished loading zipmods from [{modDirectory}] in {sw.ElapsedMilliseconds}ms");
                    subject.OnCompleted();
                }
            }

            try
            {
                var task = new Task(ReadSideloaderModsAsync, token, TaskCreationOptions.LongRunning);
                task.Start();
                return task;
            }
            catch (OperationCanceledException)
            {
                return Task.FromCanceled(token);
            }
        }

        public static SardineModInfo LoadFromFile(string filename)
        {
            var location = new FileInfo(filename);

            if (!IsValidZipmodExtension(location.Extension))
                throw new ArgumentException($"The file {filename} has an invalid extension and can't be a zipmod", nameof(filename));

            using (var zf = ZipArchive.Open(location))
            {
                // Without this reading crashes if any entry name has invalid characters
                // TODO not available in sharplib - zf.IgnoreDuplicateFiles = true;

                var manifest = Manifest.LoadFromZip(zf);

                if (manifest == null)
                    throw new InvalidDataException("manifest.xml was not found in the mod archive. Make sure this is a zipmod.");

                var images = new List<Func<Image>>();
                foreach (var imageFile in zf.Entries
                                            .Where(x =>
                                            {
                                                try
                                                {
                                                    return x.Key != null && (x.Key.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                                             x.Key.EndsWith(".png", StringComparison.OrdinalIgnoreCase));
                                                }
                                                catch (Exception e)
                                                {
                                                    // Handle entries with invalid characters in filename
                                                    Console.WriteLine($"WARN: Zipmod={location.Name} Entry={x.Key} Error={e.Message}");
                                                    return false;
                                                }
                                            })
                                            .OrderBy(x => x.Key).Take(5))
                {
                    var imgName = imageFile.Key;

                    images.Add(() =>
                    {
                        try
                        {
                            using (var zf2 = ZipArchive.Open(location))
                            {
                                var if2 = zf2.Entries.First(x => x.Key == imgName);
                                using (var archiveStream = if2.OpenEntryStream())
                                {
                                    using (var img = Image.FromStream(archiveStream))
                                    {
                                        return img.GetThumbnailImage(200, 200, null, IntPtr.Zero);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load image \"{imgName}\" from mod archive \"{location.Name}\" with error: {ex.Message}");
                            return null;
                        }
                    });
                }

                var contents = zf.Entries.Where(x => !x.IsDirectory).Select(x => x.Key?.Replace('/', '\\')).ToList();

                return new SideloaderModInfo(location, manifest, images, contents);
            }
        }

        public static bool IsValidZipmodExtension(string extension)
        {
            var exts = new[]
            {
                ".zip",
                ".zi_",
                ".zipmod",
                ".zi_mod",
            };

            return exts.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
        public static bool IsDisabledZipmod(string extension, out string enabledExtension)
        {
            if (extension.Equals(".zi_", StringComparison.OrdinalIgnoreCase))
            {
                enabledExtension = ".zip";
                return true;
            }
            if (extension.Equals(".zi_mod", StringComparison.OrdinalIgnoreCase))
            {
                enabledExtension = ".zipmod";
                return true;
            }
            enabledExtension = null;
            return false;
        }
    }
}