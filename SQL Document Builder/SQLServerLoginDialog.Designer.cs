namespace SQL_Document_Builder
{
    partial class SQLServerLoginDialog
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
            logonGroupBox = new System.Windows.Forms.GroupBox();
            authenticationComboBox = new System.Windows.Forms.ComboBox();
            passwordTextBox = new System.Windows.Forms.TextBox();
            passwordLabel = new System.Windows.Forms.Label();
            userNameTextBox = new System.Windows.Forms.TextBox();
            userNameLabel = new System.Windows.Forms.Label();
            authenticationLabel = new System.Windows.Forms.Label();
            databaseNameLabel = new System.Windows.Forms.Label();
            serverNameTextBox = new System.Windows.Forms.TextBox();
            serverNameLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            databaseComboBox = new System.Windows.Forms.ComboBox();
            cancelButton = new System.Windows.Forms.Button();
            logonGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // logonGroupBox
            // 
            logonGroupBox.Controls.Add(authenticationComboBox);
            logonGroupBox.Controls.Add(passwordTextBox);
            logonGroupBox.Controls.Add(passwordLabel);
            logonGroupBox.Controls.Add(userNameTextBox);
            logonGroupBox.Controls.Add(userNameLabel);
            logonGroupBox.Controls.Add(authenticationLabel);
            logonGroupBox.Location = new System.Drawing.Point(10, 70);
            logonGroupBox.Name = "logonGroupBox";
            logonGroupBox.Size = new System.Drawing.Size(309, 117);
            logonGroupBox.TabIndex = 8;
            logonGroupBox.TabStop = false;
            logonGroupBox.Text = "Log on to the server";
            // 
            // authenticationComboBox
            // 
            authenticationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            authenticationComboBox.FormattingEnabled = true;
            authenticationComboBox.Items.AddRange(new object[] { "Windows Authentication", "SQL Server Authentication" });
            authenticationComboBox.Location = new System.Drawing.Point(108, 24);
            authenticationComboBox.Name = "authenticationComboBox";
            authenticationComboBox.Size = new System.Drawing.Size(195, 23);
            authenticationComboBox.TabIndex = 1;
            authenticationComboBox.SelectedIndexChanged += AuthenticationComboBox_SelectedIndexChanged;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new System.Drawing.Point(108, 82);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new System.Drawing.Size(157, 23);
            passwordTextBox.TabIndex = 1;
            passwordTextBox.TextChanged += ServerNameTextBox_TextChanged;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(42, 85);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(60, 15);
            passwordLabel.TabIndex = 0;
            passwordLabel.Text = "Password:";
            // 
            // userNameTextBox
            // 
            userNameTextBox.Location = new System.Drawing.Point(108, 53);
            userNameTextBox.Name = "userNameTextBox";
            userNameTextBox.Size = new System.Drawing.Size(157, 23);
            userNameTextBox.TabIndex = 1;
            userNameTextBox.TextChanged += ServerNameTextBox_TextChanged;
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Location = new System.Drawing.Point(36, 56);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new System.Drawing.Size(66, 15);
            userNameLabel.TabIndex = 0;
            userNameLabel.Text = "User name:";
            // 
            // authenticationLabel
            // 
            authenticationLabel.AutoSize = true;
            authenticationLabel.Location = new System.Drawing.Point(13, 27);
            authenticationLabel.Name = "authenticationLabel";
            authenticationLabel.Size = new System.Drawing.Size(89, 15);
            authenticationLabel.TabIndex = 0;
            authenticationLabel.Text = "Authentication:";
            // 
            // databaseNameLabel
            // 
            databaseNameLabel.AutoSize = true;
            databaseNameLabel.Location = new System.Drawing.Point(21, 44);
            databaseNameLabel.Name = "databaseNameLabel";
            databaseNameLabel.Size = new System.Drawing.Size(91, 15);
            databaseNameLabel.TabIndex = 4;
            databaseNameLabel.Text = "Database name:";
            // 
            // serverNameTextBox
            // 
            serverNameTextBox.Location = new System.Drawing.Point(118, 12);
            serverNameTextBox.Name = "serverNameTextBox";
            serverNameTextBox.Size = new System.Drawing.Size(201, 23);
            serverNameTextBox.TabIndex = 6;
            serverNameTextBox.TextChanged += ServerNameTextBox_TextChanged;
            // 
            // serverNameLabel
            // 
            serverNameLabel.AutoSize = true;
            serverNameLabel.Location = new System.Drawing.Point(37, 15);
            serverNameLabel.Name = "serverNameLabel";
            serverNameLabel.Size = new System.Drawing.Size(75, 15);
            serverNameLabel.TabIndex = 5;
            serverNameLabel.Text = "Server name:";
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(164, 197);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 9;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // databaseComboBox
            // 
            databaseComboBox.FormattingEnabled = true;
            databaseComboBox.Location = new System.Drawing.Point(118, 41);
            databaseComboBox.Name = "databaseComboBox";
            databaseComboBox.Size = new System.Drawing.Size(201, 23);
            databaseComboBox.TabIndex = 7;
            databaseComboBox.TextChanged += ServerNameTextBox_TextChanged;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(245, 197);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 10;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // SQLServerLoginDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(337, 232);
            Controls.Add(logonGroupBox);
            Controls.Add(databaseNameLabel);
            Controls.Add(serverNameTextBox);
            Controls.Add(serverNameLabel);
            Controls.Add(okButton);
            Controls.Add(databaseComboBox);
            Controls.Add(cancelButton);
            Name = "SQLServerLoginDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "SQL Server Login Dialog";
            logonGroupBox.ResumeLayout(false);
            logonGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox logonGroupBox;
        private System.Windows.Forms.ComboBox authenticationComboBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox databaseComboBox;
        private System.Windows.Forms.Button cancelButton;
    }
}