using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    public class LoadStatsEventArgs : EventArgs
    {
        public string CurrentObject;
        public bool LoadComplete;
    }
    class Analyzer
    {
        public event EventHandler<LoadStatsEventArgs> LoadStatsReport = delegate { };
        public void LoadStatsEventHandler(object sender, LoadStatsEventArgs e)
        {
            LoadStatsReport(sender, e);
        }

        string _tablename;
        string _dblocation;
        DataTable dt;

        public int ObjectCount { get; set; }
        public bool Cancel { get; set; }

        internal Analyzer(string DatabaseLocation, string TableName = "all")
        {
            _dblocation = DatabaseLocation;
            _tablename = TableName;
            string sql = string.Format("Select * From sqlite_master Where type in (\"table\", \"index\") {0} ", _tablename.ToLower() == "all" ? string.Empty : string.Format("And tbl_name = \"{0}\"", _tablename));
            dt = DataAccess.ExecuteDataTable(_dblocation, sql, out SQLiteErrorCode returnCode);
            ObjectCount = dt.Rows.Count;
        }

        ~Analyzer()
        {

        }

        internal void Start()
        {
            BuildStats();
        }
        private bool BuildStats()
        {
            SQLiteErrorCode returnCode;
            string sql = CreateSQL();
            LoadStatsEventArgs e = new LoadStatsEventArgs();

            int rc = DataAccess.ExecuteNonQuery(MainForm.mInstance.CurrentDB, sql, out returnCode);
            rc = DataAccess.ExecuteNonQuery(MainForm.mInstance.CurrentDB, string.Format("Delete From {0}", Common.StatsTable), out returnCode);

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            DataAccess.OpenDB(MainForm.mInstance.CurrentDB, ref conn, ref cmd, out returnCode, false);
            BindFunction(conn, new isleaf());
            BindFunction(conn, new isoverflow());
            BindFunction(conn, new isinternal());
            conn.Progress += ProgressReport;

            foreach (DataRow dr in dt.Rows)
            {
                e.CurrentObject = dr["name"].ToString();
                e.LoadComplete = false;
                LoadStatsReport(this, e);
                string select = CreateSelectStmt(dr["name"].ToString(), dr["tbl_name"].ToString());
                cmd.CommandText = select;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("is_index", dr["type"].ToString() == "index" ? true : false);
                cmd.ExecuteNonQuery();
                int gap_cnt = 0;
                long prevpage = 0;
                sql = string.Format("SELECT pageno, pagetype FROM dbstat WHERE name = \"{0}\" ORDER BY pageno", dr["name"].ToString());
                cmd.CommandText = sql;
                SQLiteDataReader pdr = cmd.ExecuteReader();
                while (pdr.Read())
                {
                    if (prevpage > 0 && pdr["pagetype"].ToString() == "leaf" && (long)pdr["pageno"] != prevpage + 1) gap_cnt++;
                    prevpage = (long)pdr["pageno"];
                }
                pdr.Close();
                sql = string.Format("Update {0} Set gap_cnt = {1} Where name = \"{2}\"", Common.StatsTable, gap_cnt.ToString(), dr["name"].ToString());
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                e.LoadComplete = true;
                LoadStatsReport(this, e);
                if (Cancel)
                {
                    rc = DataAccess.ExecuteNonQuery(MainForm.mInstance.CurrentDB, string.Format("Delete From {0}", Common.StatsTable), out returnCode);
                    break;
                }
            }
            conn.Progress -= ProgressReport;
            DataAccess.CloseDB(conn);

            return true;
        }

        internal void ProgressReport(object sender, ProgressEventArgs e)
        {
            if (Cancel) e.ReturnCode = SQLiteProgressReturnCode.Interrupt;
            System.Windows.Forms.Application.DoEvents();
        }

        private string CreateSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}(", Common.StatsTable);
            sb.Append("sdate,");                        //Date table was last refreshed
            sb.Append("name clob,");                    //Name of a table or index in the database file
            sb.Append("tblname clob,");                 //Name of associated table
            sb.Append("isindex boolean,");              //TRUE if it is an index, false for a table
            sb.Append("nentry int,");                   //Number of entries in the BTree
            sb.Append("leaf_entries int,");             //Number of leaf entries
            sb.Append("payload int,");                  //Total amount of data stored in this table or index
            sb.Append("ovfl_payload int,");             //Total amount of data stored on overflow pages
            sb.Append("ovfl_cnt int,");                 //Number of entries that use overflow
            sb.Append("mx_payload int,");               //Maximum payload size
            sb.Append("int_pages int,");                //Number of interior pages used
            sb.Append("leaf_pages int,");               //Number of leaf pages used
            sb.Append("ovfl_pages int,");               //Number of overflow pages used
            sb.Append("int_unused int,");               //Number of unused bytes on interior pages
            sb.Append("leaf_unused int,");              //Number of unused bytes on primary pages
            sb.Append("ovfl_unused int,");              //Number of unused bytes on overflow pages
            sb.Append("gap_cnt int,");                  //Number of gaps in the page layout
            sb.Append("compressed_size int");           // Total bytes stored on disk
            sb.Append(") ;");
        


            /* 
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE IF NOT EXISTS SQLite_Workshop.stats(");
            sb.Append("  sdate      DATE,");             // Date stats were created
            sb.Append("  name       STRING,");           // Name of table or index 
            sb.Append("  path       INTEGER,");          // Path to page from root 
            sb.Append("  pageno     INTEGER,");          // Page number 
            sb.Append("  pagetype   STRING,");           // 'internal', 'leaf' or 'overflow' 
            sb.Append("  ncell      INTEGER,");          // Cells on page (0 for overflow) 
            sb.Append("  payload    INTEGER,");          // Bytes of payload on this page 
            sb.Append("  unused     INTEGER,");          // Bytes of unused space on this page 
            sb.Append("  mx_payload INTEGER,");          // Largest payload size of all cells 
            sb.Append("  pgoffset   INTEGER,");          // Offset of page in file 
            sb.Append("  pgsize     INTEGER ");          // Size of the page 
            sb.Append(");");
            */
            return sb.ToString();
        }

        private string CreateSelectStmt(string objName, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Insert Into {0} ", Common.StatsTable);
            sb.Append("SELECT ");
            sb.Append("CURRENT_TIMESTAMP AS sdate,");
            sb.AppendFormat("\"{0}\" AS name,", objName);
            sb.AppendFormat("\"{0}\" AS tblname,", tableName);
            sb.Append("$is_index AS isindex,");
            sb.Append("sum(ncell) AS nentry,");
            sb.Append("sum(isleaf(pagetype, $is_index) * ncell) AS leaf_entries,");
            sb.Append("sum(payload) AS payload,");
            sb.Append("sum(isoverflow(pagetype, $is_index) * payload) AS ovfl_payload,");
            sb.Append("sum(path LIKE '%+000000') AS ovfl_cnt,");
            sb.Append("max(mx_payload) AS mx_payload,");
            sb.Append("sum(isinternal(pagetype, $is_index)) AS int_pages,");
            sb.Append("sum(isleaf(pagetype, $is_index)) AS leaf_pages,");
            sb.Append("sum(isoverflow(pagetype, $is_index)) AS ovfl_pages,");
            sb.Append("sum(isinternal(pagetype, $is_index) * unused) AS int_unused,");
            sb.Append("sum(isleaf(pagetype, $is_index) * unused) AS leaf_unused,");
            sb.Append("sum(isoverflow(pagetype, $is_index) * unused) AS ovfl_unused,");
            sb.Append("0 AS gap_cnt,");
            sb.Append("sum(pgsize) AS compressed_size ");
            sb.AppendFormat("FROM dbstat WHERE name = \"{0}\"", objName);

            return sb.ToString();
        }

        static void BindFunction(SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }

        [SQLiteFunction(Name = "isleaf", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class isleaf : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                string pagetype = args[0].ToString(); 
                bool isIndex = (long)args[1] == 1 ? true : false;
                return pagetype == "leaf" || (pagetype == "internal" && isIndex);
            }
        }

        [SQLiteFunction(Name = "isinternal", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class isinternal : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                string pagetype = args[0].ToString();
                bool isIndex = (long)args[1] == 1 ? true : false;
                return pagetype == "internal" && !isIndex;
            }
        }

        [SQLiteFunction(Name = "isoverflow", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class isoverflow : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                string pagetype = args[0].ToString();
                bool isIndex = (long)args[1] == 1 ? true : false;
                return pagetype == "overflow";
            }
        }
    }
}
