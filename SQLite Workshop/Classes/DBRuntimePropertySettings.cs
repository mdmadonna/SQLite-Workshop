using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBRuntimePropertySettings
    {

        [DisplayName("Application ID"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The 32-bit unsigned big-endian \"Application ID\" integer located at offset 68 into the database header.")]
        public string DbApplicationID { get; set; }


        [DisplayName("Auto Vacuum"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current auto_vacuum setting.")]
        public string DbAutoVacuum { get; set; }

        [DisplayName("Automatic Index"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current automatic_index setting.")]
        public string DbAutomaticIndex { get; set; }

        [DisplayName("Busy Timeout"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current busy_timeout setting in milliseconds.")]
        public string DbBusyTimeout { get; set; }

        [DisplayName("Cache Size"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The suggested maximum number of database disk pages that SQLite will hold in memory per open database file.")]
        public string DbCacheSize { get; set; }

        [DisplayName("Cache Spill"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current cache_spill setting.")]
        public string DbCacheSpill { get; set; }

        [DisplayName("Case Sensitive LIKE"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("When enabled (value = 1), SQLite will ignore case.")]
        public string DbCaseSensitiveLike { get; set; }

        [DisplayName("Cell Size Check"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("cell_size_check enables or disables additional sanity checking on database b-tree pages as they are initially read from disk.")]
        public string DbCellSizeCheck { get; set; }

        [DisplayName("Checkpoint Fullfsync"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current checkpoint_fullfsunc setting (MacOS only).")]
        public string DbCheckpointFullfsync { get; set; }

        [DisplayName("Collation List"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The collating sequences defined for the current database connection.")]
        public string DbCollationList { get; set; }

        [DisplayName("Compile Options"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The names of compile-time options used when building SQLite.")]
        public string DbCompileOptions { get; set; }

        [DisplayName("Data Version"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the data_version setting.")]
        public string DbDataVersion { get; set; }

        [DisplayName("Database List"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("A list of currently attached databases.")]
        public string DbDatabaseList { get; set; }

        [DisplayName("Defer Foreign Keys"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the defer_foreign_keys setting.")]
        public string DbDeferForeignKeys { get; set; }

        [DisplayName("Foreign Keys"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the foreign_keys setting. When disabled (value = 0) Foreign Keys are NOT enforced.")]
        public string DbForeignKeys { get; set; }

        [DisplayName("Full Fsync"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the fullfsync setting (MacOS only).")]
        public string DbFullfsync { get; set; }

        [DisplayName("Function List"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("A list of SQL functions known to the database connection.  This is only available if SQLite is built using the -DSQLITE_INTROSPECTION_PRAGMAS compile-time option")]
        public string DbFunctionList { get; set; }

        [DisplayName("Ignore Check Constraints"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the ignore_check_constraints setting. When OFF (value = 0) CHECK constraints ARE enforced.")]
        public string DbIgnoreCheckConstraints { get; set; }

        [DisplayName("Journal Mode"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current journal_mode setting.")]
        public string DbJournalMode { get; set; }

        [DisplayName("Journal Size Limit"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the journal_size_limit setting.")]
        public string DbJournalSizeLimit { get; set; }

        [DisplayName("Legacy File Format"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the legacy_file_format setting.  This is used to determine the format used to create new databases.  It does not indicate which format was used to create the current database.")]
        public string DbLegacyFileFormat { get; set; }

        [DisplayName("Locking Mode"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the locking_mode setting.")]
        public string DbLockingMode { get; set; }

        [DisplayName("Max Page Count"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The maximum number of pages in this database.")]
        public string DbMaxPageCount { get; set; }

        [DisplayName("Memory Mapped I/O Size"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The maximum number of bytes that are set aside for memory-mapped I/O on a single database.")]
        public string DbMemoryMapSize { get; set; }

        [DisplayName("Module List"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("A list of virtual table modules registered with the database connection. This is only available if SQLite is built using the -DSQLITE_INTROSPECTION_PRAGMAS compile-time option.")]
        public string DbModuleList { get; set; }

        [DisplayName("Pragma List"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("A list of PRAGMA commands known to the database connection. This is only available if SQLite is built using the -DSQLITE_INTROSPECTION_PRAGMAS compile-time option.")]
        public string DbPragmaList { get; set; }

        [DisplayName("Query Only"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the query_only setting.")]
        public string DbQueryOnly { get; set; }

        [DisplayName("Read Uncommitted"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the read_uncommitted isolation setting.")]
        public string DbReadUncommitted { get; set; }

        [DisplayName("Application ID"),
        ReadOnly(true),
        CategoryAttribute("Recursive Triggers"),
        DescriptionAttribute("The current value of the recursive_triggers setting. The depth of recursion for triggers has a hard upper limit set by the SQLITE_MAX_TRIGGER_DEPTH compile-time option.")]
        public string DbRecursive_Tiggers { get; set; }

        [DisplayName("Reverse Unordered Selects"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the reverse_unordered_selects setting.")]
        public string DbReverseUnorderedSelects { get; set; }

        [DisplayName("Secure Delete"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the secure_delete setting.")]
        public string DbSecureDelete { get; set; }


        [DisplayName("synchronous ID"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the synchronous setting.")]
        public string DbSynchronous { get; set; }

        [DisplayName("Temp Store"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The current value of the temp_store setting.")]
        public string DbTempStore { get; set; }

        [DisplayName("Threads"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The upper bound on the number of auxiliary threads that a prepared statement is allowed to launch to assist with a query.")]
        public string DbThreads { get; set; }

        [DisplayName("WAL AutoCheckpoint"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("The number of pages in the Write Ahead Log that will trigger a checkpoint. When 0, AutoCheckpoint is turned off.")]
        public string DbWalAutoCheckpoint { get; set; }

        [DisplayName("Writable Schema"),
        ReadOnly(true),
        CategoryAttribute("Runtime Properties"),
        DescriptionAttribute("When enabled (value = 1) the SQLITE_MASTER tables can be modified using standard SQL. (Extremely HIGH RISK.)")]
        public string DbWritableSchema { get; set; }

    }
}
