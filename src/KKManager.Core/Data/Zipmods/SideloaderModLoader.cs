using System;
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

namespace KKManager.Data.Zipmods
{
    public static class SideloaderModLoader
    {
        public static IObservable<SideloaderModInfo> Zipmods
        {
            get => _zipmods ?? StartReload();
        }

        private static bool _isUpdating = false;
        private static readonly object _lock = new object();
        private static ReplaySubject<SideloaderModInfo> _zipmods;

        public static IObservable<SideloaderModInfo> StartReload()
        {
            var needsUpdate = false;
            lock (_lock)
            {
                if (_zipmods == null || !_isUpdating)
                {
                    _zipmods = new ReplaySubject<SideloaderModInfo>();
                    _isUpdating = true;
                    needsUpdate = true;
                }
            }
            if (needsUpdate) TryReadSideloaderMods(InstallDirectoryHelper.ModsPath.FullName, _zipmods);
            return _zipmods;
        }

        private static CancellationTokenSource _cancelSource;
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
        private static void TryReadSideloaderMods(string modDirectory, ReplaySubject<SideloaderModInfo> subject, SearchOption searchOption = SearchOption.AllDirectories)
        {
            Console.WriteLine("Start loading zipmods");

            _cancelSource?.Dispose();
            _cancelSource = new CancellationTokenSource();
            var token = _cancelSource.Token;

            void ReadSideloaderModsAsync()
            {
                try
                {
                    if (!Directory.Exists(modDirectory))
                    {
                        subject.OnCompleted();
                        _isUpdating = false;
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
                    _isUpdating = false;
                    Console.WriteLine("Finished loading zipmods");
                    subject.OnCompleted();
                }
            }

            try
            {
                Task.Run(ReadSideloaderModsAsync, token);
            }
            catch (OperationCanceledException)
            {
                _isUpdating = false;
            }
            catch
            {
                _isUpdating = false;
                throw;
            }
        }

        public static SideloaderModInfo LoadFromFile(string filename)
        {
            var location = new FileInfo(filename);

            if (!IsValidZipmodExtension(location.Extension))
                throw new ArgumentException($"The file {filename} has an invalid extension and can't be a zipmod", nameof(filename));

            using (var zf = SharpCompress.Archives.ArchiveFactory.Open(location))
            {
                var manifestEntry = zf.Entries.FirstOrDefault(x => PathTools.PathsEqual(x.Key, "manifest.xml"));

                if (manifestEntry == null)
                    throw new InvalidDataException("manifest.xml was not found in the mod archive. Make sure this is a zipmod.");

                using (var fileStream = manifestEntry.OpenEntryStream())
                {
                    var manifest = XDocument.Load(fileStream, LoadOptions.None);

                    if (manifest.Root?.Element("guid")?.IsEmpty != false)
                        throw new InvalidDataException("The manifest.xml file is in an invalid format");

                    var guid = manifest.Root.Element("guid")?.Value;
                    var version = manifest.Root.Element("version")?.Value;
                    var name = manifest.Root.Element("name")?.Value ?? location.Name;
                    var author = manifest.Root.Element("author")?.Value;
                    var description = manifest.Root.Element("description")?.Value;
                    var website = manifest.Root.Element("website")?.Value;

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

                    return new SideloaderModInfo(location, guid, name, version, author, description, website, images, contents);
                }
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