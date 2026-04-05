using System;
namespace SQL_Document_Builder.DatabaseAccess
{
    internal static class DatabaseAccessProviderFactory
    {
        private static readonly IDatabaseAccessProvider CassandraProvider = new CassandraDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider MariaDbProvider = new MariaDbDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider MongoDbProvider = new MongoDbDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider MySqlProvider = new MySqlDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider SqlServerProvider = new SqlServerDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider OdbcProvider = new OdbcDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider OracleProvider = new OracleDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider OtherProvider = new OtherDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider PostgreSqlProvider = new PostgreSqlDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider RedisProvider = new RedisDatabaseAccessProvider();
        private static readonly IDatabaseAccessProvider SqliteProvider = new SqliteDatabaseAccessProvider();

        internal static IDatabaseAccessProvider GetProvider(DatabaseConnectionItem? connection)
        {
            if (connection == null)
            {
                return SqlServerProvider;
            }

            return connection.DBMSType switch
            {
                DBMSTypeEnums.SQLServer => connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase)
                    ? OdbcProvider
                    : SqlServerProvider,
                DBMSTypeEnums.MySQL => MySqlProvider,
                DBMSTypeEnums.PostgreSQL => PostgreSqlProvider,
                DBMSTypeEnums.Oracle => OracleProvider,
                DBMSTypeEnums.MongoDB => MongoDbProvider,
                DBMSTypeEnums.SQLite => SqliteProvider,
                DBMSTypeEnums.Redis => RedisProvider,
                DBMSTypeEnums.Cassandra => CassandraProvider,
                DBMSTypeEnums.MariaDB => MariaDbProvider,
                DBMSTypeEnums.Other => connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase)
                    ? OdbcProvider
                    : OtherProvider,
                _ => SqlServerProvider
            };
        }
    }
}
