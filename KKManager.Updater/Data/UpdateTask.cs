using System;
using System.Collections.Generic;
using System.Linq;
using KKManager.Util;

namespace KKManager.Updater.Data
{
    public sealed class UpdateTask
    {
        public UpdateTask(string taskName, List<IUpdateItem> items, UpdateInfo info, DateTime modifiedTime)
        {
            ModifiedTime = modifiedTime;
            TaskName = taskName ?? throw new ArgumentNullException(nameof(taskName));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            TotalUpdateSize = FileSize.SumFileSizes(Items.Select(x => x.ItemSize));
        }

        public string TaskName { get; }
        public bool EnableByDefault => Info.IsEnabledByDefault();
        public FileSize TotalUpdateSize { get; }
        public bool UpToDate => Items.Count == 0;
        public DateTime? ModifiedTime { get; }
        public UpdateInfo Info { get; }

        public List<IUpdateItem> Items { get; }
        public List<UpdateTask> AlternativeSources { get; } = new List<UpdateTask>();

        public IGrouping<string, Tuple<UpdateInfo, IUpdateItem>>[] GetUpdateItems()
        {
            // Find identical versions of items available at other sources
            var alternativeItems = AlternativeSources
                .SelectMany(task => task.Items.Where(altItem => Items.Any(localItem => string.Equals(localItem.TargetPath.FullName, altItem.TargetPath.FullName, StringComparison.OrdinalIgnoreCase) && localItem.ItemSize == altItem.ItemSize))
                .Select(item => new Tuple<UpdateInfo, IUpdateItem>(task.Info, item)));

            return Items.Select(x => new Tuple<UpdateInfo, IUpdateItem>(Info, x)).Concat(alternativeItems).GroupBy(item => item.Item2.TargetPath.FullName.ToLower()).ToArray();
        }
    }
}
