using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using FluentFTP.Helpers;
using FluentFTP.Proxy;
using KKManager.Properties;
using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Sources
{
    public class FtpUpdater : UpdateSourceBase
    {
        private readonly FtpClient _client;

        private Dictionary<string, FtpListItem> AllNodesLookup
        {
            get
            {
                if (_allNodesLookup == null)
                    PopulateNodeLookups(CancellationToken.None).Wait();
                return _allNodesLookup;
            }
        }

        public Dictionary<FtpListItem, string> AllNodesNameLookup
        {
            get
            {
                if (_allNodesNameLookup == null)
                    PopulateNodeLookups(CancellationToken.None).Wait();
                return _allNodesNameLookup;
            }
        }

        public ILookup<string, FtpListItem> ChildNodesLookup
        {
            get
            {
                if (_childNodesLookup == null)
                    PopulateNodeLookups(CancellationToken.None).Wait();
                return _childNodesLookup;
            }
        }

        private async Task PopulateNodeLookups(CancellationToken cancellationToken)
        {
            if (_childNodesLookup != null) return;

            var allNodes = await _client.GetListingAsync("/", FtpListOption.Recursive | FtpListOption.Size, cancellationToken);

            // Deal with case-insensitive servers having duplicate files with different cases
            var groups = allNodes.GroupBy(x => x.FullName, StringComparer.InvariantCultureIgnoreCase);
#if DEBUG
            foreach (var group in groups)
            {
                if (group.Count() > 1)
                    Console.WriteLine($"Multiple copies on [{Origin}]: {string.Join(" | ", group.Select(x => x.FullName))}");
            }
#endif
            _allNodesLookup = groups.Select(x => x.OrderByDescending(GetDate).First()).ToDictionary(
                item => GetNormalizedNodeName(item.FullName),
                item => item);
            _allNodesNameLookup = AllNodesLookup.ToDictionary(x => x.Value, x => x.Key);

            _childNodesLookup = AllNodesLookup.ToLookup(x =>
            {
                var normalizedNodeName = GetNormalizedNodeName(Path.GetDirectoryName(x.Key));
                Debug.Assert(x.Key.StartsWith(normalizedNodeName), "wtf " + normalizedNodeName + " - " + x.Key);
                return normalizedNodeName;
            }, x => x.Value);
        }
        private Dictionary<FtpListItem, string> _allNodesNameLookup;
        private ILookup<string, FtpListItem> _childNodesLookup;
        private Dictionary<string, FtpListItem> _allNodesLookup;

        public FtpUpdater(Uri serverUri, int discoveryPriority, int downloadPriority = 1, NetworkCredential credentials = null, int maxConcurrentDownloads = 1)
            : base(serverUri.Host, discoveryPriority, downloadPriority, maxConcurrentDownloads, true)
        {
            if (serverUri == null) throw new ArgumentNullException(nameof(serverUri));

            if (credentials == null)
            {
                var info = serverUri.UserInfo.Split(new[] { ':' }, 2, StringSplitOptions.None);
                if (info.Length == 2)
                    credentials = new NetworkCredential(info[0], info[1]);
            }

            if (Settings.Default.UseProxy && !System.Net.WebRequest.DefaultWebProxy.IsBypassed(serverUri))
            {
                var proxy = System.Net.WebRequest.DefaultWebProxy.GetProxy(serverUri);
                _client = new FtpClientHttp11Proxy(new ProxyInfo { Host = proxy.Host, Port = proxy.Port });
                _client.Host = serverUri.Host;
                _client.Credentials = credentials;
                if (!serverUri.IsDefaultPort)
                {
                    _client.Port = serverUri.Port;
                }
            }
            else
            {
                if (serverUri.IsDefaultPort)
                    _client = new FtpClient(serverUri.Host, credentials);
                else
                    _client = new FtpClient(serverUri.Host, serverUri.Port, credentials);
            }

            _client.EncryptionMode = FtpEncryptionMode.Explicit;
            _client.DataConnectionEncryption = true;
            _client.RetryAttempts = 3;
            _client.DownloadDataType = FtpDataType.Binary;
            _client.ListingDataType = FtpDataType.Binary;

            FtpTrace.EnableTracing = false;
        }

        public override void Dispose()
        {
            _client.Dispose();
        }

        public override async Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken, bool onlyDiscover, IProgress<float> progressCallback)
        {
            await Connect(cancellationToken);

            return await base.GetUpdateItems(cancellationToken, onlyDiscover, progressCallback);
        }

        private static string GetNormalizedNodeName(string itemFullName)
        {
            return PathTools.NormalizePath(itemFullName).Replace('\\', '/').TrimEnd('/').ToLowerInvariant();
        }

        protected override async Task<Stream> DownloadFileAsync(string updateFileName, CancellationToken cancellationToken)
        {
            //var item = GetRemoteItem(updateFileName);
            //if (item == null)
            //    throw new FileNotFoundException("File doesn't exist on host");

            cancellationToken.ThrowIfCancellationRequested();
            var str = new MemoryStream();
            if (await _client.DownloadAsync(str, updateFileName, 0, null, cancellationToken))
            {
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            else
            {
                // Cleanup if download fails
                str.Dispose();
                cancellationToken.ThrowIfCancellationRequested();
                throw new IOException("Failed to download file");
            }
        }

        protected override async Task<IRemoteItem> GetRemoteRootItem(string serverPath, CancellationToken cancellationToken)
        {
            if (serverPath == null) throw new ArgumentNullException(nameof(serverPath));

            await PopulateNodeLookups(cancellationToken);

            if (!AllNodesLookup.TryGetValue(GetNormalizedNodeName(serverPath), out var remote) || remote == null)
            {
                Debug.Fail("Could not find " + serverPath);
                return null;
            }

            var remoteItem = new FtpRemoteItem(remote, this, remote.FullName);
            return remoteItem;
        }

        private async Task Connect(CancellationToken cancellationToken)
        {
            if (!_client.IsConnected)
            {
                // Need to wrap the connect into a new task because it can block main thread when failing to connect
                await Task.Run(async () =>
                {
                    await _client.AutoConnectAsync(cancellationToken);
                }, cancellationToken);

                // todo hack, some servers don't announce the capability, needed for proper functionality
                _client.RecursiveList = true;
            }
        }

        private static DateTime GetDate(FtpListItem ftpListItem)
        {
            if (ftpListItem == null) throw new ArgumentNullException(nameof(ftpListItem));
            return ftpListItem.Modified != DateTime.MinValue ? ftpListItem.Modified : ftpListItem.Created;
        }

        private IEnumerable<FtpListItem> GetSubNodes(FtpListItem remoteDir)
        {
            if (remoteDir == null) throw new ArgumentNullException(nameof(remoteDir));
            if (remoteDir.Type != FtpFileSystemObjectType.Directory) throw new ArgumentException("remoteDir has to be a directory");

            var name = AllNodesNameLookup[remoteDir];
            return ChildNodesLookup[name];
        }

        private async Task UpdateItem(FtpListItem sourceItem, FileInfo targetPath, IProgress<double> progressCallback, CancellationToken cancellationToken)
        {
            // Delete old file if any exists so the download doesn't try to append to it. Append mode is needed for retrying downloads to resume instead of restarting
            targetPath.Delete();

            await Connect(cancellationToken);

            await _client.DownloadFileAsync(
                targetPath.FullName, sourceItem.FullName,
                FtpLocalExists.Resume, FtpVerify.Retry | FtpVerify.Delete | FtpVerify.Throw,
                new Progress<FtpProgress>(progress => progressCallback.Report(progress.Progress)),
                cancellationToken);
        }

        private sealed class FtpRemoteItem : IRemoteItem
        {
            private readonly string _rootFolder;

            public FtpRemoteItem(FtpListItem sourceItem, FtpUpdater source, string rootFolder)
            {
                if (sourceItem == null) throw new ArgumentNullException(nameof(sourceItem));
                if (source == null) throw new ArgumentNullException(nameof(source));

                if (rootFolder != null)
                {
                    _rootFolder = rootFolder;
                    if (!sourceItem.FullName.StartsWith(_rootFolder)) throw new IOException($"Remote item full path {sourceItem.FullName} doesn't start with the specified root path {_rootFolder}");
                    ClientRelativeFileName = sourceItem.FullName.Substring(_rootFolder.Length);
                }

                SourceItem = sourceItem;
                Source = source;
                ItemSize = SourceItem.Size;
                ModifiedTime = GetDate(SourceItem);
            }

            public string Name => SourceItem.Name;
            public long ItemSize { get; }
            public DateTime ModifiedTime { get; }
            public bool IsDirectory => SourceItem.Type == FtpFileSystemObjectType.Directory;
            public bool IsFile => SourceItem.Type == FtpFileSystemObjectType.File;
            public string ClientRelativeFileName { get; }

            public FtpUpdater Source { get; }
            public FtpListItem SourceItem { get; }
            UpdateSourceBase IRemoteItem.Source => Source;

            public IRemoteItem[] GetDirectoryContents(CancellationToken cancellationToken)
            {
                return Source.GetSubNodes(SourceItem).Select(x => (IRemoteItem)new FtpRemoteItem(x, Source, _rootFolder)).ToArray();
            }

            public async Task Download(FileInfo downloadTarget, Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                await Source.UpdateItem(SourceItem, downloadTarget, progressCallback, cancellationToken);
            }
        }
    }
}
