using SQL_Document_Builder.Properties;

namespace SQL_Document_Builder
{
    partial class ColumnDefView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            namePanel = new ObjectNamePanel();
            openButton = new System.Windows.Forms.Button();
            openTableContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tableDescTextBox = new System.Windows.Forms.TextBox();
            tableContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            addDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            columnDefDataGridView = new System.Windows.Forms.DataGridView();
            columnContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            columnValueFrequencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            namePanel.SuspendLayout();
            openTableContextMenuStrip.SuspendLayout();
            tableContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)columnDefDataGridView).BeginInit();
            columnContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // namePanel
            // 
            namePanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            namePanel.Controls.Add(openButton);
            namePanel.Dock = System.Windows.Forms.DockStyle.Top;
            namePanel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            namePanel.Location = new System.Drawing.Point(0, 0);
            namePanel.Name = "namePanel";
            namePanel.Size = new System.Drawing.Size(529, 30);
            namePanel.TabIndex = 0;
            // 
            // openButton
            // 
            openButton.Dock = System.Windows.Forms.DockStyle.Right;
            openButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            openButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            openButton.Location = new System.Drawing.Point(499, 0);
            openButton.Name = "openButton";
            openButton.Padding = new System.Windows.Forms.Padding(3);
            openButton.Size = new System.Drawing.Size(30, 30);
            openButton.TabIndex = 1;
            openButton.Text = "📂";
            openButton.UseVisualStyleBackColor = true;
            openButton.Click += OpenToolStripMenuItem_Click;
            // 
            // openTableContextMenuStrip
            // 
            openTableContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem });
            openTableContextMenuStrip.Name = "openTableContextMenuStrip";
            openTableContextMenuStrip.Size = new System.Drawing.Size(104, 26);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // tableDescTextBox
            // 
            tableDescTextBox.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            tableDescTextBox.ContextMenuStrip = tableContextMenuStrip;
            tableDescTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            tableDescTextBox.Location = new System.Drawing.Point(0, 554);
            tableDescTextBox.Multiline = true;
            tableDescTextBox.Name = "tableDescTextBox";
            tableDescTextBox.Size = new System.Drawing.Size(529, 72);
            tableDescTextBox.TabIndex = 1;
            tableDescTextBox.Click += TableLabel_Click;
            tableDescTextBox.Validated += TableLabel_Validated;
            // 
            // tableContextMenuStrip
            // 
            tableContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator1, addDescriptionToolStripMenuItem });
            tableContextMenuStrip.Name = "tableContextMenuStrip";
            tableContextMenuStrip.Size = new System.Drawing.Size(159, 76);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            pasteToolStripMenuItem.Text = "Paste";
            pasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // addDescriptionToolStripMenuItem
            // 
            addDescriptionToolStripMenuItem.Name = "addDescriptionToolStripMenuItem";
            addDescriptionToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            addDescriptionToolStripMenuItem.Text = "Add description";
            addDescriptionToolStripMenuItem.Click += AddDescriptionToolStripMenuItem_Click;
            // 
            // columnDefDataGridView
            // 
            columnDefDataGridView.AllowUserToAddRows = false;
            columnDefDataGridView.AllowUserToDeleteRows = false;
            columnDefDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            columnDefDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            columnDefDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            columnDefDataGridView.ContextMenuStrip = columnContextMenuStrip;
            columnDefDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            columnDefDataGridView.Location = new System.Drawing.Point(0, 30);
            columnDefDataGridView.Name = "columnDefDataGridView";
            columnDefDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            columnDefDataGridView.Size = new System.Drawing.Size(529, 524);
            columnDefDataGridView.TabIndex = 1;
            columnDefDataGridView.CellClick += ColumnDefDataGridView_CellClick;
            columnDefDataGridView.CellValidated += ColumnDefDataGridView_CellValidated;
            // 
            // columnContextMenuStrip
            // 
            columnContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { columnValueFrequencyToolStripMenuItem });
            columnContextMenuStrip.Name = "columnContextMenuStrip";
            columnContextMenuStrip.Size = new System.Drawing.Size(207, 26);
            // 
            // columnValueFrequencyToolStripMenuItem
            // 
            columnValueFrequencyToolStripMenuItem.Name = "columnValueFrequencyToolStripMenuItem";
            columnValueFrequencyToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            columnValueFrequencyToolStripMenuItem.Text = "Column Value Frequency";
            columnValueFrequencyToolStripMenuItem.Click += ColumnValueFrequencyToolStripMenuItem_Click;
            // 
            // ColumnDefView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(columnDefDataGridView);
            Controls.Add(tableDescTextBox);
            Controls.Add(namePanel);
            Name = "ColumnDefView";
            Size = new System.Drawing.Size(529, 626);
            namePanel.ResumeLayout(false);
            openTableContextMenuStrip.ResumeLayout(false);
            tableContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)columnDefDataGridView).EndInit();
            columnContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ObjectNamePanel namePanel;
        private System.Windows.Forms.TextBox tableDescTextBox;
        private System.Windows.Forms.DataGridView columnDefDataGridView;
        private System.Windows.Forms.ContextMenuStrip columnContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem columnValueFrequencyToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip tableContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addDescriptionToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip openTableContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Button openButton;
    }
}
