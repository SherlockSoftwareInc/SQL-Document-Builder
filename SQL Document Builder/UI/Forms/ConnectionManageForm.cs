
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SQL_Document_Builder.DatabaseAccess;
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
            dbDescTextBox.Text = string.Empty;

            _connectionTypeComboBox.SelectedIndex = 0;
            _sqlConnSettingBox.ServerName = string.Empty;
            _sqlConnSettingBox.DatabaseName = string.Empty;
            _sqlConnSettingBox.UserName = string.Empty;
            _sqlConnSettingBox.Password = string.Empty;
            _sqlConnSettingBox.RememberPassword = false;

            _odbcConnSettingBox.DSN = string.Empty;
            _odbcConnSettingBox.UserName = string.Empty;
            _odbcConnSettingBox.Password = string.Empty;
            _odbcConnSettingBox.RememberPassword = false;
            _odbcConnSettingBox.RequireManualLogin = false;

            tabPage1.Enabled = false;
            _changed = false;
            _populating = false;
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
                    connectionsListBox.Items.Add(item);
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
                dbDescTextBox.Text = item.DatabaseDescription;

                bool isOdbc = item.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase);
                _connectionTypeComboBox.SelectedIndex = isOdbc ? 1 : 0;

                if (isOdbc)
                {
                    _odbcConnSettingBox.DSN = item.DSN ?? item.ServerName ?? string.Empty;
                    _odbcConnSettingBox.UserName = item.UserName ?? string.Empty;
                    _odbcConnSettingBox.Password = item.Password ?? string.Empty;
                    _odbcConnSettingBox.RememberPassword = item.RememberPassword;
                    _odbcConnSettingBox.RequireManualLogin = item.RequireManualLogin;
                }
                else
                {
                    _sqlConnSettingBox.ServerName = item.ServerName ?? string.Empty;
                    _sqlConnSettingBox.DatabaseName = item.Database ?? string.Empty;
                    _sqlConnSettingBox.Authentication = (short)item.AuthenticationType;
                    _sqlConnSettingBox.UserName = item.UserName ?? string.Empty;
                    _sqlConnSettingBox.Password = item.Password ?? string.Empty;
                    _sqlConnSettingBox.RememberPassword = item.RememberPassword;
                    _sqlConnSettingBox.EncryptConnection = item.EncryptConnection;
                    _sqlConnSettingBox.TrustServerCertificate = item.TrustServerCertificate;
                }

                ApplyConnectionTypeVisibility();

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
                _currentConnectionItem.Name = connectionNameTextBox.Text.Trim();
                _currentConnectionItem.DatabaseDescription = dbDescTextBox.Text;

                if (IsOdbcSelected())
                {
                    var selectedDsn = _odbcConnSettingBox.CanSave ? _odbcConnSettingBox.SelectedConnection : null;
                    _currentConnectionItem.ConnectionType = "ODBC";
                    _currentConnectionItem.DSN = _odbcConnSettingBox.DSN;
                    _currentConnectionItem.ServerName = _odbcConnSettingBox.DSN;
                    _currentConnectionItem.Database = string.Empty;
                    _currentConnectionItem.DBMSType = _odbcConnSettingBox.DBMSType;
                    _currentConnectionItem.Driver = selectedDsn?.Driver;
                    _currentConnectionItem.UserName = _odbcConnSettingBox.UserName;
                    _currentConnectionItem.Password = _odbcConnSettingBox.Password;
                    _currentConnectionItem.RememberPassword = _odbcConnSettingBox.RememberPassword;
                    _currentConnectionItem.RequireManualLogin = _odbcConnSettingBox.RequireManualLogin;
                    _currentConnectionItem.BuildConnectionString();
                }
                else
                {
                    _currentConnectionItem.ConnectionType = "SQL Server";
                    _currentConnectionItem.DBMSType = DBMSTypeEnums.SQLServer;
                    _currentConnectionItem.ServerName = _sqlConnSettingBox.ServerName;
                    _currentConnectionItem.Database = _sqlConnSettingBox.DatabaseName;
                    _currentConnectionItem.AuthenticationType = (AuthenticationMethod)_sqlConnSettingBox.Authentication;
                    _currentConnectionItem.UserName = _sqlConnSettingBox.UserName;
                    _currentConnectionItem.Password = _sqlConnSettingBox.Password;
                    _currentConnectionItem.RememberPassword = _sqlConnSettingBox.RememberPassword;
                    _currentConnectionItem.EncryptConnection = _sqlConnSettingBox.EncryptConnection;
                    _currentConnectionItem.TrustServerCertificate = _sqlConnSettingBox.TrustServerCertificate;
                    _currentConnectionItem.BuildConnectionString();
                }

                _currentConnectionItem.IsCustom = false;

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
            var connection = new DatabaseConnectionItem()
            {
                Name = connectionNameTextBox.Text.Trim(),
                DatabaseDescription = dbDescTextBox.Text
            };

            if (IsOdbcSelected())
            {
                var selectedDsn = _odbcConnSettingBox.CanSave ? _odbcConnSettingBox.SelectedConnection : null;
                connection.ConnectionType = "ODBC";
                connection.DSN = _odbcConnSettingBox.DSN;
                connection.ServerName = _odbcConnSettingBox.DSN;
                connection.Database = string.Empty;
                connection.DBMSType = _odbcConnSettingBox.DBMSType;
                connection.Driver = selectedDsn?.Driver;
                connection.UserName = _odbcConnSettingBox.UserName;
                connection.Password = _odbcConnSettingBox.Password;
                connection.RememberPassword = _odbcConnSettingBox.RememberPassword;
                connection.RequireManualLogin = _odbcConnSettingBox.RequireManualLogin;
                connection.BuildConnectionString();
            }
            else
            {
                connection.ConnectionType = "SQL Server";
                connection.DBMSType =  DBMSTypeEnums.SQLServer;
                connection.ServerName = _sqlConnSettingBox.ServerName;
                connection.Database = _sqlConnSettingBox.DatabaseName;
                connection.AuthenticationType = (AuthenticationMethod)_sqlConnSettingBox.Authentication;
                connection.UserName = _sqlConnSettingBox.UserName;
                connection.Password = _sqlConnSettingBox.Password;
                connection.RememberPassword = _sqlConnSettingBox.RememberPassword;
                connection.EncryptConnection = _sqlConnSettingBox.EncryptConnection;
                connection.TrustServerCertificate = _sqlConnSettingBox.TrustServerCertificate;
                connection.BuildConnectionString();
            }

            _connections.Add(connection);

            PopulateConnections();
            if (connectionsListBox.Items.Count > 0)
                connectionsListBox.SelectedIndex = connectionsListBox.Items.Count - 1;
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
                var item = new DatabaseConnectionItem();

                if (CanSave())
                {
                    item.Name = connectionNameTextBox.Text;
                    if (IsOdbcSelected())
                    {
                        var selectedDsn = _odbcConnSettingBox.CanSave ? _odbcConnSettingBox.SelectedConnection : null;
                        item.ConnectionType = "ODBC";
                        item.DSN = _odbcConnSettingBox.DSN;
                        item.ServerName = _odbcConnSettingBox.DSN;
                        item.DBMSType = _odbcConnSettingBox.DBMSType;
                        item.Driver = selectedDsn?.Driver;
                        item.UserName = _odbcConnSettingBox.UserName;
                        item.Password = _odbcConnSettingBox.Password;
                        item.RememberPassword = _odbcConnSettingBox.RememberPassword;
                        item.RequireManualLogin = _odbcConnSettingBox.RequireManualLogin;
                    }
                    else
                    {
                        item.ConnectionType = "SQL Server";
                        item.ServerName = _sqlConnSettingBox.ServerName;
                        item.Database = _sqlConnSettingBox.DatabaseName;
                        item.AuthenticationType = (AuthenticationMethod)_sqlConnSettingBox.Authentication;
                        item.UserName = _sqlConnSettingBox.UserName;
                        item.Password = _sqlConnSettingBox.Password;
                        item.RememberPassword = _sqlConnSettingBox.RememberPassword;
                        item.EncryptConnection = _sqlConnSettingBox.EncryptConnection;
                        item.TrustServerCertificate = _sqlConnSettingBox.TrustServerCertificate;
                    }

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
        /// Tests the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A Task.</returns>
        private static async Task<bool> TestConnection(DatabaseConnectionItem connection)
        {
            if (connection == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                connection.BuildConnectionString();
            }

            if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                return false;
            }

            var provider = DatabaseAccessProviderFactory.GetProvider(connection);
            return await provider.TestConnectionAsync(connection.ConnectionString);
        }

        /// <summary>
        /// Cans the save.
        /// </summary>
        /// <returns>A bool.</returns>
        private bool CanSave()
        {
            return IsOdbcSelected() ? _odbcConnSettingBox.CanSave : _sqlConnSettingBox.CanSave;
        }

        private bool IsOdbcSelected() => _connectionTypeComboBox.SelectedIndex == 1;

        private void ConnectionTypeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_populating)
            {
                _changed = true;
            }

            ApplyConnectionTypeVisibility();
        }

        private void ApplyConnectionTypeVisibility()
        {
            bool isOdbc = IsOdbcSelected();
            _odbcConnSettingBox.Visible = isOdbc;
            _sqlConnSettingBox.Visible = !isOdbc;

            if (isOdbc)
            {
                _odbcConnSettingBox.BringToFront();
            }
            else
            {
                _sqlConnSettingBox.BringToFront();
            }
        }

    }
}