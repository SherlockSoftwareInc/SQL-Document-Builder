using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VSDiagnostics;

namespace SQL_Document_Builder;
[CPUUsageDiagnoser]
public class TableOpenFlowBenchmark
{
    private DatabaseConnectionItem _connection = null !;
    [GlobalSetup]
    public void Setup()
    {
        var savedConnections = new DatabaseConnections();
        savedConnections.Load();
        _connection = savedConnections.Connections.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.ConnectionString) && c.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase)) ?? savedConnections.Connections.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.ConnectionString)) ?? throw new InvalidOperationException("No saved database connection with a valid connection string was found.");
    }

    [Benchmark]
    public async Task OpenSchemaWithColumnsAndDescriptionsAsync()
    {
        var schema = new DBSchema();
        var opened = await schema.OpenAsync(_connection, loadColumns: true);
        if (!opened)
        {
            throw new InvalidOperationException("Schema open failed during benchmark.");
        }
    }
}