namespace Sideloader.AutoResolver
{
    /// <summary>
    /// Type of migration that will be performed
    /// </summary>
    public enum MigrationType
    {
        /// <summary>
        /// Change the old GUID to the new GUID and the old ID to the new ID
        /// </summary>
        Migrate,
        /// <summary>
        /// Change the old GUID to the new GUID for all IDs
        /// </summary>
        MigrateAll,
        /// <summary>
        /// Remove the GUID and perform compatibility resolve
        /// </summary>
        StripAll
    };
}