using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ScintillaNET.Style;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The share point builder.
    /// </summary>
    internal class SharePointBuilder
    {
        //private readonly SQLServerConnections _connections = new();

        //private int _connectionCount = 0;
        private readonly string? _database = string.Empty;

        private readonly System.Text.StringBuilder _script = new();

        //private SQLDatabaseConnectionItem? _selectedConnection;
        private readonly string? _server = string.Empty;

        /// <summary>
        /// Texts the to table.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        /// <returns>A string.</returns>
        public static string TextToTable(string metaData)
        {
            // Normalize line endings and split into lines
            var lines = metaData.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (lines.Length < 2)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<table class=\"wikitable\" style=\"margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;\">");
            sb.AppendLine("<tbody>");

            // Header
            sb.AppendLine("<tr>");
            foreach (var headItem in lines[0].Split('\t'))
                sb.AppendLine($"<th style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;\">{System.Net.WebUtility.HtmlEncode(headItem)}</th>");
            sb.AppendLine("</tr>");

            // Data rows
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                sb.AppendLine("<tr>");
                foreach (var column in lines[i].Split('\t'))
                    sb.AppendLine($"<td style=\"padding: 0.2em 0.4em; border: 1px solid #a2a9b1;\">{System.Net.WebUtility.HtmlEncode(column)}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Builds the table list as an HTML table.
        /// </summary>
        /// <param name="selectedObjects">The selected objects</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <returns>A Task returning the HTML table of tables.</returns>
        public async Task<string> BuildTableListAsync(List<ObjectName> selectedObjects, string connectionString, IProgress<int> progress)
        {
            _script.Clear();

            try
            {
                if (selectedObjects.Count > 0)
                {
                    _script.AppendLine(@"<table class=""wikitable"" style=""margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;"">");
                    _script.AppendLine("<tbody>");
                    _script.AppendLine(@"    <tr>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Schema</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Name</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Description</th>
    </tr>");

                    int lastPercent = -1;
                    for (int i = 0; i < selectedObjects.Count; i++)
                    {
                        int percentComplete = (i * 100) / selectedObjects.Count;
                        if (percentComplete != lastPercent && percentComplete % 2 == 0)
                        {
                            progress?.Report(percentComplete + 1);
                            lastPercent = percentComplete;
                        }

                        ObjectName dr = selectedObjects[i];
                        string tableSchema = System.Net.WebUtility.HtmlEncode(dr.Schema ?? string.Empty);
                        string tableName = System.Net.WebUtility.HtmlEncode(dr.Name ?? string.Empty);

                        string description = "&nbsp;";
                        try
                        {
                            // Fetch description if available
                            string desc = await DatabaseHelper.GetTableDescriptionAsync(dr, connectionString);
                            if (!string.IsNullOrWhiteSpace(desc))
                                description = System.Net.WebUtility.HtmlEncode(desc);
                        }
                        catch
                        {
                            // If description fetch fails, leave as &nbsp;
                        }

                        _script.AppendLine($@"    <tr>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{tableSchema}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">[[{tableSchema}.{tableName}|{tableName}]]</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{description}</td>
    </tr>");
                    }
                    _script.AppendLine("</tbody>");
                    _script.AppendLine("</table>");
                }
                else
                {
                    _script.AppendLine("<p><em>No tables found.</em></p>");
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return _script.ToString();
        }

        /// <summary>
        /// Build value list of the given table for wiki
        /// </summary>
        /// <param name="tableName"></param>
        public static async Task<string> GetTableValuesAsync(string sql, string connectionString)
        {
            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
            if (dt == null || dt.Rows.Count == 0)
            {
                return "''No data found.''";
            }
            return DataTableToDoc(dt);
        }

        /// <summary>
        /// Data the table to doc.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A string.</returns>
        internal static string DataTableToDoc(DataTable data)
        {
            var sb = new StringBuilder();

            if (data != null && data.Rows.Count > 0)
            {
                sb.AppendLine(Common.DataTableToHTML(data));
            }
            else
            {
                sb.AppendLine("<p>No data found.</p>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate content for wiki of the given table or view, with HTML output for all sections.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="templateBody">The template body.</param>
        /// <returns>HTML documentation for the table/view.</returns>
        public async Task<string> GetTableViewDef(ObjectName objectName, string connectionString, string templateBody)
        {
            string doc = templateBody;

            // Open the database object
            var tableView = new DBObject();
            await tableView.OpenAsync(objectName, connectionString);

            // Replace placeholders with actual values (HTML-encoded where appropriate)
            doc = doc.Replace("~ObjectName~", System.Net.WebUtility.HtmlEncode(objectName.Name));
            doc = doc.Replace("~ObjectSchema~", System.Net.WebUtility.HtmlEncode(objectName.Schema));
            doc = doc.Replace("~ObjectFullName~", System.Net.WebUtility.HtmlEncode(objectName.FullName));
            doc = doc.Replace("~ObjectType~", System.Net.WebUtility.HtmlEncode(ObjectTypeToString(objectName.ObjectType)));

            doc = doc.Replace("~Description~", string.IsNullOrWhiteSpace(tableView.Description) ? "&nbsp;" : System.Net.WebUtility.HtmlEncode(tableView.Description));
            doc = doc.Replace("~Definition~", $"<pre style=\"white-space: pre-wrap;\">{System.Net.WebUtility.HtmlEncode(tableView.Definition)}</pre>");

            // Build the columns table (HTML)
            if (doc.Contains("~Columns~"))
            {
                var columnBody = GetColumnsBody(tableView.Columns);
                doc = doc.Replace("~Columns~", columnBody);
            }

            // Build the indexes table (HTML)
            if (doc.Contains("~Indexes~"))
            {
                var indexBody = GetIndexesBody(tableView.Indexes);
                doc = doc.Replace("~Indexes~", indexBody);
            }

            // Build the constraints table (HTML)
            if (doc.Contains("~Constraints~"))
            {
                var constraintBody = GetConstraintsBody(tableView.Constraints);
                doc = doc.Replace("~Constraints~", constraintBody);
            }

            return doc;
        }

        /// <summary>
        /// Gets the function or procedure definition and parameters as an HTML table.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A Task returning the HTML-formatted documentation.</returns>
        internal async Task<string> GetFunctionProcedureDef(ObjectName objectName, string connectionString, string templateBody)
        {
            string doc = templateBody;

            // Open the database object
            var func = new DBObject();
            await func.OpenAsync(objectName, connectionString);

            // Replace placeholders with HTML-encoded values
            doc = doc.Replace("~ObjectName~", System.Net.WebUtility.HtmlEncode(objectName.Name));
            doc = doc.Replace("~ObjectSchema~", System.Net.WebUtility.HtmlEncode(objectName.Schema));
            doc = doc.Replace("~ObjectFullName~", System.Net.WebUtility.HtmlEncode(objectName.FullName));
            doc = doc.Replace("~ObjectType~", System.Net.WebUtility.HtmlEncode(ObjectTypeToString(objectName.ObjectType)));
            doc = doc.Replace("~Description~", string.IsNullOrWhiteSpace(func.Description) ? "&nbsp;" : System.Net.WebUtility.HtmlEncode(func.Description));
            doc = doc.Replace("~Definition~", $"<pre style=\"white-space: pre-wrap;\">{System.Net.WebUtility.HtmlEncode(func.Definition)}</pre>");

            // Build the Parameters table (HTML)
            if (doc.Contains("~Parameters~"))
            {
                var paraBody = GetParametersBody(func.Parameters);
                doc = doc.Replace("~Parameters~", paraBody);
            }

            return doc;
        }

        /// <summary>
        /// Gets the columns body as an HTML table.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A string.</returns>
        private static string GetColumnsBody(List<DBColumn> columns)
        {
            var sb = new StringBuilder();
            if (columns != null && columns.Count > 0)
            {
                sb.AppendLine(@"<table class=""wikitable"" style=""margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;"">");
                sb.AppendLine("<tbody>");
                sb.AppendLine(@"    <tr>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Ord</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Name</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Data Type</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Nullable</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Description</th>
    </tr>");

                foreach (DBColumn col in columns)
                {
                    string ord = System.Net.WebUtility.HtmlEncode(col.Ord ?? string.Empty);
                    string colName = System.Net.WebUtility.HtmlEncode(string.IsNullOrEmpty(col.ColumnName) ? " " : col.ColumnName);
                    string dataType = System.Net.WebUtility.HtmlEncode(col.DataType);
                    string nullable = col.Nullable ? "Yes" : "No";
                    string description = System.Net.WebUtility.HtmlEncode(col.Description);

                    sb.AppendLine($@"    <tr>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{ord}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{colName}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{dataType}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{nullable}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{description}</td>
    </tr>");
                }
                sb.AppendLine("</tbody>");
                sb.AppendLine("</table>");

                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return "_No columns found_";
            }
        }

        /// <summary>
        /// Gets the constraints body as an HTML table.
        /// </summary>
        /// <param name="constraints">The constraints.</param>
        /// <returns>An HTML string representing the constraints table.</returns>
        private static string GetConstraintsBody(List<ConstraintItem> constraints)
        {
            if (constraints == null || constraints.Count == 0)
            {
                return "_No constraints found_";
            }

            var sb = new StringBuilder();
            sb.AppendLine(@"<table class=""wikitable"" style=""margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;"">");
            sb.AppendLine("<tbody>");
            sb.AppendLine(@"    <tr>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Constraint Name</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Type</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Column(s)</th>
    </tr>");

            foreach (var constraint in constraints)
            {
                string name = System.Net.WebUtility.HtmlEncode(constraint.Name);
                string type = System.Net.WebUtility.HtmlEncode(constraint.Type);
                string column = System.Net.WebUtility.HtmlEncode(constraint.Column);

                sb.AppendLine($@"    <tr>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;""><code>{name}</code></td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{type}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{column}</td>
    </tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
        }

        /// <summary>
        /// Gets the indexes body as an HTML table.
        /// </summary>
        /// <param name="indexes">The indexes.</param>
        /// <returns>An HTML string representing the indexes table.</returns>
        private static string GetIndexesBody(List<IndexItem> indexes)
        {
            var sb = new StringBuilder();

            if (indexes != null && indexes.Count > 0)
            {
                sb.AppendLine(@"<table class=""wikitable"" style=""margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;"">");
                sb.AppendLine("<tbody>");
                sb.AppendLine(@"    <tr>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Index Name</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Type</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Columns</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Unique</th>
    </tr>");

                foreach (var index in indexes)
                {
                    string idxName = System.Net.WebUtility.HtmlEncode(index.Name);
                    string type = System.Net.WebUtility.HtmlEncode(index.Type);
                    string columns = System.Net.WebUtility.HtmlEncode(index.Columns);
                    string unique = index.IsUnique ? "Yes" : "No";
                    sb.AppendLine($@"    <tr>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;""><code>{idxName}</code></td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{type}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{columns}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{unique}</td>
    </tr>");
                }

                sb.AppendLine("</tbody>");
                sb.AppendLine("</table>");
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return "_No indexes found_";
            }
        }

        /// <summary>
        /// Gets the parameters body.
        /// </summary>
        /// <param name="paramDt">The param dt.</param>
        /// <returns>A string.</returns>
        private static string GetParametersBody(List<DBParameter> paramDt)
        {
            if (paramDt != null && paramDt.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"<table class=""wikitable"" style=""margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;"">");
                sb.AppendLine("<tbody>");
                sb.AppendLine(@"    <tr>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Ord</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Name</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Data Type</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Direction</th>
        <th style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;"">Description</th>
    </tr>");
                foreach (DBParameter dr in paramDt)
                {
                    string ord = System.Net.WebUtility.HtmlEncode(dr.Ord);
                    string name = System.Net.WebUtility.HtmlEncode(string.IsNullOrEmpty(dr.Name) ? " " : dr.Name);
                    string dataType = System.Net.WebUtility.HtmlEncode(dr.DataType);
                    string direction = System.Net.WebUtility.HtmlEncode(dr.Mode);
                    string description = System.Net.WebUtility.HtmlEncode(dr.Description ?? string.Empty);
                    sb.AppendLine($@"    <tr>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{ord}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;""><code>{name}</code></td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{dataType}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{direction}</td>
        <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{description}</td>
    </tr>");
                }
                sb.AppendLine("</tbody>");
                sb.AppendLine("</table>");

                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }

            return "_No parameters_";
        }

        /// <summary>
        /// Objects the type to string.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>A string.</returns>
        private static string ObjectTypeToString(ObjectName.ObjectTypeEnums objectType)
        {
            return objectType switch
            {
                ObjectName.ObjectTypeEnums.Table => "Table",
                ObjectName.ObjectTypeEnums.View => "View",
                ObjectName.ObjectTypeEnums.Function => "Function",
                ObjectName.ObjectTypeEnums.StoredProcedure => "Stored Procedure",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Append text to the bottom of the text box
        /// </summary>
        /// <param name="text"></param>
        private void AppendLine(string text)
        {
            //sqlTextBox.AppendText(text + Environment.NewLine);
            _script.AppendLine(text);
        }
    }
}