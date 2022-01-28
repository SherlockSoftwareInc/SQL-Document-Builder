using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class ColumnDefView : UserControl
    {
        private readonly List<TableColumnItem> _columns = new List<TableColumnItem>();
        private ObjectName? _objectName;

        public ColumnDefView()
        {
            InitializeComponent();
        }

        public event EventHandler SelectedColumnChanged;

        public event EventHandler TableDescSelected;

        /// <summary>
        /// Gets or sets database connection
        /// </summary>
        public string ConnectionString { get; set; } = "";

        /// <summary>
        /// Gets object schema
        /// </summary>
        public string? Schema => _objectName?.Schema;

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
        public string? TableName => _objectName?.Name;

        /// <summary>
        /// Gets object type
        /// </summary>
        public ObjectName.ObjectTypeEnums? TableType => _objectName?.ObjectType;

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
                _objectName = null;
                return;
            }
            else
            {
                _objectName = objectName;
            }

            tablenameLabel.Text = objectName.FullName;

            if (ConnectionString.Length > 0)
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                try
                {
                    using SqlCommand cmd = new SqlCommand() { Connection = conn, CommandType = System.Data.CommandType.Text };
                    cmd.Parameters.Add(new SqlParameter("@Schema", Schema));
                    cmd.Parameters.Add(new SqlParameter("@TableName", TableName));
                    cmd.CommandText = "SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,IS_NULLABLE,COLUMN_DEFAULT FROM information_schema.columns WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION";
                    conn.Open();

                    _columns.Clear();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        _columns.Add(new TableColumnItem(dr));
                    }
                    dr.Close();

                    foreach (var column in _columns)
                    {
                        column.Description = Common.GetColumnDescription(_objectName, column.ColumnName);
                    }

                    columnDefDataGridView.DataSource = _columns;
                    columnDefDataGridView.AutoResizeColumns();

                    columnDefDataGridView.Visible = true;
                }
                catch (SqlException)
                {
                    return;
                }
                finally
                {
                    conn.Close();
                }
                tableLabel.Text = GetTableDesc();
            }
            else
            {
                if (columnDefDataGridView.DataSource != null)
                {
                    columnDefDataGridView.DataSource = null;
                }
            }

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
        public void SetColumnDescription(string columnName, string description)
        {
            foreach (var column in _columns)
            {
                if (column.ColumnName == columnName)
                {
                    column.Description = description;
                    columnDefDataGridView.Refresh();
                    break;
                }
            }
        }

        /// <summary>
        /// Change selected row
        /// </summary>
        private void ChangeRowSelection()
        {
            if (columnDefDataGridView.SelectedRows != null)
            {
                int rowIndex = columnDefDataGridView.CurrentCell.RowIndex;
                string columnName = columnDefDataGridView.Rows[rowIndex].Cells["ColumnName"].Value.ToString();
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
        /// Get column description
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetColumnDescription(string column)
        {
            string result = string.Empty;
            string sql;
            if (_objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.views T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", _objectName.Schema, _objectName.Name, column);
            }
            else
            {
                sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", _objectName.Schema, _objectName.Name, column);
            }
            var conn = new SqlConnection(ConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr[0].ToString();
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                //MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Get description of table/view from the database
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private string GetTableDesc()
        {
            string result = string.Empty;
            string sql = string.Format(String.Format("SELECT value FROM fn_listextendedproperty (NULL, 'schema', '{0}', '{2}', '{1}', default, default) WHERE name = N'MS_Description'", _objectName.Schema, _objectName.Name, (_objectName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")));

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr[0].ToString();
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
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