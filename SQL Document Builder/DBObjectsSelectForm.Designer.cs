namespace SQL_Document_Builder
{
    partial class DBObjectsSelectForm
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
            label1 = new System.Windows.Forms.Label();
            objectTypeComboBox = new System.Windows.Forms.ComboBox();
            schemaComboBox = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            filterTextBox = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            clearButton = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            selectableListBox = new System.Windows.Forms.ListBox();
            addAllButton = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            selectedListBox = new System.Windows.Forms.ListBox();
            addButton = new System.Windows.Forms.Button();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            removeButton = new System.Windows.Forms.Button();
            removeAllButton = new System.Windows.Forms.Button();
            pasteButton = new System.Windows.Forms.Button();
            copyButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(73, 15);
            label1.TabIndex = 0;
            label1.Text = "Object Type:";
            // 
            // objectTypeComboBox
            // 
            objectTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            objectTypeComboBox.FormattingEnabled = true;
            objectTypeComboBox.Items.AddRange(new object[] { "Table", "View", "Stored Procedure", "Function" });
            objectTypeComboBox.Location = new System.Drawing.Point(105, 12);
            objectTypeComboBox.Name = "objectTypeComboBox";
            objectTypeComboBox.Size = new System.Drawing.Size(183, 23);
            objectTypeComboBox.TabIndex = 1;
            objectTypeComboBox.SelectedIndexChanged += ObjectTypeComboBox_SelectedIndexChanged;
            // 
            // schemaComboBox
            // 
            schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Items.AddRange(new object[] { "Table", "View", "Stored Procedure", "Function" });
            schemaComboBox.Location = new System.Drawing.Point(105, 41);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(183, 23);
            schemaComboBox.TabIndex = 1;
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 44);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(52, 15);
            label2.TabIndex = 0;
            label2.Text = "Schema:";
            // 
            // filterTextBox
            // 
            filterTextBox.Location = new System.Drawing.Point(105, 70);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new System.Drawing.Size(161, 23);
            filterTextBox.TabIndex = 2;
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 73);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(36, 15);
            label3.TabIndex = 0;
            label3.Text = "Filter:";
            // 
            // clearButton
            // 
            clearButton.Location = new System.Drawing.Point(265, 69);
            clearButton.Name = "clearButton";
            clearButton.Size = new System.Drawing.Size(23, 23);
            clearButton.TabIndex = 3;
            clearButton.Text = "X";
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += ClearButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 105);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(104, 15);
            label4.TabIndex = 0;
            label4.Text = "Selectable objects:";
            // 
            // selectableListBox
            // 
            selectableListBox.FormattingEnabled = true;
            selectableListBox.ItemHeight = 15;
            selectableListBox.Location = new System.Drawing.Point(12, 123);
            selectableListBox.Name = "selectableListBox";
            selectableListBox.Size = new System.Drawing.Size(276, 289);
            selectableListBox.TabIndex = 4;
            selectableListBox.DoubleClick += SelectableListBox_DoubleClick;
            // 
            // addAllButton
            // 
            addAllButton.Location = new System.Drawing.Point(294, 141);
            addAllButton.Name = "addAllButton";
            addAllButton.Size = new System.Drawing.Size(32, 23);
            addAllButton.TabIndex = 3;
            addAllButton.Text = ">>";
            addAllButton.UseVisualStyleBackColor = true;
            addAllButton.Click += AddAllButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(332, 9);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(95, 15);
            label5.TabIndex = 0;
            label5.Text = "Selected objects:";
            // 
            // selectedListBox
            // 
            selectedListBox.FormattingEnabled = true;
            selectedListBox.ItemHeight = 15;
            selectedListBox.Location = new System.Drawing.Point(332, 30);
            selectedListBox.Name = "selectedListBox";
            selectedListBox.Size = new System.Drawing.Size(289, 379);
            selectedListBox.TabIndex = 4;
            selectedListBox.DoubleClick += SelectedListBox_DoubleClick;
            // 
            // addButton
            // 
            addButton.Location = new System.Drawing.Point(294, 170);
            addButton.Name = "addButton";
            addButton.Size = new System.Drawing.Size(32, 23);
            addButton.TabIndex = 3;
            addButton.Text = ">";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(553, 415);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(68, 23);
            okButton.TabIndex = 5;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(479, 415);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(68, 23);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // removeButton
            // 
            removeButton.Location = new System.Drawing.Point(294, 199);
            removeButton.Name = "removeButton";
            removeButton.Size = new System.Drawing.Size(32, 23);
            removeButton.TabIndex = 3;
            removeButton.Text = "<";
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += RemoveButton_Click;
            // 
            // removeAllButton
            // 
            removeAllButton.Location = new System.Drawing.Point(294, 228);
            removeAllButton.Name = "removeAllButton";
            removeAllButton.Size = new System.Drawing.Size(32, 23);
            removeAllButton.TabIndex = 3;
            removeAllButton.Text = "<<";
            removeAllButton.UseVisualStyleBackColor = true;
            removeAllButton.Click += RemoveAllButton_Click;
            // 
            // pasteButton
            // 
            pasteButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            pasteButton.Location = new System.Drawing.Point(405, 415);
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new System.Drawing.Size(68, 23);
            pasteButton.TabIndex = 5;
            pasteButton.Text = "&Paste";
            pasteButton.UseVisualStyleBackColor = true;
            pasteButton.Click += PasteButton_Click;
            // 
            // copyButton
            // 
            copyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            copyButton.Location = new System.Drawing.Point(331, 415);
            copyButton.Name = "copyButton";
            copyButton.Size = new System.Drawing.Size(68, 23);
            copyButton.TabIndex = 5;
            copyButton.Text = "&Copy";
            copyButton.UseVisualStyleBackColor = true;
            copyButton.Click += CopyButton_Click;
            // 
            // DBObjectsSelectForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(633, 450);
            Controls.Add(cancelButton);
            Controls.Add(copyButton);
            Controls.Add(pasteButton);
            Controls.Add(okButton);
            Controls.Add(selectedListBox);
            Controls.Add(selectableListBox);
            Controls.Add(removeAllButton);
            Controls.Add(removeButton);
            Controls.Add(addButton);
            Controls.Add(addAllButton);
            Controls.Add(clearButton);
            Controls.Add(filterTextBox);
            Controls.Add(schemaComboBox);
            Controls.Add(objectTypeComboBox);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DBObjectsSelectForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Database Objects Selector";
            Load += DBObjectsSelectForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox objectTypeComboBox;
        private System.Windows.Forms.ComboBox schemaComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox selectableListBox;
        private System.Windows.Forms.Button addAllButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox selectedListBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button removeAllButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
    }
}