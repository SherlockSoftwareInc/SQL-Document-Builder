# Database Schema Retrieval Functions

## Scope

This document summarizes functions in the project that fetch **database schema information** (objects, columns, keys, constraints, dependencies, and definitions), primarily for SQL Server.

---

## 1) Core metadata provider: `SQLDatabaseHelper`

`SQLDatabaseHelper` is the central data-access layer for schema/metadata retrieval.

### Object discovery
- `GetDatabaseObjectsAsync(ObjectTypeEnums tableType, string connectionString)`
  - Dispatcher for object-type retrieval.
- `GetAllObjectsAsync(string? connectionString)`
  - Returns all supported objects from `sys.objects` + `sys.schemas`.
- `GetTablesAsync(string? connectionString)`
  - Returns tables from `INFORMATION_SCHEMA.TABLES`.
- `GetViewsAsync(string? connectionString)`
  - Returns views from `INFORMATION_SCHEMA.TABLES`.
- `GetStoredProceduresAsync(string? connectionString)`
  - Returns procedures from `sys.procedures`.
- `GetFunctionsAsync(string? connectionString)`
  - Returns functions (`FN/IF/TF`) from `sys.objects`.
- `GetTriggersAsync(string connectionString)`
  - Returns triggers and schema context.
- `GetSynonymsAsync(string connectionString)`
  - Returns synonyms and schema context.
- `GetSchemasAsync(string? connectionString)`
  - Returns available schemas from `sys.schemas`.
- `GetDatabases(string serverName, CancellationToken cancellationToken)`
  - Uses `SqlConnection.GetSchema("Databases")` to list databases on a server.

### Object details and text metadata
- `GetObjectDefinitionAsync(ObjectName objectName, string? connectionString)`
  - Retrieves source text from `sys.sql_modules`.
- `GetObjectDescriptionAsync(ObjectName? objectName, string? connectionString)`
  - Reads `MS_Description` extended property (object-level).
- `GetColumnDescriptionAsync(ObjectName objectName, string column, string connectionString)`
  - Reads `MS_Description` for table/view columns.

### Structural metadata and relationships
- `HasIdentityColumnAsync(ObjectName? objectName, string? connectionString)`
  - Detects identity columns in a table.
- `GetReferencingObjectsAsync(ObjectName objectName, string connectionString)`
  - Objects that depend on this object (`sys.sql_expression_dependencies`).
- `GetReferencedObjectsAsync(ObjectName objectName, string connectionString)`
  - Objects this object depends on.
- `GetForeignTablesAsync(ObjectName objectName, string connectionString)`
  - FK mapping (from-column to referenced schema/table/column).
- `GetObjectsUsingTableAsync(ObjectName objectName, string connectionString)`
  - Returns objects referencing a table via expression dependencies.
- `GetRecentObjects(DateTime startDate, DateTime endDate, string connectionString)`
  - Objects created/modified in date range.

### Generic query helper used by schema loaders
- `GetDataTableAsync(string sql, string? connectionString)`
  - Base method used by most metadata queries.

---

## 2) Object-level schema assembly: `DBObject`

`DBObject` composes complete metadata for a selected object by calling `SQLDatabaseHelper` and running additional metadata queries.

### Entry point
- `OpenAsync(ObjectName objectName, DatabaseConnectionItem? connection)`
  - Routes by object type to schema loaders:
    - table/view -> `OpenTableAsync()`
    - proc/function -> `OpenFunctionAsync()`
    - trigger -> `OpenTriggerAsync()`
    - synonym -> `OpenSynonymAsync()`

### Table/view metadata loading
- `GetColumnsAsync()`
  - Loads column definitions from `INFORMATION_SCHEMA.COLUMNS`.
- `GetPrimaryKeysAsync(ObjectName objectName)`
  - Loads PK columns from `INFORMATION_SCHEMA.KEY_COLUMN_USAGE`.
- `GetConstraintsAsync()`
  - Loads PK/FK/CHECK/DEFAULT constraints from `sys.*`.
- `GetIndexesAsync(ObjectName? objectName)`
  - Loads non-PK indexes and indexed columns from `sys.indexes` joins.
- `GetColumnDescAsync()`
  - Loads column extended properties.

### Procedure/function metadata loading
- `GetParametersAsync(ObjectName objectName, DatabaseConnectionItem connection)`
  - Loads parameters from `INFORMATION_SCHEMA.PARAMETERS`.
- `GetParameterDescAsync()`
  - Loads parameter descriptions from extended properties.

### Object relationships/fk wrappers
- `GetReferencingObjectsAsync(DatabaseConnectionItem connection)`
- `GetReferencedObjectsAsync(DatabaseConnectionItem connection)`
- `GetForeignTablesAsync(DatabaseConnectionItem connection)`

### Additional table structure helper
- `GetIdentityColumns(DatabaseConnectionItem connection)`
  - Returns identity seed/increment per identity column.

---

## 3) Specialized metadata models (`OpenAsync` loaders)

These classes retrieve object-specific schema summary data:

- `TableInfo.OpenAsync(ObjectName objectName, string connectionString)`
  - Table create/modify date, row count, data size.
- `ViewInfo.OpenAsync(ObjectName objectName, string connectionString)`
  - View create/modify date.
- `ProcedureInfo.OpenAsync(ObjectName objectName, string connectionString)`
  - Procedure create/modify date and system-object flag.
- `FunctionInfo.OpenAsync(ObjectName objectName, string connectionString)`
  - Function create/modify date, system-object flag, function type.
- `TriggerInfo.OpenAsync(ObjectName objectName, string connectionString)`
  - Trigger type, parent object, disabled status, dates.
- `SynonymInfo.OpenAsync(ObjectName objectName, string connectionString)`
  - Base object name/type and dates.

---

## 4) UI flows that trigger schema retrieval

### `TableBuilderForm`
- `ChangeDBConnectionAsync(DatabaseConnectionItem connection)`
  - Connects and loads `_allObjects` via `GetAllObjectsAsync`.
- `ObjectTypeComboBox_SelectedIndexChanged(...)`
  - Loads selected object-type list via `GetDatabaseObjectsAsync`.

### `DBObjectsSelectForm`
- `PopulateSchemasAsync()`
  - Loads schema list via `GetSchemasAsync`.
- Clipboard import flow uses `GetAllObjectsAsync` for object matching.

---

## 5) Related notes from reviewed open files

- `ObjectName.cs` supports schema/object parsing and type mapping (`ConvertObjectType`), used by metadata retrieval flows.
- `InputBox.cs` and `SqlEditBox.cs` do not perform database schema retrieval directly.

---

## Summary

Schema retrieval is centralized in `SQLDatabaseHelper`, with `DBObject` orchestrating full object metadata assembly for documentation and script generation workflows. UI forms (`TableBuilderForm`, `DBObjectsSelectForm`) call into these layers to populate schemas, object lists, and detailed metadata views.