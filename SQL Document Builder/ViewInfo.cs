using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents summary information about a SQL Server view.
    /// </summary>
    internal class ViewInfo
    {
        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the modify date.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Loads view info asynchronously for the given view.
        /// </summary>
        /// <param name="objectName">The view's object name.</param>
        /// <param name="connectionString">The connection string.</param>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            string sql = $@"
SELECT
    v.create_date AS CreateDate,
    v.modify_date AS ModifyDate
FROM sys.views v
WHERE v.object_id = OBJECT_ID(N'{objectName.FullName}')";
            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                CreateDate = row["CreateDate"] != DBNull.Value ? Convert.ToDateTime(row["CreateDate"]) : null;
                ModifyDate = row["ModifyDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifyDate"]) : null;
            }
        }
    }
}