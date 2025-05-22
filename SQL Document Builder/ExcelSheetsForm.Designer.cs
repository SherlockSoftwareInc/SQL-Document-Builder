namespace SQL_Document_Builder
{
    partial class ExcelSheetsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcelSheetsForm));
            selectSheetLabel = new System.Windows.Forms.Label();
            closeButton = new System.Windows.Forms.Button();
            loadButton = new System.Windows.Forms.Button();
            sheetsListBox = new System.Windows.Forms.ListBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            infoToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            label1 = new System.Windows.Forms.Label();
            tableNameTextBox = new System.Windows.Forms.TextBox();
            nullCheckBox = new System.Windows.Forms.CheckBox();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // selectSheetLabel
            // 
            resources.ApplyResources(selectSheetLabel, "selectSheetLabel");
            selectSheetLabel.Name = "selectSheetLabel";
            // 
            // closeButton
            // 
            resources.ApplyResources(closeButton, "closeButton");
            closeButton.Name = "closeButton";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButton_Click;
            // 
            // loadButton
            // 
            resources.ApplyResources(loadButton, "loadButton");
            loadButton.Name = "loadButton";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += AnalysisButton_Click;
            // 
            // sheetsListBox
            // 
            sheetsListBox.FormattingEnabled = true;
            resources.ApplyResources(sheetsListBox, "sheetsListBox");
            sheetsListBox.Name = "sheetsListBox";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { infoToolStripStatusLabel });
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.Name = "statusStrip1";
            // 
            // infoToolStripStatusLabel
            // 
            infoToolStripStatusLabel.Name = "infoToolStripStatusLabel";
            resources.ApplyResources(infoToolStripStatusLabel, "infoToolStripStatusLabel");
            infoToolStripStatusLabel.Spring = true;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // tableNameTextBox
            // 
            resources.ApplyResources(tableNameTextBox, "tableNameTextBox");
            tableNameTextBox.Name = "tableNameTextBox";
            // 
            // nullCheckBox
            // 
            resources.ApplyResources(nullCheckBox, "nullCheckBox");
            nullCheckBox.Checked = true;
            nullCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            nullCheckBox.Name = "nullCheckBox";
            nullCheckBox.UseVisualStyleBackColor = true;
            // 
            // ExcelSheetsForm
            // 
            AcceptButton = loadButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(nullCheckBox);
            Controls.Add(tableNameTextBox);
            Controls.Add(statusStrip1);
            Controls.Add(label1);
            Controls.Add(selectSheetLabel);
            Controls.Add(closeButton);
            Controls.Add(loadButton);
            Controls.Add(sheetsListBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExcelSheetsForm";
            Load += ExcelSheetsForm_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label selectSheetLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.ListBox sheetsListBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel infoToolStripStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tableNameTextBox;
        private System.Windows.Forms.CheckBox nullCheckBox;
    }
}