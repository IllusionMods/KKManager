﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKManager.Util;

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

        public static async Task<List<UpdateTask>> GetUpdates(CancellationToken cancellationToken, params IUpdateSource[] updateSources)
        {
            var results = new ConcurrentBag<UpdateTask>();

            // First start all of the sources, then wait until they all finish
            var concurrentTasks = updateSources.Select(source => RetryHelper.RetryOnExceptionAsync(
                async () =>
                {
                    foreach (var task in await source.GetUpdateItems(cancellationToken))
                        results.Add(task);
                },
                3, TimeSpan.FromSeconds(3), cancellationToken)).ToList();

            foreach (var task in concurrentTasks)
                await task;

            cancellationToken.ThrowIfCancellationRequested();

            var filteredTasks = new List<UpdateTask>();
            foreach (var modGroup in results.GroupBy(x => x.Info.Guid))
            {
                var ordered = modGroup.OrderByDescending(x => x.ModifiedTime ?? DateTime.MinValue).ToList();
                if (ordered.Count > 1)
                {
                    ordered[0].AlternativeSources.AddRange(ordered.Skip(1));
                    Console.WriteLine($"Found {ordered.Count} entries for mod GUID {modGroup.Key} - latest is from {ordered[0].Info.Origin}");
                }
                filteredTasks.Add(ordered[0]);
            }
            return filteredTasks;
        }

        public static IUpdateSource[] GetUpdateSources(string searchDirectory)
        {
            var updateSourcesPath = Path.Combine(searchDirectory, "UpdateSources");

            if (!File.Exists(updateSourcesPath))
            {
                Console.WriteLine("The UpdateSources file is missing, updating will not be available");
                return new IUpdateSource[0];
            }

            Console.WriteLine("Found UpdateSources file at " + updateSourcesPath);

            var updateSources = File.ReadAllLines(updateSourcesPath).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            var results = new List<IUpdateSource>(updateSources.Count);
            foreach (var updateSource in updateSources)
            {
                try { results.Add(UpdateSourceManager.GetUpdater(new Uri(updateSource))); }
                catch (Exception ex) { Console.WriteLine($"Could not open update source: {updateSource} - {ex}"); }
            }

            if (results.Count < updateSources.Count)
                Console.WriteLine($"Could not open {updateSources.Count - results.Count} out of {updateSources.Count} update sources, check log for details");

            return results.ToArray();
        }
    }
}
