using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The foreign keys.
    /// </summary>
    internal class ForeignKeys
    {
        private readonly DataTable dtForeignKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeys"/> class.
        /// </summary>
        public ForeignKeys()
        {
            dtForeignKeys = new();
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        public DataTable DataSource { get => dtForeignKeys; }

        /// <summary>
        /// Gets the foreign key.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>A string.</returns>
        public string GetForeignKey(string schemaName, string tableName, string columnName)
        {
            if (dtForeignKeys == null || dtForeignKeys?.Rows.Count == 0)
            {
                return string.Empty;
            }

            // get the foreign keys for the table
            var foreignKeys = from fk in dtForeignKeys?.AsEnumerable()
                              where fk.Field<string>("SchemaName") == schemaName
                              && fk.Field<string>("TableName") == tableName
                              && fk.Field<string>("ColumnName") == columnName
                              select fk;
            // return true if the column is a foreign key
            string foreignKey = string.Empty;
            if (foreignKeys?.Count() > 0)
            {
                foreach (var fk in foreignKeys)
                {
                    foreignKey += $"[{fk.Field<string>("ReferencedSchemaName")}].[{fk.Field<string>("ReferencedTableName")}].[{fk.Field<string>("ReferencedColumnName")}], ";
                }
                foreignKey = foreignKey.Substring(0, foreignKey.Length - 2);
            }
            return foreignKey;
        }

        /// <summary>
        /// Opens the database and gets foreign keys.
        /// </summary>
        public void Open(string connectionString)
        {
            using SqlConnection conn = new(connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = "SELECT SCHEMA_NAME(fk.schema_id) AS SchemaName,OBJECT_NAME(fk.parent_object_id) AS TableName,COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,SCHEMA_NAME(referencedObj.schema_id) AS ReferencedSchemaName,OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName FROM sys.foreign_keys AS fk INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id INNER JOIN sys.objects AS referencedObj ON fk.referenced_object_id = referencedObj.object_id\r\n";
                // open the connection
                conn.Open();
                var dr = cmd.ExecuteReader();
                // load the data table
                dtForeignKeys.Load(dr);
                // close the data reader
                dr.Close();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                // close the connection
                conn.Close();
            }
        }
    }
}