using System.Collections.Generic;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder.SchemaMetadata
{
    internal interface ISchemaMetadataProvider
    {
        Task<List<ObjectName>> GetDatabaseObjectsAsync(ObjectTypeEnums objectType, string connectionString, CancellationToken cancellationToken = default);

        Task<List<ObjectName>> GetAllObjectsAsync(string connectionString, CancellationToken cancellationToken = default);

        Task<List<string>> GetSchemasAsync(string connectionString, CancellationToken cancellationToken = default);

        Task<string> GetObjectDefinitionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<string> GetSynonymBaseObjectAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetTableInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetViewInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetProcedureInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetFunctionInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetTriggerInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetSynonymInfoAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<List<ObjectName>> GetRecentObjectsAsync(DateTime startDate, DateTime endDate, string connectionString, CancellationToken cancellationToken = default);

        Task<string> GetObjectDescriptionAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<string> GetColumnDescriptionAsync(ObjectName objectName, string columnName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetReferencedObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetReferencingObjectsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetObjectRelationshipsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetForeignTablesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<List<string>> GetPrimaryKeyColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetObjectIndexesAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetObjectConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetCheckConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetDefaultConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetForeignKeyConstraintsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetObjectColumnsAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetObjectParametersAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task<bool> HasIdentityColumnAsync(ObjectName objectName, string connectionString, CancellationToken cancellationToken = default);

        Task UpdateObjectDescriptionAsync(ObjectName objectName, string description, string connectionString, CancellationToken cancellationToken = default);

        Task UpdateLevel2DescriptionAsync(ObjectName objectName, string level2Name, string description, string connectionString, CancellationToken cancellationToken = default);
    }
}
