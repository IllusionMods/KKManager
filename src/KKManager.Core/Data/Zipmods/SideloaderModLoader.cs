﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using KKManager.Functions;
using KKManager.Util;
using Sideloader;

namespace KKManager.Data.Zipmods
{
    public static class SideloaderModLoader
    {
        public static IObservable<SideloaderModInfo> Zipmods
        {
            get => _zipmods ?? StartReload();
        }

        private static readonly object _lock = new object();
        private static ReplaySubject<SideloaderModInfo> _zipmods;

        public static IObservable<SideloaderModInfo> StartReload()
        {
            lock (_lock)
            {
                if (_zipmods == null || _currentTask == null || _currentTask.IsCompleted)
                {
                    _zipmods = new ReplaySubject<SideloaderModInfo>();
                    _cancelSource?.Dispose();
                    _cancelSource = new CancellationTokenSource();
                    _currentTask = TryReadSideloaderMods(InstallDirectoryHelper.ModsPath.FullName, _zipmods, _cancelSource.Token);
                }
            }
            return _zipmods;
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
        /// <param name="searchOption">Where to search</param>
        public static Task TryReadSideloaderMods(string modDirectory, ReplaySubject<SideloaderModInfo> subject, CancellationToken cancellationToken, SearchOption searchOption = SearchOption.AllDirectories)
        {
            Console.WriteLine("Start loading zipmods from " + modDirectory);

            var token = cancellationToken;

            void ReadSideloaderModsAsync()
            {
                try
                {
                    if (!Directory.Exists(modDirectory))
                    {
                        subject.OnCompleted();
                        Console.WriteLine("No zipmod folder detected");
                        return;
                    }

                    foreach (var file in Directory.EnumerateFiles(modDirectory, "*.*", searchOption))
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();

                            if (!IsValidZipmodExtension(Path.GetExtension(file))) continue;

                            //var modInfo = Task.Run(() => LoadFromFile(file), token);
                            //if (!modInfo.Wait(TimeSpan.FromSeconds(8))) throw new TimeoutException();
                            //subject.OnNext(modInfo.Result);
                            subject.OnNext(LoadFromFile(file));
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load zipmod from \"{file}\" with error: {ex}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    if (ex is SecurityException || ex is UnauthorizedAccessException)
                        MessageBox.Show("Could not load information about zipmods because access to the plugins folder was denied. Check the permissions of your mods folder and try again.\n\n" + ex.Message,
                            "Load zipmods", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    Console.WriteLine("Crash when loading zipmods: " + ex);
                    subject.OnError(ex);
                }
                finally
                {
                    Console.WriteLine("Finished loading zipmods");
                    subject.OnCompleted();
                }
            }

            try
            {
                return Task.Run(ReadSideloaderModsAsync, token);
            }
            catch (OperationCanceledException)
            {
                return Task.FromCanceled(token);
            }
        }

        public static SideloaderModInfo LoadFromFile(string filename)
        {
            var location = new FileInfo(filename);

            if (!IsValidZipmodExtension(location.Extension))
                throw new ArgumentException($"The file {filename} has an invalid extension and can't be a zipmod", nameof(filename));

            using (var reader = location.OpenRead())
            using (var zf = SharpCompress.Archives.ArchiveFactory.Open(reader))
            {
                var manifest = Manifest.LoadFromZip(zf);

                if (manifest == null)
                    throw new InvalidDataException("manifest.xml was not found in the mod archive. Make sure this is a zipmod.");

                var images = new List<Image>();
                // TODO load from drive instead of caching to ram
                foreach (var imageFile in zf.Entries
                                            .Where(x => ".jpg".Equals(Path.GetExtension(x.Key), StringComparison.OrdinalIgnoreCase) ||
                                                        ".png".Equals(Path.GetExtension(x.Key), StringComparison.OrdinalIgnoreCase))
                                            .OrderBy(x => x.Key).Take(3))
                {
                    try
                    {
                        using (var stream = imageFile.OpenEntryStream())
                        using (var img = Image.FromStream(stream))
                        {
                            images.Add(img.GetThumbnailImage(200, 200, null, IntPtr.Zero));
                        }
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine($"Failed to load image \"{imageFile.Key}\" from mod archive \"{location.Name}\" with error: {ex.Message}");
                    }
                }

                var contents = zf.Entries.Where(x => !x.IsDirectory).Select(x => x.Key.Replace('/', '\\')).ToList();

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
    }
}