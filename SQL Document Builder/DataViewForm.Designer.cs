using System.Drawing;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    partial class DataViewForm
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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            copySQLToClipboardToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            topToolStripComboBox = new ToolStripComboBox();
            toolStripSeparator4 = new ToolStripSeparator();
            saveToolStripButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripButton = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            messageLabel = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            dataGridView = new DataGridView();
            timer1 = new Timer(components);
            timer2 = new Timer(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            valueFrequencyToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(3, 1, 0, 1);
            menuStrip1.Size = new Size(807, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { copySQLToClipboardToolStripMenuItem, toolStripSeparator3, saveToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 22);
            fileToolStripMenuItem.Text = "&File";
            // 
            // copySQLToClipboardToolStripMenuItem
            // 
            copySQLToClipboardToolStripMenuItem.Name = "copySQLToClipboardToolStripMenuItem";
            copySQLToClipboardToolStripMenuItem.Size = new Size(193, 22);
            copySQLToClipboardToolStripMenuItem.Text = "Copy SQL to clipboard";
            copySQLToClipboardToolStripMenuItem.Click += CopySQLToClipboardToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(190, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(193, 22);
            saveToolStripMenuItem.Text = "&Save";
            saveToolStripMenuItem.Visible = false;
            saveToolStripMenuItem.Click += SaveToolStripButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(190, 6);
            toolStripSeparator2.Visible = false;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(193, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_ClickAsync;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, topToolStripComboBox, toolStripSeparator4, saveToolStripButton, toolStripSeparator1, exitToolStripButton });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(0, 0, 2, 0);
            toolStrip1.Size = new Size(807, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(39, 22);
            toolStripLabel1.Text = "Show:";
            // 
            // topToolStripComboBox
            // 
            topToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            topToolStripComboBox.Items.AddRange(new object[] { "TOP 1000", "All" });
            topToolStripComboBox.Name = "topToolStripComboBox";
            topToolStripComboBox.Size = new Size(121, 25);
            topToolStripComboBox.SelectedIndexChanged += TopToolStripComboBox_SelectedIndexChanged;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.ImageTransparentColor = Color.Magenta;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Size = new Size(35, 22);
            saveToolStripButton.Text = "&Save";
            saveToolStripButton.Visible = false;
            saveToolStripButton.Click += SaveToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // exitToolStripButton
            // 
            exitToolStripButton.ImageTransparentColor = Color.Magenta;
            exitToolStripButton.Name = "exitToolStripButton";
            exitToolStripButton.Size = new Size(40, 22);
            exitToolStripButton.Text = "Close";
            exitToolStripButton.Click += ExitToolStripMenuItem_ClickAsync;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { messageLabel, progressBar });
            statusStrip1.Location = new Point(0, 474);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 8, 0);
            statusStrip1.Size = new Size(807, 23);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // messageLabel
            // 
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new Size(174, 18);
            messageLabel.Text = "Please wait while loading data...";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(100, 17);
            progressBar.Style = ProgressBarStyle.Marquee;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(0, 49);
            dataGridView.Margin = new Padding(2, 1, 2, 1);
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.RowHeadersWidth = 82;
            dataGridView.Size = new Size(807, 425);
            dataGridView.TabIndex = 3;
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;
            // 
            // timer1
            // 
            timer1.Interval = 20;
            timer1.Tick += Timer1_Tick;
            // 
            // timer2
            // 
            timer2.Interval = 200;
            timer2.Tick += Timer2_Tick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { valueFrequencyToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(161, 26);
            // 
            // valueFrequencyToolStripMenuItem
            // 
            valueFrequencyToolStripMenuItem.Name = "valueFrequencyToolStripMenuItem";
            valueFrequencyToolStripMenuItem.Size = new Size(160, 22);
            valueFrequencyToolStripMenuItem.Text = "Value Frequency";
            valueFrequencyToolStripMenuItem.Click += ValueFrequencyToolStripMenuItem_Click;
            // 
            // DataViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 497);
            Controls.Add(dataGridView);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Margin = new Padding(2, 1, 2, 1);
            Name = "DataViewForm";
            Text = "Data View Form";
            Load += DataViewForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton saveToolStripButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel messageLabel;
        private DataGridView dataGridView;
        private System.Windows.Forms.Timer timer1;
        private ToolStripProgressBar progressBar;
        private System.Windows.Forms.Timer timer2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton exitToolStripButton;
        private ToolStripMenuItem copySQLToClipboardToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem valueFrequencyToolStripMenuItem;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox topToolStripComboBox;
        private ToolStripSeparator toolStripSeparator4;
    }
}