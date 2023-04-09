using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    internal class DBObject
    {
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
        /// Returns script for add description
        /// </summary>
        /// <returns></returns>
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

        public bool Open(ObjectName objectName, string connectionString)
        {
            return Open(objectName.Schema, objectName.Name, objectName.ObjectType, connectionString);
        }

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
            }

            if (ConnectionString.Length > 0)
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

                    foreach (var column in Columns)
                    {
                        if (column.ColumnName != null)
                            column.Description = Common.GetColumnDescription(ObjectName, column.ColumnName);
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
                Description = GetTableDesc();
            }

            return result;
        }

        /// <summary>
        /// Get description of table/view from the database
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
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