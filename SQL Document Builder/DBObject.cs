using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        /// Gets or sets object name
        /// </summary>
        internal ObjectName ObjectName { get; private set; }

        /// <summary>
        /// Gets or sets database connection string
        /// </summary>
        private string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Updates the column description.
        /// </summary>
        /// <param name="columnOrParameter">The column name.</param>
        /// <param name="newDescription">The new description.</param>
        public async Task UpdateLevel1DescriptionAsync(string columnOrParameter, string newDescription, ObjectName.ObjectTypeEnums objectType)
        {
            if (ConnectionString.Length == 0) return;

            string newDesc = newDescription.Replace("'", "''");
            string level1type = objectType switch
            {
                ObjectTypeEnums.Table => "TABLE",
                ObjectTypeEnums.View => "VIEW",
                ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                ObjectTypeEnums.Function => "FUNCTION",
                ObjectTypeEnums.Trigger => "TRIGGER",
                _ => throw new InvalidOperationException("Unsupported object type for description update.")
            };

            string level2Type = objectType switch
            {
                ObjectTypeEnums.Table or ObjectTypeEnums.View => "COLUMN",
                ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => "PARAMETER",
                _ => throw new InvalidOperationException("Unsupported object type for description update.")
            };

            string query;
            if (string.IsNullOrEmpty(newDescription))
            {
                query = $@"EXEC sys.sp_dropextendedproperty
@name = N'MS_Description',
@level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
@level1type = N'{level1type}', @level1name = '{ObjectName.Name}',
@level2type = N'{level2Type}', @level2name = '{columnOrParameter}'";
            }
            else
            {
                query = $@"
IF EXISTS (
    SELECT 1
    FROM fn_listextendedproperty (
		N'MS_Description',
        N'SCHEMA',
        N'{ObjectName.Schema}',
        N'{level1type}',
        N'{ObjectName.Name}',
        N'{level2Type}',
        N'{columnOrParameter}')
)
BEGIN
    EXEC sys.sp_updateextendedproperty
        @name = N'MS_Description',
        @value = N'{newDesc}',
        @level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
        @level1type = N'{level1type}', @level1name = '{ObjectName.Name}',
        @level2type = N'{level2Type}', @level2name = '{columnOrParameter}';
END
ELSE
BEGIN
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'{newDesc}',
        @level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
        @level1type = N'{level1type}', @level1name = '{ObjectName.Name}',
        @level2type = N'{level2Type}', @level2name = '{columnOrParameter}';
END;";
            }

            Clipboard.SetText(query);

            await DatabaseHelper.ExecuteSQLAsync(query, ConnectionString);

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
        }

        /// <summary>
        /// Updates the table desc.
        /// </summary>
        /// <param name="newDescription">The description.</param>
        public async Task UpdateTableDescAsync(string newDescription)
        {
            Description = newDescription;

            if (ConnectionString.Length == 0 || ObjectName.IsEmpty()) return;

            if (!ObjectName.IsEmpty() && newDescription.Length > 0)
            {
                string newDesc = newDescription.Replace("'", "''");
                string level1type = ObjectName.ObjectType switch
                {
                    ObjectTypeEnums.Table => "TABLE",
                    ObjectTypeEnums.View => "VIEW",
                    ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                    ObjectTypeEnums.Function => "FUNCTION",
                    ObjectTypeEnums.Trigger => "TRIGGER",
                    _ => throw new InvalidOperationException("Unsupported object type for description update.")
                };

                string query;
                if (string.IsNullOrEmpty(newDescription))
                {
                    query = $@"EXEC sys.sp_dropextendedproperty
@name = N'MS_Description',
@level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
@level1type = N'{level1type}', @level1name = '{ObjectName.Name}'";
                }
                else
                {
                    query = $@"IF EXISTS (
    SELECT 1
      FROM sys.extended_properties
     WHERE class = 1 AND major_id = OBJECT_ID(N'{ObjectName.FullName}')
       AND minor_id = 0
       AND name = 'MS_Description')
        EXEC sp_updateextendedproperty
            @name = N'MS_Description',
            @value = N'{newDesc}',
            @level0type = N'SCHEMA', @level0name = N'{ObjectName.Schema}',
            @level1type = N'{level1type}', @level1name = N'{ObjectName.Name}';
    ELSE
        EXEC sp_addextendedproperty
            @name = N'MS_Description',
            @value = N'{newDesc}',
            @level0type = N'SCHEMA', @level0name = N'{ObjectName.Schema}',
            @level1type = N'{level1type}', @level1name = N'{ObjectName.Name}';";
                }

                var result = await DatabaseHelper.ExecuteSQLAsync(query, ConnectionString);
                if (result != string.Empty)
                {
                    Common.MsgBox("Failed to update object description." + Environment.NewLine + result, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Gets the list of views where the specified table is used.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<List<(string ViewName, string SchemaName)>> GetViewsUsingTableAsync(ObjectName objectName, string connectionString)
        {
            var views = new List<(string ViewName, string SchemaName)>();
            if (string.IsNullOrEmpty(objectName.Name))
            {
                return views;
            }

            if (string.IsNullOrEmpty(objectName.Schema))
                objectName.Schema = "dbo";

            string query = $@"
SELECT
    v.name AS ViewName,
    s.name AS SchemaName
FROM
    sys.views v
    INNER JOIN sys.sql_expression_dependencies d ON v.object_id = d.referencing_id
    INNER JOIN sys.objects o ON d.referenced_id = o.object_id
    INNER JOIN sys.schemas s ON v.schema_id = s.schema_id
WHERE
    o.name = '{objectName.Name}' AND
    s.name = '{objectName.Schema}'";

            var dt = await DatabaseHelper.GetDataTableAsync(query, connectionString);
            if (dt == null || dt.Rows.Count == 0)
                return views;

            foreach (DataRow row in dt.Rows)
            {
                string viewName = row["ViewName"]?.ToString() ?? "";
                string schema = row["SchemaName"]?.ToString() ?? "";
                views.Add((viewName, schema));
            }

            return views;
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
            string sql = $@"SELECT
    cc.name AS ConstraintName,
    s.name AS SchemaName,
    t.name AS TableName,
    cc.definition AS CheckDefinition
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{ObjectName.Schema}' AND t.name = '{ObjectName.Name}'
ORDER BY cc.name;";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
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
            string sql = $@"SELECT
    dc.name AS ConstraintName,
    s.name AS SchemaName,
    t.name AS TableName,
    c.name AS ColumnName,
    dc.definition AS DefaultDefinition
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{ObjectName.Schema}' AND t.name = '{ObjectName.Name}'
ORDER BY dc.name;";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
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
            string sql = $@"SELECT
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
WHERE s.name = '{ObjectName.Schema}' AND tp.name = '{ObjectName.Name}'
ORDER BY fk.name, fkc.constraint_column_id;";

            // Use DatabaseHelper to get the data as a DataTable
            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
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
        internal async Task<Dictionary<string, (int SeedValue, int IncrementValue)>> GetIdentityColumns(string connectionString)
        {
            var identityColumns = new Dictionary<string, (int SeedValue, int IncrementValue)>();

            string identityQuery = $@"
SELECT
    ic.name AS identity_column_name,
    ic.seed_value,
    ic.increment_value
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = '{ObjectName.Name}'
AND s.name = '{ObjectName.Schema}';";

            var dt = await DatabaseHelper.GetDataTableAsync(identityQuery, connectionString);
            if (dt == null || dt.Rows.Count == 0)
                return identityColumns;

            foreach (DataRow row in dt.Rows)
            {
                string columnName = row["identity_column_name"]?.ToString() ?? string.Empty;
                int seedValue = row["seed_value"] != DBNull.Value ? Convert.ToInt32(row["seed_value"]) : 0;
                int incrementValue = row["increment_value"] != DBNull.Value ? Convert.ToInt32(row["increment_value"]) : 0;
                if (!string.IsNullOrEmpty(columnName))
                    identityColumns[columnName] = (seedValue, incrementValue);
            }

            return identityColumns;
        }

        /// <summary>
        /// Opens the data object.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        internal async Task<bool> OpenAsync(ObjectName objectName, string connectionString)
        {
            this.ObjectName = objectName;
            this.ConnectionString = connectionString;

            var objectType = objectName.ObjectType;
            if (objectType == ObjectTypeEnums.None)
            {
                // determine the object type using DatabaseHelper
                string sql = $@"
SELECT TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = '{objectName.Schema}'
  AND TABLE_NAME = '{objectName.Name}'";

                var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string? objType = dt.Rows[0]["TABLE_TYPE"]?.ToString();
                    if (!string.IsNullOrEmpty(objType))
                    {
                        objectType = objType.Equals("view", StringComparison.CurrentCultureIgnoreCase)
                            ? ObjectTypeEnums.View
                            : ObjectTypeEnums.Table;
                    }
                }
            }

            return objectType switch
            {
                ObjectTypeEnums.Table or ObjectTypeEnums.View => await OpenTableAsync(),
                ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => await OpenFunctionAsync(),
                _ => false,
            };
        }

        /// <summary>
        /// Gets the function parameters async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private static async Task<DataTable?> GetParametersAsync(ObjectName objectName, string connectionString)
        {
            string paramSql = $@"
SELECT
    p.ORDINAL_POSITION,
    p.PARAMETER_NAME,
    p.DATA_TYPE,
    p.CHARACTER_MAXIMUM_LENGTH,
    p.PARAMETER_MODE
FROM INFORMATION_SCHEMA.PARAMETERS p
WHERE p.SPECIFIC_SCHEMA = N'{objectName.Schema}'
  AND p.SPECIFIC_NAME = N'{objectName.Name}'
  AND p.ORDINAL_POSITION > 0
ORDER BY p.ORDINAL_POSITION";

            return await DatabaseHelper.GetDataTableAsync(paramSql, connectionString);
        }

        /// <summary>
        /// Gets the column desc.
        /// </summary>
        private async Task GetColumnDescAsync()
        {
            string sql = $@"
SELECT C.Name, E.value AS Description
FROM sys.schemas S
INNER JOIN sys.{(ObjectName?.ObjectType == ObjectTypeEnums.Table ? "tables" : "views")} T ON S.schema_id = T.schema_id
INNER JOIN sys.columns C ON T.object_id = C.object_id
INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id
WHERE E.name = N'MS_Description'
  AND S.name = '{ObjectName?.Schema}'
  AND T.name = '{ObjectName?.Name}'";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                var columnName = row["Name"]?.ToString();
                var description = row["Description"]?.ToString();
                if (!string.IsNullOrEmpty(columnName) && description != null)
                {
                    var column = Columns.Find(col => col.ColumnName == columnName);
                    if (column != null)
                    {
                        column.Description = description;
                    }
                }
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

            if (ConnectionString.Length > 0 && ObjectName.Name.Length > 0)
            {
                string sql = $@"
SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, IS_NULLABLE, COLUMN_DEFAULT
FROM information_schema.columns
WHERE TABLE_SCHEMA = '{ObjectName.Schema}' AND TABLE_NAME = '{ObjectName.Name}'
ORDER BY ORDINAL_POSITION";

                var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);

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

            string schemaName = ObjectName.Schema;
            string tableName = ObjectName.Name;

            string sql = $@"
SELECT
    kc.name AS ConstraintName,
    kc.type_desc AS ConstraintType,
    c.name AS ColumnName
FROM sys.key_constraints kc
INNER JOIN sys.tables t ON kc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
LEFT JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
LEFT JOIN sys.columns c ON c.object_id = t.object_id AND c.column_id = ic.column_id
WHERE s.name = '{schemaName}'
  AND t.name = '{tableName}'
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
WHERE s.name = '{schemaName}'
  AND t.name = '{tableName}'
UNION
SELECT
    cc.name AS ConstraintName,
    'CHECK_CONSTRAINT' AS ConstraintType,
    c.name AS ColumnName
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
LEFT JOIN sys.columns c ON cc.parent_object_id = c.object_id AND cc.parent_column_id = c.column_id
WHERE s.name = '{schemaName}'
  AND t.name = '{tableName}'
UNION
SELECT
    dc.name AS ConstraintName,
    'DEFAULT_CONSTRAINT' AS ConstraintType,
    c.name AS ColumnName
FROM sys.default_constraints dc
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE s.name = '{schemaName}'
  AND t.name = '{tableName}'";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
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

            // Query to get all indexes and their columns for the table/view
            var sql = $@"
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
WHERE s.name = '{objectName.Schema}'
  AND t.name = '{objectName.Name}'
  AND ind.is_primary_key = 0 -- Exclude PK, unless you want to include it
ORDER BY ind.name, ic.key_ordinal";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
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
                string columns = string.Join(", ", group.Select(r => $"[{r["ColumnName"]}]"));

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
        /// Gets the indexes.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        private async Task GetIndexesAsync1(ObjectName? objectName)
        {
            if (objectName == null || objectName.IsEmpty())
                return;

            var sql = $@"
SELECT DISTINCT col.name AS ColumnName
FROM sys.indexes ind
INNER JOIN sys.index_columns ic ON ind.object_id = ic.object_id AND ind.index_id = ic.index_id
INNER JOIN sys.columns col ON ic.object_id = col.object_id AND ic.column_id = col.column_id
INNER JOIN sys.tables t ON ind.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{objectName.Schema}'
AND t.name = '{objectName.Name}'";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string? columnName = row["ColumnName"]?.ToString();
                if (string.IsNullOrEmpty(columnName))
                    continue;

                // find the column in the Columns
                var column = Columns.Find(c => c.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
                if (column != null)
                {
                    if (column.Ord == null || !column.Ord.EndsWith("🗝"))
                        column.Ord += "🔢";
                }
            }
        }

        /// <summary>
        /// Gets the object description async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<string> GetObjectDescriptionAsync()
        {
            string result = string.Empty;
            if (ObjectName != null)
            {
                try
                {
                    var objectType = ObjectName.ObjectType switch
                    {
                        ObjectTypeEnums.Table => "table",
                        ObjectTypeEnums.View => "view",
                        ObjectTypeEnums.StoredProcedure => "procedure",
                        ObjectTypeEnums.Function => "function",
                        _ => ""
                    };

                    if (string.IsNullOrEmpty(objectType)) return string.Empty;

                    string sql = $"SELECT value FROM fn_listextendedproperty (NULL, 'schema', '{ObjectName.Schema}', '{objectType}', '{ObjectName.Name}', default, default) WHERE name = N'MS_Description'";

                    var returnValue = await DatabaseHelper.ExecuteScalarAsync(sql, ConnectionString);

                    if (returnValue != null && returnValue != DBNull.Value)
                    {
                        result = returnValue.ToString() ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Common.MsgBox(ex.Message, MessageBoxIcon.Error);
                }
            }

            return result;
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
            string objectType = ObjectName.ObjectType == ObjectName.ObjectTypeEnums.StoredProcedure ? "PROCEDURE" : "FUNCTION";

            string sql = $@"
SELECT p.name AS ParameterName,
    CAST(ep.value AS VARCHAR(MAX)) AS PropertyValue
FROM sys.extended_properties AS ep
INNER JOIN sys.objects AS o ON ep.major_id = o.object_id
INNER JOIN sys.schemas AS s ON o.schema_id = s.schema_id
LEFT JOIN sys.parameters AS p
    ON ep.major_id = p.object_id AND ep.minor_id = p.parameter_id
WHERE ep.name = 'MS_Description'
  AND p.name IS NOT NULL
  AND o.type IN ('FN', 'TF', 'IF', 'P', 'PC')
  AND o.name = '{ObjectName.Name}'
  AND s.name = '{ObjectName.Schema}'";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string paramName = row["ParameterName"]?.ToString() ?? "";
                string desc = row["PropertyValue"]?.ToString() ?? "";

                // Parameter names in sys.parameters start with '@', match accordingly
                var param = Parameters.Find(p => p.Name.Equals(paramName, StringComparison.OrdinalIgnoreCase));
                if (param != null)
                {
                    param.Description = desc;
                }
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

            var sql = $@"
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
  AND TABLE_NAME = '{objectName.Name}'
  AND TABLE_SCHEMA = '{objectName.Schema}'";

            var dt = await DatabaseHelper.GetDataTableAsync(sql, ConnectionString);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                var columnName = row["COLUMN_NAME"]?.ToString();
                if (string.IsNullOrEmpty(columnName))
                    continue;

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
        /// <param name="objectName">The object name.</param>
        /// <param string connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        private async Task<bool> OpenFunctionAsync()
        {
            //get the object description and definition
            Description = await GetObjectDescriptionAsync();
            Definition = await DatabaseHelper.GetObjectDefinitionAsync(ObjectName, ConnectionString);

            // Get the function parameters
            Parameters.Clear();
            var dtParameters = await GetParametersAsync(ObjectName, ConnectionString);
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
        /// Opens the table or view.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<bool> OpenTableAsync()
        {
            bool result = true;

            if (ConnectionString.Length > 0 && ObjectName.Name.Length > 0)
            {
                result = await GetColumnsAsync();

                if (ObjectName?.ObjectType == ObjectTypeEnums.Table)
                {
                    await GetPrimaryKeysAsync(ObjectName);

                    await GetConstraintsAsync();
                }

                await GetIndexesAsync(ObjectName);

                Description = await GetObjectDescriptionAsync();
                await GetColumnDescAsync();

                // get the definition if it is a view
                if (ObjectName.ObjectType == ObjectTypeEnums.View)
                {
                    Definition = await DatabaseHelper.GetObjectDefinitionAsync(ObjectName, ConnectionString);
                }
            }

            return result;
        }
    }
}