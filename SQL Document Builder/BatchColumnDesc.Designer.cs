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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.searchToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tablesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.selectAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.unselectAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.searchToolStripTextBox,
            this.searchToolStripButton,
            this.toolStripSeparator1,
            this.selectAllToolStripButton,
            this.unselectAllToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(122, 22);
            this.toolStripLabel1.Text = "Search column name:";
            // 
            // searchToolStripTextBox
            // 
            this.searchToolStripTextBox.Name = "searchToolStripTextBox";
            this.searchToolStripTextBox.Size = new System.Drawing.Size(200, 25);
            this.searchToolStripTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchToolStripTextBox_KeyUp);
            // 
            // searchToolStripButton
            // 
            this.searchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("searchToolStripButton.Image")));
            this.searchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchToolStripButton.Name = "searchToolStripButton";
            this.searchToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.searchToolStripButton.Text = "toolStripButton1";
            this.searchToolStripButton.Click += new System.EventHandler(this.SearchToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tablesCheckedListBox
            // 
            this.tablesCheckedListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.tablesCheckedListBox.FormattingEnabled = true;
            this.tablesCheckedListBox.Location = new System.Drawing.Point(0, 25);
            this.tablesCheckedListBox.Name = "tablesCheckedListBox";
            this.tablesCheckedListBox.Size = new System.Drawing.Size(217, 425);
            this.tablesCheckedListBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(223, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Column description:";
            // 
            // descTextBox
            // 
            this.descTextBox.Location = new System.Drawing.Point(223, 43);
            this.descTextBox.Name = "descTextBox";
            this.descTextBox.Size = new System.Drawing.Size(565, 23);
            this.descTextBox.TabIndex = 3;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(223, 72);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // selectAllToolStripButton
            // 
            this.selectAllToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("selectAllToolStripButton.Image")));
            this.selectAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectAllToolStripButton.Name = "selectAllToolStripButton";
            this.selectAllToolStripButton.Size = new System.Drawing.Size(73, 22);
            this.selectAllToolStripButton.Text = "Select all";
            this.selectAllToolStripButton.Click += new System.EventHandler(this.SelectAllToolStripButton_Click);
            // 
            // unselectAllToolStripButton
            // 
            this.unselectAllToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("unselectAllToolStripButton.Image")));
            this.unselectAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.unselectAllToolStripButton.Name = "unselectAllToolStripButton";
            this.unselectAllToolStripButton.Size = new System.Drawing.Size(87, 22);
            this.unselectAllToolStripButton.Text = "Unselect all";
            this.unselectAllToolStripButton.Click += new System.EventHandler(this.UnselectAllToolStripButton_Click);
            // 
            // BatchColumnDesc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.descTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tablesCheckedListBox);
            this.Controls.Add(this.toolStrip1);
            this.Name = "BatchColumnDesc";
            this.Text = "BatchColumnDesc";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox searchToolStripTextBox;
        private System.Windows.Forms.ToolStripButton searchToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckedListBox tablesCheckedListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.ToolStripButton selectAllToolStripButton;
        private System.Windows.Forms.ToolStripButton unselectAllToolStripButton;
    }
}