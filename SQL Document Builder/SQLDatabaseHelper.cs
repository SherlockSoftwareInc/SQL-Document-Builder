using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The database helper.
    /// </summary>
    internal class SQLDatabaseHelper
    {
        #region SQL Execution & Validation

        /// <summary>
        /// Executes the scalar async.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>A Task.</returns>
        internal static async Task<object?> ExecuteScalarAsync(string sql, string connectionString)
        {
            object? value;

            if (string.IsNullOrEmpty(connectionString))
            {
                return DBNull.Value;
            }

            using var connection = new SqlConnection(connectionString);
            try
            {
                await using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                await connection.OpenAsync(); // Ensure the connection is opened before executing commands
                value = await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = null;
            }
            finally
            {
                await connection.CloseAsync();
            }
            return value;
        }

        /// <summary>
        /// Executes the SQL statement asynchronously.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="connectionString">The connection string. If not provided, the default connection string is used.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        internal static async Task<string> ExecuteSQLAsync(string sql, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return "No database connection specified.";

            using var conn = new SqlConnection(connectionString);
            try
            {
                await using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                await conn.CloseAsync();
            }

            return string.Empty; // Return an empty string if no error occurred
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>A DataTable? .</returns>
        internal static async Task<DataTable?> GetDataTableAsync(string sql, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(sql))
            {
                return null;
            }

            var tables = new DataTable();
            using var connection = new SqlConnection(connectionString);
            try
            {
                await using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync(); // Ensure the connection is opened before executing commands
                await using var reader = await command.ExecuteReaderAsync();

                tables.Load(reader);
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
            return tables;
        }

        /// <summary>
        /// Are the valid select statement.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<bool> IsValidSelectStatement(string sql, string connectionString)
        {
            bool isValid = true;

            // List of unsupported SQL Server types
            var unsupportedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "timestamp",
        "hierarchyid",
        "geometry",
        "geography",
        "sql_variant",
        "xml"
    };

            using var connection = new SqlConnection(connectionString);
            try
            {
                await using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
                var schemaTable = reader.GetSchemaTable();
                if (schemaTable != null)
                {
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        var dataTypeNameFull = row["DataTypeName"]?.ToString();
                        var dataTypeName = dataTypeNameFull?.Split('.') is { Length: > 0 } parts
                            ? parts[^1]
                            : dataTypeNameFull;

                        if (!string.IsNullOrEmpty(dataTypeName) && unsupportedTypes.Contains(dataTypeName))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // show error message
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            finally
            {
                await connection.CloseAsync();
            }
            return isValid;
        }

        /// <summary>
        /// Syntaxes the check async.
        /// </summary>
        /// <param name="userQuery">The user query.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> SyntaxCheckAsync(string userQuery, string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return "No database connection specified.";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(userQuery, connection);

            var resultBuilder = new StringBuilder();

            try
            {
                await connection.OpenAsync();

                // Turn NOEXEC ON to check syntax only
                await using (var setNoExecOn = new SqlCommand("SET NOEXEC ON", connection))
                {
                    await setNoExecOn.ExecuteNonQueryAsync();
                }

                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        resultBuilder.AppendLine(ex.Errors[i].Message);
                    }
                }
                finally
                {
                    // Turn NOEXEC OFF
                    await using var setNoExecOff = new SqlCommand("SET NOEXEC OFF", connection);
                    await setNoExecOff.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                resultBuilder.AppendLine("Unhandled error: " + ex.Message);
            }

            return resultBuilder.ToString().Trim();
        }

        /// <summary>
        /// Test connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns><![CDATA[Task<bool>]]></returns>
        internal static async Task<bool> TestConnectionAsync(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false; // Return false if the connection string is null or empty
            }

            using SqlConnection connection = new(connectionString);
            try
            {
                await connection.OpenAsync();

                // perform a simple query to ensure the connection is valid
                using var command = new SqlCommand("SELECT 1", connection);
                command.CommandTimeout = 50000;
                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        #endregion SQL Execution & Validation

        #region Database Object Metadata Retrieval

        /// <summary>
        /// Gets the database objects asynchronously based on the specified object type.
        /// </summary>
        /// <param name="tableType">The type of database object (e.g., Table, View, StoredProcedure, Function).</param>
        /// <returns>A Task containing a list of ObjectName objects.</returns>
        internal static async Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums tableType, string connectionString)
        {
            try
            {
                return tableType switch
                {
                    ObjectTypeEnums.Table => await GetTablesAsync(connectionString),
                    ObjectTypeEnums.View => await GetViewsAsync(connectionString),
                    ObjectTypeEnums.StoredProcedure => await GetStoredProceduresAsync(connectionString),
                    ObjectTypeEnums.Function => await GetFunctionsAsync(connectionString),
                    ObjectTypeEnums.Trigger => await GetTriggersAsync(connectionString),
                    ObjectTypeEnums.Synonym => await GetSynonymsAsync(connectionString),
                    _ => throw new NotSupportedException($"Unsupported object type: {tableType}.")
                };
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                MessageBox.Show($"Error retrieving database objects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return []; // Return an empty list in case of an error
            }
        }

        /// <summary>
        /// Gets the all objects async.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetAllObjectsAsync(string? connectionString)
        {
            List < ObjectName > objects = [];

            if (!string.IsNullOrEmpty(connectionString))
            {
                string sql = $@"SELECT 
s.name AS SchemaName,
o.name AS ObjectName,
o.type AS ObjectType
FROM sys.objects o
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE o.type NOT IN ('D','IT', 'PK', 'S', 'SQ', 'UQ')";

                try
                {
                    var dt = await GetDataTableAsync(sql, connectionString);
                    if (dt?.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string schemaName = row["SchemaName"].ToString();
                            string objectName = row["ObjectName"].ToString();
                            ObjectTypeEnums objectType = ObjectName.ConvertObjectType(row["ObjectType"].ToString());

                            objects.Add(new ObjectName(objectType, schemaName, objectName));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return objects;
        }

        /// <summary>
        /// Get database list of a sql server
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        internal static async Task<List<string>> GetDatabases(string serverName, CancellationToken cancellationToken)
        {
            List<String> databases = [];

            try
            {
                Microsoft.Data.SqlClient.SqlConnectionStringBuilder connection = new()
                {
                    DataSource = serverName,
                    IntegratedSecurity = true,
                    Encrypt = true,
                    TrustServerCertificate = true
                };

                String strConn = connection.ToString();
                SqlConnection sqlConn = new(strConn);
                await sqlConn.OpenAsync(cancellationToken);

                //get databases
                DataTable tblDatabases = sqlConn.GetSchema("Databases");

                sqlConn.Close();

                foreach (DataRow row in tblDatabases.Rows)
                {
                    String? strDatabaseName = row["database_name"].ToString();

                    if (!string.IsNullOrEmpty(strDatabaseName))
                    {
                        databases.Add(strDatabaseName);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (SqlException)
            {
                //Ignore the error
            }
            catch (Exception)
            {
                throw;
            }

            databases.Sort();
            return databases;
        }

        /// <summary>
        /// Gets the functions async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetFunctionsAsync(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return [];
            }

            var objects = new List<ObjectName>();

            var query = @"SELECT
    s.name AS SchemaName,
    o.name AS FunctionName,
    o.type_desc AS FunctionType
FROM sys.objects o
JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE o.type IN ('FN', 'IF', 'TF')
ORDER BY s.name, o.name;";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.Function, reader["SchemaName"].ToString(), reader["FunctionName"].ToString()));
            }

            return objects;
        }

        /// <summary>
        /// Gets the schemas async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<string>> GetSchemasAsync(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return [];
            }

            var schemas = new List<string>();
            var query = "SELECT name FROM sys.schemas ORDER BY name";
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var schemaName = reader["name"].ToString();
                    if (!string.IsNullOrEmpty(schemaName))
                        schemas.Add(schemaName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schemas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await connection.CloseAsync();
            }
            return schemas;
        }

        /// <summary>
        /// Gets the stored procedures async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetStoredProceduresAsync(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return [];
            }

            var objects = new List<ObjectName>();

            var query = @"SELECT
    s.name AS SchemaName,
    p.name AS SPName
FROM sys.procedures p
JOIN sys.schemas s ON p.schema_id = s.schema_id
ORDER BY s.name, p.name;";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection) { CommandType = CommandType.Text, CommandTimeout = 50000 };
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.StoredProcedure, reader["SchemaName"].ToString(), reader["SPName"].ToString()));
            }

            return objects;
        }

        /// <summary>
        /// Gets the tables async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetTablesAsync(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return [];
            }

            var objects = new List<ObjectName>();

            var query = @"
                SELECT
                    TABLE_SCHEMA,
                    TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection) { CommandType = CommandType.Text, CommandTimeout = 50000 };
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.Table, reader["TABLE_SCHEMA"].ToString(), reader["TABLE_NAME"].ToString()));
            }

            return objects;
        }

        /// <summary>
        /// Gets the synonyms async.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<List<ObjectName>> GetSynonymsAsync(string connectionString)
        {
            var objects = new List<ObjectName>();

            var query = @"
SELECT
    sch.name AS SchemaName,
    syn.name AS SynonymName
FROM sys.synonyms syn
INNER JOIN sys.schemas sch ON syn.schema_id = sch.schema_id
ORDER BY sch.name, syn.name;";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.Synonym, reader["SchemaName"].ToString(), reader["SynonymName"].ToString()));
            }

            return objects;
        }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<List<ObjectName>> GetTriggersAsync(string connectionString)
        {
            var objects = new List<ObjectName>();

            var query = @"SELECT s.name AS SchemaName, tr.name AS TriggerName
FROM sys.triggers tr
JOIN sys.objects o ON tr.parent_id = o.object_id
JOIN sys.schemas s ON o.schema_id = s.schema_id";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.Trigger, reader["SchemaName"].ToString(), reader["TriggerName"].ToString()));
            }

            return objects;
        }

        /// <summary>
        /// Gets the views async.
        /// </summary>
        /// <returns>A Task.</returns>
        private static async Task<List<ObjectName>> GetViewsAsync(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return [];
            }

            var objects = new List<ObjectName>();

            var query = @"
                SELECT
                    TABLE_SCHEMA,
                    TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'View'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.View, reader["TABLE_SCHEMA"].ToString(), reader["TABLE_NAME"].ToString()));
            }

            return objects;
        }

        #endregion Database Object Metadata Retrieval

        #region Object & Column Descriptions

        /// <summary>
        /// Adds the usp_addupdateextendedproperty stored procedures to the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> AddObjectDescriptionSPs(string connectionString)
        {
            var script = DatabaseDocBuilder.UspAddObjectDescription();

            var sqlStatements = Regex.Split(script, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // execute each statement
            foreach (var sql in sqlStatements)
            {
                //Execute(builder.ConnectionString, sql);
                if (sql.Length > 0)
                {
                    var result = await SQLDatabaseHelper.ExecuteSQLAsync(sql, connectionString);
                    if (result != string.Empty)
                    {
                        return result;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the column description.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="column">The column.</param>
        /// <returns>A string.</returns>
        internal static async Task<string> GetColumnDescriptionAsync(ObjectName objectName, string column, string connectionString)
        {
            string result = string.Empty;
            string sql;
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                sql = $"SELECT E.value Description FROM sys.schemas S INNER JOIN sys.views T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = N'{objectName.Schema}' AND T.name = N'{objectName.Name}' AND C.name = N'{column}'";
            }
            else
            {
                sql = $"SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = N'{objectName.Schema}' AND T.name = N'{objectName.Name}' AND C.name = N'{column}'";
            }
            var conn = new SqlConnection(connectionString);
            try
            {
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };
                await using var dr = await cmd.ExecuteReaderAsync();
                if (await dr.ReadAsync())
                {
                    result = dr.GetString(0);
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                await conn.CloseAsync();
            }

            return result;
        }

        /// <summary>
        /// Gets the object description async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetObjectDescriptionAsync(ObjectName? objectName, string? connectionString)
        {
            if (objectName == null || objectName?.ObjectType == ObjectTypeEnums.None || string.IsNullOrEmpty(connectionString))
                return string.Empty;

            string result = string.Empty;

            if (objectName != null)
            {
                try
                {
                    string sql = string.Empty;

                    switch (objectName.ObjectType)
                    {
                        case ObjectTypeEnums.Table:
                            sql = $@"
SELECT value
FROM fn_listextendedproperty (
    NULL, 'schema', '{objectName.Schema}', 'table', '{objectName.Name}', default, default
)
WHERE name = N'MS_Description'";
                            break;

                        case ObjectTypeEnums.View:
                            sql = $@"
SELECT value
FROM fn_listextendedproperty (
    NULL, 'schema', '{objectName.Schema}', 'view', '{objectName.Name}', default, default
)
WHERE name = N'MS_Description'";
                            break;

                        case ObjectTypeEnums.StoredProcedure:
                            sql = $@"
SELECT value
FROM fn_listextendedproperty (
    NULL, 'schema', '{objectName.Schema}', 'procedure', '{objectName.Name}', default, default
)
WHERE name = N'MS_Description'";
                            break;

                        case ObjectTypeEnums.Function:
                            sql = $@"
SELECT value
FROM fn_listextendedproperty (
    NULL, 'schema', '{objectName.Schema}', 'function', '{objectName.Name}', default, default
)
WHERE name = N'MS_Description'";
                            break;

                        case ObjectTypeEnums.Trigger:
                            // Get trigger parent info
                            var (parentName, parentType) = await GetTriggerParentAsync(objectName.Name, connectionString);
                            if (string.IsNullOrEmpty(parentName) || string.IsNullOrEmpty(parentType)) return string.Empty;

                            sql = $@"
SELECT value
FROM fn_listextendedproperty (
    NULL, 'schema', '{objectName.Schema}', '{parentType.ToLower()}', '{parentName}', 'trigger', '{objectName.Name}'
)
WHERE name = N'MS_Description'";
                            break;

                        default:
                            return string.Empty;
                    }

                    var returnValue = await SQLDatabaseHelper.ExecuteScalarAsync(sql, connectionString);

                    if (returnValue != null && returnValue != DBNull.Value)
                    {
                        result = returnValue.ToString() ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Common.MsgBox(ex.Message, MessageBoxIcon.Error);
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the discription for the object.
        /// </summary>
        /// <param name="newDescription">The new description.</param>
        /// <returns>A Task.</returns>
        public static async Task UpdateObjectDescAsync(ObjectName objectName, string newDescription, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName.IsEmpty()) return;

            string newDesc = (newDescription ?? string.Empty).Replace("'", "''");

            string level1type;
            string level1name;
            string level2type = null;
            string level2name = null;

            switch (objectName.ObjectType)
            {
                case ObjectTypeEnums.Table:
                    level1type = "TABLE";
                    level1name = objectName.Name;
                    break;

                case ObjectTypeEnums.View:
                    level1type = "VIEW";
                    level1name = objectName.Name;
                    break;

                case ObjectTypeEnums.StoredProcedure:
                    level1type = "PROCEDURE";
                    level1name = objectName.Name;
                    break;

                case ObjectTypeEnums.Function:
                    level1type = "FUNCTION";
                    level1name = objectName.Name;
                    break;

                case ObjectTypeEnums.Trigger:
                    var (triggerParent, parentType) = await GetTriggerParentAsync(objectName.Name, connectionString);
                    level1type = parentType;
                    level1name = triggerParent;
                    level2type = "TRIGGER";
                    level2name = objectName.Name;
                    break;

                case ObjectTypeEnums.Synonym:
                    level1type = "SYNONYM";
                    level1name = objectName.Name;
                    break;

                default:
                    throw new InvalidOperationException("Unsupported object type for description update.");
            }

            string level2Clause = (level2type != null)
                ? $", @level2type = N'{level2type}', @level2name = N'{level2name}'"
                : string.Empty;

            string query;
            if (string.IsNullOrEmpty(newDescription))
            {
                query = $@"IF EXISTS (
    SELECT 1
      FROM sys.extended_properties
     WHERE class = 1 AND major_id = OBJECT_ID(N'{objectName.FullName}')
       AND minor_id = 0
       AND name = 'MS_Description')
    BEGIN
        EXEC sys.sp_dropextendedproperty
@name = N'MS_Description',
@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}',
@level1type = N'{level1type}', @level1name = N'{level1name}'{level2Clause};
    END;";
            }
            else
            {
                query = $@"IF EXISTS (
    SELECT 1
      FROM sys.extended_properties
     WHERE class = 1 AND major_id = OBJECT_ID(N'{objectName.FullName}')
       AND minor_id = 0
       AND name = 'MS_Description')
    BEGIN
        EXEC sys.sp_updateextendedproperty
            @name = N'MS_Description',
            @value = N'{newDesc}',
            @level0type = N'SCHEMA', @level0name = N'{objectName.Schema}',
            @level1type = N'{level1type}', @level1name = N'{level1name}'{level2Clause};
    END
    ELSE
    BEGIN
        EXEC sys.sp_addextendedproperty
            @name = N'MS_Description',
            @value = N'{newDesc}',
            @level0type = N'SCHEMA', @level0name = N'{objectName.Schema}',
            @level1type = N'{level1type}', @level1name = N'{level1name}'{level2Clause};
    END";
            }

            var result = await SQLDatabaseHelper.ExecuteSQLAsync(query, connectionString);
            if (!string.IsNullOrEmpty(result))
            {
                Common.MsgBox("Failed to update object description." + Environment.NewLine + result, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the column/parameter (level2) description.
        /// </summary>
        /// <param name="columnOrParameter">The column name.</param>
        /// <param name="newDescription">The new description.</param>
        public static async Task UpdateLevel2DescriptionAsync(ObjectName objectName, string columnOrParameter, string newDescription, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName.IsEmpty()) return;

            var objectType = objectName.ObjectType;
            string newDesc = newDescription.Replace("'", "''");
            string level1type = objectType switch
            {
                ObjectTypeEnums.Table => "TABLE",
                ObjectTypeEnums.View => "VIEW",
                ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                ObjectTypeEnums.Function => "FUNCTION",
                ObjectTypeEnums.Trigger => "TRIGGER",
                ObjectTypeEnums.Synonym => "Synonym",
                _ => throw new InvalidOperationException("Unsupported object type for description update.")
            };

            string level2Type = objectType switch
            {
                ObjectTypeEnums.Table or ObjectTypeEnums.View => "COLUMN",
                ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => "PARAMETER",
                _ => throw new InvalidOperationException("Unsupported object type for description update.")
            };

            string query;
            if (string.IsNullOrEmpty(newDescription))
            {
                query = $@"EXEC sys.sp_dropextendedproperty
@name = N'MS_Description',
@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}',
@level1type = N'{level1type}', @level1name = N'{objectName.Name}',
@level2type = N'{level2Type}', @level2name = N'{columnOrParameter}'";
            }
            else
            {
                query = $@"
IF EXISTS (
    SELECT 1
    FROM fn_listextendedproperty (
		N'MS_Description',
        N'SCHEMA',
        N'{objectName.Schema}',
        N'{level1type}',
        N'{objectName.Name}',
        N'{level2Type}',
        N'{columnOrParameter}')
)
BEGIN
    EXEC sys.sp_updateextendedproperty
        @name = N'MS_Description',
        @value = N'{newDesc}',
        @level0type = N'SCHEMA', @level0name = N'{objectName.Schema}',
        @level1type = N'{level1type}', @level1name = N'{objectName.Name}',
        @level2type = N'{level2Type}', @level2name = N'{columnOrParameter}';
END
ELSE
BEGIN
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'{newDesc}',
        @level0type = N'SCHEMA', @level0name = N'{objectName.Schema}',
        @level1type = N'{level1type}', @level1name = N'{objectName.Name}',
        @level2type = N'{level2Type}', @level2name = N'{columnOrParameter}';
END;";
            }

            await SQLDatabaseHelper.ExecuteSQLAsync(query, connectionString);
        }

        /// <summary>
        /// Gets the trigger parent async.
        /// </summary>
        /// <param name="triggerName">The trigger name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<(string TriggerParent, string ParentType)> GetTriggerParentAsync(string triggerName, string connectionString)
        {
            string triggerParent = null;
            string parentType = null;

            string query = @"
SELECT
    o.name AS ParentName,
    o.type_desc AS ParentType
FROM sys.triggers t
INNER JOIN sys.objects o ON t.parent_id = o.object_id
WHERE t.name = @TriggerName;";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TriggerName", triggerName);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        triggerParent = reader["ParentName"]?.ToString();
                        var typeDesc = reader["ParentType"]?.ToString();

                        parentType = typeDesc switch
                        {
                            "USER_TABLE" => "TABLE",
                            "VIEW" => "VIEW",
                            _ => "UNKNOWN"
                        };
                    }
                }
            }

            return (triggerParent, parentType);
        }

        #endregion Object & Column Descriptions

        #region Object Definitions

        /// <summary>
        /// Gets the object definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetObjectDefinitionAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType == ObjectTypeEnums.None || string.IsNullOrEmpty(connectionString))
            {
                return string.Empty;
            }

            string query = $@"SELECT sm.definition FROM sys.sql_modules sm WHERE sm.object_id = OBJECT_ID(N'{objectName.QuotedFullName}')";
            var result = await ExecuteScalarAsync(query, connectionString);
            return result != null && result != DBNull.Value ? result.ToString() ?? string.Empty : string.Empty;
        }

        #endregion Object Definitions

        #region Miscellaneous Methods

        /// <summary>
        /// Gets the row count of a table or view asynchronously.
        /// </summary>
        /// <param name="fullName">The full name of the object.</param>
        /// <returns>A Task<int> representing the row count.</returns>
        internal static async Task<int> GetRowCountAsync(string fullName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(fullName))
            {
                return 0;
            }

            var sql = $"SELECT COUNT(*) FROM {fullName}";
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 0;
        }

        /// <summary>
        /// Gets the identity column name for the specified table asynchronously, or null if none exists.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>The identity column name, or null.</returns>
        internal static async Task<bool> HasIdentityColumnAsync(ObjectName? objectName, string? connectionString)
        {
            if (objectName == null || objectName?.ObjectType == ObjectTypeEnums.None || string.IsNullOrEmpty(connectionString))
                return false;

            bool result = false;
            string sql = @"
SELECT
    ic.name AS identity_column_name
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = @TableName
AND s.name = @SchemaName;";

            await using var conn = new SqlConnection(connectionString);
            try
            {
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };
                cmd.Parameters.AddWithValue("@TableName", objectName?.Name);
                cmd.Parameters.AddWithValue("@SchemaName", objectName?.Schema);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await conn.CloseAsync();
            }

            return result;
        }

        /// <summary>
        /// Gets the objects using table async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetObjectsUsingTableAsync(ObjectName objectName, string connectionString)
        {
            var views = new List<ObjectName>();

            //var tableSchema = string.IsNullOrEmpty(objectName.Schema) ? "dbo" : objectName.Schema;

            string query = $@"
SELECT
    o.type_desc AS ReferencingObjectType,
    s.name AS ReferencingSchema,
    o.name AS ReferencingObjectName
FROM
    sys.sql_expression_dependencies d
INNER JOIN
    sys.objects o ON d.referencing_id = o.object_id
INNER JOIN
    sys.schemas s ON o.schema_id = s.schema_id
WHERE
    d.referenced_schema_name = N'{objectName.Schema}'
    AND d.referenced_entity_name = N'{objectName.Name}'";

            var dt = await GetDataTableAsync(query, connectionString);
            if (dt == null || dt.Rows.Count == 0)
                return views;

            foreach (DataRow row in dt.Rows)
            {
                string typeName = row["ReferencingObjectType"]?.ToString() ?? "";
                var objectType = typeName switch
                {
                    "VIEW" => ObjectName.ObjectTypeEnums.View,
                    "SQL_SCALAR_FUNCTION" => ObjectName.ObjectTypeEnums.Function,
                    "SQL_STORED_PROCEDURE" => ObjectName.ObjectTypeEnums.StoredProcedure,
                    _ => ObjectName.ObjectTypeEnums.Table
                };

                string name = row["ReferencingObjectName"]?.ToString() ?? "";
                string schema = row["ReferencingSchema"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(schema))
                {
                    views.Add(new ObjectName(objectType, schema, name));
                }
            }

            return views;
        }

        #endregion Miscellaneous Methods

        /*
        /// <summary>
        /// Gets all foreign keys in the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A DataTable containing all foreign key relationships.</returns>
        internal static DataTable? GetAllForeignKeys(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return null; // Return null if the connection string is null or empty
            }

            var dtForeignKeys = new DataTable();
            using SqlConnection conn = new(connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = @"
SELECT
    SCHEMA_NAME(fk.schema_id) AS SchemaName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    SCHEMA_NAME(referencedObj.schema_id) AS ReferencedSchemaName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.objects AS referencedObj ON fk.referenced_object_id = referencedObj.object_id";
                conn.Open();
                var dr = cmd.ExecuteReader();
                dtForeignKeys.Load(dr);
                dr.Close();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return dtForeignKeys;
        }

        /// <summary>
        /// Gets the referenced foreign key(s) for a specific column in a table.
        /// </summary>
        /// <param name="dtForeignKeys">The DataTable of foreign keys (from GetAllForeignKeys).</param>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>A string listing referenced columns, or empty if none.</returns>
        internal static string GetForeignKey(DataTable dtForeignKeys, string schemaName, string tableName, string columnName)
        {
            if (dtForeignKeys == null || dtForeignKeys.Rows.Count == 0)
            {
                return string.Empty;
            }

            var foreignKeys = from fk in dtForeignKeys.AsEnumerable()
                              where fk.Field<string>("SchemaName") == schemaName
                                 && fk.Field<string>("TableName") == tableName
                                 && fk.Field<string>("ColumnName") == columnName
                              select fk;

            string foreignKey = string.Empty;
            foreach (var fk in foreignKeys)
            {
                foreignKey += $"[{fk.Field<string>("ReferencedSchemaName")}].[{fk.Field<string>("ReferencedTableName")}].[{fk.Field<string>("ReferencedColumnName")}], ";
            }
            if (foreignKey.Length > 2)
                foreignKey = foreignKey.Substring(0, foreignKey.Length - 2);

            return foreignKey;
        }
        */
    }
}