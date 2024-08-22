using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        public List<DBColumn> Columns { get; set; } = new List<DBColumn>();

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
        /// Descriptions the script.
        /// </summary>
        /// <returns>A string.</returns>
        public string DescriptionScript()
        {
            var sb = new System.Text.StringBuilder();

            if (!ObjectName.IsEmpty())
            {
                if (Description.Length > 0)
                {
                    sb.AppendLine(string.Format("EXEC ADMIN.usp_AddObjectDescription '{0}', N'{1}'", ObjectName.FullName, Description));
                }
                foreach (var column in Columns)
                {
                    if (column.Description.Length > 0)
                    {
                        sb.AppendLine(string.Format("EXEC ADMIN.usp_AddColumnDescription '{0}', '{1}', N'{2}'", ObjectName.FullName, column.ColumnName, Description));
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A bool.</returns>
        public bool Open(ObjectName objectName, string connectionString)
        {
            var objectType = objectName.ObjectType;
            if(objectType == ObjectTypeEnums.None)
            {
                // determine the object type
                var sql = $"SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{objectName.Schema}' AND TABLE_NAME = '{objectName.Name}'";
                using SqlConnection conn = new(connectionString);
                try
                {
                    using SqlCommand cmd = new(sql, conn);
                    conn.Open();
                    var objType = cmd.ExecuteScalar();
                    if (objType != null)
                    {
                        objectType = objType.ToString().ToLower() == "view" ? ObjectTypeEnums.View : ObjectTypeEnums.Table;
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

                    //foreach (var column in Columns)
                    //{
                    //    if (column.ColumnName != null)
                    //    {
                    //        column.Description = Common.GetColumnDescription(ObjectName, column.ColumnName);
                    //    }
                    //}

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

        ///// <summary>
        ///// Updates the column desc.
        ///// </summary>
        ///// <param name="columnName">The column name.</param>
        ///// <param name="description">The description.</param>
        //public void UpdateColumnDesc(string columnName, string description)
        //{
        //    if (ConnectionString.Length == 0) return;

        //    DBColumn? column = GetColumn(columnName);

        //    if (column != null)
        //    {
        //        column.Description = description;

        //        var conn = new SqlConnection(ConnectionString);
        //        try
        //        {
        //            var cmd = new SqlCommand("ADMIN.usp_AddColumnDescription", conn) { CommandType = CommandType.StoredProcedure };

        //            cmd.Parameters.AddWithValue("@TableName", ObjectName.FullName);
        //            cmd.Parameters.AddWithValue("@ColumnName", columnName);
        //            cmd.Parameters.AddWithValue("@Description", description);

        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {
        //            Common.MsgBox(ex.Message, MessageBoxIcon.Error);
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

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
        public void UpdateTableDesc(string newDescription)
        {
            Description = newDescription;

            if (!ObjectName.IsEmpty() && ConnectionString.Length > 0 && newDescription.Length > 0)
            {
                string newDesc = newDescription.Replace("'", "''");
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
                )
                BEGIN
                    EXEC sys.sp_updateextendedproperty
                        @name = N'MS_Description',
                        @value = '{newDesc}',
                        @level0type = N'SCHEMA',
                        @level0name = '{ObjectName.Schema}',
                        @level1type = N'{GetObjectType()}',
                        @level1name = '{ObjectName.Name}';
                END
                ELSE
                BEGIN
                    EXEC sys.sp_addextendedproperty
                        @name = N'MS_Description',
                        @value = '{newDesc}',
                        @level0type = N'SCHEMA',
                        @level0name = '{ObjectName.Schema}',
                        @level1type = N'{GetObjectType()}',
                        @level1name = '{ObjectName.Name}';
                END";

                    using SqlCommand command = new(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
                //var conn = new SqlConnection(ConnectionString);
                //try
                //{
                //    var cmd = new SqlCommand("ADMIN.usp_AddObjectDescription", conn) { CommandType = CommandType.StoredProcedure };

                //    cmd.Parameters.AddWithValue("@TableName", ObjectName.FullName);
                //    cmd.Parameters.AddWithValue("@Description", description);

                //    conn.Open();
                //    cmd.ExecuteNonQuery();
                //}
                //catch (Exception ex)
                //{
                //    Common.MsgBox(ex.Message, MessageBoxIcon.Error);
                //}
                //finally
                //{
                //    conn.Close();
                //}
            }
        }

        private string GetObjectType()
        {
            string objectName = ObjectName.Name;

            // You can add more cases to handle different object types if needed
            if (objectName.ToLower().StartsWith("sp_") || objectName.ToLower().StartsWith("usp_"))
            {
                return "PROCEDURE";
            }
            else if (objectName.ToLower().StartsWith("vw_") || objectName.ToLower().StartsWith("v_"))
            {
                return "VIEW";
            }
            else
            {
                return "TABLE";
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