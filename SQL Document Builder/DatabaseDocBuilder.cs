using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The database doc builder.
    /// </summary>
    internal class DatabaseDocBuilder
    {
        /// <summary>
        /// Gets the SQL script to create the specified database object.
        /// </summary>
        /// <param name="dbObject">The database object for which the creation script is to be generated.</param>
        /// <returns>A Task<string?> containing the creation script, or null if the object type is unsupported.</returns>
        public static async Task<string?> GetCreateObjectScriptAsync(ObjectName dbObject, DatabaseConnectionItem connection)
        {
            if (dbObject == null)
            {
                throw new ArgumentNullException(nameof(dbObject), "The database object cannot be null.");
            }

            if (dbObject.IsEmpty())
            {
                throw new InvalidOperationException("The database object must have a valid schema and name.");
            }

            // Determine the object type and retrieve the corresponding creation script
            return dbObject.ObjectType switch
            {
                ObjectName.ObjectTypeEnums.Table => await GetCreateTableScriptAsync(dbObject, connection),
                ObjectName.ObjectTypeEnums.View => await GetCreateViewScriptAsync(dbObject, connection),
                ObjectName.ObjectTypeEnums.StoredProcedure => await GetCreateStoredProcedureScriptAsync(dbObject, connection),
                ObjectName.ObjectTypeEnums.Function => await GetCreateFunctionScriptAsync(dbObject, connection),
                ObjectName.ObjectTypeEnums.Trigger => await GetCreateTriggerScriptAsync(dbObject, connection),
                ObjectName.ObjectTypeEnums.Synonym => await GetCreateSynonymScriptAsync(dbObject, connection),
                _ => throw new NotSupportedException($"The object type '{dbObject.ObjectType}' is not supported.")
            };
        }

        /// <summary>
        /// Gets the create synonym script async.
        /// </summary>
        /// <param name="dbObject">The db object.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateSynonymScriptAsync(ObjectName dbObject, DatabaseConnectionItem connection)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Synonym {dbObject.FullName} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                // Add drop synonym statement
                createScript.AppendLine($"IF OBJECT_ID(N'{dbObject.FullName}', 'SN') IS NOT NULL");
                createScript.AppendLine($"\tDROP SYNONYM {dbObject.FullName};");
                createScript.AppendLine("GO");
            }

            // Query to get the base object name for the synonym
            string sql = $@"
SELECT s.base_object_name
FROM sys.synonyms s
INNER JOIN sys.schemas sch ON s.schema_id = sch.schema_id
WHERE sch.name = N'{dbObject.Schema}' AND s.name = N'{dbObject.Name}'";

            var dt = await SQLDatabaseHelper.GetDataTableAsync(sql, connection.ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            string baseObjectName = dt.Rows[0]["base_object_name"]?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(baseObjectName))
                return string.Empty;

            // Add the CREATE SYNONYM statement
            createScript.AppendLine($"CREATE SYNONYM {dbObject.FullName} FOR {baseObjectName};");
            createScript.AppendLine("GO");

            return createScript.ToString();
        }

        /// <summary>
        /// Convert the Query data to the insert statements.
        /// </summary>
        /// <param name="sql">The SQL query to fetch data.</param>
        /// <param name="tableName">The table name for the insert statements.</param>
        /// <returns>A Task<string> containing the generated insert statements or a warning if rows exceed 500.</returns>
        public static async Task<string> QueryDataToInsertStatementAsync(string sql, DatabaseConnectionItem connection, string tableName = "YourTableName", bool hasIdentity = false)
        {
            var sb = new StringBuilder();

            try
            {
                // Use SQLDatabaseHelper to get the data as a DataTable
                var dt = await SQLDatabaseHelper.GetDataTableAsync(sql, connection.ConnectionString);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Get the column names
                    var columnNames = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}]"));

                    int rowCount = 0;
                    int batchCount = 0;

                    if (hasIdentity)
                    {
                        // add SET IDENTITY_INSERT  ON;
                        sb.AppendLine($"SET IDENTITY_INSERT {tableName} ON;");
                    }

                    int batchSize = Properties.Settings.Default.InsertBatchRows;
                    if (batchSize <= 0)
                    {
                        batchSize = 20; // Default batch size
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        // Start a new batch every batchSize rows
                        if (rowCount % batchSize == 0)
                        {
                            if (batchCount > 0)
                            {
                                // Remove the trailing comma from the previous batch
                                sb.Length -= 3; // Remove ",\n"
                                sb.AppendLine(";");
                            }
                            sb.AppendLine($"INSERT INTO {tableName} ({columnNames}) VALUES");
                            batchCount++;
                        }

                        sb.Append("\t(");
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var value = row[i];
                            var type = dt.Columns[i].DataType;

                            if (value == DBNull.Value)
                            {
                                sb.Append("NULL");
                            }
                            else if (type == typeof(string))
                            {
                                sb.Append("N'" + value.ToString().Replace("'", "''") + "'");
                            }
                            else if (type == typeof(DateTime))
                            {
                                sb.Append("'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            }
                            else if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) ||
                                     type == typeof(decimal) || type == typeof(double) || type == typeof(float) ||
                                     type == typeof(byte))
                            {
                                sb.Append(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
                            }
                            else if (type == typeof(bool))
                            {
                                sb.Append((bool)value ? "1" : "0");
                            }
                            else
                            {
                                sb.Append("N'" + value.ToString().Replace("'", "''") + "'");
                            }

                            if (i < dt.Columns.Count - 1)
                            {
                                sb.Append(", ");
                            }
                        }
                        sb.AppendLine("),");

                        rowCount++;
                    }

                    // Remove the trailing comma from the last row in the last batch
                    if (sb.Length > 0)
                    {
                        sb.Length -= 3; // Remove ",\n"
                        sb.AppendLine(";");
                    }

                    if (hasIdentity)
                    {
                        sb.AppendLine($"SET IDENTITY_INSERT {tableName} OFF;");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Pull data from a table or view and convert the data to insert statements.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <returns>A Task.</returns>
        public static async Task<string> TableToInsertStatementAsync(ObjectName tableName, DatabaseConnectionItem? connection)
        {
            if (tableName.IsEmpty() || connection == null || string.IsNullOrWhiteSpace(connection?.ConnectionString))
            {
                throw new ArgumentException("Invalid table name or connection.");
            }

            var sql = $"select * from {tableName.FullName}";
            var hasIdentity = await SQLDatabaseHelper.HasIdentityColumnAsync(tableName, connection.ConnectionString);
            return await QueryDataToInsertStatementAsync(sql, connection, tableName.FullName, hasIdentity);
        }

        /// <summary>
        /// Generates the SQL script to recreate all non-primary key, non-unique constraint indexes for the specified table.
        /// Handles XML indexes with the correct syntax.
        /// </summary>
        /// <returns>A string containing the SQL script to recreate the indexes.</returns>
        internal static async Task<string?> GetCreateIndexesScript(ObjectName objectName, DatabaseConnectionItem connection)
        {
            if (objectName.IsEmpty() ||
                (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View))
                return string.Empty;

            StringBuilder sb = new();

            var sql = $@"SELECT
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique,
    s.name AS SchemaName,
    o.name AS ObjectName,
    (
        SELECT STUFF((
            SELECT ',' + COL_NAME(ic2.object_id, ic2.column_id)
            FROM sys.index_columns ic2
            WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id
            ORDER BY ic2.key_ordinal
            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
    ) AS IndexColumns,
    i.filter_definition AS FilterDefinition,
    i.is_disabled AS IsDisabled,
    xi.using_xml_index_id,
    xi.secondary_type
FROM sys.indexes i
INNER JOIN sys.objects o ON i.object_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
LEFT JOIN sys.xml_indexes xi ON i.object_id = xi.object_id AND i.index_id = xi.index_id
WHERE s.name = N'{objectName.Schema}'
  AND o.name = N'{objectName.Name}'
  AND o.type IN ('U', 'V') -- U: Table, V: View
  AND i.is_primary_key = 0
  AND i.is_unique_constraint = 0
  AND i.type_desc <> 'HEAP'
  AND i.name IS NOT NULL
ORDER BY i.name";

            var dt = await SQLDatabaseHelper.GetDataTableAsync(sql, connection.ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            // Build a lookup for XML primary index names by index_id
            var xmlPrimaryIndexes = new Dictionary<int, string>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                string indexType = dr["IndexType"].ToString() ?? "";
                string? secondaryType = dr["secondary_type"] as string;
                if (indexType == "XML" && string.IsNullOrEmpty(secondaryType))
                {
                    int indexId = dt.Rows.IndexOf(dr) + 1;
                    xmlPrimaryIndexes[indexId] = dr["IndexName"].ToString() ?? "";
                }
            }

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                string indexName = dr["IndexName"].ToString() ?? "";
                string indexType = dr["IndexType"].ToString() ?? "";
                bool isUnique = dr["IsUnique"] != DBNull.Value && (bool)dr["IsUnique"];
                string schema = dr["SchemaName"].ToString() ?? "";
                string objName = dr["ObjectName"].ToString() ?? "";
                string columns = dr["IndexColumns"].ToString() ?? "";
                string? filter = dr["FilterDefinition"] as string;
                bool isDisabled = dr["IsDisabled"] != DBNull.Value && (bool)dr["IsDisabled"];
                object usingXmlIndexIdObj = dr["using_xml_index_id"];
                string? secondaryType = dr["secondary_type"] as string;

                if (indexType == "XML" && !string.IsNullOrEmpty(secondaryType))
                {
                    string xmlType = secondaryType.ToUpperInvariant();
                    string usingXmlIndexName = "";
                    if (usingXmlIndexIdObj != DBNull.Value)
                    {
                        int usingXmlIndexId = Convert.ToInt32(usingXmlIndexIdObj);
                        xmlPrimaryIndexes.TryGetValue(usingXmlIndexId, out usingXmlIndexName);
                    }
                    sb.Append($"CREATE XML INDEX [{indexName}] ON [{schema}].[{objName}] ([{columns.Trim()}]) ");
                    if (!string.IsNullOrEmpty(usingXmlIndexName))
                        sb.Append($"USING XML INDEX [{usingXmlIndexName}] ");
                    sb.Append($"FOR {xmlType};");
                    if (isDisabled)
                        sb.Append(" -- Index is disabled");
                    sb.AppendLine();
                }
                else if (indexType == "XML")
                {
                    sb.Append($"CREATE PRIMARY XML INDEX [{indexName}] ON [{schema}].[{objName}] ([{columns.Trim()}]);");
                    if (isDisabled)
                        sb.Append(" -- Index is disabled");
                    sb.AppendLine();
                }
                else
                {
                    sb.Append($"CREATE {(isUnique ? "UNIQUE " : "")}{indexType.Replace("_", " ")} INDEX [{indexName}] ON [{schema}].[{objName}] ({string.Join(", ", columns.Split(',').Select(c => $"[{c.Trim()}]"))})");
                    if (!string.IsNullOrWhiteSpace(filter))
                        sb.Append($" WHERE {filter}");
                    sb.Append(';');
                    if (isDisabled)
                        sb.Append(" -- Index is disabled");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the SQL script to add object description.
        /// </summary>
        /// <returns>A string.</returns>
        internal static string UspAddObjectDescription()
        {
            return $@"IF OBJECT_ID('dbo.usp_addupdateextendedproperty', 'P') IS NOT NULL
	DROP PROCEDURE dbo.usp_addupdateextendedproperty;
GO
/*
The usp_addupdateextendedproperty is an extension of the native sp_addextendedproperty
and sp_updateextendedproperty of SQL Server. Since sp_addextendedproperty can only be used to
add ,and sp_updateextendedproperty can only be used to update, the usp_addupdateextendedproperty
combines them to ensure that the description of an object can be added to or updated at any time.
*/
CREATE PROCEDURE dbo.usp_addupdateextendedproperty
    @name NVARCHAR(128),
    @value SQL_VARIANT,
    @level0type NVARCHAR(128) = NULL,
    @level0name NVARCHAR(128) = NULL,
    @level1type NVARCHAR(128) = NULL,
    @level1name NVARCHAR(128) = NULL,
    @level2type NVARCHAR(128) = NULL,
    @level2name NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM fn_listextendedproperty (
            @name,
            @level0type, @level0name,
            @level1type, @level1name,
            @level2type, @level2name
        )
    )
    BEGIN
		IF COALESCE(@value, '') = ''
			EXEC sys.sp_dropextendedproperty
				@name = @name,
                @level0type = @level0type, @level0name = @level0name,
                @level1type = @level1type, @level1name = @level1name,
                @level2type = @level2type, @level2name = @level2name;
		ELSE
            EXEC sys.sp_updateextendedproperty
                @name = @name,
                @value = @value,
                @level0type = @level0type, @level0name = @level0name,
                @level1type = @level1type, @level1name = @level1name,
                @level2type = @level2type, @level2name = @level2name;
    END
    ELSE
    BEGIN
        EXEC sys.sp_addextendedproperty
            @name = @name,
            @value = @value,
            @level0type = @level0type, @level0name = @level0name,
            @level1type = @level1type, @level1name = @level1name,
            @level2type = @level2type, @level2name = @level2name;
    END
END;
GO";
        }

        /// <summary>
        /// Gets the create function script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateFunctionScriptAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Function {objectName.FullName} ******/");

            // Use IF EXISTS with sys.objects and type IN ('FN','IF','TF')
            createScript.AppendLine(
                $"IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{objectName.FullName}') AND type IN ('FN','IF','TF'))");
            createScript.AppendLine($"\tDROP FUNCTION {objectName.FullName};");
            createScript.AppendLine($"GO");

            var script = await GetDefinitionAsync(objectName, connection);
            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }
            createScript.Append(script);
            createScript.AppendLine($"GO");
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create stored procedure script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateStoredProcedureScriptAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Stored Procedure {objectName.FullName} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                // Add drop table statement
                createScript.AppendLine($"IF OBJECT_ID(N'{objectName.FullName}', 'P') IS NOT NULL");
                createScript.AppendLine($"\tDROP PROCEDURE {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            var script = await GetDefinitionAsync(objectName, connection);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine($"GO");
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create table script async.
        /// </summary>
        /// <param name="dbObject">The db object.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTableScriptAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object: Table {objectName.FullName} ******/");

            // open the table object
            var table = new DBObject();
            if (!await table.OpenAsync(objectName, connection))
            {
                return string.Empty;
            }

            // Add drop table statement
            if (Properties.Settings.Default.AddDropStatement)
            {
                createScript.AppendLine($"IF OBJECT_ID(N'{objectName.FullName}', 'U') IS NOT NULL");
                createScript.AppendLine($"\tDROP TABLE {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            // Get the primary key column names that the Ord ends with "🗝"
            string primaryKeyColumns = table.PrimaryKeyColumns;

            // Retrieve identity column details
            var identityColumns = await table.GetIdentityColumns(connection);

            // Add the CREATE TABLE statement
            createScript.AppendLine($"CREATE TABLE {objectName.FullName} (");

            // Iterate through the rows in the DataGridView
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];

                // Safely retrieve column values
                string columnName = column.ColumnName;
                string dataType = column.DataType;
                string isNullable = column.Nullable ? "NULL" : "NOT NULL";

                // Check if the column is an identity column
                if (identityColumns.TryGetValue(columnName, out var identityInfo))
                {
                    createScript.Append($"\t[{columnName}] {dataType} IDENTITY({identityInfo.SeedValue}, {identityInfo.IncrementValue}) {isNullable}");
                }
                else
                {
                    createScript.Append($"\t[{columnName}] {dataType} {isNullable}");
                }

                // Add a comma if it's not the last valid row
                if (i < table.Columns.Count - 1)
                {
                    createScript.AppendLine(",");
                }
                else if (string.IsNullOrEmpty(primaryKeyColumns))
                {
                    createScript.AppendLine(); // No primary key, just end the line
                }
                else
                {
                    createScript.AppendLine(","); // Add a comma if primary key exists
                }
            }

            // Add the primary key constraint if it exists
            if (!string.IsNullOrEmpty(primaryKeyColumns))
            {
                createScript.AppendLine($"\tCONSTRAINT PK_{objectName.Schema}_{objectName.Name} PRIMARY KEY ({primaryKeyColumns})");
            }
            createScript.AppendLine(");");
            createScript.AppendLine("GO");

            // add the foreign key constraints
            var foreignKeyConstraints = await table.GetForeignKeyConstraintsScript();
            if (!string.IsNullOrEmpty(foreignKeyConstraints))
            {
                // remove the new line at the end of the script
                foreignKeyConstraints = foreignKeyConstraints.TrimEnd('\r', '\n');
                createScript.AppendLine(foreignKeyConstraints);
            }

            // add the check constraints
            var checkConstraints = await table.GetCheckConstraintsScript();
            if (!string.IsNullOrEmpty(checkConstraints))
            {
                // remove the new line at the end of the script
                checkConstraints = checkConstraints.TrimEnd('\r', '\n');
                createScript.AppendLine(checkConstraints);
            }

            // add the default constraints
            var defaultConstraints = await table.GetDefaultConstraintsScript();
            if (!string.IsNullOrEmpty(defaultConstraints))
            {
                // remove the new line at the end of the script
                defaultConstraints = defaultConstraints.TrimEnd('\r', '\n');
                createScript.AppendLine(defaultConstraints);
            }

            var indexScript = await GetCreateIndexesScript(objectName, connection);
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createScript.AppendLine(indexScript);
            }

            var triggerScript = await GetCreateTriggersScriptAsync(objectName, connection);
            if (!string.IsNullOrEmpty(triggerScript))
            {
                // if the createScript does not ended with a line of "GO", add it
                AddGOStatement(createScript);
                //if (!createScript.ToString().EndsWith(Environment.NewLine + "GO" + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
                //{
                //    createScript.AppendLine($"GO");
                //}

                // remove the new line at the end of the script
                triggerScript = triggerScript.TrimEnd('\r', '\n');
                createScript.AppendLine(triggerScript);
            }

            // add GO statement
            AddGOStatement(createScript);
            //if (!createScript.ToString().EndsWith(Environment.NewLine + "GO" + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
            //{
            //    createScript.AppendLine($"GO");
            //}

            return createScript.ToString();
        }

        /// <summary>
        /// Adds GO statement to the script if the script does not already end with it.
        /// </summary>
        /// <param name="sb">The sb.</param>
        private static void AddGOStatement(StringBuilder sb)
        {
            // Add a "GO" statement to separate batches in the script
            if (!sb.ToString().EndsWith(Environment.NewLine + "GO" + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine($"GO");
            }
        }

        /// <summary>
        /// Gets the create trigger script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTriggerScriptAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Trigger {objectName.Name} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                // Add drop table statement
                createScript.AppendLine($"IF OBJECT_ID(N'{objectName.FullName}', 'TR') IS NOT NULL");
                createScript.AppendLine($"\tDROP TRIGGER {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            var script = await SQLDatabaseHelper.GetObjectDefinitionAsync(objectName, connection.ConnectionString);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine($"GO");
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create triggers script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTriggersScriptAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            // Only tables and views can have DML triggers
            if (objectName.IsEmpty() ||
                (objectName.ObjectType != ObjectName.ObjectTypeEnums.Table &&
                 objectName.ObjectType != ObjectName.ObjectTypeEnums.View))
                return string.Empty;

            StringBuilder sb = new();

            // Query to get all triggers for the specified table/view
            string sql = $@"
SELECT
    tr.name AS TriggerName,
    s.name AS SchemaName,
    OBJECT_NAME(tr.parent_id) AS ParentObjectName,
    sm.definition AS TriggerDefinition,
    tr.is_disabled
FROM sys.triggers tr
INNER JOIN sys.objects o ON tr.parent_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
INNER JOIN sys.sql_modules sm ON tr.object_id = sm.object_id
WHERE s.name = N'{objectName.Schema}'
  AND o.name = N'{objectName.Name}'
ORDER BY tr.name";

            var dt = await SQLDatabaseHelper.GetDataTableAsync(sql, connection.ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            foreach (DataRow dr in dt.Rows)
            {
                string triggerName = dr["TriggerName"].ToString() ?? "";
                string schema = dr["SchemaName"].ToString() ?? "";
                string parentName = dr["ParentObjectName"].ToString() ?? "";
                string definition = dr["TriggerDefinition"].ToString() ?? "";
                bool isDisabled = dr["is_disabled"] != DBNull.Value && (bool)dr["is_disabled"];

                // Add header
                sb.AppendLine($"/****** Object:  Trigger [{schema}].[{triggerName}] on [{schema}].[{parentName}] ******/");

                // Add drop statement if configured
                if (Properties.Settings.Default.AddDropStatement)
                {
                    sb.AppendLine($"IF OBJECT_ID(N'{schema}.{triggerName}', 'TR') IS NOT NULL");
                    sb.AppendLine($"\tDROP TRIGGER [{schema}].[{triggerName}];");
                    sb.AppendLine("GO");
                }

                // Add the trigger definition
                sb.Append(definition.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine);
                sb.AppendLine("GO");

                // Optionally, add DISABLE TRIGGER if the trigger is disabled
                if (isDisabled)
                {
                    sb.AppendLine($"ALTER TABLE [{schema}].[{parentName}] DISABLE TRIGGER [{triggerName}];");
                    sb.AppendLine("GO");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the create view script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateViewScriptAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  View {objectName.FullName} ******/");

            // Add drop table statement
            if (Properties.Settings.Default.AddDropStatement)
            {
                createScript.AppendLine($"IF OBJECT_ID(N'{objectName.FullName}', 'V') IS NOT NULL");
                createScript.AppendLine($"\tDROP VIEW {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            var script = await GetDefinitionAsync(objectName, connection);
            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }
            createScript.Append(script);

            var indexScript = await DatabaseDocBuilder.GetCreateIndexesScript(objectName, connection);
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createScript.AppendLine(indexScript);
            }

            createScript.AppendLine($"GO");
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the stored procedure definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetDefinitionAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            // Validate the single full name parameter
            if (!string.IsNullOrWhiteSpace(objectName.FullName))
            {
                string? definition = await SQLDatabaseHelper.GetObjectDefinitionAsync(objectName, connection?.ConnectionString);

                if (string.IsNullOrEmpty(definition))
                {
                    // If no definition was found, return an empty string or a comment indicating no definition
                    //return $"-- No definition found for stored procedure {spName}";
                    return string.Empty; // Return empty if no definition found
                }
                else
                {
                    // Ensure the definition ends with a newline for better readability
                    return definition.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine;
                }
            }
            else
            {
                // If the full name is empty or null, return an empty string
                return string.Empty;
            }
        }
    }
}