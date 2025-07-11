using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents information about a SQL Server function.
    /// </summary>
    internal class FunctionInfo
    {
        /// <summary>
        /// Gets or sets the creation date of the function.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the function was created.")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The name of the function.")]
        public string? FunctionName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the function is a system object.
        /// </summary>
        [ReadOnly(true)]
        [Description("Indicates whether the function is a system object.")]
        public bool? IsSystemObject { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the function.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the function was last modified.")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the type description of the function (e.g., SQL_SCALAR_FUNCTION).
        /// </summary>
        [ReadOnly(true)]
        [Description("The type description of the function.")]
        public string? ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The schema name of the function.")]
        public string? SchemaName { get; set; }

        /// <summary>
        /// Loads function information from the database and assigns to this instance.
        /// </summary>
        /// <param name="objectName">The function's object name.</param>
        /// <param name="connectionString">The SQL connection string.</param>
        /// <returns>A <see cref="Task{bool}"/> indicating if the function was found and loaded.</returns>
        public async Task<bool> OpenAsync(ObjectName objectName, string connectionString)
        {
            SchemaName = objectName.Schema;
            FunctionName = objectName.Name;

            if (objectName == null || string.IsNullOrEmpty(objectName.Name) || string.IsNullOrEmpty(objectName.Schema))
                return false;

            string sql = $@"
SELECT
    o.create_date AS CreateDate,
    o.modify_date AS ModifyDate,
    o.is_ms_shipped AS IsSystemObject,
    o.type_desc AS ObjectType
FROM
    sys.objects o
JOIN
    sys.schemas s ON o.schema_id = s.schema_id
WHERE
    o.object_id = OBJECT_ID(N'{objectName.Schema}.{objectName.Name}')
    AND o.type IN ('FN', 'IF', 'TF');";

            DataTable? dt = await SQLDatabaseHelper.GetDataTableAsync(sql, connectionString);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                CreateDate = row.Field<DateTime?>("CreateDate");
                ModifyDate = row.Field<DateTime?>("ModifyDate");
                IsSystemObject = row.Field<bool?>("IsSystemObject");
                ObjectType = row["ObjectType"]?.ToString();
                return true;
            }
            return false;
        }
    }
}