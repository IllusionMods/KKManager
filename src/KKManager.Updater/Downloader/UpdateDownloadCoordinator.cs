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
        public enum UpdateStatus
        {
            Aborted = -1,
            Stopped = 0,
            Starting,
            Running,
            Finished,
        }

        public UpdateStatus Status
        {
            get => _status;
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    UpdateStatusChanged?.Invoke(this, new UpdateStatusChangedEventArgs(this, value));
                }
            }
        }

        public class UpdateStatusChangedEventArgs : EventArgs
        {
            public UpdateStatusChangedEventArgs(UpdateDownloadCoordinator source, UpdateStatus status)
            {
                Source = source;
                Status = status;
            }
            public UpdateDownloadCoordinator Source { get; }
            public UpdateStatus Status { get; }
        }

        public static event EventHandler<UpdateStatusChangedEventArgs> UpdateStatusChanged;

        private List<UpdateDownloadItem> _updateItems;
        private UpdateStatus _status = UpdateStatus.Stopped;

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
            Status = UpdateStatus.Starting;
            try
            {
                // One thread per server
                var allSources = _updateItems
                                 .SelectMany(x => x.DownloadSources.Keys)
                                 .Distinct();

                var runningTasks = new List<Tuple<Thread, DownloadSourceInfo>>();
                foreach (var updateSource in allSources)
                {
                    if (updateSource is TorrentUpdater.TorrentSource) //todo
                        for (int i = 0; i < updateSource.MaxConcurrentDownloads; i++)
                        {
                            var updateSourceInfo = new DownloadSourceInfo(updateSource, i);
                            var thread = new Thread(() => UpdateThread(updateSourceInfo, cancellationToken));
                            thread.Start();
                            runningTasks.Add(new Tuple<Thread, DownloadSourceInfo>(thread, updateSourceInfo));
                        }
                }

                Status = UpdateStatus.Running;

                while (runningTasks.Any(x => x.Item1.IsAlive))
                {
                    await Task.Delay(1000, CancellationToken.None);
                }

                cancellationToken.ThrowIfCancellationRequested();

                Status = UpdateStatus.Finished;
            }
            catch (OperationCanceledException)
            {
                foreach (var updateItem in _updateItems)
                {
                    if (updateItem.Status == UpdateDownloadStatus.Downloading || updateItem.Status == UpdateDownloadStatus.Waiting)
                        updateItem.MarkAsCancelled();
                }

                Status = UpdateStatus.Aborted;
                throw;
            }
            catch
            {
                Status = UpdateStatus.Aborted;
                throw;
            }
        }

        /// <summary>
        /// Thead handling a single server.
        /// It looks for updates that can be downloaded from that server and picks what it can download.
        /// When no more work is available the task finishes.
        /// </summary>
        private void UpdateThread(DownloadSourceInfo updateSource, CancellationToken cancellationToken)
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
                        Console.WriteLine($"Closing source downloader {updateSource}");
                        return;
                    }

                    var progress = new Progress<double>(percent => currentDownloadItem.FinishPercent = percent);

                    try
                    {
                        currentDownloadItem.FinishPercent = 0;

                        currentlyDownloading.Update(progress, cancellationToken).Wait(CancellationToken.None);

                        currentDownloadItem.FinishPercent = 100;
                        currentDownloadItem.Status = UpdateDownloadStatus.Finished;

                        failCount = 0;

                        TorrentUpdater.OnFileUpdateFinished(currentDownloadItem);
                    }
                    catch (Exception e)
                    {
                        if (e is AggregateException aex)
                            e = aex.Flatten().InnerExceptions.First();

                        if (e is OperationCanceledException)
                        {
                            currentDownloadItem.MarkAsCancelled(e);

                            if (cancellationToken.IsCancellationRequested)
                                return;
                            else
                                continue;
                        }

                        Console.WriteLine($"Marking source {updateSource.Source.Origin} as broken because of exception: {e.ToStringDemystified()}");

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
                        var e = new DownloadSourceCrashedException("Update source " + updateSource + " closed early because of other issues", updateSource.Source, failReason);
                        foreach (var updateTask in _updateItems)
                            updateTask.TryMarkSourceAsFailed(updateSource.Source, e);
                    }
                }
            }
        }

        private class DownloadSourceInfo
        {
            /// <summary>
            /// Used when a source supports multiple simultaneous downloads to specify which download thread this is.
            /// </summary>
            public readonly int Index;
            public readonly UpdateSourceBase Source;

            public DownloadSourceInfo(UpdateSourceBase updateSource, int index = -1)
            {
                Source = updateSource ?? throw new ArgumentNullException(nameof(updateSource));
                if (Source.MaxConcurrentDownloads > 1 && index < 0)
                    throw new ArgumentException($"Invalid download index {index} for source with MaxConcurrentDownloads={Source.MaxConcurrentDownloads}", nameof(index));
                Index = index;
            }

            public override string ToString()
            {
                return Source.MaxConcurrentDownloads > 1 ? $"{Source.Origin}(#{Index + 1}/{Source.MaxConcurrentDownloads})" : Source.Origin;
            }
        }
    }
}