using KKManager.Updater.Data;
using KKManager.Util;

namespace KKManager.Updater.Utils
{
    public static class Extensions
    {
        public static FileSize GetFancyItemSize(this IRemoteItem item) => FileSize.FromBytes(item.ItemSize);
    }
}
