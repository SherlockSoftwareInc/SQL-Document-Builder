using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The DB object.
    /// </summary>
    internal class DBObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DBObject"/> class.
        /// </summary>
        public DBObject()
        {
            ObjectName = new ObjectName();
        }

        /// <summary>
        /// Gets or sets columns
        /// </summary>
        public List<DBColumn> Columns { get; set; } = [];

        /// <summary>
        /// Gets or set description for the object
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets object name
        /// </summary>
        public ObjectName ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the primary key columns.
        /// </summary>
        public string PrimaryKeyColumns { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets name of the object
        /// </summary>
        public string TableName { get => ObjectName.Name; set => ObjectName.Name = value; }

        /// <summary>
        /// Gets or sets schema of the object
        /// </summary>
        public string TableSchema { get => ObjectName.Schema; set => ObjectName.Schema = value; }

        //public string TableCatalog { get => ObjectName.Catealog; set => ObjectName.Catealog = value; }

        /// <summary>
        /// Gets or set object type
        /// </summary>
        public ObjectTypeEnums TableType { get => ObjectName.ObjectType; set => ObjectName.ObjectType = value; }

        /// <summary>
        /// Gets or sets database connection string
        /// </summary>
        private string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets the list of views where the specified table is used.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="tableName">The name of the table to check.</param>
        /// <param name="schemaName">The schema name of the table.</param>
        /// <returns>A list of tuples containing the view name and schema name.</returns>
        public static async Task<List<(string ViewName, string SchemaName)>> GetViewsUsingTableAsync(string? schemaName, string? tableName)
        {
            var views = new List<(string ViewName, string SchemaName)>();
            if (string.IsNullOrEmpty(tableName))
            {
                return views;
            }

            if (string.IsNullOrEmpty(schemaName))
                schemaName = "dbo";

            string query = @"
SELECT
    v.name AS ViewName,
    s.name AS SchemaName
FROM
    sys.views v
    INNER JOIN sys.sql_expression_dependencies d ON v.object_id = d.referencing_id
    INNER JOIN sys.objects o ON d.referenced_id = o.object_id
    INNER JOIN sys.schemas s ON v.schema_id = s.schema_id
WHERE
    o.name = @TableName AND
    s.name = @SchemaName;";

            using (var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.Parameters.AddWithValue("@SchemaName", schemaName);

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        string viewName = reader.GetString(0);
                        string schema = reader.GetString(1);
                        views.Add((viewName, schema));
                    }
                }
                catch (Exception ex)
                {
                    Common.MsgBox($"Error retrieving views: {ex.Message}", MessageBoxIcon.Error);
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return views;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A DBColumn? .</returns>
        public DBColumn? GetColumn(string columnName)
        {
            foreach (var col in Columns)
            {
                if (col.ColumnName == columnName)
                {
                    return col;
                }
            }
            return null;
        }

        /// <summary>
        /// Generates the SQL script to recreate all non-primary key, non-unique constraint indexes for the specified table.
        /// </summary>
        /// <param name="fullTableName">The full table name in the format [Schema].[TableName].</param>
        /// <returns>A string containing the SQL script to recreate the indexes.</returns>
        public string GetCreateIndexesScript()
        {
            if (ObjectName.IsEmpty())
                return string.Empty;

            StringBuilder indexScript = new();

            // Fix: Use CROSS APPLY for included columns and avoid string concatenation with NULLs
            string query = $@"
SELECT
    'CREATE '
    + CASE WHEN i.is_unique = 1 THEN 'UNIQUE ' ELSE '' END
    + i.type_desc COLLATE DATABASE_DEFAULT
    + ' INDEX ' + QUOTENAME(i.name)
    + ' ON ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name)
    + ' (' + ISNULL(key_columns.key_cols, '') + ')'
    + CASE
        WHEN include_columns.inc_cols IS NOT NULL AND include_columns.inc_cols <> ''
          THEN ' INCLUDE (' + include_columns.inc_cols + ')'
        ELSE ''
      END
    + CASE
        WHEN i.filter_definition IS NOT NULL
             AND i.filter_definition <> ''
          THEN ' WHERE ' + i.filter_definition
        ELSE ''
      END
    + ';'
FROM sys.indexes i
JOIN sys.tables t
  ON i.object_id = t.object_id
JOIN sys.schemas s
  ON t.schema_id = s.schema_id
-- Generate the key column list
CROSS APPLY (
    SELECT
      key_cols = STUFF((
        SELECT
          ', '
          + QUOTENAME(c.name)
          + CASE WHEN ic2.is_descending_key = 1 THEN ' DESC' ELSE ' ASC' END
        FROM sys.index_columns ic2
        JOIN sys.columns      c
          ON ic2.object_id = c.object_id
         AND ic2.column_id = c.column_id
        WHERE ic2.object_id = i.object_id
          AND ic2.index_id  = i.index_id
          AND ic2.is_included_column = 0
        ORDER BY ic2.key_ordinal
        FOR XML PATH(''), TYPE
      ).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
) AS key_columns
-- Generate the INCLUDE column list
OUTER APPLY (
    SELECT
      inc_cols = STUFF((
        SELECT
          ', ' + QUOTENAME(c.name)
        FROM sys.index_columns ic3
        JOIN sys.columns      c
          ON ic3.object_id = c.object_id
         AND ic3.column_id = c.column_id
        WHERE ic3.object_id = i.object_id
          AND ic3.index_id  = i.index_id
          AND ic3.is_included_column = 1
        ORDER BY ic3.index_column_id
        FOR XML PATH(''), TYPE
      ).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
) AS include_columns
WHERE
    i.object_id = OBJECT_ID(N'{ObjectName.FullName}')
    AND i.is_primary_key       = 0
    AND i.is_unique_constraint = 0
    AND i.is_hypothetical      = 0
    AND i.index_id             > 0
ORDER BY
    i.index_id;";

            //System.Diagnostics.Debug.Print(query);

            using (SqlConnection connection = new(ConnectionString))
            {
                SqlCommand command = new(query, connection);
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    indexScript.AppendLine(reader.GetString(0));
                }
            }

            return indexScript.ToString();
        }

        /// <summary>
        /// Retrieves identity column details for the current table.
        /// </summary>
        /// <returns>A dictionary where the key is the column name and the value is a tuple containing seed and increment values.</returns>
        public Dictionary<string, (int SeedValue, int IncrementValue)> GetIdentityColumns()
        {
            var identityColumns = new Dictionary<string, (int SeedValue, int IncrementValue)>();

            string identityQuery = $@"
SELECT
    ic.name AS identity_column_name,
    ic.seed_value,
    ic.increment_value
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = '{TableName}'
AND s.name = '{TableSchema}';";

            using (var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString))
            {
                connection.Open();
                using var command = new SqlCommand(identityQuery, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string columnName = reader.GetString(0);
                    object? seedValueObj = reader.GetValue(1);
                    object? incrementValueObj = reader.GetValue(2);
                    int seedValue = seedValueObj != DBNull.Value ? Convert.ToInt32(seedValueObj) : 0;
                    int incrementValue = incrementValueObj != DBNull.Value ? Convert.ToInt32(incrementValueObj) : 0;
                    identityColumns[columnName] = (seedValue, incrementValue);
                }
            }

            return identityColumns;
        }

        /// <summary>
        /// Updates the column description.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="newDescription">The new description.</param>
        public async Task UpdateColumnDescriptionAsync(string columnName, string newDescription, bool isView)
        {
            if (ConnectionString.Length == 0 || newDescription.Length == 0) return;

            DBColumn? column = GetColumn(columnName);

            if (column != null)
            {
                column.Description = newDescription;
                string newDesc = newDescription.Replace("'", "''");
                string tableType = isView ? "VIEW" : "TABLE";

                using SqlConnection connection = new(ConnectionString);

                try
                {
                    await connection.OpenAsync();

                    string query = $@"
IF EXISTS (
    SELECT 1
    FROM sys.extended_properties AS ep
    JOIN sys.objects AS o ON ep.major_id = o.object_id
    WHERE o.name = '{ObjectName.FullName}'
        AND ep.name = 'MS_Description'
        AND ep.minor_id = (
            SELECT column_id
            FROM sys.columns
            WHERE object_id = o.object_id
                AND name = '{columnName}'
        )
)
BEGIN
    EXEC sys.sp_updateextendedproperty
        @name = N'MS_Description',
        @value = '{newDesc}',
        @level0type = N'SCHEMA',
        @level0name = '{ObjectName.Schema}',
        @level1type = N'{tableType}',
        @level1name = '{ObjectName.Name}',
        @level2type = N'COLUMN',
        @level2name = '{columnName}';
END
ELSE
BEGIN
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = '{newDesc}',
        @level0type = N'SCHEMA',
        @level0name = '{ObjectName.Schema}',
        @level1type = N'{tableType}',
        @level1name = '{ObjectName.Name}',
        @level2type = N'COLUMN',
        @level2name = '{columnName}';
END";

                    await using SqlCommand command = new(query, connection);
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

        /// <summary>
        /// Updates the table desc.
        /// </summary>
        /// <param name="newDescription">The description.</param>
        public async Task UpdateTableDescAsync(string newDescription)
        {
            Description = newDescription;

            if (!ObjectName.IsEmpty() && newDescription.Length > 0)
            {
                string newDesc = newDescription.Replace("'", "''");
                string sql = $@"
DECLARE @TableName varchar(100) = '{ObjectName.FullName}';
DECLARE @Schema varchar(100) = OBJECT_SCHEMA_NAME(OBJECT_ID(@TableName));
DECLARE @ObjectName varchar(100) = OBJECT_NAME(OBJECT_ID(@TableName));
DECLARE @ObjectType varchar(100);
SELECT @ObjectType = CASE type_desc WHEN 'USER_TABLE' THEN 'TABLE' ELSE 'VIEW' END
FROM sys.objects
WHERE object_id = OBJECT_ID(@TableName);

IF EXISTS (SELECT value
	    FROM sys.extended_properties
	    WHERE class = 1 AND major_id = OBJECT_ID(@TableName)
	    AND minor_id = 0
	    AND name = 'MS_Description')
	EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'{newDesc}',
		@level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
		@level1type = @ObjectType, @level1name = '{ObjectName.Name}';
ELSE
	EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'{newDesc}',
		@level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
		@level1type = @ObjectType, @level1name = '{ObjectName.Name}';";

                var result = await DatabaseHelper.ExecuteSQLAsync(sql);
                if (result != string.Empty)
                {
                    Common.MsgBox("Failed to update table description." + Environment.NewLine + result, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Gets the check constraints script.
        /// </summary>
        /// <returns>A Task.</returns>
        internal async Task<string?> GetCheckConstraintsScript()
        {
            if (ObjectName.IsEmpty())
                return null;

            StringBuilder sb = new();
            string sql = $@"
SELECT
    cc.name AS ConstraintName,
    s.name AS SchemaName,
    t.name AS TableName,
    cc.definition AS CheckDefinition
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{TableSchema}' AND t.name = '{TableName}'
ORDER BY cc.name;
";

            var dt = await DatabaseHelper.GetDataTableAsync(sql);
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            foreach (System.Data.DataRow row in dt.Rows)
            {
                string constraintName = row["ConstraintName"].ToString() ?? "";
                string schema = row["SchemaName"].ToString() ?? "";
                string table = row["TableName"].ToString() ?? "";
                string definition = row["CheckDefinition"].ToString() ?? "";

                sb.AppendLine($"ALTER TABLE [{schema}].[{table}]");
                sb.AppendLine($"    ADD CONSTRAINT [{constraintName}] CHECK {definition};");
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the default constraints script.
        /// </summary>
        /// <returns>A Task.</returns>
        internal async Task<string?> GetDefaultConstraintsScript()
        {
            if (ObjectName.IsEmpty())
                return null;

            StringBuilder sb = new();
            string sql = $@"
SELECT
    dc.name AS ConstraintName,
    s.name AS SchemaName,
    t.name AS TableName,
    c.name AS ColumnName,
    dc.definition AS DefaultDefinition
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{TableSchema}' AND t.name = '{TableName}'
ORDER BY dc.name;
";

            var dt = await DatabaseHelper.GetDataTableAsync(sql);
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            foreach (System.Data.DataRow row in dt.Rows)
            {
                string constraintName = row["ConstraintName"].ToString() ?? "";
                string schema = row["SchemaName"].ToString() ?? "";
                string table = row["TableName"].ToString() ?? "";
                string column = row["ColumnName"].ToString() ?? "";
                string definition = row["DefaultDefinition"].ToString() ?? "";

                sb.AppendLine($"ALTER TABLE [{schema}].[{table}]");
                sb.AppendLine($"    ADD CONSTRAINT [{constraintName}] DEFAULT {definition} FOR [{column}];");
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the foreign key constraints script.
        /// </summary>
        /// <returns>A Task.</returns>
        internal async Task<string?> GetForeignKeyConstraintsScript()
        {
            if (ObjectName.IsEmpty())
                return null;

            StringBuilder sb = new();
            string sql = $@"
SELECT
    fk.name AS ForeignKeyName,
    s.name AS SchemaName,
    tp.name AS ParentTable,
    fkc.constraint_column_id AS ColumnOrder,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn,
    rs.name AS ReferencedSchema,
    fk.delete_referential_action_desc AS OnDeleteAction,
    fk.update_referential_action_desc AS OnUpdateAction
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.schemas s ON tp.schema_id = s.schema_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND cp.object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.schemas rs ON tr.schema_id = rs.schema_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND cr.object_id = tr.object_id
WHERE s.name = '{TableSchema}' AND tp.name = '{TableName}'
ORDER BY fk.name, fkc.constraint_column_id;
";

            // Use DatabaseHelper to get the data as a DataTable
            var dt = await DatabaseHelper.GetDataTableAsync(sql);
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            // Group by FK name for multi-column FKs
            var fkDict = new Dictionary<string, (string Schema, string Table, string RefSchema, string RefTable, List<string> Columns, List<string> RefColumns, string OnDelete, string OnUpdate)>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                string fkName = row["ForeignKeyName"].ToString() ?? "";
                string schema = row["SchemaName"].ToString() ?? "";
                string table = row["ParentTable"].ToString() ?? "";
                string parentColumn = row["ParentColumn"].ToString() ?? "";
                string refTable = row["ReferencedTable"].ToString() ?? "";
                string refColumn = row["ReferencedColumn"].ToString() ?? "";
                string refSchema = row["ReferencedSchema"].ToString() ?? "";
                string onDelete = row["OnDeleteAction"]?.ToString() ?? "";
                string onUpdate = row["OnUpdateAction"]?.ToString() ?? "";

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
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Opens the data object.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        internal async Task<bool> OpenAsync(ObjectName objectName, string connectionString)
        {
            this.ObjectName = objectName;

            var objectType = objectName.ObjectType;
            if (objectType == ObjectTypeEnums.None)
            {
                // determine the object type
                var sql = $"SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{objectName.Schema}' AND TABLE_NAME = '{objectName.Name}'";
                using SqlConnection conn = new(connectionString);
                try
                {
                    await conn.OpenAsync();
                    await using var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };

                    string? objType = (string)cmd.ExecuteScalar();
                    if (objType != null)
                    {
                        objectType = objType.ToString().Equals("view", StringComparison.CurrentCultureIgnoreCase) ? ObjectTypeEnums.View : ObjectTypeEnums.Table;
                    }
                }
                catch (SqlException)
                {
                    //return;
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
            return await OpenAsync(objectName.Schema, objectName.Name, objectType, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="objectType">The object type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        internal async Task<bool> OpenAsync(string schemaName, string tableName, ObjectTypeEnums objectType, string connectionString)
        {
            bool result = false;

            TableName = tableName ?? string.Empty;
            TableSchema = schemaName ?? string.Empty;
            TableType = objectType;

            ConnectionString = connectionString ?? string.Empty;
            Columns.Clear();

            if (ConnectionString.Length > 0 && TableName.Length > 0)
            {
                using SqlConnection conn = new(ConnectionString);
                try
                {
                    await conn.OpenAsync();
                    await using var cmd = new SqlCommand()
                    {
                        CommandText = "SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,IS_NULLABLE,COLUMN_DEFAULT FROM information_schema.columns WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION",
                        Connection = conn,
                        CommandType = System.Data.CommandType.Text
                    };
                    cmd.Parameters.Add(new SqlParameter("@Schema", schemaName));
                    cmd.Parameters.Add(new SqlParameter("@TableName", tableName));

                    SqlDataReader dr = await cmd.ExecuteReaderAsync();
                    while (await dr.ReadAsync())
                    {
                        Columns.Add(new DBColumn(dr));
                    }
                    dr.Close();

                    if (objectType == ObjectTypeEnums.Table)
                    {
                        await GetPrimaryKeysAsync(TableSchema, TableName);

                        await GetIndexesAsync(TableSchema, TableName);
                    }

                    result = true;
                }
                catch (SqlException)
                {
                    //return;
                }
                finally
                {
                    await conn.CloseAsync();
                }
                Description = await GetTableDescAsync();
                await GetColumnDescAsync();
            }

            return result;
        }

        /// <summary>
        /// Gets the column desc.
        /// </summary>
        private async Task GetColumnDescAsync()
        {
            var conn = new SqlConnection(ConnectionString);
            try
            {
                await using var cmd = new SqlCommand()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = $"SELECT C.Name, E.value Description FROM sys.schemas S INNER JOIN sys.{(TableType == ObjectTypeEnums.Table ? "tables" : "views")} T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id WHERE E.name = N'MS_Description' AND S.name = @Schema AND T.name = @TableName",
                };
                cmd.Parameters.Add(new SqlParameter("@Schema", TableSchema));
                cmd.Parameters.Add(new SqlParameter("@TableName", TableName));
                await conn.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    if (dr[1] != DBNull.Value)
                    {
                        var columnName = dr.GetString(0);
                        var column = GetColumn(columnName);
                        if (column != null)
                        {
                            column.Description = dr.GetString(1);
                        }
                    }
                }
                dr.Close();
            }
            catch (Exception)
            {
                // ignore the error
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        private async Task GetIndexesAsync(string tableSchema, string tableName)
        {
            var sql = $@"SELECT distinct col.name AS ColumnName
FROM sys.indexes ind
INNER JOIN sys.index_columns ic ON ind.object_id = ic.object_id AND ind.index_id = ic.index_id
INNER JOIN sys.columns col ON ic.object_id = col.object_id AND ic.column_id = col.column_id
INNER JOIN sys.tables t ON ind.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{tableSchema}'
AND t.name = '{tableName}'";

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                await using var cmd = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = sql,
                    CommandType = CommandType.Text
                };
                await conn.OpenAsync();
                var dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    string? columnName = dr.GetString(0);
                    // find the column in the Columns
                    foreach (var column in Columns)
                    {
                        if (column.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (column.ColID.EndsWith("🗝") == false)
                                column.ColID += "🔢";
                        }
                    }
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
        }

        /// <summary>
        /// Primaries the keys.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <returns>A list of string.</returns>
        private async Task GetPrimaryKeysAsync(string schemaName, string tableName)
        {
            PrimaryKeyColumns = string.Empty;

            var sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{schemaName}'";
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                await using var cmd = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = sql,
                    CommandType = CommandType.Text
                };
                await conn.OpenAsync();
                var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    var columnName = dr.GetString(0);
                    // find the column in the Columns
                    foreach (var column in Columns)
                    {
                        if (column.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            column.ColID += "🗝";

                            if (PrimaryKeyColumns.Length > 0)
                                PrimaryKeyColumns += ", ";
                            PrimaryKeyColumns += $"[{column.ColumnName}]";

                            break;
                        }
                    }
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
        }

        /// <summary>
        /// Gets the table desc.
        /// </summary>
        /// <returns>A string.</returns>
        private async Task<string> GetTableDescAsync()
        {
            string result = string.Empty;
            if (ObjectName != null)
            {
                string sql = string.Format(String.Format("SELECT value FROM fn_listextendedproperty (NULL, 'schema', '{0}', '{2}', '{1}', default, default) WHERE name = N'MS_Description'", ObjectName.Schema, ObjectName.Name, (ObjectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")));

                var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                try
                {
                    await using var cmd = new SqlCommand()
                    {
                        Connection = conn,
                        CommandText = sql,
                        CommandType = CommandType.Text
                    };
                    await conn.OpenAsync();
                    var dr = await cmd.ExecuteReaderAsync();
                    if (await dr.ReadAsync())
                    {
                        if (dr[0] != DBNull.Value)
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
            }

            return result;
        }
    }
}