using DarkModeForms;
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
        private DataTable? _tables = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchColumnDesc"/> class.
        /// </summary>
        public BatchColumnDesc()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
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
                    await Task.Run(() => ApplyDescriptonsAsync(columnName, desc, progress));

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
        private async Task ApplyDescriptonsAsync(string columnName, string desc, IProgress<int> progress)
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
                    await DatabaseHelper.SaveColumnDescAsync(objectName, columnName, desc);
                }
            }
        }

        /// <summary>
        /// Handles the load event of the form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void BatchColumnDesc_Load(object sender, EventArgs e)
        {
            _tables = await DatabaseHelper.GetDataTableAsync($"SELECT * FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_SCHEMA, TABLE_NAME");
            PopulateSchema();
        }

        /// <summary>
        /// Performs the search.
        /// </summary>
        private async Task PerformSearchAsync()
        {
            objectsListBox.Items.Clear();

            string searchFor = searchToolStripTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchFor))
            {
                return;
            }

            // replace single quotes with double quotes
            searchFor = searchFor.Replace("'", "''");
            string sql;
            //if (searchFor.Contains('%'))
            //{
            //    sql = $"SELECT DISTINCT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, 'BASE TABLE' TABLE_TYPE, COLUMN_NAME FROM information_schema.columns WHERE column_name LIKE '{searchFor}' ORDER BY TABLE_NAME";
            //}
            //else
            //{
            sql = $"SELECT DISTINCT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, 'BASE TABLE' TABLE_TYPE, COLUMN_NAME FROM information_schema.columns WHERE column_name = '{searchFor}' ORDER BY TABLE_NAME";
            //}

            var dt = await DatabaseHelper.GetDataTableAsync(sql);

            if (dt?.Rows.Count > 0)
            {
                // get the select schema name
                string? schemaName = string.Empty;
                if (schemaComboBox.SelectedIndex > 0)
                    schemaName = schemaComboBox.SelectedItem?.ToString();

                foreach (DataRow dr in dt.Rows)
                {
                    string tableSchema = (string)dr["TABLE_SCHEMA"];
                    string tableName = (string)dr["TABLE_NAME"];

                    if (schemaName?.Length > 0)
                    {
                        if (tableSchema.Equals(schemaName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            objectsListBox.Items.Add(string.Format("{0}.{1}", tableSchema, tableName));
                        }
                    }
                    else
                    {
                        // add all tables
                        objectsListBox.Items.Add(string.Format("{0}.{1}", tableSchema, tableName));
                    }
                }
            }

            messageToolStripStatusLabel.Text = objectsListBox.Items.Count switch
            {
                0 => "No match found",
                1 => "one matche found",
                _ => string.Format("{0} matches found", objectsListBox.Items.Count),
            };

            // select all items in the list box
            SelectAllToolStripButton_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Populates the schema.
        /// </summary>
        private void PopulateSchema()
        {
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");

            if (_tables == null)
            {
                return;
            }

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
        private async void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await PerformSearchAsync();
        }

        /// <summary>
        /// Handles the click event of the search button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SearchToolStripButton_Click(object sender, EventArgs e)
        {
            await PerformSearchAsync();
        }

        /// <summary>
        /// Handles the key up event of the search text box
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SearchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await PerformSearchAsync();
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