using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace SQLiteWorkshop
{
    class ImportWizTextPropertySettings
    {

        private string name;
        private int columnWidth;
        private string type;
        private bool exclude;
        private int primarykey;
        private bool unique;
        private bool allownulls;

        [DisplayName("Column Name"),
        ReadOnly(false),
        DescriptionAttribute("The Name of this column.")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DisplayName("Type"),
        ReadOnly(false),
        TypeConverter(typeof(TypeConverter)),
        DescriptionAttribute("The data type of this column.  Note that internally, SQLite only uses datatypes of integer, text, real, blob and numeric.")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        
        [DisplayName("Column Width"),
        ReadOnly(false),
        DescriptionAttribute("The maximum width of the column being imported.  This is ignored for all non-text columns.")]
        public int ColumnWidth
        {
            get { return columnWidth; }
            set { columnWidth = value; }
        }


        [DisplayName("Exclude"),
        ReadOnly(false),
        DescriptionAttribute("True to exclude this column from import.")]
        public bool Exclude
        {
            get { return exclude; }
            set { exclude = value; }
        }

        [DisplayName("Primary Key"),
        ReadOnly(true),
        DescriptionAttribute("If this column is a primary key or part of a primary key, enter the sequence number of it's position in the primary key (i.e. 1 for 1st column in the key, 2 for the 2nd column in the key, etc.).")]
        public int PrimaryKey
        {
            get { return primarykey; }
            set { primarykey = value; }
        }

        [DisplayName("Unique"),
        ReadOnly(false),
        DescriptionAttribute("Answer true if all rows contain a unique value in this column.")]
        public bool Unique
        {
            get { return unique; }
            set { exclude = unique; }
        }

        [DisplayName("Allow Null Values"),
        ReadOnly(false),
        DescriptionAttribute("Answer true if this column may contain a Null Value.")]
        public bool AllowNulls
        {
            get { return allownulls; }
            set { exclude = allownulls; }
        }

        public void SetReadOnly(string property, bool value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[property];
            ReadOnlyAttribute attrib = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
            FieldInfo isReadOnly = attrib.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
            isReadOnly.SetValue(attrib, value);
        }
    }

    internal class TypeConverter : StringConverter
    {
        private string[] TypeChoices = Common.SQLiteTypes;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Array.Sort(TypeChoices);
            return new StandardValuesCollection(TypeChoices);
        }

    }

    internal class ColumnDelimiterConverter : StringConverter
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
