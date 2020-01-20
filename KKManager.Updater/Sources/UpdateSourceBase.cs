using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Sources
{
    public abstract class UpdateSourceBase : IDisposable
    {
        private DateTime _latestModifiedDate = DateTime.MinValue;

        protected UpdateSourceBase(string origin, int priority)
        {
            Origin = origin;
            Priority = priority;
        }

        protected string Origin { get; }
        protected int Priority { get; }

        public abstract void Dispose();

        public virtual async Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken)
        {
            var updateInfos = new List<UpdateInfo>();

            var filenamesToTry = new[] { UpdateInfo.UpdateFileName, "Updates1.xml", "Updates2.xml" };

            foreach (var fn in filenamesToTry)
            {
                try
                {
                    using (var str = await DownloadFileAsync(fn, cancellationToken))
                    {
                        if (str != null)
                            updateInfos.AddRange(UpdateInfo.ParseUpdateManifest(str, Origin, Priority));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (updateInfos.Count == 0)
                throw new FileNotFoundException($"Failed to get update list from host {Origin} - check log for details.");

            updateInfos.RemoveAll(
                info =>
                {
                    if (!info.CheckConditions())
                    {
                        Console.WriteLine($"Skipping {info.GUID} because of conditions");
                        return true;
                    }
                    return false;
                });

            var allResults = new List<UpdateTask>();
            if (updateInfos.Any())
            {
                foreach (var updateInfo in updateInfos)
                {
                    _latestModifiedDate = DateTime.MinValue;
                    var remoteItem = GetRemoteRootItem(updateInfo.ServerPath);
                    if (remoteItem == null) throw new DirectoryNotFoundException($"Could not find ServerPath: {updateInfo.ServerPath} in host: {Origin}");

                    var versionEqualsComparer = GetVersionEqualsComparer(updateInfo);

                    await Task.Run(
                        () =>
                        {
                            var results = ProcessDirectory(
                                remoteItem, updateInfo.ClientPathInfo,
                                updateInfo.Recursive, updateInfo.RemoveExtraClientFiles, versionEqualsComparer,
                                cancellationToken);

                            allResults.Add(new UpdateTask(updateInfo.Name ?? remoteItem.Name, results, updateInfo, _latestModifiedDate));
                        }, cancellationToken);
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
            return allResults;
        }

        protected abstract Task<Stream> DownloadFileAsync(string updateFileName, CancellationToken cancellationToken);

        protected abstract IRemoteItem GetRemoteRootItem(string serverPath);

        private static void ApplyExtendedItems(string targetGuid, List<UpdateItem> itemsToReplace, List<UpdateTask> allResults)
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

        private static Func<IRemoteItem, FileInfo, bool> GetVersionEqualsComparer(UpdateInfo updateInfo)
        {
            switch (updateInfo.Versioning)
            {
                case UpdateInfo.VersioningMode.Size:
                    return (item, info) => item.ItemSize == info.Length;
                case UpdateInfo.VersioningMode.Date:
                    return (item, info) => item.ModifiedTime <= info.LastWriteTimeUtc;
                case UpdateInfo.VersioningMode.Contents:
                    if (!updateInfo.ContentHashes.Any())
                    {
                        Console.WriteLine($"No hashes found in {updateInfo.GUID} while VersioningMode was set to Contents, falling back to Size");
                        goto case UpdateInfo.VersioningMode.Size;
                    }

                    return (remote, local) =>
                    {
                        var match = updateInfo.ContentHashes.FirstOrDefault(hash => PathTools.PathsEqual(hash.RelativeFileName, remote.ClientRelativeFileName));
                        if (match == null || match.Hash == 0)
                        {
                            Console.WriteLine($"No hash found on remote for file {remote.ClientRelativeFileName} - comparing size instead");
                            return remote.ItemSize == local.Length;
                        }
                        return FileContentsCalculator.GetFileHash(local) == match.Hash;
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<UpdateItem> ProcessDirectory(IRemoteItem remoteDir, DirectoryInfo localDir,
            bool recursive, bool removeNotExisting, Func<IRemoteItem, FileInfo, bool> versionEqualsComparer,
            CancellationToken cancellationToken)
        {
            if (!remoteDir.IsDirectory) throw new DirectoryNotFoundException();

            var results = new List<UpdateItem>();

            var localContents = new List<FileSystemInfo>();
            if (localDir.Exists)
                localContents.AddRange(localDir.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly));

            foreach (var remoteItem in remoteDir.GetDirectoryContents(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (remoteItem.IsDirectory)
                {
                    if (recursive)
                    {
                        var localItem = localContents.OfType<DirectoryInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                        if (localItem == null)
                            localItem = new DirectoryInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                        else
                            localContents.Remove(localItem);

                        results.AddRange(ProcessDirectory(remoteItem, localItem, recursive, removeNotExisting, versionEqualsComparer, cancellationToken));
                    }
                }
                else if (remoteItem.IsFile)
                {
                    var itemDate = remoteItem.ModifiedTime;
                    if (itemDate > _latestModifiedDate) _latestModifiedDate = itemDate;

                    var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                    if (localFile == null)
                        localFile = new FileInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                    else
                        localContents.Remove(localFile);

                    var localIsUpToDate = localFile.Exists && versionEqualsComparer(remoteItem, localFile);
                    results.Add(new UpdateItem(localFile, remoteItem, localIsUpToDate));
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Remove all files that were not on the remote
            if (removeNotExisting)
                results.AddRange(UpdateSourceManager.FileInfosToDeleteItems(localContents));

            return results;
        }
    }
}
