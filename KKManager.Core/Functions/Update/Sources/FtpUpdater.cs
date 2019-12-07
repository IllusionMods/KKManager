using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using KKManager.Util;

namespace KKManager.Functions.Update
{
    public class FtpUpdater : IUpdateSource
    {
        private readonly FtpClient _client;

        private FtpListItem[] _allNodes;
        private DateTime _latestModifiedDate = DateTime.MinValue;

        public FtpUpdater(Uri serverUri, NetworkCredential credentials = null)
        {
            if (serverUri == null) throw new ArgumentNullException(nameof(serverUri));

            if (credentials == null)
            {
                var info = serverUri.UserInfo.Split(new[] { ':' }, 2, StringSplitOptions.None);
                if (info.Length == 2)
                    credentials = new NetworkCredential(info[0], info[1]);
            }

            if (serverUri.IsDefaultPort)
                _client = new FtpClient(serverUri.Host, credentials);
            else
                _client = new FtpClient(serverUri.Host, serverUri.Port, credentials);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken)
        {
            await Connect();

            var allResults = new List<UpdateTask>();
            using (var str = new MemoryStream())
            {
                var b = await _client.DownloadAsync(str, UpdateInfo.UpdateFileName, 0, null, cancellationToken);
                if (!b) throw new FileNotFoundException($"Failed to get the update list - {UpdateInfo.UpdateFileName} is missing in host: {_client.Host}");

                str.Seek(0, SeekOrigin.Begin);
                var updateInfos = UpdateInfo.ParseUpdateManifest(str, _client.Host, 1).ToList();

                if (updateInfos.Any())
                {
                    _allNodes = _client.GetListing("/", FtpListOption.Recursive | FtpListOption.Size);

                    foreach (var updateInfo in updateInfos)
                    {
                        var remote = GetNode(updateInfo.ServerPath);
                        if (remote == null) throw new DirectoryNotFoundException($"Could not find ServerPath: {updateInfo.ServerPath} in host: {_client.Host}");

                        var results = await ProcessDirectory(remote, updateInfo.ClientPath, updateInfo.Recursive, updateInfo.RemoveExtraClientFiles, cancellationToken);

                        allResults.Add(new UpdateTask(updateInfo.Name ?? remote.Name, results, updateInfo, _latestModifiedDate));
                    }
                }
            }
            return allResults;
        }

        private async Task Connect()
        {
            if (!_client.IsConnected)
            {
                await _client.AutoConnectAsync();
                if (_client.ServerType == FtpServer.VsFTPd)
                    _client.RecursiveList = true;
            }
        }

        private static DateTime GetDate(FtpListItem ftpListItem)
        {
            if (ftpListItem == null) throw new ArgumentNullException(nameof(ftpListItem));
            return ftpListItem.Modified != DateTime.MinValue ? ftpListItem.Modified : ftpListItem.Created;
        }

        private FtpListItem GetNode(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return _allNodes.FirstOrDefault(item => PathTools.PathsEqual(item.FullName, path));
        }

        private IEnumerable<FtpListItem> GetSubNodes(FtpListItem remoteDir)
        {
            if (remoteDir == null) throw new ArgumentNullException(nameof(remoteDir));
            if (remoteDir.Type != FtpFileSystemObjectType.Directory) throw new ArgumentException("remoteDir has to be a directory");

            var remoteDirName = PathTools.SanitizeFileName(remoteDir.FullName);
            var remoteDirDepth = remoteDirName.Count(c => c == '/' || c == '\\');

            return _allNodes.Where(
                item =>
                {
                    if (item == remoteDir) return false;
                    var itemFilename = PathTools.SanitizeFileName(item.FullName);
                    // Make sure it's inside the directory and not inside one of the subdirectories
                    return itemFilename.StartsWith(remoteDirName, StringComparison.OrdinalIgnoreCase) &&
                           itemFilename.Count(c => c == '/' || c == '\\') == remoteDirDepth;
                });
        }

        private async Task<List<IUpdateItem>> ProcessDirectory(FtpListItem remoteDir, DirectoryInfo localDir, bool recursive, bool removeNotExisting, CancellationToken cancellationToken)
        {
            if (remoteDir.Type != FtpFileSystemObjectType.Directory) throw new DirectoryNotFoundException();

            var results = new List<IUpdateItem>();

            var localContents = new List<FileSystemInfo>();
            if (localDir.Exists)
                localContents.AddRange(localDir.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly));

            foreach (var remoteItem in GetSubNodes(remoteDir))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (remoteItem.Type == FtpFileSystemObjectType.Directory)
                {
                    if (recursive)
                    {
                        var localItem = localContents.OfType<DirectoryInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                        if (localItem == null)
                            localItem = new DirectoryInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                        else
                            localContents.Remove(localItem);

                        results.AddRange(await ProcessDirectory(remoteItem, localItem, recursive, removeNotExisting, cancellationToken));
                    }
                }
                else if (remoteItem.Type == FtpFileSystemObjectType.File)
                {
                    var itemDate = GetDate(remoteItem);
                    if (itemDate > _latestModifiedDate) _latestModifiedDate = itemDate;

                    var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                    if (localFile == null)
                        localFile = new FileInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                    else
                        localContents.Remove(localFile);

                    var localIsUpToDate = localFile.Exists && localFile.Length == remoteItem.Size;
                    if (!localIsUpToDate)
                        results.Add(new FtpUpdateItem(remoteItem, this, localFile));
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (removeNotExisting)
            {
                // Remove all files that were not on the remote //todo not only zip
                foreach (var localItem in localContents)
                {
                    if (!localItem.Exists) continue;

                    switch (localItem)
                    {
                        case FileInfo fi:
                            if (fi.Extension.ToLowerInvariant().StartsWith(".zip"))
                                results.Add(new DeleteFileUpdateItem(fi));
                            break;

                        case DirectoryInfo di:
                            foreach (var subFi in di.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Extension.ToLowerInvariant().StartsWith(".zip")))
                                results.Add(new DeleteFileUpdateItem(subFi));
                            break;
                    }
                }
            }

            return results;
        }

        private async Task UpdateItem(FtpUpdateItem item, IProgress<double> progressCallback, CancellationToken cancellationToken)
        {
            await Connect();

            await _client.DownloadFileAsync(
                item.TargetPath.FullName, item.SourceItem.FullName,
                FtpLocalExists.Overwrite, FtpVerify.Retry | FtpVerify.Delete | FtpVerify.Throw,
                new Progress<FtpProgress>(progress => progressCallback.Report(progress.Progress)),
                cancellationToken);
        }

        public sealed class FtpUpdateItem : IUpdateItem
        {
            private readonly FtpUpdater _source;

            public FtpUpdateItem(FtpListItem item, FtpUpdater source, FileSystemInfo targetPath)
            {
                TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
                SourceItem = item ?? throw new ArgumentNullException(nameof(item));
                _source = source ?? throw new ArgumentNullException(nameof(source));
                ItemSize = FileSize.FromBytes(item.Size);
                ModifiedTime = GetDate(SourceItem);
            }

            public FtpListItem SourceItem { get; }
            public FileSize ItemSize { get; }
            public DateTime? ModifiedTime { get; }
            public FileSystemInfo TargetPath { get; }

            public async Task Update(Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                await _source.UpdateItem(this, progressCallback, cancellationToken);
            }
        }
    }
}
