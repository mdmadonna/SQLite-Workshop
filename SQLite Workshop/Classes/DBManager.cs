using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    internal struct DBColumn
    {
        internal string Name;
        internal int Ordinal;
        internal string Type;
        internal string SqlType;
        internal int Size;
        internal int PrimaryKey;
        internal bool IncludeInImport;
        internal int NumericPrecision;
        internal int NumericScale;
        internal bool IsLong;
        internal bool IsNullable;
        internal bool IsKey;
        internal bool IsUnique;
        internal bool IsAutoIncrement;
        internal bool HasDefault;
        internal string DefaultValue;
        internal int DatetimePrecision;
    }

    internal struct DBTable
    {
        internal string Name;
    }

    internal struct DBSchema
    {
        internal Dictionary<string, DBTable> Tables;
    }

    internal struct DBInfo
    {
        internal string Name;
    }

    internal struct DBDatabaseList
    {
        internal Dictionary<string, DBInfo> Databases;
    }

    internal enum ImportStatus
    {
        Starting,
        InProgress,
        Failed,
        Complete
    }

    internal class StatusEventArgs : EventArgs
    {
        internal ImportStatus Status;
        internal string TblName;
        internal long RowCount;
    }

    abstract class DBManager
    {
        internal event EventHandler<StatusEventArgs> StatusReport;
        internal string currType { get; set; }
        internal string TargetDB { get; private set; }
        internal string ImportKey { get; set; }

        private string _sourceDB;
        internal string SourceDB {
            get { return _sourceDB; } 
            set
            {
                _sourceDB = value;
                conn?.Dispose();
                cmd?.Dispose();
                ida = null;
                conn = null;
            }
        }
        internal string SourceServer { get; private set; }
        internal string SourceUserName { get; private set; }
        internal string SourcePassword { get; private set; }

        // For SqlServer
        private bool _useWindowsAuthentication;
        internal bool UseWindowsAuthentication
        {
            get { return _useWindowsAuthentication; }
            set
            {
                _useWindowsAuthentication = value;
                conn?.Dispose();
                cmd?.Dispose();
                ida = null;
                conn = null;
            }
        }
        internal IDbConnection conn { get; set; }
        internal IDbCommand cmd { get; set; }
        internal IDbDataAdapter ida { get; set; }
        internal IDataReader idr { get; set; }

        protected const int MAX_ERRORS = 10;
        internal ArrayList FormatErrors { get; set; }
        internal string LastError { get; set; }

        protected int FormatErrorCount;

        abstract internal DBDatabaseList GetDatabaseList();

        abstract internal DBSchema GetSchema();

        private string TableQualStart = string.Empty;
        private string TableQualEnd = string.Empty;

        internal DBManager(string sourceDB, string targetDb, string sourceServer = null, string sourceUserName = null, string sourcePassword = null)
        {
            currType = this.GetType().Name.ToLower();
            _sourceDB = sourceDB;
            TargetDB = targetDb;
            SourceServer = sourceServer;
            SourceUserName = sourceUserName;
            SourcePassword = sourcePassword;
        }

        protected bool OpenImportDB()
        {
            if (conn == null) InitConnection();
            if ((!string.IsNullOrEmpty(SourcePassword)) && currType == "dbsqlitemanager") ((SQLiteConnection)conn).SetPassword(ToBytes(SourcePassword));
            try
            {
                conn.Open();
            }
            catch (OleDbException ex)
            {
                ShowMsg(string.Format(ERR_OLEDBOPENERR, ex.Message));
                LastError = ex.Message;
                throw new Exception(ex.Message);
            }
            catch (OdbcException ex)
            {
                ShowMsg(string.Format(ERR_ODBCDBOPENERR, ex.Message));
                LastError = ex.Message;
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                switch (currType)
                {
                    case "dbmsaccessmanager":
                        string bitness = Environment.Is64BitProcess ? "64" : "32";
                        ShowMsg(string.Format(ERR_MSACCESSOPENERR, ex.Message, bitness, MSACCESS_PROVIDER));
                        throw new Exception(ex.Message);

                    default:
                        LastError = ex.Message;
                        throw new Exception(ex.Message);
                }
            }
            return true;
        }

        protected bool CloseImportDB()
        {
            if (conn == null || conn.State != ConnectionState.Open) return false;
            conn.Close();
            return true;
        }

        /// <summary>
        /// Test the Connection.  Warning, this routine should only be called
        /// from within a try...catch construct
        /// </summary>
        /// <returns>true if connection is made, otherwise false</returns>
        internal bool TestConnection()
        {
            if (OpenImportDB())
            {
                CloseImportDB();
                return true;
            }
            return false;
        }

        protected void FireStatusEvent(ImportStatus status, long RecordCount, string tblName = "")
        {
            StatusEventArgs e = new StatusEventArgs
            {
                Status = status,
                TblName = tblName,
                RowCount = RecordCount
            };
            EventHandler<StatusEventArgs> eventHandler = StatusReport;
            eventHandler(this, e);
        }


                
        private void InitConnection()
        {
            switch (currType)
            {
                case "dbsqlservermanager":
                    InitSqlServer();
                    break;
                case "dbodbcmanager":
                    InitOdbc();
                    break;
                case "dbmsaccessmanager":
                    InitMSAccess();
                    break;
                case "dbmysqlmanager":
                    InitMySql();
                    break;
                case "dbsqlitemanager":
                    InitSQLite();
                    break;
                default:
                    throw new Exception("Invalid Type: " + currType);
            }
        }

        virtual internal Dictionary<string, DBColumn> GetColumns(string TableName)
        {

            Dictionary<string, DBColumn> DBColumns = new Dictionary<string, DBColumn>();
            IDataReader iReader = null;

            try
            {
                if (conn == null || conn.State != ConnectionState.Open) OpenImportDB();
            }
            catch { return DBColumns; }

            try
            {
                cmd.CommandText = string.Format("Select * From {0}", QualTableName(TableName));
                iReader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                iReader.Read();
                DataTable ColumnList = iReader.GetSchemaTable();
                int i = 0;
                foreach (DataRow dr in ColumnList.Rows)
                {
                    DBColumn dbc = new DBColumn() { Name = dr["ColumnName"].ToString() };
                    dbc.Ordinal = (int)dr["ColumnOrdinal"];
                    dbc.Type = dr["DataType"].ToString();
                    dbc.SqlType = iReader.GetDataTypeName(currType == "dbmysqlmanager" ? dbc.Ordinal - 1 : dbc.Ordinal);
                    // kluge for encrypted sqlite db's
                    if (dbc.SqlType.IndexOf("(") >= 0) dbc.SqlType = dbc.SqlType.Substring(0, dbc.SqlType.IndexOf("("));
                    dbc.Size = (int)dr["ColumnSize"];
                    dbc.NumericPrecision = DBNull.Value.Equals(dr["NumericPrecision"]) ? 0 : Convert.ToInt32(dr["NumericPrecision"]);
                    dbc.NumericScale = DBNull.Value.Equals(dr["NumericScale"]) ? 0 : Convert.ToInt32(dr["NumericScale"]);
                    dbc.IsLong = (bool)dr["IsLong"];
                    dbc.IsNullable = (bool)dr["AllowDBNull"];
                    dbc.IsUnique = (bool)dr["IsUnique"];
                    dbc.IsKey = DBNull.Value.Equals(dr["IsKey"]) ? false : (bool)dr["IsKey"];
                    dbc.IsAutoIncrement = (bool)dr["IsAutoIncrement"];
                    DBColumns.Add(dbc.Name, dbc);
                    i++;
                }
            }
            catch (Exception ex)
            {
                ShowMsg(String.Format(ERR_METADATA, ex.Message));
                LastError = ex.Message;
            }
            finally
            {
                cmd.Cancel();
                iReader.Close();
                CloseImportDB();
            }
            return DBColumns;
        }

        virtual internal DataTable PreviewData(string TableName)
        {
            OpenImportDB();
            switch (currType)
            {
                case "dbmysqlmanager":
                    cmd.CommandText = string.Format("Select * FROM {0} LIMIT 100", QualTableName(TableName));
                    break;
                default:
                    cmd.CommandText = string.Format("Select Top 100 * FROM {0}", QualTableName(TableName));
                    break;
            }
            ida.SelectCommand = cmd;
            DataSet sds = new DataSet();

            try
            {
                ida.Fill(sds);
                return sds.Tables[0];
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
            finally
            {
                CloseImportDB();
            }
        }

        virtual internal bool Import(string SourceTable, string DestTable, Dictionary<string, DBColumn> columns = null)
        {
            bool rCode;
            long rtnCode;
            string InsertSQL;
            long recCount = 0;
            SQLiteTransaction sqlT = null;
            IDataReader dr = null;

            SQLiteErrorCode returnCode;
            if (columns == null) columns = GetColumns(SourceTable);

            // Open the Target SQLite Database
            SQLiteConnection SQConn = new SQLiteConnection();
            SQLiteCommand SQCmd = new SQLiteCommand();
            rCode = DataAccess.OpenDB(TargetDB, ref SQConn, ref SQCmd);
            if (!rCode)
            {
                FireStatusEvent(ImportStatus.Failed, 0, SourceTable);
                ShowMsg(String.Format(ERR_SQL, DataAccess.LastError, SQLiteErrorCode.CantOpen));
                return false;
            }

            // Indicate the Import is starting
            FireStatusEvent(ImportStatus.Starting, 0, SourceTable);
            
            // Create the Destination table and Import the table within a transaction. 
            // If something goes wrong all actions will automatically be backed out.
            try
            {
                sqlT = SQConn.BeginTransaction();

                // Create the destination table
                SQCmd.CommandText = BuildCreateSql(DestTable, columns, out InsertSQL);
                rtnCode = DataAccess.ExecuteNonQuery(SQCmd, out returnCode);
                if (rtnCode < 0 || returnCode != SQLiteErrorCode.Ok)
                {
                    sqlT.Rollback();
                    FireStatusEvent(ImportStatus.Failed, 0, SourceTable);
                    ShowMsg(String.Format(ERR_SQL, DataAccess.LastError, returnCode));
                    return false;
                }

                // Start the import
                SQCmd.CommandText = InsertSQL;
                OpenImportDB();
                cmd.CommandText = string.Format("Select * FROM {0}", QualTableName(SourceTable));
                dr = cmd.ExecuteReader();

                // Loop thru all records and insert them into the Target SQLite DB
                while (dr.Read())
                {
                    SQCmd.Parameters.Clear();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        SQCmd.Parameters.AddWithValue(String.Empty, dr[i]);
                    }
                    SQCmd.ExecuteNonQuery();
                    recCount++;

                    // Update the StatusBar every 100 records
                    if (recCount % 100 == 0) FireStatusEvent(ImportStatus.InProgress, recCount, SourceTable);
                }
                sqlT.Commit();
            }
            catch (Exception ex)
            {
                // If an error occurs, roll back any updates
                sqlT.Rollback();
                // Indicate the import for this table has failed.
                FireStatusEvent(ImportStatus.Failed, 0, SourceTable);

                ShowMsg(string.Format(ERR_SQL, ex.Message, SQLiteErrorCode.Ok));
                return false;
            }
            finally
            {
                if (dr != null)
                {
                    cmd.Cancel();       
                    dr.Close();
                }
                CloseImportDB();
                DataAccess.CloseDB(SQConn);
            }

            // Update the Database Tree
            MainForm.mInstance.AddTable(DestTable, TargetDB);
            // Indicate this table has imported successfully
            try { FireStatusEvent(ImportStatus.Complete, recCount, SourceTable); } catch { }
            return true;
        }

        protected DataTable LoadPreviewData(IDbCommand cmd)
        {
            DataTable dt = new DataTable();
            int recordcount = 0;
            FormatErrorCount = 0;

            IDataReader dr;
            try
            {
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }

            for (int i = 0; i < dr.FieldCount; i++) { dt.Columns.Add(dr.GetName(i), typeof(string)); }

            try
            {
                while (dr.Read())
                {
                    recordcount++;
                    DataRow dRow = dt.NewRow();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        try { dRow[i] = dr[i] == null ? string.Empty : dr[i].ToString(); }
                        catch (Exception ex)
                        {
                            FormatErrorCount++;
                            if (FormatErrorCount <= MAX_ERRORS) FormatErrors.Add(string.Format("Format Error on Record {0} Column {1}: {2}", recordcount, dr.GetName(i), ex.Message));
                        }
                    }
                    dt.Rows.Add(dRow);
                    if (recordcount >= 100) break;
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
            return dt;
        }

        protected string BuildCreateSql(string TableName, Dictionary<string, DBColumn> columns)
        {
            // Remap foreign column layout to internal SQLite Column Layout
            Dictionary<string, ColumnLayout> SQColumns = new Dictionary<string, ColumnLayout>();
            foreach (var col in columns)
            {
                if (col.Value.IncludeInImport)
                {
                    ColumnLayout SQCol = new ColumnLayout
                    {
                        Check = string.Empty,
                        Collation = string.Empty,
                        ColumnType = col.Value.Type,
                        DefaultValue = col.Value.HasDefault ? col.Value.DefaultValue : string.Empty,
                        // Foreign key will not be used during Import
                        ForeignKey = new ForeignKeyLayout(),
                        NullType = col.Value.IsNullable ? 0 : 1,
                        PrimaryKey = col.Value.PrimaryKey,
                        Unique = col.Value.IsUnique
                    };
                    SQColumns.Add(col.Value.Name, SQCol);
                }
            }
            return SqlFactory.CreateSQL(TableName, SQColumns);
        }

        protected string BuildInsertSql(string TableName, DataTable dtColumns)
        {
            StringBuilder sbI = new StringBuilder();
            StringBuilder sbV = new StringBuilder();

            sbI.Append(string.Format("Insert Into \"{0}\" (", TableName));
            sbV.Append(" Values (");

            int colCount = 0;
            foreach (DataRow dr in dtColumns.Rows)
            {
                sbI.Append(colCount == 0 ? string.Empty : ",").Append("\"").Append(dr["Name"].ToString()).Append("\" ");
                sbV.Append(colCount == 0 ? string.Empty : ",").Append("?");
                colCount++;
            }
            sbI.Append(")").Append(sbV.ToString()).Append(")");
            return sbI.ToString();
        }
        protected string BuildCreateSql(string TableName, Dictionary<string, DBColumn> columns, out string InsertSQL)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbI = new StringBuilder();
            StringBuilder sbV = new StringBuilder();

            sb.Append(String.Format("Create Table If Not Exists \"{0}\"", TableName));
            sb.Append("(\r\n");
            sbI.Append(string.Format("Insert Into \"{0}\" (", TableName));
            sbV.Append(" Values (");

            int colCount = 0;
            string primaryKey = string.Empty;
            foreach (var column in columns)
            {
                sb.Append("\r\n\t").Append(colCount == 0 ? string.Empty : ",").Append("\"").Append(column.Value.Name).Append("\" ");
                sbI.Append(colCount == 0 ? string.Empty : ",").Append("\"").Append(column.Value.Name).Append("\" ");
                sbV.Append(colCount == 0 ? string.Empty : ",").Append("?");
                if (column.Value.IsAutoIncrement)
                {
                    sb.Append(" integer primary key autoincrement not null");
                }
                else
                {
                    sb.Append(GetColumnType(column.Value.SqlType, column.Value.Type, column.Value.Size, column.Value.NumericPrecision, column.Value.NumericScale));
                    sb.Append(column.Value.IsNullable ? " Null" : " Not Null");
                }
                if (column.Value.HasDefault)
                {  sb.Append(" Default ").Append(column.Value.DefaultValue);  }
                colCount++;
            }
            if (!string.IsNullOrEmpty(primaryKey)) sb.Append("\t,Primary Key(\"").Append(primaryKey).Append("\"");
            sb.Append(")");
            sbI.Append(")").Append(sbV.ToString()).Append(")");
            InsertSQL = sbI.ToString();
            return sb.ToString();
        }

        /// <summary>
        /// Wrap a table name with text qualifiers appropriate to the DB being imported from.
        /// </summary>
        /// <param name="TableName">Table being imported</param>
        /// <returns>Table Name wrapped in text qualifiers</returns>
        protected string QualTableName(string TableName)
        {
            return string.Format("{0}{1}{2}", TableQualStart, TableName, TableQualEnd);
        }
        protected string GetColumnType(string ColType, string NetType, int ColSize, int Precision, int Scale)
        {
            switch (ColType.ToLower())
            {
                case "bigint":
                    return "bigint";
                case "binary":
                case "image":
                case "varbinary":
                case "dbtype_wlongvarchar":
                case "dbtype_longvarbinary":
                    return "blob";
                case "bit":
                case "dbtype_bool":
                case "boolean":
                case "bool":
                    return "boolean";
                case "char":
                    return string.Format("char({0})", ColSize.ToString());
                case "currency":
                case "money":
                case "smallmoney":
                case "dbtype_cy":
                    return "decimal(18,2)";
                case "date":
                    return "date";
                case "datetimeoffset":
                case "datetime2":
                case "datetime":
                case "smalldatetime":
                case "time":
                case "timestamp":
                case "dbtype_date":
                    return "datetime";
                case "decimal":
                    return string.Format("decimal({0}, {1})", Precision.ToString(), Scale.ToString());
                case "double":
                case "dbtype_r8":
                    return "double";
                case "int":
                case "integer":
                case "mediumint":
                case "int identity":
                case "system.int32":
                case "dbtype_i4":
                    return "int";
                case "float":
                    return "float";
                case "nchar":
                    return string.Format("nchar({0})", ColSize.ToString());
                case "ntext":
                case "text":
                case "xml":
                case "dbtype_guid":
                    return "text";
                case "nvarchar":
                case "dbtype_wvarchar":
                    return string.Format("nvarchar({0})", ColSize.ToString());
                case "numeric":
                case "dbtype_numeric":
                    return "numeric";
                case "real":
                    return "real";
                case "single":
                case "dbtype_r4":
                    return "single";
                case "smallint":
                case "dbtype_i2":
                    return "smallint";
                case "tinyint":
                case "dbtype_ui1":
                    return "tinyint";
                case "varchar":
                case "system.string":
                    return string.Format("varchar({0})", ColSize.ToString());
                case "unsigned smallint":
                    return "smallint";
                case "unsigned int":
                    return "unsigned int";
                case "unsigned bigint":
                    return "unsigned bigint";
                default:
                    break;
            }

            // We fell thru so it's a type we don't recognize.  We will create a type based on the .NET
            // implementation

            switch (NetType.ToLower())
            {
                case "system.string":
                    return string.Format("nvarchar({0})", ColSize.ToString());
                case "system.boolean":
                    return "boolean";
                case "system.byte":
                case "system.sbyte":        //SQLite doesn't exactly support this
                    return "tinyint";
                case "system.char":
                    return string.Format("nchar({0})", ColSize.ToString());
                case "system.decimal":
                    return string.Format("decimal({0}, {1})", Precision.ToString(), Scale.ToString());
                case "system.double":
                    return "double";;
                case "system.single":
                    return "single";
                case "system.int32":
                    return "integer";
                case "system.uint32":
                    return "unsigned integer";
                case "system.int64":
                    return "bigint";
                case "system.uint64":
                    return "unsigned bigint";
                case "system.int16":
                    return "small integer";
                case "system.uint16":
                    return "unsigned small integer";
                default:
                    return "blob";
            }
        }

        private void InitOdbc()
        {
            conn = new OdbcConnection
            {
                ConnectionString = String.Format("DSN={0};", SourceDB)
            };
            if (!string.IsNullOrEmpty(SourceUserName)) conn.ConnectionString = string.Format("{0}UID={1};Pwd={2};", conn.ConnectionString, SourceUserName, SourcePassword);
            cmd = new OdbcCommand()
            {
                Connection = (OdbcConnection)conn,
                CommandTimeout = 0
            };
            ida = new OdbcDataAdapter();
            TableQualStart = "[";
            TableQualEnd = "]";
        }

        private void InitMSAccess()
        {
            conn = new OleDbConnection
            {
                ConnectionString = string.Format("Provider={0}; Data Source={1}", MSACCESS_PROVIDER, SourceDB)
            };
            if (!string.IsNullOrEmpty(SourceUserName)) conn.ConnectionString = string.Format("{0}UID={1};Pwd={2};", conn.ConnectionString, SourceUserName, SourcePassword);
            cmd = new OleDbCommand()
            {
                Connection = (OleDbConnection)conn,
                CommandTimeout = 0
            };
            ida = new OleDbDataAdapter();
            TableQualStart = "[";
            TableQualEnd = "]";
        }

        Assembly assMySql;
        object iMySql;

        private void InitMySql()
        {
            FileInfo fi = new FileInfo(MYSQLLIBRARY);
            if (!fi.Exists)
            {
                ShowMsg("The MySql driver is not found.  Please place it the the execution directory.");
                return;
            }

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assMySql = Assembly.LoadFile(string.Format("{0}\\{1}", dir, MYSQLLIBRARY));
            iMySql = assMySql.CreateInstance(MYSQLCONNECTIONCLASS, true);
            conn = (IDbConnection)assMySql.CreateInstance(MYSQLCONNECTIONCLASS, true);

            conn.ConnectionString = String.Format("Server={0};Database={1};", SourceServer, SourceDB);

            if (!string.IsNullOrEmpty(SourceUserName)) conn.ConnectionString = string.Format("{0};UID={1};Pwd={2};", conn.ConnectionString, SourceUserName, SourcePassword);
            cmd = (IDbCommand)assMySql.CreateInstance(MYSQLCOMMANDCLASS, true);
            cmd.Connection = (IDbConnection)conn;
            cmd.CommandTimeout = 0;
            ida = (IDbDataAdapter)assMySql.CreateInstance(MYSQLDATAADAPTERCLASS, true);
            TableQualStart = "`";
            TableQualEnd = "`";
        }

        private void InitSqlServer()
        {
            conn = new SqlConnection
            {
                ConnectionString = UseWindowsAuthentication ? string.Format("Server={0};Database={1};Trusted_Connection=True;", SourceServer, SourceDB) : string.Format("Server={0};Database={1};User Id={2};Password={3};", SourceServer, SourceDB, SourceUserName, SourcePassword)
            };
            cmd = new SqlCommand()
            {
                Connection = (SqlConnection)conn,
                CommandTimeout = 0
            };
            ida = new SqlDataAdapter();
            TableQualStart = "[";
            TableQualEnd = "]";
        }

        /// <summary>
        /// Initialize an SQLite connection string.  This is use only for encrypted databases.
        /// </summary>
        private void InitSQLite()
        {
            conn = new SQLiteConnection
            {
                ConnectionString = string.Format("Data Source={0};Version=3;New=false", SourceDB)
            };
            
            cmd = new SQLiteCommand()
            {
                Connection = (SQLiteConnection)conn,
                CommandTimeout = 0
            };
            ida = new SQLiteDataAdapter();
            TableQualStart = "'";
            TableQualEnd = "'";
        }

    }
}
