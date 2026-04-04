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
            SQLServerConnections sqlServerConnections1 = new SQLServerConnections();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            connectionTypeLabel = new System.Windows.Forms.Label();
            connectionTypeComboBox = new System.Windows.Forms.ComboBox();
            sqlConnSettingBox = new SQLConnectionBox();
            odbcConnSettingBox = new ODBCConnectionBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            connectionTabPage = new System.Windows.Forms.TabPage();
            descriptionTabPage = new System.Windows.Forms.TabPage();
            labelDbDesc = new System.Windows.Forms.Label();
            dbDescTextBox = new System.Windows.Forms.TextBox();
            tabControl1.SuspendLayout();
            connectionTabPage.SuspendLayout();
            descriptionTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(250, 352);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 5;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(331, 352);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // connectionTypeLabel
            // 
            connectionTypeLabel.AutoSize = true;
            connectionTypeLabel.Location = new System.Drawing.Point(26, 13);
            connectionTypeLabel.Name = "connectionTypeLabel";
            connectionTypeLabel.Size = new System.Drawing.Size(98, 15);
            connectionTypeLabel.TabIndex = 0;
            connectionTypeLabel.Text = "Connection type:";
            // 
            // connectionTypeComboBox
            // 
            connectionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            connectionTypeComboBox.FormattingEnabled = true;
            connectionTypeComboBox.Items.AddRange(new object[] { "SQL Server", "ODBC" });
            connectionTypeComboBox.Location = new System.Drawing.Point(129, 10);
            connectionTypeComboBox.Name = "connectionTypeComboBox";
            connectionTypeComboBox.Size = new System.Drawing.Size(254, 23);
            connectionTypeComboBox.TabIndex = 1;
            connectionTypeComboBox.SelectedIndexChanged += ConnectionTypeComboBox_SelectedIndexChanged;
            // 
            // sqlConnSettingBox
            // 
            sqlConnSettingBox.Authentication = 0;
            sqlConnSettingBox.DatabaseName = "";
            sqlConnSettingBox.EncryptConnection = true;
            sqlConnSettingBox.Location = new System.Drawing.Point(12, 39);
            sqlConnSettingBox.Margin = new System.Windows.Forms.Padding(2);
            sqlConnSettingBox.Name = "sqlConnSettingBox";
            sqlConnSettingBox.Password = "";
            sqlConnSettingBox.RememberPassword = false;
            sqlConnSettingBox.RequireManualLogin = false;
            sqlConnSettingBox.ServerName = "";
            sqlConnSettingBox.Size = new System.Drawing.Size(378, 271);
            sqlConnSettingBox.TabIndex = 2;
            sqlConnSettingBox.TrustServerCertificate = true;
            sqlConnSettingBox.UserName = "";
            sqlConnSettingBox.CompleteStatusChanged += OnSettingsChanged;
            // 
            // odbcConnSettingBox
            // 
            odbcConnSettingBox.Connections = sqlServerConnections1;
            odbcConnSettingBox.DSN = "JCM_Prod";
            odbcConnSettingBox.Location = new System.Drawing.Point(12, 39);
            odbcConnSettingBox.Margin = new System.Windows.Forms.Padding(2);
            odbcConnSettingBox.Name = "odbcConnSettingBox";
            odbcConnSettingBox.Password = "";
            odbcConnSettingBox.ReadOnly = false;
            odbcConnSettingBox.RememberPassword = false;
            odbcConnSettingBox.RequireManualLogin = false;
            odbcConnSettingBox.Size = new System.Drawing.Size(378, 255);
            odbcConnSettingBox.TabIndex = 3;
            odbcConnSettingBox.UserName = "";
            odbcConnSettingBox.CompleteStatusChanged += OnSettingsChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(connectionTabPage);
            tabControl1.Controls.Add(descriptionTabPage);
            tabControl1.Location = new System.Drawing.Point(7, 7);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(409, 339);
            tabControl1.TabIndex = 7;
            // 
            // connectionTabPage
            // 
            connectionTabPage.Controls.Add(connectionTypeLabel);
            connectionTabPage.Controls.Add(connectionTypeComboBox);
            connectionTabPage.Controls.Add(sqlConnSettingBox);
            connectionTabPage.Controls.Add(odbcConnSettingBox);
            connectionTabPage.Location = new System.Drawing.Point(4, 24);
            connectionTabPage.Name = "connectionTabPage";
            connectionTabPage.Padding = new System.Windows.Forms.Padding(3);
            connectionTabPage.Size = new System.Drawing.Size(401, 311);
            connectionTabPage.TabIndex = 0;
            connectionTabPage.Text = "Connection";
            connectionTabPage.UseVisualStyleBackColor = true;
            // 
            // descriptionTabPage
            // 
            descriptionTabPage.Controls.Add(labelDbDesc);
            descriptionTabPage.Controls.Add(dbDescTextBox);
            descriptionTabPage.Location = new System.Drawing.Point(4, 24);
            descriptionTabPage.Name = "descriptionTabPage";
            descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
            descriptionTabPage.Size = new System.Drawing.Size(401, 311);
            descriptionTabPage.TabIndex = 1;
            descriptionTabPage.Text = "Description";
            descriptionTabPage.UseVisualStyleBackColor = true;
            // 
            // labelDbDesc
            // 
            labelDbDesc.AutoSize = true;
            labelDbDesc.Location = new System.Drawing.Point(6, 3);
            labelDbDesc.Name = "labelDbDesc";
            labelDbDesc.Size = new System.Drawing.Size(209, 15);
            labelDbDesc.TabIndex = 10;
            labelDbDesc.Text = "Database Description (for AI assistant):";
            // 
            // dbDescTextBox
            // 
            dbDescTextBox.Location = new System.Drawing.Point(6, 21);
            dbDescTextBox.Multiline = true;
            dbDescTextBox.Name = "dbDescTextBox";
            dbDescTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            dbDescTextBox.Size = new System.Drawing.Size(384, 271);
            dbDescTextBox.TabIndex = 11;
            // 
            // NewSQLServerConnectionDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(423, 383);
            Controls.Add(tabControl1);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewSQLServerConnectionDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "New Connection Dialog";
            Load += NewSQLServerConnectionDialog_Load;
            tabControl1.ResumeLayout(false);
            connectionTabPage.ResumeLayout(false);
            connectionTabPage.PerformLayout();
            descriptionTabPage.ResumeLayout(false);
            descriptionTabPage.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label connectionTypeLabel;
        private System.Windows.Forms.ComboBox connectionTypeComboBox;
        private SQLConnectionBox sqlConnSettingBox;
        private ODBCConnectionBox odbcConnSettingBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage connectionTabPage;
        private System.Windows.Forms.TabPage descriptionTabPage;
        private System.Windows.Forms.Label labelDbDesc;
        private System.Windows.Forms.TextBox dbDescTextBox;
    }
}