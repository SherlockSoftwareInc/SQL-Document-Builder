using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class TableBuilderForm : Form
    {
        private DataTable _tables;

        public TableBuilderForm()
        {
            InitializeComponent();
        }

        public SQLDatabaseConnectionItem Connection { get; set; }

        private void AddListItem(string tableType, string schema, string tableName)
        {
            ObjectName.ObjectTypeEnums objectType = (tableType == "VIEW") ? ObjectName.ObjectTypeEnums.View : ObjectName.ObjectTypeEnums.Table;
            objectsListBox.Items.Add(new ObjectName()
            {
                ObjectType = objectType,
                Schema = schema,
                Name = tableName
            });
        }

        /// <summary>
        /// Append text to the bottom of the text box
        /// </summary>
        /// <param name="text"></param>
        private void AppendLine(string text)
        {
            sqlTextBox.AppendText(text + Environment.NewLine);
        }

        private void BatchToolStripButton_Click(object sender, EventArgs e)
        {
            using var frm = new BatchColumnDesc();
            frm.ShowDialog();
        }

        private void BuildScript()
        {
            if (objectsListBox.SelectedItem != null)
            {
                if (Connection != null)
                {
                    var objectName = (ObjectName)objectsListBox.SelectedItem;
                    sqlTextBox.Text = string.Empty;

                    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.management.smo.scriptingoptions?view=sql-smo-160
                    ScriptingOptions scriptOptions = new ScriptingOptions()
                    {
                        ContinueScriptingOnError = true,
                        ScriptDrops = scriptDropsCheckBox.Checked,
                        ScriptForCreateDrop = scriptForCreateDropCheckBox.Checked,
                        IncludeIfNotExists = includeIfNotExistsCheckBox.Checked,
                        ExtendedProperties = extendedPropertiesCheckBox.Checked,
                        AnsiPadding = ansiPaddingCheckBox.Checked,
                        NoCollation = noCollationCheckBox.Checked,
                        ScriptData = scriptDataCheckBox.Checked,
                        IncludeHeaders = includeHeadersCheckBox.Checked
                    };

                    //GetTableDefWiki(RemoveQuota(tableElement[0]), RemoveQuota(tableElement[1]));
                    Server server = new Server(Connection.ServerName);
                    Database database = new Database();
                    database = server.Databases[Connection.Database];
                    Table table = database.Tables[objectName.Name, objectName.Schema];
                    //Table table = database.Tables["pt_case"];
                    if (table != null)
                    {
                        StringCollection result = table.Script(scriptOptions);
                        foreach (var line in result)
                        {
                            AppendLine(line);
                        }
                        AppendLine("GO" + Environment.NewLine);

                        IndexCollection indexCol = table.Indexes;
                        foreach (Microsoft.SqlServer.Management.Smo.Index myIndex in table.Indexes)
                        {
                            /* Generating IF EXISTS and DROP command for table indexes */
                            StringCollection indexScripts = myIndex.Script(scriptOptions);
                            foreach (string script in indexScripts)
                            {
                                AppendLine(script);
                            }

                            /* Generating CREATE INDEX command for table indexes */
                            indexScripts = myIndex.Script();
                            foreach (string script in indexScripts)
                            {
                                AppendLine(script);
                            }
                        }

                        //Scripter scripter = new Scripter(server);
                        ///* With ScriptingOptions you can specify different scripting
                        //* options, for example to include IF NOT EXISTS, DROP
                        //* statements, output location etc*/

                        //foreach (Table myTable in database.Tables)
                        //{
                        //    /* Generating IF EXISTS and DROP command for tables */
                        //    StringCollection tableScripts = myTable.Script(scriptOptions);
                        //    foreach (string script in tableScripts)
                        //    {
                        //        AppendLine(script);
                        //    }

                        //    tableScripts = myTable.Script();
                        //    foreach (string script in tableScripts)
                        //    {
                        //        AppendLine(script);
                        //    }

                        //    IndexCollection indexCol = myTable.Indexes;
                        //    foreach (Microsoft.SqlServer.Management.Smo.Index myIndex in myTable.Indexes)
                        //    {
                        //        /* Generating IF EXISTS and DROP command for table indexes */
                        //        StringCollection indexScripts = myIndex.Script(scriptOptions);
                        //        foreach (string script in indexScripts)
                        //        {
                        //            AppendLine(script);
                        //        }

                        //        /* Generating CREATE INDEX command for table indexes */
                        //        indexScripts = myIndex.Script();
                        //        foreach (string script in indexScripts)
                        //        {
                        //            AppendLine(script);
                        //        }
                        //    }
                        //}

                        //foreach (var index in table.Indexes)
                        //{
                        //    //AppendLine(index.ToString());
                        //    /* Generating IF EXISTS and DROP command for table indexes */
                        //    StringCollection indexScripts = index.Script(scriptOptions);
                        //    foreach (string script in indexScripts)
                        //        Console.WriteLine(script);
                        //    /* Generating CREATE INDEX command for table indexes */
                        //    indexScripts = index.Script();
                        //    foreach (string script in indexScripts)
                        //        Console.WriteLine(script);
                        //}

                        //statusToolStripStatusLabe.Text = "Complete";
                    }
                }
            }
        }

        private void BuildToolStripButton_Click(object sender, EventArgs e)
        {
            BuildScript();
        }

        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DescEditToolStripButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                using var dlg = new DescEditForm()
                {
                    TableName = objectName
                };
                dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Get description of a column
        /// </summary>
        /// <param name="schema">object schema</param>
        /// <param name="table">object name</param>
        /// <param name="column">column name</param>
        /// <returns></returns>
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
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
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
                    string colID = dr["ORDINAL_POSITION"].ToString();
                    string colName = dr["COLUMN_NAME"].ToString();
                    string dataType = dr["DATA_TYPE"].ToString();
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
        /// Generate contect for wiki of the give table
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        private void GetTableDefWiki(ObjectName objectName)
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
            //AppendLine("</br>");
            //AppendLine("----");
            //if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
            //{
            //    AppendLine("Back to [[DW: Database tables|Database tables]]");
            //}
            //else
            //{
            //    AppendLine("Back to [[DW: Database views|Database views]]");
            //}
            //AppendLine("[[Category: CSBC data warehouse]]");
        }

        /// <summary>
        /// Get table list from the database
        /// </summary>
        private void GetTableList()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_SCHEMA, TABLE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var ds = new DataSet();
                var dat = new SqlDataAdapter(cmd);
                dat.Fill(ds);
                if (ds.Tables.Count > 0)
                { _tables = ds.Tables[0]; }
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
        private void GetTableValues(string tableName)
        {
            sqlTextBox.Text = string.Empty;

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

            //statusToolStripStatusLabe.Text = "Complete";
        }

        private void ObjectsListBox_DoubleClick(object sender, EventArgs e)
        {
            BuildScript();
        }

        /// <summary>
        /// Populate the object list box
        /// </summary>
        private void Populate()
        {
            if (_tables != null)
            {
                objectsListBox.Items.Clear();
                string searchFor = searchTextBox.Text.Trim();
                if (searchFor.Length == 0)
                {
                    foreach (DataRow row in _tables.Rows)
                    {
                        AddListItem(row["TABLE_TYPE"].ToString(), row["TABLE_SCHEMA"].ToString(), row["TABLE_NAME"].ToString());
                    }
                }
                else
                {
                    var matches = _tables.Select(string.Format("TABLE_NAME LIKE '%{0}%'", searchFor));
                    foreach (DataRow row in matches)
                    {
                        AddListItem(row["TABLE_TYPE"].ToString(), row["TABLE_SCHEMA"].ToString(), row["TABLE_NAME"].ToString());
                    }
                }
            }
        }
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            Populate();
        }

        private void TableBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.HeaderText = headerTextBox.Text;
            Properties.Settings.Default.FooterText = footerTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void TableBuilderForm_Load(object sender, EventArgs e)
        {
            headerTextBox.Text = Properties.Settings.Default.HeaderText;
            footerTextBox.Text = Properties.Settings.Default.FooterText;
            GetTableList();
            Populate();
            WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Handles "Table wiki" button click event: Generate wiki content of table structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableWikiToolStripButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                if (headerTextBox.Text.Length > 0)
                {
                    sqlTextBox.Text = headerTextBox.Text + "\r\n";
                }
                else
                {
                    sqlTextBox.Text = String.Empty;
                }
                GetTableDefWiki(objectName);
                AppendLine(footerTextBox.Text);
            }
        }

        /// <summary>
        /// Handles "Value wiki" button click event: generate wiki content of a table value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValuesWikiToolStripButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                sqlTextBox.Text = string.Empty;
                GetTableValues(objectName.FullName);
            }
        }
    }
}