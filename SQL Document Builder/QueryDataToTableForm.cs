using System;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class QueryDataToTableForm : Form
    {
        public QueryDataToTableForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                sqlTextBox.Text = Clipboard.GetText();
            }
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            sqlTextBox.Text = string.Empty;
        }


        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            htmlTextBox.SelectAll();
            htmlTextBox.Copy();
        }

        private void executeToolStripButton_Click(object sender, EventArgs e)
        {
            if (sqlTextBox.Text.ToLower().StartsWith("select "))
            {
                try
                {
                    htmlTextBox.Text = Common.QueryDataToHTMLTable(sqlTextBox.Text);
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

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            htmlTextBox.SelectAll();
        }

        private void sqlTextBox_TextChanged(object sender, EventArgs e)
        {
            htmlTextBox.Text = string.Empty;
        }
    }
}