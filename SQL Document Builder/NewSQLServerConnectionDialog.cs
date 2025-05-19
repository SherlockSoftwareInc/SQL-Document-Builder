using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The new sql server connection dialog.
    /// </summary>
    public partial class NewSQLServerConnectionDialog : Form
    {
        private string _serverName = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSQLServerConnectionDialog"/> class.
        /// </summary>
        public NewSQLServerConnectionDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Authentication type
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SqlAuthenticationMethod Authentication { get; set; }

        /// <summary>
        /// Connection name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ConnectionName { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Database name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? DatabaseName { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Password { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RememberPassword { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ServerName { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? UserName { get; set; }

        /// <summary>
        /// Handle authentication type selected index change event:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, SqlAuthenticationMethod>?;

            if (selectedItem.HasValue) // Fix for CS8629: Ensure the nullable value type is not null
            {
                var authenticationMethod = selectedItem.Value.Value; // Fix for CS0029: Access the 'Value' property of the KeyValuePair to get the SqlAuthenticationMethod

                switch (authenticationMethod) // Fix for CS0077: Directly use the value without 'as' since it's a non-nullable value type
                {
                    case SqlAuthenticationMethod.NotSpecified:
                        break;

                    case SqlAuthenticationMethod.SqlPassword:
                    case SqlAuthenticationMethod.ActiveDirectoryPassword:
                    case SqlAuthenticationMethod.ActiveDirectoryServicePrincipal:
                    case SqlAuthenticationMethod.ActiveDirectoryManagedIdentity:
                        userNameTextBox.Enabled = true;
                        passwordTextBox.Enabled = true;
                        rememberPasswordCheckBox.Enabled = true;
                        break;

                    case SqlAuthenticationMethod.ActiveDirectoryIntegrated:
                    case SqlAuthenticationMethod.ActiveDirectoryInteractive:
                        userNameTextBox.Enabled = true;
                        passwordTextBox.Text = String.Empty;
                        passwordTextBox.Enabled = false;
                        rememberPasswordCheckBox.Enabled = false;
                        break;

                    default:
                        userNameTextBox.Enabled = false;
                        passwordTextBox.Text = String.Empty;
                        passwordTextBox.Enabled = false;
                        rememberPasswordCheckBox.Enabled = false;
                        break;
                }
            }

            BuildConnectionString();
            EnableOKButton();
        }

        /// <summary>
        /// Gets the authentication.
        /// </summary>
        /// <returns>A SqlAuthenticationMethod.</returns>
        private SqlAuthenticationMethod GetAuthentication()
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, SqlAuthenticationMethod>?;
            return selectedItem?.Value ?? SqlAuthenticationMethod.ActiveDirectoryIntegrated;
        }

        /// <summary>
        /// Build connection string:
        /// </summary>
        /// <returns></returns>
        private bool BuildConnectionString()
        {
            var result = false;
            string serverName = serverNameTextBox.Text.Trim();
            string dbName = databaseComboBox.Text.Trim();
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(dbName))
            {
                var builder = new SqlConnectionStringBuilder()
                {
                    DataSource = serverName,
                    InitialCatalog = dbName,
                    Encrypt = encrptyCheckBox.Checked,
                    TrustServerCertificate = trustCertificateCheckBox.Checked,
                    Authentication = GetAuthentication()
                };

                if (!string.IsNullOrEmpty(userNameTextBox.Text.Trim()))
                    builder.UserID = userNameTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(passwordTextBox.Text.Trim()))
                    builder.Password = passwordTextBox.Text.Trim();

                ConnectionString = builder.ConnectionString;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Handle cancel button click event:
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
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, SqlAuthenticationMethod>?;
            bool enabled = (!string.IsNullOrEmpty(serverNameTextBox.Text.Trim()) &&
                            !string.IsNullOrEmpty(databaseComboBox.Text.Trim()));
            if (enabled && selectedItem?.Value == SqlAuthenticationMethod.SqlPassword)
            {
                enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
            }
            okButton.Enabled = enabled;
        }

        /// <summary>
        /// Handle form load event:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NewSQLServerConnectionDialog_Load(object sender, EventArgs e)
        {
            // Creating a dictionary mapping authentication method names to SqlAuthenticationMethod values
            Dictionary<string, SqlAuthenticationMethod> authMethods = new()
            {
                { "SQL Server Authentication", SqlAuthenticationMethod.SqlPassword },
                { "Microsoft Entra Integrated", SqlAuthenticationMethod.ActiveDirectoryIntegrated },
                { "Microsoft Entra MFA", SqlAuthenticationMethod.ActiveDirectoryInteractive },
                { "Microsoft Entra Password", Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryPassword },
                { "Microsoft Entra Managed Identity", SqlAuthenticationMethod.ActiveDirectoryManagedIdentity },
                { "Microsoft Entra Service Principal", SqlAuthenticationMethod.ActiveDirectoryServicePrincipal }
            };

            // Adding authentication methods to the combo box using data binding
            authenticationComboBox.DataSource = new BindingSource(authMethods, null);
            authenticationComboBox.DisplayMember = "Key"; // Display the name of the authentication method
            authenticationComboBox.ValueMember = "Value"; // Use the SqlAuthenticationMethod as the value
            //authenticationComboBox.SelectedIndex = 0;

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
            authenticationComboBox.SelectedValue = Authentication;
            EnableOKButton();
            if (Authentication == SqlAuthenticationMethod.SqlPassword || Authentication == SqlAuthenticationMethod.ActiveDirectoryPassword)
            {
                passwordTextBox.Focus();
            }
        }

        /// <summary>
        /// Handle OK button click event:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (BuildConnectionString())
            {
                ServerName = serverNameTextBox.Text.Trim();
                DatabaseName = databaseComboBox.Text.Trim();
                ConnectionName = $"{DatabaseName} @ {ServerName}";
                UserName = userNameTextBox.Text.Trim();
                Authentication = GetAuthentication();
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
        /// Handle password text box text changed event:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            BuildConnectionString();
            EnableOKButton();
        }

        /// <summary>
        /// Handle server name text box text changed event:
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
        private async void ServerNameTextBox_Validated(object sender, EventArgs e)
        {
            string server = serverNameTextBox.Text.Trim();
            if (server != _serverName && server.Length > 1)
            {
                databaseComboBox.Items.Clear();

                _serverName = server;
                Cursor = Cursors.WaitCursor;

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var databases = await DatabaseHelper.GetDatabases(server, cancellationTokenSource.Token);

                    if (databases.Count > 0 && !cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        databaseComboBox.Items.AddRange(databases.ToArray());
                    }

                    if (databaseComboBox.Items.Count > 0)
                    {
                        databaseComboBox.SelectedIndex = 0;
                    }
                }

                Cursor = Cursors.Default;
            }
        }
    }
}