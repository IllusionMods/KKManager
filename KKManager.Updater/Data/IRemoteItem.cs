using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KKManager.Updater.Data
{
    public interface IRemoteItem
    {
        string Name { get; }
        long ItemSize { get; }
        DateTime ModifiedTime { get; }
        bool IsDirectory { get; }
        bool IsFile { get; }
        string ClientRelativeFileName { get; }

        IRemoteItem[] GetDirectoryContents(CancellationToken cancellationToken);
        Task Download(FileInfo downloadTarget, Progress<double> progressCallback, CancellationToken cancellationToken);
    }
}