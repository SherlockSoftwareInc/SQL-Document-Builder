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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnReferenceDialog));
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
            resources.ApplyResources(searchButton, "searchButton");
            searchButton.Name = "searchButton";
            searchButton.UseVisualStyleBackColor = true;
            // 
            // clearSerachButton
            // 
            resources.ApplyResources(clearSerachButton, "clearSerachButton");
            clearSerachButton.Name = "clearSerachButton";
            clearSerachButton.UseVisualStyleBackColor = true;
            clearSerachButton.Click += ClearSerachButton_Click;
            // 
            // searchTextBox
            // 
            resources.ApplyResources(searchTextBox, "searchTextBox");
            searchTextBox.Name = "searchTextBox";
            // 
            // schemaLabel
            // 
            resources.ApplyResources(schemaLabel, "schemaLabel");
            schemaLabel.Name = "schemaLabel";
            // 
            // searchLabel
            // 
            resources.ApplyResources(searchLabel, "searchLabel");
            searchLabel.Name = "searchLabel";
            // 
            // schemaComboBox
            // 
            resources.ApplyResources(schemaComboBox, "schemaComboBox");
            schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Name = "schemaComboBox";
            // 
            // objectsListBox
            // 
            resources.ApplyResources(objectsListBox, "objectsListBox");
            objectsListBox.FormattingEnabled = true;
            objectsListBox.Name = "objectsListBox";
            objectsListBox.DoubleClick += OkButton_Click;
            // 
            // okButton
            // 
            resources.ApplyResources(okButton, "okButton");
            okButton.Name = "okButton";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.Name = "cancelButton";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // ColumnReferenceDialog
            // 
            AcceptButton = okButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
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