using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The column def view.
    /// </summary>
    public partial class ColumnDefView : UserControl
    {
        /// <summary>
        /// The db object.
        /// </summary>
        private DBObject _dbObject = new();

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
        }

        public event EventHandler? SelectedColumnChanged;

        public event EventHandler? TableDescSelected;

        /// <summary>
        /// Gets or sets database connection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ConnectionString { get; set; } = "";

        /// <summary>
        /// Gets object schema
        /// </summary>
        public string? Schema => _dbObject?.TableSchema;

        /// <summary>
        /// Get selected column name
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? SelectedColumn { get; private set; }

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
        public string? TableFullName => _dbObject?.ObjectName.FullName;

        /// <summary>
        /// Gets table name
        /// </summary>
        public string? TableName => _dbObject?.TableName;

        /// <summary>
        /// Gets object type
        /// </summary>
        public ObjectName.ObjectTypeEnums? TableType => _dbObject?.TableType;

        /// <summary>
        ///
        /// </summary>
        public void Clear()
        {
            tablenameLabel.Text = "";
            SelectedColumn = "";
            if (columnDefDataGridView.DataSource != null)
            {
                columnDefDataGridView.Visible = false;
                columnDefDataGridView.Columns.Clear();
                columnDefDataGridView.DataSource = null;
            }
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
        ///
        /// </summary>
        public void Copy()
        {
            //Clipboard.SetDataObject(columnDefDataGridView.GetClipboardContent());
            var currentControl = this.ActiveControl;
            if (currentControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)currentControl;
                textBox.Copy();
            }
            else if (currentControl?.GetType() == typeof(Label))
            {
                Label label = (Label)currentControl;
                Clipboard.SetText(label.Text);
            }
            else if (currentControl?.GetType() == typeof(DataGridView))
            {
                DataGridView defView = (DataGridView)currentControl;
                var value = defView.CurrentCell?.Value.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    Clipboard.SetText(value);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Cut()
        {
            var currentControl = this.ActiveControl;
            if (currentControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)currentControl;
                if (textBox.Enabled)
                {
                    textBox.Cut();
                }
            }
        }

        /// <summary>
        /// Open the database object schema
        /// </summary>
        /// <param name="objectName">The object name.</param>
        public void Open(ObjectName? objectName)
        {
            Clear();
            SelectedColumnChanged?.Invoke(this, EventArgs.Empty);

            if (objectName == null)
            {
                return;
            }
            else
            {
                _dbObject = new DBObject();
                if (!_dbObject.Open(objectName, Properties.Settings.Default.dbConnectionString))
                {
                    if (columnDefDataGridView.DataSource != null)
                    {
                        columnDefDataGridView.DataSource = null;
                    }
                    return;
                }
            }

            tablenameLabel.Text = objectName.FullName;
            TableDescription = _dbObject.Description;
            columnDefDataGridView.DataSource = _dbObject.Columns;
            columnDefDataGridView.AutoResizeColumns();

            columnDefDataGridView.Visible = true;

            if (columnDefDataGridView.Rows.Count > 0)
            {
                ChangeRowSelection();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Paste()
        {
            var currentControl = this.ActiveControl;
            if (currentControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)currentControl;
                if (textBox.Enabled)
                {
                    textBox.Paste();
                }
            }
        }

        /// <summary>
        /// Selects the all.
        /// </summary>
        public void SelectAll()
        {
            var currentControl = this.ActiveControl;
            if (currentControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)currentControl;
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Updates the column desc.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="description">The description.</param>
        public void UpdateColumnDesc(string columnName, string description, bool isView)
        {
            _dbObject.UpdateColumnDescription(columnName, description, isView);
        }

        /// <summary>
        /// Updates the table description.
        /// </summary>
        /// <param name="description">The description.</param>
        public void UpdateTableDescription(string description)
        {
            _dbObject.UpdateTableDesc(description);
            TableDescription = description;
        }

        /// <summary>
        /// Change row selection.
        /// </summary>
        private void ChangeRowSelection()
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
        /// Columns the def data grid view cell click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ColumnDefDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeRowSelection();
        }

        /// <summary>
        /// Columns the def data grid view cell double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ColumnDefDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditOptionDefinition(e.RowIndex);
        }

        /// <summary>
        /// Columns the def data grid view cell validated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ColumnDefDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex >= 0)
            {
                string columnName = (string)columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"].Value;
                string columnDesc = (string)columnDefDataGridView.Rows[rowIndex].Cells["Description"].Value;
                UpdateColumnDesc(columnName, columnDesc, _dbObject.TableType == ObjectName.ObjectTypeEnums.View);

                //await SaveOptionDefinition(optionCode, optionDefinition);
            }
        }

        /// <summary>
        /// Columns the def data grid view cell value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ColumnDefDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //changed = true;
        }

        /// <summary>
        /// Edits the option definition.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        private void EditOptionDefinition(int rowIndex)
        {
            if (rowIndex == -1) return;

            //string optionCode = columnDefDataGridView.Rows[rowIndex].Cells["Option Code"].Value.ToString();
            //string optionDefinition = columnDefDataGridView.Rows[rowIndex].Cells["Questionnaire Item Option Definition"].Value.ToString();
            //string displayText = columnDefDataGridView.Rows[rowIndex].Cells["Option Display Text"].Value.ToString();

            //using (EditForm form = new EditForm()
            //{
            //    ReadOnly = !CanEdit,
            //    DefinitionText = optionDefinition,
            //    DisplayText = displayText,
            //    OptionCode = optionCode
            //})
            //{
            //    if (form.ShowDialog() == DialogResult.OK)
            //    {
            //        if (CanEdit)
            //        {
            //            if (!optionDefinition.Equals(form.DefinitionText))
            //            {
            //                columnDefDataGridView.Rows[rowIndex].Cells["Questionnaire Item Option Definition"].Value = form.DefinitionText;
            //                await SaveOptionDefinition(optionCode, form.DefinitionText);
            //            }
            //        }
            //    }
            //}
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
        /// Table the label validated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void TableLabel_Validated(object sender, EventArgs e)
        {
            _dbObject.UpdateTableDesc(tableDescTextBox.Text);
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
    AND object_id = OBJECT_ID('[{Schema}].[{TableName}]')
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
        /// Gets the create object script.
        /// </summary>
        /// <returns>A string.</returns>
        public async Task<string?> GetCreateObjectScript()
        {
            if (string.IsNullOrEmpty(TableName))
            {
                throw new InvalidOperationException("Table name cannot be null or empty.");
            }

            // if the table is a view, return the create view script
            if (TableType == ObjectName.ObjectTypeEnums.View)
            {
                StringBuilder createViewScript = new();

                // Add the header
                createViewScript.AppendLine($"/****** Object:  View [{Schema}].[{TableName}] ******/");

                // Add drop table statement
                createViewScript.AppendLine($"IF OBJECT_ID('[{Schema}].[{TableName}]', 'V') IS NOT NULL");
                createViewScript.AppendLine($"\tDROP VIEW [{Schema}].[{TableName}];");
                createViewScript.AppendLine($"GO");

                var script = await _dbObject.GetViewDefinitionAsync();
                createViewScript.Append(script);
                createViewScript.AppendLine($"GO");
                return createViewScript.ToString();
            }
            else
            {
                return GetCreateTableScript();
            }
        }

        /*
                /// <summary>
                /// Gets the create table script.
                /// </summary>
                /// <param name="connectionString">The connection string.</param>
                /// <param name="tableName">The table name.</param>
                /// <returns>A string.</returns>
                public string GetCreateTableScript()
                {
                    StringBuilder createTableScript = new();

                    // Add the header
                    createTableScript.AppendLine($"--****** Object:  Table [{Schema}].[{TableName}] ******");

                    // Add drop table statement
                    createTableScript.AppendLine($"IF OBJECT_ID('[{Schema}].[{TableName}]', 'U') IS NOT NULL");
                    createTableScript.AppendLine($"\tDROP TABLE [{Schema}].[{TableName}];");
                    createTableScript.AppendLine($"GO");

                    // Get the primary key column names that the ColID ends with "🗝"
                    string primaryKeyColumns = string.Empty;
                    for (int i = 0; i < columnDefDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = columnDefDataGridView.Rows[i];
                        if (row.IsNewRow) continue; // Skip the new row placeholder
                        string colId = row.Cells["ColID"].Value?.ToString() ?? string.Empty;
                        if (colId.EndsWith("🗝"))
                        {
                            string columnName = row.Cells["ColumnName"].Value?.ToString() ?? throw new InvalidOperationException("Column name cannot be null.");
                            primaryKeyColumns += $"[{columnName}], ";
                        }
                    }
                    if (primaryKeyColumns.Length > 0)
                    {
                        primaryKeyColumns = primaryKeyColumns.TrimEnd(',', ' ');
                    }

                    // Add the CREATE TABLE statement
                    createTableScript.AppendLine($"CREATE TABLE [{Schema}].[{TableName}] (");

                    // Iterate through the rows in the DataGridView
                    for (int i = 0; i < columnDefDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = columnDefDataGridView.Rows[i];
                        if (row.IsNewRow) continue; // Skip the new row placeholder

                        // Safely retrieve column values
                        string columnName = row.Cells["ColumnName"].Value?.ToString() ?? throw new InvalidOperationException("Column name cannot be null.");
                        string dataType = row.Cells["DataType"].Value?.ToString() ?? throw new InvalidOperationException($"Data type for column '{columnName}' cannot be null.");
                        string isNullable = Convert.ToBoolean(row.Cells["Nullable"].Value) ? "NULL" : "NOT NULL";

                        // Append the column definition
                        createTableScript.Append($"\t[{columnName}] {dataType} {isNullable}");

                        // Add a comma if it's not the last valid row
                        if (i < columnDefDataGridView.Rows.Count - 1 && !columnDefDataGridView.Rows[i + 1].IsNewRow)
                        {
                            createTableScript.AppendLine(",");
                        }
                        else if (string.IsNullOrEmpty(primaryKeyColumns))
                        {
                            createTableScript.AppendLine(); // No primary key, just end the line
                        }
                        else
                        {
                            createTableScript.AppendLine(","); // Add a comma if primary key exists
                        }
                    }

                    // Add the primary key constraint if it exists
                    if (!string.IsNullOrEmpty(primaryKeyColumns))
                    {
                        createTableScript.AppendLine($"\tCONSTRAINT PK_{Schema}_{TableName} PRIMARY KEY ({primaryKeyColumns})");
                    }

                    createTableScript.AppendLine(");");
                    createTableScript.AppendLine($"GO");

                    var indexScript = _dbObject.GetCreateIndexesScript($"[{Schema}].[{TableName}]");
                    if (!string.IsNullOrEmpty(indexScript))
                    {
                        // remove the new line at the end of the script
                        indexScript = indexScript.TrimEnd('\r', '\n');

                        createTableScript.AppendLine(indexScript);
                        createTableScript.AppendLine($"GO");
                    }

                    return createTableScript.ToString();
                }
        */

        /// <summary>
        /// Gets the create table script.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetCreateTableScript()
        {
            StringBuilder createTableScript = new();

            // Add the header
            createTableScript.AppendLine($"/****** Object:  Table [{Schema}].[{TableName}] ******/");

            // Add drop table statement
            createTableScript.AppendLine($"IF OBJECT_ID('[{Schema}].[{TableName}]', 'U') IS NOT NULL");
            createTableScript.AppendLine($"\tDROP TABLE [{Schema}].[{TableName}];");
            createTableScript.AppendLine($"GO");

            // Get the primary key column names that the ColID ends with "🗝"
            string primaryKeyColumns = string.Empty;
            for (int i = 0; i < columnDefDataGridView.Rows.Count; i++)
            {
                DataGridViewRow row = columnDefDataGridView.Rows[i];
                if (row.IsNewRow) continue; // Skip the new row placeholder
                string colId = row.Cells["ColID"].Value?.ToString() ?? string.Empty;
                if (colId.EndsWith("🗝"))
                {
                    string columnName = row.Cells["ColumnName"].Value?.ToString() ?? throw new InvalidOperationException("Column name cannot be null.");
                    primaryKeyColumns += $"[{columnName}], ";
                }
            }
            if (primaryKeyColumns.Length > 0)
            {
                primaryKeyColumns = primaryKeyColumns.TrimEnd(',', ' ');
            }

            // Retrieve identity column details
            var identityColumns = DBObject.GetIdentityColumns(Schema, TableName);

            // Add the CREATE TABLE statement
            createTableScript.AppendLine($"CREATE TABLE [{Schema}].[{TableName}] (");

            // Iterate through the rows in the DataGridView
            for (int i = 0; i < columnDefDataGridView.Rows.Count; i++)
            {
                DataGridViewRow row = columnDefDataGridView.Rows[i];
                if (row.IsNewRow) continue; // Skip the new row placeholder

                // Safely retrieve column values
                string columnName = row.Cells["ColumnName"].Value?.ToString() ?? throw new InvalidOperationException("Column name cannot be null.");
                string dataType = row.Cells["DataType"].Value?.ToString() ?? throw new InvalidOperationException($"Data type for column '{columnName}' cannot be null.");
                string isNullable = Convert.ToBoolean(row.Cells["Nullable"].Value) ? "NULL" : "NOT NULL";

                // Check if the column is an identity column
                if (identityColumns.TryGetValue(columnName, out var identityInfo))
                {
                    createTableScript.Append($"\t[{columnName}] {dataType} IDENTITY({identityInfo.SeedValue}, {identityInfo.IncrementValue}) {isNullable}");
                }
                else
                {
                    createTableScript.Append($"\t[{columnName}] {dataType} {isNullable}");
                }

                // Add a comma if it's not the last valid row
                if (i < columnDefDataGridView.Rows.Count - 1 && !columnDefDataGridView.Rows[i + 1].IsNewRow)
                {
                    createTableScript.AppendLine(",");
                }
                else if (string.IsNullOrEmpty(primaryKeyColumns))
                {
                    createTableScript.AppendLine(); // No primary key, just end the line
                }
                else
                {
                    createTableScript.AppendLine(","); // Add a comma if primary key exists
                }
            }

            // Add the primary key constraint if it exists
            if (!string.IsNullOrEmpty(primaryKeyColumns))
            {
                createTableScript.AppendLine($"\tCONSTRAINT PK_{Schema}_{TableName} PRIMARY KEY ({primaryKeyColumns})");
            }

            createTableScript.AppendLine(");");
            createTableScript.AppendLine($"GO");

            var indexScript = _dbObject.GetCreateIndexesScript($"[{Schema}].[{TableName}]");
            if (!string.IsNullOrEmpty(indexScript))
            {
                // remove the new line at the end of the script
                indexScript = indexScript.TrimEnd('\r', '\n');

                createTableScript.AppendLine(indexScript);
                createTableScript.AppendLine($"GO");
            }

            return createTableScript.ToString();
        }

        /// <summary>
        /// Gets the table dependency script.
        /// </summary>
        /// <returns>A string.</returns>
        private string GetTableDependencyScript()
        {
            var viewList = DBObject.GetViewsUsingTable(Schema, TableName);

            if (viewList != null && viewList.Count > 0)
            {
                // build the view list string
                var viewListString = string.Join(", ", viewList.Select(v => $"[{v.SchemaName}].[{v.ViewName}]"));

                return $"This table, [{Schema}].[{TableName}], serves as a reference table defining .... It is utilized in the views: {viewListString}.";
            }

            return string.Empty;
        }

        /// <summary>
        /// Handles the column value frequency tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ColumnValueFrequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    DatabaseIndex = 0
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
        /// Handles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableDescTextBox.Paste();
            _dbObject.UpdateTableDesc(tableDescTextBox.Text);
        }

        /// <summary>
        /// Handles the add description tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
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
            description += GetTableDependencyScript();

            tableDescTextBox.Text = description;
            _dbObject.UpdateTableDesc(description);
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = $@"SELECT * FROM [{Schema}].[{TableName}]";
            using var dlg = new DataViewForm()
            {
                SQL = sql,
                MultipleValue = false,
                Text = $"{TableName}",
                TableName = $"[{Schema}].[{TableName}]",
                EnableValueFrequency = true,
                DatabaseIndex = 0
            };
            dlg.ShowDialog();
        }
    }
}