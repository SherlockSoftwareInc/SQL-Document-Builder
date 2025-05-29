using DarkModeForms;
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
            _ = new DarkModeCS(this);
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the INSERT statement.
        /// </summary>
        public string DocumentBody { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the generated document is a insert statement or not.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InsertStatement { get; set; } = false;

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
                textBox.SelectAll();
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
            if (sqlTextBox.Text.StartsWith("select ", StringComparison.CurrentCultureIgnoreCase))
            {
                DocumentBody = string.Empty;

                try
                {
                    if (InsertStatement)
                    {
                        htmlTextBox.Text = await DatabaseDocBuilder.QueryDataToInsertStatementAsync(sqlTextBox.Text, ConnectionString);
                    }
                    else
                    {
                        htmlTextBox.Text = await Common.QueryDataToHTMLTableAsync(sqlTextBox.Text, ConnectionString);
                    }

                    DocumentBody = htmlTextBox.Text;

                    if (!string.IsNullOrEmpty(htmlTextBox.Text))
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    messageLabel.Text = ex.Message;
                }
            }
            else
            {
                messageLabel.Text = "Invalid query.";
            }
        }

        /// <summary>
        /// Handles the click event of the exit tool strip menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                sqlTextBox.Text = Clipboard.GetText();
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
            htmlTextBox.Text = string.Empty;
        }
    }
}