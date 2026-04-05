using System.Collections.Generic;

namespace SQL_Document_Builder
{
    public sealed class DBSchemaItem
    {
        public DBSchemaItem(string schemaName, string objectName)
        {
            SchemaName = schemaName ?? string.Empty;
            ObjectName = objectName ?? string.Empty;
            Columns = [];
        }

        public string SchemaName { get; set; }

        public string ObjectName { get; set; }

        public List<DBColumn> Columns { get; }

        public string Description { get; set; } = string.Empty;

        public bool DescriptionsLoaded { get; set; }
    }
}
