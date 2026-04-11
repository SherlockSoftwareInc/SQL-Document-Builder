using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SQL_Document_Builder.DatabaseAccess;
using SQL_Document_Builder.SchemaMetadata;
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

            if (dbObject.IsEmpty() || dbObject.ObjectType == ObjectTypeEnums.None)
            {
                //throw new InvalidOperationException("The database object must have a valid schema and name.");
                return string.Empty; // Return empty string if the object is not valid
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
                AppendConditionalDropStatement(
                    createScript,
                    connection,
                    $"OBJECT_ID(N'{dbObject.FullName}', 'SN') IS NOT NULL",
                    $"DROP SYNONYM {dbObject.FullName}",
                    $"DROP SYNONYM IF EXISTS {dbObject.FullName}");
            }

            string baseObjectName = await SchemaMetadataProviderContext.Current.GetSynonymBaseObjectAsync(dbObject, connection.ConnectionString);
            if (string.IsNullOrEmpty(baseObjectName))
                return string.Empty;

            // Add the CREATE SYNONYM statement
            createScript.AppendLine($"CREATE SYNONYM {dbObject.FullName} FOR {baseObjectName};");
            createScript.AppendLine(GetBatchSeparator(connection));

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
                var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                var dt = await provider.GetDataTableAsync(sql, connection.ConnectionString);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Get the column names
                    var columnNames = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => QuoteIdentifier(c.ColumnName, connection)));
                    var qualifiedTableName = QuoteQualifiedName(tableName, connection);

                    var identityOverrideClause = GetIdentityInsertOverrideClause(connection, hasIdentity);
                    bool isOracle = IsOracleConnection(connection);

                    if (hasIdentity && IsSqlServerConnection(connection))
                    {
                        // add SET IDENTITY_INSERT  ON;
                        sb.AppendLine($"SET IDENTITY_INSERT {qualifiedTableName} ON;");
                    }

                    if (isOracle)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            var rowValues = BuildInsertRowValues(row, dt.Columns, connection);
                            sb.AppendLine($"INSERT INTO {qualifiedTableName} ({columnNames}){identityOverrideClause} VALUES ({rowValues});");
                        }
                    }
                    else
                    {
                        int rowCount = 0;
                        int batchCount = 0;
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
                                sb.AppendLine($"INSERT INTO {qualifiedTableName} ({columnNames}){identityOverrideClause} VALUES");
                                batchCount++;
                            }

                            var rowValues = BuildInsertRowValues(row, dt.Columns, connection);
                            sb.AppendLine($"\t({rowValues}),");
                            rowCount++;
                        }

                        // Remove the trailing comma from the last row in the last batch
                        if (rowCount > 0)
                        {
                            sb.Length -= 3; // Remove ",\n"
                            sb.AppendLine(";");
                        }
                    }

                    if (hasIdentity && IsSqlServerConnection(connection))
                    {
                        sb.AppendLine($"SET IDENTITY_INSERT {qualifiedTableName} OFF;");
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

            var sourceTableName = QuoteQualifiedName(tableName.FullNameNoQuote, connection);
            var sql = $"select * from {sourceTableName}";
            // check if the SQL statement is a valid SELECT statement to generate JSON data
            var provider = DatabaseAccessProviderFactory.GetProvider(connection);
            if (!await provider.IsValidSelectStatementAsync(sql, connection.ConnectionString))
            {
                MessageBox.Show("Cannot generate the JSON data because the table or view contains columns with unsupported data types.", "Invalid SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }

            var hasIdentity = await SchemaMetadataProviderContext.Current.HasIdentityColumnAsync(tableName, connection.ConnectionString);
            return await QueryDataToInsertStatementAsync(sql, connection, tableName.FullNameNoQuote, hasIdentity);
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

            static bool ReadBooleanValue(object value)
            {
                if (value == null || value == DBNull.Value)
                {
                    return false;
                }

                if (value is bool b)
                {
                    return b;
                }

                if (value is byte or sbyte or short or ushort or int or uint or long or ulong)
                {
                    return Convert.ToInt64(value) != 0;
                }

                if (bool.TryParse(value.ToString(), out var parsedBool))
                {
                    return parsedBool;
                }

                if (long.TryParse(value.ToString(), out var parsedNumber))
                {
                    return parsedNumber != 0;
                }

                return false;
            }

            StringBuilder sb = new();

            var dt = await SchemaMetadataProviderContext.Current.GetCreateIndexesMetadataAsync(objectName, connection.ConnectionString);
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
                bool isUnique = ReadBooleanValue(dr["IsUnique"]);
                string schema = dr["SchemaName"].ToString() ?? "";
                string objName = dr["ObjectName"].ToString() ?? "";
                string columns = dr["IndexColumns"].ToString() ?? "";
                string? filter = dr["FilterDefinition"] as string;
                bool isDisabled = ReadBooleanValue(dr["IsDisabled"]);
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
add, and sp_updateextendedproperty can only be used to update, the usp_addupdateextendedproperty
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

    BEGIN TRY
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
            BEGIN
                BEGIN TRY
                    EXEC sys.sp_dropextendedproperty
                        @name = @name,
                        @level0type = @level0type, @level0name = @level0name,
                        @level1type = @level1type, @level1name = @level1name,
                        @level2type = @level2type, @level2name = @level2name;
                END TRY
                BEGIN CATCH
                    -- Suppress errors, optionally log if desired
                END CATCH
            END
            ELSE
            BEGIN
                BEGIN TRY
                    EXEC sys.sp_updateextendedproperty
                        @name = @name,
                        @value = @value,
                        @level0type = @level0type, @level0name = @level0name,
                        @level1type = @level1type, @level1name = @level1name,
                        @level2type = @level2type, @level2name = @level2name;
                END TRY
                BEGIN CATCH
                    -- Suppress errors, optionally log if desired
                END CATCH
            END
        END
        ELSE
        BEGIN
            BEGIN TRY
                EXEC sys.sp_addextendedproperty
                    @name = @name,
                    @value = @value,
                    @level0type = @level0type, @level0name = @level0name,
                    @level1type = @level1type, @level1name = @level1name,
                    @level2type = @level2type, @level2name = @level2name;
            END TRY
            BEGIN CATCH
                -- Suppress errors, optionally log if desired
            END CATCH
        END
    END TRY
    BEGIN CATCH
        -- Top-level CATCH: suppress or handle unexpected errors in the procedure
    END CATCH
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

            AppendConditionalDropStatement(
                createScript,
                connection,
                $"EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{objectName.FullName}') AND type IN ('FN','IF','TF'))",
                $"DROP FUNCTION {objectName.FullName}",
                $"DROP FUNCTION IF EXISTS {objectName.FullName}");

            var script = await GetDefinitionAsync(objectName, connection);
            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine(GetBatchSeparator(connection));
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
                AppendConditionalDropStatement(
                    createScript,
                    connection,
                    $"OBJECT_ID(N'{objectName.FullName}', 'P') IS NOT NULL",
                    $"DROP PROCEDURE {objectName.FullName}",
                    $"DROP PROCEDURE IF EXISTS {objectName.FullName}");
            }

            var script = await GetDefinitionAsync(objectName, connection);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine(GetBatchSeparator(connection));
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
                AppendConditionalDropStatement(
                    createScript,
                    connection,
                    $"OBJECT_ID(N'{objectName.FullName}', 'U') IS NOT NULL",
                    $"DROP TABLE {objectName.FullName}",
                    $"DROP TABLE IF EXISTS {objectName.FullName}");
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
            createScript.AppendLine(GetBatchSeparator(connection));

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
                AddBatchSeparatorStatement(createScript, connection);

                // remove the new line at the end of the script
                triggerScript = triggerScript.TrimEnd('\r', '\n');
                createScript.AppendLine(triggerScript);
            }

            // add GO statement
            AddBatchSeparatorStatement(createScript, connection);
            //if (!createScript.ToString().EndsWith(Environment.NewLine + "GO" + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
            //{
            //    createScript.AppendLine($"GO");
            //}

            return createScript.ToString();
        }

        /// <summary>
        /// Adds the DBMS batch separator to the script if the script does not already end with it.
        /// </summary>
        /// <param name="sb">The sb.</param>
        private static void AddBatchSeparatorStatement(StringBuilder sb, DatabaseConnectionItem connection)
        {
            var batchSeparator = GetBatchSeparator(connection);
            if (!sb.ToString().EndsWith(Environment.NewLine + batchSeparator + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine(batchSeparator);
            }
        }

        private static string GetBatchSeparator(DatabaseConnectionItem connection)
        {
            bool isSqlServer = IsSqlServerConnection(connection);

            return isSqlServer ? "GO" : ";";
        }

        private static bool IsSqlServerConnection(DatabaseConnectionItem connection)
        {
            return connection.ConnectionType.Equals("SQL Server", StringComparison.OrdinalIgnoreCase) ||
                   (!connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase) &&
                    connection.DBMSType == DBMSTypeEnums.SQLServer);
        }

        private static bool IsPostgreSqlConnection(DatabaseConnectionItem connection)
        {
            return connection.DBMSType == DBMSTypeEnums.PostgreSQL ||
                   connection.ConnectionType.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOracleConnection(DatabaseConnectionItem connection)
        {
            return connection.DBMSType == DBMSTypeEnums.Oracle ||
                   connection.ConnectionType.Equals("Oracle", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetIdentityInsertOverrideClause(DatabaseConnectionItem connection, bool hasIdentity)
        {
            if (!hasIdentity)
            {
                return string.Empty;
            }

            if (IsPostgreSqlConnection(connection) || IsOracleConnection(connection))
            {
                return " OVERRIDING SYSTEM VALUE";
            }

            return string.Empty;
        }

        private static string BuildInsertRowValues(DataRow row, DataColumnCollection columns, DatabaseConnectionItem connection)
        {
            var values = new List<string>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                values.Add(FormatInsertValue(row[i], columns[i].DataType, connection));
            }

            return string.Join(", ", values);
        }

        private static string FormatInsertValue(object value, Type type, DatabaseConnectionItem connection)
        {
            if (value == DBNull.Value)
            {
                return "NULL";
            }

            if (type == typeof(string))
            {
                return BuildStringLiteral(value.ToString(), connection);
            }

            if (type == typeof(DateTime))
            {
                return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) ||
                type == typeof(decimal) || type == typeof(double) || type == typeof(float) ||
                type == typeof(byte))
            {
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (type == typeof(bool))
            {
                if (IsPostgreSqlConnection(connection))
                {
                    return (bool)value ? "TRUE" : "FALSE";
                }

                return (bool)value ? "1" : "0";
            }

            return BuildStringLiteral(value.ToString(), connection);
        }

        private static string BuildStringLiteral(string? value, DatabaseConnectionItem connection)
        {
            var escapedValue = (value ?? string.Empty).Replace("'", "''");
            var prefix = IsSqlServerConnection(connection) ? "N" : string.Empty;
            return $"{prefix}'{escapedValue}'";
        }

        private static string QuoteQualifiedName(string name, DatabaseConnectionItem connection)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return string.Join(".", name.Split('.').Select(part => QuoteIdentifier(part, connection)));
        }

        private static string QuoteIdentifier(string identifier, DatabaseConnectionItem connection)
        {
            string value = identifier.Trim();
            if (value.Length >= 2)
            {
                bool isBracketQuoted = value[0] == '[' && value[^1] == ']';
                bool isDoubleQuoted = value[0] == '"' && value[^1] == '"';
                bool isBacktickQuoted = value[0] == '`' && value[^1] == '`';
                if (isBracketQuoted || isDoubleQuoted || isBacktickQuoted)
                {
                    value = value[1..^1];
                }
            }

            if (IsSqlServerConnection(connection))
            {
                return $"[{value.Replace("]", "]]" )}]";
            }

            if (connection.DBMSType == DBMSTypeEnums.MySQL || connection.DBMSType == DBMSTypeEnums.MariaDB)
            {
                return $"`{value.Replace("`", "``")}`";
            }

            if (IsPostgreSqlConnection(connection) || IsOracleConnection(connection))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return $"[{value.Replace("]", "]]" )}]";
        }

        private static void AppendConditionalDropStatement(
            StringBuilder script,
            DatabaseConnectionItem connection,
            string sqlServerCondition,
            string sqlServerDropStatement,
            string nonSqlServerDropStatement)
        {
            if (IsSqlServerConnection(connection))
            {
                script.AppendLine($"IF {sqlServerCondition}");
                script.AppendLine($"\t{sqlServerDropStatement};");
            }
            else
            {
                script.AppendLine($"{nonSqlServerDropStatement};");
            }

            script.AppendLine(GetBatchSeparator(connection));
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
                AppendConditionalDropStatement(
                    createScript,
                    connection,
                    $"OBJECT_ID(N'{objectName.FullName}', 'TR') IS NOT NULL",
                    $"DROP TRIGGER {objectName.FullName}",
                    $"DROP TRIGGER IF EXISTS {objectName.FullName}");
            }

            var script = await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(objectName, connection.ConnectionString);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine(GetBatchSeparator(connection));
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

            var allTriggers = await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Trigger, connection.ConnectionString);
            if (allTriggers.Count == 0)
                return string.Empty;

            foreach (var triggerObject in allTriggers)
            {
                var triggerInfo = await SchemaMetadataProviderContext.Current.GetTriggerInfoAsync(triggerObject, connection.ConnectionString);
                if (triggerInfo == null || triggerInfo.Rows.Count == 0)
                {
                    continue;
                }

                var infoRow = triggerInfo.Rows[0];
                string schema = infoRow["ParentObjectSchema"]?.ToString() ?? string.Empty;
                string parentName = infoRow["ParentObjectName"]?.ToString() ?? string.Empty;

                if (!schema.Equals(objectName.Schema, StringComparison.OrdinalIgnoreCase) ||
                    !parentName.Equals(objectName.Name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string triggerName = triggerObject.Name;
                string definition = await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(triggerObject, connection.ConnectionString);
                bool isDisabled = string.Equals(infoRow["IsDisabled"]?.ToString(), "Yes", StringComparison.OrdinalIgnoreCase);

                // Add header
                sb.AppendLine($"/****** Object:  Trigger [{schema}].[{triggerName}] on [{schema}].[{parentName}] ******/");

                // Add drop statement if configured
                if (Properties.Settings.Default.AddDropStatement)
                {
                    AppendConditionalDropStatement(
                        sb,
                        connection,
                        $"OBJECT_ID(N'[{schema}].[{triggerName}]', 'TR') IS NOT NULL",
                        $"DROP TRIGGER [{schema}].[{triggerName}]",
                        $"DROP TRIGGER IF EXISTS [{schema}].[{triggerName}]");
                }

                // Add the trigger definition
                sb.Append(definition.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine);
                sb.AppendLine(GetBatchSeparator(connection));

                // Optionally, add DISABLE TRIGGER if the trigger is disabled
                if (isDisabled)
                {
                    sb.AppendLine($"ALTER TABLE [{schema}].[{parentName}] DISABLE TRIGGER [{triggerName}];");
                    sb.AppendLine(GetBatchSeparator(connection));
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
                AppendConditionalDropStatement(
                    createScript,
                    connection,
                    $"OBJECT_ID(N'{objectName.FullName}', 'V') IS NOT NULL",
                    $"DROP VIEW {objectName.FullName}",
                    $"DROP VIEW IF EXISTS {objectName.FullName}");
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

            createScript.AppendLine(GetBatchSeparator(connection));
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
            string? definition = await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(objectName, connection?.ConnectionString);

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