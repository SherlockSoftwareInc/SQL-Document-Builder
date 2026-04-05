# DBSchema Metadata Refactor Plan

## Goal
After `TableBuilderForm` changes the active connection, load database schema metadata into a local in-memory `DBSchema` instance, and route schema metadata consumers through this cache.

## Scope
- `TableBuilderForm` owns and refreshes a connection-scoped `DBSchema` snapshot.
- UI object browsing (schema/object type/search) reads from `DBSchema`.
- Select dialogs and batch tools that are opened from `TableBuilderForm` can consume the same in-memory snapshot.
- Expand `DBSchema` where metadata is currently required but not represented.
- Extend provider integration where required metadata is not currently exposed by `DBSchema`.

## Current Metadata Consumers
- `TableBuilderForm`: object list, schema list, search, post-execute refresh.
- `DBObjectsSelectForm`: object type list, schema list, paste-lookup.
- `BatchColumnDesc`: schema list and per-object column scan.
- `ColumnReferenceDialog`: table list and schema filter.
- `ColumnDefView` / `DBObject`: object details metadata loading.
- `DocumentBuilder`, `JsonBuilder`, `ObjectDescription`: object/column metadata for generated output.

## Implementation Steps

### Step 1: Add design document and baseline
- Add this plan to `docs/`.

### Step 2: Expand `DBSchema` object inventory support
- Add in-memory support for:
  - Stored procedures
  - Triggers
  - Synonyms
- Add query APIs for these object types and unified all-object projection.
- Ensure `LoadAllObjectsAsync` and `LoadObjectsForSchemaAsync` include all supported object types.

### Step 3: Wire `TableBuilderForm` to `DBSchema`
- Add a local field: `private readonly DBSchema _dbSchema = new();`
- On successful connection open:
  - Load `_dbSchema` using `OpenAsync(connection, loadColumns: true, token)`.
  - Populate `_allObjects`/`_tables` from `_dbSchema` projections (transitional compatibility).
- Replace object type loading (`ObjectTypeComboBox_SelectedIndexChanged`) with `_dbSchema` lookups.
- Replace post-execute metadata refresh with `_dbSchema` refresh.
- Keep cancellation behavior compatible with existing connection-change flow.

### Step 4: Route select/search tools through `DBSchema`
- `DBObjectsSelectForm`
  - Add optional `SchemaCache` input.
  - Use cache for schema/object lists and paste lookup.
- `BatchColumnDesc`
  - Add optional `SchemaCache` input.
  - Use cache for schemas and column search where available.
- `TableBuilderForm.SelectObjects`
  - Pass `_dbSchema` into `DBObjectsSelectForm`.
- `TableBuilderForm.BatchToolStripButton_Click`
  - Pass `_dbSchema` into `BatchColumnDesc`.

### Step 5: Fill metadata gaps in `DBSchema`
- Add structures/APIs for metadata currently fetched directly:
  - Descriptions (object, column, parameter)
  - Parameters
  - Object info (table/view/proc/function/trigger/synonym summaries)
  - Indexes/constraints/PK/identity
  - Referenced/referencing/dependencies
  - Definitions
- Add targeted refresh APIs (`RefreshObjectAsync`, `RefreshDescriptionsAsync`, etc.) as needed.

### Step 6: Provider integration updates
If not currently available in `SchemaProvider.Core` and needed for efficient preload, add provider APIs and adaptors for missing metadata categories or bulk retrieval patterns.

### Step 7: Migrate remaining metadata callers
- Move `DBObject`, `ColumnDefView`, `DocumentBuilder`, `JsonBuilder`, and `ObjectDescription` to consume `DBSchema` metadata where practical.
- Keep data-query execution paths (`ExecuteSQL`, `GetDataTableAsync`) outside schema cache scope.

### Step 8: Validation
- Build and verify no regressions.
- Connection switch/cancellation checks.
- Schema/object list parity checks vs previous behavior.
- Basic performance sanity checks for first load and refresh.

## Definition of Done
- `TableBuilderForm` maintains a connection-scoped `DBSchema` snapshot.
- Object/schema browsing and connected helper dialogs can use the same in-memory metadata.
- Missing schema metadata required by current features is represented in `DBSchema` and reachable through provider integration.
- Build is green.
