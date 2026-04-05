using System;

namespace SQL_Document_Builder.DatabaseAccess
{
    internal sealed class OracleDatabaseAccessProvider : UnsupportedDatabaseAccessProviderBase
    {
        public OracleDatabaseAccessProvider() : base("Oracle")
        {
        }

        public override string ApplyRowLimit(string sql, int rowLimit)
        {
            if (string.IsNullOrWhiteSpace(sql) || rowLimit <= 0)
            {
                return sql;
            }

            if (sql.Contains(" FETCH FIRST ", StringComparison.OrdinalIgnoreCase)
                || sql.Contains(" ROWNUM ", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }

            return $"SELECT * FROM ({sql}) WHERE ROWNUM <= {rowLimit}";
        }
    }
}
