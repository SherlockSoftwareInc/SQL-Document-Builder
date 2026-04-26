using System;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class SQLConnectionBox : UserControl
    {
        private bool _completeStatus = false;
        private string _serverName;
        private bool _init = false;

        public SQLConnectionBox()
        {
            InitializeComponent();

            serverNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            databaseTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            authenticationComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            userNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Resize += SQLConnectionBox_Resize;
        }

        public event EventHandler SettingChanged;

        public event EventHandler CompleteStatusChanged;

        /// <summary>
        /// Gets or set authentication type
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public short Authentication
        {
            get => (short)authenticationComboBox.SelectedIndex;
            set => SetInitializing(() => authenticationComboBox.SelectedIndex = value);
        }

        /// <summary>
        /// Returns whether the current input meet the requirement
        /// </summary>
        public bool CanSave
        {
            get
            {
                bool enabled = (serverNameTextBox.Text.Trim().Length > 1 &&
                    databaseTextBox.Text.Trim().Length > 1);
                if (enabled)
                {
                    if (authenticationComboBox.SelectedIndex == 1)
                    {
                        enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
                    }
                }
                return enabled;
            }
        }

        /// <summary>
        /// Get default connection name
        /// </summary>
        public string ConnectionName
        {
            get => string.Format("{0} @ {1}", DatabaseName, ServerName);
            //set => connectionName = value;
        }

        /// <summary>
        /// Gets connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                string serverName = serverNameTextBox.Text.Trim();
                string dbName = databaseTextBox.Text;
                if (serverName.Length > 0 && dbName.Length > 0)
                {
                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
                    {
                        DataSource = serverName,
                        InitialCatalog = dbName,
                        Encrypt = encryptCheckBox.Checked,
                        TrustServerCertificate = trustCheckBox.Checked,
                    };
                    switch (Authentication)
                    {
                        case 1: //SQL Server authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.SqlPassword;
                            break;

                        case 2: //Active Directory Password Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryPassword;
                            break;

                        case 3: //Active Directory Integrated Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                            break;

                        case 4: //Active Directory Interactive Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryInteractive;
                            break;

                        case 5: //Service Principal Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryServicePrincipal;
                            break;

                        case 6: //Managed Service Identity Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow;
                            break;

                        default:    //Windows Authentication
                            builder.IntegratedSecurity = true;
                            builder.TrustServerCertificate = true;
                            break;
                    }

                    if (UserName.Length > 0)
                        builder.UserID = UserName;
                    if (Password.Length > 0)
                        builder.Password = Password;

                    return builder.ConnectionString;

                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets database name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DatabaseName
        {
            get => databaseTextBox.Text;
            set => SetInitializing(() => databaseTextBox.Text = value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use encrypted connections
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EncryptConnection
        {
            get => encryptCheckBox.Checked;
            set => SetInitializing(() => encryptCheckBox.Checked = value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to trust the server certificate
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TrustServerCertificate
        {
            get => trustCheckBox.Checked;
            set => SetInitializing(() => trustCheckBox.Checked = value);
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Password
        {
            get => passwordTextBox.Text;
            set => SetInitializing(() => passwordTextBox.Text = value);
        }

        /// <summary>
        /// Remember password flag
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RememberPassword
        {
            get => rememberPasswordCheckBox.Checked;
            set => SetInitializing(() => rememberPasswordCheckBox.Checked = value);
        }

        /// <summary>
        /// Require manually login flag
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RequireManualLogin { get; set; }

        /// <summary>
        /// Gets or set server name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ServerName
        {
            get => serverNameTextBox.Text;
            set => SetInitializing(() => serverNameTextBox.Text = value);
        }

        /// <summary>
        /// Returns input field left position
        /// </summary>
        public int TextboxPosition
        {
            get { return serverNameTextBox.Left + Left; }
        }

        /// <summary>
        /// Gets or set user name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UserName
        {
            get => userNameTextBox.Text;
            set => SetInitializing(() => userNameTextBox.Text = value);
        }

        public void Clear()
        {
            SetInitializing(() =>
            {
                serverNameTextBox.Clear();
                databaseTextBox.SelectedIndex = -1;
                databaseTextBox.Text = string.Empty;
                databaseTextBox.Items.Clear();
                authenticationComboBox.SelectedIndex = 0;
                userNameTextBox.Clear();
                passwordTextBox.Clear();
                rememberPasswordCheckBox.Checked = false;
                encryptCheckBox.Checked = true;
                trustCheckBox.Checked = true;
                _serverName = string.Empty;
            });

            OnSettingsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles authentication combo box selected index change event: Enable/disable user name
        /// and password by the selected authentication type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (authenticationComboBox.SelectedIndex)
            {
                case 1:
                case 2:
                case 6:
                    userNameTextBox.Enabled = true;
                    passwordTextBox.Enabled = true;
                    rememberPasswordCheckBox.Enabled = true;
                    break;

                case 4:
                case 5:
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

            if (!_init) SettingChanged?.Invoke(this, EventArgs.Empty);

            CheckCompleteStatus();
        }

        /// <summary>
        /// Enable OK button:
        /// connection name, server name, database name is required.
        /// If use sql server login, user name is required
        /// </summary>
        private void CheckCompleteStatus()
        {
            if (CanSave != _completeStatus)
            {
                _completeStatus = CanSave;
                CompleteStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handle database name selected index change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseTextBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (databaseTextBox.SelectedIndex >= 0)
            {
                CheckCompleteStatus();
                if (!_init) SettingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles setting changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (!_init) SettingChanged?.Invoke(this, EventArgs.Empty);

            CheckCompleteStatus();
        }

        /// <summary>
        /// Handles server name text box text changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            string server = serverNameTextBox.Text;
            if (server != _serverName)
            {
                if (databaseTextBox.SelectedIndex >= 0)
                {
                    databaseTextBox.SelectedIndex = -1;
                    databaseTextBox.Items.Clear();
                    _completeStatus = !CanSave;
                    CheckCompleteStatus();
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
                databaseTextBox.Items.Clear();

                _serverName = server;
                Cursor = Cursors.WaitCursor;
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    //_cancellation = cancellationTokenSource;
                    var databases = await DBData.GetDatabases(server, cancellationTokenSource.Token);

                    if (databases.Count > 0 && !cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        databaseTextBox.Items.AddRange(databases.ToArray());
                    }
                }

                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles user control load event: layout the controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SQLConnectionBox_Load(object sender, EventArgs e)
        {
            UpdateControlLayout();

            SetInitializing(() =>
            {
                serverNameTextBox.Text = ServerName;
                databaseTextBox.Text = DatabaseName;
                userNameTextBox.Text = UserName;
                authenticationComboBox.SelectedIndex = Authentication;
                encryptCheckBox.Checked = EncryptConnection;
                trustCheckBox.Checked = TrustServerCertificate;
            });

            CheckCompleteStatus();
            if (Authentication == 1)
            {
                passwordTextBox.Focus();
            }
        }

        private void SQLConnectionBox_Resize(object sender, EventArgs e)
        {
            UpdateControlLayout();
        }

        private void SetInitializing(Action action)
        {
            _init = true;
            try
            {
                action();
            }
            finally
            {
                _init = false;
            }
        }

        private void UpdateControlLayout()
        {
            const int horizontalMargin = 6;
            const int labelGap = 4;

            int contentRight = ClientSize.Width - horizontalMargin;

            serverNameTextBox.Width = Math.Max(80, contentRight - serverNameTextBox.Left);
            databaseTextBox.Width = Math.Max(80, contentRight - databaseTextBox.Left);

            int minGroupBoxHeight = GetGroupBoxMinimumHeight(horizontalMargin);
            groupBox1.Width = Math.Max(120, contentRight - groupBox1.Left);
            groupBox1.MinimumSize = new Size(groupBox1.MinimumSize.Width, minGroupBoxHeight);
            groupBox1.Height = Math.Max(minGroupBoxHeight, ClientSize.Height - groupBox1.Top - horizontalMargin);

            int groupContentRight = groupBox1.ClientSize.Width - horizontalMargin;
            authenticationComboBox.Width = Math.Max(80, groupContentRight - authenticationComboBox.Left);
            userNameTextBox.Width = Math.Max(80, groupContentRight - userNameTextBox.Left);
            passwordTextBox.Width = Math.Max(80, groupContentRight - passwordTextBox.Left);

            int checkBoxLeft = userNameTextBox.Left;
            rememberPasswordCheckBox.Left = checkBoxLeft;
            encryptCheckBox.Left = checkBoxLeft;
            trustCheckBox.Left = checkBoxLeft;

            AlignLabelToControl(serverNameLabel, serverNameTextBox, labelGap);
            AlignLabelToControl(databaseLabel, databaseTextBox, labelGap);
            AlignLabelToControl(authenticationLabel, authenticationComboBox, labelGap);
            AlignLabelToControl(userNameLabel, userNameTextBox, labelGap);
            AlignLabelToControl(passwordLabel, passwordTextBox, labelGap);
        }

        private int GetGroupBoxMinimumHeight(int bottomMargin)
        {
            int maxBottom = 0;
            foreach (Control control in groupBox1.Controls)
            {
                if (!control.Visible) continue;
                if (control.Bottom > maxBottom)
                {
                    maxBottom = control.Bottom;
                }
            }

            int nonClientHeight = groupBox1.Height - groupBox1.ClientSize.Height;
            return maxBottom + bottomMargin + nonClientHeight;
        }

        private static void AlignLabelToControl(Label label, Control control, int gap)
        {
            int dy = (control.Height - label.Height) / 2;
            int x = control.Left - gap;
            label.Location = new Point(x - label.Width, control.Top + dy);
        }
    }
}