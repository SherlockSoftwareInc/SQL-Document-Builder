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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBObjectsSelectForm));
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
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            okToolStripButton = new System.Windows.Forms.ToolStripButton();
            cancelToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 39);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(73, 15);
            label1.TabIndex = 0;
            label1.Text = "Object Type:";
            // 
            // objectTypeComboBox
            // 
            objectTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            objectTypeComboBox.FormattingEnabled = true;
            objectTypeComboBox.Items.AddRange(new object[] { "Table", "View", "Stored Procedure", "Function", "Trigger", "Synonym" });
            objectTypeComboBox.Location = new System.Drawing.Point(105, 36);
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
            schemaComboBox.Location = new System.Drawing.Point(105, 65);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(183, 23);
            schemaComboBox.TabIndex = 3;
            toolTip1.SetToolTip(schemaComboBox, "Select schema");
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 68);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(52, 15);
            label2.TabIndex = 2;
            label2.Text = "Schema:";
            // 
            // filterTextBox
            // 
            filterTextBox.Location = new System.Drawing.Point(105, 94);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new System.Drawing.Size(160, 23);
            filterTextBox.TabIndex = 5;
            toolTip1.SetToolTip(filterTextBox, "Filter the selectable objects by");
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 97);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(36, 15);
            label3.TabIndex = 4;
            label3.Text = "Filter:";
            // 
            // clearButton
            // 
            clearButton.Location = new System.Drawing.Point(265, 94);
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
            label4.Location = new System.Drawing.Point(12, 129);
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
            selectableListBox.Location = new System.Drawing.Point(12, 147);
            selectableListBox.Name = "selectableListBox";
            selectableListBox.Size = new System.Drawing.Size(276, 286);
            selectableListBox.TabIndex = 8;
            toolTip1.SetToolTip(selectableListBox, "Selectable object list");
            selectableListBox.DoubleClick += SelectableListBox_DoubleClick;
            // 
            // addAllButton
            // 
            addAllButton.Location = new System.Drawing.Point(294, 165);
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
            label5.Location = new System.Drawing.Point(344, 33);
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
            selectedListBox.Location = new System.Drawing.Point(344, 54);
            selectedListBox.Name = "selectedListBox";
            selectedListBox.Size = new System.Drawing.Size(289, 379);
            selectedListBox.TabIndex = 10;
            toolTip1.SetToolTip(selectedListBox, "Selected object list");
            selectedListBox.DoubleClick += SelectedListBox_DoubleClick;
            // 
            // addButton
            // 
            addButton.Location = new System.Drawing.Point(294, 194);
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
            removeButton.Location = new System.Drawing.Point(294, 223);
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
            removeAllButton.Location = new System.Drawing.Point(294, 252);
            removeAllButton.Name = "removeAllButton";
            removeAllButton.Size = new System.Drawing.Size(44, 23);
            removeAllButton.TabIndex = 14;
            removeAllButton.Text = "<<";
            toolTip1.SetToolTip(removeAllButton, "Remove all selected objects");
            removeAllButton.UseVisualStyleBackColor = true;
            removeAllButton.Click += RemoveAllButton_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { okToolStripButton, cancelToolStripButton, toolStripSeparator2, copyToolStripButton, pasteToolStripButton, toolStripButton3 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
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
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // copyToolStripButton
            // 
            copyToolStripButton.Image = (System.Drawing.Image)resources.GetObject("copyToolStripButton.Image");
            copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Size = new System.Drawing.Size(55, 22);
            copyToolStripButton.Text = "Copy";
            copyToolStripButton.ToolTipText = "Copy selected objects";
            copyToolStripButton.Click += CopyButton_Click;
            // 
            // pasteToolStripButton
            // 
            pasteToolStripButton.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripButton.Image");
            pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Size = new System.Drawing.Size(55, 22);
            pasteToolStripButton.Text = "Paste";
            pasteToolStripButton.ToolTipText = "Paste selected objects from clipboard";
            pasteToolStripButton.Click += PasteButton_Click;
            // 
            // toolStripButton3
            // 
            toolStripButton3.Image = Properties.Resources.recen24t;
            toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new System.Drawing.Size(106, 22);
            toolStripButton3.Text = "Recent Objects";
            toolStripButton3.ToolTipText = "Gets Recent Added or Modified Objects";
            toolStripButton3.Click += RecentObjectsToolStripMenuItem_Click;
            // 
            // DBObjectsSelectForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(644, 442);
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
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DBObjectsSelectForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Database Objects Selector";
            Load += DBObjectsSelectForm_Load;
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton okToolStripButton;
        private System.Windows.Forms.ToolStripButton cancelToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}