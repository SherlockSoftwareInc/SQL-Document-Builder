using System;
using System.Data;
using System.Data.Odbc;
using System.Threading;
using System.Threading.Tasks;

namespace SQL_Document_Builder.DatabaseAccess
{
    internal sealed class OdbcDatabaseAccessProvider : IDatabaseAccessProvider
    {
        public string ApplyRowLimit(string sql, int rowLimit) => sql;

        public async Task<object?> ExecuteScalarAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(sql))
            {
                return DBNull.Value;
            }

            await using var connection = new OdbcConnection(connectionString);
            await using var command = new OdbcCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 50000
            };

            await connection.OpenAsync(cancellationToken);
            return await command.ExecuteScalarAsync(cancellationToken);
        }

        public async Task<string> ExecuteSqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return "No database connection specified.";
            }

            try
            {
                await using var connection = new OdbcConnection(connectionString);
                await using var command = new OdbcCommand(sql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<DataTable?> GetDataTableAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(sql))
            {
                return null;
            }

            return await ODBCDataSource.GetDataTableAsync(connectionString, sql, cancellationToken);
        }

        public async Task<bool> IsValidSelectStatementAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(sql))
            {
                return false;
            }

            try
            {
                await using var connection = new OdbcConnection(connectionString);
                await using var command = new OdbcCommand(sql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync(cancellationToken);
                await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly, cancellationToken);
                return reader != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SyntaxCheckAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return "No database connection specified.";
            }

            try
            {
                await using var connection = new OdbcConnection(connectionString);
                await using var command = new OdbcCommand(sql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync(cancellationToken);
                await using var _ = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly, cancellationToken);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Task<string> VerifySqlAsync(string sql, string connectionString, CancellationToken cancellationToken = default)
            => SyntaxCheckAsync(sql, connectionString, cancellationToken);

        public async Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return false;
            }

            try
            {
                await using var connection = new OdbcConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetRowCountAsync(string fullName, string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(fullName))
            {
                return 0;
            }

            try
            {
                var result = await ExecuteScalarAsync($"SELECT COUNT(*) FROM {fullName}", connectionString, cancellationToken);
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }
    }
}
