﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using static SQLiteWorkshop.Common;

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
        internal Dictionary<string, AttachDbLayout> AttachDbs;
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

    internal struct AttachDbLayout
    {
        internal string DbLocation;
        internal string SchemaName;
    }

    internal struct BuiltInFunction
    {
        internal string Name;
        internal bool LoadOnOpen;
        internal string Category;
        internal string Description;
        internal Type Function;
    }

    abstract class DataAccess
    {

        public static event EventHandler<ProgressEventArgs> ProgressReport = delegate { };
        public static void ProgressEventHandler(object sender, ProgressEventArgs e)
        {
            ProgressReport(sender, e);
        }

        internal static List<BuiltInFunction> FunctionList = new List<BuiltInFunction>();
        internal static Dictionary<string, SchemaDefinition> SchemaDefinitions = new Dictionary<string, SchemaDefinition>();

        internal static bool CancelAction = false;
        internal static string LastError { get; private set; } = null;
        internal static int FormatErrorCount { get; private set; } = 0;
        internal static ArrayList FormatErrors { get; private set; } = null;
        internal const int MAX_ERRORS = 100;

        const string QRY_TABLES = "SELECT * FROM sqlite_master WHERE type = 'table' ORDER BY name";
        const string QRY_TABLE = "SELECT * FROM sqlite_master WHERE name = {0} And type = 'table'";
        const string QRY_VIEWS = "SELECT * FROM sqlite_master WHERE type = 'view' ORDER BY name";
        const string QRY_TRIGGERS = "SELECT * FROM sqlite_master WHERE type = 'trigger' ORDER BY name";
        const string QRY_INDEXES = "SELECT * FROM sqlite_master WHERE type = 'index' AND tbl_name = '{0}' ORDER BY name";
        const string QRY_COLUMNS = "PRAGMA table_info(\"{0}\")";
        const string QRY_INDEXCOLUMNS = "PRAGMA index_xinfo(\"{0}\")";
        const string QRY_INDEXDETAIL = "PRAGMA index_list(\"{0}\")";
        const string QRY_FOREIGNKEYS = "PRAGMA foreign_key_list(\"{0}\")";

        internal static SchemaDefinition GetSchema(string DBLocation, bool bRefresh = false)
        {
            Dictionary<string, AttachDbLayout> AttachDbLayouts = new Dictionary<string, AttachDbLayout>();
            if (bRefresh) 
                if (SchemaDefinitions.ContainsKey(DBLocation))
                {
                    AttachDbLayouts = SchemaDefinitions[DBLocation].AttachDbs;
                    SchemaDefinitions.Remove(DBLocation);
                }

            if (SchemaDefinitions.ContainsKey(DBLocation)) return SchemaDefinitions[DBLocation];

            SchemaDefinition sd = new SchemaDefinition();

            FileInfo f = new FileInfo(DBLocation);
            if (!f.Exists)
                { LastError = "Database not found."; sd.LoadStatus = -1; return sd; }

            if (!DataAccess.IsValidDB(DBLocation, null, out _))
            {
                GetPassword gp = new GetPassword
                {
                    DBLocation = DBLocation
                };
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

            if (!OpenDB(DBLocation, ref conn, ref cmd))
            { sd.LoadStatus = -1; return sd; }

            sd.Tables = GetTables(cmd);
            sd.Views = GetViews(cmd);
            sd.Triggers = GetTriggers(cmd);
            sd.AttachDbs = AttachDbLayouts;

            CloseDB(conn);
            sd.LoadStatus = 0;
            SchemaDefinitions[DBLocation] = sd;
            return sd;
        }

        /// <summary>
        /// Remove a database from the SchemaDefinitions collection
        /// </summary>
        /// <param name="DBLocation">Fully Quakified name of the DB to remove</param>
        internal static void RemoveSchema(string DBLocation)
        {
            if (SchemaDefinitions.ContainsKey(DBLocation)) 
                SchemaDefinitions.Remove(DBLocation);
        }

        internal static bool AddTableToSchema(string DBLocation, string table)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return false;

            cmd.CommandText = string.Format(QRY_TABLE, "\"" + table + "\"");
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            if (dt.Rows.Count == 0) return false;

            TableLayout tl = LoadTable(cmd, dt.Rows[0]);
            if (SchemaDefinitions[DBLocation].Tables.ContainsKey(table))
            { SchemaDefinitions[DBLocation].Tables[table] = tl; }
            else
            { SchemaDefinitions[DBLocation].Tables.Add(table, tl); }
            return true;
        }

        internal static bool RemoveTableFromSchema(string DBLocation, string table)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return false;

            cmd.CommandText = string.Format(QRY_TABLE, "\"" + table + "\"");
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            if (dt.Rows.Count > 0) return false;

            if (SchemaDefinitions[DBLocation].Tables.ContainsKey(table))
                SchemaDefinitions[DBLocation].Tables.Remove(table);
            return true;
        }
        /// <summary>
        /// Reload all Tables into the Schema Definition Structure.  This is needed
        /// to accommodate changes to Tables made via Sql during the session.
        /// </summary>
        /// <param name="DBLocation"></param>
        internal static void ReloadTables(string DBLocation)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return;

            SchemaDefinition sd = SchemaDefinitions[DBLocation];
            sd.Tables = GetTables(cmd);
            SchemaDefinitions[DBLocation] = sd;
            CloseDB(conn);
        }

        /// <summary>
        /// Reload all Views into the Schema Definition Structure.  This is needed
        /// to accommodate changes to Views made via Sql during the session.
        /// </summary>
        /// <param name="DBLocation"></param>
        internal static void ReloadViews(string DBLocation)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return;

            SchemaDefinition sd = SchemaDefinitions[DBLocation];
            sd.Views = GetViews(cmd);
            SchemaDefinitions[DBLocation] = sd;
            CloseDB(conn);
        }

        /// <summary>
        /// Reload all Triggers into the Schema Definition Structure.  This is needed
        /// to accommodate changes to Triggers made via Sql during the session.
        /// </summary>
        /// <param name="DBLocation"></param>
        internal static void ReloadTriggers(string DBLocation)
        {
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return;

            SchemaDefinition sd = SchemaDefinitions[DBLocation];
            sd.Triggers = GetTriggers(cmd);
            SchemaDefinitions[DBLocation] = sd;
            CloseDB(conn);
        }

        private static Dictionary<string, TableLayout> GetTables(SQLiteCommand cmd)
        {
            Dictionary<string, TableLayout> TableLayouts = new Dictionary<string, TableLayout>();

            cmd.CommandText = QRY_TABLES;
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                TableLayout tl = LoadTable(cmd, dr);
                TableLayouts.Add(dr["name"].ToString(), tl);
            }

            // Special case sqlite_master
            string tblname = "sqlite_master";
            TableLayout tspecial = new TableLayout
            {
                rootpage = 0,
                CreateSQL = string.Empty,
                Indexes = GetIndexes(tblname, cmd),
                Columns = GetColumns(tblname, cmd),
                ForeignKeys = GetForeignKeys(tblname, cmd),
                TblType = SQLiteTableType.system
            };
            TableLayouts.Add(tblname, tspecial);

            return TableLayouts;
        }

        private static TableLayout LoadTable(SQLiteCommand cmd, DataRow dr)
        {
            TableLayout tl = new TableLayout
            {
                rootpage = Convert.ToInt64(dr["rootpage"]),              //Appears to be long in older version and int in the current version
                CreateSQL = dr["sql"].ToString(),
                Indexes = GetIndexes(dr["name"].ToString(), cmd),
                Columns = GetColumns(dr["name"].ToString(), cmd),
                ForeignKeys = GetForeignKeys(dr["name"].ToString(), cmd),
                TblType = IsSystemTable(dr["name"].ToString()) ? SQLiteTableType.system : SQLiteTableType.user,
                PrimaryKeys = new Dictionary<string, long>()
            };
            foreach (var column in tl.Columns)
            {
                if (column.Value.PrimaryKey > 0) tl.PrimaryKeys.Add(column.Key, column.Value.PrimaryKey);
            }
            return tl;
        }

        internal static Dictionary<string, TableLayout> GetTables(string DBLocation, string password)
        {
            Dictionary<string, TableLayout> TableLayouts;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd, false, password)) return null;

            TableLayouts = GetTables(cmd);

            CloseDB(conn);
            return TableLayouts;
        }

        private static Dictionary<string, IndexLayout> GetIndexes(string TableName, SQLiteCommand cmd)
        {
            Dictionary<string, IndexLayout> IndexLayouts = new Dictionary<string, IndexLayout>();

            cmd.CommandText = string.Format(QRY_INDEXES, TableName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            cmd.CommandText = string.Format(QRY_INDEXDETAIL, TableName);
            DataTable dtDetail = ExecuteDataTable(cmd, out returnCode);

            foreach (DataRow dr in dt.Rows)
            {
                IndexLayout il = new IndexLayout
                {
                    rootpage = Convert.ToInt64(dr["rootpage"]),
                    TableName = dr["name"].ToString(),
                    CreateSQL = dr["sql"].ToString(),
                    Columns = GetIndexColumns(dr["name"].ToString(), cmd)
                };
                foreach (DataRow drDetail in dtDetail.Rows)
                {
                    if (dr["name"].ToString() == drDetail["name"].ToString())
                    {
                        il.Unique = drDetail["unique"].ToString() != "0";
                        il.Origin = drDetail["origin"].ToString();
                        il.Partial = drDetail["partial"].ToString() != "0";
                    }
                }
                IndexLayouts.Add(dr["name"].ToString(), il);
            }

            return IndexLayouts;
        }

        private static Dictionary<string, IndexLayout> GetIndexes(string DBLocation, string TableName)
        {
            Dictionary<string, IndexLayout> IndexLayouts;

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;

            IndexLayouts = GetIndexes(TableName, cmd);

            CloseDB(conn);
            return IndexLayouts;
        }

        private static Dictionary<string, ColumnLayout> GetColumns(string TableName, SQLiteCommand cmd)
        {
            Dictionary<string, ColumnLayout> ColumnLayouts = new Dictionary<string, ColumnLayout>();

            cmd.CommandText = string.Format(QRY_COLUMNS, TableName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ColumnLayout cl = new ColumnLayout
                    {
                        Cid = Convert.ToInt64(dr["cid"]),
                        ColumnType = dr["type"].ToString(),
                        NullType = Convert.ToInt64(dr["notnull"]),
                        DefaultValue = dr["dflt_value"].ToString(),
                        PrimaryKey = Convert.ToInt64(dr["pk"])
                    };
                    ColumnLayouts.Add(dr["name"].ToString(), cl);
                }
            }

            return ColumnLayouts;
        }

        private static Dictionary<string, ColumnLayout> GetColumns(string DBLocation, string TableName)
        {
            Dictionary<string, ColumnLayout> ColumnLayouts;

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;

            ColumnLayouts = GetColumns(TableName, cmd);

            CloseDB(conn);
            return ColumnLayouts;
        }

        private static Dictionary<string, IndexColumnLayout> GetIndexColumns(string IndexName, SQLiteCommand cmd)
        {
            Dictionary<string, IndexColumnLayout> IndexColumnLayouts = new Dictionary<string, IndexColumnLayout>();

            cmd.CommandText = string.Format(QRY_INDEXCOLUMNS, IndexName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                IndexColumnLayout icl = new IndexColumnLayout
                {
                    seqno = Convert.ToInt64(dr["seqno"]),
                    Cid = Convert.ToInt64(dr["cid"]),
                    SortOrder = Convert.ToInt32(dr["desc"].ToString()) == 1 ? "Desc" : "Asc",
                    CollatingSequence = dr["coll"].ToString(),
                    key = Convert.ToInt64(dr["key"])
                };
                string idxKey = dr["name"] == null || string.IsNullOrEmpty(dr["Name"].ToString()) ? "rowid" : dr["name"].ToString();
                if (!IndexColumnLayouts.ContainsKey(idxKey)) IndexColumnLayouts.Add(idxKey, icl);
            }

            return IndexColumnLayouts;
        }

        private static Dictionary<string, ForeignKeyLayout> GetForeignKeys(string TableName, SQLiteCommand cmd)
        {
            Dictionary<string, ForeignKeyLayout> FKeyLayouts = new Dictionary<string, ForeignKeyLayout>();

            cmd.CommandText = string.Format(QRY_FOREIGNKEYS, TableName);
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                ForeignKeyLayout fl = new ForeignKeyLayout
                {
                    id = Convert.ToInt64(dr["id"]),
                    Sequence = Convert.ToInt64(dr["seq"]),
                    Table = dr["table"].ToString(),
                    From = dr["from"].ToString(),
                    To = dr["to"].ToString(),
                    OnUpdate = dr["on_update"].ToString(),
                    OnDelete = dr["on_delete"].ToString(),
                    Match = dr["match"].ToString()
                };
                FKeyLayouts.Add(i.ToString(), fl);
                i++;
            }

            return FKeyLayouts;
        }

        private static Dictionary<string, ForeignKeyLayout> GetForeignKeys(string DBLocation, string TableName)
        {
            Dictionary<string, ForeignKeyLayout> FKeyLayouts;

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;

            FKeyLayouts = GetForeignKeys(TableName, cmd);

            CloseDB(conn);
            return FKeyLayouts;
        }

        private static Dictionary<string, ViewLayout> GetViews(SQLiteCommand cmd)
        {
            Dictionary<string, ViewLayout> ViewLayouts = new Dictionary<string, ViewLayout>();

            cmd.CommandText = QRY_VIEWS;
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                ViewLayout vl = new ViewLayout
                {
                    rootpage = Convert.ToInt64(dr["rootpage"]),
                    CreateSQL = dr["sql"].ToString(),
                    Columns = GetColumns(dr["name"].ToString(), cmd)
                };
                ViewLayouts.Add(dr["name"].ToString(), vl);
            }

            return ViewLayouts;
        }

        private static Dictionary<string, TriggerLayout> GetTriggers(SQLiteCommand cmd)
        {
            Dictionary<string, TriggerLayout> TriggerLayouts = new Dictionary<string, TriggerLayout>();

            cmd.CommandText = QRY_TRIGGERS;
            DataTable dt = ExecuteDataTable(cmd, out SQLiteErrorCode returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                TriggerLayout vl = new TriggerLayout
                {
                    rootpage = Convert.ToInt64(dr["rootpage"]),
                    CreateSQL = dr["sql"].ToString()
                };
                TriggerLayouts.Add(dr["name"].ToString(), vl);
            }

            return TriggerLayouts;
        }

        private static Dictionary<string, ViewLayout> GetViews(string DBLocation)
        {
            Dictionary<string, ViewLayout> ViewLayouts;

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;

            ViewLayouts = GetViews(cmd);

            CloseDB(conn);
            return ViewLayouts;
        }

        /// <summary>
        /// Add an attached db.  When present, this db will be attached whenever the parent DB
        /// is opened.
        /// </summary>
        /// <param name="DBLocation">Parent Database</param>
        /// <param name="dbname">Database to Attach</param>
        internal static void AddAttachedDb(string DBLocation, string dbname, string schema)
        {
            string dbkey = Path.GetFileName(dbname);
            if (string.IsNullOrEmpty(dbkey)) return;

            SchemaDefinition sd = DataAccess.SchemaDefinitions[DBLocation];
            if (sd.AttachDbs.ContainsKey(dbkey)) return;

            AttachDbLayout ad = new AttachDbLayout
            {
                DbLocation = dbname,
                SchemaName = schema
            };
            sd.AttachDbs.Add(dbkey, ad);
            DataAccess.SchemaDefinitions[DBLocation] = sd;

            return;
        }

        /// <summary>
        /// Remove an attached DB
        /// </summary>
        /// <param name="DBLocation">Parent Database</param>
        /// <param name="dbname">Database to remove</param>
        internal static void DelAttachedDb(string DBLocation, string dbname)
        {
            SchemaDefinition sd = DataAccess.SchemaDefinitions[DBLocation];
            if (!sd.AttachDbs.ContainsKey(dbname)) return;
            sd.AttachDbs.Remove(dbname);
            DataAccess.SchemaDefinitions[DBLocation] = sd;
            return;
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

            // The open creates the file
            if (!OpenDB(dbName, ref conn, ref cmd, true)) return false;

            // SQLite appears to create an empty file that stays empty until you put something in it
            // Create a dummy table
            cmd.CommandText = "CREATE TABLE \"$Temp\"(\"ID\" integer );";
            ExecuteNonQuery(cmd, out SQLiteErrorCode returnCode);
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

            if (!OpenDB(dbName, ref conn, ref cmd, true)) return false;

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
            SchemaDefinition sd = SchemaDefinitions[dbName];
            sd.password = password;
            SchemaDefinitions[dbName] = sd;
            return true;
        }

        internal static bool Parse(string dbName, string sql, out SQLiteErrorCode returnCode)
        {

            sql = string.Format("Explain {0}", sql);

            DataTable dt = ExecuteDataTable(dbName, sql, out returnCode);
            if (dt == null) return false;
            dt.Dispose();
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

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;
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

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;

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
                    //if (recordcount % 100 == 0) Application.DoEvents();
                    if (CancelAction)
                    {
                        CancelAction = false;
                        returnCode = SQLiteErrorCode.Interrupt;
                        return dt;
                    }
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

        internal static long ExecuteNonQuery(string DBLocation, string SqlStatement, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return -1;

            cmd.CommandText = SqlStatement;
            long result = ExecuteNonQuery(cmd, out returnCode);
            CloseDB(conn);
            return result;
        }

        internal static long ExecuteNonQuery(string DBLocation, string SqlStatement, ArrayList parms, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return -1;
            for (int i = 0; i < parms.Count; i++)
            {
                cmd.Parameters.Add(new SQLiteParameter() { Value = parms[i] });
            }
            cmd.CommandText = SqlStatement;
            long result = ExecuteNonQuery(cmd, out returnCode);
            CloseDB(conn);
            return result;
        }

        internal static long ExecuteNonQuery(string DBLocation, string SqlStatement, ArrayList parms, out long LastInsertID, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            LastInsertID = -1;
            long result;

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return -1;
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

        internal static long ExecuteNonQuery(SQLiteCommand cmd, out SQLiteErrorCode returnCode)
        {
            long RecordCount;
            returnCode = SQLiteErrorCode.Ok;
            //SQLiteTransaction tran;
            //tran = cmd.Connection.BeginTransaction();

            try
            {
                RecordCount = cmd.ExecuteNonQuery();
                //tran.Commit();
            }
            catch (Exception ex)
            {
                //tran.Rollback();
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

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;

            cmd.CommandText = SqlStatement;
            object obj = ExecuteScalar(cmd, out returnCode);

            CloseDB(conn);
            return obj;
        }

        internal static object ExecuteScalar(string DBLocation, string SqlStatement, ArrayList parms, out SQLiteErrorCode returnCode)
        {

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            returnCode = SQLiteErrorCode.Ok;

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return null;
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
        internal static bool OpenDB(string DBLocation, ref SQLiteConnection Conn, ref SQLiteCommand Cmd, bool IsNew = false, string password = null)
        {
            LastError = string.Empty;
            Conn = new SQLiteConnection
            {
                ConnectionString = string.Format("Data Source={0};Version=3;New={1}", DBLocation, IsNew ? "True" : "False")
            };
            if (!string.IsNullOrEmpty(password)) Conn.SetPassword(ToBytes(password));

            if (SchemaDefinitions.TryGetValue(DBLocation, out SchemaDefinition sd) && string.IsNullOrEmpty(password))
                if (!string.IsNullOrEmpty(sd.password)) Conn.SetPassword(ToBytes(sd.password));

            try
            {
                Uri uriDB = new Uri(DBLocation);
                if (uriDB.IsUnc) Conn.ParseViaFramework = true;
                Conn.Open();
                Conn.SetExtendedResultCodes(true);

                // Initialize ProgressOps before registering the handler.  It will not work if you don't do it this way.
                Conn.ProgressOps = 100;
                Conn.Progress += ProgressEventHandler;
                Cmd = new SQLiteCommand()
                {
                    Connection = Conn
                };

                // Make sure it's a valid DB
                SQLiteTransaction t = Conn.BeginTransaction();
                t.Rollback();

                // Attach any connected databases
                AttachDBs(sd, Cmd);

                // Load BuiltIn Functions
                foreach (BuiltInFunction function in FunctionList)
                {
                    if (function.LoadOnOpen)
                    {
                        object f = Activator.CreateInstance(function.Function);
                        Functions.BindFunction(Conn, (SQLiteFunction)f);
                    }
                }

                //object t = Activator.CreateInstance((Type)obj[2]);
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        private static void AttachDBs(SchemaDefinition sd, SQLiteCommand cmd)
        {
            if (sd.AttachDbs == null) return;
            foreach (KeyValuePair<string, AttachDbLayout> adl in sd.AttachDbs)
            {
                cmd.CommandText = string.Format("ATTACH DATABASE '{0}' AS '{1}';", adl.Value.DbLocation, adl.Value.SchemaName);
                long rc = cmd.ExecuteNonQuery();
            }
        }

        internal static bool CloseDB(SQLiteConnection Conn)
        {
            if (Conn.State == ConnectionState.Open) Conn.Close();
            Conn.Progress -= ProgressEventHandler;
            return true;
        }

        /// <summary>
        /// Determine if a file is a valid SQLite Database.
        /// </summary>
        /// <param name="filename">Fully qualified name of the file to test</param>
        /// <returns>true if the file is a valid DB, otherwise false</returns>
        internal static bool IsValidDB(string filename, string password, out SQLiteErrorCode rc)
        {

            SQLiteConnection conn = new SQLiteConnection
            {
                ConnectionString = string.Format("Data Source={0};FailIfMissing=True;", filename)
            };
            if (!string.IsNullOrEmpty(password)) conn.SetPassword(ToBytes(password)); 

            conn.SetExtendedResultCodes(true);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                try { rc = conn.ExtendedResultCode(); } catch { rc = SQLiteErrorCode.CantOpen; }
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                return false;
            }

            try
            {
                SQLiteTransaction t = conn.BeginTransaction();
                t.Rollback();
            }
            catch
            {
                return false;
            }
            finally
            {
                rc = conn.ExtendedResultCode();
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
            return true;
        }


        internal static SQLiteCommand AttachDatabase(string DBLocation, string dbToAttach, string schema, out SQLiteErrorCode returnCode, string password = null)
        {
            returnCode = SQLiteErrorCode.Unknown;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            
            if (!OpenDB(DBLocation, ref conn, ref cmd)) return cmd;
            cmd.CommandText = string.Format("Attach Database \"{0}\" As \"{1}\"", dbToAttach, schema);
            if (!string.IsNullOrEmpty(password)) cmd.CommandText += string.Format("KEY {0}", password);
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

            if (!OpenDB(DBLocation, ref conn, ref cmd)) return false;
            if (!OpenDB(BackupDBName, ref connDest, ref cmdDest))
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

            DataAccess.OpenDB(DatabaseName, ref conn, ref cmd, false);

            cmd.CommandText = string.Format("Select {0} from \"{1}\" Limit 1", rowidName, table);
            SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
            PrimaryKeyName = dr.GetName(0);
            dr.Close();
            DataAccess.CloseDB(conn);
            return PrimaryKeyName == rowidName;
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
