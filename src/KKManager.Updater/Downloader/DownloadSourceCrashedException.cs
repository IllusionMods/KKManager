using System;
using KKManager.Updater.Sources;

namespace KKManager.Updater.Downloader
{
    internal class DownloadSourceCrashedException : Exception
    {
        public UpdateSourceBase Source { get; }

        public DownloadSourceCrashedException(string message, UpdateSourceBase source, Exception failReason) : base(message, failReason)
        {
            Source = source;
        }
    }
}