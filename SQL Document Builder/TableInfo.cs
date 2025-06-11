using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents summary information about a SQL Server table.
    /// </summary>
    internal class TableInfo
    {
        /// <summary>
        /// Gets or sets the table creation date.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the table last modification date.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        [ReadOnly(true)]
        public long RowCount { get; set; }

        /// <summary>
        /// Gets or sets the data size in megabytes.
        /// </summary>
        [ReadOnly(true)]
        public double DataSizeMB { get; set; }

        /// <summary>
        /// Loads table info asynchronously for the given table.
        /// </summary>
        /// <param name="objectName">The table's object name.</param>
        /// <param name="connectionString">The connection string.</param>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            string sql = $@"
SELECT 
    t.create_date AS CreateDate,
    t.modify_date AS ModifyDate,
    p.rows AS [RowCount],
    au.total_pages * 8 / 1024.0 AS DataSizeMB
FROM sys.tables t
JOIN sys.indexes i ON t.object_id = i.object_id AND i.index_id IN (0,1)
JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
JOIN sys.allocation_units au ON p.partition_id = au.container_id
WHERE t.object_id = OBJECT_ID(N'{objectName.FullName}')";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

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