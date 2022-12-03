﻿using MessagePack;

namespace Sideloader.AutoResolver
{
    /// <summary>
    /// Data about the migration to be performed
    /// </summary>
    [MessagePackObject]
    public class MigrationInfo
    {
        /// <summary>
        /// Type of migration to perform
        /// </summary>
        [Key(0)]
        public MigrationType MigrationType;
        /// <summary>
        /// Category of the item
        /// </summary>
        [Key(1)]
        public string Category;
        /// <summary>
        /// GUID of the item to perform migration on
        /// </summary>
        [Key(2)]
        public string GUIDOld;
        /// <summary>
        /// GUID to migrate to
        /// </summary>
        [Key(3)]
        public string GUIDNew;
        /// <summary>
        /// ID of the item to perform migration on
        /// </summary>
        [Key(4)]
        public int IDOld;
        /// <summary>
        /// ID to migrate to
        /// </summary>
        [Key(5)]
        public int IDNew;

        [SerializationConstructor]
        public MigrationInfo(MigrationType migrationType, string category, string guidOld, string guidNew, int idOld, int idNew)
        {
            MigrationType = migrationType;
            Category = category;
            GUIDOld = guidOld;
            GUIDNew = guidNew;
            IDOld = idOld;
            IDNew = idNew;
        }

        public MigrationInfo(MigrationType migrationType, string guidOld, string guidNew)
        {
            MigrationType = migrationType;
            GUIDOld = guidOld;
            GUIDNew = guidNew;
        }
    }
}
