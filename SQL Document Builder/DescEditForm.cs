using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class DescEditForm : Form
    {
        private bool _descChanged = false;

        public DescEditForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Object name to open
        /// </summary>
        public ObjectName TableName { get; set; }

        /// <summary>
        /// Handles column view selected column change event:
        ///     1. Save changes
        ///     2. Show selected column name and description in the edit panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnDefView1_SelectedColumnChanged(object sender, EventArgs e)
        {
            SaveChange();

            string selectedColumn = columnView.SelectedColumn;
            if (selectedColumn.Length == 0)
            {
                descTextBox.Text = string.Empty;
            }
            else
            {
                titleLabel.Text = "Column:";
                columnNameLabel.Text = selectedColumn;
                descTextBox.Text = Common.GetColumnDescription(TableName, selectedColumn);
                _descChanged = false;
            }
        }

        /// <summary>
        /// Handles form closing event: Ensure change has been saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveChange();
        }

        /// <summary>
        /// Handles form load event: Open the object in the column view control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescEditForm_Load(object sender, EventArgs e)
        {
            columnView.ConnectionString = Properties.Settings.Default.dbConnectionString;
            columnView.Open(TableName);
        }

        /// <summary>
        /// Handles description text box text change event: set change indicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescTextBox_TextChanged(object sender, EventArgs e)
        {
            _descChanged = true;
        }

        /// <summary>
        /// Handles description text box validated event: Save the change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescTextBox_Validated(object sender, EventArgs e)
        {
            SaveChange();
        }

        /// <summary>
        /// Checks if column description exists
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool IsColumnDescExists(string schema, string table, string column)
        {
            bool result = false;
            string sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.{3} T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", schema, table, column, TableName.ObjectType == ObjectName.ObjectTypeEnums.View ? "views" : "tables");
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = true;
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
        /// Checks if table/view description exists
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private bool IsTableDescExists(string schema, string table)
        {
            bool result = false;
            string sql = string.Format(String.Format("SELECT value FROM fn_listextendedproperty (NULL, 'schema', '{0}', '{2}', '{1}', default, default) WHERE name = N'MS_Description'", schema, table, (TableName.ObjectType == ObjectName.ObjectTypeEnums.View ? "view" : "table")));

            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = true;
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
        /// Save the change
        /// </summary>
        private void SaveChange()
        {
            if (_descChanged && columnView.TableName.Length > 0)
            {
                if (columnNameLabel.Text.Length > 0)
                {
                    SaveColumnDesc();
                }
                else
                {
                    SaveTableDesc();
                }
                _descChanged = false;
            }
        }

        /// <summary>
        /// Save the description of the selected column
        /// </summary>
        private void SaveColumnDesc()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                string spName;
                if (IsColumnDescExists(columnView.Schema, columnView.TableName, columnNameLabel.Text))
                {
                    spName = "sp_updateextendedproperty";
                }
                else
                {
                    spName = "sp_addextendedproperty";
                }
                var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("@name", "MS_Description"));
                cmd.Parameters.Add(new SqlParameter("@value", descTextBox.Text));
                cmd.Parameters.Add(new SqlParameter("@level0type ", "Schema"));
                cmd.Parameters.Add(new SqlParameter("@level0name", columnView.Schema));
                cmd.Parameters.Add(new SqlParameter("@level1type", TableName.ObjectType == ObjectName.ObjectTypeEnums.Table ? "Table" : "View"));
                cmd.Parameters.Add(new SqlParameter("@level1name", columnView.TableName));
                cmd.Parameters.Add(new SqlParameter("@level2type", "Column"));
                cmd.Parameters.Add(new SqlParameter("@level2name", columnNameLabel.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            columnView.SetColumnDescription(columnNameLabel.Text, descTextBox.Text);
        }

        /// <summary>
        /// Save the description of the object
        /// </summary>
        private void SaveTableDesc()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                string spName;
                if (IsTableDescExists(TableName.Schema, TableName.Name))
                {
                    spName = "sp_updateextendedproperty";
                }
                else
                {
                    spName = "sp_addextendedproperty";
                }
                var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("@name", "MS_Description"));
                cmd.Parameters.Add(new SqlParameter("@value", descTextBox.Text));
                cmd.Parameters.Add(new SqlParameter("@level0type ", "Schema"));
                cmd.Parameters.Add(new SqlParameter("@level0name", columnView.Schema));
                cmd.Parameters.Add(new SqlParameter("@level1type", TableName.ObjectType == ObjectName.ObjectTypeEnums.Table ? "Table" : "View"));
                cmd.Parameters.Add(new SqlParameter("@level1name", columnView.TableName));
                //cmd.Parameters.Add(new SqlParameter("@level2type", DBNull.Value));
                //cmd.Parameters.Add(new SqlParameter("@level2name", DBNull.Value));

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            columnView.TableDescription = descTextBox.Text;
        }

        private void ColumnView_TableDescSelected(object sender, EventArgs e)
        {
            titleLabel.Text = (TableName.ObjectType == ObjectName.ObjectTypeEnums.View ? "View: " : "Table: ") + TableName.FullName;
            columnNameLabel.Text = string.Empty;
            descTextBox.Text = columnView.TableDescription;
            _descChanged = false;
        }
    }
}