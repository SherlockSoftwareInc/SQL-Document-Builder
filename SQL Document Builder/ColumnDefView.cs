using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Provides a user control for viewing and editing SQL database object definitions,
    /// including tables, views, stored procedures, functions, triggers, and synonyms.
    /// Supports editing descriptions, generating scripts, and integrating with AI-based
    /// description assistants.
    /// </summary>
    public partial class ColumnDefView : UserControl
    {
        /// <summary>
        /// The database object.
        /// </summary>
        private DBObject _dbObject = new();

        private bool _init = false;
        private bool _isChanged = false;
        private TableContext _tableContext = new();

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

        /// <summary>
        /// Occurs when the user requests to add an index.
        /// </summary>
        public event EventHandler? AddIndexRequested;

        /// <summary>
        /// Occurs when the user requests to add a primary key.
        /// </summary>
        public event EventHandler? AddPrimaryKeyRequested;

        /// <summary>
        /// Occurs when AI processing is completed.
        /// </summary>
        public event EventHandler? AIProcessingCompleted;

        /// <summary>
        /// Occurs when AI processing begins.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when an AI operation starts. This event is
        /// typically raised before any processing or computation is performed. Event handlers can use this notification
        /// to update UI elements, log activity, or perform other preparatory actions.
        /// </remarks>
        public event EventHandler? AIProcessingStarted;

        /// <summary>
        /// Occurs when the selected column changes.
        /// </summary>
        public event EventHandler? SelectedColumnChanged;

        /// <summary>
        /// Occurs when the table description is selected.
        /// </summary>
        public event EventHandler? TableDescSelected;

        /// <summary>
        /// Gets or sets the database connection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DatabaseConnectionItem? Connection { get; set; }

        /// <summary>
        /// Gets or sets the object name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectName? ObjectName { get; private set; }

        /// <summary>
        /// Gets the schema of the current database object.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Schema => _dbObject?.Schema;

        /// <summary>
        /// Gets the selected column name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? SelectedColumn { get; private set; }

        /// <summary>
        /// Gets or sets the selected parameter name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? SelectedParameter { get; set; }

        /// <summary>
        /// Gets or sets the table description.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TableDescription
        {
            get { return tableDescTextBox.Text; }
            set { tableDescTextBox.Text = value; }
        }

        /// <summary>
        /// Gets the full table name, including schema.
        /// </summary>
        public string? TableFullName => _dbObject?.FullName;

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string? TableName => _dbObject?.Name;

        /// <summary>
        /// Gets the object type of the current database object.
        /// </summary>
        public ObjectName.ObjectTypeEnums? TableType => _dbObject?.ObjectType;

        /// <summary>
        /// Gets the definition text.
        /// </summary>
        public object DefinitionText { get => definitionTextBox.Text; }

        /// <summary>
        /// Clears the current view, resetting all UI elements and internal state.
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
            _isChanged = false;
        }

        /// <summary>
        /// Gets the description for a specified column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>The column description, or an empty string if not found.</returns>
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
        /// Copies the selected text or value from the currently focused control to the clipboard.
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
        /// Generates a SQL script to create an index for the selected column.
        /// </summary>
        /// <returns>The SQL script as a string, or an empty string if no column is selected.</returns>
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
        /// Cuts the selected text or value from the currently focused control to the clipboard.
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
        /// Opens the specified database object asynchronously and populates the view.
        /// </summary>
        /// <param name="objectName">The object name to open.</param>
        /// <param name="connection">The database connection to use.</param>
        public async Task OpenAsync(ObjectName? objectName, DatabaseConnectionItem? connection)
        {
            // save the description of the current object if modified
            if (_isChanged && _dbObject.ObjectType != ObjectName.ObjectTypeEnums.None)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save them before opening a new object?",
                    "Confirm Save",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await Save();
                }
            }

            Clear();

            ObjectName = objectName;

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
            aiButton.Visible = false;

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

                // Populate _tableContext with the schema (columns) after loading the DBObject
                _tableContext = new TableContext(_dbObject.Columns.ToList());
            }

            _tableContext.ObjectType = _dbObject.ObjectType.ToString();
            _tableContext.TableSchema = _dbObject.Schema ?? "";
            _tableContext.TableName = _dbObject.Name ?? "";

            TableDescription = _dbObject.Description;
            _tableContext.TableDescription = _dbObject.Description;

            definitionTextBox.ReadOnly = false;
            definitionTextBox.Text = _dbObject.Definition;
            definitionTextBox.ReadOnly = true;

            // show column definition is object type is table or view
            if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table || _dbObject.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                columnDefDataGridView.DataSource = _tableContext.Columns;
                columnDefDataGridView.Visible = true;

                columnDefDataGridView.AutoResizeColumns();

                if (columnDefDataGridView.Rows.Count > 0)
                {
                    ChangeColumnRowSelection();
                }
                openButton.Visible = true;
                aiButton.Visible = true;

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

            referencedDataGridView.Visible = false;
            referencingDataGridView.Visible = false;

            // Retrieve referenced and referencing objects in parallel using Task.Run
            if (objectName != null && connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
            {
                // Use Task.Run to avoid blocking the UI thread
                var referencedTask = Task.Run(() => _dbObject.GetReferencedObjectsAsync(connection));
                var referencedObjects = await referencedTask;
                // Populate the data grids on the UI thread
                if (referencedDataGridView.InvokeRequired)
                {
                    referencedDataGridView.Invoke(new Action(() =>
                    {
                        referencedDataGridView.DataSource = referencedObjects;
                    }));
                }
                else
                {
                    referencedDataGridView.DataSource = referencedObjects;
                }
                referencedDataGridView.Visible = true;

                var referencingTask = Task.Run(() => _dbObject.GetReferencingObjectsAsync(connection));
                var referencingObjects = await referencingTask;
                if (referencingDataGridView.InvokeRequired)
                {
                    referencingDataGridView.Invoke(new Action(() =>
                    {
                        referencingDataGridView.DataSource = referencingObjects;
                    }));
                }
                else
                {
                    referencingDataGridView.DataSource = referencingObjects;
                }
                referencingDataGridView.Visible = true;
            }

            _isChanged = false;
        }

        /// <summary>
        /// Pastes the clipboard text into the currently focused control.
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
        /// Generates a SQL script to create a primary key on the first column.
        /// </summary>
        /// <returns>The SQL script as a string, or an empty string if no columns exist.</returns>
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
        /// Saves the table and column descriptions to the database if there are changes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Save()
        {
            if (_isChanged || _dbObject.ObjectType != ObjectName.ObjectTypeEnums.None)
            {
                // save the table description
                await _dbObject.UpdateObjectDescAsync(tableDescTextBox.Text);

                // save description for each column
                if (_dbObject.ObjectType == ObjectName.ObjectTypeEnums.Table || _dbObject.ObjectType == ObjectName.ObjectTypeEnums.View)
                {
                    foreach (var column in _tableContext.Columns)
                    {
                        // Only update if the description is not null (avoid unnecessary DB calls)
                        if (!string.IsNullOrEmpty(column.ColumnName))
                        {
                            await _dbObject.UpdateLevel2DescriptionAsync(column.ColumnName, column.Description!, _dbObject.ObjectType);
                        }
                    }
                }
            }
            _isChanged = false;
        }

        /// <summary>
        /// Selects all text or cells in the currently focused control.
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
        /// Updates the description for a specified column asynchronously.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="description">The new description.</param>
        public async Task UpdateColumnDescAsync(string columnName, string description)
        {
            await _dbObject.UpdateLevel2DescriptionAsync(columnName, description, _dbObject.ObjectType);
        }

        /// <summary>
        /// Updates the table description asynchronously.
        /// </summary>
        /// <param name="description">The new table description.</param>
        public async Task UpdateTableDescriptionAsync(string description)
        {
            await _dbObject.UpdateObjectDescAsync(description);
            TableDescription = description;
        }

        /// <summary>
        /// Checks if the AI settings are ready.
        /// </summary>
        /// <returns><c>true</c> if AI settings are ready; otherwise, <c>false</c>.</returns>
        private static bool AISettingsReady()
        {
            // check if the AI settings are ready
            var aiSettings = AISettingsManager.Current;
            return !string.IsNullOrEmpty(aiSettings.AIApiKey) &&
                !string.IsNullOrEmpty(aiSettings.AIEndpoint)
                && !string.IsNullOrEmpty(aiSettings.AIModel);
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
        /// Appends a table dependency script to the table description if not already present.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// Raises the <see cref="AddIndexRequested"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void AddIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddIndexRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the add primary key tool strip menu item click event.
        /// Raises the <see cref="AddPrimaryKeyRequested"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void AddPrimaryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPrimaryKeyRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the AI button click event.
        /// Uses the AI description assistant to generate table and column descriptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void AiButton_Click(object sender, EventArgs e)
        {
            await AIAssistant();
        }

        /// <summary>
        /// Changes the selected column row and raises the <see cref="SelectedColumnChanged"/> event if changed.
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
        /// Changes the selected parameter row and updates the <see cref="SelectedParameter"/>.
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
        /// Handles the column definition DataGridView cell click event.
        /// Updates the selected column.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ColumnDefDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeColumnRowSelection();
        }

        /// <summary>
        /// Handles the column definition DataGridView cell validated event.
        /// Updates the column description in the context.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ColumnDefDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (_init) return; // prevent recursive calls during initialization

            int rowIndex = e.RowIndex;
            if (rowIndex >= 0)
            {
                string columnName = (string)columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"].Value;
                string columnDesc = (string)columnDefDataGridView.Rows[rowIndex].Cells["Description"].Value;

                // find the column in the _tableContext and update its description
                var column = _tableContext.Columns.FirstOrDefault(c => c.ColumnName == columnName);
                if (column != null)
                {
                    column.Description = columnDesc;
                }

                _isChanged = true;
            }
        }

        /// <summary>
        /// Handles the column value frequency tool strip menu item click event.
        /// Opens a dialog showing value frequency for the selected column.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ColumnValueFrequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connection == null)
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
                    ConnectionString = Connection.ConnectionString!
                };
                dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the copy tool strip menu item click event.
        /// Copies the selected text from the table description textbox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(tableDescTextBox.SelectedText);
        }

        /// <summary>
        /// Gets the reference context as a JSON string for AI processing.
        /// </summary>
        /// <returns>A task that returns the reference context as a JSON string.</returns>
        private async Task<string> GetReferenceContext()
        {
            var dt = await _dbObject.GetForeignTablesAsync(Connection!);

            if (dt == null)
            {
                return "";
            }

            // conver the datatable to a json string
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(dt, options);
                return jsonString;
            }
            catch (Exception)
            {
            }

            return "";
        }

        /// <summary>
        /// Gets the table dependency script asynchronously.
        /// </summary>
        /// <returns>A string describing table dependencies, or an empty string if none.</returns>
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
        /// Handles the open object tool strip menu item click event.
        /// Opens the selected referenced or referencing object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void OpenObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string schema = string.Empty;
            string objectName = string.Empty;
            string objectTypeStr = string.Empty;
            int rowIndex = -1;

            // when tab is referenced objects, open the referenced object
            if (tabControl1.SelectedIndex == 3 && referencedDataGridView.Rows.Count > 0)
            {
                // Get the selected row in the referenced objects grid view
                rowIndex = referencedDataGridView.CurrentCell.RowIndex;
                var selectedRow = referencedDataGridView.Rows[rowIndex];
                schema = (string)selectedRow.Cells["Schema"].Value;
                objectName = (string)selectedRow.Cells["ObjectName"].Value;
                objectTypeStr = (string)selectedRow.Cells["ObjectType"].Value;
            }
            else if (tabControl1.SelectedIndex == 4 && referencingDataGridView.Rows.Count > 0)
            {
                // Get the selected row in the current objects grid view
                rowIndex = referencingDataGridView.CurrentCell.RowIndex;
                var selectedRow = referencingDataGridView.Rows[rowIndex];
                schema = (string)selectedRow.Cells["Schema"].Value;
                objectName = (string)selectedRow.Cells["ObjectName"].Value;
                objectTypeStr = (string)selectedRow.Cells["ObjectType"].Value;
            }

            if (rowIndex == -1 || string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(objectName) || string.IsNullOrEmpty(objectTypeStr))
            {
                return;
            }

            // convert the object type string to ObjectName.ObjectTypeEnums
            ObjectName.ObjectTypeEnums objectType = ObjectName.ConvertObjectType(objectTypeStr);

            await OpenAsync(new ObjectName(objectType, schema, objectName), Connection);
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// Opens a data view dialog for the current table.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// Handles the parameter grid view cell click event.
        /// Updates the selected parameter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ParameterGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeParameterRowSelection();
        }

        /// <summary>
        /// Handles the parameter grid view cell validated event.
        /// Updates the parameter description asynchronously.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
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
        /// Pastes text into the table description textbox and updates the database.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableDescTextBox.Paste();
            await _dbObject.UpdateObjectDescAsync(tableDescTextBox.Text);
        }

        /// <summary>
        /// Handles the save button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void SaveButton_Click(object sender, EventArgs e)
        {
            await Save();
        }

        /// <summary>
        /// Handles the table description text box text changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TableDescTextBox_TextChanged(object sender, EventArgs e)
        {
            _isChanged = true;
        }

        /// <summary>
        /// Handles the table label click event.
        /// Raises the <see cref="TableDescSelected"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TableLabel_Click(object sender, EventArgs e)
        {
            TableDescSelected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the parameter description asynchronously.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="parameterDesc">The new parameter description.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateParameterDescAsync(string name, string parameterDesc)
        {
            await _dbObject.UpdateLevel2DescriptionAsync(name, parameterDesc, _dbObject.ObjectType);
        }

        /// <summary>
        /// Perform AI assistant to generagte description for the object
        /// </summary>
        /// <returns>A Task.</returns>
        internal async Task AIAssistant()
        {
            // open AI settings dialog if not ready
            if (!AISettingsReady())
            {
                using var dlg = new SettingsForm();
                dlg.ShowDialog();
            }

            if (AISettingsReady())
            {
                // raise an event to indicate AI processing has started
                AIProcessingStarted?.Invoke(this, EventArgs.Empty);

                var referenceContext = await GetReferenceContext();

                // call AIHelper to generate table and column descriptions
                var helper = new AIHelper();
                await helper.GenerateTableAndColumnDescriptionsAsync(_tableContext, referenceContext, Connection?.DatabaseDescription);

                // checks if the table object is still the same
                if (_dbObject.ObjectName == null ||
                    _dbObject.ObjectName.Name != _tableContext.TableName ||
                    _dbObject.ObjectName.Schema != _tableContext.TableSchema)
                {
                    // the object has changed, do not update the UI
                    return;
                }

                // update the table description text box
                tableDescTextBox.Text = _tableContext.TableDescription;

                // update the column descriptions in the data grid view
                foreach (DataGridViewRow row in columnDefDataGridView.Rows)
                {
                    string columnName = (string)row.Cells["ColumnName"].Value;
                    var column = _tableContext.Columns.FirstOrDefault(c => c.ColumnName == columnName);
                    if (column != null)
                    {
                        row.Cells["Description"].Value = column.Description;
                    }
                }

                // refresh the data grid view
                columnDefDataGridView.Refresh();

                // resize the columns
                columnDefDataGridView.AutoResizeColumns();

                _isChanged = true;
            }

            // raise an event to indicate AI processing has ended
            AIProcessingCompleted?.Invoke(this, EventArgs.Empty);

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles the column reference tool strip menu item click event.
        /// Open the column reference dialog and get the reference selected by user.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ColumnReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure Connection is available
            if (Connection == null)
            {
                MessageBox.Show("No database connection is available.");
                return;
            }

            // Ensure a cell is selected in the DataGridView
            if (columnDefDataGridView.CurrentCell == null)
            {
                MessageBox.Show("Please select a column to reference.");
                return;
            }

            // Open the column reference dialog
            using var dlg = new ColumnReferenceDialog()
            {
                Connection = Connection
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var selectedObject = dlg.SelectedObject;
                if (selectedObject != null)
                {
                    // Insert the column reference into the definition text box at the current cursor position
                    string referenceText = $"[{selectedObject.Schema}].[{selectedObject.Name}]";

                    // Get the current row index of the data grid view
                    int rowIndex = columnDefDataGridView.CurrentCell.RowIndex;

                    // Get column description cell
                    var columnNameCell = columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"];
                    var columnDescCell = columnDefDataGridView.Rows[rowIndex].Cells["Description"];
                    if (columnNameCell == null || columnDescCell == null)
                    {
                        MessageBox.Show("Column or description cell not found.");
                        return;
                    }

                    string columnName = columnNameCell.Value?.ToString() ?? string.Empty;
                    string columnDesc = columnDescCell.Value?.ToString() ?? string.Empty;

                    // Replace reference text in the column description if found, otherwise append at the end
                    int refIndex = columnDesc.IndexOf("Reference:");
                    if (refIndex >= 0)
                    {
                        string beforeRef = columnDesc.Substring(0, refIndex).TrimEnd();
                        if (!string.IsNullOrEmpty(beforeRef))
                        {
                            beforeRef += " ";
                        }
                        columnDesc = beforeRef + $"Reference: {referenceText}";
                    }
                    else
                    {
                        if (columnDesc.Length > 0 && !columnDesc.EndsWith(" "))
                        {
                            columnDesc += " ";
                        }
                        columnDesc += "Reference: " + referenceText;
                    }

                    // Update the column description cell with the new value
                    columnDescCell.Value = columnDesc;

                    // Update the _tableContext
                    var column = _tableContext.Columns.FirstOrDefault(c => c.ColumnName == columnName);
                    if (column != null)
                    {
                        column.Description = columnDesc;

                        // resize the description column to fit new content
                        columnDefDataGridView.AutoResizeColumn(columnDefDataGridView.Columns["Description"].Index, DataGridViewAutoSizeColumnMode.AllCells);
                    }
                }
            }
        }

        /// <summary>
        /// Performs the action of adding a column reference by invoking the corresponding menu item click handler.
        /// </summary>
        internal void AddColumnReference()
        {
            ColumnReferenceToolStripMenuItem_Click(columnReferenceToolStripMenuItem, EventArgs.Empty);
        }
    }
}