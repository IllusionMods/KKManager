using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KKManager.Functions.Update {
    internal sealed class DeleteFileUpdateItem : IUpdateItem
    {
        public DeleteFileUpdateItem(FileSystemInfo targetPath)
        {
            TargetPath = targetPath;
        }

        public Task Update(CancellationToken cancellationToken)
        {
            TargetPath.Delete();
            return Task.CompletedTask;
        }

        public FileSystemInfo TargetPath { get; }
    }
}