using SQL_Document_Builder.UI.UserControls;
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
            saveObjectDescriptionsToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem1 = new ToolStripMenuItem();
            closeAllToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator21 = new ToolStripSeparator();
            recentToolStripMenuItem = new ToolStripMenuItem();
            manageRecentFilesToolStripMenuItem = new ToolStripMenuItem();
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
            toolStripSeparator23 = new ToolStripSeparator();
            addColumnReferenceToolStripMenuItem = new ToolStripMenuItem();
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
            aIAssistantToolStripMenuItem = new ToolStripMenuItem();
            aIDescriptionAssistantToolStripMenuItem = new ToolStripMenuItem();
            descriptionAssistantPlusToolStripMenuItem = new ToolStripMenuItem();
            describeMissingToolStripMenuItem = new ToolStripMenuItem();
            matchDescribeToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator22 = new ToolStripSeparator();
            optimizeCodeToolStripMenuItem = new ToolStripMenuItem();
            modifyCodeToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator24 = new ToolStripSeparator();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            executeToolStripMenuItem = new ToolStripMenuItem();
            windowsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            sherlockSoftwareToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            progressBar = new ToolStripProgressBar();
            messageLabel = new ToolStripStatusLabel();
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
            waitTimer = new Timer(components);
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
            resources.ApplyResources(toolStrip1, "toolStrip1");
            toolStrip1.Name = "toolStrip1";
            // 
            // newToolStripButton1
            // 
            newToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(newToolStripButton1, "newToolStripButton1");
            newToolStripButton1.Name = "newToolStripButton1";
            newToolStripButton1.Click += NewToolStripMenuItem_Click;
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = Properties.Resources.openfile1;
            resources.ApplyResources(openToolStripButton, "openToolStripButton");
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Click += OpenToolStripMenuItem_Click;
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(saveToolStripButton, "saveToolStripButton");
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Click += SaveToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            resources.ApplyResources(toolStripSeparator, "toolStripSeparator");
            // 
            // cutToolStripButton
            // 
            cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(cutToolStripButton, "cutToolStripButton");
            cutToolStripButton.Name = "cutToolStripButton";
            cutToolStripButton.Click += CutToolStripMenuItem_Click;
            // 
            // copyToolStripButton
            // 
            copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(copyToolStripButton, "copyToolStripButton");
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripButton
            // 
            pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(pasteToolStripButton, "pasteToolStripButton");
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            resources.ApplyResources(toolStripLabel1, "toolStripLabel1");
            // 
            // dataSourcesToolStripComboBox
            // 
            dataSourcesToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            dataSourcesToolStripComboBox.Name = "dataSourcesToolStripComboBox";
            resources.ApplyResources(dataSourcesToolStripComboBox, "dataSourcesToolStripComboBox");
            dataSourcesToolStripComboBox.SelectedIndexChanged += DataSourcesToolStripComboBox_SelectedIndexChanged;
            // 
            // toolStripSeparator20
            // 
            toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(toolStripSeparator20, "toolStripSeparator20");
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            resources.ApplyResources(toolStripLabel2, "toolStripLabel2");
            // 
            // docTypeToolStripComboBox
            // 
            docTypeToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            docTypeToolStripComboBox.Name = "docTypeToolStripComboBox";
            resources.ApplyResources(docTypeToolStripComboBox, "docTypeToolStripComboBox");
            docTypeToolStripComboBox.SelectedIndexChanged += DocTypeToolStripComboBox_SelectedIndexChanged;
            // 
            // objDefToolStripButton
            // 
            objDefToolStripButton.Image = Properties.Resources.Build_doc;
            resources.ApplyResources(objDefToolStripButton, "objDefToolStripButton");
            objDefToolStripButton.Name = "objDefToolStripButton";
            objDefToolStripButton.Click += TableDefinitionToolStripMenuItem_Click_1;
            // 
            // mdValuesToolStripButton
            // 
            mdValuesToolStripButton.Image = Properties.Resources.Build_doc;
            resources.ApplyResources(mdValuesToolStripButton, "mdValuesToolStripButton");
            mdValuesToolStripButton.Name = "mdValuesToolStripButton";
            mdValuesToolStripButton.Click += TableValuesMDToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // createTableToolStripButton
            // 
            createTableToolStripButton.Image = Properties.Resources.sp_16;
            resources.ApplyResources(createTableToolStripButton, "createTableToolStripButton");
            createTableToolStripButton.Name = "createTableToolStripButton";
            createTableToolStripButton.Click += CreateTableToolStripButton_Click;
            // 
            // insertToolStripButton
            // 
            insertToolStripButton.Image = Properties.Resources.sp_16;
            resources.ApplyResources(insertToolStripButton, "insertToolStripButton");
            insertToolStripButton.Name = "insertToolStripButton";
            insertToolStripButton.Click += InsertToolStripButton_Click;
            // 
            // descToolStripButton
            // 
            descToolStripButton.Image = Properties.Resources.sp_16;
            resources.ApplyResources(descToolStripButton, "descToolStripButton");
            descToolStripButton.Name = "descToolStripButton";
            descToolStripButton.Click += TableDescriptionToolStripMenuItem_Click;
            // 
            // objectsTabControl
            // 
            objectsTabControl.Controls.Add(tabPage1);
            objectsTabControl.Controls.Add(tabPage2);
            resources.ApplyResources(objectsTabControl, "objectsTabControl");
            objectsTabControl.Name = "objectsTabControl";
            objectsTabControl.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(objectsListBox);
            tabPage1.Controls.Add(panel2);
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // objectsListBox
            // 
            resources.ApplyResources(objectsListBox, "objectsListBox");
            objectsListBox.FormattingEnabled = true;
            objectsListBox.Name = "objectsListBox";
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
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            panel2.Resize += Panel2_Resize;
            // 
            // searchButton
            // 
            resources.ApplyResources(searchButton, "searchButton");
            searchButton.Name = "searchButton";
            toolTip1.SetToolTip(searchButton, resources.GetString("searchButton.ToolTip"));
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += SearchButton_Click;
            // 
            // clearSerachButton
            // 
            resources.ApplyResources(clearSerachButton, "clearSerachButton");
            clearSerachButton.Name = "clearSerachButton";
            toolTip1.SetToolTip(clearSerachButton, resources.GetString("clearSerachButton.ToolTip"));
            clearSerachButton.UseVisualStyleBackColor = true;
            clearSerachButton.Click += ClearSerachButton_Click;
            // 
            // searchTextBox
            // 
            resources.ApplyResources(searchTextBox, "searchTextBox");
            searchTextBox.Name = "searchTextBox";
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchTextBox.KeyUp += SearchTextBox_KeyUp;
            // 
            // schemaLabel
            // 
            resources.ApplyResources(schemaLabel, "schemaLabel");
            schemaLabel.Name = "schemaLabel";
            // 
            // searchLabel
            // 
            resources.ApplyResources(searchLabel, "searchLabel");
            searchLabel.Name = "searchLabel";
            // 
            // objectTypeComboBox
            // 
            objectTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            objectTypeComboBox.FormattingEnabled = true;
            resources.ApplyResources(objectTypeComboBox, "objectTypeComboBox");
            objectTypeComboBox.Name = "objectTypeComboBox";
            toolTip1.SetToolTip(objectTypeComboBox, resources.GetString("objectTypeComboBox.ToolTip"));
            objectTypeComboBox.SelectedIndexChanged += ObjectTypeComboBox_SelectedIndexChanged;
            // 
            // schemaComboBox
            // 
            schemaComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            resources.ApplyResources(schemaComboBox, "schemaComboBox");
            schemaComboBox.Name = "schemaComboBox";
            toolTip1.SetToolTip(schemaComboBox, resources.GetString("schemaComboBox.ToolTip"));
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
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
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Name = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // autoCopyCheckBox
            // 
            resources.ApplyResources(autoCopyCheckBox, "autoCopyCheckBox");
            autoCopyCheckBox.Name = "autoCopyCheckBox";
            autoCopyCheckBox.UseVisualStyleBackColor = true;
            autoCopyCheckBox.CheckedChanged += Options_Changed;
            // 
            // quotedIDCheckBox
            // 
            resources.ApplyResources(quotedIDCheckBox, "quotedIDCheckBox");
            quotedIDCheckBox.Checked = true;
            quotedIDCheckBox.CheckState = CheckState.Checked;
            quotedIDCheckBox.Name = "quotedIDCheckBox";
            quotedIDCheckBox.UseVisualStyleBackColor = true;
            quotedIDCheckBox.CheckedChanged += Options_Changed;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(insertMaxTextBox);
            groupBox2.Controls.Add(insertBatchTextBox);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // insertMaxTextBox
            // 
            resources.ApplyResources(insertMaxTextBox, "insertMaxTextBox");
            insertMaxTextBox.Name = "insertMaxTextBox";
            insertMaxTextBox.Validating += InsertMaxTextBox_Validating;
            insertMaxTextBox.Validated += InsertMaxTextBox_Validated;
            // 
            // insertBatchTextBox
            // 
            resources.ApplyResources(insertBatchTextBox, "insertBatchTextBox");
            insertBatchTextBox.Name = "insertBatchTextBox";
            insertBatchTextBox.Validating += InsertBatchTextBox_Validating;
            insertBatchTextBox.Validated += InsertBatchTextBox_Validated;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(useUspDescRadioButton);
            groupBox1.Controls.Add(useExtendedPropertyRadioButton);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // useUspDescRadioButton
            // 
            resources.ApplyResources(useUspDescRadioButton, "useUspDescRadioButton");
            useUspDescRadioButton.Name = "useUspDescRadioButton";
            useUspDescRadioButton.TabStop = true;
            useUspDescRadioButton.UseVisualStyleBackColor = true;
            useUspDescRadioButton.CheckedChanged += Options_Changed;
            // 
            // useExtendedPropertyRadioButton
            // 
            resources.ApplyResources(useExtendedPropertyRadioButton, "useExtendedPropertyRadioButton");
            useExtendedPropertyRadioButton.Name = "useExtendedPropertyRadioButton";
            useExtendedPropertyRadioButton.TabStop = true;
            useExtendedPropertyRadioButton.UseVisualStyleBackColor = true;
            useExtendedPropertyRadioButton.CheckedChanged += Options_Changed;
            // 
            // indexesCheckBox
            // 
            resources.ApplyResources(indexesCheckBox, "indexesCheckBox");
            indexesCheckBox.Name = "indexesCheckBox";
            indexesCheckBox.UseVisualStyleBackColor = true;
            indexesCheckBox.CheckedChanged += Options_Changed;
            // 
            // includeHeadersCheckBox
            // 
            resources.ApplyResources(includeHeadersCheckBox, "includeHeadersCheckBox");
            includeHeadersCheckBox.Name = "includeHeadersCheckBox";
            includeHeadersCheckBox.UseVisualStyleBackColor = true;
            includeHeadersCheckBox.CheckedChanged += Options_Changed;
            // 
            // scriptDataCheckBox
            // 
            resources.ApplyResources(scriptDataCheckBox, "scriptDataCheckBox");
            scriptDataCheckBox.Name = "scriptDataCheckBox";
            scriptDataCheckBox.UseVisualStyleBackColor = true;
            scriptDataCheckBox.CheckedChanged += Options_Changed;
            // 
            // noCollationCheckBox
            // 
            resources.ApplyResources(noCollationCheckBox, "noCollationCheckBox");
            noCollationCheckBox.Checked = true;
            noCollationCheckBox.CheckState = CheckState.Checked;
            noCollationCheckBox.Name = "noCollationCheckBox";
            noCollationCheckBox.UseVisualStyleBackColor = true;
            noCollationCheckBox.CheckedChanged += Options_Changed;
            // 
            // addDataSourceCheckBox
            // 
            resources.ApplyResources(addDataSourceCheckBox, "addDataSourceCheckBox");
            addDataSourceCheckBox.Checked = true;
            addDataSourceCheckBox.CheckState = CheckState.Checked;
            addDataSourceCheckBox.Name = "addDataSourceCheckBox";
            addDataSourceCheckBox.UseVisualStyleBackColor = true;
            addDataSourceCheckBox.CheckedChanged += Options_Changed;
            // 
            // scriptDropsCheckBox
            // 
            resources.ApplyResources(scriptDropsCheckBox, "scriptDropsCheckBox");
            scriptDropsCheckBox.Checked = true;
            scriptDropsCheckBox.CheckState = CheckState.Checked;
            scriptDropsCheckBox.Name = "scriptDropsCheckBox";
            scriptDropsCheckBox.UseVisualStyleBackColor = true;
            scriptDropsCheckBox.CheckedChanged += Options_Changed;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, builderToolStripMenuItem, jSONToolStripMenuItem, toolsToolStripMenuItem, databaseDDLToolStripMenuItem, viewToolStripMenuItem, aIAssistantToolStripMenuItem, executeToolStripMenuItem, windowsToolStripMenuItem, helpToolStripMenuItem });
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectToToolStripMenuItem, newConnectionToolStripMenuItem, manageConnectionsToolStripMenuItem, manageTemplateToolStripMenuItem, toolStripSeparator4, newToolStripMenuItem1, openToolStripMenuItem, openFolderToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, saveObjectDescriptionsToolStripMenuItem, closeToolStripMenuItem1, closeAllToolStripMenuItem, toolStripSeparator21, recentToolStripMenuItem, manageRecentFilesToolStripMenuItem, toolStripSeparator5, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // connectToToolStripMenuItem
            // 
            connectToToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { localToolStripMenuItem, azureToolStripMenuItem });
            connectToToolStripMenuItem.Name = "connectToToolStripMenuItem";
            resources.ApplyResources(connectToToolStripMenuItem, "connectToToolStripMenuItem");
            // 
            // localToolStripMenuItem
            // 
            localToolStripMenuItem.Name = "localToolStripMenuItem";
            resources.ApplyResources(localToolStripMenuItem, "localToolStripMenuItem");
            // 
            // azureToolStripMenuItem
            // 
            azureToolStripMenuItem.Name = "azureToolStripMenuItem";
            resources.ApplyResources(azureToolStripMenuItem, "azureToolStripMenuItem");
            // 
            // newConnectionToolStripMenuItem
            // 
            newConnectionToolStripMenuItem.Image = Properties.Resources.add;
            newConnectionToolStripMenuItem.Name = "newConnectionToolStripMenuItem";
            resources.ApplyResources(newConnectionToolStripMenuItem, "newConnectionToolStripMenuItem");
            newConnectionToolStripMenuItem.Click += NewConnectionToolStripMenuItem_Click;
            // 
            // manageConnectionsToolStripMenuItem
            // 
            manageConnectionsToolStripMenuItem.Name = "manageConnectionsToolStripMenuItem";
            resources.ApplyResources(manageConnectionsToolStripMenuItem, "manageConnectionsToolStripMenuItem");
            manageConnectionsToolStripMenuItem.Click += ManageConnectionsToolStripMenuItem_Click;
            // 
            // manageTemplateToolStripMenuItem
            // 
            manageTemplateToolStripMenuItem.Name = "manageTemplateToolStripMenuItem";
            resources.ApplyResources(manageTemplateToolStripMenuItem, "manageTemplateToolStripMenuItem");
            manageTemplateToolStripMenuItem.Click += ManageTemplateToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            // 
            // newToolStripMenuItem1
            // 
            resources.ApplyResources(newToolStripMenuItem1, "newToolStripMenuItem1");
            newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            newToolStripMenuItem1.Click += NewToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(openToolStripMenuItem, "openToolStripMenuItem");
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            resources.ApplyResources(openFolderToolStripMenuItem, "openFolderToolStripMenuItem");
            openFolderToolStripMenuItem.Click += OpenFolderInFileExplorerToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(saveToolStripMenuItem, "saveToolStripMenuItem");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            // 
            // saveObjectDescriptionsToolStripMenuItem
            // 
            saveObjectDescriptionsToolStripMenuItem.Name = "saveObjectDescriptionsToolStripMenuItem";
            resources.ApplyResources(saveObjectDescriptionsToolStripMenuItem, "saveObjectDescriptionsToolStripMenuItem");
            saveObjectDescriptionsToolStripMenuItem.Click += SaveObjectDescriptionsToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem1
            // 
            closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            resources.ApplyResources(closeToolStripMenuItem1, "closeToolStripMenuItem1");
            closeToolStripMenuItem1.Click += CloseToolStripMenuItem1_Click;
            // 
            // closeAllToolStripMenuItem
            // 
            closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            resources.ApplyResources(closeAllToolStripMenuItem, "closeAllToolStripMenuItem");
            closeAllToolStripMenuItem.Click += CloseAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator21
            // 
            toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(toolStripSeparator21, "toolStripSeparator21");
            // 
            // recentToolStripMenuItem
            // 
            recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            resources.ApplyResources(recentToolStripMenuItem, "recentToolStripMenuItem");
            // 
            // manageRecentFilesToolStripMenuItem
            // 
            manageRecentFilesToolStripMenuItem.Name = "manageRecentFilesToolStripMenuItem";
            resources.ApplyResources(manageRecentFilesToolStripMenuItem, "manageRecentFilesToolStripMenuItem");
            manageRecentFilesToolStripMenuItem.Click += ManageRecentFilesToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.Click += CloseToolStripButton_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator27, undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator6, selectAllToolStripMenuItem, toolStripSeparator8, quickFindToolStripMenuItem, findToolStripMenuItem, findAndReplaceToolStripMenuItem, toolStripSeparator16, goToLineToolStripMenuItem, uppercaseToolStripMenuItem, lowercaseToolStripMenuItem, toolStripSeparator23, addColumnReferenceToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // cutToolStripMenuItem
            // 
            resources.ApplyResources(cutToolStripMenuItem, "cutToolStripMenuItem");
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.Click += CutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            resources.ApplyResources(copyToolStripMenuItem, "copyToolStripMenuItem");
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            resources.ApplyResources(pasteToolStripMenuItem, "pasteToolStripMenuItem");
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator27
            // 
            toolStripSeparator27.Name = "toolStripSeparator27";
            resources.ApplyResources(toolStripSeparator27, "toolStripSeparator27");
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            resources.ApplyResources(undoToolStripMenuItem, "undoToolStripMenuItem");
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            resources.ApplyResources(redoToolStripMenuItem, "redoToolStripMenuItem");
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(toolStripSeparator8, "toolStripSeparator8");
            // 
            // quickFindToolStripMenuItem
            // 
            quickFindToolStripMenuItem.Name = "quickFindToolStripMenuItem";
            resources.ApplyResources(quickFindToolStripMenuItem, "quickFindToolStripMenuItem");
            quickFindToolStripMenuItem.Click += QuickFindToolStripMenuItem_Click;
            // 
            // findToolStripMenuItem
            // 
            findToolStripMenuItem.Image = Properties.Resources.search;
            findToolStripMenuItem.Name = "findToolStripMenuItem";
            resources.ApplyResources(findToolStripMenuItem, "findToolStripMenuItem");
            findToolStripMenuItem.Click += FindDialogToolStripMenuItem_Click;
            // 
            // findAndReplaceToolStripMenuItem
            // 
            findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            resources.ApplyResources(findAndReplaceToolStripMenuItem, "findAndReplaceToolStripMenuItem");
            findAndReplaceToolStripMenuItem.Click += FindAndReplaceToolStripMenuItem_Click;
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(toolStripSeparator16, "toolStripSeparator16");
            // 
            // goToLineToolStripMenuItem
            // 
            goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            resources.ApplyResources(goToLineToolStripMenuItem, "goToLineToolStripMenuItem");
            goToLineToolStripMenuItem.Click += GoToLineToolStripMenuItem_Click;
            // 
            // uppercaseToolStripMenuItem
            // 
            uppercaseToolStripMenuItem.Name = "uppercaseToolStripMenuItem";
            resources.ApplyResources(uppercaseToolStripMenuItem, "uppercaseToolStripMenuItem");
            uppercaseToolStripMenuItem.Click += UppercaseSelectionToolStripMenuItem_Click;
            // 
            // lowercaseToolStripMenuItem
            // 
            lowercaseToolStripMenuItem.Name = "lowercaseToolStripMenuItem";
            resources.ApplyResources(lowercaseToolStripMenuItem, "lowercaseToolStripMenuItem");
            lowercaseToolStripMenuItem.Click += LowercaseSelectionToolStripMenuItem_Click;
            // 
            // toolStripSeparator23
            // 
            toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(toolStripSeparator23, "toolStripSeparator23");
            // 
            // addColumnReferenceToolStripMenuItem
            // 
            addColumnReferenceToolStripMenuItem.Name = "addColumnReferenceToolStripMenuItem";
            resources.ApplyResources(addColumnReferenceToolStripMenuItem, "addColumnReferenceToolStripMenuItem");
            addColumnReferenceToolStripMenuItem.Click += AddColumnReferenceToolStripMenuItem_Click;
            // 
            // builderToolStripMenuItem
            // 
            builderToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mdObjectListToolStripMenuItem, toolStripSeparator7, mdDefinitionToolStripMenuItem, mdValuesToolStripMenuItem, toolStripSeparator17, mdClipboardToTableToolStripMenuItem, mdQueryDataToTableToolStripMenuItem });
            builderToolStripMenuItem.Name = "builderToolStripMenuItem";
            resources.ApplyResources(builderToolStripMenuItem, "builderToolStripMenuItem");
            // 
            // mdObjectListToolStripMenuItem
            // 
            mdObjectListToolStripMenuItem.Name = "mdObjectListToolStripMenuItem";
            resources.ApplyResources(mdObjectListToolStripMenuItem, "mdObjectListToolStripMenuItem");
            mdObjectListToolStripMenuItem.Click += TableListToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(toolStripSeparator7, "toolStripSeparator7");
            // 
            // mdDefinitionToolStripMenuItem
            // 
            mdDefinitionToolStripMenuItem.Name = "mdDefinitionToolStripMenuItem";
            resources.ApplyResources(mdDefinitionToolStripMenuItem, "mdDefinitionToolStripMenuItem");
            mdDefinitionToolStripMenuItem.Click += TableDefinitionToolStripMenuItem_Click_1;
            // 
            // mdValuesToolStripMenuItem
            // 
            mdValuesToolStripMenuItem.Name = "mdValuesToolStripMenuItem";
            resources.ApplyResources(mdValuesToolStripMenuItem, "mdValuesToolStripMenuItem");
            mdValuesToolStripMenuItem.Click += TableValuesMDToolStripMenuItem_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(toolStripSeparator17, "toolStripSeparator17");
            // 
            // mdClipboardToTableToolStripMenuItem
            // 
            mdClipboardToTableToolStripMenuItem.Name = "mdClipboardToTableToolStripMenuItem";
            resources.ApplyResources(mdClipboardToTableToolStripMenuItem, "mdClipboardToTableToolStripMenuItem");
            mdClipboardToTableToolStripMenuItem.Click += ClipboardToTableToolStripMenuItem1_Click;
            // 
            // mdQueryDataToTableToolStripMenuItem
            // 
            mdQueryDataToTableToolStripMenuItem.Name = "mdQueryDataToTableToolStripMenuItem";
            resources.ApplyResources(mdQueryDataToTableToolStripMenuItem, "mdQueryDataToTableToolStripMenuItem");
            mdQueryDataToTableToolStripMenuItem.Click += QueryDataToTableToolStripMenuItem1_Click;
            // 
            // jSONToolStripMenuItem
            // 
            jSONToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { objectListToolStripMenuItem, toolStripSeparator25, jsonObjectDefinitionToolStripMenuItem, jsonTableViewValuesToolStripMenuItem, toolStripSeparator26, jsonClipboardToTableToolStripMenuItem, jsonQueryDataToTableToolStripMenuItem });
            jSONToolStripMenuItem.Name = "jSONToolStripMenuItem";
            resources.ApplyResources(jSONToolStripMenuItem, "jSONToolStripMenuItem");
            // 
            // objectListToolStripMenuItem
            // 
            objectListToolStripMenuItem.Name = "objectListToolStripMenuItem";
            resources.ApplyResources(objectListToolStripMenuItem, "objectListToolStripMenuItem");
            objectListToolStripMenuItem.Click += JsonObjectListToolStripMenuItem_Click;
            // 
            // toolStripSeparator25
            // 
            toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(toolStripSeparator25, "toolStripSeparator25");
            // 
            // jsonObjectDefinitionToolStripMenuItem
            // 
            jsonObjectDefinitionToolStripMenuItem.Name = "jsonObjectDefinitionToolStripMenuItem";
            resources.ApplyResources(jsonObjectDefinitionToolStripMenuItem, "jsonObjectDefinitionToolStripMenuItem");
            jsonObjectDefinitionToolStripMenuItem.Click += JsonObjectDefinitionToolStripMenuItem_Click;
            // 
            // jsonTableViewValuesToolStripMenuItem
            // 
            jsonTableViewValuesToolStripMenuItem.Name = "jsonTableViewValuesToolStripMenuItem";
            resources.ApplyResources(jsonTableViewValuesToolStripMenuItem, "jsonTableViewValuesToolStripMenuItem");
            jsonTableViewValuesToolStripMenuItem.Click += JsonTableViewValuesToolStripMenuItem_Click;
            // 
            // toolStripSeparator26
            // 
            toolStripSeparator26.Name = "toolStripSeparator26";
            resources.ApplyResources(toolStripSeparator26, "toolStripSeparator26");
            // 
            // jsonClipboardToTableToolStripMenuItem
            // 
            jsonClipboardToTableToolStripMenuItem.Name = "jsonClipboardToTableToolStripMenuItem";
            resources.ApplyResources(jsonClipboardToTableToolStripMenuItem, "jsonClipboardToTableToolStripMenuItem");
            jsonClipboardToTableToolStripMenuItem.Click += JsonClipboardToTableToolStripMenuItem_Click;
            // 
            // jsonQueryDataToTableToolStripMenuItem
            // 
            jsonQueryDataToTableToolStripMenuItem.Name = "jsonQueryDataToTableToolStripMenuItem";
            resources.ApplyResources(jsonQueryDataToTableToolStripMenuItem, "jsonQueryDataToTableToolStripMenuItem");
            jsonQueryDataToTableToolStripMenuItem.Click += JsonQueryDataToTableToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { customizeToolStripMenuItem, optionsToolStripMenuItem, toolStripSeparator1, batchColumnDescToolStripMenuItem, toolStripSeparator10, exportDescriptionsToolStripMenuItem, importDescriptionsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // customizeToolStripMenuItem
            // 
            customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            resources.ApplyResources(customizeToolStripMenuItem, "customizeToolStripMenuItem");
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // batchColumnDescToolStripMenuItem
            // 
            batchColumnDescToolStripMenuItem.Name = "batchColumnDescToolStripMenuItem";
            resources.ApplyResources(batchColumnDescToolStripMenuItem, "batchColumnDescToolStripMenuItem");
            batchColumnDescToolStripMenuItem.Click += BatchToolStripButton_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(toolStripSeparator10, "toolStripSeparator10");
            // 
            // exportDescriptionsToolStripMenuItem
            // 
            exportDescriptionsToolStripMenuItem.Name = "exportDescriptionsToolStripMenuItem";
            resources.ApplyResources(exportDescriptionsToolStripMenuItem, "exportDescriptionsToolStripMenuItem");
            exportDescriptionsToolStripMenuItem.Click += ExportDescriptionsToolStripMenuItem_Click;
            // 
            // importDescriptionsToolStripMenuItem
            // 
            importDescriptionsToolStripMenuItem.Name = "importDescriptionsToolStripMenuItem";
            resources.ApplyResources(importDescriptionsToolStripMenuItem, "importDescriptionsToolStripMenuItem");
            importDescriptionsToolStripMenuItem.Click += ImportDescriptionsToolStripMenuItem_Click;
            // 
            // databaseDDLToolStripMenuItem
            // 
            databaseDDLToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cREATEToolStripMenuItem1, insertToolStripMenuItem, tableDescriptionToolStripMenuItem, toolStripSeparator13, createIndexToolStripMenuItem, createPrimaryKeyToolStripMenuItem, toolStripSeparator14, uspToolStripMenuItem, toolStripSeparator12, queryInsertToolStripMenuItem, excelToINSERTToolStripMenuItem, toolStripSeparator15, batchToolStripMenuItem });
            databaseDDLToolStripMenuItem.Name = "databaseDDLToolStripMenuItem";
            resources.ApplyResources(databaseDDLToolStripMenuItem, "databaseDDLToolStripMenuItem");
            // 
            // cREATEToolStripMenuItem1
            // 
            cREATEToolStripMenuItem1.Name = "cREATEToolStripMenuItem1";
            resources.ApplyResources(cREATEToolStripMenuItem1, "cREATEToolStripMenuItem1");
            cREATEToolStripMenuItem1.Click += CreateTableToolStripButton_Click;
            // 
            // insertToolStripMenuItem
            // 
            insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            resources.ApplyResources(insertToolStripMenuItem, "insertToolStripMenuItem");
            insertToolStripMenuItem.Click += InsertToolStripButton_Click;
            // 
            // tableDescriptionToolStripMenuItem
            // 
            tableDescriptionToolStripMenuItem.Name = "tableDescriptionToolStripMenuItem";
            resources.ApplyResources(tableDescriptionToolStripMenuItem, "tableDescriptionToolStripMenuItem");
            tableDescriptionToolStripMenuItem.Click += TableDescriptionToolStripMenuItem_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(toolStripSeparator13, "toolStripSeparator13");
            // 
            // createIndexToolStripMenuItem
            // 
            createIndexToolStripMenuItem.Name = "createIndexToolStripMenuItem";
            resources.ApplyResources(createIndexToolStripMenuItem, "createIndexToolStripMenuItem");
            createIndexToolStripMenuItem.Click += CreateIndexToolStripMenuItem_Click;
            // 
            // createPrimaryKeyToolStripMenuItem
            // 
            createPrimaryKeyToolStripMenuItem.Name = "createPrimaryKeyToolStripMenuItem";
            resources.ApplyResources(createPrimaryKeyToolStripMenuItem, "createPrimaryKeyToolStripMenuItem");
            createPrimaryKeyToolStripMenuItem.Click += CreatePrimaryKeyToolStripMenuItem_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(toolStripSeparator14, "toolStripSeparator14");
            // 
            // uspToolStripMenuItem
            // 
            uspToolStripMenuItem.Name = "uspToolStripMenuItem";
            resources.ApplyResources(uspToolStripMenuItem, "uspToolStripMenuItem");
            uspToolStripMenuItem.Click += UspToolStripMenuItem_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(toolStripSeparator12, "toolStripSeparator12");
            // 
            // queryInsertToolStripMenuItem
            // 
            queryInsertToolStripMenuItem.Name = "queryInsertToolStripMenuItem";
            resources.ApplyResources(queryInsertToolStripMenuItem, "queryInsertToolStripMenuItem");
            queryInsertToolStripMenuItem.Click += QueryInsertToolStripMenuItem_Click;
            // 
            // excelToINSERTToolStripMenuItem
            // 
            excelToINSERTToolStripMenuItem.Name = "excelToINSERTToolStripMenuItem";
            resources.ApplyResources(excelToINSERTToolStripMenuItem, "excelToINSERTToolStripMenuItem");
            excelToINSERTToolStripMenuItem.Click += ExcelToINSERTToolStripMenuItem_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(toolStripSeparator15, "toolStripSeparator15");
            // 
            // batchToolStripMenuItem
            // 
            batchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cREATEToolStripMenuItem, cREATEINSERTToolStripMenuItem, objectsDescriptionToolStripMenuItem });
            batchToolStripMenuItem.Name = "batchToolStripMenuItem";
            resources.ApplyResources(batchToolStripMenuItem, "batchToolStripMenuItem");
            // 
            // cREATEToolStripMenuItem
            // 
            cREATEToolStripMenuItem.Name = "cREATEToolStripMenuItem";
            resources.ApplyResources(cREATEToolStripMenuItem, "cREATEToolStripMenuItem");
            cREATEToolStripMenuItem.Click += CreateToolStripMenuItem_Click;
            // 
            // cREATEINSERTToolStripMenuItem
            // 
            cREATEINSERTToolStripMenuItem.Name = "cREATEINSERTToolStripMenuItem";
            resources.ApplyResources(cREATEINSERTToolStripMenuItem, "cREATEINSERTToolStripMenuItem");
            cREATEINSERTToolStripMenuItem.Click += CreateInsertToolStripMenuItem_Click;
            // 
            // objectsDescriptionToolStripMenuItem
            // 
            objectsDescriptionToolStripMenuItem.Name = "objectsDescriptionToolStripMenuItem";
            resources.ApplyResources(objectsDescriptionToolStripMenuItem, "objectsDescriptionToolStripMenuItem");
            objectsDescriptionToolStripMenuItem.Click += ObjectsDescriptionToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { zoomInToolStripMenuItem, zoomOutToolStripMenuItem, zoom100ToolStripMenuItem, toolStripSeparator18, collapseAllToolStripMenuItem, expandAllToolStripMenuItem, toolStripSeparator19, showIndentGuidesToolStripMenuItem, showWhitespaceToolStripMenuItem, toolStripSeparator11, darkModeToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // zoomInToolStripMenuItem
            // 
            zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            resources.ApplyResources(zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            zoomInToolStripMenuItem.Click += ZoomInToolStripMenuItem_Click;
            // 
            // zoomOutToolStripMenuItem
            // 
            zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            resources.ApplyResources(zoomOutToolStripMenuItem, "zoomOutToolStripMenuItem");
            zoomOutToolStripMenuItem.Click += ZoomOutToolStripMenuItem_Click;
            // 
            // zoom100ToolStripMenuItem
            // 
            zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
            resources.ApplyResources(zoom100ToolStripMenuItem, "zoom100ToolStripMenuItem");
            zoom100ToolStripMenuItem.Click += Zoom100ToolStripMenuItem_Click;
            // 
            // toolStripSeparator18
            // 
            toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(toolStripSeparator18, "toolStripSeparator18");
            // 
            // collapseAllToolStripMenuItem
            // 
            collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            resources.ApplyResources(collapseAllToolStripMenuItem, "collapseAllToolStripMenuItem");
            collapseAllToolStripMenuItem.Click += CollapseAllToolStripMenuItem_Click;
            // 
            // expandAllToolStripMenuItem
            // 
            expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            resources.ApplyResources(expandAllToolStripMenuItem, "expandAllToolStripMenuItem");
            expandAllToolStripMenuItem.Click += ExpandAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator19
            // 
            toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(toolStripSeparator19, "toolStripSeparator19");
            // 
            // showIndentGuidesToolStripMenuItem
            // 
            showIndentGuidesToolStripMenuItem.Checked = true;
            showIndentGuidesToolStripMenuItem.CheckState = CheckState.Checked;
            showIndentGuidesToolStripMenuItem.Name = "showIndentGuidesToolStripMenuItem";
            resources.ApplyResources(showIndentGuidesToolStripMenuItem, "showIndentGuidesToolStripMenuItem");
            showIndentGuidesToolStripMenuItem.Click += IndentGuidesToolStripMenuItem_Click;
            // 
            // showWhitespaceToolStripMenuItem
            // 
            showWhitespaceToolStripMenuItem.Name = "showWhitespaceToolStripMenuItem";
            resources.ApplyResources(showWhitespaceToolStripMenuItem, "showWhitespaceToolStripMenuItem");
            showWhitespaceToolStripMenuItem.Click += HiddenCharactersToolStripMenuItem_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(toolStripSeparator11, "toolStripSeparator11");
            // 
            // darkModeToolStripMenuItem
            // 
            darkModeToolStripMenuItem.Name = "darkModeToolStripMenuItem";
            resources.ApplyResources(darkModeToolStripMenuItem, "darkModeToolStripMenuItem");
            darkModeToolStripMenuItem.Click += DarkModeToolStripMenuItem_Click;
            // 
            // aIAssistantToolStripMenuItem
            // 
            aIAssistantToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aIDescriptionAssistantToolStripMenuItem, descriptionAssistantPlusToolStripMenuItem, describeMissingToolStripMenuItem, matchDescribeToolStripMenuItem, toolStripSeparator22, optimizeCodeToolStripMenuItem, modifyCodeToolStripMenuItem, toolStripSeparator24, settingsToolStripMenuItem });
            aIAssistantToolStripMenuItem.Name = "aIAssistantToolStripMenuItem";
            resources.ApplyResources(aIAssistantToolStripMenuItem, "aIAssistantToolStripMenuItem");
            // 
            // aIDescriptionAssistantToolStripMenuItem
            // 
            aIDescriptionAssistantToolStripMenuItem.Image = Properties.Resources.ai_24;
            aIDescriptionAssistantToolStripMenuItem.Name = "aIDescriptionAssistantToolStripMenuItem";
            resources.ApplyResources(aIDescriptionAssistantToolStripMenuItem, "aIDescriptionAssistantToolStripMenuItem");
            aIDescriptionAssistantToolStripMenuItem.Click += AIDescriptionAssistantToolStripMenuItem_Click;
            // 
            // descriptionAssistantPlusToolStripMenuItem
            // 
            descriptionAssistantPlusToolStripMenuItem.Name = "descriptionAssistantPlusToolStripMenuItem";
            resources.ApplyResources(descriptionAssistantPlusToolStripMenuItem, "descriptionAssistantPlusToolStripMenuItem");
            descriptionAssistantPlusToolStripMenuItem.Click += DescriptionAssistantPlusToolStripMenuItem_Click;
            // 
            // describeMissingToolStripMenuItem
            // 
            describeMissingToolStripMenuItem.Name = "describeMissingToolStripMenuItem";
            resources.ApplyResources(describeMissingToolStripMenuItem, "describeMissingToolStripMenuItem");
            describeMissingToolStripMenuItem.Click += DescribeMissingToolStripMenuItem_Click;
            // 
            // matchDescribeToolStripMenuItem
            // 
            matchDescribeToolStripMenuItem.Name = "matchDescribeToolStripMenuItem";
            resources.ApplyResources(matchDescribeToolStripMenuItem, "matchDescribeToolStripMenuItem");
            matchDescribeToolStripMenuItem.Click += BatchDescribeToolStripMenuItem_Click;
            // 
            // toolStripSeparator22
            // 
            toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(toolStripSeparator22, "toolStripSeparator22");
            // 
            // optimizeCodeToolStripMenuItem
            // 
            optimizeCodeToolStripMenuItem.Name = "optimizeCodeToolStripMenuItem";
            resources.ApplyResources(optimizeCodeToolStripMenuItem, "optimizeCodeToolStripMenuItem");
            optimizeCodeToolStripMenuItem.Click += OptimizeCodeToolStripMenuItem_Click;
            // 
            // modifyCodeToolStripMenuItem
            // 
            modifyCodeToolStripMenuItem.Name = "modifyCodeToolStripMenuItem";
            resources.ApplyResources(modifyCodeToolStripMenuItem, "modifyCodeToolStripMenuItem");
            modifyCodeToolStripMenuItem.Click += ModifyCodeToolStripMenuItem_Click;
            // 
            // toolStripSeparator24
            // 
            toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(toolStripSeparator24, "toolStripSeparator24");
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(settingsToolStripMenuItem, "settingsToolStripMenuItem");
            settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;
            // 
            // executeToolStripMenuItem
            // 
            executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            resources.ApplyResources(executeToolStripMenuItem, "executeToolStripMenuItem");
            executeToolStripMenuItem.Click += OnExecuteToolStripMenuItem_Click;
            // 
            // windowsToolStripMenuItem
            // 
            windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            resources.ApplyResources(windowsToolStripMenuItem, "windowsToolStripMenuItem");
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, toolStripSeparator9, sherlockSoftwareToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(toolStripSeparator9, "toolStripSeparator9");
            // 
            // sherlockSoftwareToolStripMenuItem
            // 
            sherlockSoftwareToolStripMenuItem.Name = "sherlockSoftwareToolStripMenuItem";
            resources.ApplyResources(sherlockSoftwareToolStripMenuItem, "sherlockSoftwareToolStripMenuItem");
            sherlockSoftwareToolStripMenuItem.Click += SherlockSoftwareToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { progressBar, messageLabel, serverToolStripStatusLabel, databaseToolStripStatusLabel });
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.Name = "statusStrip1";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Style = ProgressBarStyle.Continuous;
            // 
            // messageLabel
            // 
            messageLabel.Name = "messageLabel";
            resources.ApplyResources(messageLabel, "messageLabel");
            messageLabel.Spring = true;
            // 
            // serverToolStripStatusLabel
            // 
            serverToolStripStatusLabel.Image = Properties.Resources.server;
            serverToolStripStatusLabel.Name = "serverToolStripStatusLabel";
            resources.ApplyResources(serverToolStripStatusLabel, "serverToolStripStatusLabel");
            // 
            // databaseToolStripStatusLabel
            // 
            databaseToolStripStatusLabel.Image = Properties.Resources.database;
            databaseToolStripStatusLabel.Name = "databaseToolStripStatusLabel";
            resources.ApplyResources(databaseToolStripStatusLabel, "databaseToolStripStatusLabel");
            // 
            // searchPanel
            // 
            searchPanel.Controls.Add(closeSearchButton);
            searchPanel.Controls.Add(nextSearchButton);
            searchPanel.Controls.Add(prevSearchButton);
            searchPanel.Controls.Add(searchSQLTextBox);
            resources.ApplyResources(searchPanel, "searchPanel");
            searchPanel.Name = "searchPanel";
            // 
            // closeSearchButton
            // 
            resources.ApplyResources(closeSearchButton, "closeSearchButton");
            closeSearchButton.BackColor = System.Drawing.Color.White;
            closeSearchButton.ForeColor = System.Drawing.Color.Black;
            closeSearchButton.Name = "closeSearchButton";
            closeSearchButton.UseVisualStyleBackColor = false;
            closeSearchButton.Click += CloseQuickSearch_Click;
            // 
            // nextSearchButton
            // 
            nextSearchButton.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(nextSearchButton, "nextSearchButton");
            nextSearchButton.ForeColor = System.Drawing.SystemColors.ControlText;
            nextSearchButton.Name = "nextSearchButton";
            nextSearchButton.Tag = "Find next (Enter)";
            nextSearchButton.UseVisualStyleBackColor = false;
            nextSearchButton.Click += SearchNext_Click;
            // 
            // prevSearchButton
            // 
            prevSearchButton.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(prevSearchButton, "prevSearchButton");
            prevSearchButton.ForeColor = System.Drawing.SystemColors.ControlText;
            prevSearchButton.Name = "prevSearchButton";
            prevSearchButton.Tag = "Find previous (Shift+Enter)";
            prevSearchButton.UseVisualStyleBackColor = false;
            prevSearchButton.Click += SearchPrevious_Click;
            // 
            // searchSQLTextBox
            // 
            resources.ApplyResources(searchSQLTextBox, "searchSQLTextBox");
            searchSQLTextBox.Name = "searchSQLTextBox";
            searchSQLTextBox.TextChanged += TxtSearch_TextChanged;
            searchSQLTextBox.KeyDown += SearchTextBox_KeyDown;
            // 
            // definitionPanel
            // 
            resources.ApplyResources(definitionPanel, "definitionPanel");
            definitionPanel.Name = "definitionPanel";
            toolTip1.SetToolTip(definitionPanel, resources.GetString("definitionPanel.ToolTip"));
            definitionPanel.AddIndexRequested += CreateIndexToolStripMenuItem_Click;
            definitionPanel.AddPrimaryKeyRequested += CreatePrimaryKeyToolStripMenuItem_Click;
            definitionPanel.AIProcessingCompleted += DefinitionPanel_AIProcessingCompleted;
            definitionPanel.AIProcessingStarted += DefinitionPanel_AIProcessingStarted;
            // 
            // defiCollapsibleSplitter
            // 
            defiCollapsibleSplitter.AnimationDelay = 2;
            defiCollapsibleSplitter.AnimationStep = 100;
            defiCollapsibleSplitter.BorderStyle3D = Border3DStyle.Flat;
            defiCollapsibleSplitter.ControlToHide = definitionPanel;
            defiCollapsibleSplitter.Cursor = Cursors.VSplit;
            resources.ApplyResources(defiCollapsibleSplitter, "defiCollapsibleSplitter");
            defiCollapsibleSplitter.ExpandParentForm = false;
            defiCollapsibleSplitter.Name = "defiCollapsibleSplitter";
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
            resources.ApplyResources(splitContainer1, "splitContainer1");
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
            // 
            // replacePanel
            // 
            replacePanel.Controls.Add(button1);
            replacePanel.Controls.Add(replaceAllButton);
            replacePanel.Controls.Add(replaceButton);
            replacePanel.Controls.Add(findNextButton);
            replacePanel.Controls.Add(replaceReplaceTextBox);
            replacePanel.Controls.Add(replaceSearchTextBox);
            resources.ApplyResources(replacePanel, "replacePanel");
            replacePanel.Name = "replacePanel";
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.BackColor = System.Drawing.Color.White;
            button1.ForeColor = System.Drawing.Color.Black;
            button1.Name = "button1";
            toolTip1.SetToolTip(button1, resources.GetString("button1.ToolTip"));
            button1.UseVisualStyleBackColor = false;
            button1.Click += CloseQuickSearch_Click;
            // 
            // replaceAllButton
            // 
            replaceAllButton.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(replaceAllButton, "replaceAllButton");
            replaceAllButton.ForeColor = System.Drawing.SystemColors.ControlText;
            replaceAllButton.Image = Properties.Resources.replaceAll;
            replaceAllButton.Name = "replaceAllButton";
            replaceAllButton.Tag = "Find previous (Shift+Enter)";
            toolTip1.SetToolTip(replaceAllButton, resources.GetString("replaceAllButton.ToolTip"));
            replaceAllButton.UseVisualStyleBackColor = false;
            replaceAllButton.Click += ReplaceAllButton_Click;
            // 
            // replaceButton
            // 
            replaceButton.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(replaceButton, "replaceButton");
            replaceButton.ForeColor = System.Drawing.SystemColors.ControlText;
            replaceButton.Image = Properties.Resources.replace;
            replaceButton.Name = "replaceButton";
            replaceButton.Tag = "Find previous (Shift+Enter)";
            toolTip1.SetToolTip(replaceButton, resources.GetString("replaceButton.ToolTip"));
            replaceButton.UseVisualStyleBackColor = false;
            replaceButton.Click += ReplaceButton_Click;
            // 
            // findNextButton
            // 
            findNextButton.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(findNextButton, "findNextButton");
            findNextButton.ForeColor = System.Drawing.SystemColors.ControlText;
            findNextButton.Name = "findNextButton";
            findNextButton.Tag = "Find previous (Shift+Enter)";
            toolTip1.SetToolTip(findNextButton, resources.GetString("findNextButton.ToolTip"));
            findNextButton.UseVisualStyleBackColor = false;
            findNextButton.Click += FindNextButton_Click;
            // 
            // replaceReplaceTextBox
            // 
            resources.ApplyResources(replaceReplaceTextBox, "replaceReplaceTextBox");
            replaceReplaceTextBox.Name = "replaceReplaceTextBox";
            replaceReplaceTextBox.KeyDown += ReplaceReplaceTextBox_KeyDown;
            // 
            // replaceSearchTextBox
            // 
            resources.ApplyResources(replaceSearchTextBox, "replaceSearchTextBox");
            replaceSearchTextBox.Name = "replaceSearchTextBox";
            replaceSearchTextBox.TextChanged += TxtSearch_TextChanged;
            replaceSearchTextBox.KeyDown += SearchTextBox_KeyDown;
            // 
            // tabControl1
            // 
            tabControl1.DarkMode = false;
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
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
            resources.ApplyResources(tabContextMenuStrip, "tabContextMenuStrip");
            // 
            // tabAliasToolStripMenuItem
            // 
            tabAliasToolStripMenuItem.Name = "tabAliasToolStripMenuItem";
            resources.ApplyResources(tabAliasToolStripMenuItem, "tabAliasToolStripMenuItem");
            tabAliasToolStripMenuItem.Click += TabAliasToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            resources.ApplyResources(closeToolStripMenuItem, "closeToolStripMenuItem");
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem1
            // 
            saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            resources.ApplyResources(saveToolStripMenuItem1, "saveToolStripMenuItem1");
            saveToolStripMenuItem1.Click += SaveToolStripMenuItem1_Click;
            // 
            // saveAsToolStripMenuItem1
            // 
            saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            resources.ApplyResources(saveAsToolStripMenuItem1, "saveAsToolStripMenuItem1");
            saveAsToolStripMenuItem1.Click += SaveAsToolStripMenuItem_Click;
            // 
            // openFolderInFileExplorerToolStripMenuItem
            // 
            openFolderInFileExplorerToolStripMenuItem.Name = "openFolderInFileExplorerToolStripMenuItem";
            resources.ApplyResources(openFolderInFileExplorerToolStripMenuItem, "openFolderInFileExplorerToolStripMenuItem");
            openFolderInFileExplorerToolStripMenuItem.Click += OpenFolderInFileExplorerToolStripMenuItem_Click;
            // 
            // startTimer
            // 
            startTimer.Tick += StartTimer_Tick;
            // 
            // waitTimer
            // 
            waitTimer.Interval = 200;
            waitTimer.Tick += WaitTimer_Tick;
            // 
            // TableBuilderForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Controls.Add(defiCollapsibleSplitter);
            Controls.Add(definitionPanel);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "TableBuilderForm";
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
        private System.Windows.Forms.ToolStripStatusLabel messageLabel;
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
        private ToolStripMenuItem manageRecentFilesToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem aIDescriptionAssistantToolStripMenuItem;
        private ToolStripMenuItem addColumnReferenceToolStripMenuItem;
        private ToolStripMenuItem aIAssistantToolStripMenuItem;
        private ToolStripMenuItem optimizeCodeToolStripMenuItem;
        private ToolStripMenuItem modifyCodeToolStripMenuItem;
        private Timer waitTimer;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripMenuItem descriptionAssistantPlusToolStripMenuItem;
        private ToolStripMenuItem saveObjectDescriptionsToolStripMenuItem;
        private ToolStripMenuItem matchDescribeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripMenuItem describeMissingToolStripMenuItem;
    }
}