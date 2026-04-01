using System.IO;
using KKManager.Util;

namespace KKManager.Data
{
    public interface IFileInfoBase
    {
        FileInfo Location { get; }
        string Name { get; }
        public FileSize FileSize { get; }
    }
}