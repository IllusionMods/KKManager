using System;
using System.IO;
using CG.Web.MegaApiClient;
using KKManager.Util;

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

            Size = FileSize.FromBytes(remoteFile?.Size ?? 0);
		}

		public string RelativePath { get; }
        public string Name { get; }
		public bool LocalExists => LocalFile.Exists;
		public bool RemoteExists => RemoteFile != null;

        public FileSize Size { get; }

		public DateTime UpdateDate => RemoteFile?.CreationDate ?? DateTime.MinValue;

	    public override string ToString()
		{
			return $"UpToDate={UpToDate} Local={LocalExists} Remote={RemoteExists} - {RelativePath}";
		}
	}
}
