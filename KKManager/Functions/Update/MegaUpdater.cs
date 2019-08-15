using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CG.Web.MegaApiClient;
using KKManager.Util;

namespace KKManager.Functions.Update
{
    public class MegaUpdater : IDisposable
    {
        private static readonly string[] _acceptableZipmodExtensions = { ".zip", ".zipmod" };

        private readonly MegaApiClient _client;

        private List<INode> _allNodes;

        public MegaUpdater()
        {
            var client = new MegaApiClient();
            _client = client;
        }

        public Uri CurrentFolderLink { get; private set; }

        public void Dispose()
        {
            try
            {
                _allNodes = null;
                if (_client != null && _client.IsLoggedIn)
                    _client.Logout();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task Connect()
        {
            if (!_client.IsLoggedIn)
                await RetryHelper.RetryOnExceptionAsync(async () => await _client.LoginAnonymousAsync(), 2, TimeSpan.FromSeconds(1), CancellationToken.None);
        }

        public async Task DownloadNodeAsync(SideloaderUpdateItem task, Progress<double> progress, CancellationToken cancellationToken)
        {
            await Connect();
            await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                try
                {
                    await _client.DownloadFileAsync(task.RemoteFile, task.LocalFile.FullName, progress, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Needed to avoid partially downloaded files causing issues
                    task.LocalFile.Delete();
                    throw;
                }
            }, 2, TimeSpan.FromSeconds(1), cancellationToken);
        }

        public async Task<List<INode>> GetNodesFromLinkAsync(Uri folderLink, CancellationToken cancellationToken)
        {
            await Connect();
            await RetryHelper.RetryOnExceptionAsync(async () => _allNodes = (await _client.GetNodesFromLinkAsync(folderLink)).ToList(), 2, TimeSpan.FromSeconds(1), cancellationToken);
            CurrentFolderLink = folderLink;
            return _allNodes;
        }

        public IEnumerable<INode> GetSubNodes(INode rootNode)
        {
            return _allNodes.Where(x => x.ParentId == rootNode.Id);
        }

        public async Task<IList<SideloaderUpdateItem>> GetUpdateTasksAsync(CancellationToken cancellationToken)
        {
            var link = new Uri("https://mega.nz/#F!fkYzQa5K!nSc7wkY82OUqZ4Hlff7Rlg");
            var nodes = await GetNodesFromLinkAsync(link, cancellationToken);
            return await CollectTasksAsync(nodes, cancellationToken);
        }

        private async Task<IList<SideloaderUpdateItem>> CollectTasksAsync(List<INode> nodes, CancellationToken cancellationToken)
        {
            IList<SideloaderUpdateItem> results = null;

            await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                results = await Task.Run(
                    () => CollectTasks(nodes, cancellationToken).ToList(),
                    cancellationToken);
            }, 2, TimeSpan.FromSeconds(1), cancellationToken);

            return results;
        }

        private IEnumerable<SideloaderUpdateItem> CollectTasks(List<INode> nodes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var root = nodes.Single(x => x.Type == NodeType.Root);

            var modsDirPath = InstallDirectoryHelper.GetModsPath();
            Directory.CreateDirectory(modsDirPath);

            var results = Enumerable.Empty<SideloaderUpdateItem>();

            foreach (var remoteModpackDir in GetSubNodes(root).Where(x => x.Type == NodeType.Directory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (remoteModpackDir.Name.StartsWith("Sideloader Modpack"))
                {
                    var localModpackDir = new DirectoryInfo(Path.Combine(modsDirPath, remoteModpackDir.Name));
                    if (localModpackDir.Exists)
                        results = results.Concat(ProcessDirectory(remoteModpackDir, localModpackDir, cancellationToken));
                }
                else
                {
                    Console.WriteLine("Skipping non-modpack directory " + remoteModpackDir.Name);
                }
            }

            return results;
        }

        private IEnumerable<SideloaderUpdateItem> ProcessDirectory(INode remoteDir, DirectoryInfo localDir, CancellationToken cancellationToken)
        {
            var results = new List<SideloaderUpdateItem>();

            var localContents = new List<FileSystemInfo>();
            if (localDir.Exists)
                localContents.AddRange(localDir.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly));

            foreach (var remoteItem in GetSubNodes(remoteDir))
            {
                cancellationToken.ThrowIfCancellationRequested();

                switch (remoteItem.Type)
                {
                    case NodeType.File:
                        {
                            var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                            if (localFile == null)
                                localFile = new FileInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                            else
                                localContents.Remove(localFile);

                            var extension = Path.GetExtension(remoteItem.Name)?.ToLower();

                            if (_acceptableZipmodExtensions.Contains(extension))
                            {
                                var localIsUpToDate = localFile.Exists && localFile.Length == remoteItem.Size;

                                results.Add(new SideloaderUpdateItem(remoteItem, localFile, localIsUpToDate));
                            }
                        }
                        break;

                    case NodeType.Directory:
                        {
                            var localItem = localContents.OfType<DirectoryInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                            if (localItem == null)
                                localItem = new DirectoryInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                            else
                                localContents.Remove(localItem);

                            results.AddRange(ProcessDirectory(remoteItem, localItem, cancellationToken));
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Remove all files that were not on the remote
            foreach (var localItem in localContents)
            {
                if (!localItem.Exists) continue;

                switch (localItem)
                {
                    case FileInfo fi:
                        if (fi.Extension.ToLowerInvariant().Contains("zip"))
                            results.Add(new SideloaderUpdateItem(null, fi));
                        break;

                    case DirectoryInfo di:
                        foreach (var subFi in di.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Extension.ToLowerInvariant().Contains("zip")))
                            results.Add(new SideloaderUpdateItem(null, subFi));
                        break;
                }
            }

            return results;
        }
    }
}
