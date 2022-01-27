namespace SQL_Document_Builder
{
    partial class TableBuilderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableBuilderForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buildToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tableWikiToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.valuesWikiToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.descEditToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.objectsListBox = new System.Windows.Forms.ListBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.indexesCheckBox = new System.Windows.Forms.CheckBox();
            this.includeHeadersCheckBox = new System.Windows.Forms.CheckBox();
            this.scriptDataCheckBox = new System.Windows.Forms.CheckBox();
            this.noCollationCheckBox = new System.Windows.Forms.CheckBox();
            this.ansiPaddingCheckBox = new System.Windows.Forms.CheckBox();
            this.extendedPropertiesCheckBox = new System.Windows.Forms.CheckBox();
            this.includeIfNotExistsCheckBox = new System.Windows.Forms.CheckBox();
            this.scriptForCreateDropCheckBox = new System.Windows.Forms.CheckBox();
            this.scriptDropsCheckBox = new System.Windows.Forms.CheckBox();
            this.collapsibleSplitter1 = new SQL_Document_Builder.CollapsibleSplitter();
            this.sqlTextBox = new System.Windows.Forms.TextBox();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripButton,
            this.toolStripSeparator1,
            this.buildToolStripButton,
            this.tableWikiToolStripButton,
            this.valuesWikiToolStripButton,
            this.toolStripSeparator2,
            this.descEditToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(927, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // closeToolStripButton
            // 
            this.closeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripButton.Image")));
            this.closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeToolStripButton.Name = "closeToolStripButton";
            this.closeToolStripButton.Size = new System.Drawing.Size(56, 22);
            this.closeToolStripButton.Text = "Close";
            this.closeToolStripButton.Click += new System.EventHandler(this.closeToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buildToolStripButton
            // 
            this.buildToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("buildToolStripButton.Image")));
            this.buildToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buildToolStripButton.Name = "buildToolStripButton";
            this.buildToolStripButton.Size = new System.Drawing.Size(102, 22);
            this.buildToolStripButton.Text = "CREATE TABLE";
            this.buildToolStripButton.Click += new System.EventHandler(this.buildToolStripButton_Click);
            // 
            // tableWikiToolStripButton
            // 
            this.tableWikiToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("tableWikiToolStripButton.Image")));
            this.tableWikiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tableWikiToolStripButton.Name = "tableWikiToolStripButton";
            this.tableWikiToolStripButton.Size = new System.Drawing.Size(78, 22);
            this.tableWikiToolStripButton.Text = "Table wiki";
            this.tableWikiToolStripButton.Click += new System.EventHandler(this.tableWikiToolStripButton_Click);
            // 
            // valuesWikiToolStripButton
            // 
            this.valuesWikiToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("valuesWikiToolStripButton.Image")));
            this.valuesWikiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.valuesWikiToolStripButton.Name = "valuesWikiToolStripButton";
            this.valuesWikiToolStripButton.Size = new System.Drawing.Size(84, 22);
            this.valuesWikiToolStripButton.Text = "Values wiki";
            this.valuesWikiToolStripButton.Click += new System.EventHandler(this.valuesWikiToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // descEditToolStripButton
            // 
            this.descEditToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("descEditToolStripButton.Image")));
            this.descEditToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.descEditToolStripButton.Name = "descEditToolStripButton";
            this.descEditToolStripButton.Size = new System.Drawing.Size(92, 22);
            this.descEditToolStripButton.Text = "Descriptions";
            this.descEditToolStripButton.Click += new System.EventHandler(this.descEditToolStripButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(277, 608);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.objectsListBox);
            this.tabPage1.Controls.Add(this.searchTextBox);
            this.tabPage1.Controls.Add(this.searchLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(269, 580);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Objects";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // objectsListBox
            // 
            this.objectsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectsListBox.FormattingEnabled = true;
            this.objectsListBox.ItemHeight = 15;
            this.objectsListBox.Location = new System.Drawing.Point(3, 41);
            this.objectsListBox.Name = "objectsListBox";
            this.objectsListBox.Size = new System.Drawing.Size(263, 536);
            this.objectsListBox.TabIndex = 3;
            this.objectsListBox.DoubleClick += new System.EventHandler(this.objectsListBox_DoubleClick);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(3, 18);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(263, 23);
            this.searchTextBox.TabIndex = 1;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchLabel.Location = new System.Drawing.Point(3, 3);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(60, 15);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search for";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBox10);
            this.tabPage2.Controls.Add(this.indexesCheckBox);
            this.tabPage2.Controls.Add(this.includeHeadersCheckBox);
            this.tabPage2.Controls.Add(this.scriptDataCheckBox);
            this.tabPage2.Controls.Add(this.noCollationCheckBox);
            this.tabPage2.Controls.Add(this.ansiPaddingCheckBox);
            this.tabPage2.Controls.Add(this.extendedPropertiesCheckBox);
            this.tabPage2.Controls.Add(this.includeIfNotExistsCheckBox);
            this.tabPage2.Controls.Add(this.scriptForCreateDropCheckBox);
            this.tabPage2.Controls.Add(this.scriptDropsCheckBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(269, 580);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(8, 245);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(83, 19);
            this.checkBox10.TabIndex = 7;
            this.checkBox10.Text = "checkBox1";
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // indexesCheckBox
            // 
            this.indexesCheckBox.AutoSize = true;
            this.indexesCheckBox.Location = new System.Drawing.Point(8, 220);
            this.indexesCheckBox.Name = "indexesCheckBox";
            this.indexesCheckBox.Size = new System.Drawing.Size(66, 19);
            this.indexesCheckBox.TabIndex = 8;
            this.indexesCheckBox.Text = "Indexes";
            this.indexesCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeHeadersCheckBox
            // 
            this.includeHeadersCheckBox.AutoSize = true;
            this.includeHeadersCheckBox.Location = new System.Drawing.Point(8, 181);
            this.includeHeadersCheckBox.Name = "includeHeadersCheckBox";
            this.includeHeadersCheckBox.Size = new System.Drawing.Size(108, 19);
            this.includeHeadersCheckBox.TabIndex = 9;
            this.includeHeadersCheckBox.Text = "IncludeHeaders";
            this.includeHeadersCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptDataCheckBox
            // 
            this.scriptDataCheckBox.AutoSize = true;
            this.scriptDataCheckBox.Location = new System.Drawing.Point(8, 156);
            this.scriptDataCheckBox.Name = "scriptDataCheckBox";
            this.scriptDataCheckBox.Size = new System.Drawing.Size(80, 19);
            this.scriptDataCheckBox.TabIndex = 10;
            this.scriptDataCheckBox.Text = "ScriptData";
            this.scriptDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // noCollationCheckBox
            // 
            this.noCollationCheckBox.AutoSize = true;
            this.noCollationCheckBox.Checked = true;
            this.noCollationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noCollationCheckBox.Location = new System.Drawing.Point(8, 131);
            this.noCollationCheckBox.Name = "noCollationCheckBox";
            this.noCollationCheckBox.Size = new System.Drawing.Size(90, 19);
            this.noCollationCheckBox.TabIndex = 11;
            this.noCollationCheckBox.Text = "NoCollation";
            this.noCollationCheckBox.UseVisualStyleBackColor = true;
            // 
            // ansiPaddingCheckBox
            // 
            this.ansiPaddingCheckBox.AutoSize = true;
            this.ansiPaddingCheckBox.Location = new System.Drawing.Point(8, 106);
            this.ansiPaddingCheckBox.Name = "ansiPaddingCheckBox";
            this.ansiPaddingCheckBox.Size = new System.Drawing.Size(93, 19);
            this.ansiPaddingCheckBox.TabIndex = 12;
            this.ansiPaddingCheckBox.Text = "AnsiPadding";
            this.ansiPaddingCheckBox.UseVisualStyleBackColor = true;
            // 
            // extendedPropertiesCheckBox
            // 
            this.extendedPropertiesCheckBox.AutoSize = true;
            this.extendedPropertiesCheckBox.Checked = true;
            this.extendedPropertiesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.extendedPropertiesCheckBox.Location = new System.Drawing.Point(8, 81);
            this.extendedPropertiesCheckBox.Name = "extendedPropertiesCheckBox";
            this.extendedPropertiesCheckBox.Size = new System.Drawing.Size(128, 19);
            this.extendedPropertiesCheckBox.TabIndex = 13;
            this.extendedPropertiesCheckBox.Text = "ExtendedProperties";
            this.extendedPropertiesCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeIfNotExistsCheckBox
            // 
            this.includeIfNotExistsCheckBox.AutoSize = true;
            this.includeIfNotExistsCheckBox.Location = new System.Drawing.Point(8, 56);
            this.includeIfNotExistsCheckBox.Name = "includeIfNotExistsCheckBox";
            this.includeIfNotExistsCheckBox.Size = new System.Drawing.Size(121, 19);
            this.includeIfNotExistsCheckBox.TabIndex = 14;
            this.includeIfNotExistsCheckBox.Text = "IncludeIfNotExists";
            this.includeIfNotExistsCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptForCreateDropCheckBox
            // 
            this.scriptForCreateDropCheckBox.AutoSize = true;
            this.scriptForCreateDropCheckBox.Location = new System.Drawing.Point(8, 31);
            this.scriptForCreateDropCheckBox.Name = "scriptForCreateDropCheckBox";
            this.scriptForCreateDropCheckBox.Size = new System.Drawing.Size(133, 19);
            this.scriptForCreateDropCheckBox.TabIndex = 15;
            this.scriptForCreateDropCheckBox.Text = "ScriptForCreateDrop";
            this.scriptForCreateDropCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptDropsCheckBox
            // 
            this.scriptDropsCheckBox.AutoSize = true;
            this.scriptDropsCheckBox.Location = new System.Drawing.Point(8, 6);
            this.scriptDropsCheckBox.Name = "scriptDropsCheckBox";
            this.scriptDropsCheckBox.Size = new System.Drawing.Size(87, 19);
            this.scriptDropsCheckBox.TabIndex = 16;
            this.scriptDropsCheckBox.Text = "ScriptDrops";
            this.scriptDropsCheckBox.UseVisualStyleBackColor = true;
            // 
            // collapsibleSplitter1
            // 
            this.collapsibleSplitter1.AnimationDelay = 20;
            this.collapsibleSplitter1.AnimationStep = 20;
            this.collapsibleSplitter1.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
            this.collapsibleSplitter1.ControlToHide = this.tabControl1;
            this.collapsibleSplitter1.ExpandParentForm = false;
            this.collapsibleSplitter1.Location = new System.Drawing.Point(277, 25);
            this.collapsibleSplitter1.Name = "collapsibleSplitter1";
            this.collapsibleSplitter1.Size = new System.Drawing.Size(8, 608);
            this.collapsibleSplitter1.SplitterDistance = 277;
            this.collapsibleSplitter1.TabIndex = 6;
            this.collapsibleSplitter1.TabStop = false;
            this.collapsibleSplitter1.UseAnimations = false;
            this.collapsibleSplitter1.VisualStyle = SQL_Document_Builder.VisualStyles.Mozilla;
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sqlTextBox.Location = new System.Drawing.Point(285, 25);
            this.sqlTextBox.Multiline = true;
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sqlTextBox.Size = new System.Drawing.Size(642, 608);
            this.sqlTextBox.TabIndex = 7;
            // 
            // TableBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 633);
            this.Controls.Add(this.sqlTextBox);
            this.Controls.Add(this.collapsibleSplitter1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TableBuilderForm";
            this.Text = "Table script builder";
            this.Load += new System.EventHandler(this.TableBuilderForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton closeToolStripButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.CheckBox indexesCheckBox;
        private System.Windows.Forms.CheckBox includeHeadersCheckBox;
        private System.Windows.Forms.CheckBox scriptDataCheckBox;
        private System.Windows.Forms.CheckBox noCollationCheckBox;
        private System.Windows.Forms.CheckBox ansiPaddingCheckBox;
        private System.Windows.Forms.CheckBox extendedPropertiesCheckBox;
        private System.Windows.Forms.CheckBox includeIfNotExistsCheckBox;
        private System.Windows.Forms.CheckBox scriptForCreateDropCheckBox;
        private System.Windows.Forms.CheckBox scriptDropsCheckBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buildToolStripButton;
        private System.Windows.Forms.ListBox objectsListBox;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Label searchLabel;
        private CollapsibleSplitter collapsibleSplitter1;
        private System.Windows.Forms.TextBox sqlTextBox;
        private System.Windows.Forms.ToolStripButton tableWikiToolStripButton;
        private System.Windows.Forms.ToolStripButton valuesWikiToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton descEditToolStripButton;
    }
}