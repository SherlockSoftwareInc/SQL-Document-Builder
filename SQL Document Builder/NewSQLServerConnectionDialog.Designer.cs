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
            serverNameLabel = new System.Windows.Forms.Label();
            serverNameTextBox = new System.Windows.Forms.TextBox();
            databaseNameLabel = new System.Windows.Forms.Label();
            logonGroupBox = new System.Windows.Forms.GroupBox();
            trustCertificateCheckBox = new System.Windows.Forms.CheckBox();
            encrptyCheckBox = new System.Windows.Forms.CheckBox();
            rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            authenticationComboBox = new System.Windows.Forms.ComboBox();
            passwordTextBox = new System.Windows.Forms.TextBox();
            passwordLabel = new System.Windows.Forms.Label();
            userNameTextBox = new System.Windows.Forms.TextBox();
            userNameLabel = new System.Windows.Forms.Label();
            authenticationLabel = new System.Windows.Forms.Label();
            cancelButton = new System.Windows.Forms.Button();
            okButton = new System.Windows.Forms.Button();
            databaseComboBox = new System.Windows.Forms.ComboBox();
            logonGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // serverNameLabel
            // 
            serverNameLabel.AutoSize = true;
            serverNameLabel.Location = new System.Drawing.Point(35, 9);
            serverNameLabel.Name = "serverNameLabel";
            serverNameLabel.Size = new System.Drawing.Size(75, 15);
            serverNameLabel.TabIndex = 0;
            serverNameLabel.Text = "Server name:";
            // 
            // serverNameTextBox
            // 
            serverNameTextBox.Location = new System.Drawing.Point(116, 6);
            serverNameTextBox.Name = "serverNameTextBox";
            serverNameTextBox.Size = new System.Drawing.Size(201, 23);
            serverNameTextBox.TabIndex = 1;
            serverNameTextBox.TextChanged += ServerNameTextBox_TextChanged;
            serverNameTextBox.Validated += ServerNameTextBox_Validated;
            // 
            // databaseNameLabel
            // 
            databaseNameLabel.AutoSize = true;
            databaseNameLabel.Location = new System.Drawing.Point(19, 38);
            databaseNameLabel.Name = "databaseNameLabel";
            databaseNameLabel.Size = new System.Drawing.Size(91, 15);
            databaseNameLabel.TabIndex = 2;
            databaseNameLabel.Text = "Database name:";
            // 
            // logonGroupBox
            // 
            logonGroupBox.Controls.Add(trustCertificateCheckBox);
            logonGroupBox.Controls.Add(encrptyCheckBox);
            logonGroupBox.Controls.Add(rememberPasswordCheckBox);
            logonGroupBox.Controls.Add(authenticationComboBox);
            logonGroupBox.Controls.Add(passwordTextBox);
            logonGroupBox.Controls.Add(passwordLabel);
            logonGroupBox.Controls.Add(userNameTextBox);
            logonGroupBox.Controls.Add(userNameLabel);
            logonGroupBox.Controls.Add(authenticationLabel);
            logonGroupBox.Location = new System.Drawing.Point(10, 64);
            logonGroupBox.Name = "logonGroupBox";
            logonGroupBox.Size = new System.Drawing.Size(309, 186);
            logonGroupBox.TabIndex = 4;
            logonGroupBox.TabStop = false;
            logonGroupBox.Text = "Log on to the server";
            // 
            // trustCertificateCheckBox
            // 
            trustCertificateCheckBox.AutoSize = true;
            trustCertificateCheckBox.Checked = true;
            trustCertificateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            trustCertificateCheckBox.Location = new System.Drawing.Point(106, 159);
            trustCertificateCheckBox.Name = "trustCertificateCheckBox";
            trustCertificateCheckBox.Size = new System.Drawing.Size(141, 19);
            trustCertificateCheckBox.TabIndex = 8;
            trustCertificateCheckBox.Text = "Trust server certificate";
            trustCertificateCheckBox.UseVisualStyleBackColor = true;
            // 
            // encrptyCheckBox
            // 
            encrptyCheckBox.AutoSize = true;
            encrptyCheckBox.Checked = true;
            encrptyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            encrptyCheckBox.Location = new System.Drawing.Point(105, 134);
            encrptyCheckBox.Name = "encrptyCheckBox";
            encrptyCheckBox.Size = new System.Drawing.Size(129, 19);
            encrptyCheckBox.TabIndex = 7;
            encrptyCheckBox.Text = "Encrypt connection";
            encrptyCheckBox.UseVisualStyleBackColor = true;
            // 
            // rememberPasswordCheckBox
            // 
            rememberPasswordCheckBox.AutoSize = true;
            rememberPasswordCheckBox.Location = new System.Drawing.Point(105, 109);
            rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            rememberPasswordCheckBox.Size = new System.Drawing.Size(137, 19);
            rememberPasswordCheckBox.TabIndex = 6;
            rememberPasswordCheckBox.Text = "Remember password";
            rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // authenticationComboBox
            // 
            authenticationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            authenticationComboBox.FormattingEnabled = true;
            authenticationComboBox.Items.AddRange(new object[] { "Windows Authentication", "SQL Server Authentication", "Microsoft Entra MFA", "Microsoft Entra Integrated", "Microsoft Entra Password", "Microsoft Entra Service Principal", "Microsoft Entra Managed Identity", "Microsoft Entra Default" });
            authenticationComboBox.Location = new System.Drawing.Point(106, 22);
            authenticationComboBox.Name = "authenticationComboBox";
            authenticationComboBox.Size = new System.Drawing.Size(195, 23);
            authenticationComboBox.TabIndex = 1;
            authenticationComboBox.SelectedIndexChanged += AuthenticationComboBox_SelectedIndexChanged;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new System.Drawing.Point(106, 80);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new System.Drawing.Size(195, 23);
            passwordTextBox.TabIndex = 5;
            passwordTextBox.TextChanged += OnSettingsChanged;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(40, 83);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(60, 15);
            passwordLabel.TabIndex = 4;
            passwordLabel.Text = "Password:";
            // 
            // userNameTextBox
            // 
            userNameTextBox.Location = new System.Drawing.Point(105, 51);
            userNameTextBox.Name = "userNameTextBox";
            userNameTextBox.Size = new System.Drawing.Size(196, 23);
            userNameTextBox.TabIndex = 3;
            userNameTextBox.TextChanged += OnSettingsChanged;
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Location = new System.Drawing.Point(33, 54);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new System.Drawing.Size(66, 15);
            userNameLabel.TabIndex = 2;
            userNameLabel.Text = "User name:";
            // 
            // authenticationLabel
            // 
            authenticationLabel.AutoSize = true;
            authenticationLabel.Location = new System.Drawing.Point(11, 25);
            authenticationLabel.Name = "authenticationLabel";
            authenticationLabel.Size = new System.Drawing.Size(89, 15);
            authenticationLabel.TabIndex = 0;
            authenticationLabel.Text = "Authentication:";
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(240, 258);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(159, 258);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 5;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // databaseComboBox
            // 
            databaseComboBox.FormattingEnabled = true;
            databaseComboBox.Location = new System.Drawing.Point(116, 35);
            databaseComboBox.Name = "databaseComboBox";
            databaseComboBox.Size = new System.Drawing.Size(201, 23);
            databaseComboBox.TabIndex = 3;
            databaseComboBox.SelectedIndexChanged += DatabaseComboBox_SelectedIndexChanged;
            databaseComboBox.TextChanged += OnSettingsChanged;
            // 
            // NewSQLServerConnectionDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(333, 293);
            Controls.Add(okButton);
            Controls.Add(databaseComboBox);
            Controls.Add(cancelButton);
            Controls.Add(logonGroupBox);
            Controls.Add(databaseNameLabel);
            Controls.Add(serverNameTextBox);
            Controls.Add(serverNameLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewSQLServerConnectionDialog";
            Text = "New SQL Server Connection Dialog";
            Load += NewSQLServerConnectionDialog_Load;
            logonGroupBox.ResumeLayout(false);
            logonGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.CheckBox trustCertificateCheckBox;
        private System.Windows.Forms.CheckBox encrptyCheckBox;
    }
}