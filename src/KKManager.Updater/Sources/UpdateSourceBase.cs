using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;
using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Sources
{
    public abstract class UpdateSourceBase : IDisposable
    {
        private DateTime _latestModifiedDate = DateTime.MinValue;

        protected UpdateSourceBase(string origin, int discoveryPriority, int downloadPriority)
        {
            Origin = origin;
            DiscoveryPriority = discoveryPriority;
            DownloadPriority = downloadPriority;
        }

        /// <summary>
        /// Where the source pulls updates from.
        /// </summary>
        public string Origin { get; }

        /// <summary>
        /// Priority of the source if multiple sources for a download are available. Higher will be considered as having newer update info.
        /// </summary>
        public int DiscoveryPriority { get; }

        /// <summary>
        /// Priority of the source if multiple sources for a download are available. Higher will be attempted to be downloaded first.
        /// </summary>
        public int DownloadPriority { get; }

        public abstract void Dispose();

        public virtual async Task<List<UpdateTask>> GetUpdateItems(CancellationToken cancellationToken)
        {
            var updateInfos = new List<UpdateInfo>();

            var filenamesToTry = new[] { UpdateInfo.UpdateFileName, "Updates1.xml", "Updates2.xml" };

            foreach (var fn in filenamesToTry)
            {
                Stream str = null;
                try
                {
                    var downloadFileAsync = DownloadFileAsync(fn, cancellationToken);
                    if (!await downloadFileAsync.WithTimeout(TimeSpan.FromSeconds(20), cancellationToken))
                        throw new TimeoutException("Timeout when trying to download " + fn);
                    str = downloadFileAsync.Result;
                }
                catch (TimeoutException ex)
                {
                    throw RetryHelper.DoNotAttemptToRetry(ex);
                }
                catch (FileNotFoundException)
                {
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download Updates file {fn} from {Origin} - {ex.Message}");
                }

                if (str != null)
                {
                    try
                    {
                        updateInfos.AddRange(UpdateInfo.ParseUpdateManifest(str, this));
                    }
                    catch (OutdatedVersionException ex)
                    {
                        Console.WriteLine($"Failed to parse update manifest file {fn} from {Origin} - {ex.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to parse update manifest file {fn} from {Origin} - {ex.ToStringDemystified()}");
                    }
                    finally
                    {
                        str.Dispose();
                    }
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
                        var h = FileContentsCalculator.GetFileHash(local);
                        var match = updateInfo.ContentHashes.FirstOrDefault(hash => PathTools.PathsEqual(hash.RelativeFileName, remote.ClientRelativeFileName));

                        if (match != null && h != null)
                        {
                            if (h.SB3UHash != 0 && match.SB3UHash != 0)
                                return h.SB3UHash == match.SB3UHash;

                            if (h.FileHash != 0 && match.FileHash != 0)
                                return h.FileHash == match.FileHash;
                        }

                        Console.WriteLine($"No hash found on remote for file {remote.ClientRelativeFileName}, comparing file size instead");
                        return remote.ItemSize == local.Length;
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetEnabledName(FileInfo file)
        {
            if (SideloaderModLoader.IsValidZipmodExtension(file.Extension))
            {
                return SideloaderModInfo.EnabledLocation(file).Name;
            }
            else if (PluginLoader.IsValidPluginExtension(file.Extension))
            {
                return PluginInfo.EnabledLocation(file).Name;
            }
            else
            {
                return file.Name;
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

            var enabledName = localContents.OfType<FileInfo>().ToDictionary(x => x, GetEnabledName);

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

                    var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(enabledName[x], remoteItem.Name, StringComparison.OrdinalIgnoreCase));
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
