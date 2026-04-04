namespace SQL_Document_Builder
{
    partial class ODBCConnectionBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ODBCConnectionBox));
            dsnComboBox = new System.Windows.Forms.ComboBox();
            dsnLabel = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            manualLoginCheckBox = new System.Windows.Forms.CheckBox();
            rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            passwordTextBox = new System.Windows.Forms.TextBox();
            userNameLabel = new System.Windows.Forms.Label();
            passwordLabel = new System.Windows.Forms.Label();
            userNameTextBox = new System.Windows.Forms.TextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // dsnComboBox
            // 
            dsnComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(dsnComboBox, "dsnComboBox");
            dsnComboBox.Name = "dsnComboBox";
            dsnComboBox.SelectedIndexChanged += OnSettingsChanged;
            // 
            // dsnLabel
            // 
            resources.ApplyResources(dsnLabel, "dsnLabel");
            dsnLabel.Name = "dsnLabel";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(manualLoginCheckBox);
            groupBox1.Controls.Add(rememberPasswordCheckBox);
            groupBox1.Controls.Add(passwordTextBox);
            groupBox1.Controls.Add(userNameLabel);
            groupBox1.Controls.Add(passwordLabel);
            groupBox1.Controls.Add(userNameTextBox);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // manualLoginCheckBox
            // 
            resources.ApplyResources(manualLoginCheckBox, "manualLoginCheckBox");
            manualLoginCheckBox.Name = "manualLoginCheckBox";
            manualLoginCheckBox.UseVisualStyleBackColor = true;
            manualLoginCheckBox.CheckedChanged += ManualLoginCheckBox_CheckedChanged;
            // 
            // rememberPasswordCheckBox
            // 
            resources.ApplyResources(rememberPasswordCheckBox, "rememberPasswordCheckBox");
            rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            rememberPasswordCheckBox.CheckedChanged += OnSettingsChanged;
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(passwordTextBox, "passwordTextBox");
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.TextChanged += PasswordTextBox_TextChanged;
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
            // ODBCConnectionBox
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(dsnComboBox);
            Controls.Add(dsnLabel);
            Controls.Add(groupBox1);
            Name = "ODBCConnectionBox";
            Load += ODBCConnectionBox_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox dsnComboBox;
        private System.Windows.Forms.Label dsnLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox manualLoginCheckBox;
        private System.Windows.Forms.CheckBox rememberPasswordCheckBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox userNameTextBox;
    }
}
