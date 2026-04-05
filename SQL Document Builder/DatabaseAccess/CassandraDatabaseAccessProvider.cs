namespace SQL_Document_Builder.DatabaseAccess
{
    internal sealed class CassandraDatabaseAccessProvider : UnsupportedDatabaseAccessProviderBase
    {
        public CassandraDatabaseAccessProvider() : base("Cassandra")
        {
        }
    }
}
