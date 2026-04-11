using OctofyPro.SchemaProvider.Core.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    public class ODBCDataSource
    {
        /// <summary>
        /// Gets data in a data table by a SELECT statement
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataTableAsync(string connectionString, string sql, CancellationToken cancellationToken)
        {
            DataTable dt = new DataTable();
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                try
                {
                    using (OdbcCommand cmd = new OdbcCommand()
                    {
                        Connection = conn,
                        CommandText = sql,
                        CommandType = CommandType.Text,
                        CommandTimeout = 50000
                    })
                    {
                        await conn.OpenAsync(cancellationToken);
                        using CancellationTokenRegistration crt = cancellationToken.Register(() => cmd.Cancel());
                        using var dr = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        dt = CreateDataTableWithoutConstraints(dr);
                        dr.Close();
                    }
                }
                catch (OdbcException ex)
                {
                    // https://knowledgebase.progress.com/articles/Article/3509
                    if (ex.ErrorCode != 5701 && ex.ErrorCode != 5701)
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
            return dt;
        }

        /// <summary>
        /// Generate the SELECT for a database object
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetSelectStatement(string connectionString, string tableName)
        {
            var sb = new System.Text.StringBuilder();

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                try
                {
                    using OdbcCommand cmd = new OdbcCommand() { Connection = conn, CommandType = System.Data.CommandType.Text };
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE 0=1", tableName);
                    conn.Open();

                    using var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                    var dt = CreateDataTableWithoutConstraints(reader);

                    if (dt.Columns.Count > 0)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append("SELECT ").AppendLine(dt.Columns[i].ColumnName.QuotedName());
                            }
                            else
                            {
                                sb.Append("      ,").AppendLine(dt.Columns[i].ColumnName.QuotedName());
                            }
                        }
                    }
                    sb.AppendFormat(" FROM {0}", tableName).AppendLine();
                }
                catch (Exception)
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
            return sb.ToString();
        }

        internal static DataTable CreateDataTableWithoutConstraints(IDataReader reader)
        {
            var table = new DataTable();
            var columnNameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var baseName = reader.GetName(i);
                if (string.IsNullOrWhiteSpace(baseName))
                {
                    baseName = $"Column{i + 1}";
                }

                if (!columnNameCounts.TryGetValue(baseName, out var seen))
                {
                    seen = 0;
                }

                var finalName = seen == 0 ? baseName : $"{baseName}_{seen + 1}";
                columnNameCounts[baseName] = seen + 1;

                var dataType = reader.GetFieldType(i) ?? typeof(object);
                table.Columns.Add(new DataColumn(finalName, dataType)
                {
                    AllowDBNull = true
                });
            }

            var values = new object[reader.FieldCount];
            while (reader.Read())
            {
                reader.GetValues(values);
                table.Rows.Add((object[])values.Clone());
            }

            return table;
        }

        /// <summary>
        /// Get the unique table/view list or column list from the database
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetTablesAsync(string connectionString, ObjectName.ObjectTypeEnums objectType)
        {
            var result = new List<string>();

            await using var provider = DatabaseSchemaProviderFactory.Create(DatabaseProviderKind.Odbc);
            await provider.ConnectAsync(connectionString, CancellationToken.None).ConfigureAwait(false);

            var schemas = await provider.GetSchemasAsync(CancellationToken.None).ConfigureAwait(false);
            for (int i = 0; i < schemas.Count; i++)
            {
                var schema = schemas[i];
                if (objectType == ObjectName.ObjectTypeEnums.Table || objectType == ObjectName.ObjectTypeEnums.None)
                {
                    var tables = await provider.GetAllTablesAsync(schema, CancellationToken.None).ConfigureAwait(false);
                    for (int j = 0; j < tables.Count; j++)
                    {
                        result.Add(tables[j].ObjectName);
                    }
                }

                if (objectType == ObjectName.ObjectTypeEnums.View || objectType == ObjectName.ObjectTypeEnums.None)
                {
                    var views = await provider.GetAllViewsAsync(schema, CancellationToken.None).ConfigureAwait(false);
                    for (int j = 0; j < views.Count; j++)
                    {
                        result.Add(views[j].ObjectName);
                    }
                }

                if (objectType == ObjectName.ObjectTypeEnums.All)
                {
                    var tables = await provider.GetAllTablesAsync(schema, CancellationToken.None).ConfigureAwait(false);
                    var views = await provider.GetAllViewsAsync(schema, CancellationToken.None).ConfigureAwait(false);

                    for (int j = 0; j < tables.Count; j++)
                    {
                        var columns = await provider.GetColumnsAsync(schema, tables[j].ObjectName, CancellationToken.None).ConfigureAwait(false);
                        for (int k = 0; k < columns.Count; k++)
                        {
                            result.Add(columns[k].ColumnName);
                        }
                    }

                    for (int j = 0; j < views.Count; j++)
                    {
                        var columns = await provider.GetColumnsAsync(schema, views[j].ObjectName, CancellationToken.None).ConfigureAwait(false);
                        for (int k = 0; k < columns.Count; k++)
                        {
                            result.Add(columns[k].ColumnName);
                        }
                    }
                }
            }

            result = result
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return result;
        }

        /// <summary>
        /// Test the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool TestConnection(string connectionString)
        {
            bool result = false;

            try
            {
                OdbcConnection sqlConn = new OdbcConnection(connectionString);
                sqlConn.Open();

                //get databases
                //DataTable tblDatabases = sqlConn.GetSchema("Tables");

                sqlConn.Close();

                result = true;
            }
            catch (OdbcException)
            {
                //Ignore the error
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Test the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                OdbcConnection sqlConn = new OdbcConnection(connectionString);
                await sqlConn.OpenAsync(cancellationToken);

                sqlConn.Close();

                result = true;
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}