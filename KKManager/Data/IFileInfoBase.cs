using System.IO;

namespace KKManager.Data
{
    public interface IFileInfoBase
    {
        FileInfo Location { get; }
        string Name { get; }
    }
}