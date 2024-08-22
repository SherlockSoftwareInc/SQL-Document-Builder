using System;
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
        }

        /// <summary>
        /// Gets or sets a value indicating whether insert statement.
        /// </summary>
        public bool InsertStatement { get; set; } = false;

        /// <summary>
        /// Copies the tool strip button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            htmlTextBox.SelectAll();
            htmlTextBox.Copy();
        }

        /// <summary>
        /// Executes the tool strip button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExecuteToolStripButton_Click(object sender, EventArgs e)
        {
            if (sqlTextBox.Text.ToLower().StartsWith("select "))
            {
                try
                {
                    if (InsertStatement)
                    {
                        htmlTextBox.Text = Common.QueryDataToInsertStatement(sqlTextBox.Text);
                    }
                    else
                    {
                        htmlTextBox.Text = Common.QueryDataToHTMLTable(sqlTextBox.Text);
                    }

                    Clipboard.SetText(htmlTextBox.Text);
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
        /// Exits the tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// News the tool strip button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Pastes the tool strip button_ click.
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
        /// Selects the all tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            htmlTextBox.SelectAll();
        }

        /// <summary>
        /// Sqls the text box_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SqlTextBox_TextChanged(object sender, EventArgs e)
        {
            htmlTextBox.Text = string.Empty;
        }
    }
}