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
            components = new System.ComponentModel.Container();
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
            removeButton = new System.Windows.Forms.Button();
            removeAllButton = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copySelectedObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteSelectedObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            recentObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            okToolStripButton = new System.Windows.Forms.ToolStripButton();
            cancelToolStripButton = new System.Windows.Forms.ToolStripButton();
            oKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            selectCurrentObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            removeCurrentObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 61);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(73, 15);
            label1.TabIndex = 0;
            label1.Text = "Object Type:";
            // 
            // objectTypeComboBox
            // 
            objectTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            objectTypeComboBox.FormattingEnabled = true;
            objectTypeComboBox.Items.AddRange(new object[] { "Table", "View", "Stored Procedure", "Function", "Trigger", "Synonym", "(All)" });
            objectTypeComboBox.Location = new System.Drawing.Point(105, 58);
            objectTypeComboBox.Name = "objectTypeComboBox";
            objectTypeComboBox.Size = new System.Drawing.Size(183, 23);
            objectTypeComboBox.TabIndex = 1;
            toolTip1.SetToolTip(objectTypeComboBox, "Select a object type");
            objectTypeComboBox.SelectedIndexChanged += ObjectTypeComboBox_SelectedIndexChanged;
            // 
            // schemaComboBox
            // 
            schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Location = new System.Drawing.Point(105, 87);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(183, 23);
            schemaComboBox.TabIndex = 3;
            toolTip1.SetToolTip(schemaComboBox, "Select schema");
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 90);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(52, 15);
            label2.TabIndex = 2;
            label2.Text = "Schema:";
            // 
            // filterTextBox
            // 
            filterTextBox.Location = new System.Drawing.Point(105, 116);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new System.Drawing.Size(160, 23);
            filterTextBox.TabIndex = 5;
            toolTip1.SetToolTip(filterTextBox, "Filter the selectable objects by");
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 119);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(36, 15);
            label3.TabIndex = 4;
            label3.Text = "Filter:";
            // 
            // clearButton
            // 
            clearButton.Location = new System.Drawing.Point(265, 116);
            clearButton.Name = "clearButton";
            clearButton.Size = new System.Drawing.Size(23, 23);
            clearButton.TabIndex = 6;
            clearButton.Text = "X";
            toolTip1.SetToolTip(clearButton, "Clear the filter");
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += ClearButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 151);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(104, 15);
            label4.TabIndex = 7;
            label4.Text = "Selectable objects:";
            // 
            // selectableListBox
            // 
            selectableListBox.FormattingEnabled = true;
            selectableListBox.IntegralHeight = false;
            selectableListBox.ItemHeight = 15;
            selectableListBox.Location = new System.Drawing.Point(12, 169);
            selectableListBox.Name = "selectableListBox";
            selectableListBox.Size = new System.Drawing.Size(276, 286);
            selectableListBox.TabIndex = 8;
            toolTip1.SetToolTip(selectableListBox, "Selectable object list");
            selectableListBox.DoubleClick += SelectableListBox_DoubleClick;
            // 
            // addAllButton
            // 
            addAllButton.Location = new System.Drawing.Point(294, 187);
            addAllButton.Name = "addAllButton";
            addAllButton.Size = new System.Drawing.Size(44, 23);
            addAllButton.TabIndex = 11;
            addAllButton.Text = ">>";
            toolTip1.SetToolTip(addAllButton, "Add all to selected list");
            addAllButton.UseVisualStyleBackColor = true;
            addAllButton.Click += AddAllButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(344, 55);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(95, 15);
            label5.TabIndex = 9;
            label5.Text = "Selected objects:";
            // 
            // selectedListBox
            // 
            selectedListBox.FormattingEnabled = true;
            selectedListBox.IntegralHeight = false;
            selectedListBox.ItemHeight = 15;
            selectedListBox.Location = new System.Drawing.Point(344, 76);
            selectedListBox.Name = "selectedListBox";
            selectedListBox.Size = new System.Drawing.Size(289, 379);
            selectedListBox.TabIndex = 10;
            toolTip1.SetToolTip(selectedListBox, "Selected object list");
            selectedListBox.DoubleClick += SelectedListBox_DoubleClick;
            // 
            // addButton
            // 
            addButton.Location = new System.Drawing.Point(294, 216);
            addButton.Name = "addButton";
            addButton.Size = new System.Drawing.Size(44, 23);
            addButton.TabIndex = 12;
            addButton.Text = ">";
            toolTip1.SetToolTip(addButton, "Add to selected list");
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;
            // 
            // removeButton
            // 
            removeButton.Location = new System.Drawing.Point(294, 245);
            removeButton.Name = "removeButton";
            removeButton.Size = new System.Drawing.Size(44, 23);
            removeButton.TabIndex = 13;
            removeButton.Text = "<";
            toolTip1.SetToolTip(removeButton, "Remove from selected list");
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += RemoveButton_Click;
            // 
            // removeAllButton
            // 
            removeAllButton.Location = new System.Drawing.Point(294, 274);
            removeAllButton.Name = "removeAllButton";
            removeAllButton.Size = new System.Drawing.Size(44, 23);
            removeAllButton.TabIndex = 14;
            removeAllButton.Text = "<<";
            toolTip1.SetToolTip(removeAllButton, "Remove all selected objects");
            removeAllButton.UseVisualStyleBackColor = true;
            removeAllButton.Click += RemoveAllButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(644, 24);
            menuStrip1.TabIndex = 19;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { oKToolStripMenuItem, cancelToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { copySelectedObjectsToolStripMenuItem, pasteSelectedObjectsToolStripMenuItem, recentObjectsToolStripMenuItem, toolStripSeparator1, selectAllToolStripMenuItem, removeAllToolStripMenuItem, selectCurrentObjectToolStripMenuItem, removeCurrentObjectToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // copySelectedObjectsToolStripMenuItem
            // 
            copySelectedObjectsToolStripMenuItem.Name = "copySelectedObjectsToolStripMenuItem";
            copySelectedObjectsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            copySelectedObjectsToolStripMenuItem.Text = "Copy Selected Objects";
            copySelectedObjectsToolStripMenuItem.Click += CopyButton_Click;
            // 
            // pasteSelectedObjectsToolStripMenuItem
            // 
            pasteSelectedObjectsToolStripMenuItem.Name = "pasteSelectedObjectsToolStripMenuItem";
            pasteSelectedObjectsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            pasteSelectedObjectsToolStripMenuItem.Text = "Paste Selected Objects";
            pasteSelectedObjectsToolStripMenuItem.Click += PasteButton_Click;
            // 
            // recentObjectsToolStripMenuItem
            // 
            recentObjectsToolStripMenuItem.Name = "recentObjectsToolStripMenuItem";
            recentObjectsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            recentObjectsToolStripMenuItem.Text = "Recent Objects";
            recentObjectsToolStripMenuItem.Click += RecentObjectsToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { okToolStripButton, cancelToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(644, 25);
            toolStrip1.TabIndex = 20;
            toolStrip1.Text = "toolStrip1";
            // 
            // okToolStripButton
            // 
            okToolStripButton.Image = Properties.Resources.checkmark;
            okToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            okToolStripButton.Name = "okToolStripButton";
            okToolStripButton.Size = new System.Drawing.Size(43, 22);
            okToolStripButton.Text = "&OK";
            okToolStripButton.Click += OkButton_Click;
            // 
            // cancelToolStripButton
            // 
            cancelToolStripButton.Image = Properties.Resources.cancel_icon_16;
            cancelToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            cancelToolStripButton.Name = "cancelToolStripButton";
            cancelToolStripButton.Size = new System.Drawing.Size(63, 22);
            cancelToolStripButton.Text = "&Cancel";
            cancelToolStripButton.Click += CancelButton_Click;
            // 
            // oKToolStripMenuItem
            // 
            oKToolStripMenuItem.Name = "oKToolStripMenuItem";
            oKToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            oKToolStripMenuItem.Text = "OK";
            oKToolStripMenuItem.Click += OkButton_Click;
            // 
            // cancelToolStripMenuItem
            // 
            cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            cancelToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            cancelToolStripMenuItem.Text = "Cancel";
            cancelToolStripMenuItem.Click += CancelButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(189, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            selectAllToolStripMenuItem.Text = "Select All";
            selectAllToolStripMenuItem.Click += AddAllButton_Click;
            // 
            // removeAllToolStripMenuItem
            // 
            removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            removeAllToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            removeAllToolStripMenuItem.Text = "Remove All";
            removeAllToolStripMenuItem.Click += RemoveAllButton_Click;
            // 
            // selectCurrentObjectToolStripMenuItem
            // 
            selectCurrentObjectToolStripMenuItem.Name = "selectCurrentObjectToolStripMenuItem";
            selectCurrentObjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            selectCurrentObjectToolStripMenuItem.Text = "Select Current Object";
            selectCurrentObjectToolStripMenuItem.Click += AddButton_Click;
            // 
            // removeCurrentObjectToolStripMenuItem
            // 
            removeCurrentObjectToolStripMenuItem.Name = "removeCurrentObjectToolStripMenuItem";
            removeCurrentObjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            removeCurrentObjectToolStripMenuItem.Text = "Remove Current Object";
            removeCurrentObjectToolStripMenuItem.Click += RemoveButton_Click;
            // 
            // DBObjectsSelectForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(644, 467);
            Controls.Add(toolStrip1);
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
            Controls.Add(menuStrip1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DBObjectsSelectForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Database Objects Selector";
            Load += DBObjectsSelectForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
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
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button removeAllButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton okToolStripButton;
        private System.Windows.Forms.ToolStripButton cancelToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem copySelectedObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteSelectedObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectCurrentObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeCurrentObjectToolStripMenuItem;
    }
}