using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SQL_Document_Builder.DatabaseAccess
{
    internal abstract class UnsupportedDatabaseAccessProviderBase(string providerName) : IDatabaseAccessProvider
    {
        public virtual string ApplyRowLimit(string sql, int rowLimit) => sql;

        private NotSupportedException CreateException() =>
            new($"Database access provider '{providerName}' is not implemented.");

        public Task<object?> ExecuteScalarAsync(string sql, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<object?>(CreateException());

        public Task<string> ExecuteSqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<string>(CreateException());

        public Task<DataTable?> GetDataTableAsync(string sql, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<DataTable?>(CreateException());

        public Task<bool> IsValidSelectStatementAsync(string sql, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<bool>(CreateException());

        public Task<string> SyntaxCheckAsync(string sql, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<string>(CreateException());

        public Task<string> VerifySqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<string>(CreateException());

        public Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<bool>(CreateException());

        public Task<int> GetRowCountAsync(string fullName, string connectionString, CancellationToken cancellationToken = default) =>
            Task.FromException<int>(CreateException());
    }
}
