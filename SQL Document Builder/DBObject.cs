using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SQL_Document_Builder.SchemaMetadata;
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
        /// Gets or sets the constraints.
        /// </summary>
        public List<ConstraintItem> Constraints { get; set; } = [];

        /// <summary>
        /// Gets or sets the object definition.
        /// </summary>
        public string Definition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or set description for the object
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string? FullName => ObjectName.FullName;

        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        public List<IndexItem> Indexes { get; set; } = [];

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string? Name => ObjectName.Name;

        /// <summary>
        /// Gets the object type.
        /// </summary>
        public ObjectTypeEnums ObjectType => ObjectName.ObjectType;

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public List<DBParameter> Parameters { get; set; } = [];

        /// <summary>
        /// Gets or sets the primary key columns.
        /// </summary>
        public string PrimaryKeyColumns { get; set; } = string.Empty;

        /// <summary>
        /// Gets the schema.
        /// </summary>
        public string? Schema => ObjectName.Schema;

        /// <summary>
        /// Gets or sets the trigger infomation.
        /// </summary>
        public TriggerInfo? TriggerInfomation { get; set; }

        /// <summary>
        /// Gets or sets the function information.
        /// </summary>
        internal FunctionInfo? FunctionInformation { get; set; }

        /// <summary>
        /// Gets or sets object name
        /// </summary>
        internal ObjectName ObjectName { get; private set; }

        /// <summary>
        /// Gets or sets the stored procedure Information.
        /// </summary>
        internal ProcedureInfo? ProcedureInformation { get; set; }

        /// <summary>
        /// Gets or sets the synonym information.
        /// </summary>
        internal SynonymInfo? SynonymInformation { get; set; }

        /// <summary>
        /// Gets or sets the table Information.
        /// </summary>
        internal TableInfo? TableInformation { get; set; }

        /// <summary>
        /// Gets or sets the view Information.
        /// </summary>
        internal ViewInfo? ViewInformation { get; set; }

        internal DBSchema? SchemaCache { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        private DatabaseConnectionItem? Connection { get; set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        private string ConnectionString => Connection?.ConnectionString ?? string.Empty;

        /// <summary>
        /// Updates the column/parameter (level2) description.
        /// </summary>
        /// <param name="columnOrParameter">The column name.</param>
        /// <param name="newDescription">The new description.</param>
        public async Task UpdateLevel2DescriptionAsync(string columnOrParameter, string newDescription, ObjectName.ObjectTypeEnums objectType)
        {
            if (ConnectionString?.Length == 0) return;

            if (objectType == ObjectTypeEnums.Table
                || objectType == ObjectTypeEnums.View
                || objectType == ObjectTypeEnums.StoredProcedure
                || objectType == ObjectTypeEnums.Function)
            {
                try
                {
                    await SchemaMetadataProviderContext.Current.UpdateLevel2DescriptionAsync(ObjectName, columnOrParameter, newDescription, ConnectionString);
                }
                catch (NotSupportedException)
                {
                    // Some providers do not support level-2 description updates for all object types.
                    return;
                }
            }

            if (objectType == ObjectTypeEnums.Table || objectType == ObjectTypeEnums.View)
            {
                // Update the column description in the local Columns list
                var column = Columns.Find(col => col.ColumnName == columnOrParameter);
                if (column != null)
                {
                    column.Description = newDescription;
                }
            }
            else if (objectType == ObjectTypeEnums.StoredProcedure || objectType == ObjectTypeEnums.Function)
            {
                // Update the parameter description in the local Parameters list
                var parameter = Parameters.Find(p => p.Name.Equals(columnOrParameter, StringComparison.OrdinalIgnoreCase));
                if (parameter != null)
                {
                    parameter.Description = newDescription;
                }
            }

            SchemaCache?.SetLevel2Description(ObjectName, columnOrParameter, newDescription);
        }

        /// <summary>
        /// Updates the discription for the object.
        /// </summary>
        /// <param name="newDescription">The new description.</param>
        /// <returns>A Task.</returns>
        public async Task UpdateObjectDescAsync(string newDescription)
        {
            if (string.IsNullOrEmpty(ConnectionString) || ObjectName.IsEmpty())
            {
                Description = newDescription;
                return;
            }

            if (ObjectName.ObjectType != ObjectTypeEnums.None)
            {
                try
                {
                    await SchemaMetadataProviderContext.Current.UpdateObjectDescriptionAsync(ObjectName, newDescription, ConnectionString);
                }
                catch (NotSupportedException)
                {
                    // Some providers do not support object description updates for all object types.
                    return;
                }
            }

            Description = newDescription;
            SchemaCache?.SetObjectDescription(ObjectName, newDescription);
        }

        private bool SupportsObjectDescriptionUpdate()
        {
            if (Connection == null)
            {
                return false;
            }

            return Connection.DBMSType switch
            {
                DBMSTypeEnums.SQLServer => true,
                DBMSTypeEnums.MySQL or DBMSTypeEnums.MariaDB or DBMSTypeEnums.PostgreSQL or DBMSTypeEnums.Oracle
                    => ObjectName.ObjectType is ObjectTypeEnums.Table or ObjectTypeEnums.View,
                _ => false,
            };
        }

        private bool SupportsLevel2DescriptionUpdate(ObjectName.ObjectTypeEnums objectType)
        {
            if (Connection == null)
            {
                return false;
            }

            return Connection.DBMSType switch
            {
                DBMSTypeEnums.SQLServer => objectType is ObjectTypeEnums.Table
                    or ObjectTypeEnums.View
                    or ObjectTypeEnums.StoredProcedure
                    or ObjectTypeEnums.Function,
                DBMSTypeEnums.MySQL or DBMSTypeEnums.MariaDB or DBMSTypeEnums.PostgreSQL or DBMSTypeEnums.Oracle
                    => objectType is ObjectTypeEnums.Table or ObjectTypeEnums.View,
                _ => false,
            };
        }

        /// <summary>
        /// Gets the list of object that depend on the specified table.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<List<ObjectName>> GetObjectsUsingTableAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            if (connection == null || string.IsNullOrEmpty(connection.ConnectionString) || objectName == null || objectName.IsEmpty())
            {
                return [];
            }

            if (connection?.DBMSType == DBMSTypeEnums.SQLServer)
            {
                var sqlServerObjectList = await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(ObjectTypeEnums.View, connection.ConnectionString);
                var dependentObjects = await SchemaMetadataProviderContext.Current.GetReferencingObjectsAsync(objectName, connection.ConnectionString);
                if (dependentObjects == null || dependentObjects.Rows.Count == 0)
                {
                    return [];
                }

                var filtered = new List<ObjectName>();
                foreach (DataRow row in dependentObjects.Rows)
                {
                    string typeName = row["ObjectType"]?.ToString() ?? string.Empty;
                    var objectType = ObjectName.ConvertObjectType(typeName);
                    string name = row["ObjectName"]?.ToString() ?? string.Empty;
                    string schema = row["Schema"]?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(schema))
                    {
                        filtered.Add(new ObjectName(objectType, schema, name));
                    }
                }

                var sqlServerObjectListLookup = new HashSet<string>(sqlServerObjectList.Select(o => o.FullNameNoQuote), StringComparer.OrdinalIgnoreCase);
                return filtered.Where(o => sqlServerObjectListLookup.Contains(o.FullNameNoQuote)).ToList();
            }

            return [];
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
            var dt = await SchemaMetadataProviderContext.Current.GetCheckConstraintsAsync(ObjectName, ConnectionString);
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
            var dt = await SchemaMetadataProviderContext.Current.GetDefaultConstraintsAsync(ObjectName, ConnectionString);
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
            var dt = await SchemaMetadataProviderContext.Current.GetForeignKeyConstraintsAsync(ObjectName, ConnectionString);
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
        /// Retrieves identity column details for the current table.
        /// </summary>
        /// <returns>A dictionary where the key is the column name and the value is a tuple containing seed and increment values.</returns>
        internal async Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumns(DatabaseConnectionItem connection)
        {
            return await SchemaMetadataProviderContext.Current.GetIdentityColumnsAsync(ObjectName, connection.ConnectionString);
        }

        /// <summary>
        /// Opens the data object.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        internal async Task<bool> OpenAsync(ObjectName objectName, DatabaseConnectionItem? connection)
        {
            // objnect name and connection cannot be null
            if (objectName == null || objectName.IsEmpty() || connection == null || string.IsNullOrEmpty(connection.ConnectionString))
            {
                Common.MsgBox("Object name or connection is not valid.", MessageBoxIcon.Error);
                return false;
            }

            // keep the object name and connection
            this.ObjectName = objectName;
            this.Connection = connection;

            var objectType = objectName.ObjectType;

            // Get the object properties
            await OpenObjectInfo();

            return objectType switch
            {
                ObjectTypeEnums.Table or ObjectTypeEnums.View => await OpenTableAsync(),
                ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => await OpenFunctionAsync(),
                ObjectTypeEnums.Trigger => await OpenTriggerAsync(),
                ObjectTypeEnums.Synonym => await OpenSynonymAsync(),
                _ => false,
            };
        }

        /// <summary>
        /// Gets the function parameters async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<DataTable?> GetParametersAsync(ObjectName objectName, DatabaseConnectionItem connection)
        {
            return await SchemaMetadataProviderContext.Current.GetObjectParametersAsync(objectName, connection.ConnectionString);
        }

        /// <summary>
        /// Gets the column desc.
        /// </summary>
        private async Task GetColumnDescAsync()
        {
            if (string.IsNullOrEmpty(ConnectionString) || ObjectName.IsEmpty() || Columns.Count == 0)
                return;

            foreach (var column in Columns)
            {
                column.Description = SchemaCache != null
                    ? await SchemaCache.GetLevel2DescriptionAsync(ObjectName, column.ColumnName)
                    : await SchemaMetadataProviderContext.Current.GetColumnDescriptionAsync(ObjectName, column.ColumnName, ConnectionString);
            }
        }

        /// <summary>
        /// Gets the columns async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<bool> GetColumnsAsync()
        {
            bool result = false;

            Columns.Clear();

            if (ConnectionString?.Length > 0 && ObjectName.Name.Length > 0)
            {
                if (SchemaCache != null)
                {
                    var cachedColumns = SchemaCache.GetCachedColumns(ObjectName);
                    if (cachedColumns.Count > 0)
                    {
                        Columns.AddRange(cachedColumns);
                        return true;
                    }
                }

                var dt = await SchemaMetadataProviderContext.Current.GetObjectColumnsAsync(ObjectName, ConnectionString);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Columns.Add(new DBColumn(row));
                    }
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all constraints (PK, FK, CHECK, DEFAULT) for the current table and populates the Constraints list.
        /// </summary>
        private async Task GetConstraintsAsync()
        {
            Constraints.Clear();

            if (ObjectName == null || ObjectName.IsEmpty() || string.IsNullOrEmpty(ConnectionString))
                return;

            var dt = await SchemaMetadataProviderContext.Current.GetObjectConstraintsAsync(ObjectName, ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                Constraints.Add(new ConstraintItem(row));
            }
        }

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        private async Task GetIndexesAsync(ObjectName? objectName)
        {
            Indexes.Clear();

            if (objectName == null || objectName.IsEmpty())
                return;

            var dt = await SchemaMetadataProviderContext.Current.GetObjectIndexesAsync(objectName, ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return;

            // Group columns by index
            var indexGroups = dt.AsEnumerable()
                .GroupBy(row => row["IndexName"]?.ToString() ?? string.Empty);

            foreach (var group in indexGroups)
            {
                var firstRow = group.First();
                string indexName = firstRow["IndexName"]?.ToString() ?? string.Empty;
                string type = firstRow["Type"]?.ToString() ?? string.Empty;
                bool isUnique = firstRow["IsUnique"] != DBNull.Value && Convert.ToBoolean(firstRow["IsUnique"]);
                string columns = string.Join(", ", group.Select(r => $"{r["ColumnName"]}"));

                Indexes.Add(new IndexItem(indexName, type, columns, isUnique));

                // For each column in this index, add '🔢' to Ord if not already present
                foreach (var row in group)
                {
                    string? columnName = row["ColumnName"]?.ToString();
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        var column = Columns.Find(c => c.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
                        if (column != null && (column.Ord == null || !column.Ord.Contains("🔢")))
                        {
                            column.Ord += "🔢";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parameter desc async.
        /// </summary>
        /// <returns>A Task.</returns>
        /// <summary>
        /// Gets the parameter descriptions from extended properties and updates the Parameters list.
        /// </summary>
        private async Task GetParameterDescAsync()
        {
            if (string.IsNullOrEmpty(ConnectionString) || ObjectName.IsEmpty() || Parameters.Count == 0)
                return;

            foreach (var parameter in Parameters)
            {
                parameter.Description = SchemaCache != null
                    ? await SchemaCache.GetLevel2DescriptionAsync(ObjectName, parameter.Name)
                    : await SchemaMetadataProviderContext.Current.GetColumnDescriptionAsync(ObjectName, parameter.Name, ConnectionString);
            }
        }

        /// <summary>
        /// Primaries the keys.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private async Task GetPrimaryKeysAsync(ObjectName objectName)
        {
            PrimaryKeyColumns = string.Empty;

            var primaryKeyColumns = await SchemaMetadataProviderContext.Current.GetPrimaryKeyColumnsAsync(objectName, ConnectionString);
            if (primaryKeyColumns.Count == 0)
                return;

            foreach (var columnName in primaryKeyColumns)
            {
                // find the column in the Columns
                var column = Columns.Find(c => c.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
                if (column != null)
                {
                    column.Ord += "🗝";

                    if (PrimaryKeyColumns.Length > 0)
                        PrimaryKeyColumns += ", ";
                    PrimaryKeyColumns += $"[{column.ColumnName}]";
                }
            }
        }

        /// <summary>
        /// Opens the function async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<bool> OpenFunctionAsync()
        {
            //get the object description and definition
            Description = SchemaCache != null
                ? await SchemaCache.GetObjectDescriptionAsync(ObjectName)
                : await SchemaMetadataProviderContext.Current.GetObjectDescriptionAsync(ObjectName, ConnectionString);
            Definition = await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(ObjectName, ConnectionString);

            // Get the function parameters
            Parameters.Clear();
            var dtParameters = await GetParametersAsync(ObjectName, Connection);
            if (dtParameters != null && dtParameters.Rows.Count > 0)
            {
                foreach (DataRow row in dtParameters.Rows)
                {
                    var parameter = new DBParameter(row);
                    Parameters.Add(parameter);
                }
            }

            if (Parameters.Count > 0)
            {
                await GetParameterDescAsync();
            }

            return true;
        }

        /// <summary>
        /// Opens the object info.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task OpenObjectInfo()
        {
            if (ObjectName == null || ObjectName.IsEmpty() || string.IsNullOrEmpty(ConnectionString))
                return;

            switch (ObjectName.ObjectType)
            {
                case ObjectTypeEnums.Table:
                    TableInformation = new TableInfo();
                    await TableInformation.OpenAsync(ObjectName, ConnectionString);
                    break;

                case ObjectTypeEnums.View:
                    ViewInformation = new ViewInfo();
                    await ViewInformation.OpenAsync(ObjectName, ConnectionString);
                    break;

                case ObjectTypeEnums.StoredProcedure:
                    ProcedureInformation = new ProcedureInfo();
                    await ProcedureInformation.OpenAsync(ObjectName, ConnectionString);
                    break;

                case ObjectTypeEnums.Function:
                    FunctionInformation = new FunctionInfo();
                    await FunctionInformation.OpenAsync(ObjectName, ConnectionString);
                    break;

                case ObjectTypeEnums.Trigger:
                    TriggerInfomation = new TriggerInfo();
                    await TriggerInfomation.OpenAsync(ObjectName, ConnectionString);
                    break;

                case ObjectTypeEnums.Synonym:
                    SynonymInformation = new SynonymInfo();
                    await SynonymInformation.OpenAsync(ObjectName, ConnectionString);
                    break;
            }
        }

        /// <summary>
        /// Opens the synonym async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<bool> OpenSynonymAsync()
        {
            if (ObjectName == null || ObjectName.IsEmpty() || string.IsNullOrEmpty(ConnectionString))
                return false;

            Definition = await SchemaMetadataProviderContext.Current.GetSynonymBaseObjectAsync(ObjectName, ConnectionString);
            if (string.IsNullOrEmpty(Definition))
                return false;

            Description = SchemaCache != null
                ? await SchemaCache.GetObjectDescriptionAsync(ObjectName)
                : await SchemaMetadataProviderContext.Current.GetObjectDescriptionAsync(ObjectName, ConnectionString);

            return true;
        }

        /// <summary>
        /// Opens the table or view.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<bool> OpenTableAsync()
        {
            bool result = true;

            if (ConnectionString?.Length > 0 && ObjectName.Name.Length > 0)
            {
                result = await GetColumnsAsync();

                if (ObjectName?.ObjectType == ObjectTypeEnums.Table)
                {
                    await GetPrimaryKeysAsync(ObjectName);

                    await GetConstraintsAsync();
                }

                await GetIndexesAsync(ObjectName);

                Description = SchemaCache != null
                    ? await SchemaCache.GetObjectDescriptionAsync(ObjectName)
                    : await SchemaMetadataProviderContext.Current.GetObjectDescriptionAsync(ObjectName, ConnectionString);
                await GetColumnDescAsync();

                // get the definition if it is a view
                if (ObjectName.ObjectType == ObjectTypeEnums.View)
                {
                    Definition = await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(ObjectName, ConnectionString);
                }
            }

            return result;
        }

        /// <summary>
        /// Opens the trigger async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<bool> OpenTriggerAsync()
        {
            //get the object description and definition
            Description = SchemaCache != null
                ? await SchemaCache.GetObjectDescriptionAsync(ObjectName)
                : await SchemaMetadataProviderContext.Current.GetObjectDescriptionAsync(ObjectName, ConnectionString);
            Definition = await SchemaMetadataProviderContext.Current.GetObjectDefinitionAsync(ObjectName, ConnectionString);

            // clear the parameters
            Parameters.Clear();

            return true;
        }

        /// <summary>
        /// Gets the referencing objects async.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A Task.</returns>
        internal async Task<DataTable?> GetReferencingObjectsAsync(DatabaseConnectionItem connection)
        {
            if (connection == null || string.IsNullOrEmpty(connection.ConnectionString) || ObjectName.IsEmpty())
            {
                return null;
            }
            if (connection?.DBMSType == DBMSTypeEnums.SQLServer)
            {
                return await SchemaMetadataProviderContext.Current.GetReferencingObjectsAsync(ObjectName, connection.ConnectionString);
            }
            return null;
        }

        /// <summary>
        /// Gets the objects that are referenced by this object (e.g., tables/views/functions/procedures this object depends on).
        /// </summary>
        /// <param name="connection">The database connection item.</param>
        /// <returns>A Task containing a list of referenced ObjectName objects.</returns>
        internal async Task<DataTable?> GetReferencedObjectsAsync(DatabaseConnectionItem connection)
        {
            if (connection == null || string.IsNullOrEmpty(connection.ConnectionString) || ObjectName.IsEmpty())
            {
                return null;
            }
            if (connection?.DBMSType == DBMSTypeEnums.SQLServer)
            {
                return await SchemaMetadataProviderContext.Current.GetReferencedObjectsAsync(ObjectName, connection.ConnectionString);
            }
            return null;
        }

        /// <summary>
        /// Gets the foreign tables async.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A Task.</returns>
        internal async Task<DataTable?> GetForeignTablesAsync(DatabaseConnectionItem connection)
        {
            if (connection == null || string.IsNullOrEmpty(connection.ConnectionString) || ObjectName.IsEmpty())
            {
                return null;
            }
            if (connection?.DBMSType == DBMSTypeEnums.SQLServer)
            {
                return await SchemaMetadataProviderContext.Current.GetForeignTablesAsync(ObjectName, connection.ConnectionString);
            }
            return null;
        }

    }
}