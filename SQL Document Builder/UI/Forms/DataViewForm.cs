
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using SQL_Document_Builder.DatabaseAccess;
using System.Text;
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
        private const int maxBinaryDisplayString = 8000;

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DatabaseConnectionItem? Connection { get; set; }

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
                try
                {
                    FixBinaryColumnsForDisplay(DataSource, out _);
                    dataGridView.DataSource = DataSource;
                    dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                    messageLabel.Text = $"{dataGridView.RowCount:N0} rows of data loaded";
                    //hide the progress bar
                    progressBar.Visible = false;
                    topToolStripComboBox.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed to load data",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
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
            try
            {
                var provider = DatabaseAccessProviderFactory.GetProvider(Connection);
                return await provider.GetDataTableAsync(sql, ConnectionString, cancellationToken);
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
            return null;
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
                    var provider = DatabaseAccessProviderFactory.GetProvider(Connection);
                    sql = provider.ApplyRowLimit(sql, 1000);
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

                        if (dt != null)
                        {
                            FixBinaryColumnsForDisplay(dt, out _);
                        }

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
                        Connection = Connection,
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
                        Connection = Connection,
                        ConnectionString = ConnectionString
                    };
                    dlg.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Converts binary data column to a string equivalent, including handling of null columns
        /// </summary>
        /// <param name="hexBuilder">String builder pre-allocated for maximum space needed</param>
        /// <param name="columnValue">Column value, expected to be of type byte []</param>
        /// <returns>String representation of column value</returns>
        private static string BinaryDataColumnToString(StringBuilder hexBuilder, object columnValue)
        {
            const string hexChars = "0123456789ABCDEF";
            if (columnValue == DBNull.Value)
            {
                // Return special "(null)" value here for null column values
                return "(null)";
            }
            else
            {
                // Otherwise return hex representation
                byte[] byteArray = (byte[])columnValue;
                int displayLength = (byteArray.Length > maxBinaryDisplayString) ? maxBinaryDisplayString : byteArray.Length;
                hexBuilder.Length = 0;
                hexBuilder.Append("0x");
                for (int i = 0; i < displayLength; i++)
                {
                    hexBuilder.Append(hexChars[(int)byteArray[i] >> 4]);
                    hexBuilder.Append(hexChars[(int)byteArray[i] % 0x10]);
                }
                return hexBuilder.ToString();
            }
        }

        /// <summary>
        /// Try to convert a binary column to a string column
        /// </summary>
        /// <param name="t">The data table.</param>
        /// <param name="columnName">The binary column name.</param>
        /// <returns>True if the conversion succeeded.</returns>
        private static bool FixABinaryColumn(DataTable t, string columnName)
        {
            bool result = false;
            try
            {
                // Create temporary column to copy over data
                string tempColumnName = "C" + Guid.NewGuid().ToString();
                t.Columns.Add(new DataColumn(tempColumnName, typeof(string)));
                t.Columns[tempColumnName].SetOrdinal(t.Columns[columnName].Ordinal);

                // Replace values in every row
                StringBuilder hexBuilder = new((maxBinaryDisplayString * 2) + 2);
                foreach (DataRow r in t.Rows)
                {
                    r[tempColumnName] = BinaryDataColumnToString(hexBuilder, r[columnName]);
                }

                t.Columns.Remove(columnName);
                t.Columns[tempColumnName].ColumnName = columnName;
                result = true;
            }
            catch (Exception)
            {
                // only returns false to indicates the conversion is failed
            }
            return result;
        }

        /// <summary>
        /// Accepts a datatable and converts all binary columns into a textual representation of a binary column.
        /// For use when displaying binary columns in a DataGridView.
        /// </summary>
        /// <param name="t">Input data table</param>
        /// <param name="success">Indicates whether all binary columns were converted successfully.</param>
        /// <returns>Updated data table, with binary columns replaced</returns>
        private static DataTable FixBinaryColumnsForDisplay(DataTable t, out bool success)
        {
            success = true;

            for (int i = 0; i < t.Columns.Count; i++)
            {
                bool needFix = false;
                var column = t.Columns[i];
                if (column.DataType == typeof(byte[]))
                {
                    needFix = true;
                }
                else if (column.DataType == typeof(object) && column.DataType != typeof(System.Guid))
                {
                    needFix = true;
                }

                if (needFix)
                {
                    // Remove constraints involving the binary column
                    foreach (Constraint constraint in t.Constraints)
                    {
                        // remove the constraint if the constraint column is the specified column
                        if (constraint is UniqueConstraint uniqueConstraint)
                        {
                            if (uniqueConstraint.Columns.Length == 1 && uniqueConstraint.Columns[0].ColumnName == column.ColumnName)
                            {
                                t.Constraints.Remove(constraint);
                                break;
                            }
                        }
                        else if (constraint is ForeignKeyConstraint foreignKeyConstraint)
                        {
                            if (foreignKeyConstraint.Columns.Length == 1 && foreignKeyConstraint.Columns[0].ColumnName == column.ColumnName)
                            {
                                t.Constraints.Remove(constraint);
                                break;
                            }
                        }

                        // remove all constraints
                        t.Constraints.Remove(constraint);
                    }

                    if (!FixABinaryColumn(t, column.ColumnName))
                    {
                        success = false;
                        break;
                    }
                }
            }

            return t;
        }
    }
}