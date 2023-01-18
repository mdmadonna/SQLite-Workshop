using System;
using System.ComponentModel;

namespace SQLiteWorkshop
{
    class ConnectionPropertySettings
    {

        [DisplayName("Database File Name"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("The name of the database file.")]
        public string DbFileName { get; set; }

        [DisplayName("Database Name"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("The name of the database.")]
        public string DbName { get; set; }

        [DisplayName("Execution Start"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("The last execution start time.")]
        public string ExecStart { get; set; }

        [DisplayName("Execution End"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("The last execution end time.")]
        public string ExecEnd { get; set; }

        [DisplayName("Elapsed Time"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("Elapsed time of the last execution.")]
        public string ElapsedTime { get; set; }

        [DisplayName("Rows"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("The number of rows returned or affected.")]
        public string RowsAffected { get; set; }

        [DisplayName("Last Status"),
        ReadOnly(true),
        CategoryAttribute("Connection Properties"),
        DescriptionAttribute("The last returned SQLite Status Code.")]
        public string LastSqlStatus { get; set; }
    }
}
