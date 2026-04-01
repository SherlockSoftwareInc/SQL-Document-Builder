using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents summary information about a SQL Server table.
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// Gets or sets the table creation date.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the table was created.")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the data size in megabytes.
        /// </summary>
        [ReadOnly(true)]
        [Description("The size of the table's data in megabytes.")]
        public double DataSizeMB { get; set; }

        /// <summary>
        /// Gets or sets the table last modification date.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the table was last modified.")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        [ReadOnly(true)]
        [Description("The number of rows in the table.")]
        public long RowCount { get; set; }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The schema name of the table.")]
        public string? SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The name of the table.")]
        public string? TableName { get; set; }

        /// <summary>
        /// Loads table info asynchronously for the given table.
        /// </summary>
        /// <param name="objectName">The table's object name.</param>
        /// <param name="connectionString">The connection string.</param>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            TableName = objectName.Name;
            SchemaName = objectName.Schema;

            var dt = await SQLDatabaseHelper.GetTableInfoAsync(objectName, connectionString);

            if (dt?.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                CreateDate = row["CreateDate"] != DBNull.Value ? Convert.ToDateTime(row["CreateDate"]) : null;
                ModifyDate = row["ModifyDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifyDate"]) : null;
                RowCount = row["RowCount"] != DBNull.Value ? Convert.ToInt64(row["RowCount"]) : 0;
                DataSizeMB = row["DataSizeMB"] != DBNull.Value ? Convert.ToDouble(row["DataSizeMB"]) : 0.0;
            }
        }
    }
}