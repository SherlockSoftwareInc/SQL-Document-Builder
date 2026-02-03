namespace SQL_Document_Builder
{
    partial class InputBox
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
            captionLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            inputTextBox = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // captionLabel
            // 
            captionLabel.AutoSize = true;
            captionLabel.Location = new System.Drawing.Point(10, 15);
            captionLabel.Name = "captionLabel";
            captionLabel.Size = new System.Drawing.Size(38, 15);
            captionLabel.TabIndex = 0;
            captionLabel.Text = "label1";
            // 
            // okButton
            // 
            okButton.AutoSize = true;
            okButton.Location = new System.Drawing.Point(309, 11);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(66, 25);
            okButton.TabIndex = 2;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OKButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(309, 42);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(66, 25);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new System.Drawing.Point(10, 82);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new System.Drawing.Size(364, 23);
            inputTextBox.TabIndex = 1;
            inputTextBox.AcceptsReturn = false;
            inputTextBox.AcceptsTab = false;
            inputTextBox.Multiline = false;
            // 
            // InputBox
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(387, 125);
            Controls.Add(inputTextBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(captionLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputBox";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "InputBox";
            Load += InputBox_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label captionLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox inputTextBox;
    }
}