using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class BatchColumnDesc : Form
    {
        public BatchColumnDesc()
        {
            InitializeComponent();
        }

        private void SearchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
            }
        }

        private void PerformSearch()
        {
            tablesCheckedListBox.Items.Clear();

            string searchFor = searchToolStripTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchFor))
            {
                return;
            }

            using SqlConnection conn = new(Properties.Settings.Default.dbConnectionString);
            try
            {
                using SqlCommand cmd = new() { Connection = conn };
                cmd.Parameters.Add(new SqlParameter("@searchFor", searchFor));
                cmd.CommandText = "SELECT DISTINCT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, 'BASE TABLE' TABLE_TYPE, COLUMN_NAME FROM information_schema.columns WHERE column_name = @SearchFor ORDER BY TABLE_NAME";

                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    tablesCheckedListBox.Items.Add(string.Format("{0}.{1}", dr["TABLE_SCHEMA"].ToString(), dr["TABLE_NAME"].ToString()));
                }
                dr.Close();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            SelectAllToolStripButton_Click(this, EventArgs.Empty);
        }

        private void SearchToolStripButton_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            var desc = descTextBox.Text.Trim();
            var columnName = searchToolStripTextBox.Text.Trim();
            if (desc.Length > 0 && columnName.Length > 0)
            {
                foreach (var item in tablesCheckedListBox.CheckedItems)
                {
                    SaveColumnDesc(item.ToString(), columnName, desc);
                }
            }
        }

        /// <summary>
        /// Save the description of the selected column
        /// </summary>
        private void SaveColumnDesc(string objectName, string columnName, string desc)
        {
            var names = objectName.Split('.');
            var schema = names[0];
            var tableName = names[1];

            if (!IsColumnDescExists(schema, tableName, columnName))
            {
                SqlConnection? conn = new(Properties.Settings.Default.dbConnectionString);
                try
                {
                    var cmd = new SqlCommand("sp_addextendedproperty", conn) { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.Add(new SqlParameter("@name", "MS_Description"));
                    cmd.Parameters.Add(new SqlParameter("@value", desc));
                    cmd.Parameters.Add(new SqlParameter("@level0type ", "Schema"));
                    cmd.Parameters.Add(new SqlParameter("@level0name", schema));
                    cmd.Parameters.Add(new SqlParameter("@level1type", GetTableTyp(schema, tableName)));
                    cmd.Parameters.Add(new SqlParameter("@level1name", tableName));
                    cmd.Parameters.Add(new SqlParameter("@level2type", "Column"));
                    cmd.Parameters.Add(new SqlParameter("@level2name", columnName));

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
            }
        }

        private static string GetTableTyp(string schema, string tableName)
        {
            string result = "table";
            using (SqlConnection conn = new(Properties.Settings.Default.dbConnectionString))
            {
                try
                {
                    using (SqlCommand cmd = new() { Connection = conn })
                    {
                        cmd.Parameters.Add(new SqlParameter("@schema", schema));
                        cmd.Parameters.Add(new SqlParameter("@tableName", tableName));
                        cmd.CommandText = "SELECT TABLE_TYPE FROM information_schema.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName";
                        conn.Open();
                        var dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            if (string.Compare(dr["TABLE_TYPE"].ToString(), "View", true) == 0)
                            {
                                result = "view";
                            }
                        }
                        dr.Close();
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if column description exists
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static bool IsColumnDescExists(string schema, string table, string column)
        {
            bool result = false;
            string sql = string.Format("SELECT E.value Description FROM sys.schemas S INNER JOIN sys.{3}s T ON S.schema_id = T.schema_id INNER JOIN sys.columns C ON T.object_id = C.object_id INNER JOIN sys.extended_properties E ON T.object_id = E.major_id AND C.column_id = E.minor_id AND E.name = 'MS_Description' AND S.name = '{0}' AND T.name = '{1}' AND C.name = '{2}'", schema, table, column, GetTableTyp(schema, table));
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

        private void SelectAllToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tablesCheckedListBox.Items.Count; i++)
            {
                tablesCheckedListBox.SetItemChecked(i, true);
            }
        }

        private void UnselectAllToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tablesCheckedListBox.Items.Count; i++)
            {
                tablesCheckedListBox.SetItemChecked(i, false);
            }
        }
    }
}