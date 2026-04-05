using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        #region Utility Methods

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

        #endregion Utility Methods

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
        /// Verifies the SQL code syntax.
        /// </summary>
        /// <param name="sqlCode">The sql code.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task&lt;string&gt;? .</returns>
        internal static async Task<string>? VerifySQL(string sqlCode, string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return "No database connection specified.";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sqlCode, connection);
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

        #endregion Miscellaneous Methods
    }
}