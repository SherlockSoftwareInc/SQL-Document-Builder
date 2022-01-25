namespace SQL_Document_Builder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.storedProcedureButton = new System.Windows.Forms.Button();
            this.ViewButton = new System.Windows.Forms.Button();
            this.functionButton = new System.Windows.Forms.Button();
            this.tableButton = new System.Windows.Forms.Button();
            this.tableWikiButton = new System.Windows.Forms.Button();
            this.valueWikiButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // storedProcedureButton
            // 
            this.storedProcedureButton.Location = new System.Drawing.Point(12, 12);
            this.storedProcedureButton.Name = "storedProcedureButton";
            this.storedProcedureButton.Size = new System.Drawing.Size(75, 23);
            this.storedProcedureButton.TabIndex = 0;
            this.storedProcedureButton.Text = "SPs";
            this.storedProcedureButton.UseVisualStyleBackColor = true;
            this.storedProcedureButton.Click += new System.EventHandler(this.storedProcedureButton_Click);
            // 
            // ViewButton
            // 
            this.ViewButton.Location = new System.Drawing.Point(12, 41);
            this.ViewButton.Name = "ViewButton";
            this.ViewButton.Size = new System.Drawing.Size(75, 23);
            this.ViewButton.TabIndex = 0;
            this.ViewButton.Text = "Views";
            this.ViewButton.UseVisualStyleBackColor = true;
            this.ViewButton.Click += new System.EventHandler(this.ViewButton_Click);
            // 
            // functionButton
            // 
            this.functionButton.Location = new System.Drawing.Point(12, 70);
            this.functionButton.Name = "functionButton";
            this.functionButton.Size = new System.Drawing.Size(75, 23);
            this.functionButton.TabIndex = 0;
            this.functionButton.Text = "Functions";
            this.functionButton.UseVisualStyleBackColor = true;
            this.functionButton.Click += new System.EventHandler(this.functionButton_Click);
            // 
            // tableButton
            // 
            this.tableButton.Location = new System.Drawing.Point(12, 99);
            this.tableButton.Name = "tableButton";
            this.tableButton.Size = new System.Drawing.Size(75, 23);
            this.tableButton.TabIndex = 0;
            this.tableButton.Text = "Tables";
            this.tableButton.UseVisualStyleBackColor = true;
            this.tableButton.Click += new System.EventHandler(this.tableButton_Click);
            // 
            // tableWikiButton
            // 
            this.tableWikiButton.Location = new System.Drawing.Point(12, 128);
            this.tableWikiButton.Name = "tableWikiButton";
            this.tableWikiButton.Size = new System.Drawing.Size(75, 23);
            this.tableWikiButton.TabIndex = 0;
            this.tableWikiButton.Text = "Table wiki";
            this.tableWikiButton.UseVisualStyleBackColor = true;
            this.tableWikiButton.Click += new System.EventHandler(this.tableWikiButton_Click);
            // 
            // valueWikiButton
            // 
            this.valueWikiButton.Location = new System.Drawing.Point(12, 157);
            this.valueWikiButton.Name = "valueWikiButton";
            this.valueWikiButton.Size = new System.Drawing.Size(75, 23);
            this.valueWikiButton.TabIndex = 0;
            this.valueWikiButton.Text = "Value wiki";
            this.valueWikiButton.UseVisualStyleBackColor = true;
            this.valueWikiButton.Click += new System.EventHandler(this.valueWikiButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.valueWikiButton);
            this.Controls.Add(this.tableWikiButton);
            this.Controls.Add(this.tableButton);
            this.Controls.Add(this.functionButton);
            this.Controls.Add(this.ViewButton);
            this.Controls.Add(this.storedProcedureButton);
            this.Name = "Form1";
            this.Text = "SQL Document Builder";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button storedProcedureButton;
        private System.Windows.Forms.Button ViewButton;
        private System.Windows.Forms.Button functionButton;
        private System.Windows.Forms.Button tableButton;
        private System.Windows.Forms.Button tableWikiButton;
        private System.Windows.Forms.Button valueWikiButton;
    }
}
