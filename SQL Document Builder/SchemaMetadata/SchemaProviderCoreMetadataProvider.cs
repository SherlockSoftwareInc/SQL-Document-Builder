using OctofyPro.SchemaProvider.Core.Abstractions;
using OctofyPro.SchemaProvider.Core.Models;
using OctofyPro.SchemaProvider.Core.Providers;
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
    internal sealed class SchemaProviderCoreMetadataProvider : ISchemaMetadataProvider
    {
        public async Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums objectType, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);

            return objectType switch
            {
                ObjectTypeEnums.Table => (await provider.GetAllTablesAsync(string.Empty, cancellationToken))
                    .Select(t => new ObjectName(ObjectTypeEnums.Table, t.SchemaName, t.ObjectName))
                    .ToList(),
                ObjectTypeEnums.View => (await provider.GetAllViewsAsync(string.Empty, cancellationToken))
                    .Select(v => new ObjectName(ObjectTypeEnums.View, v.SchemaName, v.ObjectName))
                    .ToList(),
                ObjectTypeEnums.StoredProcedure => (await provider.GetAllStoredProceduresAsync(string.Empty, cancellationToken))
                    .Select(p => new ObjectName(ObjectTypeEnums.StoredProcedure, p.SchemaName, p.ObjectName))
                    .ToList(),
                ObjectTypeEnums.Function => (await provider.GetAllFunctionsAsync(string.Empty, cancellationToken))
                    .Select(f => new ObjectName(ObjectTypeEnums.Function, f.SchemaName, f.ObjectName))
                    .ToList(),
                ObjectTypeEnums.Trigger => (await provider.GetAllTriggersAsync(string.Empty, cancellationToken))
                    .Select(t => new ObjectName(ObjectTypeEnums.Trigger, t.SchemaName, t.ObjectName))
                    .ToList(),
                ObjectTypeEnums.Synonym => (await provider.GetAllSynonymsAsync(string.Empty, cancellationToken))
                    .Select(s => new ObjectName(ObjectTypeEnums.Synonym, s.SchemaName, s.ObjectName))
                    .ToList(),
                ObjectTypeEnums.All => await GetAllObjectsAsync(connectionString, cancellationToken),
                _ => []
            };
        }

        public async Task<List<ObjectName>> GetAllObjectsAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);

            var objects = new List<ObjectName>();

            objects.AddRange((await provider.GetAllTablesAsync(string.Empty, cancellationToken))
                .Select(t => new ObjectName(ObjectTypeEnums.Table, t.SchemaName, t.ObjectName)));

            objects.AddRange((await provider.GetAllViewsAsync(string.Empty, cancellationToken))
                .Select(v => new ObjectName(ObjectTypeEnums.View, v.SchemaName, v.ObjectName)));

            objects.AddRange((await provider.GetAllStoredProceduresAsync(string.Empty, cancellationToken))
                .Select(p => new ObjectName(ObjectTypeEnums.StoredProcedure, p.SchemaName, p.ObjectName)));

            objects.AddRange((await provider.GetAllFunctionsAsync(string.Empty, cancellationToken))
                .Select(f => new ObjectName(ObjectTypeEnums.Function, f.SchemaName, f.ObjectName)));

            objects.AddRange((await provider.GetAllTriggersAsync(string.Empty, cancellationToken))
                .Select(t => new ObjectName(ObjectTypeEnums.Trigger, t.SchemaName, t.ObjectName)));

            objects.AddRange((await provider.GetAllSynonymsAsync(string.Empty, cancellationToken))
                .Select(s => new ObjectName(ObjectTypeEnums.Synonym, s.SchemaName, s.ObjectName)));

            return objects;
        }

        public async Task<List<string>> GetSchemasAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return (await provider.GetSchemasAsync(cancellationToken)).ToList();
        }

        public async Task<string> GetObjectDefinitionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetObjectDefinitionAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetObjectDescriptionAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);

            if (objectName.ObjectType == ObjectTypeEnums.StoredProcedure || objectName.ObjectType == ObjectTypeEnums.Function)
            {
                var parameterDescriptions = await provider.GetParameterDescriptionsAsync(objectName.Schema, objectName.Name, cancellationToken);
                return parameterDescriptions.TryGetValue(columnName, out var parameterValue) ? parameterValue : string.Empty;
            }

            var columnDescriptions = await provider.GetColumnDescriptionsAsync(objectName.Schema, objectName.Name, cancellationToken);
            return columnDescriptions.TryGetValue(columnName, out var columnValue) ? columnValue : string.Empty;
        }

        public async Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var rows = await provider.GetReferencedObjectsAsync(objectName.Schema, objectName.Name, cancellationToken);

            var table = new DataTable();
            table.Columns.Add("Schema", typeof(string));
            table.Columns.Add("ObjectName", typeof(string));
            table.Columns.Add("ObjectType", typeof(string));

            foreach (var row in rows)
            {
                table.Rows.Add(row.SchemaName, row.ObjectName, row.ObjectType.ToString().ToUpperInvariant());
            }

            return table;
        }

        public async Task<List<string>> GetPrimaryKeyColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            if (objectName.ObjectType != ObjectTypeEnums.Table)
            {
                return [];
            }

            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var columns = await provider.GetColumnsAsync(objectName.Schema, objectName.Name, cancellationToken);
            return columns.Where(c => c.IsPrimaryKey).Select(c => c.ColumnName).ToList();
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

        public async Task<DataTable?> GetObjectColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var columns = await provider.GetColumnsAsync(objectName.Schema, objectName.Name, cancellationToken);

            var table = new DataTable();
            table.Columns.Add("ORDINAL_POSITION", typeof(int));
            table.Columns.Add("COLUMN_NAME", typeof(string));
            table.Columns.Add("DATA_TYPE", typeof(string));
            table.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
            table.Columns.Add("NUMERIC_PRECISION", typeof(int));
            table.Columns.Add("NUMERIC_SCALE", typeof(int));
            table.Columns.Add("DATETIME_PRECISION", typeof(int));
            table.Columns.Add("IS_NULLABLE", typeof(string));
            table.Columns.Add("COLUMN_DEFAULT", typeof(string));

            var ordinal = 1;
            foreach (var column in columns)
            {
                var row = table.NewRow();
                row["ORDINAL_POSITION"] = ordinal++;
                row["COLUMN_NAME"] = column.ColumnName;
                row["DATA_TYPE"] = column.DataType;
                row["CHARACTER_MAXIMUM_LENGTH"] = column.MaxLength.HasValue ? column.MaxLength.Value : DBNull.Value;
                row["NUMERIC_PRECISION"] = DBNull.Value;
                row["NUMERIC_SCALE"] = DBNull.Value;
                row["DATETIME_PRECISION"] = DBNull.Value;
                row["IS_NULLABLE"] = column.IsNullable ? "YES" : "NO";
                row["COLUMN_DEFAULT"] = DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }

        public async Task<DataTable?> GetObjectParametersAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            if (objectName.ObjectType == ObjectTypeEnums.Function)
            {
                await using var provider = await ConnectAsync(connectionString, cancellationToken);
                var functionMetadata = await provider.GetFunctionMetadataAsync(objectName.Schema, objectName.Name, cancellationToken);

                var table = new DataTable();
                table.Columns.Add("ORDINAL_POSITION", typeof(int));
                table.Columns.Add("PARAMETER_NAME", typeof(string));
                table.Columns.Add("DATA_TYPE", typeof(string));
                table.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
                table.Columns.Add("PARAMETER_MODE", typeof(string));

                if (functionMetadata == null)
                {
                    return table;
                }

                var ordinal = 1;
                foreach (var parameter in functionMetadata.Parameters.Where(p => !p.IsReturn))
                {
                    var row = table.NewRow();
                    row["ORDINAL_POSITION"] = ordinal++;
                    row["PARAMETER_NAME"] = parameter.Name;
                    row["DATA_TYPE"] = parameter.DataType;
                    row["CHARACTER_MAXIMUM_LENGTH"] = DBNull.Value;
                    row["PARAMETER_MODE"] = parameter.IsOutput ? "INOUT" : "IN";
                    table.Rows.Add(row);
                }

                return table;
            }

            if (objectName.ObjectType == ObjectTypeEnums.StoredProcedure)
            {
                const string paramSql = @"
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

                var table = new DataTable();
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand(paramSql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                command.Parameters.AddWithValue("@SchemaName", objectName.Schema);
                command.Parameters.AddWithValue("@ObjectName", objectName.Name);

                await connection.OpenAsync(cancellationToken);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                table.Load(reader);
                return table;
            }

            return new DataTable();
        }

        public async Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var rows = await provider.GetReferencingObjectsAsync(objectName.Schema, objectName.Name, cancellationToken);

            var table = new DataTable();
            table.Columns.Add("Schema", typeof(string));
            table.Columns.Add("ObjectName", typeof(string));
            table.Columns.Add("ObjectType", typeof(string));

            foreach (var row in rows)
            {
                table.Rows.Add(row.SchemaName, row.ObjectName, row.ObjectType.ToString().ToUpperInvariant());
            }

            return table;
        }

        public async Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var rows = await provider.GetDependenciesAsync(objectName.Schema, objectName.Name, cancellationToken);

            var table = new DataTable();
            table.Columns.Add("FromColumn", typeof(string));
            table.Columns.Add("ToSchema", typeof(string));
            table.Columns.Add("ToTable", typeof(string));
            table.Columns.Add("ToColumn", typeof(string));

            foreach (var row in rows)
            {
                table.Rows.Add(row.ParentColumn, row.ReferencedSchema, row.ReferencedTable, row.ReferencedColumn);
            }

            return table;
        }

        public async Task<bool> HasIdentityColumnAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            if (objectName.ObjectType != ObjectTypeEnums.Table)
            {
                return false;
            }

            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var tableMetadata = await provider.GetTableMetadataAsync(objectName.Schema, objectName.Name, cancellationToken);
            return tableMetadata?.HasIdentityColumn ?? false;
        }

        public async Task UpdateObjectDescriptionAsync(ObjectName objectName, string description, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            await provider.UpdateObjectDescriptionAsync(
                objectName.Schema,
                objectName.Name,
                ToDatabaseObjectType(objectName.ObjectType),
                description,
                cancellationToken);
        }

        public async Task UpdateLevel2DescriptionAsync(ObjectName objectName, string level2Name, string description, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);

            if (objectName.ObjectType == ObjectTypeEnums.Table || objectName.ObjectType == ObjectTypeEnums.View)
            {
                await provider.UpdateColumnDescriptionAsync(objectName.Schema, objectName.Name, level2Name, description, cancellationToken);
            }
            else if (objectName.ObjectType == ObjectTypeEnums.StoredProcedure || objectName.ObjectType == ObjectTypeEnums.Function)
            {
                await provider.UpdateParameterDescriptionAsync(objectName.Schema, objectName.Name, level2Name, description, cancellationToken);
            }
            else
            {
                throw new NotSupportedException($"Level-2 description updates are not supported for object type '{objectName.ObjectType}'.");
            }
        }

        private static async Task<IDatabaseSchemaProvider> ConnectAsync(string connectionString, CancellationToken cancellationToken)
        {
            var provider = DatabaseSchemaProviderFactory.Create(DatabaseProviderKind.SqlServer);
            await provider.ConnectAsync(connectionString, cancellationToken);
            return provider;
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

        private static DatabaseObjectType ToDatabaseObjectType(ObjectTypeEnums objectType)
        {
            return objectType switch
            {
                ObjectTypeEnums.Table => DatabaseObjectType.Table,
                ObjectTypeEnums.View => DatabaseObjectType.View,
                ObjectTypeEnums.StoredProcedure => DatabaseObjectType.Procedure,
                ObjectTypeEnums.Function => DatabaseObjectType.Function,
                ObjectTypeEnums.Trigger => DatabaseObjectType.Trigger,
                ObjectTypeEnums.Synonym => DatabaseObjectType.Synonym,
                _ => DatabaseObjectType.Unknown
            };
        }
    }
}
