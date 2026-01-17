namespace SQL_Document_Builder
{
    partial class ColumnReferenceDialog
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
            searchButton = new System.Windows.Forms.Button();
            clearSerachButton = new System.Windows.Forms.Button();
            searchTextBox = new System.Windows.Forms.TextBox();
            schemaLabel = new System.Windows.Forms.Label();
            searchLabel = new System.Windows.Forms.Label();
            schemaComboBox = new System.Windows.Forms.ComboBox();
            objectsListBox = new System.Windows.Forms.ListBox();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // searchButton
            // 
            searchButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            searchButton.Location = new System.Drawing.Point(359, 68);
            searchButton.Name = "searchButton";
            searchButton.Size = new System.Drawing.Size(23, 23);
            searchButton.TabIndex = 3;
            searchButton.Text = "🔍";
            searchButton.UseVisualStyleBackColor = true;
            // 
            // clearSerachButton
            // 
            clearSerachButton.Location = new System.Drawing.Point(336, 68);
            clearSerachButton.Name = "clearSerachButton";
            clearSerachButton.Size = new System.Drawing.Size(23, 23);
            clearSerachButton.TabIndex = 2;
            clearSerachButton.Text = "X";
            clearSerachButton.UseVisualStyleBackColor = true;
            clearSerachButton.Click += ClearSerachButton_Click;
            // 
            // searchTextBox
            // 
            searchTextBox.Location = new System.Drawing.Point(12, 69);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new System.Drawing.Size(320, 23);
            searchTextBox.TabIndex = 1;
            // 
            // schemaLabel
            // 
            schemaLabel.AutoSize = true;
            schemaLabel.Location = new System.Drawing.Point(12, 9);
            schemaLabel.Name = "schemaLabel";
            schemaLabel.Size = new System.Drawing.Size(52, 15);
            schemaLabel.TabIndex = 6;
            schemaLabel.Text = "Schema:";
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Location = new System.Drawing.Point(12, 51);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new System.Drawing.Size(60, 15);
            searchLabel.TabIndex = 0;
            searchLabel.Text = "Search for";
            // 
            // schemaComboBox
            // 
            schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Location = new System.Drawing.Point(12, 26);
            schemaComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(370, 23);
            schemaComboBox.TabIndex = 7;
            // 
            // objectsListBox
            // 
            objectsListBox.FormattingEnabled = true;
            objectsListBox.IntegralHeight = false;
            objectsListBox.ItemHeight = 15;
            objectsListBox.Location = new System.Drawing.Point(12, 98);
            objectsListBox.Name = "objectsListBox";
            objectsListBox.Size = new System.Drawing.Size(370, 324);
            objectsListBox.TabIndex = 8;
            objectsListBox.DoubleClick += OkButton_Click;
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(399, 12);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 9;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(399, 41);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 10;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // ColumnReferenceDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(490, 432);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(searchButton);
            Controls.Add(clearSerachButton);
            Controls.Add(searchTextBox);
            Controls.Add(schemaLabel);
            Controls.Add(searchLabel);
            Controls.Add(schemaComboBox);
            Controls.Add(objectsListBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ColumnReferenceDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Column Reference Picker Dialog";
            Load += ColumnReferenceDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button clearSerachButton;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Label schemaLabel;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ComboBox schemaComboBox;
        private System.Windows.Forms.ListBox objectsListBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}