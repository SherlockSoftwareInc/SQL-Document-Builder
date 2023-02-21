using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class TableBuilderForm : Form
    {
        private DataTable _tables = new();

        public TableBuilderForm()
        {
            InitializeComponent();
        }

        public SQLDatabaseConnectionItem? Connection { get; set; }

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
                    ScriptingOptions scriptOptions = new()
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
                    Server server = new(Connection.ServerName);
                    Database database = server.Databases[Connection.Database];
                    Table table = database.Tables[objectName.Name, objectName.Schema];
                    //Table table = database.Tables["pt_case"];
                    if (table != null)
                    {
                        StringCollection result = table.Script(scriptOptions);
                        foreach (var line in result)
                        {
                            if (line != null)
                                AppendLine(line);
                        }
                        AppendLine("GO" + Environment.NewLine);

                        //IndexCollection indexCol = table.Indexes;
                        foreach (Microsoft.SqlServer.Management.Smo.Index myIndex in table.Indexes)
                        {
                            /* Generating IF EXISTS and DROP command for table indexes */
                            StringCollection indexScripts = myIndex.Script(scriptOptions);
                            foreach (string? script in indexScripts)
                            {
                                if (script != null)
                                    AppendLine(script);
                            }

                            /* Generating CREATE INDEX command for table indexes */
                            indexScripts = myIndex.Script();
                            foreach (string? script in indexScripts)
                            {
                                if (script != null)
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

        ///// <summary>
        ///// Get description of a column
        ///// </summary>
        ///// <param name="schema">object schema</param>
        ///// <param name="table">object name</param>
        ///// <param name="column">column name</param>
        ///// <returns></returns>
        //private string GetColumnDesc(string schema, string table, string column)
        //{
        //    string result = string.Empty;
        //    string sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", schema, table, column);
        //    var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
        //    try
        //    {
        //        var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
        //        conn.Open();
        //        var dr = cmd.ExecuteReader();
        //        if (dr.Read())
        //        {
        //            result = dr.GetString(0);  //dr[0].ToString();
        //        }

        //        dr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.MsgBox(ex.Message, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    return result;
        //}

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

        private void ObjectsListBox_DoubleClick(object sender, EventArgs e)
        {
            BuildScript();
        }

        private void PopulateSchema()
        {
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");

            var dtSchemas = _tables.DefaultView.ToTable(true, "TABLE_SCHEMA");
            var schemas = new List<string>();
            foreach (DataRow dr in dtSchemas.Rows)
            {
                schemas.Add(dr[0].ToString());
            }
            schemas.Sort();
            foreach (var item in schemas)
            {
                schemaComboBox.Items.Add(item);
            }
        }

        /// <summary>
        /// Populate the object list box
        /// </summary>
        private void Populate()
        {
            string schemaName = string.Empty;
            if (schemaComboBox.SelectedIndex > 0) schemaName = schemaComboBox.Items[schemaComboBox.SelectedIndex].ToString();

            if (_tables != null)
            {
                objectsListBox.Items.Clear();
                string searchFor = searchTextBox.Text.Trim();
                if (searchFor?.Length == 0)
                {
                    if (schemaName?.Length == 0)
                    {
                        foreach (DataRow row in _tables.Rows)
                        {
                            AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
                        }
                    }
                    else
                    {
                        foreach (DataRow row in _tables.Rows)
                        {
                            if (schemaName.Equals(row["TABLE_SCHEMA"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                                AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
                        }
                    }
                }
                else
                {
                    var matches = _tables.Select(string.Format("TABLE_NAME LIKE '%{0}%'", searchFor));
                    if (schemaName?.Length == 0)
                    {
                        foreach (DataRow row in matches)
                        {
                            AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
                        }
                    }
                    else
                    {
                        foreach (DataRow row in matches)
                        {
                            if (schemaName.Equals(row["TABLE_SCHEMA"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                                AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
                        }
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
            PopulateSchema();
            if (schemaComboBox.Items.Count > 0) schemaComboBox.SelectedIndex = 0;
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
                var builder = new Wiki();
                sqlTextBox.AppendText(builder.GetTableDef(objectName));
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
                var builder = new Wiki();
                sqlTextBox.Text = builder.GetTableValues(objectName.FullName);
            }
        }

        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Populate();
        }

        private void TableDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
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
                var builder = new SharePoint();
                sqlTextBox.AppendText(builder.GetTableDef(objectName));
                AppendLine(footerTextBox.Text);
            }
        }

        private void ValueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var builder = new SharePoint();
                sqlTextBox.Text = builder.GetTableValues(objectName.FullName);
            }
        }
    }
}