using Microsoft.Data.SqlClient;
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
        public static async Task<string?> GetCreateObjectScriptAsync(ObjectName dbObject, string connectionString)
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
                ObjectName.ObjectTypeEnums.Table => await GetCreateTableScriptAsync(dbObject, connectionString),
                ObjectName.ObjectTypeEnums.View => await GetCreateViewScriptAsync(dbObject, connectionString),
                ObjectName.ObjectTypeEnums.StoredProcedure => await GetCreateStoredProcedureScriptAsync(dbObject, connectionString),
                ObjectName.ObjectTypeEnums.Function => await GetCreateFunctionScriptAsync(dbObject, connectionString),
                _ => throw new NotSupportedException($"The object type '{dbObject.ObjectType}' is not supported.")
            };
        }

        /// <summary>
        /// Retrieves the T-SQL definition script for a specific view using its fully qualified name.
        /// </summary>
        /// <returns>A string containing the view's definition, or null if not found or an error occurs.</returns>
        /// <remarks>
        /// This function executes SQL targeting sys.sql_modules for objects identifiable via OBJECT_ID.
        /// It assumes the object corresponding to fullViewName is a view or other SQL module.
        /// It does NOT generate CREATE INDEX scripts.
        /// </remarks>
        public static async Task<string> GetViewDefinitionAsync(ObjectName objectName, string connectionString)
        {
            string fullViewName = objectName.FullName;

            string viewDefinition = string.Empty;

            // Validate the single full name parameter
            if (!string.IsNullOrWhiteSpace(fullViewName))
            {
                try
                {
                    // The SQL query remains the same, using OBJECT_ID which handles schema-qualified names
                    string query = $@"SELECT sm.definition
FROM sys.sql_modules sm
WHERE sm.object_id = OBJECT_ID('{objectName.FullName}')";

                    // ExecuteScalarAsync is efficient for retrieving a single value
                    object? result = await DatabaseHelper.ExecuteScalarAsync(query, connectionString);
                    // Check if a result was returned and it's not DBNull
                    if (result != null && result != DBNull.Value)
                    {
                        viewDefinition = result.ToString().TrimStart('\r', '\n', ' ', '\t');
                    }
                }
                catch (SqlException)
                {
                    // Log the exception (replace Console.WriteLine with your logging framework)
                    //viewDefinition = $"-- SQL Error getting view definition for {fullViewName}: {ex.Message}";
                }
                catch (Exception)
                {
                    // Handle other potential exceptions
                    //viewDefinition = $"-- Error getting view definition for {fullViewName}: {ex.Message}";
                }
            }

            if (string.IsNullOrEmpty(viewDefinition))
            {
                // If no definition was found, return an empty string or a comment indicating no definition
                //return $"-- No definition found for view {fullViewName}";
                return string.Empty; // Return empty if no definition found
            }
            else
            {
                // Ensure the definition ends with a newline for better readability
                return viewDefinition.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine;
            }
        }

        /// <summary>
        /// Queries the data and generates the insert statements.
        /// </summary>
        /// <param name="sql">The SQL query to fetch data.</param>
        /// <param name="tableName">The table name for the insert statements.</param>
        /// <returns>A string containing the generated insert statements or a warning if rows exceed 500.</returns>
        /// <summary>
        /// Queries the data and generates the insert statements asynchronously.
        /// </summary>
        /// <param name="sql">The SQL query to fetch data.</param>
        /// <param name="tableName">The table name for the insert statements.</param>
        /// <returns>A Task<string> containing the generated insert statements or a warning if rows exceed 500.</returns>
        public static async Task<string> QueryDataToInsertStatementAsync(string sql, string connectionString, string tableName = "YourTableName", bool hasIdentity = false)
        {
            var sb = new StringBuilder();
            var conn = new SqlConnection(connectionString);

            try
            {
                await using var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                await conn.OpenAsync();

                // Execute the query and read the data asynchronously
                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    // Get the column names
                    var columnNames = string.Empty;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames += $"[{reader.GetName(i)}], ";
                    }
                    columnNames = columnNames[..^2]; // Remove trailing comma and space

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

                    while (await reader.ReadAsync())
                    {
                        // Start a new batch every 50 rows
                        if (rowCount % batchSize == 0)
                        {
                            if (batchCount > 0)
                            {
                                // Remove the trailing comma from the previous batch
                                sb.Length -= 3; // Remove ",\n"
                                sb.AppendLine(";"); // Close the previous batch
                            }
                            sb.AppendLine($"INSERT INTO {tableName} ({columnNames}) VALUES");
                            batchCount++;
                        }

                        // Generate the values for the current row
                        sb.Append("\t(");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.IsDBNull(i))
                            {
                                sb.Append("NULL");
                            }
                            else
                            {
                                var typeName = reader.GetFieldType(i).Name;
                                //System.Diagnostics.Debug.Print(typeName);

                                switch (typeName)
                                {
                                    case "String":
                                        sb.Append("N'" + reader.GetString(i).Replace("'", "''") + "'");
                                        break;

                                    case "DateTime":
                                        sb.Append("'" + reader.GetDateTime(i).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                        break;

                                    case "Int16":
                                        sb.Append(reader.GetInt16(i));
                                        break;

                                    case "Int32":
                                        sb.Append(reader.GetInt32(i));
                                        break;

                                    case "Int64":
                                        sb.Append(reader.GetInt64(i));
                                        break;

                                    case "Decimal":
                                        sb.Append(reader.GetDecimal(i));
                                        break;

                                    case "Double":
                                        sb.Append(reader.GetDouble(i));
                                        break;

                                    case "Single":
                                        sb.Append(reader.GetFloat(i));
                                        break;

                                    case "Boolean":
                                        sb.Append(reader.GetBoolean(i) ? "1" : "0");
                                        break;

                                    case "Byte":
                                        sb.Append(reader.GetByte(i));
                                        break;

                                    default:
                                        sb.Append("N'" + reader.GetValue(i).ToString().Replace("'", "''") + "'");
                                        break;
                                }
                            }
                            if (i < reader.FieldCount - 1)
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
                        sb.AppendLine(";"); // Close the last batch
                    }

                    if (hasIdentity)
                    {
                        sb.AppendLine($"SET IDENTITY_INSERT {tableName} OFF;");
                    }
                }
                else
                {
                    sb.AppendLine($"-- No data found for the query: {sql}");
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                await conn.CloseAsync();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Queries the data to insert statement async.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <returns>A Task.</returns>
        public static async Task<string> TableToInsertStatementAsync(ObjectName tableName, string connectionString)
        {
            var sql = $"select * from {tableName.FullName}";
            var hasIdentity = await DatabaseHelper.HasIdentityColumnAsync(tableName, connectionString);
            return await QueryDataToInsertStatementAsync(sql, connectionString, tableName.FullName, hasIdentity);
        }

        /// <summary>
        /// Generates the SQL script to recreate all non-primary key, non-unique constraint indexes for the specified table.
        /// Handles XML indexes with the correct syntax.
        /// </summary>
        /// <returns>A string containing the SQL script to recreate the indexes.</returns>
        internal static async Task<string?> GetCreateIndexesScript(ObjectName objectName, string connectionString)
        {
            if (objectName.IsEmpty() ||
                (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View))
                return string.Empty;

            StringBuilder sb = new();

            string sql = $@"
SELECT
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique,
    s.name AS SchemaName,
    o.name AS ObjectName,
    STRING_AGG(COL_NAME(ic.object_id, ic.column_id), ',') AS IndexColumns,
    i.filter_definition AS FilterDefinition,
    i.is_disabled AS IsDisabled,
    xi.using_xml_index_id,
    xi.secondary_type
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.objects o ON i.object_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
LEFT JOIN sys.xml_indexes xi ON i.object_id = xi.object_id AND i.index_id = xi.index_id
WHERE s.name = '{objectName.Schema}'
  AND o.name = '{objectName.Name}'
  AND o.type IN ('U', 'V') -- U: Table, V: View
  AND i.is_primary_key = 0
  AND i.is_unique_constraint = 0
  AND i.type_desc <> 'HEAP'
  AND i.name IS NOT NULL
GROUP BY i.name, i.type_desc, i.is_unique, s.name, o.name, i.filter_definition, i.is_disabled, xi.using_xml_index_id, xi.secondary_type
ORDER BY i.name";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
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
            return $@"IF OBJECT_ID('dbo.usp_AddObjectDescription', 'P') IS NOT NULL
	DROP PROCEDURE dbo.usp_AddObjectDescription;
GO
CREATE PROCEDURE usp_AddObjectDescription
(
	@TableName sysname,
	@Description nvarchar(1024)
)
AS
BEGIN
    SET NOCOUNT ON

	IF OBJECT_ID(@TableName) IS NOT NULL AND LEN(COALESCE(@Description, '')) > 0
	BEGIN
		DECLARE @Schema varchar(100) = OBJECT_SCHEMA_NAME(OBJECT_ID(@TableName));
		DECLARE @ObjectName varchar(100) = OBJECT_NAME(OBJECT_ID(@TableName));
		DECLARE @ObjectType varchar(100);
		SELECT @ObjectType = CASE type_desc WHEN 'USER_TABLE' THEN 'TABLE' ELSE 'VIEW' END
		  FROM sys.objects
		 WHERE object_id = OBJECT_ID(@TableName);

		IF EXISTS (	SELECT value
					FROM sys.extended_properties
					WHERE class = 1 AND major_id = OBJECT_ID(@TableName)
						AND minor_id = 0
						AND name = 'MS_Description')
			EXEC sp_updateextendedproperty @name = N'MS_Description', @value = @Description,
				@level0type = N'SCHEMA', @level0name = @Schema,
				@level1type = @ObjectType, @level1name = @ObjectName;
		ELSE
			EXEC sp_addextendedproperty @name = N'MS_Description', @value = @Description,
				@level0type = N'SCHEMA', @level0name = @Schema,
				@level1type = @ObjectType, @level1name = @ObjectName;

	END
END
GO
IF OBJECT_ID('dbo.usp_AddColumnDescription', 'P') IS NOT NULL
	DROP PROCEDURE dbo.usp_AddColumnDescription;
GO
CREATE PROCEDURE usp_AddColumnDescription
(
	@TableName sysname,
	@ColumnName varchar(200),
	@Description nvarchar(1024)
)
AS
BEGIN
    SET NOCOUNT ON

	IF OBJECT_ID(@TableName) IS NOT NULL AND LEN(COALESCE(@Description, '')) > 0
		IF EXISTS (SELECT name
					 FROM sys.columns
					WHERE object_id = OBJECT_ID(@TableName)
					  AND name = @ColumnName)
		BEGIN

			DECLARE @Schema varchar(100) = OBJECT_SCHEMA_NAME(OBJECT_ID(@TableName));
			DECLARE @ObjectName varchar(100) = OBJECT_NAME(OBJECT_ID(@TableName));
			DECLARE @ObjectType varchar(100);
			SELECT @ObjectType = CASE type_desc WHEN 'USER_TABLE' THEN 'TABLE' ELSE 'VIEW' END
			  FROM sys.objects
			 WHERE object_id = OBJECT_ID(@TableName);

			IF EXISTS (	SELECT value
						FROM sys.extended_properties
						WHERE class = 1 AND major_id = OBJECT_ID(@TableName)
							AND minor_id = (SELECT column_id FROM sys.columns WHERE name = @ColumnName AND object_id = OBJECT_ID(@TableName))
							AND name = 'MS_Description')
				EXEC sp_updateextendedproperty @name = N'MS_Description', @value = @Description,
					@level0type = N'SCHEMA', @level0name = @Schema,
					@level1type = @ObjectType, @level1name = @ObjectName,
					@level2type = N'COLUMN', @level2name = @ColumnName
			ELSE
				EXEC sp_addextendedproperty @name = N'MS_Description', @value = @Description,
					@level0type = N'SCHEMA', @level0name = @Schema,
					@level1type = @ObjectType, @level1name = @ObjectName,
					@level2type = N'COLUMN', @level2name = @ColumnName

		END
END
GO";
        }

        /// <summary>
        /// Gets the create function script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateFunctionScriptAsync(ObjectName objectName, string connectionString)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Function {objectName.FullName} ******/");

            // Use IF EXISTS with sys.objects and type IN ('FN','IF','TF')
            createScript.AppendLine(
                $"IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('{objectName.FullName}') AND type IN ('FN','IF','TF'))");
            createScript.AppendLine($"\tDROP FUNCTION {objectName.FullName};");
            createScript.AppendLine($"GO");

            var script = await GetFunctionDefinitionAsync(objectName, connectionString);
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
        private static async Task<string?> GetCreateStoredProcedureScriptAsync(ObjectName objectName, string connectionString)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Stored Procedure {objectName.FullName} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                // Add drop table statement
                createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'P') IS NOT NULL");
                createScript.AppendLine($"\tDROP PROCEDURE {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            var script = await GetStoredProcedureDefinitionAsync(objectName, connectionString);

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
        private static async Task<string?> GetCreateTableScriptAsync(ObjectName objectName, string connectionString)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object: Table {objectName.FullName} ******/");

            // open the table object
            var table = new DBObject();
            if (!await table.OpenAsync(objectName, connectionString))
            {
                return string.Empty;
            }

            // Add drop table statement
            if (Properties.Settings.Default.AddDropStatement)
            {
                createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'U') IS NOT NULL");
                createScript.AppendLine($"\tDROP TABLE {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            // Get the primary key column names that the ColID ends with "🗝"
            string primaryKeyColumns = table.PrimaryKeyColumns;

            // Retrieve identity column details
            var identityColumns = table.GetIdentityColumns(connectionString);

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

            var indexScript = await GetCreateIndexesScript(objectName, connectionString);
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createScript.AppendLine(indexScript);
            }

            var triggerScript = await GetCreateTriggersScriptAsync(objectName, connectionString);
            if (!string.IsNullOrEmpty(triggerScript))
            {
                // if the createScript does not ended with a line of "GO", add it
                if (!createScript.ToString().EndsWith(Environment.NewLine + "GO" + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
                {
                    createScript.AppendLine($"GO");
                }

                // remove the new line at the end of the script
                triggerScript = triggerScript.TrimEnd('\r', '\n');
                createScript.AppendLine(triggerScript);
            }

            // add GO statement
            if (!createScript.ToString().EndsWith(Environment.NewLine + "GO" + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
            {
                createScript.AppendLine($"GO");
            }

            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create triggers script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTriggersScriptAsync(ObjectName objectName, string connectionString)
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
WHERE s.name = '{objectName.Schema}'
  AND o.name = '{objectName.Name}'
ORDER BY tr.name;";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
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
                    sb.AppendLine($"IF OBJECT_ID('{schema}.{triggerName}', 'TR') IS NOT NULL");
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
        private static async Task<string?> GetCreateViewScriptAsync(ObjectName objectName, string connectionString)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  View {objectName.FullName} ******/");

            // Add drop table statement
            if (Properties.Settings.Default.AddDropStatement)
            {
                createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'V') IS NOT NULL");
                createScript.AppendLine($"\tDROP VIEW {objectName.FullName};");
                createScript.AppendLine($"GO");
            }

            var script = await GetViewDefinitionAsync(objectName, connectionString);
            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }
            createScript.Append(script);

            var indexScript = await DatabaseDocBuilder.GetCreateIndexesScript(objectName, connectionString);
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
        /// Gets the function definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string> GetFunctionDefinitionAsync(ObjectName objectName, string connectionString)
        {
            string fnName = objectName.FullName;

            string definition = string.Empty;

            // Validate the single full name parameter
            if (!string.IsNullOrWhiteSpace(fnName))
            {
                try
                {
                    // The SQL query remains the same, using OBJECT_ID which handles schema-qualified names
                    string query = $@"SELECT sm.definition
FROM sys.sql_modules sm
WHERE sm.object_id = OBJECT_ID('{objectName.FullName}');";

                    // ExecuteScalarAsync is efficient for retrieving a single value
                    object? result = await DatabaseHelper.ExecuteScalarAsync(query, connectionString);

                    // Check if a result was returned and it's not DBNull
                    if (result != null && result != DBNull.Value)
                    {
                        definition = result.ToString().TrimStart('\r', '\n', ' ', '\t');
                    }
                    // Command is disposed here
                    // Connection is disposed here
                }
                catch (SqlException)
                {
                    // Log the exception (replace Console.WriteLine with your logging framework)
                    //definition = $"-- SQL Error getting view definition for {fnName}: {ex.Message}";
                }
                catch (Exception)
                {
                    // Handle other potential exceptions
                    //definition = $"-- Error getting view definition for {fnName}: {ex.Message}";
                }
            }

            if (string.IsNullOrEmpty(definition))
            {
                // If no definition was found, return an empty string or a comment indicating no definition
                return string.Empty;
            }
            else
            {
                // Ensure the definition ends with a newline for better readability
                return definition.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine;
            }
        }

        /// <summary>
        /// Gets the stored procedure definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetStoredProcedureDefinitionAsync(ObjectName objectName, string connectionString)
        {
            string spName = objectName.FullName;
            string? definition = string.Empty;

            // Validate the single full name parameter
            if (!string.IsNullOrWhiteSpace(spName))
            {
                // The SQL query remains the same, using OBJECT_ID which handles schema-qualified names
                string query = $@"SELECT sm.definition
FROM sys.sql_modules sm
WHERE sm.object_id = OBJECT_ID('{objectName.FullName}')";

                try
                {
                    // ExecuteScalarAsync is efficient for retrieving a single value
                    object? result = await DatabaseHelper.ExecuteScalarAsync(query, connectionString);

                    // Check if a result was returned and it's not DBNull
                    if (result != null && result != DBNull.Value)
                    {
                        definition = result.ToString().TrimStart('\r', '\n', ' ', '\t');
                    }
                }
                catch (SqlException)
                {
                    // Log the exception (replace Console.WriteLine with your logging framework)
                    //definition = $"-- SQL Error getting view definition for {spName}: {ex.Message}";
                }
                catch (Exception)
                {
                    // Handle other potential exceptions
                    //definition = $"-- Error getting view definition for {spName}: {ex.Message}";
                }
            }

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
    }
}