namespace SQL_Document_Builder
{
    partial class BatchColumnDesc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchColumnDesc));
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            searchToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            searchToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            selectAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            unselectAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            objectsListBox = new System.Windows.Forms.CheckedListBox();
            label1 = new System.Windows.Forms.Label();
            descTextBox = new System.Windows.Forms.TextBox();
            applyButton = new System.Windows.Forms.Button();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            progressBar = new System.Windows.Forms.ToolStripProgressBar();
            messageToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            panel1 = new System.Windows.Forms.Panel();
            schemaComboBox = new System.Windows.Forms.ComboBox();
            schemaLabel = new System.Windows.Forms.Label();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripLabel1, searchToolStripTextBox, searchToolStripButton, toolStripSeparator1, selectAllToolStripButton, unselectAllToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(122, 22);
            toolStripLabel1.Text = "Search column name:";
            // 
            // searchToolStripTextBox
            // 
            searchToolStripTextBox.Name = "searchToolStripTextBox";
            searchToolStripTextBox.Size = new System.Drawing.Size(200, 25);
            searchToolStripTextBox.TextBoxTextAlignChanged += searchToolStripTextBox_TextBoxTextAlignChanged;
            searchToolStripTextBox.KeyUp += SearchToolStripTextBox_KeyUp;
            // 
            // searchToolStripButton
            // 
            searchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            searchToolStripButton.Image = Properties.Resources.search;
            searchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            searchToolStripButton.Name = "searchToolStripButton";
            searchToolStripButton.Size = new System.Drawing.Size(23, 22);
            searchToolStripButton.Text = "toolStripButton1";
            searchToolStripButton.Click += SearchToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // selectAllToolStripButton
            // 
            selectAllToolStripButton.Image = (System.Drawing.Image)resources.GetObject("selectAllToolStripButton.Image");
            selectAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            selectAllToolStripButton.Name = "selectAllToolStripButton";
            selectAllToolStripButton.Size = new System.Drawing.Size(73, 22);
            selectAllToolStripButton.Text = "Select all";
            selectAllToolStripButton.Click += SelectAllToolStripButton_Click;
            // 
            // unselectAllToolStripButton
            // 
            unselectAllToolStripButton.Image = (System.Drawing.Image)resources.GetObject("unselectAllToolStripButton.Image");
            unselectAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            unselectAllToolStripButton.Name = "unselectAllToolStripButton";
            unselectAllToolStripButton.Size = new System.Drawing.Size(87, 22);
            unselectAllToolStripButton.Text = "Unselect all";
            unselectAllToolStripButton.Click += UnselectAllToolStripButton_Click;
            // 
            // objectsListBox
            // 
            objectsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            objectsListBox.FormattingEnabled = true;
            objectsListBox.Location = new System.Drawing.Point(0, 38);
            objectsListBox.Name = "objectsListBox";
            objectsListBox.Size = new System.Drawing.Size(200, 365);
            objectsListBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(223, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(115, 15);
            label1.TabIndex = 2;
            label1.Text = "Column description:";
            // 
            // descTextBox
            // 
            descTextBox.Location = new System.Drawing.Point(223, 43);
            descTextBox.Name = "descTextBox";
            descTextBox.Size = new System.Drawing.Size(565, 23);
            descTextBox.TabIndex = 3;
            // 
            // applyButton
            // 
            applyButton.Location = new System.Drawing.Point(223, 72);
            applyButton.Name = "applyButton";
            applyButton.Size = new System.Drawing.Size(75, 23);
            applyButton.TabIndex = 4;
            applyButton.Text = "Apply";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += ApplyButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { progressBar, messageToolStripStatusLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(800, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(100, 16);
            progressBar.Visible = false;
            // 
            // messageToolStripStatusLabel
            // 
            messageToolStripStatusLabel.Name = "messageToolStripStatusLabel";
            messageToolStripStatusLabel.Size = new System.Drawing.Size(785, 17);
            messageToolStripStatusLabel.Spring = true;
            messageToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(objectsListBox);
            panel1.Controls.Add(schemaComboBox);
            panel1.Controls.Add(schemaLabel);
            panel1.Dock = System.Windows.Forms.DockStyle.Left;
            panel1.Location = new System.Drawing.Point(0, 25);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(200, 403);
            panel1.TabIndex = 6;
            // 
            // schemaComboBox
            // 
            schemaComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Location = new System.Drawing.Point(0, 15);
            schemaComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(200, 23);
            schemaComboBox.TabIndex = 9;
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // schemaLabel
            // 
            schemaLabel.AutoSize = true;
            schemaLabel.Dock = System.Windows.Forms.DockStyle.Top;
            schemaLabel.Location = new System.Drawing.Point(0, 0);
            schemaLabel.Name = "schemaLabel";
            schemaLabel.Size = new System.Drawing.Size(52, 15);
            schemaLabel.TabIndex = 8;
            schemaLabel.Text = "Schema:";
            // 
            // BatchColumnDesc
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(applyButton);
            Controls.Add(descTextBox);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Name = "BatchColumnDesc";
            Text = "BatchColumnDesc";
            Load += BatchColumnDesc_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox searchToolStripTextBox;
        private System.Windows.Forms.ToolStripButton searchToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckedListBox objectsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.ToolStripButton selectAllToolStripButton;
        private System.Windows.Forms.ToolStripButton unselectAllToolStripButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel messageToolStripStatusLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox schemaComboBox;
        private System.Windows.Forms.Label schemaLabel;
    }
}