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
            panel1 = new System.Windows.Forms.Panel();
            tablenameLabel = new System.Windows.Forms.Label();
            tableLabel = new System.Windows.Forms.TextBox();
            columnDefDataGridView = new System.Windows.Forms.DataGridView();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)columnDefDataGridView).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            panel1.Controls.Add(tablenameLabel);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(529, 30);
            panel1.TabIndex = 0;
            // 
            // tablenameLabel
            // 
            tablenameLabel.AutoSize = true;
            tablenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tablenameLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            tablenameLabel.Location = new System.Drawing.Point(6, 6);
            tablenameLabel.Name = "tablenameLabel";
            tablenameLabel.Size = new System.Drawing.Size(0, 18);
            tablenameLabel.TabIndex = 0;
            // 
            // tableLabel
            // 
            tableLabel.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            tableLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            tableLabel.Location = new System.Drawing.Point(0, 554);
            tableLabel.Multiline = true;
            tableLabel.Name = "tableLabel";
            tableLabel.Size = new System.Drawing.Size(529, 72);
            tableLabel.TabIndex = 1;
            tableLabel.Click += TableLabel_Click;
            tableLabel.Validated += TableLabel_Validated;
            // 
            // columnDefDataGridView
            // 
            columnDefDataGridView.AllowUserToAddRows = false;
            columnDefDataGridView.AllowUserToDeleteRows = false;
            columnDefDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            columnDefDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            columnDefDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            columnDefDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            columnDefDataGridView.Location = new System.Drawing.Point(0, 30);
            columnDefDataGridView.Name = "columnDefDataGridView";
            columnDefDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            columnDefDataGridView.Size = new System.Drawing.Size(529, 524);
            columnDefDataGridView.TabIndex = 1;
            columnDefDataGridView.CellClick += ColumnDefDataGridView_CellClick;
            columnDefDataGridView.CellDoubleClick += ColumnDefDataGridView_CellDoubleClick;
            columnDefDataGridView.CellValidated += ColumnDefDataGridView_CellValidated;
            // 
            // ColumnDefView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(columnDefDataGridView);
            Controls.Add(tableLabel);
            Controls.Add(panel1);
            Name = "ColumnDefView";
            Size = new System.Drawing.Size(529, 626);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)columnDefDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tableLabel;
        private System.Windows.Forms.DataGridView columnDefDataGridView;
        private System.Windows.Forms.Label tablenameLabel;
    }
}
