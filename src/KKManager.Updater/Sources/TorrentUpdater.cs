using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Updater.Downloader;
using MonoTorrent;
using MonoTorrent.Client;

namespace KKManager.Updater.Sources
{
    public static class TorrentUpdater
    {
        private static TorrentSource _source;

        public static async Task<UpdateTask> GetUpdateTask(Torrent torrent, UpdateInfo updateInfo, CancellationToken cancellationToken)
        {
            //todo reset the whole client at update start?
            if (_source == null) _source = new TorrentSource();

            var existing = _source.Client.Torrents.FirstOrDefault(x => x.Torrent.Equals(torrent));
            if (existing != null)
            {
                await existing.StopAsync();
                await _source.Client.RemoveAsync(existing, RemoveMode.KeepAllData);
            }

            var targetDir = updateInfo.ClientPathInfo;
            if (targetDir.Parent == null) throw new ArgumentException("targetDir.Parent == null");

            // Check if the torrent's root directory is inside targetDir, or if targetDir itself is the torrent's root directory in which case we need to save to directory that contains targetDir
            var adjustedTargetDir = torrent.Files.Any(x => x.Path.StartsWith(targetDir.Name)) ? targetDir.Parent.FullName : targetDir.FullName;
            
            var torrentManager = await _source.Client.AddAsync(torrent, adjustedTargetDir, new TorrentSettingsBuilder()
            {
                CreateContainingDirectory = false,
                AllowInitialSeeding = true
                //todo
            }.ToSettings());


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

            var remoteFiles = new List<UpdateItem>();
            foreach (var file in torrentManager.Files)
            {
                // Seed finished files, but do not download unfinished yet
                var isFinished = file.BitField.PercentComplete >= 100d;

                var info = new TorrentFileInfo(file, torrentManager, updateInfo.ClientPathInfo.FullName);
                var ui = new UpdateItem(new FileInfo(file.FullPath), info, isFinished, CustomMoveResult);
                remoteFiles.Add(ui);

                await torrentManager.SetFilePriorityAsync(file, isFinished ? Priority.Low : Priority.DoNotDownload);
                if (!isFinished)
                    await torrentManager.MoveFileAsync(file, (await UpdateItem.GetTempDownloadFilename()).FullName);
            }

            cancellationToken.ThrowIfCancellationRequested();

            //todo needs to be redone to not be in-place, or no other source can touch

            // Start seeding already
            await torrentManager.StartAsync();

            var updateInfoCopy = updateInfo.Copy(_source);
            //updateInfo.Source + " -> " + updateInfo.TorrentFileName
            updateInfoCopy.TorrentFileName = null;
            return new UpdateTask(updateInfo.Name, remoteFiles, updateInfoCopy, torrent.CreationDate);
        }

        private static bool CustomMoveResult(FileInfo currentpath, FileInfo targetpath, UpdateItem item)
        {
            ((TorrentFileInfo)item.RemoteFile).Move(targetpath);
            return true;
        }

        public class TorrentSource : UpdateSourceBase
        {
            public ClientEngine Client { get; }

            public TorrentSource(int discoveryPriority = -1, int downloadPriority = 69, bool handlesRetry = true) : base("p2p/torrent", discoveryPriority, downloadPriority, 5, handlesRetry)
            {
                Client = new ClientEngine(new EngineSettingsBuilder
                {
                    DhtPort = -1,
                    CacheDirectory = Path.Combine(InstallDirectoryHelper.TempDir.FullName, "KKManager_p2pcache"),
                    AllowPortForwarding = true,
                    AllowLocalPeerDiscovery = false,
                    AutoSaveLoadDhtCache = false,
                    //todo allow specifying port
                }.ToSettings());
            }
            public override void Dispose()
            {
                Client.Dispose();
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

            protected override IRemoteItem GetRemoteRootItem(string serverPath)
            {
                throw new NotSupportedException();
            }
        }

        public class TorrentFileInfo : IRemoteItem
        {
            private readonly TorrentManager _torrent;
            private readonly ITorrentFileInfo _info;
            //internal IRemoteItem[] Contents { get; set; }

            public TorrentFileInfo(ITorrentFileInfo torrentFileInfo, TorrentManager torrent, string rootDirectory)
            {
                _info = torrentFileInfo ?? throw new ArgumentNullException(nameof(torrentFileInfo));
                _torrent = torrent;
                var targetPath = torrentFileInfo.FullPath;
                Name = Path.GetFileName(targetPath);
                ItemSize = torrentFileInfo.Length;
                ModifiedTime = torrent.Torrent.CreationDate;
                IsDirectory = false;
                IsFile = true;
                ClientRelativeFileName = targetPath.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase) ? targetPath.Substring(rootDirectory.Length + 1) : targetPath;
            }

            public string Name { get; }
            public long ItemSize { get; }
            public DateTime ModifiedTime { get; }
            public bool IsDirectory { get; }
            public bool IsFile { get; }
            public string ClientRelativeFileName { get; }
            public UpdateSourceBase Source => _source;
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
                if (_torrent.State != TorrentState.Downloading)
                    await _torrent.StartAsync();
                while (PercentComplete < 100d)
                {
                    await Task.Delay(1000, CancellationToken.None);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Managed to finish in time
                        if (PercentComplete >= 100d) break;

                        await _torrent.SetFilePriorityAsync(_info, Priority.DoNotDownload);
                        throw new OperationCanceledException();
                    }

                    ((IProgress<double>)progressCallback).Report(PercentComplete);
                }
                await _torrent.SetFilePriorityAsync(_info, Priority.Low);
                ((IProgress<double>)progressCallback).Report(100d);
            }

            public void StartSeeding()
            {
                if (_info.Priority == Priority.DoNotDownload)
                {
                    var f = new FileInfo(_info.FullPath);
                    if (f.Exists && f.Length == _info.Length)
                        _torrent.SetFilePriorityAsync(_info, Priority.Low).Wait();
                    else
                        Console.WriteLine($"WARN: Tried to start seeding [{f.FullName}] but it looks like it {(f.Exists ? "doesn't exist" : $"has size {f.Length} instead of expected {_info.Length}")}!");
                }
            }

            public void Move(FileInfo targetpath)
            {
                _torrent.MoveFileAsync(_info, targetpath.FullName).Wait();
            }
        }

        internal static void OnFileUpdateFinished(UpdateDownloadItem updateItem)
        {
            foreach (var torrentFileInfo in updateItem.DownloadSources.Select(x => x.Value.RemoteFile).OfType<TorrentFileInfo>())
            {
                torrentFileInfo.StartSeeding();
            }
        }
    }
}
