using DarkModeForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

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

        private bool _changed = false;

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
        private List<ObjectName>? _tables = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBuilderForm"/> class.
        /// </summary>
        public TableBuilderForm()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
        }

        /// <summary>
        /// Executes the scripts.
        /// </summary>
        private static async Task<string> ExecuteScriptsAsync(SQLDatabaseConnectionItem connection, string script)
        {
            // from the sqlTextBox.Text, break it into individual SQL statements by the GO keyword
            //var sqlStatements = sqlTextBox.Text.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);
            var sqlStatements = Regex.Split(script, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // execute each statement
            foreach (var sql in sqlStatements)
            {
                //Execute(builder.ConnectionString, sql);
                if (sql.Length > 0)
                {
                    var result = await DatabaseHelper.ExecuteSQLAsync(sql, connection.ConnectionString);
                    if (result != string.Empty)
                    {
                        return result;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Recursively gets the currently focused control within a container.
        /// </summary>
        /// <param name="container">The container control.</param>
        /// <returns>The focused control, or null if none is focused.</returns>
        private static Control? GetFocusedControl(Control container)
        {
            if (container == null)
                return null;

            if (container.Focused)
                return container;

            foreach (Control child in container.Controls)
            {
                Control? focused = GetFocusedControl(child);
                if (focused != null)
                    return focused;
            }

            return null;
        }

        /// <summary>
        /// Gets the object create script.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        private static async Task<string> GetObjectCreateScriptAsync(ObjectName objectName)
        {
            string? createScript = string.Empty;
            if (objectName != null)
            {
                createScript = await DatabaseDocBuilder.GetCreateObjectScriptAsync(objectName);
            }

            if (string.IsNullOrEmpty(createScript))
            {
                return string.Empty;
            }

            // remove the space and new line at the end of the script
            createScript = createScript.TrimEnd([' ', '\r', '\n', '\t']);

            // add "GO" at the end of the script if it doesn't exist
            if (!createScript.EndsWith("GO", StringComparison.CurrentCultureIgnoreCase))
            {
                createScript += Environment.NewLine + "GO" + Environment.NewLine;
            }
            else
            {
                createScript += Environment.NewLine;
            }

            // get the object description for table and view
            if (objectName?.ObjectType == ObjectName.ObjectTypeEnums.Table || objectName?.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                var description = await ObjectDescription.BuildObjectDescription(objectName, Properties.Settings.Default.UseExtendedProperties);
                if (description.Length > 0)
                {
                    // append the description to the script
                    createScript += description;
                    createScript += Environment.NewLine + "GO" + Environment.NewLine;
                }
            }

            return createScript;
        }

        /// <summary>
        /// Selects the objects.
        /// </summary>
        /// <returns>A List&lt;ObjectName&gt;? .</returns>
        private static List<ObjectName>? SelectObjects()
        {
            var form = new DBObjectsSelectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                return form.SelectedObjects;
            }
            return null;
        }

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
        private async Task<bool> AddConnection()
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

                await ChangeDBConnectionAsync(connection);

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
        private void AddListItem(ObjectTypeEnums tableType, string schema, string tableName)
        {
            objectsListBox.Items.Add(new ObjectName(tableType, schema, tableName));
        }

        /// <summary>
        /// Appends the current text in the sqlTextBox to the file.
        /// </summary>
        private void AppendSave()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                saveAsToolStripMenuItem.PerformClick();
            }
            else
            {
                // append the script to the file
                try
                {
                    File.AppendAllText(_fileName, Environment.NewLine + sqlTextBox.Text);
                    _changed = false;
                }
                catch (Exception)
                {
                }
            }
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
        private async Task ChangeDBConnectionAsync(SQLDatabaseConnectionItem connection)
        {
            if (connection != null)
            {
                bool connectionChanged = true;

                if (connectionChanged)
                {
                    _selectedConnection = connection;
                    serverToolStripStatusLabel.Text = "";
                    databaseToolStripStatusLabel.Text = "";
                    schemaComboBox.Items.Clear();
                    searchTextBox.Text = string.Empty;
                    objectsListBox.Items.Clear();

                    string? connectionString = connection?.ConnectionString?.Length == 0 ? await connection.Login() : connection?.ConnectionString;

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

                            Properties.Settings.Default.LastAccessConnectionIndex = i;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            submenuitem.Checked = false;
                        }
                    }

                    // set the object type combo box to the first item
                    objectTypeComboBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Handles the "Clear search" button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ClearSerachButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
            searchTextBox.Focus();
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
            if (_changed)
            {
                var result = Common.MsgBox("Do you want to save the changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    AppendSave();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            Close();
        }

        /// <summary>
        /// Copy tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null)
            {
                return;
            }

            if (focusedControl is TextBox textBox)
            {
                textBox.Copy();
            }
            else if (focusedControl is DBObjectDefPanel dBObjectDefPanel)
            {
                dBObjectDefPanel.Copy();
            }
            else
            {
                statusToolStripStatusLabe.Text = "No valid control is focused for copying.";
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
        /// Handles the "create insert" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CREATEINSERTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;

            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            StartBuild();

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                int percentComplete = (i * 100) / selectedObjects.Count;
                if (percentComplete > 0 && percentComplete % 2 == 0)
                {
                    progressBar.Value = percentComplete;
                }
                statusToolStripStatusLabe.Text = $"Processing {percentComplete}%...";

                var obj = selectedObjects[i];

                // get the object create script
                var script = await GetObjectCreateScriptAsync(obj);
                sqlTextBox.AppendText(script);

                // get the insert statement for the object
                // get the number of rows in the table
                var rowCount = await DatabaseHelper.GetRowCountAsync(obj.FullName);

                // confirm if the user wants to continue when the number of rows is too much
                if (rowCount > 1000)
                {
                    sqlTextBox.AppendText("-- Too many rows to insert" + Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    var sql = $"select * from {obj.FullName}";
                    var insertScript = await DatabaseDocBuilder.QueryDataToInsertStatementAsync(sql, obj.FullName);
                    sqlTextBox.AppendText(insertScript + "GO" + Environment.NewLine);
                }
            }

            EndBuild();
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
        /// Handles the "create table" tool strip button click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CreateTableToolStripButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;

            if (objectsListBox.SelectedItem is not ObjectName objectName)
            {
                return;
            }

            var script = await GetObjectCreateScriptAsync(objectName);

            if (!string.IsNullOrEmpty(script))
            {
                SetScript(script);
            }
        }

        /// <summary>
        /// Handles the "Create" tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CREATEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;

            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            StartBuild();

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                int percentComplete = (i * 100) / selectedObjects.Count;
                if (percentComplete > 0 && percentComplete % 2 == 0)
                {
                    progressBar.Value = percentComplete;
                }
                statusToolStripStatusLabe.Text = $"Processing {percentComplete}%...";

                var script = await GetObjectCreateScriptAsync(selectedObjects[i]);

                sqlTextBox.AppendText(script);
            }

            EndBuild();
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
        /// Gets the table schemas.
        /// </summary>
        /// <returns>A list of string.</returns>
        private List<string> GetTableSchemas()
        {
            // get the unique schema names from the _table
            List<string> dtSchemas = [];
            if (_tables != null)
            {
                foreach (var table in _tables)
                {
                    if (!dtSchemas.Contains(table.Schema))
                    {
                        dtSchemas.Add(table.Schema);
                    }
                }
            }
            return dtSchemas;
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
        /// Handles the "insert" tool strip button click:
        ///     Build the INSERT statement for the selected object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void InsertToolStripButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem is ObjectName objectName)
            {
                // return if object type is not a table or view
                if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table || objectName.ObjectType == ObjectTypeEnums.View)
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

                    // checks if the table has identify column
                    var hasIdentityColumn = await DatabaseHelper.HasIdentityColumnAsync(objectName);

                    var sql = $"select * from {objectName.FullName}";
                    var script = await DatabaseDocBuilder.QueryDataToInsertStatementAsync(sql, objectName.FullName);

                    if (script == "Too much rows")
                    {
                        Common.MsgBox("Too much rows to insert", MessageBoxIcon.Error);
                        return;
                    }
                    else if (!string.IsNullOrEmpty(script))
                    {
                        // add SET IDENTITY_INSERT ON if the table has identity column
                        if (hasIdentityColumn)
                        {
                            sqlTextBox.AppendText("SET IDENTITY_INSERT " + objectName.FullName + " ON;" + Environment.NewLine + "GO" + Environment.NewLine);
                        }

                        // append the insert statement to the script
                        sqlTextBox.AppendText(script);

                        // add SET IDENTITY_INSERT OFF if the table has identity column
                        if (hasIdentityColumn)
                        {
                            sqlTextBox.AppendText("GO" + Environment.NewLine);
                            sqlTextBox.AppendText("SET IDENTITY_INSERT " + objectName.FullName + " OFF;" + Environment.NewLine);
                        }

                        sqlTextBox.AppendText("GO" + Environment.NewLine);
                        if (!string.IsNullOrEmpty(sqlTextBox.Text))
                        {
                            Clipboard.SetText(sqlTextBox.Text);
                        }
                    }
                    else
                    {
                        Common.MsgBox("No data found", MessageBoxIcon.Information);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the "Manage connections" tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ManageConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using var frm = new ConnectionManageForm() { DataConnections = _connections };
                frm.ShowDialog();

                await PopulateConnections();
            }
            catch (Exception)
            {
                // ignore the exception
            }
        }

        /// <summary>
        /// Handles the "New connection" tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void NewConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await AddConnection();
        }

        /// <summary>
        /// Handles the Click event of the NewToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_changed)
            {
                if (Common.MsgBox("Do you want to save the changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(sender, e);
                }
            }

            _fileName = string.Empty;
            this.Text = $"SharePoint Script Builder - (New)";

            // clear the sqlTextBox
            sqlTextBox.Text = string.Empty;
            sqlTextBox.Focus();

            _changed = false;
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
                await Task.Run(async () =>
                {
                    scripts = await ObjectDescription.BuildObjectDescriptionsAsync(ObjectName.ObjectTypeEnums.Table, dlg.Schema, progress);
                });

                if (scripts != null)
                {
                    SetScript(scripts);
                }

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the "Objects description" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ObjectsDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;

            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            StartBuild();

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                int percentComplete = (i * 100) / selectedObjects.Count;
                if (percentComplete > 0 && percentComplete % 2 == 0)
                {
                    progressBar.Value = percentComplete;
                }
                statusToolStripStatusLabe.Text = $"Processing {percentComplete}%...";

                var obj = selectedObjects[i];
                var script = await ObjectDescription.BuildObjectDescription(obj, Properties.Settings.Default.UseExtendedProperties);

                // add "GO" and new line after each object description if it is not empty
                if (!string.IsNullOrEmpty(script))
                {
                    script += Environment.NewLine + "GO" + Environment.NewLine;
                }

                sqlTextBox.AppendText(script);
            }

            EndBuild();
        }

        /// <summary>
        /// Objects the list box selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void ObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusToolStripStatusLabe.Text = string.Empty;
            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                await definitionPanel.OpenAsync(objectName);
            }
            else
            {
                await definitionPanel.OpenAsync(null);
            }
        }

        /// <summary>
        /// Handles the "ObjectType" combo box selected index changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ObjectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectTypeComboBox.SelectedIndex >= 0)
            {
                // get the database object by the selected object type
                switch (objectTypeComboBox.SelectedIndex)
                {
                    case 0:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Table);
                        break;

                    case 1:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.View);
                        break;

                    case 2:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.StoredProcedure);
                        break;

                    case 3:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Function);
                        break;

                    default:
                        break;
                }

                // keep the selected schema
                string schemaName = schemaComboBox.Text;

                PopulateSchema();
                if (schemaComboBox.Items.Count > 0)
                {
                    int index = 0;
                    for (int i = 0; i < schemaComboBox.Items.Count; i++)
                    {
                        if (schemaComboBox.Items[i].ToString().Equals(schemaName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }

                    schemaComboBox.SelectedIndex = index;
                }
            }
        }

        /// <summary>
        /// On connection tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void OnConnectionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender?.GetType() == typeof(ConnectionMenuItem))
            {
                ConnectionMenuItem menuItem = (ConnectionMenuItem)sender;

                statusToolStripStatusLabe.Text = string.Format("Connect to {0}...", menuItem.ToString());
                Cursor = Cursors.WaitCursor;

                await ChangeDBConnectionAsync(menuItem.Connection);

                ObjectTypeComboBox_SelectedIndexChanged(sender, e);

                Cursor = Cursors.Default;
                statusToolStripStatusLabe.Text = "";
            }
        }

        /// <summary>
        /// Handles the "Execute " tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void OnExecuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the selected text from the sqlTextBox
            string script = sqlTextBox.SelectedText;
            if (script.Length == 0)
            {
                // if no text is selected, get the whole text from the sqlTextBox
                script = sqlTextBox.Text;
            }

            // if the script is empty, show a message box and return
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

            if (sender?.GetType() == typeof(ConnectionMenuItem))
            {
                // get the connection from the sender
                ConnectionMenuItem menuItem = (ConnectionMenuItem)sender;

                // get the connection string from the menu item
                var connection = menuItem.Connection;
                if (connection == null || string.IsNullOrEmpty(connection?.ConnectionString))
                {
                    // show a message box if the connection string is empty
                    Common.MsgBox("Database connection not available", MessageBoxIcon.Error);
                    return;
                }

                statusToolStripStatusLabe.Text = string.Format("Execute on {0}...", menuItem.ToString());
                Cursor = Cursors.WaitCursor;

                var executeResults = await ExecuteScriptsAsync(connection, script);

                if (executeResults.Length > 0)
                {
                    Common.MsgBox(executeResults, MessageBoxIcon.Error);
                }
                else
                {
                    Common.MsgBox("Execute completed successfully", MessageBoxIcon.Information);
                }

                Cursor = Cursors.Default;
                statusToolStripStatusLabe.Text = "";
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
        /// Handles the "Panel2" resize event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Panel2_Resize(object sender, EventArgs e)
        {
            int width = panel2.Width - 6;
            if (width > 0)
            {
                objectTypeComboBox.Width = width;
                schemaComboBox.Width = width;
                clearSerachButton.Left = panel2.Width - clearSerachButton.Width - 2;
                searchTextBox.Width = panel2.Width - clearSerachButton.Width - 6;
                panel2.Height = searchTextBox.Top + searchTextBox.Height + 2;
            }
        }

        /// <summary>
        /// Handles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null)
            {
                return;
            }

            if (focusedControl is TextBox textBox)
            {
                textBox.Paste();
            }
            else if (focusedControl is DBObjectDefPanel dBObjectDefPanel)
            {
                dBObjectDefPanel.Paste();
            }
            else
            {
                statusToolStripStatusLabe.Text = "No valid control is focused for pasting.";
            }
        }

        /// <summary>
        /// Populates the objects list box with tables, ordered by schema name and table name.
        /// </summary>
        private async Task PopulateAsync()
        {
            await definitionPanel.OpenAsync(null);
            string schemaName = string.Empty;
            if (schemaComboBox.SelectedIndex > 0)
                schemaName = schemaComboBox.Items[schemaComboBox.SelectedIndex].ToString();

            if (_tables != null)
            {
                objectsListBox.Items.Clear();
                string searchFor = searchTextBox.Text.Trim();

                foreach (var table in _tables)
                {
                    if (string.IsNullOrEmpty(searchFor))
                    {
                        if (string.IsNullOrEmpty(schemaName))
                        {
                            AddListItem(table.ObjectType, table.Schema, table.Name);
                        }
                        else
                        {
                            if (schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
                                AddListItem(table.ObjectType, table.Schema, table.Name);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(schemaName))
                        {
                            // Use the LIKE operator to find tables that contain the search string
                            if (table.Name.Contains(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                AddListItem(table.ObjectType, table.Schema, table.Name);
                        }
                        else
                        {
                            if (schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase) &&
                                table.Name.Contains(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                AddListItem(table.ObjectType, table.Schema, table.Name);
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
        private async Task PopulateConnections()
        {
            _selectedConnection = null;

            // clear menu items under Connect to... menu
            for (int i = connectToToolStripMenuItem.DropDown.Items.Count - 1; i >= 0; i--)
            {
                var submenuitem = connectToToolStripMenuItem.DropDown.Items[i];
                submenuitem.Click -= OnConnectionToolStripMenuItem_Click;
                connectToToolStripMenuItem.DropDownItems.RemoveAt(i);
            }

            for (int i = exeToolStripDropDownButton.DropDown.Items.Count - 1; i >= 0; i--)
            {
                var submenuitem = exeToolStripDropDownButton.DropDown.Items[i];
                submenuitem.Click -= OnExecuteToolStripMenuItem_Click;
                exeToolStripDropDownButton.DropDownItems.RemoveAt(i);
            }

            var connections = _connections.Connections;
            if (connections.Count == 0)
            {
                await AddConnection();
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

                        // add ToolStripMenuItem for the DropDown menu
                        var executeMenuItem = new ConnectionMenuItem(item)
                        {
                            Name = string.Format("ExecuteMenuItem{0}", i + 1),
                            Size = new Size(300, 26),
                        };
                        executeMenuItem.Click += OnExecuteToolStripMenuItem_Click;
                        exeToolStripDropDownButton.DropDown.Items.Add(executeMenuItem);
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

            var schemas = GetTableSchemas();
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
            if (!string.IsNullOrEmpty(form.DocumentBody))
            {
                sqlTextBox.Text = form.DocumentBody;
            }
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

            if (!string.IsNullOrEmpty(form.DocumentBody))
            {
                sqlTextBox.Text = form.DocumentBody;
            }
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
        /// Save tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_changed) return;

            if (string.IsNullOrEmpty(_fileName))
            {
                saveAsToolStripMenuItem.PerformClick();
                return;
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
        private async void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await PopulateAsync();
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
        private async void TableBuilderForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            _connections.Load();
            await PopulateConnections();

            var lastConnection = Properties.Settings.Default.LastAccessConnectionIndex;
            // set lastConnection to 0 if it is not set or out of range
            if (lastConnection < 0 || lastConnection >= connectToToolStripMenuItem.DropDown.Items.Count)
                lastConnection = 0;

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
                await ChangeDBConnectionAsync(selectedItem.Connection);
            }
            else
            {
                Close();
            }

            extendedPropertiesCheckBox.Checked = Properties.Settings.Default.UseExtendedProperties;

            WindowState = FormWindowState.Maximized;
            splitContainer1.SplitterDistance = 200;
            if (collapsibleSplitter1 != null)
                collapsibleSplitter1.SplitterDistance = (int)(this.Width * 0.4F);
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
                sqlTextBox.AppendText(await builder.GetTableDef(objectName));

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
        private async void TableDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objectName = objectsListBox.SelectedItem as ObjectName;
            if (!string.IsNullOrEmpty(objectName?.Name))
            {
                SetScript(await ObjectDescription.BuildObjectDescription(objectName, Properties.Settings.Default.UseExtendedProperties));
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
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildTableList(dlg.Schema, progress);
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
        /// Handles timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            await PopulateAsync();
        }

        /// <summary>
        /// Handles the "Create stored procedure" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void UspToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = DatabaseDocBuilder.UspAddObjectDescription();
            Clipboard.SetText(sqlTextBox.Text);
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
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildViewList(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
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
                await Task.Run(async () =>
                {
                    scripts = await ObjectDescription.BuildObjectDescriptionsAsync(ObjectName.ObjectTypeEnums.View, dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }
    }
}