using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Updater.Data;
using KKManager.Updater.Sources;
using KKManager.Util;

namespace KKManager.Updater.Downloader
{
    internal class UpdateDownloadCoordinator
    {
        private List<UpdateDownloadItem> _updateItems;

        private UpdateDownloadCoordinator()
        {
        }

        public IReadOnlyList<UpdateDownloadItem> UpdateItems => _updateItems;

        public static UpdateDownloadCoordinator Create(IEnumerable<UpdateTask> updateTasks)
        {
            var downloadCoordinator = new UpdateDownloadCoordinator();

            // Split the tasks into individual files to download with lists of servers to they can be downloaded from
            // so later the download threads can pick them easily
            var updateItemInfos = updateTasks.SelectMany(updateTask =>
            {
                var updateItemGroups = updateTask.GetUpdateItems();
                return updateItemGroups.Select(updateItemGroup => new UpdateDownloadItem(
                    updateTask,
                    new FileInfo(updateItemGroup.Key),
                    updateItemGroup
                        .DistinctBy(x => x.Item1.Source) // Extra safeguard in case of duplicate entries, shouldn't ever be needed but apparently is
                        .ToDictionary(x => x.Item1.Source, x => x.Item2)));
            });

            var sortedUpdateItemInfos = updateItemInfos
                // Remove unnecessary to avoid potential conflicts if the update is aborted midway and a newer version is added
                .OrderByDescending(sources => sources.IsFileDelete)
                // Try items with a single source first since they are the most risky
                .ThenBy(sources => sources.DownloadSources.Count)
                .ThenBy(sources => sources.DownloadPath.FullName)
                .ToList();

            for (var i = 0; i < sortedUpdateItemInfos.Count; i++)
                sortedUpdateItemInfos[i].Order = i + 1;

            downloadCoordinator._updateItems = sortedUpdateItemInfos;

            return downloadCoordinator;
        }

        public async Task RunUpdate(CancellationToken cancellationToken)
        {
            try
            {
                // One thread per server
                var allSources = _updateItems
                    .SelectMany(x => x.DownloadSources.Keys)
                    .Distinct();

                var runningTasks = new List<Tuple<Task, DownloadSourceInfo>>();
                foreach (var updateSource in allSources)
                {
                    var updateSourceInfo = new DownloadSourceInfo(updateSource);
                    var task = Task.Run(async () => await UpdateThread(updateSourceInfo, cancellationToken),
                        cancellationToken);
                    runningTasks.Add(new Tuple<Task, DownloadSourceInfo>(task, updateSourceInfo));
                }

                await Task.WhenAll(runningTasks.Select(x => x.Item1));

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                foreach (var updateItem in _updateItems)
                {
                    if (updateItem.Status == UpdateDownloadStatus.Downloading || updateItem.Status == UpdateDownloadStatus.Waiting)
                        updateItem.MarkAsCancelled();
                }

                throw;
            }
        }

        /// <summary>
        /// Thead handling a single server.
        /// It looks for updates that can be downloaded from that server and picks what it can download.
        /// When no more work is available the task finishes.
        /// </summary>
        private async Task UpdateThread(DownloadSourceInfo updateSource, CancellationToken cancellationToken)
        {
            Exception failReason = null;
            try
            {
                // Exit early if the source keeps failing
                var failCount = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    UpdateDownloadItem currentDownloadItem = null;
                    UpdateItem currentlyDownloading = null;
                    lock (_updateItems)
                    {
                        currentDownloadItem = _updateItems.FirstOrDefault(x =>
                            x.Status == UpdateDownloadStatus.Waiting &&
                            x.DownloadSources.ContainsKey(updateSource.Source));
                        if (currentDownloadItem != null)
                        {
                            currentDownloadItem.Status = UpdateDownloadStatus.Downloading;
                            currentlyDownloading = currentDownloadItem.DownloadSources[updateSource.Source];
                        }
                    }

                    if (currentlyDownloading == null || cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Closing source downloader {updateSource.Source.Origin}");
                        return;
                    }

                    var progress = new Progress<double>(percent => currentDownloadItem.FinishPercent = percent);

                    try
                    {
                        currentDownloadItem.FinishPercent = 0;

                        await RetryHelper.RetryOnExceptionAsync(
                            () => currentlyDownloading.Update(progress, cancellationToken), 3, TimeSpan.FromSeconds(3),
                            cancellationToken);

                        currentDownloadItem.FinishPercent = 100;
                        currentDownloadItem.Status = UpdateDownloadStatus.Finished;

                        failCount = 0;
                    }
                    catch (Exception e)
                    {
                        if (e is OperationCanceledException)
                        {
                            currentDownloadItem.MarkAsCancelled(e);

                            if (cancellationToken.IsCancellationRequested)
                                return;
                            else
                                continue;
                        }

                        Console.WriteLine(
                            $"Marking source {updateSource.Source.Origin} as broken because of exception: {e.ToStringDemystified()}");

                        lock (_updateItems)
                        {
                            currentDownloadItem.TryMarkSourceAsFailed(updateSource.Source, e);
                            currentDownloadItem.FinishPercent = 0;
                        }

                        // Give the source a couple of chances before ditching it
                        if (++failCount >= 2)
                        {
                            failReason = e;
                            return;
                        }
                    }
                }
            }
            finally
            {
                if (failReason != null)
                {
                    lock (_updateItems)
                    {
                        var e = new DownloadSourceCrashedException("Update source " + updateSource.Source.Origin + " closed early because of other issues", updateSource.Source, failReason);
                        foreach (var updateTask in _updateItems)
                            updateTask.TryMarkSourceAsFailed(updateSource.Source, e);
                    }
                }
            }
        }

        private class DownloadSourceInfo
        {
            public readonly UpdateSourceBase Source;

            public DownloadSourceInfo(UpdateSourceBase updateSource)
            {
                Source = updateSource;
            }
        }
    }
}