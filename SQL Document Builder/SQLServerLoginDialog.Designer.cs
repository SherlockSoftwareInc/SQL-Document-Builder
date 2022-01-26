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
            this.logonGroupBox = new System.Windows.Forms.GroupBox();
            this.authenticationComboBox = new System.Windows.Forms.ComboBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.authenticationLabel = new System.Windows.Forms.Label();
            this.databaseNameLabel = new System.Windows.Forms.Label();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.databaseComboBox = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.logonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // logonGroupBox
            // 
            this.logonGroupBox.Controls.Add(this.authenticationComboBox);
            this.logonGroupBox.Controls.Add(this.passwordTextBox);
            this.logonGroupBox.Controls.Add(this.passwordLabel);
            this.logonGroupBox.Controls.Add(this.userNameTextBox);
            this.logonGroupBox.Controls.Add(this.userNameLabel);
            this.logonGroupBox.Controls.Add(this.authenticationLabel);
            this.logonGroupBox.Location = new System.Drawing.Point(10, 70);
            this.logonGroupBox.Name = "logonGroupBox";
            this.logonGroupBox.Size = new System.Drawing.Size(309, 117);
            this.logonGroupBox.TabIndex = 8;
            this.logonGroupBox.TabStop = false;
            this.logonGroupBox.Text = "Log on to the server";
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
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(108, 82);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(157, 23);
            this.passwordTextBox.TabIndex = 1;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.ServerNameTextBox_TextChanged);
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
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(108, 53);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(157, 23);
            this.userNameTextBox.TabIndex = 1;
            this.userNameTextBox.TextChanged += new System.EventHandler(this.ServerNameTextBox_TextChanged);
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
            // authenticationLabel
            // 
            this.authenticationLabel.AutoSize = true;
            this.authenticationLabel.Location = new System.Drawing.Point(13, 27);
            this.authenticationLabel.Name = "authenticationLabel";
            this.authenticationLabel.Size = new System.Drawing.Size(89, 15);
            this.authenticationLabel.TabIndex = 0;
            this.authenticationLabel.Text = "Authentication:";
            // 
            // databaseNameLabel
            // 
            this.databaseNameLabel.AutoSize = true;
            this.databaseNameLabel.Location = new System.Drawing.Point(21, 44);
            this.databaseNameLabel.Name = "databaseNameLabel";
            this.databaseNameLabel.Size = new System.Drawing.Size(91, 15);
            this.databaseNameLabel.TabIndex = 4;
            this.databaseNameLabel.Text = "Database name:";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Location = new System.Drawing.Point(118, 12);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(201, 23);
            this.serverNameTextBox.TabIndex = 6;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.ServerNameTextBox_TextChanged);
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(37, 15);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(75, 15);
            this.serverNameLabel.TabIndex = 5;
            this.serverNameLabel.Text = "Server name:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(164, 197);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // databaseComboBox
            // 
            this.databaseComboBox.FormattingEnabled = true;
            this.databaseComboBox.Location = new System.Drawing.Point(118, 41);
            this.databaseComboBox.Name = "databaseComboBox";
            this.databaseComboBox.Size = new System.Drawing.Size(201, 23);
            this.databaseComboBox.TabIndex = 7;
            this.databaseComboBox.TextChanged += new System.EventHandler(this.ServerNameTextBox_TextChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(245, 197);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SQLServerLoginDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 232);
            this.Controls.Add(this.logonGroupBox);
            this.Controls.Add(this.databaseNameLabel);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.serverNameLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.databaseComboBox);
            this.Controls.Add(this.cancelButton);
            this.Name = "SQLServerLoginDialog";
            this.Text = "SQL Server Login Dialog";
            this.logonGroupBox.ResumeLayout(false);
            this.logonGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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