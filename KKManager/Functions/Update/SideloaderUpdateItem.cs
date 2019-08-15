using System;
using System.IO;
using CG.Web.MegaApiClient;

namespace KKManager.Functions.Update
{
	public sealed class SideloaderUpdateItem
	{
		public readonly FileInfo LocalFile;
		public readonly INode RemoteFile;

		/// <summary>
		/// Local file is up to date with remote file and doesn't need to be redownloaded
		/// </summary>
		public readonly bool UpToDate;

		public SideloaderUpdateItem(INode remoteFile, FileInfo localFile, bool upToDate = false)
		{
            UpToDate = upToDate;
		    LocalFile = localFile ?? throw new ArgumentNullException(nameof(localFile));
		    RemoteFile = remoteFile;

            RelativePath = localFile.FullName.Substring(InstallDirectoryHelper.KoikatuDirectory.FullName.Length);
		    Name = localFile.Name;
		}

		public string RelativePath { get; }
        public string Name { get; }
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
			return $"UpToDate={UpToDate} Local={LocalExists} Remote={RemoteExists} - {RelativePath}";
		}
	}
}
