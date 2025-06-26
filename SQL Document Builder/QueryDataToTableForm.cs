
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The query data to table form.
    /// </summary>
    public partial class QueryDataToTableForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDataToTableForm"/> class.
        /// </summary>
        public QueryDataToTableForm()
        {
            InitializeComponent();
            _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the generated document is a insert statement or not.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InsertStatement { get; set; } = false;

        /// <summary>
        /// Gets the sql statement.
        /// </summary>
        public string SQL { get; private set; }

        /// <summary>
        /// Handles the click event of the Copy tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            // if active control is a text box, copy the text to clipboard
            if (ActiveControl is TextBoxBase textBox)
            {
                textBox.Copy();
                return;
            }
        }

        /// <summary>
        /// Handles the click event of the execute tool strip button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ExecuteToolStripButton_Click(object sender, EventArgs e)
        {
            if (sqlTextBox.Text.Contains("select", StringComparison.CurrentCultureIgnoreCase))
            {
                var syntaxCheck = await SQLDatabaseHelper.SyntaxCheckAsync(sqlTextBox.Text, ConnectionString);

                if (string.IsNullOrEmpty(syntaxCheck))
                {
                    SQL = sqlTextBox.Text;
                    DialogResult = DialogResult.OK;

                    Close();
                }
                else
                {
                    errorTextBox.Text = syntaxCheck;
                    return;
                }
            }
            else
            {
                errorTextBox.Text = "The SQL statement must contain a SELECT statement.";
                return;
            }
        }

        /// <summary>
        /// Handles the click event of the exit tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the click event of the new tool strip button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Handles the click event of the paste tool strip button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                sqlTextBox.Paste();
            }
        }

        /// <summary>
        /// Handles the Load event of the QueryDataToTableForm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void QueryDataToTableForm_Load(object sender, EventArgs e)
        {
            if (InsertStatement)
            {
                Text = "Query to INSERT Statement";
            }
            else
            {
                Text = "Query to HTML";
            }
            errorTextBox.ForeColor = System.Drawing.Color.LightCoral;
        }

        /// <summary>
        /// Handles the click event of the select all tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveControl is TextBoxBase textBox)
            {
                textBox.SelectAll();
                return;
            }
        }

        /// <summary>
        /// Handles the text changed event of the SQL text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SqlTextBox_TextChanged(object sender, EventArgs e)
        {
            errorTextBox.Text = string.Empty;
        }
    }
}