using System;
using System.Data;
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
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the procedure.
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the procedure is a system object.
        /// </summary>
        public bool IsSystemObject { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureInfo"/> class.
        /// </summary>
        public ProcedureInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureInfo"/> class with specified values.
        /// </summary>
        /// <param name="createDate">The creation date.</param>
        /// <param name="modifyDate">The modification date.</param>
        /// <param name="isSystemObject">Whether it is a system object.</param>
        public ProcedureInfo(DateTime createDate, DateTime modifyDate, bool isSystemObject)
        {
            CreateDate = createDate;
            ModifyDate = modifyDate;
            IsSystemObject = isSystemObject;
        }

        /// <summary>
        /// Asynchronously loads procedure information from the database using DatabaseHelper and assigns to this instance.
        /// </summary>
        /// <param name="connectionString">The SQL connection string.</param>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="procName">The procedure name.</param>
        /// <returns>A <see cref="Task{bool}"/> indicating if the procedure was found and loaded.</returns>
        public async Task<bool> OpenAsync(ObjectName objectName, string connectionString)
        {
            string sql = $@"
SELECT 
    o.create_date AS CreateDate,
    o.modify_date AS ModifyDate,
    o.is_ms_shipped AS IsSystemObject
FROM sys.procedures p
JOIN sys.objects o ON p.object_id = o.object_id
JOIN sys.schemas s ON p.schema_id = s.schema_id
WHERE p.object_id = OBJECT_ID(N'{objectName.FullName}')";

            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
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