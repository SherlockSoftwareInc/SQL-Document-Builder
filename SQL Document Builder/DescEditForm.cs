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

        public string ObjectName { get; set; }

        private void columnDefView1_SelectedColumnChanged(object sender, EventArgs e)
        {
            SaveChange();

            string selectedColumn = columnView.SelectedColumn;
            if (selectedColumn.Length == 0)
            {
                descTextBox.Text = string.Empty;
            }
            else
            {
                columnNameLabel.Text = selectedColumn;
                descTextBox.Text = Common.GetColumnDesc(columnView.Schema, columnView.TableName, selectedColumn);
                _descChanged = false;
            }
        }

        private void SaveChange()
        {
            if (_descChanged && columnView.TableName.Length > 0 && columnNameLabel.Text.Length > 0)
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
                    cmd.Parameters.Add(new SqlParameter("@level1type", "Table"));
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
                _descChanged = false;
            }
        }

        private bool IsColumnDescExists(string schema, string table, string column)
        {
            bool result = false;
            string sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.tables T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", schema, table, column);
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
               Common. MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        private void DescEditForm_Load(object sender, EventArgs e)
        {
            columnView.ConnectionString = Properties.Settings.Default.dbConnectionString;
            columnView.Open(ObjectName);
        }

        private void descTextBox_TextChanged(object sender, EventArgs e)
        {
            _descChanged = true;
        }

        private void descTextBox_Validated(object sender, EventArgs e)
        {
            SaveChange();
        }

        private void DescEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveChange();
        }
    }
}