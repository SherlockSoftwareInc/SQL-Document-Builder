using System;

namespace SQL_Document_Builder.DatabaseAccess
{
    internal sealed class MySqlDatabaseAccessProvider : UnsupportedDatabaseAccessProviderBase
    {
        public MySqlDatabaseAccessProvider() : base("MySQL")
        {
        }

        public override string ApplyRowLimit(string sql, int rowLimit)
        {
            if (string.IsNullOrWhiteSpace(sql) || rowLimit <= 0)
            {
                return sql;
            }

            if (sql.Contains(" LIMIT ", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }

            return $"{sql.TrimEnd(';')} LIMIT {rowLimit}";
        }
    }
}
