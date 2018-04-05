using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBPropertySettings
    {
        internal string[] FKList { get; set; }

        [DisplayName("Database File Name"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The name of the database file.")]
        public string DbFileName { get; set; }
        
        [DisplayName("Database Name"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The name of the database.")]
        public string DbName { get; set; }

        [DisplayName("Database Size"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The size, in bytes, of the database.")]
        public string DbSize { get; set; }

        [DisplayName("Creation Date"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The date this database was created..")]
        public string DbCreateDate { get; set; }

        [DisplayName("Last Update"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The date this database was last updated.")]
        public string DbLastUpdate { get; set; }

        [DisplayName("Tables"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The number of tables in this database.")]
        public string DbTables { get; set; }

        [DisplayName("Indexes"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The number of indexes in this database.")]
        public string DbIndexes { get; set; }

        [DisplayName("Views"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The number of views in this database.")]
        public string DbViews { get; set; }

        [DisplayName("Triggers"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The number of Triggers in this database.")]
        public string DbTriggers { get; set; }

        [DisplayName("Encoding"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The text encoding used by the main database.")]
        public string DbEncoding { get; set; }

        [DisplayName("Foreign Key Check"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("A list of foreign key constraints that are violated.")]
        public string DbForeignKeyCheck { get; set; }

        /// <summary>
        /// Declared internal for the time being.  I may in the future build a list
        /// of all foreign keys in the database and place it here.
        /// </summary>
        [DisplayName("Foreign Key List"),
        ReadOnly(true),
        TypeConverter(typeof(FKListConverter)),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("A list of foreign key constraints in the database.")]
        internal string DbForeignKeyList { get; set; }

        [DisplayName("Free List Count"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The number of unused pages in the database file.")]
        public string DbFreeListCount { get; set; }

        [DisplayName("Page Count"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The total number of pages in the database file.")]
        public string DbPageCount { get; set; }

        [DisplayName("Page Size"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The default size of each page in this database.")]
        public string DbPageSize { get; set; }

        [DisplayName("Schema Version"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The value of the schema-version integer at offset 40 in the database header.")]
        public string DbSchemaVersion { get; set; }

        [DisplayName("User Version"),
        ReadOnly(true),
        CategoryAttribute("Database Properties"),
        DescriptionAttribute("The value of the user-version integer at offset 60 in the database header.")]
        public string DbUserVersion { get; set; }

        public class FKListConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                DBPropertySettings db = context.Instance as DBPropertySettings;
                Array.Sort(db.FKList);
                return new StandardValuesCollection(db.FKList);
            }

        }
    }
}
