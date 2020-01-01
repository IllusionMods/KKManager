using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Sources
{
    public class ZipUpdater : IUpdateSource
    {
        private readonly ZipFile _zipfile;

        public ZipUpdater(FileInfo archive)
        {
            _zipfile = ZipFile.Read(archive.FullName);
            _zipfile.FlattenFoldersOnExtract = true;
        }

        public void Dispose()
        {
            _zipfile.Dispose();
        }

        public async Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken)
        {
            var allResults = new List<UpdateTask>();

            var manifestFile = _zipfile.Entries.FirstOrDefault(entry => string.Equals(Path.GetFileName(entry.FileName), UpdateInfo.UpdateFileName, StringComparison.OrdinalIgnoreCase));
            if (manifestFile == null) throw new FileNotFoundException($"Failed to get the update list - {UpdateInfo.UpdateFileName} is missing in host: {_zipfile.Name}");
            using (var str = manifestFile.OpenReader())
            {
                foreach (var updateInfo in UpdateInfo.ParseUpdateManifest(str, _zipfile.Name, 100))
                {
                    _latestModifiedDate = DateTime.MinValue;

                    // Clean up the path into a usable form
                    var serverPath = updateInfo.ServerPath.Trim(' ', '\\', '/').Replace('\\', '/') + "/";

                    var remote = _zipfile.Entries.FirstOrDefault(entry => string.Equals(entry.FileName, serverPath, StringComparison.OrdinalIgnoreCase));
                    if (remote == null) throw new DirectoryNotFoundException($"Could not find ServerPath: {updateInfo.ServerPath} in host: {_zipfile.Name}");

                    var versionEqualsComparer = GetVersionEqualsComparer(updateInfo);

                    var results = await ProcessDirectory(remote, updateInfo.ClientPathInfo,
                        updateInfo.Recursive, updateInfo.RemoveExtraClientFiles, versionEqualsComparer,
                        cancellationToken);

                    allResults.Add(new UpdateTask(updateInfo.Name ?? Path.GetFileName(remote.FileName.Trim(' ', '\\', '/')), results, updateInfo, _latestModifiedDate));
                }
            }
            return allResults;
        }

        private static Func<ZipEntry, FileInfo, bool> GetVersionEqualsComparer(UpdateInfo updateInfo)
        {
            switch (updateInfo.Versioning)
            {
                case UpdateInfo.VersioningMode.Size:
                    return (item, info) => item.UncompressedSize == info.Length;
                case UpdateInfo.VersioningMode.Date:
                    return (item, info) => item.LastModified <= info.LastWriteTimeUtc;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<ZipEntry> GetChildren(ZipEntry directory)
        {
            var dirDepth = directory.FileName.Count(c => c == '\\' || c == '/');
            return _zipfile.Entries.Where(
                x =>
                {
                    var fname = x.FileName.TrimEnd('\\', '/');
                    return fname.StartsWith(directory.FileName) && fname.Count(c => c == '\\' || c == '/') == dirDepth;
                });
        }

        private DateTime _latestModifiedDate = DateTime.MinValue;

        private async Task<List<IUpdateItem>> ProcessDirectory(ZipEntry remoteDir, DirectoryInfo localDir,
            bool recursive, bool removeNotExisting, Func<ZipEntry, FileInfo, bool> versionEqualsComparer,
            CancellationToken cancellationToken)
        {
            if (!remoteDir.IsDirectory) throw new DirectoryNotFoundException();

            var results = new List<IUpdateItem>();

            var localContents = new List<FileSystemInfo>();
            if (localDir.Exists)
                localContents.AddRange(localDir.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly));

            foreach (var remoteItem in GetChildren(remoteDir))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var remoteFilename = Path.GetFileName(remoteItem.FileName.TrimEnd('\\', '/'));

                if (remoteItem.IsDirectory)
                {
                    if (recursive)
                    {
                        var localItem = localContents.OfType<DirectoryInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteFilename, StringComparison.OrdinalIgnoreCase));
                        if (localItem == null)
                            localItem = new DirectoryInfo(Path.Combine(localDir.FullName, remoteFilename));
                        else
                            localContents.Remove(localItem);

                        results.AddRange(await ProcessDirectory(remoteItem, localItem, recursive, removeNotExisting, versionEqualsComparer, cancellationToken));
                    }
                }
                else
                {
                    var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteFilename, StringComparison.OrdinalIgnoreCase));
                    if (localFile == null)
                        localFile = new FileInfo(Path.Combine(localDir.FullName, remoteFilename));
                    else
                        localContents.Remove(localFile);

                    var localIsUpToDate = localFile.Exists && versionEqualsComparer(remoteItem, localFile);
                    if (!localIsUpToDate)
                        results.Add(new ZipUpdateItem(remoteItem, localFile, this));

                    if (_latestModifiedDate < remoteItem.LastModified)
                        _latestModifiedDate = remoteItem.LastModified;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Remove all files that were not on the remote
            if (removeNotExisting)
                results.AddRange(UpdateSourceManager.FileInfosToDeleteItems(localContents));

            return results;
        }

        private async Task UpdateItem(ZipUpdateItem item, IProgress<double> progressCallback, CancellationToken cancellationToken)
        {
            var localFile = (FileInfo)item.TargetPath;
            await Task.Run(
                () =>
                {
                    var localDir = localFile.DirectoryName;
                    var zipEntry = item.SourceItem;
                    var extractedFileName = Path.Combine(localDir, Path.GetFileName(zipEntry.FileName));

                    void OnExtractProgress(object sender, ExtractProgressEventArgs args)
                    {
                        if (args.CurrentEntry == zipEntry && args.TotalBytesToTransfer > 0)
                            progressCallback.Report((args.BytesTransferred / args.TotalBytesToTransfer) * 100);
                    }

                    try
                    {
                        _zipfile.ExtractProgress += OnExtractProgress;

                        zipEntry.Extract(localDir, ExtractExistingFileAction.OverwriteSilently);

                        // Check if the file needs to be renamed
                        if (!string.Equals(Path.GetFileName(zipEntry.FileName), localFile.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(localFile.FullName);
                            File.Move(extractedFileName, localFile.FullName);
                        }
                    }
                    catch (Exception e)
                    {
                        File.Delete(extractedFileName);
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        _zipfile.ExtractProgress -= OnExtractProgress;
                    }
                }, cancellationToken);
        }

        public sealed class ZipUpdateItem : IUpdateItem
        {
            private readonly ZipUpdater _owner;

            public ZipUpdateItem(ZipEntry item, FileSystemInfo targetPath, ZipUpdater owner)
            {
                _owner = owner;
                TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
                SourceItem = item ?? throw new ArgumentNullException(nameof(item));
                ItemSize = FileSize.FromBytes(item.UncompressedSize);
                ModifiedTime = SourceItem.LastModified;
            }

            public ZipEntry SourceItem { get; }
            public FileSize ItemSize { get; }
            public DateTime? ModifiedTime { get; }
            public bool UpToDate { get; }
            public FileSystemInfo TargetPath { get; }

            public async Task Update(Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                await _owner.UpdateItem(this, progressCallback, cancellationToken);
            }
        }
    }
}
