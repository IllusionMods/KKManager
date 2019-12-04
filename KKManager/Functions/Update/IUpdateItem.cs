using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Util;

namespace KKManager.Functions.Update
{
    public interface IUpdateItem
    {
        Task Update(CancellationToken cancellationToken);
        FileSystemInfo TargetPath { get; }
        FileSize ItemSize { get; }
        DateTime? ModifiedTime { get; }
    }
}