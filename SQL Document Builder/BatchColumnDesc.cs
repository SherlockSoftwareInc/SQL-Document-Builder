using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The batch column desc.
    /// </summary>
    public partial class BatchColumnDesc : Form
    {
        private DataTable _tables = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchColumnDesc"/> class.
        /// </summary>
        public BatchColumnDesc()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the table typ.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="tableName">The table name.</param>
        /// <returns>A string.</returns>
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

        /// <summary>
        /// Save the description of the selected column
        /// </summary>
        private static void SaveColumnDesc(string? objectName, string columnName, string desc)
        {
            SqlConnection? conn = new(Properties.Settings.Default.dbConnectionString);
            try
            {
                var cmd = new SqlCommand("usp_AddColumnDescription", conn) { CommandType = CommandType.StoredProcedure };

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
        }

        /// <summary>
        /// Handles the click event of the apply button
        ///     apply the column description for selected objects
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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
        /// Handles the load event of the form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        /// <summary>
        /// Performs the search.
        /// </summary>
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
            messageToolStripStatusLabel.Text = objectsListBox.Items.Count switch
            {
                0 => "No match found",
                1 => "one matche found",
                _ => string.Format("{0} matches found", objectsListBox.Items.Count),
            };
            SelectAllToolStripButton_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Populates the schema.
        /// </summary>
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

        /// <summary>
        /// Handles the selected index changed event of the schema combo box
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        /// <summary>
        /// Handles the click event of the search button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchToolStripButton_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        /// <summary>
        /// Handles the key up event of the search text box
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
            }
        }

        /// <summary>
        /// Handles the text changed event of the search text box
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchToolStripTextBox_TextBoxTextAlignChanged(object sender, EventArgs e)
        {
            messageToolStripStatusLabel.Text = string.Empty;
        }

        /// <summary>
        /// Handles the click event of the select all button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectAllToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objectsListBox.Items.Count; i++)
            {
                objectsListBox.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// Handles the click event of the unselect all button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void UnselectAllToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < objectsListBox.Items.Count; i++)
            {
                objectsListBox.SetItemChecked(i, false);
            }
        }
    }
}