using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SQL_Document_Builder.DatabaseAccess;
using SQL_Document_Builder.SchemaMetadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    public class DBData
    {
        /// <summary>
        /// Get databases list of a specified server
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetDatabases(string serverName, CancellationToken cancellationToken)
        {
            return await SQLDatabaseHelper.GetDatabases(serverName, cancellationToken);
        }

        /// <summary>
        /// Gets DataTable by a sql statement
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataTableAsync(DatabaseConnectionItem connection, string sql, CancellationToken cancellationToken)
        {
            DataTable dt;
            try
            {
                var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                dt = await provider.GetDataTableAsync(sql, connection.ConnectionString, cancellationToken) ?? new DataTable();
            }
            catch (System.Exception)
            {
                throw;
            }

            return dt;
        }

        /// <summary>
        /// Get SELECT statement of a database object
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static string GetSelectStatement(DatabaseConnectionItem connection, ObjectName dBTable)
        {
            if (string.Equals(connection.ConnectionType, "SQL Server", StringComparison.OrdinalIgnoreCase))
                return $"SELECT *{Environment.NewLine} FROM {dBTable.FullName}";
            else
                return ODBCDataSource.GetSelectStatement(connection.ConnectionString, dBTable.FullName);
        }

        /// <summary>
        /// Gets database tables or views
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetTablesAsync(DatabaseConnectionItem connection, ObjectName.ObjectTypeEnums objectType)
        {
            if (string.Equals(connection.ConnectionType, "SQL Server", StringComparison.OrdinalIgnoreCase))
            {
                var objects = await SchemaMetadataProviderContext.Current.GetDatabaseObjectsAsync(objectType, connection.ConnectionString);
                return objects.Select(o => o.FullNameNoQuote).ToList();
            }
            else
                return await ODBCDataSource.GetTablesAsync(connection.ConnectionString, objectType);
        }

        /// <summary>
        /// Test the connection string
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool TestConnection(DatabaseConnectionItem connection)
        {
            if (connection.ConnectionString.Length > 0)
            {
                var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                return provider.TestConnectionAsync(connection.ConnectionString).GetAwaiter().GetResult();
            }
            return false;
        }

        public async Task<bool> TestConnectionAsync(DatabaseConnectionItem connection, CancellationToken cancellationToken)
        {
            bool result = false;
            if (connection.ConnectionString.Length > 0)
            {
                var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                result = await provider.TestConnectionAsync(connection.ConnectionString, cancellationToken);
            }
            return result;
        }
    }
}
