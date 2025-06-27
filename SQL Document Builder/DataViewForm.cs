
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The data view form.
    /// </summary>
    public partial class DataViewForm : Form
    {
        private CancellationTokenSource? cancellationTokenSource;
        private readonly DataTable? data;
        private int databaseIndex = 0;

        private Task<DataTable?> getDataTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewForm"/> class.
        /// </summary>
        public DataViewForm()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);

            dataGridView.RowTemplate.Height = Font.Height + 6;
            dataGridView.RowHeadersWidth = Font.Height + 12;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Indicates which database to access: 0 = cvi-source, 1 = EDW
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DatabaseIndex
        {
            set => databaseIndex = value != 0 ? 1 : 0;
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DataSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable value frequency.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableValueFrequency { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether maximize form.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MaximizeForm { get; set; } = true;

        /// <summary>
        /// Indicates multiple value columns in the data source
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MultipleValue { get; set; } = false;

        /// <summary>
        /// Gets SQL statement for pulling data
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SQL { private get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the table name of the data source.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Ons the form closing.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel)
            {
                // Ensure cancellation when the form is closing
                CloseFormAndCancelTask();
            }
        }

        /// <summary>
        /// Merge the multiple value columns into one column
        /// </summary>
        /// <param name="r"></param>
        private static void MergeADataRow(DataRow r)
        {
            string stringValue = string.Empty;
            if (r["ValueString"] != DBNull.Value)
            {
                stringValue = (string)(r["ValueString"]);
            }
            else if (r["ValueCodingCode"] != DBNull.Value)
            {
                stringValue = (string)(r["ValueCodingCode"]);
            }
            else if (r["ValueReferenceDisplay"] != DBNull.Value)
            {
                stringValue = (string)(r["ValueReferenceDisplay"]);
            }
            if (stringValue.Length > 0)
            {
                r["Value"] = stringValue;
                return;
            }

            if (r["ValueInteger"] != DBNull.Value)
            {
                r["Value"] = Convert.ToInt64(r["ValueInteger"]).ToString();
                return;
            }

            if (r["ValueDecimal"] != DBNull.Value)
            {
                r["Value"] = Convert.ToDecimal(r["ValueDecimal"]).ToString();
                return;
            }

            string dateValue = string.Empty;
            if (r["ValueDate"] != DBNull.Value)
            {
                dateValue = Convert.ToDateTime(r["ValueDate"]).ToString();
            }
            else if (r["ValueDateTime"] != DBNull.Value)
            {
                dateValue = Convert.ToDateTime(r["ValueDateTime"]).ToString();
            }
            if (dateValue.Length > 0)
            {
                r["Value"] = dateValue;
                return;
            }

            if (r["ValueQuantityValue"] != DBNull.Value)
            {
                r["Value"] = r["ValueQuantityValue"].ToString() + " " + r["ValueQuantityUnit"].ToString();
            }
        }

        /// <summary>
        /// Process a data row and merge the values in different columns into value column
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static DataTable? MergeValueColumns(DataTable data)
        {
            if (data != null)
            {
                data.Columns.Add(new DataColumn("Value", typeof(string)));
                data.Columns["Value"].SetOrdinal(data.Columns["ValueString"].Ordinal);

                foreach (DataRow r in data.Rows)
                {
                    MergeADataRow(r);
                }

                data.Columns.Remove("ValueReferenceDisplay");
                data.Columns.Remove("ValueQuantityUnit");
                data.Columns.Remove("ValueQuantityValue");
                data.Columns.Remove("ValueCodingCode");
                data.Columns.Remove("ValueString");
                data.Columns.Remove("ValueDateTime");
                data.Columns.Remove("ValueDate");
                data.Columns.Remove("ValueInteger");
                data.Columns.Remove("ValueDecimal");
            }

            return data;
        }

        /// <summary>
        /// Builds the column frequency.
        /// </summary>
        /// <returns>A DataTable.</returns>
        private DataTable? BuildColumnFrequency(string colName)
        {
            DataTable? dt = (DataTable)dataGridView.DataSource;

            if (dt == null)
            {
                return null;
            }

            var query = from row in dt.AsEnumerable()
                        group row by row.Field<object>(colName) into grp
                        select new
                        {
                            ColumnValue = grp.Key,
                            Count = grp.Count()
                        };

            // Create a new DataTable to store the results
            DataTable resultTable = new();
            resultTable.Columns.Add(colName, typeof(object));
            resultTable.Columns.Add("Count", typeof(int));

            foreach (var item in query)
            {
                resultTable.Rows.Add(item.ColumnValue, item.Count);
            }

            return resultTable;
        }

        /// <summary>
        /// Closes the form and cancel task.
        /// </summary>
        private async void CloseFormAndCancelTask()
        {
            if (cancellationTokenSource == null) return;

            // Signal cancellation
            cancellationTokenSource.Cancel();

            try
            {
                // Await the task if it has been started
                if (getDataTask != null)
                {
                    await getDataTask;
                }
            }
            catch (OperationCanceledException)
            {
                // Perform any cleanup after cancellation here
                Console.WriteLine("Operation was cancelled and cleaned up.");
            }
            finally
            {
                // Clean up resources, if any
            }
        }

        /// <summary>
        /// Handles "Copy SQL to clipboard" menu item click event: copy the query statement to the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopySQLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SQL);
        }

        /// <summary>
        /// Handles data grid view cell double click event: Open the patient, episode, or form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var columnName = dataGridView.Columns[e.ColumnIndex].DataPropertyName;
                switch (columnName.ToLower())
                {
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles form load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataViewForm_Load(object sender, EventArgs e)
        {
            if (MaximizeForm)
                WindowState = FormWindowState.Maximized;

            if (DataSource != null)
            {
                dataGridView.DataSource = DataSource;
                dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                messageLabel.Text = $"{dataGridView.RowCount:N0} rows of data loaded";
                //hide the progress bar
                progressBar.Visible = false;
                topToolStripComboBox.Enabled = false;
                return;
            }

            topToolStripComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles "Exit" menu item click event: Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ExitToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();

                try
                {
                    await getDataTask;
                }
                catch (OperationCanceledException)
                {
                    // this is expected
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error");
                }
            }

            this.Close();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        private async Task<DataTable?> GetDataAsync(string sql, CancellationToken cancellationToken)
        {
            DataTable dt = new();

            try
            {
                using var conn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };

                await conn.OpenAsync(cancellationToken);
                using var crt = cancellationToken.Register(() => cmd.Cancel());

                using var dr = await cmd.ExecuteReaderAsync(cancellationToken);

                // Load the schema from the data reader and create DataTable columns
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                }

                // Read rows one by one, checking for cancellation
                while (await dr.ReadAsync(cancellationToken))
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        row[i] = dr.IsDBNull(i) ? DBNull.Value : dr.GetValue(i);
                    }
                    dt.Rows.Add(row);

                    // Check for cancellation request
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on cancellation, do not show error or close form
                return null;
            }
            catch (Exception)
            {
                //// show error message and close the form
                //MessageBox.Show(ex.Message, "Failed to load data",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.Invoke((MethodInvoker)delegate { this.Close(); });
                throw;
            }
            return dt;
        }

        /// <summary>
        /// Fetch the data and populates the data grid view
        /// </summary>
        private async Task OpenAsync()
        {
            if (SQL.Length > 0 && ConnectionString.Length > 0)
            {
                // build the SQL statement based on the selected top value
                var sql = SQL;
                if (topToolStripComboBox.SelectedIndex == 0)
                {
                    // ensure the SQL statement contain the "TOP" keyword, if not, add TOP 1000 to the SQL statement
                    if (!sql.Contains(" TOP ", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sql = sql.Insert(sql.IndexOf("SELECT", StringComparison.CurrentCultureIgnoreCase) + 6, " TOP 1000 ");
                    }
                }
                else
                {
                    // remove the TOP keyword from the SQL statement
                    if (sql.Contains(" TOP ", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sql = sql.Remove(sql.IndexOf(" TOP 1000 ", StringComparison.CurrentCultureIgnoreCase), 9);
                    }
                }

                messageLabel.Text = "Please wait while loading data...";
                Cursor = Cursors.WaitCursor;
                progressBar.Visible = true;
                progressBar.Value = 0;
                timer2.Start();

                try
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    getDataTask = Task.Run(() => GetDataAsync(sql, cancellationTokenSource.Token));

                    var dt = await getDataTask;
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (dt == null) Close();

                        if (MultipleValue && databaseIndex == 0)
                        {
                            dataGridView.DataSource = MergeValueColumns(dt);
                        }
                        else
                        {
                            dataGridView.DataSource = dt;
                        }
                        dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                        progressBar.Visible = false;
                        if (dataGridView.RowCount == 0)
                        {
                            messageLabel.Text = "No data found";
                            MessageBox.Show("There is no data for " + Text, "No data found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            Close();
                        }
                        else if (dataGridView.RowCount == 1)
                        {
                            messageLabel.Text = "One row of data loaded";
                        }
                        else
                            messageLabel.Text = $"{dataGridView.RowCount.ToString("N0")} rows of data loaded";
                    });

                    // release the cancellationTokenSource
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
                catch (OperationCanceledException)
                {
                    // this is expected
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed to load data",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
                finally
                {
                    timer2.Stop();
                    dataGridView.Visible = true;
                    Cursor = Cursors.Default;
                }

                if (EnableValueFrequency && dataGridView.Rows.Count > 2)
                {
                    dataGridView.ContextMenuStrip = contextMenuStrip1;
                }
            }
        }

        /// <summary>
        /// Handles "Save" menu item click event: Save the current data to a cvs file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            //ExportToCSV(dataGridView);
        }

        /// <summary>
        /// Handles form load timer tick event: Start to pull data
        /// This timer is used to show the form first before start to pull data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        /// <summary>
        /// Handles progress timer tick event: Show progress bar by increse the bar value
        /// Timer2 is used for showing data loading progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer2_Tick(object sender, EventArgs e)
        {
            var value = progressBar.Value;
            value++;
            if (value > progressBar.Maximum)
                value = progressBar.Minimum;
            progressBar.Value = value;
        }

        /// <summary>
        /// handles the top tool strip combo box selection change event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void TopToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            // clear the data grid view
            //dataGridView.DataSource = null;
            dataGridView.Visible = false;

            await OpenAsync();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Values the frequency tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ValueFrequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnableValueFrequency || dataGridView.Rows.Count < 2 || dataGridView.CurrentCell == null)
            {
                return;
            }

            // get the column name
            var colName = dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(colName))
            {
                return;
            }

            if (DataSource == null && !string.IsNullOrWhiteSpace(TableName))
            {
                using var dlg = new DataViewForm()
                {
                    SQL = $"SELECT {colName}, COUNT(*) AS RecNum FROM {TableName} GROUP BY {colName}",
                    Text = $"Value Frequency of {colName}",
                    EnableValueFrequency = false,
                    DatabaseIndex = 0,
                    MultipleValue = false,
                    MaximizeForm = false,
                    ConnectionString = ConnectionString
                };
                dlg.ShowDialog();
            }
            else
            {
                // get the column name of the current cell
                var columnName = dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex].DataPropertyName;
                if (string.IsNullOrEmpty(columnName))
                {
                    return;
                }
                var dt = BuildColumnFrequency(columnName);
                if (dt != null && dt.Rows.Count > 0)
                {
                    using var dlg = new DataViewForm()
                    {
                        DataSource = dt,
                        Text = $"Value Frequency of {columnName}",
                        EnableValueFrequency = false,
                        DatabaseIndex = 0,
                        MultipleValue = false,
                        MaximizeForm = false,
                        ConnectionString = ConnectionString
                    };
                    dlg.ShowDialog();
                }
            }
        }
    }
}