using System;
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
            string sql = string.Format(String.Format("SELECT value FROM fn_listextendedproperty (NULL, 'schema', '{0}', '{2}', '{1}', default, default) WHERE name = N'MS_Description'", objectName.Schema, objectName.Name, (objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")));

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr[0].ToString();
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
                    result = dr[0].ToString();
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
                    result = dr[0].ToString();
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
        /// Show input box
        /// </summary>
        /// <param name="prompt">Prompt text</param>
        /// <param name="title">Title of the input box</param>
        /// <param name="defaultValue">default value</param>
        /// <returns></returns>
        public static string InputBox(string prompt, string title, string defaultValue)
        {
            using (var dlg = new InputBox()
            {
                Title = title,
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
                return text.Substring(1, text.Length - 2);
            }
            else
            {
                return text;
            }
        }
    }
}