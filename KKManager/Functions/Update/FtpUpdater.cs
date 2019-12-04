using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace KKManager.Functions.Update {
    internal class FtpUpdater : IUpdateSource
    {
        public sealed class FtpUpdateItem : IUpdateItem
        {
            public FtpListItem SourceItem { get; }
            private readonly FtpUpdater _source;
            public FileSystemInfo TargetPath { get; }

            public FtpUpdateItem(FtpListItem item, FtpUpdater source, FileSystemInfo targetPath)
            {
                TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
                SourceItem = item ?? throw new ArgumentNullException(nameof(item));
                _source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public async Task Update(CancellationToken cancellationToken)
            {
                await _source.UpdateItem(this, cancellationToken);
            }
        }

        private readonly FtpClient _client;

        public FtpUpdater(Uri serverUri, NetworkCredential credentials)
        {
            if (serverUri == null) throw new ArgumentNullException(nameof(serverUri));

            if (serverUri.IsDefaultPort)
                _client = new FtpClient(serverUri.Host, credentials);
            else
                _client = new FtpClient(serverUri.Host, serverUri.Port, credentials);
        }

        private async Task Connect()
        {
            if (!_client.IsConnected)
                await _client.AutoConnectAsync();
        }

        private async Task UpdateItem(FtpUpdateItem item, CancellationToken cancellationToken)
        {
            await Connect();

            await _client.DownloadFileAsync(item.TargetPath.FullName, item.SourceItem.FullName, FtpLocalExists.Overwrite, FtpVerify.Retry | FtpVerify.Delete | FtpVerify.Throw, null, cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private async Task<List<IUpdateItem>> ProcessDirectory(FtpListItem remoteDir, DirectoryInfo localDir, bool recursive, bool removeNotExisting, CancellationToken cancellationToken)
        {
            if (remoteDir.Type != FtpFileSystemObjectType.Directory) throw new DirectoryNotFoundException();

            var results = new List<IUpdateItem>();

            var localContents = new List<FileSystemInfo>();
            if (localDir.Exists)
                localContents.AddRange(localDir.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly));

            foreach (var remoteItem in await _client.GetListingAsync(remoteDir.FullName, FtpListOption.SizeModify | FtpListOption.DerefLinks, cancellationToken))
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
                else
                {
                    var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                    if (localFile == null)
                        localFile = new FileInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                    else
                        localContents.Remove(localFile);

                    var localIsUpToDate = localFile.Length != remoteItem.Size;
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

        public async Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken)
        {
            var allResults = new List<UpdateTask>();
            using (var str = new MemoryStream())
            {
                var b = await _client.DownloadAsync(str, UpdateInfo.UpdateFileName, 0, null, cancellationToken);
                if (!b) throw new FileNotFoundException("Failed to get the update list");

                str.Seek(0, SeekOrigin.Begin);
                foreach (var updateInfo in UpdateInfo.ParseUpdateManifest(str))
                {
                    var remote = await _client.GetObjectInfoAsync(updateInfo.ServerPath);
                    if (remote == null) throw new ArgumentNullException(nameof(remote));

                    var results = await ProcessDirectory(remote, updateInfo.ClientPath, updateInfo.Recursive, updateInfo.RemoveExtraClientFiles, cancellationToken);

                    // todo change local.Exists to something else? allow per-file?
                    allResults.Add(new UpdateTask(remote.Name, results, updateInfo.ClientPath.Exists));
                }
            }
            return allResults;
        }
    }
}