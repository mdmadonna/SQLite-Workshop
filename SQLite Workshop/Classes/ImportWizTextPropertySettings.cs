using System.Collections;
using System.ComponentModel;

namespace SQLiteWorkshop
{
    class ImportWizTextPropertySettings
    {

        private string name;
        private string columnDelimiter;
        private int columnWidth;
        private string type;
        private bool exclude;
        private int primarykey;
        private bool autoincrement;
        private bool unique;
        private bool allownulls;

        [DisplayName("Column Name"),
        DescriptionAttribute("The Name of this column.")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DisplayName("Column Delimeter"),
        TypeConverter(typeof(ColumnDelimeterConverter)),
        DescriptionAttribute("The Delimeter used to separate this columns from others in the file.")]
        public string ColumnDelimeter
        {
            get { return columnDelimiter; }
            set { columnDelimiter = value; }
        }

        [DisplayName("Column Width"),
        DescriptionAttribute("The maximum width of the column being imported.  This is ignored for all non-text columns.")]
        public int ColumnWidth
        {
            get { return columnWidth; }
            set { columnWidth = value; }
        }

        [DisplayName("Type"),
        TypeConverter(typeof(TypeConverter)),
        DescriptionAttribute("The data type of this column.  Note that internally, SQLite really has datatypes of INTEGER, TEXT, REAL, BLOB and NUMERIC.")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        [DisplayName("Exclude"),
        DescriptionAttribute("Do not import this column.")]
        public bool Exclude
        {
            get { return exclude; }
            set { exclude = value; }
        }

        [DisplayName("Primary Key"),
        DescriptionAttribute("If this column is a primary key or part of a primary key, enter the sequence number of it's position in the primary key (i.e. 1 for 1st column in the key, 2 for the 2nd column in the key, etc.).")]
        public int PrimaryKey
        {
            get { return primarykey; }
            set { primarykey = value; }
        }

        [DisplayName("Auto Increment"),
        DescriptionAttribute("Answer true if this column should autoincrement.")]
        public bool AutoIncrement
        {
            get { return autoincrement; }
            set { exclude = autoincrement; }
        }

        [DisplayName("Unique"),
        DescriptionAttribute("Answer true if all rows contain a unique value in this column.")]
        public bool Unique
        {
            get { return unique; }
            set { exclude = unique; }
        }

        [DisplayName("Allow Null Values"),
        DescriptionAttribute("Answer true if this column may contain a Null Value.")]
        public bool AllowNulls
        {
            get { return allownulls; }
            set { exclude = allownulls; }
        }
    }

    internal class TypeConverter : StringConverter
    {
        private string[] TypeChoices = new string[] { "TEXT", "INTEGER", "REAL", "NUMERIC", "BLOB" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(TypeChoices);
        }

    }

    internal class ColumnDelimeterConverter : StringConverter
    {
        private string[] ColumnDelimeterChoices = new string[] { ",", "|", ";", "TAB", "SPACE" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ColumnDelimeterChoices);
        }

    }
}
