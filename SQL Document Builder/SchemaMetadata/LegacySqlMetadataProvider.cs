using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
        public Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums objectType, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<List<ObjectName>> GetAllObjectsAsync(string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<List<string>> GetSchemasAsync(string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<string> GetObjectDefinitionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task UpdateObjectDescriptionAsync(ObjectName objectName, string description, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");

        public Task UpdateLevel2DescriptionAsync(ObjectName objectName, string level2Name, string description, string connectionString, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Legacy provider is no longer supported.");
    }
}
