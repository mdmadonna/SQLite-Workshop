using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    enum SQLType
    {
        SQLGenCreate,
        SQLGenDropAndCreate,
        SQLGenDrop,
        SQLGenSelect,
        SQLGenInsert,
        SQLGenUpdate,
        SQLGenDelete,
        SQLSelect1000,
        SQLTruncate,
        SQLDrop,
        SQLRename,
        SQLCompress,
        SQLEncrypt,
        SQLIntegrityCheck,
        SQLBackup,
        SQLOptimize,
        SQLClone,
        SQLGenIndex,
        SQLGenAllIndexes,
        SQLDeleteIndex,
        SQLDeleteAllIndexes,
        SQLRebuildIndex,
        SQLRebuildAllIndexes,
        SQLCreateView,
        SQLEditView,
        SQLDeleteView,
        SQLSelect1000View,
        SQLGenViewCreate,
        SQLCreateTrigger,
        SQLEditTrigger,
        SQLGenTriggerCreate,
        SQLDeleteTrigger,
        SQLAddColumn,
        SQLDeleteColumn,
        SQLRenameColumn,
        SQLModifyColumn,
        SQLNewQuery,
        SQLRestore,
        SQLRebuild,
        SQLDelete,
        SQLAttach
    }

    public class MessageLog
    {
        public string MsgDate { get { return DateTime.Now.ToString(); } }
        public string MsgSource { get; set; }
        public string MsgText { get; set; }
        public string MsgExSource { get; set; }
        public string MsgHResult { get; set; }
        public string MsgExText { get; set; }
        public string MsgStackTrace { get; set; }
        public string MsgInnerSource { get; set; }
        public string MsgInnerHResult { get; set; }
        public string MsgInnerText { get; set; }
        public string MsgInnerStackTrace { get; set; }

    }

    internal class JobEventArgs : EventArgs
    {
        internal string StatusMsg;
        internal long LineCount;
    }

    public class ImportData
    {
        public DateTime lastuse { get; set; }
        public string name { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string authtype { get; set; }
    }
       
    class Common
    {
        internal static event EventHandler<JobEventArgs> JobReport;

        /// <summary>
        /// Application StartUp Routines
        /// </summary>
        internal static void ApplicationStartUp()
        {
            // Load Custom Messages
            LoadMessages();

            GblPassword = appSetting(CFG_KEYPHRASE) ?? "SQLite Workshop";

            // Load any Custom BuiltIn SQLite Functions
            string[] fNoLoad = appSetting(Config.CFG_FUNCNOLOAD)?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            Type FuncType = (typeof(Functions));
            Type[] SQLiteFuncTypes = FuncType.GetNestedTypes();
            foreach (Type t in SQLiteFuncTypes)
            {
                object f = Activator.CreateInstance(t);
                var cat = t.GetProperty("Category").GetValue(f);
                if (cat != null)
                {
                    BuiltInFunction bf = new BuiltInFunction()
                    {
                        Name = t.Name,
                        LoadOnOpen = fNoLoad == null || !fNoLoad.Contains(t.Name),
                        Category = cat.ToString(),
                        Description = t.GetProperty("Description").GetValue(f).ToString(),
                        Function = t
                    };
                    DataAccess.FunctionList.Add(bf);
                }
                ((SQLiteFunction)f).Dispose();
            }
        }

        /// <summary>
        /// Load message values from the language file. This can be used for any
        /// language or simply to change a specific message.
        /// </summary>
        internal static void LoadMessages()
        {
            string language = appSetting("CFG_LANGUAGE") ?? "en.txt";
            string fName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "language",language);
            FileInfo fi = new FileInfo(fName);
            if (fi.Exists)
            {
                Type msgtype = typeof(Common);
                string line;
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(fName);

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith("#")) continue;
                        string[] k = line.Split('|');
                        if (k.Length == 2)
                        {
                            try
                            {
                                FieldInfo field = msgtype.GetField(k[0].Trim(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                                field.SetValue(field, k[1].Trim());
                            }
                            catch { LogMsg(string.Format(ERR_LANG_INVALID, language, line)); }
                        }
                        else
                        { LogMsg(string.Format("ERR_LANG_INVALID", language, line)); }
                    }
                }
                catch (Exception ex)
                {
                    LogMsg(string.Format(ERR_LANG_LOAD, language), null, ex);
                }
                finally
                {
                    sr.Close();
                }
            }
        }

        //constants - do not change
        internal const int MAX_SQL_FILESIZE             = 4194304;
        internal const string TOOLS_DIR                 = "tools";
        internal const string TOOLS_UTILITY             = "sqlite3.exe";
        internal const string TOOLS_DIFF                = "sqldiff.exe";
        internal const string TOOLS_ANALYZER            = "sqlite3_analyzer.exe";

        internal static long DEF_ROWEDIT                = 1000;


        #region MessageText
        internal static string APPNAME                  = "SQLite Workshop";
        internal static string TOOLNAMES                = "sqlite3.exe|sqldiff.exe|sqlite3_analyzer.exe";
        internal static string ERR_FILEENTRY            = "Please enter a file name.";
        internal static string LOGFILENAME              = "SQLite_Workshop.log";
        internal static string MSACCESS_PROVIDER        = "Microsoft.ACE.OLEDB.12.0";
        internal static string MSG_FILEEXISTS           = "This file already exists.  Do you want to overwrite it?";
        internal static string MYSQLCONNECTIONCLASS     = "MySql.Data.MySqlClient.MySqlConnection";
        internal static string MYSQLCOMMANDCLASS        = "MySql.Data.MySqlClient.MySqlCommand";
        internal static string MYSQLDATAADAPTERCLASS    = "MySql.Data.MySqlClient.MySqlDataAdapter";
        internal static string MYSQLLIBRARY             = "MySql.Data.dll";
        internal static string NOTIMPLEMENTED           = "This feature is not yet implemented.";
        internal static string OKCANCEL                 = "\r\nPress 'Ok' to continue or 'Cancel' to exit.";
        internal static string StatsTable               = "SQL_Workshop_Stats";
        internal static string WORKING                  = "Working...";


        internal static string OK_ATTACH                = "Attach Ok: DB {0} attached.";
        internal static string OK_BACKUP                = "Backup Ok: Database {0} copied to {1}.";
        internal static string OK_OPTIMIZE              = "Optmize Ok: Database {0} has been optimized.";
        internal static string OK_CLONE                 = "Clone Ok: Database {0} cloned to {1}.";
        internal static string OK_DBCREATED             = "Create Ok: Database {0} created.";
        internal static string OK_DECRYPT               = "Decrypt Ok: Database {0} decrypted.";
        internal static string OK_DELINDEX              = "Delete Ok: Index {0} deleted.";
        internal static string OK_DELALLINDEXES         = "Delete Ok: All Indexes on Table {0} deleted.";
        internal static string OK_DELTRIGGER            = "Delete Ok: Trigger {0} deleted.";
        internal static string OK_DELVIEW               = "Delete Ok: Index {0} deleted.";
        internal static string OK_DUMP                  = "Dump Complete.";
        internal static string OK_ENCRYPT               = "Encrypt Ok: Database {0} encrypted.";
        internal static string OK_EXPLAIN               = "Explain executed successfully.";
        internal static string OK_IDXCREATED            = "Create Ok: Index {0} created.";
        internal static string OK_OPTIONS               = "Options Saved.";
        internal static string OK_QUERY                 = "Query Executed Successfully.";
        internal static string OK_RECORDSAFFECTED       = "{0} records affected.";
        internal static string OK_RECOVERY              = "Recovery Complete.";
        internal static string OK_REINDEX               = "Reindex Ok: Index {0} rebuilt.";
        internal static string OK_REINDEXALL            = "Reindex Ok: All indexes on Table {0} rebuilt.";
        internal static string OK_RENAME                = "Rename Ok: Table {0} renamed.";
        internal static string OK_RESTORE               = "Restore Ok: DB {0} restored.";
        internal static string OK_SQL                   = "SQL Executed Successfully.";
        internal static string OK_TBLDELETE             = "Delete Ok: Table {0} deleted.";
        internal static string OK_VACUUM                = "Compress Ok: Database {0} compressed.";
        internal static string OK_PARSE                 = "The statement was successfully parsed.";

        #region Messages
        internal static string MB_DELETE_ERROR          = "Error {0}\n{1}";
        internal static string MB_CAPTION               = "SQLite Workshop";
        internal static string MB_SELECTDB              = "You must select a Database before executing this command";
        internal static string MB_SELECTQRY             = "You must open a Query Tab before executing this command";

        #endregion

        #region Warning Messages
        internal static string WARN_DBEDITOR            = "WARNING!!!.  This editor does not provide any data validation.  Data will be stored as entered.  If you enter the wrong format (i.e. text in a datetime column), programs using this data may experience unpredictable results.\n\r\n\r\nIMPORTANT!!! Enter datetime columns as yyyy-mm-dd hh:mm:ss.\n\r\n\r\nPress 'Ok' to continue.";
        internal static string WARN_EXPCANCEL           = "Click 'Yes' to Cancel Export.";
        internal static string WARN_NOTADBORENCRYPT     = "Database {0} is either encrypted or not a database.\r\nPlease enter the correct password or press [Cancel].";
        internal static string WARN_NOTADB              = "Database {0} is either encrypted or not a database.\r\nPress [Yes] to continue or [No] to cancel.";
        internal static string WARN_NOTFOUND            = "Not Found";

        internal static string WARN_RECOVER             = "This feature will attempt to recover a corrupt database. The 'Dump' option uses SQL to recover data and will halt at the first error.  The 'Recover' option reads native database pages and is much more Comprehensive.\r\n";
        internal static string WARN_COMPONENT           = "Additional components are required.  They will be downloaded and installed.\r\n";
        #endregion

        #region Error Messages
        internal static string ERR_ATTACH               = "Attach Failed";
        internal static string ERR_BACKUPFAILED         = "Backup Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_BACKUPSAMEFILE       = "Source database and target database are the same file. Select a different target file.";
        internal static string ERR_BADRECORD            = "Improperly formatted record found.\r\n{0}";
        internal static string ERR_CANCELLED            = "Cancelled.";
        internal static string ERR_CANTCREATEOUTPUT     = "Cannot create output file {0}\r\n{1}";
        internal static string ERR_CANTDELETE           = "{0} cannot be deleted.";
        internal static string ERR_CANTLOADTOOLS        = "The SQLite Tools cannot be loaded. You will need to\r\ninstall them manually.";
        internal static string ERR_CANTOPENINPUT        = "Cannot open input file {0}.\r\nError - {1}";
        internal static string ERR_CANTRENAME           = "Cannot rename {0}\r\n{1}";
        internal static string ERR_CLONEFAILED          = "Clone Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_CONFIGCHANGED        = "The configuration file has been changed by another program.";
        internal static string ERR_CREATEDBFAILED       = "Create Database Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_CREATEIDXFAILED      = "Create Index Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_ENCRYPTFAILED        = "Encryption Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_ENCRYPTKEY           = "Encryption Key is Required."; 
        internal static string ERR_EXPLAIN              = "Explain Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_EXPLAINERR           = "Explain executed with errors.";
        internal static string ERR_FATALERR             = "SQLite Workshop has encountered a fatal error.\r\n{0}";
        internal static string ERR_FORMATERROR          = "Formatting errors were detected.  Not all rows are viewable.";
        internal static string ERR_FORMATERRORCOUNT     = "{0} Format Errors detected.";
        internal static string ERR_GENERAL              = "Error: {0}.";
        internal static string ERR_IMPDBOPEN            = "Database Open Error.\r\n{0}";
        internal static string ERR_INVALIDLIMIT         = "Invalid Limit Parameter. Please Re-enter.";
        internal static string ERR_INVALIDSCHEMA        = "{0} is not a valid Schema Name. Please Re-enter.";
        internal static string ERR_LANG_INVALID         = "Invalid Item in language file {0}\r\n{1}";
        internal static string ERR_LANG_LOAD            = "Cannot load language file {0}";
        internal static string ERR_LOGOPEN              = "Message Log Open Error.\r\n{0}"; 
        internal static string ERR_LOGWRITE             = "Message Log Write Error.\r\n{0}";
        internal static string ERR_MSACCESSOPENERR      = "MS Access Open Error.\r\n{0}\r\nPlease insure you have the correct {1} bit driver\r\n{2}";
        internal static string ERR_METADATA             = "Error retrieving metadata.\r\n{0}";
        internal static string ERR_MULTIUPDATE          = "This update cannot be made because it would cause multiple rows to be modified.";
        internal static string ERR_NEEDRECOVERYDB       = "\r\n Select another name for your recovered database.";
        internal static string ERR_NOATTACHENCRYPT      = "Attach is not supported for Encrypted Databases.";
        internal static string ERR_NODISPLAY            = "Unable to display data.";
        internal static string ERR_OLEDBOPENERR         = "OleDB Open Error.\r\n{0}";
        internal static string ERR_ODBCDBOPENERR        = "OdncDB Open Error.\r\n{0}";
        internal static string ERR_OPEN                 = "Open Error: {0}";
        internal static string ERR_PARSE                = "Parse Error\r\n{0}.";
        internal static string ERR_QUERY                = "Query Executed with errors.";
        internal static string ERR_RENAMEFAIL           = "Rename failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_REQUIREDENTRY        = "This field is required.";
        internal static string ERR_RESTOREFAIL          = "Restore failed.\r\n{0}.";
        internal static string ERR_SQL                  = "SQL Execution Failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_SQLWIDE              = "SQL Executed with errors.\r\n\r\n{0} errors encountered.\r\nSQL Error Code: {1}.\r\n\r\nThe last {2} error(s) are:\r\n\r\n{3}";
        internal static string ERR_SQLERR               = "SQL Executed with errors.";
        internal static string ERR_SQLLOADERR           = "SQL Load Error/\r\n{0}";
        internal static string ERR_SQLWHERE             = "Error processing SQL.  Please review your WHERE clause.\r\n{0}";
        internal static string ERR_TBLEXISTS            = "Table {0} Already exists\r\nClick OK to import or Cancel to stop.";
        internal static string ERR_VACUUMFAIL           = "Compress failed: {0}\r\nSQL Error Code: {1}.";
        internal static string ERR_VALIDDB              = "Enter a valid Database Name.";
        internal static string ERR_VALIDDBBKP           = "Enter a valid Backup Database Name.";
        internal static string ERR_VALIDSCHEMA          = "Enter a valid Schema Name.";
        #endregion

        #region Import Messages
        internal static string IMP_START                = "Starting Import.\r\n";
        internal static string IMP_COMPLETE             = "Import Complete.\r\n";
        internal static string IMP_FAILED               = "Import Failed.\r\n";
        #endregion

        internal static string EXP_CANCEL               = "Export Cancelled.";
        internal static string EXP_COMPLETE             = "Export Complete.";
        internal static string EXP_FAILED               = "Export Failed.";

        #region Execute Form Strings
        internal static string EXEC_ATTACHDB            = "Select Database To Attach";
        internal static string EXEC_SELECTDB            = "Select Database Backup File";
        internal static string EXEC_SELECTCLONE         = "Select Cloned Database File";
        internal static string TITLE_TRUNCATE           = "Truncate Table";
        internal static string TITLE_DROP               = "Drop Table";
        internal static string TITLE_RENAME             = "Rename Table";
        internal static string TITLE_COMPRESS           = "Compress Database";
        internal static string TITLE_ENCRYPT            = "Encrypt Database";
        internal static string TITLE_BACKUP             = "Backup Database";
        internal static string TITLE_OPTIMIZE           = "Optimize Database";
        internal static string TITLE_CLONE              = "Clone Database";
        internal static string TITLE_DELETEINDEX        = "Delete Index";
        internal static string TITLE_DELETEINDEXES      = "Delete All Indexes";
        internal static string TITLE_DELETEVIEW         = "Delete View";
        internal static string TITLE_REBUILDINDEX       = "Rebuild Index";
        internal static string TITLE_REBUILDALLINDEXES  = "Rebuild All Indexes";
        internal static string TITLE_DELETETRIGGER      = "Delete Trigger";
        internal static string TITLE_RESTOREDB          = "Restore Database";
        internal static string TITLE_ATTACHDB           = "Attach Database to {0}";
        #endregion

        #region Execute Warning Messages
        internal static string TRUNCATEWARNING          = "WARNING!!  This action will delete all rows from {0} in Database {1}.  Once deleted, these rows cannot be recovered.";
        internal static string DROPWARNING              = "WARNING!!  This action will delete Table {0} from Database {1}.  Once deleted, this table cannot be recovered.";
        internal static string RENAMEWARNING            = "WARNING!!  This action will rename Table {0} in Database {1}. Any Views that refer to this table will need to be recreated after the table is renamed.  Additionally, any Triggers that execute statements that refer to this table may need to be recreated.";
        internal static string COMPRESSWARNING          = "WARNING!!  This action will compress (vacuum) Database {0} to reorganize and recover unused space. Depending on the size of the database, this may take a long time.";
        internal static string ENCRYPTWARNING           = "WARNING!!  This action will Encrypt Database {0}. Encryption may not be supported on all platforms or by all interfaces - use it at your own risk. Enter blanks to remove encryption.\r\n\r\nMAKE A BACKUP FIRST!!";
        internal static string BACKUPWARNING            = "WARNING!!  This action will Backup {0}. Depending on the size of your database, this may take some time.";
        internal static string OPTIMIZEWARNING          = "WARNING!!  This action will attempt to optimize {0}. This process generally runs quickly.";
        internal static string CLONEWARNING             = "WARNING!!  This action will Clone {0}. The cloned database will contain all data structures found in this database but no data will be copied.";
        internal static string DELALLINDEXWARNING       = "WARNING!!  This action will Delete all indexes on table {0}. ";
        internal static string DELINDEXWARNING          = "WARNING!!  This action will Delete index {0}.";
        internal static string DELVIEWWARNING           = "WARNING!!  This action will Delete View {0}. Once deleted, this view cannot be recovered.";
        internal static string REINDEXWARNING           = "WARNING!!  This action will Rebuild index {0}.";
        internal static string REINDEXALLWARNING        = "WARNING!!  This action will Rebuild all indexes for table {0}.";
        internal static string DELTRIGGERWARNING        = "WARNING!!  This action will Delete Trigger {0}. Once deleted, this trigger cannot be recovered.";
        internal static string RESTOREDBWARNING         = "WARNING!!  This action will rename database {0} and copy an existing backup to {0}. Exclusive use of the database is required.";
        internal static string EXECCONTINUE             = "Press Yes to continue or Cancel to terminate.";
        #endregion

        internal static string DBRECOVERY               = "Corrupt Database Recovery";
        #endregion

        #region Helper Routines

        #region keywords
        /// <summary>
        /// List of SQLite reserved words in upper case.
        /// </summary>
        internal static string[] keywords = new string[] {
            "ABORT",
            "ACTION",
            "ADD",
            "AFTER",
            "ALL",
            "ALTER",
            "ANALYZE",
            "AND",
            "AS",
            "ASC",
            "ATTACH",
            "AUTOINCREMENT",
            "BEFORE",
            "BEGIN",
            "BETWEEN",
            "BY",
            "CASCADE",
            "CASE",
            "CAST",
            "CHECK",
            "COLLATE",
            "COLUMN",
            "COMMIT",
            "CONFLICT",
            "CONSTRAINT",
            "CREATE",
            "CROSS",
            "CURRENT_DATE",
            "CURRENT_TIME",
            "CURRENT_TIMESTAMP",
            "DATABASE",
            "DEFAULT",
            "DEFERRABLE",
            "DEFERRED",
            "DELETE",
            "DESC",
            "DETACH",
            "DISTINCT",
            "DROP",
            "EACH",
            "ELSE",
            "END",
            "ESCAPE",
            "EXCEPT",
            "EXCLUSIVE",
            "EXISTS",
            "EXPLAIN",
            "FAIL",
            "FOR",
            "FOREIGN",
            "FROM",
            "FULL",
            "GLOB",
            "GROUP",
            "HAVING",
            "IF",
            "IGNORE",
            "IMMEDIATE",
            "IN",
            "INDEX",
            "INDEXED",
            "INITIALLY",
            "INNER",
            "INSERT",
            "INSTEAD",
            "INTERSECT",
            "INTO",
            "IS",
            "ISNULL",
            "JOIN",
            "KEY",
            "LEFT",
            "LIKE",
            "LIMIT",
            "MATCH",
            "NATURAL",
            "NO",
            "NOT",
            "NOTNULL",
            "NULL",
            "OF",
            "OFFSET",
            "ON",
            "OR",
            "ORDER",
            "OUTER",
            "PLAN",
            "PRAGMA",
            "PRIMARY",
            "QUERY",
            "RAISE",
            "RECURSIVE",
            "REFERENCES",
            "REGEXP",
            "REINDEX",
            "RELEASE",
            "RENAME",
            "REPLACE",
            "RESTRICT",
            "RIGHT",
            "ROLLBACK",
            "ROW",
            "SAVEPOINT",
            "SELECT",
            "SET",
            "TABLE",
            "TEMP",
            "TEMPORARY",
            "THEN",
            "TO",
            "TRANSACTION",
            "TRIGGER",
            "UNION",
            "UNIQUE",
            "UPDATE",
            "USING",
            "VACUUM",
            "VALUES",
            "VIEW",
            "VIRTUAL",
            "WHEN",
            "WHERE",
            "WITH",
            "WITHOUT" };
        #endregion

        /// <summary>
        /// Determine if a word is an SQLite reserved word.
        /// </summary>
        /// <param name="token">Word to check.</param>
        /// <returns>True if the word is a reserved word, otherwise false.</returns>
        internal static bool IsKeyword(string token)
        {
            return keywords.Contains(token.ToUpper());
        }

        /// <summary>
        /// Retrieve a setting from the configuration file.
        /// </summary>
        /// <param name="setting">Name of the setting to retrieve</param>
        /// <returns>value of the setting or null if the setting is not found.</returns>
        internal static string appSetting(string setting)
        {
            return MainForm.cfg.appSetting(setting);
        }

        /// <summary>
        /// Save a setting to the configuration file.  If the setting does not
        /// exist it will be created.
        /// </summary>
        /// <param name="setting">Name of the setting to add or update.</param>
        /// <param name="value">Value of the setting.</param>
        internal static void saveSetting(string setting, string value)
        {
            MainForm.cfg.setSetting(setting, value);
        }

        /// <summary>
        /// Determine if external components have beed downloaded and installed.
        /// </summary>
        /// <returns>true if installed, otherwise false</returns>
        internal static bool ComponentsFound()
        {
            string[] componentnames = TOOLNAMES.Split('|');
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SQLite_Workshop", TOOLS_DIR);
            foreach (string name in componentnames)
            {
                FileInfo fi = new FileInfo(Path.Combine(path, name));
                if (!fi.Exists) return false;
            }
            return true;
        }

        /// <summary>
        /// Get the uri of the Tools zip at sqlite.org then download it and place
        /// the executables in the appdata directory
        /// </summary>
        /// <returns>true id successful, otherwise false</returns>
        internal static bool LoadComponents()
        {
            string ToolsUrl = null;
            string DlFile = null;

            // Fetch the url of the tools location.  This is initially defined on Github so if the parameter
            // is not defined, assume we can get it from the default url below.
            string szUrl = appSetting(Config.CFG_TOOLSLOCATION);
            if (string.IsNullOrEmpty(szUrl)) szUrl = @"https://raw.githubusercontent.com/mdmadonna/SQLite-Workshop/master/sqlite.md";

            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            try
            {
                Stream data = client.OpenRead(szUrl);
                using (StreamReader sr = new StreamReader(data))
                {
                    ToolsUrl = sr.ReadToEnd();
                    sr.Close();
                    ToolsUrl = ToolsUrl.Replace("\n", string.Empty).Replace("\r", string.Empty);
                }
            }
            catch (Exception ex)
            {
                LogMsg("Tools Location Lookup Failed", null, ex);
                return false;
            }

            // Next we will download the actual tools archive
            try
            {
                DlFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
                client.DownloadFile(ToolsUrl, DlFile);
            }
            catch (Exception ex)
            {
                LogMsg("Tools Download Failed", null, ex);
                if (new FileInfo(DlFile).Exists) KillFile(DlFile);
                return false;
            }
            finally { client.Dispose(); }

            // Lastly, let's move the actual tools to the ProgramData Application Folder
            string ToolsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SQLite_Workshop", TOOLS_DIR);
            Directory.CreateDirectory(ToolsDir);

            try
            {
                var z = ZipFile.OpenRead(DlFile);
                foreach (ZipArchiveEntry entry in z.Entries)
                {
                    if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(ToolsDir, Path.GetFileName(entry.FullName)));
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
                z.Dispose();
            }
            catch (Exception ex)
            {
                LogMsg("Tools Extract Failed", null, ex);
                return false;
            }
            finally
            {
                KillFile(DlFile);
            }
            return true;
        }
        /// <summary>
        /// List of characters that we won't allow in column names.
        /// </summary>
        static readonly string invalidChars = "'\"@?\\%";

        /// <summary>
        /// Determine is text contains invalid characters.
        /// </summary>
        /// <param name="token">Text to check.</param>
        /// <param name="chars">Invalid character found in text.</param>
        /// <returns>True if the string contains invalid characters, otherwise false.</returns>
        internal static bool HasInvalidChars(string token, out string chars)
        {
            chars = invalidChars;
            return token.IndexOfAny(invalidChars.ToCharArray()) != -1;
        }
  
        
        /// <summary>
        /// Search a line of SQL to determine if it is complete by finding a semicolon ';'
        /// at the end of the line and insuring that it is not within a quoted string.
        /// </summary>
        /// <param name="line">Full or partial SQL Statement</param>
        /// <returns>true if it is a complete SQL Statement, otherwise false</returns>
        internal static bool FoundEndMarker(string line)
        {
            // Will need to rework this routine.  It currently assumes that a " is the only
            // delimeter but SQLite uses ",',` and perhaps others.  Right now I'll assume that
            // we have a complete sql statement if the line ends with a semi-colon.  We still
            // need to determine if the semi-colon is part of a quoted string.  This routine
            // will always work unless a semi-colon appears at the end of a line but is actually
            // part of a quoted string.

            if (line.Trim().ToLower().StartsWith("create trigger")) return line.ToLower().EndsWith("end;");
            return line.EndsWith(";");
            /*
            if (!line.TrimEnd().EndsWith(";")) return false;

            bool Quoted = false;
            int i;
            int j;

            i = line.IndexOf("\"");
            while (i >= 0)
            {
                if (Quoted)
                {
                    // The line ends with a ';' so we know this won't generate an exception
                    if (line[i + 1] == '\"')
                    { i++; }
                    else
                    { Quoted = !Quoted; }
                }
                else
                {
                    Quoted = !Quoted;
                }
                j = line.Substring(i + 1).IndexOf("\"");
                i = j < 0 ? j : i + j + 1;
            }
            return !Quoted;
            */
        }

        /// <summary>
        /// General purpose routine to display informative messages in a Dialog Box.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="buttons">Buttons that will be displayed in the Dialog Box.</param>
        /// <param name="icon">Icon to be diaplayed in the Dialog Box.</param>
        /// <returns>DiaglogResult representing the button presses by the user.</returns>
        internal static DialogResult ShowMsg(string message, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            return MessageBox.Show(message, APPNAME, buttons, icon);
        }

        /// <summary>
        /// List of SQLite system tables.
        /// </summary>
        static readonly string[] SysTables = new string[] { "sqlite_master", "sqlite_stat1", "sqlite_sequence", "sqlite_stat2", "sqlite_stat3", "sqlite_stat4" };

        /// <summary>
        /// Determine if a string (table name) is a valid SQLite system table.
        /// </summary>
        /// <param name="tableName">Name of the SQLite table to check.</param>
        /// <returns>True if the name is an SQLite system table, otherwise false.</returns>
        internal static bool IsSystemTable(string tableName)
        {
            return SysTables.Contains(tableName);
        }

        #region Get UNC
        /// <summary>
        /// Helper routines to determine the Universal Naming Convention(UNC) version of a file name.
        /// This routine is used to insure a file will not be copied on top of itself.
        /// </summary>
        /// <param name="lpLocalPath"></param>
        /// <param name="dwInfoLevel"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="lpBufferSize"></param>
        /// <returns></returns>
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int WNetGetUniversalName(
        string lpLocalPath,
        [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
        IntPtr lpBuffer,
        [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);

        const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
        const int REMOTE_NAME_INFO_LEVEL = 0x00000002;

        const int ERROR_MORE_DATA = 234;
        const int NOERROR = 0;

        /// <summary>
        /// Retrieve a Universal Name for a file.  This routine will fail if a 
        /// UNC file name or a local file name is passed.
        /// </summary>
        /// <param name="localPath">Local name of the file.</param>
        /// <returns>Universal file name or null</returns>
        internal static string GetUniversalName(string localPath)
        {

            string fileName = null;
            IntPtr buffer = IntPtr.Zero;
            int returnCode;
            int size = 0;

            try
            {
                returnCode = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);
                if (returnCode != ERROR_MORE_DATA) return null;
                buffer = Marshal.AllocCoTaskMem(size);
            }
            catch { return null; }

            try
            {
                returnCode = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref size);
                if (returnCode != NOERROR) return null;
                // and pass to PtrToStringAnsi.
                fileName = Marshal.PtrToStringAuto(new IntPtr(buffer.ToInt64() + IntPtr.Size), size);
                fileName = fileName.Substring(0, fileName.IndexOf('\0'));
            }
            catch { }
            finally
            {  Marshal.FreeCoTaskMem(buffer); }
            return fileName;
        }

        /// <summary>
        /// Generate a table name that is not in use within the current database;
        /// </summary>
        /// <returns>Table name that can safely be used in a CREATE statement.</returns>
        internal static string TempTableName()
        {
            string tmpTable = "SW_TempTable";
            string workTable = tmpTable;
            string sql;

            for (int i = 0; i < 100; i++)
            {
                sql = string.Format("Select Count(*) From sqlite_master Where name = \"{0}\"", workTable);
                long count = (long)DataAccess.ExecuteScalar(MainForm.mInstance.CurrentDB, sql, out SQLiteErrorCode _);
                if (count == 0) return tmpTable;
                workTable = string.Format("{0}_{1}", tmpTable, i.ToString());
            }
            return string.Empty;

        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="szFile">Name of file to delete</param>
        /// <returns>True if the file is deleted, otherwise false.</returns>
        static internal bool KillFile(string szFile)
        {
            FileInfo f = new FileInfo(szFile);
            try { f.Delete(); } catch { return false; }
            return true;
        }

        private static readonly string FILE_EXISTS = "The Target File already exists.";
        /// <summary>
        /// Rename a file. The new file name cannot exist.  Delete it
        /// before using this method.
        /// </summary>
        /// <param name="oldFile">File to rename</param>
        /// <param name="newFile">New file name</param>
        /// <param name="msg">Error Message</param>
        /// <returns>true if successful, otherwise false</returns>
        static internal bool RenameFile(string oldFile, string newFile, out string msg)
        {
            // Make sure all objects that might maintain a lock on the DB
            // are disposed.
            GC.Collect();
            FileInfo fn = new FileInfo(newFile);
            if (fn.Exists)
            {
                msg = FILE_EXISTS;
                return false;
            }
            FileInfo f = new FileInfo(oldFile);
            try { f.MoveTo(newFile); } 
            catch (Exception ex)
            {
                msg = ex.Message;
                return false; 
            }
            msg = string.Empty;
            return true;
        }

        /// <summary>
        /// Rename a file by appending an number to the end of it..
        /// </summary>
        /// <param name="oldFile">File to rename.</param>
        /// <param name="newname">Returns new file name.</param>
        /// <param name="msg">Error message</param>
        /// <returns>true if file is renamed, otherwise false</returns>
        static internal bool RenameFile(string oldFile, out string newname, out string msg)
        {
            int i = 1;
            string newfile = Path.Combine(Path.GetDirectoryName(oldFile), string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(oldFile), i, Path.GetExtension(oldFile)));

            while (!RenameFile(oldFile, newfile, out string message))
            {
                if (message != FILE_EXISTS)
                {
                    msg = message;
                    newname = string.Empty;
                    return false;
                }
                i++;
                newfile = Path.Combine(Path.GetDirectoryName(oldFile), string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(oldFile), i, Path.GetExtension(oldFile)));
            }
            msg = string.Empty;
            newname = newfile;
            return true;
        }

        /// <summary>
        /// Class use to pass data to the GetFileName Method
        /// </summary>
        internal class FileDialogInfo
        {
            internal bool CheckFileExists = true;
            internal bool AddExtension = true;
            internal bool AutoUpgradeEnabled = true;
            internal string Title = APPNAME;
            internal string DefaultExt;
            internal string Filter;
            internal int FilterIndex;
            internal string InitialDirectory;
            internal bool RestoreDirectory = true;
            internal bool Multiselect = false;
            internal bool ShowReadOnly = false;
            internal bool ValidateNames = true;
        };
        

        /// <summary>
        /// Find a file on local or network attached disk.
        /// </summary>
        /// <returns></returns>
        internal static string GetFileName(FileDialogInfo fi)
        {
            
            OpenFileDialog openFile = new OpenFileDialog
            {
                CheckFileExists = fi.CheckFileExists,
                AddExtension = fi.AddExtension,
                AutoUpgradeEnabled = fi.AutoUpgradeEnabled,
                Title = fi.Title,
                DefaultExt = fi.DefaultExt,
                Filter = fi.Filter,
                FilterIndex = fi.FilterIndex,
                RestoreDirectory = fi.RestoreDirectory,
                Multiselect = fi.Multiselect,
                ShowReadOnly = fi.ShowReadOnly,
                ValidateNames = fi.ValidateNames
            };
            if (!string.IsNullOrEmpty(fi.InitialDirectory)) openFile.InitialDirectory = fi.InitialDirectory;
            if (openFile.ShowDialog() != DialogResult.OK) return string.Empty;
            return openFile.FileName;
        }

        /// <summary>
        /// Insure the source file and target file are not the same physical
        /// files.
        /// </summary>
        /// <param name="source">Source File Name</param>
        /// <param name="target">Target File Name</param>
        /// <param name="overwrite">When true, do not check if the target file exists</param>
        /// <param name="ErrorMsg">Error message</param>
        /// <returns></returns>
        internal static bool ValNoOverwrite(string source, string target, out string ErrorMsg, bool overwrite = false)
        {
            ErrorMsg = string.Empty;
            if (string.IsNullOrEmpty(target))
            {
                ErrorMsg  = ERR_FILEENTRY;
                return false;
            }

            // If the target does not exist, it is ok to create and write to it
            FileInfo f = new FileInfo(target);
            if (!f.Exists) return true;

            // Let's make sure it's OK to overwrite and let's insure we're not writing on top of the same file
            string UNCSource;
            string UNCTarget;

            Uri uriDB = new Uri(source);
            UNCSource = uriDB.IsUnc ? source : GetUniversalName(source);
            if (string.IsNullOrEmpty(UNCSource)) UNCSource = source;
            uriDB = new Uri(target);
            UNCTarget = uriDB.IsUnc ? target : GetUniversalName(target);
            if (string.IsNullOrEmpty(UNCTarget)) UNCTarget = target;

            if (UNCSource.ToLower() == UNCTarget.ToLower())
            {
                ShowMsg(ERR_BACKUPSAMEFILE);
                return false;
            }

            if (!overwrite)
            {
                DialogResult result = ShowMsg(MSG_FILEEXISTS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return false;
            }
            return true;
        }


        internal class ProcessData
        {
            internal string FileName;
            internal string WorkingDirectory = null;
            internal string Parms;
            internal string Input;
            internal ProcessWindowStyle WindowStyle;
            internal bool WaitForCompletion;
            internal string Domain = null;
            internal string UserName = null;
            internal string Password = null;
            internal Process p;
            internal bool RedirectOutput;
            internal string OutputFile = string.Empty;
            internal bool LoadUserProfile = false;
            internal string Output;
            internal string errorOutput;
            internal string errorMsg;
            internal int ExitCode;
        }

        internal static void SubmitJob(ref ProcessData p)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = p.FileName,
                WindowStyle = p.WindowStyle,
                RedirectStandardInput = true,
                UseShellExecute = false,
                RedirectStandardOutput = p.RedirectOutput,
                RedirectStandardError = p.RedirectOutput
            };

            if (p.Parms != null) startInfo.Arguments = p.Parms;
            if (p.WorkingDirectory != null) startInfo.WorkingDirectory = p.WorkingDirectory;
            if (p.WindowStyle == ProcessWindowStyle.Hidden) startInfo.CreateNoWindow = true;
            if (p.LoadUserProfile) startInfo.LoadUserProfile = p.LoadUserProfile;
            if (!string.IsNullOrEmpty(p.Domain)) startInfo.Domain = p.Domain;
            if (!string.IsNullOrEmpty(p.UserName)) startInfo.UserName = p.UserName;
            //if (!string.IsNullOrEmpty(p.Password)) startInfo.Password = MakeSecureString(p.Password);

            p.p = new Process();
            {
                p.p.StartInfo.CreateNoWindow = true;
                p.p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            };
            p.p = Process.Start(startInfo);
            if (!string.IsNullOrEmpty(p.Input))
            {
                StreamWriter sw = p.p.StandardInput;
                sw.WriteLine(p.Input);
                sw.Close();
            }
            if (p.RedirectOutput)
            {
                if (string.IsNullOrEmpty(p.OutputFile))
                { p.Output = p.p.StandardOutput.ReadToEnd(); }
                else
                {
                    long lines = 0;
                    StreamWriter sw = new StreamWriter(p.OutputFile);
                    string sz;
                    while ((sz = p.p.StandardOutput.ReadLine()) != null)
                    { 
                        sw.WriteLine(sz);
                        lines++;
                        if (lines % 100 == 0) FireJobEvent(string.Empty, lines);
                    }
                    sw.Close();
                    FireJobEvent(string.Empty, lines);
                }
            }
            if (p.RedirectOutput) p.errorOutput = p.p.StandardError.ReadToEnd();
            if (p.WaitForCompletion) p.p.WaitForExit();
            p.ExitCode = p.p.ExitCode;
            return;
        }

        protected static void FireJobEvent(string status, long RecordCount)
        {
            JobEventArgs e = new JobEventArgs
            {
                StatusMsg = status,
                LineCount = RecordCount
            };
            EventHandler<JobEventArgs> eventHandler = JobReport;
            eventHandler(null, e);
        }


        #endregion
        #endregion

        #region Type Checking
        internal static string[] TextTypes          = new string[] { "char", "nchar", "varchar", "nvarchar", "text" };
        internal static string[] IntegerTypes       = new string[] { "autoincrement", "bigint", "boolean", "int", "mediumint", "smallint", "tinyint", "unsigned" };
        internal static string[] RealTypes          = new string[] { "double", "float", "real" };
        internal static string[] BooleanTypes       = new string[] { "boolean" };
        internal static string[] NumericTypes       = new string[] { "boolean", "decimal", "numeric"};
        internal static string[] DateTypes          = new string[] { "date", "datetime" };
        internal static string[] BlobTypes          = new string[] { "blob", "clob" };
        internal static string[] NumberTypes        = new string[] { "bigint", "boolean", "decimal", "double", "float", "int", "mediumint", "numeric", "real", "smallint", "tinyint", "unsigned" };
        internal static string[] SQLiteTypes        = TextTypes.Concat(IntegerTypes).Concat(RealTypes).Concat(BooleanTypes).Concat(NumericTypes).Concat(DateTypes).Concat(NumberTypes).Concat(BlobTypes).ToArray().Distinct().ToArray();

        /// <summary>
        /// Determine if a column type has Text affinity.  Used to determine if column should be wrapped in quotes.
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column has text affinity, False if not</returns>
        internal static bool IsText(string ColType)
        {
            foreach (string szType in TextTypes)
            {
                if (ColType.ToLower().StartsWith(szType.ToLower())) return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if a column type has Integer affinity.  
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column has Integer affinity, False if not</returns>
        internal static bool IsInteger(string ColType)
        {
            foreach (string szType in IntegerTypes)
            {
                if (ColType.ToLower().StartsWith(szType)) return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if a column type has Real affinity.  
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column has Real affinity, False if not</returns>
        internal static bool IsReal(string ColType)
        {
            foreach (string szType in RealTypes)
            {
                if (ColType.ToLower().StartsWith(szType)) return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if a column type is boolean.  
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column is boolean, False if not</returns>
        internal static bool IsBoolean(string ColType)
        {
            foreach (string szType in BooleanTypes)
            {
                if (ColType.ToLower().StartsWith(szType)) return true;
            }
            return false;
        }
        /// <summary>
        /// Determine if a column type has Numeric affinity. Note that this routine excludes Boolean and Dates
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column has Numeric affinity, False if not</returns>
        internal static bool IsNumeric(string ColType)
        {
            foreach (string szType in NumericTypes)
            {
                if (ColType.ToLower().StartsWith(szType)) return true;
            }
            return false;
        }
        /// <summary>
        /// Determine if a column type is a Date type with Numeric affinity.  
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column is a Date type, False if not</returns>
        internal static bool IsDate(string ColType)
        {
            foreach (string szType in DateTypes)
            {
                if (ColType.ToLower().StartsWith(szType)) return true;
            }
            return false;
        }


        /// <summary>
        /// Determine if a column type has Integer, Real, or Numeric affinity.  
        /// </summary>
        /// <param name="ColType">Valid SQLite Column Type.</param>
        /// <returns>True if the column has affinity to a numeric value, False if not</returns>
        internal static bool IsNumber(string ColType)
        {
            foreach (string szType in NumberTypes)
            {
                if (ColType.ToLower().StartsWith(szType)) return true;
            }
            return false;
        }

        internal static bool IsSelect(string sql)
        {
            string strippedSql = RemoveComments(sql, true).Trim().ToLower();

            if (strippedSql.StartsWith("select") || strippedSql.StartsWith("pragma") || strippedSql.StartsWith("with")) return true;
            return false;
        }

        /// <summary>
        /// Convert a string to a Byte array
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>Btye Array</returns>
        internal static byte[] ToBytes(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }

        private static readonly Regex everythingExceptNewLines = new Regex("[^\r\n]");
        private static string RemoveComments(string input, bool preservePositions, bool removeLiterals = false)
        {

            var lineComments = @"--(.*?)\r?\n";
            var lineCommentsOnLastLine = @"--(.*?)$"; // because it's possible that there's no \r\n after the last line comment
                                                      // literals ('literals'), bracketedIdentifiers ([object]) and quotedIdentifiers ("object"), they follow the same structure:
                                                      // there's the start character, any consecutive pairs of closing characters are considered part of the literal/identifier, and then comes the closing character
            var literals = @"('(('')|[^'])*')"; // 'John', 'O''malley''s', etc
            var bracketedIdentifiers = @"\[((\]\])|[^\]])* \]"; // [object], [ % object]] ], etc
            var quotedIdentifiers = @"(\""((\""\"")|[^""])*\"")"; // "object", "object[]", etc - when QUOTED_IDENTIFIER is set to ON, they are identifiers, else they are literals
                                                                  //var blockComments = @"/\*(.*?)\*/";  //the original code was for C#, but Microsoft SQL allows a nested block comments // //https://msdn.microsoft.com/en-us/library/ms178623.aspx
                                                                  //so we should use balancing groups // http://weblogs.asp.net/whaggard/377025
            var nestedBlockComments = @"/\*
                                (?>
                                /\*  (?<LEVEL>)      # On opening push level
                                | 
                                \*/ (?<-LEVEL>)     # On closing pop level
                                |
                                (?! /\* | \*/ ) . # Match any char unless the opening and closing strings   
                                )+                         # /* or */ in the lookahead string
                                (?(LEVEL)(?!))             # If level exists then fail
                                \*/";

            string noComments = Regex.Replace(input,
                    nestedBlockComments + "|" + lineComments + "|" + lineCommentsOnLastLine + "|" + literals + "|" + bracketedIdentifiers + "|" + quotedIdentifiers,
                me => {
                    if (me.Value.StartsWith("/*") && preservePositions)
                        return everythingExceptNewLines.Replace(me.Value, " "); // preserve positions and keep line-breaks // return new string(' ', me.Value.Length);
            else if (me.Value.StartsWith("/*") && !preservePositions)
                        return "";
                    else if (me.Value.StartsWith("--") && preservePositions)
                        return everythingExceptNewLines.Replace(me.Value, " "); // preserve positions and keep line-breaks
            else if (me.Value.StartsWith("--") && !preservePositions)
                        return everythingExceptNewLines.Replace(me.Value, ""); // preserve only line-breaks // Environment.NewLine;
            else if (me.Value.StartsWith("[") || me.Value.StartsWith("\""))
                        return me.Value; // do not remove object identifiers ever
            else if (!removeLiterals) // Keep the literal strings
                return me.Value;
                    else if (removeLiterals && preservePositions) // remove literals, but preserving positions and line-breaks
            {
                        var literalWithLineBreaks = everythingExceptNewLines.Replace(me.Value, " ");
                        return "'" + literalWithLineBreaks.Substring(1, literalWithLineBreaks.Length - 2) + "'";
                    }
                    else if (removeLiterals && !preservePositions) // wrap completely all literals
                return "''";
                    else
                        throw new NotImplementedException();
                },
                RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            return noComments;
        }

        #endregion

        #region encryption
        static string _gblpassword = null;
        static readonly byte[] _salt = new byte[] { 0x05, 0x6A, 0xE7, 0x6e, 0x20, 0x00, 0x01, 0x67, 0x76, 0x65, 0x9a, 0x03, 0x6F };

        internal static string GblPassword
        {
            get { return _gblpassword; }
            set { _gblpassword = value; }
        }
        /// <summary>
        /// Decrypt a string - initialize password before using
        /// </summary>
        /// <param name="cipherText">string to decrypt</param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        internal static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(_gblpassword)) throw new Exception("Password not initialized.");
            if (string.IsNullOrEmpty(cipherText)) return null;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(_gblpassword, _salt);
            byte[] dd = Decrypt(cipherBytes, db.GetBytes(32), db.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(dd);
        }
        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = null;
            try
            {
                Rijndael alg = Rijndael.Create();
                alg.Key = Key;
                alg.IV = IV;
                cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(cipherData, 0, cipherData.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
            }
        }

        /// <summary>
        /// Encrypt a string - initialize password before using
        /// </summary>
        /// <param name="clearText">string to encrypt</param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        internal static string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(_gblpassword)) throw new Exception("Password not initialized.");
            if (string.IsNullOrEmpty(clearText)) return null;
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(_gblpassword, _salt);
            byte[] encryptedData = Encrypt(clearBytes, db.GetBytes(32), db.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }


        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = null;
            try
            {
                Rijndael alg = Rijndael.Create();
                alg.Key = Key;
                alg.IV = IV;
                cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearData, 0, clearData.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
            }
        }
        #endregion

        #region TableData

        internal static TableLayout FindTableLayout(string DatabaseName, string tablename)   //mdm 12/2020
        {
            SchemaDefinition sd = DataAccess.GetSchema(DatabaseName);        //mdm 12/2020
            return sd.Tables[tablename];
        }

        internal static ColumnLayout FindColumnLayout(string DatabaseName, string tablename, string columnname)  //mdm 12/2020
        {
            SchemaDefinition sd = DataAccess.GetSchema(DatabaseName);                       //mdm 12/2020
            TableLayout tl = sd.Tables[tablename];
            return tl.Columns[columnname];
        }

        static readonly string[] boolvalues = new string[] { "0", "1", "true", "false" };
        /// <summary>
        /// Validate data based on datatype and convert it to SQLite acceptable value
        /// </summary>
        /// <param name="datatype"></param>   Type of database column
        /// <param name="datavalue"></param>  Value entered 
        /// <param name="szValue"></param>    Value to be inserted into DB
        /// <returns></returns>
        internal static bool ValidateData(string datatype, string datavalue, out string szValue)
        {
            szValue = "";
            if (IsBoolean(datatype))
            {
                if (boolvalues.Contains(datavalue.ToLower().Trim()))
                {
                    szValue = datavalue;
                    return true;
                }
            }
            else if (IsDate(datatype))
            {
                if (DateTime.TryParse(datavalue, out DateTime newdate))
                {
                    szValue = newdate.ToString("yyyy-MM-dd HH:mm:ss");
                    return true;
                }
            }
            else if (IsInteger(datatype))
            {
                if (int.TryParse(datavalue, out int _))
                {
                    szValue = datavalue;
                    return true;
                }
            }
            else if (IsNumber(datatype))
            {
                if (double.TryParse(datavalue, out double newdbl))
                {
                    szValue = newdbl.ToString();
                    return true;
                }
            }
            else
            {
                szValue = datavalue;
                return true;
            }

            return false;

        }

        #endregion

        #region Error Handling
        /// <summary>
        /// All Unhandled Exceptions end up here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal static void ErrorHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            ShowMsg(string.Format(ERR_FATALERR, e.Message));
            LogMsg(e.Message, null, e);
        }

        private static bool bLogError = false;
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static bool LogMsg(string Message, string Source = null, Exception ex = null)
        {
            StreamWriter sw = null;
            string LogFile = string.Format(@"{0}\{1}\{2}", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SQLite_Workshop", LOGFILENAME);
            try
            {
                sw = new StreamWriter(LogFile, true);
            }
            catch (Exception logEx)
            {
                if (bLogError) return false;
                bLogError = true;
                ShowMsg(string.Format(ERR_LOGOPEN, logEx.Message));
            }

            try
            {
                MessageLog ml = new MessageLog
                {
                    MsgText = Message
                };
                if (string.IsNullOrEmpty(Source))
                {
                    var st = new StackTrace(new StackFrame(1));
                    ml.MsgSource = st.GetFrame(0).GetMethod().Name;
                }
                ml.MsgExSource = ex?.Source;
                ml.MsgHResult = ex?.HResult.ToString();
                ml.MsgExText = ex?.Message;
                ml.MsgStackTrace = ex?.StackTrace?.ToString();
                ml.MsgInnerSource = ex?.InnerException?.Source;
                ml.MsgHResult = ex?.InnerException?.HResult.ToString();
                ml.MsgInnerText = ex?.InnerException?.Message;
                ml.MsgInnerStackTrace = ex?.InnerException?.StackTrace?.ToString();
                string msg = JsonSerializer.Serialize(ml);
                sw.WriteLine(msg);
            }
            catch (Exception logEx)
            {
                if (!bLogError) ShowMsg(string.Format(ERR_LOGWRITE, logEx.Message));
            }
            finally
            {
                sw.Close();
            }
            return true;
        }
        #endregion

       
    }
}
