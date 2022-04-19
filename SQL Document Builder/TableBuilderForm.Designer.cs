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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.batchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.objectsListBox = new System.Windows.Forms.ListBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.schemaComboBox = new System.Windows.Forms.ComboBox();
            this.schemaLabel = new System.Windows.Forms.Label();
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
            this.label2 = new System.Windows.Forms.Label();
            this.headerTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.footerTextBox = new System.Windows.Forms.TextBox();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripButton,
            this.toolStripSeparator1,
            this.buildToolStripButton,
            this.tableWikiToolStripButton,
            this.valuesWikiToolStripButton,
            this.toolStripSeparator2,
            this.descEditToolStripButton,
            this.toolStripSeparator3,
            this.batchToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1059, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // closeToolStripButton
            // 
            this.closeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripButton.Image")));
            this.closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeToolStripButton.Name = "closeToolStripButton";
            this.closeToolStripButton.Size = new System.Drawing.Size(69, 24);
            this.closeToolStripButton.Text = "Close";
            this.closeToolStripButton.Click += new System.EventHandler(this.CloseToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // buildToolStripButton
            // 
            this.buildToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("buildToolStripButton.Image")));
            this.buildToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buildToolStripButton.Name = "buildToolStripButton";
            this.buildToolStripButton.Size = new System.Drawing.Size(129, 24);
            this.buildToolStripButton.Text = "CREATE TABLE";
            this.buildToolStripButton.Click += new System.EventHandler(this.BuildToolStripButton_Click);
            // 
            // tableWikiToolStripButton
            // 
            this.tableWikiToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("tableWikiToolStripButton.Image")));
            this.tableWikiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tableWikiToolStripButton.Name = "tableWikiToolStripButton";
            this.tableWikiToolStripButton.Size = new System.Drawing.Size(98, 24);
            this.tableWikiToolStripButton.Text = "Table wiki";
            this.tableWikiToolStripButton.Click += new System.EventHandler(this.TableWikiToolStripButton_Click);
            // 
            // valuesWikiToolStripButton
            // 
            this.valuesWikiToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("valuesWikiToolStripButton.Image")));
            this.valuesWikiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.valuesWikiToolStripButton.Name = "valuesWikiToolStripButton";
            this.valuesWikiToolStripButton.Size = new System.Drawing.Size(105, 24);
            this.valuesWikiToolStripButton.Text = "Values wiki";
            this.valuesWikiToolStripButton.Click += new System.EventHandler(this.ValuesWikiToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // descEditToolStripButton
            // 
            this.descEditToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("descEditToolStripButton.Image")));
            this.descEditToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.descEditToolStripButton.Name = "descEditToolStripButton";
            this.descEditToolStripButton.Size = new System.Drawing.Size(115, 24);
            this.descEditToolStripButton.Text = "Descriptions";
            this.descEditToolStripButton.Click += new System.EventHandler(this.DescEditToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // batchToolStripButton
            // 
            this.batchToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("batchToolStripButton.Image")));
            this.batchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.batchToolStripButton.Name = "batchToolStripButton";
            this.batchToolStripButton.Size = new System.Drawing.Size(161, 24);
            this.batchToolStripButton.Text = "Batch Column Desc";
            this.batchToolStripButton.Click += new System.EventHandler(this.BatchToolStripButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(317, 817);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.objectsListBox);
            this.tabPage1.Controls.Add(this.searchTextBox);
            this.tabPage1.Controls.Add(this.searchLabel);
            this.tabPage1.Controls.Add(this.schemaComboBox);
            this.tabPage1.Controls.Add(this.schemaLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(309, 784);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Objects";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // objectsListBox
            // 
            this.objectsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectsListBox.FormattingEnabled = true;
            this.objectsListBox.ItemHeight = 20;
            this.objectsListBox.Location = new System.Drawing.Point(3, 99);
            this.objectsListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.objectsListBox.Name = "objectsListBox";
            this.objectsListBox.Size = new System.Drawing.Size(303, 681);
            this.objectsListBox.TabIndex = 3;
            this.objectsListBox.DoubleClick += new System.EventHandler(this.ObjectsListBox_DoubleClick);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(3, 72);
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(303, 27);
            this.searchTextBox.TabIndex = 1;
            this.searchTextBox.TextChanged += new System.EventHandler(this.SearchTextBox_TextChanged);
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchLabel.Location = new System.Drawing.Point(3, 52);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(76, 20);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search for";
            // 
            // schemaComboBox
            // 
            this.schemaComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.schemaComboBox.FormattingEnabled = true;
            this.schemaComboBox.Location = new System.Drawing.Point(3, 24);
            this.schemaComboBox.Name = "schemaComboBox";
            this.schemaComboBox.Size = new System.Drawing.Size(303, 28);
            this.schemaComboBox.TabIndex = 5;
            this.schemaComboBox.SelectedIndexChanged += new System.EventHandler(this.schemaComboBox_SelectedIndexChanged);
            // 
            // schemaLabel
            // 
            this.schemaLabel.AutoSize = true;
            this.schemaLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.schemaLabel.Location = new System.Drawing.Point(3, 4);
            this.schemaLabel.Name = "schemaLabel";
            this.schemaLabel.Size = new System.Drawing.Size(64, 20);
            this.schemaLabel.TabIndex = 4;
            this.schemaLabel.Text = "Schema:";
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
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(309, 784);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(9, 327);
            this.checkBox10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(101, 24);
            this.checkBox10.TabIndex = 7;
            this.checkBox10.Text = "checkBox1";
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // indexesCheckBox
            // 
            this.indexesCheckBox.AutoSize = true;
            this.indexesCheckBox.Location = new System.Drawing.Point(9, 293);
            this.indexesCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.indexesCheckBox.Name = "indexesCheckBox";
            this.indexesCheckBox.Size = new System.Drawing.Size(81, 24);
            this.indexesCheckBox.TabIndex = 8;
            this.indexesCheckBox.Text = "Indexes";
            this.indexesCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeHeadersCheckBox
            // 
            this.includeHeadersCheckBox.AutoSize = true;
            this.includeHeadersCheckBox.Location = new System.Drawing.Point(9, 241);
            this.includeHeadersCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.includeHeadersCheckBox.Name = "includeHeadersCheckBox";
            this.includeHeadersCheckBox.Size = new System.Drawing.Size(134, 24);
            this.includeHeadersCheckBox.TabIndex = 9;
            this.includeHeadersCheckBox.Text = "IncludeHeaders";
            this.includeHeadersCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptDataCheckBox
            // 
            this.scriptDataCheckBox.AutoSize = true;
            this.scriptDataCheckBox.Location = new System.Drawing.Point(9, 208);
            this.scriptDataCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scriptDataCheckBox.Name = "scriptDataCheckBox";
            this.scriptDataCheckBox.Size = new System.Drawing.Size(101, 24);
            this.scriptDataCheckBox.TabIndex = 10;
            this.scriptDataCheckBox.Text = "ScriptData";
            this.scriptDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // noCollationCheckBox
            // 
            this.noCollationCheckBox.AutoSize = true;
            this.noCollationCheckBox.Checked = true;
            this.noCollationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noCollationCheckBox.Location = new System.Drawing.Point(9, 175);
            this.noCollationCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.noCollationCheckBox.Name = "noCollationCheckBox";
            this.noCollationCheckBox.Size = new System.Drawing.Size(111, 24);
            this.noCollationCheckBox.TabIndex = 11;
            this.noCollationCheckBox.Text = "NoCollation";
            this.noCollationCheckBox.UseVisualStyleBackColor = true;
            // 
            // ansiPaddingCheckBox
            // 
            this.ansiPaddingCheckBox.AutoSize = true;
            this.ansiPaddingCheckBox.Location = new System.Drawing.Point(9, 141);
            this.ansiPaddingCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ansiPaddingCheckBox.Name = "ansiPaddingCheckBox";
            this.ansiPaddingCheckBox.Size = new System.Drawing.Size(113, 24);
            this.ansiPaddingCheckBox.TabIndex = 12;
            this.ansiPaddingCheckBox.Text = "AnsiPadding";
            this.ansiPaddingCheckBox.UseVisualStyleBackColor = true;
            // 
            // extendedPropertiesCheckBox
            // 
            this.extendedPropertiesCheckBox.AutoSize = true;
            this.extendedPropertiesCheckBox.Checked = true;
            this.extendedPropertiesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.extendedPropertiesCheckBox.Location = new System.Drawing.Point(9, 108);
            this.extendedPropertiesCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.extendedPropertiesCheckBox.Name = "extendedPropertiesCheckBox";
            this.extendedPropertiesCheckBox.Size = new System.Drawing.Size(160, 24);
            this.extendedPropertiesCheckBox.TabIndex = 13;
            this.extendedPropertiesCheckBox.Text = "ExtendedProperties";
            this.extendedPropertiesCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeIfNotExistsCheckBox
            // 
            this.includeIfNotExistsCheckBox.AutoSize = true;
            this.includeIfNotExistsCheckBox.Location = new System.Drawing.Point(9, 75);
            this.includeIfNotExistsCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.includeIfNotExistsCheckBox.Name = "includeIfNotExistsCheckBox";
            this.includeIfNotExistsCheckBox.Size = new System.Drawing.Size(149, 24);
            this.includeIfNotExistsCheckBox.TabIndex = 14;
            this.includeIfNotExistsCheckBox.Text = "IncludeIfNotExists";
            this.includeIfNotExistsCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptForCreateDropCheckBox
            // 
            this.scriptForCreateDropCheckBox.AutoSize = true;
            this.scriptForCreateDropCheckBox.Location = new System.Drawing.Point(9, 41);
            this.scriptForCreateDropCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scriptForCreateDropCheckBox.Name = "scriptForCreateDropCheckBox";
            this.scriptForCreateDropCheckBox.Size = new System.Drawing.Size(167, 24);
            this.scriptForCreateDropCheckBox.TabIndex = 15;
            this.scriptForCreateDropCheckBox.Text = "ScriptForCreateDrop";
            this.scriptForCreateDropCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptDropsCheckBox
            // 
            this.scriptDropsCheckBox.AutoSize = true;
            this.scriptDropsCheckBox.Location = new System.Drawing.Point(9, 8);
            this.scriptDropsCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scriptDropsCheckBox.Name = "scriptDropsCheckBox";
            this.scriptDropsCheckBox.Size = new System.Drawing.Size(109, 24);
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
            this.collapsibleSplitter1.Location = new System.Drawing.Point(317, 27);
            this.collapsibleSplitter1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.collapsibleSplitter1.Name = "collapsibleSplitter1";
            this.collapsibleSplitter1.Size = new System.Drawing.Size(8, 817);
            this.collapsibleSplitter1.SplitterDistance = 317;
            this.collapsibleSplitter1.TabIndex = 6;
            this.collapsibleSplitter1.TabStop = false;
            this.collapsibleSplitter1.UseAnimations = false;
            this.collapsibleSplitter1.VisualStyle = SQL_Document_Builder.VisualStyles.Mozilla;
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sqlTextBox.Location = new System.Drawing.Point(325, 27);
            this.sqlTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.sqlTextBox.Multiline = true;
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sqlTextBox.Size = new System.Drawing.Size(734, 651);
            this.sqlTextBox.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(325, 678);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(734, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Header";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // headerTextBox
            // 
            this.headerTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.headerTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.headerTextBox.Location = new System.Drawing.Point(325, 698);
            this.headerTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.headerTextBox.Multiline = true;
            this.headerTextBox.Name = "headerTextBox";
            this.headerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.headerTextBox.Size = new System.Drawing.Size(734, 63);
            this.headerTextBox.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(325, 761);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(734, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Footer";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // footerTextBox
            // 
            this.footerTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.footerTextBox.Location = new System.Drawing.Point(325, 781);
            this.footerTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.footerTextBox.Multiline = true;
            this.footerTextBox.Name = "footerTextBox";
            this.footerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.footerTextBox.Size = new System.Drawing.Size(734, 63);
            this.footerTextBox.TabIndex = 8;
            this.footerTextBox.Text = "</br>\r\n----\r\nBack to [[BCCR: Database tables|BCCR database tables]]</br>\r\nBack to" +
    " [[DW: Database tables|Data warehouse tables]]\r\n[[Category: CSBC data warehouse]" +
    "]\r\n";
            // 
            // TableBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 844);
            this.Controls.Add(this.sqlTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.headerTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.footerTextBox);
            this.Controls.Add(this.collapsibleSplitter1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TableBuilderForm";
            this.Text = "Table script builder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TableBuilderForm_FormClosing);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton batchToolStripButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox headerTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox footerTextBox;
        private System.Windows.Forms.ComboBox schemaComboBox;
        private System.Windows.Forms.Label schemaLabel;
    }
}