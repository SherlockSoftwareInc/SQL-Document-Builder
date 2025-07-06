namespace SQL_Document_Builder
{
    partial class DateRangeSelector
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
            label1 = new System.Windows.Forms.Label();
            todayRadioButton = new System.Windows.Forms.RadioButton();
            twoDaysRadioButton = new System.Windows.Forms.RadioButton();
            threeDaysRadioButton = new System.Windows.Forms.RadioButton();
            sevenDaysRadioButton = new System.Windows.Forms.RadioButton();
            monthRadioButton = new System.Windows.Forms.RadioButton();
            specifyDateRadioButton = new System.Windows.Forms.RadioButton();
            specifyDateGroupBox = new System.Windows.Forms.GroupBox();
            endDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            label3 = new System.Windows.Forms.Label();
            startDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            label2 = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            specifyDateGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(269, 15);
            label1.TabIndex = 0;
            label1.Text = "Select a date range for added or modified objects:";
            // 
            // todayRadioButton
            // 
            todayRadioButton.AutoSize = true;
            todayRadioButton.Checked = true;
            todayRadioButton.Location = new System.Drawing.Point(20, 30);
            todayRadioButton.Name = "todayRadioButton";
            todayRadioButton.Size = new System.Drawing.Size(57, 19);
            todayRadioButton.TabIndex = 1;
            todayRadioButton.TabStop = true;
            todayRadioButton.Text = "Today";
            todayRadioButton.UseVisualStyleBackColor = true;
            // 
            // twoDaysRadioButton
            // 
            twoDaysRadioButton.AutoSize = true;
            twoDaysRadioButton.Location = new System.Drawing.Point(20, 55);
            twoDaysRadioButton.Name = "twoDaysRadioButton";
            twoDaysRadioButton.Size = new System.Drawing.Size(117, 19);
            twoDaysRadioButton.TabIndex = 1;
            twoDaysRadioButton.Text = "Within last 2 days";
            twoDaysRadioButton.UseVisualStyleBackColor = true;
            // 
            // threeDaysRadioButton
            // 
            threeDaysRadioButton.AutoSize = true;
            threeDaysRadioButton.Location = new System.Drawing.Point(20, 80);
            threeDaysRadioButton.Name = "threeDaysRadioButton";
            threeDaysRadioButton.Size = new System.Drawing.Size(117, 19);
            threeDaysRadioButton.TabIndex = 1;
            threeDaysRadioButton.Text = "Within last 3 days";
            threeDaysRadioButton.UseVisualStyleBackColor = true;
            // 
            // sevenDaysRadioButton
            // 
            sevenDaysRadioButton.AutoSize = true;
            sevenDaysRadioButton.Location = new System.Drawing.Point(20, 105);
            sevenDaysRadioButton.Name = "sevenDaysRadioButton";
            sevenDaysRadioButton.Size = new System.Drawing.Size(117, 19);
            sevenDaysRadioButton.TabIndex = 1;
            sevenDaysRadioButton.Text = "Within last 7 days";
            sevenDaysRadioButton.UseVisualStyleBackColor = true;
            // 
            // monthRadioButton
            // 
            monthRadioButton.AutoSize = true;
            monthRadioButton.Location = new System.Drawing.Point(20, 130);
            monthRadioButton.Name = "monthRadioButton";
            monthRadioButton.Size = new System.Drawing.Size(123, 19);
            monthRadioButton.TabIndex = 1;
            monthRadioButton.Text = "Within last 30 days";
            monthRadioButton.UseVisualStyleBackColor = true;
            // 
            // specifyDateRadioButton
            // 
            specifyDateRadioButton.AutoSize = true;
            specifyDateRadioButton.Location = new System.Drawing.Point(20, 155);
            specifyDateRadioButton.Name = "specifyDateRadioButton";
            specifyDateRadioButton.Size = new System.Drawing.Size(66, 19);
            specifyDateRadioButton.TabIndex = 1;
            specifyDateRadioButton.Text = "Specify:";
            specifyDateRadioButton.UseVisualStyleBackColor = true;
            specifyDateRadioButton.CheckedChanged += SpecifyDate_CheckedChanged;
            // 
            // specifyDateGroupBox
            // 
            specifyDateGroupBox.Controls.Add(endDateDateTimePicker);
            specifyDateGroupBox.Controls.Add(label3);
            specifyDateGroupBox.Controls.Add(startDateDateTimePicker);
            specifyDateGroupBox.Controls.Add(label2);
            specifyDateGroupBox.Enabled = false;
            specifyDateGroupBox.Location = new System.Drawing.Point(34, 180);
            specifyDateGroupBox.Name = "specifyDateGroupBox";
            specifyDateGroupBox.Size = new System.Drawing.Size(249, 85);
            specifyDateGroupBox.TabIndex = 2;
            specifyDateGroupBox.TabStop = false;
            specifyDateGroupBox.Text = "Custom";
            // 
            // endDateDateTimePicker
            // 
            endDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            endDateDateTimePicker.Location = new System.Drawing.Point(91, 51);
            endDateDateTimePicker.Name = "endDateDateTimePicker";
            endDateDateTimePicker.Size = new System.Drawing.Size(130, 23);
            endDateDateTimePicker.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(31, 57);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(23, 15);
            label3.TabIndex = 0;
            label3.Text = "To:";
            // 
            // startDateDateTimePicker
            // 
            startDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            startDateDateTimePicker.Location = new System.Drawing.Point(91, 22);
            startDateDateTimePicker.Name = "startDateDateTimePicker";
            startDateDateTimePicker.Size = new System.Drawing.Size(130, 23);
            startDateDateTimePicker.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(31, 28);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(38, 15);
            label2.TabIndex = 0;
            label2.Text = "From:";
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(146, 280);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 3;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(227, 280);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // DateRangeSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(316, 315);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(specifyDateGroupBox);
            Controls.Add(specifyDateRadioButton);
            Controls.Add(monthRadioButton);
            Controls.Add(sevenDaysRadioButton);
            Controls.Add(threeDaysRadioButton);
            Controls.Add(twoDaysRadioButton);
            Controls.Add(todayRadioButton);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DateRangeSelector";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Date Range Selector";
            specifyDateGroupBox.ResumeLayout(false);
            specifyDateGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton todayRadioButton;
        private System.Windows.Forms.RadioButton twoDaysRadioButton;
        private System.Windows.Forms.RadioButton threeDaysRadioButton;
        private System.Windows.Forms.RadioButton sevenDaysRadioButton;
        private System.Windows.Forms.RadioButton monthRadioButton;
        private System.Windows.Forms.RadioButton specifyDateRadioButton;
        private System.Windows.Forms.GroupBox specifyDateGroupBox;
        private System.Windows.Forms.DateTimePicker endDateDateTimePicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker startDateDateTimePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}