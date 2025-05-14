namespace SQL_Document_Builder
{
    partial class QueryDataToTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryDataToTableForm));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            newToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            executeToolStripButton = new System.Windows.Forms.ToolStripButton();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            messageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            sqlTextBox = new System.Windows.Forms.TextBox();
            htmlTextBox = new System.Windows.Forms.TextBox();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            exitToolStripMenuItem.Text = "&Close";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator4, selectAllToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("copyToolStripMenuItem.Image");
            copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
            copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Click += CopyToolStripButton_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripMenuItem.Image");
            pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V;
            pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Click += PasteToolStripButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripButton, toolStripSeparator6, copyToolStripButton, pasteToolStripButton, toolStripSeparator7, executeToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            newToolStripButton.Image = (System.Drawing.Image)resources.GetObject("newToolStripButton.Image");
            newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            newToolStripButton.Name = "newToolStripButton";
            newToolStripButton.Size = new System.Drawing.Size(23, 22);
            newToolStripButton.Text = "&New";
            newToolStripButton.Click += NewToolStripButton_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // copyToolStripButton
            // 
            copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            copyToolStripButton.Image = (System.Drawing.Image)resources.GetObject("copyToolStripButton.Image");
            copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            copyToolStripButton.Text = "&Copy";
            copyToolStripButton.Click += CopyToolStripButton_Click;
            // 
            // pasteToolStripButton
            // 
            pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            pasteToolStripButton.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripButton.Image");
            pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            pasteToolStripButton.Text = "&Paste";
            pasteToolStripButton.Click += PasteToolStripButton_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // executeToolStripButton
            // 
            executeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            executeToolStripButton.Image = Properties.Resources.table_view;
            executeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            executeToolStripButton.Name = "executeToolStripButton";
            executeToolStripButton.Size = new System.Drawing.Size(23, 22);
            executeToolStripButton.Text = "toolStripButton1";
            executeToolStripButton.Click += ExecuteToolStripButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { messageLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(800, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // messageLabel
            // 
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new System.Drawing.Size(785, 17);
            messageLabel.Spring = true;
            messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(sqlTextBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(htmlTextBox);
            splitContainer1.Size = new System.Drawing.Size(800, 379);
            splitContainer1.SplitterDistance = 200;
            splitContainer1.TabIndex = 3;
            // 
            // sqlTextBox
            // 
            sqlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            sqlTextBox.Location = new System.Drawing.Point(0, 0);
            sqlTextBox.Multiline = true;
            sqlTextBox.Name = "sqlTextBox";
            sqlTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            sqlTextBox.Size = new System.Drawing.Size(800, 200);
            sqlTextBox.TabIndex = 0;
            sqlTextBox.TextChanged += SqlTextBox_TextChanged;
            // 
            // htmlTextBox
            // 
            htmlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            htmlTextBox.Location = new System.Drawing.Point(0, 0);
            htmlTextBox.Multiline = true;
            htmlTextBox.Name = "htmlTextBox";
            htmlTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            htmlTextBox.Size = new System.Drawing.Size(800, 175);
            htmlTextBox.TabIndex = 1;
            // 
            // QueryDataToTableForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "QueryDataToTableForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Query Data to INSERT statements";
            Load += QueryDataToTableForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel messageLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox sqlTextBox;
        private System.Windows.Forms.TextBox htmlTextBox;
        private System.Windows.Forms.ToolStripButton executeToolStripButton;
    }
}