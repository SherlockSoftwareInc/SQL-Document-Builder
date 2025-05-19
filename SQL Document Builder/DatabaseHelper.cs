using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The sql.</param>
        internal static async Task ExecuteNonQueryAsync(string sql, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return;

            using var conn = new SqlConnection(connectionString);
            try
            {
                await using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await conn.CloseAsync();
            }
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
                // Handle or log the exception as needed
                //MessageBox.Show(ex.Message, "An Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                await using var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                await using var dr = await cmd.ExecuteReaderAsync();
                if (await dr.ReadAsync())
                {
                    result = dr.GetString(0);   // dr[0].ToString();
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
                return new List<ObjectName>(); // Return an empty list in case of an error
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
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A DataTable? .</returns>
        internal static DataTable? GetDataTable(string sql, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return null;

            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
            return null;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A DataTable? .</returns>
        internal static DataTable? GetDataTable(string sql, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                connection.Open(); // Ensure the connection is opened before executing commands
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                using var reader = command.ExecuteReader();

                var dataTable = new DataTable();

                // Fill DataTable manually with support for cancellation
                LoadFromReader(reader, dataTable, cancellationToken);

                return dataTable;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Operation was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null; // Or handle it as needed
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
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
            using var command = new SqlCommand(query, connection);
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
        /// Gets the scalar value.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>An Object? .</returns>
        internal static Object? GetScalarValue(string sql)
        {
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                connection.Open();
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
            return null;
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
            using var command = new SqlCommand(query, connection);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    schemas.Add(reader["name"].ToString());
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
            using var command = new SqlCommand(query, connection);
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
                await using var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
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

        internal static async Task<DataTable> GetTableListAsync(string schemaName)
        {
            throw new NotImplementedException();
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
            using var command = new SqlCommand(query, connection);
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
                await using var cmd = new SqlCommand(sql, conn);
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
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new ObjectName(ObjectTypeEnums.View, reader["TABLE_SCHEMA"].ToString(), reader["TABLE_NAME"].ToString()));
            }

            return objects;
        }

        /// <summary>
        /// Loads the from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private static void LoadFromReader(SqlDataReader reader, DataTable dataTable, CancellationToken cancellationToken)
        {
            // Add columns to DataTable based on reader fields
            for (int i = 0; i < reader.FieldCount; i++)
            {
                dataTable.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
            }

            // Read rows from reader
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool read;
                try
                {
                    read = reader.Read();
                }
                catch (Exception ex)
                {
                    // Handle or log the exception as needed
                    throw new InvalidOperationException("Error reading data from the reader.", ex);
                }

                if (!read) break; // Exit loop if no more rows

                var row = dataTable.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                }
                dataTable.Rows.Add(row);
            }
        }
    }
}