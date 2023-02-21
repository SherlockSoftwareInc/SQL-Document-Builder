using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    internal class SharePoint
    {
        private readonly SQLServerConnections _connections = new();
        private int _connectionCount = 0;
        private string? _database = string.Empty;
        private SQLDatabaseConnectionItem? _selectedConnection;
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
                AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
                AppendLine("\t<tbody>");
                //AppendLine("\t\t<tr>");
                //AppendLine("\t</tr>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Schema</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Table Name</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Description</th>");
                AppendLine("\t\t<tr>");

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
                    AppendLine("\t\t<tr>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + tableSchema + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + string.Format("[[{0}.{1}|{1}]]", tableSchema, tableName) + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName }) + "</td>");
                    //AppendLine(string.Format("| {0} || [[DW Table: {0}.{1}|{1}]] || {2}", tableSchema, tableName, Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName })));
                    AppendLine("\t\t</tr>");
                }

                dr.Close();
                AppendLine("\t<tbody>");
                AppendLine("\t</table>");
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
                AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
                AppendLine("\t<tbody>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Schema</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">View Name</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Description</th>");
                AppendLine("\t\t<tr>");

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
                    AppendLine("\t\t<tr>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + tableSchema + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + string.Format("[[{0}.{1}|{1}]]", tableSchema, tableName) + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName }) + "</td>");
                    AppendLine("\t\t</tr>");
                }

                dr.Close();
                AppendLine("\t<tbody>");
                AppendLine("\t</table>");
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
                AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
                AppendLine("\t<tbody>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Schema</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Stored Procedure</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Description</th>");
                AppendLine("\t\t<tr>");

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
                        AppendLine("\t\t<tr>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + schema + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + string.Format("[[{0}.{1}|{1}]]", schema, spName) + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + " " + "</td>");
                        AppendLine("\t\t</tr>");
                    }
                }

                dr.Close();
                AppendLine("\t<tbody>");
                AppendLine("\t</table>");
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
                AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
                AppendLine("\t<tbody>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Schema</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Function</th>");
                AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Description</th>");
                AppendLine("\t\t<tr>");

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
                        AppendLine("\t\t<tr>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + schema + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + string.Format("[[{0}.{1}|{1}]]", schema, spName) + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + " " + "</td>");
                        AppendLine("\t\t</tr>");
                    }
                }

                dr.Close();
                AppendLine("\t<tbody>");
                AppendLine("\t</table>");
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

        public string TextToTable(string metaData)
        {
            metaData = metaData.Replace("\r\n", "\r");
            metaData = metaData.Replace("\n\r", "\r");
            var lines = metaData.Split('\r');
            if (lines.Length > 1)
            {
                AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
                AppendLine("\t<tbody>");
                AppendLine("\t\t<tr>");
                var headItems = lines[0].Split('\t');
                foreach (var headItem in headItems)
                {
                    AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">" + headItem + "</th>");
                }
                AppendLine("\t\t<tr>");

                for (int i = 0; i < lines.Length; i++)
                {
                    AppendLine("\t\t<tr>");
                    var columns = lines[i].Split('\t');
                    foreach (var column in columns)
                    {
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + column + "</td>");
                    }
                    AppendLine("\t\t</tr>");
                }
                AppendLine("\t<tbody>");
                AppendLine("\t</table>");
            }
            return _script.ToString();
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

        /// <summary>
        /// Generate contect for wiki of the give table
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        public string GetTableDef(ObjectName objectName)
        {
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
            {
                AppendLine(string.Format("<h1>TABLE NAME: {0}.{1}</h1>", objectName.Schema, objectName.Name));
            }
            else
            {
                AppendLine(string.Format("<h1>VIEW NAME: {0}.{1}</h1>", objectName.Schema, objectName.Name));
            }
            var objectDesc = Common.GetTableDescription(objectName);
            if (objectDesc.Length > 0)
            {
                AppendLine("<p>" + objectDesc + "</p>");
            }

            AppendLine("<div>");
            AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
            AppendLine("\t<tbody>");
            AppendLine("\t\t<tr>");
            AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Col ID</th>");
            AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Name</th>");
            AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Data Type</th>");
            AppendLine("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">Description</th>");
            AppendLine("\t\t<tr>");

            GetTableDefinition(objectName);

            AppendLine("\t<tbody>");
            AppendLine("\t</table>");
            AppendLine("</div>");

            if (objectName.Schema.Equals("PCR", StringComparison.CurrentCultureIgnoreCase))
            {
                BuildFormList(objectName);
                BuildVariableDefinition(objectName);

                // add ETL section
                AppendLine("<div>");
                AppendLine("<h2>ETL Process to Build This Table</h2>");
                AppendLine("<p>There is no data transformation during the ETL process. Data replicated by source.</p>");
                AppendLine("<p>The procedure name or coding code has been mapped to the procedure ID using the mapping table with MappingCategoryID = 3. The CviProc column replicated the procedure name of the coding code from the source.</p>");

                AppendLine("</div>");

                // add footer
                AppendLine("<hr/>");
                AppendLine("<div>Back to [[PCR database tables (CVI.Source)]]</div>");
                AppendLine("<div>Back to [[Data warehouse tables]]</div>");
            }

            return _script.ToString();
        }

        private void BuildFormList(ObjectName objectName)
        {
            AppendLine("<div>");
            AppendLine("<h2>Questionnaires (Forms)&#160;where the data extracted from</h2>");
            QueryDataToHTMLTable(string.Format("SELECT distinct Program, FormName [Form Name], FormTitle [Form Title], QuestionnaireId, '' AS Version, '' AS [Start Date], '' AS [End Date] FROM ETL.vw_MappingControl WHERE targettable = '{0}' AND active = 1", objectName.Name));
            AppendLine("</div>");
        }

        private void BuildVariableDefinition(ObjectName objectName)
        {
            AppendLine("<div>");
            AppendLine("<h2>Questionnaire items (Variables)&#160;where the data extracted from for the target column</h2>");
            AppendLine("</div>");

            string sql = string.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME NOT IN ('EpisodeID', 'FormID', 'ParentFormID', 'CardID') AND TABLE_SCHEMA = 'PCR' AND TABLE_NAME = '{0}' ORDER BY ORDINAL_POSITION", objectName.Name);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string colName = dr.GetString("COLUMN_NAME");
                    if (colName.ToLower().StartsWith("cvi"))
                    {
                        AppendLine("<div>");
                        AppendLine(string.Format("<h3>Column: {0}</h3>", colName));
                        AppendLine("<p>The column is used to replicate the coding code extracted from the source.</p>");
                        AppendLine("</div>");
                    }
                    BuildColumnDefinition(objectName.Name, colName);
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

        private void BuildColumnDefinition(string tableName, string columnName)
        {
            AppendLine("<div>");
            AppendLine(string.Format("\t\t<h3>Column: {0}</h3>", columnName));

            string sql = string.Format("SELECT FormName [Form Name], QuestionTitle [Front-End Field Name], LinkId, TransformType [Data Transform Type], MappingCat [Mapping Category ID], CategoryName [Mapping Category Name],SourceDataType [CVI Data Type] FROM ETL.vw_MappingControl WHERE targettable = '{0}' AND TargetColumn = '{1}' ORDER BY FormName", tableName, columnName);
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
                    var dt = ds.Tables[0];
                    bool isMapping = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int transformType = Convert.ToInt16(dr["Mapping Category ID"]);
                        if (transformType > 0)
                        {
                            isMapping = true;
                        }
                    }
                    if (!isMapping)
                    {
                        dt.Columns.Remove("Mapping Category ID");
                        dt.Columns.Remove("Mapping Category Name");
                    }
                    DataTableToHTML(dt);
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

            AppendLine("</div>");
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
                    int colID = dr.GetInt32("ORDINAL_POSITION");
                    string colName = dr.GetString("COLUMN_NAME");
                    string dataType = dr.GetString("DATA_TYPE");
                    if (dr["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                    {
                        dataType = string.Format("{0}({1})", dataType, dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }
                    AppendLine("\t\t<tr>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + colID.ToString() + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + colName + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + dataType + "</td>");
                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + Common.GetColumnDescription(objectName, colName) + "</td>");
                    AppendLine("\t\t</tr>");
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

        private void QueryDataToHTMLTable(string sql)
        {
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
                    DataTableToHTML(ds.Tables[0]);
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
        }

        private void DataTableToHTML(DataTable dt)
        {
            AppendLine("\t<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
            AppendLine("\t<tbody>");
            AppendLine("\t\t<tr>");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colName = dt.Columns[i].ColumnName;
                AppendLine(string.Format("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">{0}</th>", colName));
            }
            AppendLine("\t\t<tr>");

            foreach (DataRow dataRow in dt.Rows)
            {
                AppendLine("\t\t<tr>");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string? value = " ";
                    if (dataRow[i] != DBNull.Value)
                        value = Convert.ToString(dataRow[i]);

                    AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + value?.ToString() + "</td>");
                }
                AppendLine("\t\t<tr>");
            }

            AppendLine("\t<tbody>");
            AppendLine("\t</table>");
        }
    }
}