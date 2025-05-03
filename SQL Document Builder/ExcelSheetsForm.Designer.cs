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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcelSheetsForm));
            selectSheetLabel = new System.Windows.Forms.Label();
            closeButton = new System.Windows.Forms.Button();
            analysisButton = new System.Windows.Forms.Button();
            sheetsListBox = new System.Windows.Forms.ListBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            infoToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            startTimer = new System.Windows.Forms.Timer(components);
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
            // analysisButton
            // 
            resources.ApplyResources(analysisButton, "analysisButton");
            analysisButton.Name = "analysisButton";
            analysisButton.UseVisualStyleBackColor = true;
            analysisButton.Click += AnalysisButton_Click;
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
            // startTimer
            // 
            startTimer.Interval = 10;
            startTimer.Tick += StartTimer_Tick;
            // 
            // ExcelSheetsForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(statusStrip1);
            Controls.Add(selectSheetLabel);
            Controls.Add(closeButton);
            Controls.Add(analysisButton);
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
        private System.Windows.Forms.Button analysisButton;
        private System.Windows.Forms.ListBox sheetsListBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel infoToolStripStatusLabel;
        private System.Windows.Forms.Timer startTimer;
    }
}