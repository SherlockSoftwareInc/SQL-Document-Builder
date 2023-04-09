using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class BatchColumnDesc : Form
    {
        private DataTable _tables = new();

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
            objectsListBox.Items.Clear();

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
                string schemaName = string.Empty;
                if (schemaComboBox.SelectedIndex > 0) schemaName = (string)schemaComboBox.Items[schemaComboBox.SelectedIndex];

                while (dr.Read())
                {
                    if (schemaName.Length > 0)
                    {
                        string tableSchema = (string)dr["TABLE_SCHEMA"];
                        if (tableSchema.Equals(schemaName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            objectsListBox.Items.Add(string.Format("{0}.{1}", dr["TABLE_SCHEMA"].ToString(), dr["TABLE_NAME"].ToString()));
                        }
                    }
                    else
                    {
                        objectsListBox.Items.Add(string.Format("{0}.{1}", dr["TABLE_SCHEMA"].ToString(), dr["TABLE_NAME"].ToString()));
                    }
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
            switch (objectsListBox.Items.Count)
            {
                case 0:
                    messageToolStripStatusLabel.Text = "No match found";
                    break;

                case 1:
                    messageToolStripStatusLabel.Text = "one matche found";
                    break;

                default:
                    messageToolStripStatusLabel.Text = string.Format("{0} matches found", objectsListBox.Items.Count);
                    break;
            }

            SelectAllToolStripButton_Click(this, EventArgs.Empty);
        }

        private void SearchToolStripButton_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private async void ApplyButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.CheckedItems.Count > 0)
            {
                var desc = descTextBox.Text.Trim();
                var columnName = searchToolStripTextBox.Text.Trim();
                if (desc.Length > 0 && columnName.Length > 0)
                {
                    Cursor = Cursors.WaitCursor;
                    progressBar.Maximum = objectsListBox.CheckedItems.Count;
                    progressBar.Value = 0;
                    progressBar.Visible = true;

                    // Set up an IProgress<int> instance to update the progress bar.
                    var progress = new Progress<int>(value => progressBar.Value = value);

                    // DoProcessing is run on the thread pool.
                    await Task.Run(() => ApplyDescriptons(columnName, desc, progress));

                    progressBar.Visible = false;
                    Cursor = Cursors.Default;
                    messageToolStripStatusLabel.Text = "Complete";
                }
            }
        }

        /// <summary>
        /// Apply column description for selected objects
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="desc"></param>
        /// <param name="progress"></param>
        private void ApplyDescriptons(string columnName, string desc, IProgress<int> progress)
        {
            for (int i = 0; i < objectsListBox.CheckedItems.Count; i++)
            {
                if (i % 10 == 0)
                {
                    //var percentComplete = (i * 100) / objectsListBox.CheckedItems.Count;
                    progress.Report(i);
                }
                var item = objectsListBox.CheckedItems[i];
                if (item != null)
                {
                    var objectName = item.ToString();
                    SaveColumnDesc(objectName, columnName, desc);
                }
            }
        }

        /// <summary>
        /// Save the description of the selected column
        /// </summary>
        private static void SaveColumnDesc(string? objectName, string columnName, string desc)
        {
            SqlConnection? conn = new(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("ADMIN.usp_AddColumnDescription", conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("@TableName", objectName));
                cmd.Parameters.Add(new SqlParameter("@ColumnName", columnName));
                cmd.Parameters.Add(new SqlParameter("@Description", desc));

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
            /*
            var names = objectName.Split('.');
            var schema = names[0];
            var tableName = names[1];

            //EXEC  'PCRL1.AfReferral', 'Priority', N'Priority at time of referral'

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
            */
        }

        private static string GetTableTyp(string schema, string tableName)
        {
            string result = "table";
            using (SqlConnection conn = new(Properties.Settings.Default.dbConnectionString))
            {
                try
                {
                    using SqlCommand cmd = new() { Connection = conn };
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
            for (int i = 0; i < objectsListBox.Items.Count; i++)
            {
                objectsListBox.SetItemChecked(i, true);
            }
        }

        private void UnselectAllToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objectsListBox.Items.Count; i++)
            {
                objectsListBox.SetItemChecked(i, false);
            }
        }

        private void BatchColumnDesc_Load(object sender, EventArgs e)
        {
            GetTableList();
            PopulateSchema();
        }

        /// <summary>
        /// Get table list from the database
        /// </summary>
        private void GetTableList()
        {
            var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_SCHEMA, TABLE_NAME", conn) { CommandType = CommandType.Text };
                conn.Open();
                var ds = new DataSet();
                var dat = new SqlDataAdapter(cmd);
                dat.Fill(ds);
                if (ds.Tables.Count > 0)
                { _tables = ds.Tables[0]; }
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

        private void PopulateSchema()
        {
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");

            var dtSchemas = _tables.DefaultView.ToTable(true, "TABLE_SCHEMA");
            var schemas = new List<string>();
            foreach (DataRow dr in dtSchemas.Rows)
            {
                schemas.Add((string)dr[0]);
            }
            schemas.Sort();
            foreach (var item in schemas)
            {
                schemaComboBox.Items.Add(item);
            }
        }

        ///// <summary>
        ///// Populate the object list box
        ///// </summary>
        //private void Populate()
        //{
        //    string schemaName = string.Empty;
        //    if (schemaComboBox.SelectedIndex > 0) schemaName = schemaComboBox.Items[schemaComboBox.SelectedIndex].ToString();

        //    if (_tables != null)
        //    {
        //        objectsListBox.Items.Clear();
        //        string searchFor = searchTextBox.Text.Trim();
        //        if (searchFor?.Length == 0)
        //        {
        //            if (schemaName?.Length == 0)
        //            {
        //                foreach (DataRow row in _tables.Rows)
        //                {
        //                    AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
        //                }
        //            }
        //            else
        //            {
        //                foreach (DataRow row in _tables.Rows)
        //                {
        //                    if (schemaName.Equals(row["TABLE_SCHEMA"].ToString(), StringComparison.CurrentCultureIgnoreCase))
        //                        AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var matches = _tables.Select(string.Format("TABLE_NAME LIKE '%{0}%'", searchFor));
        //            if (schemaName?.Length == 0)
        //            {
        //                foreach (DataRow row in matches)
        //                {
        //                    AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
        //                }
        //            }
        //            else
        //            {
        //                foreach (DataRow row in matches)
        //                {
        //                    if (schemaName.Equals(row["TABLE_SCHEMA"].ToString(), StringComparison.CurrentCultureIgnoreCase))
        //                        AddListItem((string)row["TABLE_TYPE"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void AddListItem(string tableType, string schema, string tableName)
        //{
        //    ObjectName.ObjectTypeEnums objectType = (tableType == "VIEW") ? ObjectName.ObjectTypeEnums.View : ObjectName.ObjectTypeEnums.Table;
        //    objectsListBox.Items.Add(new ObjectName()
        //    {
        //        ObjectType = objectType,
        //        Schema = schema,
        //        Name = tableName
        //    });
        //}

        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void searchToolStripTextBox_TextBoxTextAlignChanged(object sender, EventArgs e)
        {
            messageToolStripStatusLabel.Text = string.Empty;
        }
    }
}