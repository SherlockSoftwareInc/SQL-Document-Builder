using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class NewSQLServerConnectionDialog : Form
    {
        private string _serverName = "";

        public NewSQLServerConnectionDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Authentication type
        /// </summary>
        public short Authentication { get; set; }

        /// <summary>
        /// Connection name
        /// </summary>
        public string? ConnectionName { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Database name
        /// </summary>
        public string? DatabaseName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool RememberPassword { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string? ServerName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authenticationComboBox.SelectedIndex == 0)
            {
                userNameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
                rememberPasswordCheckBox.Enabled = false;
            }
            else
            {
                userNameTextBox.Enabled = true;
                passwordTextBox.Enabled = true;
                rememberPasswordCheckBox.Enabled = true;
            }

            BuildConnectionString();
            EnableOKButton();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private bool BuildConnectionString()
        {
            var result = default(bool);
            string serverName = serverNameTextBox.Text.Trim();
            string dbName = databaseComboBox.Text;
            if (serverName.Length > 0 && dbName.Length > 0)
            {
                //https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-string-syntax
                //bool integratedSecurity = (authenticationComboBox.SelectedIndex == 0);
                //var builder = new System.Data.Odbc.OdbcConnectionStringBuilder()
                //{
                //    DataSource = serverName,
                //    InitialCatalog = dbName,
                //    IntegratedSecurity = integratedSecurity
                //};
                //if (!integratedSecurity)
                //{
                //    builder.UserID = userNameTextBox.Text;
                //    builder.Password = passwordTextBox.Text;
                //}
                OdbcConnectionStringBuilder builder = new()
                {
                    Driver = "ODBC Driver 17 for SQL Server"
                };
                builder.Add("Server", serverName);
                builder.Add("Database", dbName);
                switch (authenticationComboBox.SelectedIndex)
                {
                    case 0:     //Windows Authentication
                        builder.Add("Trusted_Connection", "yes");
                        break;

                    case 1:     //SQL Server Authentication
                        break;

                    case 2:     //Active Directory - Interactive
                        builder.Add("Authentication", "ActiveDirectoryInteractive");
                        break;

                    case 3:     //Active Directory - Integrated
                        builder.Add("Authentication", "ActiveDirectoryIntegrated");
                        break;

                    case 4:     //Active Directory - Password
                        builder.Add("Authentication", "ActiveDirectoryPassword");
                        break;

                    case 5:     //Active Directory -Service Principal
                        builder.Add("Authentication", "ActiveDirectoryServicePrincipal");
                        break;

                    default:
                        break;
                }

                if (userNameTextBox.Enabled && userNameTextBox.Text.Trim().Length > 0)
                    builder.Add("UID", userNameTextBox.Text);
                if (passwordTextBox.Enabled && passwordTextBox.Text.Trim().Length > 0)
                    builder.Add("PWD", passwordTextBox.Text);

                //Driver={ODBC Driver 17 for SQL Server};Server=svmsq06;Database=AFDataMart_DEV;Trusted_Connection=Yes;

                ConnectionString = builder.ConnectionString;
                result = true;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            ConnectionString = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionStringDialog_Load(object sender, EventArgs e)
        {
            int x = serverNameTextBox.Left - 4;
            serverNameLabel.Left = x - serverNameLabel.Width;
            databaseNameLabel.Left = x - databaseNameLabel.Width;

            x = authenticationComboBox.Left - 4;
            authenticationLabel.Left = x - authenticationLabel.Width;
            userNameLabel.Left = x - userNameLabel.Width;
            passwordLabel.Left = x - passwordLabel.Width;

            serverNameTextBox.Text = ServerName;
            databaseComboBox.Text = DatabaseName;
            userNameTextBox.Text = UserName;
            authenticationComboBox.SelectedIndex = Authentication;
            EnableOKButton();
            if (Authentication == 1)
            {
                passwordTextBox.Focus();
            }
        }

        /// <summary>
        /// Handle database name selected index change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (databaseComboBox.SelectedIndex >= 0)
            {
                BuildConnectionString();
                EnableOKButton();
            }
        }

        /// <summary>
        /// Enable OK button:
        /// connection name, server name, database name is required.
        /// If use sql server login, user name is required
        /// </summary>
        private void EnableOKButton()
        {
            bool enabled = (serverNameTextBox.Text.Trim().Length > 1 &&
                databaseComboBox.Text.Trim().Length > 1);
            if (enabled)
            {
                if (authenticationComboBox.SelectedIndex == 1)
                {
                    enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
                }
            }

            okButton.Enabled = enabled;
        }

        /// <summary>
        /// Get database list of a sql server
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private List<string> GetDatabases(string serverName)
        {
            List<String> databases = new();
            //try
            //{
            //    OdbcConnectionStringBuilder connection = new()
            //    {
            //        //DataSource = serverName,
            //        //IntegratedSecurity = true
            //    };

            //    String strConn = connection.ToString();
            //    OdbcConnection sqlConn = new(strConn);
            //    sqlConn.Open();

            //    //get databases
            //    DataTable tblDatabases = sqlConn.GetSchema("Databases");

            //    sqlConn.Close();

            //    foreach (DataRow row in tblDatabases.Rows)
            //    {
            //        String? strDatabaseName = row["database_name"].ToString();
            //        if (strDatabaseName != null) databases.Add(strDatabaseName);
            //    }
            //}
            //catch (OdbcException)
            //{
            //    //Ignore the error
            //}
            //databases.Sort();
            return databases;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (BuildConnectionString())
            {
                ServerName = serverNameTextBox.Text;
                DatabaseName = databaseComboBox.Text;
                ConnectionName = string.Format("{0} @ {1}", DatabaseName, ServerName);
                UserName = userNameTextBox.Text;
                Authentication = (short)authenticationComboBox.SelectedIndex;
                Password = passwordTextBox.Text;
                RememberPassword = rememberPasswordCheckBox.Checked;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                Console.Beep();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            BuildConnectionString();
            EnableOKButton();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            string server = serverNameTextBox.Text;
            if (server != _serverName)
            {
                if (databaseComboBox.SelectedIndex >= 0)
                {
                    databaseComboBox.SelectedIndex = -1;
                    databaseComboBox.Items.Clear();
                    okButton.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Handle server name validated event:
        ///     When server name changed, get the databases in the server
        ///     and list them in the database name combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerNameTextBox_Validated(object sender, EventArgs e)
        {
            string server = serverNameTextBox.Text.Trim();
            if (server != _serverName && server.Length > 1)
            {
                databaseComboBox.Items.Clear();

                _serverName = server;
                Cursor = Cursors.WaitCursor;
                var databases = GetDatabases(server);
                if (databases.Count > 0)
                {
                    databaseComboBox.Items.AddRange(databases.ToArray());
                }

                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Gets the ODBC driver names from the registry.
        /// </summary>
        /// <returns>a string array containing the ODBC driver names, if the registry key is present; null, otherwise.</returns>
        public static string[] GetOdbcDriverNames()
        {
            string[] odbcDriverNames = null;
            using (RegistryKey localMachineHive = Registry.LocalMachine)
            using (RegistryKey odbcDriversKey = localMachineHive.OpenSubKey(@"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers"))
            {
                if (odbcDriversKey != null)
                {
                    odbcDriverNames = odbcDriversKey.GetValueNames();
                }
            }

            return odbcDriverNames;
        }

        /// <summary>
        /// Gets the name of an ODBC driver for Microsoft SQL Server giving preference to the most recent one.
        /// </summary>
        /// <returns>the name of an ODBC driver for Microsoft SQL Server, if one is present; null, otherwise.</returns>
        public static string GetOdbcSqlDriverName()
        {
            List<string> driverPrecedence = new() { "SQL Server Native Client 11.0", "SQL Server Native Client 10.0", "SQL Server Native Client 9.0", "SQL Server" };
            string[] availableOdbcDrivers = GetOdbcDriverNames();
            string? driverName = null;

            if (availableOdbcDrivers != null)
            {
                driverName = driverPrecedence.Intersect(availableOdbcDrivers).FirstOrDefault();
            }

            return driverName;
        }

        private void NewSQLServerConnectionDialog_Load(object sender, EventArgs e)
        {
            foreach (var driver in GetOdbcDriverNames())
            {
                odbcDriverComboBox.Items.Add(driver);
            }
            if (odbcDriverComboBox.Items.Count > 0)
                odbcDriverComboBox.SelectedIndex = 0;
        }
    }
}