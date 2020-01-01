using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Util;

namespace KKManager.Updater.Data
{
    public interface IUpdateItem
    {
        Task Update(Progress<double> progressCallback, CancellationToken cancellationToken);
        FileSystemInfo TargetPath { get; }
        FileSize ItemSize { get; }
        DateTime? ModifiedTime { get; }
        bool UpToDate { get; }
    }
}