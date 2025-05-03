using System;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The excel sheets form.
    /// </summary>
    public partial class ExcelSheetsForm : Form
    {
        private string _connectionString = "";  // connection string to open the Excel file

        private DataTable _resultDataTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSheetsForm"/> class.
        /// </summary>
        public ExcelSheetsForm()
        {
            InitializeComponent();
            //if (Properties.Settings.Default.DarkMode)
            //{
            //    _ = new DarkTheme(this);
            //}
        }

        /// <summary>
        /// Gets the result data table.
        /// </summary>
        public DataTable ResultDataTable => _resultDataTable;

        /// <summary>
        /// The Excel file name to analysis
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Open the Excel file with task cancellation control
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> OpenExcelFileAsync(string sql, string connectionString, CancellationToken cancellationToken)
        {
            var result = "";
            if (connectionString.Length > 0)
            {
                using var conn = new System.Data.OleDb.OleDbConnection(connectionString);
                try
                {
                    using var cmd = new System.Data.OleDb.OleDbCommand(sql, conn)
                    {
                        CommandType = CommandType.Text
                    };
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using CancellationTokenRegistration crt = cancellationToken.Register(() => cmd.Cancel());
                    using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false); /*.ConfigureAwait(false)*/
                    _resultDataTable = new DataTable();
                    _resultDataTable.Load(reader);
                    reader.Close();
                }
                catch (OperationCanceledException)
                {
                    result = "Cancelled";
                }
                catch (System.Exception ex)
                {
                    result = ex.Message;
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
        /// Analysis button click event handle: start analysis selected sheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AnalysisButton_Click(object sender, EventArgs e)
        {
            try
            {
                var sheetName = sheetsListBox.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(sheetName))
                {
                    var sql = $"select * from [{sheetName}]";
                    var result = await OpenExcelFileAsync(sql, _connectionString, CancellationToken.None);
                }
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, Properties.Resources.A005, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Close button click event handle: Close the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form load event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcelSheetsForm_Load(object sender, EventArgs e)
        {
            const string strPass = "";
            if (FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                _connectionString = String.Format("provider=Microsoft.ACE.OLEDB.12.0;data source={0};{1}Extended Properties=Excel 12.0;", FileName, strPass);
            }
            else
            {
                _connectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};{1}Extended Properties=Excel 8.0;", FileName, strPass);
            }
            startTimer.Start();
        }

        /// <summary>
        /// start timer tick event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTimer_Tick(object sender, EventArgs e)
        {
            startTimer.Stop();

            if (_connectionString.Length > 1)
            {
                try
                {
                    using var conn = new System.Data.OleDb.OleDbConnection(_connectionString);
                    conn.Open();
                    DataTable dtSheets = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);

                    foreach (DataRow dr in dtSheets.Rows)
                    {
                        sheetsListBox.Items.Add(dr["TABLE_NAME"].ToString());
                    }

                    if (sheetsListBox.Items.Count > 0)
                    {
                        sheetsListBox.SelectedIndex = 0;
                        analysisButton.Enabled = true;
                    }
                    infoToolStripStatusLabel.Text = System.IO.Path.GetFileName(FileName);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, Properties.Resources.A005, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    //throw;
                }
            }
        }
    }
}