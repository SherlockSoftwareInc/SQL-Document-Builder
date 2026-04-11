using System.Data;
using System.Data.Odbc;

if (args.Length == 0)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project Tools/PostgreSqlOdbcTableSelectionTest -- \"<odbc-connection-string>\" [schema] [table]");
    return;
}

var connectionString = args[0];
var schema = args.Length > 1 ? args[1] : "public";
var table = args.Length > 2 ? args[2] : "film_category";

Console.WriteLine($"Testing PostgreSQL ODBC metadata for {schema}.{table}");

using var connection = new OdbcConnection(connectionString);
await connection.OpenAsync();

await RunStep("TableInfo", async () =>
{
    const string sql = """
        SELECT
            CAST(NULL AS timestamp) AS CreateDate,
            CAST(NULL AS timestamp) AS ModifyDate,
            COALESCE(st.n_live_tup, 0) AS RowCount,
            pg_total_relation_size(quote_ident(n.nspname) || '.' || quote_ident(c.relname)) / 1024.0 / 1024.0 AS DataSizeMB
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        LEFT JOIN pg_stat_all_tables st ON st.relid = c.oid
        WHERE n.nspname = ?
          AND c.relname = ?
          AND c.relkind = 'r'
        """;

    await LoadAsync(connection, sql, schema, table);
});

await RunStep("Columns", async () =>
{
    const string sql = """
        SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ?
        ORDER BY ORDINAL_POSITION;
        """;

    await LoadAsync(connection, sql, schema, table);
});

await RunStep("PrimaryKeys", async () =>
{
    const string sql = """
        SELECT kcu.column_name
        FROM information_schema.table_constraints tc
        JOIN information_schema.key_column_usage kcu
          ON tc.constraint_schema = kcu.constraint_schema
         AND tc.table_name = kcu.table_name
         AND tc.constraint_name = kcu.constraint_name
        WHERE tc.table_schema = ?
          AND tc.table_name = ?
          AND tc.constraint_type = 'PRIMARY KEY'
        ORDER BY kcu.ordinal_position
        """;

    await LoadAsync(connection, sql, schema, table);
});

await RunStep("Constraints", async () =>
{
    const string sql = """
        SELECT
            tc.constraint_name AS ConstraintName,
            tc.constraint_type AS ConstraintType,
            kcu.column_name AS ColumnName
        FROM information_schema.table_constraints tc
        LEFT JOIN information_schema.key_column_usage kcu
          ON tc.constraint_schema = kcu.constraint_schema
         AND tc.table_name = kcu.table_name
         AND tc.constraint_name = kcu.constraint_name
        WHERE tc.table_schema = ?
          AND tc.table_name = ?
        ORDER BY tc.constraint_name, kcu.ordinal_position
        """;

    await LoadAsync(connection, sql, schema, table);
});

await RunStep("Indexes", async () =>
{
    const string sql = """
        SELECT
            i.relname AS IndexName,
            CASE WHEN ix.indisunique THEN 'UNIQUE INDEX' ELSE 'INDEX' END AS Type,
            a.attname AS ColumnName,
            CASE WHEN ix.indisunique THEN 1 ELSE 0 END AS IsUnique
        FROM pg_class t
        JOIN pg_namespace n ON n.oid = t.relnamespace
        JOIN pg_index ix ON ix.indrelid = t.oid
        JOIN pg_class i ON i.oid = ix.indexrelid
        JOIN unnest(ix.indkey) WITH ORDINALITY AS cols(attnum, ord) ON TRUE
        JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = cols.attnum
        WHERE n.nspname = ?
          AND t.relname = ?
          AND t.relkind IN ('r','m')
          AND ix.indisprimary = false
        ORDER BY i.relname, cols.ord
        """;

    await LoadAsync(connection, sql, schema, table);
});

await RunStep("ObjectDescription", async () =>
{
    const string sql = """
        SELECT COALESCE(obj_description(c.oid), '')
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname = ?
          AND c.relname = ?
        LIMIT 1
        """;

    await LoadAsync(connection, sql, schema, table);
});

await RunStep("ColumnDescriptions", async () =>
{
    const string sql = """
        SELECT a.attname AS column_name,
               COALESCE(col_description(c.oid, a.attnum), '') AS column_comment
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        JOIN pg_attribute a ON a.attrelid = c.oid
        WHERE n.nspname = ?
          AND c.relname = ?
          AND a.attnum > 0
          AND NOT a.attisdropped
        ORDER BY a.attnum
        """;

    await LoadAsync(connection, sql, schema, table);
});

Console.WriteLine("Done.");

static async Task RunStep(string step, Func<Task> action)
{
    try
    {
        await action();
        Console.WriteLine($"[OK] {step}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FAIL] {step}: {ex.GetType().Name} - {ex.Message}");
    }
}

static async Task<DataTable> LoadAsync(OdbcConnection connection, string sql, params object[] parameters)
{
    using var command = new OdbcCommand(sql, connection);
    for (var i = 0; i < parameters.Length; i++)
    {
        command.Parameters.AddWithValue($"@p{i + 1}", parameters[i]);
    }

    using var reader = await command.ExecuteReaderAsync();
    var table = new DataTable();
    table.Load(reader);
    return table;
}
