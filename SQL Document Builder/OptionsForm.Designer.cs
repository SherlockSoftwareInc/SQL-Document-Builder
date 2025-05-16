namespace SQL_Document_Builder
{
    partial class OptionsForm
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
            useIfExistsCheckBox = new System.Windows.Forms.CheckBox();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // useIfExistsCheckBox
            // 
            useIfExistsCheckBox.AutoSize = true;
            useIfExistsCheckBox.Location = new System.Drawing.Point(24, 16);
            useIfExistsCheckBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            useIfExistsCheckBox.Name = "useIfExistsCheckBox";
            useIfExistsCheckBox.Size = new System.Drawing.Size(95, 19);
            useIfExistsCheckBox.TabIndex = 0;
            useIfExistsCheckBox.Text = "Use IF EXISTS";
            useIfExistsCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(245, 172);
            okButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(81, 22);
            okButton.TabIndex = 1;
            okButton.Text = "&Ok";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(329, 172);
            cancelButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(81, 22);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // OptionsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(431, 211);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(useIfExistsCheckBox);
            Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            Name = "OptionsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Options";
            Load += OptionsForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox useIfExistsCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}