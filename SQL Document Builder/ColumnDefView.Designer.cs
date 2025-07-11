﻿using SQL_Document_Builder.Properties;

namespace SQL_Document_Builder
{
    partial class ColumnDefView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            namePanel = new ObjectNamePanel();
            openButton = new System.Windows.Forms.Button();
            openTableContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tableDescTextBox = new System.Windows.Forms.TextBox();
            tableContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            addDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            columnDefDataGridView = new System.Windows.Forms.DataGridView();
            columnContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            columnValueFrequencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            addIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addPrimaryKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            triggerGridView = new System.Windows.Forms.DataGridView();
            parameterGridView = new System.Windows.Forms.DataGridView();
            tabPage2 = new System.Windows.Forms.TabPage();
            definitionTextBox = new SqlEditBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            objectPropertyGrid = new System.Windows.Forms.PropertyGrid();
            panel1 = new System.Windows.Forms.Panel();
            saveTableDescButton = new System.Windows.Forms.Button();
            descriptionLabel = new System.Windows.Forms.Label();
            namePanel.SuspendLayout();
            openTableContextMenuStrip.SuspendLayout();
            tableContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)columnDefDataGridView).BeginInit();
            columnContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)triggerGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)parameterGridView).BeginInit();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // namePanel
            // 
            namePanel.BackColor = System.Drawing.SystemColors.ControlLight;
            namePanel.Controls.Add(openButton);
            namePanel.Dock = System.Windows.Forms.DockStyle.Top;
            namePanel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            namePanel.Location = new System.Drawing.Point(0, 0);
            namePanel.Name = "namePanel";
            namePanel.Size = new System.Drawing.Size(359, 28);
            namePanel.TabIndex = 0;
            // 
            // openButton
            // 
            openButton.Dock = System.Windows.Forms.DockStyle.Right;
            openButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            openButton.Image = Resources.openfile20;
            openButton.Location = new System.Drawing.Point(331, 0);
            openButton.Name = "openButton";
            openButton.Padding = new System.Windows.Forms.Padding(3);
            openButton.Size = new System.Drawing.Size(28, 28);
            openButton.TabIndex = 1;
            openButton.UseVisualStyleBackColor = true;
            openButton.Click += OpenToolStripMenuItem_Click;
            // 
            // openTableContextMenuStrip
            // 
            openTableContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem });
            openTableContextMenuStrip.Name = "openTableContextMenuStrip";
            openTableContextMenuStrip.Size = new System.Drawing.Size(104, 26);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // tableDescTextBox
            // 
            tableDescTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableDescTextBox.ContextMenuStrip = tableContextMenuStrip;
            tableDescTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            tableDescTextBox.Location = new System.Drawing.Point(0, 28);
            tableDescTextBox.Multiline = true;
            tableDescTextBox.Name = "tableDescTextBox";
            tableDescTextBox.Size = new System.Drawing.Size(359, 100);
            tableDescTextBox.TabIndex = 1;
            tableDescTextBox.Click += TableLabel_Click;
            // 
            // tableContextMenuStrip
            // 
            tableContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator1, addDescriptionToolStripMenuItem });
            tableContextMenuStrip.Name = "tableContextMenuStrip";
            tableContextMenuStrip.Size = new System.Drawing.Size(159, 76);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            pasteToolStripMenuItem.Text = "Paste";
            pasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // addDescriptionToolStripMenuItem
            // 
            addDescriptionToolStripMenuItem.Name = "addDescriptionToolStripMenuItem";
            addDescriptionToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            addDescriptionToolStripMenuItem.Text = "Add description";
            addDescriptionToolStripMenuItem.Click += AddDescriptionToolStripMenuItem_Click;
            // 
            // columnDefDataGridView
            // 
            columnDefDataGridView.AllowUserToAddRows = false;
            columnDefDataGridView.AllowUserToDeleteRows = false;
            columnDefDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            columnDefDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            columnDefDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            columnDefDataGridView.ContextMenuStrip = columnContextMenuStrip;
            columnDefDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            columnDefDataGridView.Location = new System.Drawing.Point(3, 3);
            columnDefDataGridView.Name = "columnDefDataGridView";
            columnDefDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            columnDefDataGridView.Size = new System.Drawing.Size(345, 432);
            columnDefDataGridView.TabIndex = 1;
            columnDefDataGridView.Visible = false;
            columnDefDataGridView.CellClick += ColumnDefDataGridView_CellClick;
            columnDefDataGridView.CellValidated += ColumnDefDataGridView_CellValidated;
            // 
            // columnContextMenuStrip
            // 
            columnContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { columnValueFrequencyToolStripMenuItem, toolStripSeparator2, addIndexToolStripMenuItem, addPrimaryKeyToolStripMenuItem });
            columnContextMenuStrip.Name = "columnContextMenuStrip";
            columnContextMenuStrip.Size = new System.Drawing.Size(207, 76);
            // 
            // columnValueFrequencyToolStripMenuItem
            // 
            columnValueFrequencyToolStripMenuItem.Name = "columnValueFrequencyToolStripMenuItem";
            columnValueFrequencyToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            columnValueFrequencyToolStripMenuItem.Text = "Column Value Frequency";
            columnValueFrequencyToolStripMenuItem.Click += ColumnValueFrequencyToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(203, 6);
            // 
            // addIndexToolStripMenuItem
            // 
            addIndexToolStripMenuItem.Name = "addIndexToolStripMenuItem";
            addIndexToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            addIndexToolStripMenuItem.Text = "Create Index";
            addIndexToolStripMenuItem.Click += AddIndexToolStripMenuItem_Click;
            // 
            // addPrimaryKeyToolStripMenuItem
            // 
            addPrimaryKeyToolStripMenuItem.Name = "addPrimaryKeyToolStripMenuItem";
            addPrimaryKeyToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            addPrimaryKeyToolStripMenuItem.Text = "Create Primary Key";
            addPrimaryKeyToolStripMenuItem.Click += AddPrimaryKeyToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tableDescTextBox);
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Size = new System.Drawing.Size(359, 598);
            splitContainer1.SplitterDistance = 466;
            splitContainer1.TabIndex = 3;
            // 
            // tabControl1
            // 
            tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(359, 466);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(triggerGridView);
            tabPage1.Controls.Add(parameterGridView);
            tabPage1.Controls.Add(columnDefDataGridView);
            tabPage1.Location = new System.Drawing.Point(4, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(351, 438);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Columns";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // triggerGridView
            // 
            triggerGridView.AllowUserToAddRows = false;
            triggerGridView.AllowUserToDeleteRows = false;
            triggerGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            triggerGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            triggerGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            triggerGridView.ContextMenuStrip = columnContextMenuStrip;
            triggerGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            triggerGridView.Location = new System.Drawing.Point(3, 3);
            triggerGridView.Name = "triggerGridView";
            triggerGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            triggerGridView.Size = new System.Drawing.Size(345, 432);
            triggerGridView.TabIndex = 2;
            triggerGridView.Visible = false;
            // 
            // parameterGridView
            // 
            parameterGridView.AllowUserToAddRows = false;
            parameterGridView.AllowUserToDeleteRows = false;
            parameterGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            parameterGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            parameterGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            parameterGridView.ContextMenuStrip = columnContextMenuStrip;
            parameterGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            parameterGridView.Location = new System.Drawing.Point(3, 3);
            parameterGridView.Name = "parameterGridView";
            parameterGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            parameterGridView.Size = new System.Drawing.Size(345, 432);
            parameterGridView.TabIndex = 2;
            parameterGridView.Visible = false;
            parameterGridView.CellClick += ParameterGridView_CellClick;
            parameterGridView.CellValidated += ParameterGridView_CellValidated;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(definitionTextBox);
            tabPage2.Location = new System.Drawing.Point(4, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(351, 437);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Definition";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // definitionTextBox
            // 
            definitionTextBox.AllowDrop = true;
            definitionTextBox.AutocompleteListSelectedBackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            definitionTextBox.AutomaticFold = ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click | ScintillaNET.AutomaticFold.Change;
            definitionTextBox.CaretForeColor = System.Drawing.Color.White;
            definitionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            definitionTextBox.Font = new System.Drawing.Font("Cascadia Mono", 10F);
            definitionTextBox.IndentationGuides = ScintillaNET.IndentView.LookBoth;
            definitionTextBox.LexerName = "sql";
            definitionTextBox.Location = new System.Drawing.Point(3, 3);
            definitionTextBox.Name = "definitionTextBox";
            definitionTextBox.SelectionBackColor = System.Drawing.Color.FromArgb(17, 77, 156);
            definitionTextBox.Size = new System.Drawing.Size(345, 431);
            definitionTextBox.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(objectPropertyGrid);
            tabPage3.Location = new System.Drawing.Point(4, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(351, 437);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Properties";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // objectPropertyGrid
            // 
            objectPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            objectPropertyGrid.Location = new System.Drawing.Point(3, 3);
            objectPropertyGrid.Name = "objectPropertyGrid";
            objectPropertyGrid.Size = new System.Drawing.Size(345, 431);
            objectPropertyGrid.TabIndex = 4;
            // 
            // panel1
            // 
            panel1.Controls.Add(saveTableDescButton);
            panel1.Controls.Add(descriptionLabel);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(359, 28);
            panel1.TabIndex = 0;
            // 
            // saveTableDescButton
            // 
            saveTableDescButton.Dock = System.Windows.Forms.DockStyle.Right;
            saveTableDescButton.Image = Resources.save;
            saveTableDescButton.Location = new System.Drawing.Point(331, 0);
            saveTableDescButton.Name = "saveTableDescButton";
            saveTableDescButton.Size = new System.Drawing.Size(28, 28);
            saveTableDescButton.TabIndex = 1;
            saveTableDescButton.UseVisualStyleBackColor = true;
            saveTableDescButton.Click += SaveTableDescButton_Click;
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new System.Drawing.Point(3, 6);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new System.Drawing.Size(107, 15);
            descriptionLabel.TabIndex = 0;
            descriptionLabel.Text = "Object description:";
            // 
            // ColumnDefView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Controls.Add(namePanel);
            Name = "ColumnDefView";
            Size = new System.Drawing.Size(359, 626);
            namePanel.ResumeLayout(false);
            openTableContextMenuStrip.ResumeLayout(false);
            tableContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)columnDefDataGridView).EndInit();
            columnContextMenuStrip.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)triggerGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)parameterGridView).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ObjectNamePanel namePanel;
        private System.Windows.Forms.TextBox tableDescTextBox;
        private System.Windows.Forms.DataGridView columnDefDataGridView;
        private System.Windows.Forms.ContextMenuStrip columnContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem columnValueFrequencyToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip tableContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addDescriptionToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip openTableContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button saveTableDescButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private SqlEditBox definitionTextBox;
        private System.Windows.Forms.DataGridView parameterGridView;
        private System.Windows.Forms.DataGridView triggerGridView;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PropertyGrid objectPropertyGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPrimaryKeyToolStripMenuItem;
    }
}
