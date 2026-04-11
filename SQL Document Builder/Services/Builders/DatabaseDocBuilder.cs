using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
        public static async Task<string?> GetCreateObjectScriptAsync(ObjectName dbObject, DBSchema schemaCache)
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

            if (schemaCache == null)
            {
                throw new ArgumentNullException(nameof(schemaCache));
            }

            // Determine the object type and retrieve the corresponding creation script
            return dbObject.ObjectType switch
            {
                ObjectName.ObjectTypeEnums.Table => await GetCreateTableScriptAsync(dbObject, schemaCache),
                ObjectName.ObjectTypeEnums.View => await GetCreateViewScriptAsync(dbObject, schemaCache),
                ObjectName.ObjectTypeEnums.StoredProcedure => await GetCreateStoredProcedureScriptAsync(dbObject, schemaCache),
                ObjectName.ObjectTypeEnums.Function => await GetCreateFunctionScriptAsync(dbObject, schemaCache),
                ObjectName.ObjectTypeEnums.Trigger => await GetCreateTriggerScriptAsync(dbObject, schemaCache),
                ObjectName.ObjectTypeEnums.Synonym => await GetCreateSynonymScriptAsync(dbObject, schemaCache),
                _ => throw new NotSupportedException($"The object type '{dbObject.ObjectType}' is not supported.")
            };
        }

        /// <summary>
        /// Gets the create synonym script async.
        /// </summary>
        /// <param name="dbObject">The db object.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateSynonymScriptAsync(ObjectName dbObject, DBSchema schemaCache)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Synonym {dbObject.FullName} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                AppendConditionalDropStatement(
                    createScript,
                    schemaCache,
                    $"OBJECT_ID(N'{dbObject.FullName}', 'SN') IS NOT NULL",
                    $"DROP SYNONYM {dbObject.FullName}",
                    $"DROP SYNONYM IF EXISTS {dbObject.FullName}");
            }

            string baseObjectName = await schemaCache.GetSynonymBaseObjectAsync(dbObject);
            if (string.IsNullOrEmpty(baseObjectName))
                return string.Empty;

            // Add the CREATE SYNONYM statement
            createScript.AppendLine($"CREATE SYNONYM {dbObject.FullName} FOR {baseObjectName};");
            createScript.AppendLine(GetBatchSeparator(schemaCache));

            return createScript.ToString();
        }

        /// <summary>
        /// Convert the Query data to the insert statements.
        /// </summary>
        /// <param name="sql">The SQL query to fetch data.</param>
        /// <param name="tableName">The table name for the insert statements.</param>
        /// <returns>A Task<string> containing the generated insert statements or a warning if rows exceed 500.</returns>
        public static async Task<string> QueryDataToInsertStatementAsync(string sql, DBSchema schemaCache, string tableName = "YourTableName", bool hasIdentity = false)
        {
            if (schemaCache == null)
            {
                throw new ArgumentNullException(nameof(schemaCache));
            }

            var connection = schemaCache.Connection;
            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                throw new ArgumentException("Invalid schema cache connection.");
            }

            var sb = new StringBuilder();

            try
            {
                var dt = await DBData.GetDataTableAsync(connection, sql, CancellationToken.None);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!hasIdentity)
                    {
                        var cachedIdentity = TryGetIdentityFromSchemaCache(tableName, schemaCache);
                        if (cachedIdentity.HasValue)
                        {
                            hasIdentity = cachedIdentity.Value;
                        }
                    }

                    // Get the column names
                    var columnNames = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => QuoteIdentifier(c.ColumnName, schemaCache)));
                    var qualifiedTableName = QuoteQualifiedName(tableName, schemaCache);

                    var identityOverrideClause = GetIdentityInsertOverrideClause(schemaCache, hasIdentity);
                    bool isOracle = IsOracleConnection(schemaCache);

                    if (hasIdentity && IsSqlServerConnection(schemaCache))
                    {
                        // add SET IDENTITY_INSERT  ON;
                        sb.AppendLine($"SET IDENTITY_INSERT {qualifiedTableName} ON;");
                    }

                    if (isOracle)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            var rowValues = BuildInsertRowValues(row, dt.Columns, schemaCache);
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

                            var rowValues = BuildInsertRowValues(row, dt.Columns, schemaCache);
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

                    if (hasIdentity && IsSqlServerConnection(schemaCache))
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
        public static async Task<string> TableToInsertStatementAsync(ObjectName tableName, DBSchema schemaCache)
        {
            if (tableName.IsEmpty())
            {
                throw new ArgumentException("Invalid table name.");
            }

            if (schemaCache == null)
            {
                throw new ArgumentNullException(nameof(schemaCache));
            }

            var connection = schemaCache.Connection;
            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                throw new ArgumentException("Invalid schema cache connection.");
            }

            var sourceTableName = QuoteQualifiedName(tableName.FullNameNoQuote, schemaCache);
            var sql = $"select * from {sourceTableName}";

            var hasIdentity = TryGetIdentityFromSchemaCache(tableName, schemaCache)
                ?? (await schemaCache.GetIdentityColumnsAsync(tableName)).Count > 0;

            return await QueryDataToInsertStatementAsync(sql, schemaCache, tableName.FullNameNoQuote, hasIdentity);
        }

        private static bool? TryGetIdentityFromSchemaCache(ObjectName objectName, DBSchema schemaCache)
        {
            if (objectName == null || objectName.IsEmpty())
            {
                return null;
            }

            var cachedColumns = schemaCache.GetCachedColumns(objectName);
            if (cachedColumns.Count == 0)
            {
                return null;
            }

            // Identity information is not guaranteed in cached columns for all providers.
            // Use cache only when identity is explicitly encoded in data type metadata.
            return cachedColumns.Any(c => c.DataType?.Contains("identity", StringComparison.OrdinalIgnoreCase) == true)
                ? true
                : null;
        }

        private static bool? TryGetIdentityFromSchemaCache(string tableName, DBSchema schemaCache)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return null;
            }

            var objectName = new ObjectName
            {
                FullName = tableName,
                ObjectType = ObjectTypeEnums.Table
            };

            var identityFromTable = TryGetIdentityFromSchemaCache(objectName, schemaCache);
            if (identityFromTable.HasValue)
            {
                return identityFromTable;
            }

            objectName.ObjectType = ObjectTypeEnums.View;
            return TryGetIdentityFromSchemaCache(objectName, schemaCache);
        }

        /// <summary>
        /// Generates the SQL script to recreate all non-primary key, non-unique constraint indexes for the specified table.
        /// Handles XML indexes with the correct syntax.
        /// </summary>
        /// <returns>A string containing the SQL script to recreate the indexes.</returns>
        internal static async Task<string?> GetCreateIndexesScript(ObjectName objectName, DBSchema schemaCache)
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

            var dt = await schemaCache.GetCreateIndexesMetadataAsync(objectName);
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
        private static async Task<string?> GetCreateFunctionScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Function {objectName.FullName} ******/");

            AppendConditionalDropStatement(
                createScript,
                schemaCache,
                $"EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{objectName.FullName}') AND type IN ('FN','IF','TF'))",
                $"DROP FUNCTION {objectName.FullName}",
                $"DROP FUNCTION IF EXISTS {objectName.FullName}");

            var script = await GetDefinitionAsync(objectName, schemaCache);
            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine(GetBatchSeparator(schemaCache));
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create stored procedure script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateStoredProcedureScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Stored Procedure {objectName.FullName} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                AppendConditionalDropStatement(
                    createScript,
                    schemaCache,
                    $"OBJECT_ID(N'{objectName.FullName}', 'P') IS NOT NULL",
                    $"DROP PROCEDURE {objectName.FullName}",
                    $"DROP PROCEDURE IF EXISTS {objectName.FullName}");
            }

            var script = await GetDefinitionAsync(objectName, schemaCache);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine(GetBatchSeparator(schemaCache));
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create table script async.
        /// </summary>
        /// <param name="dbObject">The db object.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTableScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object: Table {objectName.FullName} ******/");

            if (!schemaCache.HasObject(objectName))
            {
                return string.Empty;
            }

            var columns = (await schemaCache.GetColumnsAsync(objectName))
                .OrderBy(c => int.TryParse(c.OrdinalPosition, out var position) ? position : int.MaxValue)
                .ThenBy(c => c.ColumnName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (columns.Count == 0)
            {
                return string.Empty;
            }

            // Add drop table statement
            if (Properties.Settings.Default.AddDropStatement)
            {
                AppendConditionalDropStatement(
                    createScript,
                    schemaCache,
                    $"OBJECT_ID(N'{objectName.FullName}', 'U') IS NOT NULL",
                    $"DROP TABLE {objectName.FullName}",
                    $"DROP TABLE IF EXISTS {objectName.FullName}");
            }

            // Get the primary key column names that the Ord ends with "🗝"
            var primaryKeyColumnNames = await schemaCache.GetPrimaryKeyColumnsAsync(objectName);
            string primaryKeyColumns = string.Join(", ", primaryKeyColumnNames.Select(c => $"[{c}]"));

            // Retrieve identity column details
            var identityColumns = await schemaCache.GetIdentityColumnsAsync(objectName);

            // Add the CREATE TABLE statement
            createScript.AppendLine($"CREATE TABLE {objectName.FullName} (");

            // Iterate through the rows in the DataGridView
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];

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
                if (i < columns.Count - 1)
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
            createScript.AppendLine(GetBatchSeparator(schemaCache));

            // add the foreign key constraints
            var foreignKeyConstraints = await BuildForeignKeyConstraintsScriptAsync(objectName, schemaCache);
            if (!string.IsNullOrEmpty(foreignKeyConstraints))
            {
                // remove the new line at the end of the script
                foreignKeyConstraints = foreignKeyConstraints.TrimEnd('\r', '\n');
                createScript.AppendLine(foreignKeyConstraints);
            }

            // add the check constraints
            var checkConstraints = await BuildCheckConstraintsScriptAsync(objectName, schemaCache);
            if (!string.IsNullOrEmpty(checkConstraints))
            {
                // remove the new line at the end of the script
                checkConstraints = checkConstraints.TrimEnd('\r', '\n');
                createScript.AppendLine(checkConstraints);
            }

            // add the default constraints
            var defaultConstraints = await BuildDefaultConstraintsScriptAsync(objectName, schemaCache);
            if (!string.IsNullOrEmpty(defaultConstraints))
            {
                // remove the new line at the end of the script
                defaultConstraints = defaultConstraints.TrimEnd('\r', '\n');
                createScript.AppendLine(defaultConstraints);
            }

            var indexScript = await GetCreateIndexesScript(objectName, schemaCache);
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createScript.AppendLine(indexScript);
            }

            var triggerScript = await GetCreateTriggersScriptAsync(objectName, schemaCache);
            if (!string.IsNullOrEmpty(triggerScript))
            {
                // if the createScript does not ended with a line of "GO", add it
                AddBatchSeparatorStatement(createScript, schemaCache);

                // remove the new line at the end of the script
                triggerScript = triggerScript.TrimEnd('\r', '\n');
                createScript.AppendLine(triggerScript);
            }

            // add GO statement
            AddBatchSeparatorStatement(createScript, schemaCache);

            return createScript.ToString();
        }

        private static async Task<string?> BuildCheckConstraintsScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            var dt = await schemaCache.GetCheckConstraintsAsync(objectName);
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            foreach (DataRow row in dt.Rows)
            {
                string constraintName = row["ConstraintName"]?.ToString() ?? string.Empty;
                string schema = row["SchemaName"]?.ToString() ?? string.Empty;
                string table = row["TableName"]?.ToString() ?? string.Empty;
                string definition = row["CheckDefinition"]?.ToString() ?? string.Empty;

                sb.AppendLine($"ALTER TABLE [{schema}].[{table}]");
                sb.AppendLine($"    ADD CONSTRAINT [{constraintName}] CHECK {definition};");
            }

            return sb.ToString();
        }

        private static async Task<string?> BuildDefaultConstraintsScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            var dt = await schemaCache.GetDefaultConstraintsAsync(objectName);
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            foreach (DataRow row in dt.Rows)
            {
                string constraintName = row["ConstraintName"]?.ToString() ?? string.Empty;
                string schema = row["SchemaName"]?.ToString() ?? string.Empty;
                string table = row["TableName"]?.ToString() ?? string.Empty;
                string column = row["ColumnName"]?.ToString() ?? string.Empty;
                string definition = row["DefaultDefinition"]?.ToString() ?? string.Empty;

                sb.AppendLine($"ALTER TABLE [{schema}].[{table}]");
                sb.AppendLine($"    ADD CONSTRAINT [{constraintName}] DEFAULT {definition} FOR [{column}];");
            }

            return sb.ToString();
        }

        private static async Task<string?> BuildForeignKeyConstraintsScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            var dt = await schemaCache.GetForeignKeyConstraintsAsync(objectName);
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            var fkDict = new Dictionary<string, (string Schema, string Table, string RefSchema, string RefTable, List<string> Columns, List<string> RefColumns, string OnDelete, string OnUpdate)>();

            foreach (DataRow row in dt.Rows)
            {
                string fkName = row["ForeignKeyName"]?.ToString() ?? string.Empty;
                string schema = row["SchemaName"]?.ToString() ?? string.Empty;
                string table = row["ParentTable"]?.ToString() ?? string.Empty;
                string parentColumn = row["ParentColumn"]?.ToString() ?? string.Empty;
                string refTable = row["ReferencedTable"]?.ToString() ?? string.Empty;
                string refColumn = row["ReferencedColumn"]?.ToString() ?? string.Empty;
                string refSchema = row["ReferencedSchema"]?.ToString() ?? string.Empty;
                string onDelete = row["OnDeleteAction"]?.ToString() ?? string.Empty;
                string onUpdate = row["OnUpdateAction"]?.ToString() ?? string.Empty;

                if (!fkDict.ContainsKey(fkName))
                {
                    fkDict[fkName] = (schema, table, refSchema, refTable, new List<string>(), new List<string>(), onDelete, onUpdate);
                }

                fkDict[fkName].Columns.Add(parentColumn);
                fkDict[fkName].RefColumns.Add(refColumn);
            }

            foreach (var kvp in fkDict)
            {
                var fkName = kvp.Key;
                var (schema, table, refSchema, refTable, columns, refColumns, onDelete, onUpdate) = kvp.Value;

                sb.AppendLine($"ALTER TABLE [{schema}].[{table}]");
                sb.AppendLine($"    ADD CONSTRAINT [{fkName}] FOREIGN KEY ({string.Join(", ", columns.ConvertAll(c => $"[{c}]"))})");
                sb.AppendLine($"        REFERENCES [{refSchema}].[{refTable}] ({string.Join(", ", refColumns.ConvertAll(c => $"[{c}]"))})" +
                    $"{(onDelete != "NO_ACTION" && !string.IsNullOrEmpty(onDelete) ? $" ON DELETE {onDelete.Replace("_", " ")}" : "")}" +
                    $"{(onUpdate != "NO_ACTION" && !string.IsNullOrEmpty(onUpdate) ? $" ON UPDATE {onUpdate.Replace("_", " ")}" : "")};");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds the DBMS batch separator to the script if the script does not already end with it.
        /// </summary>
        /// <param name="sb">The sb.</param>
        private static void AddBatchSeparatorStatement(StringBuilder sb, DBSchema schemaCache)
        {
            var batchSeparator = GetBatchSeparator(schemaCache);
            if (!sb.ToString().EndsWith(Environment.NewLine + batchSeparator + Environment.NewLine, StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine(batchSeparator);
            }
        }

        private static string GetBatchSeparator(DBSchema schemaCache)
        {
            bool isSqlServer = IsSqlServerConnection(schemaCache);

            return isSqlServer ? "GO" : "";
        }

        private static bool IsSqlServerConnection(DBSchema schemaCache)
        {
            var connection = schemaCache.Connection;
            if (connection == null)
            {
                return true;
            }

            return connection.DBMSType == DBMSTypeEnums.SQLServer;
        }

        private static bool IsPostgreSqlConnection(DBSchema schemaCache)
        {
            var connection = schemaCache.Connection;
            if (connection == null)
            {
                return false;
            }

            return connection.DBMSType == DBMSTypeEnums.PostgreSQL;
        }

        private static bool IsOracleConnection(DBSchema schemaCache)
        {
            var connection = schemaCache.Connection;
            if (connection == null)
            {
                return false;
            }

            return connection.DBMSType == DBMSTypeEnums.Oracle;
        }

        private static string GetIdentityInsertOverrideClause(DBSchema schemaCache, bool hasIdentity)
        {
            if (!hasIdentity)
            {
                return string.Empty;
            }

            if (IsPostgreSqlConnection(schemaCache) || IsOracleConnection(schemaCache))
            {
                return " OVERRIDING SYSTEM VALUE";
            }

            return string.Empty;
        }

        private static string BuildInsertRowValues(DataRow row, DataColumnCollection columns, DBSchema schemaCache)
        {
            var values = new List<string>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                values.Add(FormatInsertValue(row[i], columns[i].DataType, schemaCache));
            }

            return string.Join(", ", values);
        }

        private static string FormatInsertValue(object value, Type type, DBSchema schemaCache)
        {
            if (value == DBNull.Value)
            {
                return "NULL";
            }

            if (type == typeof(string))
            {
                return BuildStringLiteral(value.ToString(), schemaCache);
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
                if (IsPostgreSqlConnection(schemaCache))
                {
                    return (bool)value ? "TRUE" : "FALSE";
                }

                return (bool)value ? "1" : "0";
            }

            return BuildStringLiteral(value.ToString(), schemaCache);
        }

        private static string BuildStringLiteral(string? value, DBSchema schemaCache)
        {
            var escapedValue = (value ?? string.Empty).Replace("'", "''");
            var prefix = IsSqlServerConnection(schemaCache) ? "N" : string.Empty;
            return $"{prefix}'{escapedValue}'";
        }

        private static string QuoteQualifiedName(string name, DBSchema schemaCache)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return string.Join(".", name.Split('.').Select(part => QuoteIdentifier(part, schemaCache)));
        }

        private static string QuoteIdentifier(string identifier, DBSchema schemaCache)
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

            var connection = schemaCache.Connection;

            if (IsSqlServerConnection(schemaCache))
            {
                return $"[{value.Replace("]", "]]" )}]";
            }

            if (connection != null && (connection.DBMSType == DBMSTypeEnums.MySQL || connection.DBMSType == DBMSTypeEnums.MariaDB))
            {
                return $"`{value.Replace("`", "``")}`";
            }

            if (IsPostgreSqlConnection(schemaCache) || IsOracleConnection(schemaCache))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return $"[{value.Replace("]", "]]" )}]";
        }

        private static void AppendConditionalDropStatement(
            StringBuilder script,
            DBSchema schemaCache,
            string sqlServerCondition,
            string sqlServerDropStatement,
            string nonSqlServerDropStatement)
        {
            if (IsSqlServerConnection(schemaCache))
            {
                script.AppendLine($"IF {sqlServerCondition}");
                script.AppendLine($"\t{sqlServerDropStatement};");
            }
            else
            {
                script.AppendLine($"{nonSqlServerDropStatement};");
            }

            script.AppendLine(GetBatchSeparator(schemaCache));
        }

        /// <summary>
        /// Gets the create trigger script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTriggerScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  Trigger {objectName.Name} ******/");

            if (Properties.Settings.Default.AddDropStatement)
            {
                AppendConditionalDropStatement(
                    createScript,
                    schemaCache,
                    $"OBJECT_ID(N'{objectName.FullName}', 'TR') IS NOT NULL",
                    $"DROP TRIGGER {objectName.FullName}",
                    $"DROP TRIGGER IF EXISTS {objectName.FullName}");
            }

            var script = await schemaCache.GetObjectDefinitionAsync(objectName);

            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }

            createScript.Append(script);
            createScript.AppendLine(GetBatchSeparator(schemaCache));
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the create triggers script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateTriggersScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            // Only tables and views can have DML triggers
            if (objectName.IsEmpty() ||
                (objectName.ObjectType != ObjectName.ObjectTypeEnums.Table &&
                 objectName.ObjectType != ObjectName.ObjectTypeEnums.View))
                return string.Empty;

            StringBuilder sb = new();

            var allTriggers = schemaCache.Objects(ObjectName.ObjectTypeEnums.Trigger);
            if (allTriggers.Count == 0)
                return string.Empty;

            foreach (var triggerObject in allTriggers)
            {
                var triggerInfo = await schemaCache.GetTriggerInfoAsync(triggerObject);
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
                string definition = await schemaCache.GetObjectDefinitionAsync(triggerObject);
                bool isDisabled = string.Equals(infoRow["IsDisabled"]?.ToString(), "Yes", StringComparison.OrdinalIgnoreCase);

                // Add header
                sb.AppendLine($"/****** Object:  Trigger [{schema}].[{triggerName}] on [{schema}].[{parentName}] ******/");

                // Add drop statement if configured
                if (Properties.Settings.Default.AddDropStatement)
                {
                    AppendConditionalDropStatement(
                        sb,
                        schemaCache,
                        $"OBJECT_ID(N'[{schema}].[{triggerName}]', 'TR') IS NOT NULL",
                        $"DROP TRIGGER [{schema}].[{triggerName}]",
                        $"DROP TRIGGER IF EXISTS [{schema}].[{triggerName}]");
                }

                // Add the trigger definition
                sb.Append(definition.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine);
                sb.AppendLine(GetBatchSeparator(schemaCache));

                // Optionally, add DISABLE TRIGGER if the trigger is disabled
                if (isDisabled)
                {
                    sb.AppendLine($"ALTER TABLE [{schema}].[{parentName}] DISABLE TRIGGER [{triggerName}];");
                    sb.AppendLine(GetBatchSeparator(schemaCache));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the create view script async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetCreateViewScriptAsync(ObjectName objectName, DBSchema schemaCache)
        {
            StringBuilder createScript = new();

            // Add the header
            createScript.AppendLine($"/****** Object:  View {objectName.FullName} ******/");

            // Add drop table statement
            if (Properties.Settings.Default.AddDropStatement)
            {
                AppendConditionalDropStatement(
                    createScript,
                    schemaCache,
                    $"OBJECT_ID(N'{objectName.FullName}', 'V') IS NOT NULL",
                    $"DROP VIEW {objectName.FullName}",
                    $"DROP VIEW IF EXISTS {objectName.FullName}");
            }

            var script = await GetDefinitionAsync(objectName, schemaCache);
            if (string.IsNullOrEmpty(script))
            {
                return string.Empty;
            }
            createScript.Append(script);

            var indexScript = await DatabaseDocBuilder.GetCreateIndexesScript(objectName, schemaCache);
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createScript.AppendLine(indexScript);
            }

            createScript.AppendLine(GetBatchSeparator(schemaCache));
            return createScript.ToString();
        }

        /// <summary>
        /// Gets the stored procedure definition async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string?> GetDefinitionAsync(ObjectName objectName, DBSchema schemaCache)
        {
            // Validate the single full name parameter
            if (!string.IsNullOrWhiteSpace(objectName.FullName))
            {
            string? definition = await schemaCache.GetObjectDefinitionAsync(objectName);

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