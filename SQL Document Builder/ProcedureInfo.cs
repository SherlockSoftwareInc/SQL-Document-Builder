using System;
using System.ComponentModel;
using System.Data;
using SQL_Document_Builder.SchemaMetadata;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents information about a SQL stored procedure.
    /// </summary>
    internal class ProcedureInfo
    {
        /// <summary>
        /// Gets or sets the creation date of the procedure.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the procedure was created.")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the procedure is a system object.
        /// </summary>
        [ReadOnly(true)]
        [Description("Indicates whether the procedure is a system object.")]
        public bool IsSystemObject { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the procedure.
        /// </summary>
        [ReadOnly(true)]
        [Description("The date and time when the procedure was last modified.")]
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the procedure name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The name of the procedure.")]
        public string? ProcedureName { get; set; }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        [ReadOnly(true)]
        [Description("The schema name of the procedure.")]
        public string? SchemaName { get; set; }

        /// <summary>
        /// Asynchronously loads procedure information from the database using DatabaseHelper and assigns to this instance.
        /// </summary>
        /// <param name="connectionString">The SQL connection string.</param>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="procName">The procedure name.</param>
        /// <returns>A <see cref="Task{bool}"/> indicating if the procedure was found and loaded.</returns>
        public async Task<bool> OpenAsync(ObjectName objectName, string connectionString)
        {
            SchemaName = objectName.Schema;
            ProcedureName = objectName.Name;

            DataTable? dt = await SchemaMetadataProviderContext.Current.GetProcedureInfoAsync(objectName, connectionString);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                CreateDate = row.Field<DateTime>("CreateDate");
                ModifyDate = row.Field<DateTime>("ModifyDate");
                IsSystemObject = row.Field<bool>("IsSystemObject");
                return true;
            }
            return false;
        }
    }
}