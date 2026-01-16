using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents a dialog window for creating a new SQL Server connection.
    /// Allows the user to specify server, authentication, database, and connection options.
    /// </summary>
    public partial class NewSQLServerConnectionDialog : Form
    {
        private string _serverName = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSQLServerConnectionDialog"/> class.
        /// Sets up the UI and applies dark mode if enabled in settings.
        /// </summary>
        public NewSQLServerConnectionDialog()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets the authentication method for the SQL Server connection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AuthenticationMethod Authentication { get; set; }

        /// <summary>
        /// Gets or sets the display name for the connection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets the constructed database connection string.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the database to connect to.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication, if required.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the password should be remembered.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RememberPassword { get; set; }

        /// <summary>
        /// Gets or sets the description of the database.
        /// </summary>
        public string DatabaseDescription { get; set; }

        /// <summary>
        /// Gets or sets the SQL Server instance name or address.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ServerName { get; set; }

        /// <summary>
        /// Gets or sets the user name for authentication, if required.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? UserName { get; set; }

        /// <summary>
        /// Handles the event when the authentication type selection changes.
        /// Enables or disables user/password fields based on the selected authentication method.
        /// </summary>
        private void AuthenticationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, AuthenticationMethod>?;

            if (selectedItem.HasValue)
            {
                var authenticationMethod = selectedItem.Value.Value;

                switch (authenticationMethod)
                {
                    case AuthenticationMethod.NotSpecified:
                        break;

                    case AuthenticationMethod.SqlPassword:
                    case AuthenticationMethod.ActiveDirectoryPassword:
                    case AuthenticationMethod.ActiveDirectoryServicePrincipal:
                    case AuthenticationMethod.ActiveDirectoryManagedIdentity:
                        userNameTextBox.Enabled = true;
                        passwordTextBox.Enabled = true;
                        rememberPasswordCheckBox.Enabled = true;
                        break;

                    case AuthenticationMethod.ActiveDirectoryIntegrated:
                    case AuthenticationMethod.ActiveDirectoryInteractive:
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
        /// Gets the currently selected authentication method from the combo box.
        /// </summary>
        /// <returns>The selected <see cref="AuthenticationMethod"/>.</returns>
        private AuthenticationMethod GetAuthentication()
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, AuthenticationMethod>?;
            return selectedItem?.Value ?? AuthenticationMethod.ActiveDirectoryIntegrated;
        }

        /// <summary>
        /// Builds the SQL Server connection string based on the current form values.
        /// </summary>
        /// <returns><c>true</c> if the connection string was built successfully; otherwise, <c>false</c>.</returns>
        private bool BuildConnectionString()
        {
            var result = false;
            string serverName = serverNameTextBox.Text.Trim();
            string dbName = databaseComboBox.Text.Trim();
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(dbName))
            {
                var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
                {
                    DataSource = serverName,
                    InitialCatalog = dbName,
                    Encrypt = encrptyCheckBox.Checked,
                    TrustServerCertificate = trustCertificateCheckBox.Checked,
                    Authentication = (Microsoft.Data.SqlClient.SqlAuthenticationMethod)GetAuthentication()
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
        /// Handles the Cancel button click event.
        /// Closes the dialog and clears the connection string.
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            ConnectionString = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the event when the selected database changes.
        /// Rebuilds the connection string and updates the OK button state.
        /// </summary>
        private void DatabaseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (databaseComboBox.SelectedIndex >= 0)
            {
                BuildConnectionString();
                EnableOKButton();
            }
        }

        /// <summary>
        /// Enables or disables the OK button based on required field validation.
        /// Requires server name and database name; for SQL authentication, also requires user name and password.
        /// </summary>
        private void EnableOKButton()
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, AuthenticationMethod>?;
            bool enabled = (!string.IsNullOrEmpty(serverNameTextBox.Text.Trim()) &&
                            !string.IsNullOrEmpty(databaseComboBox.Text.Trim()));
            if (enabled && selectedItem?.Value == AuthenticationMethod.SqlPassword)
            {
                enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
            }
            okButton.Enabled = enabled;
        }

        /// <summary>
        /// Handles the form load event.
        /// Initializes authentication options, positions labels, and populates fields with existing values.
        /// </summary>
        private void NewSQLServerConnectionDialog_Load(object sender, EventArgs e)
        {
            // Creating a dictionary mapping authentication method names to AuthenticationMethod values
            Dictionary<string, AuthenticationMethod> authMethods = new()
            {
                { "SQL Server Authentication", AuthenticationMethod.SqlPassword },
                { "Microsoft Entra Integrated", AuthenticationMethod.ActiveDirectoryIntegrated },
                { "Microsoft Entra MFA", AuthenticationMethod.ActiveDirectoryInteractive },
                { "Microsoft Entra Password", AuthenticationMethod.ActiveDirectoryPassword },
                { "Microsoft Entra Managed Identity", AuthenticationMethod.ActiveDirectoryManagedIdentity },
                { "Microsoft Entra Service Principal", AuthenticationMethod.ActiveDirectoryServicePrincipal }
            };

            // Adding authentication methods to the combo box using data binding
            authenticationComboBox.DataSource = new BindingSource(authMethods, null);
            authenticationComboBox.DisplayMember = "Key";
            authenticationComboBox.ValueMember = "Value";

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
            dbDescTextBox.Text = DatabaseDescription;
            EnableOKButton();
            if (Authentication == AuthenticationMethod.SqlPassword || Authentication == AuthenticationMethod.ActiveDirectoryPassword)
            {
                passwordTextBox.Focus();
            }
        }

        /// <summary>
        /// Handles the OK button click event.
        /// Validates and saves the connection details, then closes the dialog.
        /// </summary>
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
                DatabaseDescription = dbDescTextBox.Text.Trim();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                Console.Beep();
            }
        }

        /// <summary>
        /// Handles changes to settings-related fields (e.g., user name, password, checkboxes).
        /// Rebuilds the connection string and updates the OK button state.
        /// </summary>
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            BuildConnectionString();
            EnableOKButton();
        }

        /// <summary>
        /// Handles changes to the server name text box.
        /// Resets the database selection if the server name changes.
        /// </summary>
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
        /// Handles the server name validated event.
        /// When the server name changes, retrieves the list of databases from the server and populates the database combo box.
        /// </summary>
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
                    var databases = await SQLDatabaseHelper.GetDatabases(server, cancellationTokenSource.Token);

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