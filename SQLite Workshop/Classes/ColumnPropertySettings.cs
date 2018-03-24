using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class ColumnPropertySettings
    {

        private string fkParent;
        private string fkColumn;
        private string fkOnUpdate;
        private string fkOnDelete;
        private string type;
        private int size;
        private int precision;
        private int scale;
        private bool allownull;
        private string defaultValue;
        private string collatingSequence;

        internal static string tablename;

        [DisplayName("Type"),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(ColumnTypeConverter)),
        DescriptionAttribute("The type of data this column will hold.")]
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                SetReadOnly(new string[] { "Size" }, type == "integer" ? true : false);
            }
        }

        [DisplayName("Size"),
        DescriptionAttribute("The size of this column.")]
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        [DisplayName("Precision"),
        DescriptionAttribute("The precision of this column.")]
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        [DisplayName("Scale"),
        DescriptionAttribute("The scale of this column.")]
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        [DisplayName("Allow Nulls"),
        DescriptionAttribute("Enter true to allow null values in this column. If you enter false, you must enter a Default Value.")]
        public bool AllowNull
        {
            get { return allownull; }
            set { allownull = value; }
        }


        [DisplayName("Default Value"),
        DescriptionAttribute("The Default Value for this column. Note that functions are not allowed.")]
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        [DisplayName("Collating Sequence"),
        TypeConverter(typeof(CollatingSequenceConverter)),
        DescriptionAttribute("The Collating Sequence for this column.")]
        public string CollatingSequence
        {
            get { return collatingSequence; }
            set { collatingSequence = value; }
        }


        [DisplayName("Foreign Key Parent Table"),
        CategoryAttribute("Foreign Key"),
        TypeConverter(typeof(ForeignTableConverter)),
        DescriptionAttribute("The Table containing the Foreign Key Column.")]
        public string ForeignKeyParent
        {
            get { return fkParent; }
            set { fkParent = value; }
        }

        [DisplayName("Foreign Key Column Name"),
        CategoryAttribute("Foreign Key"),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(ForeignColumnConverter)),
        DescriptionAttribute("The Foreign Key Column.")]
        public string ForeignKeyColumn
        {
            get { return fkColumn; }
            set { fkColumn = value; }
        }

        [DisplayName("Foreign Key OnUpdate Action"),
        CategoryAttribute("Foreign Key"),
        TypeConverter(typeof(ForeignKeyActionConverter)),
        DescriptionAttribute("The action to take when a Foreign Key is updated.")]
        public string ForeignKeyOnUpdate
        {
            get { return fkOnUpdate; }
            set { fkOnUpdate = value; }
        }

        [DisplayName("Foreign Key OnDelete Action"),
        CategoryAttribute("Foreign Key"),
        TypeConverter(typeof(ForeignKeyActionConverter)),
        DescriptionAttribute("The action to take when a Foreign Key is deleted.")]
        public string ForeignKeyOnDelete
        {
            get { return fkOnDelete; }
            set { fkOnDelete = value; }
        }

        public void ReadOnly(bool value)
        {
            string[] props = new string[]
            {
                "ForeignKeyParent",
                "ForeignKeyColumn",
                "ForeignKeyOnUpdate",
                "ForeignKeyOnDelete",
                "Type",
                "Size",
                "Precision",
                "Scale",
                "AllowNull",
                "DefaultValue",
                "CollatingSequence"
            };
            SetReadOnly(props, value);
        }

        protected void SetReadOnly(string[] props, bool value)
        {
            foreach (string prop in props)
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[prop];
                ReadOnlyAttribute attrib = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
                FieldInfo isReadOnly = attrib.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                isReadOnly.SetValue(attrib, value);
            }
        }

        internal void LoadFKColumns(string table)
        {
            if (tablename == table) return;
            tablename = table;
            fkColumn = string.Empty;
            if (string.IsNullOrEmpty(table))
            {
                fkOnDelete = string.Empty;
                fkOnUpdate = string.Empty;
            }
        }

        internal class ForeignColumnConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var t = this;

                return new StandardValuesCollection(BuildColumnList());
            }

            private string[] BuildColumnList()
            {
                var t = this;
                if (string.IsNullOrEmpty(TableEditorPropertySettings.tablename)) return null;

                SchemaDefinition sd = DataAccess.SchemaDefinitions[MainForm.mInstance.CurrentDB];
                TableLayout table = sd.Tables[TableEditorPropertySettings.tablename];
                ArrayList columnList = new ArrayList();

                columnList.Add(string.Empty);
                foreach (var column in sd.Tables[TableEditorPropertySettings.tablename].Columns)
                {
                    columnList.Add(column.Key);
                }

                return columnList.ToArray(typeof(string)) as string[];
            }

        }

        internal class ColumnTypeConverter : StringConverter
        {
            private string[] TypeChoices = new string[] { "char", "varchar", "text", "integer", "datetime", "date", "float", "single", "double", "decimal" };
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(TypeChoices);
            }

        }
    }
}
