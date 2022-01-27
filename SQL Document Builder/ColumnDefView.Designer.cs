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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tablenameLabel = new System.Windows.Forms.Label();
            this.tableLabel = new System.Windows.Forms.Label();
            this.columnDefDataGridView = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnDefDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Controls.Add(this.tablenameLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(529, 30);
            this.panel1.TabIndex = 0;
            // 
            // tablenameLabel
            // 
            this.tablenameLabel.AutoSize = true;
            this.tablenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tablenameLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.tablenameLabel.Location = new System.Drawing.Point(6, 6);
            this.tablenameLabel.Name = "tablenameLabel";
            this.tablenameLabel.Size = new System.Drawing.Size(0, 18);
            this.tablenameLabel.TabIndex = 0;
            // 
            // tableLabel
            // 
            this.tableLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLabel.Location = new System.Drawing.Point(0, 590);
            this.tableLabel.Name = "tableLabel";
            this.tableLabel.Size = new System.Drawing.Size(529, 36);
            this.tableLabel.TabIndex = 1;
            // 
            // columnDefDataGridView
            // 
            this.columnDefDataGridView.AllowUserToAddRows = false;
            this.columnDefDataGridView.AllowUserToDeleteRows = false;
            this.columnDefDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnDefDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnDefDataGridView.Location = new System.Drawing.Point(0, 30);
            this.columnDefDataGridView.Name = "columnDefDataGridView";
            this.columnDefDataGridView.ReadOnly = true;
            this.columnDefDataGridView.RowTemplate.Height = 25;
            this.columnDefDataGridView.Size = new System.Drawing.Size(529, 560);
            this.columnDefDataGridView.TabIndex = 2;
            this.columnDefDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ColumnDefDataGridView_CellClick);
            // 
            // ColumnDefView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.columnDefDataGridView);
            this.Controls.Add(this.tableLabel);
            this.Controls.Add(this.panel1);
            this.Name = "ColumnDefView";
            this.Size = new System.Drawing.Size(529, 626);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnDefDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label tableLabel;
        private System.Windows.Forms.DataGridView columnDefDataGridView;
        private System.Windows.Forms.Label tablenameLabel;
    }
}
