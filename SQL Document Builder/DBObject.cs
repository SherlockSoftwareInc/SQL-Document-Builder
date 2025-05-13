using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The DB object.
    /// </summary>
    internal class DBObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DBObject"/> class.
        /// </summary>
        public DBObject()
        {
            ObjectName = new ObjectName();
        }

        /// <summary>
        /// Gets or sets columns
        /// </summary>
        public List<DBColumn> Columns { get; set; } = [];

        /// <summary>
        /// Gets or set description for the object
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets object name
        /// </summary>
        public ObjectName ObjectName { get; set; }

        /// <summary>
        /// Gets or sets name of the object
        /// </summary>
        public string TableName { get => ObjectName.Name; set => ObjectName.Name = value; }

        /// <summary>
        /// Gets or sets schema of the object
        /// </summary>
        public string TableSchema { get => ObjectName.Schema; set => ObjectName.Schema = value; }

        //public string TableCatalog { get => ObjectName.Catealog; set => ObjectName.Catealog = value; }

        /// <summary>
        /// Gets or set object type
        /// </summary>
        public ObjectTypeEnums TableType { get => ObjectName.ObjectType; set => ObjectName.ObjectType = value; }

        /// <summary>
        /// Gets or sets database connection string
        /// </summary>
        private string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Retrieves identity column details for the current table.
        /// </summary>
        /// <returns>A dictionary where the key is the column name and the value is a tuple containing seed and increment values.</returns>
        public Dictionary<string, (int SeedValue, int IncrementValue)> GetIdentityColumns()
        {
            var identityColumns = new Dictionary<string, (int SeedValue, int IncrementValue)>();

            string identityQuery = $@"
SELECT
    ic.name AS identity_column_name,
    ic.seed_value,
    ic.increment_value
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns AS ic ON t.object_id = ic.object_id
WHERE t.name = '{TableName}'
AND s.name = '{TableSchema}';";

            using (var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString))
            {
                connection.Open();
                using var command = new SqlCommand(identityQuery, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string columnName = reader.GetString(0);
                    int seedValue = reader.GetInt32(1);
                    int incrementValue = reader.GetInt32(2);
                    identityColumns[columnName] = (seedValue, incrementValue);
                }
            }

            return identityColumns;
        }

        /// <summary>
        /// Gets the list of views where the specified table is used.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="tableName">The name of the table to check.</param>
        /// <param name="schemaName">The schema name of the table.</param>
        /// <returns>A list of tuples containing the view name and schema name.</returns>
        public static List<(string ViewName, string SchemaName)> GetViewsUsingTable(string? schemaName, string? tableName)
        {
            var views = new List<(string ViewName, string SchemaName)>();
            if (string.IsNullOrEmpty(tableName))
            {
                return views;
            }

            if (string.IsNullOrEmpty(schemaName))
                schemaName = "dbo";

            string query = @"
SELECT
    v.name AS ViewName,
    s.name AS SchemaName
FROM
    sys.views v
    INNER JOIN sys.sql_expression_dependencies d ON v.object_id = d.referencing_id
    INNER JOIN sys.objects o ON d.referenced_id = o.object_id
    INNER JOIN sys.schemas s ON v.schema_id = s.schema_id
WHERE
    o.name = @TableName AND
    s.name = @SchemaName;";

            using (var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString))
            {
                try
                {
                    connection.Open();
                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.Parameters.AddWithValue("@SchemaName", schemaName);

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string viewName = reader.GetString(0);
                        string schema = reader.GetString(1);
                        views.Add((viewName, schema));
                    }
                }
                catch (Exception ex)
                {
                    Common.MsgBox($"Error retrieving views: {ex.Message}", MessageBoxIcon.Error);
                }
            }

            return views;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A DBColumn? .</returns>
        public DBColumn? GetColumn(string columnName)
        {
            foreach (var col in Columns)
            {
                if (col.ColumnName == columnName)
                {
                    return col;
                }
            }
            return null;
        }

        /// <summary>
        /// Generates the SQL script to recreate all non-primary key, non-unique constraint indexes for the specified table.
        /// </summary>
        /// <param name="fullTableName">The full table name in the format [Schema].[TableName].</param>
        /// <returns>A string containing the SQL script to recreate the indexes.</returns>
        public string GetCreateIndexesScript()
        {
            if(ObjectName.IsEmpty())
                return string.Empty;

            StringBuilder indexScript = new();

            string query = $@"
SELECT
    'CREATE ' +
    CASE WHEN i.is_unique = 1 THEN 'UNIQUE ' ELSE '' END +
    i.type_desc COLLATE DATABASE_DEFAULT + ' INDEX ' +
    QUOTENAME(i.name) + ' ON ' +
    '{ObjectName.FullName}' +
    ' (' +
    (SELECT STRING_AGG(QUOTENAME(c.name) + CASE WHEN ic.is_descending_key = 1 THEN ' DESC' ELSE ' ASC' END, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal)
     FROM sys.index_columns ic
     INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
     WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 0) + ')' +
    ISNULL(' INCLUDE (' +
    (SELECT STRING_AGG(QUOTENAME(c2.name), ', ') WITHIN GROUP (ORDER BY ic2.index_column_id)
     FROM sys.index_columns ic2
     INNER JOIN sys.columns c2 ON ic2.object_id = c2.object_id AND ic2.column_id = c2.column_id
     WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id AND ic2.is_included_column = 1) + ')', '') +
    ISNULL(' WHERE ' + i.filter_definition, '') +
    ';'
FROM
    sys.indexes i
WHERE
    i.object_id = OBJECT_ID('{ObjectName.FullName}')
    AND i.is_primary_key = 0
    AND i.is_unique_constraint = 0
    AND i.is_hypothetical = 0
    AND i.index_id > 0
ORDER BY
    i.index_id;";

            using (SqlConnection connection = new(ConnectionString))
            {
                SqlCommand command = new(query, connection);
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    indexScript.AppendLine(reader.GetString(0));
                }
            }

            return indexScript.ToString();
        }

        /// <summary>
        /// Opens the data object.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        public bool Open(ObjectName objectName, string connectionString)
        {
            this.ObjectName = objectName;

            var objectType = objectName.ObjectType;
            if (objectType == ObjectTypeEnums.None)
            {
                // determine the object type
                var sql = $"SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{objectName.Schema}' AND TABLE_NAME = '{objectName.Name}'";
                using SqlConnection conn = new(connectionString);
                try
                {
                    using SqlCommand cmd = new(sql, conn);
                    conn.Open();
                    string? objType = (string)cmd.ExecuteScalar();
                    if (objType != null)
                    {
                        objectType = objType.ToString().Equals("view", StringComparison.CurrentCultureIgnoreCase) ? ObjectTypeEnums.View : ObjectTypeEnums.Table;
                    }
                }
                catch (SqlException)
                {
                    //return;
                }
                finally
                {
                    conn.Close();
                }
            }
            return Open(objectName.Schema, objectName.Name, objectType, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="objectType">The object type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        public bool Open(string schemaName, string tableName, ObjectTypeEnums objectType, string connectionString)
        {
            bool result = false;

            TableName = tableName ?? string.Empty;
            TableSchema = schemaName ?? string.Empty;
            TableType = objectType;

            ConnectionString = connectionString ?? string.Empty;
            Columns.Clear();

            if (ConnectionString.Length > 0 && TableName.Length > 0)
            {
                using SqlConnection conn = new(ConnectionString);
                try
                {
                    using SqlCommand cmd = new()
                    {
                        CommandText = "SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,IS_NULLABLE,COLUMN_DEFAULT FROM information_schema.columns WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION",
                        Connection = conn,
                        CommandType = System.Data.CommandType.Text
                    };
                    cmd.Parameters.Add(new SqlParameter("@Schema", schemaName));
                    cmd.Parameters.Add(new SqlParameter("@TableName", tableName));

                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Columns.Add(new DBColumn(dr));
                    }
                    dr.Close();

                    if (objectType == ObjectTypeEnums.Table)
                    {
                        GetPrimaryKeys(TableSchema, TableName);

                        GetIndexes(TableSchema, TableName);
                    }

                    result = true;
                }
                catch (SqlException)
                {
                    //return;
                }
                finally
                {
                    conn.Close();
                }
                Description = GetTableDesc();
                GetColumnDesc();
            }

            return result;
        }

        /// <summary>
        /// Updates the column description.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="newDescription">The new description.</param>
        public void UpdateColumnDescription(string columnName, string newDescription, bool isView)
        {
            if (ConnectionString.Length == 0 || newDescription.Length == 0) return;

            DBColumn? column = GetColumn(columnName);

            if (column != null)
            {
                column.Description = newDescription;
                string newDesc = newDescription.Replace("'", "''");
                string tableType = isView ? "VIEW" : "TABLE";

                try
                {
                    using SqlConnection connection = new(ConnectionString);
                    connection.Open();

                    string query = $@"
IF EXISTS (
    SELECT 1
    FROM sys.extended_properties AS ep
    JOIN sys.objects AS o ON ep.major_id = o.object_id
    WHERE o.name = '{ObjectName.FullName}'
        AND ep.name = 'MS_Description'
        AND ep.minor_id = (
            SELECT column_id
            FROM sys.columns
            WHERE object_id = o.object_id
                AND name = '{columnName}'
        )
)
BEGIN
    EXEC sys.sp_updateextendedproperty
        @name = N'MS_Description',
        @value = '{newDesc}',
        @level0type = N'SCHEMA',
        @level0name = '{ObjectName.Schema}',
        @level1type = N'{tableType}',
        @level1name = '{ObjectName.Name}',
        @level2type = N'COLUMN',
        @level2name = '{columnName}';
END
ELSE
BEGIN
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = '{newDesc}',
        @level0type = N'SCHEMA',
        @level0name = '{ObjectName.Schema}',
        @level1type = N'{tableType}',
        @level1name = '{ObjectName.Name}',
        @level2type = N'COLUMN',
        @level2name = '{columnName}';
END";

                    using SqlCommand command = new(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Updates the table desc.
        /// </summary>
        /// <param name="newDescription">The description.</param>
        public async void UpdateTableDesc(string newDescription)
        {
            Description = newDescription;

            if (!ObjectName.IsEmpty() && newDescription.Length > 0)
            {
                string newDesc = newDescription.Replace("'", "''");
                string sql = $@"
DECLARE @TableName varchar(100) = '{ObjectName.FullName}';
DECLARE @Schema varchar(100) = OBJECT_SCHEMA_NAME(OBJECT_ID(@TableName));
DECLARE @ObjectName varchar(100) = OBJECT_NAME(OBJECT_ID(@TableName));
DECLARE @ObjectType varchar(100);
SELECT @ObjectType = CASE type_desc WHEN 'USER_TABLE' THEN 'TABLE' ELSE 'VIEW' END
FROM sys.objects
WHERE object_id = OBJECT_ID(@TableName);

IF EXISTS (SELECT value
	    FROM sys.extended_properties
	    WHERE class = 1 AND major_id = OBJECT_ID(@TableName)
	    AND minor_id = 0
	    AND name = 'MS_Description')
	EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'{newDesc}',
		@level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
		@level1type = @ObjectType, @level1name = '{ObjectName.Name}';
ELSE
	EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'{newDesc}',
		@level0type = N'SCHEMA', @level0name = '{ObjectName.Schema}',
		@level1type = @ObjectType, @level1name = '{ObjectName.Name}';";

                await DatabaseHelper.ExecuteSQLAsync(sql);
            }
        }

        /// <summary>
        /// Gets the column desc.
        /// </summary>
        private void GetColumnDesc()
        {
            var conn = new SqlConnection(ConnectionString);
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = string.Format("SELECT C.Name, E.value Description FROM sys.schemas S INNER JOIN sys.{0} T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id WHERE E.name = N'MS_Description' AND S.name = @Schema AND T.name = @TableName", TableType == ObjectTypeEnums.Table ? "tables" : "views"),
                };
                cmd.Parameters.Add(new SqlParameter("@Schema", TableSchema));
                cmd.Parameters.Add(new SqlParameter("@TableName", TableName));
                conn.Open();

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1] != DBNull.Value)
                    {
                        var columnName = dr.GetString(0);
                        var column = GetColumn(columnName);
                        if (column != null)
                        {
                            column.Description = dr.GetString(1);
                        }
                    }
                }
                dr.Close();
            }
            catch (Exception)
            {
                // ignore the error
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableName">The table name.</param>
        private void GetIndexes(string tableSchema, string tableName)
        {
            var sql = $@"SELECT distinct col.name AS ColumnName
FROM sys.indexes ind
INNER JOIN sys.index_columns ic ON ind.object_id = ic.object_id AND ind.index_id = ic.index_id
INNER JOIN sys.columns col ON ic.object_id = col.object_id AND ic.column_id = col.column_id
INNER JOIN sys.tables t ON ind.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = '{tableSchema}'
AND t.name = '{tableName}'";

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = sql,
                    CommandType = CommandType.Text
                };
                conn.Open();
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string? columnName = dr.GetString(0);
                    // find the column in the Columns
                    foreach (var column in Columns)
                    {
                        if (column.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (column.ColID.EndsWith("🗝") == false)
                                column.ColID += "🔢";
                        }
                    }
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Gets or sets the primary key columns.
        /// </summary>
        public string PrimaryKeyColumns { get; set; } = string.Empty;


        /// <summary>
        /// Primaries the keys.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <returns>A list of string.</returns>
        private void GetPrimaryKeys(string schemaName, string tableName)
        {
            PrimaryKeyColumns = string.Empty;

            var sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{schemaName}'";
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = sql,
                    CommandType = CommandType.Text
                };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var columnName = dr.GetString(0);
                    // find the column in the Columns
                    foreach (var column in Columns)
                    {
                        if (column.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            column.ColID += "🗝";

                            if (PrimaryKeyColumns.Length > 0)
                                PrimaryKeyColumns += ", ";
                            PrimaryKeyColumns += $"[{column.ColumnName}]";

                            break;
                        }
                    }
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Gets the table desc.
        /// </summary>
        /// <returns>A string.</returns>
        private string GetTableDesc()
        {
            string result = string.Empty;
            if (ObjectName != null)
            {
                string sql = string.Format(String.Format("SELECT value FROM fn_listextendedproperty (NULL, 'schema', '{0}', '{2}', '{1}', default, default) WHERE name = N'MS_Description'", ObjectName.Schema, ObjectName.Name, (ObjectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")));

                var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                try
                {
                    var cmd = new SqlCommand()
                    {
                        Connection = conn,
                        CommandText = sql,
                        CommandType = CommandType.Text
                    };
                    conn.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr[0] != DBNull.Value)
                            result = dr.GetString(0);   // dr[0].ToString();
                    }

                    dr.Close();
                }
                catch (Exception ex)
                {
                    Common.MsgBox(ex.Message, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }

            return result;
        }

     }
}