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
            this.authenticationLabel = new System.Windows.Forms.Label();
            this.authenticationComboBox = new System.Windows.Forms.ComboBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.databaseComboBox = new System.Windows.Forms.ComboBox();
            this.logonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(39, 9);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(75, 15);
            this.serverNameLabel.TabIndex = 0;
            this.serverNameLabel.Text = "Server name:";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Location = new System.Drawing.Point(120, 6);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(201, 23);
            this.serverNameTextBox.TabIndex = 1;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.ServerNameTextBox_TextChanged);
            this.serverNameTextBox.Validated += new System.EventHandler(this.ServerNameTextBox_Validated);
            // 
            // databaseNameLabel
            // 
            this.databaseNameLabel.AutoSize = true;
            this.databaseNameLabel.Location = new System.Drawing.Point(23, 38);
            this.databaseNameLabel.Name = "databaseNameLabel";
            this.databaseNameLabel.Size = new System.Drawing.Size(91, 15);
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
            this.logonGroupBox.Location = new System.Drawing.Point(12, 64);
            this.logonGroupBox.Name = "logonGroupBox";
            this.logonGroupBox.Size = new System.Drawing.Size(309, 140);
            this.logonGroupBox.TabIndex = 2;
            this.logonGroupBox.TabStop = false;
            this.logonGroupBox.Text = "Log on to the server";
            // 
            // authenticationLabel
            // 
            this.authenticationLabel.AutoSize = true;
            this.authenticationLabel.Location = new System.Drawing.Point(13, 27);
            this.authenticationLabel.Name = "authenticationLabel";
            this.authenticationLabel.Size = new System.Drawing.Size(89, 15);
            this.authenticationLabel.TabIndex = 0;
            this.authenticationLabel.Text = "Authentication:";
            // 
            // authenticationComboBox
            // 
            this.authenticationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticationComboBox.FormattingEnabled = true;
            this.authenticationComboBox.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this.authenticationComboBox.Location = new System.Drawing.Point(108, 24);
            this.authenticationComboBox.Name = "authenticationComboBox";
            this.authenticationComboBox.Size = new System.Drawing.Size(195, 23);
            this.authenticationComboBox.TabIndex = 1;
            this.authenticationComboBox.SelectedIndexChanged += new System.EventHandler(this.AuthenticationComboBox_SelectedIndexChanged);
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(36, 56);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(66, 15);
            this.userNameLabel.TabIndex = 0;
            this.userNameLabel.Text = "User name:";
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(108, 53);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(157, 23);
            this.userNameTextBox.TabIndex = 1;
            this.userNameTextBox.TextChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(42, 85);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(60, 15);
            this.passwordLabel.TabIndex = 0;
            this.passwordLabel.Text = "Password:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(108, 82);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(157, 23);
            this.passwordTextBox.TabIndex = 1;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // rememberPasswordCheckBox
            // 
            this.rememberPasswordCheckBox.AutoSize = true;
            this.rememberPasswordCheckBox.Location = new System.Drawing.Point(108, 111);
            this.rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            this.rememberPasswordCheckBox.Size = new System.Drawing.Size(137, 19);
            this.rememberPasswordCheckBox.TabIndex = 2;
            this.rememberPasswordCheckBox.Text = "Remember password";
            this.rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(240, 218);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(159, 218);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // databaseComboBox
            // 
            this.databaseComboBox.FormattingEnabled = true;
            this.databaseComboBox.Location = new System.Drawing.Point(120, 35);
            this.databaseComboBox.Name = "databaseComboBox";
            this.databaseComboBox.Size = new System.Drawing.Size(201, 23);
            this.databaseComboBox.TabIndex = 1;
            this.databaseComboBox.SelectedIndexChanged += new System.EventHandler(this.DatabaseComboBox_SelectedIndexChanged);
            this.databaseComboBox.TextChanged += new System.EventHandler(this.OnSettingsChanged);
            // 
            // NewSQLServerConnectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 253);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.databaseComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.logonGroupBox);
            this.Controls.Add(this.databaseNameLabel);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.serverNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewSQLServerConnectionDialog";
            this.Text = "New SQL Server Connection Dialog";
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
    }
}