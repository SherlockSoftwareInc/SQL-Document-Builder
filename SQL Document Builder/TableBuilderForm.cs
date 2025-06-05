using DarkModeForms;
using ScintillaNET;
using SQL_Document_Builder.ScintillaNetUtils;
using SQL_Document_Builder.Template;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.AccessControl;
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

        private string _connectionString = string.Empty;

        private ObjectName? _selectedObject;

        /// <summary>
        /// The tables.
        /// </summary>
        private List<ObjectName>? _tables = [];

        private int _tabNo = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBuilderForm"/> class.
        /// </summary>
        public TableBuilderForm()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
        }

        /// <summary>
        /// Gets the current edit box.
        /// </summary>
        private SqlEditBox? CurrentEditBox
        {
            get
            {
                // get the currently selected tab page
                var selectedTab = tabControl1.SelectedTab;
                if (selectedTab != null)
                {
                    // get the edit box from the selected tab page
                    var editBox = selectedTab.Controls[0] as SqlEditBox;
                    return editBox;
                }

                return null;
            }
        }

        /// <summary>
        /// Executes the scripts.
        /// </summary>
        private static async Task<string> ExecuteScriptsAsync(SQLDatabaseConnectionItem connection, string script)
        {
            //// perform the syntax check first
            //var syntaxCheckResult = await SyntaxCheckAsync(script, connection.ConnectionString);
            //if (syntaxCheckResult != string.Empty)
            //{
            //    return syntaxCheckResult; // return the syntax error message
            //}

            // from the CurrentEditBox?.Text, break it into individual SQL statements by the GO keyword
            //var sqlStatements = CurrentEditBox?.Text.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);
            var sqlStatements = Regex.Split(script, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // execute each statement
            foreach (var sql in sqlStatements)
            {
                //Execute(builder.ConnectionString, sql);
                if (sql.Length > 0)
                {
                    //var parseResult = await SyntaxCheckAsync(sql, connection.ConnectionString);
                    //if(string.IsNullOrEmpty(parseResult))
                    //{
                    var result = await DatabaseHelper.ExecuteSQLAsync(sql, connection.ConnectionString);
                    if (result != string.Empty)
                    {
                        return result;
                    }
                    //}
                    //else
                    //{
                    //    return parseResult; // return the syntax error message
                    //}
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Files the display name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>A string.</returns>
        private static string FileDisplayName(string? fileName)
        {
            if (fileName?.Length > 50)
            {
                // Show the first 7 chars, then "...", then the last 20 chars
                return string.Concat(fileName.AsSpan(0, 7), "...", fileName.AsSpan(fileName.Length - 40));
            }
            else
            {
                return fileName ?? string.Empty;
            }
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
        private static async Task<string> GetObjectCreateScriptAsync(ObjectName objectName, string connectionString)
        {
            string? createScript = string.Empty;
            if (objectName != null)
            {
                createScript = await DatabaseDocBuilder.GetCreateObjectScriptAsync(objectName, connectionString);
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
            var description = await ObjectDescription.BuildObjectDescription(objectName, connectionString, Properties.Settings.Default.UseExtendedProperties);
            if (description.Length > 0)
            {
                // append the description to the script
                createScript += description;
                createScript += "GO" + Environment.NewLine;
            }

            return createScript;
        }

        /// <summary>
        /// Headers the text.
        /// </summary>
        /// <returns>A string.</returns>
        private static string HeaderText()
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
        /// Checks if the usp_addupdateextendedproperty stored procedure exists in the database.
        /// </summary>
        /// <returns>A Task.</returns>
        private static async Task<bool> IsAddObjectDescriptionSPExists(string connectionString)
        {
            string sql = "select ROUTINE_NAME from INFORMATION_SCHEMA.ROUTINES where ROUTINE_SCHEMA = 'dbo' and ROUTINE_NAME = 'usp_addupdateextendedproperty'";
            var returnValue = await DatabaseHelper.ExecuteScalarAsync(sql, connectionString);
            if (returnValue == null || returnValue == DBNull.Value)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform the syntax check for the given script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>A Task.</returns>
        private static async Task<string> SyntaxCheckAsync(string script, string connectionString = "")
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
                    var result = await DatabaseHelper.SyntaxCheckAsync(sql, connectionString);
                    if (result != string.Empty)
                    {
                        return result;
                    }
                }
            }
            return string.Empty;
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

                dataSourcesToolStripComboBox.Items.Add(connection);

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
        /// Adds the data source tag to the document.
        /// </summary>
        private void AddDataSourceText()
        {
            var editBox = CurrentEditBox;
            if (editBox == null) return;

            // add the data source tag to the document when the document is empty
            if (Properties.Settings.Default.AddDataSource)
            {
                string dataSourceText = $"{serverToolStripStatusLabel.Text}::{databaseToolStripStatusLabel.Text}";
                if (string.IsNullOrEmpty(editBox.Text) || editBox.DataSourceName != dataSourceText)
                {
                    editBox.DataSourceName = dataSourceText;
                    editBox.AppendText($"-- Data source: {dataSourceText}" + Environment.NewLine);
                }
            }
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
        /// Add a new tab with query edit box
        /// </summary>
        /// <param name="fileName"></param>
        private bool AddTab(string fileName)
        {
            // number of tabs is limited to 128
            if (tabControl1.TabPages.Count >= 128)
            {
                MessageBox.Show("You can open up to 128 editing windows.", "Too many editing windows",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            var queryTextBox = GetNewTextBox(!(fileName.Length == 0));

            if (fileName.Length > 0)
            {
                queryTextBox.OpenFile(fileName);
            }

            AddWindowsMenuItem(queryTextBox.FileName?.Length == 0 ? tabControl1.SelectedTab.Text : FileDisplayName(queryTextBox.FileName), queryTextBox.ID, tabControl1.SelectedTab.ToolTipText);

            CurrentEditBox?.Focus();

            return true;
        }

        /// <summary>
        /// Add a menu item under the windows dropdown to link the tab
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private void AddWindowsMenuItem(string name, string id, string tooltipText)
        {
            for (int i = 0; i < windowsToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (id == windowsToolStripMenuItem.DropDownItems[i].Tag?.ToString())
                {
                    windowsToolStripMenuItem.DropDownItems[i].Text = name;
                    windowsToolStripMenuItem.DropDownItems[i].ToolTipText = tooltipText;
                    return;
                }
            }

            ToolStripMenuItem tabItem = new(name) { Tag = id, ToolTipText = tooltipText };
            tabItem.Click += WindowItem_Click;

            windowsToolStripMenuItem.DropDownItems.Add(tabItem);
        }

        /// <summary>
        /// Handles the "Assistant content" tool strip menu item click event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void AssistantContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Text) != DialogResult.Yes) return;

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
                    contents = builder.SchemaContent(_connectionString, progress);
                });

                CurrentEditBox?.AppendText(contents);

                // Move caret to end and scroll to it
                ScrollToCaret();

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
            using var frm = new BatchColumnDesc()
            {
                ConnectionString = _connectionString
            };
            frm.ShowDialog();
        }

        /// <summary>
        /// Begin adding ddl script.
        /// </summary>
        /// <returns>A bool.</returns>
        private bool BeginAddDDLScript()
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Sql) != DialogResult.Yes) return false;

            AddDataSourceText();

            return true;
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
                    serverToolStripStatusLabel.Text = "";
                    databaseToolStripStatusLabel.Text = "";
                    //schemaComboBox.Items.Clear();
                    //searchTextBox.Text = string.Empty;
                    objectsListBox.Items.Clear();

                    string? connectionString = connection?.ConnectionString?.Length == 0 ? await connection.Login() : connection?.ConnectionString;

                    if (connectionString?.Length > 0)
                    {
                        serverToolStripStatusLabel.Text = connection?.ServerName;
                        databaseToolStripStatusLabel.Text = connection?.Database;
                        Properties.Settings.Default.dbConnectionString = connectionString;
                        _connectionString = connectionString;
                    }

                    for (int i = 0; i < connectToToolStripMenuItem.DropDown.Items.Count; i++)
                    {
                        var submenuitem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[i];
                        if (submenuitem.Connection.Equals(connection))
                        {
                            submenuitem.Checked = true;

                            Properties.Settings.Default.LastAccessConnection = submenuitem.Connection.GUID;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            submenuitem.Checked = false;
                        }
                    }
                }
            }
            else
            {
                _connectionString = string.Empty;
            }
        }

        /// <summary>
        /// Checks the current document type.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <returns>A DialogResult.</returns>
        private DialogResult CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums targetType)
        {
            bool result = false;

            /// Check if the current edit box is null or not
            if (CurrentEditBox == null)
            {
                AddTab("");
                result = true;
            }
            else if (CurrentEditBox.Text.Length == 0)
            {
                result = true;
            }
            else if (CurrentEditBox.DocumentType != targetType)
            {
                if (Common.MsgBox("The current document type is not the same as the generated document type. Do you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    result = true;
                }
            }
            else
            {
                result = (CurrentEditBox.DocumentType == targetType);
            }

            if (result)
            {
                CurrentEditBox.DocumentType = targetType;
                return DialogResult.Yes;
            }
            else
            {
                return DialogResult.No;
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
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Html) != DialogResult.Yes) return;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new SharePointBuilder();
                    var scripts = SharePointBuilder.TextToTable(metaData);
                    CurrentEditBox?.AppendText(scripts);

                    // Move caret to end and scroll to it
                    ScrollToCaret();

                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Closes the all tabs.
        /// </summary>
        /// <returns>A DialogResult.</returns>
        private DialogResult CloseAllTabs()
        {
            if (tabControl1.TabCount == 0) return DialogResult.OK;
            for (int i = tabControl1.TabCount - 1; i >= 0; i--)
            {
                if (CloseATab(i) == DialogResult.Cancel) return DialogResult.Cancel;
            }
            return DialogResult.OK;
        }

        /// <summary>
        /// handles the click event of the close all tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseAllTabs() == DialogResult.Cancel)
            {
                return;
            }

            AddTab("");
        }

        /// <summary>
        /// Closes the a tab by index.
        /// </summary>
        /// <param name="tabIndex">The tab index.</param>
        /// <returns>A DialogResult.</returns>
        private DialogResult CloseATab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= tabControl1.TabCount) return DialogResult.OK;

            tabControl1.SelectedIndex = tabIndex;

            var queryTextBox = GetTextBoxAt(tabIndex);
            if (queryTextBox != null)
            {
                if (queryTextBox.Modified)
                {
                    var checkResult = queryTextBox.SaveCheck();
                    if (checkResult == DialogResult.Cancel) return DialogResult.Cancel;

                    if (checkResult == DialogResult.Yes)
                    {
                        var saveResult = queryTextBox.Save();
                        if (saveResult == DialogResult.Cancel) return DialogResult.Cancel;
                    }
                }
                // Remove the menu item in Windows dropdown menu
                for (int i = 0; i < windowsToolStripMenuItem.DropDownItems.Count; i++)
                {
                    if (queryTextBox.ID == windowsToolStripMenuItem.DropDownItems[i].Tag?.ToString())
                    {
                        var menuItem = windowsToolStripMenuItem.DropDownItems[i];
                        menuItem.Click -= WindowItem_Click;
                        windowsToolStripMenuItem.DropDownItems.RemoveAt(i);
                        break;
                    }
                }
                // Remove the query edit box
                //queryTextBox.FontChanged -= new System.EventHandler(this.SqlTextBox_FontChanged);
                queryTextBox.TextChanged -= new System.EventHandler(this.OnTextChanged);
                queryTextBox.FileNameChanged -= new System.EventHandler(this.EditBox_FileNameChanged);
                //queryTextBox.Validated -= new System.EventHandler(this.SqlTextBox_Validated);
                queryTextBox.Dispose();
            }

            // Removes the tab:
            tabControl1.TabPages.RemoveAt(tabIndex);

            return DialogResult.OK;
        }

        /// <summary>
        /// Close a specified tab
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        private DialogResult CloseTab(int tabIndex)
        {
            var queryTextBox = GetTextBoxAt(tabIndex);
            if (queryTextBox != null)
            {
                if (queryTextBox.SaveCheck() == DialogResult.Cancel) return DialogResult.Cancel;

                // Remove the menu item in Windows dropdown menu
                for (int i = 0; i < windowsToolStripMenuItem.DropDownItems.Count; i++)
                {
                    if (queryTextBox.ID == windowsToolStripMenuItem.DropDownItems[i].Tag?.ToString())
                    {
                        var menuItem = windowsToolStripMenuItem.DropDownItems[i];
                        menuItem.Click -= WindowItem_Click;
                        windowsToolStripMenuItem.DropDownItems.RemoveAt(i);
                    }
                }

                // Remove the query edit box
                //queryTextBox.FontChanged -= new System.EventHandler(this.SqlTextBox_FontChanged);
                //queryTextBox.TextChanged -= new System.EventHandler(this.SqlTextBox_TextChanged);
                //queryTextBox.Validated -= new System.EventHandler(this.SqlTextBox_Validated);
                queryTextBox.Dispose();
            }

            if (tabIndex > 0)
            {
                tabControl1.SelectedIndex = tabIndex - 1;
            }
            // Removes the tab:
            tabControl1.TabPages.RemoveAt(tabIndex);

            return DialogResult.OK;
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
                //if (CurrentEditBox.Changed)
                //{
                //    var result = Common.MsgBox("Do you want to save the changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                //    if (result == DialogResult.Yes)
                //    {
                //        AppendSave();
                //    }
                //    else if (result == DialogResult.Cancel)
                //    {
                //        return;
                //    }
                //}
            }

            Close();
        }

        /// <summary>
        /// Handles the "Close" tool strip menu item click event:
        /// Close the mouse on tab
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mouseOnTabIndex < 0 || _mouseOnTabIndex >= tabControl1.TabCount) return;
            if (CloseTab(_mouseOnTabIndex) != DialogResult.Cancel)
            {
                if (tabControl1.TabCount == 0) AddTab("");
                if (tabControl1.TabCount > 0)
                {
                    tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                }
            }
        }

        /// <summary>
        /// Handles the click event of the close tool strip menu item:
        /// Close the current tab
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CloseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var selectedTabIndex = tabControl1.SelectedIndex;
            if (selectedTabIndex < 0 || selectedTabIndex >= tabControl1.TabCount) return;

            if (CloseTab(selectedTabIndex) != DialogResult.Cancel)
            {
                if (tabControl1.TabCount == 0) AddTab("");
            }
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
            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            if (BeginAddDDLScript())
            {
                editBox.AppendText(definitionPanel.CreateIndexScript());
                // Move caret to end and scroll to it
                ScrollToCaret();
            }
        }

        /// <summary>
        /// Handles the "create insert" tool strip menu item click:
        /// create insert statement for the selected objects and selected column
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CreateInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            if (BeginAddDDLScript())
            {
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
                    var script = await GetObjectCreateScriptAsync(obj, _connectionString);
                    if (editBox == null) return;
                    editBox.AppendText(script);

                    // Move caret to end and scroll to it
                    ScrollToCaret();

                    // get the insert statement for the object
                    // get the number of rows in the table
                    var rowCount = await DatabaseHelper.GetRowCountAsync(obj.FullName, _connectionString);

                    // confirm if the user wants to continue when the number of rows is too much
                    if (rowCount > Properties.Settings.Default.InertMaxRows)
                    {
                        CurrentEditBox?.AppendText("-- Too many rows to insert" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        var insertScript = await DatabaseDocBuilder.TableToInsertStatementAsync(obj, _connectionString);
                        if (editBox == null) return;
                        editBox.AppendText(insertScript + "GO" + Environment.NewLine);

                        // Move caret to end and scroll to it
                        ScrollToCaret();
                    }
                }

                EndBuild();
            }

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// handles the "create primary key" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CreatePrimaryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            if (BeginAddDDLScript())
            {
                editBox.AppendText(definitionPanel.PrimaryKeyScript());

                // Move caret to end and scroll to it
                ScrollToCaret();
            }
        }

        /// <summary>
        /// Handles the "create table" tool strip button click:
        /// Generate the CREATE script for the selected object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CreateTableToolStripButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem is not ObjectName objectName) return;

            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            if (!BeginAddDDLScript()) return;

            editBox.Cursor = Cursors.WaitCursor;
            var script = await GetObjectCreateScriptAsync(objectName, _connectionString);

            if (!string.IsNullOrEmpty(script))
            {
                AddDataSourceText();

                editBox.AppendText(script);
                // Move caret to end and scroll to it
                ScrollToCaret();
            }
            editBox.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles the "Create" tool strip menu item click event.
        /// Batch generate the CREATE scripts for the selected objects
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void CreateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            if (BeginAddDDLScript())
            {
                StartBuild();

                // get the current connection string
                string connectionString = _connectionString;

                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    int percentComplete = (i * 100) / selectedObjects.Count;
                    if (percentComplete > 0 && percentComplete % 2 == 0)
                    {
                        progressBar.Value = percentComplete;
                    }
                    statusToolStripStatusLabe.Text = $"Processing {percentComplete}%...";

                    var script = await GetObjectCreateScriptAsync(selectedObjects[i], connectionString);

                    if (editBox == null) return;
                    editBox?.AppendText(script);

                    // Move caret to end and scroll to it
                    ScrollToCaret();
                }

                EndBuild();
            }

            Cursor = Cursors.Default;
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
        /// Handles data source combo box selected index change event: Change the data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSourcesToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataSourcesToolStripComboBox.SelectedItem != null && !_ignoreConnectionComboBoxIndexChange)
            {
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                if (dataSourcesToolStripComboBox.SelectedItem is SQLDatabaseConnectionItem selectedItem)
                {
                    //await ChangeDBConnectionAsync(selectedItem);

                    // find the menu item
                    foreach (ToolStripMenuItem item in connectToToolStripMenuItem.DropDown.Items)
                    {
                        if (item is ConnectionMenuItem connectionMenuItem)
                        {
                            if (connectionMenuItem.Connection.GUID == selectedItem.GUID)
                            {
                                connectionMenuItem.Checked = true;
                                // perform menu item click event to update the connection
                                _ignoreConnectionComboBoxIndexChange = true;
                                OnConnectionToolStripMenuItem_Click(connectionMenuItem, EventArgs.Empty);
                                _ignoreConnectionComboBoxIndexChange = false;
                            }
                            else
                            {
                                connectionMenuItem.Checked = false;
                            }
                        }
                    }
                }

                Cursor = Cursors.Default;
                statusToolStripStatusLabe.Text = "";
            }
        }

        /// <summary>
        /// Edits the box_ file name changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void EditBox_FileNameChanged(object? sender, EventArgs e)
        {
            if (sender is SqlEditBox editBox)
            {
                // get the tab where the edit box is located
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    TabPage tabPage = tabControl1.TabPages[i];
                    if (tabPage != null)
                    {
                        if (tabPage.Controls[0] is SqlEditBox queryTextBox && editBox.ID == queryTextBox.ID)
                        {
                            // Set tab text to the title, or "new {index}" if empty
                            tabPage.Text = string.IsNullOrEmpty(queryTextBox.Title)
                                ? $"new {i}"
                                : queryTextBox.Title;

                            // Set tooltip to the full file name (full path)
                            tabPage.ToolTipText = queryTextBox.FileName;
                            break;
                        }
                    }
                }

                // set the windows menu item text to the file name
                for (int i = 0; i < windowsToolStripMenuItem.DropDownItems.Count; i++)
                {
                    if (editBox.ID == windowsToolStripMenuItem.DropDownItems[i].Tag?.ToString())
                    {
                        var menuItem = windowsToolStripMenuItem.DropDownItems[i];
                        menuItem.Text = FileDisplayName(editBox.FileName);
                        menuItem.ToolTipText = editBox.FileName;
                        break;
                    }
                }

                // add the file to the MRU files list
                _mruFiles.AddFile(editBox.FileName);
                PopulateMRUFiles();
            }
        }

        /// <summary>
        /// Enables the table value generation buttons/menu items.
        /// </summary>
        /// <param name="enabled">If true, enabled.</param>
        private void EnableTableValue(bool enabled)
        {
            mdValuesToolStripButton.Enabled = enabled;
            htmlValuesToolStripButton.Enabled = enabled;
            mdValuesToolStripMenuItem.Enabled = enabled;
            spValuesToolStripMenuItem.Enabled = enabled;
            insertToolStripButton.Enabled = enabled;
            insertToolStripMenuItem.Enabled = enabled;
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

                //if (CurrentEditBox.Text.Length > 0)
                //{
                //    Clipboard.SetText(CurrentEditBox.Text);
                //}
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
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Sql) == DialogResult.Yes)
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
                    var editBox = CurrentEditBox;
                    if (editBox == null)
                    {
                        return;
                    }

                    editBox.AppendText($"-- Data source: {fileName}" + Environment.NewLine);

                    var dataHelper = new ExcelDataHelper(form.ResultDataTable);
                    editBox.AppendText(dataHelper.GetInsertStatement(form.TableName, form.NullForBlank));
                    // Move caret to end and scroll to it
                    ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// Handles the click event of the find next button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void FindNextButton_Click(object sender, EventArgs e)
        {
            ReplaceManager.Find(true, false);
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
        /// Get next available empty new query edit box, add one if there is no empty
        /// </summary>
        /// <returns></returns>
        private SqlEditBox GetNewTextBox(bool lookForEmpty)
        {
            TabPage tabPage;
            if (lookForEmpty)
            {
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    tabPage = (TabPage)tabControl1.Controls[i];
                    if (tabPage?.Controls.Count > 0)
                    {
                        tabControl1.SelectedIndex = i;
                        var editBox = (SqlEditBox)tabPage.Controls[0];
                        if (editBox.FileName?.Length == 0 && editBox.Text.Length == 0) return editBox;
                    }
                }
            }

            var nextPageNum = GetNextTabNum();
            var textBoxName = $"sqlTextBox{nextPageNum}";
            var tabPageName = $"tabPage{nextPageNum}";
            var tabTitle = $"new {nextPageNum}";
            _tabNo++;

            var queryTextBox = new SqlEditBox()
            {
                Name = textBoxName,
                Dock = DockStyle.Fill,
                Location = new Point(6, 6),
                Margin = new Padding(6),
                TabIndex = _tabNo
            };

            //queryTextBox.QueryTextFontChanged += new System.EventHandler(this.SqlTextBox_FontChanged);
            //queryTextBox.QueryTextChanged += new EventHandler(SqlTextBox_TextChanged);
            //queryTextBox.QueryTextValidated += new System.EventHandler(this.SqlTextBox_Validated);
            //queryTextBox.QueryScriptGenerated += new System.EventHandler<QueryScriptGeneratedArgs>(this.SqlTextBox_QueryScriptGenerated);
            queryTextBox.FileNameChanged += EditBox_FileNameChanged;
            queryTextBox.TextChanged += OnTextChanged;

            tabPage = new TabPage(tabTitle)
            {
                Location = new Point(8, 39),
                Margin = new Padding(6),
                Padding = new Padding(6),
                Size = new Size(1806, 420),
                TabIndex = _tabNo,
                Name = tabPageName,
                UseVisualStyleBackColor = true
            };
            tabPage.SuspendLayout();
            tabPage.Controls.Add(queryTextBox);

            tabControl1.Controls.Add(tabPage);
            tabPage.ResumeLayout(true);

            queryTextBox.DefaultStyleFont = Properties.Settings.Default.EditorFont;
            //if (Properties.Settings.Default.DarkMode)
            //{
            //    queryTextBox.SetDarkTheme();
            //}
            //queryTextBox.ChangeEditorFont(Properties.Settings.Default.EditorFont);

            tabControl1.SelectedIndex = tabControl1.TabCount - 1;

            return queryTextBox;
        }

        /// <summary>
        /// Gets the next available tab number.
        /// </summary>
        /// <returns>An int.</returns>
        private int GetNextTabNum()
        {
            int pageNum = 1;

            for (int i = 1; i < 500; i++)
            {
                string pageName = $"tabPage{i}";
                // check if the tab page name already exists
                if (tabControl1.TabPages[pageName] == null)
                {
                    pageNum = i;
                    break;
                }
            }
            return pageNum;
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
        /// Get query text box on the specified tab
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        private SqlEditBox GetTextBoxAt(int tabIndex)
        {
            if (tabIndex >= 0 && tabIndex < tabControl1.TabCount)
            {
                if (tabControl1.TabPages.Count > 0)
                {
                    if (tabControl1.TabPages[tabIndex].Controls[0] is SqlEditBox box)
                    {
                        return box;
                    }
                }
            }

            return null;
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
                    var rowCount = await DatabaseHelper.GetRowCountAsync(objectName.FullName, _connectionString);

                    // confirm if the user wants to continue when the number of rows is too much
                    if (rowCount > 1000)
                    {
                        if (Common.MsgBox($"The table has {rowCount} rows. Are you sure you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    var editBox = CurrentEditBox;
                    if (editBox == null)
                    {
                        return;
                    }

                    Cursor = Cursors.WaitCursor;

                    // checks if the table has identify column
                    //var hasIdentityColumn = await DatabaseHelper.HasIdentityColumnAsync(objectName);

                    var script = await DatabaseDocBuilder.TableToInsertStatementAsync(objectName, _connectionString);

                    if (script == "Too much rows")
                    {
                        Common.MsgBox("Too much rows to insert", MessageBoxIcon.Error);
                        return;
                    }
                    else if (!string.IsNullOrEmpty(script))
                    {
                        if (!BeginAddDDLScript()) return;

                        // append the insert statement to the script
                        editBox.AppendText(script);

                        editBox.AppendText("GO" + Environment.NewLine);

                        // Move caret to end and scroll to it
                        ScrollToCaret();
                    }
                    else
                    {
                        Common.MsgBox("No data found", MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            Cursor = Cursors.Default;
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

                RestoreLastConnection();
            }
            catch (Exception)
            {
                // ignore the exception
            }
        }

        /// <summary>
        /// Handles the click event of the manage template tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ManageTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var templateEditor = new TemplateEditor();
            templateEditor.ShowDialog(this);
        }

        /// <summary>
        /// Handles the "New connection" tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void NewConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (await AddConnection())
            {
                if (dataSourcesToolStripComboBox.Items.Count > 0)
                {
                    dataSourcesToolStripComboBox.SelectedIndex = dataSourcesToolStripComboBox.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the NewToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTab("");
        }

        /// <summary>
        /// Handles the "Objects description" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ObjectsDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0) return;

            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            if (BeginAddDDLScript())
            {
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
                    var script = await ObjectDescription.BuildObjectDescription(obj, _connectionString, Properties.Settings.Default.UseExtendedProperties);

                    // add "GO" and new line after each object description if it is not empty
                    if (!string.IsNullOrEmpty(script))
                    {
                        script += Environment.NewLine + "GO" + Environment.NewLine;
                        if (editBox == null) return;
                        editBox.AppendText(script);

                        // Move caret to end and scroll to it
                        ScrollToCaret();
                    }
                }

                EndBuild();
            }

            Cursor = Cursors.Default;
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
                _selectedObject = (ObjectName)objectsListBox.SelectedItem;
                await definitionPanel.OpenAsync(_selectedObject, _connectionString);
            }
            else
            {
                await definitionPanel.OpenAsync(null, _connectionString);
            }
        }

        /// <summary>
        /// Handles the "ObjectType" combo box selected index changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ObjectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // keep the selected schema
            string schemaName = schemaComboBox.Text;

            if (objectTypeComboBox.SelectedIndex >= 0)
            {
                // get the database object by the selected object type
                switch (objectTypeComboBox.SelectedIndex)
                {
                    case 0:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Table, _connectionString);
                        EnableTableValue(true);
                        break;

                    case 1:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.View, _connectionString);
                        EnableTableValue(true);
                        break;

                    case 2:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.StoredProcedure, _connectionString);
                        EnableTableValue(false);
                        break;

                    case 3:
                        _tables = await DatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Function, _connectionString);
                        EnableTableValue(false);
                        break;

                    default:
                        break;
                }

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
                    if (schemaComboBox.SelectedIndex != index)
                    {
                        schemaComboBox.SelectedIndex = index;
                    }
                    else
                    {
                        SchemaComboBox_SelectedIndexChanged(sender, e); // re-populate the object list box
                    }
                }

                // if objects list box is not empty, select the first item
                if (objectsListBox.Items.Count > 0 && objectsListBox.SelectedItem == null)
                {
                    objectsListBox.SelectedIndex = 0;
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

                _populating = true;

                // keep the current selection in the combo boxes
                var selectedObjectType = objectTypeComboBox.Text;
                //var selectedSchema = schemaComboBox.Text;

                // clean up the schema and object list boxes
                //schemaComboBox.Items.Clear();
                objectsListBox.Items.Clear();

                await ChangeDBConnectionAsync(menuItem.Connection);

                if (!_ignoreConnectionComboBoxIndexChange)
                    SetConnectionComboBox(menuItem.Connection);

                // restore the object type
                if (!string.IsNullOrEmpty(selectedObjectType))
                {
                    int objectTypeIndex = objectTypeComboBox.Items.Count > 0 ? 0 : -1;
                    for (int i = 0; i < objectTypeComboBox.Items.Count; i++)
                    {
                        if (objectTypeComboBox.Items[i].ToString().Equals(selectedObjectType, StringComparison.CurrentCultureIgnoreCase))
                        {
                            objectTypeIndex = i;
                            break;
                        }
                    }
                    if (objectTypeComboBox.SelectedIndex == objectTypeIndex)
                    {
                        ObjectTypeComboBox_SelectedIndexChanged(sender, e); // re-populate the object list box
                    }
                    else
                    {
                        objectTypeComboBox.SelectedIndex = objectTypeIndex;
                    }
                }
                else
                {
                    objectTypeComboBox.SelectedIndex = 0; // default to the first item
                }

                _populating = false;
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
            string? script = CurrentEditBox?.SelectedText;
            if (script?.Length == 0)
            {
                // if no text is selected, get the whole text from the sqlTextBox
                script = CurrentEditBox?.Text;
            }

            // if the script is empty, show a message box and return
            if (string.IsNullOrEmpty(script))
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

            // get current connection from dataSourcesToolStripComboBox
            if (dataSourcesToolStripComboBox.SelectedItem == null)
            {
                // show a message box if no connection is selected
                Common.MsgBox("No database connection selected", MessageBoxIcon.Error);
                return;
            }

            // convert the selected item to SQLDatabaseConnectionItem
            if (dataSourcesToolStripComboBox.SelectedItem is SQLDatabaseConnectionItem connection)
            {
                if (connection.ConnectionString == null || connection.ConnectionString.Length == 0)
                {
                    Common.MsgBox("No database connection selected", MessageBoxIcon.Error);
                    return;
                }

                // check if the script contains a 'usp_addupdateextendedproperty' statement
                if (script.Contains("usp_addupdateextendedproperty", StringComparison.CurrentCultureIgnoreCase))
                {
                    bool spExists = await IsAddObjectDescriptionSPExists(connection.ConnectionString);
                    if (!spExists)
                    {
                        // ask for confirmation to execute the script
                        if (Common.MsgBox("The target database does not contains a usp_addupdateextendedproperty stored procedure. Do you want to create it?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            await DatabaseHelper.AddObjectDescriptionSPs(connection.ConnectionString);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                statusToolStripStatusLabe.Text = string.Format("Execute on {0}...", connection.ToString());
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
        /// Opens the file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        private void OpenFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            // checks if the file has already opened
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                var queryTextBox = GetTextBoxAt(i);
                if (queryTextBox != null && queryTextBox?.FileName == fileName)
                {
                    // if the file is already opened, select the tab and return
                    tabControl1.SelectedIndex = i;
                    return;
                }
            }

            // add a new tab with the opened file
            AddTab(fileName);

            // add the file to the MRU files list
            _mruFiles.AddFile(fileName);
            PopulateMRUFiles();
        }

        /// <summary>
        /// Handles the Click event of the OpenToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;

            var oFile = new OpenFileDialog()
            {
                Filter = "SQL script(*.sql)|*.sql|Markdown files(*.md)|*.md|HTML files(*.html)|*.html|Text file(*.txt)|*.txt|All files(*.*)|*.*",
                Multiselect = false
            };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                OpenFile(oFile.FileName);
            }
        }

        /// <summary>
        /// Handles the control changs of the output options:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Options_Changed(object sender, EventArgs e)
        {
            Properties.Settings.Default.AddDataSource = addDataSourceCheckBox.Checked;
            Properties.Settings.Default.AddDropStatement = scriptDropsCheckBox.Checked;
            Properties.Settings.Default.UseExtendedProperties = useExtendedPropertyRadioButton.Checked;
            Properties.Settings.Default.Save();
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
            await definitionPanel.OpenAsync(null, _connectionString);
            string schemaName = string.Empty;
            if (schemaComboBox.SelectedIndex > 0)
                schemaName = schemaComboBox.Text;

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

                // find the selected object in the list box if it is not null
                if (objectsListBox.Items.Count > 0)
                {
                    if (_selectedObject != null)
                    {
                        if (_selectedObject is ObjectName prevSelected)
                        {
                            for (int i = 0; i < objectsListBox.Items.Count; i++)
                            {
                                if (objectsListBox.Items[i] is ObjectName obj && obj.Equals(prevSelected))
                                {
                                    objectsListBox.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // select the first item in the list box if no object is selected
                        objectsListBox.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Populates the connections.
        /// </summary>
        private async Task PopulateConnections()
        {
            // clear menu items under Connect to... menu
            for (int i = connectToToolStripMenuItem.DropDown.Items.Count - 1; i >= 0; i--)
            {
                var submenuitem = connectToToolStripMenuItem.DropDown.Items[i];
                submenuitem.Click -= OnConnectionToolStripMenuItem_Click;
                connectToToolStripMenuItem.DropDownItems.RemoveAt(i);
            }

            // clear connections combobox
            if (dataSourcesToolStripComboBox.Items.Count > 0)
            {
                dataSourcesToolStripComboBox.Items.Clear();
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
                        // add connection to the combobox
                        dataSourcesToolStripComboBox.Items.Add(item);

                        // add ToolStripMenuItem for the Connect to... menu
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
        private async void QueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Html) != DialogResult.Yes) return;

            using var form = new QueryDataToTableForm() { ConnectionString = _connectionString };
            if (form.ShowDialog() == DialogResult.OK)
            {
                CurrentEditBox?.AppendText(await SharePointBuilder.GetTableValuesAsync(form.SQL, _connectionString));

                // Move caret to end and scroll to it
                ScrollToCaret();
            }
        }

        /// <summary>
        /// Handles "Query to INSERT" strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void QueryInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editBox = CurrentEditBox;
            if (editBox == null)
            {
                return;
            }

            if (BeginAddDDLScript())
            {
                using var form = new QueryDataToTableForm()
                {
                    ConnectionString = _connectionString,
                    InsertStatement = true
                };
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CurrentEditBox?.AppendText(await SharePointBuilder.GetTableValuesAsync(form.SQL, _connectionString));

                    // Move caret to end and scroll to it
                    ScrollToCaret();

                    //var sql = form.SQL;
                    //if (!string.IsNullOrEmpty(sql))
                    //{
                    //    AddDataSourceText();

                    //    editBox.AppendText(sql);

                    //    // Move caret to end and scroll to it
                    //    ScrollToCaret();
                    //}
                }
            }
        }

        /// <summary>
        /// Handles the click event of the replace all button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            ReplaceManager.ReplaceAll();
        }

        /// <summary>
        /// Handles the click event of the find previous button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            ReplaceManager.Replace();
        }

        /// <summary>
        /// Handles the key down event of the replace text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ReplaceReplaceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                if (ReplaceIsOpen)
                {
                    ReplaceManager.Replace();
                    //replaceButton.PerformClick();
                    //e.Handled = true;
                    //e.SuppressKeyPress = true;
                    replaceReplaceTextBox.Focus();
                }
            }
        }

        /// <summary>
        /// Restores the last connection.
        /// </summary>
        private void RestoreLastConnection()
        {
            // perform add connection if there is no connections
            if (_connections.Connections.Count == 0)
            {
                newConnectionToolStripMenuItem.PerformClick();
            }

            // if there are no connections, return
            if (_connections.Connections.Count == 0)
            {
                return;
            }

            // Restore the last connection from settings
            ConnectionMenuItem? matchedItems = null;
            var lastConnection = Properties.Settings.Default.LastAccessConnection;
            if (!string.IsNullOrEmpty(lastConnection))
            {
                // find the connection that matches the last access connection GUID
                for (int i = 0; i < connectToToolStripMenuItem.DropDown.Items.Count; i++)
                {
                    if (connectToToolStripMenuItem.DropDown.Items[i] is ConnectionMenuItem menuItem && menuItem.Connection.GUID == lastConnection)
                    {
                        matchedItems = menuItem;
                        break;
                    }
                }
            }

            // use the first connection if no match found
            matchedItems ??= connectToToolStripMenuItem.DropDown.Items[0] as ConnectionMenuItem;

            // if matched item is not null, perform click on it
            if (matchedItems != null)
            {
                OnConnectionToolStripMenuItem_Click(matchedItems, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the "Save as" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;
            CurrentEditBox.SaveAs();
            statusToolStripStatusLabe.Text = "Complete";
        }

        /// <summary>
        /// Handles the "Save" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return;
            CurrentEditBox?.Save();
        }

        /// <summary>
        /// Handles the click event of the save tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_mouseOnTabIndex < 0 || _mouseOnTabIndex >= tabControl1.TabCount) return;

            // get the edit box at the selected tab index
            var editBox = GetTextBoxAt(_mouseOnTabIndex);

            if (editBox == null) return;

            // save the file
            editBox.Save();

            //tabControl1.SelectedIndex = _mouseOnTabIndex;
            //if (_currentEditBox.FileName.Length > 0)
            //{
            //    _currentEditBox.SaveFile();
            //}
            //else
            //{
            //    SaveAsToolStripMenuItem_Click(sender, e);
            //}
        }

        /// <summary>
        /// Handles the "Schema" combo box selected index changed event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            await PopulateAsync();

            // if objects list box is not empty, select the first item
            if (objectsListBox.Items.Count > 0 && objectsListBox.SelectedItem == null)
            {
                objectsListBox.SelectedIndex = 0;
            }

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Scrolls the to caret.
        /// </summary>
        private void ScrollToCaret()
        {
            if (CurrentEditBox != null)
            {
                CurrentEditBox.SelectionStart = CurrentEditBox.TextLength;
                CurrentEditBox.SelectionEnd = CurrentEditBox.TextLength;
                CurrentEditBox.ScrollCaret();
                CurrentEditBox.Focus();
            }
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
        /// Selects the objects.
        /// </summary>
        /// <returns>A List&lt;ObjectName&gt;? .</returns>
        private List<ObjectName>? SelectObjects()
        {
            var form = new DBObjectsSelectForm()
            {
                ConnectionString = _connectionString,
            };
            if (form.ShowDialog() == DialogResult.OK)
            {
                return form.SelectedObjects;
            }
            return null;
        }

        /// <summary>
        /// Sets the connection combo box.
        /// </summary>
        /// <param name="connectionItem">The connection item.</param>
        private void SetConnectionComboBox(SQLDatabaseConnectionItem connectionItem)
        {
            _ignoreConnectionComboBoxIndexChange = true;

            // go through the dataSourcesToolStripComboBox and find the matched item
            for (int i = 0; i < dataSourcesToolStripComboBox.Items.Count; i++)
            {
                if (dataSourcesToolStripComboBox.Items[i] is SQLDatabaseConnectionItem comboItem)
                {
                    if (string.Equals(comboItem.GUID, connectionItem.GUID, StringComparison.OrdinalIgnoreCase))
                    {
                        dataSourcesToolStripComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            _ignoreConnectionComboBoxIndexChange = false;
        }

        /// <summary>
        /// Start build.
        /// </summary>
        private void StartBuild()
        {
            if (CurrentEditBox == null) return;

            CurrentEditBox.Enabled = false;
            statusToolStripStatusLabe.Text = "Please wait while generate the scripts";
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = true;
        }

        /// <summary>
        /// Handles the click event of the tab alias tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TabAliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var inputBox = new InputBox()
            {
                Title = "Alias",
                Prompt = "Please enter alias:",
                MaxLength = 50
            };
            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                string alias = inputBox.InputText.Trim();
                if (alias.Length > 0)
                {
                    var editBox = GetTextBoxAt(_mouseOnTabIndex);
                    if (editBox != null)
                    {
                        editBox.Alias = alias;
                        tabControl1.TabPages[_mouseOnTabIndex].Text = editBox.Title;
                        tabControl1.TabPages[_mouseOnTabIndex].ToolTipText = alias;

                        // change the text of the menu item in Windows dropdown
                        for (int i = 0; i < windowsToolStripMenuItem.DropDownItems.Count; i++)
                        {
                            if (editBox.ID == windowsToolStripMenuItem.DropDownItems[i].Tag.ToString())
                            {
                                var menuItem = windowsToolStripMenuItem.DropDownItems[i];
                                menuItem.Text = editBox.Title;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles Tab control draw item event: Draw tab with color for the selected tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Color tabColor = e.Index == tabControl1.SelectedIndex ? Color.LightBlue : SystemColors.Control;
            using Brush br = new SolidBrush(tabColor);
            e.Graphics.FillRectangle(br, e.Bounds);
            SizeF sz = e.Graphics.MeasureString(tabControl1.TabPages[e.Index].Text, e.Font);
            e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

            Rectangle rect = e.Bounds;
            rect.Offset(0, 1);
            rect.Inflate(0, -1);
            e.Graphics.DrawRectangle(Pens.DarkGray, rect);
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Handles MouseUp event of the tab control: Show context menu strip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                for (int i = 0; i < tabControl1.TabCount; ++i)
                {
                    Rectangle r = tabControl1.GetTabRect(i);
                    if (r.Contains(e.Location) /* && it is the header that was clicked*/)
                    {
                        _mouseOnTabIndex = i;
                        tabContextMenuStrip.Show(tabControl1, e.Location);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the resize event of the tab control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TabControl1_Resize(object sender, EventArgs e)
        {
            searchPanel.Top = tabControl1.Top + 36;
            searchPanel.Left = tabControl1.Left + tabControl1.Width - searchPanel.Width - 16;

            replacePanel.Top = searchPanel.Top;
            replacePanel.Left = searchPanel.Left;
        }

        /// <summary>
        /// Handles the selected index changed event of the tab control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null) return;

            if (SearchIsOpen)
            {
                CloseSearch();
                OpenSearch();
            }
            if (ReplaceIsOpen)
            {
                CloseSearch();
                OpenFindReplace();
            }
        }

        /// <summary>
        /// Handles the "Table builder form" form closing event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseAllTabs() == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            Properties.Settings.Default.LeftSplitterDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.RightSplitterDistance = defiCollapsibleSplitter.SplitterDistance;
            //Properties.Settings.Default.SchemaSettings = _settingItems.Settings;
            //Properties.Settings.Default.Templetes = templates.TemplatesValue;
            Properties.Settings.Default.Save();
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

            _connections.Load();
            await PopulateConnections();

            RestoreLastConnection();

            addDataSourceCheckBox.Checked = Properties.Settings.Default.AddDataSource;
            scriptDropsCheckBox.Checked = Properties.Settings.Default.AddDropStatement;
            if (Properties.Settings.Default.UseExtendedProperties)
                useExtendedPropertyRadioButton.Checked = true;
            else
                useUspDescRadioButton.Checked = true;

            insertBatchTextBox.Text = Properties.Settings.Default.InsertBatchRows.ToString();
            insertMaxTextBox.Text = Properties.Settings.Default.InertMaxRows.ToString();

            WindowState = FormWindowState.Maximized;
            //if (collapsibleSplitter1 != null)
            //    collapsibleSplitter1.SplitterDistance = (int)(this.Width * 0.4F);

            defiCollapsibleSplitter.SplitterDistance = Properties.Settings.Default.RightSplitterDistance;
            splitContainer1.SplitterDistance = Properties.Settings.Default.LeftSplitterDistance;

            // populate most recent used files
            PopulateMRUFiles();

            AddTab("");
        }

        /// <summary>
        /// Handles the "Table definition" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void TableDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Html) != DialogResult.Yes) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var header = HeaderText();
                if (header.Length > 0)
                {
                    CurrentEditBox?.AppendText(header + Environment.NewLine);
                }

                var template = GetTemplateText(TemplateItem.DocumentTypeEnums.SharePoint, objectName.ObjectType);

                if (string.IsNullOrWhiteSpace(template))
                {
                    Common.MsgBox($"No template found for {objectName.ObjectType} in SharePoint format.", MessageBoxIcon.Warning);
                    return;
                }

                var builder = new SharePointBuilder();
                var scripts = objectName.ObjectType switch
                {
                    ObjectTypeEnums.Table or ObjectTypeEnums.View => await builder.GetTableViewDef(objectName, _connectionString, template),
                    ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => await builder.GetFunctionProcedureDef(objectName, _connectionString, template),
                    _ => string.Empty
                };

                if (scripts.Length == 0)
                {
                    Common.MsgBox($"No definition found for {objectName.FullName}", MessageBoxIcon.Information);
                    return;
                }
                CurrentEditBox?.AppendText(scripts);

                // Move caret to end and scroll to it
                ScrollToCaret();

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
                var description = await ObjectDescription.BuildObjectDescription(objectName, _connectionString, Properties.Settings.Default.UseExtendedProperties);
                if (!string.IsNullOrEmpty(description))
                {
                    var editBox = CurrentEditBox;
                    if (editBox == null)
                    {
                        return;
                    }

                    Cursor = Cursors.WaitCursor;
                    if (BeginAddDDLScript())
                    {
                        // add GO statement if the script not empty
                        description += "GO" + Environment.NewLine;
                        editBox.AppendText(description);

                        // Move caret to end and scroll to it
                        ScrollToCaret();
                    }
                }
                else
                {
                    Common.MsgBox($"No description found for {objectName.FullName}", MessageBoxIcon.Information);
                }
            }
            else
            {
                Common.MsgBox("No database object selected.", MessageBoxIcon.Warning);
            }

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles the "Table list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void TableListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Html) != DialogResult.Yes) return;

            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            StartBuild();

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
            });

            string scripts = String.Empty;
            var builder = new SharePointBuilder();
            await Task.Run(async () =>
            {
                scripts = await builder.BuildTableListAsync(selectedObjects, _connectionString, progress);
            });

            CurrentEditBox?.AppendText(scripts);

            // Move caret to end and scroll to it
            ScrollToCaret();

            EndBuild();
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
            //    CurrentEditBox?.AppendText(builder.GetTableViewDef(objectName));
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
            Cursor = Cursors.WaitCursor;
            timer1.Stop();
            await PopulateAsync();

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles the "Create stored procedure" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void UspToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Sql) == DialogResult.Yes)
            {
                // when the current edit box is not empty, add a tab
                if (CurrentEditBox?.Text.Length > 0)
                {
                    AddTab("");
                }

                var editBox = CurrentEditBox;
                if (editBox == null)
                {
                    return;
                }

                editBox.AppendText(DatabaseDocBuilder.UspAddObjectDescription());
                // Move caret to end and scroll to it
                ScrollToCaret();
            }
        }

        /// <summary>
        /// Handles the "Value list" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void ValueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Html) != DialogResult.Yes) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;

                // check if the object is a table or view
                if (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View)
                {
                    Common.MsgBox($"The feature is for Table or View only.", MessageBoxIcon.Warning);
                    return;
                }

                var scripts = await SharePointBuilder.GetTableValuesAsync($"select * from {objectName.FullName}", _connectionString);
                CurrentEditBox?.AppendText(scripts);

                // Move caret to end and scroll to it
                ScrollToCaret();

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
        /// Handles recent file menu item click event: open selected file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowItem_Click(object? sender, EventArgs e)
        {
            if (sender == null) return;

            if (sender is ToolStripMenuItem menuItem)
            {
                var itemTag = menuItem?.Tag?.ToString();

                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    var queryTextBox = GetTextBoxAt(i);

                    if (itemTag != null && queryTextBox != null)
                    {
                        if (queryTextBox.ID == itemTag)
                        {
                            tabControl1.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the template text.
        /// </summary>
        /// <param name="docType">The doc type.</param>
        /// <param name="objType">The obj type.</param>
        /// <returns>A string.</returns>
        private static string GetTemplateText(TemplateItem.DocumentTypeEnums docType, ObjectTypeEnums objType)
        {
            string templateText = string.Empty;

            // get the template body
            TemplateItem.ObjectTypeEnums objectType = objType switch
            {
                ObjectTypeEnums.Table => TemplateItem.ObjectTypeEnums.Table,
                ObjectTypeEnums.View => TemplateItem.ObjectTypeEnums.View,
                ObjectTypeEnums.StoredProcedure => TemplateItem.ObjectTypeEnums.StoredProcedure,
                ObjectTypeEnums.Function => TemplateItem.ObjectTypeEnums.Function,
                _ => TemplateItem.ObjectTypeEnums.Table
            };

            // get the template text from the templates
            Templates templates = new();
            templates.Load();

            var templateItem = templates.GetTemplate(docType, objectType);
            if (templateItem != null)
            {
                templateText = templateItem.Body;
            }

            // if the template text is empty, show a message box and open the template editor
            if (string.IsNullOrEmpty(templateText))
            {
                Common.MsgBox($"Template for {docType} and {objType} is not defined.", MessageBoxIcon.Information);
                using var templateForm = new TemplateEditor()
                {
                    DocumentType = docType,
                    ObjectType = objectType
                };
                templateForm.ShowDialog();

                // get the template text again after editing
                templates.Load();
                templateItem = templates.GetTemplate(docType, objectType);
                if (templateItem != null)
                {
                    templateText = templateItem.Body;
                }
            }

            return templateText;
        }

        #region ScintillaNET

        /// <summary>
        /// Inits the hotkeys.
        /// </summary>
        private void InitHotkeys()
        {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(this, OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(this, OpenFindReplace, Keys.F, true, false, true);
            //HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.R, true);
            //HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.H, true);
            HotKeyManager.AddHotKey(this, Uppercase, Keys.U, true);
            HotKeyManager.AddHotKey(this, Lowercase, Keys.L, true);
            HotKeyManager.AddHotKey(this, ZoomIn, Keys.Oemplus, true);
            HotKeyManager.AddHotKey(this, ZoomOut, Keys.OemMinus, true);
            HotKeyManager.AddHotKey(this, ZoomDefault, Keys.D0, true);
            HotKeyManager.AddHotKey(this, CloseSearch, Keys.Escape);
        }

        /// <summary>
        /// Handles the text changed event of the SQL text box:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnTextChanged(object? sender, EventArgs e)
        {
            if (CurrentEditBox == null || sender == null) return;
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

        /// <summary>
        /// Handles the click event of the find and replace tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void FindAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFindReplace();
        }

        private void FindDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Hiddens the characters tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void HiddenCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // toggle view whitespace
            showWhitespaceToolStripMenuItem.Checked = !showWhitespaceToolStripMenuItem.Checked;
            if (CurrentEditBox != null)
            {
                CurrentEditBox.ViewWhitespace = showWhitespaceToolStripMenuItem.Checked
                    ? WhitespaceMode.VisibleAlways
                    : WhitespaceMode.Invisible;
            }
        }

        /// <summary>
        /// Handles the click event of the indent guides tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void IndentGuidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // toggle indent guides
            showIndentGuidesToolStripMenuItem.Checked = !showIndentGuidesToolStripMenuItem.Checked;
            if (CurrentEditBox != null)
            {
                CurrentEditBox.IndentationGuides = showIndentGuidesToolStripMenuItem.Checked
                    ? ScintillaNET.IndentView.LookBoth
                    : ScintillaNET.IndentView.None;
            }
        }

        private void IndentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Indent();
        }

        /// <summary>
        /// Handles the click event of the lowercase selection tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        /// <summary>
        /// Handles the click event of the upper case selection tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        /// <summary>
        /// Handles the click event of the zoom default tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomDefault();
        }

        /// <summary>
        /// Handles the click event of the zoom in tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// Handles the click event of the zoom out tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        private bool _ignoreConnectionComboBoxIndexChange;
        private int _mouseOnTabIndex;
        private bool _populating = false;
        private bool ReplaceIsOpen = false;
        private bool SearchIsOpen = false;
        // Indicates if the form is currently populating data sources

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
            if (ReplaceIsOpen)
            {
                ReplaceIsOpen = false;
                InvokeIfNeeded(delegate ()
                {
                    replacePanel.Visible = false;
                });
            }
        }

        /// <summary>
        /// Opens the search.
        /// </summary>
        private void OpenSearch()
        {
            if (CurrentEditBox == null) return;

            // if the replace panel is open, close it
            if (ReplaceIsOpen)
            {
                CloseSearch();
            }

            SearchManager.SearchBox = searchSQLTextBox;
            SearchManager.TextArea = CurrentEditBox;

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
        /// Handles the key down event of the search text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                if (SearchIsOpen)
                    SearchManager.Find(true, false);
                else if (ReplaceIsOpen)
                    ReplaceManager.Find(true, false);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
            {
                if (SearchIsOpen)
                    SearchManager.Find(false, false);
                else if (ReplaceIsOpen)
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
            if (SearchIsOpen)
                SearchManager.Find(true, true);
            else if (ReplaceIsOpen)
                ReplaceManager.Find(true, true);
        }

        #endregion Quick Search Bar

        #region Find & Replace Dialog

        /// <summary>
        /// Opens the find dialog.
        /// </summary>
        /// <summary>
        /// Opens the find & replace dialog.
        /// </summary>
        private void OpenFindReplace()
        {
            if (CurrentEditBox == null) return;

            if (SearchIsOpen)
                CloseSearch();

            ReplaceManager.SearchBox = replaceSearchTextBox;
            ReplaceManager.ReplaceBox = replaceReplaceTextBox;
            ReplaceManager.TextArea = CurrentEditBox;

            // If the search box is blank, fill it with the word at the caret
            if (string.IsNullOrWhiteSpace(replaceSearchTextBox.Text))
            {
                int caretPos = CurrentEditBox.CurrentPosition;
                string word = CurrentEditBox.GetWordFromPosition(caretPos);
                if (!string.IsNullOrWhiteSpace(word))
                    replaceSearchTextBox.Text = word;
            }

            if (!ReplaceIsOpen)
            {
                ReplaceIsOpen = true;
                InvokeIfNeeded(delegate ()
                {
                    replacePanel.Visible = true;
                    // Only set LastSearch if it's not blank, otherwise keep the word at caret
                    if (!string.IsNullOrWhiteSpace(ReplaceManager.LastSearch))
                        replaceSearchTextBox.Text = ReplaceManager.LastSearch;
                    replaceSearchTextBox.Focus();
                    replaceSearchTextBox.SelectAll();
                });
            }
            else
            {
                InvokeIfNeeded(delegate ()
                {
                    replaceSearchTextBox.Focus();
                    replaceSearchTextBox.SelectAll();
                });
            }
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
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Markdown) != DialogResult.Yes) return;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new MarkdownBuilder();

                    CurrentEditBox?.AppendText(builder.TextToTable(metaData));

                    // Move caret to end and scroll to it
                    ScrollToCaret();

                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Handles the click event of the query data to table tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void QueryDataToTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Markdown) != DialogResult.Yes) return;

            using var form = new QueryDataToTableForm() { ConnectionString = _connectionString };
            if (form.ShowDialog() == DialogResult.OK)
            {
                CurrentEditBox?.AppendText(await MarkdownBuilder.GetTableValuesAsync(form.SQL, _connectionString));

                // Move caret to end and scroll to it
                ScrollToCaret();
            }
        }

        /// <summary>
        /// Handles the click event of the table definition tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void TableDefinitionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Markdown) != DialogResult.Yes) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var header = HeaderText();
                if (header.Length > 0)
                {
                    CurrentEditBox?.AppendText(header + Environment.NewLine);
                }

                var template = GetTemplateText(TemplateItem.DocumentTypeEnums.Markdown, objectName.ObjectType);

                if (string.IsNullOrWhiteSpace(template))
                {
                    Common.MsgBox($"No template found for {objectName.ObjectType} in Markdown format.", MessageBoxIcon.Warning);
                    return;
                }

                var builder = new MarkdownBuilder();
                var scripts = objectName.ObjectType switch
                {
                    ObjectTypeEnums.Table or ObjectTypeEnums.View => await MarkdownBuilder.GetTableViewDef(objectName, _connectionString, template),
                    ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => await MarkdownBuilder.GetFunctionProcedureDef(objectName, _connectionString, template),
                    _ => string.Empty
                };

                if (scripts.Length == 0)
                {
                    Common.MsgBox($"No definition found for {objectName.FullName}", MessageBoxIcon.Information);
                    return;
                }
                CurrentEditBox?.AppendText(scripts);

                // Move caret to end and scroll to it
                ScrollToCaret();

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
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Markdown) != DialogResult.Yes) return;

            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            StartBuild();

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
            });

            string scripts = String.Empty;
            var builder = new MarkdownBuilder();
            await Task.Run(async () =>
            {
                scripts = await builder.BuildObjectList(selectedObjects, _connectionString, progress);
            });

            CurrentEditBox?.AppendText(scripts);

            // Move caret to end and scroll to it
            ScrollToCaret();

            EndBuild();
        }

        /// <summary>
        /// Handles the click event of the table values (MarkDown) tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void TableValuesMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Markdown) != DialogResult.Yes) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;

                // check if the object is a table or view
                if (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View)
                {
                    Common.MsgBox($"The feature is for Table or View only.", MessageBoxIcon.Warning);
                    return;
                }

                CurrentEditBox?.AppendText(await MarkdownBuilder.GetTableValuesAsync($"select * from {objectName.FullName}", _connectionString));

                // Move caret to end and scroll to it
                ScrollToCaret();

                EndBuild();
            }
        }

        #endregion Markdown document builder

        #region "MRU files"

        private MostRecentUsedFiles _mruFiles;

        /// <summary>
        /// Populate the MRU files to the menu strip
        /// </summary>
        private void PopulateMRUFiles()
        {
            if (_mruFiles == null)
            {
                _mruFiles = new MostRecentUsedFiles();
                _mruFiles.Load();
            }
            else
            {
                RemoveMRUFileMenuItems();
            }

            foreach (string item in _mruFiles?.Files)
            {
                ToolStripMenuItem fileRecent = new(item);
                fileRecent.Click += RecentFile_click;

                recentToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
            recentToolStripMenuItem.Enabled = _mruFiles.Files.Length > 0;
        }

        /// <summary>
        /// Handles recent file menu item click event: open selected file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecentFile_click(object? sender, EventArgs e)
        {
            OpenFile(sender.ToString());
        }

        /// <summary>
        /// Remove MRU file list from menu
        /// </summary>
        private void RemoveMRUFileMenuItems()
        {
            for (int i = 0; i < recentToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)recentToolStripMenuItem.DropDownItems[i];
                menuItem.Click -= RecentFile_click;
            }
            recentToolStripMenuItem.DropDownItems.Clear();
        }

        #endregion "MRU files"

        /// <summary>
        /// Handles the click event of the "WkObjectList" tool strip menu item:
        /// Generates a wiki object list for the selected objects.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void WkObjectListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Wiki) != DialogResult.Yes) return;

            List<ObjectName>? selectedObjects = SelectObjects();

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            StartBuild();

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
            });

            string scripts = String.Empty;
            var builder = new WikiBuilder();
            await Task.Run(async () =>
            {
                scripts = await builder.BuildObjectList(selectedObjects, _connectionString, progress);
            });

            CurrentEditBox?.AppendText(scripts);

            // Move caret to end and scroll to it
            ScrollToCaret();

            EndBuild();
        }

        /// <summary>
        /// Handles the click event of the "WkObjectDefinition" tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void WkObjectDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Wiki) != DialogResult.Yes) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;
                var header = HeaderText();
                if (header.Length > 0)
                {
                    CurrentEditBox?.AppendText(header + Environment.NewLine);
                }

                var template = GetTemplateText(TemplateItem.DocumentTypeEnums.Wiki, objectName.ObjectType);

                if (string.IsNullOrWhiteSpace(template))
                {
                    Common.MsgBox($"No template found for {objectName.ObjectType} in Markdown format.", MessageBoxIcon.Warning);
                    return;
                }

                var builder = new WikiBuilder();
                var scripts = objectName.ObjectType switch
                {
                    ObjectTypeEnums.Table or ObjectTypeEnums.View => await MarkdownBuilder.GetTableViewDef(objectName, _connectionString, template),
                    ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => await MarkdownBuilder.GetFunctionProcedureDef(objectName, _connectionString, template),
                    _ => string.Empty
                };

                if (scripts.Length == 0)
                {
                    Common.MsgBox($"No definition found for {objectName.FullName}", MessageBoxIcon.Information);
                    return;
                }
                CurrentEditBox?.AppendText(scripts);

                // Move caret to end and scroll to it
                ScrollToCaret();

                EndBuild();
            }
        }

        /// <summary>
        /// Wks the table view values tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void WkTableViewValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Wiki) != DialogResult.Yes) return;

            if (objectsListBox.SelectedItem != null)
            {
                var objectName = (ObjectName)objectsListBox.SelectedItem;

                // check if the object is a table or view
                if (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View)
                {
                    Common.MsgBox($"The feature is for Table or View only.", MessageBoxIcon.Warning);
                    return;
                }

                CurrentEditBox?.AppendText(await WikiBuilder.GetTableValuesAsync($"select * from {objectName.FullName}", _connectionString));

                // Move caret to end and scroll to it
                ScrollToCaret();

                EndBuild();
            }
        }

        /// <summary>
        /// Handles the click event of the "WkClipboardToTable" tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void WkClipboardToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Wiki) != DialogResult.Yes) return;

            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    var builder = new WikiBuilder();

                    CurrentEditBox?.AppendText(builder.TextToTable(metaData));

                    // Move caret to end and scroll to it
                    ScrollToCaret();

                    EndBuild();
                }
            }

            statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Handles the click event of the "WkClipboardToTable" tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void WkQueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Wiki) != DialogResult.Yes) return;

            using var form = new QueryDataToTableForm() { ConnectionString = _connectionString };
            if (form.ShowDialog() == DialogResult.OK)
            {
                CurrentEditBox?.AppendText(await WikiBuilder.GetTableValuesAsync(form.SQL, _connectionString));

                // Move caret to end and scroll to it
                ScrollToCaret();
            }
        }
    }
}