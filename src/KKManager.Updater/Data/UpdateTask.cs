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
            string NormalizePath(UpdateItem x) => PathTools.NormalizePath(x.TargetPath.FullName).Replace('/', '\\').ToLowerInvariant();

            // Add all items from the primary source
            var results = Items.ToDictionary(NormalizePath, item => new List<Tuple<UpdateInfo, UpdateItem>> { new Tuple<UpdateInfo, UpdateItem>(Info, item) });

            // Find identical versions of items available at other sources
            foreach (var alternativeSource in AlternativeSources)
            {
                foreach (var alternativeSourceItem in alternativeSource.Items)
                {
                    if (results.TryGetValue(NormalizePath(alternativeSourceItem), out var list))
                    {
                        if (list[0].Item2.RemoteFile?.ItemSize == alternativeSourceItem.RemoteFile?.ItemSize)
                        {
                            list.Add(new Tuple<UpdateInfo, UpdateItem>(alternativeSource.Info, alternativeSourceItem));
                        }
                    }
                }
            }
            
            // Convert from dict to igrouping
            return results.SelectMany(x => x.Value.Select(y => new { x.Key, y })).GroupBy(x => x.Key, x => x.y).ToArray();
        }
    }
}