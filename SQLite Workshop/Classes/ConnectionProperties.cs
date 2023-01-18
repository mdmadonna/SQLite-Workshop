using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    internal class ConnectionProperties
    {
        //SchemaDefinition sd;
        internal ConnectionPropertySettings connSettings { get; set; }

        internal ConnectionProperties()
        {
            connSettings = new ConnectionPropertySettings();
        }
        ~ConnectionProperties()
        { }

        protected void LoadConnProperties()
        {
            //sd = DataAccess.GetSchema(DatabaseLocation);
            //connSettings.DbFileName = DatabaseLocation;
            //connSettings.DbName = sd.DBName;
        }
    }
}
