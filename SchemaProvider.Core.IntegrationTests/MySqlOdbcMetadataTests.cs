using System.Data.Odbc;
using System.Data.Odbc;
using OctofyPro.SchemaProvider.Core.Models;
using OctofyPro.SchemaProvider.Core.Providers;
using Xunit;
using Xunit.Abstractions;

namespace SchemaProvider.Core.IntegrationTests;

public sealed class MySqlOdbcMetadataTests
{
    private const string ConnectionString = "Dsn=mysql;Driver={MySQL ODBC 8.0 Unicode Driver}";

    private readonly ITestOutputHelper _output;

    public MySqlOdbcMetadataTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task UpdateAndReadBack_SingleObject_Descriptions()
    {
        var schemaName = await ResolveSchemaNameAsync(ConnectionString);
        Assert.False(string.IsNullOrWhiteSpace(schemaName), "Unable to determine MySQL schema name.");

        await using var provider = DatabaseSchemaProviderFactory.Create(DatabaseProviderKind.Odbc);
        await provider.ConnectAsync(ConnectionString, CancellationToken.None);

        // Save originals
        var originalTableDesc = await provider.GetObjectDescriptionAsync(schemaName, "api_keys", CancellationToken.None);
        var originalColDescs = await provider.GetColumnDescriptionsAsync(schemaName, "api_keys", CancellationToken.None);
        originalColDescs.TryGetValue("user_id", out var originalUserIdDesc);
        originalUserIdDesc ??= string.Empty;

        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var newTableDesc = $"api_keys table test {suffix}";
        var newUserIdDesc = $"user_id column test {suffix}";

        try
        {
            // Write
            await provider.UpdateObjectDescriptionAsync(schemaName, "api_keys", DatabaseObjectType.Table, newTableDesc, CancellationToken.None);
            await provider.UpdateColumnDescriptionAsync(schemaName, "api_keys", "user_id", newUserIdDesc, CancellationToken.None);

            // Read back single-object APIs
            var readTableDesc = await provider.GetObjectDescriptionAsync(schemaName, "api_keys", CancellationToken.None);
            _output.WriteLine($"[Single] Table description: '{readTableDesc}'");
            Assert.Contains(newTableDesc, readTableDesc, StringComparison.Ordinal);

            var readColDescs = await provider.GetColumnDescriptionsAsync(schemaName, "api_keys", CancellationToken.None);
            _output.WriteLine($"[Single] Column descriptions count: {readColDescs.Count}");
            foreach (var kvp in readColDescs)
                _output.WriteLine($"  [{kvp.Key}] = '{kvp.Value}'");

            Assert.True(readColDescs.ContainsKey("user_id"), "user_id key missing from single-object column descriptions");
            Assert.Contains(newUserIdDesc, readColDescs["user_id"], StringComparison.Ordinal);
        }
        finally
        {
            await provider.UpdateObjectDescriptionAsync(schemaName, "api_keys", DatabaseObjectType.Table, originalTableDesc, CancellationToken.None);
            await provider.UpdateColumnDescriptionAsync(schemaName, "api_keys", "user_id", originalUserIdDesc, CancellationToken.None);
        }
    }

    [Fact]
    public async Task BulkDescriptionAPIs_ReturnApiKeysDescriptions()
    {
        var schemaName = await ResolveSchemaNameAsync(ConnectionString);
        Assert.False(string.IsNullOrWhiteSpace(schemaName), "Unable to determine MySQL schema name.");

        await using var provider = DatabaseSchemaProviderFactory.Create(DatabaseProviderKind.Odbc);
        await provider.ConnectAsync(ConnectionString, CancellationToken.None);

        // Set known descriptions
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var testTableDesc = $"bulk test table {suffix}";
        var testColDesc = $"bulk test col {suffix}";

        var origTableDesc = await provider.GetObjectDescriptionAsync(schemaName, "api_keys", CancellationToken.None);
        var origColDescs = await provider.GetColumnDescriptionsAsync(schemaName, "api_keys", CancellationToken.None);
        origColDescs.TryGetValue("user_id", out var origUserIdDesc);
        origUserIdDesc ??= string.Empty;

        try
        {
            await provider.UpdateObjectDescriptionAsync(schemaName, "api_keys", DatabaseObjectType.Table, testTableDesc, CancellationToken.None);
            await provider.UpdateColumnDescriptionAsync(schemaName, "api_keys", "user_id", testColDesc, CancellationToken.None);

            // ---- Test GetObjectDescriptionsAsync (bulk) ----
            var allObjectDescs = await provider.GetObjectDescriptionsAsync(CancellationToken.None);
            _output.WriteLine($"[Bulk] GetObjectDescriptionsAsync returned {allObjectDescs.Count} entries");

            var apiKeysObjKey = allObjectDescs.Keys.FirstOrDefault(k =>
                k.ObjectName.Equals("api_keys", StringComparison.OrdinalIgnoreCase)
                && k.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase));

            if (apiKeysObjKey.ObjectName != null)
            {
                _output.WriteLine($"[Bulk] api_keys object description: '{allObjectDescs[apiKeysObjKey]}'");
                Assert.Contains(testTableDesc, allObjectDescs[apiKeysObjKey], StringComparison.Ordinal);
            }
            else
            {
                // Dump what keys we got for debugging
                foreach (var k in allObjectDescs.Keys.Where(k => k.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase)).Take(10))
                    _output.WriteLine($"  key: ({k.SchemaName}, {k.ObjectName}) = '{allObjectDescs[k]}'");
                Assert.Fail($"api_keys not found in bulk object descriptions for schema '{schemaName}'");
            }

            // ---- Test GetColumnDescriptionsAsync (bulk) ----
            var allColDescs = await provider.GetColumnDescriptionsAsync(CancellationToken.None);
            _output.WriteLine($"[Bulk] GetColumnDescriptionsAsync returned {allColDescs.Count} entries");

            var apiKeysColKey = allColDescs.Keys.FirstOrDefault(k =>
                k.ObjectName.Equals("api_keys", StringComparison.OrdinalIgnoreCase)
                && k.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase));

            if (apiKeysColKey.ObjectName != null)
            {
                var colMap = allColDescs[apiKeysColKey];
                _output.WriteLine($"[Bulk] api_keys column descriptions count: {colMap.Count}");
                foreach (var c in colMap)
                    _output.WriteLine($"  [{c.Key}] = '{c.Value}'");

                Assert.True(colMap.ContainsKey("user_id"), "user_id missing from bulk column descriptions");
                Assert.Contains(testColDesc, colMap["user_id"], StringComparison.Ordinal);
            }
            else
            {
                foreach (var k in allColDescs.Keys.Where(k => k.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase)).Take(10))
                    _output.WriteLine($"  key: ({k.SchemaName}, {k.ObjectName})");
                Assert.Fail($"api_keys not found in bulk column descriptions for schema '{schemaName}'");
            }
        }
        finally
        {
            await provider.UpdateObjectDescriptionAsync(schemaName, "api_keys", DatabaseObjectType.Table, origTableDesc, CancellationToken.None);
            await provider.UpdateColumnDescriptionAsync(schemaName, "api_keys", "user_id", origUserIdDesc, CancellationToken.None);
        }
    }

    [Fact]
    public async Task GetAllTables_ReturnsTablesWithNonEmptySchemaName()
    {
        var schemaName = await ResolveSchemaNameAsync(ConnectionString);
        Assert.False(string.IsNullOrWhiteSpace(schemaName), "Unable to determine MySQL schema name.");

        await using var provider = DatabaseSchemaProviderFactory.Create(DatabaseProviderKind.Odbc);
        await provider.ConnectAsync(ConnectionString, CancellationToken.None);

        var tables = await provider.GetAllTablesAsync(string.Empty, CancellationToken.None);
        _output.WriteLine($"Total tables: {tables.Count}");

        var apiKeysTable = tables.FirstOrDefault(t =>
            t.ObjectName.Equals("api_keys", StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(apiKeysTable);
        _output.WriteLine($"api_keys SchemaName: '{apiKeysTable.SchemaName}'");
        Assert.False(string.IsNullOrWhiteSpace(apiKeysTable.SchemaName),
            "api_keys table was returned with empty SchemaName — MySQL ODBC TABLE_SCHEM/TABLE_CAT resolution is broken.");
        Assert.Equal(schemaName, apiKeysTable.SchemaName, StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<string> ResolveSchemaNameAsync(string connectionString)
    {
        var builder = new OdbcConnectionStringBuilder(connectionString);
        if (TryGetValue(builder, "Database", out var database)
            || TryGetValue(builder, "Initial Catalog", out database))
        {
            return database;
        }

        await using var connection = new OdbcConnection(connectionString);
        await connection.OpenAsync(CancellationToken.None);

        await using var command = new OdbcCommand("SELECT DATABASE()", connection);
        var result = await command.ExecuteScalarAsync(CancellationToken.None);
        return result?.ToString() ?? string.Empty;
    }

    private static bool TryGetValue(OdbcConnectionStringBuilder builder, string key, out string value)
    {
        value = string.Empty;
        if (!builder.ContainsKey(key))
        {
            return false;
        }

        value = builder[key]?.ToString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }
}
