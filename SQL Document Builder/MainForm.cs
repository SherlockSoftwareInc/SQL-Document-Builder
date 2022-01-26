using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class MainForm : Form
    {
        private readonly SQLServerConnections _connections = new SQLServerConnections();
        private int _connectionCount = 0;
        private SQLDatabaseConnectionItem? _selectedConnection;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show input box
        /// </summary>
        /// <param name="prompt">Prompt text</param>
        /// <param name="title">Title of the input box</param>
        /// <param name="defaultValue">default value</param>
        /// <returns></returns>
        private static string InputBox(string prompt, string title, string defaultValue)
        {
            using (var dlg = new InputBox()
            {
                Title = title,
                Prompt = prompt,
                Default = defaultValue
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.InputText;
                }
            }
            return "";
        }

        /// <summary>
        /// Open add connection dialog and start to add a new database connection
        /// </summary>
        /// <returns></returns>
        private bool AddConnection()
        {
            using (var dlg = new NewSQLServerConnectionDialog())
            {
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

                    //dataSourcesToolStripComboBox.Items.Add(connection);

                    var submenuitem = new ConnectionMenuItem(connection)
                    {
                        Name = string.Format("ConnectionMenuItem{0}", _connectionCount++),
                        Size = new Size(300, 26),
                    };
                    submenuitem.Click += OnConnectionToolStripMenuItem_Click;
                    connectToToolStripMenuItem.DropDown.Items.Add(submenuitem);

                    return true;
                }
            }
            return false;
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
        /// Scan user tables in the database and generate the creation script for them
        /// </summary>
        private void BuildTableListWiki()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("|-");
                AppendLine("! Schema !! Table Name !! Description");
                var cmd = new SqlCommand("SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables order by table_schema, table_name", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tableSchema = dr[0].ToString();
                    string tableName = dr[1].ToString();
                    AppendLine("|-");
                    AppendLine(string.Format("| {0} || [[DW Table: {0}.{1}|{1}]] || ", tableSchema, tableName));
                    //if (tableSchema == "dbo")
                    //{
                    //AppendLine("/*====================================================");
                    //AppendLine("\t" + string.Format("DW Table: {0}.{1}", tableSchema, tableName));
                    //AppendLine("======================================================*/");
                    //AppendLine(string.Format("===TABLE NAME: {0}.{1}===", tableSchema, tableName));
                    //AppendLine("{| class=\"wikitable\"");
                    //AppendLine("|-");
                    //AppendLine("! Col ID !! Name !! Data Type !! Description");
                    //GetTableDefinition(tableSchema, tableName);
                    //AppendLine("|}");
                    //AppendLine("</br>");
                    //AppendLine("----");
                    //AppendLine("Back to [[DW: Database tables|Database tables]]");
                    //AppendLine("[[Category: CSBC data warehouse]]");
                    //// AppendLine("")
                    //// AppendLine("")
                    //AppendLine("");
                    //}
                }

                dr.Close();
                AppendLine("|}");
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
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

                    string connectionString = connection.ConnectionString.Length == 0 ? connection.Login() : connection.ConnectionString;

                    //string errMessage;
                    if (connectionString.Length > 0)
                    {
                        //errMessage = dbObjects.Open(connection);

                        //if (errMessage.Length > 0)
                        //{
                        //    connection.ConnectionString = "";
                        //    connection.Password = "";
                        //}
                        //else
                        //{
                        serverToolStripStatusLabel.Text = connection.ServerName;
                        databaseToolStripStatusLabel.Text = connection.Database;
                        Properties.Settings.Default.dbConnectionString = connectionString;
                        //}
                    }

                    //if (errMessage.Length > 0)
                    //{
                    //    MessageBox.Show(errMessage, Properties.Resources.A005, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                }
            }
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
        private void FunctionButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            ScanAllFunctions();
        }

        /// <summary>
        /// Generate creation script for tables
        /// https://chanmingman.wordpress.com/2021/03/26/sql-server-generate-create-table-script-using-c/
        /// https://csharp.hotexamples.com/examples/Microsoft.SqlServer.Management.Smo/Database/-/php-database-class-examples.html
        /// </summary>
        /// <param name="myServer"></param>
        private void GenerateTableScript(Server myServer)
        {
            Scripter scripter = new Scripter(myServer);

            //Database myAdventureWorks = myServer.Databases[“AdventureWorks”];
            Database myAdventureWorks = myServer.Databases["AFDataMart_DEV"];

            /* With ScriptingOptions you can specify different scripting
             * options, for example to include IF NOT EXISTS, DROP
             * statements, output location etc*/

            ScriptingOptions scriptOptions = new ScriptingOptions()
            {
                ScriptDrops = true,
                IncludeIfNotExists = true
            };

            foreach (Table myTable in myAdventureWorks.Tables)
            {
                /* Generating IF EXISTS and DROP command for tables */
                StringCollection tableScripts = myTable.Script(scriptOptions);
                foreach (string script in tableScripts)
                {
                    AppendLine(script);
                    AppendLine("GO");
                }

                /* Generating CREATE TABLE command */
                tableScripts = myTable.Script();
                foreach (string script in tableScripts)
                {
                    AppendLine(script);
                    AppendLine("GO");
                }
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
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Get script of a stored procedure
        /// </summary>
        /// <param name="schema">schema name</param>
        /// <param name="spName">stored procedure name</param>
        /// <returns></returns>
        private void GetSPDefinition(string schema, string spName)
        {
            string sql = string.Format("SELECT definition FROM sys.sql_modules WHERE object_id = (OBJECT_ID(N'[{0}].[{1}]'))", schema, spName);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    AppendLine(dr[0].ToString());
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Get table structure for wiki
        /// </summary>
        /// <param name="TableSchema">Schame name</param>
        /// <param name="TableName">Table name</param>
        /// <returns></returns>
        private void GetTableDefinition(string TableSchema, string TableName)
        {
            string sql = string.Format("SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = N'{0}' AND TABLE_NAME = N'{1}' ORDER BY ORDINAL_POSITION", TableSchema, TableName);
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

                    AppendLine(string.Format("| {0} || {1} || {2} || {3}", colID, colName, dataType, GetColumnDesc(TableSchema, TableName, colName)));
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
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
        private void GetTableDefWiki(string tableSchema, string tableName)
        {
            AppendLine(string.Format("===TABLE NAME: {0}.{1}===", tableSchema, tableName));
            AppendLine("{| class=\"wikitable\"");
            AppendLine("|-");
            AppendLine("! Col ID !! Name !! Data Type !! Description");
            GetTableDefinition(tableSchema, tableName);
            AppendLine("|}");
            AppendLine("</br>");
            AppendLine("----");
            AppendLine("Back to [[DW: Database tables|Database tables]]");
            AppendLine("[[Category: CSBC data warehouse]]");
        }

        /// <summary>
        /// Build value list of the given table for wiki
        /// </summary>
        /// <param name="tableName"></param>
        private void GetTableValues(string tableName)
        {
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
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            AppendLine("|}");
            AppendLine("</br>");

            statusToolStripStatusLabe.Text = "Complete";
        }

        /// <summary>
        /// Get the creation script for a view
        /// </summary>
        /// <param name="viewSchema"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private void GetViewDefinition(string viewSchema, string viewName)
        {
            string sql = string.Format("select definition from sys.objects o join sys.sql_modules m on m.object_id = o.object_id where o.object_id = OBJECT_ID(N'[{0}].[{1}]') and o.type = 'V'", viewSchema, viewName);
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    AppendLine(dr[0].ToString());
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
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
            Properties.Settings.Default.LastAccessConnection = _selectedConnection?.ToString();
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

            var lastConnection = Properties.Settings.Default.LastAccessConnection;
            ConnectionMenuItem? selectedItem = null;
            if (lastConnection.Length == 0)
            {
                selectedItem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[0];
            }
            else
            {
                foreach (ConnectionMenuItem submenuitem in connectToToolStripMenuItem.DropDown.Items)
                {
                    if (submenuitem.ConnectionName == lastConnection.ToString())
                    {
                        selectedItem = submenuitem;
                        break;
                    }
                }
            }
            if (selectedItem != null)
            {
                ChangeDBConnection(selectedItem.Connection);
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        private void MsgBox(string message, MessageBoxIcon icon)
        {
            MessageBox.Show(message, "Message", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Handle connection menu item click event:
        ///     Open selected connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(ConnectionMenuItem))
            {
                ConnectionMenuItem menuItem = (ConnectionMenuItem)sender;

                statusToolStripStatusLabe.Text = string.Format("Connect to {0}...", menuItem.ToString());
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                ChangeDBConnection(menuItem.Connection);

                Cursor = Cursors.Default;
                statusToolStripStatusLabe.Text = "";

                //for (int i = 0; i < dataSourcesToolStripComboBox.ComboBox.Items.Count; i++)
                //{
                //    if (menuItem.Connection.Equals((SQLDatabaseConnectionItem)dataSourcesToolStripComboBox.ComboBox.Items[i]))
                //    {
                //        dataSourcesToolStripComboBox.SelectedIndex = i;
                //    }
                //}
            }
        }

        /// <summary>
        /// Handles "Paste" menu item click event: Paste the text from the clipboard to the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
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
                ConnectionMenuItem submenuitem = (ConnectionMenuItem)connectToToolStripMenuItem.DropDown.Items[i];
                submenuitem.Click -= OnConnectionToolStripMenuItem_Click;
                connectToToolStripMenuItem.DropDownItems.RemoveAt(i);
            }

            //// clear connections combobox
            //if (dataSourcesToolStripComboBox.Items.Count > 0)
            //{
            //    dataSourcesToolStripComboBox.Items.Clear();
            //}

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
                    if (item.ConnectionString.Length > 1)
                    {
                        //dataSourcesToolStripComboBox.Items.Add(item);

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
        /// Remove quota from the object name
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveQuota(string text)
        {
            if (text.StartsWith("[") && text.EndsWith("]"))
            {
                return text.Substring(1, text.Length - 2);
            }
            else
            {
                return text;
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
        private void ScanAllFunctions()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' ORDER BY ROUTINE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string schema = Convert.ToString(dr["ROUTINE_SCHEMA"]);
                    string spName = dr[1].ToString();
                    AppendLine("/*====================================================");
                    AppendLine("\t" + spName);
                    AppendLine("======================================================*/");
                    AppendLine(string.Format("DROP FUNCTION IF EXISTS [{0}].[{1}]", schema, spName));
                    AppendLine("GO");
                    GetSPDefinition(schema, spName);
                    AppendLine("GO");
                    AppendLine(string.Format("--GRANT EXEC ON [{0}].[{1}] TO [db_execproc]", schema, spName));
                    AppendLine("--GO");
                    AppendLine("");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Scan stored procedures in the database and generate the creation script for them
        /// </summary>
        private void ScanAllSPs()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string schema = dr[0].ToString();
                    string spName = dr[1].ToString();
                    AppendLine("/*====================================================");
                    AppendLine("\t" + spName);
                    AppendLine("======================================================*/");
                    AppendLine(string.Format("DROP PROCEDURE IF EXISTS [{0}].[{1}]", schema, spName));
                    AppendLine("GO");
                    GetSPDefinition(schema, spName);
                    AppendLine("GO");
                    AppendLine(string.Format("GRANT EXEC ON [{0}].[{1}] TO [db_execproc]", schema, spName));
                    AppendLine("GO");
                    AppendLine("");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Scan views in the database and generate the creation script for them
        /// </summary>
        private void ScanAllViews()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                // Dim cmd As New SqlCommand("SELECT name AS view_name FROM sys.views where SCHEMA_NAME(schema_id) = 'dbo' order by view_name", conn) With {
                var cmd = new SqlCommand("SELECT SCHEMA_NAME(schema_id) AS view_schema, name AS view_name FROM sys.views", conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string viewSchema = dr[0].ToString();
                    string viewName = dr[1].ToString();
                    AppendLine("/*====================================================");
                    AppendLine("\t" + viewName);
                    AppendLine("======================================================*/");
                    AppendLine(string.Format("DROP VIEW IF EXISTS [{0}].[{1}]", viewSchema, viewName));
                    AppendLine("GO");
                    GetViewDefinition(viewSchema, viewName);
                    AppendLine("GO");
                    AppendLine("");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
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

        /// <summary>
        /// Handles "SPs" button click event: Generate script for stored procedures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoredProcedureButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            ScanAllSPs();
            statusToolStripStatusLabe.Text = "Complete";
        }

        /// <summary>
        /// Handles "Table" button click event: Generate script for tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            BuildTableListWiki();
        }

        /// <summary>
        /// Hanels "Tables" button click event: Generate script for all tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TablesToolStripButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = String.Empty;
            GenerateTableScript(new Server(@"svmsq06"));
        }

        /// <summary>
        /// Handles "Table wiki" button click event: Generate wiki content of table structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableWikiButton_Click(object sender, EventArgs e)
        {
            string tableName = InputBox("Table name:", "Input table name", "dbo.");
            if (tableName.IndexOf(".") > 0)
            {
                sqlTextBox.Text = string.Empty;
                var tableElement = tableName.Split('.');
                if (tableElement.Length == 2)
                {
                    GetTableDefWiki(RemoveQuota(tableElement[0]), RemoveQuota(tableElement[1]));
                    statusToolStripStatusLabe.Text = "Complete";
                }
                else
                {
                    statusToolStripStatusLabe.Text = "Failed";
                }
            }
        }

        /// <summary>
        /// Handles "Value wiki" button click event: generate wiki content of a table value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueWikiButton_Click(object sender, EventArgs e)
        {
            string tableName = InputBox("Table name:", "Input table name", "dbo.");
            if (tableName.IndexOf(".") > 0)
            {
                GetTableValues(tableName);
            }
        }

        /// <summary>
        /// Hanels "Views" button click event: Generate script for all views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
            ScanAllViews();
            //messageLabel.Text = "Complete";
        }
    }
}