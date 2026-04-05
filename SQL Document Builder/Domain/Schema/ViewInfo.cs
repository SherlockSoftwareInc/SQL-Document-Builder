using System;
using System.ComponentModel;
using SQL_Document_Builder.SchemaMetadata;
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
        [Description("The date and time when the view was created.")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the modify date.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the view was last modified.")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The schema name of the view.")]
        public string? SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the view name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The name of the view.")]
        public string? ViewName { get; set; }

        /// <summary>
        /// Loads view info asynchronously for the given view.
        /// </summary>
        /// <param name="objectName">The view's object name.</param>
        /// <param name="connectionString">The connection string.</param>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            SchemaName = objectName.Schema;
            ViewName = objectName.Name;

            var dt = await SchemaMetadataProviderContext.Current.GetViewInfoAsync(objectName, connectionString);

            if (dt?.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                CreateDate = row["CreateDate"] != DBNull.Value ? Convert.ToDateTime(row["CreateDate"]) : null;
                ModifyDate = row["ModifyDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifyDate"]) : null;
            }
        }
    }
}