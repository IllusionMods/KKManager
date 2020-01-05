using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Sources
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

                str.Seek(0, SeekOrigin.Begin);
                try
                {
                    if (await _client.DownloadAsync(str, "Updates1.xml", 0, null, cancellationToken))
                    {
                        str.Seek(0, SeekOrigin.Begin);
                        updateInfos.AddRange(UpdateInfo.ParseUpdateManifest(str, _client.Host, 1));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                updateInfos.RemoveAll(info =>
                {
                    if (!info.CheckConditions())
                    {
                        Console.WriteLine("Skipping " + info.GUID + " because of conditions");
                        return true;
                    }
                    return false;
                });

                if (updateInfos.Any())
                {
                    _allNodes = _client.GetListing("/", FtpListOption.Recursive | FtpListOption.Size);

                    foreach (var updateInfo in updateInfos)
                    {
                        _latestModifiedDate = DateTime.MinValue;

                        var remote = GetNode(updateInfo.ServerPath);
                        if (remote == null) throw new DirectoryNotFoundException($"Could not find ServerPath: {updateInfo.ServerPath} in host: {_client.Host}");

                        var versionEqualsComparer = GetVersionEqualsComparer(updateInfo, remote);

                        var results = await ProcessDirectory(remote, updateInfo.ClientPathInfo,
                            updateInfo.Recursive, updateInfo.RemoveExtraClientFiles, versionEqualsComparer,
                            cancellationToken);

                        allResults.Add(new UpdateTask(updateInfo.Name ?? remote.Name, results, updateInfo, _latestModifiedDate));
                    }

                    // If a task is expanded by other tasks, remove the items that other tasks expand from it
                    foreach (var resultTask in allResults)
                    {
                        if (!string.IsNullOrEmpty(resultTask.Info.ExpandsGUID))
                        {
                            Console.WriteLine($"Expanding task {resultTask.Info.ExpandsGUID} with task {resultTask.Info.GUID}");
                            ApplyExtendedItems(resultTask.Info.ExpandsGUID, resultTask.Items, allResults);
                        }
                    }
                }
            }
            return allResults;
        }

        private static void ApplyExtendedItems(string targetGuid, List<IUpdateItem> itemsToReplace, List<UpdateTask> allResults)
        {
            foreach (var targetTask in allResults.Where(x => x.Info.GUID == targetGuid))
            {
                targetTask.Items.RemoveAll(x => itemsToReplace.Any(y => PathTools.PathsEqual(x.TargetPath, y.TargetPath)));

                if (!string.IsNullOrEmpty(targetTask.Info.ExpandsGUID))
                {
                    // Walk down the expanding stack
                    Console.WriteLine($"Also expanding task {targetTask.Info.ExpandsGUID} because it is expanded by {targetTask.Info.GUID}");
                    ApplyExtendedItems(targetTask.Info.ExpandsGUID, itemsToReplace, allResults);
                }
            }
        }

        private static Func<FtpListItem, FileInfo, bool> GetVersionEqualsComparer(UpdateInfo updateInfo, FtpListItem updateRoot)
        {
            switch (updateInfo.Versioning)
            {
                case UpdateInfo.VersioningMode.Size:
                    return (item, info) => item.Size == info.Length;
                case UpdateInfo.VersioningMode.Date:
                    return (item, info) => GetDate(item) <= info.LastWriteTimeUtc;
                case UpdateInfo.VersioningMode.Contents:
                    if (!updateInfo.ContentHashes.Any())
                    {
                        Console.WriteLine("No hashes found in " + updateInfo.GUID + " while VersioningMode was set to Contents, falling back to Size");
                        goto case UpdateInfo.VersioningMode.Size;
                    }

                    var rootLength = updateRoot.FullName.Length;
                    return (item, info) =>
                    {
                        var match = updateInfo.ContentHashes.FirstOrDefault(x => PathTools.PathsEqual(x.RelativeFileName, item.FullName.Substring(rootLength)));
                        if (match == null || match.Hash == 0)
                        {
                            Console.WriteLine($"No hash found on remote for file {item.FullName.Substring(rootLength)} - comparing size instead");
                            return item.Size == info.Length;
                        }
                        return FileContentsCalculator.GetFileHash(info) == match.Hash;
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

            var remoteDirName = PathTools.NormalizePath(remoteDir.FullName) + "/";
            var remoteDirDepth = remoteDirName.Count(c => c == '/' || c == '\\');

            return _allNodes.Where(
                item =>
                {
                    if (item == remoteDir) return false;
                    var itemFilename = PathTools.NormalizePath(item.FullName);
                    // Make sure it's inside the directory and not inside one of the subdirectories
                    return itemFilename.StartsWith(remoteDirName, StringComparison.OrdinalIgnoreCase) &&
                           itemFilename.Count(c => c == '/' || c == '\\') == remoteDirDepth;
                });
        }

        private async Task<List<IUpdateItem>> ProcessDirectory(FtpListItem remoteDir, DirectoryInfo localDir,
            bool recursive, bool removeNotExisting, Func<FtpListItem, FileInfo, bool> versionEqualsComparer,
            CancellationToken cancellationToken)
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

                        results.AddRange(await ProcessDirectory(remoteItem, localItem, recursive, removeNotExisting, versionEqualsComparer, cancellationToken));
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

                    var localIsUpToDate = localFile.Exists && versionEqualsComparer(remoteItem, localFile);
                    results.Add(new FtpUpdateItem(remoteItem, this, localFile, localIsUpToDate));
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Remove all files that were not on the remote
            if (removeNotExisting)
                results.AddRange(UpdateSourceManager.FileInfosToDeleteItems(localContents));

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

            public FtpUpdateItem(FtpListItem item, FtpUpdater source, FileSystemInfo targetPath, bool upToDate)
            {
                UpToDate = upToDate;
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
            public bool UpToDate { get; }

            public async Task Update(Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                await _source.UpdateItem(this, progressCallback, cancellationToken);
            }
        }
    }
}
