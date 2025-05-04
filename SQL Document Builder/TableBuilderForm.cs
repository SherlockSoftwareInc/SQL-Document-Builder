using DarkModeForms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
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

        private bool _changed = false;

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
            _changed = false;

            statusToolStripStatusLabe.Text = "Complete";
        }

        /// <summary>
        /// Save tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_changed) return;

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
                AppendSave();
            }
            statusToolStripStatusLabe.Text = "Script appended to the file";
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
            // reset the timer and start it
            if (timer1 != null)
            {
                timer1.Stop();
                timer1.Interval = 500;
                timer1.Start();
            }
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
            //lastConnection = 1;
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

            extendedPropertiesCheckBox.Checked = Properties.Settings.Default.UseExtendedProperties;

            WindowState = FormWindowState.Maximized;
            splitContainer1.SplitterDistance = 200;
            if (collapsibleSplitter1 != null)
                collapsibleSplitter1.SplitterDistance = (int)(this.Width * 0.3F);

        }

        /// <summary>
        /// Tables the definition tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void TableDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
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
                    sqlTextBox.AppendText(await valueBuilder.GetTableValuesAsync(objectName.FullName));
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
            var objectName = objectsListBox.SelectedItem as ObjectName;
            if (!string.IsNullOrEmpty(objectName?.Name))
            {
                SetScript(ObjectDescription.BuildObjectDescription(objectName, Properties.Settings.Default.UseExtendedProperties));
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
        private async void ValueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var builder = new SharePoint();
                SetScript(await builder.GetTableValuesAsync(objectName.FullName));
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
                _changed = false;
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
                _changed = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the execute on DEV button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnDEVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteScripts("csbc-reporting-prod-sqldb-prod");
        }

        /// <summary>
        /// Handles the Click event of the execute on PROD button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnPRODToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteScripts("csbc-reporting-prod-sqldb-dev");
        }

        /// <summary>
        /// Executes the scripts.
        /// </summary>
        private void ExecuteScripts(string initialCatalog)
        {
            string script = sqlTextBox.SelectedText;
            if (script.Length == 0)
            {
                script = sqlTextBox.Text;
            }

            if (script.Length == 0)
            {
                Common.MsgBox("No SQL statements to execute", MessageBoxIcon.Information);
                return;
            }

            // checks if there is a DROP statement in the script, ask for confirmation
            if (script.Contains("DROP ", StringComparison.CurrentCultureIgnoreCase))
            {
                if (Common.MsgBox("The script contains a DROP statement. Are you sure you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            var builder = new SqlConnectionStringBuilder
            {
                Encrypt = true,
                TrustServerCertificate = true,
                DataSource = "csbc-reporting-prod-sql.database.windows.net",
                InitialCatalog = initialCatalog,
                Authentication = SqlAuthenticationMethod.ActiveDirectoryIntegrated,
                UserID = $"{Environment.UserName}@phsa.ca"
            };

            // from the sqlTextBox.Text, break it into individual SQL statements by the GO keyword
            //var sqlStatements = sqlTextBox.Text.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);
            var sqlStatements = Regex.Split(script, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // execute each statement
            foreach (var sql in sqlStatements)
            {
                Execute(builder.ConnectionString, sql);
            }
        }

        /// <summary>
        /// Executes the.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sql">The sql.</param>
        private void Execute(string connectionString, string sql)
        {
            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                statusToolStripStatusLabe.Text = "Complete!";
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
        /// Saves the replace tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SaveReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save the text in the sqlTextBox to the file
            if (string.IsNullOrEmpty(_fileName))
            {
                saveAsToolStripMenuItem.PerformClick();
            }
            else
            {
                File.WriteAllText(_fileName, sqlTextBox.Text);
                _changed = false;
            }
        }

        /// <summary>
        /// Appends the save.
        /// </summary>
        private void AppendSave()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                return;
            }

            // append the script to the file
            try
            {
                File.AppendAllText(_fileName, Environment.NewLine + sqlTextBox.Text);
                _changed = false;

                //var file = new System.IO.StreamWriter(_fileName, true);
                //file.WriteLine(sql);
                //file.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Handles the sql text box text changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SqlTextBox_TextChanged(object sender, EventArgs e)
        {
            _changed = true;
            statusToolStripStatusLabe.Text = string.Empty;
        }

        /// <summary>
        /// Handles the "create table" tool strip button click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CreateTableToolStripButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;

            var createScript = await definitionPanel.GetCreateObjectScript();
            if (createScript == null)
            {
                return;
            }

            SetScript(createScript);

            var objectName = objectsListBox.SelectedItem as ObjectName;
            if (!string.IsNullOrEmpty(objectName?.Name))
            {
                var description = ObjectDescription.BuildObjectDescription(objectName, Properties.Settings.Default.UseExtendedProperties);
                if (description.Length > 0)
                {
                    // append the description to the script
                    sqlTextBox.AppendText(description);
                    sqlTextBox.AppendText("GO" + Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(sqlTextBox.Text))
                {
                    Clipboard.SetText(sqlTextBox.Text);
                }
            }
        }

        /// <summary>
        /// Handles timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            Populate();
        }

        /// <summary>
        /// Handles the "insert" tool strip button click:
        ///     Build the INSERT statement for the selected object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void InsertToolStripButton_Click(object sender, EventArgs e)
        {
            ObjectName? objectName = objectsListBox.SelectedItem as ObjectName;
            if (objectName != null)
            {
                // checks if the object is a table or view
                if (objectName.ObjectType != ObjectName.ObjectTypeEnums.Table)
                {
                    // confirm if the user wants to continue
                    if (Common.MsgBox("The object is not a table. Are you sure you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return;
                    }
                }

                // get the number of rows in the table
                var rowCount = await DatabaseHelper.GetRowCountAsync(objectName.FullName);

                // confirm if the user wants to continue when the number of rows is too much
                if (rowCount > 1000)
                {
                    if (Common.MsgBox($"The table has {rowCount} rows. Are you sure you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return;
                    }
                }

                var sql = $"select * from {objectName.FullName}";
                var script = await Common.QueryDataToInsertStatementAsync(sql, objectName.FullName);

                if (script == "Too much rows")
                {
                    Common.MsgBox("Too much rows to insert", MessageBoxIcon.Error);
                    return;
                }

                // append the insert statement to the script
                sqlTextBox.AppendText(script);
                sqlTextBox.AppendText("GO" + Environment.NewLine);
                if (!string.IsNullOrEmpty(sqlTextBox.Text))
                {
                    Clipboard.SetText(sqlTextBox.Text);
                }
            }
        }

        /// <summary>
        /// Handles the "execute on STG" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExecuteOnSTGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteScripts("csbc-reporting-prod-sqldb-stg");
        }

        /// <summary>
        /// Handles the "Excel to INSERT" tool strip menu item click:
        ///     Load Excel file and generate INSERT statements for the data in the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExcelToINSERTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the Excel file name
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileName = openFileDialog.FileName;

            using var form = new ExcelSheetsForm()
            {
                FileName = fileName
            };
            form.ShowDialog();
            if (form.ResultDataTable != null)
            {
                var dataHelper = new ExcelDataHelper(form.ResultDataTable);
                sqlTextBox.Text = dataHelper.GetInsertStatement();
                //sqlTextBox.AppendText("GO" + Environment.NewLine);
                if (!string.IsNullOrEmpty(sqlTextBox.Text))
                {
                    Clipboard.SetText(sqlTextBox.Text);
                }
            }
        }

        /// <summary>
        /// Handles the "Extended properties" check box checked changed event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExtendedPropertiesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseExtendedProperties = extendedPropertiesCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the "Create stored procedure" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void UspToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = $@"IF OBJECT_ID('dbo.usp_AddObjectDescription', 'P') IS NOT NULL
	DROP PROCEDURE dbo.usp_AddObjectDescription;
GO
CREATE PROCEDURE usp_AddObjectDescription
(
	@TableName sysname,
	@Description nvarchar(1024)
)
AS
BEGIN
    SET NOCOUNT ON

	IF OBJECT_ID(@TableName) IS NOT NULL AND LEN(COALESCE(@Description, '')) > 0
	BEGIN
		DECLARE @Schema varchar(100) = OBJECT_SCHEMA_NAME(OBJECT_ID(@TableName));
		DECLARE @ObjectName varchar(100) = OBJECT_NAME(OBJECT_ID(@TableName));
		DECLARE @ObjectType varchar(100);
		SELECT @ObjectType = CASE type_desc WHEN 'USER_TABLE' THEN 'TABLE' ELSE 'VIEW' END
		  FROM sys.objects
		 WHERE object_id = OBJECT_ID(@TableName);

		IF EXISTS (	SELECT value
					FROM sys.extended_properties
					WHERE class = 1 AND major_id = OBJECT_ID(@TableName)
						AND minor_id = 0
						AND name = 'MS_Description')
			EXEC sp_updateextendedproperty @name = N'MS_Description', @value = @Description, 
				@level0type = N'SCHEMA', @level0name = @Schema, 
				@level1type = @ObjectType, @level1name = @ObjectName;
		ELSE
			EXEC sp_addextendedproperty @name = N'MS_Description', @value = @Description, 
				@level0type = N'SCHEMA', @level0name = @Schema, 
				@level1type = @ObjectType, @level1name = @ObjectName;
	
	END
END
GO
IF OBJECT_ID('dbo.usp_AddColumnDescription', 'P') IS NOT NULL
	DROP PROCEDURE dbo.usp_AddColumnDescription;
GO
CREATE PROCEDURE usp_AddColumnDescription
(
	@TableName sysname,
	@ColumnName varchar(200),
	@Description nvarchar(1024)
)
AS
BEGIN
    SET NOCOUNT ON

	IF OBJECT_ID(@TableName) IS NOT NULL AND LEN(COALESCE(@Description, '')) > 0
		IF EXISTS (SELECT name
					 FROM sys.columns 
					WHERE object_id = OBJECT_ID(@TableName) 
					  AND name = @ColumnName)
		BEGIN 

			DECLARE @Schema varchar(100) = OBJECT_SCHEMA_NAME(OBJECT_ID(@TableName));
			DECLARE @ObjectName varchar(100) = OBJECT_NAME(OBJECT_ID(@TableName));
			DECLARE @ObjectType varchar(100);
			SELECT @ObjectType = CASE type_desc WHEN 'USER_TABLE' THEN 'TABLE' ELSE 'VIEW' END
			  FROM sys.objects
			 WHERE object_id = OBJECT_ID(@TableName);

			IF EXISTS (	SELECT value
						FROM sys.extended_properties
						WHERE class = 1 AND major_id = OBJECT_ID(@TableName)
							AND minor_id = (SELECT column_id FROM sys.columns WHERE name = @ColumnName AND object_id = OBJECT_ID(@TableName))
							AND name = 'MS_Description')
				EXEC sp_updateextendedproperty @name = N'MS_Description', @value = @Description, 
					@level0type = N'SCHEMA', @level0name = @Schema, 
					@level1type = @ObjectType, @level1name = @ObjectName, 
					@level2type = N'COLUMN', @level2name = @ColumnName
			ELSE
				EXEC sp_addextendedproperty @name = N'MS_Description', @value = @Description, 
					@level0type = N'SCHEMA', @level0name = @Schema, 
					@level1type = @ObjectType, @level1name = @ObjectName, 
					@level2type = N'COLUMN', @level2name = @ColumnName
	
		END
END
GO
";
            Clipboard.SetText(sqlTextBox.Text);
        }

        /// <summary>
        /// Handles the "Panel1" resize event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Panel1_Resize(object sender, EventArgs e)
        {
            panel1.Height = searchTextBox.Height + 1;
            clearSearchButton.Left = panel1.Width - clearSearchButton.Width - 1;
            searchTextBox.Width = panel1.Width - clearSearchButton.Width - 2;
        }

        /// <summary>
        /// Handles the "Clear search" button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ClearSearchButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
            searchTextBox.Focus();
        }
    }
}