using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class MigrateForm : Form
    {
        private DataTable? _table;

        public MigrateForm()
        {
            InitializeComponent();
        }

        private void GroupBox1_Resize(object sender, EventArgs e)
        {
            if (objectGroupBox.Width > 100)
                commentsTextBox.Width = objectGroupBox.Width - 100;
        }

        private void MigrateForm_Load(object sender, EventArgs e)
        {
            LoadSourceData();
        }

        private void LoadSourceData()
        {
            using (var conn = new SqlConnection(Properties.Settings.Default.edwConnectionString))
            {
                try
                {
                    using (var cmd = new SqlCommand()
                    {
                        CommandText = "SELECT * FROM [ADMIN].[TablesToImport]",
                        CommandType = CommandType.Text,
                        Connection = conn
                    })
                    {
                        conn.Open();
                        var dat = new SqlDataAdapter(cmd);
                        var ds = new DataSet();
                        dat.Fill(ds);
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                _table = ds.Tables[0];
                                toolStripStatusLabel1.Text = String.Format("Total rows: {0}", _table.Rows.Count.ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }

            Populate();
        }

        private void Populate()
        {
            _currentTableName = string.Empty;
            _currentSchema = string.Empty;
            _currentIndex = -1;

            if (_table == null) return;

            _condition = string.Empty;
            if (schemaComboBox.SelectedIndex > 0)
                _condition = "TABLE_SCHEMA = '" + schemaComboBox.Text + "'";

            if (tableTextBox.Text.Length > 0)
            {
                AppendFileCondition(String.Format("TABLE_NAME LIKE '%{0}%'", tableTextBox.Text.Replace("'", "''")));
            }

            AppendFileCondition(String.Format("[NeedToMigrate] = {0}", needMigrateCheckBox.Checked ? "1" : "0"));

            AppendFileCondition(String.Format("[Imported] = {0}", importedCheckBox.Checked ? "1" : "0"));

            AppendFileCondition(String.Format("[PostImportProcess] = {0}", postMigrateCheckBox.Checked ? "1" : "0"));

            AppendFileCondition(String.Format("[WikiDone] = {0}", wikiCheckBox.Checked ? "1" : "0"));

            DataView view = new DataView()
            {
                Table = _table,
                RowFilter = _condition,
                Sort = "TABLE_NAME ASC"
            };

            dataGridView1.DataSource = view;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            rowsToolStripStatusLabel.Text = String.Format("Rows {0}", dataGridView1.Rows.Count);

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
                PopulateObjectItem();
            }
            else
            {
                ClearEditor();
            }
        }

        private void AppendFileCondition(string condition)
        {
            if (_condition.Length == 0)
            {
                _condition = condition;
            }
            else
            {
                _condition += " AND " + condition;
            }
        }

        private string _condition = string.Empty;

        private void schemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Populate();
        }

        private bool _ignoreChange = false;

        private void ClearEditor()
        {
            objectGroupBox.Text = String.Empty;
            editMigrateCheckBox.Checked = false;
            editImportedCheckBox.Checked = false;
            editPostMigrateCheckBox.Checked = false;
            editWikiCheckBox.Checked = false;
            commentsTextBox.Text = String.Empty;
        }

        private string _currentSchema = string.Empty;
        private string _currentTableName = string.Empty;
        private int _currentIndex = -1;

        private void PopulateObjectItem()
        {
            if (dataGridView1.CurrentRow == null)
            {
                ClearEditor();
            }
            else
            {
                _ignoreChange = true;
                var rowIndex = dataGridView1.CurrentRow.Index;
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    _currentIndex = rowIndex;
                    _currentSchema = (string)dataGridView1.Rows[rowIndex].Cells["TABLE_SCHEMA"].Value;
                    _currentTableName = (string)dataGridView1.Rows[rowIndex].Cells["TABLE_NAME"].Value;
                    objectGroupBox.Text = String.Format("{0}.{1}", _currentSchema, _currentTableName);
                    editMigrateCheckBox.Checked = (bool)dataGridView1.Rows[rowIndex].Cells["NeedToMigrate"].Value;
                    editImportedCheckBox.Checked = (bool)dataGridView1.Rows[rowIndex].Cells["Imported"].Value;
                    editPostMigrateCheckBox.Checked = (bool)dataGridView1.Rows[rowIndex].Cells["PostImportProcess"].Value;
                    editWikiCheckBox.Checked = (bool)dataGridView1.Rows[rowIndex].Cells["WikiDone"].Value;
                    commentsTextBox.Text = (string)dataGridView1.Rows[rowIndex].Cells["Comments"].Value;
                }

                _ignoreChange = false;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PopulateObjectItem();
        }

        private void editItem_DataChanged(object sender, EventArgs e)
        {
            if (!_ignoreChange && _currentTableName.Length > 0)
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.edwConnectionString))
                {
                    try
                    {
                        using (var cmd = new SqlCommand()
                        {
                            CommandText = "[ADMIN].[usp_UpdateMigrateTaskItem]",
                            CommandType = CommandType.StoredProcedure,
                            Connection = conn
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@TABLE_SCHEMA", _currentSchema));
                            cmd.Parameters.Add(new SqlParameter("@TABLE_NAME", _currentTableName));
                            cmd.Parameters.Add(new SqlParameter("@NeedToMigrate", editMigrateCheckBox.Checked));
                            cmd.Parameters.Add(new SqlParameter("@Imported", editImportedCheckBox.Checked));
                            cmd.Parameters.Add(new SqlParameter("@PostImportProcess", editPostMigrateCheckBox.Checked));
                            cmd.Parameters.Add(new SqlParameter("@WikiDone", editWikiCheckBox.Checked));
                            cmd.Parameters.Add(new SqlParameter("@Comments", commentsTextBox.Text));

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                if (_currentIndex >= 0)
                {
                    var schema = dataGridView1.Rows[_currentIndex].Cells["TABLE_SCHEMA"].Value.ToString();
                    var tableName = dataGridView1.Rows[_currentIndex].Cells["TABLE_NAME"].Value.ToString();
                    if (_currentSchema.Equals(schema) && _currentTableName.Equals(tableName))
                    {
                        dataGridView1.Rows[_currentIndex].Cells["NeedToMigrate"].Value = editMigrateCheckBox.Checked;
                        dataGridView1.Rows[_currentIndex].Cells["Imported"].Value = editImportedCheckBox.Checked;
                        dataGridView1.Rows[_currentIndex].Cells["PostImportProcess"].Value = editPostMigrateCheckBox.Checked;
                        dataGridView1.Rows[_currentIndex].Cells["WikiDone"].Value = editWikiCheckBox.Checked;
                        dataGridView1.Rows[_currentIndex].Cells["Comments"].Value = commentsTextBox.Text;
                        dataGridView1.Refresh();
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadSourceData();
            if (dataGridView1.Rows.Count > 0 && _currentIndex >= 0 && _currentIndex < dataGridView1.Rows.Count)
            {
                dataGridView1.Rows[_currentIndex].Selected = true;
            }
        }
    }
}