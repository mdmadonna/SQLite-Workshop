using System;
using System.Collections.Generic;
using System.Text;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    class SqlFactory
    {


        static readonly string SPACER = Environment.NewLine + "\t";

        /// <summary>
        /// Build an SQLite CREATE TABLE Sql Statement.
        /// </summary>
        /// <param name="TableName">Name of the Table to create.</param>
        /// <param name="Columns">ColumnLayout Definitions for each column in the Table.</param>
        /// <returns></returns>
        internal static string CreateSQL(string TableName, Dictionary<string, ColumnLayout> Columns)
        {
            StringBuilder sb = new StringBuilder();
            int i;
            
            sb.Append("Create Table \"").Append(TableName).Append("\" (\r\n");
            string[] PrimaryKeys = new string[Columns.Count];

            StringBuilder sbFKeyClause = new StringBuilder();

            bool bComma = false;
            string comma = string.Empty;
            foreach (var column in Columns)
            {
                sb.Append("\t").Append(comma).Append(CreateColumnEntry(column.Key, column.Value)).Append("\r\n");
                // Is this a Primary Key or part of a Primary Key?
                if (column.Value.PrimaryKey > 0) PrimaryKeys[column.Value.PrimaryKey - 1] = column.Key;
                // Start constructing the Foriegn Key clause
                if (!string.IsNullOrEmpty(column.Value.ForeignKey.Table))
                {
                    if (sbFKeyClause.Length != 0) sbFKeyClause.Append(",\r\n\t");
                    sbFKeyClause.Append("Foreign Key (\"").Append(column.Key).Append("\") References \"").Append(column.Value.ForeignKey.Table).Append("\"(\"").Append(column.Value.ForeignKey.To).Append("\")");
                    if (!string.IsNullOrEmpty(column.Value.ForeignKey.OnDelete)) sbFKeyClause.Append(" On Delete ").Append(column.Value.ForeignKey.OnDelete);
                    if (!string.IsNullOrEmpty(column.Value.ForeignKey.OnUpdate)) sbFKeyClause.Append(" On Update ").Append(column.Value.ForeignKey.OnUpdate);
                }
                if (!bComma) { bComma = true; comma = ","; }
            }

            // Let's construct the Primary Key clause
            string JoinedKeys = string.Join("|", PrimaryKeys);
            string[] Keys = JoinedKeys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (Keys.Length > 0 && !string.IsNullOrEmpty(Keys[0]))
            {
                sb.Append(",\r\n\t Primary Key(");
                for (i = 0; i < Keys.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(Keys[i])) sb.Append("\"").Append(Keys[i]).Append("\",");
                }
                sb.Append("\"").Append(Keys[i]).Append("\")");
            }

            // Let's construct the Foriegn Key clause
            if (sbFKeyClause.Length > 0)
            {
                sb.Append(",\r\n\t").Append(sbFKeyClause.ToString());
            }
            sb.Append("\r\n);");
            return sb.ToString();
        }

        /// <summary>
        /// Create a column description used in a Create Table command
        /// </summary>
        /// <param name="ColumnName">The Name of the column.</param>
        /// <param name="column">A ColumnLayout structure defining the characteristics of the column.</param>
        /// <returns></returns>
        protected static string CreateColumnEntry(string ColumnName, ColumnLayout column)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\"").Append(ColumnName).Append("\" ");
            sb.Append(column.ColumnType == "autoincrement" ? "integer primary key autoincrement" : column.ColumnType);
            sb.Append(column.NullType == 0 ? " Null" : " Not Null");
            if (column.Unique) sb.Append(" Unique");
            if (!string.IsNullOrEmpty(column.DefaultValue))
            {
                sb.Append(" Default ");
                // If Column Type is a Text Type, wrap the default value in Quotes.
                // Note that Default values that are functions must be preceeded by a "("
                if (!column.DefaultValue.Trim().StartsWith("(") || IsText(column.ColumnType))
                { sb.Append("\"").Append(column.DefaultValue).Append("\""); }
                else
                { sb.Append(column.DefaultValue); }
            }
            if (!string.IsNullOrEmpty(column.Check)) sb.Append(" Check").Append(column.Check);
            if (!string.IsNullOrEmpty(column.Collation)) sb.Append(" Collate ").Append(column.Collation);
            
            return sb.ToString();
        }

        /// <summary>
        /// Generate SQL needed to add a column to a table.
        /// </summary>
        /// <param name="TableName">Name of the table being modified.</param>
        /// <param name="ColumnName">Name of the column being added.</param>
        /// <param name="column">Column descriptor containing attributes of the new column.</param>
        /// <returns>Sql to add a new column.</returns>
        internal static string BuildAddColumnSQL(string TableName, string ColumnName, ColumnLayout column)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Alter Table \"{0}\" \r\n Add Column {1}", TableName, CreateColumnEntry(ColumnName, column));
            return sb.ToString();
        }

        internal static string CreateIndexSQL(string TableName, string IndexName, Dictionary<string, IndexColumnLayout> columns, bool Unique = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Create").Append(Unique ? " Unique" : string.Empty).AppendFormat(" Index {0} On {1} (", IndexName, TableName);
            bool bComma = false;
            foreach (var col in columns)
            {
                sb.Append(bComma ? "," : string.Empty).AppendFormat("{0} {1}", col.Key, col.Value.SortOrder);
                if (!string.IsNullOrEmpty(col.Value.CollatingSequence)) sb.AppendFormat(" Collate {0}", col.Value.CollatingSequence);
                bComma = true;
            }
            sb.Append(")");
            return sb.ToString();
        }

        internal static string DropSql(string TableName)
        {
            return string.Format("Drop Table if Exists \"{0}\"", TableName);
        }

        internal static string IntegrityCheckSql()
        {
            return "Pragma integrity_check";
        }
        internal static string SelectSql(SchemaDefinition sd, string TableName, int count = 0)
        {
            StringBuilder sb = new StringBuilder();

            var tableItem = sd.Tables[TableName];

            sb.Append("Select ");
            bool bComma = false;
            foreach (KeyValuePair<string, ColumnLayout> item in tableItem.Columns)
            {
                sb.Append(SPACER).Append(bComma ? "," : string.Empty).Append("\"").Append(item.Key).Append("\"");
                bComma = true;
            }
            sb.Append(SPACER).Append("FROM ").Append(TableName);
            if (count > 0) sb.Append(SPACER).Append("LIMIT ").Append(count.ToString());

            return sb.ToString();
        }

        internal static string SelectSql(string TableName, Dictionary<string, ColumnLayout> Columns, int count = 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Select ");
            bool bComma = false;
            foreach (var col in Columns)
            {
                sb.Append(SPACER).Append(bComma ? "," : string.Empty).Append("\"").Append(col.Key).Append("\"");
                bComma = true;
            }
            sb.Append(SPACER).Append("FROM ").Append(TableName);
            if (count > 0) sb.Append(SPACER).Append("LIMIT ").Append(count.ToString());

            return sb.ToString();
        }

        internal static string SelectViewSql(SchemaDefinition sd, string ViewName, int count = 0)
        {
            StringBuilder sb = new StringBuilder();

            var viewItem = sd.Views[ViewName];

            sb.Append("Select ");
            bool comma = false;
            foreach (KeyValuePair<string, ColumnLayout> item in viewItem.Columns)
            {
                if (comma) { sb.Append(SPACER).Append(","); }
                else { comma = true; }
                sb.Append("\"").Append(item.Key).Append("\"");
            }
            sb.Append(SPACER).Append("FROM ").Append(ViewName);
            if (count > 0) sb.Append(SPACER).Append("LIMIT ").Append(count.ToString());

            return sb.ToString();
        }

        internal static string InsertSql(string TableName, Dictionary<string, ColumnLayout> Columns)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.Append(string.Format("INSERT INTO {0} (", TableName));
            bool bComma = false;
            foreach (var col in Columns)
            {
                sb.Append(SPACER).Append(bComma ? "," : string.Empty).Append("\"").Append(col.Key).Append("\"");
                bComma = true;
                count++;
            }
            sb.Append(SPACER).Append(") VALUES (?");
            for (int i = 2; i <= count; i++)
            {
                sb.Append(", ?");
            }
            sb.Append(")");

            return sb.ToString();
        }

        internal static string SelectTemplate(SchemaDefinition sd, string TableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(SelectSql(sd, TableName));
            sb.Append(SPACER).Append("Where \"columnname\" = 'value'");
            sb.Append(SPACER).Append("And");
            sb.Append(SPACER).Append("\"columnname\" Like '%value%'");
            sb.Append(SPACER).Append("Or");
            sb.Append(SPACER).Append("\"columnname\" In ('value1', 'value2', 'value3')");
            sb.Append(SPACER).Append("Order By \"columnname\"").Append(";");
            return sb.ToString();
        }

        /// <summary>
        /// Create Sql to nsert data into an existing Table.
        /// </summary>
        /// <param name="sd">Structure containing the schema of the database being managed.</param>
        /// <param name="TableName">Name of the Table to update.</param>
        /// <returns>Displayable Sql to INSERT rows into a Table.</returns>
        internal static string InsertTemplate(SchemaDefinition sd, string TableName)
        {
            StringBuilder sb = new StringBuilder();

            var tableItem = sd.Tables[TableName];

            sb.Append("Insert Into \"").Append(TableName).Append("\"");
            sb.Append(SPACER).Append("(");
            string comma = string.Empty;
            foreach (KeyValuePair<string, ColumnLayout> item in tableItem.Columns)
            {
                sb.Append(SPACER).Append(comma).Append("\"").Append(item.Key).Append("\"");
                comma = ",";
            }
            sb.Append(SPACER).Append(")");
            sb.Append(SPACER).Append("Values");
            sb.Append(SPACER).Append("(");

            comma = string.Empty;
            foreach (KeyValuePair<string, ColumnLayout> item in tableItem.Columns)
            {
                sb.Append(SPACER).Append(comma).Append("<").Append("\"").Append(item.Key).Append("\", ");
                sb.Append(item.Value.ColumnType).Append(">");
                comma = ",";
            }

            sb.Append(SPACER).Append(")");
            sb.Append(";");

            return sb.ToString();
        }

        /// <summary>
        /// Create Sql to update data in an existing Table.
        /// </summary>
        /// <param name="sd">Structure containing the schema of the database being managed.</param>
        /// <param name="TableName">Name of the Table to update.</param>
        /// <returns>Displayable Sql to UPDATE a Table.</returns>
        internal static string UpdateTemplate(SchemaDefinition sd, string TableName)
        {
            StringBuilder sb = new StringBuilder();

            var tableItem = sd.Tables[TableName];

            sb.Append("Update \"").Append(TableName).Append("\"");
            string comma = string.Empty;
            foreach (KeyValuePair<string, ColumnLayout> item in tableItem.Columns)
            {
                sb.Append(SPACER).Append(comma).Append("Set ").Append("\"").Append(item.Key).Append("\" = <").Append(item.Value.ColumnType).Append(">");
                comma = ",";
            }
            sb.Append(SPACER).Append("WHERE \"columnname\" = 'value'");
            sb.Append(";");

            return sb.ToString();
        }

        /// <summary>
        /// Create Sql to delete rows from an existing Table.
        /// </summary>
        /// <param name="TableName">Name of the Table to update.</param>
        /// <returns>Displayable Sql to DELETE rows from a Table.</returns>
        internal static string DeleteTemplate(string TableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Delete From \"").Append(TableName).Append("\"");
            sb.Append(SPACER).Append("Where \"columnname\" = 'value';");
            return sb.ToString();
        }


        /// <summary>
        /// Create Sql to edit an existing View.
        /// </summary>
        /// <param name="sd">Structure containing the schema of the database being managed.</param>
        /// <param name="ViewName">Name of the View to edit.</param>
        /// <returns>Displayable Sql to DROP and create a View.</returns>
        internal static string EditViewSql(SchemaDefinition sd, string ViewName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--***************************************************************************************************************************\r\n");
            sb.Append("--\r\n");
            sb.Append("--     ATTENTION this Script will first delete the View and subsequently add it with your changes.\r\n");
            sb.Append("--     EXERCISE CAUTION!!!.\r\n");
            sb.Append("--\r\n");
            sb.Append("--***************************************************************************************************************************\r\n\r\n");
            sb.Append("--\r\n");
            sb.Append("--\r\n");
            sb.Append("\tDrop View \"").Append(ViewName).Append("\";\r\n");
            sb.Append("--\r\n");
            sb.Append("--\r\n");
            return sb.Append(sd.Views[ViewName].CreateSQL).ToString();
        }

        /// <summary>
        /// Return an SQL Template for building an SQLite View.
        /// </summary>
        internal static string ViewTemplate()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--***************************************************************************************************************************\r\n");
            sb.Append("--\r\n");
            sb.Append("--     SQL Template for a simple view\r\n");
            sb.Append("--\r\n");
            sb.Append("--***************************************************************************************************************************\r\n");
            sb.Append("Create View If Not Exists \"ViewName\"\r\n");
            sb.Append("(\r\n");
            sb.Append("\t\"ColumnName 1\",\r\n");
            sb.Append("\t\"ColumnName 2\",\r\n");
            sb.Append("\t\"ColumnName ...\"\r\n");
            sb.Append(")\r\n");
            sb.Append("As \r\n");
            sb.Append("Select \r\n");
            sb.Append("\t\"ColumnName 1\",\r\n");
            sb.Append("\t\"ColumnName 2\",\r\n");
            sb.Append("\t\"ColumnName ...\"\r\n");
            sb.Append("From \r\n");
            sb.Append("\t\"TableName\" \r\n");
            sb.Append("Where \"ColumnName1\" Like \"%text%\" \r\n");
            sb.Append("Order By \"ColumnName1 Asc\", \"ColumnName2\" Asc, \"ColumnName ...\" Desc");
            return sb.ToString();
        }

        /// <summary>
        /// Create Sql to edit an existing Trigger.
        /// </summary>
        /// <param name="sd">Structure containing the schema of the database being managed.</param>
        /// <param name="TriggerName">Name of the Trigger to edit.</param>
        /// <returns>Displayable Sql to DROP and create a Trigger.</returns>
        internal static string EditTriggerSql(SchemaDefinition sd, string TriggerName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--***************************************************************************************************************************\r\n");
            sb.Append("--\r\n");
            sb.Append("--     ATTENTION this Script will first delete the Trigger and subsequently add it with your changes.\r\n");
            sb.Append("--     EXERCISE CAUTION!!!.\r\n");
            sb.Append("--\r\n");
            sb.Append("--***************************************************************************************************************************\r\n\r\n");
            sb.Append("--\r\n");
            sb.Append("--\r\n");
            sb.Append("\tDrop Trigger \"").Append(TriggerName).Append("\";\r\n");
            sb.Append("--\r\n");
            sb.Append("--\r\n");
            return sb.Append(sd.Triggers[TriggerName].CreateSQL).ToString();

        }

        /// <summary>
        /// Return an SQL Template for building an SQLite Trigger.
        /// </summary>
        internal static string TriggerTemplate()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--***************************************************************************************************************************\r\n");
            sb.Append("--\r\n");
            sb.Append("--     SQL Template for a simple Trigger\r\n");
            sb.Append("--\r\n");
            sb.Append("--***************************************************************************************************************************\r\n");
            sb.Append("Create Trigger If Not Exists \"TriggerName\"\r\n");
            sb.Append("\tBefore|After|Instead Of Delete|Insert|Update On \"TableName\"(\r\n");
            sb.Append("\tWhen Expression,\r\n");
            sb.Append("\tBegin\r\n");
            sb.Append("\t\tUpdate Statement\r\n");
            sb.Append("\t\tInsert Statement\r\n");
            sb.Append("\t\tDelete Statement\r\n");
            sb.Append("\t\tSelect Statement\r\n");
            sb.Append("\t\t;\r\n");
            sb.Append("\tEnd \r\n");
            return sb.ToString();
        }

    }
}
