using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KKManager.Updater.Data;
using KKManager.Updater.Sources;
using KKManager.Util;

namespace KKManager.Updater.Downloader
{
    internal class UpdateDownloadItem
    {
        public readonly Dictionary<UpdateSourceBase, UpdateItem> DownloadSources;

        private double _finishPercent;

        public UpdateDownloadStatus Status;

        public UpdateDownloadItem(UpdateTask updateTask, FileInfo downloadPath,
            Dictionary<UpdateSourceBase, UpdateItem> downloadSources)
        {
            DownloadSources = downloadSources ?? throw new ArgumentNullException(nameof(downloadSources));
            UpdateTask = updateTask ?? throw new ArgumentNullException(nameof(updateTask));
            DownloadPath = downloadPath ?? throw new ArgumentNullException(nameof(downloadPath));

            Status = UpdateDownloadStatus.Waiting;

            IsFileDelete = downloadSources.Any(x => x.Value is DeleteFileUpdateItem);
            TotalSize = DownloadSources.FirstOrDefault().Value.GetDownloadSize();
        }

        public UpdateTask UpdateTask { get; }

        public FileInfo DownloadPath { get; }

        public double FinishPercent
        {
            get => _finishPercent;
            set => _finishPercent = Math.Max(0, Math.Min(100, value));
        }

        public bool IsFileDelete { get; }
        public List<Exception> Exceptions { get; } = new List<Exception>();
        public FileSize TotalSize { get; }

        public int Order { get; set; }

        public FileSize GetDownloadedSize()
        {
            return FileSize.FromKilobytes((long)Math.Round(TotalSize.GetKbSize() * (FinishPercent / 100d)));
        }

        public void TryMarkSourceAsFailed(UpdateSourceBase source, Exception exception)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (DownloadSources.Remove(source))
            {
                if (exception != null) Exceptions.Add(exception);
                if (Status != UpdateDownloadStatus.Finished && Status != UpdateDownloadStatus.Failed)
                {
                    if (DownloadSources.Count == 0)
                        Status = UpdateDownloadStatus.Failed;
                }
            }
        }

        public void MarkAsCancelled(Exception cancelException = null)
        {
            Status = UpdateDownloadStatus.Cancelled;
            if (cancelException != null) Exceptions.Add(cancelException);
        }
    }
}