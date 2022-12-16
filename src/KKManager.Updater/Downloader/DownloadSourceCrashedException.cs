using System;
using KKManager.Updater.Sources;

namespace KKManager.Updater.Downloader
{
    internal class DownloadSourceCrashedException : Exception
    {
        public UpdateSourceBase DownloadSource { get; }

        public DownloadSourceCrashedException(string message, UpdateSourceBase source, Exception failReason) : base(message, failReason)
        {
            DownloadSource = source;
        }
    }
}