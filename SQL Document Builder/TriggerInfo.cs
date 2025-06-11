using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The trigger info.
    /// </summary>
    internal class TriggerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerInfo"/> class.
        /// </summary>
        public TriggerInfo()
        {
            TriggerType = string.Empty;
            IsDisabled = string.Empty;
            ParentObjectSchema = string.Empty;
            ParentObjectName = string.Empty;
            ParentObjectType = string.Empty;
            CreateDate = DateTime.MinValue;
            ModifyDate = DateTime.MinValue;
        }

        /// <summary>
        /// Gets or sets the type of the trigger (e.g., DML, DDL).
        /// </summary>
        [ReadOnly(true)]
        public string? TriggerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the trigger is disabled.
        /// </summary>
        [ReadOnly(true)]
        public string? IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets the schema of the parent object to which the trigger belongs.
        /// </summary>
        [ReadOnly(true)]
        public string? ParentObjectSchema { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent object to which the trigger belongs.
        /// </summary>
        [ReadOnly(true)]
        public string? ParentObjectName { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent object (e.g., Table, View).
        /// </summary>
        [ReadOnly(true)]
        public string? ParentObjectType { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the trigger was created.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the trigger was last modified.
        /// </summary>
        [ReadOnly(true)]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Opens the async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal async Task OpenAsync(ObjectName objectName, string connectionString)
        {
            string sql = $@"SELECT
    CASE WHEN tr.is_instead_of_trigger = 1 THEN 'INSTEAD OF' ELSE 'AFTER' END AS TriggerType,
    CASE WHEN tr.is_disabled = 1 THEN 'Yes' ELSE 'No' END AS IsDisabled,
    sch.name AS ParentObjectSchema,
    obj.name AS ParentObjectName,
    obj.type_desc AS ParentObjectType,
    tr.create_date AS CreateDate,
    tr.modify_date AS ModifyDate
FROM sys.triggers tr
JOIN sys.objects obj ON tr.parent_id = obj.object_id
JOIN sys.schemas sch ON obj.schema_id = sch.schema_id
WHERE tr.name = N'{objectName.Name}'";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                TriggerType = row["TriggerType"].ToString();
                IsDisabled = row["IsDisabled"].ToString();
                ParentObjectSchema = row["ParentObjectSchema"].ToString();
                ParentObjectName = row["ParentObjectName"].ToString();
                ParentObjectType = row["ParentObjectType"].ToString();
                CreateDate = Convert.ToDateTime(row["CreateDate"]);
                ModifyDate = Convert.ToDateTime(row["ModifyDate"]);
            }
        }
    }
}