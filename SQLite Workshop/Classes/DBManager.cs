using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    enum ColumnType
    {
        Char,
        Varchar,
        nChar,
        nVarchar,
        Real,
        Single,
        Double,
        Float,
        Numeric,
        Integer,
        LongInteger,
        ShortInteger,
        TinyInteger,
        Currency,
        Boolean,
        Date,
        Datetime
    }
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

    abstract class DBManager
    {
        
        internal DBManager(string ConnectionString)
        { }

        abstract internal DBDatabaseList GetDatabaseList();

        abstract internal DBSchema GetSchema();

        abstract internal Dictionary<string, DBColumn> GetColumns(string TableName);

        abstract internal bool Import(string SourceTable, string DestTable, Dictionary<string, DBColumn> columns = null);

        protected string BuildCreateSql(string TableName, Dictionary<string, DBColumn> columns)
        {
            // Remap foreign column layout to internal SQLite Column Layout
            Dictionary<string, ColumnLayout> SQColumns = new Dictionary<string, ColumnLayout>();
            foreach (var col in columns)
            {
                if (col.Value.IncludeInImport)
                {
                    ColumnLayout SQCol = new ColumnLayout();
                    SQCol.Check = string.Empty;
                    SQCol.Collation = string.Empty;
                    SQCol.ColumnType = col.Value.Type;
                    SQCol.DefaultValue = col.Value.HasDefault ? col.Value.DefaultValue : string.Empty;
                    // Foreign key will not be used during Import
                    SQCol.ForeignKey = new ForeignKeyLayout();
                    SQCol.NullType = col.Value.IsNullable ? 0 : 1;
                    SQCol.PrimaryKey = col.Value.PrimaryKey;
                    SQCol.Unique = col.Value.IsUnique;
                    SQColumns.Add(col.Value.Name, SQCol);
                }
            }
            return SqlFactory.CreateSQL(TableName, SQColumns);
        }

        protected string BuildInsertSql(string TableName, DataTable dtColumns)
        {
            StringBuilder sbI = new StringBuilder();
            StringBuilder sbV = new StringBuilder();

            sbI.Append(string.Format("Insert Into {0} (", TableName));
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
            sbI.Append(string.Format("Insert Into {0} (", TableName));
            sbV.Append(" Values (");

            int colCount = 0;
            string primaryKey = string.Empty;
            foreach (var column in columns)
            {
                sb.Append("\r\n\t").Append(colCount == 0 ? string.Empty : ",").Append("\"").Append(column.Value.Name).Append("\" ");
                sbI.Append(colCount == 0 ? string.Empty : ",").Append("\"").Append(column.Value.Name).Append("\" ");
                sbV.Append(colCount == 0 ? string.Empty : ",").Append("?");
                sb.Append(GetColumnType(column.Value.Type, column.Value.Size, out ColumnType colType, out bool isText));
                if (column.Value.IsAutoIncrement) sb.Append(" auto_increment");
                sb.Append(column.Value.IsNullable ? " Null" : "Not Null");
                if (column.Value.HasDefault)
                {
                    string sep = isText ? "\"" : string.Empty;
                    sb.Append(" ").Append(sep).Append(column.Value.DefaultValue).Append(sep);
                }
                colCount++;
            }
            if (!string.IsNullOrEmpty(primaryKey)) sb.Append("\t,Primary Key(\"").Append(primaryKey).Append("\"");
            sb.Append(")");
            sbI.Append(")").Append(sbV.ToString()).Append(")");
            InsertSQL = sbI.ToString();
            return sb.ToString();
        }

        protected string GetColumnType(string ColType, int ColSize, out ColumnType colType, out bool isText)
        {
            switch (ColType)
            {
                case "System.String":
                    colType = ColumnType.Varchar;
                    isText = true;
                    return string.Format("varchar({0})", ColSize.ToString());
                default:
                    colType = ColumnType.Double;
                    isText = false;
                    return "double";
            }
        }
    }
}
