namespace SQL_Document_Builder
{
    partial class SQLConnectionBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLConnectionBox));
            serverNameTextBox = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            trustCheckBox = new System.Windows.Forms.CheckBox();
            encryptCheckBox = new System.Windows.Forms.CheckBox();
            rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            authenticationLabel = new System.Windows.Forms.Label();
            passwordTextBox = new System.Windows.Forms.TextBox();
            authenticationComboBox = new System.Windows.Forms.ComboBox();
            userNameLabel = new System.Windows.Forms.Label();
            passwordLabel = new System.Windows.Forms.Label();
            userNameTextBox = new System.Windows.Forms.TextBox();
            databaseTextBox = new System.Windows.Forms.ComboBox();
            serverNameLabel = new System.Windows.Forms.Label();
            databaseLabel = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // serverNameTextBox
            // 
            resources.ApplyResources(serverNameTextBox, "serverNameTextBox");
            serverNameTextBox.Name = "serverNameTextBox";
            serverNameTextBox.TextChanged += ServerNameTextBox_TextChanged;
            serverNameTextBox.Validated += ServerNameTextBox_Validated;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(trustCheckBox);
            groupBox1.Controls.Add(encryptCheckBox);
            groupBox1.Controls.Add(rememberPasswordCheckBox);
            groupBox1.Controls.Add(authenticationLabel);
            groupBox1.Controls.Add(passwordTextBox);
            groupBox1.Controls.Add(authenticationComboBox);
            groupBox1.Controls.Add(userNameLabel);
            groupBox1.Controls.Add(passwordLabel);
            groupBox1.Controls.Add(userNameTextBox);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // trustCheckBox
            // 
            resources.ApplyResources(trustCheckBox, "trustCheckBox");
            trustCheckBox.Checked = true;
            trustCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            trustCheckBox.Name = "trustCheckBox";
            trustCheckBox.UseVisualStyleBackColor = true;
            trustCheckBox.CheckedChanged += OnSettingsChanged;
            // 
            // encryptCheckBox
            // 
            resources.ApplyResources(encryptCheckBox, "encryptCheckBox");
            encryptCheckBox.Checked = true;
            encryptCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            encryptCheckBox.Name = "encryptCheckBox";
            encryptCheckBox.UseVisualStyleBackColor = true;
            encryptCheckBox.CheckedChanged += OnSettingsChanged;
            // 
            // rememberPasswordCheckBox
            // 
            resources.ApplyResources(rememberPasswordCheckBox, "rememberPasswordCheckBox");
            rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            rememberPasswordCheckBox.CheckedChanged += OnSettingsChanged;
            // 
            // authenticationLabel
            // 
            resources.ApplyResources(authenticationLabel, "authenticationLabel");
            authenticationLabel.Name = "authenticationLabel";
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(passwordTextBox, "passwordTextBox");
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.TextChanged += OnSettingsChanged;
            // 
            // authenticationComboBox
            // 
            authenticationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            authenticationComboBox.FormattingEnabled = true;
            authenticationComboBox.Items.AddRange(new object[] { resources.GetString("authenticationComboBox.Items"), resources.GetString("authenticationComboBox.Items1"), resources.GetString("authenticationComboBox.Items2"), resources.GetString("authenticationComboBox.Items3"), resources.GetString("authenticationComboBox.Items4"), resources.GetString("authenticationComboBox.Items5"), resources.GetString("authenticationComboBox.Items6") });
            resources.ApplyResources(authenticationComboBox, "authenticationComboBox");
            authenticationComboBox.Name = "authenticationComboBox";
            authenticationComboBox.SelectedIndexChanged += AuthenticationComboBox_SelectedIndexChanged;
            // 
            // userNameLabel
            // 
            resources.ApplyResources(userNameLabel, "userNameLabel");
            userNameLabel.Name = "userNameLabel";
            // 
            // passwordLabel
            // 
            resources.ApplyResources(passwordLabel, "passwordLabel");
            passwordLabel.Name = "passwordLabel";
            // 
            // userNameTextBox
            // 
            resources.ApplyResources(userNameTextBox, "userNameTextBox");
            userNameTextBox.Name = "userNameTextBox";
            userNameTextBox.TextChanged += OnSettingsChanged;
            // 
            // databaseTextBox
            // 
            resources.ApplyResources(databaseTextBox, "databaseTextBox");
            databaseTextBox.Name = "databaseTextBox";
            databaseTextBox.SelectedIndexChanged += DatabaseTextBox_SelectedIndexChanged;
            databaseTextBox.TextChanged += OnSettingsChanged;
            // 
            // serverNameLabel
            // 
            resources.ApplyResources(serverNameLabel, "serverNameLabel");
            serverNameLabel.Name = "serverNameLabel";
            // 
            // databaseLabel
            // 
            resources.ApplyResources(databaseLabel, "databaseLabel");
            databaseLabel.Name = "databaseLabel";
            // 
            // SQLConnectionBox
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(databaseTextBox);
            Controls.Add(serverNameTextBox);
            Controls.Add(groupBox1);
            Controls.Add(serverNameLabel);
            Controls.Add(databaseLabel);
            Name = "SQLConnectionBox";
            Load += SQLConnectionBox_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox rememberPasswordCheckBox;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.ComboBox authenticationComboBox;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.ComboBox databaseTextBox;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.Label databaseLabel;
        private System.Windows.Forms.CheckBox trustCheckBox;
        private System.Windows.Forms.CheckBox encryptCheckBox;
    }
}
