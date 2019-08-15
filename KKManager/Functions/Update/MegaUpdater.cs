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
                _client.Logout();
            }
            catch { }
        }

        public async Task Connect()
        {
            if (!_client.IsLoggedIn)
                await RetryHelper.RetryOnExceptionAsync(async () => await _client.LoginAnonymousAsync(), 2, TimeSpan.FromSeconds(1), CancellationToken.None);
        }

        public async Task DownloadNodeAsync(SideloaderUpdateItem task, Progress<double> progress, CancellationToken cancellationToken)
        {
            await Connect();
            await RetryHelper.RetryOnExceptionAsync(async () => await _client.DownloadFileAsync(task.RemoteFile, task.LocalFile.FullName, progress, cancellationToken), 2, TimeSpan.FromSeconds(1), cancellationToken);
        }

        public async Task<List<INode>> GetNodesFromLinkAsync(Uri folderLink)
        {
            await Connect();
            await RetryHelper.RetryOnExceptionAsync(async () => _allNodes = (await _client.GetNodesFromLinkAsync(folderLink)).ToList(), 2, TimeSpan.FromSeconds(1), CancellationToken.None);
            CurrentFolderLink = folderLink;
            return _allNodes;
        }

        public IEnumerable<INode> GetSubNodes(INode rootNode)
        {
            return _allNodes.Where(x => x.ParentId == rootNode.Id);
        }

        public async Task<IList<SideloaderUpdateItem>> GetUpdateTasksAsync()
        {
            var link = new Uri("https://mega.nz/#F!fkYzQa5K!nSc7wkY82OUqZ4Hlff7Rlg");
            var nodes = await GetNodesFromLinkAsync(link);
            return await CollectTasksAsync(nodes);
        }

        private async Task<IList<SideloaderUpdateItem>> CollectTasksAsync(List<INode> nodes)
        {
            IList<SideloaderUpdateItem> results = null;

            await RetryHelper.RetryOnExceptionAsync(async () => results = await Task.Run(() => CollectTasks(nodes).ToList()), 2, TimeSpan.FromSeconds(1), CancellationToken.None);

            return results;
        }

        private IEnumerable<SideloaderUpdateItem> CollectTasks(List<INode> nodes)
        {
            var root = nodes.Single(x => x.Type == NodeType.Root);
            //var root = nodes.Single(x => x.ParentId == null);

            var modsDirPath = InstallDirectoryHelper.GetModsPath();
            Directory.CreateDirectory(modsDirPath);
            //var modsDir = new DirectoryInfo(modsDirPath);

            var results = Enumerable.Empty<SideloaderUpdateItem>();

            foreach (var modpackType in GetSubNodes(root).Where(x => x.Type == NodeType.Directory))
            {
                if (modpackType.Name.StartsWith("Sideloader Modpack"))
                {
                    var localDir = Path.Combine(modsDirPath, modpackType.Name);
                    var modpackDir = new DirectoryInfo(localDir);
                    results = results.Concat(ProcessDirectory(modpackType, modpackDir));
                }
                else
                    Console.WriteLine("Skipping non-modpack directory " + modpackType.Name);
            }

            return results;
        }

        private IEnumerable<SideloaderUpdateItem> ProcessDirectory(INode remoteDir, DirectoryInfo localDir)
        {
            var results = new List<SideloaderUpdateItem>();

            var localContents = new List<FileSystemInfo>();
            if (localDir.Exists)
                localContents.AddRange(localDir.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly));

            foreach (var remoteItem in GetSubNodes(remoteDir))
            {
                switch (remoteItem.Type)
                {
                    case NodeType.File:
                        {
                            var localFile = localContents.OfType<FileInfo>().FirstOrDefault(x => string.Equals(x.Name, remoteItem.Name, StringComparison.OrdinalIgnoreCase));
                            if (localFile == null)
                                localFile = new FileInfo(Path.Combine(localDir.FullName, remoteItem.Name));
                            else
                                localContents.Remove(localFile);

                            if (!localFile.Exists)
                                results.Add(new SideloaderUpdateItem(remoteItem, localFile));
                            else
                            {
                                var localIsUpToDate = localFile.Length == remoteItem.Size;
                                //todo re-add and remove the span add?
                                //	&& localFile.CreationTimeUtc + TimeSpan.FromDays(1) >= remoteItem.CreationDate.ToUniversalTime();

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

                            results.AddRange(ProcessDirectory(remoteItem, localItem));
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
