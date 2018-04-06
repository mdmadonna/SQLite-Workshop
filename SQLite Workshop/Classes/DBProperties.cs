using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    internal class DBProperties
    {

        private string[] dbPragmaList = new string[]
        {
            "encoding",
            "foreign_key_check",
            "foreign_key_list",
            "freelist_count",
            "page_count",
            "page_size",
            "schema_version",
            "user_version",
        };

        private string[] pragmaList = new string[]
        {
            "application_id",
            "auto_vacuum",
            "automatic_index",
            "busy_timeout",
            "cache_size",
            "cache_spill",
            "case_sensitive_like",
            "cell_size_check",
            "checkpoint_fullfsync",
            "collation_list",
            "compile_options",
            "data_version",
            "database_list",
            "defer_foreign_keys",
            //"encoding",
            //"foreign_key_check",
            //"foreign_key_list",
            "foreign_keys",
            //"freelist_count",
            "fullfsync",
            "function_list",
            "ignore_check_constraints",
            //"incremental_vacuum",
            //"index_info",
            //"index_list",
            //"index_xinfo",
            //"integrity_check",
            "journal_mode",
            "journal_size_limit",
            "legacy_file_format",
            "locking_mode",
            "max_page_count",
            "mmap_size",
            "module_list",
            //"optimize",
            //"page_count",
            //"page_size",
            //"parser_trace",
            "pragma_list",
            "query_only",
            //"quick_check",
            "read_uncommitted",
            "recursive_triggers",
            "reverse_unordered_selects",
            //"schema_version",
            "secure_delete",
            //"shrink_memory",
            //"soft_heap_limit",
            //"stats",
            "synchronous",
            //"table_info",
            "temp_store",
            "threads",
            //"user_version",
            //"vdbe_addoptrace",
            //"vdbe_debug",
            //"vdbe_listing",
            //"vdbe_trace",
            "wal_autocheckpoint",
            //"wal_checkpoint",
            "writable_schema"
        };

        SchemaDefinition sd;
        internal DBPropertySettings dbprops;
        internal DBRuntimePropertySettings dbRT;
        protected string DatabaseLocation;

        internal DBProperties()
        {
            DatabaseLocation = MainForm.mInstance.CurrentDB;
            LoadDBProperties();
            LoadPragmaProperties();
            LoadRuntimeProperties();
        }
        ~DBProperties()
        { }

        protected void LoadDBProperties()
        {
            sd = DataAccess.GetSchema(DatabaseLocation);
            dbprops = new DBPropertySettings();

            dbprops.DbFileName = DatabaseLocation;
            dbprops.DbName = sd.DBName;
            dbprops.DbSize = sd.DBSize.ToString();
            dbprops.DbCreateDate = sd.CreateDate.ToString();
            dbprops.DbLastUpdate = sd.LastUpDate.ToString();
            dbprops.DbTables = sd.Tables.Count.ToString();
            dbprops.DbViews = sd.Views.Count.ToString();
            
            int idxCount = 0;
            foreach (var table in sd.Tables) { idxCount += table.Value.Indexes.Count; }
            dbprops.DbIndexes = idxCount.ToString();
            dbprops.DbTriggers = sd.Triggers.Count.ToString();
        }

        protected void LoadPragmaProperties()
        {
            string sql;
            SQLiteErrorCode returnCode;
            DataTable dt;

            foreach (string option in dbPragmaList)
            {
                sql = string.Format("Pragma {0}", option);
                dt = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
                if (returnCode == SQLiteErrorCode.Ok) { InitProperty(option, dt); continue; }
                Common.ShowMsg(String.Format("Error executing Pragma {0}.\r\n{1}", option, DataAccess.LastError));
            }
        }

        protected void InitProperty( string propName, DataTable dt)
        {
            switch (propName)
            {
                case "encoding":
                    dbprops.DbEncoding = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "foreign_key_check":
                    dbprops.FKCheckList = dt.Rows.Count == 0 ? new string[] { string.Empty } : FormatOptionsArray(dt);
                    break;
                case "foreign_key_list":
                    dbprops.FKList = dt.Rows.Count == 0 ? new string[] { string.Empty } : FormatOptionsArray(dt);
                    break;
                case "freelist_count":
                    dbprops.DbFreeListCount = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "page_count":
                    dbprops.DbPageCount = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "page_size":
                    dbprops.DbPageSize = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "schema_version":
                    dbprops.DbSchemaVersion = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "user_version":
                    dbprops.DbUserVersion = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                default:
                    break;
            }
        }

        protected void LoadRuntimeProperties()
        {
            dbRT = new DBRuntimePropertySettings();
            string sql;
            SQLiteErrorCode returnCode;
            DataTable dt;

            foreach (string option in pragmaList)
            {
                sql = string.Format("Pragma {0}", option);
                dt = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
                if (returnCode == SQLiteErrorCode.Ok) InitRuntimeProperty(option, dt);
            }
        }

        protected void InitRuntimeProperty(string propName, DataTable dt)
        {
            switch (propName)
            {
                case "application_id":
                    dbRT.DbApplicationID = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "auto_vacuum":
                    dbRT.DbAutoVacuum = dt.Rows.Count == 0 ? string.Empty : FormatAutoVacuum(dt.Rows[0]);
                    break;
                case "automatic_index":
                    dbRT.DbAutomaticIndex = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "busy_timeout":
                    dbRT.DbBusyTimeout = dt.Rows.Count == 0 ? string.Empty : string.Format("{0} ms", FormatValue(dt.Rows[0]));
                    break;
                case "cache_size":
                    dbRT.DbCacheSize = dt.Rows.Count == 0 ? string.Empty : FormatCacheSize(dt.Rows[0]);
                    break;
                case "cache_spill":
                    dbRT.DbCacheSpill = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "case_sensitive_like":
                    dbRT.DbCaseSensitiveLike = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "cell_size_check":
                    dbRT.DbCellSizeCheck = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "checkpoint_fullfsync":
                    dbRT.DbCheckpointFullfsync = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "collation_list":
                    dbRT.DbCollationList = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "compile_options":
                    dbRT.CompileOptionsList = dt.Rows.Count == 0 ? new string[] { string.Empty } : FormatOptionsArray(dt);
                    break;
                case "data_version":
                    dbRT.DbDataVersion = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "database_list":
                    dbRT.DbDatabaseList = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "defer_foreign_keys":
                    dbRT.DbDeferForeignKeys = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "foreign_keys":
                    dbRT.DbForeignKeys = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "fullfsync":
                    dbRT .DbFullfsync= dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "function_list":
                    dbRT.FunctionList = dt.Rows.Count == 0 ? new string[] { string.Empty } : FormatOptionsArray(dt);
                    break;
                case "ignore_check_constraints":
                    dbRT.DbIgnoreCheckConstraints = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "journal_mode":
                    dbRT.DbJournalMode = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]).ToUpper();
                    break;
                case "journal_size_limit":
                    dbRT.DbJournalSizeLimit = dt.Rows.Count == 0 ? string.Empty : FormatJournalSizeLimit(dt.Rows[0]);
                    break;
                case "legacy_file_format":
                    dbRT.DbLegacyFileFormat = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "locking_mode":
                    dbRT.DbLockingMode = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "max_page_count":
                    dbRT.DbMaxPageCount = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "mmap_size":
                    dbRT.DbMemoryMapSize = dt.Rows.Count == 0 ? string.Empty : FormatMMapSize(dt.Rows[0]);
                    break;
                case "module_list":
                    dbRT.ModuleList = dt.Rows.Count == 0 ? new string[] { string.Empty } : FormatOptionsArray(dt);
                    break;
                case "pragma_list":
                    dbRT.PragmaList = dt.Rows.Count == 0 ? new string[] { string.Empty } : FormatOptionsArray(dt);
                    break;
                case "query_only":
                    dbRT.DbQueryOnly = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "read_uncommitted":
                    dbRT.DbReadUncommitted = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "recursive_triggers":
                    dbRT.DbRecursive_Tiggers = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "reverse_unordered_selects":
                    dbRT.DbReverseUnorderedSelects = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "secure_delete":
                    dbRT.DbSecureDelete = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                case "synchronous":
                    dbRT.DbSynchronous = dt.Rows.Count == 0 ? string.Empty : FormatSynchronous(dt.Rows[0]);
                    break;
                case "temp_store":
                    dbRT.DbTempStore = dt.Rows.Count == 0 ? string.Empty : FormatTempStore(dt.Rows[0]);
                    break;
                case "threads":
                    dbRT.DbThreads = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "wal_autocheckpoint":
                    dbRT.DbWalAutoCheckpoint = dt.Rows.Count == 0 ? string.Empty : FormatValue(dt.Rows[0]);
                    break;
                case "writable_schema":
                    dbRT.DbWritableSchema = dt.Rows.Count == 0 ? string.Empty : FormatValueOnOff(dt.Rows[0]);
                    break;
                default:
                    break;
            }
        }

        private string FormatValue(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(dr.ItemArray[0] == null ? string.Empty : dr.ItemArray[0].ToString());
            for (int i = 1; i < dr.ItemArray.Count(); i++)
            {
                sb.Append("|").Append(dr.ItemArray[i] == null ? string.Empty : dr.ItemArray[0].ToString());
            }
            return sb.ToString();
        }
        private string FormatValueOnOff(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            if (!int.TryParse(dr.ItemArray[0].ToString(), out int value)) return string.Empty;
            return value == 0 ? "Enabled" : "Disabled";
        }

        private string FormatAutoVacuum(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            switch (dr.ItemArray[0].ToString())
            {
                case "0":
                    return "None";
                case "1":
                    return "Full";
                case "2":
                    return "Incremental";
                default:
                    return "Unknown";
            }
        }

        private string FormatCacheSize(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            if (!int.TryParse(dr.ItemArray[0].ToString(), out int cachesize)) return string.Empty;
            return cachesize < 0 ? string.Format("{0} KB", cachesize * -1) : string.Format("{0} Pages", cachesize);
        }

        private string[] FormatOptionsArray(DataTable dt)
        {
            string[] options = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                options[i] = dt.Rows[i].ItemArray[0] == null ? string.Empty : dt.Rows[i].ItemArray[0].ToString();
                for (int j = 1; j < dt.Rows[j].ItemArray.Count(); i++)
                {
                    options[i] = string.Format("{0} | {1}", options[i], dt.Rows[i].ItemArray[j].ToString());
                }
            }
            return options;
        }

        private string FormatJournalSizeLimit(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            if (!int.TryParse(dr.ItemArray[0].ToString(), out int sizelimit)) return string.Empty;
            return sizelimit < 0 ? "No Limit" : string.Format("{0} Bytes", sizelimit);
        }

        private string FormatMMapSize(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            if (!int.TryParse(dr.ItemArray[0].ToString(), out int mmapsize)) return string.Empty;
            return mmapsize == 0 ? "Disabled" : mmapsize < 0 ? "Default" : string.Format("{0} Bytes", mmapsize);
        }

        private string FormatSynchronous(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            switch (dr.ItemArray[0].ToString())
            {
                case "0":
                    return "Off";
                case "1":
                    return "Normal";
                case "2":
                    return "Full";
                case "3":
                    return "Extra";
                default:
                    return "Unknown";
            }
        }

        private string FormatTempStore(DataRow dr)
        {
            if (dr.ItemArray[0] == null) return string.Empty;
            switch (dr.ItemArray[0].ToString())
            {
                case "0":
                    return "Default";
                case "1":
                    return "File";
                case "2":
                    return "Memory";
                default:
                    return "Unknown";
            }
        }
    }
}
