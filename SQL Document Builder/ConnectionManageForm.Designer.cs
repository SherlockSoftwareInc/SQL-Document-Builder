namespace SQL_Document_Builder
{
    partial class ConnectionManageForm
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
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            addToolStripButton = new System.Windows.Forms.ToolStripButton();
            deleteToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            moveUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            moveDownToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            testToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            cancelToolStripButton = new System.Windows.Forms.ToolStripButton();
            connectionsLabel = new System.Windows.Forms.Label();
            connectionsListBox = new System.Windows.Forms.ListBox();
            saveButton = new System.Windows.Forms.Button();
            logonGroupBox = new System.Windows.Forms.GroupBox();
            trustCertificateCheckBox = new System.Windows.Forms.CheckBox();
            encrptyCheckBox = new System.Windows.Forms.CheckBox();
            rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            authenticationComboBox = new System.Windows.Forms.ComboBox();
            passwordTextBox = new System.Windows.Forms.TextBox();
            userNameTextBox = new System.Windows.Forms.TextBox();
            passwordLabel = new System.Windows.Forms.Label();
            authenticationLabel = new System.Windows.Forms.Label();
            userNameLabel = new System.Windows.Forms.Label();
            serverNameTextBox = new System.Windows.Forms.TextBox();
            connectionNameTextBox = new System.Windows.Forms.TextBox();
            serverNameLabel = new System.Windows.Forms.Label();
            databaseComboBox = new System.Windows.Forms.ComboBox();
            databaseNameLabel = new System.Windows.Forms.Label();
            connectionNameLabel = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            tabPage2 = new System.Windows.Forms.TabPage();
            labelDbDesc = new System.Windows.Forms.Label();
            dbDescTextBox = new System.Windows.Forms.TextBox();
            toolStrip1.SuspendLayout();
            logonGroupBox.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { addToolStripButton, deleteToolStripButton, toolStripSeparator1, moveUpToolStripButton, moveDownToolStripButton, toolStripSeparator2, testToolStripButton, toolStripSeparator3, closeToolStripButton, cancelToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(592, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // addToolStripButton
            // 
            addToolStripButton.Image = Properties.Resources.add;
            addToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            addToolStripButton.Name = "addToolStripButton";
            addToolStripButton.Size = new System.Drawing.Size(51, 22);
            addToolStripButton.Text = "New";
            addToolStripButton.Click += AddToolStripButton_Click;
            // 
            // deleteToolStripButton
            // 
            deleteToolStripButton.Image = Properties.Resources.delete_icon;
            deleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            deleteToolStripButton.Name = "deleteToolStripButton";
            deleteToolStripButton.Size = new System.Drawing.Size(60, 22);
            deleteToolStripButton.Text = "Delete";
            deleteToolStripButton.Click += DeleteToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // moveUpToolStripButton
            // 
            moveUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            moveUpToolStripButton.Image = Properties.Resources.up_arrow;
            moveUpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            moveUpToolStripButton.Name = "moveUpToolStripButton";
            moveUpToolStripButton.Size = new System.Drawing.Size(23, 22);
            moveUpToolStripButton.Text = "Move up";
            moveUpToolStripButton.Click += MoveUpToolStripButton_Click;
            // 
            // moveDownToolStripButton
            // 
            moveDownToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            moveDownToolStripButton.Image = Properties.Resources.down_arrow1;
            moveDownToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            moveDownToolStripButton.Name = "moveDownToolStripButton";
            moveDownToolStripButton.Size = new System.Drawing.Size(23, 22);
            moveDownToolStripButton.Text = "Move down";
            moveDownToolStripButton.Click += MoveDownToolStripButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // testToolStripButton
            // 
            testToolStripButton.Image = Properties.Resources.checkmark;
            testToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            testToolStripButton.Name = "testToolStripButton";
            testToolStripButton.Size = new System.Drawing.Size(48, 22);
            testToolStripButton.Text = "Test";
            testToolStripButton.Click += TestButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // closeToolStripButton
            // 
            closeToolStripButton.Image = Properties.Resources.save;
            closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            closeToolStripButton.Name = "closeToolStripButton";
            closeToolStripButton.Size = new System.Drawing.Size(43, 22);
            closeToolStripButton.Text = "&OK";
            closeToolStripButton.Click += CloseToolStripButton_Click;
            // 
            // cancelToolStripButton
            // 
            cancelToolStripButton.Image = Properties.Resources.cancel_icon_16;
            cancelToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            cancelToolStripButton.Name = "cancelToolStripButton";
            cancelToolStripButton.Size = new System.Drawing.Size(63, 22);
            cancelToolStripButton.Text = "&Cancel";
            cancelToolStripButton.Click += CancelToolStripButton1_Click;
            // 
            // connectionsLabel
            // 
            connectionsLabel.AutoSize = true;
            connectionsLabel.Location = new System.Drawing.Point(8, 27);
            connectionsLabel.Name = "connectionsLabel";
            connectionsLabel.Size = new System.Drawing.Size(77, 15);
            connectionsLabel.TabIndex = 1;
            connectionsLabel.Text = "Connections:";
            // 
            // connectionsListBox
            // 
            connectionsListBox.FormattingEnabled = true;
            connectionsListBox.ItemHeight = 15;
            connectionsListBox.Location = new System.Drawing.Point(8, 43);
            connectionsListBox.Name = "connectionsListBox";
            connectionsListBox.Size = new System.Drawing.Size(202, 334);
            connectionsListBox.TabIndex = 2;
            connectionsListBox.SelectedIndexChanged += ConnectionsListBox_SelectedIndexChanged;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(283, 285);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(75, 23);
            saveButton.TabIndex = 7;
            saveButton.Text = "Add";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButton_Click;
            // 
            // logonGroupBox
            // 
            logonGroupBox.Controls.Add(trustCertificateCheckBox);
            logonGroupBox.Controls.Add(encrptyCheckBox);
            logonGroupBox.Controls.Add(rememberPasswordCheckBox);
            logonGroupBox.Controls.Add(authenticationComboBox);
            logonGroupBox.Controls.Add(passwordTextBox);
            logonGroupBox.Controls.Add(userNameTextBox);
            logonGroupBox.Controls.Add(passwordLabel);
            logonGroupBox.Controls.Add(authenticationLabel);
            logonGroupBox.Controls.Add(userNameLabel);
            logonGroupBox.Location = new System.Drawing.Point(6, 93);
            logonGroupBox.Name = "logonGroupBox";
            logonGroupBox.Size = new System.Drawing.Size(352, 188);
            logonGroupBox.TabIndex = 6;
            logonGroupBox.TabStop = false;
            logonGroupBox.Text = "Log on to the server";
            // 
            // trustCertificateCheckBox
            // 
            trustCertificateCheckBox.AutoSize = true;
            trustCertificateCheckBox.Checked = true;
            trustCertificateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            trustCertificateCheckBox.Location = new System.Drawing.Point(101, 160);
            trustCertificateCheckBox.Name = "trustCertificateCheckBox";
            trustCertificateCheckBox.Size = new System.Drawing.Size(141, 19);
            trustCertificateCheckBox.TabIndex = 8;
            trustCertificateCheckBox.Text = "Trust server certificate";
            trustCertificateCheckBox.UseVisualStyleBackColor = true;
            trustCertificateCheckBox.CheckedChanged += SettingsChanged;
            // 
            // encrptyCheckBox
            // 
            encrptyCheckBox.AutoSize = true;
            encrptyCheckBox.Checked = true;
            encrptyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            encrptyCheckBox.Location = new System.Drawing.Point(100, 135);
            encrptyCheckBox.Name = "encrptyCheckBox";
            encrptyCheckBox.Size = new System.Drawing.Size(129, 19);
            encrptyCheckBox.TabIndex = 7;
            encrptyCheckBox.Text = "Encrypt connection";
            encrptyCheckBox.UseVisualStyleBackColor = true;
            encrptyCheckBox.CheckedChanged += SettingsChanged;
            // 
            // rememberPasswordCheckBox
            // 
            rememberPasswordCheckBox.AutoSize = true;
            rememberPasswordCheckBox.Location = new System.Drawing.Point(100, 110);
            rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            rememberPasswordCheckBox.Size = new System.Drawing.Size(137, 19);
            rememberPasswordCheckBox.TabIndex = 6;
            rememberPasswordCheckBox.Text = "Remember password";
            rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            rememberPasswordCheckBox.CheckedChanged += SettingsChanged;
            // 
            // authenticationComboBox
            // 
            authenticationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            authenticationComboBox.FormattingEnabled = true;
            authenticationComboBox.Location = new System.Drawing.Point(100, 23);
            authenticationComboBox.Name = "authenticationComboBox";
            authenticationComboBox.Size = new System.Drawing.Size(247, 23);
            authenticationComboBox.TabIndex = 1;
            authenticationComboBox.SelectedIndexChanged += AuthenticationComboBox_SelectedIndexChanged;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new System.Drawing.Point(100, 81);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new System.Drawing.Size(247, 23);
            passwordTextBox.TabIndex = 5;
            passwordTextBox.TextChanged += SettingsChanged;
            // 
            // userNameTextBox
            // 
            userNameTextBox.Location = new System.Drawing.Point(100, 52);
            userNameTextBox.Name = "userNameTextBox";
            userNameTextBox.Size = new System.Drawing.Size(247, 23);
            userNameTextBox.TabIndex = 3;
            userNameTextBox.TextChanged += SettingsChanged;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(34, 84);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(60, 15);
            passwordLabel.TabIndex = 4;
            passwordLabel.Text = "Password:";
            // 
            // authenticationLabel
            // 
            authenticationLabel.AutoSize = true;
            authenticationLabel.Location = new System.Drawing.Point(5, 26);
            authenticationLabel.Name = "authenticationLabel";
            authenticationLabel.Size = new System.Drawing.Size(89, 15);
            authenticationLabel.TabIndex = 0;
            authenticationLabel.Text = "Authentication:";
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Location = new System.Drawing.Point(28, 55);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new System.Drawing.Size(66, 15);
            userNameLabel.TabIndex = 2;
            userNameLabel.Text = "User name:";
            // 
            // serverNameTextBox
            // 
            serverNameTextBox.Location = new System.Drawing.Point(106, 35);
            serverNameTextBox.Name = "serverNameTextBox";
            serverNameTextBox.Size = new System.Drawing.Size(247, 23);
            serverNameTextBox.TabIndex = 3;
            serverNameTextBox.TextChanged += SettingsChanged;
            // 
            // connectionNameTextBox
            // 
            connectionNameTextBox.Location = new System.Drawing.Point(106, 6);
            connectionNameTextBox.Name = "connectionNameTextBox";
            connectionNameTextBox.Size = new System.Drawing.Size(247, 23);
            connectionNameTextBox.TabIndex = 1;
            connectionNameTextBox.TextChanged += SettingsChanged;
            connectionNameTextBox.Validated += ConnectionNameTextBox_Validated;
            // 
            // serverNameLabel
            // 
            serverNameLabel.AutoSize = true;
            serverNameLabel.Location = new System.Drawing.Point(25, 38);
            serverNameLabel.Name = "serverNameLabel";
            serverNameLabel.Size = new System.Drawing.Size(75, 15);
            serverNameLabel.TabIndex = 2;
            serverNameLabel.Text = "Server name:";
            // 
            // databaseComboBox
            // 
            databaseComboBox.FormattingEnabled = true;
            databaseComboBox.Location = new System.Drawing.Point(106, 64);
            databaseComboBox.Name = "databaseComboBox";
            databaseComboBox.Size = new System.Drawing.Size(247, 23);
            databaseComboBox.TabIndex = 5;
            databaseComboBox.SelectedIndexChanged += SettingsChanged;
            databaseComboBox.TextChanged += SettingsChanged;
            // 
            // databaseNameLabel
            // 
            databaseNameLabel.AutoSize = true;
            databaseNameLabel.Location = new System.Drawing.Point(42, 67);
            databaseNameLabel.Name = "databaseNameLabel";
            databaseNameLabel.Size = new System.Drawing.Size(58, 15);
            databaseNameLabel.TabIndex = 4;
            databaseNameLabel.Text = "Database:";
            // 
            // connectionNameLabel
            // 
            connectionNameLabel.AutoSize = true;
            connectionNameLabel.Location = new System.Drawing.Point(58, 9);
            connectionNameLabel.Name = "connectionNameLabel";
            connectionNameLabel.Size = new System.Drawing.Size(42, 15);
            connectionNameLabel.TabIndex = 0;
            connectionNameLabel.Text = "Name:";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new System.Drawing.Point(214, 35);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(374, 342);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(saveButton);
            tabPage1.Controls.Add(connectionNameTextBox);
            tabPage1.Controls.Add(logonGroupBox);
            tabPage1.Controls.Add(connectionNameLabel);
            tabPage1.Controls.Add(serverNameTextBox);
            tabPage1.Controls.Add(databaseNameLabel);
            tabPage1.Controls.Add(databaseComboBox);
            tabPage1.Controls.Add(serverNameLabel);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(366, 314);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Connection";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(labelDbDesc);
            tabPage2.Controls.Add(dbDescTextBox);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(366, 314);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Database Description";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // labelDbDesc
            // 
            labelDbDesc.AutoSize = true;
            labelDbDesc.Location = new System.Drawing.Point(6, 3);
            labelDbDesc.Name = "labelDbDesc";
            labelDbDesc.Size = new System.Drawing.Size(209, 15);
            labelDbDesc.TabIndex = 12;
            labelDbDesc.Text = "Database Description (for AI assistant):";
            // 
            // dbDescTextBox
            // 
            dbDescTextBox.Location = new System.Drawing.Point(6, 21);
            dbDescTextBox.Multiline = true;
            dbDescTextBox.Name = "dbDescTextBox";
            dbDescTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            dbDescTextBox.Size = new System.Drawing.Size(354, 287);
            dbDescTextBox.TabIndex = 13;
            dbDescTextBox.TextChanged += SettingsChanged;
            // 
            // ConnectionManageForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(592, 383);
            Controls.Add(tabControl1);
            Controls.Add(connectionsListBox);
            Controls.Add(connectionsLabel);
            Controls.Add(toolStrip1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConnectionManageForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Connection Manager";
            Load += ConnectionManageForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            logonGroupBox.ResumeLayout(false);
            logonGroupBox.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addToolStripButton;
        private System.Windows.Forms.ToolStripButton deleteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton moveUpToolStripButton;
        private System.Windows.Forms.ToolStripButton moveDownToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton testToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton closeToolStripButton;
        private System.Windows.Forms.ToolStripButton cancelToolStripButton;
        private System.Windows.Forms.Label connectionsLabel;
        private System.Windows.Forms.ListBox connectionsListBox;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.TextBox connectionNameTextBox;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.ComboBox databaseComboBox;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.Label connectionNameLabel;
        private System.Windows.Forms.GroupBox logonGroupBox;
        private System.Windows.Forms.CheckBox rememberPasswordCheckBox;
        private System.Windows.Forms.ComboBox authenticationComboBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.CheckBox trustCertificateCheckBox;
        private System.Windows.Forms.CheckBox encrptyCheckBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelDbDesc;
        private System.Windows.Forms.TextBox dbDescTextBox;
    }
}