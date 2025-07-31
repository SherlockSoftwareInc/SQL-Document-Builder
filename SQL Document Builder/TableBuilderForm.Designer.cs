using System.Windows.Forms;

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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableBuilderForm));
            toolStrip1 = new ToolStrip();
            newToolStripButton1 = new ToolStripButton();
            openToolStripButton = new ToolStripButton();
            saveToolStripButton = new ToolStripButton();
            toolStripSeparator = new ToolStripSeparator();
            cutToolStripButton = new ToolStripButton();
            copyToolStripButton = new ToolStripButton();
            pasteToolStripButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            dataSourcesToolStripComboBox = new ToolStripComboBox();
            toolStripSeparator20 = new ToolStripSeparator();
            toolStripLabel2 = new ToolStripLabel();
            docTypeToolStripComboBox = new ToolStripComboBox();
            objDefToolStripButton = new ToolStripButton();
            mdValuesToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            createTableToolStripButton = new ToolStripButton();
            insertToolStripButton = new ToolStripButton();
            descToolStripButton = new ToolStripButton();
            objectsTabControl = new TabControl();
            tabPage1 = new TabPage();
            objectsListBox = new ListBox();
            panel2 = new Panel();
            searchButton = new Button();
            clearSerachButton = new Button();
            searchTextBox = new TextBox();
            schemaLabel = new Label();
            searchLabel = new Label();
            objectTypeComboBox = new ComboBox();
            schemaComboBox = new ComboBox();
            label1 = new Label();
            tabPage2 = new TabPage();
            autoCopyCheckBox = new CheckBox();
            quotedIDCheckBox = new CheckBox();
            groupBox2 = new GroupBox();
            label2 = new Label();
            label3 = new Label();
            insertMaxTextBox = new TextBox();
            insertBatchTextBox = new TextBox();
            groupBox1 = new GroupBox();
            useUspDescRadioButton = new RadioButton();
            useExtendedPropertyRadioButton = new RadioButton();
            indexesCheckBox = new CheckBox();
            includeHeadersCheckBox = new CheckBox();
            scriptDataCheckBox = new CheckBox();
            noCollationCheckBox = new CheckBox();
            addDataSourceCheckBox = new CheckBox();
            scriptDropsCheckBox = new CheckBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            connectToToolStripMenuItem = new ToolStripMenuItem();
            localToolStripMenuItem = new ToolStripMenuItem();
            azureToolStripMenuItem = new ToolStripMenuItem();
            newConnectionToolStripMenuItem = new ToolStripMenuItem();
            manageConnectionsToolStripMenuItem = new ToolStripMenuItem();
            manageTemplateToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            newToolStripMenuItem1 = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem1 = new ToolStripMenuItem();
            closeAllToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator21 = new ToolStripSeparator();
            recentToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator27 = new ToolStripSeparator();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            quickFindToolStripMenuItem = new ToolStripMenuItem();
            findToolStripMenuItem = new ToolStripMenuItem();
            findAndReplaceToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator16 = new ToolStripSeparator();
            goToLineToolStripMenuItem = new ToolStripMenuItem();
            uppercaseToolStripMenuItem = new ToolStripMenuItem();
            lowercaseToolStripMenuItem = new ToolStripMenuItem();
            builderToolStripMenuItem = new ToolStripMenuItem();
            mdObjectListToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            mdDefinitionToolStripMenuItem = new ToolStripMenuItem();
            mdValuesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator17 = new ToolStripSeparator();
            mdClipboardToTableToolStripMenuItem = new ToolStripMenuItem();
            mdQueryDataToTableToolStripMenuItem = new ToolStripMenuItem();
            jSONToolStripMenuItem = new ToolStripMenuItem();
            objectListToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator25 = new ToolStripSeparator();
            jsonObjectDefinitionToolStripMenuItem = new ToolStripMenuItem();
            jsonTableViewValuesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator26 = new ToolStripSeparator();
            jsonClipboardToTableToolStripMenuItem = new ToolStripMenuItem();
            jsonQueryDataToTableToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            customizeToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            batchColumnDescToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            exportDescriptionsToolStripMenuItem = new ToolStripMenuItem();
            importDescriptionsToolStripMenuItem = new ToolStripMenuItem();
            databaseDDLToolStripMenuItem = new ToolStripMenuItem();
            cREATEToolStripMenuItem1 = new ToolStripMenuItem();
            insertToolStripMenuItem = new ToolStripMenuItem();
            tableDescriptionToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator13 = new ToolStripSeparator();
            createIndexToolStripMenuItem = new ToolStripMenuItem();
            createPrimaryKeyToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator14 = new ToolStripSeparator();
            uspToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator12 = new ToolStripSeparator();
            queryInsertToolStripMenuItem = new ToolStripMenuItem();
            excelToINSERTToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator15 = new ToolStripSeparator();
            batchToolStripMenuItem = new ToolStripMenuItem();
            cREATEToolStripMenuItem = new ToolStripMenuItem();
            cREATEINSERTToolStripMenuItem = new ToolStripMenuItem();
            objectsDescriptionToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            zoomInToolStripMenuItem = new ToolStripMenuItem();
            zoomOutToolStripMenuItem = new ToolStripMenuItem();
            zoom100ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator18 = new ToolStripSeparator();
            collapseAllToolStripMenuItem = new ToolStripMenuItem();
            expandAllToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator19 = new ToolStripSeparator();
            showIndentGuidesToolStripMenuItem = new ToolStripMenuItem();
            showWhitespaceToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator11 = new ToolStripSeparator();
            darkModeToolStripMenuItem = new ToolStripMenuItem();
            executeToolStripMenuItem = new ToolStripMenuItem();
            windowsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            sherlockSoftwareToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            progressBar = new ToolStripProgressBar();
            statusToolStripStatusLabe = new ToolStripStatusLabel();
            serverToolStripStatusLabel = new ToolStripStatusLabel();
            databaseToolStripStatusLabel = new ToolStripStatusLabel();
            searchPanel = new Panel();
            closeSearchButton = new Button();
            nextSearchButton = new Button();
            prevSearchButton = new Button();
            searchSQLTextBox = new TextBox();
            definitionPanel = new ColumnDefView();
            defiCollapsibleSplitter = new CollapsibleSplitter();
            timer1 = new Timer(components);
            splitContainer1 = new SplitContainer();
            replacePanel = new Panel();
            button1 = new Button();
            replaceAllButton = new Button();
            replaceButton = new Button();
            findNextButton = new Button();
            replaceReplaceTextBox = new TextBox();
            replaceSearchTextBox = new TextBox();
            tabControl1 = new ClosableTabControl();
            tabContextMenuStrip = new ContextMenuStrip(components);
            tabAliasToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem1 = new ToolStripMenuItem();
            saveAsToolStripMenuItem1 = new ToolStripMenuItem();
            openFolderInFileExplorerToolStripMenuItem = new ToolStripMenuItem();
            toolTip1 = new ToolTip(components);
            startTimer = new Timer(components);
            toolStrip1.SuspendLayout();
            objectsTabControl.SuspendLayout();
            tabPage1.SuspendLayout();
            panel2.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            searchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            replacePanel.SuspendLayout();
            tabContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { newToolStripButton1, openToolStripButton, saveToolStripButton, toolStripSeparator, cutToolStripButton, copyToolStripButton, pasteToolStripButton, toolStripSeparator3, toolStripLabel1, dataSourcesToolStripComboBox, toolStripSeparator20, toolStripLabel2, docTypeToolStripComboBox, objDefToolStripButton, mdValuesToolStripButton, toolStripSeparator2, createTableToolStripButton, insertToolStripButton, descToolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(0, 0, 2, 0);
            toolStrip1.Size = new System.Drawing.Size(1267, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // newToolStripButton1
            // 
            newToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newToolStripButton1.Image = (System.Drawing.Image)resources.GetObject("newToolStripButton1.Image");
            newToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            newToolStripButton1.Name = "newToolStripButton1";
            newToolStripButton1.Size = new System.Drawing.Size(23, 22);
            newToolStripButton1.Text = "&New";
            newToolStripButton1.ToolTipText = "New document";
            newToolStripButton1.Click += NewToolStripMenuItem_Click;
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = (System.Drawing.Image)resources.GetObject("openToolStripButton.Image");
            openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new System.Drawing.Size(23, 22);
            openToolStripButton.Text = "&Open";
            openToolStripButton.Click += OpenToolStripMenuItem_Click;
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveToolStripButton.Image = (System.Drawing.Image)resources.GetObject("saveToolStripButton.Image");
            saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            saveToolStripButton.Text = "&Save";
            saveToolStripButton.Click += SaveToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // cutToolStripButton
            // 
            cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            cutToolStripButton.Image = (System.Drawing.Image)resources.GetObject("cutToolStripButton.Image");
            cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            cutToolStripButton.Name = "cutToolStripButton";
            cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            cutToolStripButton.Text = "C&ut";
            cutToolStripButton.Click += CutToolStripMenuItem_Click;
            // 
            // copyToolStripButton
            // 
            copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            copyToolStripButton.Image = (System.Drawing.Image)resources.GetObject("copyToolStripButton.Image");
            copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            copyToolStripButton.Text = "&Copy";
            copyToolStripButton.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripButton
            // 
            pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            pasteToolStripButton.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripButton.Image");
            pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            pasteToolStripButton.Text = "&Paste";
            pasteToolStripButton.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(72, 22);
            toolStripLabel1.Text = "Data source:";
            // 
            // dataSourcesToolStripComboBox
            // 
            dataSourcesToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            dataSourcesToolStripComboBox.Name = "dataSourcesToolStripComboBox";
            dataSourcesToolStripComboBox.Size = new System.Drawing.Size(250, 25);
            dataSourcesToolStripComboBox.ToolTipText = "Data source";
            dataSourcesToolStripComboBox.SelectedIndexChanged += DataSourcesToolStripComboBox_SelectedIndexChanged;
            // 
            // toolStripSeparator20
            // 
            toolStripSeparator20.Name = "toolStripSeparator20";
            toolStripSeparator20.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(94, 22);
            toolStripLabel2.Text = "Document Type:";
            // 
            // docTypeToolStripComboBox
            // 
            docTypeToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            docTypeToolStripComboBox.Name = "docTypeToolStripComboBox";
            docTypeToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            docTypeToolStripComboBox.ToolTipText = "Target document type";
            docTypeToolStripComboBox.SelectedIndexChanged += DocTypeToolStripComboBox_SelectedIndexChanged;
            // 
            // objDefToolStripButton
            // 
            objDefToolStripButton.Image = Properties.Resources.markdown;
            objDefToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            objDefToolStripButton.Name = "objDefToolStripButton";
            objDefToolStripButton.Size = new System.Drawing.Size(79, 22);
            objDefToolStripButton.Text = "Definition";
            objDefToolStripButton.ToolTipText = "Generate object definition document";
            objDefToolStripButton.Click += TableDefinitionToolStripMenuItem_Click_1;
            // 
            // mdValuesToolStripButton
            // 
            mdValuesToolStripButton.Image = Properties.Resources.markdown;
            mdValuesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            mdValuesToolStripButton.Name = "mdValuesToolStripButton";
            mdValuesToolStripButton.Size = new System.Drawing.Size(60, 22);
            mdValuesToolStripButton.Text = "Values";
            mdValuesToolStripButton.ToolTipText = "Generate document for object data";
            mdValuesToolStripButton.Click += TableValuesMDToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // createTableToolStripButton
            // 
            createTableToolStripButton.Image = Properties.Resources.sp_16;
            createTableToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            createTableToolStripButton.Name = "createTableToolStripButton";
            createTableToolStripButton.Size = new System.Drawing.Size(68, 22);
            createTableToolStripButton.Text = "CREATE";
            createTableToolStripButton.ToolTipText = "Generate CREATE statement for object";
            createTableToolStripButton.Click += CreateTableToolStripButton_Click;
            // 
            // insertToolStripButton
            // 
            insertToolStripButton.Image = Properties.Resources.sp_16;
            insertToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            insertToolStripButton.Name = "insertToolStripButton";
            insertToolStripButton.Size = new System.Drawing.Size(64, 22);
            insertToolStripButton.Text = "INSERT";
            insertToolStripButton.ToolTipText = "Generate INSERT statement for object data";
            insertToolStripButton.Click += InsertToolStripButton_Click;
            // 
            // descToolStripButton
            // 
            descToolStripButton.Image = Properties.Resources.sp_16;
            descToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            descToolStripButton.Name = "descToolStripButton";
            descToolStripButton.Size = new System.Drawing.Size(87, 22);
            descToolStripButton.Text = "Description";
            descToolStripButton.ToolTipText = "Generate object description statements";
            descToolStripButton.Click += TableDescriptionToolStripMenuItem_Click;
            // 
            // objectsTabControl
            // 
            objectsTabControl.Controls.Add(tabPage1);
            objectsTabControl.Controls.Add(tabPage2);
            objectsTabControl.Dock = DockStyle.Fill;
            objectsTabControl.Location = new System.Drawing.Point(0, 0);
            objectsTabControl.Name = "objectsTabControl";
            objectsTabControl.SelectedIndex = 0;
            objectsTabControl.Size = new System.Drawing.Size(213, 676);
            objectsTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(objectsListBox);
            tabPage1.Controls.Add(panel2);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new System.Drawing.Size(205, 648);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Objects";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // objectsListBox
            // 
            objectsListBox.Dock = DockStyle.Fill;
            objectsListBox.FormattingEnabled = true;
            objectsListBox.IntegralHeight = false;
            objectsListBox.ItemHeight = 15;
            objectsListBox.Location = new System.Drawing.Point(3, 138);
            objectsListBox.Name = "objectsListBox";
            objectsListBox.Size = new System.Drawing.Size(199, 507);
            objectsListBox.TabIndex = 1;
            objectsListBox.SelectedIndexChanged += ObjectsListBox_SelectedIndexChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(searchButton);
            panel2.Controls.Add(clearSerachButton);
            panel2.Controls.Add(searchTextBox);
            panel2.Controls.Add(schemaLabel);
            panel2.Controls.Add(searchLabel);
            panel2.Controls.Add(objectTypeComboBox);
            panel2.Controls.Add(schemaComboBox);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new System.Drawing.Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(199, 135);
            panel2.TabIndex = 0;
            panel2.Resize += Panel2_Resize;
            // 
            // searchButton
            // 
            searchButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            searchButton.Location = new System.Drawing.Point(152, 107);
            searchButton.Name = "searchButton";
            searchButton.Size = new System.Drawing.Size(23, 23);
            searchButton.TabIndex = 7;
            searchButton.Text = "🔍";
            toolTip1.SetToolTip(searchButton, "Global search");
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += SearchButton_Click;
            // 
            // clearSerachButton
            // 
            clearSerachButton.Location = new System.Drawing.Point(123, 107);
            clearSerachButton.Name = "clearSerachButton";
            clearSerachButton.Size = new System.Drawing.Size(23, 23);
            clearSerachButton.TabIndex = 6;
            clearSerachButton.Text = "X";
            toolTip1.SetToolTip(clearSerachButton, "Clear the search box");
            clearSerachButton.UseVisualStyleBackColor = true;
            clearSerachButton.Click += ClearSerachButton_Click;
            // 
            // searchTextBox
            // 
            searchTextBox.Location = new System.Drawing.Point(3, 107);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new System.Drawing.Size(114, 23);
            searchTextBox.TabIndex = 5;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchTextBox.KeyUp += SearchTextBox_KeyUp;
            // 
            // schemaLabel
            // 
            schemaLabel.AutoSize = true;
            schemaLabel.Location = new System.Drawing.Point(3, 47);
            schemaLabel.Name = "schemaLabel";
            schemaLabel.Size = new System.Drawing.Size(52, 15);
            schemaLabel.TabIndex = 2;
            schemaLabel.Text = "Schema:";
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Location = new System.Drawing.Point(3, 89);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new System.Drawing.Size(60, 15);
            searchLabel.TabIndex = 4;
            searchLabel.Text = "Search for";
            // 
            // objectTypeComboBox
            // 
            objectTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            objectTypeComboBox.FormattingEnabled = true;
            objectTypeComboBox.Location = new System.Drawing.Point(3, 21);
            objectTypeComboBox.Name = "objectTypeComboBox";
            objectTypeComboBox.Size = new System.Drawing.Size(168, 23);
            objectTypeComboBox.TabIndex = 1;
            toolTip1.SetToolTip(objectTypeComboBox, "Object type");
            objectTypeComboBox.SelectedIndexChanged += ObjectTypeComboBox_SelectedIndexChanged;
            // 
            // schemaComboBox
            // 
            schemaComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Location = new System.Drawing.Point(3, 64);
            schemaComboBox.Margin = new Padding(3, 2, 3, 2);
            schemaComboBox.Name = "schemaComboBox";
            schemaComboBox.Size = new System.Drawing.Size(171, 23);
            schemaComboBox.TabIndex = 3;
            toolTip1.SetToolTip(schemaComboBox, "Schema");
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 3);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(71, 15);
            label1.TabIndex = 0;
            label1.Text = "Object type:";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(autoCopyCheckBox);
            tabPage2.Controls.Add(quotedIDCheckBox);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Controls.Add(groupBox1);
            tabPage2.Controls.Add(indexesCheckBox);
            tabPage2.Controls.Add(includeHeadersCheckBox);
            tabPage2.Controls.Add(scriptDataCheckBox);
            tabPage2.Controls.Add(noCollationCheckBox);
            tabPage2.Controls.Add(addDataSourceCheckBox);
            tabPage2.Controls.Add(scriptDropsCheckBox);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new System.Drawing.Size(205, 648);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Output options";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // autoCopyCheckBox
            // 
            autoCopyCheckBox.AutoSize = true;
            autoCopyCheckBox.Location = new System.Drawing.Point(8, 284);
            autoCopyCheckBox.Name = "autoCopyCheckBox";
            autoCopyCheckBox.Size = new System.Drawing.Size(200, 19);
            autoCopyCheckBox.TabIndex = 22;
            autoCopyCheckBox.Text = "Copy to Clipboard Automatically";
            autoCopyCheckBox.UseVisualStyleBackColor = true;
            autoCopyCheckBox.CheckedChanged += Options_Changed;
            // 
            // quotedIDCheckBox
            // 
            quotedIDCheckBox.AutoSize = true;
            quotedIDCheckBox.Checked = true;
            quotedIDCheckBox.CheckState = CheckState.Checked;
            quotedIDCheckBox.Location = new System.Drawing.Point(8, 57);
            quotedIDCheckBox.Name = "quotedIDCheckBox";
            quotedIDCheckBox.Size = new System.Drawing.Size(138, 19);
            quotedIDCheckBox.TabIndex = 21;
            quotedIDCheckBox.Text = "Use Quoted Identifier";
            quotedIDCheckBox.UseVisualStyleBackColor = true;
            quotedIDCheckBox.CheckedChanged += Options_Changed;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(insertMaxTextBox);
            groupBox2.Controls.Add(insertBatchTextBox);
            groupBox2.Location = new System.Drawing.Point(8, 165);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(250, 113);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            groupBox2.Text = "INSERT statement options";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 19);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(108, 15);
            label2.TabIndex = 17;
            label2.Text = "INSERT batch rows:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 63);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(132, 15);
            label3.TabIndex = 17;
            label3.Text = "INSERT maximum rows:";
            // 
            // insertMaxTextBox
            // 
            insertMaxTextBox.Location = new System.Drawing.Point(7, 81);
            insertMaxTextBox.MaxLength = 5;
            insertMaxTextBox.Name = "insertMaxTextBox";
            insertMaxTextBox.Size = new System.Drawing.Size(100, 23);
            insertMaxTextBox.TabIndex = 18;
            insertMaxTextBox.Text = "1000";
            insertMaxTextBox.Validating += InsertMaxTextBox_Validating;
            insertMaxTextBox.Validated += InsertMaxTextBox_Validated;
            // 
            // insertBatchTextBox
            // 
            insertBatchTextBox.Location = new System.Drawing.Point(6, 37);
            insertBatchTextBox.MaxLength = 3;
            insertBatchTextBox.Name = "insertBatchTextBox";
            insertBatchTextBox.Size = new System.Drawing.Size(100, 23);
            insertBatchTextBox.TabIndex = 18;
            insertBatchTextBox.Text = "50";
            insertBatchTextBox.Validating += InsertBatchTextBox_Validating;
            insertBatchTextBox.Validated += InsertBatchTextBox_Validated;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(useUspDescRadioButton);
            groupBox1.Controls.Add(useExtendedPropertyRadioButton);
            groupBox1.Location = new System.Drawing.Point(8, 82);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(250, 74);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "Database object description";
            // 
            // useUspDescRadioButton
            // 
            useUspDescRadioButton.AutoSize = true;
            useUspDescRadioButton.Location = new System.Drawing.Point(6, 47);
            useUspDescRadioButton.Name = "useUspDescRadioButton";
            useUspDescRadioButton.Size = new System.Drawing.Size(221, 19);
            useUspDescRadioButton.TabIndex = 0;
            useUspDescRadioButton.TabStop = true;
            useUspDescRadioButton.Text = "Use usp_addupdateextendedproperty";
            useUspDescRadioButton.UseVisualStyleBackColor = true;
            useUspDescRadioButton.CheckedChanged += Options_Changed;
            // 
            // useExtendedPropertyRadioButton
            // 
            useExtendedPropertyRadioButton.AutoSize = true;
            useExtendedPropertyRadioButton.Location = new System.Drawing.Point(6, 22);
            useExtendedPropertyRadioButton.Name = "useExtendedPropertyRadioButton";
            useExtendedPropertyRadioButton.Size = new System.Drawing.Size(177, 19);
            useExtendedPropertyRadioButton.TabIndex = 0;
            useExtendedPropertyRadioButton.TabStop = true;
            useExtendedPropertyRadioButton.Text = "Use sp_addextendedproperty";
            useExtendedPropertyRadioButton.UseVisualStyleBackColor = true;
            useExtendedPropertyRadioButton.CheckedChanged += Options_Changed;
            // 
            // indexesCheckBox
            // 
            indexesCheckBox.AutoSize = true;
            indexesCheckBox.Location = new System.Drawing.Point(9, 528);
            indexesCheckBox.Name = "indexesCheckBox";
            indexesCheckBox.Size = new System.Drawing.Size(65, 19);
            indexesCheckBox.TabIndex = 8;
            indexesCheckBox.Text = "Indexes";
            indexesCheckBox.UseVisualStyleBackColor = true;
            indexesCheckBox.Visible = false;
            indexesCheckBox.CheckedChanged += Options_Changed;
            // 
            // includeHeadersCheckBox
            // 
            includeHeadersCheckBox.AutoSize = true;
            includeHeadersCheckBox.Location = new System.Drawing.Point(9, 503);
            includeHeadersCheckBox.Name = "includeHeadersCheckBox";
            includeHeadersCheckBox.Size = new System.Drawing.Size(108, 19);
            includeHeadersCheckBox.TabIndex = 9;
            includeHeadersCheckBox.Text = "IncludeHeaders";
            includeHeadersCheckBox.UseVisualStyleBackColor = true;
            includeHeadersCheckBox.Visible = false;
            includeHeadersCheckBox.CheckedChanged += Options_Changed;
            // 
            // scriptDataCheckBox
            // 
            scriptDataCheckBox.AutoSize = true;
            scriptDataCheckBox.Location = new System.Drawing.Point(9, 478);
            scriptDataCheckBox.Name = "scriptDataCheckBox";
            scriptDataCheckBox.Size = new System.Drawing.Size(80, 19);
            scriptDataCheckBox.TabIndex = 10;
            scriptDataCheckBox.Text = "ScriptData";
            scriptDataCheckBox.UseVisualStyleBackColor = true;
            scriptDataCheckBox.Visible = false;
            scriptDataCheckBox.CheckedChanged += Options_Changed;
            // 
            // noCollationCheckBox
            // 
            noCollationCheckBox.AutoSize = true;
            noCollationCheckBox.Checked = true;
            noCollationCheckBox.CheckState = CheckState.Checked;
            noCollationCheckBox.Location = new System.Drawing.Point(9, 438);
            noCollationCheckBox.Name = "noCollationCheckBox";
            noCollationCheckBox.Size = new System.Drawing.Size(90, 19);
            noCollationCheckBox.TabIndex = 11;
            noCollationCheckBox.Text = "NoCollation";
            noCollationCheckBox.UseVisualStyleBackColor = true;
            noCollationCheckBox.Visible = false;
            noCollationCheckBox.CheckedChanged += Options_Changed;
            // 
            // addDataSourceCheckBox
            // 
            addDataSourceCheckBox.AutoSize = true;
            addDataSourceCheckBox.Checked = true;
            addDataSourceCheckBox.CheckState = CheckState.Checked;
            addDataSourceCheckBox.Location = new System.Drawing.Point(8, 32);
            addDataSourceCheckBox.Name = "addDataSourceCheckBox";
            addDataSourceCheckBox.Size = new System.Drawing.Size(184, 19);
            addDataSourceCheckBox.TabIndex = 15;
            addDataSourceCheckBox.Text = "Add Data Source at Beginning";
            addDataSourceCheckBox.UseVisualStyleBackColor = true;
            addDataSourceCheckBox.CheckedChanged += Options_Changed;
            // 
            // scriptDropsCheckBox
            // 
            scriptDropsCheckBox.AutoSize = true;
            scriptDropsCheckBox.Checked = true;
            scriptDropsCheckBox.CheckState = CheckState.Checked;
            scriptDropsCheckBox.Location = new System.Drawing.Point(8, 6);
            scriptDropsCheckBox.Name = "scriptDropsCheckBox";
            scriptDropsCheckBox.Size = new System.Drawing.Size(139, 19);
            scriptDropsCheckBox.TabIndex = 16;
            scriptDropsCheckBox.Text = "Add DROP Statement";
            scriptDropsCheckBox.UseVisualStyleBackColor = true;
            scriptDropsCheckBox.CheckedChanged += Options_Changed;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, builderToolStripMenuItem, jSONToolStripMenuItem, toolsToolStripMenuItem, databaseDDLToolStripMenuItem, viewToolStripMenuItem, executeToolStripMenuItem, windowsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(1267, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectToToolStripMenuItem, newConnectionToolStripMenuItem, manageConnectionsToolStripMenuItem, manageTemplateToolStripMenuItem, toolStripSeparator4, newToolStripMenuItem1, openToolStripMenuItem, openFolderToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, closeToolStripMenuItem1, closeAllToolStripMenuItem, toolStripSeparator21, recentToolStripMenuItem, toolStripSeparator5, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // connectToToolStripMenuItem
            // 
            connectToToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { localToolStripMenuItem, azureToolStripMenuItem });
            connectToToolStripMenuItem.Name = "connectToToolStripMenuItem";
            connectToToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            connectToToolStripMenuItem.Text = "Connect to...";
            // 
            // localToolStripMenuItem
            // 
            localToolStripMenuItem.Name = "localToolStripMenuItem";
            localToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            localToolStripMenuItem.Text = "AFDataMart_DEV";
            // 
            // azureToolStripMenuItem
            // 
            azureToolStripMenuItem.Name = "azureToolStripMenuItem";
            azureToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            azureToolStripMenuItem.Text = "(local)";
            // 
            // newConnectionToolStripMenuItem
            // 
            newConnectionToolStripMenuItem.Image = Properties.Resources.add;
            newConnectionToolStripMenuItem.Name = "newConnectionToolStripMenuItem";
            newConnectionToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            newConnectionToolStripMenuItem.Text = "New connection";
            newConnectionToolStripMenuItem.Click += NewConnectionToolStripMenuItem_Click;
            // 
            // manageConnectionsToolStripMenuItem
            // 
            manageConnectionsToolStripMenuItem.Name = "manageConnectionsToolStripMenuItem";
            manageConnectionsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            manageConnectionsToolStripMenuItem.Text = "Manage connections";
            manageConnectionsToolStripMenuItem.Click += ManageConnectionsToolStripMenuItem_Click;
            // 
            // manageTemplateToolStripMenuItem
            // 
            manageTemplateToolStripMenuItem.Name = "manageTemplateToolStripMenuItem";
            manageTemplateToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            manageTemplateToolStripMenuItem.Text = "Manage template";
            manageTemplateToolStripMenuItem.Click += ManageTemplateToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(215, 6);
            // 
            // newToolStripMenuItem1
            // 
            newToolStripMenuItem1.Image = (System.Drawing.Image)resources.GetObject("newToolStripMenuItem1.Image");
            newToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Magenta;
            newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            newToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            newToolStripMenuItem1.Text = "&New";
            newToolStripMenuItem1.Click += NewToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            openToolStripMenuItem.Text = "&Open";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            openFolderToolStripMenuItem.Text = "Open Folder in File Explorer";
            openFolderToolStripMenuItem.Click += OpenFolderInFileExplorerToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            saveToolStripMenuItem.Text = "&Save";
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem1
            // 
            closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            closeToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.W;
            closeToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            closeToolStripMenuItem1.Text = "Close";
            closeToolStripMenuItem1.Click += CloseToolStripMenuItem1_Click;
            // 
            // closeAllToolStripMenuItem
            // 
            closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            closeAllToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            closeAllToolStripMenuItem.Text = "Close All";
            closeAllToolStripMenuItem.Click += CloseAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator21
            // 
            toolStripSeparator21.Name = "toolStripSeparator21";
            toolStripSeparator21.Size = new System.Drawing.Size(215, 6);
            // 
            // recentToolStripMenuItem
            // 
            recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            recentToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            recentToolStripMenuItem.Text = "Recent files";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(215, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            exitToolStripMenuItem.Text = "&Exit";
            exitToolStripMenuItem.Click += CloseToolStripButton_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator27, undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator6, selectAllToolStripMenuItem, toolStripSeparator8, quickFindToolStripMenuItem, findToolStripMenuItem, findAndReplaceToolStripMenuItem, toolStripSeparator16, goToLineToolStripMenuItem, uppercaseToolStripMenuItem, lowercaseToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("cutToolStripMenuItem.Image");
            cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            cutToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            cutToolStripMenuItem.Click += CutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("copyToolStripMenuItem.Image");
            copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            copyToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripMenuItem.Image");
            pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator27
            // 
            toolStripSeparator27.Name = "toolStripSeparator27";
            toolStripSeparator27.Size = new System.Drawing.Size(213, 6);
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Z";
            undoToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Y";
            redoToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(213, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.ShortcutKeyDisplayString = "";
            selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            selectAllToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(213, 6);
            // 
            // quickFindToolStripMenuItem
            // 
            quickFindToolStripMenuItem.Name = "quickFindToolStripMenuItem";
            quickFindToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
            quickFindToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            quickFindToolStripMenuItem.Text = "Quick Find...";
            quickFindToolStripMenuItem.Click += QuickFindToolStripMenuItem_Click;
            // 
            // findToolStripMenuItem
            // 
            findToolStripMenuItem.Image = Properties.Resources.search;
            findToolStripMenuItem.Name = "findToolStripMenuItem";
            findToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt | Keys.F;
            findToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            findToolStripMenuItem.Text = "Find...";
            findToolStripMenuItem.Visible = false;
            findToolStripMenuItem.Click += FindDialogToolStripMenuItem_Click;
            // 
            // findAndReplaceToolStripMenuItem
            // 
            findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            findAndReplaceToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H;
            findAndReplaceToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            findAndReplaceToolStripMenuItem.Text = "Find and Replace...";
            findAndReplaceToolStripMenuItem.Click += FindAndReplaceToolStripMenuItem_Click;
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new System.Drawing.Size(213, 6);
            toolStripSeparator16.Visible = false;
            // 
            // goToLineToolStripMenuItem
            // 
            goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            goToLineToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G;
            goToLineToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            goToLineToolStripMenuItem.Text = "Go To Line...";
            goToLineToolStripMenuItem.Visible = false;
            goToLineToolStripMenuItem.Click += GoToLineToolStripMenuItem_Click;
            // 
            // uppercaseToolStripMenuItem
            // 
            uppercaseToolStripMenuItem.Name = "uppercaseToolStripMenuItem";
            uppercaseToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+U";
            uppercaseToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            uppercaseToolStripMenuItem.Text = "Uppercase";
            uppercaseToolStripMenuItem.Visible = false;
            uppercaseToolStripMenuItem.Click += UppercaseSelectionToolStripMenuItem_Click;
            // 
            // lowercaseToolStripMenuItem
            // 
            lowercaseToolStripMenuItem.Name = "lowercaseToolStripMenuItem";
            lowercaseToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+L";
            lowercaseToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            lowercaseToolStripMenuItem.Text = "Lowercase";
            lowercaseToolStripMenuItem.Visible = false;
            lowercaseToolStripMenuItem.Click += LowercaseSelectionToolStripMenuItem_Click;
            // 
            // builderToolStripMenuItem
            // 
            builderToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mdObjectListToolStripMenuItem, toolStripSeparator7, mdDefinitionToolStripMenuItem, mdValuesToolStripMenuItem, toolStripSeparator17, mdClipboardToTableToolStripMenuItem, mdQueryDataToTableToolStripMenuItem });
            builderToolStripMenuItem.Name = "builderToolStripMenuItem";
            builderToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            builderToolStripMenuItem.Text = "Document Build";
            // 
            // mdObjectListToolStripMenuItem
            // 
            mdObjectListToolStripMenuItem.Name = "mdObjectListToolStripMenuItem";
            mdObjectListToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            mdObjectListToolStripMenuItem.Text = "Object list";
            mdObjectListToolStripMenuItem.Click += TableListToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(203, 6);
            // 
            // mdDefinitionToolStripMenuItem
            // 
            mdDefinitionToolStripMenuItem.Name = "mdDefinitionToolStripMenuItem";
            mdDefinitionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            mdDefinitionToolStripMenuItem.Text = "Object Definition";
            mdDefinitionToolStripMenuItem.Click += TableDefinitionToolStripMenuItem_Click_1;
            // 
            // mdValuesToolStripMenuItem
            // 
            mdValuesToolStripMenuItem.Name = "mdValuesToolStripMenuItem";
            mdValuesToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            mdValuesToolStripMenuItem.Text = "Table/View Values";
            mdValuesToolStripMenuItem.Click += TableValuesMDToolStripMenuItem_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new System.Drawing.Size(203, 6);
            // 
            // mdClipboardToTableToolStripMenuItem
            // 
            mdClipboardToTableToolStripMenuItem.Name = "mdClipboardToTableToolStripMenuItem";
            mdClipboardToTableToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            mdClipboardToTableToolStripMenuItem.Text = "Clipboard to Document";
            mdClipboardToTableToolStripMenuItem.Click += ClipboardToTableToolStripMenuItem1_Click;
            // 
            // mdQueryDataToTableToolStripMenuItem
            // 
            mdQueryDataToTableToolStripMenuItem.Name = "mdQueryDataToTableToolStripMenuItem";
            mdQueryDataToTableToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            mdQueryDataToTableToolStripMenuItem.Text = "Query Data to Document";
            mdQueryDataToTableToolStripMenuItem.Click += QueryDataToTableToolStripMenuItem1_Click;
            // 
            // jSONToolStripMenuItem
            // 
            jSONToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { objectListToolStripMenuItem, toolStripSeparator25, jsonObjectDefinitionToolStripMenuItem, jsonTableViewValuesToolStripMenuItem, toolStripSeparator26, jsonClipboardToTableToolStripMenuItem, jsonQueryDataToTableToolStripMenuItem });
            jSONToolStripMenuItem.Name = "jSONToolStripMenuItem";
            jSONToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            jSONToolStripMenuItem.Text = "JSON";
            // 
            // objectListToolStripMenuItem
            // 
            objectListToolStripMenuItem.Name = "objectListToolStripMenuItem";
            objectListToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            objectListToolStripMenuItem.Text = "Object List";
            objectListToolStripMenuItem.Click += JsonObjectListToolStripMenuItem_Click;
            // 
            // toolStripSeparator25
            // 
            toolStripSeparator25.Name = "toolStripSeparator25";
            toolStripSeparator25.Size = new System.Drawing.Size(175, 6);
            // 
            // jsonObjectDefinitionToolStripMenuItem
            // 
            jsonObjectDefinitionToolStripMenuItem.Name = "jsonObjectDefinitionToolStripMenuItem";
            jsonObjectDefinitionToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            jsonObjectDefinitionToolStripMenuItem.Text = "Object Definition";
            jsonObjectDefinitionToolStripMenuItem.Click += JsonObjectDefinitionToolStripMenuItem_Click;
            // 
            // jsonTableViewValuesToolStripMenuItem
            // 
            jsonTableViewValuesToolStripMenuItem.Name = "jsonTableViewValuesToolStripMenuItem";
            jsonTableViewValuesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            jsonTableViewValuesToolStripMenuItem.Text = "Table/View Values";
            jsonTableViewValuesToolStripMenuItem.Click += JsonTableViewValuesToolStripMenuItem_Click;
            // 
            // toolStripSeparator26
            // 
            toolStripSeparator26.Name = "toolStripSeparator26";
            toolStripSeparator26.Size = new System.Drawing.Size(175, 6);
            // 
            // jsonClipboardToTableToolStripMenuItem
            // 
            jsonClipboardToTableToolStripMenuItem.Name = "jsonClipboardToTableToolStripMenuItem";
            jsonClipboardToTableToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            jsonClipboardToTableToolStripMenuItem.Text = "Clipboard to JSON";
            jsonClipboardToTableToolStripMenuItem.Click += JsonClipboardToTableToolStripMenuItem_Click;
            // 
            // jsonQueryDataToTableToolStripMenuItem
            // 
            jsonQueryDataToTableToolStripMenuItem.Name = "jsonQueryDataToTableToolStripMenuItem";
            jsonQueryDataToTableToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            jsonQueryDataToTableToolStripMenuItem.Text = "Query Data to JSON";
            jsonQueryDataToTableToolStripMenuItem.Click += JsonQueryDataToTableToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { customizeToolStripMenuItem, optionsToolStripMenuItem, toolStripSeparator1, batchColumnDescToolStripMenuItem, toolStripSeparator10, exportDescriptionsToolStripMenuItem, importDescriptionsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // customizeToolStripMenuItem
            // 
            customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            customizeToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            customizeToolStripMenuItem.Text = "&Customize";
            customizeToolStripMenuItem.Visible = false;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            optionsToolStripMenuItem.Text = "&Options";
            optionsToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(175, 6);
            toolStripSeparator1.Visible = false;
            // 
            // batchColumnDescToolStripMenuItem
            // 
            batchColumnDescToolStripMenuItem.Name = "batchColumnDescToolStripMenuItem";
            batchColumnDescToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            batchColumnDescToolStripMenuItem.Text = "Batch Column Desc";
            batchColumnDescToolStripMenuItem.Click += BatchToolStripButton_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new System.Drawing.Size(175, 6);
            // 
            // exportDescriptionsToolStripMenuItem
            // 
            exportDescriptionsToolStripMenuItem.Name = "exportDescriptionsToolStripMenuItem";
            exportDescriptionsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            exportDescriptionsToolStripMenuItem.Text = "Export Descriptions";
            exportDescriptionsToolStripMenuItem.Click += ExportDescriptionsToolStripMenuItem_Click;
            // 
            // importDescriptionsToolStripMenuItem
            // 
            importDescriptionsToolStripMenuItem.Name = "importDescriptionsToolStripMenuItem";
            importDescriptionsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            importDescriptionsToolStripMenuItem.Text = "Import Descriptions";
            importDescriptionsToolStripMenuItem.Click += ImportDescriptionsToolStripMenuItem_Click;
            // 
            // databaseDDLToolStripMenuItem
            // 
            databaseDDLToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cREATEToolStripMenuItem1, insertToolStripMenuItem, tableDescriptionToolStripMenuItem, toolStripSeparator13, createIndexToolStripMenuItem, createPrimaryKeyToolStripMenuItem, toolStripSeparator14, uspToolStripMenuItem, toolStripSeparator12, queryInsertToolStripMenuItem, excelToINSERTToolStripMenuItem, toolStripSeparator15, batchToolStripMenuItem });
            databaseDDLToolStripMenuItem.Name = "databaseDDLToolStripMenuItem";
            databaseDDLToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            databaseDDLToolStripMenuItem.Text = "Database DDL";
            // 
            // cREATEToolStripMenuItem1
            // 
            cREATEToolStripMenuItem1.Name = "cREATEToolStripMenuItem1";
            cREATEToolStripMenuItem1.Size = new System.Drawing.Size(248, 22);
            cREATEToolStripMenuItem1.Text = "CREATE Statement";
            cREATEToolStripMenuItem1.Click += CreateTableToolStripButton_Click;
            // 
            // insertToolStripMenuItem
            // 
            insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            insertToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            insertToolStripMenuItem.Text = "INSERT Statement";
            insertToolStripMenuItem.Click += InsertToolStripButton_Click;
            // 
            // tableDescriptionToolStripMenuItem
            // 
            tableDescriptionToolStripMenuItem.Name = "tableDescriptionToolStripMenuItem";
            tableDescriptionToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            tableDescriptionToolStripMenuItem.Text = "Object Descriptions";
            tableDescriptionToolStripMenuItem.Click += TableDescriptionToolStripMenuItem_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new System.Drawing.Size(245, 6);
            // 
            // createIndexToolStripMenuItem
            // 
            createIndexToolStripMenuItem.Name = "createIndexToolStripMenuItem";
            createIndexToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            createIndexToolStripMenuItem.Text = "Create Index";
            createIndexToolStripMenuItem.Click += CreateIndexToolStripMenuItem_Click;
            // 
            // createPrimaryKeyToolStripMenuItem
            // 
            createPrimaryKeyToolStripMenuItem.Name = "createPrimaryKeyToolStripMenuItem";
            createPrimaryKeyToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            createPrimaryKeyToolStripMenuItem.Text = "Create Primary Key";
            createPrimaryKeyToolStripMenuItem.Click += CreatePrimaryKeyToolStripMenuItem_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new System.Drawing.Size(245, 6);
            // 
            // uspToolStripMenuItem
            // 
            uspToolStripMenuItem.Name = "uspToolStripMenuItem";
            uspToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            uspToolStripMenuItem.Text = "usp_addupdateextendedproperty";
            uspToolStripMenuItem.Click += UspToolStripMenuItem_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new System.Drawing.Size(245, 6);
            // 
            // queryInsertToolStripMenuItem
            // 
            queryInsertToolStripMenuItem.Name = "queryInsertToolStripMenuItem";
            queryInsertToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            queryInsertToolStripMenuItem.Text = "Query to INSERT statements";
            queryInsertToolStripMenuItem.Click += QueryInsertToolStripMenuItem_Click;
            // 
            // excelToINSERTToolStripMenuItem
            // 
            excelToINSERTToolStripMenuItem.Name = "excelToINSERTToolStripMenuItem";
            excelToINSERTToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            excelToINSERTToolStripMenuItem.Text = "Excel to INSERT statements";
            excelToINSERTToolStripMenuItem.Click += ExcelToINSERTToolStripMenuItem_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new System.Drawing.Size(245, 6);
            // 
            // batchToolStripMenuItem
            // 
            batchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cREATEToolStripMenuItem, cREATEINSERTToolStripMenuItem, objectsDescriptionToolStripMenuItem });
            batchToolStripMenuItem.Name = "batchToolStripMenuItem";
            batchToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            batchToolStripMenuItem.Text = "Batch";
            // 
            // cREATEToolStripMenuItem
            // 
            cREATEToolStripMenuItem.Name = "cREATEToolStripMenuItem";
            cREATEToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            cREATEToolStripMenuItem.Text = "CREATE";
            cREATEToolStripMenuItem.Click += CreateToolStripMenuItem_Click;
            // 
            // cREATEINSERTToolStripMenuItem
            // 
            cREATEINSERTToolStripMenuItem.Name = "cREATEINSERTToolStripMenuItem";
            cREATEINSERTToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            cREATEINSERTToolStripMenuItem.Text = "CREATE + INSERT";
            cREATEINSERTToolStripMenuItem.Click += CreateInsertToolStripMenuItem_Click;
            // 
            // objectsDescriptionToolStripMenuItem
            // 
            objectsDescriptionToolStripMenuItem.Name = "objectsDescriptionToolStripMenuItem";
            objectsDescriptionToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            objectsDescriptionToolStripMenuItem.Text = "Objects Description";
            objectsDescriptionToolStripMenuItem.Click += ObjectsDescriptionToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { zoomInToolStripMenuItem, zoomOutToolStripMenuItem, zoom100ToolStripMenuItem, toolStripSeparator18, collapseAllToolStripMenuItem, expandAllToolStripMenuItem, toolStripSeparator19, showIndentGuidesToolStripMenuItem, showWhitespaceToolStripMenuItem, toolStripSeparator11, darkModeToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // zoomInToolStripMenuItem
            // 
            zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            zoomInToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Plus";
            zoomInToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            zoomInToolStripMenuItem.Text = "Zoom In";
            zoomInToolStripMenuItem.Click += ZoomInToolStripMenuItem_Click;
            // 
            // zoomOutToolStripMenuItem
            // 
            zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            zoomOutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Minus";
            zoomOutToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            zoomOutToolStripMenuItem.Text = "Zoom Out";
            zoomOutToolStripMenuItem.Click += ZoomOutToolStripMenuItem_Click;
            // 
            // zoom100ToolStripMenuItem
            // 
            zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
            zoom100ToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+0";
            zoom100ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            zoom100ToolStripMenuItem.Text = "Zoom 100%";
            zoom100ToolStripMenuItem.Click += Zoom100ToolStripMenuItem_Click;
            // 
            // toolStripSeparator18
            // 
            toolStripSeparator18.Name = "toolStripSeparator18";
            toolStripSeparator18.Size = new System.Drawing.Size(193, 6);
            // 
            // collapseAllToolStripMenuItem
            // 
            collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            collapseAllToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            collapseAllToolStripMenuItem.Text = "Collapse All";
            collapseAllToolStripMenuItem.Click += CollapseAllToolStripMenuItem_Click;
            // 
            // expandAllToolStripMenuItem
            // 
            expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            expandAllToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            expandAllToolStripMenuItem.Text = "Expand All";
            expandAllToolStripMenuItem.Click += ExpandAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator19
            // 
            toolStripSeparator19.Name = "toolStripSeparator19";
            toolStripSeparator19.Size = new System.Drawing.Size(193, 6);
            // 
            // showIndentGuidesToolStripMenuItem
            // 
            showIndentGuidesToolStripMenuItem.Checked = true;
            showIndentGuidesToolStripMenuItem.CheckState = CheckState.Checked;
            showIndentGuidesToolStripMenuItem.Name = "showIndentGuidesToolStripMenuItem";
            showIndentGuidesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            showIndentGuidesToolStripMenuItem.Text = "Show Indent Guides";
            showIndentGuidesToolStripMenuItem.Click += IndentGuidesToolStripMenuItem_Click;
            // 
            // showWhitespaceToolStripMenuItem
            // 
            showWhitespaceToolStripMenuItem.Name = "showWhitespaceToolStripMenuItem";
            showWhitespaceToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            showWhitespaceToolStripMenuItem.Text = "Show Whitespace";
            showWhitespaceToolStripMenuItem.Click += HiddenCharactersToolStripMenuItem_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new System.Drawing.Size(193, 6);
            // 
            // darkModeToolStripMenuItem
            // 
            darkModeToolStripMenuItem.Name = "darkModeToolStripMenuItem";
            darkModeToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            darkModeToolStripMenuItem.Text = "Dark Mode";
            darkModeToolStripMenuItem.Click += DarkModeToolStripMenuItem_Click;
            // 
            // executeToolStripMenuItem
            // 
            executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            executeToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            executeToolStripMenuItem.Text = "EXECUTE";
            executeToolStripMenuItem.Click += OnExecuteToolStripMenuItem_Click;
            // 
            // windowsToolStripMenuItem
            // 
            windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            windowsToolStripMenuItem.Text = "&Windows";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, toolStripSeparator9, sherlockSoftwareToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            aboutToolStripMenuItem.Text = "&About...";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new System.Drawing.Size(229, 6);
            // 
            // sherlockSoftwareToolStripMenuItem
            // 
            sherlockSoftwareToolStripMenuItem.Name = "sherlockSoftwareToolStripMenuItem";
            sherlockSoftwareToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            sherlockSoftwareToolStripMenuItem.Text = "Sherlock Software on the Web";
            sherlockSoftwareToolStripMenuItem.Click += SherlockSoftwareToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { progressBar, statusToolStripStatusLabe, serverToolStripStatusLabel, databaseToolStripStatusLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 725);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1267, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(120, 19);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.ToolTipText = "Process progres";
            progressBar.Visible = false;
            // 
            // statusToolStripStatusLabe
            // 
            statusToolStripStatusLabe.Name = "statusToolStripStatusLabe";
            statusToolStripStatusLabe.Size = new System.Drawing.Size(1128, 17);
            statusToolStripStatusLabe.Spring = true;
            statusToolStripStatusLabe.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // serverToolStripStatusLabel
            // 
            serverToolStripStatusLabel.Image = Properties.Resources.server;
            serverToolStripStatusLabel.Name = "serverToolStripStatusLabel";
            serverToolStripStatusLabel.Size = new System.Drawing.Size(54, 17);
            serverToolStripStatusLabel.Text = "server";
            serverToolStripStatusLabel.ToolTipText = "Server name";
            // 
            // databaseToolStripStatusLabel
            // 
            databaseToolStripStatusLabel.Image = Properties.Resources.database;
            databaseToolStripStatusLabel.Name = "databaseToolStripStatusLabel";
            databaseToolStripStatusLabel.Size = new System.Drawing.Size(70, 17);
            databaseToolStripStatusLabel.Text = "database";
            databaseToolStripStatusLabel.ToolTipText = "Database name";
            // 
            // searchPanel
            // 
            searchPanel.Controls.Add(closeSearchButton);
            searchPanel.Controls.Add(nextSearchButton);
            searchPanel.Controls.Add(prevSearchButton);
            searchPanel.Controls.Add(searchSQLTextBox);
            searchPanel.Location = new System.Drawing.Point(343, 3);
            searchPanel.Name = "searchPanel";
            searchPanel.Size = new System.Drawing.Size(240, 29);
            searchPanel.TabIndex = 1;
            searchPanel.Visible = false;
            // 
            // closeSearchButton
            // 
            closeSearchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeSearchButton.BackColor = System.Drawing.Color.White;
            closeSearchButton.FlatStyle = FlatStyle.Flat;
            closeSearchButton.ForeColor = System.Drawing.Color.Black;
            closeSearchButton.Location = new System.Drawing.Point(214, 2);
            closeSearchButton.Name = "closeSearchButton";
            closeSearchButton.Size = new System.Drawing.Size(23, 23);
            closeSearchButton.TabIndex = 3;
            closeSearchButton.Text = "❌";
            closeSearchButton.UseVisualStyleBackColor = false;
            closeSearchButton.Click += CloseQuickSearch_Click;
            // 
            // nextSearchButton
            // 
            nextSearchButton.BackColor = System.Drawing.Color.White;
            nextSearchButton.FlatStyle = FlatStyle.Flat;
            nextSearchButton.ForeColor = System.Drawing.SystemColors.ControlText;
            nextSearchButton.Location = new System.Drawing.Point(188, 2);
            nextSearchButton.Name = "nextSearchButton";
            nextSearchButton.Size = new System.Drawing.Size(23, 23);
            nextSearchButton.TabIndex = 2;
            nextSearchButton.Tag = "Find next (Enter)";
            nextSearchButton.Text = "🠞";
            nextSearchButton.UseVisualStyleBackColor = false;
            nextSearchButton.Click += SearchNext_Click;
            // 
            // prevSearchButton
            // 
            prevSearchButton.BackColor = System.Drawing.Color.White;
            prevSearchButton.FlatStyle = FlatStyle.Flat;
            prevSearchButton.ForeColor = System.Drawing.SystemColors.ControlText;
            prevSearchButton.Location = new System.Drawing.Point(162, 2);
            prevSearchButton.Name = "prevSearchButton";
            prevSearchButton.Size = new System.Drawing.Size(23, 23);
            prevSearchButton.TabIndex = 1;
            prevSearchButton.Tag = "Find previous (Shift+Enter)";
            prevSearchButton.Text = "🠜";
            prevSearchButton.UseVisualStyleBackColor = false;
            prevSearchButton.Click += SearchPrevious_Click;
            // 
            // searchSQLTextBox
            // 
            searchSQLTextBox.Location = new System.Drawing.Point(3, 3);
            searchSQLTextBox.Name = "searchSQLTextBox";
            searchSQLTextBox.Size = new System.Drawing.Size(156, 23);
            searchSQLTextBox.TabIndex = 0;
            searchSQLTextBox.TextChanged += TxtSearch_TextChanged;
            searchSQLTextBox.KeyDown += SearchTextBox_KeyDown;
            // 
            // definitionPanel
            // 
            definitionPanel.Dock = DockStyle.Right;
            definitionPanel.Location = new System.Drawing.Point(907, 49);
            definitionPanel.Margin = new Padding(6);
            definitionPanel.Name = "definitionPanel";
            definitionPanel.Size = new System.Drawing.Size(360, 676);
            definitionPanel.TabIndex = 4;
            toolTip1.SetToolTip(definitionPanel, "Object description edit box");
            definitionPanel.AddIndexRequested += CreateIndexToolStripMenuItem_Click;
            definitionPanel.AddPrimaryKeyRequested += CreatePrimaryKeyToolStripMenuItem_Click;
            // 
            // defiCollapsibleSplitter
            // 
            defiCollapsibleSplitter.AnimationDelay = 2;
            defiCollapsibleSplitter.AnimationStep = 100;
            defiCollapsibleSplitter.BorderStyle3D = Border3DStyle.Flat;
            defiCollapsibleSplitter.ControlToHide = definitionPanel;
            defiCollapsibleSplitter.Cursor = Cursors.VSplit;
            defiCollapsibleSplitter.Dock = DockStyle.Right;
            defiCollapsibleSplitter.ExpandParentForm = false;
            defiCollapsibleSplitter.Location = new System.Drawing.Point(899, 49);
            defiCollapsibleSplitter.Name = "defiCollapsibleSplitter";
            defiCollapsibleSplitter.Size = new System.Drawing.Size(8, 676);
            defiCollapsibleSplitter.TabIndex = 3;
            defiCollapsibleSplitter.TabStop = false;
            defiCollapsibleSplitter.UseAnimations = true;
            defiCollapsibleSplitter.VisualStyle = VisualStyles.Mozilla;
            // 
            // timer1
            // 
            timer1.Interval = 500;
            timer1.Tick += Timer1_Tick;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(objectsTabControl);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(replacePanel);
            splitContainer1.Panel2.Controls.Add(searchPanel);
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new System.Drawing.Size(899, 676);
            splitContainer1.SplitterDistance = 213;
            splitContainer1.TabIndex = 18;
            // 
            // replacePanel
            // 
            replacePanel.Controls.Add(button1);
            replacePanel.Controls.Add(replaceAllButton);
            replacePanel.Controls.Add(replaceButton);
            replacePanel.Controls.Add(findNextButton);
            replacePanel.Controls.Add(replaceReplaceTextBox);
            replacePanel.Controls.Add(replaceSearchTextBox);
            replacePanel.Location = new System.Drawing.Point(285, 203);
            replacePanel.Name = "replacePanel";
            replacePanel.Size = new System.Drawing.Size(240, 60);
            replacePanel.TabIndex = 2;
            replacePanel.Visible = false;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = System.Drawing.Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = System.Drawing.Color.Black;
            button1.Location = new System.Drawing.Point(214, 2);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(23, 23);
            button1.TabIndex = 3;
            button1.Text = "❌";
            toolTip1.SetToolTip(button1, "Close");
            button1.UseVisualStyleBackColor = false;
            button1.Click += CloseQuickSearch_Click;
            // 
            // replaceAllButton
            // 
            replaceAllButton.BackColor = System.Drawing.Color.White;
            replaceAllButton.FlatStyle = FlatStyle.Flat;
            replaceAllButton.ForeColor = System.Drawing.SystemColors.ControlText;
            replaceAllButton.Image = Properties.Resources.replaceAll;
            replaceAllButton.Location = new System.Drawing.Point(214, 31);
            replaceAllButton.Name = "replaceAllButton";
            replaceAllButton.Size = new System.Drawing.Size(23, 23);
            replaceAllButton.TabIndex = 5;
            replaceAllButton.Tag = "Find previous (Shift+Enter)";
            toolTip1.SetToolTip(replaceAllButton, "Replace All");
            replaceAllButton.UseVisualStyleBackColor = false;
            replaceAllButton.Click += ReplaceAllButton_Click;
            // 
            // replaceButton
            // 
            replaceButton.BackColor = System.Drawing.Color.White;
            replaceButton.FlatStyle = FlatStyle.Flat;
            replaceButton.ForeColor = System.Drawing.SystemColors.ControlText;
            replaceButton.Image = Properties.Resources.replace;
            replaceButton.Location = new System.Drawing.Point(188, 31);
            replaceButton.Name = "replaceButton";
            replaceButton.Size = new System.Drawing.Size(23, 23);
            replaceButton.TabIndex = 4;
            replaceButton.Tag = "Find previous (Shift+Enter)";
            toolTip1.SetToolTip(replaceButton, "Replace");
            replaceButton.UseVisualStyleBackColor = false;
            replaceButton.Click += ReplaceButton_Click;
            // 
            // findNextButton
            // 
            findNextButton.BackColor = System.Drawing.Color.White;
            findNextButton.FlatStyle = FlatStyle.Flat;
            findNextButton.ForeColor = System.Drawing.SystemColors.ControlText;
            findNextButton.Location = new System.Drawing.Point(188, 2);
            findNextButton.Name = "findNextButton";
            findNextButton.Size = new System.Drawing.Size(23, 23);
            findNextButton.TabIndex = 2;
            findNextButton.Tag = "Find previous (Shift+Enter)";
            findNextButton.Text = "🠞";
            toolTip1.SetToolTip(findNextButton, "Find next");
            findNextButton.UseVisualStyleBackColor = false;
            findNextButton.Click += FindNextButton_Click;
            // 
            // replaceReplaceTextBox
            // 
            replaceReplaceTextBox.Location = new System.Drawing.Point(3, 32);
            replaceReplaceTextBox.Name = "replaceReplaceTextBox";
            replaceReplaceTextBox.Size = new System.Drawing.Size(182, 23);
            replaceReplaceTextBox.TabIndex = 1;
            replaceReplaceTextBox.KeyDown += ReplaceReplaceTextBox_KeyDown;
            // 
            // replaceSearchTextBox
            // 
            replaceSearchTextBox.Location = new System.Drawing.Point(3, 3);
            replaceSearchTextBox.Name = "replaceSearchTextBox";
            replaceSearchTextBox.Size = new System.Drawing.Size(182, 23);
            replaceSearchTextBox.TabIndex = 0;
            replaceSearchTextBox.TextChanged += TxtSearch_TextChanged;
            replaceSearchTextBox.KeyDown += SearchTextBox_KeyDown;
            // 
            // tabControl1
            // 
            tabControl1.DarkMode = false;
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new System.Drawing.Point(12, 3);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(682, 676);
            tabControl1.TabIndex = 0;
            tabControl1.TabCloseRequested += TabControl1_TabCloseRequested;
            tabControl1.DrawItem += TabControl1_DrawItem;
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            tabControl1.MouseUp += TabControl1_MouseUp;
            tabControl1.Resize += TabControl1_Resize;
            // 
            // tabContextMenuStrip
            // 
            tabContextMenuStrip.Items.AddRange(new ToolStripItem[] { tabAliasToolStripMenuItem, closeToolStripMenuItem, saveToolStripMenuItem1, saveAsToolStripMenuItem1, openFolderInFileExplorerToolStripMenuItem });
            tabContextMenuStrip.Name = "tabContextMenuStrip";
            tabContextMenuStrip.Size = new System.Drawing.Size(219, 114);
            // 
            // tabAliasToolStripMenuItem
            // 
            tabAliasToolStripMenuItem.Name = "tabAliasToolStripMenuItem";
            tabAliasToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            tabAliasToolStripMenuItem.Text = "Tab Alias";
            tabAliasToolStripMenuItem.Click += TabAliasToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem1
            // 
            saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            saveToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            saveToolStripMenuItem1.Text = "Save";
            saveToolStripMenuItem1.Click += SaveToolStripMenuItem1_Click;
            // 
            // saveAsToolStripMenuItem1
            // 
            saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            saveAsToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            saveAsToolStripMenuItem1.Text = "Save as...";
            saveAsToolStripMenuItem1.Click += SaveAsToolStripMenuItem_Click;
            // 
            // openFolderInFileExplorerToolStripMenuItem
            // 
            openFolderInFileExplorerToolStripMenuItem.Name = "openFolderInFileExplorerToolStripMenuItem";
            openFolderInFileExplorerToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            openFolderInFileExplorerToolStripMenuItem.Text = "Open Folder in File Explorer";
            openFolderInFileExplorerToolStripMenuItem.Click += OpenFolderInFileExplorerToolStripMenuItem_Click;
            // 
            // startTimer
            // 
            startTimer.Tick += StartTimer_Tick;
            // 
            // TableBuilderForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1267, 747);
            Controls.Add(splitContainer1);
            Controls.Add(defiCollapsibleSplitter);
            Controls.Add(definitionPanel);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "TableBuilderForm";
            Text = "SQL Server Script and Document Builder";
            FormClosing += TableBuilderForm_FormClosing;
            Load += TableBuilderForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            objectsTabControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            replacePanel.ResumeLayout(false);
            replacePanel.PerformLayout();
            tabContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabControl objectsTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox indexesCheckBox;
        private System.Windows.Forms.CheckBox includeHeadersCheckBox;
        private System.Windows.Forms.CheckBox scriptDataCheckBox;
        private System.Windows.Forms.CheckBox noCollationCheckBox;
        private System.Windows.Forms.CheckBox addDataSourceCheckBox;
        private System.Windows.Forms.CheckBox scriptDropsCheckBox;
        private System.Windows.Forms.ListBox objectsListBox;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ToolStripButton tableWikiToolStripButton;
        private System.Windows.Forms.ToolStripButton htmlValuesToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ComboBox schemaComboBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem azureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusToolStripStatusLabe;
        private System.Windows.Forms.ToolStripStatusLabel serverToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel databaseToolStripStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private ColumnDefView definitionPanel;
        private CollapsibleSplitter defiCollapsibleSplitter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem batchColumnDescToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem queryInsertToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem tableDescriptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createPrimaryKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripButton createTableToolStripButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripButton insertToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem excelToINSERTToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem uspToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem batchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cREATEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectsDescriptionToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox objectTypeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label schemaLabel;
        private System.Windows.Forms.Button clearSerachButton;
        private System.Windows.Forms.ToolStripMenuItem cREATEINSERTToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton newToolStripButton1;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageConnectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseDDLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem builderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mdObjectListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem mdClipboardToTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mdQueryDataToTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickFindToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAndReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Button prevSearchButton;
        private System.Windows.Forms.TextBox searchSQLTextBox;
        private System.Windows.Forms.Button closeSearchButton;
        private System.Windows.Forms.Button nextSearchButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem mdDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mdValuesToolStripMenuItem;
        private System.Windows.Forms.TextBox insertMaxTextBox;
        private System.Windows.Forms.TextBox insertBatchTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private ClosableTabControl tabControl1;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip tabContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tabAliasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem showIndentGuidesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWhitespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem uppercaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lowercaseToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel replacePanel;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.TextBox replaceReplaceTextBox;
        private System.Windows.Forms.TextBox replaceSearchTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox dataSourcesToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton descToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem cREATEToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton useUspDescRadioButton;
        private System.Windows.Forms.RadioButton useExtendedPropertyRadioButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton objDefToolStripButton;
        private System.Windows.Forms.ToolStripButton mdValuesToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem manageTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripMenuItem jsonObjectDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jsonTableViewValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripMenuItem jsonClipboardToTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jsonQueryDataToTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox docTypeToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem sherlockSoftwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem exportDescriptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDescriptionsToolStripMenuItem;
        private System.Windows.Forms.Timer startTimer;
        private System.Windows.Forms.CheckBox quotedIDCheckBox;
        private System.Windows.Forms.CheckBox autoCopyCheckBox;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem darkModeToolStripMenuItem;
        private ToolStripMenuItem openFolderInFileExplorerToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem1;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private Button searchButton;
    }
}