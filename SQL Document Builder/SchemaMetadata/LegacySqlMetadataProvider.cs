using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder.SchemaMetadata
{
    internal sealed class LegacySqlMetadataProvider : ISchemaMetadataProvider
    {
        public async Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums objectType, string connectionString, CancellationToken cancellationToken = default)
        {
            return objectType switch
            {
                ObjectTypeEnums.Table => await GetTablesAsync(connectionString, cancellationToken),
                ObjectTypeEnums.View => await GetViewsAsync(connectionString, cancellationToken),
                ObjectTypeEnums.StoredProcedure => await GetStoredProceduresAsync(connectionString, cancellationToken),
                ObjectTypeEnums.Function => await GetFunctionsAsync(connectionString, cancellationToken),
                ObjectTypeEnums.Trigger => await GetTriggersAsync(connectionString, cancellationToken),
                ObjectTypeEnums.Synonym => await GetSynonymsAsync(connectionString, cancellationToken),
                ObjectTypeEnums.All => await GetAllObjectsAsync(connectionString, cancellationToken),
                _ => []
            };
        }

        public async Task<List<ObjectName>> GetAllObjectsAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var objects = new List<ObjectName>();
            objects.AddRange(await GetTablesAsync(connectionString, cancellationToken));
            objects.AddRange(await GetViewsAsync(connectionString, cancellationToken));
            objects.AddRange(await GetStoredProceduresAsync(connectionString, cancellationToken));
            objects.AddRange(await GetFunctionsAsync(connectionString, cancellationToken));
            objects.AddRange(await GetTriggersAsync(connectionString, cancellationToken));
            objects.AddRange(await GetSynonymsAsync(connectionString, cancellationToken));
            return objects;
        }

        public async Task<List<string>> GetSchemasAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT name FROM sys.schemas ORDER BY name;";

            var schemas = new List<string>();

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var schema = reader["name"]?.ToString();
                if (!string.IsNullOrWhiteSpace(schema))
                {
                    schemas.Add(schema);
                }
            }

            return schemas;
        }

        public async Task<string> GetObjectDefinitionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT OBJECT_DEFINITION(OBJECT_ID(QUOTENAME(@SchemaName) + '.' + QUOTENAME(@ObjectName)));";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            command.Parameters.AddWithValue("@SchemaName", objectName.Schema);
            command.Parameters.AddWithValue("@ObjectName", objectName.Name);

            await connection.OpenAsync(cancellationToken);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result == null || result == DBNull.Value ? string.Empty : result.ToString() ?? string.Empty;
        }

        public async Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT CAST(ep.value AS NVARCHAR(MAX))
FROM sys.extended_properties ep
INNER JOIN sys.objects o ON ep.major_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE ep.name = N'MS_Description'
  AND ep.minor_id = 0
  AND s.name = @SchemaName
  AND o.name = @ObjectName;";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            command.Parameters.AddWithValue("@SchemaName", objectName.Schema);
            command.Parameters.AddWithValue("@ObjectName", objectName.Name);

            await connection.OpenAsync(cancellationToken);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result == null || result == DBNull.Value ? string.Empty : result.ToString() ?? string.Empty;
        }

        public async Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default)
        {
            var level2Type = objectName.ObjectType == ObjectTypeEnums.StoredProcedure || objectName.ObjectType == ObjectTypeEnums.Function
                ? "PARAMETER"
                : "COLUMN";

            var level1Type = ToLevel1Type(objectName.ObjectType);

            const string sql = @"
SELECT CAST(value AS NVARCHAR(MAX))
FROM fn_listextendedproperty(
    'MS_Description',
    'SCHEMA', @SchemaName,
    @Level1Type, @ObjectName,
    @Level2Type, @Level2Name);";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            command.Parameters.AddWithValue("@SchemaName", objectName.Schema);
            command.Parameters.AddWithValue("@Level1Type", level1Type);
            command.Parameters.AddWithValue("@ObjectName", objectName.Name);
            command.Parameters.AddWithValue("@Level2Type", level2Type);
            command.Parameters.AddWithValue("@Level2Name", columnName);

            await connection.OpenAsync(cancellationToken);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result == null || result == DBNull.Value ? string.Empty : result.ToString() ?? string.Empty;
        }

        public async Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT
    COALESCE(d.referenced_schema_name, s.name) AS [Schema],
    d.referenced_entity_name AS [ObjectName],
    COALESCE(o.type_desc, 'UNKNOWN') AS [ObjectType]
FROM sys.sql_expression_dependencies d
LEFT JOIN sys.objects o ON d.referenced_id = o.object_id
LEFT JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE d.referencing_id = OBJECT_ID(QUOTENAME(@SchemaName) + '.' + QUOTENAME(@ObjectName));";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetObjectColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, DATETIME_PRECISION, IS_NULLABLE, COLUMN_DEFAULT
FROM Information_schema.columns
WHERE TABLE_SCHEMA = @SchemaName AND TABLE_NAME = @ObjectName
ORDER BY ORDINAL_POSITION";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetObjectParametersAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT
    p.ORDINAL_POSITION,
    p.PARAMETER_NAME,
    p.DATA_TYPE,
    p.CHARACTER_MAXIMUM_LENGTH,
    p.PARAMETER_MODE
FROM Information_SCHEMA.PARAMETERS p
WHERE p.SPECIFIC_SCHEMA = @SchemaName
  AND p.SPECIFIC_NAME = @ObjectName
  AND p.ORDINAL_POSITION > 0
ORDER BY p.ORDINAL_POSITION";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT
    s.name AS [Schema],
    o.name AS [ObjectName],
    o.type_desc AS [ObjectType]
FROM sys.sql_expression_dependencies d
INNER JOIN sys.objects o ON d.referencing_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE d.referenced_schema_name = @SchemaName
  AND d.referenced_entity_name = @ObjectName;";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT
    cp.name AS [FromColumn],
    rs.name AS [ToSchema],
    tr.name AS [ToTable],
    cr.name AS [ToColumn]
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.schemas s ON tp.schema_id = s.schema_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND cp.object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.schemas rs ON tr.schema_id = rs.schema_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND cr.object_id = tr.object_id
WHERE s.name = @SchemaName
  AND tp.name = @ObjectName
ORDER BY fk.name, fkc.constraint_column_id;";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<List<string>> GetPrimaryKeyColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT COLUMN_NAME
FROM Information_SCHEMA.KEY_COLUMN_USAGE
WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
  AND TABLE_NAME = @ObjectName
  AND TABLE_SCHEMA = @SchemaName";

            var table = await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@ObjectName", objectName.Name),
                new SqlParameter("@SchemaName", objectName.Schema));

            if (table.Rows.Count == 0)
            {
                return [];
            }

            return table.AsEnumerable()
                .Select(r => r["COLUMN_NAME"]?.ToString())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v!)
                .ToList();
        }

        public async Task<DataTable?> GetObjectIndexesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT
    ind.name AS IndexName,
    ind.type_desc AS Type,
    col.name AS ColumnName,
    ind.is_unique AS IsUnique
FROM sys.indexes ind
INNER JOIN sys.index_columns ic ON ind.object_id = ic.object_id AND ind.index_id = ic.index_id
INNER JOIN sys.columns col ON ic.object_id = col.object_id AND ic.column_id = col.column_id
INNER JOIN sys.tables t ON ind.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = @SchemaName
  AND t.name = @ObjectName
  AND ind.is_primary_key = 0
ORDER BY ind.name, ic.key_ordinal";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetObjectConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"
SELECT
    kc.name AS ConstraintName,
    kc.type_desc AS ConstraintType,
    c.name AS ColumnName
FROM sys.key_constraints kc
INNER JOIN sys.tables t ON kc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
LEFT JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
LEFT JOIN sys.columns c ON c.object_id = t.object_id AND c.column_id = ic.column_id
WHERE s.name = @SchemaName
  AND t.name = @ObjectName
UNION
SELECT
    fk.name AS ConstraintName,
    'FOREIGN_KEY_CONSTRAINT' AS ConstraintType,
    c.name AS ColumnName
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
WHERE s.name = @SchemaName
  AND t.name = @ObjectName
UNION
SELECT
    cc.name AS ConstraintName,
    'CHECK_CONSTRAINT' AS ConstraintType,
    c.name AS ColumnName
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
LEFT JOIN sys.columns c ON cc.parent_object_id = c.object_id AND cc.parent_column_id = c.column_id
WHERE s.name = @SchemaName
  AND t.name = @ObjectName
UNION
SELECT
    dc.name AS ConstraintName,
    'DEFAULT_CONSTRAINT' AS ConstraintType,
    c.name AS ColumnName
FROM sys.default_constraints dc
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE s.name = @SchemaName
  AND t.name = @ObjectName";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetCheckConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT
    cc.name AS ConstraintName,
    s.name AS SchemaName,
    t.name AS TableName,
    cc.definition AS CheckDefinition
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = @SchemaName AND t.name = @ObjectName
ORDER BY cc.name;";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetDefaultConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT
    dc.name AS ConstraintName,
    s.name AS SchemaName,
    t.name AS TableName,
    c.name AS ColumnName,
    dc.definition AS DefaultDefinition
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = @SchemaName AND t.name = @ObjectName
ORDER BY dc.name;";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<DataTable?> GetForeignKeyConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT
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
WHERE s.name = @SchemaName AND tp.name = @ObjectName
ORDER BY fk.name, fkc.constraint_column_id;";

            return await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name));
        }

        public async Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            var identityColumns = new Dictionary<string, (int SeedValue, int IncrementValue)>();

            if (objectName.ObjectType != ObjectTypeEnums.Table)
            {
                return identityColumns;
            }

            const string sql = @"
SELECT
    ic.name AS identity_column_name,
    ic.seed_value,
    ic.increment_value
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = @ObjectName
AND s.name = @SchemaName;";

            var dt = await LoadDataTableAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@ObjectName", objectName.Name),
                new SqlParameter("@SchemaName", objectName.Schema));

            foreach (DataRow row in dt.Rows)
            {
                string columnName = row["identity_column_name"]?.ToString() ?? string.Empty;
                int seedValue = row["seed_value"] != DBNull.Value ? Convert.ToInt32(row["seed_value"]) : 0;
                int incrementValue = row["increment_value"] != DBNull.Value ? Convert.ToInt32(row["increment_value"]) : 0;

                if (!string.IsNullOrEmpty(columnName))
                {
                    identityColumns[columnName] = (seedValue, incrementValue);
                }
            }

            return identityColumns;
        }

        public async Task<bool> HasIdentityColumnAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            if (objectName.ObjectType != ObjectTypeEnums.Table)
            {
                return false;
            }

            const string sql = @"
SELECT TOP (1) 1
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = @ObjectName
  AND s.name = @SchemaName;";

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            command.Parameters.AddWithValue("@ObjectName", objectName.Name);
            command.Parameters.AddWithValue("@SchemaName", objectName.Schema);

            await connection.OpenAsync(cancellationToken);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result != null && result != DBNull.Value;
        }

        public async Task UpdateObjectDescriptionAsync(ObjectName objectName, string description, string connectionString, CancellationToken cancellationToken = default)
        {
            var level1Type = ToLevel1Type(objectName.ObjectType);

            const string sql = @"
IF EXISTS (
    SELECT 1
    FROM fn_listextendedproperty(
        'MS_Description',
        'SCHEMA', @SchemaName,
        @Level1Type, @ObjectName,
        NULL, NULL)
)
BEGIN
    IF COALESCE(@Description, N'') = N''
        EXEC sys.sp_dropextendedproperty
            @name = N'MS_Description',
            @level0type = N'SCHEMA', @level0name = @SchemaName,
            @level1type = @Level1Type, @level1name = @ObjectName;
    ELSE
        EXEC sys.sp_updateextendedproperty
            @name = N'MS_Description', @value = @Description,
            @level0type = N'SCHEMA', @level0name = @SchemaName,
            @level1type = @Level1Type, @level1name = @ObjectName;
END
ELSE IF COALESCE(@Description, N'') <> N''
BEGIN
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description', @value = @Description,
        @level0type = N'SCHEMA', @level0name = @SchemaName,
        @level1type = @Level1Type, @level1name = @ObjectName;
END";

            await ExecuteNonQueryAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name),
                new SqlParameter("@Level1Type", level1Type),
                new SqlParameter("@Description", description ?? string.Empty));
        }

        public async Task UpdateLevel2DescriptionAsync(ObjectName objectName, string level2Name, string description, string connectionString, CancellationToken cancellationToken = default)
        {
            if (objectName.ObjectType != ObjectTypeEnums.Table &&
                objectName.ObjectType != ObjectTypeEnums.View &&
                objectName.ObjectType != ObjectTypeEnums.StoredProcedure &&
                objectName.ObjectType != ObjectTypeEnums.Function)
            {
                throw new NotSupportedException($"Level-2 description updates are not supported for object type '{objectName.ObjectType}'.");
            }

            var level1Type = ToLevel1Type(objectName.ObjectType);
            var level2Type = objectName.ObjectType == ObjectTypeEnums.Table || objectName.ObjectType == ObjectTypeEnums.View
                ? "COLUMN"
                : "PARAMETER";

            const string sql = @"
IF EXISTS (
    SELECT 1
    FROM fn_listextendedproperty(
        'MS_Description',
        'SCHEMA', @SchemaName,
        @Level1Type, @ObjectName,
        @Level2Type, @Level2Name)
)
BEGIN
    IF COALESCE(@Description, N'') = N''
        EXEC sys.sp_dropextendedproperty
            @name = N'MS_Description',
            @level0type = N'SCHEMA', @level0name = @SchemaName,
            @level1type = @Level1Type, @level1name = @ObjectName,
            @level2type = @Level2Type, @level2name = @Level2Name;
    ELSE
        EXEC sys.sp_updateextendedproperty
            @name = N'MS_Description', @value = @Description,
            @level0type = N'SCHEMA', @level0name = @SchemaName,
            @level1type = @Level1Type, @level1name = @ObjectName,
            @level2type = @Level2Type, @level2name = @Level2Name;
END
ELSE IF COALESCE(@Description, N'') <> N''
BEGIN
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description', @value = @Description,
        @level0type = N'SCHEMA', @level0name = @SchemaName,
        @level1type = @Level1Type, @level1name = @ObjectName,
        @level2type = @Level2Type, @level2name = @Level2Name;
END";

            await ExecuteNonQueryAsync(sql, connectionString, cancellationToken,
                new SqlParameter("@SchemaName", objectName.Schema),
                new SqlParameter("@ObjectName", objectName.Name),
                new SqlParameter("@Level1Type", level1Type),
                new SqlParameter("@Level2Type", level2Type),
                new SqlParameter("@Level2Name", level2Name),
                new SqlParameter("@Description", description ?? string.Empty));
        }

        private static async Task<List<ObjectName>> GetTablesAsync(string connectionString, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME;";

            return await LoadObjectsAsync(sql, connectionString, ObjectTypeEnums.Table, "TABLE_SCHEMA", "TABLE_NAME", cancellationToken);
        }

        private static async Task<List<ObjectName>> GetViewsAsync(string connectionString, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.VIEWS
ORDER BY TABLE_SCHEMA, TABLE_NAME;";

            return await LoadObjectsAsync(sql, connectionString, ObjectTypeEnums.View, "TABLE_SCHEMA", "TABLE_NAME", cancellationToken);
        }

        private static async Task<List<ObjectName>> GetStoredProceduresAsync(string connectionString, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT s.name AS SchemaName, p.name AS ObjectName
FROM sys.procedures p
INNER JOIN sys.schemas s ON p.schema_id = s.schema_id
ORDER BY s.name, p.name;";

            return await LoadObjectsAsync(sql, connectionString, ObjectTypeEnums.StoredProcedure, "SchemaName", "ObjectName", cancellationToken);
        }

        private static async Task<List<ObjectName>> GetFunctionsAsync(string connectionString, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT s.name AS SchemaName, o.name AS ObjectName
FROM sys.objects o
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE o.type IN ('FN', 'IF', 'TF')
ORDER BY s.name, o.name;";

            return await LoadObjectsAsync(sql, connectionString, ObjectTypeEnums.Function, "SchemaName", "ObjectName", cancellationToken);
        }

        private static async Task<List<ObjectName>> GetTriggersAsync(string connectionString, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT s.name AS SchemaName, tr.name AS ObjectName
FROM sys.triggers tr
INNER JOIN sys.objects o ON tr.parent_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
ORDER BY s.name, tr.name;";

            return await LoadObjectsAsync(sql, connectionString, ObjectTypeEnums.Trigger, "SchemaName", "ObjectName", cancellationToken);
        }

        private static async Task<List<ObjectName>> GetSynonymsAsync(string connectionString, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT s.name AS SchemaName, syn.name AS ObjectName
FROM sys.synonyms syn
INNER JOIN sys.schemas s ON syn.schema_id = s.schema_id
ORDER BY s.name, syn.name;";

            return await LoadObjectsAsync(sql, connectionString, ObjectTypeEnums.Synonym, "SchemaName", "ObjectName", cancellationToken);
        }

        private static async Task<List<ObjectName>> LoadObjectsAsync(string sql, string connectionString, ObjectTypeEnums objectType, string schemaColumn, string nameColumn, CancellationToken cancellationToken)
        {
            var objects = new List<ObjectName>();

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var schema = reader[schemaColumn]?.ToString();
                var name = reader[nameColumn]?.ToString();

                if (!string.IsNullOrWhiteSpace(schema) && !string.IsNullOrWhiteSpace(name))
                {
                    objects.Add(new ObjectName(objectType, schema, name));
                }
            }

            return objects;
        }

        private static async Task<DataTable> LoadDataTableAsync(string sql, string connectionString, CancellationToken cancellationToken, params SqlParameter[] parameters)
        {
            var table = new DataTable();

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            table.Load(reader);

            return table;
        }

        private static async Task ExecuteNonQueryAsync(string sql, string connectionString, CancellationToken cancellationToken, params SqlParameter[] parameters)
        {
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private static string ToLevel1Type(ObjectTypeEnums objectType)
        {
            return objectType switch
            {
                ObjectTypeEnums.Table => "TABLE",
                ObjectTypeEnums.View => "VIEW",
                ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                ObjectTypeEnums.Function => "FUNCTION",
                ObjectTypeEnums.Trigger => "TRIGGER",
                ObjectTypeEnums.Synonym => "SYNONYM",
                _ => throw new InvalidOperationException($"Unsupported object type '{objectType}'.")
            };
        }
    }
}
