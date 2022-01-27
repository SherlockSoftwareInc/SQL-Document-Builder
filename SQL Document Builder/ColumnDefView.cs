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

        public ColumnDefView()
        {
            InitializeComponent();
        }

        public event EventHandler SelectedColumnChanged;

        public string ConnectionString { get; set; } = "";

        public string Schema { get; set; }

        public string TableName { get; set; }

        public string SelectedColumn { get; private set; }

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
                    column.Description = GetColumnDescription(Schema, TableName, column.ColumnName);
                    columnDefDataGridView.Refresh();
                }
            }
        }

        /// <summary>
        /// Open database object and populate its columns' info
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        public void Open(string objectName)
        {
            Clear();
            SelectedColumnChanged?.Invoke(this, EventArgs.Empty);

            if (objectName.Length == 0 || objectName.IndexOf(".") < 0)
            {
                return;
            }

            var names = objectName.Split('.');
            if (names.Length == 2)
            {
                Schema = names[0];
                TableName = names[1];
            }
            else
            {
                return;
            }

            tablenameLabel.Text = objectName;

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
                        column.Description = GetColumnDescription(Schema, TableName, column.ColumnName);
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
        /// Get column description
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetColumnDescription(string schema, string table, string column)
        {
            string result = string.Empty;
            string sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", schema, table, column);
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
        /// Handle data grid cell click event:
        ///     Tell host that selected column changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnDefDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ChangeRowSelection();
        }

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
    }
}