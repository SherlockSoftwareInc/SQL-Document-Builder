using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The common class.
    /// </summary>
    internal class Common
    {
        /// <summary>
        /// Get the database object type.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="objectSchema">The object schema.</param>
        /// <param name="objectName">The object name.</param>
        /// <returns>A string.</returns>
        static string GetObjectType(string connectionString, ObjectName objectName)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            string query = $@"
                SELECT TABLE_TYPE
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = '{objectName.Schema}' AND TABLE_NAME = '{objectName.Name}';";

            using (SqlCommand command = new(query, connection))
            {
                string? objectType = command.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(objectType))
                {
                    return "Unknown"; // Object not found or not a table/view
                }
                else if (objectType == "BASE TABLE")
                {
                    return "Table";
                }
                else if (objectType == "VIEW")
                {
                    return "View";
                }
                else
                {
                    return "Other"; // Handle other object types as needed
                }
            }
        }


        /// <summary>
        /// Gets the table description.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A string.</returns>
        public static string GetTableDescription(ObjectName objectName)
        {
            string result = string.Empty;
            string sql = string.Format(String.Format("SELECT value FROM fn_listextendedproperty (NULL, 'schema', N'{0}', '{2}', N'{1}', default, default) WHERE name = N'MS_Description'", objectName.Schema, objectName.Name, (objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")));

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
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

            return result;
        }

        /// <summary>
        /// Gets the column description.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="column">The column.</param>
        /// <returns>A string.</returns>
        public static string GetColumnDescription(ObjectName objectName, string column)
        {
            string result = string.Empty;
            string sql;
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.views T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", objectName.Schema, objectName.Name, column);
            }
            else
            {
                sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", objectName.Schema, objectName.Name, column);
            }
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr.GetString(0);   // dr[0].ToString();
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Gets the column desc.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns>A string.</returns>
        public static string GetColumnDesc(string schema, string table, string column)
        {
            string result = string.Empty;
            string sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", schema, table, column);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr.GetString(0);   // dr[0].ToString();
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Input the box.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="title">The title.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A string.</returns>
        public static string InputBox(string prompt, string title = "", string defaultValue = "")
        {
            using (var dlg = new InputBox()
            {
                Title = title.Length == 0 ? Application.ProductName : title,
                Prompt = prompt,
                Default = defaultValue
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.InputText;
                }
            }
            return "";
        }

        /// <summary>
        /// Msg the box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="icon">The icon.</param>
        public static DialogResult MsgBox(string message, MessageBoxIcon icon)
        {
           return MessageBox.Show(message, "Message", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Remove quota.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A string.</returns>
        public static string RemoveQuota(string text)
        {
            if (text.StartsWith("[") && text.EndsWith("]"))
            {
                return text[1..^1];
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// Queries the data to an HTML table asynchronously.
        /// </summary>
        /// <param name="sql">The SQL query to fetch data.</param>
        /// <returns>A Task<string> containing the HTML table.</returns>
        public static async Task<string> QueryDataToHTMLTableAsync(string sql)
        {
            var sb = new StringBuilder();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);

            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                await conn.OpenAsync();

                var dat = new SqlDataAdapter(cmd);
                var ds = new DataSet();

                // Fill the dataset asynchronously
                await Task.Run(() => dat.Fill(ds));

                if (ds?.Tables.Count > 0)
                {
                    sb.AppendLine(DataTableToHTML(ds.Tables[0]));
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                await conn.CloseAsync();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Data the table to HTML.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="margin">The margin.</param>
        /// <param name="fontSize">The font size.</param>
        /// <returns>A string.</returns>
        public static string DataTableToHTML(DataTable dt, int margin = 1, Single fontSize = 14)
        {
            var sb = new System.Text.StringBuilder();
            if (fontSize > 0)
            {
                sb.AppendLine(string.Format("<table class=\"wikitable\" style=\"margin: {0}em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: {1}px; background-color: #f8f9fa;\">", margin, fontSize));
            }
            else
            {
                sb.AppendLine(string.Format("<table class=\"wikitable\" style=\"margin: {0}em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; background-color: #f8f9fa;\">", margin));
            }
            sb.AppendLine("\t<tbody>");
            sb.AppendLine("\t\t<tr>");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colName = dt.Columns[i].ColumnName;
                sb.AppendLine(string.Format("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">{0}</th>", colName));
            }
            sb.AppendLine("\t\t</tr>");

            foreach (DataRow dataRow in dt.Rows)
            {
                sb.AppendLine("\t\t<tr>");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string? value = "&#160;";
                    if (dataRow[i] != DBNull.Value)
                        value = Convert.ToString(dataRow[i]);

                    sb.AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + value?.ToString() + "</td>");
                }
                sb.AppendLine("\t\t</tr>");
            }

            sb.AppendLine("\t</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Get the object list.
        /// </summary>
        /// <param name="objType">The obj type.</param>
        /// <returns><![CDATA[List<ObjectName>]]></returns>
        public static List<ObjectName> GetObjectList(ObjectName.ObjectTypeEnums objType)
        {
            var tables = new List<ObjectName>();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var sql = string.Format("SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = @TableType ORDER BY TABLE_SCHEMA, TABLE_NAME");
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@TableType", objType == ObjectName.ObjectTypeEnums.View ? "View" : "BASE TABLE");
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var schemaName = reader.GetString(0);
                    if (!schemaName.Equals("sys", StringComparison.CurrentCultureIgnoreCase))
                    {
                        tables.Add(new ObjectName()
                        {
                            ObjectType = objType,
                            Schema = schemaName,
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return tables;
        }

        ///// <summary>
        ///// Get table list from the database
        ///// </summary>
        //public static List<ObjectName> GetViewList()
        //{
        //    var tables = new List<ObjectName>();
        //    var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
        //    try
        //    {
        //        var cmd = new SqlCommand("SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'View' ORDER BY TABLE_SCHEMA, TABLE_NAME", conn) { CommandType = CommandType.Text };
        //        conn.Open();
        //        var reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            tables.Add(new ObjectName()
        //            {
        //                ObjectType = ObjectName.ObjectTypeEnums.View,
        //                Schema = reader.GetString(0),
        //                Name = reader.GetString(1)
        //            });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Common.MsgBox(ex.Message, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return tables;
        //}

        /// <summary>
        /// Gets the table columns.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <returns><![CDATA[List<string>]]></returns>
        public static List<string> GetTableColumns(ObjectName tableName)
        {
            var columns = new List<string>();

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(string.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}'", tableName.Schema, tableName.Name), conn) { CommandType = CommandType.Text };
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return columns;
        }

        /// <summary>
        /// Msgs the box.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="yesNo">The yes no.</param>
        /// <param name="warning">The warning.</param>
        /// <returns>A DialogResult.</returns>
        internal static DialogResult MsgBox(string v, MessageBoxButtons yesNo, MessageBoxIcon warning)
        {
            return MessageBox.Show(v, "Message", yesNo, warning);
        }
    }
}