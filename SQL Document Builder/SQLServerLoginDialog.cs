using System;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class SQLServerLoginDialog : Form
    {
        public SQLServerLoginDialog()
        {
            InitializeComponent();
        }

        public short Authentication { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? Password { get; set; }
        public string? ServerName { get; set; }
        public string? UserName { get; set; }

        private void AuthenticationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authenticationComboBox.SelectedIndex == 0)
            {
                userNameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
            }
            else
            {
                userNameTextBox.Enabled = true;
                passwordTextBox.Enabled = true;
            }

            BuildConnectionString();
            EnableOKButton();
        }

        private bool BuildConnectionString()
        {
            var result = default(bool);
            string serverName = serverNameTextBox.Text.Trim();
            string dbName = databaseComboBox.Text;
            if (serverName.Length > 0 && dbName.Length > 0)
            {
                //bool integratedSecurity = authenticationComboBox.SelectedIndex == 0;
                //var builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
                //{
                //DataSource = serverName,
                //InitialCatalog = dbName,
                //IntegratedSecurity = integratedSecurity
                //};
                //if (!integratedSecurity)
                //{
                //builder.UserID = userNameTextBox.Text;
                //builder.Password = passwordTextBox.Text;
                //}

                SqlConnectionStringBuilder builder = new()
                {
                    //Driver = "Sql Driver 17 for SQL Server"
                };
                //builder.Add("Server", "phsa-csbc-pcr-prod-sql-server.database.windows.net");
                //builder.Add("Database", "pcr_analytic");
                //builder.Add("Authentication", "ActiveDirectoryInteractive");
                //builder.Add("UID", userNameTextBox.Text);
                //builder.Add("PWD", passwordTextBox.Text);
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

                ConnectionString = builder.ConnectionString;
                result = true;
            }

            return result;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ConnectionString = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ConnectionStringDialog_Load(object sender, EventArgs e)
        {
            serverNameTextBox.Text = ServerName;
            databaseComboBox.Text = DatabaseName;
            userNameTextBox.Text = UserName;
            authenticationComboBox.SelectedIndex = Authentication;
            EnableOKButton();
            if (Authentication == 1)
                passwordTextBox.Focus();
        }

        private void EnableOKButton()
        {
            bool enabled = (serverNameTextBox.Text.Trim().Length > 1 && databaseComboBox.Text.Trim().Length > 1);
            if (enabled)
            {
                if (authenticationComboBox.SelectedIndex == 1)
                {
                    enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
                }
            }

            okButton.Enabled = enabled;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (BuildConnectionString())
            {
                ServerName = serverNameTextBox.Text;
                DatabaseName = databaseComboBox.Text;
                UserName = userNameTextBox.Text;
                Password = passwordTextBox.Text;
                Authentication = (short)authenticationComboBox.SelectedIndex;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                Console.Beep();
            }
        }

        private void ServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            BuildConnectionString();
            EnableOKButton();
        }
    }
}