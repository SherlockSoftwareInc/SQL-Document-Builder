using ReverseMarkdown;
using ScintillaNET;
using SQL_Document_Builder.AI;
using SQL_Document_Builder.DatabaseAccess;
using SQL_Document_Builder.ScintillaNetUtils;
using SQL_Document_Builder.Template;
using SQL_Document_Builder.UI.UserControls;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        private readonly DatabaseConnections _connections = new();

        private readonly DBSchema _dbSchema = new();

        private DatabaseConnectionItem? _currentConnection;
        private List<ObjectName> _allObjects = new();
        private ObjectName? _selectedObject;
        private bool _init = false;

        private bool _aiDescriptionBusy = false;
        private ObjectName? _previousSelectedObject = null;
        private bool _ignoreObjectListSelect = false;
        private ObjectName? _openingObject;
        private CancellationTokenSource? _connectionChangeCts;

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
            //_ = new DarkMode(this);
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
        /// Gets the selected object.
        /// </summary>
        private ObjectName? SelectedObject => definitionPanel.ObjectName;

        private DatabaseConnectionItem? ActiveSchemaConnection => _dbSchema.Connection ?? _currentConnection;

        private IDatabaseAccessProvider ActiveProvider => DatabaseAccessProviderFactory.GetProvider(ActiveSchemaConnection);

        private string CurrentBatchSeparator => _currentConnection?.IsSQLServer == true ? "GO" : string.Empty;

        /// <summary>
        /// Executes the scripts.
        /// </summary>
        private static async Task<string> ExecuteScriptsAsync(DatabaseConnectionItem connection, string script)
        {
            // First, remove multi-line comments that start with /* and end with */.
            // The pattern ensures that even if the closing */ is missing, the content from /* to the
            // end of the script is removed.
            var scriptWithoutComments = Regex.Replace(script, @"/\*.*?(?:\*/|$)", "", RegexOptions.Singleline);

            bool isSqlServer = connection.IsSQLServer;

            string[] sqlStatements;

            if (isSqlServer)
            {
                // Split SQL Server script batches by GO
                sqlStatements = Regex.Split(scriptWithoutComments, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                // Split non-SQL Server scripts by ';' statement terminator
                sqlStatements = scriptWithoutComments.Split(';');
            }

            // Execute each statement
            foreach (var sql in sqlStatements)
            {
                var query = sql.Trim(' ', '\t', '\r', '\n'); // Trim whitespace

                if (!string.IsNullOrEmpty(query))
                {
                    var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                    var result = await provider.ExecuteSqlAsync(query, connection.ConnectionString);
                    if (result != string.Empty)
                    {
                        // Return immediately if an error occurs
                        return result;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// builds the display name for a file by truncating it if necessary.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>A string.</returns>
        private static string FileDisplayName(string? fileName)
        {
            if (fileName?.Length > 50)
            {
                // Show the first 7 chars, then "...", then the last 40 chars
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
        private async Task<string> GetObjectCreateScriptAsync(ObjectName objectName, DatabaseConnectionItem? connection, DBSchema? schemaCache = null)
        {
            if (connection == null || string.IsNullOrEmpty(objectName.FullName))
            {
                return string.Empty; // If connection is null or object name is empty, return an empty string
            }

            var effectiveSchemaCache = schemaCache ?? _dbSchema;

            string? createScript = string.Empty;
            if (objectName != null)
            {
                createScript = await DatabaseDocBuilder.GetCreateObjectScriptAsync(objectName, effectiveSchemaCache);
            }

            if (string.IsNullOrEmpty(createScript))
            {
                return string.Empty;
            }

            // remove the space and new line at the end of the script
            createScript = createScript.TrimEnd([' ', '\r', '\n', '\t']);

            bool isSqlServer = connection.IsSQLServer;

            string batchSeparator = isSqlServer ? "GO" : "";

            // add batch separator at the end of the script if it doesn't exist
            if (!createScript.EndsWith(batchSeparator, StringComparison.CurrentCultureIgnoreCase))
            {
                createScript += Environment.NewLine + batchSeparator + Environment.NewLine;
            }
            else
            {
                createScript += Environment.NewLine;
            }

            // Build description script when supported by the active DBMS.
            // ObjectDescription.BuildObjectDescription handles DBMS-specific script generation internally.
            try
            {
                var description = await ObjectDescription.BuildObjectDescription(objectName, effectiveSchemaCache, Properties.Settings.Default.UseExtendedProperties);
                if (description.Length > 0)
                {
                    // append the description to the script
                    createScript += description;
                    createScript += batchSeparator + Environment.NewLine;
                }
            }
            catch
            {
                // Ignore description script failures; CREATE script should still be returned.
            }

            return createScript;
        }

        /// <summary>
        /// Perform the syntax check for the given script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>A Task.</returns>
        private static async Task<string> SyntaxCheckAsync(string script, string connectionString = "")
        {
            var sqlStatements = Regex.Split(script, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // execute each statement
            foreach (var sql in sqlStatements)
            {
                //Execute(builder.ConnectionString, sql);
                if (sql.Length > 0)
                {
                    var result = await SQLDatabaseHelper.SyntaxCheckAsync(sql, connectionString);
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
        /// <param name="e">The event arguments.</param>
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
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            bool isOdbc = dlg.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase);

            var connection = new DatabaseConnectionItem()
            {
                Name = dlg.ConnectionName,
                ConnectionType = isOdbc ? "ODBC" : "SQL Server",
                DatabaseDescription = dlg.DatabaseDescription,
                UserName = dlg.UserName,
                Password = dlg.Password,
                RememberPassword = dlg.RememberPassword,
                DBMSType = dlg.DBMSType,
                RequireManualLogin = dlg.RequireManualLogin
            };

            if (isOdbc)
            {
                connection.DSN = dlg.DSN;
                connection.ServerName = dlg.DSN;
                connection.Database = string.Empty;
            }
            else
            {
                connection.ServerName = dlg.ServerName;
                connection.Database = dlg.DatabaseName;
                connection.AuthenticationType = dlg.Authentication;
                connection.EncryptConnection = dlg.EncryptConnection;
                connection.TrustServerCertificate = dlg.TrustServerCertificate;
            }

            connection.BuildConnectionString();
            _connections.Add(connection);
            _connections.Save();

            dataSourcesToolStripComboBox.Items.Add(connection);

            var submenuitem = new ConnectionMenuItem(connection)
            {
                Name = ConnectionMenuItemName(),
                Size = new Size(300, 26),
            };
            submenuitem.Click += new System.EventHandler(this.OnConnectionToolStripMenuItem_Click);
            connectToToolStripMenuItem.DropDown.Items.Add(submenuitem);

            await ChangeDBConnectionAsync(connection);

            return true;
        }

        /// <summary>
        /// builds a unique name for the connection menu item.
        /// </summary>
        /// <returns>A string.</returns>
        private string ConnectionMenuItemName()
        {
            int connectionCount = _connections.Connections.Count;
            string name = $"ConnectionMenuItem{connectionCount}";

            while (true)
            {
                if (connectToToolStripMenuItem.DropDown.Items.Cast<ToolStripMenuItem>().All(item => item.Name != name))
                {
                    break; // Found a unique name
                }
                connectionCount++;
                name = $"ConnectionMenuItem{connectionCount}";
            }

            return name;
        }

        /// <summary>
        /// Adds the data source tag to the document.
        /// </summary>
        private void AddDataSourceText()
        {
            if (GetCurrentEditBox(out SqlEditBox editBox) == false)
            {
                return; // If we can't get the edit box, exit early
            }

            // add the data source tag to the document when the document is empty
            if (Properties.Settings.Default.AddDataSource)
            {
                string dataSourceText = $"{serverToolStripStatusLabel.Text}::{databaseToolStripStatusLabel.Text}";
                if (string.IsNullOrEmpty(editBox?.Text) || editBox.DataSourceName != dataSourceText)
                {
                    editBox.DataSourceName = dataSourceText;
                    AppendText(editBox, $"-- Data source: {dataSourceText}" + Environment.NewLine);
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
        private SqlEditBox? AddTab(string fileName)
        {
            // number of tabs is limited to 128
            if (tabControl1.TabPages.Count >= 128)
            {
                MessageBox.Show("You can open up to 128 editing windows.", "Too many editing windows",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            var queryTextBox = GetNewTextBox(!(fileName.Length == 0));

            if (fileName.Length > 0)
            {
                queryTextBox.OpenFile(fileName);
            }

            AddWindowsMenuItem(queryTextBox.FileName?.Length == 0 ? tabControl1.SelectedTab.Text : FileDisplayName(queryTextBox.FileName), queryTextBox.ID, tabControl1.SelectedTab.ToolTipText);

            queryTextBox.Focus();

            return queryTextBox;
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
        /// Appends the text.
        /// </summary>
        /// <param name="editBox">The edit box.</param>
        /// <param name="text">The text.</param>
        private void AppendText(SqlEditBox? editBox, string text)
        {
            try
            {
                if (editBox == null || string.IsNullOrEmpty(text)) return;
                editBox.BeginUndoAction();
                editBox.AppendText(text);
                editBox.EndUndoAction();

                // Move caret to end and scroll to it
                ScrollToCaret();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Handles the "Batch column description" tool strip button click event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BatchToolStripButton_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            using var frm = new BatchColumnDesc()
            {
                ConnectionString = connectionString,
                SchemaCache = _dbSchema
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
        private async Task ChangeDBConnectionAsync(DatabaseConnectionItem connection)
        {
            var previousConnection = _currentConnection;

            if (connection == null)
            {
                await OpenConnectionAsync(null, CancellationToken.None);
                SetConnectionMenuChecked(null);
                return;
            }

            bool isOdbc = connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase);

            if (isOdbc && !TrySelectOdbcDbqFile(connection))
            {
                connection.ConnectionString = string.Empty;
                connection.LoginSucceed = false;

                if (previousConnection != null && !string.IsNullOrWhiteSpace(previousConnection.ConnectionString))
                {
                    await OpenConnectionAsync(previousConnection, CancellationToken.None);
                    SetConnectionComboBox(previousConnection);
                    SetConnectionMenuChecked(previousConnection);
                }
                else
                {
                    await OpenConnectionAsync(null, CancellationToken.None);
                    SetConnectionMenuChecked(null);
                }

                return;
            }

            bool isDifferentConnection = previousConnection == null || previousConnection.ConnectionID != connection.ConnectionID;
            if (isDifferentConnection)
            {
                CancelPendingSchemaLoad();
                _connectionChangeCts = new CancellationTokenSource();
                connection.LoginSucceed = false;
            }

            var cancellationToken = _connectionChangeCts?.Token ?? CancellationToken.None;

            try
            {
                if (!connection.LoginSucceed)
                {
                    bool needsLogin = string.IsNullOrWhiteSpace(connection.ConnectionString) || (isOdbc && connection.RequireManualLogin);
                    var loginConnectionString = needsLogin ? await connection.Login() : connection.ConnectionString;
                    connection.LoginSucceed = !string.IsNullOrWhiteSpace(loginConnectionString);

                    if (connection.LoginSucceed)
                    {
                        connection.ConnectionString = loginConnectionString;
                    }
                }

                if (!connection.LoginSucceed || string.IsNullOrWhiteSpace(connection.ConnectionString))
                {
                    await OpenConnectionAsync(null, cancellationToken);
                    SetConnectionMenuChecked(null);
                    return;
                }

                bool opened = await OpenConnectionAsync(connection, cancellationToken);
                if (!opened)
                {
                    connection.LoginSucceed = false;
                    connection.ConnectionString = string.Empty;

                    if (previousConnection != null && !string.IsNullOrWhiteSpace(previousConnection.ConnectionString))
                    {
                        await OpenConnectionAsync(previousConnection, cancellationToken);
                        SetConnectionComboBox(previousConnection);
                        SetConnectionMenuChecked(previousConnection);
                    }
                    else
                    {
                        await OpenConnectionAsync(null, cancellationToken);
                        SetConnectionMenuChecked(null);
                    }

                    Common.MsgBox("Failed to connect to the database. Please check your connection settings.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SetConnectionMenuChecked(connection);
                Properties.Settings.Default.LastAccessConnection = connection.ConnectionID.ToString();
                Properties.Settings.Default.Save();
            }
            catch (OperationCanceledException)
            {
                // Ignore canceled connection changes.
            }
        }

        private void CancelPendingSchemaLoad()
        {
            if (_connectionChangeCts != null)
            {
                _connectionChangeCts.Cancel();
                _connectionChangeCts.Dispose();
                _connectionChangeCts = null;
            }
        }

        private void ResetSchemaCacheState()
        {
            _dbSchema.Clear();
            _tables = [];
            _allObjects = [];
            definitionPanel.SchemaCache = null;
        }

        private async Task<bool> OpenConnectionAsync(DatabaseConnectionItem? connection, CancellationToken cancellationToken)
        {
            if (connection == null) return false;

            connection.BuildConnectionString();

            UpdateSqlServerSpecificMenuItems(connection);

            serverToolStripStatusLabel.Text = string.Empty;
            databaseToolStripStatusLabel.Text = string.Empty;
            objectsListBox.Items.Clear();
            messageLabel.Text = string.Empty;
            ResetSchemaCacheState();

            await definitionPanel.OpenAsync(null, null);

            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                _currentConnection = null;
                StringExtensions.SetIdentifierQuoteDbms(null);
                return false;
            }

            bool isOdbc = connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase);
            string endpoint = isOdbc
                ? (connection.DSN ?? connection.ServerName ?? connection.Name ?? string.Empty)
                : (connection.ServerName ?? string.Empty);
            string database = isOdbc ? "ODBC" : (connection.Database ?? string.Empty);

            serverToolStripStatusLabel.Text = $"Connect to {endpoint}...";

            cancellationToken.ThrowIfCancellationRequested();

            var provider = DatabaseAccessProviderFactory.GetProvider(connection);
            bool canConnect = await provider.TestConnectionAsync(connection.ConnectionString, cancellationToken);

            if (!canConnect)
            {
                serverToolStripStatusLabel.Text = $"Unable to connect to {endpoint}";
                databaseToolStripStatusLabel.Text = database;
                return false;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var schemaLoaded = await _dbSchema.OpenAsync(connection, loadColumns: true, cancellationToken);
            if (!schemaLoaded)
            {
                // Fallback: keep connection usable even if eager column preload fails.
                schemaLoaded = await _dbSchema.OpenAsync(connection, loadColumns: false, cancellationToken);
            }

            if (!schemaLoaded)
            {
                serverToolStripStatusLabel.Text = $"Unable to load metadata from {endpoint}";
                databaseToolStripStatusLabel.Text = database;
                return false;
            }

            _allObjects = _dbSchema.AllObjects();

            cancellationToken.ThrowIfCancellationRequested();

            _currentConnection = connection;
            StringExtensions.SetIdentifierQuoteDbms(_currentConnection);
            definitionPanel.SchemaCache = _dbSchema;
            Properties.Settings.Default.dbConnectionString = connection.ConnectionString;
            serverToolStripStatusLabel.Text = endpoint;
            databaseToolStripStatusLabel.Text = database;

            await definitionPanel.OpenAsync(null, _currentConnection);

            return true;
        }

        private void UpdateSqlServerSpecificMenuItems(DatabaseConnectionItem? connection)
        {
            bool isSqlServer = connection != null && connection.IsSQLServer;
            uspToolStripMenuItem.Visible = isSqlServer;
            toolStripSeparator14.Visible = isSqlServer;
        }

        private bool TrySelectOdbcDbqFile(DatabaseConnectionItem connection)
        {
            if (!connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(connection.FileFilter))
            {
                return true;
            }

            using var openFileDialog = new OpenFileDialog
            {
                Filter = connection.FileFilter,
                Multiselect = false,
                CheckFileExists = true,
                Title = "Select ODBC source file"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                connection.ConnectionString = string.Empty;
                return false;
            }

            if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                connection.BuildConnectionString();
            }

            var builder = string.IsNullOrWhiteSpace(connection.ConnectionString)
                ? new OdbcConnectionStringBuilder()
                : new OdbcConnectionStringBuilder(connection.ConnectionString);

            if (string.IsNullOrWhiteSpace(builder.Dsn))
            {
                builder.Dsn = connection.DSN ?? connection.ServerName ?? connection.Name ?? string.Empty;
            }

            builder["Dbq"] = openFileDialog.FileName;
            connection.ConnectionString = builder.ConnectionString;

            return true;
        }

        private void SetConnectionMenuChecked(DatabaseConnectionItem? connection)
        {
            for (int i = 0; i < connectToToolStripMenuItem.DropDown.Items.Count; i++)
            {
                if (connectToToolStripMenuItem.DropDown.Items[i] is ConnectionMenuItem submenuitem)
                {
                    submenuitem.Checked = connection != null && submenuitem.Connection.ConnectionID == connection.ConnectionID;
                }
            }
        }

        /// <summary>
        /// Checks the current document type.
        /// </summary>
        /// <returns>A DialogResult.</returns>
        private DialogResult CheckCurrentDocumentType()
        {
            SqlEditBox.DocumentTypeEnums docType = docTypeToolStripComboBox.Text.ToUpper() switch
            {
                "SHAREPOINT" => SqlEditBox.DocumentTypeEnums.Html,
                "MARKDOWN" => SqlEditBox.DocumentTypeEnums.Markdown,
                "WIKI" => SqlEditBox.DocumentTypeEnums.Wiki,
                _ => SqlEditBox.DocumentTypeEnums.Text,
            };
            return CheckCurrentDocumentType(docType);
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
        /// <param name="e">The event arguments.</param>
        private void ClearSerachButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
            searchTextBox.Focus();
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the "Close" tool strip menu item click event:
        /// Close the mouse on tab
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.Copy();
            }
            else if (focusedControl is SqlEditBox editBox)
            {
                editBox.Copy();
            }
            else if (focusedControl is ListBox listBox)
            {
                if (listBox.SelectedItem != null)
                {
                    Clipboard.SetText(listBox.SelectedItem.ToString() ?? string.Empty);
                }
            }
            else if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is in edit mode, copy from the editing control (usually a TextBox)
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    if (dataGridView.EditingControl is TextBox editingTextBox)
                    {
                        editingTextBox.Copy();
                        return;
                    }
                }
                // Otherwise, copy the selected cells as a DataObject (tabular, for pasting into Excel, etc.)
                var clipboardContent = dataGridView.GetClipboardContent();
                if (clipboardContent != null)
                {
                    Clipboard.SetDataObject(clipboardContent);
                }
                // If nothing is selected, fallback to copying the current cell's value as text
                else
                {
                    var value = dataGridView.CurrentCell?.Value?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        Clipboard.SetText(value);
                    }
                }
            }
            else
            {
                messageLabel.Text = "No valid control is focused for copying.";
            }
        }

        /// <summary>
        /// Handles the "create index" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            if (BeginAddDDLScript())
            {
                AppendText(editBox, definitionPanel.CreateIndexScript());
                CopyToClipboard(editBox);
            }
        }

        /// <summary>
        /// Handles the "create insert" tool strip menu item click:
        /// create insert statement for the selected objects and selected column
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void CreateInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            List<ObjectName>? selectedObjects = SelectObjects(false);

            if (selectedObjects == null || selectedObjects.Count == 0) { return; }

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            Cursor = Cursors.WaitCursor;
            if (BeginAddDDLScript())
            {
                StartBuild(editBox);

                progressBar.Value = 0;
                messageLabel.Text = "Preparing scripts...";
                await Task.Yield();

                var scriptBuilder = new StringBuilder();
                var provider = ActiveProvider;
                IProgress<int> progress = new Progress<int>(percent =>
                {
                    progressBar.Value = Math.Clamp(percent, 0, 100);
                    messageLabel.Text = $"Processing {percent}%...";
                });

                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    var obj = selectedObjects[i];

                    // get the object create script
                    var createScript = await GetObjectCreateScriptAsync(obj, _currentConnection);
                    if (!string.IsNullOrEmpty(createScript))
                    {
                        scriptBuilder.Append(createScript);
                    }

                    // get the insert statement for the object
                    var rowCount = await provider.GetRowCountAsync(obj.FullNameNoQuote, connectionString);

                    if (rowCount > Properties.Settings.Default.InertMaxRows)
                    {
                        scriptBuilder.Append("-- Too many rows to insert").AppendLine().AppendLine();
                    }
                    else
                    {
                        var insertScript = await DatabaseDocBuilder.TableToInsertStatementAsync(obj, _dbSchema);
                        if (!string.IsNullOrEmpty(insertScript))
                        {
                            scriptBuilder.Append(insertScript);

                            if (!string.IsNullOrEmpty(CurrentBatchSeparator))
                            {
                                scriptBuilder.Append(CurrentBatchSeparator).AppendLine();
                            }
                        }
                    }

                    progress.Report(((i + 1) * 100) / selectedObjects.Count);
                }

                if (editBox == null) return;
                AppendText(editBox, scriptBuilder.ToString());
            }

            EndBuild(editBox);
        }

        /// <summary>
        /// Gets the connection string from the current connection.
        /// Returns true if a valid connection string is obtained, otherwise false.
        /// </summary>
        /// <param name="connectionString">The output connection string.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool GetConnectionString(out string connectionString)
        {
            connectionString = string.Empty;

            // Check if there is a current connection
            if (_currentConnection == null)
            {
                Common.MsgBox("Please connect to a database first.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // Build the connection string if it is missing or empty
            if (string.IsNullOrWhiteSpace(_currentConnection.ConnectionString))
            {
                _currentConnection.BuildConnectionString();
            }

            connectionString = _currentConnection.ConnectionString ?? string.Empty;

            // Return true if the connection string is not null or empty
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        /// <summary>
        /// handles the "create primary key" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CreatePrimaryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            if (BeginAddDDLScript())
            {
                AppendText(editBox, definitionPanel.PrimaryKeyScript());
                CopyToClipboard(editBox);
            }
        }

        /// <summary>
        /// Handles the "create table" tool strip button click:
        /// Generate the CREATE script for the selected object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void CreateTableToolStripButton_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (SelectedObject == null || SelectedObject?.ObjectType == ObjectTypeEnums.None) return;

            Cursor = Cursors.WaitCursor;
            try
            {
                var script = await GetObjectCreateScriptAsync(SelectedObject, _currentConnection);

                if (!string.IsNullOrEmpty(script))
                {
                    if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                    if (!BeginAddDDLScript()) return;

                    AddDataSourceText();

                    AppendText(editBox, script);
                    CopyToClipboard(editBox);
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox($"Failed to generate CREATE script for {SelectedObject?.FullName}: {ex.Message}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the "Create" tool strip menu item click event.
        /// Batch generate the CREATE scripts for the selected objects
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void CreateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            List<ObjectName>? selectedObjects = SelectObjects(false);

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            Cursor = Cursors.WaitCursor;
            if (BeginAddDDLScript())
            {
                StartBuild(editBox);

                progressBar.Value = 0;
                messageLabel.Text = "Preparing scripts...";

                // Let UI repaint before starting heavy async work.
                await Task.Yield();

                IProgress<int> progress = new Progress<int>(percent =>
                {
                    progressBar.Value = Math.Clamp(percent, 0, 100);
                    messageLabel.Text = $"Processing {percent}%...";
                    //Application.DoEvents();
                });

                var scripts = await BuildCreateScriptsAsync(selectedObjects, _currentConnection, progress);

                if (editBox == null) return;

                var output = string.Concat(scripts);
                if (!string.IsNullOrEmpty(output))
                {
                    AppendText(editBox, output);
                }
            }

            EndBuild(editBox);
        }

        private async Task<string[]> BuildCreateScriptsAsync(IReadOnlyList<ObjectName> selectedObjects, DatabaseConnectionItem? connection, IProgress<int>? progress)
        {
            var scripts = new string[selectedObjects.Count];
            if (selectedObjects.Count == 0)
            {
                progress?.Report(100);
                return scripts;
            }

            int completed = 0;
            int maxParallel = Math.Min(4, Math.Max(1, Environment.ProcessorCount / 2));
            using var semaphore = new SemaphoreSlim(maxParallel, maxParallel);

            var tasks = selectedObjects.Select(async (obj, index) =>
            {
                await semaphore.WaitAsync();
                try
                {
                    scripts[index] = await GetObjectCreateScriptAsync(obj, connection, _dbSchema);
                }
                finally
                {
                    semaphore.Release();
                    int done = Interlocked.Increment(ref completed);
                    progress?.Report((done * 100) / selectedObjects.Count);
                }
            });

            await Task.WhenAll(tasks);
            return scripts;
        }

        /// <summary>
        /// Handles the "Cut" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
            else if (focusedControl is SqlEditBox editBox)
            {
                editBox.Cut();
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.Cut();
            }
            else if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is active, cut the current cell's value
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    if (dataGridView.EditingControl is TextBox editingTextBox && editingTextBox.Enabled)
                    {
                        editingTextBox.Cut();
                    }
                }
            }
            else
            {
                messageLabel.Text = "No valid control is focused for cutting.";
            }
        }

        /// <summary>
        /// Handles data source combo box selected index change event: Change the data source
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void DataSourcesToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataSourcesToolStripComboBox.SelectedItem != null && !_ignoreConnectionComboBoxIndexChange)
            {
                if (dataSourcesToolStripComboBox.SelectedItem is DatabaseConnectionItem selectedItem)
                {
                    await SwitchConnectionAsync(selectedItem, updateComboBoxSelection: false, sender, e);
                }

                messageLabel.Text = "";
            }
        }

        /// <summary>
        /// Docs the type tool strip combo box_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DocTypeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if selected item is not null, save the settings
            if (docTypeToolStripComboBox.SelectedItem != null)
            {
                Properties.Settings.Default.DocumentType = docTypeToolStripComboBox.SelectedItem.ToString();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Edits the box_ file name changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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

                // Enable the "Open Folder" menu item if the file name is not empty
                openFolderToolStripMenuItem.Enabled = !string.IsNullOrEmpty(CurrentEditBox?.FileName);

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
                var mruFiles = new MostRecentUsedFiles();
                mruFiles.Load();

                mruFiles.AddFile(editBox.FileName);
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
            mdValuesToolStripMenuItem.Enabled = enabled;
            insertToolStripButton.Enabled = enabled;
            insertToolStripMenuItem.Enabled = enabled;
            jsonTableViewValuesToolStripMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// End build.
        /// </summary>
        private void EndBuild(SqlEditBox editBox)
        {
            progressBar.Visible = false;
            messageLabel.Text = "Complete!";
            this.Cursor = Cursors.Default;
            if (editBox != null)
            {
                editBox.Enabled = true;
                editBox.Cursor = Cursors.Default;

                CopyToClipboard(editBox);

                if (editBox == CurrentEditBox)
                {
                    // If the current edit box is the one we just built, set focus to it
                    editBox.Focus();
                }
            }
        }

        /// <summary>
        /// Handles the "Excel to INSERT" tool strip menu item click:
        ///     Load Excel file and generate INSERT statements for the data in the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void ExcelToINSERTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Sql) == DialogResult.Yes)
            {
                // get the source file name
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Data files (*.xls;*.xlsx;*.csv;*.txt)|*.xls;*.xlsx;*.csv;*.txt|Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var fileName = openFileDialog.FileName;

                var extension = Path.GetExtension(fileName);
                DataTable? resultDataTable = null;
                string tableName = Path.GetFileNameWithoutExtension(fileName);
                bool nullForBlank = false;

                if (extension.Equals(".xls", StringComparison.OrdinalIgnoreCase) || extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    using var form = new ExcelSheetsForm()
                    {
                        FileName = fileName
                    };
                    form.ShowDialog();

                    resultDataTable = form.ResultDataTable;
                    tableName = string.IsNullOrWhiteSpace(form.TableName) ? tableName : form.TableName;
                    nullForBlank = form.NullForBlank;
                }
                else if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase) || extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    resultDataTable = LoadInsertDataFromDelimitedFile(fileName);
                }

                if (resultDataTable != null)
                {
                    if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                    Cursor = Cursors.WaitCursor;
                    StartBuild(editBox);

                    progressBar.Value = 0;
                    messageLabel.Text = "Generating INSERT statements...";
                    await Task.Yield();

                    AppendText(editBox, $"-- Data source: {fileName}" + Environment.NewLine);

                    var dataHelper = new ExcelDataHelper(resultDataTable);
                    var connection = _currentConnection;
                    var insertScript = await Task.Run(() => dataHelper.GetInsertStatement(tableName, nullForBlank, connection));

                    AppendText(editBox, insertScript);

                    EndBuild(editBox);
                }
            }
        }

        private static DataTable? LoadInsertDataFromDelimitedFile(string fileName)
        {
            try
            {
                var table = LoadDescriptionDataFromCsv(fileName);
                if (table != null)
                {
                    table.TableName = Path.GetFileNameWithoutExtension(fileName);
                }
                return table;
            }
            catch (Exception ex)
            {
                Common.MsgBox($"Error reading file: {ex.Message}", MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Handles the click event of the find next button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void FindNextButton_Click(object sender, EventArgs e)
        {
            ReplaceManager.Find(true, false);
        }

        /// <summary>
        /// Gets the current edit box.
        /// </summary>
        /// <param name="editBox">The edit box.</param>
        /// <returns>A bool.</returns>
        private bool GetCurrentEditBox(out SqlEditBox editBox)
        {
            // if no tabs, create a new tab with an empty edit box
            if (tabControl1.TabCount == 0)
            {
                var newEditor = AddTab(string.Empty);
                if (newEditor == null)
                {
                    editBox = new SqlEditBox();
                    return false;
                }
                else
                {
                    editBox = newEditor;
                    return true;
                }
            }

            // if no tab is selected, select the last tab
            if (tabControl1.SelectedTab == null)
            {
                tabControl1.SelectedIndex = tabControl1.TabCount - 1;
            }

            if (CurrentEditBox == null)
            {
                MessageBox.Show("No active edit box found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                editBox = new SqlEditBox();
                return false;
            }
            editBox = CurrentEditBox;
            return true;
        }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        /// <returns>A string.</returns>
        private string GetDocumentType()
        {
            return docTypeToolStripComboBox.Text;
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
                TabIndex = _tabNo,
                DarkMode = Properties.Settings.Default.DarkMode,
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
        /// Gets the template with selected document type.
        /// </summary>
        /// <returns>A Template.Template? .</returns>
        private Template.Template? GetTemplate()
        {
            var docType = GetDocumentType();
            if (string.IsNullOrEmpty(docType))
            {
                Common.MsgBox("Please select a document type first.", MessageBoxIcon.Information);
                return null;
            }

            // load the templates
            var templates = new Templates();
            templates.Load();

            // find the template for the document type
            var template = templates.GetTemplate(docType);

            return template;
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
        private async void InsertToolStripButton_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (SelectedObject == null || SelectedObject?.ObjectType == ObjectTypeEnums.None) return;

            if (SelectedObject is ObjectName objectName)
            {
                // return if object type is not a table or view
                if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table || objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
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
                    var provider = ActiveProvider;
                    var rowCount = await provider.GetRowCountAsync(objectName.FullName, connectionString);

                    // confirm if the user wants to continue when the number of rows is too much
                    if (rowCount > 1000)
                    {
                        if (Common.MsgBox($"The table has {rowCount} rows. Are you sure you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    Cursor = Cursors.WaitCursor;

                    // checks if the table has identify column
                    //var hasIdentityColumn = await DatabaseHelper.HasIdentityColumnAsync(objectName);

                    var script = await DatabaseDocBuilder.TableToInsertStatementAsync(objectName, _dbSchema);

                    if (script == "Too much rows")
                    {
                        Common.MsgBox("Too much rows to insert", MessageBoxIcon.Error);
                        return;
                    }
                    else if (!string.IsNullOrEmpty(script))
                    {
                        if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                        if (!BeginAddDDLScript()) return;

                        // append the insert statement to the script
                        AppendText(editBox, script);

                        if (!string.IsNullOrEmpty(CurrentBatchSeparator))
                        {
                            AppendText(editBox, CurrentBatchSeparator + Environment.NewLine);
                        }
                        CopyToClipboard(editBox);
                    }
                    else
                    {
                        Common.MsgBox("No data found", MessageBoxIcon.Information);
                    }
                }
            }
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles the "Manage connections" tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
        private void ManageTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var templateEditor = new TemplateEditor() { CurrentDocType = docTypeToolStripComboBox.Text };
            templateEditor.ShowDialog(this);
            PopulateDocumentType();
        }

        /// <summary>
        /// Handles the "New connection" tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTab("");
        }

        /// <summary>
        /// Handles the "Objects description" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void ObjectsDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            List<ObjectName>? selectedObjects = SelectObjects(false);

            if (selectedObjects == null || selectedObjects.Count == 0) return;

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            Cursor = Cursors.WaitCursor;
            if (BeginAddDDLScript())
            {
                StartBuild(editBox);

                progressBar.Value = 0;
                messageLabel.Text = "Preparing descriptions...";
                await Task.Yield();

                IProgress<int> progress = new Progress<int>(percent =>
                {
                    progressBar.Value = Math.Clamp(percent, 0, 100);
                    messageLabel.Text = $"Processing {percent}%...";
                });

                var scriptBuilder = new StringBuilder();
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    var obj = selectedObjects[i];
                    var script = await ObjectDescription.BuildObjectDescription(obj, _dbSchema, Properties.Settings.Default.UseExtendedProperties);

                    // add "GO" and new line after each object description if it is not empty
                    if (!string.IsNullOrEmpty(script))
                    {
                        scriptBuilder.Append(script);

                        if (!string.IsNullOrEmpty(CurrentBatchSeparator))
                        {
                            scriptBuilder.Append(CurrentBatchSeparator).AppendLine();
                        }
                    }

                    progress.Report(((i + 1) * 100) / selectedObjects.Count);
                }

                if (editBox == null) return;
                AppendText(editBox, scriptBuilder.ToString());
            }

            EndBuild(editBox);
        }

        /// <summary>
        /// Handles the "Objects list" list box selected index changed event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void ObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreObjectListSelect) return;

            messageLabel.Text = string.Empty;

            if (_aiDescriptionBusy)
            {
                var result = Common.MsgBox(
                    "AI description building is in progress. Do you want to cancel and switch to the new object?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    // Revert selection to previous object
                    if (_previousSelectedObject != null)
                    {
                        for (int i = 0; i < objectsListBox.Items.Count; i++)
                        {
                            if (objectsListBox.Items[i] is ObjectName obj && obj.Equals(_previousSelectedObject))
                            {
                                _ignoreObjectListSelect = true;
                                objectsListBox.SelectedIndex = i;
                                _ignoreObjectListSelect = false;
                                // Do NOT set _aiDescriptionBusy here; AI may have completed.
                                return;
                            }
                        }
                    }
                    return;
                }
                else
                {
                    // Cancel AI processing (implement cancellation logic if needed)
                    // For now, just set busy to false
                    _aiDescriptionBusy = false;
                    EnableDisableUI(true);
                }
            }

            // Track previous selection
            if (objectsListBox.SelectedItem is ObjectName selectedObj)
            {
                if ((_openingObject != null && selectedObj.Equals(_openingObject)) ||
                    (_selectedObject != null && selectedObj.Equals(_selectedObject) &&
                     definitionPanel.ObjectName != null && selectedObj.Equals(definitionPanel.ObjectName)))
                {
                    return;
                }

                _previousSelectedObject = _selectedObject;
                _selectedObject = selectedObj;
                _openingObject = selectedObj;

                try
                {
                    await definitionPanel.OpenAsync(_selectedObject, _currentConnection);
                }
                finally
                {
                    if (_openingObject != null && _openingObject.Equals(selectedObj))
                    {
                        _openingObject = null;
                    }
                }

                // trun off AI description if it is not table or view
                if (_selectedObject.ObjectType != ObjectName.ObjectTypeEnums.Table && _selectedObject.ObjectType != ObjectName.ObjectTypeEnums.View)
                {
                    aIAssistantToolStripMenuItem.Enabled = false;
                }
                else
                {
                    aIAssistantToolStripMenuItem.Enabled = true;
                }
            }
            else
            {
                if (definitionPanel.ObjectName == null)
                {
                    return;
                }

                await definitionPanel.OpenAsync(null, _currentConnection);
                aIAssistantToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Handles the "ObjectType" combo box selected index changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ObjectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init || _currentConnection == null || !_dbSchema.HasObjects) return;

            // keep the selected schema
            string schemaName = schemaComboBox.Text;

            if (objectTypeComboBox.SelectedIndex >= 0)
            {
                // get the database object by the selected object type
                switch (objectTypeComboBox.SelectedIndex)
                {
                    case 0:
                        _tables = _dbSchema.Objects(ObjectName.ObjectTypeEnums.Table);
                        EnableTableValue(true);
                        break;

                    case 1:
                        _tables = _dbSchema.Objects(ObjectName.ObjectTypeEnums.View);
                        EnableTableValue(true);
                        break;

                    case 2:
                        _tables = _dbSchema.Objects(ObjectName.ObjectTypeEnums.StoredProcedure);
                        EnableTableValue(false);
                        break;

                    case 3:
                        _tables = _dbSchema.Objects(ObjectName.ObjectTypeEnums.Function);
                        EnableTableValue(false);
                        break;

                    case 4:
                        _tables = _dbSchema.Objects(ObjectName.ObjectTypeEnums.Trigger);
                        EnableTableValue(false);
                        break;

                    case 5:
                        _tables = _dbSchema.Objects(ObjectName.ObjectTypeEnums.Synonym);
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
        /// <param name="e">The event arguments.</param>
        private async void OnConnectionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender?.GetType() == typeof(ConnectionMenuItem))
            {
                ConnectionMenuItem menuItem = (ConnectionMenuItem)sender;
                await SwitchConnectionAsync(menuItem.Connection, updateComboBoxSelection: true, sender, e);
            }
        }

        private async Task SwitchConnectionAsync(DatabaseConnectionItem connection, bool updateComboBoxSelection, object? sender, EventArgs e)
        {
            messageLabel.Text = string.Format("Connect to {0}...", connection.ToString());
            Cursor = Cursors.WaitCursor;

            _populating = true;

            try
            {
                // keep the current selection in the combo boxes
                int selectedObjectTypeIndex = objectTypeComboBox.SelectedIndex;

                // clean up the object list box
                objectsListBox.Items.Clear();

                await ChangeDBConnectionAsync(connection);

                if (updateComboBoxSelection && !_ignoreConnectionComboBoxIndexChange)
                    SetConnectionComboBox(connection);

                if (_currentConnection != null)
                {
                    // restore the object type
                    int objectTypeIndex = selectedObjectTypeIndex;
                    if (objectTypeIndex < 0 && objectTypeComboBox.Items.Count > 0)
                    {
                        objectTypeIndex = 0; // default to the first item
                    }

                    if (objectTypeIndex >= 0)
                    {
                        if (objectTypeComboBox.SelectedIndex == objectTypeIndex)
                        {
                            ObjectTypeComboBox_SelectedIndexChanged(sender ?? this, e); // re-populate the object list box
                        }
                        else
                        {
                            objectTypeComboBox.SelectedIndex = objectTypeIndex;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                messageLabel.Text = "Connection change was canceled.";
            }
            catch (Exception ex)
            {
                messageLabel.Text = "Connection change failed.";
                Common.MsgBox($"Failed to change database connection: {ex.Message}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _populating = false;
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the "Execute " tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnExecuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            string dataSourceText = $"{serverToolStripStatusLabel.Text}::{databaseToolStripStatusLabel.Text}";
            string message;

            // get the selected text from the sqlTextBox
            string? script = CurrentEditBox?.SelectedText;
            if (script?.Length == 0)
            {
                // if no text is selected, get the whole text from the sqlTextBox
                script = CurrentEditBox?.Text;
                message = $"SQL statement(s) will be executed on {dataSourceText}. Continue?";
            }
            else
            {
                message = $"The selected SQL statement(s) will be executed on {dataSourceText}. Continue?";
            }

            // if the script is empty, show a message box and return
            if (string.IsNullOrEmpty(script))
            {
                Common.MsgBox("No SQL statements to execute", MessageBoxIcon.Information);
                return;
            }

            // confirm if the user wants to execute the script
            if (Common.MsgBox(message, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            // checks if there is a DROP statement in the script, ask for confirmation
            if (script.Contains("DROP ", StringComparison.CurrentCultureIgnoreCase))
            {
                if (Common.MsgBox("WARNING!\nThe scripts contains DROP statement(s). This may result in PERMANENT DATA LOSS. Are you sure you want to continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, "WARNING") == DialogResult.No)
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
            if (dataSourcesToolStripComboBox.SelectedItem is DatabaseConnectionItem connection)
            {
                if (connection.ConnectionString == null || connection.ConnectionString.Length == 0)
                {
                    Common.MsgBox("No database connection selected", MessageBoxIcon.Error);
                    return;
                }

                // check if the script contains a 'usp_addupdateextendedproperty' statement
                if (script.Contains("usp_addupdateextendedproperty", StringComparison.CurrentCultureIgnoreCase))
                {
                    // ask for confirmation to execute the script
                    if (Common.MsgBox("The script references usp_addupdateextendedproperty. Do you want to (re)create it in the target database now?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        await SQLDatabaseHelper.AddObjectDescriptionSPs(connection.ConnectionString);
                    }
                    else
                    {
                        return;
                    }
                }

                messageLabel.Text = string.Format("Execute on {0}...", connection.ToString());
                Cursor = Cursors.WaitCursor;

                var executeResults = await ExecuteScriptsAsync(connection, script);

                if (executeResults.Length > 0)
                {
                    Common.MsgBox(executeResults, MessageBoxIcon.Error);
                }
                else
                {
                    Common.MsgBox("Execute completed successfully", MessageBoxIcon.Information);
                    _allObjects = _dbSchema.AllObjects();
                }

                Cursor = Cursors.Default;
                messageLabel.Text = "";
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
            var mruFiles = new MostRecentUsedFiles();
            mruFiles.Load();
            mruFiles.AddFile(fileName);

            PopulateMRUFiles();
        }

        /// <summary>
        /// Handles the Click event of the OpenToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
        /// <param name="e">The event arguments.</param>
        private void Options_Changed(object sender, EventArgs e)
        {
            Properties.Settings.Default.AddDataSource = addDataSourceCheckBox.Checked;
            Properties.Settings.Default.AddDropStatement = scriptDropsCheckBox.Checked;
            Properties.Settings.Default.UseExtendedProperties = useExtendedPropertyRadioButton.Checked;
            Properties.Settings.Default.UseQuotedIdentifier = quotedIDCheckBox.Checked;
            Properties.Settings.Default.CopyToClipboard = autoCopyCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the "Panel2" resize event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Panel2_Resize(object sender, EventArgs e)
        {
            int margin = 2;
            int spacing = 2;
            int width = panel2.Width - 2 * margin;

            if (width > 0)
            {
                objectTypeComboBox.Width = width;
                schemaComboBox.Width = width;

                // Layout searchTextBox, searchButton, clearSerachButton in one row
                searchTextBox.Left = margin;
                searchTextBox.Width = width - searchButton.Width - clearSerachButton.Width - 2 * spacing;

                searchButton.Left = searchTextBox.Right + spacing;
                clearSerachButton.Left = searchButton.Right + spacing;

                panel2.Height = searchTextBox.Top + searchTextBox.Height + margin;
            }
        }

        /// <summary>
        /// Handles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is TextBox textBox)
            {
                textBox.Paste();
            }
            else if (focusedControl is DBObjectDefPanel dBObjectDefPanel)
            {
                dBObjectDefPanel.Paste();
            }
            else if (focusedControl is SqlEditBox editBox)
            {
                if (editBox.DocumentType == SqlEditBox.DocumentTypeEnums.empty || editBox.DocumentType == SqlEditBox.DocumentTypeEnums.Markdown)
                {
                    if (editBox.DocumentType == SqlEditBox.DocumentTypeEnums.empty)
                    {
                        editBox.DocumentType = SqlEditBox.DocumentTypeEnums.Markdown;
                    }

                    editBox.ReplaceSelection(GetMarkdownFromClipboard());
                }
                else
                {
                    editBox.Paste();
                }
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.Paste();
            }
            else if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is active, paste into the current cell
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    if (dataGridView.EditingControl is TextBox editingTextBox && editingTextBox.Enabled)
                    {
                        editingTextBox.Paste();
                    }
                }
            }
            else
            {
                messageLabel.Text = "No valid control is focused for pasting.";
            }
        }

        private string GetMarkdownFromClipboard()
        {
            // 1. Check if the clipboard contains HTML
            if (Clipboard.ContainsData(DataFormats.Html))
            {
                // 2. Extract the HTML string
                string html = Clipboard.GetText(TextDataFormat.Html);

                // 3. Configure the converter
                var config = new Config
                {
                    UnknownTags = Config.UnknownTagsOption.PassThrough, // Keep tags it doesn't recognize
                    GithubFlavored = true // Use GFM (tables, task lists, etc.)
                };

                var converter = new Converter(config);

                // 4. Convert and return
                return converter.Convert(html);
            }

            // Fallback to plain text if no formatting exists
            return Clipboard.GetText();
        }

        /// <summary>
        /// Populates the objects list box with tables, ordered by schema name and table name.
        /// </summary>
        private async Task PopulateAsync()
        {
            await definitionPanel.OpenAsync(null, _currentConnection);
            string schemaName = string.Empty;
            if (schemaComboBox.SelectedIndex > 0)
                schemaName = schemaComboBox.Text;

            if (_tables != null)
            {
                objectsListBox.Items.Clear();
                string searchFor = searchTextBox.Text.Trim();

                bool useExactMatch = false;

                if (searchFor.IndexOf('.') > 0)
                {
                    // try parse the search string to a valid SQL identifier
                    if (ObjectName.TryParse(searchFor, out ObjectName objName))
                    {
                        schemaName = objName.Schema;
                        searchFor = objName.Name;
                        useExactMatch = true; // use exact match if the search string is a valid SQL identifier
                    }
                }

                // use exact match if the search string is quoted
                if (searchFor.StartsWith('[') && searchFor.EndsWith(']') || searchFor.StartsWith("'") && searchFor.EndsWith("'"))
                {
                    useExactMatch = true;
                }
                searchFor = searchFor.RemoveQuote();

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
                            // if use exact match, check for exact match of the table name
                            if (useExactMatch)
                            {
                                // Use the Equals method for exact match
                                if (table.Name.Equals(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                    AddListItem(table.ObjectType, table.Schema, table.Name);
                            }
                            else
                            {
                                // Use the LIKE operator to find tables that contain the search string
                                if (table.Name.Contains(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                    AddListItem(table.ObjectType, table.Schema, table.Name);
                            }
                        }
                        else
                        {
                            // if use exact match, check for exact match of the table name
                            if (useExactMatch)
                            {
                                // Use the Equals method for exact match
                                if (table.Schema.Equals(schemaName, StringComparison.CurrentCultureIgnoreCase) &&
                                    table.Name.Equals(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                    AddListItem(table.ObjectType, table.Schema, table.Name);
                            }
                            else
                            {
                                // Use the LIKE operator to find tables that contain the search string
                                if (schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase) &&
                                    table.Name.Contains(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                    AddListItem(table.ObjectType, table.Schema, table.Name);
                            }
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
        /// Using the template settings and populates the document type
        /// </summary>
        private void PopulateDocumentType()
        {
            // Clear existing items in the document type combobox
            docTypeToolStripComboBox.Items.Clear();

            var templates = new Templates();
            templates.Load();

            // open template editor if no templates found
            if (templates.TemplateLists.Count == 0)
            {
                var templateEditor = new TemplateEditor();
                templateEditor.ShowDialog();
                templates.Load(); // Reload templates after editing
            }

            // close the app if no templates found
            if (templates.TemplateLists.Count == 0)
            {
                Common.MsgBox("No templates found. Please create a template first.", MessageBoxIcon.Error);
                Close();
                return;
            }

            foreach (var type in templates.TemplateLists)
            {
                // add the document type to the combobox
                docTypeToolStripComboBox.Items.Add(type.DocumentType);
            }

            var lastDocType = Properties.Settings.Default.DocumentType;
            if (string.IsNullOrEmpty(lastDocType))
            {
                lastDocType = templates.TemplateLists[0].DocumentType;
            }

            // check the menu item that matches the last document type
            foreach (var item in docTypeToolStripComboBox.Items)
            {
                if (item.ToString().Equals(lastDocType, StringComparison.OrdinalIgnoreCase))
                {
                    // select the item in the combobox
                    docTypeToolStripComboBox.SelectedItem = item;
                    break;
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
        /// Handles "Query to INSERT" strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void QueryInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            using var form = new QueryDataToTableForm()
            {
                ConnectionString = connectionString,
                Connection = _currentConnection,
                InsertStatement = true
            };
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (BeginAddDDLScript())
                {
                    // Check if the SQL statement is a valid SELECT statement
                    var provider = ActiveProvider;
                    if (!await provider.IsValidSelectStatementAsync(form.SQL, connectionString))
                    {
                        MessageBox.Show("Cannot generate the INSERT statement because the table or view contains columns with unsupported data types.", "Invalid SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var insertStatements = await DatabaseDocBuilder.QueryDataToInsertStatementAsync(form.SQL, _dbSchema);

                    AppendText(editBox, insertStatements);
                    CopyToClipboard(editBox);
                }
            }
        }

        /// <summary>
        /// Handles the click event of the replace all button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            ReplaceManager.ReplaceAll();
        }

        /// <summary>
        /// Handles the click event of the find previous button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            ReplaceManager.Replace();
        }

        /// <summary>
        /// Handles the key down event of the replace text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReplaceReplaceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                if (ReplaceIsOpen)
                {
                    ReplaceManager.Replace();
                    replaceReplaceTextBox.Focus();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
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
                var lastConnectionGuid = new Guid(lastConnection);

                // find the connection that matches the last access connection GUID
                for (int i = 0; i < connectToToolStripMenuItem.DropDown.Items.Count; i++)
                {
                    if (connectToToolStripMenuItem.DropDown.Items[i] is ConnectionMenuItem menuItem && menuItem.Connection.ConnectionID.Equals(lastConnectionGuid))
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
        /// <param name="e">The event arguments.</param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return; // If there is no current edit box, exit early

            CurrentEditBox.SaveAs();
            messageLabel.Text = "Complete";
        }

        /// <summary>
        /// Handles the "Save" tool strip menu item click:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return; // If there is no current edit box, exit early
            CurrentEditBox.Save();
        }

        /// <summary>
        /// Handles the click event of the save tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_mouseOnTabIndex < 0 || _mouseOnTabIndex >= tabControl1.TabCount) return;

            // get the edit box at the selected tab index
            var editBox = GetTextBoxAt(_mouseOnTabIndex);

            if (editBox == null) return;

            // save the file
            editBox.Save();
        }

        /// <summary>
        /// Handles the "Schema" combo box selected index changed event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
            else if (focusedControl is SqlEditBox editBox)
            {
                editBox.SelectAll();
            }
            else if (focusedControl is ColumnDefView columnDefView)
            {
                columnDefView.SelectAll();
            }
            else if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is active, select all cells
                dataGridView.SelectAll();
            }
            else
            {
                messageLabel.Text = "No valid control is focused for select.";
            }
        }

        /// <summary>
        /// Selects the objects.
        /// </summary>
        /// <returns>A List&lt;ObjectName&gt;? .</returns>
        private List<ObjectName>? SelectObjects(bool tableViewOnly)
        {
            if (!GetConnectionString(out string connectionString)) return []; // If we don't have a connection string, exit early

            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            var form = new DBObjectsSelectForm()
            {
                ConnectionString = connectionString,
                SchemaCache = _dbSchema,
                TableViewOnly = tableViewOnly
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
        private void SetConnectionComboBox(DatabaseConnectionItem connectionItem)
        {
            _ignoreConnectionComboBoxIndexChange = true;

            // go through the dataSourcesToolStripComboBox and find the matched item
            for (int i = 0; i < dataSourcesToolStripComboBox.Items.Count; i++)
            {
                if (dataSourcesToolStripComboBox.Items[i] is DatabaseConnectionItem comboItem)
                {
                    if (comboItem.ConnectionID == connectionItem.ConnectionID)
                    {
                        dataSourcesToolStripComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            _ignoreConnectionComboBoxIndexChange = false;
        }

        /// <summary>
        /// Handles the click event of the Sherlock Software tool strip menu item.
        /// Navigates to the Sherlock Software website.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SherlockSoftwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "https://www.sherlocksoftwareinc.com/",
                UseShellExecute = true
            });
        }

        /// <summary>
        /// Start build.
        /// </summary>
        private void StartBuild(SqlEditBox editBox)
        {
            editBox.Enabled = false;
            messageLabel.Text = "Please wait while generate the scripts";
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = true;
            editBox.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
        }

        /// <summary>
        /// Handles the click event of the tab alias tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
            /*
            const int CloseButtonSize = 15;
            const int CloseButtonMargin = 5;
            bool _darkModeEnabled = true;

            var tabRect = tabControl1.GetTabRect(e.Index);
            var tabPage = tabControl1.TabPages[e.Index];
            var tabText = tabPage.Text;

            bool isSelected = (e.Index == tabControl1.SelectedIndex);

            // Use a lighter color for the selected tab
            Color backColor = _darkModeEnabled
                ? (isSelected ? Color.FromArgb(70, 70, 70) : Color.FromArgb(40, 40, 40))
                : (isSelected ? SystemColors.ControlLightLight : SystemColors.Control);
            Color textColor = Color.White; // Always white for visibility
            Color closeColor = _darkModeEnabled ? Color.LightGray : Color.Black;

            // Fill background
            using (var b = new SolidBrush(backColor))
                e.Graphics.FillRectangle(b, tabRect);

            // Draw a border for the selected tab
            if (isSelected)
            {
                using var borderPen = new Pen(Color.FromArgb(120, 120, 120), 2);
                Rectangle borderRect = tabRect;
                borderRect.Width -= 1;
                borderRect.Height -= 1;
                e.Graphics.DrawRectangle(borderPen, borderRect);
            }

            // Vertically center the text
            using (var textBrush = new SolidBrush(textColor))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
                Rectangle textRect = new(
                    tabRect.X + 4,
                    tabRect.Y,
                    tabRect.Width - CloseButtonSize - CloseButtonMargin - 8,
                    tabRect.Height
                );
                e.Graphics.DrawString(tabText, this.Font, textBrush, textRect, sf);
            }

            // Draw close button (simple X), vertically centered
            Rectangle closeRect = new(
                tabRect.Right - CloseButtonSize - CloseButtonMargin,
                tabRect.Top + (tabRect.Height - CloseButtonSize) / 2,
                CloseButtonSize,
                CloseButtonSize);

            using var pen = new Pen(closeColor, 2);
            int padding = 4;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawLine(pen, closeRect.Left + padding, closeRect.Top + padding, closeRect.Right - padding, closeRect.Bottom - padding);
            e.Graphics.DrawLine(pen, closeRect.Right - padding, closeRect.Top + padding, closeRect.Left + padding, closeRect.Bottom - padding);
            */
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
                // hide open folder if the current edit box has no file name
                if (CurrentEditBox != null && string.IsNullOrEmpty(CurrentEditBox.FileName))
                {
                    openFolderInFileExplorerToolStripMenuItem.Visible = false;
                }
                else
                {
                    openFolderInFileExplorerToolStripMenuItem.Visible = true;
                }

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
        /// Copies the  to clipboard.
        /// </summary>
        private void CopyToClipboard(SqlEditBox editBox)
        {
            if (Properties.Settings.Default.CopyToClipboard && editBox != null && editBox?.Text.Trim().Length > 0)
            {
                Clipboard.SetText(editBox.Text);
            }
        }

        /// <summary>
        /// Handles the resize event of the tab control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TabControl1_Resize(object sender, EventArgs e)
        {
            searchPanel.Top = tabControl1.Top + 36;
            searchPanel.Left = tabControl1.Left + tabControl1.Width - searchPanel.Width - 16 - SystemInformation.VerticalScrollBarWidth;

            replacePanel.Top = searchPanel.Top;
            replacePanel.Left = searchPanel.Left;
        }

        /// <summary>
        /// Handles the selected index changed event of the tab control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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

            // disable Open Folder in File Explorer if the current edit box has no file name
            if (CurrentEditBox != null && string.IsNullOrEmpty(CurrentEditBox.FileName))
            {
                openFolderToolStripMenuItem.Enabled = false;
            }
            else
            {
                openFolderToolStripMenuItem.Enabled = true;
            }

            // set the focused control to the current edit box
            CurrentEditBox?.Focus();
        }

        /// <summary>
        /// Handles the "Table builder form" form closing event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TableBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CancelPendingSchemaLoad();

            if (CloseAllTabs() == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            Properties.Settings.Default.LeftSplitterDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.RightSplitterDistance = defiCollapsibleSplitter.SplitterDistance;
            Properties.Settings.Default.UseQuotedIdentifier = quotedIDCheckBox.Checked;
            Properties.Settings.Default.CopyToClipboard = autoCopyCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the "Table builder form" load event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TableBuilderForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                darkModeToolStripMenuItem.Checked = true;
                _ = new DarkMode(this);
            }

            WindowState = FormWindowState.Maximized;

            _init = true;
            // Populate the object types combo box with dictionary values
            var objectTypeDict = new Dictionary<ObjectName.ObjectTypeEnums, string>
            {
                { ObjectName.ObjectTypeEnums.Table, "Table" },
                { ObjectName.ObjectTypeEnums.View, "View" },
                { ObjectName.ObjectTypeEnums.StoredProcedure, "Stored Procedure" },
                { ObjectName.ObjectTypeEnums.Function, "Function" },
                { ObjectName.ObjectTypeEnums.Trigger, "Trigger" },
                { ObjectName.ObjectTypeEnums.Synonym, "Synonym" }
            };

            objectTypeComboBox.DataSource = new BindingSource(objectTypeDict, null);
            objectTypeComboBox.DisplayMember = "Value";
            objectTypeComboBox.ValueMember = "Key";
            _init = false;

            startTimer.Start();
        }

        /// <summary>
        /// Handles the "Table description" tool strip menu item click:
        /// generate the table description and column descriptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void TableDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedObject == null || !GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            var objectName = SelectedObject;
            if (!string.IsNullOrEmpty(objectName?.Name))
            {
                var description = await ObjectDescription.BuildObjectDescription(objectName, _dbSchema, Properties.Settings.Default.UseExtendedProperties);
                if (!string.IsNullOrEmpty(description))
                {
                    if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                    if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Sql) != DialogResult.Yes) return;

                    Cursor = Cursors.WaitCursor;
                    if (BeginAddDDLScript())
                    {
                        // add batch separator only for SQL Server
                        if (!string.IsNullOrEmpty(CurrentBatchSeparator))
                        {
                            description += CurrentBatchSeparator + Environment.NewLine;
                        }
                        AppendText(editBox, description);
                        CopyToClipboard(editBox);
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
        /// Handles timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
        private void UspToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Sql) == DialogResult.Yes)
            {
                AppendText(editBox, DatabaseDocBuilder.UspAddObjectDescription());
                CopyToClipboard(editBox);
            }
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

            HotKeyManager.AddHotKey(this, FindNextF3, Keys.F3);
        }

        /// <summary>
        /// Finds the next match by F3 key.
        /// </summary>
        private void FindNextF3()
        {
            if (SearchIsOpen)
            {
                // Quick search mode: use last search term
                SearchManager.Find(true, false);
            }
            else if (ReplaceIsOpen)
            {
                // Replace mode: use last search term
                ReplaceManager.Find(true, false);
            }
        }

        /// <summary>
        /// Handles the text changed event of the SQL text box:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTextChanged(object? sender, EventArgs e)
        {
            //if (CurrentEditBox == null || sender == null) return;
            //CurrentEditBox.OnTextChanged(sender, e);
            messageLabel.Text = string.Empty;
        }

        #region Main Menu Commands

        /// <summary>
        /// Handles the click event of the collapse all tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CollapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditBox?.FoldAll(FoldAction.Contract);
        }

        /// <summary>
        /// Handles the click event of the collapse all tool strip menu item:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ExpandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditBox?.FoldAll(FoldAction.Expand);
        }

        /// <summary>
        /// Handles the click event of the find and replace tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Handles the click event of the lowercase selection tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LowercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lowercase();
        }

        /// <summary>
        /// Handles the click event of the quick find tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void QuickFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSearch();
        }

        /// <summary>
        /// Handles the click event of the redo tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return; // If there is no current edit box, exit early
            if (CurrentEditBox.CanRedo)
            {
                CurrentEditBox.Redo();
            }
        }

        /// <summary>
        /// Handles the click event of the undo tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox == null) return; // If there is no current edit box, exit early
            if (CurrentEditBox.CanUndo)
            {
                CurrentEditBox.Undo();
            }
        }

        /// <summary>
        /// Handles the click event of the upper case selection tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UppercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uppercase();
        }

        /// <summary>
        /// Handles the click event of the zoom default tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomDefault();
        }

        /// <summary>
        /// Handles the click event of the zoom in tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// Handles the click event of the zoom out tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
            if (CurrentEditBox == null) return; // If there is no current edit box, exit early

            // save the selection
            int start = CurrentEditBox.SelectionStart;
            int end = CurrentEditBox.SelectionEnd;

            // modify the selected text
            CurrentEditBox.ReplaceSelection(CurrentEditBox.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            CurrentEditBox.SetSelection(start, end);
        }

        /// <summary>
        /// Uppercases the.
        /// </summary>
        private void Uppercase()
        {
            if (CurrentEditBox == null) return; // If there is no current edit box, exit early

            var editBox = CurrentEditBox;

            // save the selection
            int start = editBox.SelectionStart;
            int end = editBox.SelectionEnd;

            // modify the selected text
            editBox?.ReplaceSelection(editBox?.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            editBox?.SetSelection(start, end);
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
        /// <param name="e">The event arguments.</param>
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

            // focus the current edit box if it exists
            CurrentEditBox?.Focus();
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

            if (string.IsNullOrWhiteSpace(searchSQLTextBox.Text))
            {
                int caretPos = CurrentEditBox.CurrentPosition;
                string word = CurrentEditBox.SelectedText;
                if (string.IsNullOrWhiteSpace(word)) word = CurrentEditBox.GetWordFromPosition(caretPos);
                if (!string.IsNullOrWhiteSpace(word))
                {
                    SearchManager.LastSearch = word;
                    //searchSQLTextBox.Text = word;
                }
            }

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
        /// <param name="e">The event arguments.</param>
        private void SearchNext_Click(object sender, EventArgs e)
        {
            SearchManager.Find(true, false);
        }

        /// <summary>
        /// Handles the click event of the search previous button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SearchPrevious_Click(object sender, EventArgs e)
        {
            SearchManager.Find(false, false);
        }

        /// <summary>
        /// Handles the key down event of the search text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                if (SearchIsOpen)
                    SearchManager.Find(true, false);
                else if (ReplaceIsOpen)
                    ReplaceManager.Find(true, false);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
            {
                if (SearchIsOpen)
                    SearchManager.Find(false, false);
                else if (ReplaceIsOpen)
                    ReplaceManager.Find(false, false);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Handles the text changed event of the search text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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

            int caretPos = CurrentEditBox.CurrentPosition;
            string word = CurrentEditBox.SelectedText;
            if (string.IsNullOrWhiteSpace(word)) word = CurrentEditBox.GetWordFromPosition(caretPos);
            if (!string.IsNullOrWhiteSpace(word))
            {
                ReplaceManager.LastSearch = word;
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
        /// <param name="e">The event arguments.</param>
        private void ClipboardToTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                    if (CheckCurrentDocumentType() != DialogResult.Yes) return;

                    var docType = GetDocumentType();

                    // get the template for the object list
                    var template = GetTemplate();
                    if (template == null)
                    {
                        Common.MsgBox($"No template found for {docType} format.", MessageBoxIcon.Warning);
                        return;
                    }

                    // find the objec list template item in the template
                    var datatableTemplate = template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.DataTable);

                    AppendText(editBox, DocumentBuilder.TextToTable(metaData, datatableTemplate));

                    EndBuild(editBox);
                }
            }

            messageLabel.Text = "Complete!";
        }

        /// <summary>
        /// Handles the click event of the query data to table tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void QueryDataToTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            using var form = new QueryDataToTableForm() { ConnectionString = connectionString, Connection = _currentConnection };
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (CheckCurrentDocumentType() != DialogResult.Yes) return;

                var docType = GetDocumentType();

                // get the template for the object list
                var template = GetTemplate();
                if (template == null)
                {
                    Common.MsgBox($"No template found for {docType} format.", MessageBoxIcon.Warning);
                    return;
                }

                // find the objec list template item in the template
                var datatableTemplate = template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.DataTable);
                if (datatableTemplate != null)
                {
                    // Check if the SQL statement is a valid SELECT statement
                    var provider = ActiveProvider;
                    if (!await provider.IsValidSelectStatementAsync(form.SQL, connectionString))
                    {
                        MessageBox.Show("Cannot generate the INSERT statement because the table or view contains columns with unsupported data types.", "Invalid SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    AppendText(editBox, await DocumentBuilder.GetTableValuesAsync(form.SQL, _currentConnection, datatableTemplate));
                    CopyToClipboard(editBox);
                }
                else
                {
                    Common.MsgBox($"No object list template found for {docType} format.", MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        /// <summary>
        /// Handles the click event of the table definition tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void TableDefinitionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (SelectedObject != null)
            {
                var objectName = SelectedObject;

                var docType = GetDocumentType();
                // get the template for the object list
                var template = GetTemplate();
                if (template == null)
                {
                    Common.MsgBox($"No template found for {objectName.ObjectType} in {docType} format.", MessageBoxIcon.Warning);
                    return;
                }

                // find the objec list template item in the template
                var objectTemplate = objectName.ObjectType switch
                {
                    ObjectTypeEnums.Table => template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.Table),
                    ObjectTypeEnums.View => template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.View),
                    ObjectTypeEnums.StoredProcedure => template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.StoredProcedure),
                    ObjectTypeEnums.Function => template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.Function),
                    ObjectTypeEnums.Trigger => template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.Trigger),
                    ObjectTypeEnums.Synonym => template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.Synonym),
                    _ => null
                };

                if (objectTemplate == null)
                {
                    Common.MsgBox($"No template found for {objectName.ObjectType} in {docType} format.", MessageBoxIcon.Warning);
                    return;
                }

                // get data table template if the object is a table or view
                TemplateItem? datatableTemplate = null;
                if (objectName.ObjectType == ObjectTypeEnums.Table || objectName.ObjectType == ObjectTypeEnums.View)
                {
                    datatableTemplate = template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.DataTable);
                }

                var scripts = await DocumentBuilder.GetObjectDef(objectName, _currentConnection, objectTemplate, datatableTemplate, _dbSchema);
                if (scripts.Length == 0)
                {
                    Common.MsgBox($"No definition found for {objectName.FullName}", MessageBoxIcon.Information);
                    return;
                }

                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (CheckCurrentDocumentType() != DialogResult.Yes) return;

                AppendText(editBox, scripts);

                EndBuild(editBox);
            }
        }

        /// <summary>
        /// Handles the click event of the table list tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void TableListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            List<ObjectName>? selectedObjects = SelectObjects(false);

            if (selectedObjects == null || selectedObjects.Count == 0) return;  // If no objects are selected, exit early

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            if (CheckCurrentDocumentType() != DialogResult.Yes) return;

            var docType = GetDocumentType();
            // get the template for the object list
            var template = GetTemplate();
            if (template == null)
            {
                Common.MsgBox($"No template found for {docType} format.", MessageBoxIcon.Warning);
                return;
            }

            // find the objec list template item in the template
            var objectListTemplate = template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.ObjectList);

            if (objectListTemplate == null)
            {
                Common.MsgBox($"No object list template found for {docType} format.", MessageBoxIcon.Warning);
                return;
            }

            StartBuild(editBox);

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
            });

            string scripts = String.Empty;
            await Task.Run(async () =>
            {
                scripts = await DocumentBuilder.BuildObjectList(selectedObjects, _currentConnection, objectListTemplate, progress, _dbSchema);
            });

            AppendText(editBox, scripts);

            EndBuild(editBox);
        }

        /// <summary>
        /// Handles the click event of the table values (MarkDown) tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void TableValuesMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (SelectedObject != null)
            {
                var objectName = SelectedObject;

                // check if the object is a table or view
                if (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View)
                {
                    Common.MsgBox($"The feature is for Table or View only.", MessageBoxIcon.Warning);
                    return;
                }

                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (CheckCurrentDocumentType() != DialogResult.Yes) return;

                var docType = GetDocumentType();

                // get the template for the object list
                var template = GetTemplate();
                if (template == null)
                {
                    Common.MsgBox($"No template found for {docType} format.", MessageBoxIcon.Warning);
                    return;
                }

                // find the objec list template item in the template
                var datatableTemplate = template.TemplateLists.FirstOrDefault(t => t.ObjectType == TemplateItem.ObjectTypeEnums.DataTable);

                if (datatableTemplate == null)
                {
                    Common.MsgBox($"No object list template found for {docType} format.", MessageBoxIcon.Warning);
                    return;
                }

                string sql = $"select * from {objectName.FullName}";
                // Check if the SQL statement is a valid SELECT statement
                var provider = ActiveProvider;
                if (!await provider.IsValidSelectStatementAsync(sql, connectionString))
                {
                    MessageBox.Show("Cannot generate script or document because the table or view contains columns with unsupported data types.", "Invalid SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AppendText(editBox, await DocumentBuilder.GetTableValuesAsync(sql, _currentConnection, datatableTemplate));

                EndBuild(editBox);
            }
        }

        #endregion Markdown document builder

        #region "MRU files"

        /// <summary>
        /// Populate the MRU files to the menu strip
        /// </summary>
        private void PopulateMRUFiles()
        {
            var mruFiles = new MostRecentUsedFiles();
            mruFiles.Load();

            // Remove existing MRU file menu items
            RemoveMRUFileMenuItems();

            // Only take the first 30 files
            foreach (string item in mruFiles.Files?.Take(30) ?? Array.Empty<string>())
            {
                ToolStripMenuItem fileRecent = new(item);
                fileRecent.Click += RecentFile_click;
                recentToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
            recentToolStripMenuItem.Enabled = (mruFiles.Files?.Length ?? 0) > 0;
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

        #region Json document builder

        /// <summary>
        /// Handles the click event of the Json clipboard to table tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void JsonClipboardToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var metaData = Clipboard.GetText();

                if (metaData.Length > 1)
                {
                    if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                    if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Json) != DialogResult.Yes) return;

                    var builder = new JsonBuilder();

                    AppendText(editBox, JsonBuilder.TextToTable(metaData));

                    EndBuild(editBox);
                }
            }
        }

        /// <summary>
        /// Handles the click event of the Json object definition tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void JsonObjectDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (SelectedObject != null)
            {
                var objectName = SelectedObject;

                var scripts = objectName.ObjectType switch
                {
                    ObjectTypeEnums.Table => await JsonBuilder.GetTableDef(objectName, _currentConnection),
                    ObjectTypeEnums.View => await JsonBuilder.GetViewDef(objectName, _currentConnection),
                    ObjectTypeEnums.StoredProcedure or ObjectTypeEnums.Function => await JsonBuilder.GetFunctionProcedureDef(objectName, _currentConnection),
                    _ => string.Empty
                };

                if (scripts.Length == 0)
                {
                    Common.MsgBox($"No definition found for {objectName.FullName}", MessageBoxIcon.Information);
                    return;
                }

                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Json) != DialogResult.Yes) return;

                AppendText(editBox, scripts);

                EndBuild(editBox);
            }
        }

        /// <summary>
        /// Handles the click event of the Json table view values tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void JsonObjectListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            List<ObjectName>? selectedObjects = SelectObjects(false);

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

            if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Json) != DialogResult.Yes) return;

            StartBuild(editBox);

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
            });

            string scripts = String.Empty;
            var builder = new JsonBuilder();
            await Task.Run(async () =>
            {
                scripts = await JsonBuilder.BuildObjectList(selectedObjects, _currentConnection, progress);
            });

            AppendText(editBox, scripts);

            EndBuild(editBox);
        }

        /// <summary>
        /// Handles the click event of the Json query data to table tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void JsonQueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            using var form = new QueryDataToTableForm() { ConnectionString = connectionString, Connection = _currentConnection };
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Json) != DialogResult.Yes) return;

                // check if the SQL statement is a valid SELECT statement to generate JSON data
                var provider = ActiveProvider;
                if (!await provider.IsValidSelectStatementAsync(form.SQL, connectionString))
                {
                    MessageBox.Show("Cannot generate the JSON data because the table or view contains columns with unsupported data types.", "Invalid SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AppendText(editBox, await JsonBuilder.GetTableValuesAsync(form.SQL, _currentConnection));
                CopyToClipboard(editBox);
            }
        }

        /// <summary>
        /// Handles the click event of the table or view data to json tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void JsonTableViewValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            if (SelectedObject != null)
            {
                var objectName = SelectedObject;

                // check if the object is a table or view
                if (objectName.ObjectType != ObjectTypeEnums.Table && objectName.ObjectType != ObjectTypeEnums.View)
                {
                    Common.MsgBox($"The feature is for Table or View only.", MessageBoxIcon.Warning);
                    return;
                }

                // Check if the SQL statement is a valid SELECT statement
                string sql = $"select * from {objectName.FullName}";
                var provider = ActiveProvider;
                if (!await provider.IsValidSelectStatementAsync(sql, connectionString))
                {
                    MessageBox.Show("Cannot generate the JSON data because the table or view contains columns with unsupported data types.", "Invalid SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var scripts = await JsonBuilder.GetTableValuesAsync(sql, _currentConnection);

                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (CheckCurrentDocumentType(SqlEditBox.DocumentTypeEnums.Json) != DialogResult.Yes) return;

                AppendText(editBox, scripts);

                EndBuild(editBox);
            }
        }

        #endregion Json document builder

        /// <summary>
        /// Handles the click event of the export descriptions tool strip menu item.
        /// Export descriptions of the selected objects to a CSV file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void ExportDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            // get the file name to save the CSV file
            using SaveFileDialog saveFileDialog = new()
            {
                Filter = "CSV Files|*.csv",
                Title = "Export Descriptions to CSV",
                FileName = "Descriptions.csv"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // get the selected objects
                List<ObjectName>? selectedObjects = SelectObjects(false);
                if (selectedObjects == null || selectedObjects.Count == 0)
                {
                    Common.MsgBox("No objects selected to export descriptions.", MessageBoxIcon.Warning);
                    return;
                }

                Cursor = Cursors.WaitCursor;
                messageLabel.Text = "Exporting descriptions to CSV...";
                // export the descriptions to CSV
                try
                {
                    await ExcelDataHelper.ExportDescriptionsToCsv(selectedObjects, saveFileDialog.FileName, _currentConnection);

                    Common.MsgBox("Descriptions exported successfully.", MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Common.MsgBox($"Error exporting descriptions to CSV: {ex.Message}", MessageBoxIcon.Error);
                }
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the click event of the import descriptions tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ImportDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            // get the file name (CSV default, Excel supported)
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileName = openFileDialog.FileName;

            DataTable? descriptionData;
            if (Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                descriptionData = LoadDescriptionDataFromCsv(fileName);
            }
            else
            {
                using var form = new ExcelSheetsForm()
                {
                    FileName = fileName
                };
                form.ShowDialog();
                descriptionData = form.ResultDataTable;
            }

            if (descriptionData != null)
            {
                if (!IsValidDescriptionData(descriptionData))
                {
                    Common.MsgBox("The selected file does not contain valid description data.", MessageBoxIcon.Warning);
                    return;
                }

                // Add a new edit box if it's not empty
                if (CurrentEditBox == null || CurrentEditBox.Text.Length > 0)
                {
                    AddTab("");
                }

                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                editBox.DocumentType = SqlEditBox.DocumentTypeEnums.Sql; // Set the document type to SQL

                AppendText(editBox, $"-- Data source: {fileName}" + Environment.NewLine);

                var dataHelper = new ExcelDataHelper(descriptionData);
                AppendText(editBox, dataHelper.GetDescriptionStatement(_currentConnection));
            }
        }

        private static DataTable? LoadDescriptionDataFromCsv(string fileName)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            var configuration = new ExcelReaderConfiguration
            {
                AutodetectSeparators = [',', ';', '\t', '|', '#'],
                FallbackEncoding = Encoding.UTF8
            };

            using var reader = ExcelReaderFactory.CreateCsvReader(stream, configuration);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
        }

        /// <summary>
        /// Are the valid description data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A bool.</returns>
        private bool IsValidDescriptionData(DataTable data)
        {
            // Check if the DataTable has the required columns
            var requiredColumns = new[] { "Level0Type", "Level0Name", "Level1Type", "Level1Name", "Level2Type", "Level2Name", "Value" };
            foreach (var column in requiredColumns)
            {
                if (!data.Columns.Contains(column))
                {
                    return false; // Missing required column
                }
            }
            return true; // All required columns are present
        }

        /// <summary>
        /// Starts the timer_ tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void StartTimer_Tick(object sender, EventArgs e)
        {
            startTimer.Stop(); // Stop the timer to prevent multiple invocations

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

            defiCollapsibleSplitter.SplitterDistance = Properties.Settings.Default.RightSplitterDistance;
            splitContainer1.SplitterDistance = Properties.Settings.Default.LeftSplitterDistance;
            quotedIDCheckBox.Checked = Properties.Settings.Default.UseQuotedIdentifier;
            autoCopyCheckBox.Checked = Properties.Settings.Default.CopyToClipboard;

            PopulateDocumentType();

            // populate most recent used files
            PopulateMRUFiles();

            AddTab("");
        }

        /// <summary>
        /// Handles the tab close requested event of the tab control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TabControl1_TabCloseRequested(object sender, TabCloseRequestedEventArgs e)
        {
            if (e.TabPage.Controls[0] is SqlEditBox queryTextBox)
            {
                if (queryTextBox.SaveCheck() == DialogResult.Cancel)
                {
                    e.Cancel = true; // Cancel the tab close if SaveCheck returns Cancel
                    return;
                }
                ;

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
                queryTextBox.Dispose();

                // Remove the tab page
                tabControl1.TabPages.Remove(e.TabPage);

                if (tabControl1.TabCount == 0) AddTab("");
            }
        }

        /// <summary>
        /// Handles the click event of the dark mode tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DarkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            darkModeToolStripMenuItem.Checked = !darkModeToolStripMenuItem.Checked;
            Properties.Settings.Default.DarkMode = !Properties.Settings.Default.DarkMode;
            ApplyDarkMode(Properties.Settings.Default.DarkMode);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Applies the dark mode.
        /// </summary>
        /// <param name="darkMode">If true, dark mode.</param>
        private void ApplyDarkMode(bool darkMode)
        {
            if (darkMode)
            {
                _ = new DarkMode(this);
            }
            else
            {
                // restart the application to apply light mode
                Application.Restart();
            }
        }

        /// <summary>
        /// Opens the folder in file explorer tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenFolderInFileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditBox != null && !string.IsNullOrEmpty(CurrentEditBox.FileName))
            {
                var folderPath = System.IO.Path.GetDirectoryName(CurrentEditBox.FileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    try
                    {
                        if (System.IO.Directory.Exists(folderPath))
                        {
                            // Use quotes to handle spaces and special characters
                            var psi = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Arguments = $"\"{folderPath}\"",
                                UseShellExecute = true
                            };
                            System.Diagnostics.Process.Start(psi);
                        }
                        else
                        {
                            Common.MsgBox($"Folder does not exist:\n{folderPath}", MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.MsgBox($"Error opening folder:\n{folderPath}\n{ex.Message}", MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the key up event of the search text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // If the user presses Enter, perform the search
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the beep sound on Enter key press
                if (!string.IsNullOrEmpty(searchTextBox.Text))
                {
                    // Fallback: perform the regular search logic
                    await PopulateAsync();

                    // If no matches found, perform a global search by name
                    if (objectsListBox.Items.Count == 0)
                    {
                        searchButton.PerformClick();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the click event of the search button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextBox.Text))
            {
                // Try to parse the search text into schema and name
                if (ObjectName.TryParse(searchTextBox.Text.Trim(), out ObjectName searchObj))
                {
                    // Remove any quotes/brackets from schema and name
                    string schema = searchObj.Schema.RemoveQuote();
                    string name = searchObj.Name.RemoveQuote();

                    // Find all matches in _allObjects
                    var matches = _allObjects
                        .Where(obj =>
                            obj.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase) &&
                            obj.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (matches.Count == 1)
                    {
                        // Set object type combo box to the matched type using ValueMember (ObjectTypeEnums)
                        var matched = matches[0];
                        objectTypeComboBox.SelectedValue = matched.ObjectType;

                        // List only the matched object in the list box
                        objectsListBox.Items.Clear();
                        objectsListBox.Items.Add(matched);
                        objectsListBox.SelectedIndex = 0;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the click event of the manage recent files tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ManageRecentFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the RecentFilesManageForm to manage recent files
            using RecentFilesManageForm form = new();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // If a file was selected to open, open it
                if (!string.IsNullOrEmpty(form.FileToOpen))
                {
                    OpenFile(form.FileToOpen);
                }
            }

            // Refresh the MRU files in the menu
            PopulateMRUFiles();
        }

        /// <summary>
        /// Handles the click event of the settings tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new SettingsForm();
            dlg.ShowDialog();
        }

        /// <summary>
        /// Handles the AI processing started event of the definition panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DefinitionPanel_AIProcessingStarted(object sender, EventArgs e)
        {
            WorkingMode(true, "⠋ The AI ​​description assistant is working...");
        }

        /// <summary>
        /// Enables or disables the UI components.
        /// </summary>
        /// <param name="v">If true, enables the UI; otherwise, disables it.</param>
        private void EnableDisableUI(bool v)
        {
            // Enable or disable UI components based on the parameter
            toolStrip1.Enabled = v;
            menuStrip1.Enabled = v;
            messageLabel.Enabled = v;

            // Set the cursor for the form and all its controls
            var cur = v ? Cursors.Default : Cursors.WaitCursor;

            Cursor = cur;
            foreach (Control control in Controls)
            {
                SetControlCursor(control, cur);
            }
        }

        /// <summary>
        /// Sets the control cursor.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="c">The c.</param>
        private static void SetControlCursor(Control control, Cursor c)
        {
            control.Cursor = c;
            foreach (Control child in control.Controls)
            {
                if (child is SqlEditBox editBox)
                {
                    editBox.UseWaitCursor = (c == Cursors.WaitCursor);
                }
                else
                    SetControlCursor(child, c);
            }
        }

        /// <summary>
        /// Handles the AI processing completed event of the definition panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DefinitionPanel_AIProcessingCompleted(object sender, EventArgs e)
        {
            WorkingMode(false, "The AI ​​description assistant has completed its work.");
        }

        /// <summary>
        /// Handles the click event of the AI Description Assistant tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void AIDescriptionAssistantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open AI settings dialog if not ready
            if (!AISettingsReady())
            {
                using var dlg = new SettingsForm();
                dlg.ShowDialog();
            }

            if (!AISettingsReady()) return;

            await definitionPanel.AIAssistant();
        }

        /// <summary>
        /// Handles the click event of the add column reference tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddColumnReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            definitionPanel.AddColumnReference();
        }

        /// <summary>
        /// Handles the click event of the optimize code tool strip menu item.
        /// Uses AI to optimize the code of the selected database object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void OptimizeCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            // available only for object type is view, stored procedure, or function
            // set the selected object
            if (SelectedObject == null ||
                (SelectedObject.ObjectType != ObjectTypeEnums.View &&
                SelectedObject.ObjectType != ObjectTypeEnums.StoredProcedure &&
                SelectedObject.ObjectType != ObjectTypeEnums.Function))
            {
                Common.MsgBox("The feature is for View, Stored Procedure, or Function only.", MessageBoxIcon.Warning);
                return;
            }

            WorkingMode(true, "⠋ The AI ​code advisor is working...");

            // get the object definition from the definitionPanel
            var definition = definitionPanel.DefinitionText;

            var content = await AIHelper.OptimizeCodeAsync(definition);

            int tryCount = 3;
            string verifyResult = string.Empty;

            // user AI to midify the code based on user request, retry up to 3 times if the SQL verification fails
            while (tryCount > 0)
            {
                // verify the SQL code syntax
                verifyResult = await VerifySQL(content!);

                if (string.IsNullOrEmpty(verifyResult))
                {
                    break; // exit the loop if verification is successful
                }
                else
                {
                    tryCount--;
                    if (tryCount == 0)
                    {
                        messageLabel.Text = "The SQL code verification failed after multiple attempts.";
                        break;
                    }

                    // use AI to fix the SQL code
                    content = await AIHelper.FixSQLCodeAsync(definition!, content!, verifyResult);
                }
            }

            if (!string.IsNullOrEmpty(content))
            {
                if (!GetCurrentEditBox(out SqlEditBox editBox)) return; // If we can't get the edit box, exit early

                if (!BeginAddDDLScript()) return;

                AddDataSourceText();

                AppendText(editBox, RemoveSqlMarkers(content));

                // when verification fails, append the verification result as comment
                if (!string.IsNullOrEmpty(verifyResult))
                {
                    AppendText(editBox, $"\n/* Verification Result:\n{verifyResult}\n*/");
                }
            }

            if (string.IsNullOrEmpty(verifyResult))
            {
                WorkingMode(false, "The AI code advisor has completed its work.");
            }
            else
            {
                WorkingMode(false, "The AI code advisor failed to complete its work.");
            }
        }

        /// <summary>
        /// Removes the sql markers.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>A string.</returns>
        private string RemoveSqlMarkers(string content)
        {
            // remove the ```sql` and ``` markers if exist
            string pattern = @"```sql(.*?)```";
            Match match = Regex.Match(content, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : content.Trim();
        }

        /// <summary>
        /// Handles the click event of the modify code tool strip menu item.
        /// Gets the modification request from the user and uses AI to modify the code accordingly.
        /// </summary>
        private async void ModifyCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return;

            if (SelectedObject == null || (SelectedObject.ObjectType == ObjectTypeEnums.None))
            {
                Common.MsgBox("There is no object selected.", MessageBoxIcon.Warning);
                return;
            }

            string createScript = await DatabaseDocBuilder.GetCreateObjectScriptAsync(SelectedObject, _dbSchema);
            createScript = createScript?.TrimEnd(' ', '\r', '\n', '\t');

            var description = await ObjectDescription.BuildObjectDescription(SelectedObject, _dbSchema, Properties.Settings.Default.UseExtendedProperties);
            createScript = $"{description}\n\n{createScript}";

            using (var inputBox = new InputBox())
            {
                inputBox.Title = "Modify Code Request";
                inputBox.Prompt = "Please enter your modification request:";
                inputBox.Default = "";
                inputBox.Multiline = true;
                inputBox.MaxLength = 4000;

                if (inputBox.ShowDialog() == DialogResult.OK)
                {
                    var userRequest = inputBox.InputText;

                    if (!string.IsNullOrEmpty(userRequest))
                    {
                        WorkingMode(true, "⠋ The AI code advisor is working...");

                        // Initial AI request
                        var content = await AIHelper.ModifyCodeAsync(createScript!, userRequest);

                        // --- Start Verification Logic ---
                        int tryCount = 3;
                        string verifyResult = string.Empty;

                        while (tryCount > 0)
                        {
                            // Verify the SQL code syntax
                            verifyResult = await VerifySQL(content!);

                            if (string.IsNullOrEmpty(verifyResult))
                            {
                                break; // Success
                            }
                            else
                            {
                                tryCount--;
                                if (tryCount == 0)
                                {
                                    messageLabel.Text = "The SQL code verification failed after multiple attempts.";
                                    break;
                                }

                                // Use AI to fix the modified code using the original script and the error result
                                content = await AIHelper.FixSQLCodeAsync(createScript!, content!, verifyResult);
                            }
                        }
                        // --- End Verification Logic ---

                        if (!string.IsNullOrEmpty(content))
                        {
                            if (!GetCurrentEditBox(out SqlEditBox editBox)) return;

                            if (!BeginAddDDLScript()) return;

                            AddDataSourceText();

                            // Apply the content and clean up markers
                            AppendText(editBox, RemoveSqlMarkers(content));

                            // Append verification errors as comments if any remain
                            if (!string.IsNullOrEmpty(verifyResult))
                            {
                                AppendText(editBox, $"\n/* Verification Result:\n{verifyResult}\n*/");
                            }
                        }

                        if (string.IsNullOrEmpty(verifyResult))
                        {
                            WorkingMode(false, "The AI code advisor has completed its work.");
                        }
                        else
                        {
                            WorkingMode(false, "The AI code advisor failed to complete its work.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifies the SQL code syntax.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A Task.</returns>
        private async Task<string> VerifySQL(string context)
        {
            // extract the SQL code from the context that is between the ```sql and ``` markers
            string pattern = @"```sql(.*?)```";
            Match match = Regex.Match(context, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            string sqlCode = match.Success ? match.Groups[1].Value.Trim() : context.Trim();

            // Split the SQL code into batches using 'GO' as a delimiter
            var sqlBatches = Regex.Split(sqlCode, @"\bGO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (string batch in sqlBatches)
            {
                var trimmedBatch = batch.Trim();
                if (!string.IsNullOrEmpty(trimmedBatch))
                {
                    var provider = ActiveProvider;
                    string verifyResult = await Task.Run(() => provider.VerifySqlAsync(trimmedBatch, ActiveSchemaConnection?.ConnectionString ?? string.Empty));
                    if (!string.IsNullOrEmpty(verifyResult))
                    {
                        return verifyResult;
                    }
                }
            }

            return string.Empty;
        }

        private int _dotCount = 0; // Class-level variable to track dots

        /// <summary>
        /// Handles the tick event of the wait timer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void WaitTimer_Tick(object sender, EventArgs e)
        {
            var moonPhases = new[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
            string currentText = messageLabel.Text ?? "Processing";

            // --- 1. Handle the Rolling Moon (Start of string) ---
            int moonIndex = -1;
            string currentMoon = "";

            foreach (var moon in moonPhases)
            {
                if (currentText.StartsWith(moon))
                {
                    moonIndex = Array.IndexOf(moonPhases, moon);
                    currentMoon = moon;
                    break;
                }
            }

            int nextMoonIndex = (moonIndex + 1) % moonPhases.Length;
            string nextMoon = moonPhases[nextMoonIndex];

            // --- 2. Extract the Core Message ---
            // Remove the moon and any leading/trailing whitespace/dots to get the clean text
            string cleanMessage = currentText;
            if (!string.IsNullOrEmpty(currentMoon))
            {
                cleanMessage = cleanMessage.Substring(currentMoon.Length).Trim();
            }
            // Strip existing trailing dots
            cleanMessage = cleanMessage.TrimEnd('.');

            // --- 3. Handle Growing Dots (End of string) ---
            _dotCount++;
            if (_dotCount > 6) _dotCount = 1;
            string dots = new string('.', _dotCount);

            // --- 4. Reconstruct the String ---
            messageLabel.Text = $"{nextMoon} {cleanMessage}{dots}";
        }

        /// <summary>
        /// sets the working mode.
        /// </summary>
        /// <param name="isBusy">If true, is busy.</param>
        /// <param name="statusMessage">The status message.</param>
        private void WorkingMode(bool isBusy, string statusMessage)
        {
            if (isBusy)
            {
                _aiDescriptionBusy = true;
                EnableDisableUI(false);
                Cursor = Cursors.WaitCursor;

                messageLabel.Text = statusMessage;
                messageLabel.Enabled = true;
                waitTimer.Start();
            }
            else
            {
                waitTimer.Stop();
                _aiDescriptionBusy = false;
                EnableDisableUI(true);
                Cursor = Cursors.Default;
                messageLabel.Text = statusMessage;
            }
        }

        /// <summary>
        /// Handles the click event of the Description Assistant Plus tool strip menu item.
        /// Generates a detailed description of the selected database object with additional
        /// information such as column descriptions, index information, and more, using AI.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void DescriptionAssistantPlusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open AI settings dialog if not ready
            if (!AISettingsReady())
            {
                using var dlg = new SettingsForm();
                dlg.ShowDialog();
            }

            if (!AISettingsReady()) return;

            // get user input for additional information
            using (var inputBox = new InputBox())
            {
                inputBox.Title = "Description Assistant Plus - Additional Information";
                inputBox.Prompt = "Please enter any additional information or specific requirements for the description (optional):";
                inputBox.Default = "";
                inputBox.Multiline = true;
                inputBox.MaxLength = 8000;
                if (inputBox.ShowDialog() == DialogResult.OK)
                {
                    var additionalInfo = inputBox.InputText;
                    await definitionPanel.AIAssistant(additionalInfo);
                }
            }
        }

        /// <summary>
        /// Handles the click event of the save object descriptions tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SaveObjectDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await definitionPanel.Save();
        }

        /// <summary>
        /// Handles the click event of the batch describe tool strip menu item.
        /// Batch generates descriptions for multiple database objects at once using AI,
        /// and allows the user to review and edit the generated descriptions before saving them to the database.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void BatchDescribeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetConnectionString(out string connectionString)) return; // If we don't have a connection string, exit early

            // open AI settings dialog if not ready
            if (!AISettingsReady())
            {
                using var dlg = new SettingsForm();
                dlg.ShowDialog();
            }

            if (!AISettingsReady()) return;

            List<ObjectName>? selectedObjects = SelectObjects(true);

            if (selectedObjects == null || selectedObjects.Count == 0)
            {
                return;
            }

            string additionalInfo = string.Empty;

            // get user input for additional information
            using (var inputBox = new InputBox())
            {
                inputBox.Title = "Description Assistant Plus - Additional Information";
                inputBox.Prompt = "Please enter any additional information or specific requirements for the description (optional):";
                inputBox.Default = "";
                inputBox.Multiline = true;
                inputBox.MaxLength = 4000;
                if (inputBox.ShowDialog() == DialogResult.OK)
                {
                    additionalInfo = inputBox.InputText;
                }
            }

            WorkingMode(true, "⠋ The AI code advisor is working...");
            progressBar.Visible = true;
            objectsTabControl.Enabled = false;
            definitionPanel.Enabled = false;

            try
            {
                if (_currentConnection == null)
                {
                    return;
                }

                // Save any pending edits for the currently opened object before batch processing.
                await definitionPanel.Save();

                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    int percentComplete = (i * 100) / selectedObjects.Count;
                    progressBar.Value = Math.Clamp(percentComplete, 0, 100);
                    messageLabel.Text = $"Processing {percentComplete}%...";

                    var obj = selectedObjects[i];
                    await definitionPanel.OpenAsync(obj, _currentConnection);
                    await definitionPanel.DescribeMissing(additionalInfo);
                    await definitionPanel.Save();
                }

                progressBar.Value = 100;
            }
            finally
            {
                WorkingMode(false, "The AI code advisor has completed its work.");
                progressBar.Visible = false;
                objectsTabControl.Enabled = true;
                definitionPanel.Enabled = true;
            }
        }

        /// <summary>
        /// Checks if the AI settings are ready.
        /// </summary>
        /// <returns><c>true</c> if AI settings are ready; otherwise, <c>false</c>.</returns>
        private static bool AISettingsReady()
        {
            // check if the AI settings are ready
            var aiSettings = AISettingsManager.Current;
            return !string.IsNullOrEmpty(aiSettings.AIApiKey) &&
                !string.IsNullOrEmpty(aiSettings.AIEndpoint)
                && !string.IsNullOrEmpty(aiSettings.AIModel);
        }

        private async void DescribeMissingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open AI settings dialog if not ready
            if (!AISettingsReady())
            {
                using var dlg = new SettingsForm();
                dlg.ShowDialog();
            }

            if (!AISettingsReady()) return;

            await definitionPanel.DescribeMissing(string.Empty);
        }
    }
}