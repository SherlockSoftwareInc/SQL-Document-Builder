using OctofyPro.SchemaProvider.Core.Abstractions;
using OctofyPro.SchemaProvider.Core.Models;
using OctofyPro.SchemaProvider.Core.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    public sealed class DBSchema
    {
        private static readonly string[] SqlServerInformationSchemaViews =
        [
            "CHECK_CONSTRAINTS",
            "COLUMN_DOMAIN_USAGE",
            "COLUMN_PRIVILEGES",
            "COLUMNS",
            "CONSTRAINT_COLUMN_USAGE",
            "CONSTRAINT_TABLE_USAGE",
            "DOMAIN_CONSTRAINTS",
            "DOMAINS",
            "KEY_COLUMN_USAGE",
            "PARAMETERS",
            "REFERENTIAL_CONSTRAINTS",
            "ROUTINES",
            "SCHEMATA",
            "TABLE_CONSTRAINTS",
            "TABLE_PRIVILEGES",
            "TABLES",
            "VIEW_COLUMN_USAGE",
            "VIEW_TABLE_USAGE",
            "VIEWS"
        ];

        private static readonly string[] SqlServerBuiltInFunctions =
        [
            "GETDATE", "SYSDATETIME", "DATEADD", "DATEDIFF", "DATENAME",
            "ISNULL", "COALESCE", "NULLIF", "CAST", "CONVERT", "TRY_CAST", "TRY_CONVERT",
            "IIF", "CHOOSE", "FORMAT", "NEWID", "ABS", "ROUND", "CEILING", "FLOOR",
            "POWER", "SQRT", "LEN", "LEFT", "RIGHT", "SUBSTRING", "REPLACE", "LOWER", "UPPER",
            "CONCAT", "STRING_AGG", "COUNT", "SUM", "AVG", "MIN", "MAX"
        ];

        private DatabaseConnectionItem? _connection;
        private readonly List<string> _schemas = [];
        private readonly HashSet<string> _schemaSet = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<DBSchemaItem> _tables = [];
        private readonly List<DBSchemaItem> _views = [];
        private readonly List<DBSchemaItem> _functions = [];
        private readonly List<DBSchemaItem> _storedProcedures = [];
        private readonly List<DBSchemaItem> _triggers = [];
        private readonly List<DBSchemaItem> _synonyms = [];
        private readonly Dictionary<string, string> _objectDescriptions = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Dictionary<string, string>> _level2Descriptions = new(StringComparer.OrdinalIgnoreCase);

        public string Catalog { get; private set; } = string.Empty;

        public DatabaseConnectionItem? Connection => _connection;

        public bool Schemaless { get; private set; }

        public IReadOnlyList<string> Schemas => _schemas;

        public List<DBSchemaItem> DataObjects => [.. _tables, .. _views];

        public List<DBSchemaItem> FunctionObjects => _functions;

        public bool HasObjects => _tables.Count > 0
            || _views.Count > 0
            || _functions.Count > 0
            || _storedProcedures.Count > 0
            || _triggers.Count > 0
            || _synonyms.Count > 0;

        public bool HasColumns => _tables.Any(t => t.Columns.Count > 0) || _views.Any(v => v.Columns.Count > 0);

        public async Task<bool> OpenAsync(DatabaseConnectionItem? connection, bool loadColumns, CancellationToken token = default)
        {
            if (connection == null)
            {
                return false;
            }

            _connection = connection;
            Catalog = ReadConnectionCatalog(connection);
            ClearData();

            if (!await InitializeAsync(token))
            {
                return false;
            }

            if (!await LoadAllObjectsAsync(token))
            {
                return false;
            }

            if (loadColumns && !await LoadAllColumnsAsync(token))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> InitializeAsync(CancellationToken token = default)
        {
            if (_connection == null)
            {
                return false;
            }

            _schemas.Clear();
            _schemaSet.Clear();

            try
            {
                await using var provider = await ConnectAsync(_connection, token);
                var schemas = await provider.GetSchemasAsync(token);

                foreach (var schema in schemas)
                {
                    if (ShouldSkipSchema(schema))
                    {
                        continue;
                    }

                    AddSchema(schema);
                }

                if (IsSqlServer(_connection))
                {
                    AddSchema("INFORMATION_SCHEMA");
                    AddSchema("sys");
                }

                _schemas.Sort(StringComparer.OrdinalIgnoreCase);
                Schemaless = _schemas.Count == 0;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoadAllObjectsAsync(CancellationToken token = default)
        {
            if (_connection == null)
            {
                return false;
            }

            _tables.Clear();
            _views.Clear();
            _functions.Clear();

            try
            {
                await using var provider = await ConnectAsync(_connection, token);

                var tables = await provider.GetAllTablesAsync(string.Empty, token);
                var views = await provider.GetAllViewsAsync(string.Empty, token);
                var functions = await provider.GetAllFunctionsAsync(string.Empty, token);

                foreach (var table in tables)
                {
                    AddObjectIfInSchema(_tables, ReadSchema(table), ReadObjectName(table));
                }

                foreach (var view in views)
                {
                    AddObjectIfInSchema(_views, ReadSchema(view), ReadObjectName(view));
                }

                foreach (var function in functions)
                {
                    AddObjectIfInSchema(_functions, ReadSchema(function), ReadObjectName(function));
                }

                if (IsSqlServer(_connection))
                {
                    foreach (var viewName in SqlServerInformationSchemaViews)
                    {
                        AddObjectIfInSchema(_views, "INFORMATION_SCHEMA", viewName);
                    }

                    foreach (var functionName in SqlServerBuiltInFunctions)
                    {
                        AddObjectIfInSchema(_functions, "sys", functionName);
                    }
                }

                SortObjects(_tables);
                SortObjects(_views);
                SortObjects(_functions);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoadAllColumnsAsync(CancellationToken token = default)
        {
            if (_connection == null)
            {
                return false;
            }

            try
            {
                await using var provider = await ConnectAsync(_connection, token);
                var lookup = await provider.GetAllColumnsAsync(token);

                foreach (var item in DataObjects)
                {
                    token.ThrowIfCancellationRequested();

                    item.Columns.Clear();

                    if (lookup.TryGetValue((item.SchemaName, item.ObjectName), out var rawColumns))
                    {
                        var ordinal = 1;
                        foreach (var rawColumn in rawColumns)
                        {
                            var column = CreateDbColumn(rawColumn, ordinal++);
                            if (column != null)
                            {
                                item.Columns.Add(column);
                            }
                        }

                        continue;
                    }

                    if (item.SchemaName.Equals("INFORMATION_SCHEMA", StringComparison.OrdinalIgnoreCase))
                    {
                        var fallbackColumns = await provider.GetColumnsAsync(item.SchemaName, item.ObjectName, token);
                        var ordinal = 1;
                        foreach (var fallbackColumn in fallbackColumns)
                        {
                            var column = CreateDbColumn(fallbackColumn, ordinal++);
                            if (column != null)
                            {
                                item.Columns.Add(column);
                            }
                        }
                    }
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoadObjectsForSchemaAsync(string schemaName, CancellationToken token = default)
        {
            if (_connection == null || string.IsNullOrWhiteSpace(schemaName))
            {
                return false;
            }

            try
            {
                await using var provider = await ConnectAsync(_connection, token);

                var tables = await provider.GetAllTablesAsync(schemaName, token);
                var views = await provider.GetAllViewsAsync(schemaName, token);
                var functions = await provider.GetAllFunctionsAsync(schemaName, token);

                foreach (var table in tables)
                {
                    AddObjectIfInSchema(_tables, ReadSchema(table), ReadObjectName(table), skipDuplicates: true);
                }

                foreach (var view in views)
                {
                    AddObjectIfInSchema(_views, ReadSchema(view), ReadObjectName(view), skipDuplicates: true);
                }

                foreach (var function in functions)
                {
                    AddObjectIfInSchema(_functions, ReadSchema(function), ReadObjectName(function), skipDuplicates: true);
                }

                SortObjects(_tables);
                SortObjects(_views);
                SortObjects(_functions);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<string> Tables(string schemaName) =>
            _tables
                .Where(t => IsSchemaMatch(t.SchemaName, schemaName))
                .Select(t => t.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<string> Views(string schemaName) =>
            _views
                .Where(v => IsSchemaMatch(v.SchemaName, schemaName))
                .Select(v => v.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<string> Functions(string schemaName = "") =>
            _functions
                .Where(f => string.IsNullOrWhiteSpace(schemaName) || IsSchemaMatch(f.SchemaName, schemaName))
                .Select(f => f.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<string> StoredProcedures(string schemaName = "") =>
            _storedProcedures
                .Where(p => string.IsNullOrWhiteSpace(schemaName) || IsSchemaMatch(p.SchemaName, schemaName))
                .Select(p => p.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<string> Triggers(string schemaName = "") =>
            _triggers
                .Where(t => string.IsNullOrWhiteSpace(schemaName) || IsSchemaMatch(t.SchemaName, schemaName))
                .Select(t => t.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<string> Synonyms(string schemaName = "") =>
            _synonyms
                .Where(s => string.IsNullOrWhiteSpace(schemaName) || IsSchemaMatch(s.SchemaName, schemaName))
                .Select(s => s.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<ObjectName> Objects(ObjectTypeEnums objectType, string schemaName = "")
        {
            return objectType switch
            {
                ObjectTypeEnums.Table => ToObjectNames(_tables, ObjectTypeEnums.Table, schemaName),
                ObjectTypeEnums.View => ToObjectNames(_views, ObjectTypeEnums.View, schemaName),
                ObjectTypeEnums.StoredProcedure => ToObjectNames(_storedProcedures, ObjectTypeEnums.StoredProcedure, schemaName),
                ObjectTypeEnums.Function => ToObjectNames(_functions, ObjectTypeEnums.Function, schemaName),
                ObjectTypeEnums.Trigger => ToObjectNames(_triggers, ObjectTypeEnums.Trigger, schemaName),
                ObjectTypeEnums.Synonym => ToObjectNames(_synonyms, ObjectTypeEnums.Synonym, schemaName),
                ObjectTypeEnums.All => AllObjects(schemaName),
                _ => []
            };
        }

        public List<ObjectName> AllObjects(string schemaName = "")
        {
            var result = new List<ObjectName>();
            result.AddRange(ToObjectNames(_tables, ObjectTypeEnums.Table, schemaName));
            result.AddRange(ToObjectNames(_views, ObjectTypeEnums.View, schemaName));
            result.AddRange(ToObjectNames(_storedProcedures, ObjectTypeEnums.StoredProcedure, schemaName));
            result.AddRange(ToObjectNames(_functions, ObjectTypeEnums.Function, schemaName));
            result.AddRange(ToObjectNames(_triggers, ObjectTypeEnums.Trigger, schemaName));
            result.AddRange(ToObjectNames(_synonyms, ObjectTypeEnums.Synonym, schemaName));
            return result;
        }

        public List<string> SearchObjects(string schema, string likePattern)
        {
            var regex = CreateLikeRegex(likePattern);

            return DataObjects
                .Where(o => (string.IsNullOrWhiteSpace(schema) || IsSchemaMatch(o.SchemaName, schema))
                    && regex.IsMatch(o.ObjectName))
                .Select(o => o.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<string> SearchColumns(string schema, string likePattern)
        {
            var regex = CreateLikeRegex(likePattern);

            return DataObjects
                .Where(o => (string.IsNullOrWhiteSpace(schema) || IsSchemaMatch(o.SchemaName, schema))
                    && o.Columns.Any(c => regex.IsMatch(ReadColumnName(c))))
                .Select(o => o.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<string> GetObjectNames() =>
            DataObjects
                .Select(o => o.ObjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<string> GetColumnNames() =>
            DataObjects
                .SelectMany(o => o.Columns)
                .Select(ReadColumnName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public List<DBColumn> GetCachedColumns(ObjectName objectName)
        {
            if (objectName == null || objectName.IsEmpty())
            {
                return [];
            }

            var source = objectName.ObjectType switch
            {
                ObjectTypeEnums.Table => _tables,
                ObjectTypeEnums.View => _views,
                _ => null
            };

            if (source == null)
            {
                return [];
            }

            var item = source.Find(i =>
                i.SchemaName.Equals(objectName.Schema, StringComparison.OrdinalIgnoreCase)
                && i.ObjectName.Equals(objectName.Name, StringComparison.OrdinalIgnoreCase));

            if (item == null || item.Columns.Count == 0)
            {
                return [];
            }

            return item.Columns.Select(CloneColumn).ToList();
        }

        public async Task<string> GetObjectDescriptionAsync(ObjectName objectName, CancellationToken token = default)
        {
            if (_connection == null || objectName == null || objectName.IsEmpty())
            {
                return string.Empty;
            }

            var key = GetObjectCacheKey(objectName);
            if (_objectDescriptions.TryGetValue(key, out var cachedValue))
            {
                return cachedValue;
            }

            try
            {
                await using var provider = await ConnectAsync(_connection, token);
                var description = await provider.GetObjectDescriptionAsync(objectName.Schema, objectName.Name, token);
                description ??= string.Empty;
                _objectDescriptions[key] = description;
                return description;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GetLevel2DescriptionAsync(ObjectName objectName, string level2Name, CancellationToken token = default)
        {
            if (_connection == null || objectName == null || objectName.IsEmpty() || string.IsNullOrWhiteSpace(level2Name))
            {
                return string.Empty;
            }

            var objectKey = GetObjectCacheKey(objectName);
            if (!_level2Descriptions.TryGetValue(objectKey, out var objectDescriptions))
            {
                objectDescriptions = await LoadLevel2DescriptionsAsync(objectName, token);
                _level2Descriptions[objectKey] = objectDescriptions;
            }

            return objectDescriptions.TryGetValue(level2Name, out var value) ? value : string.Empty;
        }

        public void Clear()
        {
            _connection = null;
            Catalog = string.Empty;
            ClearData();
        }

        private void ClearData()
        {
            _schemas.Clear();
            _schemaSet.Clear();
            _tables.Clear();
            _views.Clear();
            _functions.Clear();
            _storedProcedures.Clear();
            _triggers.Clear();
            _synonyms.Clear();
            _objectDescriptions.Clear();
            _level2Descriptions.Clear();
            Schemaless = false;
        }

        private static async Task<IDatabaseSchemaProvider> ConnectAsync(DatabaseConnectionItem connection, CancellationToken token)
        {
            var providerKind = ResolveProviderKind(connection);
            var provider = DatabaseSchemaProviderFactory.Create(providerKind);
            await provider.ConnectAsync(ReadConnectionString(connection), token);
            return provider;
        }

        private static DatabaseProviderKind ResolveProviderKind(DatabaseConnectionItem connection)
        {
            var connectionType = ReadConnectionType(connection);
            return connectionType.Contains("odbc", StringComparison.OrdinalIgnoreCase)
                ? DatabaseProviderKind.Odbc
                : DatabaseProviderKind.SqlServer;
        }

        private static bool IsSqlServer(DatabaseConnectionItem connection) =>
            ReadConnectionType(connection).Contains("sql server", StringComparison.OrdinalIgnoreCase)
            || ReadConnectionType(connection).Contains("mssql", StringComparison.OrdinalIgnoreCase);

        private void AddSchema(string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
            {
                return;
            }

            if (_schemaSet.Add(schemaName))
            {
                _schemas.Add(schemaName);
            }
        }

        private static bool ShouldSkipSchema(string schemaName) =>
            string.IsNullOrWhiteSpace(schemaName);

        private bool IsInSchemaList(string schemaName) =>
            Schemaless || _schemaSet.Count == 0 || _schemaSet.Contains(schemaName);

        private void AddObjectIfInSchema(List<DBSchemaItem> target, string schemaName, string objectName, bool skipDuplicates = false)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(schemaName) && !IsInSchemaList(schemaName))
            {
                return;
            }

            var schema = schemaName ?? string.Empty;
            if (skipDuplicates && HasObjectInList(target, schema, objectName))
            {
                return;
            }

            target.Add(new DBSchemaItem(schema, objectName));
        }

        private static bool HasObjectInList(List<DBSchemaItem> list, string schemaName, string objectName) =>
            list.Any(i => i.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase)
                && i.ObjectName.Equals(objectName, StringComparison.OrdinalIgnoreCase));

        private static void SortObjects(List<DBSchemaItem> list) =>
            list.Sort((a, b) =>
            {
                var schemaCompare = StringComparer.OrdinalIgnoreCase.Compare(a.SchemaName, b.SchemaName);
                return schemaCompare != 0
                    ? schemaCompare
                    : StringComparer.OrdinalIgnoreCase.Compare(a.ObjectName, b.ObjectName);
            });

        private static bool IsSchemaMatch(string left, string right) =>
            string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        private static string GetObjectCacheKey(ObjectName objectName) =>
            $"{objectName.ObjectType}|{objectName.Schema}|{objectName.Name}";

        private async Task<Dictionary<string, string>> LoadLevel2DescriptionsAsync(ObjectName objectName, CancellationToken token)
        {
            if (_connection == null)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                await using var provider = await ConnectAsync(_connection, token);

                IReadOnlyDictionary<string, string> descriptions = objectName.ObjectType switch
                {
                    ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function =>
                        await provider.GetParameterDescriptionsAsync(objectName.Schema, objectName.Name, token),
                    _ => await provider.GetColumnDescriptionsAsync(objectName.Schema, objectName.Name, token)
                };

                return descriptions == null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(descriptions, StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private static List<ObjectName> ToObjectNames(List<DBSchemaItem> source, ObjectTypeEnums objectType, string schemaName = "") =>
            source
                .Where(i => string.IsNullOrWhiteSpace(schemaName) || IsSchemaMatch(i.SchemaName, schemaName))
                .Select(i => new ObjectName(objectType, i.SchemaName, i.ObjectName))
                .OrderBy(i => i.Schema, StringComparer.OrdinalIgnoreCase)
                .ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

        private static DBColumn CloneColumn(DBColumn source)
        {
            var column = new DBColumn(CreateColumnDataRow(source));
            column.Description = source.Description;
            return column;
        }

        private static DataRow CreateColumnDataRow(DBColumn source)
        {
            var table = new DataTable();
            table.Columns.Add("ORDINAL_POSITION", typeof(string));
            table.Columns.Add("COLUMN_NAME", typeof(string));
            table.Columns.Add("DATA_TYPE", typeof(string));
            table.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
            table.Columns.Add("NUMERIC_PRECISION", typeof(int));
            table.Columns.Add("NUMERIC_SCALE", typeof(int));
            table.Columns.Add("DATETIME_PRECISION", typeof(int));
            table.Columns.Add("IS_NULLABLE", typeof(string));

            var row = table.NewRow();
            row["ORDINAL_POSITION"] = source.Ord;
            row["COLUMN_NAME"] = source.ColumnName;
            row["DATA_TYPE"] = source.DataType;
            row["CHARACTER_MAXIMUM_LENGTH"] = DBNull.Value;
            row["NUMERIC_PRECISION"] = DBNull.Value;
            row["NUMERIC_SCALE"] = DBNull.Value;
            row["DATETIME_PRECISION"] = DBNull.Value;
            row["IS_NULLABLE"] = source.Nullable ? "YES" : "NO";
            table.Rows.Add(row);

            return row;
        }

        private static Regex CreateLikeRegex(string likePattern)
        {
            var pattern = string.IsNullOrWhiteSpace(likePattern) ? "%" : likePattern;
            var escaped = Regex.Escape(pattern)
                .Replace("%", ".*", StringComparison.Ordinal)
                .Replace("_", ".", StringComparison.Ordinal);

            return new Regex($"^{escaped}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }

        private static string ReadSchema(object source) =>
            ReadStringProperty(source, "SchemaName", "Schema", "TableSchema", "ObjectSchema");

        private static string ReadObjectName(object source) =>
            ReadStringProperty(source,
                "ObjectName",
                "Name",
                "TableName",
                "ViewName",
                "FunctionName",
                "ProcedureName",
                "TriggerName",
                "SynonymName");

        private static string ReadColumnName(DBColumn column) =>
            ReadStringProperty(column, "ColumnName", "Name");

        private static string ReadConnectionString(DatabaseConnectionItem connection) =>
            ReadStringProperty(connection, "ConnectionString");

        private static string ReadConnectionType(DatabaseConnectionItem connection) =>
            ReadStringProperty(connection, "ConnectionType");

        private static string ReadConnectionCatalog(DatabaseConnectionItem connection) =>
            ReadStringProperty(connection, "Database", "Catalog");

        private static string ReadStringProperty(object source, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                var value = source.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
                if (value is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return s;
                }
            }

            return string.Empty;
        }

        private static int? ReadIntProperty(object source, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                var value = source.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
                if (value == null)
                {
                    continue;
                }

                if (value is int i)
                {
                    return i;
                }

                if (int.TryParse(value.ToString(), out var parsed))
                {
                    return parsed;
                }
            }

            return null;
        }

        private static bool ReadBoolProperty(object source, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                var value = source.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
                if (value is bool b)
                {
                    return b;
                }

                if (value != null && bool.TryParse(value.ToString(), out var parsed))
                {
                    return parsed;
                }
            }

            return false;
        }

        private static DBColumn? CreateDbColumn(object rawColumn, int ordinal)
        {
            var columnName = ReadStringProperty(rawColumn, "ColumnName", "Name");
            var dataType = ReadStringProperty(rawColumn, "DataType", "TypeName");
            var nullable = ReadBoolProperty(rawColumn, "IsNullable", "Nullable");
            var maxLength = ReadIntProperty(rawColumn, "MaxLength", "Length");

            var parameterlessCtor = typeof(DBColumn).GetConstructor(Type.EmptyTypes);
            if (parameterlessCtor != null)
            {
                var column = (DBColumn)parameterlessCtor.Invoke(null);
                SetPropertyIfExists(column, "Ord", ordinal.ToString());
                SetPropertyIfExists(column, "ColumnName", columnName);
                SetPropertyIfExists(column, "DataType", BuildDataType(dataType, maxLength));
                SetPropertyIfExists(column, "Nullable", nullable);
                return column;
            }

            var dataRowCtor = typeof(DBColumn).GetConstructor(new[] { typeof(DataRow) });
            if (dataRowCtor != null)
            {
                var table = new DataTable();
                table.Columns.Add("ORDINAL_POSITION", typeof(string));
                table.Columns.Add("COLUMN_NAME", typeof(string));
                table.Columns.Add("DATA_TYPE", typeof(string));
                table.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
                table.Columns.Add("IS_NULLABLE", typeof(string));

                var row = table.NewRow();
                row["ORDINAL_POSITION"] = ordinal.ToString();
                row["COLUMN_NAME"] = columnName;
                row["DATA_TYPE"] = dataType;
                row["CHARACTER_MAXIMUM_LENGTH"] = maxLength ?? (object)DBNull.Value;
                row["IS_NULLABLE"] = nullable ? "YES" : "NO";
                table.Rows.Add(row);

                return (DBColumn)dataRowCtor.Invoke(new object[] { row });
            }

            return null;
        }

        private static string BuildDataType(string dataType, int? maxLength)
        {
            if (string.IsNullOrWhiteSpace(dataType))
            {
                return string.Empty;
            }

            if (maxLength is null or 0)
            {
                return dataType;
            }

            if (maxLength == -1)
            {
                return $"{dataType}(MAX)";
            }

            return $"{dataType}({maxLength})";
        }

        private static void SetPropertyIfExists(object target, string propertyName, object? value)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            property.SetValue(target, value);
        }
    }
}
