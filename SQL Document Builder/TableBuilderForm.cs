using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class TableBuilderForm : Form
    {
        private readonly SQLServerConnections _connections = new();
        private readonly SettingItems _settingItems = new();
        private int _connectionCount = 0;
        private SQLDatabaseConnectionItem? _selectedConnection = new SQLDatabaseConnectionItem();
        private DataTable _tables = new();

        public TableBuilderForm()
        {
            InitializeComponent();
        }

        public SQLDatabaseConnectionItem? Connection { get; set; }

        /// <summary>
        /// Open add connection dialog and start to add a new database connection
        /// </summary>
        /// <returns></returns>
        private bool AddConnection()
        {
            using var dlg = new NewSQLServerConnectionDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var connection = new SQLDatabaseConnectionItem()
                {
                    Name = dlg.ConnectionName,
                    ServerName = dlg.ServerName,
                    Database = dlg.DatabaseName,
                    AuthenticationType = dlg.Authentication,
                    UserName = dlg.UserName,
                    Password = dlg.Password,
                    RememberPassword = dlg.RememberPassword
                };

                connection.BuildConnectionString();
                _connections.Add(dlg.ConnectionName, dlg.ServerName, dlg.DatabaseName,
                    dlg.Authentication, dlg.UserName, dlg.Password,
                    connection.ConnectionString, dlg.RememberPassword);

                _connections.Save();

                var submenuitem = new ConnectionMenuItem(connection)
                {
                    Name = string.Format("ConnectionMenuItem{0}", _connectionCount++),
                    Size = new Size(300, 26),
                };
                submenuitem.Click += new System.EventHandler(this.OnConnectionToolStripMenuItem_Click);
                connectToToolStripMenuItem.DropDown.Items.Add(submenuitem);

                return true;
            }
            return false;
        }

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

        /// <summary>
        /// Show the batch description form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchToolStripButton_Click(object sender, EventArgs e)
        {
            using var frm = new BatchColumnDesc();
            frm.ShowDialog();
        }

        private void BuildToolStripButton_Click(object sender, EventArgs e)
        {
            //BuildScript();
        }

        /// <summary>
        /// Change current database connection to a new connection
        /// </summary>
        /// <param name="connection">New db connection</param>
        private void ChangeDBConnection(SQLDatabaseConnectionItem connection)
        {
            if (connection != null)
            {
                bool connectionChanged = false;
                if (_selectedConnection == null)
                {
                    connectionChanged = true;
                }
                else
                {
                    if (!_selectedConnection.Equals(connection))
                    {
                        connectionChanged = true;
                    }
                }

                if (connectionChanged)
                {
                    _selectedConnection = connection;
                    serverToolStripStatusLabel.Text = "";
                    databaseToolStripStatusLabel.Text = "";
                    //_server = connection.ServerName;
                    //_database = connection.Database;

                    string? connectionString = connection?.ConnectionString?.Length == 0 ? connection.Login() : connection?.ConnectionString;

                    //string errMessage;
                    if (connectionString?.Length > 0)
                    {
                        //errMessage = dbObjects.Open(connection);

                        //if (errMessage.Length > 0)
                        //{
                        //    connection.ConnectionString = "";
                        //    connection.Password = "";
                        //}
                        //else
                        //{
                        serverToolStripStatusLabel.Text = connection?.ServerName;
                        databaseToolStripStatusLabel.Text = connection?.Database;
                        Properties.Settings.Default.dbConnectionString = connectionString;
                        //}
                    }

                    for (int i = 0; i < connectToToolStripMenuItem.DropDown.Items.Count; i++)
                    {
                        var submenuitem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[i];
                        if (submenuitem.Connection.Equals(connection))
                        {
                            submenuitem.Checked = true;
                        }
                        else
                        {
                            submenuitem.Checked = false;
                        }
                    }

                    for (int i = 0; i < _connections.Connections.Count; i++)
                    {
                        if (_connections.Connections[i].Equals(connection))
                        {
                            Properties.Settings.Default.LastAccessConnectionIndex = i;
                            break;
                        }
                    }

                    //if (errMessage.Length > 0)
                    //{
                    //    MessageBox.Show(errMessage, Properties.Resources.A005, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}

                    GetTableList();
                    PopulateSchema();
                    if (schemaComboBox.Items.Count > 0) schemaComboBox.SelectedIndex = 0;
                }
            }
        }

        private void ClipboardToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = String.Empty;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new SharePoint();
                    sqlTextBox.Text = builder.TextToTable(metaData);
                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles "Copy" menu item click event: Copy the selected text in the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Copy();
        }

        /// <summary>
        /// Handles "Cut" menu item click event: Cut the selected text in the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Cut();
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
            else
            {
                statusToolStripStatusLabe.Text = "No object selected";
            }
        }

        private void EndBuild()
        {
            sqlTextBox.Enabled = true;
            sqlTextBox.Cursor = Cursors.Default;
            progressBar.Visible = false;
            statusToolStripStatusLabe.Text = "Complete!";
            this.Cursor = Cursors.Default;
            if (sqlTextBox.Text.Length > 0)
            {
                Clipboard.SetText(sqlTextBox.Text);
            }
        }

        private void FunctionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePoint();
                sqlTextBox.Text = builder.BuildFunctionList(dlg.Schema);
                EndBuild();
            }
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
        /// Generate description scripts for tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ObjectDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = String.Empty;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                await Task.Run(() =>
                {
                    scripts = ObjectDescription.BuildObjectDescriptions(ObjectName.ObjectTypeEnums.Table, dlg.Schema, progress);
                });

                if (scripts != null)
                {
                    sqlTextBox.Text = scripts;
                }

                EndBuild();
            }
        }

        private void ObjectsListBox_DoubleClick(object sender, EventArgs e)
        {
            //BuildScript();
        }

        /// <summary>
        /// Handle connection menu item click event:
        ///     Open selected connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender?.GetType() == typeof(ConnectionMenuItem))
            {
                ConnectionMenuItem menuItem = (ConnectionMenuItem)sender;

                statusToolStripStatusLabe.Text = string.Format("Connect to {0}...", menuItem.ToString());
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                ChangeDBConnection(menuItem.Connection);

                Cursor = Cursors.Default;
                statusToolStripStatusLabe.Text = "";
            }
        }

        /// <summary>
        /// Handles "Paste" menu item click event: Paste the text from the clipboard to the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Paste();
        }

        /// <summary>
        /// Populate the object list box
        /// </summary>
        private void Populate()
        {
            definitionPanel.Open(null);
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
                if (objectsListBox.Items.Count > 0)
                {
                    objectsListBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Populate connections to menu and combobox
        /// </summary>
        private void PopulateConnections()
        {
            _selectedConnection = null;

            // clear menu items under Connect to... menu
            for (int i = connectToToolStripMenuItem.DropDown.Items.Count - 1; i >= 0; i--)
            {
                var submenuitem = connectToToolStripMenuItem.DropDown.Items[i];
                submenuitem.Click -= OnConnectionToolStripMenuItem_Click;
                connectToToolStripMenuItem.DropDownItems.RemoveAt(i);
            }

            var connections = _connections.Connections;
            if (connections.Count == 0)
            {
                AddConnection();
            }
            else
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    var item = connections[i];
                    //if(item.DBMSType == 1)

                    if (item.ConnectionString?.Length > 1)
                    {
                        var submenuitem = new ConnectionMenuItem(item)
                        {
                            Name = string.Format("ConnectionMenuItem{0}", i + 1),
                            Size = new Size(300, 26),
                        };
                        submenuitem.Click += OnConnectionToolStripMenuItem_Click;
                        connectToToolStripMenuItem.DropDown.Items.Add(submenuitem);
                    }
                }
            }
        }

        private void PopulateSchema()
        {
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");

            var dtSchemas = _tables.DefaultView.ToTable(true, "TABLE_SCHEMA");
            var schemas = new List<string>();
            foreach (DataRow dr in dtSchemas.Rows)
            {
                schemas.Add((string)dr[0]);
            }
            schemas.Sort();
            foreach (var item in schemas)
            {
                schemaComboBox.Items.Add(item);
            }
        }

        private void QueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new QueryDataToTableForm();
            form.ShowDialog();
        }

        private async void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "Text file(*.txt)|*.txt|SQL script(*.sql)|*.sql|All files(*.*)|*.*" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                var file = new System.IO.StreamWriter(oFile.FileName, false);
                await file.WriteAsync(sqlTextBox.Text);
                file.Close();
                statusToolStripStatusLabe.Text = "Complete";
            }
        }

        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Populate();
            var schemaSetting = _settingItems.GetSchemaSetting(schemaComboBox.Text);
            if (schemaSetting != null)
            {
                headerTextBox.Text = schemaSetting.HeaderText;
                footerTextBox.Text = schemaSetting.FooterText;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            Populate();
        }

        /// <summary>
        /// Handles "Select all" menu item click event: Select all content in the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.SelectionStart = 0;
            sqlTextBox.SelectionLength = sqlTextBox.TextLength;
        }

        private void StartBuild()
        {
            this.Cursor = Cursors.WaitCursor;
            sqlTextBox.Text = String.Empty;
            sqlTextBox.Enabled = false;
            sqlTextBox.Cursor = Cursors.WaitCursor;
            statusToolStripStatusLabe.Text = "Please wait while generate the scripts";
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = true;
            Application.DoEvents();
        }

        private void StoredProcedureListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePoint();
                sqlTextBox.Text = builder.BuildSPList(dlg.Schema);
                EndBuild();
            }
        }

        private void TableBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.SchemaSettings = _settingItems.Settings;
            Properties.Settings.Default.HeaderText = headerTextBox.Text;
            Properties.Settings.Default.FooterText = footerTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void TableBuilderForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            _connections.Load();
            PopulateConnections();

            _settingItems.Settings = Properties.Settings.Default.SchemaSettings;

            var lastConnection = Properties.Settings.Default.LastAccessConnectionIndex;
            ConnectionMenuItem? selectedItem = null;
            if (lastConnection <= 0 || lastConnection >= _connections.Connections.Count)
            {
                selectedItem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[0];
            }
            else
            {
                selectedItem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[lastConnection];
            }
            if (selectedItem != null)
            {
                ChangeDBConnection(selectedItem.Connection);
            }
            else
            {
                Close();
            }

            //headerTextBox.Text = Properties.Settings.Default.HeaderText;
            //footerTextBox.Text = Properties.Settings.Default.FooterText;
            //GetTableList();
            //PopulateSchema();
            //if (schemaComboBox.Items.Count > 0) schemaComboBox.SelectedIndex = 0;
            WindowState = FormWindowState.Maximized;
            collapsibleSplitter1.SplitterDistance = (int)(this.Width * 0.25F);
        }

        private void TableDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var header = HeaderText();
                if (header.Length > 0)
                {
                    sqlTextBox.Text = header + Environment.NewLine;
                }
                else
                {
                    sqlTextBox.Text = String.Empty;
                }
                var builder = new SharePoint();
                sqlTextBox.AppendText(builder.GetTableDef(objectName));
                AppendLine(FooterText());
                EndBuild();
            }
        }

        private string HeaderText()
        {
            string result = string.Empty;
            if (headerTextBox.Text.Length > 0)
            {
                if (objectsListBox.SelectedItem != null)
                {
                    var objectName = (ObjectName)objectsListBox.SelectedItem;
                    var objectType = objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "View" : "Table";
                    result = headerTextBox.Text.Replace("$Table", objectType);
                }
            }
            return result;
        }

        private string FooterText()
        {
            string result = string.Empty;
            if (footerTextBox.Text.Length > 0)
            {
                if (objectsListBox.SelectedItem != null)
                {
                    var objectName = (ObjectName)objectsListBox.SelectedItem;
                    var objectType = objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "View" : "Table";
                    result = footerTextBox.Text.Replace("$Table", objectType);
                }
            }
            return result;
        }

        /// <summary>
        /// Handles build table list menu/toolbar button click event
        ///     Build the SharePoint script for tables in the selected schema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TableListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new SharePoint();
                await Task.Run(() =>
                {
                    scripts = builder.BuildTableList(dlg.Schema, progress);
                });

                sqlTextBox.Text = scripts;

                EndBuild();
            }
        }

        /// <summary>
        /// Handles "Table wiki" button click event: Generate wiki content of table structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableWikiToolStripButton_Click(object sender, EventArgs e)
        {
            //if (objectsListBox.SelectedItem != null)
            //{
            //    var objectName = (ObjectName)objectsListBox.SelectedItem;
            //    if (headerTextBox.Text.Length > 0)
            //    {
            //        sqlTextBox.Text = headerTextBox.Text + "\r\n";
            //    }
            //    else
            //    {
            //        sqlTextBox.Text = String.Empty;
            //    }
            //    var builder = new Wiki();
            //    sqlTextBox.AppendText(builder.GetTableDef(objectName));
            //    AppendLine(footerTextBox.Text);
            //    Clipboard.SetText(sqlTextBox.Text);
            //}
        }

        private void ValueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var builder = new SharePoint();
                sqlTextBox.Text = builder.GetTableValues(objectName.FullName);
                EndBuild();
            }
        }

        /// <summary>
        /// Handles "Value wiki" button click event: generate wiki content of a table value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValuesWikiToolStripButton_Click(object sender, EventArgs e)
        {
            //if (objectsListBox.SelectedItem != null)
            //{
            //    var objectName = (ObjectName)objectsListBox.SelectedItem;
            //    var builder = new Wiki();
            //    sqlTextBox.Text = builder.GetTableValues(objectName.FullName);
            //    Clipboard.SetText(sqlTextBox.Text);
            //}
        }

        /// <summary>
        /// Handles build view list menu/toolbar button click event
        ///     Build the SharePoint script for views in the selected schema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new SharePoint();
                await Task.Run(() =>
                {
                    scripts = builder.BuildViewList(dlg.Schema, progress);
                });

                sqlTextBox.Text = scripts;

                EndBuild();
            }
            //sqlTextBox.Text = string.Empty;
            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    var builder = new SharePoint();
            //    sqlTextBox.Text = builder.BuildViewList(dlg.Schema);
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Generate description scripts for views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewsDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = String.Empty;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                await Task.Run(() =>
                {
                    scripts = ObjectDescription.BuildObjectDescriptions(ObjectName.ObjectTypeEnums.View, dlg.Schema, progress);
                });

                sqlTextBox.Text = scripts;

                EndBuild();
            }
        }

        private void ObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                definitionPanel.Open(objectName);
            }
            else
            {
                definitionPanel.Open(null);
            }
        }

        private void SettingTextBox_Validated(object sender, EventArgs e)
        {
            var schemaSetting = _settingItems.GetSchemaSetting(schemaComboBox.Text);
            if (schemaSetting != null)
            {
                schemaSetting.HeaderText = headerTextBox.Text;
                schemaSetting.FooterText = footerTextBox.Text;
            }
            else
            {
                _settingItems.Add(schemaComboBox.Text, headerTextBox.Text, footerTextBox.Text);
            }
        }
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

//private void BuildScript()
//{
//    if (objectsListBox.SelectedItem != null)
//    {
//        if (Connection != null)
//        {
//            var objectName = (ObjectName)objectsListBox.SelectedItem;
//            sqlTextBox.Text = string.Empty;

//            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.management.smo.scriptingoptions?view=sql-smo-160
//            ScriptingOptions scriptOptions = new()
//            {
//                ContinueScriptingOnError = true,
//                ScriptDrops = scriptDropsCheckBox.Checked,
//                ScriptForCreateDrop = scriptForCreateDropCheckBox.Checked,
//                IncludeIfNotExists = includeIfNotExistsCheckBox.Checked,
//                ExtendedProperties = extendedPropertiesCheckBox.Checked,
//                AnsiPadding = ansiPaddingCheckBox.Checked,
//                NoCollation = noCollationCheckBox.Checked,
//                ScriptData = scriptDataCheckBox.Checked,
//                IncludeHeaders = includeHeadersCheckBox.Checked
//            };

//            //GetTableDefWiki(RemoveQuota(tableElement[0]), RemoveQuota(tableElement[1]));
//            Server server = new(Connection.ServerName);
//            Database database = server.Databases[Connection.Database];
//            Table table = database.Tables[objectName.Name, objectName.Schema];
//            //Table table = database.Tables["pt_case"];
//            if (table != null)
//            {
//                StringCollection result = table.Script(scriptOptions);
//                foreach (var line in result)
//                {
//                    if (line != null)
//                        AppendLine(line);
//                }
//                AppendLine("GO" + Environment.NewLine);

//                //IndexCollection indexCol = table.Indexes;
//                foreach (Microsoft.SqlServer.Management.Smo.Index myIndex in table.Indexes)
//                {
//                    /* Generating IF EXISTS and DROP command for table indexes */
//                    StringCollection indexScripts = myIndex.Script(scriptOptions);
//                    foreach (string? script in indexScripts)
//                    {
//                        if (script != null)
//                            AppendLine(script);
//                    }

//                    /* Generating CREATE INDEX command for table indexes */
//                    indexScripts = myIndex.Script();
//                    foreach (string? script in indexScripts)
//                    {
//                        if (script != null)
//                            AppendLine(script);
//                    }
//                }

//                //Scripter scripter = new Scripter(server);
//                ///* With ScriptingOptions you can specify different scripting
//                //* options, for example to include IF NOT EXISTS, DROP
//                //* statements, output location etc*/

//                //foreach (Table myTable in database.Tables)
//                //{
//                //    /* Generating IF EXISTS and DROP command for tables */
//                //    StringCollection tableScripts = myTable.Script(scriptOptions);
//                //    foreach (string script in tableScripts)
//                //    {
//                //        AppendLine(script);
//                //    }

//                //    tableScripts = myTable.Script();
//                //    foreach (string script in tableScripts)
//                //    {
//                //        AppendLine(script);
//                //    }

//                //    IndexCollection indexCol = myTable.Indexes;
//                //    foreach (Microsoft.SqlServer.Management.Smo.Index myIndex in myTable.Indexes)
//                //    {
//                //        /* Generating IF EXISTS and DROP command for table indexes */
//                //        StringCollection indexScripts = myIndex.Script(scriptOptions);
//                //        foreach (string script in indexScripts)
//                //        {
//                //            AppendLine(script);
//                //        }

//                //        /* Generating CREATE INDEX command for table indexes */
//                //        indexScripts = myIndex.Script();
//                //        foreach (string script in indexScripts)
//                //        {
//                //            AppendLine(script);
//                //        }
//                //    }
//                //}

//                //foreach (var index in table.Indexes)
//                //{
//                //    //AppendLine(index.ToString());
//                //    /* Generating IF EXISTS and DROP command for table indexes */
//                //    StringCollection indexScripts = index.Script(scriptOptions);
//                //    foreach (string script in indexScripts)
//                //        Console.WriteLine(script);
//                //    /* Generating CREATE INDEX command for table indexes */
//                //    indexScripts = index.Script();
//                //    foreach (string script in indexScripts)
//                //        Console.WriteLine(script);
//                //}

//                //statusToolStripStatusLabe.Text = "Complete";
//            }
//        }
//    }
//}