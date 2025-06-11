using SQL_Document_Builder.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The document builder.
    /// </summary>
    internal class DocumentBuilder
    {
        /// <summary>
        /// Builds the table list.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> BuildObjectList(List<ObjectName> objectList, string connectionString, TemplateItem template, IProgress<int> progress)
        {
            string doc = template.Body;
            string objectItemTemplate = template.ObjectLists.ObjectRow;

            var sb = new StringBuilder();

            if (objectList.Count > 0)
            {
                for (int i = 0; i < objectList.Count; i++)
                {
                    int percentComplete = (i * 100) / objectList.Count;
                    if (percentComplete > 0 && percentComplete % 2 == 0)
                    {
                        progress.Report(percentComplete + 1);
                    }

                    ObjectName dr = objectList[i];
                    string tableSchema = dr.Schema;
                    string tableName = dr.Name;
                    string description = await DatabaseHelper.GetTableDescriptionAsync(dr, connectionString);
                    var objectName = new ObjectName(dr.ObjectType, dr.Schema, dr.Name);

                    string objItemDoc = objectItemTemplate
                        .Replace("~ObjectName~", tableName)
                        .Replace("~ObjectSchema~", tableSchema)
                        .Replace("~ObjectFullName~", objectName.FullName)
                        .Replace("~ObjectType~", ObjectTypeToString(dr.ObjectType))
                        .Replace("~Description~", description);

                    sb.AppendLine(objItemDoc);
                }
            }

            return doc.Replace("~ObjectItem~", sb.ToString());
        }

        /// <summary>
        /// Convert the data table to markdown document.
        /// </summary>
        /// <param name="dt">The data table</param>
        /// <returns>A string.</returns>
        internal static string DataTableToDoc(DataTable dt)
        {
            var sb = new StringBuilder();

            // Header
            var headers = new StringBuilder("|");
            var separators = new StringBuilder("|");
            foreach (DataColumn col in dt.Columns)
            {
                headers.Append($" {col.ColumnName} |");
                separators.Append("------------|");
            }
            sb.AppendLine(headers.ToString());
            sb.AppendLine(separators.ToString());

            // Rows
            foreach (DataRow dr in dt.Rows)
            {
                var row = new StringBuilder("|");
                foreach (DataColumn col in dt.Columns)
                {
                    row.Append($" {dr[col]?.ToString() ?? ""} |");
                }
                sb.AppendLine(row.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Data the table to doc.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="tableTemplate">The table template.</param>
        /// <param name="rowTemplate">The row template.</param>
        /// <param name="cellTemplate">The cell template.</param>
        /// <returns>A string.</returns>
        internal static string DataTableToTemplateDoc(DataTable dt, Template.TemplateItem template)
        {
            if (dt == null || dt.Columns.Count == 0)
                return "_No data available_";

            string tableTemplate = template.Body;
            string headerCellTemplate = template.DataTable.HeaderCell;
            string rowTemplate = template.DataTable.DataRow;
            string cellTemplate = template.DataTable.Cell;

            bool isHtml = tableTemplate.Contains("<table", StringComparison.OrdinalIgnoreCase);
            bool includeAlign = tableTemplate.Contains("~Align~");

            // === Build header ===
            var headerParts = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                string headerValue = col.ColumnName;
                if (isHtml)
                    headerValue = System.Net.WebUtility.HtmlEncode(headerValue);
                headerParts.Add(headerCellTemplate.Replace("~HeaderCell~", headerValue));
            }
            string header = string.Join(isHtml ? Environment.NewLine : " | ", headerParts);

            // === Build alignment row (for Markdown only) ===
            string align = string.Empty;
            if (includeAlign)
            {
                // You can customize alignment here: center by default
                var aligns = Enumerable.Repeat(":---:", dt.Columns.Count);
                align = string.Join(" | ", aligns);
                // Replace ~Align~ in the template body now, before inserting rows/cells
                tableTemplate = tableTemplate.Replace("~Align~", align);
            }

            // === Build rows ===
            var allRows = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                var cellParts = new List<string>();
                foreach (DataColumn col in dt.Columns)
                {
                    string value = dr[col]?.ToString() ?? "";
                    if (isHtml)
                        value = System.Net.WebUtility.HtmlEncode(value);
                    cellParts.Add(cellTemplate.Replace("~Cell~", value));
                }

                string joinedCells = string.Join(isHtml ? Environment.NewLine : " | ", cellParts);
                string row = rowTemplate.Replace("~Row~", joinedCells);
                allRows.Add(row);
            }

            string rows = string.Join(Environment.NewLine, allRows);

            // === Final replacement ===
            string result = tableTemplate
                .Replace("~Header~", header)
                .Replace("~Rows~", rows);

            return result;
        }

        /// <summary>
        /// Gets the table definition
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetObjectDef(ObjectName objectName, string connectionString, Template.TemplateItem template)
        {
            string doc = template.Body;

            // open the database oobjects
            var tableView = new DBObject();
            await tableView.OpenAsync(objectName, connectionString);

            // Replace placeholders with actual values
            doc = doc.Replace("~ObjectName~", objectName.Name);
            doc = doc.Replace("~ObjectSchema~", objectName.Schema);
            doc = doc.Replace("~ObjectFullName~", objectName.FullName);
            doc = doc.Replace("~ObjectType~", ObjectTypeToString(objectName.ObjectType));

            doc = doc.Replace("~Description~", tableView.Description.Length == 0 ? " " : tableView.Description);
            doc = doc.Replace("~Definition~", tableView.Definition);

            // Build the columns table
            if (doc.Contains("~Columns~"))
            {
                string columnsDoc = template.Columns.Body;
                if (columnsDoc.Contains("~ColumnItem~", StringComparison.CurrentCultureIgnoreCase))
                {
                    string columnItemTemplate = template.Columns.ColumnRow;
                    var columnsBody = GetColumnsBody(tableView.Columns, columnItemTemplate);
                    columnsDoc = columnsDoc.Replace("~ColumnItem~", columnsBody);
                }
                doc = doc.Replace("~Columns~", columnsDoc);
            }

            // Build the indexes table
            if (doc.Contains("~Indexes~"))
            {
                string indexesDoc = template.Indexes.Body;
                if (indexesDoc.Contains("~IndexItem~", StringComparison.CurrentCultureIgnoreCase))
                {
                    string indexItemTemplate = template.Indexes.IndexRow;
                    var indexesBody = GetIndexesBody(tableView.Indexes, indexItemTemplate);
                    indexesDoc = indexesDoc.Replace("~IndexItem~", indexesBody);
                }
                doc = doc.Replace("~Indexes~", indexesDoc);
            }

            // Build the constraints table
            if (doc.Contains("~Constraints~"))
            {
                string constraintDoc = template.Constraints.Body;
                if (constraintDoc.Contains("~ConstraintItem~", StringComparison.CurrentCultureIgnoreCase))
                {
                    string constraintItemTemplate = template.Constraints.ConstraintRow;
                    var constraintsBody = GetConstraintsBody(tableView.Constraints, constraintItemTemplate);
                    constraintDoc = constraintDoc.Replace("~ConstraintItem~", constraintsBody);
                }
                doc = doc.Replace("~Constraints~", constraintDoc);
            }

            // Build the parameters table
            if (doc.Contains("~Parameters~"))
            {
                string parameterDoc = template.Parameters.Body;
                if (parameterDoc.Contains("~ParameterItem~", StringComparison.CurrentCultureIgnoreCase))
                {
                    string parameterItemTemplate = template.Parameters.ParameterRow;
                    var parametersBody = GetParametersBody(tableView.Parameters, parameterItemTemplate);
                    parameterDoc = parameterDoc.Replace("~ParameterItem~", parametersBody);
                }
                doc = doc.Replace("~Parameters~", parameterDoc);
            }

            // synonyms
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Synonym)
            {
                doc = doc.Replace("~BaseObjectName~", tableView.SynonymInformation?.BaseObjectName);
                doc = doc.Replace("~BaseObjectType~", tableView.SynonymInformation?.BaseObjectType);
            }

            //doc = doc.Replace("~Triggers~", objectName.Name);
            //doc = doc.Replace("~Relationships~", objectName.Name);

            return doc;
        }

        /// <summary>
        /// Gets the table values async.
        /// </summary>
        /// <param name="sql">The full name.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetTableValuesAsync(string sql, string connectionString, TemplateItem template)
        {
            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
            if (dt == null || dt.Rows.Count == 0)
            {
                return "''No data found.''";
            }
            return DataTableToTemplateDoc(dt, template);
        }

        /// <summary>
        /// Converts tabular text data (tab-separated, first line is header) to a Markdown table.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        /// <returns>A string.</returns>
        internal static string TextToTable(string metaData, TemplateItem template)
        {
            if (string.IsNullOrWhiteSpace(metaData))
                return string.Empty;

            // Normalize line endings and split into lines
            metaData = metaData.TrimEnd('\r', '\n');
            metaData = metaData.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
            var lines = metaData.Split('\n'); // Do NOT remove empty entries

            if (lines.Length == 0)
                return string.Empty;

            // convert the metadata to DataTable with the first line as header, tab as delimiter
            DataTable data = new();
            string[] headers = lines[0].Split('\t');
            foreach (var header in headers)
            {
                data.Columns.Add(header.Trim(), typeof(string));
            }
            for (int i = 1; i < lines.Length; i++)
            {
                // Always split with StringSplitOptions.None to preserve empty columns
                var columns = lines[i].Split('\t');
                // Pad columns to match headers length
                if (columns.Length < headers.Length)
                {
                    Array.Resize(ref columns, headers.Length);
                }
                DataRow row = data.NewRow();
                for (int j = 0; j < headers.Length; j++)
                {
                    row[j] = (columns[j] ?? string.Empty).Trim();
                }
                data.Rows.Add(row);
            }

            return DataTableToTemplateDoc(data, template);
        }

        /// <summary>
        /// Gets the columns body.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A string.</returns>
        private static string GetColumnsBody(List<DBColumn> columns, string columnTemplate)
        {
            var sb = new StringBuilder();
            if (columns.Count > 0)
            {
                foreach (DBColumn col in columns) // Fix: Use DBColumn instead of DataColumn
                {
                    string ord = col.Ord ?? string.Empty; // Fix: Access Ord from DBColumn
                    string colName = col.ColumnName;
                    if (string.IsNullOrEmpty(colName)) colName = " ";
                    string nullable = col.Nullable ? "Yes" : "No";

                    string colDoc = columnTemplate
                        .Replace("~ColumnOrd~", ord)
                        .Replace("~ColumnName~", colName)
                        .Replace("~ColumnDataType~", col.DataType)
                        .Replace("~ColumnNullable~", nullable)
                        .Replace("~ColumnDescription~", col.Description ?? string.Empty);

                    sb.AppendLine(colDoc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return columnTemplate
                        .Replace("~ColumnOrd~", "")
                        .Replace("~ColumnName~", "_No column found_")
                        .Replace("~ColumnDataType~", "")
                        .Replace("~ColumnNullable~", "")
                        .Replace("~ColumnDescription~", "");
            }
        }

        /// <summary>
        /// Gets the constraints body.
        /// </summary>
        /// <param name="constraints">The constraints.</param>
        /// <returns>A string.</returns>
        private static string GetConstraintsBody(List<ConstraintItem> constraints, string strTemplate)
        {
            var sb = new StringBuilder();
            if (constraints.Count > 0)
            {
                foreach (var constraint in constraints)
                {
                    string colDoc = strTemplate
                        .Replace("~ConstraintName~", constraint.Name)
                        .Replace("~ConstraintType~", constraint.Type)
                        .Replace("~ConstraintColumn~", constraint.Column);

                    sb.AppendLine(colDoc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return strTemplate
                        .Replace("~ConstraintName~", "_No constraint found_")
                        .Replace("~ConstraintType~", "")
                        .Replace("~ConstraintColumn~", "");
            }
        }

        /// <summary>
        /// Gets the indexes body.
        /// </summary>
        /// <param name="indexes">The indexes.</param>
        /// <returns>An object.</returns>
        private static string GetIndexesBody(List<IndexItem> indexes, string indexTemplate)
        {
            var sb = new StringBuilder();
            if (indexes.Count > 0)
            {
                foreach (var index in indexes)
                {
                    string colDoc = indexTemplate
                        .Replace("~IndexName~", index.Name)
                        .Replace("~IndexType~", index.Type)
                        .Replace("~IndexColumns~", index.Columns)
                        .Replace("~UniqueIndex~", index.IsUnique ? "Yes" : "No");

                    sb.AppendLine(colDoc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return indexTemplate
                        .Replace("~IndexName~", "_No index found_")
                        .Replace("~IndexType~", "")
                        .Replace("~IndexColumns~", "")
                        .Replace("~UniqueIndex~", "");
            }
        }

        /// <summary>
        /// Gets the parameters body.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        /// <param name="parameterTemplate">The parameter template.</param>
        /// <returns>A string.</returns>
        private static string GetParametersBody(List<DBParameter> parameters, string parameterTemplate)
        {
            var sb = new StringBuilder();
            if (parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    string doc = parameterTemplate
                        .Replace("~ParameterOrd~", item.Ord)
                        .Replace("~ParameterName~", item.Name)
                        .Replace("~ParameterDataType~", item.DataType)
                        .Replace("~ParameterDirection~", item.Mode)
                        .Replace("~ParameterDescription~", item.Description ?? string.Empty);

                    sb.AppendLine(doc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return parameterTemplate
                        .Replace("~ParameterOrd~", "")
                        .Replace("~ParameterName~", "_No parameter found_")
                        .Replace("~ParameterDataType~", "")
                        .Replace("~ParameterDirection~", "")
                        .Replace("~ParameterDescription~", "");
            }
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
                ObjectName.ObjectTypeEnums.Trigger => "Trigger",
                ObjectName.ObjectTypeEnums.Synonym => "Synonym",
                _ => "Unknown"
            };
        }
    }
}