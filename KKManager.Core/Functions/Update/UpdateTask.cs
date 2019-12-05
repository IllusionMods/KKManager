using System;
using System.Collections.Generic;
using System.Linq;
using KKManager.Util;

namespace KKManager.Functions.Update
{
    public sealed class UpdateTask
    {
        public UpdateTask(string taskName, List<IUpdateItem> items, UpdateInfo info)
        {
            TaskName = taskName ?? throw new ArgumentNullException(nameof(taskName));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            ModifiedTime = Items.Count == 0 ? (DateTime?)null : Items.Max(x => x.ModifiedTime ?? DateTime.MinValue);
            TotalUpdateSize = FileSize.SumFileSizes(Items.Select(x => x.ItemSize));
        }

        public string TaskName { get; }
        public List<IUpdateItem> Items { get; }
        public bool EnableByDefault => Info.IsEnabledByDefault();

        public FileSize TotalUpdateSize { get; }
        public bool UpToDate => Items.Count == 0;

        public DateTime? ModifiedTime { get; }

        public UpdateInfo Info { get; }
    }
}
