using System;
using System.Collections.Generic;
using System.Linq;
using KKManager.Util;

namespace KKManager.Updater.Data
{
    public sealed class UpdateTask
    {
        public UpdateTask(string taskName, List<UpdateItem> items, UpdateInfo info, DateTime modifiedTime)
        {
            ModifiedTime = modifiedTime;
            TaskName = taskName ?? throw new ArgumentNullException(nameof(taskName));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public string TaskName { get; }
        public bool EnableByDefault => Info.IsEnabledByDefault();

        public FileSize TotalUpdateSize => FileSize.FromBytes(Items.Where(x => !x.UpToDate && x.RemoteFile != null).Sum(x => x.RemoteFile.ItemSize));

        public bool UpToDate => Items.Count == 0;
        public DateTime? ModifiedTime { get; }
        public UpdateInfo Info { get; }

        public List<UpdateItem> Items { get; }
        public List<UpdateTask> AlternativeSources { get; } = new List<UpdateTask>();

        public IGrouping<string, Tuple<UpdateInfo, UpdateItem>>[] GetUpdateItems()
        {
            // Find identical versions of items available at other sources
            var alternativeItems = AlternativeSources.SelectMany(
                task => task.Items.Where(
                    altItem => Items.Any(
                        localItem => PathTools.PathsEqual(localItem.TargetPath.FullName, altItem.TargetPath.FullName) && localItem.RemoteFile?.ItemSize == altItem.RemoteFile?.ItemSize))
                        .Select(item => new Tuple<UpdateInfo, UpdateItem>(task.Info, item)))
                        .ToList();

            var mergedItems = Items.Select(x => new Tuple<UpdateInfo, UpdateItem>(Info, x)).Concat(alternativeItems).GroupBy(item => item.Item2.TargetPath.FullName.ToLower()).ToArray();
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Updates missing a mirror
                var _ = Items.Select(x => x.TargetPath.FullName).Except(alternativeItems.Select(x => x.Item2.TargetPath.FullName)).ToList();
                //System.Diagnostics.Debugger.Break();
            }
#endif
            return mergedItems;
        }
    }
}