using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The schema content builder for MS SQL server.
    /// </summary>
    internal class MSSchemaContentBuilder
    {
        // the class to hold the foreign keys
        private readonly ForeignKeys foreignKeys = new();

        // the class to hold the primary keys
        private readonly PrimaryKeys primaryKeys = new();

        // the string builder to hold the schema content
        private readonly StringBuilder sbSchemaContent = new();

        // local variable to hold the connection string
        private string _connectionString = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether include table description.
        /// </summary>
        public bool IncludeTableDescription { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether include column description.
        /// </summary>
        public bool IncludeColumnDescription { get; set; } = false;

        /// <summary>
        /// Build and returns schemata contents.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A string.</returns>
        public string SchemaContent(string connectionString, IProgress<int> progress)
        {
            _connectionString = connectionString;

            sbSchemaContent.Clear();
            sbSchemaContent.AppendLine("Here is the database schema:");

            // open the primary keys
            primaryKeys.Open(connectionString);

            // open the foreign keys
            foreignKeys.Open(connectionString);

            // open the database and get all tables and views
            using SqlConnection conn = new(connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = "SELECT TABLE_SCHEMA,TABLE_NAME,TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_TYPE,TABLE_SCHEMA,TABLE_NAME";
                // open the connection
                conn.Open();

                using DataSet ds = new();
                using SqlDataAdapter da = new(cmd);
                da.Fill(ds);

                if (ds?.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        int percentComplete = (i * 100) / ds.Tables[0].Rows.Count;
                        if (percentComplete > 0 && percentComplete % 2 == 0)
                            progress.Report(percentComplete + 1);

                        var row = ds.Tables[0].Rows[i];

                        // get the schema name
                        string schemaName = row["TABLE_SCHEMA"].ToString();
                        // get the table name
                        string tableName = row["TABLE_NAME"].ToString();
                        // get the table type
                        string tableType = row["TABLE_TYPE"].ToString();
                        // build the content for the table or view
                        BuildTableContent(schemaName, tableName, tableType, i + 1);
                    }
                }
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

            return sbSchemaContent.ToString();
        }

        /// <summary>
        /// Builds the table column content.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        private void BuildTableColumnContent(string schemaName, string tableName)
        {
            // open the database and get all tables and views
            using SqlConnection conn = new(_connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = $"SELECT COLUMN_NAME,IS_NULLABLE,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{schemaName}' AND TABLE_NAME = '{tableName}'";
                // open the connection
                conn.Open();
                var dr = cmd.ExecuteReader();
                // go through each column
                while (dr.Read())
                {
                    // get the column name
                    string columnName = dr.GetString(0);
                    // get the column type
                    string columnType = dr.GetString(2);
                    // get the column description
                    string columnDescription = string.Empty;
                    if (IncludeColumnDescription)
                    {
                        columnDescription = GetColumnDescription(schemaName, tableName, columnName);
                    }
                    // get the column foreign key
                    string columnForeignKey = foreignKeys.GetForeignKey(schemaName, tableName, columnName);
                    // get the column primary key
                    bool isPrimaryKey = primaryKeys.IsPrimaryKey(schemaName, tableName, columnName);
                    // build the content for the column
                    string content = $"- '{columnName}' ({columnType}";
                    if (isPrimaryKey)
                    {
                        content += $", primary Key)";
                    }
                    else
                    {
                        // add the foreign key if any
                        if (!string.IsNullOrEmpty(columnForeignKey))
                        {
                            //content += $", foreign Key references '{columnForeignKey}')";
                            content += $", references to '{columnForeignKey}')";
                        }
                        else
                        {
                            content += ")";
                        }
                    }

                    if (!string.IsNullOrEmpty(columnDescription))
                    {
                        content += $" - {columnDescription}";
                    }
                    // add to the schema content
                    sbSchemaContent.AppendLine(content);
                }

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

        /// <summary>
        /// Builds the table content.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="tableType">The table type.</param>
        private void BuildTableContent(string schemaName, string tableName, string tableType, int index)
        {
            var objectType = tableType == "BASE TABLE" ? "Table" : "View";
            string tableDesc = $"{index}.{objectType} [{schemaName}].[{tableName}]";
            string tableDescription = GetTableDescription(schemaName, tableName);
            if (IncludeTableDescription)
            {
                if (!string.IsNullOrEmpty(tableDescription))
                {
                    tableDesc += $": {tableDescription}";
                }
                else
                {
                    tableDesc += ":";
                }
            }
            else
            {
                tableDesc += ":";
            }

            sbSchemaContent.AppendLine(tableDesc);

            BuildTableColumnContent(schemaName, tableName);
        }

        /// <summary>
        /// Gets the column description.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>A string.</returns>
        private string GetColumnDescription(string schemaName, string tableName, string columnName)
        {
            string desc = string.Empty;
            using SqlConnection conn = new(_connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = $"SELECT ep.value AS ColumnDescription FROM sys.columns AS c LEFT JOIN sys.extended_properties AS ep ON ep.major_id = c.object_id AND ep.minor_id = c.column_id AND ep.class = 1 AND ep.name = 'MS_Description' WHERE SCHEMA_NAME(OBJECTPROPERTY(c.object_id, 'SchemaId')) = '{schemaName}' AND OBJECT_NAME(c.object_id) = '{tableName}' AND c.name = N'{columnName}'";
                // open the connection
                conn.Open();
                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        desc = dr.GetString(0);
                }

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

            return desc.Replace("\r\n", " ");
        }

        /// <summary>
        /// Gets the description of a table or view.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <returns>A string.</returns>
        private string GetTableDescription(string schemaName, string tableName)
        {
            string tableDesc = string.Empty;
            using SqlConnection conn = new(_connectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.CommandText = $"SELECT ep.value AS TableDescription FROM sys.tables AS t LEFT JOIN sys.extended_properties AS ep ON ep.major_id = t.object_id AND ep.minor_id = 0 AND ep.class = 1 AND ep.name = 'MS_Description' WHERE SCHEMA_NAME(t.schema_id) = N'{schemaName}' AND t.name = N'{tableName}'";
                // open the connection
                conn.Open();
                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        tableDesc = dr.GetString(0);
                }

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

            return tableDesc.Replace("\r\n", " ");
        }
    }
}