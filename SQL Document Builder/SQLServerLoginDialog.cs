using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The s q l server login dialog.
    /// </summary>
    public partial class SQLServerLoginDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLServerLoginDialog"/> class.
        /// </summary>
        public SQLServerLoginDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SqlAuthenticationMethod Authentication { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ServerName { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? UserName { get; set; }

        /// <summary>
        /// Handles the selected index changed event of the authentication combo box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <returns>A bool.</returns>
        private bool BuildConnectionString()
        {
            var result = default(bool);
            string serverName = serverNameTextBox.Text.Trim();
            string dbName = databaseComboBox.Text;
            if (serverName.Length > 0 && dbName.Length > 0)
            {
                SqlConnectionStringBuilder builder = new()
                {
                    //builder.Add("Server", "phsa-csbc-pcr-prod-sql-server.database.windows.net");
                    //builder.Add("Database", "pcr_analytic");
                    //builder.Add("Authentication", "ActiveDirectoryInteractive");
                    //builder.Add("UID", userNameTextBox.Text);
                    //builder.Add("PWD", passwordTextBox.Text);
                    { "Server", serverName },
                    { "Database", dbName }
                };
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

        /// <summary>
        /// Handles the cancel button click event of the cancel button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            ConnectionString = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the load event of the connection string dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ConnectionStringDialog_Load(object sender, EventArgs e)
        {
            serverNameTextBox.Text = ServerName;
            databaseComboBox.Text = DatabaseName;
            userNameTextBox.Text = UserName;
            authenticationComboBox.SelectedValue = Authentication;
            EnableOKButton();
            if (Authentication == SqlAuthenticationMethod.SqlPassword)
                passwordTextBox.Focus();
        }

        /// <summary>
        /// Enables the ok button.
        /// </summary>
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

        /// <summary>
        /// Handles the OK button click event of the OK button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (BuildConnectionString())
            {
                ServerName = serverNameTextBox.Text;
                DatabaseName = databaseComboBox.Text;
                UserName = userNameTextBox.Text;
                Password = passwordTextBox.Text;
                var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, SqlAuthenticationMethod>?;
                Authentication = selectedItem?.Value ?? SqlAuthenticationMethod.NotSpecified;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                Console.Beep();
            }
        }

        /// <summary>
        /// Handles the text changed event of the server name text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            BuildConnectionString();
            EnableOKButton();
        }
    }
}