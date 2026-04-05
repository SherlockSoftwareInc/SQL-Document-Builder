namespace SQL_Document_Builder
{
    partial class SettingsForm
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
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            labelEndpoint = new System.Windows.Forms.Label();
            textBoxEndpoint = new System.Windows.Forms.TextBox();
            labelModel = new System.Windows.Forms.Label();
            textBoxModel = new System.Windows.Forms.TextBox();
            labelApiKey = new System.Windows.Forms.Label();
            textBoxApiKey = new System.Windows.Forms.TextBox();
            labelLanguage = new System.Windows.Forms.Label();
            comboBoxLanguage = new System.Windows.Forms.ComboBox();
            buttonSave = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new System.Drawing.Point(3, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(408, 285);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(labelEndpoint);
            tabPage1.Controls.Add(textBoxEndpoint);
            tabPage1.Controls.Add(labelModel);
            tabPage1.Controls.Add(textBoxModel);
            tabPage1.Controls.Add(labelApiKey);
            tabPage1.Controls.Add(textBoxApiKey);
            tabPage1.Controls.Add(labelLanguage);
            tabPage1.Controls.Add(comboBoxLanguage);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(400, 257);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "AI Settings";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // labelEndpoint
            // 
            labelEndpoint.AutoSize = true;
            labelEndpoint.Location = new System.Drawing.Point(16, 20);
            labelEndpoint.Name = "labelEndpoint";
            labelEndpoint.Size = new System.Drawing.Size(58, 15);
            labelEndpoint.TabIndex = 0;
            labelEndpoint.Text = "Endpoint:";
            // 
            // textBoxEndpoint
            // 
            textBoxEndpoint.Location = new System.Drawing.Point(84, 17);
            textBoxEndpoint.Name = "textBoxEndpoint";
            textBoxEndpoint.Size = new System.Drawing.Size(300, 23);
            textBoxEndpoint.TabIndex = 1;
            // 
            // labelModel
            // 
            labelModel.AutoSize = true;
            labelModel.Location = new System.Drawing.Point(16, 49);
            labelModel.Name = "labelModel";
            labelModel.Size = new System.Drawing.Size(58, 15);
            labelModel.TabIndex = 2;
            labelModel.Text = "AI Model:";
            // 
            // textBoxModel
            // 
            textBoxModel.Location = new System.Drawing.Point(84, 46);
            textBoxModel.Name = "textBoxModel";
            textBoxModel.Size = new System.Drawing.Size(300, 23);
            textBoxModel.TabIndex = 3;
            // 
            // labelApiKey
            // 
            labelApiKey.AutoSize = true;
            labelApiKey.Location = new System.Drawing.Point(16, 78);
            labelApiKey.Name = "labelApiKey";
            labelApiKey.Size = new System.Drawing.Size(50, 15);
            labelApiKey.TabIndex = 4;
            labelApiKey.Text = "API Key:";
            // 
            // textBoxApiKey
            // 
            textBoxApiKey.Location = new System.Drawing.Point(84, 75);
            textBoxApiKey.Name = "textBoxApiKey";
            textBoxApiKey.Size = new System.Drawing.Size(300, 23);
            textBoxApiKey.TabIndex = 5;
            textBoxApiKey.UseSystemPasswordChar = true;
            // 
            // labelLanguage
            // 
            labelLanguage.AutoSize = true;
            labelLanguage.Location = new System.Drawing.Point(16, 107);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new System.Drawing.Size(62, 15);
            labelLanguage.TabIndex = 6;
            labelLanguage.Text = "Language:";
            // 
            // comboBoxLanguage
            // 
            comboBoxLanguage.Location = new System.Drawing.Point(84, 104);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new System.Drawing.Size(300, 23);
            comboBoxLanguage.TabIndex = 7;
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(305, 290);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(100, 30);
            buttonSave.TabIndex = 10;
            buttonSave.Text = "&OK";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(199, 290);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(100, 30);
            cancelButton.TabIndex = 10;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(420, 326);
            Controls.Add(tabControl1);
            Controls.Add(cancelButton);
            Controls.Add(buttonSave);
            Name = "SettingsForm";
            Text = "Settings";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label labelEndpoint;
        private System.Windows.Forms.TextBox textBoxEndpoint;
        private System.Windows.Forms.Label labelModel;
        private System.Windows.Forms.TextBox textBoxModel;
        private System.Windows.Forms.Label labelApiKey;
        private System.Windows.Forms.TextBox textBoxApiKey;
        private System.Windows.Forms.Label labelLanguage;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button cancelButton;
    }
}