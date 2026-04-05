namespace SQL_Document_Builder.DatabaseAccess
{
    internal sealed class RedisDatabaseAccessProvider : UnsupportedDatabaseAccessProviderBase
    {
        public RedisDatabaseAccessProvider() : base("Redis")
        {
        }
    }
}
