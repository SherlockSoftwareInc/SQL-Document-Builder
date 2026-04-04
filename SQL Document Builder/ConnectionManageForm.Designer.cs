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
            SQLServerConnections sqlServerConnections1 = new SQLServerConnections();
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
            connectionNameTextBox = new System.Windows.Forms.TextBox();
            connectionNameLabel = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            _connectionTypeLabel = new System.Windows.Forms.Label();
            _connectionTypeComboBox = new System.Windows.Forms.ComboBox();
            _sqlConnSettingBox = new SQLConnectionBox();
            _odbcConnSettingBox = new ODBCConnectionBox();
            tabPage2 = new System.Windows.Forms.TabPage();
            labelDbDesc = new System.Windows.Forms.Label();
            dbDescTextBox = new System.Windows.Forms.TextBox();
            toolStrip1.SuspendLayout();
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
            connectionsListBox.Size = new System.Drawing.Size(202, 379);
            connectionsListBox.TabIndex = 2;
            connectionsListBox.SelectedIndexChanged += ConnectionsListBox_SelectedIndexChanged;
            // 
            // saveButton
            // 
            saveButton.Location = new System.Drawing.Point(278, 331);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(75, 23);
            saveButton.TabIndex = 7;
            saveButton.Text = "Add";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButton_Click;
            // 
            // connectionNameTextBox
            // 
            connectionNameTextBox.Location = new System.Drawing.Point(125, 6);
            connectionNameTextBox.Name = "connectionNameTextBox";
            connectionNameTextBox.Size = new System.Drawing.Size(228, 23);
            connectionNameTextBox.TabIndex = 1;
            connectionNameTextBox.TextChanged += SettingsChanged;
            connectionNameTextBox.Validated += ConnectionNameTextBox_Validated;
            // 
            // connectionNameLabel
            // 
            connectionNameLabel.AutoSize = true;
            connectionNameLabel.Location = new System.Drawing.Point(77, 9);
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
            tabControl1.Size = new System.Drawing.Size(374, 389);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(saveButton);
            tabPage1.Controls.Add(connectionNameTextBox);
            tabPage1.Controls.Add(connectionNameLabel);
            tabPage1.Controls.Add(_connectionTypeLabel);
            tabPage1.Controls.Add(_connectionTypeComboBox);
            tabPage1.Controls.Add(_sqlConnSettingBox);
            tabPage1.Controls.Add(_odbcConnSettingBox);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(366, 361);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Connection";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // _connectionTypeLabel
            // 
            _connectionTypeLabel.AutoSize = true;
            _connectionTypeLabel.Location = new System.Drawing.Point(21, 38);
            _connectionTypeLabel.Name = "_connectionTypeLabel";
            _connectionTypeLabel.Size = new System.Drawing.Size(98, 15);
            _connectionTypeLabel.TabIndex = 2;
            _connectionTypeLabel.Text = "Connection type:";
            // 
            // _connectionTypeComboBox
            // 
            _connectionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _connectionTypeComboBox.FormattingEnabled = true;
            _connectionTypeComboBox.Items.AddRange(new object[] { "SQL Server", "ODBC" });
            _connectionTypeComboBox.Location = new System.Drawing.Point(125, 35);
            _connectionTypeComboBox.Name = "_connectionTypeComboBox";
            _connectionTypeComboBox.Size = new System.Drawing.Size(228, 23);
            _connectionTypeComboBox.TabIndex = 3;
            _connectionTypeComboBox.SelectedIndexChanged += ConnectionTypeComboBox_SelectedIndexChanged;
            // 
            // _sqlConnSettingBox
            // 
            _sqlConnSettingBox.Authentication = 0;
            _sqlConnSettingBox.DatabaseName = "";
            _sqlConnSettingBox.EncryptConnection = true;
            _sqlConnSettingBox.Location = new System.Drawing.Point(6, 64);
            _sqlConnSettingBox.Margin = new System.Windows.Forms.Padding(2);
            _sqlConnSettingBox.Name = "_sqlConnSettingBox";
            _sqlConnSettingBox.Password = "";
            _sqlConnSettingBox.RememberPassword = false;
            _sqlConnSettingBox.RequireManualLogin = false;
            _sqlConnSettingBox.ServerName = "";
            _sqlConnSettingBox.Size = new System.Drawing.Size(352, 262);
            _sqlConnSettingBox.TabIndex = 4;
            _sqlConnSettingBox.TrustServerCertificate = true;
            _sqlConnSettingBox.UserName = "";
            _sqlConnSettingBox.SettingChanged += SettingsChanged;
            _sqlConnSettingBox.CompleteStatusChanged += SettingsChanged;
            // 
            // _odbcConnSettingBox
            // 
            _odbcConnSettingBox.Connections = sqlServerConnections1;
            _odbcConnSettingBox.DSN = "JCM_Prod";
            _odbcConnSettingBox.Location = new System.Drawing.Point(6, 64);
            _odbcConnSettingBox.Margin = new System.Windows.Forms.Padding(2);
            _odbcConnSettingBox.Name = "_odbcConnSettingBox";
            _odbcConnSettingBox.Password = "";
            _odbcConnSettingBox.ReadOnly = false;
            _odbcConnSettingBox.RememberPassword = false;
            _odbcConnSettingBox.RequireManualLogin = false;
            _odbcConnSettingBox.Size = new System.Drawing.Size(352, 262);
            _odbcConnSettingBox.TabIndex = 5;
            _odbcConnSettingBox.UserName = "";
            _odbcConnSettingBox.SettingChanged += SettingsChanged;
            _odbcConnSettingBox.CompleteStatusChanged += SettingsChanged;
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
            ClientSize = new System.Drawing.Size(592, 430);
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
        private System.Windows.Forms.TextBox connectionNameTextBox;
        private System.Windows.Forms.Label connectionNameLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelDbDesc;
        private System.Windows.Forms.TextBox dbDescTextBox;
        private System.Windows.Forms.Label _connectionTypeLabel;
        private System.Windows.Forms.ComboBox _connectionTypeComboBox;
        private SQLConnectionBox _sqlConnSettingBox;
        private ODBCConnectionBox _odbcConnSettingBox;
    }
}