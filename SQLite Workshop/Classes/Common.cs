using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        SQLNewQuery
    }

    class Common
    {
        #region Constants
        internal const string APPNAME                   = "SQLite Workshop";
        internal const string StatsTable                = "SQL_Workshop_Stats";

        internal const int MAX_SQL_FILESIZE             = 1048576;

        internal const string NOTIMPLEMENTED            = "This feature is not yet implemented.";
        internal const string MSG_FILEEXISTS            = "This file already exists.  Do you want to overwrite it?";
        internal const string ERR_FILEENTRY             = "Please enter a file name.";

        internal const string OK_BACKUP                 = "Backup Ok: Database {0} copied to {1}.";
        internal const string OK_OPTIMIZE               = "Optmize Ok: Database {0} has been optimized.";
        internal const string OK_CLONE                  = "Clone Ok: Database {0} cloned to {1}.";
        internal const string OK_DBCREATED              = "Create Ok: Database {0} created.";
        internal const string OK_DELINDEX               = "Delete Ok: Index {0} deleted.";
        internal const string OK_DELALLINDEXES          = "Delete Ok: All Indexes on Table {0} deleted.";
        internal const string OK_DELTRIGGER             = "Delete Ok: Trigger {0} deleted.";
        internal const string OK_DELVIEW                = "Delete Ok: Index {0} deleted.";
        internal const string OK_ENCRYPT                = "Encrypt Ok: Database {0} encrypted.";
        internal const string OK_EXPLAIN                = "Explain executed successfully.";
        internal const string OK_IDXCREATED             = "Create Ok: Index {0} created.";
        internal const string OK_QUERY                  = "Query Executed Successfully.";
        internal const string OK_RECORDSAFFECTED        = "{0} records affected.";
        internal const string OK_REINDEX                = "Reindex Ok: Index {0} rebuilt.";
        internal const string OK_REINDEXALL             = "Reindex Ok: All indexes on Table {0} rebuilt.";
        internal const string OK_RENAME                 = "Rename Ok: Table {0} renamed.";
        internal const string OK_SQL                    = "SQL Executed Successfully.";
        internal const string OK_TBLDELETE              = "Delete Ok: Table {0} deleted.";
        internal const string OK_VACUUM                 = "Compress Ok: Database {0} compressed.";
        
        #region Error Messages
        internal const string ERR_BACKUPFAILED          = "Backup Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_BACKUPSAMEFILE        = "Source database and target database are the same file. Select a different target file.";
        internal const string ERR_CLONEFAILED           = "Clone Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_CREATEDBFAILED        = "Create Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_CREATEIDXFAILED       = "Create Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_ENCRYPTFAILED         = "Encryption Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_EXPLAIN               = "Explain Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_EXPLAINERR            = "Explain executed with errors.";
        internal const string ERR_FORMATERROR           = "Formatting errors were detected.  Not all rows are viewable.";
        internal const string ERR_MULTIUPDATE           = "This update cannot be made because it would cause multiple rows to be modified.";
        internal const string ERR_OPEN                  = "Open Error: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_QUERY                 = "Query Executed with errors.";
        internal const string ERR_RENAMEFAIL            = "Rename failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_SQL                   = "SQL Execution Failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_SQLWIDE               = "SQL Executed with errors.\r\n\r\n{0} errors encountered.\r\nSQL Error Code: {1}.\r\n\r\nThe last {2} error(s) are:\r\n\r\n{3}";
        internal const string ERR_SQLERR                = "SQL Executed with errors.";
        internal const string ERR_VACUUMFAIL            = "Compress failed: {0}\r\nSQL Error Code: {1}.";
        internal const string ERR_GENERAL               = "Error: {0}.";
        #endregion

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
        /// <returns>True if the word is a reserved word, oyherwise false.</returns>
        internal static bool IsKeyword(string token)
        {
            return keywords.Contains(token.ToUpper());
        }

        /// <summary>
        /// List of characters that we won't allow in column names.
        /// </summary>
        static string invalidChars = "'\"@?\\%";

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
        static string[] SysTables = new string[] { "sqlite_master", "sqlite_stat1", "sqlite_sequence", "sqlite_stat2", "sqlite_stat3", "sqlite_stat4" };

        /// <summary>
        /// Determine if a string (table name) is a valid SQLite system table.
        /// </summary>
        /// <param name="tableName">Name of the SQLite table to check.</param>
        /// <returns>True if the name is an SQLite system table, otherwise false.</returns>
        internal static bool IsSystemTable(string tableName)
        {
            return SysTables.Contains(tableName);
        }

        /// <summary>
        /// Helper routines to determine the Universal Naming Convention(UNC) version of a file name.
        /// This routine is used to insure a file will not be copied on top of itself.
        /// </summary>
        /// <param name="lpLocalPath"></param>
        /// <param name="dwInfoLevel"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="lpBufferSize"></param>
        /// <returns></returns>
        #region Get UNC

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
        /// <returns></returns>
        internal static string GetUniversalName(string localPath)
        {

            string fileName = null;
            IntPtr buffer = IntPtr.Zero;

            try
            {
                int size = 0;
                int returnCode = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);
                if (returnCode != ERROR_MORE_DATA) return null;

                buffer = Marshal.AllocCoTaskMem(size);
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
                long count = (long)DataAccess.ExecuteScalar(MainForm.mInstance.CurrentDB, sql, out SQLiteErrorCode returnCode);
                if (count == 0) return tmpTable;
                workTable = string.Format("{0}_{1}", tmpTable, i.ToString());
            }
            return string.Empty;

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
            // Note that this routines assume all Column types are in lower case
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
            // Note that this routines assume all Column types are in lower case
            foreach (string szType in IntegerTypes)
            {
                if (ColType.StartsWith(szType)) return true;
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
            // Note that this routines assume all Column types are in lower case
            foreach (string szType in RealTypes)
            {
                if (ColType.StartsWith(szType)) return true;
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
            // Note that this routines assume all Column types are in lower case
            foreach (string szType in BooleanTypes)
            {
                if (ColType.StartsWith(szType)) return true;
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
            // Note that this routines assume all Column types are in lower case
            foreach (string szType in NumericTypes)
            {
                if (ColType.StartsWith(szType)) return true;
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
            // Note that this routines assume all Column types are in lower case
            foreach (string szType in DateTypes)
            {
                if (ColType.StartsWith(szType)) return true;
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
            // Note that this routines assume all Column types are in lower case
            foreach (string szType in NumberTypes)
            {
                if (ColType.StartsWith(szType)) return true;
            }
            return false;
        }

        internal static bool IsSelect(string sql)
        {
            string strippedSql = RemoveComments(sql, true).Trim().ToLower();

            if (strippedSql.StartsWith("select") || strippedSql.StartsWith("pragma") || strippedSql.StartsWith("with")) return true;
            return false;
        }

        private static Regex everythingExceptNewLines = new Regex("[^\r\n]");
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
        static string _password = null;
        static byte[] _salt = new byte[] { 0x05, 0x6A, 0xE7, 0x6e, 0x20, 0x00, 0x01, 0x67, 0x76, 0x65, 0x9a, 0x03, 0x6F };

        internal static string password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// Decrypt a string - initialize password before using
        /// </summary>
        /// <param name="cipherText">string to decrypt</param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        internal static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(_password)) throw new Exception("Password not initialized.");
            if (string.IsNullOrEmpty(cipherText)) return null;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(_password, _salt);
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
            if (string.IsNullOrEmpty(_password)) throw new Exception("Password not initialized.");
            if (string.IsNullOrEmpty(clearText)) return null;
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(_password, _salt);
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
    }
}
