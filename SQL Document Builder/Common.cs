using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The common class.
    /// </summary>
    internal class Common
    {
        /// <summary>
        /// Data the table to HTML.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="margin">The margin.</param>
        /// <param name="fontSize">The font size.</param>
        /// <returns>A string.</returns>
        public static string DataTableToHTML(DataTable dt, int margin = 1, Single fontSize = 14)
        {
            var sb = new System.Text.StringBuilder();
            if (fontSize > 0)
            {
                sb.AppendLine(string.Format("<table class=\"wikitable\" style=\"margin: {0}em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: {1}px; background-color: #f8f9fa;\">", margin, fontSize));
            }
            else
            {
                sb.AppendLine(string.Format("<table class=\"wikitable\" style=\"margin: {0}em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; background-color: #f8f9fa;\">", margin));
            }
            sb.AppendLine("\t<tbody>");
            sb.AppendLine("\t\t<tr>");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colName = dt.Columns[i].ColumnName;
                sb.AppendLine(string.Format("\t\t<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">{0}</th>", colName));
            }
            sb.AppendLine("\t\t</tr>");

            foreach (DataRow dataRow in dt.Rows)
            {
                sb.AppendLine("\t\t<tr>");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string? value = "&#160;";
                    if (dataRow[i] != DBNull.Value)
                        value = Convert.ToString(dataRow[i]);

                    sb.AppendLine("\t\t<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">" + value?.ToString() + "</td>");
                }
                sb.AppendLine("\t\t</tr>");
            }

            sb.AppendLine("\t</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Input the box.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="title">The title.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A string.</returns>
        public static string InputBox(string prompt, string title = "", string defaultValue = "")
        {
            using var dlg = new InputBox()
            {
                Title = title.Length == 0 ? Application.ProductName : title,
                Prompt = prompt,
                Default = defaultValue
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.InputText;
            }
            return "";
        }

        /// <summary>
        /// Msg the box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="icon">The icon.</param>
        public static DialogResult MsgBox(string message, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(message, "Message", messageBoxButtons, icon);
        }

        /// <summary>
        /// Queries the data to an HTML table asynchronously.
        /// </summary>
        /// <param name="sql">The SQL query to fetch data.</param>
        /// <returns>A Task<string> containing the HTML table.</returns>
        public static async Task<string> QueryDataToHTMLTableAsync(string sql, string connectionString)
        {
            var sb = new StringBuilder();

            var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt != null && dt.Rows.Count > 0)
            {
                sb.AppendLine(DataTableToHTML(dt));
            }
            else
            {
                sb.AppendLine("<p>No data found.</p>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Msgs the box.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="yesNo">The yes no.</param>
        /// <param name="warning">The warning.</param>
        /// <returns>A DialogResult.</returns>
        internal static DialogResult MsgBox(string v, MessageBoxButtons yesNo, MessageBoxIcon warning, string title = "Message")
        {
            return MessageBox.Show(v, title, yesNo, warning);
        }
    }
}