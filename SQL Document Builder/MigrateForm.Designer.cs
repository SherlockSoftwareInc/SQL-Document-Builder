namespace SQL_Document_Builder
{
    partial class MigrateForm
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.wikiCheckBox = new System.Windows.Forms.CheckBox();
            this.postMigrateCheckBox = new System.Windows.Forms.CheckBox();
            this.importedCheckBox = new System.Windows.Forms.CheckBox();
            this.needMigrateCheckBox = new System.Windows.Forms.CheckBox();
            this.schemaComboBox = new System.Windows.Forms.ComboBox();
            this.tableTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.editWikiCheckBox = new System.Windows.Forms.CheckBox();
            this.editPostMigrateCheckBox = new System.Windows.Forms.CheckBox();
            this.editImportedCheckBox = new System.Windows.Forms.CheckBox();
            this.editMigrateCheckBox = new System.Windows.Forms.CheckBox();
            this.commentsTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.objectGroupBox = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.rowsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.objectGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(895, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::SQL_Document_Builder.Properties.Resources.save;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.editItem_DataChanged);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::SQL_Document_Builder.Properties.Resources.refresh;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.rowsToolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(895, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(802, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.wikiCheckBox);
            this.panel1.Controls.Add(this.postMigrateCheckBox);
            this.panel1.Controls.Add(this.importedCheckBox);
            this.panel1.Controls.Add(this.needMigrateCheckBox);
            this.panel1.Controls.Add(this.schemaComboBox);
            this.panel1.Controls.Add(this.tableTextBox);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(895, 49);
            this.panel1.TabIndex = 2;
            // 
            // wikiCheckBox
            // 
            this.wikiCheckBox.AutoSize = true;
            this.wikiCheckBox.Location = new System.Drawing.Point(525, 27);
            this.wikiCheckBox.Name = "wikiCheckBox";
            this.wikiCheckBox.Size = new System.Drawing.Size(15, 14);
            this.wikiCheckBox.TabIndex = 3;
            this.wikiCheckBox.UseVisualStyleBackColor = true;
            this.wikiCheckBox.CheckedChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // postMigrateCheckBox
            // 
            this.postMigrateCheckBox.AutoSize = true;
            this.postMigrateCheckBox.Location = new System.Drawing.Point(461, 27);
            this.postMigrateCheckBox.Name = "postMigrateCheckBox";
            this.postMigrateCheckBox.Size = new System.Drawing.Size(15, 14);
            this.postMigrateCheckBox.TabIndex = 3;
            this.postMigrateCheckBox.UseVisualStyleBackColor = true;
            this.postMigrateCheckBox.CheckedChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // importedCheckBox
            // 
            this.importedCheckBox.AutoSize = true;
            this.importedCheckBox.Location = new System.Drawing.Point(385, 27);
            this.importedCheckBox.Name = "importedCheckBox";
            this.importedCheckBox.Size = new System.Drawing.Size(15, 14);
            this.importedCheckBox.TabIndex = 3;
            this.importedCheckBox.UseVisualStyleBackColor = true;
            this.importedCheckBox.CheckedChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // needMigrateCheckBox
            // 
            this.needMigrateCheckBox.AutoSize = true;
            this.needMigrateCheckBox.Location = new System.Drawing.Point(313, 27);
            this.needMigrateCheckBox.Name = "needMigrateCheckBox";
            this.needMigrateCheckBox.Size = new System.Drawing.Size(15, 14);
            this.needMigrateCheckBox.TabIndex = 3;
            this.needMigrateCheckBox.UseVisualStyleBackColor = true;
            this.needMigrateCheckBox.CheckedChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // schemaComboBox
            // 
            this.schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.schemaComboBox.FormattingEnabled = true;
            this.schemaComboBox.Items.AddRange(new object[] {
            "(All)",
            "AF",
            "BCCR",
            "DIH"});
            this.schemaComboBox.Location = new System.Drawing.Point(48, 23);
            this.schemaComboBox.Name = "schemaComboBox";
            this.schemaComboBox.Size = new System.Drawing.Size(121, 23);
            this.schemaComboBox.TabIndex = 2;
            this.schemaComboBox.SelectedIndexChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // tableTextBox
            // 
            this.tableTextBox.Location = new System.Drawing.Point(175, 23);
            this.tableTextBox.Name = "tableTextBox";
            this.tableTextBox.Size = new System.Drawing.Size(100, 23);
            this.tableTextBox.TabIndex = 1;
            this.tableTextBox.TextChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(517, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "wiki:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(434, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Post migrate:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(369, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Imported:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(281, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Need migrate:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Schema:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter:";
            // 
            // editWikiCheckBox
            // 
            this.editWikiCheckBox.AutoSize = true;
            this.editWikiCheckBox.Location = new System.Drawing.Point(317, 22);
            this.editWikiCheckBox.Name = "editWikiCheckBox";
            this.editWikiCheckBox.Size = new System.Drawing.Size(47, 19);
            this.editWikiCheckBox.TabIndex = 3;
            this.editWikiCheckBox.Text = "wiki";
            this.editWikiCheckBox.UseVisualStyleBackColor = true;
            this.editWikiCheckBox.CheckedChanged += new System.EventHandler(this.editItem_DataChanged);
            // 
            // editPostMigrateCheckBox
            // 
            this.editPostMigrateCheckBox.AutoSize = true;
            this.editPostMigrateCheckBox.Location = new System.Drawing.Point(218, 22);
            this.editPostMigrateCheckBox.Name = "editPostMigrateCheckBox";
            this.editPostMigrateCheckBox.Size = new System.Drawing.Size(93, 19);
            this.editPostMigrateCheckBox.TabIndex = 3;
            this.editPostMigrateCheckBox.Text = "Post migrate";
            this.editPostMigrateCheckBox.UseVisualStyleBackColor = true;
            this.editPostMigrateCheckBox.CheckedChanged += new System.EventHandler(this.editItem_DataChanged);
            // 
            // editImportedCheckBox
            // 
            this.editImportedCheckBox.AutoSize = true;
            this.editImportedCheckBox.Location = new System.Drawing.Point(128, 22);
            this.editImportedCheckBox.Name = "editImportedCheckBox";
            this.editImportedCheckBox.Size = new System.Drawing.Size(75, 19);
            this.editImportedCheckBox.TabIndex = 3;
            this.editImportedCheckBox.Text = "Imported";
            this.editImportedCheckBox.UseVisualStyleBackColor = true;
            this.editImportedCheckBox.CheckedChanged += new System.EventHandler(this.editItem_DataChanged);
            // 
            // editMigrateCheckBox
            // 
            this.editMigrateCheckBox.AutoSize = true;
            this.editMigrateCheckBox.Location = new System.Drawing.Point(12, 22);
            this.editMigrateCheckBox.Name = "editMigrateCheckBox";
            this.editMigrateCheckBox.Size = new System.Drawing.Size(98, 19);
            this.editMigrateCheckBox.TabIndex = 3;
            this.editMigrateCheckBox.Text = "Need migrate";
            this.editMigrateCheckBox.UseVisualStyleBackColor = true;
            this.editMigrateCheckBox.CheckedChanged += new System.EventHandler(this.editItem_DataChanged);
            // 
            // commentsTextBox
            // 
            this.commentsTextBox.Location = new System.Drawing.Point(87, 41);
            this.commentsTextBox.MaxLength = 100;
            this.commentsTextBox.Name = "commentsTextBox";
            this.commentsTextBox.Size = new System.Drawing.Size(701, 23);
            this.commentsTextBox.TabIndex = 1;
            this.commentsTextBox.Validated += new System.EventHandler(this.editItem_DataChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "Comments:";
            // 
            // objectGroupBox
            // 
            this.objectGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.objectGroupBox.Controls.Add(this.commentsTextBox);
            this.objectGroupBox.Controls.Add(this.editWikiCheckBox);
            this.objectGroupBox.Controls.Add(this.editMigrateCheckBox);
            this.objectGroupBox.Controls.Add(this.label12);
            this.objectGroupBox.Controls.Add(this.editPostMigrateCheckBox);
            this.objectGroupBox.Controls.Add(this.editImportedCheckBox);
            this.objectGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.objectGroupBox.Location = new System.Drawing.Point(0, 357);
            this.objectGroupBox.Name = "objectGroupBox";
            this.objectGroupBox.Size = new System.Drawing.Size(895, 71);
            this.objectGroupBox.TabIndex = 4;
            this.objectGroupBox.TabStop = false;
            this.objectGroupBox.Text = "object name";
            this.objectGroupBox.Resize += new System.EventHandler(this.GroupBox1_Resize);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 74);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(895, 283);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // rowsToolStripStatusLabel
            // 
            this.rowsToolStripStatusLabel.Name = "rowsToolStripStatusLabel";
            this.rowsToolStripStatusLabel.Size = new System.Drawing.Size(47, 17);
            this.rowsToolStripStatusLabel.Text = "Rows: 0";
            // 
            // MigrateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 450);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.objectGroupBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MigrateForm";
            this.Text = "MigrateForm";
            this.Load += new System.EventHandler(this.MigrateForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.objectGroupBox.ResumeLayout(false);
            this.objectGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox wikiCheckBox;
        private System.Windows.Forms.CheckBox postMigrateCheckBox;
        private System.Windows.Forms.CheckBox importedCheckBox;
        private System.Windows.Forms.CheckBox needMigrateCheckBox;
        private System.Windows.Forms.ComboBox schemaComboBox;
        private System.Windows.Forms.TextBox tableTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox editWikiCheckBox;
        private System.Windows.Forms.CheckBox editPostMigrateCheckBox;
        private System.Windows.Forms.CheckBox editImportedCheckBox;
        private System.Windows.Forms.CheckBox editMigrateCheckBox;
        private System.Windows.Forms.TextBox commentsTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox objectGroupBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripStatusLabel rowsToolStripStatusLabel;
    }
}