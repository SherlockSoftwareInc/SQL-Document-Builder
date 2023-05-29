namespace SQL_Document_Builder
{
    partial class DBObjectDefPanel
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
            columnView = new ColumnDefView();
            titleLabel = new System.Windows.Forms.Label();
            descTextBox = new System.Windows.Forms.TextBox();
            columnNameLabel = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // columnView
            // 
            columnView.ConnectionString = "";
            columnView.Dock = System.Windows.Forms.DockStyle.Fill;
            columnView.Location = new System.Drawing.Point(0, 0);
            columnView.Name = "columnView";
            columnView.Size = new System.Drawing.Size(608, 574);
            columnView.TabIndex = 6;
            columnView.TableDescription = "";
            columnView.SelectedColumnChanged += ColumnDefView1_SelectedColumnChanged;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Location = new System.Drawing.Point(5, 5);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(53, 15);
            titleLabel.TabIndex = 3;
            titleLabel.Text = "Column:";
            // 
            // descTextBox
            // 
            descTextBox.Location = new System.Drawing.Point(6, 38);
            descTextBox.Multiline = true;
            descTextBox.Name = "descTextBox";
            descTextBox.Size = new System.Drawing.Size(596, 37);
            descTextBox.TabIndex = 4;
            descTextBox.TextChanged += DescTextBox_TextChanged;
            descTextBox.Validated += DescTextBox_Validated;
            // 
            // columnNameLabel
            // 
            columnNameLabel.AutoSize = true;
            columnNameLabel.Location = new System.Drawing.Point(64, 5);
            columnNameLabel.Name = "columnNameLabel";
            columnNameLabel.Size = new System.Drawing.Size(0, 15);
            columnNameLabel.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(5, 20);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(70, 15);
            label3.TabIndex = 3;
            label3.Text = "Description:";
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel1.Controls.Add(titleLabel);
            panel1.Controls.Add(descTextBox);
            panel1.Controls.Add(columnNameLabel);
            panel1.Controls.Add(label3);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 574);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(608, 84);
            panel1.TabIndex = 7;
            // 
            // DBObjectDefPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(columnView);
            Controls.Add(panel1);
            Name = "DBObjectDefPanel";
            Size = new System.Drawing.Size(608, 658);
            SizeChanged += DBObjectDefPanel_SizeChanged;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ColumnDefView columnView;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Label columnNameLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
    }
}
