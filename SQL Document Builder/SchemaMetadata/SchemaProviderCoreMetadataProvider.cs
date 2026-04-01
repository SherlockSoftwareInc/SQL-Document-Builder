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

        public async Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            return await provider.GetObjectDescriptionAsync(objectName.Schema, objectName.Name, cancellationToken);
        }

        public async Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default)
        {
            await using var provider = await ConnectAsync(connectionString, cancellationToken);
            var columnDescriptions = await provider.GetColumnDescriptionsAsync(objectName.Schema, objectName.Name, cancellationToken);
            return columnDescriptions.TryGetValue(columnName, out var value) ? value : string.Empty;
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
