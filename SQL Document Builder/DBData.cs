using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                if (string.Equals(connection.ConnectionType, "ODBC", StringComparison.OrdinalIgnoreCase))
                {
                    dt = await ODBCDataSource.GetDataTableAsync(connection.ConnectionString, sql, cancellationToken);
                }
                else
                {
                    dt = await SQLDatabaseHelper.GetDataTableAsync(sql, connection.ConnectionString) ?? new DataTable();
                }
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
                var objects = await SQLDatabaseHelper.GetDatabaseObjectsAsync(objectType, connection.ConnectionString);
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
                if (string.Equals(connection.ConnectionType, "SQL Server", StringComparison.OrdinalIgnoreCase))
                    return SQLDatabaseHelper.TestConnectionAsync(connection.ConnectionString).GetAwaiter().GetResult();
                return ODBCDataSource.TestConnection(connection.ConnectionString);
            }
            return false;
        }

        public async Task<bool> TestConnectionAsync(DatabaseConnectionItem connection, CancellationToken cancellationToken)
        {
            bool result = false;
            if (connection.ConnectionString.Length > 0)
            {
                if (string.Equals(connection.ConnectionType, "SQL Server", StringComparison.OrdinalIgnoreCase))
                {
                    result = await SQLDatabaseHelper.TestConnectionAsync(connection.ConnectionString);
                }
                else
                {
                    var odbc = new ODBCDataSource();
                    result = await odbc.TestConnectionAsync(connection.ConnectionString, cancellationToken);
                }
            }
            return result;
        }
    }
}
