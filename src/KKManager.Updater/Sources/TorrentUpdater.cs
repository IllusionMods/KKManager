using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Functions;
using KKManager.Updater.Data;
using KKManager.Updater.Utils;
using KKManager.Util;
using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Logging;

namespace KKManager.Updater.Sources
{
    public static class TorrentUpdater
    {
        public static async Task Test()
        {
#if DEBUG
            return;
            // var tc = new TorrentCreator(TorrentType.V1V2Hybrid);
            // tc.Announces.Add(new List<string>{"udp://127.1.1.1:11/announce"});
            // tc.Announces.Add(new List<string>{"http://127.1.1.1:11/announce"});
            // tc.Create(new TorrentFileSource(@"E:\nobeta"), "e:\\out.torrent");
            // 
            // var t = Torrent.Load("e:\\out.torrent");
            // 
            // Debugger.Break();

            LoggerFactory.Register(className => new TextLogger(Console.Out, className));

            var client = new ClientEngine(new EngineSettingsBuilder
            {
                DhtEndPoint = null,
                CacheDirectory = @"E:\p2pcache",
                AllowPortForwarding = true,
                AllowLocalPeerDiscovery = false,
                AutoSaveLoadDhtCache = false,
                ListenEndPoints = new Dictionary<string, IPEndPoint> { { "ipv4", new IPEndPoint(IPAddress.Any, 15847) } }
            }.ToSettings());

            var torrentManager = await client.AddAsync(@"E:\Torrents\[betterrepack.com] Sideloader Modpack - BR Community UserData Pack KK.torrent", @"e:\test", new TorrentSettingsBuilder
            {
                CreateContainingDirectory = false,
                AllowInitialSeeding = true,
                AllowDht = false,
                AllowPeerExchange = true
            }.ToSettings());

            client.AddDebugLogging();

            await torrentManager.WaitForMetadataAsync();
            await torrentManager.HashCheckAsync(false);

            var trackerUri = new Uri(torrentManager.Torrent.AnnounceUrls.First().First());
            var seedUri = new Uri("http://" + trackerUri.Host + ":" + 15847);
            await torrentManager.AddPeerAsync(new PeerInfo(seedUri, BEncodedString.Empty, true));

            await client.StartAllAsync();

            while (!torrentManager.Complete)
            {
                await Task.Delay(1000);
                Debug.WriteLine($"{torrentManager.Bitfield.PercentComplete}% {torrentManager.Monitor.DataBytesReceived}Bps");
            }
#endif
        }

        private static ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>> _filesToMove = new ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>>();
        private static ClientEngine _client;

        private static async Task<ClientEngine> GetClient()
        {
#if DEBUG
            if (_client == null)
                LoggerFactory.Register(className => new TextLogger(Console.Out, className));
#endif
            if (_client == null || _client.Disposed)
            {
                _client = new ClientEngine(new EngineSettingsBuilder
                {
                    DhtEndPoint = null,
                    CacheDirectory = Path.Combine(InstallDirectoryHelper.TempDir.FullName, "KKManager_p2pcache"),
                    AllowPortForwarding = KKManager.Properties.Settings.Default.P2P_PortForward,
                    AllowLocalPeerDiscovery = false,
                    ListenEndPoints = new Dictionary<string, IPEndPoint> { { "ipv4", new IPEndPoint(IPAddress.Any, KKManager.Properties.Settings.Default.P2P_Port) } },
                    MaximumOpenFiles = 900000,
                    MaximumHalfOpenConnections = 15,
                    MaximumConnections = 200,
                    //UsePartialFiles = true todo bugged in beta builds
                }.ToSettings());

                _client.AddDebugLogging();

                if (IPAddress.TryParse((await new HttpClient().GetStringAsync("http://icanhazip.com")).Trim(), out var publicIp))
                {
                    var loopbackIp = IPAddress.Parse("127.0.0.1");
                    _client.ConnectionManager.BanPeer += (sender, eventArgs) =>
                    {
                        if (IPAddress.TryParse(eventArgs.Peer.ConnectionUri.Host, out var addr) && (addr.Equals(publicIp) || addr.Equals(loopbackIp)))
                        {
#if DEBUG
                            Console.WriteLine($"[P2P] Banning own IP to stop failed connections ({eventArgs.Peer.ConnectionUri})");
#endif
                            eventArgs.BanPeer = true;
                        }
                    };
                }
                else
                {
                    Console.WriteLine("[P2P] Failed to get the public IP address");
                }
            }

            return _client;
        }

        private static async Task DisposeClient()
        {
            if (_client == null || _client.Disposed) return;
            try
            {
                Console.WriteLine("[P2P] Stopping the client");

                await _client.StopAllAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _client.Dispose();
        }

        public static async Task<UpdateTask> GetUpdateTask(Torrent torrent, UpdateInfo updateInfo, CancellationToken cancellationToken)
        {
            if (torrent == null) throw new ArgumentNullException(nameof(torrent));
            if (updateInfo == null) throw new ArgumentNullException(nameof(updateInfo));
            if (!KKManager.Properties.Settings.Default.P2P_Enabled) throw new InvalidOperationException("P2P is disabled");

            var client = await GetClient();

            var existing = client.Torrents.FirstOrDefault(x => torrent.Equals(x.Torrent));
            if (existing != null)
            {
                await existing.StopAsync();
                await client.RemoveAsync(existing, RemoveMode.KeepAllData);
            }

            var targetDir = updateInfo.ClientPathInfo;
            if (targetDir.Parent == null) throw new ArgumentException("targetDir.Parent == null");

            // Check if the torrent's root directory is inside targetDir, or if targetDir itself is the torrent's root directory in which case we need to save to directory that contains targetDir
            var allFilesInsideSubdirectory = torrent.Files.All(x => x.Path.StartsWith(targetDir.Name + "\\", StringComparison.OrdinalIgnoreCase) || x.Path.StartsWith(targetDir.Name + "/", StringComparison.OrdinalIgnoreCase));
            var adjustedTargetDir = allFilesInsideSubdirectory ? targetDir.Parent.FullName : targetDir.FullName;

            var torrentManager = await client.AddAsync(torrent, adjustedTargetDir, new TorrentSettingsBuilder
            {
                CreateContainingDirectory = false,
                AllowInitialSeeding = true,
                AllowDht = false,
                AllowPeerExchange = true,
                MaximumConnections = 120,
                UploadSlots = 20
            }.ToSettings());

            torrentManager.AddDebugLogging();

            // Doesn't do anything on torrent files?
            await torrentManager.WaitForMetadataAsync(cancellationToken);

            var origin = $"{updateInfo.Source.Origin}->{updateInfo.TorrentFileName}";

            var sw = Stopwatch.StartNew();

            if (torrentManager.State == TorrentState.Stopped)
            {
                Console.WriteLine($"[P2P] Hash checking {origin}, this can take a while!");
                //todo fast resume?
                await torrentManager.HashCheckAsync(false);
                Console.WriteLine($"[P2P] Hash checking {origin} finished in {sw.ElapsedMilliseconds}ms");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var newSource = new TorrentSource(origin, Math.Max(1, Math.Min(80, torrentManager.Files.Count(x => x.BitField.PercentComplete < 100d))));

            var remoteFiles = new List<UpdateItem>();
            foreach (var file in torrentManager.Files)
            {
                // Seed finished files, but do not download unfinished yet
                var isFinished = file.BitField.PercentComplete >= 100d;

                var info = new TorrentFileInfo(file, torrentManager, updateInfo.ClientPathInfo.FullName, newSource);
                var targetPath = new FileInfo(file.FullPath);

                await torrentManager.SetFilePriorityAsync(file, isFinished ? Priority.Low : Priority.DoNotDownload);
                // If files don't exist, a 0 byte placeholder is created by HashCheckAsync, which messes things up later
                // Partially downloaded files aren't changed until the torrent is started
                // If a file is missing, other fully downloaded files can show as partially downloaded if they share chunks, so don't move those until we start
                if (!isFinished && targetPath.Exists && targetPath.Length == 0)
                {
                    Debug.WriteLine($"IncompletePath={file.DownloadIncompleteFullPath}  ->  CompletePath={file.DownloadCompleteFullPath}");
                    await torrentManager.MoveFileAsync(file, (await UpdateItem.GetTempDownloadFilename()).FullName);

                    if (torrentManager.State == TorrentState.Error)
                    {
                        await torrentManager.StopAsync();
                        await Task.Delay(200, cancellationToken);
                        await torrentManager.MoveFileAsync(file, (await UpdateItem.GetTempDownloadFilename()).FullName);
                        if (torrentManager.State == TorrentState.Error)
                            throw new IOException("Failed to move file " + file.FullPath, torrentManager.Error?.Exception);
                    }
                }
                var updateItem = new UpdateItem(targetPath, info, isFinished, CustomMoveResult);
                remoteFiles.Add(updateItem);
            }

            if (torrentManager.State == TorrentState.Error)
                throw torrentManager.Error?.Exception ?? new IOException("Failed to create torrent " + torrentManager.Error?.Reason);

            cancellationToken.ThrowIfCancellationRequested();

            //foreach (var torrentTracker in torrentManager.TrackerManager.Tiers.SelectMany(x => x.Trackers))
            //    await torrentManager.TrackerManager.RemoveTrackerAsync(torrentTracker);

            var trackerUri = new Uri(torrent.AnnounceUrls.First().First());
            var seedUri = new Uri($"ipv4://{trackerUri.Host}:{15847}");
            await torrentManager.AddPeerAsync(new PeerInfo(seedUri, BEncodedString.Empty, true));

            if (updateInfo.RemoveExtraClientFiles && updateInfo.ClientPathInfo.Exists)
            {
                string NormalizePath(string x)
                {
                    return PathTools.NormalizePath(x).Replace('/', '\\').ToLowerInvariant();
                }

                var allTorrentFiles = new HashSet<string>(torrentManager.Files.Select(x => NormalizePath(x.DownloadCompleteFullPath)));
                var localFiles = updateInfo.ClientPathInfo.GetFiles("*", SearchOption.AllDirectories);
                var toRemove = localFiles.Where(x => !allTorrentFiles.Contains(NormalizePath(x.FullName))).ToList();
                Debug.WriteLine($"{toRemove.Count} / {localFiles.Length} local files will be removed based on torrent ({torrentManager.Files.Count(x => x.Priority != Priority.DoNotDownload)} / {torrentManager.Files.Count} already finished) from {updateInfo.GUID}");
                remoteFiles.AddRange(toRemove.Select(file => new UpdateItem(file, null, false)));
            }

            var updateInfoCopy = updateInfo.Copy(newSource);
            updateInfoCopy.TorrentFileName = null;
            return new UpdateTask(updateInfo.Name, remoteFiles, updateInfoCopy, torrent.CreationDate);
        }

        private static bool CustomMoveResult(FileInfo currentpath, FileInfo targetpath, UpdateItem item)
        {
            //((TorrentFileInfo)item.RemoteFile).Move(targetpath);
            _filesToMove.Add(new KeyValuePair<TorrentFileInfo, FileInfo>((TorrentFileInfo)item.RemoteFile, targetpath));
            return true;
        }

        public class TorrentSource : UpdateSourceBase
        {
            public TorrentSource(string origin, int maxConcurrentDownloads, int discoveryPriority = 99, int downloadPriority = 99, bool handlesRetry = true) : base(
                $"p2p({origin})", discoveryPriority, downloadPriority, maxConcurrentDownloads, handlesRetry)
            { }

            public override void Dispose() { }

            public override Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken, bool onlyDiscover, IProgress<float> progressCallback)
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
            private readonly ITorrentManagerFile _info;

            private readonly TorrentManager _torrent;
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

            /// <summary>
            /// 0-100
            /// </summary>
            public double PercentComplete => _info.BitField.PercentComplete;

            public string Name { get; }
            public long ItemSize { get; }
            public DateTime ModifiedTime { get; }
            public bool IsDirectory { get; }
            public bool IsFile { get; }
            public string ClientRelativeFileName { get; }
            public UpdateSourceBase Source { get; }

            public IRemoteItem[] GetDirectoryContents(CancellationToken cancellationToken)
            {
                throw new NotSupportedException();
            }

            public async Task Download(FileInfo downloadTarget, Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                await _torrent.SetFilePriorityAsync(_info, Priority.High);
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

                await _torrent.SetFilePriorityAsync(_info, Priority.Normal);
                ((IProgress<double>)progressCallback).Report(100d);
            }

            public void Move(FileInfo targetpath)
            {
                //todo do this in a cleaner way
                if (PercentComplete >= 100 && !PathTools.PathsEqual(_info.DownloadCompleteFullPath, targetpath.FullName))
                {
                    Console.WriteLine($"[P2P] Moving finished torrent download [{_info.FullPath}] to [{targetpath.FullName}]{(targetpath.Exists ? " (overwrite existing)" : "")}");

                    //if (File.Exists(_info.FullPath)) 
                    File.Move(_info.FullPath, targetpath.FullName);
                }
            }
        }

        public static async Task Stop()
        {
            await DisposeClient();

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
        }

        public static async Task Start()
        {
            // If null, there's nothing to seed
            if (_client == null || _client.IsRunning)
                return;

            if (_client.Disposed)
                throw new InvalidOperationException("Client is disposed");

            _filesToMove = new ConcurrentBag<KeyValuePair<TorrentFileInfo, FileInfo>>();

            var unfinishedCount = _client.Torrents.Count(x => !x.Complete);

            Console.WriteLine($"[P2P] Starting the client with {_client.Torrents.Count} torrents ({unfinishedCount} unfinished)");

            if (unfinishedCount > 0)
            {
                foreach (var torrent in _client.Torrents)
                {
                    foreach (var file in torrent.Files)
                    {
                        // Seed finished files, but do not download unfinished yet
                        var isFinished = file.BitField.PercentComplete >= 100d;

                        var targetPath = new FileInfo(file.FullPath);
                        // If files don't exist, a 0 byte placeholder is created by HashCheckAsync, which messes things up later
                        // Partially downloaded files aren't changed until the torrent is started
                        // If a file is missing, other fully downloaded files can show as partially downloaded if they share chunks, so don't move those until we start
                        if (!isFinished && targetPath.Exists && targetPath.Length > 0)
                        {
                            Debug.WriteLine($"IncompletePath={file.DownloadIncompleteFullPath}  ->  CompletePath={file.DownloadCompleteFullPath}");
                            await torrent.MoveFileAsync(file, UpdateItem.GetTempDownloadFilename().Result.FullName);
                            if (torrent.State == TorrentState.Error)
                            {
                                await torrent.StopAsync();
                                await Task.Delay(200, CancellationToken.None);
                                await torrent.MoveFileAsync(file, (await UpdateItem.GetTempDownloadFilename()).FullName);
                                if (torrent.State == TorrentState.Error)
                                    throw new IOException("Failed to move file " + file.FullPath, torrent.Error?.Exception);
                            }
                        }
                    }
                }
            }

            await _client.StartAllAsync();
        }

        public static long? GetCurrentUpload()
        {
            if (_client == null || !_client.IsRunning)
                return null;

            return _client.TotalUploadRate;
        }

        public static int? GetPeerCount()
        {
            if (_client == null || !_client.IsRunning)
                return null;

            return _client.ConnectionManager.OpenConnections;
        }
    }
}
