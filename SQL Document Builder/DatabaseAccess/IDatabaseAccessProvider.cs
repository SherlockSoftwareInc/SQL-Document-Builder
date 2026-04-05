using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SQL_Document_Builder.DatabaseAccess
{
    internal interface IDatabaseAccessProvider
    {
        string ApplyRowLimit(string sql, int rowLimit);

        Task<object?> ExecuteScalarAsync(string sql, string connectionString, CancellationToken cancellationToken = default);

        Task<string> ExecuteSqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default);

        Task<DataTable?> GetDataTableAsync(string sql, string connectionString, CancellationToken cancellationToken = default);

        Task<bool> IsValidSelectStatementAsync(string sql, string connectionString, CancellationToken cancellationToken = default);

        Task<string> SyntaxCheckAsync(string sql, string connectionString, CancellationToken cancellationToken = default);

        Task<string> VerifySqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default);

        Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default);

        Task<int> GetRowCountAsync(string fullName, string connectionString, CancellationToken cancellationToken = default);
    }
}
