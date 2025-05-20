using DarkModeForms;
using ScintillaNET;
using SQL_Document_Builder.ScintillaNetUtils;
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
            // from the CurrentEditBox?.Text, break it into individual SQL statements by the GO keyword
            //var sqlStatements = CurrentEditBox?.Text.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);
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
        /// Handles the "About" tool strip menu item click event:
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
            if (CurrentEditBox == null) return;

            if (string.IsNullOrEmpty(_fileName))
            {
                saveAsToolStripMenuItem.PerformClick();
            }
            else
            {
                // append the script to the file
                try
                {
                    File.AppendAllText(_fileName, Environment.NewLine + CurrentEditBox?.Text);
                    CurrentEditBox.Changed = false;
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Sets the title of the form.
        /// </summary>
        private void SetTitle(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                this.Text = $"SQL Server Document Builder - (New)";
            }
            else
            {
                this.Text = $"SQL Server Document Builder - {Path.GetFileName(fileName)}";
            }
        }

        /// <summary>
        /// Gets the current edit box.
        /// </summary>
        private SqlEditBox? CurrentEditBox => sqlTextBox;

        /// <summary>
        /// Clears the script text box.
        /// </summary>
        private DialogResult ClearTextBox()
        {
            if (string.IsNullOrEmpty(_fileName) && CurrentEditBox != null && CurrentEditBox?.Changed == false)
            {
                CurrentEditBox.Text = string.Empty;
                CurrentEditBox.Changed = false;
                SetTitle();

                return DialogResult.OK;
            }

            // if the Control key is pressed, clear the text box without asking to save
            if (ModifierKeys != Keys.Control)
            {
                if (CurrentEditBox.Changed)
                {
                    var result = Common.MsgBox("Do you want to save the changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return result;
                    }

                    if (result == DialogResult.Yes)
                    {
                        saveToolStripMenuItem.PerformClick();
                    }
                }
            }

            if (CurrentEditBox != null)
            {
                CurrentEditBox.Text = string.Empty;
            }
            CurrentEditBox.Changed = false;
            _fileName = string.Empty;
            SetTitle();
            return DialogResult.OK;
        }

        /// <summary>
        /// Handles the "Assistant content" tool strip menu item click event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void AssistantContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

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
        /// Handles the "Batch column description" tool strip button click event:
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
            if (ClearTextBox() == DialogResult.Cancel) return;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new SharePointBuilder();
                    SetScript(builder.TextToTable(metaData));
                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Handles the "Close" tool strip button click event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox != null)
            {
                if (CurrentEditBox.Changed)
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
            }

            Close();
        }

        /// <summary>
        /// Handles the "Copy" tool strip menu item click:
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
            else if (focusedControl is Scintilla scintilla)
            {
                scintilla.Copy();
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.Copy();
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
            if (ClearTextBox() == DialogResult.Cancel) return;

            SetScript(definitionPanel.CreateIndexScript());
        }

        /// <summary>
        /// Handles the "create insert" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CREATEINSERTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

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
                CurrentEditBox?.AppendText(script);

                // get the insert statement for the object
                // get the number of rows in the table
                var rowCount = await DatabaseHelper.GetRowCountAsync(obj.FullName);

                // confirm if the user wants to continue when the number of rows is too much
                if (rowCount > Properties.Settings.Default.InertMaxRows)
                {
                    CurrentEditBox?.AppendText("-- Too many rows to insert" + Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    var insertScript = await DatabaseDocBuilder.TableToInsertStatementAsync(obj);
                    CurrentEditBox?.AppendText(insertScript + "GO" + Environment.NewLine);
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
            if (ClearTextBox() == DialogResult.Cancel) return;

            SetScript(definitionPanel.PrimaryKeyScript());
            if (!string.IsNullOrEmpty(CurrentEditBox?.Text))
            {
                Clipboard.SetText(CurrentEditBox?.Text);
            }
        }

        /// <summary>
        /// Handles the "create table" tool strip button click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CreateTableToolStripButton_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

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
            if (ClearTextBox() == DialogResult.Cancel) return;

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

                CurrentEditBox?.AppendText(script);
            }

            EndBuild();
        }

        /// <summary>
        /// Handles the "Cut" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null)
            {
                return;
            }

            if (focusedControl is TextBox textBox)
            {
                textBox.Cut();
            }
            else if (focusedControl is DBObjectDefPanel dBObjectDefPanel)
            {
                dBObjectDefPanel.Cut();
            }
            else if (focusedControl is Scintilla scintilla)
            {
                scintilla.Cut();
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.Cut();
            }
            else
            {
                statusToolStripStatusLabe.Text = "No valid control is focused for cutting.";
            }
        }

        /// <summary>
        /// End build.
        /// </summary>
        private void EndBuild()
        {
            progressBar.Visible = false;
            statusToolStripStatusLabe.Text = "Complete!";
            this.Cursor = Cursors.Default;
            if (CurrentEditBox != null)
            {
                CurrentEditBox.Enabled = true;
                CurrentEditBox.Cursor = Cursors.Default;

                if (CurrentEditBox.Text.Length > 0)
                {
                    Clipboard.SetText(CurrentEditBox.Text);
                }
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
            if (CurrentEditBox == null) return;

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
                if (ClearTextBox() == DialogResult.Cancel) return;

                var dataHelper = new ExcelDataHelper(form.ResultDataTable);
                CurrentEditBox.Text = dataHelper.GetInsertStatement();
                //CurrentEditBox?.AppendText("GO" + Environment.NewLine);
                if (!string.IsNullOrEmpty(CurrentEditBox.Text))
                {
                    Clipboard.SetText(CurrentEditBox.Text);
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
        /// Handles the "Function list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void FunctionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePointBuilder();
                SetScript(await builder.BuildFunctionList(dlg.Schema));
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
        /// Handles the click event of the quick find tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void GoToLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // go to line in scintilla
            //var lineNumber = CurrentEditBox?.LineFromPosition(CurrentEditBox?.CurrentPosition) + 1;

            //int lineNumber = 1;
            //using (var dlg = new GoToLineForm(lineNumber))
            //{
            //    if (dlg.ShowDialog() == DialogResult.OK)
            //    {
            //        lineNumber = dlg.LineNumber - 1;
            //        CurrentEditBox?.GotoLine(lineNumber);
            //        CurrentEditBox?.SetEmptySelection(CurrentEditBox?.Lines[lineNumber].Position);
            //    }
            //}
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

                    var script = await DatabaseDocBuilder.TableToInsertStatementAsync(objectName);

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
                            CurrentEditBox?.AppendText("SET IDENTITY_INSERT " + objectName.FullName + " ON;" + Environment.NewLine + "GO" + Environment.NewLine);
                        }

                        // append the insert statement to the script
                        CurrentEditBox?.AppendText(script);

                        // add SET IDENTITY_INSERT OFF if the table has identity column
                        if (hasIdentityColumn)
                        {
                            CurrentEditBox?.AppendText("GO" + Environment.NewLine);
                            CurrentEditBox?.AppendText("SET IDENTITY_INSERT " + objectName.FullName + " OFF;" + Environment.NewLine);
                        }

                        CurrentEditBox?.AppendText("GO" + Environment.NewLine);
                        if (!string.IsNullOrEmpty(CurrentEditBox?.Text))
                        {
                            Clipboard.SetText(CurrentEditBox?.Text);
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
            if (CurrentEditBox != null)
            {
                // clear the sqlTextBox
                if (ClearTextBox() == DialogResult.Cancel) return;

                CurrentEditBox?.Focus();

                CurrentEditBox.Changed = false;
            }
        }

        /// <summary>
        /// Handles the "Objects description" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ObjectsDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

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

                CurrentEditBox?.AppendText(script);
            }

            EndBuild();
        }

        /// <summary>
        /// Handles the "Objects list" list box selected index changed event:
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
            string script = CurrentEditBox?.SelectedText;
            if (script.Length == 0)
            {
                // if no text is selected, get the whole text from the sqlTextBox
                script = CurrentEditBox?.Text;
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
            if (CurrentEditBox == null) return;

            if (CurrentEditBox.Changed)
            {
                if (Common.MsgBox("Do you want to save the changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(sender, e);
                }
            }

            var oFile = new OpenFileDialog() { Filter = "SQL script(*.sql)|*.sql|Text file(*.txt)|*.txt|All files(*.*)|*.*" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                _fileName = oFile.FileName;
                SetTitle(_fileName);

                CurrentEditBox.Text = File.ReadAllText(_fileName);
                CurrentEditBox.Changed = false;
            }
        }

        /// <summary>
        /// Handles the "Panel2" resize event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param {e">The e.</param>
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
            else if (focusedControl is Scintilla scintilla)
            {
                scintilla.Paste();
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.Paste();
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
        /// Handles the "Query data to table" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void QueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            if (ClearTextBox() == DialogResult.Cancel) return;

            using var form = new QueryDataToTableForm();
            form.ShowDialog();
            if (!string.IsNullOrEmpty(form.DocumentBody))
            {
                CurrentEditBox.Text = form.DocumentBody;
            }
        }

        /// <summary>
        /// Handles "Query to INSERT" strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void QueryInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            if (ClearTextBox() == DialogResult.Cancel) return;

            using var form = new QueryDataToTableForm()
            {
                InsertStatement = true
            };
            form.ShowDialog();

            if (!string.IsNullOrEmpty(form.DocumentBody))
            {
                CurrentEditBox.Text = form.DocumentBody;
            }
        }

        /// <summary>
        /// Handles the "Save as" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            try
            {
                var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql|Text file(*.txt)|*.txt|All files(*.*)|*.*" };
                if (oFile.ShowDialog() == DialogResult.OK)
                {
                    _fileName = oFile.FileName;
                    SetTitle(_fileName);

                    var file = new System.IO.StreamWriter(_fileName, false);
                    await file.WriteAsync(CurrentEditBox?.Text);
                    file.Close();
                    CurrentEditBox.Changed = false;

                    statusToolStripStatusLabe.Text = "Complete";
                }
            }
            catch (Exception ex)
            {
                // show error message if the file cannot be saved
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the "Save and replace" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SaveReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            // save the text in the sqlTextBox to the file
            if (string.IsNullOrEmpty(_fileName))
            {
                saveAsToolStripMenuItem.PerformClick();
            }
            else
            {
                File.WriteAllText(_fileName, CurrentEditBox?.Text);
                CurrentEditBox.Changed = false;
            }
        }

        /// <summary>
        /// Handles the "Save" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            if (!CurrentEditBox.Changed) return;

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
        /// Handles the "Schema" combo box selected index changed event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await PopulateAsync();
        }

        /// <summary>
        /// Handles the "Search" text box text changed event:
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
        /// Handles the "Select all" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null)
            {
                return;
            }

            if (focusedControl is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (focusedControl is DBObjectDefPanel dBObjectDefPanel)
            {
                dBObjectDefPanel.SelectAll();
            }
            else if (focusedControl is Scintilla scintilla)
            {
                scintilla.SelectAll();
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.SelectAll();
            }
            else
            {
                statusToolStripStatusLabe.Text = "No valid control is focused for select.";
            }
        }

        /// <summary>
        /// Saves the script.
        /// </summary>
        /// <param name="sql">The sql.</param>
        private void SetScript(string sql)
        {
            if (string.IsNullOrEmpty(sql) || CurrentEditBox == null)
            {
                return;
            }

            CurrentEditBox.Text = sql;
            Clipboard.SetText(sql);
        }

        /// <summary>
        /// Handles the resize event of the SQL text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SqlTextBox_Resize(object sender, EventArgs e)
        {
            searchPanel.Left = sqlTextBox.Left + sqlTextBox.Width - searchPanel.Width - 5;
        }

        /// <summary>
        /// Start build.
        /// </summary>
        private void StartBuild()
        {
            if (CurrentEditBox == null) return;

            this.Cursor = Cursors.WaitCursor;
            CurrentEditBox.Text = String.Empty;
            CurrentEditBox.Enabled = false;
            CurrentEditBox.Cursor = Cursors.WaitCursor;
            statusToolStripStatusLabe.Text = "Please wait while generate the scripts";
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = true;
        }

        /// <summary>
        /// Handles the "Stored procedure list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void StoredProcedureListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePointBuilder();
                SetScript(await builder.BuildSPList(dlg.Schema));
                EndBuild();
            }
        }

        /// <summary>
        /// Handles the "Table builder form" form closing event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurrentEditBox == null) return;
            if (CurrentEditBox.Changed)
            {
                if (Common.MsgBox("Do you want to save the changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(sender, e);
                }
            }
            //Properties.Settings.Default.SchemaSettings = _settingItems.Settings;
            //Properties.Settings.Default.Templetes = templates.TemplatesValue;
            //Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the "Table builder form" load event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void TableBuilderForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            // INIT HOTKEYS
            InitHotkeys();

            SetTitle();

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
            insertBatchTextBox.Text = Properties.Settings.Default.InsertBatchRows.ToString();
            insertMaxTextBox.Text = Properties.Settings.Default.InertMaxRows.ToString();

            WindowState = FormWindowState.Maximized;
            splitContainer1.SplitterDistance = 200;
            if (collapsibleSplitter1 != null)
                collapsibleSplitter1.SplitterDistance = (int)(this.Width * 0.4F);
        }

        /// <summary>
        /// Handles the "Table definition" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void TableDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            if (ClearTextBox() == DialogResult.Cancel) return;

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
                    CurrentEditBox.Text = String.Empty;
                }
                var builder = new SharePointBuilder();
                CurrentEditBox?.AppendText(await builder.GetTableDef(objectName));

                //if ((objectName.Name.StartsWith("LT_")) || (objectName.Name.StartsWith("AT_")))
                //{
                //    var valueBuilder = new SharePointBuilder();
                //    CurrentEditBox?.AppendText(await valueBuilder.GetTableValuesAsync(objectName.FullName));
                //}

                //CurrentEditBox?.AppendText(FooterText() + Environment.NewLine);
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
            if (ClearTextBox() == DialogResult.Cancel) return;

            var objectName = objectsListBox.SelectedItem as ObjectName;
            if (!string.IsNullOrEmpty(objectName?.Name))
            {
                SetScript(await ObjectDescription.BuildObjectDescription(objectName, Properties.Settings.Default.UseExtendedProperties));
                if (!string.IsNullOrEmpty(CurrentEditBox?.Text))
                {
                    Clipboard.SetText(CurrentEditBox?.Text);
                }
                else
                {
                    statusToolStripStatusLabe.Text = "No object description found";
                }
            }
        }

        /// <summary>
        /// Handles the "Table list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void TableListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new SharePointBuilder();
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildTableListAsync(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the "Table SQL" tool strip menu item click:
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
            //        CurrentEditBox?.Text = headerTextBox.Text + "\r\n";
            //    }
            //    else
            //    {
            //        CurrentEditBox?.Text = String.Empty;
            //    }
            //    var builder = new Wiki();
            //    CurrentEditBox?.AppendText(builder.GetTableDef(objectName));
            //    AppendLine(footerTextBox.Text);
            //    Clipboard.SetText(CurrentEditBox?.Text);
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
            if (CurrentEditBox == null)
            {
                return;
            }

            if (ClearTextBox() == DialogResult.Cancel) return;

            CurrentEditBox.Text = DatabaseDocBuilder.UspAddObjectDescription();
            Clipboard.SetText(CurrentEditBox?.Text);
        }

        /// <summary>
        /// Handles the "Value list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void ValueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var builder = new SharePointBuilder();
                SetScript(await builder.GetTableValuesAsync(objectName.FullName));
                EndBuild();
            }
        }

        /// <summary>
        /// Handles the "Values wiki" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ValuesWikiToolStripButton_Click(object sender, EventArgs e)
        {
            //if (objectsListBox.SelectedItem != null)
            //{
            //    var objectName = (ObjectName)objectsListBox.SelectedItem;
            //    var builder = new Wiki();
            //    CurrentEditBox?.Text = builder.GetTableValues(objectName.FullName);
            //    Clipboard.SetText(CurrentEditBox?.Text);
            //}
        }

        /// <summary>
        /// Handles the "View list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void ViewListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new SharePointBuilder();
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildViewListAsync(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }

        #region ScintillaNET

        /// <summary>
        /// Inits the hotkeys.
        /// </summary>
        private void InitHotkeys()
        {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(this, OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(this, OpenFindDialog, Keys.F, true, false, true);
            HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.R, true);
            HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.H, true);
            HotKeyManager.AddHotKey(this, Uppercase, Keys.U, true);
            HotKeyManager.AddHotKey(this, Lowercase, Keys.L, true);
            HotKeyManager.AddHotKey(this, ZoomIn, Keys.Oemplus, true);
            HotKeyManager.AddHotKey(this, ZoomOut, Keys.OemMinus, true);
            HotKeyManager.AddHotKey(this, ZoomDefault, Keys.D0, true);
            HotKeyManager.AddHotKey(this, CloseSearch, Keys.Escape);

            // remove conflicting hotkeys from scintilla
            sqlTextBox.ClearCmdKey(Keys.Control | Keys.F);
            sqlTextBox.ClearCmdKey(Keys.Control | Keys.R);
            sqlTextBox.ClearCmdKey(Keys.Control | Keys.H);
            sqlTextBox.ClearCmdKey(Keys.Control | Keys.L);
            sqlTextBox.ClearCmdKey(Keys.Control | Keys.U);
        }

        /// <summary>
        /// Handles the text changed event of the SQL text box:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnTextChanged(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;
            CurrentEditBox.OnTextChanged(sender, e);
            statusToolStripStatusLabe.Text = string.Empty;
        }

        #region Main Menu Commands

        private void ClearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditBox?.SetEmptySelection(0);
        }

        private void CollapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditBox?.FoldAll(FoldAction.Contract);
        }

        private void ExpandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditBox?.FoldAll(FoldAction.Expand);
        }

        private void FindAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenReplaceDialog();
        }

        private void FindDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFindDialog();
        }

        private void HiddenCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// toggle view whitespace
            //hiddenCharactersItem.Checked = !hiddenCharactersItem.Checked;
            //CurrentEditBox?.ViewWhitespace = hiddenCharactersItem.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void IndentGuidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// toggle indent guides
            //indentGuidesItem.Checked = !indentGuidesItem.Checked;
            //CurrentEditBox?.IndentationGuides = indentGuidesItem.Checked ? IndentView.LookBoth : IndentView.None;
        }

        private void IndentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Indent();
        }

        private void LowercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lowercase();
        }

        private void OutdentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Outdent();
        }

        /// <summary>
        /// Handles the click event of the quick find tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void QuickFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSearch();
        }

        private void SelectLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            Line line = CurrentEditBox.Lines[CurrentEditBox.CurrentLine];
            CurrentEditBox?.SetSelection(line.Position + line.Length, line.Position);
        }

        private void UppercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uppercase();
        }

        private void WordWrapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //// toggle word wrap
            //wordWrapItem.Checked = !wordWrapItem.Checked;
            //CurrentEditBox?.WrapMode = wordWrapItem.Checked ? WrapMode.Word : WrapMode.None;
        }

        private void Zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomDefault();
        }

        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        #endregion Main Menu Commands

        #region Uppercase / Lowercase

        /// <summary>
        /// Lowercases the.
        /// </summary>
        private void Lowercase()
        {
            if (CurrentEditBox == null) return;

            // save the selection
            int start = CurrentEditBox.SelectionStart;
            int end = CurrentEditBox.SelectionEnd;

            // modify the selected text
            CurrentEditBox?.ReplaceSelection(CurrentEditBox?.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            CurrentEditBox?.SetSelection(start, end);
        }

        /// <summary>
        /// Uppercases the.
        /// </summary>
        private void Uppercase()
        {
            if (CurrentEditBox == null) return;

            // save the selection
            int start = CurrentEditBox.SelectionStart;
            int end = CurrentEditBox.SelectionEnd;

            // modify the selected text
            CurrentEditBox?.ReplaceSelection(CurrentEditBox?.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            CurrentEditBox?.SetSelection(start, end);
        }

        #endregion Uppercase / Lowercase

        #region Indent / Outdent

        /// <summary>
        /// Generates the keystrokes.
        /// </summary>
        /// <param name="keys">The keys.</param>
        private void GenerateKeystrokes(string keys)
        {
            HotKeyManager.Enable = false;
            CurrentEditBox?.Focus();
            SendKeys.Send(keys);
            HotKeyManager.Enable = true;
        }

        /// <summary>
        /// Indents the.
        /// </summary>
        private void Indent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to indent,
            // although the indentation function exists. Pressing TAB with the editor focused confirms this.
            GenerateKeystrokes("{TAB}");
        }

        /// <summary>
        /// Outdents the.
        /// </summary>
        private void Outdent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to outdent,
            // although the indentation function exists. Pressing Shift+Tab with the editor focused confirms this.
            GenerateKeystrokes("+{TAB}");
        }

        #endregion Indent / Outdent

        #region Zoom

        /// <summary>
        /// Zooms the default.
        /// </summary>
        private void ZoomDefault()
        {
            if (CurrentEditBox == null) return;
            CurrentEditBox.Zoom = 0;
        }

        /// <summary>
        /// Zooms the in.
        /// </summary>
        private void ZoomIn()
        {
            CurrentEditBox?.ZoomIn();
        }

        /// <summary>
        /// Zooms the out.
        /// </summary>
        private void ZoomOut()
        {
            CurrentEditBox?.ZoomOut();
        }

        #endregion Zoom

        #endregion ScintillaNET

        #region Quick Search Bar

        private bool SearchIsOpen = false;

        /// <summary>
        /// Handles the click event of the close quick search button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CloseQuickSearch_Click(object sender, EventArgs e)
        {
            CloseSearch();
        }

        /// <summary>
        /// Handles the click event of the search next button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchNext_Click(object sender, EventArgs e)
        {
            SearchManager.Find(true, false);
        }

        /// <summary>
        /// Handles the click event of the search previous button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchPrevious_Click(object sender, EventArgs e)
        {
            SearchManager.Find(false, false);
        }

        /// <summary>
        /// Closes the search.
        /// </summary>
        private void CloseSearch()
        {
            if (SearchIsOpen)
            {
                SearchIsOpen = false;
                InvokeIfNeeded(delegate ()
                {
                    searchPanel.Visible = false;
                    //CurBrowser.GetBrowser().StopFinding(true);
                });
            }
        }

        /// <summary>
        /// Opens the search.
        /// </summary>
        private void OpenSearch()
        {
            if (CurrentEditBox == null) return;

            SearchManager.SearchBox = searchSQLTextBox;
            SearchManager.TextArea = sqlTextBox;

            if (!SearchIsOpen)
            {
                SearchIsOpen = true;
                InvokeIfNeeded(delegate ()
                {
                    searchPanel.Visible = true;
                    searchSQLTextBox.Text = SearchManager.LastSearch;
                    searchSQLTextBox.Focus();
                    searchSQLTextBox.SelectAll();
                });
            }
            else
            {
                InvokeIfNeeded(delegate ()
                {
                    searchSQLTextBox.Focus();
                    searchSQLTextBox.SelectAll();
                });
            }
        }

        /// <summary>
        /// Handles the key down event of the search text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                SearchManager.Find(true, false);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
            {
                SearchManager.Find(false, false);
            }
        }

        /// <summary>
        /// Handles the text changed event of the search text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchManager.Find(true, true);
        }

        #endregion Quick Search Bar

        #region Find & Replace Dialog

        /// <summary>
        /// Opens the find dialog.
        /// </summary>
        private void OpenFindDialog()
        {
            //open the find dialog
            //FindDialog dlg = new FindDialog(sqlTextBox, this);
            //dlg.Show(this);
            //dlg.BringToFront();
            //dlg.Focus();
        }

        /// <summary>
        /// Opens the replace dialog.
        /// </summary>
        private void OpenReplaceDialog()
        {
        }

        #endregion Find & Replace Dialog

        #region Utils

        /// <summary>
        /// Ints the to color.
        /// </summary>
        /// <param name="rgb">The rgb.</param>
        /// <returns>A Color.</returns>
        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// Invokes the if needed.
        /// </summary>
        /// <param name="action">The action.</param>
        public void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        #endregion Utils

        #region Markdown document builder

        /// <summary>
        /// Handles the click event of the clipboard to table tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ClipboardToTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new MarkdownBuilder();
                    SetScript(builder.TextToTable(metaData));
                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Handles the click event of the function list tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void FunctionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new MarkdownBuilder();
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildFunctionListAsync(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the stored procedure list tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void StoredProcedureListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new MarkdownBuilder();
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildSPListAsync(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the table definition tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void TableDefinitionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            if (ClearTextBox() == DialogResult.Cancel) return;

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
                    CurrentEditBox.Text = String.Empty;
                }
                var builder = new MarkdownBuilder();
                CurrentEditBox?.AppendText(await builder.GetTableDef(objectName));

                //if ((objectName.Name.StartsWith("LT_")) || (objectName.Name.StartsWith("AT_")))
                //{
                //    var valueBuilder = new MarkdownBuilder();
                //    CurrentEditBox?.AppendText(await valueBuilder.GetTableValuesAsync(objectName.FullName));
                //}

                //CurrentEditBox?.AppendText(FooterText() + Environment.NewLine);
                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the table list tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void TableListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new MarkdownBuilder();
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildTableList(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the table values (MarkDown) tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void TableValuesMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;

                var valueBuilder = new MarkdownBuilder();
                CurrentEditBox?.AppendText(await valueBuilder.GetTableValuesAsync(objectName.FullName));

                //CurrentEditBox?.AppendText(FooterText() + Environment.NewLine);
                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the view list tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ViewListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearTextBox() == DialogResult.Cancel) return;

            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                StartBuild();

                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                });

                string scripts = String.Empty;
                var builder = new MarkdownBuilder();
                await Task.Run(async () =>
                {
                    scripts = await builder.BuildViewListAsync(dlg.Schema, progress);
                });

                SetScript(scripts);

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the query data to table tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void QueryDataToTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        #endregion Markdown document builder

        /// <summary>
        /// Handles the validating event of the insert batch text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InsertBatchTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // checks the input value of the insert batch text box. it should be a number and between 1 and 100
            if (int.TryParse(insertBatchTextBox.Text, out int value))
            {
                if (value < 1 || value > 100)
                {
                    // show error message if the value is not between 1 and 100
                    Common.MsgBox("The value should be between 1 and 100", MessageBoxIcon.Error);
                    insertBatchTextBox.Text = Properties.Settings.Default.InsertBatchRows.ToString();
                    insertBatchTextBox.Focus();
                    insertBatchTextBox.SelectAll();
                }
            }
            else
            {
                // show error message if the value is not a number
                Common.MsgBox("The value should be a number", MessageBoxIcon.Error);
                insertBatchTextBox.Text = Properties.Settings.Default.InsertBatchRows.ToString();
                insertBatchTextBox.Focus();
                insertBatchTextBox.SelectAll();
            }
        }

        /// <summary>
        /// Handles the validated event of the insert batch text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InsertBatchTextBox_Validated(object sender, EventArgs e)
        {
            if (int.TryParse(insertBatchTextBox.Text, out int value))
            {
                Properties.Settings.Default.InsertBatchRows = value;
                Properties.Settings.Default.Save();
            }
            else
            {
                // show error message if the value is not a number
                Common.MsgBox("The value should be a number", MessageBoxIcon.Error);
                insertBatchTextBox.Text = Properties.Settings.Default.InsertBatchRows.ToString();
                insertBatchTextBox.Focus();
                insertBatchTextBox.SelectAll();
            }
        }

        /// <summary>
        /// Handles the validating event of the insert max text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InsertMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {            // checks the input value of the insert batch text box. it should be a number and between 1 and 100
            if (int.TryParse(insertMaxTextBox.Text, out int value))
            {
                if (value < 1 || value > 10000)
                {
                    // show error message if the value is not between 1 and 10000
                    Common.MsgBox("The value should be between 1 and 10,000", MessageBoxIcon.Error);
                    insertMaxTextBox.Text = Properties.Settings.Default.InertMaxRows.ToString();
                    insertMaxTextBox.Focus();
                    insertMaxTextBox.SelectAll();
                }
            }
            else
            {
                // show error message if the value is not a number
                Common.MsgBox("The value should be a number", MessageBoxIcon.Error);
                insertMaxTextBox.Text = Properties.Settings.Default.InertMaxRows.ToString();
                insertMaxTextBox.Focus();
                insertMaxTextBox.SelectAll();
            }

        }

        /// <summary>
        /// Handles the validated event of the insert max text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InsertMaxTextBox_Validated(object sender, EventArgs e)
        {
            if (int.TryParse(insertMaxTextBox.Text, out int value))
            {
                Properties.Settings.Default.InertMaxRows = value;
                Properties.Settings.Default.Save();
            }
            else
            {
                // show error message if the value is not a number
                Common.MsgBox("The value should be a number", MessageBoxIcon.Error);
                insertMaxTextBox.Text = Properties.Settings.Default.InertMaxRows.ToString();
                insertMaxTextBox.Focus();
                insertMaxTextBox.SelectAll();
            }
        }
    }
}