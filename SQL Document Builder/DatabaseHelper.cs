using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    internal class DatabaseHelper
    {
        /// <summary>
        /// Adds the object description s ps.
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
                    var result = await DatabaseHelper.ExecuteSQLAsync(sql, connectionString);
                    if (result != string.Empty)
                    {
                        return result;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Executes the scalar async.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>A Task.</returns>
        internal static async Task<object?> ExecuteScalarAsync(string sql, string connectionString = "")
        {
            object? value;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Properties.Settings.Default.dbConnectionString;
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
        internal static async Task<string> ExecuteSQLAsync(string sql, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = Properties.Settings.Default.dbConnectionString;

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
        /// Gets the column description.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="column">The column.</param>
        /// <returns>A string.</returns>
        internal static async Task<string> GetColumnDescriptionAsync(ObjectName objectName, string column)
        {
            string result = string.Empty;
            string sql;
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                sql = $"SELECT E.value Description FROM sys.schemas S INNER JOIN sys.views T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{objectName.Schema}' AND T.name = '{objectName.Name}' AND C.name = '{column}'";
            }
            else
            {
                sql = $"SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{objectName.Schema}' AND T.name = '{objectName.Name}' AND C.name = '{column}'";
            }
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the database objects asynchronously based on the specified object type.
        /// </summary>
        /// <param name="tableType">The type of database object (e.g., Table, View, StoredProcedure, Function).</param>
        /// <returns>A Task containing a list of ObjectName objects.</returns>
        internal static async Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums tableType)
        {
            try
            {
                return tableType switch
                {
                    ObjectTypeEnums.Table => await GetTablesAsync(),
                    ObjectTypeEnums.View => await GetViewsAsync(),
                    ObjectTypeEnums.StoredProcedure => await GetStoredProceduresAsync(),
                    ObjectTypeEnums.Function => await GetFunctionsAsync(),
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
        /// Get database list of a sql server
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        internal static async Task<List<string>> GetDatabases(string serverName, CancellationToken cancellationToken)
        {
            List<String> databases = [];

            try
            {
                SqlConnectionStringBuilder connection = new()
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
        /// Gets the data table.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>A DataTable? .</returns>
        internal static async Task<DataTable?> GetDataTableAsync(string sql)
        {
            var tables = new DataTable();
            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the functions async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetFunctionsAsync()
        {
            var objects = new List<ObjectName>();

            var query = @"SELECT
    s.name AS SchemaName,
    o.name AS FunctionName,
    o.type_desc AS FunctionType
FROM sys.objects o
JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE o.type IN ('FN', 'IF', 'TF')
ORDER BY s.name, o.name;";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the row count of a table or view asynchronously.
        /// </summary>
        /// <param name="fullName">The full name of the object.</param>
        /// <returns>A Task<int> representing the row count.</returns>
        internal static async Task<int> GetRowCountAsync(string fullName)
        {
            var sql = $"SELECT COUNT(*) FROM {fullName}";
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the schemas async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<string>> GetSchemasAsync()
        {
            var schemas = new List<string>();
            var query = "SELECT name FROM sys.schemas ORDER BY name";
            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        internal static async Task<List<ObjectName>> GetStoredProceduresAsync()
        {
            var objects = new List<ObjectName>();

            var query = @"SELECT
    s.name AS SchemaName,
    p.name AS SPName
FROM sys.procedures p
JOIN sys.schemas s ON p.schema_id = s.schema_id
ORDER BY s.name, p.name;";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the table description asynchronously.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task<string> containing the description.</returns>
        internal static async Task<string> GetTableDescriptionAsync(ObjectName objectName)
        {
            string result = string.Empty;
            string sql = $"SELECT value FROM fn_listextendedproperty (NULL, 'schema', N'{objectName.Schema}', '{(objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")}', N'{objectName.Name}', default, default) WHERE name = N'MS_Description'";

            await using var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the tables async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetTablesAsync()
        {
            var objects = new List<ObjectName>();

            var query = @"
                SELECT
                    TABLE_SCHEMA,
                    TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
        /// Gets the identity column name for the specified table asynchronously, or null if none exists.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>The identity column name, or null.</returns>
        internal static async Task<bool> HasIdentityColumnAsync(ObjectName objectName)
        {
            bool result = false;
            string sql = @"
SELECT
    ic.name AS identity_column_name
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = @TableName
AND s.name = @SchemaName;";

            await using var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };
                cmd.Parameters.AddWithValue("@TableName", objectName.Name);
                cmd.Parameters.AddWithValue("@SchemaName", objectName.Schema);

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
        /// Save the description of the selected column
        /// </summary>
        internal static async Task SaveColumnDescAsync(string? objectName, string columnName, string desc)
        {
            const string sql = @"
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

    IF EXISTS (SELECT value
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
END";
            await using var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                await using var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text, CommandTimeout = 5000 };
                cmd.Parameters.AddWithValue("@TableName", objectName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnName", columnName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", desc ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        /// <summary>
        /// Syntaxes the check async.
        /// </summary>
        /// <param name="userQuery">The user query.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> SyntaxCheckAsync(string userQuery, string connectionString = "")
        {
            string result = string.Empty;

            await ExecuteSQLAsync("SET NOEXEC ON");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Properties.Settings.Default.dbConnectionString;
            }
            using SqlConnection connection = new(connectionString);
            SqlCommand command = new(userQuery, connection);

            try
            {
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                result = ex.Message;
            }
            finally
            {
                await connection.CloseAsync();
            }

            await ExecuteSQLAsync("SET NOEXEC OFF");

            return result;
        }

        /// <summary>
        /// Test connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns><![CDATA[Task<bool>]]></returns>
        internal static async Task<bool> TestConnectionAsync(string connectionString)
        {
            using SqlConnection connection = new(connectionString);
            try
            {
                await connection.OpenAsync();
                connection.Close();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the views async.
        /// </summary>
        /// <returns>A Task.</returns>
        private static async Task<List<ObjectName>> GetViewsAsync()
        {
            var objects = new List<ObjectName>();

            var query = @"
                SELECT
                    TABLE_SCHEMA,
                    TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'View'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
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
    }
}