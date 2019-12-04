using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Util;

namespace KKManager.Functions.Update
{
    public sealed class UpdateTask
    {
        public UpdateTask(string taskName, List<IUpdateItem> items, bool enableByDefault)
        {
            EnableByDefault = enableByDefault;
            TaskName = taskName ?? throw new ArgumentNullException(nameof(taskName));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public string TaskName { get; }
        public List<IUpdateItem> Items { get; }
        public bool EnableByDefault { get; }

        public FileSize TotalUpdateSize => FileSize.SumFileSizes(Items.Select(x=>x.ItemSize));
        public bool UpToDate => Items.Count == 0;

        public DateTime ModifiedTime => Items.Max(x => x.ModifiedTime ?? DateTime.MinValue);

        public async Task RunUpdate(CancellationToken cancellationToken)
        {
            foreach (var updateItem in Items)
                await updateItem.Update(cancellationToken);
        }
    }
}
