using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    internal enum SQLiteTableType
    {
        system,
        user,
        special
    }

    internal struct SchemaDefinition
    {
        internal int LoadStatus;
        internal SQLiteErrorCode LoadError;
        internal string DBName;
        internal string DBLocation;
        internal long DBSize;
        internal string password;
        internal DateTime CreateDate;
        internal DateTime LastUpDate;
        internal DateTime LastAccess;
        internal Dictionary<string, TableLayout> Tables;
        internal Dictionary<string, ViewLayout> Views;
        internal Dictionary<string, TriggerLayout> Triggers;
    }
    internal struct TableLayout
    {
        internal long rootpage;
        internal SQLiteTableType TblType;
        internal string CreateSQL;
        internal Dictionary<string, long> PrimaryKeys;
        internal Dictionary<string, IndexLayout> Indexes;
        internal Dictionary<string, ColumnLayout> Columns;
        internal Dictionary<string, ForeignKeyLayout> ForeignKeys;
    }

    internal struct IndexLayout
    {
        internal long rootpage;
        internal string TableName;
        internal string CreateSQL;
        internal bool Unique;
        internal string Origin;
        internal bool Partial;
        internal Dictionary<string, IndexColumnLayout> Columns;
    }

    internal struct IndexColumnLayout
    {
        internal long seqno;
        internal long Cid;
        internal string SortOrder;
        internal string CollatingSequence;
        internal long key;
    }

    internal struct ColumnLayout
    {
        internal long Cid;
        internal string ColumnType;
        internal long NullType;
        internal string DefaultValue;
        internal long PrimaryKey;
        internal bool Unique;
        internal string Check;
        internal string Collation;
        internal ForeignKeyLayout ForeignKey;
    }

    internal struct ForeignKeyLayout
    {
        internal long id;
        internal long Sequence;
        internal string Table;
        internal string From;
        internal string To;
        internal string OnUpdate;
        internal string OnDelete;
        internal string Match;
    }

    internal struct ViewLayout
    {
        internal long rootpage;
        internal string CreateSQL;
        internal Dictionary<string, ColumnLayout> Columns;
    }

    internal struct TriggerLayout
    {
        internal long rootpage;
        internal string CreateSQL;
    }

    abstract class DataAccess
    {
        public static event EventHandler<ProgressEventArgs> ProgressReport = delegate { };
        public static void ProgressEventHandler(object sender, ProgressEventArgs e)
        {
            ProgressReport(sender, e);
        }

        internal static Dictionary<string, SchemaDefinition> SchemaDefinitions = new Dictionary<string, SchemaDefinition>();

        internal static string LastError { get; private set; } = null;
        internal static int FormatErrorCount { get; private set; } = 0;
        internal static ArrayList FormatErrors { get; private set; } = null;
        internal const int MAX_ERRORS = 100;

        const string QRY_TABLES = "SELECT * FROM sqlite_master WHERE type = 'table' ORDER BY name";
        const string QRY_VIEWS = "SELECT * FROM sqlite_master WHERE type = 'view' ORDER BY name";
        const string QRY_TRIGGERS = "SELECT * FROM sqlite_master WHERE type = 'trigger' ORDER BY name";
        const string QRY_INDEXES = "SELECT * FROM sqlite_master WHERE type = 'index' AND tbl_name = '{0}' ORDER BY name";
        const string QRY_COLUMNS = "PRAGMA table_info(\"{0}\")";
        const string QRY_INDEXCOLUMNS = "PRAGMA index_xinfo(\"{0}\")";
        const string QRY_INDEXDETAIL = "PRAGMA index_list(\"{0}\")";
        const string QRY_FOREIGNKEYS = "PRAGMA foreign_key_list(\"{0}\")";

        internal static SchemaDefinition GetSchema(string DBLocation)
        {

            SchemaDefinition sd = new SchemaDefinition();

            //SchemaDefinitions is a consideration for future use.  For the time being
            //Clear it before adding a new schema
            SchemaDefinitions.Clear();

            FileInfo f = new FileInfo(DBLocation);
            if (!f.Exists)
                { LastError = "Database not found."; sd.LoadStatus = -1; return sd; }

            if (DataAccess.IsEncrypted(DBLocation))
            {
                GetPassword gp = new GetPassword();
                gp.DBLocation = DBLocation;
                gp.ShowDialog();
                if (gp.Cancelled) { sd.LoadStatus = -1; return sd; }
                sd.password = gp.Password;
            }
            else
            { sd.password = null; }

            sd.DBLocation = DBLocation;
            sd.DBName = f.Name;
            sd.DBSize = f.Length;
            sd.CreateDate = f.CreationTime;
            sd.LastUpDate = f.LastWriteTime;
            sd.LastAccess = f.LastAccessTime;
            SchemaDefinitions.Add(DBLocation, sd);

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out SQLiteErrorCode returnCode))
            { sd.LoadError = returnCode; sd.LoadStatus = -1; return sd; }

            sd.Tables = GetTables(cmd);
            sd.Views = GetViews(cmd);
            sd.Triggers = GetTriggers(cmd);

            CloseDB(conn);
            sd.LoadStatus = 0;
            sd.LoadError = SQLiteErrorCode.Ok;
            SchemaDefinitions[DBLocation] = sd;
            return sd;
        }

        internal static Dictionary<string, TableLayout> GetTables(SQLiteCommand cmd)
        {
            Dictionary<string, TableLayout> TableLayouts = new Dictionary<string, TableLayout>();

            cmd.CommandText = QRY_TABLES;
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                TableLayout tl = new TableLayout();
                tl.rootpage = Convert.ToInt64(dr["rootpage"]);              //Appears to be long in older version and int in the current version
                tl.CreateSQL = dr["sql"].ToString();
                tl.Indexes = GetIndexes(dr["name"].ToString(), cmd);
                tl.Columns = GetColumns(dr["name"].ToString(), cmd);
                tl.ForeignKeys = GetForeignKeys(dr["name"].ToString(), cmd);
                tl.TblType = Common.IsSystemTable(dr["name"].ToString()) ? SQLiteTableType.system : SQLiteTableType.user;
                tl.PrimaryKeys = new Dictionary<string, long>();
                foreach (var column in tl.Columns)
                {
                    if (column.Value.PrimaryKey > 0) tl.PrimaryKeys.Add(column.Key, column.Value.PrimaryKey);
                }
                TableLayouts.Add(dr["name"].ToString(), tl);
            }

            // Special case sqlite_master
            string tblname = "sqlite_master";
            TableLayout tspecial = new TableLayout();
            tspecial.rootpage = 0;
            tspecial.CreateSQL = string.Empty;
            tspecial.Indexes = GetIndexes(tblname, cmd);
            tspecial.Columns = GetColumns(tblname, cmd);
            tspecial.ForeignKeys = GetForeignKeys(tblname, cmd);
            tspecial.TblType = SQLiteTableType.system;
            TableLayouts.Add(tblname, tspecial);

            return TableLayouts;
        }

        internal static Dictionary<string, TableLayout> GetTables(string DBLocation)
        {
            Dictionary<string, TableLayout> TableLayouts = new Dictionary<string, TableLayout>();
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out SQLiteErrorCode returnCode)) return null;

            TableLayouts = GetTables(cmd);

            CloseDB(conn);
            return TableLayouts;
        }

        internal static Dictionary<string, IndexLayout> GetIndexes(string TableName, SQLiteCommand cmd)
        {
            Dictionary<string, IndexLayout> IndexLayouts = new Dictionary<string, IndexLayout>();
            SQLiteErrorCode returnCode;

            cmd.CommandText = string.Format(QRY_INDEXES, TableName);
            DataTable dt = ExecuteDataTable(cmd, out returnCode);
            cmd.CommandText = string.Format(QRY_INDEXDETAIL, TableName);
            DataTable dtDetail = ExecuteDataTable(cmd, out returnCode);

            foreach (DataRow dr in dt.Rows)
            {
                IndexLayout il = new IndexLayout();
                il.rootpage = Convert.ToInt64(dr["rootpage"]);
                il.TableName = dr["name"].ToString();
                il.CreateSQL = dr["sql"].ToString();
                il.Columns = GetIndexColumns(dr["name"].ToString(), cmd);
                foreach (DataRow drDetail in dtDetail.Rows)
                {
                    if (dr["name"].ToString() == drDetail["name"].ToString())
                    {
                        il.Unique = drDetail["unique"].ToString() == "0" ? false : true;
                        il.Origin = drDetail["origin"].ToString();
                        il.Partial = drDetail["partial"].ToString() == "0" ? false : true;
                    }
                }
                IndexLayouts.Add(dr["name"].ToString(), il);
            }

            return IndexLayouts;
        }

        internal static Dictionary<string, IndexLayout> GetIndexes(string DBLocation, string TableName)
        {
            Dictionary<string, IndexLayout> IndexLayouts = new Dictionary<string, IndexLayout>();

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out SQLiteErrorCode returnCode)) return null;

            IndexLayouts = GetIndexes(TableName, cmd);

            CloseDB(conn);
            return IndexLayouts;
        }

        internal static Dictionary<string, ColumnLayout> GetColumns(string TableName, SQLiteCommand cmd)
        {
            Dictionary<string, ColumnLayout> ColumnLayouts = new Dictionary<string, ColumnLayout>();

            cmd.CommandText = string.Format(QRY_COLUMNS, TableName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                ColumnLayout cl = new ColumnLayout();
                cl.Cid = Convert.ToInt64(dr["cid"]);
                cl.ColumnType = dr["type"].ToString();
                cl.NullType = Convert.ToInt64(dr["notnull"]);
                cl.DefaultValue = dr["dflt_value"].ToString();
                cl.PrimaryKey = Convert.ToInt64(dr["pk"]);
                ColumnLayouts.Add(dr["name"].ToString(), cl);
            }

            return ColumnLayouts;
        }

        internal static Dictionary<string, ColumnLayout> GetColumns(string DBLocation, string TableName)
        {
            Dictionary<string, ColumnLayout> ColumnLayouts = new Dictionary<string, ColumnLayout>();

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out SQLiteErrorCode returnCode)) return null;

            ColumnLayouts = GetColumns(TableName, cmd);

            CloseDB(conn);
            return ColumnLayouts;
        }

        internal static Dictionary<string, IndexColumnLayout> GetIndexColumns(string IndexName, SQLiteCommand cmd)
        {
            Dictionary<string, IndexColumnLayout> IndexColumnLayouts = new Dictionary<string, IndexColumnLayout>();

            cmd.CommandText = string.Format(QRY_INDEXCOLUMNS, IndexName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                IndexColumnLayout icl = new IndexColumnLayout();
                icl.seqno = Convert.ToInt64(dr["seqno"]);
                icl.Cid = Convert.ToInt64(dr["cid"]);
                icl.SortOrder = Convert.ToInt32(dr["desc"].ToString()) == 1 ? "Desc" : "Asc";
                icl.CollatingSequence = dr["coll"].ToString();
                icl.key = Convert.ToInt64(dr["key"]);
                IndexColumnLayouts.Add(dr["name"] == null || string.IsNullOrEmpty(dr["Name"].ToString()) ? "rowid" : dr["name"].ToString(), icl);
            }

            return IndexColumnLayouts;
        }

        internal static Dictionary<string, ForeignKeyLayout> GetForeignKeys(string TableName, SQLiteCommand cmd)
        {
            Dictionary<string, ForeignKeyLayout> FKeyLayouts = new Dictionary<string, ForeignKeyLayout>();

            cmd.CommandText = string.Format(QRY_FOREIGNKEYS, TableName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                ForeignKeyLayout fl = new ForeignKeyLayout();
                fl.id = Convert.ToInt64(dr["id"]);
                fl.Sequence = Convert.ToInt64(dr["seq"]);
                fl.Table = dr["table"].ToString();
                fl.From = dr["from"].ToString();
                fl.To = dr["to"].ToString();
                fl.OnUpdate = dr["on_update"].ToString();
                fl.OnDelete = dr["on_delete"].ToString();
                fl.Match = dr["match"].ToString();
                FKeyLayouts.Add(i.ToString(), fl);
                i++;
            }

            return FKeyLayouts;
        }

        internal static Dictionary<string, ForeignKeyLayout> GetForeignKeys(string DBLocation, string TableName)
        {
            Dictionary<string, ForeignKeyLayout> FKeyLayouts = new Dictionary<string, ForeignKeyLayout>();

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out SQLiteErrorCode returnCode)) return null;

            FKeyLayouts = GetForeignKeys(TableName, cmd);

            CloseDB(conn);
            return FKeyLayouts;
        }

        internal static Dictionary<string, ViewLayout> GetViews(SQLiteCommand cmd)
        {
            Dictionary<string, ViewLayout> ViewLayouts = new Dictionary<string, ViewLayout>();

            cmd.CommandText = QRY_VIEWS;
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                ViewLayout vl = new ViewLayout();
                vl.rootpage = Convert.ToInt64(dr["rootpage"]);
                vl.CreateSQL = dr["sql"].ToString();
                vl.Columns = GetColumns(dr["name"].ToString(), cmd);
                ViewLayouts.Add(dr["name"].ToString(), vl);
            }

            return ViewLayouts;
        }

        internal static Dictionary<string, TriggerLayout> GetTriggers(SQLiteCommand cmd)
        {
            Dictionary<string, TriggerLayout> TriggerLayouts = new Dictionary<string, TriggerLayout>();

            cmd.CommandText = QRY_TRIGGERS;
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                TriggerLayout vl = new TriggerLayout();
                vl.rootpage = Convert.ToInt64(dr["rootpage"]);
                vl.CreateSQL = dr["sql"].ToString();
                TriggerLayouts.Add(dr["name"].ToString(), vl);
            }

            return TriggerLayouts;
        }

        internal static Dictionary<string, ViewLayout> GetViews(string DBLocation)
        {
            Dictionary<string, ViewLayout> ViewLayouts = new Dictionary<string, ViewLayout>();

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out SQLiteErrorCode returnCode)) return null;

            ViewLayouts = GetViews(cmd);

            CloseDB(conn);
            return ViewLayouts;
        }
        /// <summary>
        /// Create a new empty SQLite database.
        /// </summary>
        /// <param name="dbName">Fully Qualified file name of the database to create.</param>
        /// <returns></returns>
        internal static bool CreateDB(string dbName)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            SQLiteErrorCode returnCode;

            // The open creates the file
            if (!OpenDB(dbName, ref conn, ref cmd, out returnCode, true)) return false;

            // SQLite appears to create an empty file that stays empty until you put something in it
            // Create a dummy table
            cmd.CommandText = "CREATE TABLE \"$Temp\"(\"ID\" integer );";
            ExecuteNonQuery(cmd, out returnCode);
            //Delete it right away
            cmd.CommandText = "DROP TABLE \"$Temp\";";
            ExecuteNonQuery(cmd, out returnCode);
            CloseDB(conn);
            return true;
        }

        /// <summary>
        /// Encrypt an SQLite database
        /// </summary>
        /// <param name="dbName">Fully Qulified File Name of the SQLite database</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        internal static bool EncryptDB(string dbName, string password, out SQLiteErrorCode returnCode)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            returnCode = SQLiteErrorCode.Ok;

            if (!OpenDB(dbName, ref conn, ref cmd, out returnCode, true)) return false;

            try
            {
                conn.ChangePassword(password);
            }
            catch (Exception ex)
            {
                returnCode = conn.ExtendedResultCode();
                LastError = ex.Message;
                return false;
            }
            finally { CloseDB(conn); }
            return true;
        }

        internal static bool Parse(string dbName, string sql, out SQLiteErrorCode returnCode)
        {

            sql = string.Format("Explain {0}", sql);

            DataTable dt = DataAccess.ExecuteDataTable(dbName, sql, out returnCode);
            if (dt == null)
            {
                returnCode = SQLiteErrorCode.Error;
                return false;
            }
            dt.Dispose();
            returnCode = SQLiteErrorCode.Error;
            return true;

        }

        // caller must open db
        protected static SQLiteDataReader ExecuteDataReader(SQLiteCommand cmd, out SQLiteErrorCode returnCode)
        {
            SQLiteDataReader dr = null;

            try
            {
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
            returnCode = cmd.Connection.ExtendedResultCode();
            return dr;

        }

        internal static SQLiteDataAdapter ExecuteDataAdapter(string DBLocation, string SqlStatement, out DataTable dt, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            dt = null;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return null;
            if (returnCode != SQLiteErrorCode.Ok) return null;

            cmd.CommandText = SqlStatement;
            SQLiteDataAdapter da = ExecuteDataAdapter(cmd, out dt, out returnCode);

            CloseDB(conn);
            return da;
        }

        protected static SQLiteDataAdapter ExecuteDataAdapter(SQLiteCommand cmd, out DataTable dt, out SQLiteErrorCode returnCode)
        {
            dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

            FormatErrorCount = 0;
            FormatErrors = new ArrayList();

            try
            {
                da.FillError += Da_FillError;
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            returnCode = FormatErrorCount == 0 ? cmd.Connection.ExtendedResultCode() : SQLiteErrorCode.Error;
            return da;
        }

        private static void Da_FillError(object sender, FillErrorEventArgs e)
        {
            e.Continue = true;
            FormatErrorCount++;
            if (FormatErrorCount <= MAX_ERRORS) FormatErrors.Add(string.Format("Format Error: {0}", e.Errors.Message));
        }

        internal static DataTable ExecuteDataTable(string DBLocation, string SqlStatement, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return null;

            cmd.CommandText = SqlStatement;
            DataTable dt = ExecuteDataTable(cmd, out returnCode);

            CloseDB(conn);
            return dt;
        }

        internal static DataTable ExecuteDataTable(SQLiteCommand cmd, out SQLiteErrorCode returnCode)
        {
            FormatErrors = new ArrayList();
            FormatErrorCount = 0;
            int recordcount = 0;

            // Cannot use a dataadapter here because of potential format errors
            DataTable dt = new DataTable();

            SQLiteDataReader dr;

            try { dr = cmd.ExecuteReader(); }
            catch (Exception ex)
            {
                LastError = ex.Message;
                returnCode = cmd.Connection.ExtendedResultCode();
                return null;
            }

            for (int i = 0; i < dr.FieldCount; i++)
            {
                //dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                dt.Columns.Add(dr.GetName(i), typeof(string));
            }
            //dt.Columns.AddRange(new DataColumn[dr.FieldCount]);
            try
            {
                while (dr.Read())
                {
                    recordcount++;
                    DataRow dRow = dt.NewRow();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        try
                        {
                            dRow[i] = dr[i] == null ? string.Empty : dr[i].ToString();
                        }
                        catch (Exception ex)
                        {
                            FormatErrorCount++;
                            if ((FormatErrorCount <= MAX_ERRORS)) FormatErrors.Add(string.Format("Format Error on Record {0} Column {1}: {2}", recordcount, dr.GetName(i), ex.Message));
                        }
                    }
                    dt.Rows.Add(dRow);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
            finally { dr.Close(); }
            returnCode = FormatErrorCount > 0 ? SQLiteErrorCode.Error : cmd.Connection.ExtendedResultCode();
            return dt;
        }

        internal static int ExecuteNonQuery(string DBLocation, string SqlStatement, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return -1;

            cmd.CommandText = SqlStatement;
            int result = ExecuteNonQuery(cmd, out returnCode);
            CloseDB(conn);
            return result;
        }

        internal static int ExecuteNonQuery(string DBLocation, string SqlStatement, ArrayList parms, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return -1;
            for (int i = 0; i < parms.Count; i++)
            {
                cmd.Parameters.Add(new SQLiteParameter() { Value = parms[i] });
            }
            cmd.CommandText = SqlStatement;
            int result = ExecuteNonQuery(cmd, out returnCode);
            CloseDB(conn);
            return result;
        }

        internal static int ExecuteNonQuery(string DBLocation, string SqlStatement, ArrayList parms, out long LastInsertID, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            LastInsertID = -1;
            int result;

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return -1;
            for (int i = 0; i < parms.Count; i++)
            {
                cmd.Parameters.Add(new SQLiteParameter() { Value = parms[i] });
            }
            cmd.CommandText = SqlStatement;
            result = ExecuteNonQuery(cmd, out returnCode);
            LastInsertID = conn.LastInsertRowId;
            CloseDB(conn);
            return result;
        }

        internal static int ExecuteNonQuery(SQLiteCommand cmd, out SQLiteErrorCode returnCode)
        {
            int RecordCount = 0;
            returnCode = SQLiteErrorCode.Ok;
            SQLiteTransaction tran;
            tran = cmd.Connection.BeginTransaction();

            try
            {
                RecordCount = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                LastError = ex.Message;
                returnCode = cmd.Connection.ExtendedResultCode();
                return -1;
            }
            return RecordCount;

        }

        internal static object ExecuteScalar(string DBLocation, string SqlStatement, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return null;

            cmd.CommandText = SqlStatement;
            object obj = ExecuteScalar(cmd, out returnCode);

            CloseDB(conn);
            return obj;
        }

        internal static object ExecuteScalar(string DBLocation, string SqlStatement, ArrayList parms, out SQLiteErrorCode returnCode)
        {

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return null;
            cmd.CommandText = SqlStatement;
            for (int i = 0; i < parms.Count; i++)
            {
                cmd.Parameters.Add(new SQLiteParameter() { Value = parms[i] });
            }
            object obj = ExecuteScalar(cmd, out returnCode);

            CloseDB(conn);
            return obj;
        }

        protected static object ExecuteScalar(SQLiteCommand cmd, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;

            object obj = null;
            try
            {
                obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                returnCode = cmd.Connection.ExtendedResultCode();
            }
            return obj;
        }

        /// <summary>
        /// Open or Create an SQLite database
        /// </summary>
        /// <param name="DBLocation">Fully qualified file name of the SQLite database.</param>
        /// <param name="Conn">The SQLite Connection to open.</param>
        /// <param name="Cmd">The SQLite Command associated with the Connection.</param>
        /// <param name="returnCode">The SQLite Result Code returned by the Open request.</param>
        /// <param name="IsNew">True if the database is to be created, False if the database already exists.</param>
        /// <param name="password">The password needed to access an encrypted SQLite database.</param>
        /// <returns>True if successful, False if the open fails</returns>
        internal static bool OpenDB(string DBLocation, ref SQLiteConnection Conn, ref SQLiteCommand Cmd, out SQLiteErrorCode returnCode, bool IsNew = false, string password = null)
        {

            Conn = new SQLiteConnection();
            Conn.ConnectionString = string.Format("Data Source={0};Version=3;New={1}", DBLocation, IsNew ? "True" : "False");
            if (password != null) Conn.ConnectionString += string.Format(";Password={0}", password);

            SchemaDefinition sd;
            if (SchemaDefinitions.TryGetValue(DBLocation, out sd))
                if (!string.IsNullOrEmpty(sd.password)) Conn.ConnectionString += string.Format(";Password={0}", sd.password);

            try
            {
                Uri uriDB = new Uri(DBLocation);
                if (uriDB.IsUnc) Conn.ParseViaFramework = true;
                Conn.Open();
                Conn.SetExtendedResultCodes(true);

                // Initialize ProgressOps before registering the handler.  It will not work if you don't do it this way.
                Conn.ProgressOps = 100;
                Conn.Progress += ProgressEventHandler;
                Cmd = new SQLiteCommand();
                Cmd.Connection = Conn;
                returnCode = SQLiteErrorCode.Ok;
                return true;
            }
            catch (Exception ex)
            {
                returnCode = SQLiteErrorCode.CantOpen;
                LastError = ex.Message;
                try { returnCode = Conn.ExtendedResultCode(); } catch { }
                return false;
            }
        }

        internal static bool CloseDB(SQLiteConnection Conn)
        {
            if (Conn.State == ConnectionState.Open) Conn.Close();
            Conn.Progress -= ProgressEventHandler;
            return true;
        }


        /// <summary>
        /// Determine if an SQLite database is encrypted.
        /// </summary>
        /// <param name="dbName">The fully qualified file name of the SQLite database.</param>
        /// <returns>True if the database is encrypted and requires a password, False if the database is not encrypted.</returns>
        internal static bool IsEncrypted(string dbName)
        {
            return !VerifyPassword(dbName);
        }

        /// <summary>
        /// Confirm that a password for an encrypted SQLite database is valid.
        /// </summary>
        /// <param name="DBLocation">The fully qualified file name of the SQLite database.</param>
        /// <param name="password">The password needed to access the database.</param>
        /// <returns>True if the password is valid, False if the password is not valid.</returns>
        internal static bool VerifyPassword(string DBLocation, string password = null)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            SQLiteErrorCode returnCode;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode, false, password)) return false;

            // It appears that the database may open successfully if the wrong password is supplied.
            // Attempt to access it to determine if the password is valid.
            cmd.CommandText = "Select 1 AS Field1 FROM sqlite_master";
            try
            {
                object obj = ExecuteScalar(cmd, out returnCode);
                return true;
            }
            catch { return false; }
            finally { CloseDB(conn); }
        }

        internal static SQLiteCommand AttachDatabase(string DBLocation, string dbToAttach, string schema, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Unknown;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            
            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return cmd;
            cmd.CommandText = string.Format("Attach Database \"{0}\" As \"{1}\"", dbToAttach, schema);
            try
            {
                object obj = ExecuteScalar(cmd, out returnCode);
                return cmd;
            }
            catch { return cmd; }
            //finally { CloseDB(conn); }
        }

        internal static bool DetachDatabase(SQLiteCommand cmd, string schema, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Unknown;
            //SQLiteConnection conn = null;
            //SQLiteCommand cmd = null;

            //if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return false;
            cmd.CommandText = string.Format("Detach Database \"{0}\"", schema);
            try
            {
                object obj = ExecuteScalar(cmd, out returnCode);
                return true;
            }
            catch { return false; }
            finally { CloseDB(cmd.Connection); }
        }


        internal static bool BackupDatabase(string DBLocation, string BackupDBName, out SQLiteErrorCode returnCode)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            SQLiteConnection connDest = new SQLiteConnection();
            SQLiteCommand cmdDest = null;
            returnCode = SQLiteErrorCode.Ok;

            if (!OpenDB(DBLocation, ref conn, ref cmd, out returnCode)) return false;
            if (!OpenDB(BackupDBName, ref connDest, ref cmdDest, out returnCode))
            {
                CloseDB(conn);
                return false;
            }

            try
            {
                bCancelBackup = false;
                conn.BackupDatabase(connDest, "main", "main", 80, BackupCallback, 1000);
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                returnCode = conn.ExtendedResultCode();
                return false;
            }
            finally { CloseDB(conn); CloseDB(connDest);  }
        }

        public delegate bool SQLiteBackupCallback(SQLiteConnection source, string sourceName, SQLiteConnection destination, string destinationName, int pages, int remainingPages, int totalPages, bool retry);

        internal static bool bCancelBackup;
        internal static bool BackupCallback(SQLiteConnection source, string sourceName, SQLiteConnection destination, string destinationName, int pages, int remainingPages, int totalPages, bool retry)
        {
            return !bCancelBackup;
        }

        internal static bool CheckForRowID(string DatabaseName, string table, out string rowidName, out string PrimaryKeyName)
        {

            string[] Rowids = new string[] { "rowid", "_rowid_", "OID" };

            rowidName = string.Empty;
            for (int i = 0; i < Rowids.Length; i++)
            {
                if (!DataAccess.SchemaDefinitions[DatabaseName].Tables[table].Columns.ContainsKey(Rowids[i]))
                {
                    rowidName = Rowids[i];
                    break;
                }
            }

            if (string.IsNullOrEmpty(rowidName)) return GetPrimaryKey(table, out rowidName, out PrimaryKeyName);

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteErrorCode returnCode;

            DataAccess.OpenDB(DatabaseName, ref conn, ref cmd, out returnCode, false);

            cmd.CommandText = string.Format("Select {0} from \"{1}\" Limit 1", rowidName, table);
            SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
            PrimaryKeyName = dr.GetName(0);
            dr.Close();
            DataAccess.CloseDB(conn);
            return PrimaryKeyName == rowidName ? true : false;
        }


        protected static bool GetPrimaryKey(string table, out string rowid, out string PrimaryKey)
        {
            // only reason we're here is that all rowid synonyms are used
            rowid = string.Empty;
            PrimaryKey = string.Empty;
            return false;
        }
    }
}
