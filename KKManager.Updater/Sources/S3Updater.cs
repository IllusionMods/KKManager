using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using KKManager.Updater.Data;

namespace KKManager.Updater.Sources
{
    public class S3Updater : UpdateSourceBase
    {
        private readonly string _bucketName;
        private readonly List<S3Object> _results = new List<S3Object>();

        private readonly AmazonS3Client _s3Client;

        public S3Updater(Uri serverUri, int discoveryPriority, int downloadPriority) : base(serverUri.Host,
            discoveryPriority, downloadPriority)
        {
            var info = serverUri.UserInfo.Split(new[] { ':' }, 2, StringSplitOptions.None);
            if (info.Length != 2)
                throw new ArgumentException("User information is missing or invalid", nameof(serverUri));
            var credentials = new NetworkCredential(info[0], info[1]);

            if (string.IsNullOrEmpty(serverUri.Host)) throw new ArgumentException("Host is missing", nameof(serverUri));
            var endpoint = "https://" + serverUri.Host;

            _bucketName = serverUri.LocalPath.Trim('/');
            if (string.IsNullOrEmpty(_bucketName)) throw new ArgumentException("Bucket is missing", nameof(serverUri));

            var config = new AmazonS3Config { ServiceURL = endpoint };
            _s3Client = new AmazonS3Client(credentials.UserName, credentials.Password, config);
            //// Make sure the connection is possible
            //if (!_s3Client.ListObjectsV2(new ListObjectsV2Request { BucketName = _bucketName, MaxKeys = 1 }).S3Objects.Any())
            //    throw new ArgumentException("Bucket doesn't exist or other information is wrong", nameof(serverUri));
        }

        public override void Dispose()
        {
            _s3Client.Dispose();
            _results.Clear();
        }

        protected override async Task<Stream> DownloadFileAsync(string updateFileName,
            CancellationToken cancellationToken)
        {
            var objectResponse = await _s3Client.GetObjectAsync(_bucketName, updateFileName, cancellationToken);
            return objectResponse.ResponseStream;
        }

        protected override IRemoteItem GetRemoteRootItem(string serverPath)
        {
            Task.Run(() => GetFileListing().Wait()).Wait(); //todo async? move somewhere else? Need to run through task.run to avoid deadlock

            var pathParts = serverPath.Trim().Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var cleanPath = string.Join("/", pathParts);

            return new WasabiRemoteItem(cleanPath, this, cleanPath);
        }

        private async Task GetFileListing()
        {
            if (_results.Any()) return;

            //_results.Clear();

            // read all files in the bucket
            ListObjectsV2Response result;
            string continuationToken = null;
            do
            {
                result = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    RequestPayer = RequestPayer.Requester,
                    ContinuationToken = continuationToken
                });
                _results.AddRange(result.S3Objects);
                continuationToken = result.NextContinuationToken;
            } while (result.IsTruncated);
        }

        private async Task UpdateItem(S3Object sourceItem, FileInfo targetPath, IProgress<double> progressCallback,
            CancellationToken cancellationToken)
        {
            //_s3Client.DownloadToFilePathAsync() doesn't support progress callbacks, need to do all this instead
            var objectResponse = await _s3Client.GetObjectAsync(sourceItem.BucketName, sourceItem.Key, cancellationToken);
            using (var input = objectResponse.ResponseStream)
            using (var output = targetPath.OpenWrite())
            {
                // buffer based on file size to have decent rate of progress reporting
                const int preferredUpdateSteps = 20;
                const long minBufferSize = 128 * 1024; //kb
                const long maxBufferSize = 5 * 1024 * 1024; //mb
                var buffer = new byte[Math.Min(maxBufferSize, Math.Max(minBufferSize, sourceItem.Size / preferredUpdateSteps))];
                int bytesRead;
                while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    var task = output.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                    progressCallback.Report(100d * output.Position / sourceItem.Size);

                    await task;
                }
            }
        }

        private IEnumerable<WasabiRemoteItem> GetSubItems(string fullPath, string rootFolder)
        {
            var searchPath = fullPath.Trim().Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            var folders = new HashSet<string>();

            foreach (var node in _results)
            {
                // Remote keys always use / as separators, no leading or trailing /
                var remotePath = node.Key.Split('/');

                // Make sure the path leading up to the item is identical
                if (remotePath.Length <= searchPath.Length) continue;
                for (var i = 0; i < searchPath.Length; i++)
                    if (!remotePath[i].Equals(searchPath[i], StringComparison.InvariantCultureIgnoreCase))
                        goto skip;

                // Check if the item is directly inside this directory
                if (remotePath.Length <= searchPath.Length + 1)
                    yield return new WasabiRemoteItem(node, this, rootFolder);
                // If not then take note of the subfolder
                else
                    folders.Add(string.Join("/", remotePath.Take(searchPath.Length + 1)));

                skip:;
            }

            // Generate items for subfolders directly inside this directory
            foreach (var folder in folders.Distinct(StringComparer.InvariantCultureIgnoreCase))
                yield return new WasabiRemoteItem(folder, this, rootFolder);
        }

        private sealed class WasabiRemoteItem : IRemoteItem
        {
            public WasabiRemoteItem(S3Object sourceItem, S3Updater source, string rootFolder)
            {
                if (sourceItem == null) throw new ArgumentNullException(nameof(sourceItem));
                if (source == null) throw new ArgumentNullException(nameof(source));

                if (rootFolder != null)
                {
                    _rootFolder = rootFolder;
                    if (!sourceItem.Key.StartsWith(_rootFolder))
                        throw new IOException($"Remote item full path {sourceItem.Key} doesn't start with the specified root path {_rootFolder}");
                    ClientRelativeFileName = sourceItem.Key.Substring(_rootFolder.Length);
                }

                IsFile = true;

                _sourceItem = sourceItem;
                _source = source;
                ItemSize = _sourceItem.Size;
                ModifiedTime = _sourceItem.LastModified;
                Name = Path.GetFileName(_sourceItem.Key);
            }

            public WasabiRemoteItem(string folderPath, S3Updater source, string rootFolder)
            {
                if (folderPath == null) throw new ArgumentNullException(nameof(folderPath));
                if (source == null) throw new ArgumentNullException(nameof(source));

                if (rootFolder != null)
                {
                    _rootFolder = rootFolder;
                    if (!folderPath.StartsWith(_rootFolder))
                        throw new IOException($"Remote item full path {folderPath} doesn't start with the specified root path {_rootFolder}");
                    ClientRelativeFileName = folderPath.Substring(_rootFolder.Length);
                }

                IsDirectory = true;

                _folderPath = folderPath;

                _source = source;
                Name = Path.GetFileName(folderPath);
            }

            private readonly S3Updater _source;
            private readonly S3Object _sourceItem;
            private readonly string _folderPath;
            private readonly string _rootFolder;

            public string Name { get; }
            public long ItemSize { get; }
            public DateTime ModifiedTime { get; }
            public bool IsDirectory { get; }
            public bool IsFile { get; }
            public string ClientRelativeFileName { get; }
            UpdateSourceBase IRemoteItem.Source => _source;

            public IRemoteItem[] GetDirectoryContents(CancellationToken cancellationToken)
            {
                if (IsFile) throw new InvalidOperationException("Can't get directory contents of a file");
                return _source.GetSubItems(_folderPath, _rootFolder).Cast<IRemoteItem>().ToArray();
            }

            public async Task Download(FileInfo downloadTarget, Progress<double> progressCallback,
                CancellationToken cancellationToken)
            {
                if (IsDirectory) throw new InvalidOperationException("Can't download a directory");
                await _source.UpdateItem(_sourceItem, downloadTarget, progressCallback, cancellationToken);
            }
        }
    }
}