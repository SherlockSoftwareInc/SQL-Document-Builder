using Microsoft.Data.SqlClient;
using SQL_Document_Builder.SchemaMetadata;
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
                return await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(tableType, connectionString);
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
            if (string.IsNullOrEmpty(connectionString))
            {
                return [];
            }

            return await SchemaMetadataProviderContext.Current.GetAllObjectsAsync(connectionString);
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

            return await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(ObjectTypeEnums.Function, connectionString);
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

            return await SchemaMetadataProviderContext.Current.GetSchemasAsync(connectionString);
        }

        internal static async Task<DataTable?> GetObjectColumnsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetObjectColumnsAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetObjectParametersAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetObjectParametersAsync(objectName, connectionString);
        }

        internal static async Task<List<string>> GetPrimaryKeyColumnsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return [];
            }

            return await SchemaMetadataProviderContext.Current.GetPrimaryKeyColumnsAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetObjectIndexesAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetObjectIndexesAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetCreateIndexesMetadataAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetCreateIndexesMetadataAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetObjectConstraintsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetObjectConstraintsAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetCheckConstraintsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetCheckConstraintsAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetDefaultConstraintsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetDefaultConstraintsAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetForeignKeyConstraintsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetForeignKeyConstraintsAsync(objectName, connectionString);
        }

        internal static async Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumnsAsync(ObjectName objectName, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName == null || objectName.IsEmpty())
            {
                return [];
            }

            return await SchemaMetadataProviderContext.Current.GetIdentityColumnsAsync(objectName, connectionString);
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

            return await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(ObjectTypeEnums.StoredProcedure, connectionString);
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

            return await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(ObjectTypeEnums.Table, connectionString);
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
            return await SchemaMetadataProviderContext.Current.GetColumnDescriptionAsync(objectName, column, connectionString);
        }

        /// <summary>
        /// Gets the object description async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetObjectDescriptionAsync(ObjectName? objectName, string? connectionString)
        {
            if (objectName == null || objectName?.ObjectType == ObjectTypeEnums.None || string.IsNullOrEmpty(connectionString))
                return string.Empty;

            return await SchemaMetadataProviderContext.Current.GetObjectDescriptionAsync(objectName, connectionString);
        }

        /// <summary>
        /// Updates the discription for the object.
        /// </summary>
        /// <param name="newDescription">The new description.</param>
        /// <returns>A Task.</returns>
        public static async Task UpdateObjectDescAsync(ObjectName objectName, string newDescription, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName.IsEmpty()) return;

            await SchemaMetadataProviderContext.Current.UpdateObjectDescriptionAsync(objectName, newDescription, connectionString);
        }

        /// <summary>
        /// Updates the column/parameter (level2) description.
        /// </summary>
        /// <param name="columnOrParameter">The column name.</param>
        /// <param name="newDescription">The new description.</param>
        public static async Task UpdateLevel2DescriptionAsync(ObjectName objectName, string columnOrParameter, string newDescription, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || objectName.IsEmpty()) return;

            await SchemaMetadataProviderContext.Current.UpdateLevel2DescriptionAsync(objectName, columnOrParameter, newDescription, connectionString);
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

            return await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(objectName, connectionString);
        }

        internal static async Task<string> GetSynonymBaseObjectAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.Synonym || string.IsNullOrEmpty(connectionString))
            {
                return string.Empty;
            }

            return await SchemaMetadataProviderContext.Current.GetSynonymBaseObjectAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetTableInfoAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.Table || string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetTableInfoAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetViewInfoAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.View || string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetViewInfoAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetProcedureInfoAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.StoredProcedure || string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetProcedureInfoAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetFunctionInfoAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.Function || string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetFunctionInfoAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetTriggerInfoAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.Trigger || string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetTriggerInfoAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetSynonymInfoAsync(ObjectName objectName, string? connectionString)
        {
            if (objectName == null || objectName.ObjectType != ObjectTypeEnums.Synonym || string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            return await SchemaMetadataProviderContext.Current.GetSynonymInfoAsync(objectName, connectionString);
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

            try
            {
                return await SchemaMetadataProviderContext.Current.HasIdentityColumnAsync(objectName, connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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

            var dt = await SchemaMetadataProviderContext.Current.GetReferencingObjectsAsync(objectName, connectionString);
            if (dt == null || dt.Rows.Count == 0)
                return views;

            foreach (DataRow row in dt.Rows)
            {
                string typeName = row["ObjectType"]?.ToString() ?? "";
                var objectType = ObjectName.ConvertObjectType(typeName);

                string name = row["ObjectName"]?.ToString() ?? "";
                string schema = row["Schema"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(schema))
                {
                    views.Add(new ObjectName(objectType, schema, name));
                }
            }

            return views;
        }

        /// <summary>
        /// Gets the recent objects.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A list of ObjectNames.</returns>
        internal static List<ObjectName> GetRecentObjects(DateTime startDate, DateTime endDate, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return [];

            try
            {
                return SchemaMetadataProviderContext.Current
                    .GetRecentObjectsAsync(startDate, endDate, connectionString)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return [];
            }
        }

        /// <summary>
        /// Gets the referencing objects async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString)
        {
            if (objectName == null || string.IsNullOrEmpty(objectName.Schema) || string.IsNullOrEmpty(objectName.Name) || string.IsNullOrEmpty(connectionString))
                return null;

            return await SchemaMetadataProviderContext.Current.GetReferencingObjectsAsync(objectName, connectionString);
        }

        internal static async Task<DataTable?> GetObjectRelationshipsAsync(ObjectName objectName, string connectionString)
        {
            if (objectName == null || string.IsNullOrEmpty(objectName.Schema) || string.IsNullOrEmpty(objectName.Name) || string.IsNullOrEmpty(connectionString))
                return null;

            return await SchemaMetadataProviderContext.Current.GetObjectRelationshipsAsync(objectName, connectionString);
        }

        /// <summary>
        /// Gets the referenced objects async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString)
        {
            if (objectName == null || string.IsNullOrEmpty(objectName.Schema) || string.IsNullOrEmpty(objectName.Name) || string.IsNullOrEmpty(connectionString))
                return null;

            return await SchemaMetadataProviderContext.Current.GetReferencedObjectsAsync(objectName, connectionString);
        }

        /// <summary>
        /// Gets the referenced objects async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString)
        {
            if (objectName == null || string.IsNullOrEmpty(objectName.Schema) || string.IsNullOrEmpty(objectName.Name) || string.IsNullOrEmpty(connectionString))
                return null;

            return await SchemaMetadataProviderContext.Current.GetForeignTablesAsync(objectName, connectionString);
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