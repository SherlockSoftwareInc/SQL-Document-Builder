using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The primary keys.
    /// </summary>
    internal class PrimaryKeys
    {
        // data table to hold the primary keys
        private static readonly System.Data.DataTable dtPrimaryKeys = new();

        /// <summary>
        /// Gets the data source.
        /// </summary>
        public DataTable DataSource { get => dtPrimaryKeys; }

        /// <summary>
        /// Are specified column the primary key.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>A bool.</returns>
        public bool IsPrimaryKey(string schemaName, string tableName, string columnName)
        {
            if (dtPrimaryKeys == null || dtPrimaryKeys?.Rows.Count == 0)
            {
                return false;
            }

            // get the primary keys for the table
            var primaryKeys = from pk in dtPrimaryKeys.AsEnumerable()
                              where pk.Field<string>("SchemaName") == schemaName
                              && pk.Field<string>("TableName") == tableName
                              && pk.Field<string>("ColumnName") == columnName
                              select pk;
            // return true if the column is a primary key
            return primaryKeys.Count() > 0;
        }

        /// <summary>
        /// Open the database and gets primary keys.
        /// </summary>
        public void Open(string connectionString)
        {
            using SqlConnection conn = new(connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = "SELECT s.name AS SchemaName,t.name AS TableName,c.name AS ColumnName FROM sys.key_constraints AS kc INNER JOIN sys.index_columns AS ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id INNER JOIN sys.columns AS c ON ic.object_id = c.object_id AND ic.column_id = c.column_id INNER JOIN sys.tables AS t ON kc.parent_object_id = t.object_id INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id WHERE kc.type = 'PK'";
                // open the connection
                conn.Open();
                var dr = cmd.ExecuteReader();
                // load the data table
                dtPrimaryKeys.Load(dr);
                // close the data reader
                dr.Close();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}