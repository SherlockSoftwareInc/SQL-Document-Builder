using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class ODBCLoginDialog : Form
    {
        public ODBCLoginDialog()
        {
            if (Properties.Settings.Default.DarkMode)
            {
                _ = new DarkMode(this);
            }
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DSN { get; set; } = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Password { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UserName { get; set; }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EnableOKButton()
        {
            okButton.Enabled = (userNameTextBox.Text.Trim().Length > 1 && passwordTextBox.Text.Trim().Length > 1);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            UserName = userNameTextBox.Text;
            Password = passwordTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableOKButton();
        }

        private void ODBCLoginDialog_Load(object sender, EventArgs e)
        {
            dsnLabel.Text = DSN;
            userNameTextBox.Text = UserName;
            passwordTextBox.Text = Password;
            EnableOKButton();
            passwordTextBox.Focus();
        }
    }
}