using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using KKManager.Updater.Data;

namespace KKManager.Updater.Sources
{
    public class ZipUpdater : UpdateSourceBase
    {
        private readonly ZipFile _zipfile;

        public ZipUpdater(FileInfo archive) : base(archive.Name, 100)
        {
            _zipfile = ZipFile.Read(archive.FullName);
            _zipfile.FlattenFoldersOnExtract = true;
        }

        public override void Dispose()
        {
            _zipfile.Dispose();
        }
        
        protected override async Task<Stream> DownloadFileAsync(string updateFileName, CancellationToken cancellationToken)
        {
            try
            {
                return await Task.Run(() => (Stream)GetZipEntry(updateFileName)?.OpenReader(), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read file {updateFileName} from zip archive - {ex}");
                return null;
            }
        }

        protected override IRemoteItem GetRemoteRootItem(string serverPath)
        {
            var remote = GetZipEntry(serverPath);
            if (remote == null) return null;
            return new ZipUpdateItem(remote, this, remote.FileName);
        }

        private ZipEntry GetZipEntry(string serverPath)
        {
            // Clean up the path into a usable form
            serverPath = serverPath.Trim(' ', '\\', '/').Replace('\\', '/') + "/";
            var remote = _zipfile.Entries.FirstOrDefault(entry => string.Equals(entry.FileName, serverPath, StringComparison.OrdinalIgnoreCase));
            return remote;
        }

        private IEnumerable<ZipEntry> GetChildren(ZipEntry directory)
        {
            var dirDepth = directory.FileName.Count(c => c == '\\' || c == '/');
            return _zipfile.Entries.Where(
                x =>
                {
                    var fname = x.FileName.TrimEnd('\\', '/');
                    return fname.StartsWith(directory.FileName) && fname.Count(c => c == '\\' || c == '/') == dirDepth;
                });
        }

        private async Task UpdateItem(ZipEntry zipEntry, FileInfo downloadTarget, IProgress<double> progressCallback, CancellationToken cancellationToken)
        {
            await Task.Run(
                () =>
                {
                    var localDir = downloadTarget.DirectoryName;
                    var extractedFileName = Path.Combine(localDir, Path.GetFileName(zipEntry.FileName));

                    void OnExtractProgress(object sender, ExtractProgressEventArgs args)
                    {
                        if (args.CurrentEntry == zipEntry && args.TotalBytesToTransfer > 0)
                            progressCallback.Report((args.BytesTransferred / args.TotalBytesToTransfer) * 100);
                    }

                    try
                    {
                        _zipfile.ExtractProgress += OnExtractProgress;

                        zipEntry.Extract(localDir, ExtractExistingFileAction.OverwriteSilently);

                        // Check if the file needs to be renamed
                        if (!string.Equals(Path.GetFileName(zipEntry.FileName), downloadTarget.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(downloadTarget.FullName);
                            File.Move(extractedFileName, downloadTarget.FullName);
                        }
                    }
                    catch (Exception e)
                    {
                        File.Delete(extractedFileName);
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        _zipfile.ExtractProgress -= OnExtractProgress;
                    }
                }, cancellationToken);
        }

        public sealed class ZipUpdateItem : IRemoteItem
        {
            private readonly ZipUpdater _owner;
            private readonly ZipEntry _sourceItem;
            private readonly string _rootFolder;

            public ZipUpdateItem(ZipEntry item, ZipUpdater owner, string rootFolder)
            {
                _owner = owner;
                _sourceItem = item ?? throw new ArgumentNullException(nameof(item));
                ItemSize = item.UncompressedSize;
                ModifiedTime = _sourceItem.LastModified;
                Name = Path.GetFileName(item.FileName);
                IsDirectory = item.IsDirectory;
                IsFile = !item.IsDirectory;

                if (rootFolder != null)
                {
                    _rootFolder = rootFolder;
                    if (!_sourceItem.FileName.StartsWith(_rootFolder)) throw new IOException($"Remote item full path {_sourceItem.FileName} doesn't start with the specified root path {_rootFolder}");
                    ClientRelativeFileName = _sourceItem.FileName.Substring(_rootFolder.Length);
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
                return _owner.GetChildren(_sourceItem).Select(entry => (IRemoteItem)new ZipUpdateItem(entry, _owner, _rootFolder)).ToArray();
            }

            public Task Download(FileInfo downloadTarget, Progress<double> progressCallback, CancellationToken cancellationToken)
            {
                return _owner.UpdateItem(_sourceItem, downloadTarget, progressCallback, cancellationToken);
            }
        }
    }
}
