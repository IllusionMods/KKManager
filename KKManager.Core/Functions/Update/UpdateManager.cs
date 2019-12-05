using System;
using System.IO;

namespace KKManager.Functions.Update
{
    public static class UpdateSourceManager
    {
        public static IUpdateSource GetUpdater(Uri link)
        {
            switch (link.Scheme)
            {
                case "file":
                    return new ZipUpdater(new FileInfo(link.LocalPath));
                case "ftp":
                    return new FtpUpdater(link);
                case "https":
                    if (link.Host.ToLower() == "mega.nz")
                        return new MegaUpdater(link, null);
                    throw new NotSupportedException("Host is not supported as an update source: " + link.Host);
                default:
                    throw new NotSupportedException("Link format is not supported as an update source: " + link.Scheme);
            }
        }
    }
}
