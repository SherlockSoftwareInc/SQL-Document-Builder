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
            resources.ApplyResources(toolStrip1, "toolStrip1");
            toolStrip1.Items.AddRange(new ToolStripItem[] { newToolStripButton1, openToolStripButton, saveToolStripButton, toolStripSeparator, cutToolStripButton, copyToolStripButton, pasteToolStripButton, toolStripSeparator3, toolStripLabel1, dataSourcesToolStripComboBox, toolStripSeparator20, toolStripLabel2, docTypeToolStripComboBox, objDefToolStripButton, mdValuesToolStripButton, toolStripSeparator2, createTableToolStripButton, insertToolStripButton, descToolStripButton });
            toolStrip1.Name = "toolStrip1";
            toolTip1.SetToolTip(toolStrip1, resources.GetString("toolStrip1.ToolTip"));
            // 
            // newToolStripButton1
            // 
            resources.ApplyResources(newToolStripButton1, "newToolStripButton1");
            newToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newToolStripButton1.Name = "newToolStripButton1";
            newToolStripButton1.Click += NewToolStripMenuItem_Click;
            // 
            // openToolStripButton
            // 
            resources.ApplyResources(openToolStripButton, "openToolStripButton");
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = Properties.Resources.openfile1;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Click += OpenToolStripMenuItem_Click;
            // 
            // saveToolStripButton
            // 
            resources.ApplyResources(saveToolStripButton, "saveToolStripButton");
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Click += SaveToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            resources.ApplyResources(toolStripSeparator, "toolStripSeparator");
            toolStripSeparator.Name = "toolStripSeparator";
            // 
            // cutToolStripButton
            // 
            resources.ApplyResources(cutToolStripButton, "cutToolStripButton");
            cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            cutToolStripButton.Name = "cutToolStripButton";
            cutToolStripButton.Click += CutToolStripMenuItem_Click;
            // 
            // copyToolStripButton
            // 
            resources.ApplyResources(copyToolStripButton, "copyToolStripButton");
            copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripButton
            // 
            resources.ApplyResources(pasteToolStripButton, "pasteToolStripButton");
            pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripLabel1
            // 
            resources.ApplyResources(toolStripLabel1, "toolStripLabel1");
            toolStripLabel1.Name = "toolStripLabel1";
            // 
            // dataSourcesToolStripComboBox
            // 
            resources.ApplyResources(dataSourcesToolStripComboBox, "dataSourcesToolStripComboBox");
            dataSourcesToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            dataSourcesToolStripComboBox.Name = "dataSourcesToolStripComboBox";
            dataSourcesToolStripComboBox.SelectedIndexChanged += DataSourcesToolStripComboBox_SelectedIndexChanged;
            // 
            // toolStripSeparator20
            // 
            resources.ApplyResources(toolStripSeparator20, "toolStripSeparator20");
            toolStripSeparator20.Name = "toolStripSeparator20";
            // 
            // toolStripLabel2
            // 
            resources.ApplyResources(toolStripLabel2, "toolStripLabel2");
            toolStripLabel2.Name = "toolStripLabel2";
            // 
            // docTypeToolStripComboBox
            // 
            resources.ApplyResources(docTypeToolStripComboBox, "docTypeToolStripComboBox");
            docTypeToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            docTypeToolStripComboBox.Name = "docTypeToolStripComboBox";
            docTypeToolStripComboBox.SelectedIndexChanged += DocTypeToolStripComboBox_SelectedIndexChanged;
            // 
            // objDefToolStripButton
            // 
            resources.ApplyResources(objDefToolStripButton, "objDefToolStripButton");
            objDefToolStripButton.Image = Properties.Resources.Build_doc;
            objDefToolStripButton.Name = "objDefToolStripButton";
            objDefToolStripButton.Click += TableDefinitionToolStripMenuItem_Click_1;
            // 
            // mdValuesToolStripButton
            // 
            resources.ApplyResources(mdValuesToolStripButton, "mdValuesToolStripButton");
            mdValuesToolStripButton.Image = Properties.Resources.Build_doc;
            mdValuesToolStripButton.Name = "mdValuesToolStripButton";
            mdValuesToolStripButton.Click += TableValuesMDToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // createTableToolStripButton
            // 
            resources.ApplyResources(createTableToolStripButton, "createTableToolStripButton");
            createTableToolStripButton.Image = Properties.Resources.sp_16;
            createTableToolStripButton.Name = "createTableToolStripButton";
            createTableToolStripButton.Click += CreateTableToolStripButton_Click;
            // 
            // insertToolStripButton
            // 
            resources.ApplyResources(insertToolStripButton, "insertToolStripButton");
            insertToolStripButton.Image = Properties.Resources.sp_16;
            insertToolStripButton.Name = "insertToolStripButton";
            insertToolStripButton.Click += InsertToolStripButton_Click;
            // 
            // descToolStripButton
            // 
            resources.ApplyResources(descToolStripButton, "descToolStripButton");
            descToolStripButton.Image = Properties.Resources.sp_16;
            descToolStripButton.Name = "descToolStripButton";
            descToolStripButton.Click += TableDescriptionToolStripMenuItem_Click;
            // 
            // objectsTabControl
            // 
            resources.ApplyResources(objectsTabControl, "objectsTabControl");
            objectsTabControl.Controls.Add(tabPage1);
            objectsTabControl.Controls.Add(tabPage2);
            objectsTabControl.Name = "objectsTabControl";
            objectsTabControl.SelectedIndex = 0;
            toolTip1.SetToolTip(objectsTabControl, resources.GetString("objectsTabControl.ToolTip"));
            // 
            // tabPage1
            // 
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Controls.Add(objectsListBox);
            tabPage1.Controls.Add(panel2);
            tabPage1.Name = "tabPage1";
            toolTip1.SetToolTip(tabPage1, resources.GetString("tabPage1.ToolTip"));
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // objectsListBox
            // 
            resources.ApplyResources(objectsListBox, "objectsListBox");
            objectsListBox.FormattingEnabled = true;
            objectsListBox.Name = "objectsListBox";
            toolTip1.SetToolTip(objectsListBox, resources.GetString("objectsListBox.ToolTip"));
            objectsListBox.SelectedIndexChanged += ObjectsListBox_SelectedIndexChanged;
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Controls.Add(searchButton);
            panel2.Controls.Add(clearSerachButton);
            panel2.Controls.Add(searchTextBox);
            panel2.Controls.Add(schemaLabel);
            panel2.Controls.Add(searchLabel);
            panel2.Controls.Add(objectTypeComboBox);
            panel2.Controls.Add(schemaComboBox);
            panel2.Controls.Add(label1);
            panel2.Name = "panel2";
            toolTip1.SetToolTip(panel2, resources.GetString("panel2.ToolTip"));
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
            toolTip1.SetToolTip(searchTextBox, resources.GetString("searchTextBox.ToolTip"));
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchTextBox.KeyUp += SearchTextBox_KeyUp;
            // 
            // schemaLabel
            // 
            resources.ApplyResources(schemaLabel, "schemaLabel");
            schemaLabel.Name = "schemaLabel";
            toolTip1.SetToolTip(schemaLabel, resources.GetString("schemaLabel.ToolTip"));
            // 
            // searchLabel
            // 
            resources.ApplyResources(searchLabel, "searchLabel");
            searchLabel.Name = "searchLabel";
            toolTip1.SetToolTip(searchLabel, resources.GetString("searchLabel.ToolTip"));
            // 
            // objectTypeComboBox
            // 
            resources.ApplyResources(objectTypeComboBox, "objectTypeComboBox");
            objectTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            objectTypeComboBox.FormattingEnabled = true;
            objectTypeComboBox.Name = "objectTypeComboBox";
            toolTip1.SetToolTip(objectTypeComboBox, resources.GetString("objectTypeComboBox.ToolTip"));
            objectTypeComboBox.SelectedIndexChanged += ObjectTypeComboBox_SelectedIndexChanged;
            // 
            // schemaComboBox
            // 
            resources.ApplyResources(schemaComboBox, "schemaComboBox");
            schemaComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            schemaComboBox.FormattingEnabled = true;
            schemaComboBox.Name = "schemaComboBox";
            toolTip1.SetToolTip(schemaComboBox, resources.GetString("schemaComboBox.ToolTip"));
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            toolTip1.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // tabPage2
            // 
            resources.ApplyResources(tabPage2, "tabPage2");
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
            tabPage2.Name = "tabPage2";
            toolTip1.SetToolTip(tabPage2, resources.GetString("tabPage2.ToolTip"));
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // autoCopyCheckBox
            // 
            resources.ApplyResources(autoCopyCheckBox, "autoCopyCheckBox");
            autoCopyCheckBox.Name = "autoCopyCheckBox";
            toolTip1.SetToolTip(autoCopyCheckBox, resources.GetString("autoCopyCheckBox.ToolTip"));
            autoCopyCheckBox.UseVisualStyleBackColor = true;
            autoCopyCheckBox.CheckedChanged += Options_Changed;
            // 
            // quotedIDCheckBox
            // 
            resources.ApplyResources(quotedIDCheckBox, "quotedIDCheckBox");
            quotedIDCheckBox.Checked = true;
            quotedIDCheckBox.CheckState = CheckState.Checked;
            quotedIDCheckBox.Name = "quotedIDCheckBox";
            toolTip1.SetToolTip(quotedIDCheckBox, resources.GetString("quotedIDCheckBox.ToolTip"));
            quotedIDCheckBox.UseVisualStyleBackColor = true;
            quotedIDCheckBox.CheckedChanged += Options_Changed;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(insertMaxTextBox);
            groupBox2.Controls.Add(insertBatchTextBox);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            toolTip1.SetToolTip(groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            toolTip1.SetToolTip(label2, resources.GetString("label2.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            toolTip1.SetToolTip(label3, resources.GetString("label3.ToolTip"));
            // 
            // insertMaxTextBox
            // 
            resources.ApplyResources(insertMaxTextBox, "insertMaxTextBox");
            insertMaxTextBox.Name = "insertMaxTextBox";
            toolTip1.SetToolTip(insertMaxTextBox, resources.GetString("insertMaxTextBox.ToolTip"));
            insertMaxTextBox.Validating += InsertMaxTextBox_Validating;
            insertMaxTextBox.Validated += InsertMaxTextBox_Validated;
            // 
            // insertBatchTextBox
            // 
            resources.ApplyResources(insertBatchTextBox, "insertBatchTextBox");
            insertBatchTextBox.Name = "insertBatchTextBox";
            toolTip1.SetToolTip(insertBatchTextBox, resources.GetString("insertBatchTextBox.ToolTip"));
            insertBatchTextBox.Validating += InsertBatchTextBox_Validating;
            insertBatchTextBox.Validated += InsertBatchTextBox_Validated;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(useUspDescRadioButton);
            groupBox1.Controls.Add(useExtendedPropertyRadioButton);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            toolTip1.SetToolTip(groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // useUspDescRadioButton
            // 
            resources.ApplyResources(useUspDescRadioButton, "useUspDescRadioButton");
            useUspDescRadioButton.Name = "useUspDescRadioButton";
            useUspDescRadioButton.TabStop = true;
            toolTip1.SetToolTip(useUspDescRadioButton, resources.GetString("useUspDescRadioButton.ToolTip"));
            useUspDescRadioButton.UseVisualStyleBackColor = true;
            useUspDescRadioButton.CheckedChanged += Options_Changed;
            // 
            // useExtendedPropertyRadioButton
            // 
            resources.ApplyResources(useExtendedPropertyRadioButton, "useExtendedPropertyRadioButton");
            useExtendedPropertyRadioButton.Name = "useExtendedPropertyRadioButton";
            useExtendedPropertyRadioButton.TabStop = true;
            toolTip1.SetToolTip(useExtendedPropertyRadioButton, resources.GetString("useExtendedPropertyRadioButton.ToolTip"));
            useExtendedPropertyRadioButton.UseVisualStyleBackColor = true;
            useExtendedPropertyRadioButton.CheckedChanged += Options_Changed;
            // 
            // indexesCheckBox
            // 
            resources.ApplyResources(indexesCheckBox, "indexesCheckBox");
            indexesCheckBox.Name = "indexesCheckBox";
            toolTip1.SetToolTip(indexesCheckBox, resources.GetString("indexesCheckBox.ToolTip"));
            indexesCheckBox.UseVisualStyleBackColor = true;
            indexesCheckBox.CheckedChanged += Options_Changed;
            // 
            // includeHeadersCheckBox
            // 
            resources.ApplyResources(includeHeadersCheckBox, "includeHeadersCheckBox");
            includeHeadersCheckBox.Name = "includeHeadersCheckBox";
            toolTip1.SetToolTip(includeHeadersCheckBox, resources.GetString("includeHeadersCheckBox.ToolTip"));
            includeHeadersCheckBox.UseVisualStyleBackColor = true;
            includeHeadersCheckBox.CheckedChanged += Options_Changed;
            // 
            // scriptDataCheckBox
            // 
            resources.ApplyResources(scriptDataCheckBox, "scriptDataCheckBox");
            scriptDataCheckBox.Name = "scriptDataCheckBox";
            toolTip1.SetToolTip(scriptDataCheckBox, resources.GetString("scriptDataCheckBox.ToolTip"));
            scriptDataCheckBox.UseVisualStyleBackColor = true;
            scriptDataCheckBox.CheckedChanged += Options_Changed;
            // 
            // noCollationCheckBox
            // 
            resources.ApplyResources(noCollationCheckBox, "noCollationCheckBox");
            noCollationCheckBox.Checked = true;
            noCollationCheckBox.CheckState = CheckState.Checked;
            noCollationCheckBox.Name = "noCollationCheckBox";
            toolTip1.SetToolTip(noCollationCheckBox, resources.GetString("noCollationCheckBox.ToolTip"));
            noCollationCheckBox.UseVisualStyleBackColor = true;
            noCollationCheckBox.CheckedChanged += Options_Changed;
            // 
            // addDataSourceCheckBox
            // 
            resources.ApplyResources(addDataSourceCheckBox, "addDataSourceCheckBox");
            addDataSourceCheckBox.Checked = true;
            addDataSourceCheckBox.CheckState = CheckState.Checked;
            addDataSourceCheckBox.Name = "addDataSourceCheckBox";
            toolTip1.SetToolTip(addDataSourceCheckBox, resources.GetString("addDataSourceCheckBox.ToolTip"));
            addDataSourceCheckBox.UseVisualStyleBackColor = true;
            addDataSourceCheckBox.CheckedChanged += Options_Changed;
            // 
            // scriptDropsCheckBox
            // 
            resources.ApplyResources(scriptDropsCheckBox, "scriptDropsCheckBox");
            scriptDropsCheckBox.Checked = true;
            scriptDropsCheckBox.CheckState = CheckState.Checked;
            scriptDropsCheckBox.Name = "scriptDropsCheckBox";
            toolTip1.SetToolTip(scriptDropsCheckBox, resources.GetString("scriptDropsCheckBox.ToolTip"));
            scriptDropsCheckBox.UseVisualStyleBackColor = true;
            scriptDropsCheckBox.CheckedChanged += Options_Changed;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, builderToolStripMenuItem, jSONToolStripMenuItem, toolsToolStripMenuItem, databaseDDLToolStripMenuItem, viewToolStripMenuItem, aIAssistantToolStripMenuItem, executeToolStripMenuItem, windowsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Name = "menuStrip1";
            toolTip1.SetToolTip(menuStrip1, resources.GetString("menuStrip1.ToolTip"));
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectToToolStripMenuItem, newConnectionToolStripMenuItem, manageConnectionsToolStripMenuItem, manageTemplateToolStripMenuItem, toolStripSeparator4, newToolStripMenuItem1, openToolStripMenuItem, openFolderToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, saveObjectDescriptionsToolStripMenuItem, closeToolStripMenuItem1, closeAllToolStripMenuItem, toolStripSeparator21, recentToolStripMenuItem, manageRecentFilesToolStripMenuItem, toolStripSeparator5, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // connectToToolStripMenuItem
            // 
            resources.ApplyResources(connectToToolStripMenuItem, "connectToToolStripMenuItem");
            connectToToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { localToolStripMenuItem, azureToolStripMenuItem });
            connectToToolStripMenuItem.Name = "connectToToolStripMenuItem";
            // 
            // localToolStripMenuItem
            // 
            resources.ApplyResources(localToolStripMenuItem, "localToolStripMenuItem");
            localToolStripMenuItem.Name = "localToolStripMenuItem";
            // 
            // azureToolStripMenuItem
            // 
            resources.ApplyResources(azureToolStripMenuItem, "azureToolStripMenuItem");
            azureToolStripMenuItem.Name = "azureToolStripMenuItem";
            // 
            // newConnectionToolStripMenuItem
            // 
            resources.ApplyResources(newConnectionToolStripMenuItem, "newConnectionToolStripMenuItem");
            newConnectionToolStripMenuItem.Image = Properties.Resources.add;
            newConnectionToolStripMenuItem.Name = "newConnectionToolStripMenuItem";
            newConnectionToolStripMenuItem.Click += NewConnectionToolStripMenuItem_Click;
            // 
            // manageConnectionsToolStripMenuItem
            // 
            resources.ApplyResources(manageConnectionsToolStripMenuItem, "manageConnectionsToolStripMenuItem");
            manageConnectionsToolStripMenuItem.Name = "manageConnectionsToolStripMenuItem";
            manageConnectionsToolStripMenuItem.Click += ManageConnectionsToolStripMenuItem_Click;
            // 
            // manageTemplateToolStripMenuItem
            // 
            resources.ApplyResources(manageTemplateToolStripMenuItem, "manageTemplateToolStripMenuItem");
            manageTemplateToolStripMenuItem.Name = "manageTemplateToolStripMenuItem";
            manageTemplateToolStripMenuItem.Click += ManageTemplateToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            toolStripSeparator4.Name = "toolStripSeparator4";
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
            resources.ApplyResources(openFolderToolStripMenuItem, "openFolderToolStripMenuItem");
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
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
            resources.ApplyResources(saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            // 
            // saveObjectDescriptionsToolStripMenuItem
            // 
            resources.ApplyResources(saveObjectDescriptionsToolStripMenuItem, "saveObjectDescriptionsToolStripMenuItem");
            saveObjectDescriptionsToolStripMenuItem.Name = "saveObjectDescriptionsToolStripMenuItem";
            saveObjectDescriptionsToolStripMenuItem.Click += SaveObjectDescriptionsToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem1
            // 
            resources.ApplyResources(closeToolStripMenuItem1, "closeToolStripMenuItem1");
            closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            closeToolStripMenuItem1.Click += CloseToolStripMenuItem1_Click;
            // 
            // closeAllToolStripMenuItem
            // 
            resources.ApplyResources(closeAllToolStripMenuItem, "closeAllToolStripMenuItem");
            closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            closeAllToolStripMenuItem.Click += CloseAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator21
            // 
            resources.ApplyResources(toolStripSeparator21, "toolStripSeparator21");
            toolStripSeparator21.Name = "toolStripSeparator21";
            // 
            // recentToolStripMenuItem
            // 
            resources.ApplyResources(recentToolStripMenuItem, "recentToolStripMenuItem");
            recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            // 
            // manageRecentFilesToolStripMenuItem
            // 
            resources.ApplyResources(manageRecentFilesToolStripMenuItem, "manageRecentFilesToolStripMenuItem");
            manageRecentFilesToolStripMenuItem.Name = "manageRecentFilesToolStripMenuItem";
            manageRecentFilesToolStripMenuItem.Click += ManageRecentFilesToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
            toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Click += CloseToolStripButton_Click;
            // 
            // editToolStripMenuItem
            // 
            resources.ApplyResources(editToolStripMenuItem, "editToolStripMenuItem");
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator27, undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator6, selectAllToolStripMenuItem, toolStripSeparator8, quickFindToolStripMenuItem, findToolStripMenuItem, findAndReplaceToolStripMenuItem, toolStripSeparator16, goToLineToolStripMenuItem, uppercaseToolStripMenuItem, lowercaseToolStripMenuItem, toolStripSeparator23, addColumnReferenceToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
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
            resources.ApplyResources(toolStripSeparator27, "toolStripSeparator27");
            toolStripSeparator27.Name = "toolStripSeparator27";
            // 
            // undoToolStripMenuItem
            // 
            resources.ApplyResources(undoToolStripMenuItem, "undoToolStripMenuItem");
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            resources.ApplyResources(redoToolStripMenuItem, "redoToolStripMenuItem");
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
            toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // selectAllToolStripMenuItem
            // 
            resources.ApplyResources(selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(toolStripSeparator8, "toolStripSeparator8");
            toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // quickFindToolStripMenuItem
            // 
            resources.ApplyResources(quickFindToolStripMenuItem, "quickFindToolStripMenuItem");
            quickFindToolStripMenuItem.Name = "quickFindToolStripMenuItem";
            quickFindToolStripMenuItem.Click += QuickFindToolStripMenuItem_Click;
            // 
            // findToolStripMenuItem
            // 
            resources.ApplyResources(findToolStripMenuItem, "findToolStripMenuItem");
            findToolStripMenuItem.Image = Properties.Resources.search;
            findToolStripMenuItem.Name = "findToolStripMenuItem";
            findToolStripMenuItem.Click += FindDialogToolStripMenuItem_Click;
            // 
            // findAndReplaceToolStripMenuItem
            // 
            resources.ApplyResources(findAndReplaceToolStripMenuItem, "findAndReplaceToolStripMenuItem");
            findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            findAndReplaceToolStripMenuItem.Click += FindAndReplaceToolStripMenuItem_Click;
            // 
            // toolStripSeparator16
            // 
            resources.ApplyResources(toolStripSeparator16, "toolStripSeparator16");
            toolStripSeparator16.Name = "toolStripSeparator16";
            // 
            // goToLineToolStripMenuItem
            // 
            resources.ApplyResources(goToLineToolStripMenuItem, "goToLineToolStripMenuItem");
            goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            goToLineToolStripMenuItem.Click += GoToLineToolStripMenuItem_Click;
            // 
            // uppercaseToolStripMenuItem
            // 
            resources.ApplyResources(uppercaseToolStripMenuItem, "uppercaseToolStripMenuItem");
            uppercaseToolStripMenuItem.Name = "uppercaseToolStripMenuItem";
            uppercaseToolStripMenuItem.Click += UppercaseSelectionToolStripMenuItem_Click;
            // 
            // lowercaseToolStripMenuItem
            // 
            resources.ApplyResources(lowercaseToolStripMenuItem, "lowercaseToolStripMenuItem");
            lowercaseToolStripMenuItem.Name = "lowercaseToolStripMenuItem";
            lowercaseToolStripMenuItem.Click += LowercaseSelectionToolStripMenuItem_Click;
            // 
            // toolStripSeparator23
            // 
            resources.ApplyResources(toolStripSeparator23, "toolStripSeparator23");
            toolStripSeparator23.Name = "toolStripSeparator23";
            // 
            // addColumnReferenceToolStripMenuItem
            // 
            resources.ApplyResources(addColumnReferenceToolStripMenuItem, "addColumnReferenceToolStripMenuItem");
            addColumnReferenceToolStripMenuItem.Name = "addColumnReferenceToolStripMenuItem";
            addColumnReferenceToolStripMenuItem.Click += AddColumnReferenceToolStripMenuItem_Click;
            // 
            // builderToolStripMenuItem
            // 
            resources.ApplyResources(builderToolStripMenuItem, "builderToolStripMenuItem");
            builderToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mdObjectListToolStripMenuItem, toolStripSeparator7, mdDefinitionToolStripMenuItem, mdValuesToolStripMenuItem, toolStripSeparator17, mdClipboardToTableToolStripMenuItem, mdQueryDataToTableToolStripMenuItem });
            builderToolStripMenuItem.Name = "builderToolStripMenuItem";
            // 
            // mdObjectListToolStripMenuItem
            // 
            resources.ApplyResources(mdObjectListToolStripMenuItem, "mdObjectListToolStripMenuItem");
            mdObjectListToolStripMenuItem.Name = "mdObjectListToolStripMenuItem";
            mdObjectListToolStripMenuItem.Click += TableListToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(toolStripSeparator7, "toolStripSeparator7");
            toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // mdDefinitionToolStripMenuItem
            // 
            resources.ApplyResources(mdDefinitionToolStripMenuItem, "mdDefinitionToolStripMenuItem");
            mdDefinitionToolStripMenuItem.Name = "mdDefinitionToolStripMenuItem";
            mdDefinitionToolStripMenuItem.Click += TableDefinitionToolStripMenuItem_Click_1;
            // 
            // mdValuesToolStripMenuItem
            // 
            resources.ApplyResources(mdValuesToolStripMenuItem, "mdValuesToolStripMenuItem");
            mdValuesToolStripMenuItem.Name = "mdValuesToolStripMenuItem";
            mdValuesToolStripMenuItem.Click += TableValuesMDToolStripMenuItem_Click;
            // 
            // toolStripSeparator17
            // 
            resources.ApplyResources(toolStripSeparator17, "toolStripSeparator17");
            toolStripSeparator17.Name = "toolStripSeparator17";
            // 
            // mdClipboardToTableToolStripMenuItem
            // 
            resources.ApplyResources(mdClipboardToTableToolStripMenuItem, "mdClipboardToTableToolStripMenuItem");
            mdClipboardToTableToolStripMenuItem.Name = "mdClipboardToTableToolStripMenuItem";
            mdClipboardToTableToolStripMenuItem.Click += ClipboardToTableToolStripMenuItem1_Click;
            // 
            // mdQueryDataToTableToolStripMenuItem
            // 
            resources.ApplyResources(mdQueryDataToTableToolStripMenuItem, "mdQueryDataToTableToolStripMenuItem");
            mdQueryDataToTableToolStripMenuItem.Name = "mdQueryDataToTableToolStripMenuItem";
            mdQueryDataToTableToolStripMenuItem.Click += QueryDataToTableToolStripMenuItem1_Click;
            // 
            // jSONToolStripMenuItem
            // 
            resources.ApplyResources(jSONToolStripMenuItem, "jSONToolStripMenuItem");
            jSONToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { objectListToolStripMenuItem, toolStripSeparator25, jsonObjectDefinitionToolStripMenuItem, jsonTableViewValuesToolStripMenuItem, toolStripSeparator26, jsonClipboardToTableToolStripMenuItem, jsonQueryDataToTableToolStripMenuItem });
            jSONToolStripMenuItem.Name = "jSONToolStripMenuItem";
            // 
            // objectListToolStripMenuItem
            // 
            resources.ApplyResources(objectListToolStripMenuItem, "objectListToolStripMenuItem");
            objectListToolStripMenuItem.Name = "objectListToolStripMenuItem";
            objectListToolStripMenuItem.Click += JsonObjectListToolStripMenuItem_Click;
            // 
            // toolStripSeparator25
            // 
            resources.ApplyResources(toolStripSeparator25, "toolStripSeparator25");
            toolStripSeparator25.Name = "toolStripSeparator25";
            // 
            // jsonObjectDefinitionToolStripMenuItem
            // 
            resources.ApplyResources(jsonObjectDefinitionToolStripMenuItem, "jsonObjectDefinitionToolStripMenuItem");
            jsonObjectDefinitionToolStripMenuItem.Name = "jsonObjectDefinitionToolStripMenuItem";
            jsonObjectDefinitionToolStripMenuItem.Click += JsonObjectDefinitionToolStripMenuItem_Click;
            // 
            // jsonTableViewValuesToolStripMenuItem
            // 
            resources.ApplyResources(jsonTableViewValuesToolStripMenuItem, "jsonTableViewValuesToolStripMenuItem");
            jsonTableViewValuesToolStripMenuItem.Name = "jsonTableViewValuesToolStripMenuItem";
            jsonTableViewValuesToolStripMenuItem.Click += JsonTableViewValuesToolStripMenuItem_Click;
            // 
            // toolStripSeparator26
            // 
            resources.ApplyResources(toolStripSeparator26, "toolStripSeparator26");
            toolStripSeparator26.Name = "toolStripSeparator26";
            // 
            // jsonClipboardToTableToolStripMenuItem
            // 
            resources.ApplyResources(jsonClipboardToTableToolStripMenuItem, "jsonClipboardToTableToolStripMenuItem");
            jsonClipboardToTableToolStripMenuItem.Name = "jsonClipboardToTableToolStripMenuItem";
            jsonClipboardToTableToolStripMenuItem.Click += JsonClipboardToTableToolStripMenuItem_Click;
            // 
            // jsonQueryDataToTableToolStripMenuItem
            // 
            resources.ApplyResources(jsonQueryDataToTableToolStripMenuItem, "jsonQueryDataToTableToolStripMenuItem");
            jsonQueryDataToTableToolStripMenuItem.Name = "jsonQueryDataToTableToolStripMenuItem";
            jsonQueryDataToTableToolStripMenuItem.Click += JsonQueryDataToTableToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            resources.ApplyResources(toolsToolStripMenuItem, "toolsToolStripMenuItem");
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { customizeToolStripMenuItem, optionsToolStripMenuItem, toolStripSeparator1, batchColumnDescToolStripMenuItem, toolStripSeparator10, exportDescriptionsToolStripMenuItem, importDescriptionsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            // 
            // customizeToolStripMenuItem
            // 
            resources.ApplyResources(customizeToolStripMenuItem, "customizeToolStripMenuItem");
            customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            // 
            // optionsToolStripMenuItem
            // 
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // batchColumnDescToolStripMenuItem
            // 
            resources.ApplyResources(batchColumnDescToolStripMenuItem, "batchColumnDescToolStripMenuItem");
            batchColumnDescToolStripMenuItem.Name = "batchColumnDescToolStripMenuItem";
            batchColumnDescToolStripMenuItem.Click += BatchToolStripButton_Click;
            // 
            // toolStripSeparator10
            // 
            resources.ApplyResources(toolStripSeparator10, "toolStripSeparator10");
            toolStripSeparator10.Name = "toolStripSeparator10";
            // 
            // exportDescriptionsToolStripMenuItem
            // 
            resources.ApplyResources(exportDescriptionsToolStripMenuItem, "exportDescriptionsToolStripMenuItem");
            exportDescriptionsToolStripMenuItem.Name = "exportDescriptionsToolStripMenuItem";
            exportDescriptionsToolStripMenuItem.Click += ExportDescriptionsToolStripMenuItem_Click;
            // 
            // importDescriptionsToolStripMenuItem
            // 
            resources.ApplyResources(importDescriptionsToolStripMenuItem, "importDescriptionsToolStripMenuItem");
            importDescriptionsToolStripMenuItem.Name = "importDescriptionsToolStripMenuItem";
            importDescriptionsToolStripMenuItem.Click += ImportDescriptionsToolStripMenuItem_Click;
            // 
            // databaseDDLToolStripMenuItem
            // 
            resources.ApplyResources(databaseDDLToolStripMenuItem, "databaseDDLToolStripMenuItem");
            databaseDDLToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cREATEToolStripMenuItem1, insertToolStripMenuItem, tableDescriptionToolStripMenuItem, toolStripSeparator13, createIndexToolStripMenuItem, createPrimaryKeyToolStripMenuItem, toolStripSeparator14, uspToolStripMenuItem, toolStripSeparator12, queryInsertToolStripMenuItem, excelToINSERTToolStripMenuItem, toolStripSeparator15, batchToolStripMenuItem });
            databaseDDLToolStripMenuItem.Name = "databaseDDLToolStripMenuItem";
            // 
            // cREATEToolStripMenuItem1
            // 
            resources.ApplyResources(cREATEToolStripMenuItem1, "cREATEToolStripMenuItem1");
            cREATEToolStripMenuItem1.Name = "cREATEToolStripMenuItem1";
            cREATEToolStripMenuItem1.Click += CreateTableToolStripButton_Click;
            // 
            // insertToolStripMenuItem
            // 
            resources.ApplyResources(insertToolStripMenuItem, "insertToolStripMenuItem");
            insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            insertToolStripMenuItem.Click += InsertToolStripButton_Click;
            // 
            // tableDescriptionToolStripMenuItem
            // 
            resources.ApplyResources(tableDescriptionToolStripMenuItem, "tableDescriptionToolStripMenuItem");
            tableDescriptionToolStripMenuItem.Name = "tableDescriptionToolStripMenuItem";
            tableDescriptionToolStripMenuItem.Click += TableDescriptionToolStripMenuItem_Click;
            // 
            // toolStripSeparator13
            // 
            resources.ApplyResources(toolStripSeparator13, "toolStripSeparator13");
            toolStripSeparator13.Name = "toolStripSeparator13";
            // 
            // createIndexToolStripMenuItem
            // 
            resources.ApplyResources(createIndexToolStripMenuItem, "createIndexToolStripMenuItem");
            createIndexToolStripMenuItem.Name = "createIndexToolStripMenuItem";
            createIndexToolStripMenuItem.Click += CreateIndexToolStripMenuItem_Click;
            // 
            // createPrimaryKeyToolStripMenuItem
            // 
            resources.ApplyResources(createPrimaryKeyToolStripMenuItem, "createPrimaryKeyToolStripMenuItem");
            createPrimaryKeyToolStripMenuItem.Name = "createPrimaryKeyToolStripMenuItem";
            createPrimaryKeyToolStripMenuItem.Click += CreatePrimaryKeyToolStripMenuItem_Click;
            // 
            // toolStripSeparator14
            // 
            resources.ApplyResources(toolStripSeparator14, "toolStripSeparator14");
            toolStripSeparator14.Name = "toolStripSeparator14";
            // 
            // uspToolStripMenuItem
            // 
            resources.ApplyResources(uspToolStripMenuItem, "uspToolStripMenuItem");
            uspToolStripMenuItem.Name = "uspToolStripMenuItem";
            uspToolStripMenuItem.Click += UspToolStripMenuItem_Click;
            // 
            // toolStripSeparator12
            // 
            resources.ApplyResources(toolStripSeparator12, "toolStripSeparator12");
            toolStripSeparator12.Name = "toolStripSeparator12";
            // 
            // queryInsertToolStripMenuItem
            // 
            resources.ApplyResources(queryInsertToolStripMenuItem, "queryInsertToolStripMenuItem");
            queryInsertToolStripMenuItem.Name = "queryInsertToolStripMenuItem";
            queryInsertToolStripMenuItem.Click += QueryInsertToolStripMenuItem_Click;
            // 
            // excelToINSERTToolStripMenuItem
            // 
            resources.ApplyResources(excelToINSERTToolStripMenuItem, "excelToINSERTToolStripMenuItem");
            excelToINSERTToolStripMenuItem.Name = "excelToINSERTToolStripMenuItem";
            excelToINSERTToolStripMenuItem.Click += ExcelToINSERTToolStripMenuItem_Click;
            // 
            // toolStripSeparator15
            // 
            resources.ApplyResources(toolStripSeparator15, "toolStripSeparator15");
            toolStripSeparator15.Name = "toolStripSeparator15";
            // 
            // batchToolStripMenuItem
            // 
            resources.ApplyResources(batchToolStripMenuItem, "batchToolStripMenuItem");
            batchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cREATEToolStripMenuItem, cREATEINSERTToolStripMenuItem, objectsDescriptionToolStripMenuItem });
            batchToolStripMenuItem.Name = "batchToolStripMenuItem";
            // 
            // cREATEToolStripMenuItem
            // 
            resources.ApplyResources(cREATEToolStripMenuItem, "cREATEToolStripMenuItem");
            cREATEToolStripMenuItem.Name = "cREATEToolStripMenuItem";
            cREATEToolStripMenuItem.Click += CreateToolStripMenuItem_Click;
            // 
            // cREATEINSERTToolStripMenuItem
            // 
            resources.ApplyResources(cREATEINSERTToolStripMenuItem, "cREATEINSERTToolStripMenuItem");
            cREATEINSERTToolStripMenuItem.Name = "cREATEINSERTToolStripMenuItem";
            cREATEINSERTToolStripMenuItem.Click += CreateInsertToolStripMenuItem_Click;
            // 
            // objectsDescriptionToolStripMenuItem
            // 
            resources.ApplyResources(objectsDescriptionToolStripMenuItem, "objectsDescriptionToolStripMenuItem");
            objectsDescriptionToolStripMenuItem.Name = "objectsDescriptionToolStripMenuItem";
            objectsDescriptionToolStripMenuItem.Click += ObjectsDescriptionToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            resources.ApplyResources(viewToolStripMenuItem, "viewToolStripMenuItem");
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { zoomInToolStripMenuItem, zoomOutToolStripMenuItem, zoom100ToolStripMenuItem, toolStripSeparator18, collapseAllToolStripMenuItem, expandAllToolStripMenuItem, toolStripSeparator19, showIndentGuidesToolStripMenuItem, showWhitespaceToolStripMenuItem, toolStripSeparator11, darkModeToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            // 
            // zoomInToolStripMenuItem
            // 
            resources.ApplyResources(zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            zoomInToolStripMenuItem.Click += ZoomInToolStripMenuItem_Click;
            // 
            // zoomOutToolStripMenuItem
            // 
            resources.ApplyResources(zoomOutToolStripMenuItem, "zoomOutToolStripMenuItem");
            zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            zoomOutToolStripMenuItem.Click += ZoomOutToolStripMenuItem_Click;
            // 
            // zoom100ToolStripMenuItem
            // 
            resources.ApplyResources(zoom100ToolStripMenuItem, "zoom100ToolStripMenuItem");
            zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
            zoom100ToolStripMenuItem.Click += Zoom100ToolStripMenuItem_Click;
            // 
            // toolStripSeparator18
            // 
            resources.ApplyResources(toolStripSeparator18, "toolStripSeparator18");
            toolStripSeparator18.Name = "toolStripSeparator18";
            // 
            // collapseAllToolStripMenuItem
            // 
            resources.ApplyResources(collapseAllToolStripMenuItem, "collapseAllToolStripMenuItem");
            collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            collapseAllToolStripMenuItem.Click += CollapseAllToolStripMenuItem_Click;
            // 
            // expandAllToolStripMenuItem
            // 
            resources.ApplyResources(expandAllToolStripMenuItem, "expandAllToolStripMenuItem");
            expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            expandAllToolStripMenuItem.Click += ExpandAllToolStripMenuItem_Click;
            // 
            // toolStripSeparator19
            // 
            resources.ApplyResources(toolStripSeparator19, "toolStripSeparator19");
            toolStripSeparator19.Name = "toolStripSeparator19";
            // 
            // showIndentGuidesToolStripMenuItem
            // 
            resources.ApplyResources(showIndentGuidesToolStripMenuItem, "showIndentGuidesToolStripMenuItem");
            showIndentGuidesToolStripMenuItem.Checked = true;
            showIndentGuidesToolStripMenuItem.CheckState = CheckState.Checked;
            showIndentGuidesToolStripMenuItem.Name = "showIndentGuidesToolStripMenuItem";
            showIndentGuidesToolStripMenuItem.Click += IndentGuidesToolStripMenuItem_Click;
            // 
            // showWhitespaceToolStripMenuItem
            // 
            resources.ApplyResources(showWhitespaceToolStripMenuItem, "showWhitespaceToolStripMenuItem");
            showWhitespaceToolStripMenuItem.Name = "showWhitespaceToolStripMenuItem";
            showWhitespaceToolStripMenuItem.Click += HiddenCharactersToolStripMenuItem_Click;
            // 
            // toolStripSeparator11
            // 
            resources.ApplyResources(toolStripSeparator11, "toolStripSeparator11");
            toolStripSeparator11.Name = "toolStripSeparator11";
            // 
            // darkModeToolStripMenuItem
            // 
            resources.ApplyResources(darkModeToolStripMenuItem, "darkModeToolStripMenuItem");
            darkModeToolStripMenuItem.Name = "darkModeToolStripMenuItem";
            darkModeToolStripMenuItem.Click += DarkModeToolStripMenuItem_Click;
            // 
            // aIAssistantToolStripMenuItem
            // 
            resources.ApplyResources(aIAssistantToolStripMenuItem, "aIAssistantToolStripMenuItem");
            aIAssistantToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aIDescriptionAssistantToolStripMenuItem, descriptionAssistantPlusToolStripMenuItem, describeMissingToolStripMenuItem, matchDescribeToolStripMenuItem, toolStripSeparator22, optimizeCodeToolStripMenuItem, modifyCodeToolStripMenuItem, toolStripSeparator24, settingsToolStripMenuItem });
            aIAssistantToolStripMenuItem.Name = "aIAssistantToolStripMenuItem";
            // 
            // aIDescriptionAssistantToolStripMenuItem
            // 
            resources.ApplyResources(aIDescriptionAssistantToolStripMenuItem, "aIDescriptionAssistantToolStripMenuItem");
            aIDescriptionAssistantToolStripMenuItem.Image = Properties.Resources.ai_24;
            aIDescriptionAssistantToolStripMenuItem.Name = "aIDescriptionAssistantToolStripMenuItem";
            aIDescriptionAssistantToolStripMenuItem.Click += AIDescriptionAssistantToolStripMenuItem_Click;
            // 
            // descriptionAssistantPlusToolStripMenuItem
            // 
            resources.ApplyResources(descriptionAssistantPlusToolStripMenuItem, "descriptionAssistantPlusToolStripMenuItem");
            descriptionAssistantPlusToolStripMenuItem.Name = "descriptionAssistantPlusToolStripMenuItem";
            descriptionAssistantPlusToolStripMenuItem.Click += DescriptionAssistantPlusToolStripMenuItem_Click;
            // 
            // describeMissingToolStripMenuItem
            // 
            resources.ApplyResources(describeMissingToolStripMenuItem, "describeMissingToolStripMenuItem");
            describeMissingToolStripMenuItem.Name = "describeMissingToolStripMenuItem";
            describeMissingToolStripMenuItem.Click += DescribeMissingToolStripMenuItem_Click;
            // 
            // matchDescribeToolStripMenuItem
            // 
            resources.ApplyResources(matchDescribeToolStripMenuItem, "matchDescribeToolStripMenuItem");
            matchDescribeToolStripMenuItem.Name = "matchDescribeToolStripMenuItem";
            matchDescribeToolStripMenuItem.Click += BatchDescribeToolStripMenuItem_Click;
            // 
            // toolStripSeparator22
            // 
            resources.ApplyResources(toolStripSeparator22, "toolStripSeparator22");
            toolStripSeparator22.Name = "toolStripSeparator22";
            // 
            // optimizeCodeToolStripMenuItem
            // 
            resources.ApplyResources(optimizeCodeToolStripMenuItem, "optimizeCodeToolStripMenuItem");
            optimizeCodeToolStripMenuItem.Name = "optimizeCodeToolStripMenuItem";
            optimizeCodeToolStripMenuItem.Click += OptimizeCodeToolStripMenuItem_Click;
            // 
            // modifyCodeToolStripMenuItem
            // 
            resources.ApplyResources(modifyCodeToolStripMenuItem, "modifyCodeToolStripMenuItem");
            modifyCodeToolStripMenuItem.Name = "modifyCodeToolStripMenuItem";
            modifyCodeToolStripMenuItem.Click += ModifyCodeToolStripMenuItem_Click;
            // 
            // toolStripSeparator24
            // 
            resources.ApplyResources(toolStripSeparator24, "toolStripSeparator24");
            toolStripSeparator24.Name = "toolStripSeparator24";
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(settingsToolStripMenuItem, "settingsToolStripMenuItem");
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;
            // 
            // executeToolStripMenuItem
            // 
            resources.ApplyResources(executeToolStripMenuItem, "executeToolStripMenuItem");
            executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            executeToolStripMenuItem.Click += OnExecuteToolStripMenuItem_Click;
            // 
            // windowsToolStripMenuItem
            // 
            resources.ApplyResources(windowsToolStripMenuItem, "windowsToolStripMenuItem");
            windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(helpToolStripMenuItem, "helpToolStripMenuItem");
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, toolStripSeparator9, sherlockSoftwareToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            resources.ApplyResources(toolStripSeparator9, "toolStripSeparator9");
            toolStripSeparator9.Name = "toolStripSeparator9";
            // 
            // sherlockSoftwareToolStripMenuItem
            // 
            resources.ApplyResources(sherlockSoftwareToolStripMenuItem, "sherlockSoftwareToolStripMenuItem");
            sherlockSoftwareToolStripMenuItem.Name = "sherlockSoftwareToolStripMenuItem";
            sherlockSoftwareToolStripMenuItem.Click += SherlockSoftwareToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.Items.AddRange(new ToolStripItem[] { progressBar, messageLabel, serverToolStripStatusLabel, databaseToolStripStatusLabel });
            statusStrip1.Name = "statusStrip1";
            toolTip1.SetToolTip(statusStrip1, resources.GetString("statusStrip1.ToolTip"));
            // 
            // progressBar
            // 
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Name = "progressBar";
            progressBar.Style = ProgressBarStyle.Continuous;
            // 
            // messageLabel
            // 
            resources.ApplyResources(messageLabel, "messageLabel");
            messageLabel.Name = "messageLabel";
            messageLabel.Spring = true;
            // 
            // serverToolStripStatusLabel
            // 
            resources.ApplyResources(serverToolStripStatusLabel, "serverToolStripStatusLabel");
            serverToolStripStatusLabel.Image = Properties.Resources.server;
            serverToolStripStatusLabel.Name = "serverToolStripStatusLabel";
            // 
            // databaseToolStripStatusLabel
            // 
            resources.ApplyResources(databaseToolStripStatusLabel, "databaseToolStripStatusLabel");
            databaseToolStripStatusLabel.Image = Properties.Resources.database;
            databaseToolStripStatusLabel.Name = "databaseToolStripStatusLabel";
            // 
            // searchPanel
            // 
            resources.ApplyResources(searchPanel, "searchPanel");
            searchPanel.Controls.Add(closeSearchButton);
            searchPanel.Controls.Add(nextSearchButton);
            searchPanel.Controls.Add(prevSearchButton);
            searchPanel.Controls.Add(searchSQLTextBox);
            searchPanel.Name = "searchPanel";
            toolTip1.SetToolTip(searchPanel, resources.GetString("searchPanel.ToolTip"));
            // 
            // closeSearchButton
            // 
            resources.ApplyResources(closeSearchButton, "closeSearchButton");
            closeSearchButton.BackColor = System.Drawing.Color.White;
            closeSearchButton.ForeColor = System.Drawing.Color.Black;
            closeSearchButton.Name = "closeSearchButton";
            toolTip1.SetToolTip(closeSearchButton, resources.GetString("closeSearchButton.ToolTip"));
            closeSearchButton.UseVisualStyleBackColor = false;
            closeSearchButton.Click += CloseQuickSearch_Click;
            // 
            // nextSearchButton
            // 
            resources.ApplyResources(nextSearchButton, "nextSearchButton");
            nextSearchButton.BackColor = System.Drawing.Color.White;
            nextSearchButton.ForeColor = System.Drawing.SystemColors.ControlText;
            nextSearchButton.Name = "nextSearchButton";
            nextSearchButton.Tag = "Find next (Enter)";
            toolTip1.SetToolTip(nextSearchButton, resources.GetString("nextSearchButton.ToolTip"));
            nextSearchButton.UseVisualStyleBackColor = false;
            nextSearchButton.Click += SearchNext_Click;
            // 
            // prevSearchButton
            // 
            resources.ApplyResources(prevSearchButton, "prevSearchButton");
            prevSearchButton.BackColor = System.Drawing.Color.White;
            prevSearchButton.ForeColor = System.Drawing.SystemColors.ControlText;
            prevSearchButton.Name = "prevSearchButton";
            prevSearchButton.Tag = "Find previous (Shift+Enter)";
            toolTip1.SetToolTip(prevSearchButton, resources.GetString("prevSearchButton.ToolTip"));
            prevSearchButton.UseVisualStyleBackColor = false;
            prevSearchButton.Click += SearchPrevious_Click;
            // 
            // searchSQLTextBox
            // 
            resources.ApplyResources(searchSQLTextBox, "searchSQLTextBox");
            searchSQLTextBox.Name = "searchSQLTextBox";
            toolTip1.SetToolTip(searchSQLTextBox, resources.GetString("searchSQLTextBox.ToolTip"));
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
            resources.ApplyResources(defiCollapsibleSplitter, "defiCollapsibleSplitter");
            defiCollapsibleSplitter.AnimationDelay = 2;
            defiCollapsibleSplitter.AnimationStep = 100;
            defiCollapsibleSplitter.BorderStyle3D = Border3DStyle.Flat;
            defiCollapsibleSplitter.ControlToHide = definitionPanel;
            defiCollapsibleSplitter.Cursor = Cursors.VSplit;
            defiCollapsibleSplitter.ExpandParentForm = false;
            defiCollapsibleSplitter.Name = "defiCollapsibleSplitter";
            defiCollapsibleSplitter.TabStop = false;
            toolTip1.SetToolTip(defiCollapsibleSplitter, resources.GetString("defiCollapsibleSplitter.ToolTip"));
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
            resources.ApplyResources(splitContainer1.Panel1, "splitContainer1.Panel1");
            splitContainer1.Panel1.Controls.Add(objectsTabControl);
            toolTip1.SetToolTip(splitContainer1.Panel1, resources.GetString("splitContainer1.Panel1.ToolTip"));
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(splitContainer1.Panel2, "splitContainer1.Panel2");
            splitContainer1.Panel2.Controls.Add(replacePanel);
            splitContainer1.Panel2.Controls.Add(searchPanel);
            splitContainer1.Panel2.Controls.Add(tabControl1);
            toolTip1.SetToolTip(splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.ToolTip"));
            toolTip1.SetToolTip(splitContainer1, resources.GetString("splitContainer1.ToolTip"));
            // 
            // replacePanel
            // 
            resources.ApplyResources(replacePanel, "replacePanel");
            replacePanel.Controls.Add(button1);
            replacePanel.Controls.Add(replaceAllButton);
            replacePanel.Controls.Add(replaceButton);
            replacePanel.Controls.Add(findNextButton);
            replacePanel.Controls.Add(replaceReplaceTextBox);
            replacePanel.Controls.Add(replaceSearchTextBox);
            replacePanel.Name = "replacePanel";
            toolTip1.SetToolTip(replacePanel, resources.GetString("replacePanel.ToolTip"));
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
            resources.ApplyResources(replaceAllButton, "replaceAllButton");
            replaceAllButton.BackColor = System.Drawing.Color.White;
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
            resources.ApplyResources(replaceButton, "replaceButton");
            replaceButton.BackColor = System.Drawing.Color.White;
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
            resources.ApplyResources(findNextButton, "findNextButton");
            findNextButton.BackColor = System.Drawing.Color.White;
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
            toolTip1.SetToolTip(replaceReplaceTextBox, resources.GetString("replaceReplaceTextBox.ToolTip"));
            replaceReplaceTextBox.KeyDown += ReplaceReplaceTextBox_KeyDown;
            // 
            // replaceSearchTextBox
            // 
            resources.ApplyResources(replaceSearchTextBox, "replaceSearchTextBox");
            replaceSearchTextBox.Name = "replaceSearchTextBox";
            toolTip1.SetToolTip(replaceSearchTextBox, resources.GetString("replaceSearchTextBox.ToolTip"));
            replaceSearchTextBox.TextChanged += TxtSearch_TextChanged;
            replaceSearchTextBox.KeyDown += SearchTextBox_KeyDown;
            // 
            // tabControl1
            // 
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.DarkMode = false;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            toolTip1.SetToolTip(tabControl1, resources.GetString("tabControl1.ToolTip"));
            tabControl1.TabCloseRequested += TabControl1_TabCloseRequested;
            tabControl1.DrawItem += TabControl1_DrawItem;
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            tabControl1.MouseUp += TabControl1_MouseUp;
            tabControl1.Resize += TabControl1_Resize;
            // 
            // tabContextMenuStrip
            // 
            resources.ApplyResources(tabContextMenuStrip, "tabContextMenuStrip");
            tabContextMenuStrip.Items.AddRange(new ToolStripItem[] { tabAliasToolStripMenuItem, closeToolStripMenuItem, saveToolStripMenuItem1, saveAsToolStripMenuItem1, openFolderInFileExplorerToolStripMenuItem });
            tabContextMenuStrip.Name = "tabContextMenuStrip";
            toolTip1.SetToolTip(tabContextMenuStrip, resources.GetString("tabContextMenuStrip.ToolTip"));
            // 
            // tabAliasToolStripMenuItem
            // 
            resources.ApplyResources(tabAliasToolStripMenuItem, "tabAliasToolStripMenuItem");
            tabAliasToolStripMenuItem.Name = "tabAliasToolStripMenuItem";
            tabAliasToolStripMenuItem.Click += TabAliasToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            resources.ApplyResources(closeToolStripMenuItem, "closeToolStripMenuItem");
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem1
            // 
            resources.ApplyResources(saveToolStripMenuItem1, "saveToolStripMenuItem1");
            saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            saveToolStripMenuItem1.Click += SaveToolStripMenuItem1_Click;
            // 
            // saveAsToolStripMenuItem1
            // 
            resources.ApplyResources(saveAsToolStripMenuItem1, "saveAsToolStripMenuItem1");
            saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            saveAsToolStripMenuItem1.Click += SaveAsToolStripMenuItem_Click;
            // 
            // openFolderInFileExplorerToolStripMenuItem
            // 
            resources.ApplyResources(openFolderInFileExplorerToolStripMenuItem, "openFolderInFileExplorerToolStripMenuItem");
            openFolderInFileExplorerToolStripMenuItem.Name = "openFolderInFileExplorerToolStripMenuItem";
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
            toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
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