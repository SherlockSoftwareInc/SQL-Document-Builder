using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder.SchemaMetadata
{
    internal sealed class LegacySqlMetadataProvider : ISchemaMetadataProvider
    {
        private readonly SchemaProviderCoreMetadataProvider _coreProvider = new();

        public Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums objectType, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetDatabaseObjectsAsync(objectType, connectionString, cancellationToken);

        public Task<List<ObjectName>> GetAllObjectsAsync(string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetAllObjectsAsync(connectionString, cancellationToken);

        public Task<List<string>> GetSchemasAsync(string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetSchemasAsync(connectionString, cancellationToken);

        public Task<string> GetObjectDefinitionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectDefinitionAsync(objectName, connectionString, cancellationToken);

        public Task<string> GetSynonymBaseObjectAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetSynonymBaseObjectAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetTableInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetTableInfoAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetViewInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetViewInfoAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetProcedureInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetProcedureInfoAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetFunctionInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetFunctionInfoAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetTriggerInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetTriggerInfoAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetSynonymInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetSynonymInfoAsync(objectName, connectionString, cancellationToken);

        public Task<List<ObjectName>> GetRecentObjectsAsync(DateTime startDate, DateTime endDate, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetRecentObjectsAsync(startDate, endDate, connectionString, cancellationToken);

        public Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectDescriptionAsync(objectName, connectionString, cancellationToken);

        public Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetColumnDescriptionAsync(objectName, columnName, connectionString, cancellationToken);

        public Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetReferencedObjectsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetObjectColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectColumnsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetObjectParametersAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectParametersAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetReferencingObjectsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetObjectRelationshipsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectRelationshipsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetForeignTablesAsync(objectName, connectionString, cancellationToken);

        public Task<List<string>> GetPrimaryKeyColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetPrimaryKeyColumnsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetObjectIndexesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectIndexesAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetObjectConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetObjectConstraintsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetCheckConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetCheckConstraintsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetDefaultConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetDefaultConstraintsAsync(objectName, connectionString, cancellationToken);

        public Task<DataTable?> GetForeignKeyConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetForeignKeyConstraintsAsync(objectName, connectionString, cancellationToken);

        public Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.GetIdentityColumnsAsync(objectName, connectionString, cancellationToken);

        public Task<bool> HasIdentityColumnAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.HasIdentityColumnAsync(objectName, connectionString, cancellationToken);

        public Task UpdateObjectDescriptionAsync(ObjectName objectName, string description, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.UpdateObjectDescriptionAsync(objectName, description, connectionString, cancellationToken);

        public Task UpdateLevel2DescriptionAsync(ObjectName objectName, string level2Name, string description, string connectionString, CancellationToken cancellationToken = default)
            => _coreProvider.UpdateLevel2DescriptionAsync(objectName, level2Name, description, connectionString, cancellationToken);
    }
}
