using System;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class ColumnDefView : UserControl
    {
        private DBObject _dbObject = new();

        public ColumnDefView()
        {
            InitializeComponent();
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
        public string? TableName => _dbObject?.TableName;

        /// <summary>
        /// Gets table name
        /// </summary>
        public string? TableFullName => _dbObject?.ObjectName.FullName;

        /// <summary>
        /// Gets object type
        /// </summary>
        public ObjectName.ObjectTypeEnums? TableType => _dbObject?.TableType;

        /// <summary>
        /// Clear the control
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
        /// Open database object and populate its columns' info
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
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
        /// Set the description of a column
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="description"></param>
        public void UpdateColumnDesc(string columnName, string description)
        {
            _dbObject.UpdateColumnDesc(columnName, description);
        }

        public void UpdateTableDescription(string description)
        {
            _dbObject.UpdateTableDesc(description);
            TableDescription = description;
        }

        /// <summary>
        /// Gets description of a specified column
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string ColumnDescription(string columnName)
        {
            for (int i = 0; i < _dbObject.Columns.Count; i++)
            {
                var column = _dbObject.Columns[i];
                if(column.ColumnName == columnName) { return column.Description; }
            }
            return string.Empty;
        }

        /// <summary>
        /// Change selected row
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
        /// Handle data grid cell click event:
        ///     Tell host that selected column changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnDefDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeRowSelection();
        }

        /// <summary>
        /// Handles table description lable click event: Raise an event to indicate table description has been selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableLabel_Click(object sender, EventArgs e)
        {
            TableDescSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}