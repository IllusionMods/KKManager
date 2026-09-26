using System;
using System.Collections.Generic;
using System.Linq;
using Sideloader.AutoResolver;

namespace KKManager.Data.Zipmods
{
    /// <summary>Builds the set of GUIDs satisfied by the currently installed zipmods.</summary>
    public static class ZipmodRequirementChecker
    {
        public static HashSet<string> GetInstalledGuids(IEnumerable<SideloaderModInfo> zipmods)
        {
            var allZipmods = zipmods.ToList();
            var installed = allZipmods.Select(x => x.Guid).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // A migration can satisfy an older GUID only when its replacement is installed.
            foreach (var migration in allZipmods.SelectMany(x => x.Manifest.MigrationList)
                         .Where(x => !string.IsNullOrEmpty(x.GUIDOld) &&
                                     (x.MigrationType == MigrationType.StripAll || installed.Contains(x.GUIDNew))))
                installed.Add(migration.GUIDOld);

            return installed;
        }

        public static string[] GetMissing(IEnumerable<string> requiredGuids, ISet<string> installedGuids) =>
            requiredGuids.Where(x => !string.IsNullOrWhiteSpace(x))
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .Where(x => !installedGuids.Contains(x))
                         .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                         .ToArray();
    }
}
