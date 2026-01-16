
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The connection manage form.
    /// </summary>
    public partial class ConnectionManageForm : Form
    {
        internal CancellationTokenSource? _cancellation = null;
        private bool _changed = false;
        private DatabaseConnections _connections = new();
        private DatabaseConnectionItem? _currentConnectionItem;
        private bool _populating = false;
        private int _selectedIndex = -1;    // index of current connect in the connection list

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManageForm"/> class.
        /// </summary>
        public ConnectionManageForm()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets data connections object
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DatabaseConnections DataConnections
        {
            get { return _connections; }
            set
            {
                _connections = value;
            }
        }

        /// <summary>
        /// Handle Add tool bar button click event:
        ///     Start a new connection item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToolStripButton_Click(object sender, EventArgs e)
        {
            Clear();
            //tabPage1.Text = "New";
            tabPage1.Enabled = true;
            connectionsListBox.SelectedIndex = -1;
            connectionNameTextBox.Text = "";
            saveButton.Visible = true;
            _changed = false;
        }

        /// <summary>
        /// Cancel task if there is a active one
        /// </summary>
        private void CancelTask()
        {
            if (_cancellation != null)
            {
                try
                {
                    _cancellation.Cancel();
                    _cancellation.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // ignore
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Close the dialog without save the changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelToolStripButton1_Click(object sender, EventArgs e)
        {
            CancelTask();
            _connections.Load();
            Close();
        }

        /// <summary>
        /// Clear connection setting details section
        /// </summary>
        private void Clear()
        {
            _populating = true;
            _selectedIndex = -1;
            //tabPage1.Text = "";
            connectionNameTextBox.Text = "";

            tabPage1.Enabled = false;
            _changed = false;
        }

        /// <summary>
        /// Handle tool bar close button click event:
        ///     Close the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            CancelTask();

            if (_changed && _selectedIndex >= 0)
            {
                Save();
            }

            _connections.Save();
            Close();
        }

        /// <summary>
        /// Handles the Load event of the ConnectionManageForm control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ConnectionManageForm_Load(object sender, EventArgs e)
        {
            // Creating a dictionary mapping authentication method names to AuthenticationMethod values
            Dictionary<string, AuthenticationMethod> authMethods = new()
            {
                //{ "Windows Authentication", AuthenticationMethod.ActiveDirectoryDefault },
                { "SQL Server Authentication", AuthenticationMethod.SqlPassword },
                { "Microsoft Entra Integrated", AuthenticationMethod.ActiveDirectoryIntegrated },
                { "Microsoft Entra Interactive", AuthenticationMethod.ActiveDirectoryInteractive },
                { "Microsoft Entra Password", AuthenticationMethod.ActiveDirectoryPassword },
                { "Microsoft Entra Managed Identity", AuthenticationMethod.ActiveDirectoryManagedIdentity },
                { "Microsoft Entra Service Principal", AuthenticationMethod.ActiveDirectoryServicePrincipal }
            };

            // Adding authentication methods to the combo box using data binding
            authenticationComboBox.DataSource = new BindingSource(authMethods, null);
            authenticationComboBox.DisplayMember = "Key"; // Display the name of the authentication method
            authenticationComboBox.ValueMember = "Value"; // Use the AuthenticationMethod as the value

            PopulateConnections();

            if (connectionsListBox.Items.Count > 0)
            {
                connectionsListBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles Connection name text box validated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ConnectionNameTextBox_Validated(object sender, EventArgs e)
        {
            if (_currentConnectionItem != null && connectionNameTextBox.Text.Trim().Length > 0)
            {
                _currentConnectionItem.Name = connectionNameTextBox.Text;
            }
        }

        /// <summary>
        /// Handles the selected index changed event of the connections list box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ConnectionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CancelTask();
            if (_changed && _selectedIndex >= 0)
            {
                Save();
            }
            PopulateSelectedItem();
        }

        /// <summary>
        /// Handle tool bar delete button click event:
        ///     Delete selected connection item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripButton_Click(object sender, EventArgs e)
        {
            if (connectionsListBox.SelectedItem != null && _selectedIndex >= 0)
            {
                var item = connectionsListBox.SelectedItem as DatabaseConnectionItem;

                if (item == null) return;

                string msg = $"Do you want to delete {item.Name}?";
                if (MessageBox.Show(msg, "Delete Connection Item",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)  //A059: Delete Connection Item
                {
                    DataConnections.RemoveAt(_selectedIndex);
                    _selectedIndex = -1;
                    PopulateConnections();
                    if (connectionsListBox.Items.Count > 0)
                    {
                        connectionsListBox.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Handles "Move down" tool strip button click event:
        /// Move the selected item down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void MoveDownToolStripButton_Click(object sender, EventArgs e)
        {
            // get selected item from list box and move it down
            if (connectionsListBox.SelectedItem != null && _selectedIndex >= 0 && _selectedIndex < connectionsListBox.Items.Count - 1)
            {
                var item = (DatabaseConnectionItem)(connectionsListBox.SelectedItem);
                _connections.MoveDown(item);
                PopulateConnections();
                connectionsListBox.SelectedItem = item;
            }
        }

        /// <summary>
        /// Handles "Move up" tool strip button click event:
        /// Move the select item up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void MoveUpToolStripButton_Click(object sender, EventArgs e)
        {
            // get selected item from list box and move it up
            if (connectionsListBox.SelectedItem != null && _selectedIndex > 0)
            {
                var item = (DatabaseConnectionItem)(connectionsListBox.SelectedItem);
                _connections.MoveUp(item);
                PopulateConnections();
                connectionsListBox.SelectedItem = item;
            }
        }

        /// <summary>
        /// Populates the connections.
        /// </summary>
        private void PopulateConnections()
        {
            connectionsListBox.Items.Clear();
            if (_connections != null)
            {
                foreach (var item in _connections.Connections)
                {
                    if (!item.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase))
                    {
                        connectionsListBox.Items.Add(item);
                    }
                }
            }
            saveButton.Visible = false;
        }

        /// <summary>
        /// Populate the selected connection item
        /// </summary>
        private void PopulateSelectedItem()
        {
            SetEditEnableState(false);

            var item = connectionsListBox.SelectedItem as DatabaseConnectionItem;
            _currentConnectionItem = item;
            _selectedIndex = connectionsListBox.SelectedIndex;
            if (item != null)
            {
                _populating = true;

                connectionNameTextBox.Text = item.Name;
                serverNameTextBox.Text = item.ServerName;
                databaseComboBox.Text = item.Database;
                authenticationComboBox.SelectedValue = item.AuthenticationType;
                userNameTextBox.Text = item.UserName;
                rememberPasswordCheckBox.Checked = item.RememberPassword;
                passwordTextBox.Text = item.Password;
                encrptyCheckBox.Checked = item.EncryptConnection;
                trustCertificateCheckBox.Checked = item.TrustServerCertificate;
                dbDescTextBox.Text = item.DatabaseDescription;

                _populating = true;

                tabPage1.Enabled = true;
            }
            _populating = false;
            SetEditEnableState(true);
        }

        /// <summary>
        /// Save changes of the settings
        /// </summary>
        private void Save()
        {
            if (_currentConnectionItem == null) return;

            if (!_populating)
            {
                _currentConnectionItem.Name = connectionNameTextBox.Text;
                _currentConnectionItem.ServerName = serverNameTextBox.Text;
                _currentConnectionItem.Database = databaseComboBox.Text;
                _currentConnectionItem.AuthenticationType = GetAuthentication();
                _currentConnectionItem.UserName = userNameTextBox.Text;
                _currentConnectionItem.Password = passwordTextBox.Text;
                _currentConnectionItem.RememberPassword = rememberPasswordCheckBox.Checked;
                _currentConnectionItem.EncryptConnection = encrptyCheckBox.Checked;
                _currentConnectionItem.TrustServerCertificate = trustCertificateCheckBox.Checked;
                _currentConnectionItem.BuildConnectionString();
                _currentConnectionItem.IsCustom = false;
                _currentConnectionItem.DatabaseDescription = dbDescTextBox.Text;

                _connections.SaveTemp();
                _changed = false;
            }
        }

        /// <summary>
        /// Handle tool bar save button click event:
        ///     Save the changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            var selectedAuthentication = authenticationComboBox.SelectedItem as KeyValuePair<string, AuthenticationMethod>?;

            var connection = new DatabaseConnectionItem()
            {
                Name = connectionNameTextBox.Text,
                ServerName = serverNameTextBox.Text,
                Database = databaseComboBox.Text,
                AuthenticationType = selectedAuthentication?.Value ?? AuthenticationMethod.ActiveDirectoryIntegrated,
                UserName = userNameTextBox.Text,
                Password = passwordTextBox.Text,
                RememberPassword = rememberPasswordCheckBox.Checked,
                EncryptConnection = encrptyCheckBox.Checked,
                TrustServerCertificate = trustCertificateCheckBox.Checked,
                DatabaseDescription = dbDescTextBox.Text
            };

            if (connection != null)
            {
                connection.BuildConnectionString();
                _connections.Add(connection);

                PopulateConnections();
                if (connectionsListBox.Items.Count > 0)
                    connectionsListBox.SelectedIndex = connectionsListBox.Items.Count - 1;
            }
        }

        /// <summary>
        /// Sets the edit enable state.
        /// </summary>
        /// <param name="state">If true, state.</param>
        private void SetEditEnableState(bool state)
        {
            connectionsListBox.Enabled = state;
            addToolStripButton.Enabled = state;
            deleteToolStripButton.Enabled = state;
            testToolStripButton.Enabled = state;
            closeToolStripButton.Enabled = state;
            tabPage1.Enabled = state;
        }

        /// <summary>
        /// Handle any change to the setting:
        ///     set change flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsChanged(object sender, EventArgs e)
        {
            if (!_populating)
            {
                _changed = true;
            }
        }

        /// <summary>
        /// Test button click event handle:
        /// Check if custom connection works or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TestButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                bool result = false;
                var item = new DatabaseConnectionItem()
                {
                    ConnectionType = "SQL Server",
                };

                if (CanSave())
                {
                    item.Name = connectionNameTextBox.Text;
                    item.ServerName = serverNameTextBox.Text;
                    item.Database = databaseComboBox.Text;
                    item.AuthenticationType = GetAuthentication();
                    item.UserName = userNameTextBox.Text;
                    item.Password = passwordTextBox.Text;
                    item.RememberPassword = rememberPasswordCheckBox.Checked;
                    item.EncryptConnection = encrptyCheckBox.Checked;
                    item.TrustServerCertificate = trustCertificateCheckBox.Checked;
                    item.DatabaseDescription = dbDescTextBox.Text;
                    item.BuildConnectionString();
                    result = await Task.Run(() => (TestConnection(item)));
                }

                if (result)
                {
                    MessageBox.Show("Succeed!", "Connection testing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to connect to the database!", "Connection testing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Gets the authentication.
        /// </summary>
        /// <returns>A AuthenticationMethod.</returns>
        private AuthenticationMethod GetAuthentication()
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, AuthenticationMethod>?;
            return selectedItem?.Value ?? AuthenticationMethod.ActiveDirectoryIntegrated;
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A Task.</returns>
        private static async Task<bool> TestConnection(DatabaseConnectionItem connection)
        {
            bool result = false;

            if (connection != null || !string.IsNullOrEmpty(connection?.ConnectionString))
            {
                result = await SQLDatabaseHelper.TestConnectionAsync(connection?.ConnectionString);
            }

            return result;
        }

        /// <summary>
        /// Cans the save.
        /// </summary>
        /// <returns>A bool.</returns>
        private bool CanSave()
        {
            bool enabled = (serverNameTextBox.Text.Trim().Length > 1 &&
                databaseComboBox.Text.Trim().Length > 1);
            if (enabled)
            {
                if (GetAuthentication() == AuthenticationMethod.SqlPassword)
                {
                    enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
                }
            }
            return enabled;
        }

        /// <summary>
        /// Handles the selected index changed event of the authentication combo box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AuthenticationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = authenticationComboBox.SelectedItem as KeyValuePair<string, AuthenticationMethod>?;

            if (selectedItem.HasValue) // Fix for CS8629: Ensure the nullable value type is not null
            {
                var authenticationMethod = selectedItem.Value.Value; // Fix for CS0029: Access the 'Value' property of the KeyValuePair to get the AuthenticationMethod

                switch (authenticationMethod) // Fix for CS0077: Directly use the value without 'as' since it's a non-nullable value type
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

                if (!_populating)
                {
                    _changed = true;
                }
            }

            //CheckCompleteStatus();
        }
    }
}