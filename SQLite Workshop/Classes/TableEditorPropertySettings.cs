using System.Collections;
using System.ComponentModel;

namespace SQLiteWorkshop
{
    class TableEditorPropertySettings
    {

        private string fkParent;
        private string fkColumn;
        private string fkOnUpdate;
        private string fkOnDelete;
        private string defaultValue;
        private string collatingSequence;
        private string checkConstraint;

        internal static string tablename;

        [DisplayName("Default Value"),
        DescriptionAttribute("The Default Value for this column. Please enclose functions in parentheses (i.e. (datetime('now','localtime')).")]
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
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

        [DisplayName("Collating Sequence"),
        TypeConverter(typeof(CollatingSequenceConverter)),
        DescriptionAttribute("The Collating Sequence for this column.")]
        public string CollatingSequence
        {
            get { return collatingSequence; }
            set { collatingSequence = value; }
        }

        [DescriptionAttribute("A Check Constraint for this column.  The value must be surrounded by quotes (i.e. The action to take when a Foreign Key is deleted.")]
        public string CheckConstraint
        {
            get { return checkConstraint; }
            set { checkConstraint = value; }
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
    }

    internal class CollatingSequenceConverter : StringConverter
    {
        private string[] CollatingSequenceChoices = new string[] { string.Empty, "BINARY", "NOCASE", "RTRIM" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(CollatingSequenceChoices);
        }

    }

    internal class ForeignTableConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(BuildTableList());
        }

        private string[] BuildTableList()
        {

            SchemaDefinition sd = DataAccess.SchemaDefinitions[MainForm.mInstance.CurrentDB];
            ArrayList tableList = new ArrayList();

            tableList.Add(string.Empty);
            foreach (var table in sd.Tables)
            {
                if (table.Value.TblType != SQLiteTableType.system) tableList.Add(table.Key);
            }

            return tableList.ToArray(typeof(string)) as string[];
        }

    }

    internal class ForeignKeyActionConverter : StringConverter
    {
        private string[] ForeignKeyActionChoices = new string[] { string.Empty, "SET NULL", "SET DEFAULT", "RESTRICT", "CASCADE" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ForeignKeyActionChoices);
        }

    }
}
