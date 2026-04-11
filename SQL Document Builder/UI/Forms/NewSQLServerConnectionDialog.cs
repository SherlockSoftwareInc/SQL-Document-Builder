using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents a dialog window for creating a new SQL Server connection.
    /// Allows the user to specify server, authentication, database, and connection options.
    /// </summary>
    public partial class NewSQLServerConnectionDialog : Form
    {
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
        /// Gets or sets the connection type.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ConnectionType { get; set; } = "SQL Server";

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
        /// Gets or sets the ODBC DSN name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DSN { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the DBMS type for the connection.
        /// </summary>
        public DBMSTypeEnums DBMSType { get; private set; }

        /// <summary>
        /// Gets or sets whether SQL connections are encrypted.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EncryptConnection { get; set; } = true;

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
        /// Gets or sets whether ODBC manual login is required.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RequireManualLogin { get; set; }

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
        /// Gets or sets the selected ODBC connection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DatabaseConnectionItem? SelectedConnection { get; set; }

        /// <summary>
        /// Gets or sets whether server certificate is trusted for SQL connections.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TrustServerCertificate { get; set; } = true;

        /// <summary>
        /// Gets or sets the user name for authentication, if required.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? UserName { get; set; }

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
        /// Handles the form load event.
        /// Initializes the SQL and ODBC setting controls.
        /// </summary>
        private void NewSQLServerConnectionDialog_Load(object sender, EventArgs e)
        {
            sqlConnSettingBox.ServerName = ServerName ?? string.Empty;
            sqlConnSettingBox.DatabaseName = DatabaseName ?? string.Empty;
            sqlConnSettingBox.UserName = UserName ?? string.Empty;
            sqlConnSettingBox.Password = Password ?? string.Empty;
            sqlConnSettingBox.Authentication = (short)Authentication;
            sqlConnSettingBox.RememberPassword = RememberPassword;
            sqlConnSettingBox.EncryptConnection = EncryptConnection;
            sqlConnSettingBox.TrustServerCertificate = TrustServerCertificate;

            odbcConnSettingBox.DSN = DSN;
            odbcConnSettingBox.UserName = UserName ?? string.Empty;
            odbcConnSettingBox.Password = Password ?? string.Empty;
            odbcConnSettingBox.RememberPassword = RememberPassword;
            odbcConnSettingBox.RequireManualLogin = RequireManualLogin;

            dbDescTextBox.Text = DatabaseDescription;

            connectionTypeComboBox.SelectedIndex = string.Equals(ConnectionType, "ODBC", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            ConnectionTypeComboBox_SelectedIndexChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Enables or disables the OK button based on required field validation.
        /// </summary>
        private void EnableOKButton()
        {
            if (connectionTypeComboBox.SelectedIndex == 1)
            {
                okButton.Enabled = odbcConnSettingBox.CanSave;
                return;
            }

            okButton.Enabled = sqlConnSettingBox.CanSave;
        }

        /// <summary>
        /// Handles connection type changes and toggles between SQL and ODBC settings.
        /// </summary>
        private void ConnectionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (connectionTypeComboBox.SelectedIndex == 1)
            {
                sqlConnSettingBox.Visible = false;
                sqlConnSettingBox.SendToBack();
                odbcConnSettingBox.Visible = true;
                odbcConnSettingBox.BringToFront();
            }
            else
            {
                odbcConnSettingBox.Visible = false;
                odbcConnSettingBox.SendToBack();
                sqlConnSettingBox.Visible = true;
                sqlConnSettingBox.BringToFront();
            }

            EnableOKButton();
        }

        /// <summary>
        /// Handles the OK button click event.
        /// Validates and saves the connection details, then closes the dialog.
        /// </summary>
        private void OkButton_Click(object sender, EventArgs e)
        {
            ConnectionType = connectionTypeComboBox.SelectedIndex == 1 ? "ODBC" : "SQL Server";

            if (connectionTypeComboBox.SelectedIndex == 1)
            {
                if (odbcConnSettingBox.CanSave)
                {
                    SelectedConnection = odbcConnSettingBox.SelectedConnection;
                    DSN = odbcConnSettingBox.DSN;
                    DBMSType = odbcConnSettingBox.DBMSType;
                    UserName = odbcConnSettingBox.UserName;
                    Password = odbcConnSettingBox.Password;
                    RememberPassword = odbcConnSettingBox.RememberPassword;
                    RequireManualLogin = odbcConnSettingBox.RequireManualLogin;
                    ConnectionName = string.IsNullOrWhiteSpace(DSN) ? ConnectionName : DSN;
                    ConnectionString = string.Empty;
                    DatabaseDescription = dbDescTextBox.Text.Trim();
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                Console.Beep();
                return;
            }

            if (sqlConnSettingBox.CanSave)
            {
                ServerName = sqlConnSettingBox.ServerName;
                DatabaseName = sqlConnSettingBox.DatabaseName;
                ConnectionName = sqlConnSettingBox.ConnectionName;
                DBMSType = DBMSTypeEnums.SQLServer;
                UserName = sqlConnSettingBox.UserName;
                Authentication = (AuthenticationMethod)sqlConnSettingBox.Authentication;
                Password = sqlConnSettingBox.Password;
                RememberPassword = sqlConnSettingBox.RememberPassword;
                EncryptConnection = sqlConnSettingBox.EncryptConnection;
                TrustServerCertificate = sqlConnSettingBox.TrustServerCertificate;
                ConnectionString = sqlConnSettingBox.ConnectionString;
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
            EnableOKButton();
        }
    }
}