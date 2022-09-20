namespace SQL_Document_Builder
{
    partial class NewSQLServerConnectionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.databaseNameLabel = new System.Windows.Forms.Label();
            this.logonGroupBox = new System.Windows.Forms.GroupBox();
            this.rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.authenticationComboBox = new System.Windows.Forms.ComboBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.authenticationLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.databaseComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.odbcDriverComboBox = new System.Windows.Forms.ComboBox();
            this.logonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(64, 75);
            this.serverNameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(153, 32);
            this.serverNameLabel.TabIndex = 0;
            this.serverNameLabel.Text = "Server name:";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Location = new System.Drawing.Point(215, 69);
            this.serverNameTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(370, 39);
            this.serverNameTextBox.TabIndex = 1;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.ServerNameTextBox_TextChanged);
            this.serverNameTextBox.Validated += new System.EventHandler(this.ServerNameTextBox_Validated);
            // 
            // databaseNameLabel
            // 
            this.databaseNameLabel.AutoSize = true;
            this.databaseNameLabel.Location = new System.Drawing.Point(35, 137);
            this.databaseNameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.databaseNameLabel.Name = "databaseNameLabel";
            this.databaseNameLabel.Size = new System.Drawing.Size(184, 32);
            this.databaseNameLabel.TabIndex = 0;
            this.databaseNameLabel.Text = "Database name:";
            // 
            // logonGroupBox
            // 
            this.logonGroupBox.Controls.Add(this.rememberPasswordCheckBox);
            this.logonGroupBox.Controls.Add(this.authenticationComboBox);
            this.logonGroupBox.Controls.Add(this.passwordTextBox);
            this.logonGroupBox.Controls.Add(this.passwordLabel);
            this.logonGroupBox.Controls.Add(this.userNameTextBox);
            this.logonGroupBox.Controls.Add(this.userNameLabel);
            this.logonGroupBox.Controls.Add(this.authenticationLabel);
            this.logonGroupBox.Location = new System.Drawing.Point(14, 193);
            this.logonGroupBox.Margin = new System.Windows.Forms.Padding(6);
            this.logonGroupBox.Name = "logonGroupBox";
            this.logonGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.logonGroupBox.Size = new System.Drawing.Size(574, 299);
            this.logonGroupBox.TabIndex = 2;
            this.logonGroupBox.TabStop = false;
            this.logonGroupBox.Text = "Log on to the server";
            // 
            // rememberPasswordCheckBox
            // 
            this.rememberPasswordCheckBox.AutoSize = true;
            this.rememberPasswordCheckBox.Location = new System.Drawing.Point(201, 237);
            this.rememberPasswordCheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            this.rememberPasswordCheckBox.Size = new System.Drawing.Size(268, 36);
            this.rememberPasswordCheckBox.TabIndex = 2;
            this.rememberPasswordCheckBox.Text = "Remember password";
            this.rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // authenticationComboBox
            // 
            this.authenticationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticationComboBox.FormattingEnabled = true;
            this.authenticationComboBox.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication",
            "Active Directory - Interactive",
            "Active Directory - Integrated",
            "Active Directory - P:assword",
            "Active Directory -Service Principal"});
            this.authenticationComboBox.Location = new System.Drawing.Point(201, 51);
            this.authenticationComboBox.Margin = new System.Windows.Forms.Padding(6);
            this.authenticationComboBox.Name = "authenticationComboBox";
            this.authenticationComboBox.Size = new System.Drawing.Size(359, 40);
            this.authenticationComboBox.TabIndex = 1;
            this.authenticationComboBox.SelectedIndexChanged += new System.EventHandler(this.AuthenticationComboBox_SelectedIndexChanged);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(201, 175);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(288, 39);
            this.passwordTextBox.TabIndex = 1;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(78, 181);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(116, 32);
            this.passwordLabel.TabIndex = 0;
            this.passwordLabel.Text = "Password:";
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(201, 113);
            this.userNameTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(288, 39);
            this.userNameTextBox.TabIndex = 1;
            this.userNameTextBox.TextChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(67, 119);
            this.userNameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(133, 32);
            this.userNameLabel.TabIndex = 0;
            this.userNameLabel.Text = "User name:";
            // 
            // authenticationLabel
            // 
            this.authenticationLabel.AutoSize = true;
            this.authenticationLabel.Location = new System.Drawing.Point(24, 58);
            this.authenticationLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.authenticationLabel.Name = "authenticationLabel";
            this.authenticationLabel.Size = new System.Drawing.Size(176, 32);
            this.authenticationLabel.TabIndex = 0;
            this.authenticationLabel.Text = "Authentication:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(446, 515);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(139, 49);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(295, 515);
            this.okButton.Margin = new System.Windows.Forms.Padding(6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(139, 49);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // databaseComboBox
            // 
            this.databaseComboBox.FormattingEnabled = true;
            this.databaseComboBox.Location = new System.Drawing.Point(215, 131);
            this.databaseComboBox.Margin = new System.Windows.Forms.Padding(6);
            this.databaseComboBox.Name = "databaseComboBox";
            this.databaseComboBox.Size = new System.Drawing.Size(370, 40);
            this.databaseComboBox.TabIndex = 1;
            this.databaseComboBox.SelectedIndexChanged += new System.EventHandler(this.DatabaseComboBox_SelectedIndexChanged);
            this.databaseComboBox.TextChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 32);
            this.label1.TabIndex = 4;
            this.label1.Text = "ODBC Driver:";
            // 
            // odbcDriverComboBox
            // 
            this.odbcDriverComboBox.FormattingEnabled = true;
            this.odbcDriverComboBox.Location = new System.Drawing.Point(215, 12);
            this.odbcDriverComboBox.Name = "odbcDriverComboBox";
            this.odbcDriverComboBox.Size = new System.Drawing.Size(370, 40);
            this.odbcDriverComboBox.TabIndex = 5;
            // 
            // NewSQLServerConnectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 590);
            this.Controls.Add(this.odbcDriverComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.databaseComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.logonGroupBox);
            this.Controls.Add(this.databaseNameLabel);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.serverNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewSQLServerConnectionDialog";
            this.Text = "New SQL Server Connection Dialog";
            this.Load += new System.EventHandler(this.NewSQLServerConnectionDialog_Load);
            this.logonGroupBox.ResumeLayout(false);
            this.logonGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.GroupBox logonGroupBox;
        private System.Windows.Forms.CheckBox rememberPasswordCheckBox;
        private System.Windows.Forms.ComboBox authenticationComboBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox databaseComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox odbcDriverComboBox;
    }
}