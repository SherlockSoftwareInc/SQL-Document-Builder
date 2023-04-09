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
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            buildToolStripButton = new System.Windows.Forms.ToolStripButton();
            tableWikiToolStripButton = new System.Windows.Forms.ToolStripButton();
            valuesWikiToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            descEditToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            batchToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            tableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            valuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            tableDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            valueListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            queryDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            objectsListBox = new System.Windows.Forms.ListBox();
            searchTextBox = new System.Windows.Forms.TextBox();
            searchLabel = new System.Windows.Forms.Label();
            schemaComboBox = new System.Windows.Forms.ComboBox();
            schemaLabel = new System.Windows.Forms.Label();
            tabPage2 = new System.Windows.Forms.TabPage();
            checkBox10 = new System.Windows.Forms.CheckBox();
            indexesCheckBox = new System.Windows.Forms.CheckBox();
            includeHeadersCheckBox = new System.Windows.Forms.CheckBox();
            scriptDataCheckBox = new System.Windows.Forms.CheckBox();
            noCollationCheckBox = new System.Windows.Forms.CheckBox();
            ansiPaddingCheckBox = new System.Windows.Forms.CheckBox();
            extendedPropertiesCheckBox = new System.Windows.Forms.CheckBox();
            includeIfNotExistsCheckBox = new System.Windows.Forms.CheckBox();
            scriptForCreateDropCheckBox = new System.Windows.Forms.CheckBox();
            scriptDropsCheckBox = new System.Windows.Forms.CheckBox();
            sqlTextBox = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            headerTextBox = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            footerTextBox = new System.Windows.Forms.TextBox();
            toolStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { closeToolStripButton, toolStripSeparator1, buildToolStripButton, tableWikiToolStripButton, valuesWikiToolStripButton, toolStripSeparator2, descEditToolStripButton, toolStripSeparator3, batchToolStripButton, toolStripDropDownButton1, toolStripDropDownButton2 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            toolStrip1.Size = new System.Drawing.Size(1722, 42);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // closeToolStripButton
            // 
            closeToolStripButton.Image = (System.Drawing.Image)resources.GetObject("closeToolStripButton.Image");
            closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            closeToolStripButton.Name = "closeToolStripButton";
            closeToolStripButton.Size = new System.Drawing.Size(108, 36);
            closeToolStripButton.Text = "Close";
            closeToolStripButton.Click += CloseToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 42);
            // 
            // buildToolStripButton
            // 
            buildToolStripButton.Image = (System.Drawing.Image)resources.GetObject("buildToolStripButton.Image");
            buildToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            buildToolStripButton.Name = "buildToolStripButton";
            buildToolStripButton.Size = new System.Drawing.Size(199, 36);
            buildToolStripButton.Text = "CREATE TABLE";
            buildToolStripButton.Click += BuildToolStripButton_Click;
            // 
            // tableWikiToolStripButton
            // 
            tableWikiToolStripButton.Image = (System.Drawing.Image)resources.GetObject("tableWikiToolStripButton.Image");
            tableWikiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            tableWikiToolStripButton.Name = "tableWikiToolStripButton";
            tableWikiToolStripButton.Size = new System.Drawing.Size(153, 36);
            tableWikiToolStripButton.Text = "Table wiki";
            // 
            // valuesWikiToolStripButton
            // 
            valuesWikiToolStripButton.Image = (System.Drawing.Image)resources.GetObject("valuesWikiToolStripButton.Image");
            valuesWikiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            valuesWikiToolStripButton.Name = "valuesWikiToolStripButton";
            valuesWikiToolStripButton.Size = new System.Drawing.Size(166, 36);
            valuesWikiToolStripButton.Text = "Values wiki";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 42);
            // 
            // descEditToolStripButton
            // 
            descEditToolStripButton.Image = (System.Drawing.Image)resources.GetObject("descEditToolStripButton.Image");
            descEditToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            descEditToolStripButton.Name = "descEditToolStripButton";
            descEditToolStripButton.Size = new System.Drawing.Size(181, 36);
            descEditToolStripButton.Text = "Descriptions";
            descEditToolStripButton.Click += DescEditToolStripButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 42);
            // 
            // batchToolStripButton
            // 
            batchToolStripButton.Image = (System.Drawing.Image)resources.GetObject("batchToolStripButton.Image");
            batchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            batchToolStripButton.Name = "batchToolStripButton";
            batchToolStripButton.Size = new System.Drawing.Size(258, 36);
            batchToolStripButton.Text = "Batch Column Desc";
            batchToolStripButton.Click += BatchToolStripButton_Click;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tableToolStripMenuItem, valuesToolStripMenuItem });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(114, 36);
            toolStripDropDownButton1.Text = "Wiki";
            // 
            // tableToolStripMenuItem
            // 
            tableToolStripMenuItem.Name = "tableToolStripMenuItem";
            tableToolStripMenuItem.Size = new System.Drawing.Size(315, 44);
            tableToolStripMenuItem.Text = "Table Definition";
            tableToolStripMenuItem.Click += TableWikiToolStripButton_Click;
            // 
            // valuesToolStripMenuItem
            // 
            valuesToolStripMenuItem.Name = "valuesToolStripMenuItem";
            valuesToolStripMenuItem.Size = new System.Drawing.Size(315, 44);
            valuesToolStripMenuItem.Text = "Value List";
            valuesToolStripMenuItem.Click += ValuesWikiToolStripButton_Click;
            // 
            // toolStripDropDownButton2
            // 
            toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tableDefinitionToolStripMenuItem, valueListToolStripMenuItem, queryDataToolStripMenuItem });
            toolStripDropDownButton2.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton2.Image");
            toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            toolStripDropDownButton2.Size = new System.Drawing.Size(182, 36);
            toolStripDropDownButton2.Text = "SharePoint";
            // 
            // tableDefinitionToolStripMenuItem
            // 
            tableDefinitionToolStripMenuItem.Name = "tableDefinitionToolStripMenuItem";
            tableDefinitionToolStripMenuItem.Size = new System.Drawing.Size(315, 44);
            tableDefinitionToolStripMenuItem.Text = "Table Definition";
            tableDefinitionToolStripMenuItem.Click += TableDefinitionToolStripMenuItem_Click;
            // 
            // valueListToolStripMenuItem
            // 
            valueListToolStripMenuItem.Name = "valueListToolStripMenuItem";
            valueListToolStripMenuItem.Size = new System.Drawing.Size(315, 44);
            valueListToolStripMenuItem.Text = "Value List";
            valueListToolStripMenuItem.Click += ValueListToolStripMenuItem_Click;
            // 
            // queryDataToolStripMenuItem
            // 
            queryDataToolStripMenuItem.Name = "queryDataToolStripMenuItem";
            queryDataToolStripMenuItem.Size = new System.Drawing.Size(315, 44);
            queryDataToolStripMenuItem.Text = "Query Data";
            queryDataToolStripMenuItem.Click += queryDataToolStripMenuItem_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            tabControl1.Location = new System.Drawing.Point(0, 42);
            tabControl1.Margin = new System.Windows.Forms.Padding(6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(514, 1018);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(objectsListBox);
            tabPage1.Controls.Add(searchTextBox);
            tabPage1.Controls.Add(searchLabel);
            tabPage1.Controls.Add(schemaComboBox);
            tabPage1.Controls.Add(schemaLabel);
            tabPage1.Location = new System.Drawing.Point(8, 46);
            tabPage1.Margin = new System.Windows.Forms.Padding(6);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(6);
            tabPage1.Size = new System.Drawing.Size(498, 964);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Objects";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // objectsListBox
            // 
            objectsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            objectsListBox.FormattingEnabled = true;
            objectsListBox.ItemHeight = 32;
            objectsListBox.Location = new System.Drawing.Point(6, 149);
            objectsListBox.Margin = new System.Windows.Forms.Padding(6);
            objectsListBox.Name = "objectsListBox";
            objectsListBox.Size = new System.Drawing.Size(486, 809);
            objectsListBox.TabIndex = 3;
            objectsListBox.DoubleClick += ObjectsListBox_DoubleClick;
            // 
            // searchTextBox
            // 
            searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            searchTextBox.Location = new System.Drawing.Point(6, 110);
            searchTextBox.Margin = new System.Windows.Forms.Padding(6);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new System.Drawing.Size(486, 39);
            searchTextBox.TabIndex = 1;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Dock = System.Windows.Forms.DockStyle.Top;
            searchLabel.Location = new System.Drawing.Point(6, 78);
            searchLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new System.Drawing.Size(122, 32);
            searchLabel.TabIndex = 0;
            searchLabel.Text = "Search for";
            // 
            // schemaComboBox
            // 
            schemaComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            schemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Location = new System.Drawing.Point(6, 38);
            schemaComboBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(486, 40);
            schemaComboBox.TabIndex = 5;
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // schemaLabel
            // 
            schemaLabel.AutoSize = true;
            schemaLabel.Dock = System.Windows.Forms.DockStyle.Top;
            schemaLabel.Location = new System.Drawing.Point(6, 6);
            schemaLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            schemaLabel.Name = "schemaLabel";
            schemaLabel.Size = new System.Drawing.Size(103, 32);
            schemaLabel.TabIndex = 4;
            schemaLabel.Text = "Schema:";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(checkBox10);
            tabPage2.Controls.Add(indexesCheckBox);
            tabPage2.Controls.Add(includeHeadersCheckBox);
            tabPage2.Controls.Add(scriptDataCheckBox);
            tabPage2.Controls.Add(noCollationCheckBox);
            tabPage2.Controls.Add(ansiPaddingCheckBox);
            tabPage2.Controls.Add(extendedPropertiesCheckBox);
            tabPage2.Controls.Add(includeIfNotExistsCheckBox);
            tabPage2.Controls.Add(scriptForCreateDropCheckBox);
            tabPage2.Controls.Add(scriptDropsCheckBox);
            tabPage2.Location = new System.Drawing.Point(8, 46);
            tabPage2.Margin = new System.Windows.Forms.Padding(6);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(6);
            tabPage2.Size = new System.Drawing.Size(498, 964);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Output options";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            checkBox10.AutoSize = true;
            checkBox10.Location = new System.Drawing.Point(15, 523);
            checkBox10.Margin = new System.Windows.Forms.Padding(6);
            checkBox10.Name = "checkBox10";
            checkBox10.Size = new System.Drawing.Size(159, 36);
            checkBox10.TabIndex = 7;
            checkBox10.Text = "checkBox1";
            checkBox10.UseVisualStyleBackColor = true;
            // 
            // indexesCheckBox
            // 
            indexesCheckBox.AutoSize = true;
            indexesCheckBox.Location = new System.Drawing.Point(15, 469);
            indexesCheckBox.Margin = new System.Windows.Forms.Padding(6);
            indexesCheckBox.Name = "indexesCheckBox";
            indexesCheckBox.Size = new System.Drawing.Size(127, 36);
            indexesCheckBox.TabIndex = 8;
            indexesCheckBox.Text = "Indexes";
            indexesCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeHeadersCheckBox
            // 
            includeHeadersCheckBox.AutoSize = true;
            includeHeadersCheckBox.Location = new System.Drawing.Point(15, 386);
            includeHeadersCheckBox.Margin = new System.Windows.Forms.Padding(6);
            includeHeadersCheckBox.Name = "includeHeadersCheckBox";
            includeHeadersCheckBox.Size = new System.Drawing.Size(211, 36);
            includeHeadersCheckBox.TabIndex = 9;
            includeHeadersCheckBox.Text = "IncludeHeaders";
            includeHeadersCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptDataCheckBox
            // 
            scriptDataCheckBox.AutoSize = true;
            scriptDataCheckBox.Location = new System.Drawing.Point(15, 333);
            scriptDataCheckBox.Margin = new System.Windows.Forms.Padding(6);
            scriptDataCheckBox.Name = "scriptDataCheckBox";
            scriptDataCheckBox.Size = new System.Drawing.Size(155, 36);
            scriptDataCheckBox.TabIndex = 10;
            scriptDataCheckBox.Text = "ScriptData";
            scriptDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // noCollationCheckBox
            // 
            noCollationCheckBox.AutoSize = true;
            noCollationCheckBox.Checked = true;
            noCollationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            noCollationCheckBox.Location = new System.Drawing.Point(15, 279);
            noCollationCheckBox.Margin = new System.Windows.Forms.Padding(6);
            noCollationCheckBox.Name = "noCollationCheckBox";
            noCollationCheckBox.Size = new System.Drawing.Size(173, 36);
            noCollationCheckBox.TabIndex = 11;
            noCollationCheckBox.Text = "NoCollation";
            noCollationCheckBox.UseVisualStyleBackColor = true;
            // 
            // ansiPaddingCheckBox
            // 
            ansiPaddingCheckBox.AutoSize = true;
            ansiPaddingCheckBox.Location = new System.Drawing.Point(15, 226);
            ansiPaddingCheckBox.Margin = new System.Windows.Forms.Padding(6);
            ansiPaddingCheckBox.Name = "ansiPaddingCheckBox";
            ansiPaddingCheckBox.Size = new System.Drawing.Size(177, 36);
            ansiPaddingCheckBox.TabIndex = 12;
            ansiPaddingCheckBox.Text = "AnsiPadding";
            ansiPaddingCheckBox.UseVisualStyleBackColor = true;
            // 
            // extendedPropertiesCheckBox
            // 
            extendedPropertiesCheckBox.AutoSize = true;
            extendedPropertiesCheckBox.Checked = true;
            extendedPropertiesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            extendedPropertiesCheckBox.Location = new System.Drawing.Point(15, 173);
            extendedPropertiesCheckBox.Margin = new System.Windows.Forms.Padding(6);
            extendedPropertiesCheckBox.Name = "extendedPropertiesCheckBox";
            extendedPropertiesCheckBox.Size = new System.Drawing.Size(252, 36);
            extendedPropertiesCheckBox.TabIndex = 13;
            extendedPropertiesCheckBox.Text = "ExtendedProperties";
            extendedPropertiesCheckBox.UseVisualStyleBackColor = true;
            // 
            // includeIfNotExistsCheckBox
            // 
            includeIfNotExistsCheckBox.AutoSize = true;
            includeIfNotExistsCheckBox.Location = new System.Drawing.Point(15, 119);
            includeIfNotExistsCheckBox.Margin = new System.Windows.Forms.Padding(6);
            includeIfNotExistsCheckBox.Name = "includeIfNotExistsCheckBox";
            includeIfNotExistsCheckBox.Size = new System.Drawing.Size(235, 36);
            includeIfNotExistsCheckBox.TabIndex = 14;
            includeIfNotExistsCheckBox.Text = "IncludeIfNotExists";
            includeIfNotExistsCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptForCreateDropCheckBox
            // 
            scriptForCreateDropCheckBox.AutoSize = true;
            scriptForCreateDropCheckBox.Location = new System.Drawing.Point(15, 66);
            scriptForCreateDropCheckBox.Margin = new System.Windows.Forms.Padding(6);
            scriptForCreateDropCheckBox.Name = "scriptForCreateDropCheckBox";
            scriptForCreateDropCheckBox.Size = new System.Drawing.Size(262, 36);
            scriptForCreateDropCheckBox.TabIndex = 15;
            scriptForCreateDropCheckBox.Text = "ScriptForCreateDrop";
            scriptForCreateDropCheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptDropsCheckBox
            // 
            scriptDropsCheckBox.AutoSize = true;
            scriptDropsCheckBox.Location = new System.Drawing.Point(15, 13);
            scriptDropsCheckBox.Margin = new System.Windows.Forms.Padding(6);
            scriptDropsCheckBox.Name = "scriptDropsCheckBox";
            scriptDropsCheckBox.Size = new System.Drawing.Size(169, 36);
            scriptDropsCheckBox.TabIndex = 16;
            scriptDropsCheckBox.Text = "ScriptDrops";
            scriptDropsCheckBox.UseVisualStyleBackColor = true;
            // 
            // sqlTextBox
            // 
            sqlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            sqlTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            sqlTextBox.Location = new System.Drawing.Point(514, 42);
            sqlTextBox.Margin = new System.Windows.Forms.Padding(6);
            sqlTextBox.Multiline = true;
            sqlTextBox.Name = "sqlTextBox";
            sqlTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            sqlTextBox.Size = new System.Drawing.Size(1208, 758);
            sqlTextBox.TabIndex = 7;
            // 
            // label2
            // 
            label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            label2.Location = new System.Drawing.Point(514, 800);
            label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(1208, 32);
            label2.TabIndex = 11;
            label2.Text = "Header";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // headerTextBox
            // 
            headerTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            headerTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            headerTextBox.Location = new System.Drawing.Point(514, 832);
            headerTextBox.Margin = new System.Windows.Forms.Padding(6);
            headerTextBox.Multiline = true;
            headerTextBox.Name = "headerTextBox";
            headerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            headerTextBox.Size = new System.Drawing.Size(1208, 98);
            headerTextBox.TabIndex = 9;
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            label1.Location = new System.Drawing.Point(514, 930);
            label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(1208, 32);
            label1.TabIndex = 10;
            label1.Text = "Footer";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // footerTextBox
            // 
            footerTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            footerTextBox.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            footerTextBox.Location = new System.Drawing.Point(514, 962);
            footerTextBox.Margin = new System.Windows.Forms.Padding(6);
            footerTextBox.Multiline = true;
            footerTextBox.Name = "footerTextBox";
            footerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            footerTextBox.Size = new System.Drawing.Size(1208, 98);
            footerTextBox.TabIndex = 8;
            footerTextBox.Text = "</br>\r\n----\r\nBack to [[BCCR: Database tables|BCCR database tables]]</br>\r\nBack to [[DW: Database tables|Data warehouse tables]]\r\n[[Category: CSBC data warehouse]]\r\n";
            // 
            // TableBuilderForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1722, 1060);
            Controls.Add(sqlTextBox);
            Controls.Add(label2);
            Controls.Add(headerTextBox);
            Controls.Add(label1);
            Controls.Add(footerTextBox);
            Controls.Add(tabControl1);
            Controls.Add(toolStrip1);
            Margin = new System.Windows.Forms.Padding(6);
            Name = "TableBuilderForm";
            Text = "Table script builder";
            FormClosing += TableBuilderForm_FormClosing;
            Load += TableBuilderForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem tableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem valuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem tableDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem valueListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queryDataToolStripMenuItem;
    }
}