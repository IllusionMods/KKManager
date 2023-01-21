using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Updater.Downloader;
using KKManager.Util;
using MonoTorrent;
using MonoTorrent.Client;

namespace KKManager.Updater.Sources
{
    public static class TorrentUpdater
    {
        private static ClientEngine _client;

        private static ClientEngine GetClient()
        {
            if (_client == null)
            {
                UpdateDownloadCoordinator.UpdateStatusChanged += UpdateStatusChanged;
            }
            if (_client == null || _client.Disposed)
            {
                const int clientPort = 15847;
                _client = new ClientEngine(new EngineSettingsBuilder
                {
                    DhtEndPoint = null,
                    CacheDirectory = Path.Combine(InstallDirectoryHelper.TempDir.FullName, "KKManager_p2pcache"),
                    AllowPortForwarding = true,
                    AllowLocalPeerDiscovery = false,
                    AutoSaveLoadDhtCache = false,
                    //todo allow specifying port
                    ListenEndPoint = new IPEndPoint(IPAddress.Any, clientPort),
                    //UsePartialFiles = true todo bugged in beta builds
                }.ToSettings());
            }
            return _client;
        }

        private static void DisposeClient()
        {
            if (_client == null || _client.Disposed) return;
            try { _client.StopAllAsync().Wait(60); } catch (Exception e) { Console.WriteLine(e); }
            _client.Dispose();
        }

        public static async Task<UpdateTask> GetUpdateTask(Torrent torrent, UpdateInfo updateInfo, CancellationToken cancellationToken)
        {
            if (torrent == null) throw new ArgumentNullException(nameof(torrent));
            if (updateInfo == null) throw new ArgumentNullException(nameof(updateInfo));

            var client = GetClient();

            var existing = client.Torrents.FirstOrDefault(x => torrent.Equals(x.Torrent));
            if (existing != null)
            {
                await existing.StopAsync();
                await client.RemoveAsync(existing, RemoveMode.KeepAllData);
            }

            var targetDir = updateInfo.ClientPathInfo;
            if (targetDir.Parent == null) throw new ArgumentException("targetDir.Parent == null");

            // Check if the torrent's root directory is inside targetDir, or if targetDir itself is the torrent's root directory in which case we need to save to directory that contains targetDir
            var adjustedTargetDir = torrent.Files.Any(x => x.Path.StartsWith(targetDir.Name)) ? targetDir.Parent.FullName : targetDir.FullName;

            var torrentManager = await client.AddAsync(torrent, adjustedTargetDir, new TorrentSettingsBuilder()
            {
                CreateContainingDirectory = false,
                AllowInitialSeeding = true,
                AllowDht = false,
                AllowPeerExchange = false,
            }.ToSettings());

            // Doesn't do anything on torrent files?
            await torrentManager.WaitForMetadataAsync(cancellationToken);

            var sw = Stopwatch.StartNew();

            if (torrentManager.State == TorrentState.Stopped)
            {
                Console.WriteLine($"Hash checking {torrent.Name}, this can take a while!");
                //todo fast resume?
                await torrentManager.HashCheckAsync(false);
                Console.WriteLine($"Hash checking {torrent.Name} finished in {sw.ElapsedMilliseconds}ms");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var newSource = new TorrentSource($"{updateInfo.Source.Origin}->{updateInfo.TorrentFileName}");

            var remoteFiles = new List<UpdateItem>();
            foreach (var file in torrentManager.Files)
            {
                // Seed finished files, but do not download unfinished yet
                var isFinished = file.BitField.PercentComplete >= 100d;

                //bug downloaded files get reported as not finished

                var info = new TorrentFileInfo(file, torrentManager, updateInfo.ClientPathInfo.FullName, newSource);
                var targetPath = new FileInfo(file.FullPath);
                var ui = new UpdateItem(targetPath, info, isFinished, CustomMoveResult); // (path, targetPath, item) => true);
                remoteFiles.Add(ui);

                await torrentManager.SetFilePriorityAsync(file, isFinished ? Priority.Low : Priority.DoNotDownload);
                if (!isFinished && targetPath.Exists && targetPath.Length == 0)
                    await torrentManager.MoveFileAsync(file, (await UpdateItem.GetTempDownloadFilename()).FullName);

                //Console.WriteLine($"{isFinished} - {file.DownloadIncompleteFullPath}  |  {file.DownloadCompleteFullPath}");
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (updateInfo.RemoveExtraClientFiles)
            {
                string NormalizePath(string x) => PathTools.NormalizePath(x).Replace('/', '\\').ToLowerInvariant();
                var allTorrentFiles = new HashSet<string>(torrentManager.Files.Select(x => NormalizePath(x.DownloadCompleteFullPath)));
                var localFiles = updateInfo.ClientPathInfo.GetFiles("*", SearchOption.AllDirectories);
                var toRemove = localFiles.Where(x => !allTorrentFiles.Contains(NormalizePath(x.FullName))).ToList();
                Debug.WriteLine($"{toRemove.Count} / {localFiles.Length} local files will be removed based on torrent ({torrentManager.Files.Count(x => x.Priority != Priority.DoNotDownload)} / {torrentManager.Files} already finished) from {updateInfo.GUID}");
                remoteFiles.AddRange(toRemove.Select(file => new UpdateItem(file, null, false)));
            }

            var updateInfoCopy = updateInfo.Copy(newSource);
            updateInfoCopy.TorrentFileName = null;
            return new UpdateTask(updateInfo.Name, remoteFiles, updateInfoCopy, torrent.CreationDate);
        }

        private static ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>> _filesToMove = new ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>>();
        private static bool CustomMoveResult(FileInfo currentpath, FileInfo targetpath, UpdateItem item)
        {
            _filesToMove.Add(new KeyValuePair<TorrentFileInfo, FileInfo>((TorrentFileInfo)item.RemoteFile, targetpath));
            return true;
        }
        private static void UpdateStatusChanged(object sender, UpdateDownloadCoordinator.UpdateStatusChangedEventArgs e)
        {
            //todo not called until after update starts, cancelling before breaks stuff
            switch (e.Status)
            {
                case UpdateDownloadCoordinator.UpdateStatus.Starting:
                    _filesToMove = new ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>>();
                    break;

                case UpdateDownloadCoordinator.UpdateStatus.Running:
                    if (_client != null && !_client.Disposed && _client.Torrents.Any(x => !x.Complete))
                        _client.StartAllAsync().Wait();
                    break;

                case UpdateDownloadCoordinator.UpdateStatus.Aborted:
                    //todo handle cancelling the update, unfinished files need to be cleaned up, but only if torrent was actually started
                    
                case UpdateDownloadCoordinator.UpdateStatus.Finished:
                    DisposeClient();
                    foreach (var pair in _filesToMove)
                    {
                        try
                        {
                            pair.Key.Move(pair.Value);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                    _filesToMove = new ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>>();
                    break;
            }
        }

        public class TorrentSource : UpdateSourceBase
        {
            public TorrentSource(string origin, int discoveryPriority = 99, int downloadPriority = 99, bool handlesRetry = true) : base($"p2p ({origin})", discoveryPriority, downloadPriority, 5, handlesRetry)
            {
            }

            public override void Dispose()
            {
            }

            public override Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken)
            {
                throw new NotSupportedException();
            }

            public override TimeSpan GetPing()
            {
                return TimeSpan.MinValue;
            }

            protected override Task<Stream> DownloadFileAsync(string updateFileName, CancellationToken cancellationToken)
            {
                throw new NotSupportedException();
            }

            protected override Task<IRemoteItem> GetRemoteRootItem(string serverPath, CancellationToken cancellationToken)
            {
                throw new NotSupportedException();
            }
        }

        public class TorrentFileInfo : IRemoteItem
        {
            private readonly TorrentManager _torrent;
            private readonly ITorrentManagerFile _info;
            //internal IRemoteItem[] Contents { get; set; }

            public TorrentFileInfo(ITorrentManagerFile torrentFileInfo, TorrentManager torrent, string rootDirectory, TorrentSource torrentSource)
            {
                if (rootDirectory == null) throw new ArgumentNullException(nameof(rootDirectory));
                _info = torrentFileInfo ?? throw new ArgumentNullException(nameof(torrentFileInfo));
                _torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
                if (torrent.Torrent == null) throw new ArgumentException("Only torrent files are supported, not magnets");
                Source = torrentSource ?? throw new ArgumentNullException(nameof(torrentSource));

                var targetPath = torrentFileInfo.FullPath;
                Name = Path.GetFileName(targetPath);
                ItemSize = torrentFileInfo.Length;
                ClientRelativeFileName = targetPath.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase) ? targetPath.Substring(rootDirectory.Length) : targetPath;

                ModifiedTime = torrent.Torrent.CreationDate;
                IsDirectory = false;
                IsFile = true;
            }

            public string Name { get; }
            public long ItemSize { get; }
            public DateTime ModifiedTime { get; }
            public bool IsDirectory { get; }
            public bool IsFile { get; }
            public string ClientRelativeFileName { get; }
            public UpdateSourceBase Source { get; }
            /// <summary>
            /// 0-100
            /// </summary>
            public double PercentComplete => _info.BitField.PercentComplete;

            public IRemoteItem[] GetDirectoryContents(CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public async Task Download(FileInfo downloadTarget, Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                await _torrent.SetFilePriorityAsync(_info, Priority.High);
                //if (_torrent.State == TorrentState.Stopped)
                //{
                //    try { await _torrent.StartAsync(); } catch (TorrentException) { }
                //}

                //await _torrent.StopAsync();
                //await _torrent.MoveFileAsync(_info, downloadTarget.FullName);
                //await _torrent.StartAsync();

                //if (_torrent.Monitor.DownloadRate == 0 && _torrent.State != TorrentState.Starting && _torrent.State != TorrentState.Stopping)

                while (PercentComplete < 100d)
                {
                    await Task.Delay(1000, CancellationToken.None);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Managed to finish in time
                        if (PercentComplete >= 100d) break;

                        //todo this will keep handle open
                        await _torrent.SetFilePriorityAsync(_info, Priority.DoNotDownload);
                        throw new OperationCanceledException();
                    }

                    ((IProgress<double>)progressCallback).Report(PercentComplete);
                }
                await _torrent.SetFilePriorityAsync(_info, Priority.Low);
                ((IProgress<double>)progressCallback).Report(100d);
            }

            //public void StartSeeding()
            //{
            //    if (_info.Priority == Priority.DoNotDownload)
            //    {
            //        //bug needs its path to be moved to actual output
            //        var f = new FileInfo(_info.FullPath);
            //        if (f.Exists && f.Length == _info.Length)
            //            _torrent.SetFilePriorityAsync(_info, Priority.Low).Wait();
            //        else
            //            Console.WriteLine($"WARN: Tried to start seeding [{f.FullName}] but it looks like it {(f.Exists ? "doesn't exist" : $"has size {f.Length} instead of expected {_info.Length}")}!");
            //    }
            //}

            public void Move(FileInfo targetpath)
            {
                if (!PathTools.PathsEqual(_info.FullPath, targetpath.FullName))
                {
                    File.Move(_info.FullPath, targetpath.FullName);
                }
                //_torrent.MoveFileAsync(_info, targetpath.FullName).Wait();
            }
        }

        //internal static void OnFileUpdateFinished(UpdateDownloadItem updateItem)
        //{
        //    foreach (var torrentFileInfo in updateItem.DownloadSources.Select(x => x.Value.RemoteFile).OfType<TorrentFileInfo>())
        //    {
        //        torrentFileInfo.StartSeeding();
        //    }
        //}
    }
}
