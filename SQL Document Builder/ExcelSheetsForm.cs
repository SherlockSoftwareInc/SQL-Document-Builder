using DarkModeForms;
using ExcelDataReader;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The excel sheets form.
    /// </summary>
    public partial class ExcelSheetsForm : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSheetsForm"/> class.
        /// </summary>
        public ExcelSheetsForm()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
        }

        /// <summary>
        /// Gets the result data table.
        /// </summary>
        public DataTable? ResultDataTable { get; private set; }

        /// <summary>
        /// The Excel file name to analyze.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public string TableName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether null for blank.
        /// </summary>
        public bool NullForBlank { get; private set; } = false;

        /// <summary>
        /// Open the Excel file with task cancellation control.
        /// </summary>
        /// <param name="sheetName">The sheet name to read data from.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A string indicating the result.</returns>
        public async Task<string> OpenExcelFileAsync(string sheetName, CancellationToken cancellationToken)
        {
            var result = "";
            try
            {
                using var stream = File.Open(FileName, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                var dataSetConfig = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // Use the first row as column headers
                    }
                };

                var dataSet = await Task.Run(() => reader.AsDataSet(dataSetConfig), cancellationToken);

                if (dataSet.Tables.Contains(sheetName))
                {
                    ResultDataTable = dataSet.Tables[sheetName];
                }
                else
                {
                    result = "Sheet not found.";
                }
            }
            catch (OperationCanceledException)
            {
                result = "Cancelled";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Analysis button click event handle: start analysis of the selected sheet.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void AnalysisButton_Click(object sender, EventArgs e)
        {
            try
            {
                var sheetName = sheetsListBox.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(sheetName))
                {
                    var result = await OpenExcelFileAsync(sheetName, CancellationToken.None);
                    if (!string.IsNullOrEmpty(result))
                    {
                        MessageBox.Show(result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                TableName = tableNameTextBox.Text.Trim();
                NullForBlank = nullCheckBox.Checked;
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Close button click event handle: Close the dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form load event handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ExcelSheetsForm_Load(object sender, EventArgs e)
        {
            try
            {
                using var stream = File.Open(FileName, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                var dataSet = reader.AsDataSet();
                foreach (DataTable table in dataSet.Tables)
                {
                    sheetsListBox.Items.Add(table.TableName);
                }

                if (sheetsListBox.Items.Count > 0)
                {
                    sheetsListBox.SelectedIndex = 0;
                    loadButton.Enabled = true;
                }

                infoToolStripStatusLabel.Text = Path.GetFileName(FileName);

                tableNameTextBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
