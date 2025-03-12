using System;
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
        public string ConnectionString { get; set; } = "";

        /// <summary>
        /// Gets object schema
        /// </summary>
        public string? Schema => _dbObject?.TableSchema;

        /// <summary>
        /// Get selected column name
        /// </summary>
        public string? SelectedColumn { get; private set; }

        /// <summary>
        /// Gets or sets table description
        /// </summary>
        public string TableDescription
        {
            get { return tableLabel.Text; }
            set { tableLabel.Text = value; }
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
            _dbObject.UpdateTableDesc(tableLabel.Text);
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
    ALTER TABLE [{Schema}].[{TableName}] 
    ADD CONSTRAINT PK_{Schema}_{TableName} PRIMARY KEY ({columnName})
END";
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
END";
            }

            return sql;
        }


    }
}