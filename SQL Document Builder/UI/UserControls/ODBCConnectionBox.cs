using System;

using System.Drawing;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class ODBCConnectionBox : UserControl
    {
        private bool _completeStatus = false;
        private bool _init = false;

        public ODBCConnectionBox()
        {
            InitializeComponent();

            dsnComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            userNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Resize += ODBCConnectionBox_Resize;
        }

        public event EventHandler SettingChanged;

        public event EventHandler CompleteStatusChanged;

        /// <summary>
        /// Returns whether the current input meet the requirement
        /// </summary>
        public bool CanSave
        {
            get => dsnComboBox.SelectedIndex >= 0;
        }

        /// <summary>
        /// Gets or set connection settings
        /// </summary>
        public SQLServerConnections Connections { get; set; }

        /// <summary>
        /// Gets or sets DSN
        /// </summary>
        public string DSN
        {
            get
            {
                if (dsnComboBox.SelectedItem != null)
                {
                    var selectedItem = (DatabaseConnectionItem)dsnComboBox.SelectedItem;
                    return selectedItem.Name ?? string.Empty;
                }
                return string.Empty;
                //=> (DatabaseConnectionItem)(dsnComboBox.SelectedItem).dsn;
            }
            set
            {
                SetInitializing(() =>
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        dsnComboBox.SelectedIndex = -1;
                        return;
                    }

                    for (int i = 0; i < dsnComboBox.Items.Count; i++)
                    {
                        if (dsnComboBox.Items[i] is DatabaseConnectionItem item &&
                            string.Equals(item.Name, value, StringComparison.OrdinalIgnoreCase))
                        {
                            dsnComboBox.SelectedIndex = i;
                            return;
                        }
                    }

                    dsnComboBox.SelectedIndex = -1;
                });
            }
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get => passwordTextBox.Text;
            set => SetInitializing(() => passwordTextBox.Text = value);
        }

        /// <summary>
        /// Enables/disables change DSN
        /// </summary>
        public bool ReadOnly
        {
            get => !dsnComboBox.Enabled;
            set => SetInitializing(() => dsnComboBox.Enabled = !value);
        }

        /// <summary>
        /// Remember password flag
        /// </summary>
        public bool RememberPassword
        {
            get => rememberPasswordCheckBox.Enabled && rememberPasswordCheckBox.Checked;
            set => SetInitializing(() => rememberPasswordCheckBox.Checked = value);
        }

        /// <summary>
        /// Require manually login flag
        /// </summary>
        public bool RequireManualLogin
        {
            get => manualLoginCheckBox.Checked;
            set => SetInitializing(() => manualLoginCheckBox.Checked = value);
        }

        public DatabaseConnectionItem SelectedConnection { get => (DatabaseConnectionItem)dsnComboBox.SelectedItem; }

        /// <summary>
        /// Gets or set user name
        /// </summary>
        public string UserName
        {
            get => userNameTextBox.Text;
            set => SetInitializing(() => userNameTextBox.Text = value);
        }

        /// <summary>
        ///
        /// </summary>
        public void Clear()
        {
            SetInitializing(() =>
            {
                dsnComboBox.SelectedIndex = -1;
                userNameTextBox.Clear();
                passwordTextBox.Clear();
                rememberPasswordCheckBox.Checked = false;
                manualLoginCheckBox.Checked = false;
                rememberPasswordCheckBox.Enabled = false;
                UpdateManualLoginControls();
            });

            OnSettingsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles "Require manually login" check box checked state change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualLoginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManualLoginControls();
            if (!_init) SettingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ODBCConnectionBox_Load(object sender, EventArgs e)
        {
            SetInitializing(() =>
            {
                LoadAvailableDsns();
                UpdateManualLoginControls();
                UpdateRememberPasswordState();
            });

            UpdateControlLayout();
        }

        /// <summary>
        /// Handles data entry change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (!_init) SettingChanged?.Invoke(this, EventArgs.Empty);

            if (CanSave != _completeStatus)
            {
                _completeStatus = CanSave;
                CompleteStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateRememberPasswordState();
            OnSettingsChanged(sender, e);
        }

        private void ODBCConnectionBox_Resize(object sender, EventArgs e)
        {
            UpdateControlLayout();
        }

        private void LoadAvailableDsns()
        {
            Connections ??= new SQLServerConnections();
            Connections.GetAllDSNs();

            dsnComboBox.BeginUpdate();
            try
            {
                dsnComboBox.Items.Clear();
                if (Connections.AvailableDSNs != null)
                {
                    foreach (var item in Connections.AvailableDSNs)
                    {
                        dsnComboBox.Items.Add(item);
                    }
                }

                dsnComboBox.SelectedIndex = dsnComboBox.Items.Count > 0 ? 0 : -1;
            }
            finally
            {
                dsnComboBox.EndUpdate();
            }
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

        private void UpdateManualLoginControls()
        {
            bool enabled = manualLoginCheckBox.Checked;
            userNameTextBox.Enabled = enabled;
            passwordTextBox.Enabled = enabled;
        }

        private void UpdateRememberPasswordState()
        {
            rememberPasswordCheckBox.Enabled = passwordTextBox.Text.Trim().Length > 0;
            if (!rememberPasswordCheckBox.Enabled)
            {
                rememberPasswordCheckBox.Checked = false;
            }
        }

        private void UpdateControlLayout()
        {
            const int labelGap = 4;

            AlignLabelToControl(dsnLabel, dsnComboBox, labelGap);
            AlignLabelToControl(userNameLabel, userNameTextBox, labelGap);
            AlignLabelToControl(passwordLabel, passwordTextBox, labelGap);
        }

        private static void AlignLabelToControl(Label label, Control control, int gap)
        {
            int dy = (control.Height - label.Height) / 2;
            int x = control.Left - gap;
            label.Location = new Point(x - label.Width, control.Top + dy);
        }
    }
}
