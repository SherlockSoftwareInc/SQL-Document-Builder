using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class Form1 : Form
    {
        private string _fileName = string.Empty;
        private System.IO.StreamWriter _file;

        public Form1()
        {
            InitializeComponent();
        }

        private void storedProcedureButton_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                _file = new(_fileName,  append: false);
                ScanAllSPs();
                _file.Close();
                //// Interaction.MsgBox("Complete", MsgBoxStyle.Information);
            }
        }

        private void ViewButton_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                _file = new(_fileName,append: false);
                ScanAllViews();
                _file.Close();
                //// Interaction.MsgBox("Complete", MsgBoxStyle.Information);
            }

        }

        private void functionButton_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                _file = new(_fileName, append: false);
                ScanAllFunctions();
                _file.Close();
                //// Interaction.MsgBox("Complete", MsgBoxStyle.Information);
            }
        }

        private void tableButton_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                _file = new(_fileName, false);
                ScanAllTables();
                _file.Close();
                //// Interaction.MsgBox("Complete", MsgBoxStyle.Information);
            }

        }

        private void tableWikiButton_Click(object sender, EventArgs e)
        {
            string tableName = Interaction.InputBox("Table name:", "Input table name", "dbo.");
            if (tableName.IndexOf(".") > 0)
            {
                var tableElement = tableName.Split('.');
                if (tableElement.Length == 2)
                {
                    GetTableDefWiki(tableElement[0], tableElement[1]);
                    //// Interaction.MsgBox("Complete");
                }
                else
                {
                    //// Interaction.MsgBox("Failed");
                }
            }
        }

        private void valueWikiButton_Click(object sender, EventArgs e)
        {
            string tableName = Interaction.InputBox("Table name:", "Input table name", "dbo.");
            if (tableName.IndexOf(".") > 0)
            {
                GetTableValues(tableName);
            }
        }

        private void ScanAllSPs()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string schema = dr[0].ToString();
                    string spName = dr[1].ToString();
                    _file.WriteLine("/*====================================================");
                    _file.WriteLine("\t" + spName);
                    _file.WriteLine("======================================================*/");
                    _file.WriteLine(string.Format("DROP PROCEDURE IF EXISTS [{0}].[{1}]", schema, spName));
                    _file.WriteLine("GO");
                    _file.Write(GetSPDefinition(schema, spName));
                    _file.WriteLine("GO");
                    _file.WriteLine(string.Format("GRANT EXEC ON [{0}].[{1}] TO [db_execproc]", schema, spName));
                    _file.WriteLine("GO");
                    _file.WriteLine("");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }
        }

        private void ScanAllViews()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                // Dim cmd As New SqlCommand("SELECT name AS view_name FROM sys.views where SCHEMA_NAME(schema_id) = 'dbo' order by view_name", conn) With {
                var cmd = new SqlCommand("SELECT SCHEMA_NAME(schema_id) AS view_schema, name AS view_name FROM sys.views", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string viewSchema = dr[0].ToString();
                    string viewName = dr[1].ToString();
                    _file.WriteLine("/*====================================================");
                    _file.WriteLine("\t" + viewName);
                    _file.WriteLine("======================================================*/");
                    _file.WriteLine(string.Format("DROP VIEW IF EXISTS [{0}].[{1}]", viewSchema, viewName));
                    _file.WriteLine("GO");
                    _file.Write(GetViewDefinition(viewSchema, viewName));
                    _file.WriteLine("GO");
                    _file.WriteLine("");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }
        }

        private void ScanAllFunctions()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' ORDER BY ROUTINE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string schema = dr[0].ToString();
                    string spName = dr[1].ToString();
                    _file.WriteLine("/*====================================================");
                    _file.WriteLine("\t" + spName);
                    _file.WriteLine("======================================================*/");
                    _file.WriteLine(string.Format("DROP FUNCTION IF EXISTS [{0}].[{1}]", schema, spName));
                    _file.WriteLine("GO");
                    _file.Write(GetSPDefinition(schema, spName));
                    _file.WriteLine("GO");
                    _file.WriteLine(string.Format("--GRANT EXEC ON [{0}].[{1}] TO [db_execproc]", schema, spName));
                    _file.WriteLine("--GO");
                    _file.WriteLine("");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }
        }

        private void ScanAllTables()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var sbTables = new System.Text.StringBuilder();
                sbTables.AppendLine("{| class=\"wikitable\"");
                sbTables.AppendLine("|-");
                sbTables.AppendLine("! Schema !! Table Name !! Description");
                var cmd = new SqlCommand("SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables order by table_schema, table_name", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tableSchema = dr[0].ToString();
                    string tableName = dr[1].ToString();
                    sbTables.AppendLine("|-");
                    sbTables.AppendLine(string.Format("| {0} || [[DW Table: {0}.{1}|{1}]] || ", tableSchema, tableName));
                    if (tableSchema == "dbo")
                    {
                        _file.WriteLine("/*====================================================");
                        _file.WriteLine("\t" + string.Format("DW Table: {0}.{1}", tableSchema, tableName));
                        _file.WriteLine("======================================================*/");
                        _file.WriteLine(string.Format("===TABLE NAME: {0}.{1}===", tableSchema, tableName));
                        _file.WriteLine("{| class=\"wikitable\"");
                        _file.WriteLine("|-");
                        _file.WriteLine("! Col ID !! Name !! Data Type !! Description");
                        _file.Write(GetTableDefinition(tableSchema, tableName));
                        _file.WriteLine("|}");
                        _file.WriteLine("</br>");
                        _file.WriteLine("----");
                        _file.WriteLine("Back to [[DW: Database tables|Database tables]]");
                        _file.WriteLine("[[Category: CSBC data warehouse]]");
                        // _file.WriteLine("")
                        // _file.WriteLine("")
                        _file.WriteLine("");
                    }
                }

                dr.Close();
                sbTables.AppendLine("|}");
                _file.WriteLine(sbTables.ToString());
            }
            catch (Exception ex)
            {
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }
        }

        private string GetTableDefinition(string TableSchema, string TableName)
        {
            var result = new System.Text.StringBuilder();
            string sql = string.Format("SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = N'{0}' AND TABLE_NAME = N'{1}' ORDER BY ORDINAL_POSITION", TableSchema, TableName);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.AppendLine("|-");
                    string colID = dr["ORDINAL_POSITION"].ToString();
                    string colName = dr["COLUMN_NAME"].ToString();
                    string dataType = dr["DATA_TYPE"].ToString();
                    if (dr["CHARACTER_MAXIMUM_LENGTH"]!=DBNull.Value)
                    {
                        dataType = string.Format("{0}({1})", dataType, dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }

                    result.AppendLine(string.Format("| {0} || {1} || {2} || {3}", colID, colName, dataType, GetColumnDesc(TableSchema, TableName, colName)));
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }

            return result.ToString();
        }

        private void GetTableDefWiki(string tableSchema, string tableName)
        {
            var sbTables = new System.Text.StringBuilder();
            sbTables.AppendLine(string.Format("===TABLE NAME: {0}.{1}===", tableSchema, tableName));
            sbTables.AppendLine("{| class=\"wikitable\"");
            sbTables.AppendLine("|-");
            sbTables.AppendLine("! Col ID !! Name !! Data Type !! Description");
            sbTables.AppendLine(GetTableDefinition(tableSchema, tableName));
            sbTables.AppendLine("|}");
            sbTables.AppendLine("</br>");
            sbTables.AppendLine("----");
            sbTables.AppendLine("Back to [[DW: Database tables|Database tables]]");
            sbTables.AppendLine("[[Category: CSBC data warehouse]]");
            Clipboard.SetText(sbTables.ToString());
        }

        private void GetTableValues(string tableName)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Table Values:");
            sb.AppendLine("{| class=\"wikitable\"");
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

                    sb.AppendLine("|-");
                    sb.AppendLine(strColumns);

                    // output values
                    foreach (DataRow r in dt.Rows)
                    {
                        sb.AppendLine("|-");
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

                        sb.AppendLine(strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }

            sb.AppendLine("|}");
            sb.AppendLine("</br>");
            Clipboard.SetText(sb.ToString());
            // Interaction.MsgBox("Complete!");
        }

        private string GetSPDefinition(string schema, string spName)
        {
            string result = string.Empty;
            string sql = string.Format("SELECT definition FROM sys.sql_modules WHERE object_id = (OBJECT_ID(N'[{0}].[{1}]'))", schema, spName);
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
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        private string GetViewDefinition(string viewSchema, string viewName)
        {
            string result = string.Empty;
            string sql = string.Format("select definition from sys.objects o join sys.sql_modules m on m.object_id = o.object_id where o.object_id = OBJECT_ID(N'[{0}].[{1}]') and o.type = 'V'", viewSchema, viewName);
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
                // Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        private string GetColumnDesc(string schema, string table, string column)
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
                //Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

    }
}
