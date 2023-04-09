using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    internal class Wiki
    {
        private readonly SQLServerConnections _connections = new();
        private int _connectionCount = 0;
        private string? _database = string.Empty;
        //private SQLDatabaseConnectionItem? _selectedConnection;
        private string? _server = string.Empty;
        private readonly System.Text.StringBuilder _script = new();

        /// <summary>
        /// Scan user tables in the database and generate the creation script for them
        /// </summary>
        public string BuildTableList(string schemaName)
        {
            _script.Clear();

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("|-");
                AppendLine("! Schema !! Table Name !! Description");
                string sql;
                if (schemaName.Length == 0)
                    sql = "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables order by table_schema, table_name";
                else
                    sql = string.Format("SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables WHERE SCHEMA_NAME(schema_id) = N'{0}' order by table_schema, table_name", schemaName);

                var cmd = new SqlCommand(sql, conn)
                { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tableSchema = dr.GetString(0);
                    string tableName = dr.GetString(1);
                    AppendLine("|-");
                    AppendLine(string.Format("| {0} || [[DW Table: {0}.{1}|{1}]] || {2}", tableSchema, tableName, Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName })));
                }

                dr.Close();
                AppendLine("|}");
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return _script.ToString();
        }

        /// <summary>
        /// Scan user view in the database and generate the creation script for them
        /// </summary>
        public string BuildViewList(string schemaName)
        {
            _script.Clear();

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("|-");
                AppendLine("! Schema !! View Name !! Description");
                string sql;
                if (schemaName.Length == 0)
                    sql = "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views ORDER BY table_schema, table_name";
                else
                    sql = string.Format("SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views WHERE SCHEMA_NAME(schema_id) = N'{0}' order by table_schema, table_name", schemaName);
                var cmd = new SqlCommand(sql, conn)
                { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tableSchema = dr.GetString(0);
                    string tableName = dr.GetString(1);
                    AppendLine("|-");
                    AppendLine(string.Format("| {0} || [[View: {0}.{1}|{1}]] || {2}", tableSchema, tableName, ""));
                }

                dr.Close();
                AppendLine("|}");
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return _script.ToString();
        }

        /// <summary>
        /// Scan user view in the database and generate the creation script for them
        /// </summary>
        public string BuildFunctionList(string schemaName)
        {
            _script.Clear();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("|-");
                AppendLine("! Schema !! Function !! Description");
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' ORDER BY ROUTINE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string schema = dr.GetString(0);
                    bool generate = true;
                    if (schemaName.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string spName = dr.GetString(1);
                        AppendLine("|-");
                        AppendLine(string.Format("| {0} || [[Function: {0}.{1}|{1}]] || {2}", schema, spName, ""));
                    }
                }

                dr.Close();
                AppendLine("|}");
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return _script.ToString();
        }

        /// <summary>
        /// Scan user view in the database and generate the creation script for them
        /// </summary>
        public string BuildSPList(string schemaName)
        {
            _script.Clear();

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("|-");
                AppendLine("! Schema !! Stored Procedure !! Description");
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string schema = dr.GetString(0);
                    bool generate = true;
                    if (schemaName.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string spName = dr.GetString(1);
                        AppendLine("|-");
                        AppendLine(string.Format("| {0} || [[Stored Procedure: {0}.{1}|{1}]] || {2}", schema, spName, ""));
                    }
                }

                dr.Close();
                AppendLine("|}");
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return _script.ToString();
        }

        public string TextToWikiTable(string metaData)
        {
            metaData = metaData.Replace("\r\n", "\r");
            metaData = metaData.Replace("\n\r", "\r");
            var lines = metaData.Split('\r');
            if (lines.Length > 1)
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("|-");
                AppendLine(TabToRow(lines[0], "!!"));
                for (int i = 1; i < lines.Length; i++)
                {
                    AppendLine("|-");
                    AppendLine(TabToRow(lines[i]));
                }
                AppendLine("|}");
            }
            return _script.ToString();
        }

        /// <summary>
        /// Generate contect for wiki of the give table
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        public string GetTableDef(ObjectName objectName)
        {
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
            {
                AppendLine(string.Format("===TABLE NAME: {0}.{1}===", objectName.Schema, objectName.Name));
            }
            else
            {
                AppendLine(string.Format("===VIEW NAME: {0}.{1}===", objectName.Schema, objectName.Name));
            }
            var objectDesc = Common.GetTableDescription(objectName);
            if (objectDesc.Length > 0)
            {
                AppendLine(":" + objectDesc);
            }
            AppendLine("{| class=\"wikitable\"");
            AppendLine("|-");
            AppendLine("! Col ID !! Name !! Data Type !! Description");
            GetTableDefinition(objectName);
            AppendLine("|}");

            return _script.ToString();
        }

        /// <summary>
        /// Get table structure for wiki
        /// </summary>
        /// <param name="TableSchema">Schame name</param>
        /// <param name="TableName">Table name</param>
        /// <returns></returns>
        private void GetTableDefinition(ObjectName objectName)
        {
            string sql = string.Format("SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = N'{0}' AND TABLE_NAME = N'{1}' ORDER BY ORDINAL_POSITION", objectName.Schema, objectName.Name);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    AppendLine("|-");
                    int colID = dr.GetInt32("ORDINAL_POSITION");
                    string colName = dr.GetString("COLUMN_NAME");
                    string dataType = dr.GetString("DATA_TYPE");
                    if (dr["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                    {
                        dataType = string.Format("{0}({1})", dataType, dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }

                    AppendLine(string.Format("| {0} || {1} || {2} || {3}", colID, colName, dataType, Common.GetColumnDescription(objectName, colName)));
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
        /// Build value list of the given table for wiki
        /// </summary>
        /// <param name="tableName"></param>
        public string GetTableValues(string tableName)
        {
            AppendLine("Table Values:");
            AppendLine("{| class=\"wikitable\"");
            string sql = string.Format("SELECT * FROM {0}", tableName);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var ds = new DataSet();
                var dat = new SqlDataAdapter(cmd);
                dat.Fill(ds);
                if (ds.Tables.Count == 1)
                {
                    // output table columns
                    var dt = ds.Tables[0];
                    string strColumns = "! ";
                    for (int i = 0, loopTo = dt.Columns.Count - 1; i <= loopTo; i++)
                    {
                        if (i == 0)
                        {
                            strColumns += dt.Columns[i].ColumnName;
                        }
                        else
                        {
                            strColumns += " !! " + dt.Columns[i].ColumnName;
                        }
                    }

                    AppendLine("|-");
                    AppendLine(strColumns);

                    // output values
                    foreach (DataRow r in dt.Rows)
                    {
                        AppendLine("|-");
                        string strValues = "| ";
                        for (int i = 0, loopTo1 = dt.Columns.Count - 1; i <= loopTo1; i++)
                        {
                            if (i == 0)
                            {
                                strValues += r[i].ToString();
                            }
                            else
                            {
                                strValues += " || " + r[i].ToString();
                            }
                        }

                        AppendLine(strValues);
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

            AppendLine("|}");
            AppendLine("</br>");

            return _script.ToString();
        }

        /// <summary>
        /// Convert tab to row
        /// </summary>
        /// <param name="values"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string TabToRow(string values, string delimiter = "||")
        {
            var columns = values.Split('\t');
            string results = delimiter[..1] + " ";
            for (int i = 0; i < columns.Length; i++)
            {
                if (i == 0)
                {
                    results += columns[i];
                }
                else
                {
                    results += " " + delimiter + " " + columns[i];
                }
            }
            return results;
        }

        /// <summary>
        /// Append text to the bottom of the text box
        /// </summary>
        /// <param name="text"></param>
        private void AppendLine(string text)
        {
            //sqlTextBox.AppendText(text + Environment.NewLine);
            _script.AppendLine(text);
        }
    }
}