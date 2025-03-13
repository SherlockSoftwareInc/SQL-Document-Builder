using DarkModeForms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The table builder form.
    /// </summary>
    public partial class TableBuilderForm : Form
    {
        /// <summary>
        /// The database connections.
        /// </summary>
        private readonly SQLServerConnections _connections = new();

        /// <summary>
        /// The count of database connections.
        /// </summary>
        private int _connectionCount = 0;

        private string _fileName = string.Empty;

        /// <summary>
        /// The selected connection.
        /// </summary>
        private SQLDatabaseConnectionItem? _selectedConnection = new();

        /// <summary>
        /// The tables.
        /// </summary>
        private DataTable _tables = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBuilderForm"/> class.
        /// </summary>
        public TableBuilderForm()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
        }

        /// <summary>
        /// Gets or Sets the connection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SQLDatabaseConnectionItem? Connection { get; set; }

        /// <summary>
        /// Abouts the tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show about box
            using var dlg = new AboutBox();
            dlg.ShowDialog();
        }

        /// <summary>
        /// Add connection.
        /// </summary>
        /// <returns>A bool.</returns>
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

        /// <summary>
        /// Add list item.
        /// </summary>
        /// <param name="tableType">The table type.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="tableName">The table name.</param>
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
        /// Assistants the content tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void AssistantContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;

            try
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                MSSchemaContentBuilder builder = new();

                string contents = String.Empty;
                await Task.Run(() =>
                {
                    contents = builder.SchemaContent(Properties.Settings.Default.dbConnectionString, progress);
                });

                SetScript(contents);

                EndBuild();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Batches the tool strip button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void BatchToolStripButton_Click(object sender, EventArgs e)
        {
            using var frm = new BatchColumnDesc();
            frm.ShowDialog();
        }

        /// <summary>
        /// Change DB connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
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

                    string? connectionString = connection?.ConnectionString?.Length == 0 ? connection.Login() : connection?.ConnectionString;

                    if (connectionString?.Length > 0)
                    {
                        serverToolStripStatusLabel.Text = connection?.ServerName;
                        databaseToolStripStatusLabel.Text = connection?.Database;
                        Properties.Settings.Default.dbConnectionString = connectionString;
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

                    GetTableList();
                    PopulateSchema();
                    if (schemaComboBox.Items.Count > 0) schemaComboBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Clipboard the to table tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ClipboardToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = String.Empty;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new SharePoint();
                    SetScript(builder.TextToTable(metaData));
                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Close tool strip button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Copy tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)ActiveControl;
                textBox.Copy();
            }
            else if (ActiveControl?.GetType() == typeof(DBObjectDefPanel))
            {
                DBObjectDefPanel dBObjectDefPanel = (DBObjectDefPanel)ActiveControl;
                dBObjectDefPanel.Copy();
            }
        }

        /// <summary>
        /// Handles the "create index" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CreateIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScript(definitionPanel.CreateIndexScript());
        }

        /// <summary>
        /// handles the "create primary key" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CreatePrimaryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScript(definitionPanel.PrimaryKeyScript());
            if (!string.IsNullOrEmpty(sqlTextBox.Text))
            {
                Clipboard.SetText(sqlTextBox.Text);
            }
        }

        /// <summary>
        /// Cuts the tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)ActiveControl;
                textBox.Cut();
            }
            else if (ActiveControl?.GetType() == typeof(DBObjectDefPanel))
            {
                DBObjectDefPanel dBObjectDefPanel = (DBObjectDefPanel)ActiveControl;
                dBObjectDefPanel.Cut();
            }
        }

        /// <summary>
        /// Descs the edit tool strip button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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

        /// <summary>
        /// End build.
        /// </summary>
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

        /// <summary>
        /// Footers the text.
        /// </summary>
        /// <returns>A string.</returns>
        private string FooterText()
        {
            string footerText = string.Empty;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var objectType = objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table";

                footerText = objectName.Schema.ToLower() switch
                {
                    "dbo" => $@"<hr/>
<div>Back to [[Data warehouse {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                    "af" => $@"<hr/>
<div>Back to [[AF Database {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                    "bccr" => $@"<hr/>
<div>Back to [[BCCR Database {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                    "dih" => $@"<hr/>
<div>Back to [[APPROACH (HeartIS) database {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                    "joint" => $@"<hr/>
<div>Back to [[JOINT database {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                    "pcr" or "pcrl1" => $@"<hr/>
<div>Back to [[PCR database {objectType}s (CVI.Source)]]</div>
<div>Back to [[Home]]</div>",
                    "wlv" => $@"<hr/>
<div>Back to [[WLV database {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                    _ => $@"<hr/>
<div>Back to [[{objectName.Schema.ToUpper()} schema {objectType}s]]</div>
<div>Back to [[Home]]</div>",
                };
            }

            return footerText;
        }

        /// <summary>
        /// Function the list tool strip menu item1 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void FunctionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePoint();
                SetScript(builder.BuildFunctionList(dlg.Schema));
                EndBuild();
            }
        }

        /// <summary>
        /// Gets the table list.
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
        /// Headers the text.
        /// </summary>
        /// <returns>A string.</returns>
        private string HeaderText()
        {
            string result = string.Empty;
            //if (headerTextBox.Text.Length > 0)
            //{
            //    if (objectsListBox.SelectedItem != null)
            //    {
            //        var objectName = (ObjectName)objectsListBox.SelectedItem;
            //        var objectType = objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "View" : "Table";
            //        result = headerTextBox.Text.Replace("$Table", objectType);
            //    }
            //}
            return result;
        }

        /// <summary>
        /// Object descriptions tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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
                    SetScript(scripts);
                }

                EndBuild();
            }
        }

        /// <summary>
        /// Objects the list box double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ObjectsListBox_DoubleClick(object sender, EventArgs e)
        {
            //BuildScript();
        }

        /// <summary>
        /// Objects the list box selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                definitionPanel?.Open(objectName);
            }
            else
            {
                definitionPanel.Open(null);
            }
        }

        /// <summary>
        /// On connection tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void OnConnectionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender?.GetType() == typeof(ConnectionMenuItem))
            {
                ConnectionMenuItem menuItem = (ConnectionMenuItem)sender;

                statusToolStripStatusLabe.Text = string.Format("Connect to {0}...", menuItem.ToString());
                Cursor = Cursors.WaitCursor;

                ChangeDBConnection(menuItem.Connection);

                Cursor = Cursors.Default;
                statusToolStripStatusLabe.Text = "";
            }
        }

        /// <summary>
        /// Paste tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)ActiveControl;
                textBox.Paste();
            }
            else if (ActiveControl?.GetType() == typeof(DBObjectDefPanel))
            {
                DBObjectDefPanel dBObjectDefPanel = (DBObjectDefPanel)ActiveControl;
                dBObjectDefPanel.Paste();
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void Populate()
        {
            definitionPanel?.Open(null);
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
        /// Populates the connections.
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

        /// <summary>
        /// Populates the schema.
        /// </summary>
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

        /// <summary>
        /// Queries the data to table tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void QueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new QueryDataToTableForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Handles "Query to INSERT" strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void QueryInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new QueryDataToTableForm()
            {
                InsertStatement = true
            };
            form.ShowDialog();
        }

        /// <summary>
        /// Saves the as tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql|Text file(*.txt)|*.txt|All files(*.*)|*.*" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                this.Text = $"SharePoint Script Builder - {_fileName}";
            }

            var file = new System.IO.StreamWriter(_fileName, false);
            await file.WriteAsync(sqlTextBox.Text);
            file.Close();
            statusToolStripStatusLabe.Text = "Complete";
        }

        /// <summary>
        /// Save tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql|Text file(*.txt)|*.txt|All files(*.*)|*.*" };
                if (oFile.ShowDialog() == DialogResult.OK)
                {
                    _fileName = oFile.FileName;
                    this.Text = $"SharePoint Script Builder - {_fileName}";

                    saveAsToolStripMenuItem.PerformClick();
                }
            }
            else
            {
                SetScript(sqlTextBox.Text);
            }
            statusToolStripStatusLabe.Text = "Complete";
        }

        /// <summary>
        /// Schemata the combo box selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Populate();
        }

        /// <summary>
        /// Search text box text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            Populate();
        }

        /// <summary>
        /// Selects the all tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)ActiveControl;
                textBox.SelectAll();
            }
            else if (ActiveControl?.GetType() == typeof(DBObjectDefPanel))
            {
                DBObjectDefPanel dBObjectDefPanel = (DBObjectDefPanel)ActiveControl;
                dBObjectDefPanel.SelectAll();
            }
        }

        /// <summary>
        /// Saves the script.
        /// </summary>
        /// <param name="sql">The sql.</param>
        private void SetScript(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return;
            }

            sqlTextBox.Text = sql;
            Clipboard.SetText(sql);

            if (string.IsNullOrEmpty(_fileName))
            {
                return;
            }

            // append the script to the file
            try
            {
                File.AppendAllText(_fileName, Environment.NewLine + sql);

                //var file = new System.IO.StreamWriter(_fileName, true);
                //file.WriteLine(sql);
                //file.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Start build.
        /// </summary>
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
        }

        /// <summary>
        /// Storeds the procedure list tool strip menu item1 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void StoredProcedureListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePoint();
                SetScript(builder.BuildSPList(dlg.Schema));
                EndBuild();
            }
        }

        /// <summary>
        /// Tables the builder form form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Properties.Settings.Default.SchemaSettings = _settingItems.Settings;
            //Properties.Settings.Default.Templetes = templates.TemplatesValue;
            //Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Tables the builder form load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableBuilderForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            _connections.Load();
            PopulateConnections();

            var lastConnection = Properties.Settings.Default.LastAccessConnectionIndex;
            ConnectionMenuItem? selectedItem;
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

            WindowState = FormWindowState.Maximized;
            if (collapsibleSplitter1 != null)
                collapsibleSplitter1.SplitterDistance = (int)(this.Width * 0.25F);
        }

        /// <summary>
        /// Tables the definition tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var header = HeaderText();
                if (header.Length > 0)
                {
                    SetScript(header + Environment.NewLine);
                }
                else
                {
                    sqlTextBox.Text = String.Empty;
                }
                var builder = new SharePoint();
                sqlTextBox.AppendText(builder.GetTableDef(objectName));

                if ((objectName.Name.StartsWith("LT_")) || (objectName.Name.StartsWith("AT_")))
                {
                    var valueBuilder = new SharePoint();
                    sqlTextBox.AppendText(valueBuilder.GetTableValues(objectName.FullName));
                }

                sqlTextBox.AppendText(FooterText() + Environment.NewLine);
                EndBuild();
            }
        }

        /// <summary>
        /// Handles the "Table description" tool strip menu item click:
        /// generate the table description and column descriptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TableDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objectName = (ObjectName)objectsListBox.SelectedItem;
            if (!string.IsNullOrEmpty(objectName?.Name))
            {
                SetScript(ObjectDescription.BuildObjectDescription(objectName));
                if (!string.IsNullOrEmpty(sqlTextBox.Text))
                {
                    Clipboard.SetText(sqlTextBox.Text);
                }
            }
        }

        /// <summary>
        /// Table the list tool strip menu item1 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Tables the wiki tool strip button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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

        /// <summary>
        /// Value the list tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ValueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var builder = new SharePoint();
                SetScript(builder.GetTableValues(objectName.FullName));
                EndBuild();
            }
        }

        /// <summary>
        /// Values the wiki tool strip button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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
        /// View the list tool strip menu item1 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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

                SetScript(scripts);

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
        /// Views the descriptions tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
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

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the Click event of the OpenToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var oFile = new OpenFileDialog() { Filter = "SQL script(*.sql)|*.sql|Text file(*.txt)|*.txt|All files(*.*)|*.*" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                this.Text = $"SharePoint Script Builder - {_fileName}";

                sqlTextBox.Text = File.ReadAllText(_fileName);
            }
        }

        /// <summary>
        /// Handles the Click event of the NewToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql|Text file(*.txt)|*.txt|All files(*.*)|*.*" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                this.Text = $"SharePoint Script Builder - {_fileName}";
                sqlTextBox.Text = string.Empty;
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