using OctofyPro.SchemaProvider.Core.Abstractions;
using OctofyPro.SchemaProvider.Core.Models;
using OctofyPro.SchemaProvider.Core.Providers;
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

        public async Task<string> GetSynonymBaseObjectAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetSynonymBaseObjectAsync(objectName.Schema, objectName.Name, cancellationToken);
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
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetObjectIndexesAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetIdentityColumnsAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<DataTable?> GetCheckConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetCheckConstraintsAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<DataTable?> GetDefaultConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetDefaultConstraintsAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<DataTable?> GetForeignKeyConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetForeignKeyConstraintsAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<DataTable?> GetObjectConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetObjectConstraintsAsync(objectName.Schema, objectName.Name, cancellationToken);
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
                await using var provider = await ConnectAsync(connectionString, cancellationToken);
                return await provider.GetObjectParametersAsync(objectName.Schema, objectName.Name, cancellationToken);
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
