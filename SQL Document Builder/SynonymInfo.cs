using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents information about a SQL Server synonym.
    /// </summary>
    public class SynonymInfo
    {
        /// <summary>
        /// Gets or sets the base object name that the synonym refers to.
        /// </summary>
        [ReadOnly(true)]
        public string? BaseObjectName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the schema of the base object.
        /// </summary>
        [ReadOnly(true)]
        public string? BaseObjectType { get; private set; }

        /// <summary>
        /// Gets or sets the creation date of the synonym.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the synonym.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Asynchronously loads the synonym information for the specified object from the database.
        /// </summary>
        /// <param name="objectName">The <see cref="ObjectName"/> representing the synonym.</param>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            var sql = $@"
SELECT
    s.name AS SynonymName,
    SCHEMA_NAME(s.schema_id) AS SynonymSchema,
    s.base_object_name AS BaseObjectName,
    o.type_desc AS BaseObjectType,
    s.create_date AS CreateDate,
    s.modify_date AS ModifyDate
FROM
    sys.synonyms AS s
LEFT JOIN
    sys.objects AS o ON OBJECT_ID(s.base_object_name) = o.object_id
WHERE
    s.object_id = OBJECT_ID(N'{objectName.FullName}')";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                BaseObjectName = row["BaseObjectName"].ToString();
                BaseObjectType = row["BaseObjectType"].ToString();
                CreateDate = Convert.ToDateTime(row["CreateDate"]);
                ModifyDate = Convert.ToDateTime(row["ModifyDate"]);
            }
        }
    }
}