using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static async Task<string?> GetCreateObjectScriptAsync(ObjectName dbObject)
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
                ObjectName.ObjectTypeEnums.Table => await GetCreateTableScriptAsync(dbObject),
                ObjectName.ObjectTypeEnums.View => await GetCreateViewScriptAsync(dbObject),
                ObjectName.ObjectTypeEnums.StoredProcedure => await GetCreateStoredProcedureScriptAsync(dbObject),
                ObjectName.ObjectTypeEnums.Function => await GetCreateFunctionScriptAsync(dbObject),
                _ => throw new NotSupportedException($"The object type '{dbObject.ObjectType}' is not supported.")
            };
        }

        /// <summary>
        /// Retrieves the T-SQL definition script for a specific view using its fully qualified name.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <param name="fullViewName">The fully qualified name of the view (e.g., "[dbo].[vGetAllCategories]", "sales.CustomersView"). Should be in a format recognizable by OBJECT_ID.</param>
        /// <returns>A string containing the view's definition, or null if not found or an error occurs.</returns>
        /// <exception cref="ArgumentNullException">Thrown if connectionString or fullViewName is null or empty.</exception>
        /// <remarks>
        /// This function executes SQL targeting sys.sql_modules for objects identifiable via OBJECT_ID.
        /// It assumes the object corresponding to fullViewName is a view or other SQL module.
        /// It does NOT generate CREATE INDEX scripts.
        /// </remarks>
        public static async Task<string?> GetViewDefinitionAsync(ObjectName objectName)
        {
            string fullViewName = objectName.FullName;

            // Validate the single full name parameter
            if (string.IsNullOrWhiteSpace(fullViewName))
                throw new ArgumentNullException(nameof(fullViewName));

            // The SQL query remains the same, using OBJECT_ID which handles schema-qualified names
            const string query = @"SELECT sm.definition
FROM sys.sql_modules sm
WHERE sm.object_id = OBJECT_ID(@SchemaQualifiedName);";
            // Optional: Add explicit type check if necessary
            // AND EXISTS (SELECT 1 FROM sys.objects o WHERE o.object_id = sm.object_id AND o.type = 'V');";

            string? viewDefinition = null;

            try
            {
                // Use 'await using' for automatic disposal
                await using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                await using var command = new SqlCommand(query, connection);
                // Use the provided fullViewName directly as the parameter value
                command.Parameters.AddWithValue("@SchemaQualifiedName", fullViewName);

                await connection.OpenAsync();

                // ExecuteScalarAsync is efficient for retrieving a single value
                object? result = await command.ExecuteScalarAsync();

                // Check if a result was returned and it's not DBNull
                if (result != null && result != DBNull.Value)
                {
                    viewDefinition = result.ToString();
                }
                // Command is disposed here
                // Connection is disposed here
            }
            catch (SqlException ex)
            {
                // Log the exception (replace Console.WriteLine with your logging framework)
                Console.WriteLine($"SQL Error getting view definition for {fullViewName}: {ex.Message}");
                // Depending on requirements, you might re-throw, return null, or handle differently
                // throw; // Uncomment to propagate the exception
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                Console.WriteLine($"Error getting view definition for {fullViewName}: {ex.Message}");
                // throw; // Uncomment to propagate the exception
            }

            return viewDefinition;
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
        public static async Task<string> QueryDataToInsertStatementAsync(string sql, string tableName = "YourTableName")
        {
            var sb = new StringBuilder();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);

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
                    columnNames = columnNames.Substring(0, columnNames.Length - 2); // Remove trailing comma and space

                    int rowCount = 0;
                    int batchCount = 0;

                    // add SET IDENTITY_INSERT  ON;
                    sb.AppendLine($"--SET IDENTITY_INSERT {tableName} ON;");

                    while (await reader.ReadAsync())
                    {
                        // Check if row count exceeds 5000
                        if (rowCount >= 5000)
                        {
                            return "Too much rows";
                        }

                        // Start a new batch every 50 rows
                        if (rowCount % 50 == 0)
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
                                switch (reader.GetFieldType(i).Name)
                                {
                                    case "String":
                                        sb.Append("'" + reader.GetString(i).Replace("'", "''") + "'");
                                        break;

                                    case "DateTime":
                                        sb.Append("'" + reader.GetDateTime(i).ToString("yyyy-MM-dd HH:mm:ss") + "'");
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

                                    default:
                                        sb.Append("'" + reader.GetValue(i).ToString().Replace("'", "''") + "'");
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

                    sb.AppendLine($"--SET IDENTITY_INSERT {tableName} OFF;");
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
        private static async Task<string?> GetCreateFunctionScriptAsync(ObjectName objectName)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Function {objectName.FullName} ******/");

            // Add drop table statement
            createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'V') IS NOT NULL");
            createScript.AppendLine($"\tDROP VIEW {objectName.FullName};");
            createScript.AppendLine($"GO");

            var script = await GetFunctionDefinitionAsync(objectName);
            createScript.Append(script);
            createScript.AppendLine($"GO");
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create stored procedure script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateStoredProcedureScriptAsync(ObjectName objectName)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Stored Procedure {objectName.FullName} ******/");

            // Add drop table statement
            createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'V') IS NOT NULL");
            createScript.AppendLine($"\tDROP VIEW {objectName.FullName};");
            createScript.AppendLine($"GO");

            var script = await GetStoredProcedureDefinitionAsync(objectName);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            // remove the space at the end of the script
            script = script.TrimEnd('\r', '\n', ' ', '\t');

            // add new line if the script is not end with new line
            if (!script.EndsWith(Environment.NewLine))
            {
                script += Environment.NewLine;
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
        private static async Task<string?> GetCreateTableScriptAsync(ObjectName objectName)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object: Table {objectName.FullName} ******/");

            // open the table object
            var table = new DBObject();
            if (!await table.OpenAsync(objectName, Properties.Settings.Default.dbConnectionString))
            {
                return string.Empty;
            }

            // Add drop table statement
            createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'U') IS NOT NULL");
            createScript.AppendLine($"\tDROP TABLE {objectName.FullName};");
            createScript.AppendLine($"GO");

            // Get the primary key column names that the ColID ends with "🗝"
            string primaryKeyColumns = table.PrimaryKeyColumns;

            // Retrieve identity column details
            var identityColumns = table.GetIdentityColumns();

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
            createScript.AppendLine($"GO");

            var indexScript = table.GetCreateIndexesScript();
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createScript.AppendLine(indexScript);
                createScript.AppendLine($"GO");
            }

            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create view script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateViewScriptAsync(ObjectName objectName)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  View {objectName.FullName} ******/");

            // Add drop table statement
            createScript.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'V') IS NOT NULL");
            createScript.AppendLine($"\tDROP VIEW {objectName.FullName};");
            createScript.AppendLine($"GO");

            var script = await GetViewDefinitionAsync(objectName);
            createScript.Append(script);
            createScript.AppendLine($"GO");
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the function definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetFunctionDefinitionAsync(ObjectName objectName)
        {
            string fnName = objectName.FullName;

            // Validate the single full name parameter
            if (string.IsNullOrWhiteSpace(fnName))
                throw new ArgumentNullException(nameof(fnName));

            // The SQL query remains the same, using OBJECT_ID which handles schema-qualified names
            const string query = @"SELECT sm.definition
FROM sys.sql_modules sm
WHERE sm.object_id = OBJECT_ID(@SchemaQualifiedName);";
            // Optional: Add explicit type check if necessary
            // AND EXISTS (SELECT 1 FROM sys.objects o WHERE o.object_id = sm.object_id AND o.type = 'V');";

            string? definition = null;

            try
            {
                // Use 'await using' for automatic disposal
                await using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                await using var command = new SqlCommand(query, connection);
                // Use the provided fnName directly as the parameter value
                command.Parameters.AddWithValue("@SchemaQualifiedName", fnName);

                await connection.OpenAsync();

                // ExecuteScalarAsync is efficient for retrieving a single value
                object? result = await command.ExecuteScalarAsync();

                // Check if a result was returned and it's not DBNull
                if (result != null && result != DBNull.Value)
                {
                    definition = result.ToString();
                }
                // Command is disposed here
                // Connection is disposed here
            }
            catch (SqlException ex)
            {
                // Log the exception (replace Console.WriteLine with your logging framework)
                Console.WriteLine($"SQL Error getting view definition for {fnName}: {ex.Message}");
                // Depending on requirements, you might re-throw, return null, or handle differently
                // throw; // Uncomment to propagate the exception
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                Console.WriteLine($"Error getting view definition for {fnName}: {ex.Message}");
                // throw; // Uncomment to propagate the exception
            }

            return definition;
        }

        /// <summary>
        /// Gets the stored procedure definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetStoredProcedureDefinitionAsync(ObjectName objectName)
        {
            string spName = objectName.FullName;

            // Validate the single full name parameter
            if (string.IsNullOrWhiteSpace(spName))
                throw new ArgumentNullException(nameof(spName));

            // The SQL query remains the same, using OBJECT_ID which handles schema-qualified names
            const string query = @"SELECT sm.definition
FROM sys.sql_modules sm
WHERE sm.object_id = OBJECT_ID(@SchemaQualifiedName);";
            // Optional: Add explicit type check if necessary
            // AND EXISTS (SELECT 1 FROM sys.objects o WHERE o.object_id = sm.object_id AND o.type = 'V');";

            string? definition = null;

            try
            {
                // Use 'await using' for automatic disposal
                await using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                await using var command = new SqlCommand(query, connection);
                // Use the provided spName directly as the parameter value
                command.Parameters.AddWithValue("@SchemaQualifiedName", spName);

                await connection.OpenAsync();

                // ExecuteScalarAsync is efficient for retrieving a single value
                object? result = await command.ExecuteScalarAsync();

                // Check if a result was returned and it's not DBNull
                if (result != null && result != DBNull.Value)
                {
                    definition = result.ToString();
                }
                // Command is disposed here
                // Connection is disposed here
            }
            catch (SqlException ex)
            {
                // Log the exception (replace Console.WriteLine with your logging framework)
                Console.WriteLine($"SQL Error getting view definition for {spName}: {ex.Message}");
                // Depending on requirements, you might re-throw, return null, or handle differently
                // throw; // Uncomment to propagate the exception
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                Console.WriteLine($"Error getting view definition for {spName}: {ex.Message}");
                // throw; // Uncomment to propagate the exception
            }

            return definition;
        }
    }
}