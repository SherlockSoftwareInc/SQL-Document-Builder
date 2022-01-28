namespace SQL_Document_Builder
{
    partial class DescEditForm
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
            this.columnView = new SQL_Document_Builder.ColumnDefView();
            this.titleLabel = new System.Windows.Forms.Label();
            this.columnNameLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnView
            // 
            this.columnView.ConnectionString = "";
            this.columnView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnView.Location = new System.Drawing.Point(0, 0);
            this.columnView.Name = "columnView";
            this.columnView.Size = new System.Drawing.Size(718, 483);
            this.columnView.TabIndex = 2;
            this.columnView.TableDescription = "";
            this.columnView.SelectedColumnChanged += new System.EventHandler(this.ColumnDefView1_SelectedColumnChanged);
            this.columnView.TableDescSelected += new System.EventHandler(this.ColumnView_TableDescSelected);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(5, 5);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(53, 15);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Column:";
            // 
            // columnNameLabel
            // 
            this.columnNameLabel.AutoSize = true;
            this.columnNameLabel.Location = new System.Drawing.Point(64, 5);
            this.columnNameLabel.Name = "columnNameLabel";
            this.columnNameLabel.Size = new System.Drawing.Size(0, 15);
            this.columnNameLabel.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Description:";
            // 
            // descTextBox
            // 
            this.descTextBox.Location = new System.Drawing.Point(5, 38);
            this.descTextBox.Multiline = true;
            this.descTextBox.Name = "descTextBox";
            this.descTextBox.Size = new System.Drawing.Size(706, 37);
            this.descTextBox.TabIndex = 4;
            this.descTextBox.TextChanged += new System.EventHandler(this.DescTextBox_TextChanged);
            this.descTextBox.Validated += new System.EventHandler(this.DescTextBox_Validated);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.titleLabel);
            this.panel1.Controls.Add(this.descTextBox);
            this.panel1.Controls.Add(this.columnNameLabel);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 483);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 84);
            this.panel1.TabIndex = 5;
            // 
            // DescEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 567);
            this.Controls.Add(this.columnView);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DescEditForm";
            this.Text = "Description Edit Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DescEditForm_FormClosing);
            this.Load += new System.EventHandler(this.DescEditForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private ColumnDefView columnView;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label columnNameLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Panel panel1;
    }
}