using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG.Web.MegaApiClient;

namespace KKManager.Functions
{
	public class SideloaderModpackUpdater : IDisposable
	{
		private readonly MegaApiClient _client;

		private List<INode> _allNodes;

		private SideloaderModpackUpdater(MegaApiClient client)
		{
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

		public static SideloaderModpackUpdater ConnectNew()
		{
			var client = new MegaApiClient();
			client.LoginAnonymous();
			return new SideloaderModpackUpdater(client);
		}

		public void DownloadNode(string fullLocalPath, INode node)
		{
			_client.DownloadFile(node, fullLocalPath);
		}

		public List<INode> GetNodesFromLink(Uri folderLink)
		{
			_allNodes = _client.GetNodesFromLink(folderLink).ToList();
			CurrentFolderLink = folderLink;
			return _allNodes;
		}

		public IEnumerable<INode> GetSubNodes(INode rootNode)
		{
			return _allNodes.Where(x => x.ParentId == rootNode.Id);
		}

		public IEnumerable<SideloaderUpdateTask> GetUpdateTasks()
		{
			var link = new Uri("https://mega.nz/#F!fkYzQa5K!nSc7wkY82OUqZ4Hlff7Rlg");
			var nodes = GetNodesFromLink(link);
			return CollectTasks(nodes);
		}

		private IEnumerable<SideloaderUpdateTask> CollectTasks(List<INode> nodes)
		{
			var root = nodes.Single(x => x.Type == NodeType.Root);
			//var root = nodes.Single(x => x.ParentId == null);

			var modsDirPath = Path.Combine(InstallDirectoryHelper.KoikatuDirectory.FullName, "mods");
			Directory.CreateDirectory(modsDirPath);
			//var modsDir = new DirectoryInfo(modsDirPath);

			var results = Enumerable.Empty<SideloaderUpdateTask>();

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

		private IEnumerable<SideloaderUpdateTask> ProcessDirectory(INode remoteDir, DirectoryInfo localDir)
		{
			var results = new List<SideloaderUpdateTask>();

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
								results.Add(new SideloaderUpdateTask(remoteItem, localFile));
							else
							{
								var localIsUpToDate = localFile.Length == remoteItem.Size;
								//todo re-add and remove the span add?
								//	&& localFile.CreationTimeUtc + TimeSpan.FromDays(1) >= remoteItem.CreationDate.ToUniversalTime();

								results.Add(new SideloaderUpdateTask(remoteItem, localFile, localIsUpToDate));
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
							results.Add(new SideloaderUpdateTask(null, fi));
						break;

					case DirectoryInfo di:
						foreach (var subFi in di.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Extension.ToLowerInvariant().Contains("zip")))
							results.Add(new SideloaderUpdateTask(null, subFi));
						break;
				}
			}

			return results;
		}
	}
}
