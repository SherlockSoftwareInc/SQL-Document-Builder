using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class MainForm : Form
    {
        private readonly SQLServerConnections _connections = new();
        private int _connectionCount = 0;
        private string? _database = string.Empty;
        private SQLDatabaseConnectionItem? _selectedConnection = new SQLDatabaseConnectionItem();
        private string? _server = string.Empty;
        private readonly System.Text.StringBuilder _script = new();

        public MainForm()
        {
            InitializeComponent();
        }

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

        /// <summary>
        /// Append text to the bottom of the text box
        /// </summary>
        /// <param name="text"></param>
        private void AppendLine(string text)
        {
            //sqlTextBox.AppendText(text + Environment.NewLine);
            _script.AppendLine(text);
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
                    _server = connection.ServerName;
                    _database = connection.Database;

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

                    //if (errMessage.Length > 0)
                    //{
                    //    MessageBox.Show(errMessage, Properties.Resources.A005, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                }
            }
        }

        /// <summary>
        /// Convert contents (with tab) from the clipboard to the wiki format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardToolStripButton_Click(object sender, EventArgs e)
        {
            //sqlTextBox.Text = String.Empty;
            //if (Clipboard.ContainsText())
            //{
            //    var builder = new Wiki();
            //    sqlTextBox.Text = builder.TextToWikiTable(Clipboard.GetText());
            //}
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

        /// <summary>
        /// Handles "Exit" menu item click event": Close the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles "Functions" button click event: Generate script for functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FunctionButton_Click(object sender, EventArgs e)
        {
            //_script.Clear();
            //sqlTextBox.Text = "Please wait...";

            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    Server server = new(_server);
            //    if (server != null)
            //    {
            //        Database database = server.Databases[_database];
            //        if (database != null)
            //        {
            //            statusToolStripStatusLabe.Text = "Please wait while generate the scripts...";
            //            progressBar.Value = 0;
            //            progressBar.Visible = true;
            //            Application.DoEvents();

            //            var progress = new Progress<int>(value =>
            //            {
            //                progressBar.Value = value;
            //            });
            //            await Task.Run(() => ScanAllFunctions(dlg.Schema, progress));
            //            progressBar.Visible = false;
            //            sqlTextBox.Text = _script.ToString();
            //        }
            //    }
            //}
            //else
            //{
            //    statusToolStripStatusLabe.Text = string.Empty;
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Build function list in wiki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FunctionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //sqlTextBox.Text = string.Empty;
            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    var builder = new Wiki();
            //    sqlTextBox.Text = builder.BuildFunctionList(dlg.Schema);
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        ///// <summary>
        ///// Generate creation script for tables
        ///// https://chanmingman.wordpress.com/2021/03/26/sql-server-generate-create-table-script-using-c/
        ///// https://www.mssqltips.com/sqlservertip/1833/generate-scripts-for-database-objects-with-smo-for-sql-server/
        ///// https://csharp.hotexamples.com/examples/Microsoft.SqlServer.Management.Smo/Database/-/php-database-class-examples.html
        ///// </summary>
        ///// <param name="myServer"></param>
        //private void GenerateTableScript(Database database, string schemaName, IProgress<int> progress)
        //{
        //    //Scripter scripter = new(myServer);

        //    //Database myDatabase = myServer.Databases["AFDataMart_DEV"];

        //    /* With ScriptingOptions you can specify different scripting
        //     * options, for example to include IF NOT EXISTS, DROP
        //     * statements, output location etc*/

        //    ScriptingOptions scriptOptions = new()
        //    {
        //        ScriptDrops = true,
        //        IncludeIfNotExists = true
        //    };

        //    for (int i = 0; i < database.Tables.Count; i++)
        //    {
        //        var percentComplete = (i * 100) / database.Tables.Count;
        //        progress.Report(percentComplete);

        //        var myTable = database.Tables[i];

        //        bool generate = true;
        //        if (schemaName.Length > 0)
        //        {
        //            if (!schemaName.Equals(myTable.Schema, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                generate = false;
        //            }
        //        }

        //        if (generate)
        //        {
        //            /* Generating IF EXISTS and DROP command for tables */
        //            StringCollection tableScripts = myTable.Script(scriptOptions);
        //            foreach (string? script in tableScripts)
        //            {
        //                if (script != null)
        //                {
        //                    AppendLine(script);
        //                    AppendLine("GO");
        //                }
        //            }

        //            /* Generating CREATE TABLE command */
        //            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.management.smo.scriptingoptions?redirectedfrom=MSDN&view=sql-smo-160
        //            ScriptingOptions options = new()
        //            {
        //                ClusteredIndexes = true,
        //                Default = true,
        //                DriAll = true,
        //                Indexes = true,
        //                IncludeHeaders = false,
        //                ExtendedProperties = true,
        //                SchemaQualify = false,
        //                AllowSystemObjects = false,
        //                ContinueScriptingOnError = true,
        //                NoCollation = true,
        //            };

        //            tableScripts = myTable.Script(options);
        //            foreach (string? script in tableScripts)
        //            {
        //                if (script != null)
        //                {
        //                    string tableScript = script;
        //                    if (myTable.Schema.Length > 0)
        //                    {
        //                        tableScript = script.Replace("CREATE TABLE ", String.Format("CREATE TABLE {0}.", myTable.Schema.QuotedName()));
        //                    }
        //                    AppendLine(tableScript);
        //                    AppendLine("GO");
        //                    AppendLine("");
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Get script of a stored procedure
        /// </summary>
        /// <param name="schema">schema name</param>
        /// <param name="spName">stored procedure name</param>
        /// <returns></returns>
        private void GetSPDefinition(string schema, string spName)
        {
            string sql = string.Format("SELECT definition FROM sys.sql_modules WHERE object_id = (OBJECT_ID(N'{0}.{1}'))", schema.QuotedName(), spName.QuotedName());
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    AppendLine(dr.GetString(0));
                    AppendLine("GO");
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
        /// Get the creation script for a view
        /// </summary>
        /// <param name="viewSchema"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private void GetViewDefinition(string viewSchema, string viewName)
        {
            string sql = string.Format("select definition from sys.objects o join sys.sql_modules m on m.object_id = o.object_id where o.object_id = OBJECT_ID(N'{0}.{1}') and o.type = 'V'", viewSchema.QuotedName(), viewName.QuotedName());
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    AppendLine(dr.GetString(0));
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
        /// Save current connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.dbConnectionString = String.Empty;
            //Properties.Settings.Default.LastAccessConnection = _selectedConnection?.ToString();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles form load event:
        /// Load and populate database connections and set connection to last used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            _connections.Load();
            PopulateConnections();

            var lastConnection = Properties.Settings.Default.LastAccessConnectionIndex;
            ConnectionMenuItem? selectedItem = null;
            if (lastConnection <= 0 || lastConnection >= _connections.Connections.Count)
            {
                selectedItem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[0];
            }
            else
            {
                selectedItem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[lastConnection];
                //foreach (ConnectionMenuItem submenuitem in connectToToolStripMenuItem.DropDown.Items)
                //{
                //    if (submenuitem.ConnectionName == lastConnection.ToString())
                //    {
                //        selectedItem = submenuitem;
                //        break;
                //    }
                //}
            }
            if (selectedItem != null)
            {
                ChangeDBConnection(selectedItem.Connection);
            }
            else
            {
                Close();
            }

            //if (Properties.Settings.Default.LastAccessConnectionIndex == 1)
            //{
            //    LocalToolStripMenuItem_Click(this, e);
            //}
            //else
            //{
            //    AzureToolStripMenuItem_Click(this, e);
            //}
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

                    if (item.ConnectionString?.Length > 1 && item.ConnectionType != "ODBC" )
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
        /// Handles "Save" menu item click event: Save the current content in the text box into a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var oFile = new SaveFileDialog() { Filter = "SQL script(*.sql)|*.sql" };
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                var file = new System.IO.StreamWriter(oFile.FileName, false);
                await file.WriteAsync(sqlTextBox.Text);
                file.Close();
                statusToolStripStatusLabe.Text = "Complete";
            }
        }

        /// <summary>
        /// Scan functions in the database and generate the creation script for them
        /// </summary>
        private void ScanAllFunctions(string? schemaName, IProgress<int> progress)
        {
            DataTable dt = new();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' ORDER BY ROUTINE_NAME", conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 120000
                };
                conn.Open();
                var dr = cmd.ExecuteReader();
                dt.Load(dr);
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

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var percentComplete = (i * 100) / dt.Rows.Count;
                    progress.Report(percentComplete);

                    DataRow dr = dt.Rows[i];
                    string? schema = Convert.ToString(dr["ROUTINE_SCHEMA"]);
                    bool generate = true;
                    if (schemaName?.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string? fnName = Convert.ToString(dr[1]);
                        if (schema != null && fnName != null)
                        {
                            AppendLine("/*====================================================");
                            AppendLine("\t" + fnName);
                            AppendLine("======================================================*/");
                            if (Properties.Settings.Default.UseIfExist)
                                AppendLine(string.Format("DROP FUNCTION IF EXISTS {0}.{1}", schema.QuotedName(), fnName.QuotedName()));
                            else
                            {
                                AppendLine(string.Format("IF OBJECT_ID('{0}.{1}', 'FN') IS NOT NULL", schema.QuotedName(), fnName.QuotedName()));
                                AppendLine(string.Format("\tDROP FUNCTION {0}.{1}", schema.QuotedName(), fnName.QuotedName()));
                            }
                            AppendLine("GO");
                            GetSPDefinition(schema, fnName);
                            AppendLine("GO");
                            if (Properties.Settings.Default.GrantSPPermission)
                            {
                                AppendLine(string.Format("GRANT EXEC ON {0}.{1} TO db_execproc", schema.QuotedName(), fnName.QuotedName()));
                                AppendLine("GO");
                            }
                            AppendLine("");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Scan stored procedures in the database and generate the creation script for them
        /// </summary>
        private void ScanAllSPs(string schemaName, IProgress<int> progress)
        {
            DataTable dt = new();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_NAME", conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 120000
                };
                conn.Open();
                var dr = cmd.ExecuteReader();
                dt.Load(dr);
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

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var percentComplete = (i * 100) / dt.Rows.Count;
                    progress.Report(percentComplete);

                    DataRow dr = dt.Rows[i];
                    string? schema = Convert.ToString(dr[0]);
                    bool generate = true;
                    if (schemaName.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string? spName = Convert.ToString(dr[1]);
                        if (schema != null && spName != null)
                        {
                            AppendLine("/*====================================================");
                            AppendLine("\t" + spName);
                            AppendLine("======================================================*/");
                            if (Properties.Settings.Default.UseIfExist)
                                AppendLine(string.Format("DROP PROCEDURE IF EXISTS {0}.{1}", schema.QuotedName(), spName.QuotedName()));
                            else
                            {
                                AppendLine(string.Format("IF OBJECT_ID('{0}.{1}', 'P') IS NOT NULL", schema.QuotedName(), spName.QuotedName()));
                                AppendLine(string.Format("\tDROP PROCEDURE {0}.{1}", schema.QuotedName(), spName.QuotedName()));
                            }
                            AppendLine("GO");
                            GetSPDefinition(schema, spName);
                            //AppendLine("GO");
                            if (Properties.Settings.Default.GrantSPPermission)
                            {
                                AppendLine(string.Format("GRANT EXEC ON {0}.{1} TO db_execproc", schema.QuotedName(), spName.QuotedName()));
                                AppendLine("GO");
                            }
                            AppendLine("");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Scan views in the database and generate the creation script for them
        /// </summary>
        private void ScanAllViews(string schemaName, IProgress<int> progress)
        {
            DataTable dt = new();
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                // Dim cmd As New SqlCommand("SELECT name AS view_name FROM sys.views where SCHEMA_NAME(schema_id) = 'dbo' order by view_name", conn) With {
                var cmd = new SqlCommand("SELECT SCHEMA_NAME(schema_id) AS view_schema, name AS view_name FROM sys.views", conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 120000
                };
                conn.Open();
                var dr = cmd.ExecuteReader();
                dt.Load(dr);
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

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var percentComplete = (i * 100) / dt.Rows.Count;
                    progress.Report(percentComplete);

                    DataRow dr = dt.Rows[i];
                    string? schema = Convert.ToString(dr[0]);
                    bool generate = true;
                    if (schemaName.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string? viewName = Convert.ToString(dr[1]);
                        if (schema != null && viewName != null)
                        {
                            AppendLine("/*====================================================");
                            AppendLine("\t" + viewName);
                            AppendLine("======================================================*/");
                            if (Properties.Settings.Default.UseIfExist)
                                AppendLine(string.Format("DROP VIEW IF EXISTS {0}.{1}", schema.QuotedName(), viewName.QuotedName()));
                            else
                            {
                                AppendLine(string.Format("IF OBJECT_ID('{0}.{1}', 'V') IS NOT NULL", schema.QuotedName(), viewName.QuotedName()));
                                AppendLine(string.Format("\tDROP VIEW {0}.{1}", schema.QuotedName(), viewName.QuotedName()));
                            }
                            AppendLine("GO");
                            GetViewDefinition(schema, viewName);
                            AppendLine("GO");
                            AppendLine("");
                        }
                    }
                }
            }
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

        private void SingalTableToolStripButton_Click(object sender, EventArgs e)
        {
            if (_selectedConnection != null)
            {
                using var frm = new TableBuilderForm()
                { Connection = _selectedConnection };
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Handles "SPs" button click event: Generate script for stored procedures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StoredProcedureButton_Click(object sender, EventArgs e)
        {
            //_script.Clear();
            //sqlTextBox.Text = "Please wait...";

            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    Server server = new(_server);
            //    if (server != null)
            //    {
            //        Database database = server.Databases[_database];
            //        if (database != null)
            //        {
            //            statusToolStripStatusLabe.Text = "Please wait while generate the scripts...";
            //            progressBar.Value = 0;
            //            progressBar.Visible = true;
            //            Application.DoEvents();

            //            var progress = new Progress<int>(value =>
            //            {
            //                progressBar.Value = value;
            //            });
            //            await Task.Run(() => ScanAllSPs(dlg.Schema, progress));
            //            progressBar.Visible = false;

            //            sqlTextBox.Text = _script.ToString();
            //        }
            //    }
            //    //statusToolStripStatusLabe.Text = "Complete!";
            //}
            //else
            //{
            //    statusToolStripStatusLabe.Text = string.Empty;
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Build stored procedure list in wiki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoredProcedureListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //sqlTextBox.Text = string.Empty;
            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    var builder = new Wiki();
            //    sqlTextBox.Text = builder.BuildSPList(dlg.Schema);
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Handles "Table" button click event: Generate script for tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableButton_Click(object sender, EventArgs e)
        {
            //sqlTextBox.Text = string.Empty;
            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    var builder = new Wiki();
            //    sqlTextBox.Text = builder.BuildTableList(dlg.Schema);
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Hanels "Tables" button click event: Generate script for all tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TablesToolStripButton_Click(object sender, EventArgs e)
        {
            //_script.Clear();
            //sqlTextBox.Text = "Please wait...";

            //if (_server?.Length == 0 || _database?.Length == 0)
            //{
            //    statusToolStripStatusLabe.Text = "No database selected.";
            //}
            //else
            //{
            //    using var dlg = new Schemapicker();
            //    if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //    {
            //        Server server = new(_server);
            //        if (server != null)
            //        {
            //            Database database = server.Databases[_database];
            //            if (database != null)
            //            {
            //                statusToolStripStatusLabe.Text = "Please wait while generate the scripts...";
            //                progressBar.Value = 0;
            //                progressBar.Visible = true;
            //                Application.DoEvents();

            //                var progress = new Progress<int>(value =>
            //                {
            //                    progressBar.Value = value;
            //                });
            //                await Task.Run(() => GenerateTableScript(database, dlg.Schema, progress));
            //                //GenerateTableScript(database, dlg.Schema);
            //                progressBar.Visible = false;
            //                sqlTextBox.Text = _script.ToString();
            //            }
            //        }

            //        statusToolStripStatusLabe.Text = "Complete!";
            //    }
            //    else
            //    {
            //        statusToolStripStatusLabe.Text = string.Empty;
            //    }
            //}
        }

        /// <summary>
        /// Convert tab to row
        /// </summary>
        /// <param name="values"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string TabToRow(string values, string delimiter = "||")
        {
            var columns = values.Split('\t');
            string results = delimiter[..1] + " ";
            for (int i = 0; i < columns.Length; i++)
            {
                if (i == 0)
                {
                    results += columns[i];
                }
                else
                {
                    results += " " + delimiter + " " + columns[i];
                }
            }
            return results;
        }

        /// <summary>
        /// Show task form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TasksToolStripButton_Click(object sender, EventArgs e)
        {
            using var frm = new MigrateForm();
            frm.ShowDialog();
        }

        /// <summary>
        /// Hanels "Views" button click event: Generate script for all views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewButton_Click(object sender, EventArgs e)
        {
            //_script.Clear();
            //sqlTextBox.Text = "Please wait...";

            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    Server server = new(_server);
            //    if (server != null)
            //    {
            //        Database database = server.Databases[_database];
            //        if (database != null)
            //        {
            //            statusToolStripStatusLabe.Text = "Please wait while generate the scripts...";
            //            progressBar.Value = 0;
            //            progressBar.Visible = true;
            //            Application.DoEvents();

            //            var progress = new Progress<int>(value =>
            //            {
            //                progressBar.Value = value;
            //            });
            //            await Task.Run(() => ScanAllViews(dlg.Schema, progress));
            //            progressBar.Visible = false;

            //            sqlTextBox.Text = _script.ToString();
            //        }
            //    }
            //    //statusToolStripStatusLabe.Text = "Complete!";
            //}
            //else
            //{
            //    statusToolStripStatusLabe.Text = string.Empty;
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        /// <summary>
        /// Generate view list in wiki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //sqlTextBox.Text = string.Empty;
            //using var dlg = new Schemapicker();
            //if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            //{
            //    var builder = new Wiki();
            //    sqlTextBox.Text = builder.BuildViewList(dlg.Schema);
            //}
            //statusToolStripStatusLabe.Text = "Complete!";
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        //private async void AzureToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    const string server = "(local)";
        //    const string database = "CSBC_EDW";
        //    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
        //    {
        //        DataSource = server,
        //        InitialCatalog = database,
        //        Encrypt = true,
        //        TrustServerCertificate = true,
        //        IntegratedSecurity = true,
        //        //Authentication = System.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryIntegrated,
        //    };

        //    if (await TestConnection(builder.ConnectionString))
        //    {
        //        _server = server;
        //        _database = database;
        //        serverToolStripStatusLabel.Text = server;
        //        databaseToolStripStatusLabel.Text = database;
        //        Properties.Settings.Default.dbConnectionString = builder.ConnectionString;
        //        Properties.Settings.Default.LastAccessConnectionIndex = 0;

        //        _selectedConnection.ServerName = server;
        //        _selectedConnection.Database = database;
        //    }

        //    //for (int i = 0; i < _connections.Connections.Count; i++)
        //    //{
        //    //    var item = _connections.Connections[i];
        //    //    if (item.ServerName == server && item.Database == database)
        //    //    {
        //    //        _selectedConnection = item;
        //    //    }

        //    //}
        //    //const string server = "phsa-csbc-pcr-prod-sql-server.database.windows.net";
        //    //const string database = "pcr_analytic";
        //    //var builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
        //    //{
        //    //    DataSource = server,
        //    //    InitialCatalog = database,
        //    //    Encrypt = true,
        //    //    TrustServerCertificate = true,
        //    //    //Authentication = System.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryInteractive,
        //    //    UserID = "swu2@phsa.ca"
        //    //};

        //    //if (await TestConnection(builder.ConnectionString))
        //    //{
        //    //    _server = server;
        //    //    _database = database;
        //    //    Properties.Settings.Default.dbConnectionString = builder.ConnectionString;
        //    //}
        //}

        /// <summary>
        /// Test that the server is connected
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns>true if the connection is opened</returns>
        private static async Task<bool> TestConnection(string connectionString)
        {
            using SqlConnection connection = new(connectionString);
            try
            {
                await connection.OpenAsync();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        //private async void LocalToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    const string server = "svmsq06";
        //    const string database = "AFDataMart_DEV";
        //    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
        //    {
        //        DataSource = server,
        //        InitialCatalog = database,
        //        Encrypt = true,
        //        TrustServerCertificate = true,
        //        IntegratedSecurity = true,
        //        //Authentication = System.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryIntegrated,
        //    };

        //    if (await TestConnection(builder.ConnectionString))
        //    {
        //        _server = server;
        //        _database = database;
        //        serverToolStripStatusLabel.Text = server;
        //        databaseToolStripStatusLabel.Text = database;
        //        Properties.Settings.Default.dbConnectionString = builder.ConnectionString;
        //        Properties.Settings.Default.LastAccessConnectionIndex = 1;

        //        _selectedConnection.ServerName = server;
        //        _selectedConnection.Database = database;
        //    }

        //    //for (int i = 0; i < _connections.Connections.Count; i++)
        //    //{
        //    //    var item = _connections.Connections[i];
        //    //    if (item.ServerName == server && item.Database == database)
        //    //    {
        //    //        _selectedConnection = item;
        //    //    }
        //    //}
        //}

        //private void ViewListToolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    sqlTextBox.Text = string.Empty;
        //    using var dlg = new Schemapicker();
        //    if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
        //    {
        //        var builder = new SharePoint();
        //        sqlTextBox.Text = builder.BuildViewList(dlg.Schema);
        //    }
        //    statusToolStripStatusLabe.Text = "Complete!";
        //}

        private void StoredProcedureListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePoint();
                sqlTextBox.Text = builder.BuildSPList(dlg.Schema);
            }
            statusToolStripStatusLabe.Text = "Complete!";
        }

        private void FunctionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            using var dlg = new Schemapicker();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Schema != null)
            {
                var builder = new SharePoint();
                sqlTextBox.Text = builder.BuildFunctionList(dlg.Schema);
            }
            statusToolStripStatusLabe.Text = "Complete!";
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
                }
            }
            statusToolStripStatusLabe.Text = "Complete!";
        }

        private void QueryDataToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new QueryDataToTableForm();
            form.ShowDialog();
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
        /// Output all object descriptions to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OutputDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// Create a new save file dialog
            //SaveFileDialog saveFileDialog = new()
            //{
            //    // Set the file dialog filter to XLSX files
            //    Filter = "Excel Workbook (*.xlsx)|*.xlsx"
            //};

            //// Show the save file dialog and get the result
            //DialogResult result = saveFileDialog.ShowDialog();

            //// Check if the user clicked the OK button
            //if (result == DialogResult.OK)
            //{
            //    // Create a new XLSX workbook
            //    XSSFWorkbook workbook = new();
            //    ISheet sheet = workbook.CreateSheet("Sheet1");

            //    // Write column headers
            //    IRow headerRow = sheet.CreateRow(0);
            //    headerRow.CreateCell(0).SetCellValue("Table Schema");
            //    headerRow.CreateCell(1).SetCellValue("Table Name");
            //    headerRow.CreateCell(2).SetCellValue("Column Name");
            //    headerRow.CreateCell(3).SetCellValue("Description");

            //    statusToolStripStatusLabe.Text = "Please wait while generate the list...";
            //    progressBar.Value = 0;
            //    progressBar.Visible = true;
            //    progressBar.Maximum = 100;
            //    Application.DoEvents();

            //    var progress = new Progress<int>(value =>
            //    {
            //        progressBar.Value = value > progressBar.Maximum ? progressBar.Maximum : value;
            //    });
            //    await Task.Run(() => OutputObjectsDescriptions(sheet, progress));

            //    //GenerateTableScript(database, dlg.Schema);
            //    progressBar.Visible = false;

            //    // Save the workbook to the selected file
            //    using FileStream stream = new(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
            //    workbook.Write(stream);

            //    // Open the file
            //    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            //    {
            //        FileName = saveFileDialog.FileName,
            //        UseShellExecute = true
            //    });

            //    statusToolStripStatusLabe.Text = "Complete";
            //}
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        ///// <summary>
        ///// Fetch and output descriptions for all tables and views
        ///// </summary>
        ///// <param name="sheet"></param>
        ///// <param name="progress"></param>
        //private void OutputObjectsDescriptions(ISheet sheet, IProgress<int> progress)
        //{
        //    var tables = Common.GetObjectList(ObjectName.ObjectTypeEnums.Table);
        //    var views = Common.GetObjectList(ObjectName.ObjectTypeEnums.View);
        //    int total = tables.Count + views.Count;

        //    if (total > 0)
        //    {
        //        int index = 1;
        //        for (int i = 0; i < tables.Count; i++)
        //        {
        //            var percentComplete = (i * 100) / total;
        //            progress.Report(percentComplete);

        //            var tableObject = new DBObject();
        //            tableObject.Open(tables[i], Properties.Settings.Default.dbConnectionString);
        //            index = ObjectToExcel(sheet, index, tableObject);
        //            //index++;
        //        }

        //        for (int i = 0; i < views.Count; i++)
        //        {
        //            var percentComplete = ((i + tables.Count) * 100) / total;
        //            progress.Report(percentComplete);

        //            var tableObject = new DBObject();
        //            tableObject.Open(views[i], Properties.Settings.Default.dbConnectionString);
        //            index = ObjectToExcel(sheet, index, tableObject);
        //            //index++;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Output description of a database object
        ///// </summary>
        ///// <param name="sheet"></param>
        ///// <param name="startRow"></param>
        ///// <param name="dBObject"></param>
        ///// <returns></returns>
        //private int ObjectToExcel(ISheet sheet, int startRow, DBObject dBObject)
        //{
        //    // output object description
        //    IRow objRow = sheet.CreateRow(startRow);
        //    WriteCell(objRow, 0, dBObject.TableSchema);
        //    WriteCell(objRow, 1, dBObject.TableName);
        //    WriteCell(objRow, 3, dBObject.Description);

        //    var index = startRow + 1;
        //    // output column description
        //    foreach (var column in dBObject.Columns)
        //    {
        //        IRow row = sheet.CreateRow(index++);
        //        WriteCell(row, 0, dBObject.TableSchema);
        //        WriteCell(row, 1, dBObject.TableName);
        //        WriteCell(row, 2, column.ColumnName);
        //        WriteCell(row, 3, column.Description);
        //    }

        //    return index;
        //}

        ///// <summary>
        ///// Write a value to a specfic column in a row
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="col"></param>
        ///// <param name="value"></param>
        //private void WriteCell(IRow row, int col, string value)
        //{
        //    // Write cell value
        //    ICell cell = row.CreateCell(col);
        //    cell.SetCellValue(value);
        //}
    }
}