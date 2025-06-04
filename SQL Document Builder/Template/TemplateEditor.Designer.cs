namespace SQL_Document_Builder.Template
{
    partial class TemplateEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateEditor));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            docTypeToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            docTypeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            objTypeToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            objTypeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            objectNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            objectSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            objectFullNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            objectDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            objectDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            parametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            indexesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            constraintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            relationshipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripButton = new System.Windows.Forms.ToolStripButton();
            templateTextBox = new SqlEditBox();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(923, 24);
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
            exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += ExitToolStripButton_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator3, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator4, selectAllToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z;
            undoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            undoToolStripMenuItem.Text = "&Undo";
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y;
            redoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            redoToolStripMenuItem.Text = "&Redo";
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(161, 6);
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("cutToolStripMenuItem.Image");
            cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X;
            cutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            cutToolStripMenuItem.Click += CutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("copyToolStripMenuItem.Image");
            copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
            copyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripMenuItem.Image");
            pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V;
            pasteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(161, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A;
            selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { docTypeToolStripLabel, docTypeToolStripComboBox, objTypeToolStripLabel, objTypeToolStripComboBox, toolStripSeparator6, toolStripDropDownButton1, toolStripSeparator7, exitToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(923, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // docTypeToolStripLabel
            // 
            docTypeToolStripLabel.Name = "docTypeToolStripLabel";
            docTypeToolStripLabel.Size = new System.Drawing.Size(57, 22);
            docTypeToolStripLabel.Text = "Doc type:";
            // 
            // docTypeToolStripComboBox
            // 
            docTypeToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            docTypeToolStripComboBox.Items.AddRange(new object[] { "Markdown", "SharePoint" });
            docTypeToolStripComboBox.Name = "docTypeToolStripComboBox";
            docTypeToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            docTypeToolStripComboBox.SelectedIndexChanged += DocTypeToolStripComboBox_SelectedIndexChanged;
            // 
            // objTypeToolStripLabel
            // 
            objTypeToolStripLabel.Name = "objTypeToolStripLabel";
            objTypeToolStripLabel.Size = new System.Drawing.Size(71, 22);
            objTypeToolStripLabel.Text = "Object type:";
            // 
            // objTypeToolStripComboBox
            // 
            objTypeToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            objTypeToolStripComboBox.Items.AddRange(new object[] { "Table", "View", "Stored Procedure", "Function", "Trigger" });
            objTypeToolStripComboBox.Name = "objTypeToolStripComboBox";
            objTypeToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            objTypeToolStripComboBox.SelectedIndexChanged += ObjTypeToolStripComboBox_SelectedIndexChanged;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { objectNameToolStripMenuItem, objectSchemaToolStripMenuItem, objectFullNameToolStripMenuItem, objectDescriptionToolStripMenuItem, objectDefinitionToolStripMenuItem, columnsToolStripMenuItem, parametersToolStripMenuItem, indexesToolStripMenuItem, constraintsToolStripMenuItem, relationshipsToolStripMenuItem });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(95, 22);
            toolStripDropDownButton1.Text = "Insert element";
            // 
            // objectNameToolStripMenuItem
            // 
            objectNameToolStripMenuItem.Name = "objectNameToolStripMenuItem";
            objectNameToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            objectNameToolStripMenuItem.Text = "Object name";
            objectNameToolStripMenuItem.Click += ObjectNameToolStripMenuItem_Click;
            // 
            // objectSchemaToolStripMenuItem
            // 
            objectSchemaToolStripMenuItem.Name = "objectSchemaToolStripMenuItem";
            objectSchemaToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            objectSchemaToolStripMenuItem.Text = "Object schema";
            objectSchemaToolStripMenuItem.Click += ObjectSchemaToolStripMenuItem_Click;
            // 
            // objectFullNameToolStripMenuItem
            // 
            objectFullNameToolStripMenuItem.Name = "objectFullNameToolStripMenuItem";
            objectFullNameToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            objectFullNameToolStripMenuItem.Text = "Object full name";
            objectFullNameToolStripMenuItem.Click += ObjectFullNameToolStripMenuItem_Click;
            // 
            // objectDescriptionToolStripMenuItem
            // 
            objectDescriptionToolStripMenuItem.Name = "objectDescriptionToolStripMenuItem";
            objectDescriptionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            objectDescriptionToolStripMenuItem.Text = "Object description";
            objectDescriptionToolStripMenuItem.Click += ObjectDescriptionToolStripMenuItem_Click;
            // 
            // objectDefinitionToolStripMenuItem
            // 
            objectDefinitionToolStripMenuItem.Name = "objectDefinitionToolStripMenuItem";
            objectDefinitionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            objectDefinitionToolStripMenuItem.Text = "Object definition";
            objectDefinitionToolStripMenuItem.Click += ObjectDefinitionToolStripMenuItem_Click;
            // 
            // columnsToolStripMenuItem
            // 
            columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
            columnsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            columnsToolStripMenuItem.Text = "Columns";
            columnsToolStripMenuItem.Click += ColumnsToolStripMenuItem_Click;
            // 
            // parametersToolStripMenuItem
            // 
            parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            parametersToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            parametersToolStripMenuItem.Text = "Parameters";
            parametersToolStripMenuItem.Click += ParametersToolStripMenuItem_Click;
            // 
            // indexesToolStripMenuItem
            // 
            indexesToolStripMenuItem.Name = "indexesToolStripMenuItem";
            indexesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            indexesToolStripMenuItem.Text = "Indexes";
            indexesToolStripMenuItem.Click += IndexesToolStripMenuItem_Click;
            // 
            // constraintsToolStripMenuItem
            // 
            constraintsToolStripMenuItem.Name = "constraintsToolStripMenuItem";
            constraintsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            constraintsToolStripMenuItem.Text = "Constraints";
            constraintsToolStripMenuItem.Click += ConstraintsToolStripMenuItem_Click;
            // 
            // relationshipsToolStripMenuItem
            // 
            relationshipsToolStripMenuItem.Name = "relationshipsToolStripMenuItem";
            relationshipsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            relationshipsToolStripMenuItem.Text = "Relationships";
            relationshipsToolStripMenuItem.Click += RelationshipsToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // exitToolStripButton
            // 
            exitToolStripButton.Image = Properties.Resources.icon_exit;
            exitToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            exitToolStripButton.Name = "exitToolStripButton";
            exitToolStripButton.Size = new System.Drawing.Size(56, 22);
            exitToolStripButton.Text = "Close";
            exitToolStripButton.Click += ExitToolStripButton_Click;
            // 
            // templateTextBox
            // 
            templateTextBox.Alias = "";
            templateTextBox.AllowDrop = true;
            templateTextBox.AutocompleteListSelectedBackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            templateTextBox.AutomaticFold = ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click | ScintillaNET.AutomaticFold.Change;
            templateTextBox.CaretForeColor = System.Drawing.Color.White;
            templateTextBox.DataSourceName = "";
            templateTextBox.DefaultStyleFont = new System.Drawing.Font("Cascadia Mono", 10F);
            templateTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            templateTextBox.DocumentType = SqlEditBox.DocumentTypeEnums.empty;
            templateTextBox.FileName = "";
            templateTextBox.Font = new System.Drawing.Font("Cascadia Mono", 10F);
            templateTextBox.IndentationGuides = ScintillaNET.IndentView.LookBoth;
            templateTextBox.LexerName = "sql";
            templateTextBox.Location = new System.Drawing.Point(0, 49);
            templateTextBox.Name = "templateTextBox";
            templateTextBox.SelectionBackColor = System.Drawing.Color.FromArgb(17, 77, 156);
            templateTextBox.Size = new System.Drawing.Size(923, 530);
            templateTextBox.TabIndex = 2;
            // 
            // TemplateEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(923, 579);
            Controls.Add(templateTextBox);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "TemplateEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Template Editor";
            Load += TemplateEditor_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel docTypeToolStripLabel;
        private System.Windows.Forms.ToolStripComboBox docTypeToolStripComboBox;
        private System.Windows.Forms.ToolStripLabel objTypeToolStripLabel;
        private System.Windows.Forms.ToolStripComboBox objTypeToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton exitToolStripButton;
        private SqlEditBox templateTextBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem objectNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectSchemaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectFullNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectDescriptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem constraintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem relationshipsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    }
}