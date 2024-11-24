using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    internal class SharePoint
    {
        private readonly SQLServerConnections _connections = new();

        //private int _connectionCount = 0;
        private string? _database = string.Empty;

        //private SQLDatabaseConnectionItem? _selectedConnection;
        private string? _server = string.Empty;

        private readonly System.Text.StringBuilder _script = new();

        /// <summary>
        /// Scan user tables in the database and generate the creation script for them
        /// </summary>
        public string BuildTableList(string schemaName, IProgress<int> progress)
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
                AppendLine("\t\t</tr>");

                string sql;
                if (schemaName.Length == 0)
                    sql = "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables order by table_schema, table_name";
                else
                    sql = string.Format("SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables WHERE SCHEMA_NAME(schema_id) = N'{0}' order by table_schema, table_name", schemaName);

                var cmd = new SqlCommand(sql, conn)
                { CommandType = CommandType.Text };
                conn.Open();
                var ds = new DataSet();
                var dat = new SqlDataAdapter(cmd);
                dat.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        int percentComplete = (i * 100) / ds.Tables[0].Rows.Count;
                        if (percentComplete > 0 && percentComplete % 2 == 0)
                        {
                            progress.Report(percentComplete + 1);
                        }

                        DataRow dr = ds.Tables[0].Rows[i];
                        string tableSchema = (string)dr[0];
                        string tableName = (string)dr[1];
                        AppendLine("\t\t<tr>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + tableSchema + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + string.Format("[[{0}.{1}|{1}]]", tableSchema, tableName) + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName }) + "</td>");
                        //AppendLine(string.Format("| {0} || [[DW Table: {0}.{1}|{1}]] || {2}", tableSchema, tableName, Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName })));
                        AppendLine("\t\t</tr>");
                    }
                }

                AppendLine("\t</tbody>");
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
        public string BuildViewList(string schemaName, IProgress<int> progress)
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
                AppendLine("\t\t</tr>");

                string sql;
                if (schemaName.Length == 0)
                    sql = "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views ORDER BY table_schema, table_name";
                else
                    sql = string.Format("SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views WHERE SCHEMA_NAME(schema_id) = N'{0}' order by table_schema, table_name", schemaName);
                var cmd = new SqlCommand(sql, conn)
                { CommandType = CommandType.Text };
                conn.Open();
                var ds = new DataSet();
                var dat = new SqlDataAdapter(cmd);
                dat.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        int percentComplete = (i * 100) / ds.Tables[0].Rows.Count;
                        if (percentComplete > 0 && percentComplete % 2 == 0)
                        {
                            progress.Report(percentComplete + 1);
                        }

                        DataRow dr = ds.Tables[0].Rows[i];
                        string tableSchema = (string)dr[0];
                        string tableName = (string)dr[1];
                        AppendLine("\t\t<tr>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + tableSchema + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + string.Format("[[{0}.{1}|{1}]]", tableSchema, tableName) + "</td>");
                        AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + Common.GetTableDescription(new ObjectName() { Schema = tableSchema, Name = tableName, ObjectType = ObjectName.ObjectTypeEnums.View }) + "</td>");
                        AppendLine("\t\t</tr>");
                    }
                }

                AppendLine("\t</tbody>");
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
                AppendLine("\t\t</tr>");

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
                AppendLine("\t</tbody>");
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
                AppendLine("\t\t</tr>");

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
                AppendLine("\t</tbody>");
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
                AppendLine("\t\t</tr>");

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
                AppendLine("\t</tbody>");
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
            AppendLine("\t\t</tr>");

            GetTableDefinition(objectName);

            AppendLine("\t</tbody>");
            AppendLine("\t</table>");
            AppendLine("</div>");

            if (objectName.Schema.Equals("PCR", StringComparison.CurrentCultureIgnoreCase))
            {
                // exclude the table that the name starts with "vw_" or "LT_" or "AT_" or contains "Mapping"
                bool exclude = false;
                if (objectName.Name.StartsWith("vw_")) exclude = true;
                if (objectName.Name.StartsWith("LT_")) exclude = true;
                if (objectName.Name.StartsWith("AT_")) exclude = true;
                if (objectName.Name.IndexOf("Mapping") > 0) exclude = true;

                if (!exclude)
                {
                    //BuildFormList(objectName);
                    BuildVariableDefinition(objectName);

                    if (!IsExcludePCRTable(objectName.Name))
                    {
                        // add ETL section
                        AppendLine("<div>");
                        AppendLine("<h2>ETL Process to Build This Table</h2>");
                        AppendLine("<ul>");
                        AppendLine(string.Format("<li> The data is copied from [[PCRL1.{0}]] table by removing the data from the&#160;draft&#160;or invalid forms.</li>", objectName.Name));
                        AppendLine("</ul>");
                        AppendLine("</div>");
                        AppendLine("<div>");
                        AppendLine("<h2>Codes to Build This Table</h2>");
                        AppendLine("<p>");
                        AppendLine("Paste codes here");
                        AppendLine("</p>");
                        AppendLine("</div>");
                    }
                }
                //// add footer
                //AppendLine("<hr/>");
                //AppendLine("<div>Back to [[PCR database tables (CVI.Source)]]</div>");
                //AppendLine("<div>Back to [[Data warehouse tables]]</div>");
            }
            else if (objectName.Schema.Equals("PCRL1", StringComparison.CurrentCultureIgnoreCase))
            {
                BuildFormListL1(objectName);
                BuildVariableDefinitionL1(objectName);
                BuildTargetTables(objectName);

                //// add footer
                //AppendLine("<hr/>");
                //AppendLine("<div>Back to [[PCR database tables (CVI.Source)]]</div>");
                //AppendLine("<div>Back to [[Data warehouse tables]]</div>");
            }

            return _script.ToString();
        }

        private bool IsExcludePCRTable(string tableName)
        {
            if (tableName.StartsWith("vw_")) return true;
            if (tableName.StartsWith("LT_")) return true;
            if (tableName.StartsWith("AT_")) return true;
            if (tableName.IndexOf("Mapping") > 0) return true;

            var excludeList = new List<string>() {
                "EoP","EoS","EoS_HRD","EoS_OnHold","EoS_THV","Form","OrganizationHA" };

            foreach (var item in excludeList)
            {
                if (item.Equals(tableName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        private void BuildFormList(ObjectName objectName)
        {
            AppendLine("<div>");
            AppendLine("<h2>Questionnaire responses (Forms)&#160;where the data extracted from</h2>");
            AppendLine(Common.QueryDataToHTMLTable(string.Format("SELECT distinct Program, FormName [Form Name], FormTitle [Form Title], QuestionnaireId, '' AS Version, '' AS [Start Date], '' AS [End Date] FROM ETL.vw_MappingControl WHERE targettable = '{0}'", objectName.Name)));
            AppendLine("</div>");
        }

        private void BuildFormListL1(ObjectName objectName)
        {
            AppendLine("<div>");
            AppendLine("<h2>Questionnaire responses (Forms)&#160;where the data extracted from</h2>");
            AppendLine(Common.QueryDataToHTMLTable(string.Format("SELECT distinct Program, FormName [Form Name], FormTitle [Form Title], QuestionnaireId, '' AS Version, '' AS [Start Date], '' AS [End Date] FROM ETL.vw_MappingControl WHERE L1TableName = '{0}'", objectName.Name)));
            AppendLine("</div>");
        }

        private void BuildTargetTables(ObjectName objectName)
        {
            string targetTables = string.Empty;
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                string sql = string.Format("SELECT DISTINCT TargetTable FROM ETL.vw_MappingControl WHERE L1TableName = '{0}'", objectName.Name);
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string targetTable = dr.GetString("TargetTable");
                    if (targetTables.Length > 0)
                    {
                        targetTables += string.Format(", and [[PCR.{0}]]", targetTable);
                    }
                    else
                    {
                        targetTables = string.Format("[[PCR.{0}]]", targetTable);
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

            if (targetTables.Length > 0)
            {
                AppendLine("<div>");
                AppendLine("<h2>Target Table(s)</h2>");
                AppendLine(string.Format("<p>PCRL1.{0} is a raw data table. The data will be processed and copied to {1} during the publishing process.</p>", objectName.Name, targetTables));
                AppendLine("</div>");
            }
        }

        private void BuildVariableDefinitionL1(ObjectName objectName)
        {
            AppendLine("<div>");
            AppendLine("<h2>Questionnaire items (Variables)&#160;where the data extracted from for the target column</h2>");
            AppendLine("</div>");

            string sql = string.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME NOT IN ('EpisodeID', 'FormID', 'ParentFormID', 'CardID') AND TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' ORDER BY ORDINAL_POSITION", objectName.Schema, objectName.Name);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string colName = dr.GetString("COLUMN_NAME");
                    BuildColumnDefinitionL1(objectName.Name, colName);
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

        private void BuildVariableDefinition(ObjectName objectName)
        {
            AppendLine("<h2>ETL Process of Each Column</h2>");

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
                    //if (colName.ToLower().StartsWith("cvi"))
                    //{
                    //    AppendLine("<div>");
                    //    AppendLine(string.Format("<h3 style=\"margin-top: 10px;\">Column: {0}</h3>", colName));
                    //    AppendLine("<p>The column is used to replicate the coding code extracted from the source.</p>");
                    //    AppendLine("</div>");
                    //}
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

        private void BuildColumnDefinitionL1(string tableName, string columnName)
        {
            AppendLine("<div>");
            AppendLine(string.Format("\t\t<h3>Column: {0}</h3>", columnName));

            string sql = string.Format("SELECT FormName [Form Name], QuestionnaireId, LinkId, QuestionTitle [Front-End Field Name], SourceDataType [CVI Data Type] FROM ETL.vw_MappingControl WHERE L1TableName = '{0}' AND L1ColumnName = '{1}' ORDER BY LinkID, FormName", tableName, columnName);
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
                    if (dt.Rows.Count > 0)
                    {
                        AppendLine(Common.DataTableToHTML(dt));

                        //DataRow dr = dt.Rows[0];
                        //string dataType = (string)dr["CVI Data Type"];
                        //if (dataType.Equals("choice", StringComparison.CurrentCultureIgnoreCase))
                        //    BuildColumnChoiceOptions(tableName, columnName);
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

            AppendLine("</div>");
        }

        private string BuildColumnChoiceOptions(string tableName, string columnName)
        {
            var sb = new System.Text.StringBuilder();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("Dict.usp_GetQuestionnaireItemChoices", conn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@TableName", tableName);
                cmd.Parameters.AddWithValue("@ColumnName", columnName);
                conn.Open();

                var dat = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dat.Fill(ds);

                if (ds?.Tables.Count > 0)
                {
                    var dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        //AppendLine("<div>");
                        //AppendLine(string.Format("<p>Choice options:</p>", columnName));

                        sb.AppendLine(Common.DataTableToHTML(dt, 0, 0));
                        //AppendLine("</div>");
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
            return sb.ToString();
        }

        private class MappingControl
        {
            public MappingControl()
            {
                L1Columns = new List<string>();
                L1Tables = new List<string>();
                SourceDataTypes = new List<string>();
                MappingCategories = new List<string>();
                MappingCategorieIDs = new List<short>();
                DataTransformType = string.Empty;
            }

            public List<string> L1Tables { get; set; }

            public List<string> L1Columns { get; set; }

            public List<string> SourceDataTypes { get; set; }

            public List<short> MappingCategorieIDs { get; set; }

            public List<string> MappingCategories { get; set; }

            public string DataTransformType { get; set; }

            public short DataTransformTypeID { get; set; }

            public bool Open(string tableName, string columnName)
            {
                bool result = false;

                var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                try
                {
                    var cmd = new SqlCommand("DICT.usp_GetETLImtemsByTargetColumn", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@TargetTable", tableName);
                    cmd.Parameters.AddWithValue("@TargetColumn", columnName);
                    conn.Open();

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string l1Table = reader.GetString("L1TableName");
                        if (!L1Tables.Contains(l1Table))
                        {
                            L1Tables.Add(l1Table);
                        }
                        string l1Column = reader.GetString("L1ColumnName");
                        if (!L1Columns.Contains(l1Column))
                        {
                            L1Columns.Add(l1Column);
                        }
                        string sourceDataType = reader.GetString("SourceDataType");
                        if (!SourceDataTypes.Contains(sourceDataType))
                        {
                            SourceDataTypes.Add(sourceDataType);
                        }
                        DataTransformTypeID = reader.GetInt16("ConvertMethod");
                        DataTransformType = reader.GetString("TransformType");

                        short mappingCat = reader.GetInt16("MappingCat");
                        if (mappingCat > 0 && !MappingCategorieIDs.Contains(mappingCat))
                        {
                            MappingCategorieIDs.Add(mappingCat);
                            MappingCategories.Add(reader.GetString("CategoryName"));
                        }
                    }
                    reader.Close();
                    result = true;
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
        }

        private void BuildColumnDefinition(string tableName, string columnName)
        {
            var mapping = new MappingControl();
            if (mapping.Open(tableName, columnName))
            {
                AppendLine("<div>");
                AppendLine(string.Format("<h3 style=\"margin-top: 10px;\">Column: {0}</h3>", columnName));

                AppendLine("<table cellspacing=\"0\" style=\"border-collapse: collapse; border-spacing: 0;\">");
                AppendLine("\t<tbody>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t\t<td style=\"padding-left: 20px;\">Source table:</td>");
                AppendLine(string.Format("<td style=\"padding-left: 10px;\">{0}</td>", GetSourceTables(mapping.L1Tables)));
                AppendLine("\t\t</tr>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t\t<td style=\"padding-left: 20px;\">Source columns:</td>");
                AppendLine(string.Format("<td style=\"padding-left: 10px;\">​{0}</td>", String.Join(",", mapping.L1Columns)));
                AppendLine("\t\t</tr>");
                AppendLine("\t\t<tr>");
                AppendLine("\t\t\t<td style=\"padding-left: 20px;\">Source data type:</td>");
                var sourceDataType = String.Join(",", mapping.SourceDataTypes);
                AppendLine(string.Format("<td style=\"padding-left: 10px;\">​{0}</td>", sourceDataType));
                AppendLine("\t\t</tr>");
                if (sourceDataType == "choice")
                {
                    AppendLine("\t\t<tr>");
                    AppendLine("\t\t\t<td style=\"padding-left: 20px; vertical-align: top;\">​Choice options:</td>");
                    AppendLine("<td style=\"padding-left: 10px;\">");
                    AppendLine(BuildColumnChoiceOptions(tableName, columnName));
                    AppendLine("</td>");
                    AppendLine("\t\t</tr>");
                }
                AppendLine("\t\t<tr>");
                AppendLine("\t\t\t<td style=\"padding-left: 20px;\">Data transform type:</td>");
                AppendLine(string.Format("<td style=\"padding-left: 10px;\">​{0}</td>", mapping.DataTransformType));
                AppendLine("\t\t</tr>");
                if (mapping.DataTransformTypeID == 5)
                {
                    string mappintCats = string.Empty;
                    for (int i = 0; i < mapping.MappingCategorieIDs.Count; i++)
                    {
                        if (i == 0)
                        {
                            mappintCats = string.Format("({0}) {1}", mapping.MappingCategorieIDs[i], mapping.MappingCategories[i]);
                        }
                        else
                        {
                            mappintCats += string.Format(", ({0}) {1}", mapping.MappingCategorieIDs[i], mapping.MappingCategories[i]);
                        }
                    }
                    AppendLine("\t\t<tr>");
                    AppendLine("\t\t\t<td style=\"padding-left: 20px;\">Mapping categories:</td>");
                    AppendLine(string.Format("<td style=\"padding-left: 10px;\">​{0}</td>", mappintCats));
                    AppendLine("\t\t</tr>");
                }
                AppendLine("\t\t<tr>");
                AppendLine("\t\t\t<td style=\"padding-left: 20px;\">Process details:</td>");
                if (mapping.DataTransformTypeID == 5)
                {
                    AppendLine(string.Format("<td style=\"padding-left: 10px;\">​{0}</td>", "​Convert to the value by the [[ETL.Mapping|mapping table]]"));
                }
                else
                {
                    AppendLine(string.Format("<td style=\"padding-left: 10px;\">​{0}</td>", "Replicate the value extracted from the source"));
                }
                AppendLine("\t\t</tr>");

                AppendLine("\t</tbody>");
                AppendLine("</table>");
                AppendLine("</div>");
            }
        }

        /// <summary>
        /// Get level 1 table names
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string GetSourceTables(List<string> tableNames)
        {
            string results = string.Empty;
            for (int i = 0; i < tableNames.Count; i++)
            {
                if (i == 0)
                {
                    results = string.Format("​[[PCRL1.{0}]]", tableNames[i]);
                }
                else
                {
                    results += string.Format("​, [[PCRL1.{0}]]", tableNames[i]);
                }
            }

            return results;
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
            AppendLine("<div>");
            AppendLine("<h2>Table Values:</h2>");
            string sql = string.Format("SELECT * FROM {0}", tableName);
            AppendLine(Common.QueryDataToHTMLTable(sql));
            AppendLine("</div>");

            return _script.ToString();
        }
    }
}