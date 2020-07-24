using System.IO;
using KKManager.Updater.Data;

namespace KKManager.Updater.Sources
{
    internal sealed class DeleteFileUpdateItem : UpdateItem
    {
        public DeleteFileUpdateItem(FileSystemInfo targetPath) : base(targetPath, null, false) { }
    }
}