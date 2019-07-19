using System;
using System.IO;
using CG.Web.MegaApiClient;

namespace KKManager.Functions
{
	public sealed class SideloaderUpdateTask
	{
		public readonly FileInfo LocalFile;
		public readonly INode RemoteFile;

		/// <summary>
		/// Local file is up to date with remote file and doesn't need to be redownloaded
		/// </summary>
		public readonly bool UpToDate;

		public SideloaderUpdateTask(INode remoteFile, FileInfo localFile, bool upToDate = false)
		{
			DisplayName = localFile.FullName.Substring(InstallDirectoryHelper.KoikatuDirectory.FullName.Length);

			UpToDate = upToDate;
			LocalFile = localFile ?? throw new ArgumentNullException(nameof(localFile));
			RemoteFile = remoteFile;
		}

		public string DisplayName { get; }
		public bool LocalExists => LocalFile.Exists;
		public bool RemoteExists => RemoteFile != null;

		public object UpdateDate
		{
			get
			{
				if (RemoteFile != null) return RemoteFile.CreationDate;
				return "File was removed";
			}
		}

		public override string ToString()
		{
			return $"UpToDate={UpToDate} Local={LocalExists} Remote={RemoteExists} - {DisplayName}";
		}
	}
}
