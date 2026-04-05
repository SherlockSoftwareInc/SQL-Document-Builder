using System.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SQL_Document_Builder.DatabaseAccess
{
    internal sealed class SqlServerDatabaseAccessProvider : IDatabaseAccessProvider
    {
        public string ApplyRowLimit(string sql, int rowLimit)
        {
            if (string.IsNullOrWhiteSpace(sql) || rowLimit <= 0)
            {
                return sql;
            }

            if (sql.Contains(" TOP ", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }

            var selectIndex = sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);
            if (selectIndex < 0)
            {
                return sql;
            }

            return sql.Insert(selectIndex + 6, $" TOP {rowLimit} ");
        }

        public Task<object?> ExecuteScalarAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.ExecuteScalarAsync(sql, connectionString);

        public Task<string> ExecuteSqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.ExecuteSQLAsync(sql, connectionString);

        public Task<DataTable?> GetDataTableAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetDataTableAsync(sql, connectionString);

        public Task<bool> IsValidSelectStatementAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.IsValidSelectStatement(sql, connectionString);

        public Task<string> SyntaxCheckAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.SyntaxCheckAsync(sql, connectionString);

        public Task<string> VerifySqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.VerifySQL(sql, connectionString) ?? Task.FromResult("No database connection specified.");

        public Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.TestConnectionAsync(connectionString);

        public Task<int> GetRowCountAsync(string fullName, string connectionString, CancellationToken cancellationToken = default)
            => SQLDatabaseHelper.GetRowCountAsync(fullName, connectionString);
    }
}
