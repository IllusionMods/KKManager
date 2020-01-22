using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Updater.Data;
using KKManager.Util;
using SharpCompress.Archives;

namespace KKManager.Updater.Sources
{
    public class ZipUpdater : UpdateSourceBase
    {
        private readonly IArchive _archive;

        public ZipUpdater(FileInfo archive, int discoveryPriority, int downloadPriority = 101) : base(archive.Name, discoveryPriority, downloadPriority)
        {
            _archive = ArchiveFactory.Open(archive);
        }
        public override void Dispose()
        {
            _archive.Dispose();
        }

        protected override async Task<Stream> DownloadFileAsync(string updateFileName, CancellationToken cancellationToken)
        {
            var f = _archive.Entries.FirstOrDefault(x => PathTools.PathsEqual(x.Key, updateFileName));
            if (f == null) throw new FileNotFoundException("File doesn't exist in archive");
            return f.OpenEntryStream();
        }

        protected override IRemoteItem GetRemoteRootItem(string serverPath)
        {
            var f = _archive.Entries.FirstOrDefault(x => PathTools.PathsEqual(x.Key, serverPath));
            if (f == null) return null;
            return new ArchiveItem(f, f.Key, this);
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/', ' ');
        }

        private static IEnumerable<IArchiveEntry> GetSubItems(IArchiveEntry sourceItem)
        {
            var sourcePath = NormalizePath(sourceItem.Key) + '/';
            var sourceDepth = sourcePath.Count(c => c == '/');
            var subItems = sourceItem.Archive.Entries.Where(
                x =>
                {
                    var otherPath = NormalizePath(x.Key);
                    return otherPath.StartsWith(sourcePath) && otherPath.Count(c => c == '/') == sourceDepth;
                });
            return subItems;
        }

        public sealed class ArchiveItem : IRemoteItem
        {
            private readonly IArchiveEntry _sourceItem;
            private readonly string _rootFolder;
            public UpdateSourceBase Source { get; }

            public ArchiveItem(IArchiveEntry item, string rootFolder, ZipUpdater source)
            {
                Source = source;
                _sourceItem = item ?? throw new ArgumentNullException(nameof(item));

                ItemSize = item.Size;
                ModifiedTime = _sourceItem.LastModifiedTime ?? _sourceItem.CreatedTime ?? _sourceItem.ArchivedTime ?? _sourceItem.LastAccessedTime ?? DateTime.MinValue;
                Name = Path.GetFileName(item.Key);
                IsDirectory = item.IsDirectory;
                IsFile = !item.IsDirectory;

                if (rootFolder != null)
                {
                    _rootFolder = rootFolder;
                    if (!item.Key.StartsWith(_rootFolder)) throw new IOException($"Remote item full path {_sourceItem.Key} doesn't start with the specified root path {_rootFolder}");
                    ClientRelativeFileName = item.Key.Substring(_rootFolder.Length);
                }
            }

            public string Name { get; }
            public long ItemSize { get; }
            public DateTime ModifiedTime { get; }
            public bool IsDirectory { get; }
            public bool IsFile { get; }
            public string ClientRelativeFileName { get; }

            public IRemoteItem[] GetDirectoryContents(CancellationToken cancellationToken)
            {
                var subItems = GetSubItems(_sourceItem);
                return subItems.Select(x => (IRemoteItem)new ArchiveItem(x, _rootFolder, (ZipUpdater)Source)).ToArray();
            }

            public Task Download(FileInfo downloadTarget, Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                // todo reimplement with streams to have progress
                return Task.Run(() => _sourceItem.WriteToFile(downloadTarget.FullName), cancellationToken);
            }
        }
    }
}
