﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The column def view.
    /// </summary>
    public partial class ColumnDefView : UserControl
    {
        /// <summary>
        /// The database object.
        /// </summary>
        private DBObject _dbObject = new();

        private bool _init = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefView"/> class.
        /// </summary>
        public ColumnDefView()
        {
            InitializeComponent();
            columnDefDataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            for (int i = 0; i < columnDefDataGridView.Columns.Count; i++)
            {
                var column = columnDefDataGridView.Columns[i];
                if (column.DataPropertyName == "Description")
                {
                    column.ReadOnly = false;
                }
                else
                {
                    column.ReadOnly = true;
                }
            }

            parameterGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            for (int i = 0; i < parameterGridView.Columns.Count; i++)
            {
                var column = parameterGridView.Columns[i];
                if (column.DataPropertyName == "Description")
                {
                    column.ReadOnly = false;
                }
                else
                {
                    column.ReadOnly = true;
                }
            }

            definitionTextBox.DocumentType = SqlEditBox.DocumentTypeEnums.Sql;
        }

        public event EventHandler? AddIndexRequested;

        public event EventHandler? AddPrimaryKeyRequested;

        public event EventHandler? SelectedColumnChanged;

        public event EventHandler? TableDescSelected;

        /// <summary>
        /// Gets or sets database connection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DatabaseConnectionItem? Connection { get; set; }

        /// <summary>
        /// Gets object schema
        /// </summary>
        public string? Schema => _dbObject?.Schema;

        /// <summary>
        /// Get selected column name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? SelectedColumn { get; private set; }

        /// <summary>
        /// Gets or sets the selected parameter.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? SelectedParameter { get; set; }

        /// <summary>
        /// Gets or sets table description
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TableDescription
        {
            get { return tableDescTextBox.Text; }
            set { tableDescTextBox.Text = value; }
        }

        /// <summary>
        /// Gets table name
        /// </summary>
        public string? TableFullName => _dbObject?.FullName;

        /// <summary>
        /// Gets table name
        /// </summary>
        public string? TableName => _dbObject?.Name;

        /// <summary>
        /// Gets object type
        /// </summary>
        public ObjectName.ObjectTypeEnums? TableType => _dbObject?.ObjectType;

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _init = true;

            // clear the name panel
            namePanel.Clear();
            SelectedColumn = "";

            // clear the column definition data grid view
            if (columnDefDataGridView.DataSource != null)
            {
                columnDefDataGridView.Visible = false;
                columnDefDataGridView.Columns.Clear();
                columnDefDataGridView.DataSource = null;
            }

            // clar the parameter data grid view
            if (parameterGridView.DataSource != null)
            {
                parameterGridView.Visible = false;
                parameterGridView.Columns.Clear();
                parameterGridView.DataSource = null;
            }

            // clear the definition text box
            definitionTextBox.ReadOnly = false;
            definitionTextBox.ClearAll();
            definitionTextBox.ReadOnly = true;

            // clear the object description text box
            tableDescTextBox.Clear();
            tableDescTextBox.Enabled = false;

            // clear the object property grid
            if (objectPropertyGrid.SelectedObject != null)
            {
                objectPropertyGrid.SelectedObject = null;
            }
            _init = false;
        }

        /// <summary>
        /// Columns the description.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A string.</returns>
        public string ColumnDescription(string columnName)
        {
            for (int i = 0; i < _dbObject.Columns.Count; i++)
            {
                var column = _dbObject.Columns[i];
                if (column.ColumnName == columnName) { return column.Description; }
            }
            return string.Empty;
        }

        /// <summary>
        /// Copy the selected text to the clipboard.
        /// </summary>
        public void Copy()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is TextBox text)
            {
                if (text.Text.Length > 0)
                {
                    // If the TextBox has text, copy the selected text
                    if (text.SelectionLength > 0)
                    {
                        text.Copy();
                        return;
                    }
                }
                else
                {
                    // If the TextBox is empty, copy the whole text
                    Clipboard.SetText(text.Text);
                }
                return;
            }
            else if (focusedControl is Label label)
            {
                // Copy the label's text
                Clipboard.SetText(label.Text);
            }
            else if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is in edit mode, copy from the editing control (usually a TextBox)
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    if (dataGridView.EditingControl is TextBox editingTextBox)
                    {
                        editingTextBox.Copy();
                        return;
                    }
                }
                // Otherwise, copy the selected cells as a DataObject (tabular, for pasting into Excel, etc.)
                var clipboardContent = dataGridView.GetClipboardContent();
                if (clipboardContent != null)
                {
                    Clipboard.SetDataObject(clipboardContent);
                }
                // If nothing is selected, fallback to copying the current cell's value as text
                else
                {
                    var value = dataGridView.CurrentCell?.Value?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        Clipboard.SetText(value);
                    }
                }
            }
            else if (focusedControl is ObjectNamePanel namePanel)
            {
                namePanel.Copy();
            }
        }

        /// <summary>
        /// Generate the script to create an index for the selected column.
        /// </summary>
        /// <returns>A string.</returns>
        public string CreateIndexScript()
        {
            // get the selected column name from the data grid view

            string sql = "";
            // get the selected column name from the data grid view
            if (!string.IsNullOrEmpty(SelectedColumn))
            {
                string indexName = $"IX_{Schema}_{TableName}_{SelectedColumn}";
                sql = $@"IF NOT EXISTS (
    SELECT * FROM sys.indexes
    WHERE name = '{indexName}'
    AND object_id = OBJECT_ID(N'[{Schema}].[{TableName}]')
)
BEGIN
    CREATE INDEX {indexName}
    ON [{Schema}].[{TableName}] ({SelectedColumn});
END
GO
";
            }

            return sql;
        }

        /// <summary>
        /// Cuts the selected text to the clipboard.
        /// </summary>
        public void Cut()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is active, cut the current cell's value
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    if (dataGridView.EditingControl is TextBox editingTextBox && editingTextBox.Enabled)
                    {
                        editingTextBox.Cut();
                    }
                }
            }
            else if (focusedControl is TextBox textBox)
            {
                textBox.Cut();
            }
        }

        /// <summary>
        /// Open the database object schema
        /// </summary>
        /// <param name="objectName">The object name.</param>
        public async Task OpenAsync(ObjectName? objectName, DatabaseConnectionItem? connection)
        {
            // save the description of the current object if modified
            if (tableDescTextBox.Modified && _dbObject.ObjectType != ObjectName.ObjectTypeEnums.None)
            {
                await _dbObject.UpdateObjectDescAsync(tableDescTextBox.Text);
            }

            Clear();

            if (objectName == null || connection == null || string.IsNullOrEmpty(connection.ConnectionString))
            {
                namePanel.Open(null);
                return;
            }

            namePanel.Open(objectName);

            Connection = connection;
            SelectedColumnChanged?.Invoke(this, EventArgs.Empty);

            // clear the column data grid view
            if (columnDefDataGridView.DataSource != null)
            {
                columnDefDataGridView.DataSource = null;
            }
            columnDefDataGridView.Visible = false;

            // clear the parameters data grid view
            if (parameterGridView.DataSource != null)
            {
                parameterGridView.DataSource = null;
            }
            parameterGridView.Visible = false;

            openButton.Visible = false;

            if (objectName == null)
            {
                return;
            }
            else
            {
                _dbObject = new DBObject();
                if (!await _dbObject.OpenAsync(objectName, connection))
                {
                    return;
                }
            }

            TableDescription = _dbObject.Description;

            definitionTextBox.ReadOnly = false;
            definitionTextBox.Text = _dbObject.Definition;
            definitionTextBox.ReadOnly = true;

            // show column definition is object type is table or view
            if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table || _dbObject.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                columnDefDataGridView.DataSource = _dbObject.Columns;
                columnDefDataGridView.Visible = true;

                columnDefDataGridView.AutoResizeColumns();

                if (columnDefDataGridView.Rows.Count > 0)
                {
                    ChangeColumnRowSelection();
                }
                openButton.Visible = true;

                // change the tab page text to "Columns"
                tabControl1.TabPages[0].Text = "Columns";

                if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table)
                    objectPropertyGrid.SelectedObject = _dbObject.TableInformation;
                else
                    objectPropertyGrid.SelectedObject = _dbObject.ViewInformation;
            }
            else if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.StoredProcedure || _dbObject.ObjectType == ObjectName.ObjectTypeEnums.Function)
            {
                parameterGridView.DataSource = _dbObject.Parameters;
                parameterGridView.Visible = true;

                parameterGridView.AutoResizeColumns();
                if (parameterGridView.Rows.Count > 0)
                {
                    ChangeParameterRowSelection();
                }
                // change the tab page text to "parameters"
                tabControl1.TabPages[0].Text = "Parameters";

                if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.StoredProcedure)
                    objectPropertyGrid.SelectedObject = _dbObject.ProcedureInformation;
                else
                    objectPropertyGrid.SelectedObject = _dbObject.FunctionInformation;
            }
            else if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.Trigger)
            {
                objectPropertyGrid.SelectedObject = _dbObject.TriggerInfomation;
                // change the tab page text to "Trigger"
                tabControl1.TabPages[0].Text = "Trigger";
            }
            else if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.Synonym)
            {
                objectPropertyGrid.SelectedObject = _dbObject.SynonymInformation;
                // change the tab page text to "Synonym"
                tabControl1.TabPages[0].Text = "Synonym";
            }

            //tableDescTextBox.Visible = true;
            tableDescTextBox.Modified = false;

            descriptionLabel.Text = _dbObject.ObjectType switch
            {
                ObjectName.ObjectTypeEnums.Table => "Description of the table:",
                ObjectName.ObjectTypeEnums.View => "Description of the view:",
                ObjectName.ObjectTypeEnums.StoredProcedure => "Description of the procedure:",
                ObjectName.ObjectTypeEnums.Function => "Description of the function:",
                ObjectName.ObjectTypeEnums.Trigger => "Description of the trigger:",
                ObjectName.ObjectTypeEnums.Synonym => "Description of the synonym",
                _ => "Description:"
            };

            // enable/disable the add index and primary key menu items based on the object type
            addIndexToolStripMenuItem.Enabled = _dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table || _dbObject.ObjectType == ObjectName.ObjectTypeEnums.View;
            addPrimaryKeyToolStripMenuItem.Enabled = _dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table;

            // enable/disable the column value frequency menu item based on the object type
            columnValueFrequencyToolStripMenuItem.Enabled = _dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table || _dbObject.ObjectType == ObjectName.ObjectTypeEnums.View;

            tableDescTextBox.Enabled = true;
        }

        /// <summary>
        /// Pastes the text from the clipboard into the active control.
        /// </summary>
        public void Paste()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is TextBox textBox)
            {
                textBox.Paste();
                return;
            }
            else if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is active, paste into the current cell
                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    if (dataGridView.EditingControl is TextBox editingTextBox && editingTextBox.Enabled)
                    {
                        editingTextBox.Paste();
                    }
                }
            }
        }

        /// <summary>
        /// generate the script for create the primary key
        /// </summary>
        /// <returns>A string.</returns>
        public string PrimaryKeyScript()
        {
            string sql = "";
            // get the first column name from the data grid view
            if (columnDefDataGridView.Rows.Count > 0)
            {
                string columnName = (string)columnDefDataGridView.Rows[0].Cells["ColumnName"].Value;
                sql = $@"IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND TABLE_NAME = '{TableName}'
    AND TABLE_SCHEMA = '{Schema}'
)
BEGIN
    -- Make the column non-nullable
    --ALTER TABLE [{Schema}].[{TableName}]
    --ALTER COLUMN {columnName} smallint NOT NULL;

    ALTER TABLE [{Schema}].[{TableName}]
    ADD CONSTRAINT PK_{Schema}_{TableName} PRIMARY KEY ({columnName})
END
GO
";
            }

            return sql;
        }

        /// <summary>
        /// Selects all text in the active control.
        /// </summary>
        public void SelectAll()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is DataGridView dataGridView)
            {
                // If the DataGridView is active, select all cells
                dataGridView.SelectAll();
            }
            else if (focusedControl is TextBox textBox)
            {
                // Select all text in the TextBox
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Updates the column desc.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="description">The description.</param>
        public async Task UpdateColumnDescAsync(string columnName, string description)
        {
            await _dbObject.UpdateLevel2DescriptionAsync(columnName, description, _dbObject.ObjectType);
        }

        /// <summary>
        /// Updates the table description.
        /// </summary>
        /// <param name="description">The description.</param>
        public async Task UpdateTableDescriptionAsync(string description)
        {
            await _dbObject.UpdateObjectDescAsync(description);
            TableDescription = description;
        }

        /// <summary>
        /// Recursively gets the currently focused control within a container.
        /// </summary>
        /// <param name="container">The container control.</param>
        /// <returns>The focused control, or null if none is focused.</returns>
        private static Control? GetFocusedControl(Control container)
        {
            if (container == null)
                return null;

            if (container.Focused)
                return container;

            foreach (Control child in container.Controls)
            {
                Control? focused = GetFocusedControl(child);
                if (focused != null)
                    return focused;
            }

            return null;
        }

        /// <summary>
        /// Handles the add description tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void AddDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tableDescTextBox.Text.Contains("This table,"))
            {
                MessageBox.Show("The description already contains a table dependency script.");
                return;
            }

            string description = tableDescTextBox.Text;
            if (description.Length > 0)
            {
                description += Environment.NewLine;
            }
            description += await GetTableDependencyScriptAsync();

            tableDescTextBox.Text = description;
            await _dbObject.UpdateObjectDescAsync(description);
        }

        /// <summary>
        /// Handles the add index tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddIndexRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the add primary key tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddPrimaryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPrimaryKeyRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Change row selection.
        /// </summary>
        private void ChangeColumnRowSelection()
        {
            if (columnDefDataGridView.SelectedRows != null)
            {
                int rowIndex = columnDefDataGridView.CurrentCell.RowIndex;
                string columnName = (string)columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"].Value;
                if (columnName != SelectedColumn)
                {
                    SelectedColumn = columnName;
                    SelectedColumnChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Changes the parameter row selection.
        /// </summary>
        private void ChangeParameterRowSelection()
        {
            if (parameterGridView.SelectedRows != null)
            {
                int rowIndex = parameterGridView.CurrentCell.RowIndex;
                string paraName = (string)parameterGridView.Rows[rowIndex].Cells["Name"].Value;
                if (paraName != SelectedParameter)
                {
                    SelectedParameter = paraName;
                    //SelectedColumnChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Columns the def data grid view cell click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ColumnDefDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeColumnRowSelection();
        }

        /// <summary>
        /// Columns the def data grid view cell validated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void ColumnDefDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (_init) return; // prevent recursive calls during initialization

            int rowIndex = e.RowIndex;
            if (rowIndex >= 0)
            {
                string columnName = (string)columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"].Value;
                string columnDesc = (string)columnDefDataGridView.Rows[rowIndex].Cells["Description"].Value;
                await UpdateColumnDescAsync(columnName, columnDesc);
            }
        }

        /// <summary>
        /// Handles the column value frequency tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ColumnValueFrequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Connection == null)
            {
                return;
            }

            // get the selected column name from the data grid view
            if (columnDefDataGridView.SelectedCells.Count > 0)
            {
                int rowIndex = columnDefDataGridView.CurrentCell.RowIndex;
                if (rowIndex == -1) return;

                string columnName = (string)columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"].Value;
                if (string.IsNullOrEmpty(columnName))
                {
                    MessageBox.Show("Please select a column to get the frequency.");
                    return;
                }

                string sql = $@"SELECT {columnName}, COUNT(*) AS Frequency FROM [{Schema}].[{TableName}] GROUP BY {columnName}";
                using var dlg = new DataViewForm()
                {
                    SQL = sql,
                    MultipleValue = false,
                    Text = $"{columnName}",
                    TableName = $"[{Schema}].[{TableName}]",
                    EnableValueFrequency = true,
                    DatabaseIndex = 0,
                    ConnectionString = Connection.ConnectionString
                };
                dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the copy tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(tableDescTextBox.SelectedText);
        }

        /// <summary>
        /// Gets the table dependency script.
        /// </summary>
        /// <returns>A string.</returns>
        private async Task<string> GetTableDependencyScriptAsync()
        {
            var viewList = await DBObject.GetObjectsUsingTableAsync(_dbObject.ObjectName, Connection);

            if (viewList != null && viewList.Count > 0)
            {
                // build the view list string
                var viewListString = string.Join(", ", viewList.Select(v => $"[{v.Schema}].[{v.Name}]"));

                return $"This table, [{Schema}].[{TableName}], serves as a reference table defining .... It is utilized in the views: {viewListString}.";
            }

            return string.Empty;
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connection == null || string.IsNullOrEmpty(Connection.ConnectionString) || string.IsNullOrEmpty(Schema) || string.IsNullOrEmpty(TableName))
            {
                return;
            }

            var sql = $@"SELECT * FROM [{Schema}].[{TableName}]";
            using var dlg = new DataViewForm()
            {
                SQL = sql,
                MultipleValue = false,
                Text = $"{TableName}",
                TableName = $"[{Schema}].[{TableName}]",
                EnableValueFrequency = true,
                DatabaseIndex = 0,
                ConnectionString = Connection.ConnectionString
            };
            dlg.ShowDialog();
        }

        /// <summary>
        /// Parameters the grid view_ cell click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ParameterGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeParameterRowSelection();
        }

        /// <summary>
        /// Handles the parameter grid view cell click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ParameterGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex >= 0)
            {
                string name = (string)parameterGridView.Rows[rowIndex].Cells["Name"].Value;
                string parameterDesc = (string)parameterGridView.Rows[rowIndex].Cells["Description"].Value;
                await UpdateParameterDescAsync(name, parameterDesc);
            }
        }

        /// <summary>
        /// Handles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableDescTextBox.Paste();
            await _dbObject.UpdateObjectDescAsync(tableDescTextBox.Text);
        }

        /// <summary>
        /// Handles the save table description button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SaveTableDescButton_Click(object sender, EventArgs e)
        {
            if (tableDescTextBox.Modified || _dbObject.ObjectType != ObjectName.ObjectTypeEnums.None)
            {
                await _dbObject.UpdateObjectDescAsync(tableDescTextBox.Text);
                tableDescTextBox.Modified = false;
            }
        }

        /// <summary>
        /// Table the label click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableLabel_Click(object sender, EventArgs e)
        {
            TableDescSelected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the parameter desc async.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameterDesc">The parameter desc.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>A Task.</returns>
        private async Task UpdateParameterDescAsync(string name, string parameterDesc)
        {
            await _dbObject.UpdateLevel2DescriptionAsync(name, parameterDesc, _dbObject.ObjectType);
        }
    }
}