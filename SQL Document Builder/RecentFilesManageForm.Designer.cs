namespace SQL_Document_Builder
{
    partial class RecentFilesManageForm
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
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            searchToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            openToolStripButton = new System.Windows.Forms.ToolStripButton();
            moveFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            deleteFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            filesDataGridView = new System.Windows.Forms.DataGridView();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)filesDataGridView).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripLabel1, searchToolStripTextBox, openToolStripButton, moveFileToolStripButton, deleteFileToolStripButton, toolStripSeparator1, closeToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(63, 22);
            toolStripLabel1.Text = "Search for:";
            // 
            // searchToolStripTextBox
            // 
            searchToolStripTextBox.Name = "searchToolStripTextBox";
            searchToolStripTextBox.Size = new System.Drawing.Size(300, 25);
            searchToolStripTextBox.TextChanged += SearchToolStripTextBox_TextChanged;
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = Properties.Resources.openfile24;
            openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new System.Drawing.Size(23, 22);
            openToolStripButton.Text = "Open";
            openToolStripButton.Click += OpenToolStripButton_Click;
            // 
            // moveFileToolStripButton
            // 
            moveFileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            moveFileToolStripButton.Image = Properties.Resources.move_file;
            moveFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            moveFileToolStripButton.Name = "moveFileToolStripButton";
            moveFileToolStripButton.Size = new System.Drawing.Size(23, 22);
            moveFileToolStripButton.Text = "Move";
            moveFileToolStripButton.Click += MoveFileToolStripButton_Click;
            // 
            // deleteFileToolStripButton
            // 
            deleteFileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            deleteFileToolStripButton.Image = Properties.Resources.delete_icon;
            deleteFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            deleteFileToolStripButton.Name = "deleteFileToolStripButton";
            deleteFileToolStripButton.Size = new System.Drawing.Size(23, 22);
            deleteFileToolStripButton.Text = "Delete";
            deleteFileToolStripButton.Click += DeleteFileToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // closeToolStripButton
            // 
            closeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            closeToolStripButton.Image = Properties.Resources.icon_exit;
            closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            closeToolStripButton.Name = "closeToolStripButton";
            closeToolStripButton.Size = new System.Drawing.Size(23, 22);
            closeToolStripButton.Text = "Close";
            closeToolStripButton.Click += CloseToolStripButton_Click;
            // 
            // filesDataGridView
            // 
            filesDataGridView.AllowUserToAddRows = false;
            filesDataGridView.AllowUserToDeleteRows = false;
            filesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            filesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            filesDataGridView.Location = new System.Drawing.Point(0, 25);
            filesDataGridView.Name = "filesDataGridView";
            filesDataGridView.ReadOnly = true;
            filesDataGridView.Size = new System.Drawing.Size(800, 425);
            filesDataGridView.TabIndex = 1;
            filesDataGridView.CellDoubleClick += FilesDataGridView_CellDoubleClick;
            // 
            // RecentFilesManageForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(filesDataGridView);
            Controls.Add(toolStrip1);
            Name = "RecentFilesManageForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Recent Files Manage Form";
            Load += RecentFilesManageForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)filesDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox searchToolStripTextBox;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton moveFileToolStripButton;
        private System.Windows.Forms.ToolStripButton deleteFileToolStripButton;
        private System.Windows.Forms.DataGridView filesDataGridView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton closeToolStripButton;
    }
}