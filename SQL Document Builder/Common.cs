using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    internal class Common
    {
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
        /// Get description of a column
        /// </summary>
        /// <param name="schema">object schema</param>
        /// <param name="table">object name</param>
        /// <param name="column">column name</param>
        /// <returns></returns>
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

        ///// <summary>
        ///// Show input box
        ///// </summary>
        ///// <param name="prompt">Prompt text</param>
        ///// <param name="title">Title of the input box</param>
        ///// <param name="defaultValue">default value</param>
        ///// <returns></returns>
        //public static string InputBox(string prompt, string title, string defaultValue)
        //{
        //    using (var dlg = new InputBox()
        //    {
        //        Title = title,
        //        Prompt = prompt,
        //        Default = defaultValue
        //    })
        //    {
        //        if (dlg.ShowDialog() == DialogResult.OK)
        //        {
        //            return dlg.InputText;
        //        }
        //    }
        //    return "";
        //}

        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        public static void MsgBox(string message, MessageBoxIcon icon)
        {
            MessageBox.Show(message, "Message", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Remove quota from the object name
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

        public static string QueryDataToHTMLTable(string sql)
        {
            var sb = new System.Text.StringBuilder();

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn)
                { CommandType = CommandType.Text };
                conn.Open();

                var dat = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dat.Fill(ds);

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
                conn.Close();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns HTML script for the data in the data table
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
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
        /// Get table list from the database
        /// </summary>
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

        public static List<string> GetTableColumns(ObjectName tableName) { 
            var columns = new List<string>();

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(string .Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}'", tableName.Schema, tableName.Name), conn) { CommandType = CommandType.Text };
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
    }
}