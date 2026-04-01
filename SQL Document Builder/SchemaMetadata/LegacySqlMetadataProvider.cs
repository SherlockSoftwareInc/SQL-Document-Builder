using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder.SchemaMetadata
{
    internal sealed class LegacySqlMetadataProvider : ISchemaMetadataProvider
    {
        public Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums objectType, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetDatabaseObjectsAsync(objectType, connectionString);

        public Task<List<ObjectName>> GetAllObjectsAsync(string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetAllObjectsAsync(connectionString);

        public Task<List<string>> GetSchemasAsync(string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetSchemasAsync(connectionString);

        public Task<string> GetObjectDefinitionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetObjectDefinitionAsync(objectName, connectionString);

        public Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetObjectDescriptionAsync(objectName, connectionString);

        public Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetColumnDescriptionAsync(objectName, columnName, connectionString);

        public Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetReferencedObjectsAsync(objectName, connectionString);

        public Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetReferencingObjectsAsync(objectName, connectionString);

        public Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetForeignTablesAsync(objectName, connectionString);

        public Task UpdateObjectDescriptionAsync(ObjectName objectName, string description, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.UpdateObjectDescAsync(objectName, description, connectionString);

        public Task UpdateLevel2DescriptionAsync(ObjectName objectName, string level2Name, string description, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.UpdateLevel2DescriptionAsync(objectName, level2Name, description, connectionString);
    }
}
