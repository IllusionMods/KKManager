using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Updater.Data;
using KKManager.Updater.Sources;
using KKManager.Util;

namespace KKManager.Updater.Downloader
{
    internal class UpdateDownloadCoordinator : IDisposable
    {
        public enum CoordinatorStatus
        {
            Disposed = -2,
            Aborted = -1,
            Stopped = 0,
            Starting,
            Running,
            Finished,
        }

        public BehaviorSubject<CoordinatorStatus> Status { get; } = new BehaviorSubject<CoordinatorStatus>(CoordinatorStatus.Stopped);

        private readonly List<UpdateDownloadItem> _updateItems;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private UpdateDownloadCoordinator(List<UpdateDownloadItem> updateItems, CancellationToken cancellationToken)
        {
            _updateItems = updateItems ?? throw new ArgumentNullException(nameof(updateItems));

            cancellationToken.Register(() =>
            {
                if (Status.Value == CoordinatorStatus.Stopped)
                    Status.OnNext(CoordinatorStatus.Aborted);
            });

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public IReadOnlyList<UpdateDownloadItem> UpdateItems => _updateItems;

        public static UpdateDownloadCoordinator Create(IEnumerable<UpdateTask> updateTasks, CancellationToken cancellationToken)
        {
            if (updateTasks == null) throw new ArgumentNullException(nameof(updateTasks));
            cancellationToken.ThrowIfCancellationRequested();

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

            var downloadCoordinator = new UpdateDownloadCoordinator(sortedUpdateItemInfos, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            return downloadCoordinator;
        }

        public async Task RunUpdate()
        {
            if (Status.Value != CoordinatorStatus.Stopped)
                throw new InvalidOperationException("Can only start when status is Stopped. Current status: " + Status);

            Status.OnNext(CoordinatorStatus.Starting);

            try
            {
                _cancellationToken.ThrowIfCancellationRequested();

                // One thread per server
                var allSources = _updateItems
                                 .SelectMany(x => x.DownloadSources.Keys)
                                 .Distinct();

                var runningTasks = new List<Tuple<Thread, DownloadSourceInfo>>();
                foreach (var updateSource in allSources)
                {
                    for (int i = 0; i < updateSource.MaxConcurrentDownloads; i++)
                    {
                        var updateSourceInfo = new DownloadSourceInfo(updateSource, i);
                        var thread = new Thread(() => UpdateThread(updateSourceInfo));
                        thread.Start();
                        runningTasks.Add(new Tuple<Thread, DownloadSourceInfo>(thread, updateSourceInfo));
                    }
                }

                Status.OnNext(CoordinatorStatus.Running);

                while (runningTasks.Any(x => x.Item1.IsAlive))
                {
                    await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
                }

                _cancellationToken.ThrowIfCancellationRequested();

                Status.OnNext(CoordinatorStatus.Finished);
            }
            catch (OperationCanceledException)
            {
                foreach (var updateItem in _updateItems)
                {
                    if (updateItem.Status == UpdateDownloadStatus.Downloading || updateItem.Status == UpdateDownloadStatus.Waiting)
                        updateItem.MarkAsCancelled();
                }

                Status.OnNext(CoordinatorStatus.Aborted);
                throw;
            }
            catch
            {
                Status.OnNext(CoordinatorStatus.Aborted);
                throw;
            }
        }

        /// <summary>
        /// Thead handling a single server.
        /// It looks for updates that can be downloaded from that server and picks what it can download.
        /// When no more work is available the task finishes.
        /// </summary>
        private void UpdateThread(DownloadSourceInfo updateSource)
        {
            Exception failReason = null;
            try
            {
                // Exit early if the source keeps failing
                var failCount = 0;
                while (!_cancellationToken.IsCancellationRequested)
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

                    if (currentlyDownloading == null || _cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Closing source downloader {updateSource}");
                        return;
                    }

                    var progress = new Progress<double>(percent => currentDownloadItem.FinishPercent = percent);

                    try
                    {
                        currentDownloadItem.FinishPercent = 0;

                        currentlyDownloading.Update(progress, _cancellationToken).Wait(CancellationToken.None);

                        currentDownloadItem.FinishPercent = 100;
                        currentDownloadItem.Status = UpdateDownloadStatus.Finished;

                        failCount = 0;
                    }
                    catch (Exception e)
                    {
                        if (e is AggregateException aex)
                            e = aex.Flatten().InnerExceptions.First();

                        if (e is OperationCanceledException)
                        {
                            currentDownloadItem.MarkAsCancelled(e);

                            if (_cancellationToken.IsCancellationRequested)
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

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            Status.OnNext(CoordinatorStatus.Disposed);
            Status.OnCompleted();
            Status.Dispose();
        }
    }
}