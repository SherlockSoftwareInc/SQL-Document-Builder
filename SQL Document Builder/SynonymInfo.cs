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
        [Description("The fully qualified name of the base object that the synonym refers to.")]
        public string? BaseObjectName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the schema of the base object.
        /// </summary>
        [ReadOnly(true)]
        [Description("The type description of the base object (e.g., TABLE, VIEW, PROCEDURE, etc.).")]
        public string? BaseObjectType { get; private set; }

        /// <summary>
        /// Gets or sets the creation date of the synonym.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the synonym was created.")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the synonym.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the synonym was last modified.")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The schema name of the synonym.")]
        public string? SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the synonym name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The name of the synonym.")]
        public string? SynonymName { get; set; }

        /// <summary>
        /// Asynchronously loads the synonym information for the specified object from the database.
        /// </summary>
        /// <param name="objectName">The <see cref="ObjectName"/> representing the synonym.</param>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            SchemaName = objectName.Schema;
            SynonymName = objectName.Name;

            var dt = await SQLDatabaseHelper.GetSynonymInfoAsync(objectName, connectionString);

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