namespace SQL_Document_Builder
{
    partial class Form2
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
            scintilla1 = new ScintillaNET.Scintilla();
            SuspendLayout();
            // 
            // scintilla1
            // 
            scintilla1.AutocompleteListSelectedBackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            scintilla1.Dock = System.Windows.Forms.DockStyle.Fill;
            scintilla1.LexerName = null;
            scintilla1.Location = new System.Drawing.Point(0, 0);
            scintilla1.Name = "scintilla1";
            scintilla1.ScrollWidth = 49;
            scintilla1.Size = new System.Drawing.Size(715, 443);
            scintilla1.TabIndex = 0;
            scintilla1.Text = "scintilla1";
            scintilla1.UpdateUI += scintilla1_UpdateUI;
            // 
            // Form2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(715, 443);
            Controls.Add(scintilla1);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        #endregion

        private ScintillaNET.Scintilla scintilla1;
    }
}