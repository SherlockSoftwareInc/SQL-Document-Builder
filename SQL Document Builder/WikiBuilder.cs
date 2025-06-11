using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The wiki builder.
    /// </summary>
    internal class WikiBuilder
    {
        private readonly StringBuilder _wiki = new();

        /// <summary>
        /// Gets the function definition in wiki format.
        /// </summary>
        internal static async Task<string> GetFunctionProcedureDef(ObjectName objectName, string connectionString, string templateBody)
        {
            string doc = templateBody;

            var func = new DBObject();
            await func.OpenAsync(objectName, connectionString);

            doc = doc.Replace("~ObjectName~", objectName.Name);
            doc = doc.Replace("~ObjectSchema~", objectName.Schema);
            doc = doc.Replace("~ObjectFullName~", objectName.FullName);
            doc = doc.Replace("~ObjectType~", ObjectTypeToString(objectName.ObjectType));

            doc = doc.Replace("~Description~", func.Description);
            doc = doc.Replace("~Definition~", func.Definition);

            var sb = new StringBuilder();
            var paramDt = func.Parameters;
            if (paramDt != null && paramDt.Count > 0)
            {
                sb.AppendLine("{| class=\"wikitable\"");
                sb.AppendLine("! Ord !! Name !! Data Type !! Direction !! Description");
                foreach (DBParameter dr in paramDt)
                {
                    string ord = dr.Ord;
                    string name = dr.Name;
                    string dataType = dr.DataType;
                    string direction = dr.Mode;
                    string description = dr.Description ?? string.Empty;
                    if (name.Length == 0) name = " ";
                    sb.AppendLine($"|-");
                    sb.AppendLine($"| {ord} || <code>{name}</code> || {dataType} || {direction} || {description}");
                }
                sb.AppendLine("|}");
                doc = doc.Replace("~Parameters~", sb.ToString());
            }
            else
            {
                doc = doc.Replace("~Parameters~", "{| class=\"wikitable\"\n|-\n| - || ''No parameters'' ||  ||  ||  \n|}");
            }

            return doc;
        }

        /// <summary>
        /// Gets the table or view definition in wiki format.
        /// </summary>
        internal static async Task<string> GetTableViewDef(ObjectName objectName, string connectionString, string templateBody)
        {
            string doc = templateBody;

            var tableView = new DBObject();
            await tableView.OpenAsync(objectName, connectionString);

            doc = doc.Replace("~ObjectName~", objectName.Name);
            doc = doc.Replace("~ObjectSchema~", objectName.Schema);
            doc = doc.Replace("~ObjectFullName~", objectName.FullName);
            doc = doc.Replace("~ObjectType~", ObjectTypeToString(objectName.ObjectType));

            doc = doc.Replace("~Description~", tableView.Description.Length == 0 ? " " : tableView.Description);
            doc = doc.Replace("~Definition~", tableView.Definition);

            if (doc.Contains("~Columns~"))
            {
                var columnBody = GetColumnsBody(tableView.Columns);
                doc = doc.Replace("~Columns~", columnBody);
            }

            if (doc.Contains("~Indexes~"))
            {
                var indexBody = GetIndexesBody(tableView.Indexes);
                doc = doc.Replace("~Indexes~", indexBody);
            }

            if (doc.Contains("~Constraints~"))
            {
                var constraintBody = GetConstraintsBody(tableView.Constraints);
                doc = doc.Replace("~Constraints~", constraintBody);
            }

            return doc;
        }

        /// <summary>
        /// Builds the table or view list in wiki format.
        /// </summary>
        internal async Task<string> BuildObjectList(List<ObjectName> objectList, string connectionString, IProgress<int> progress)
        {
            _wiki.Clear();

            if (objectList.Count > 0)
            {
                AppendLine("{| class=\"wikitable\"");
                AppendLine("! Schema !! Name !! Description");

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
                    AppendLine("|-");
                    AppendLine($"| {tableSchema} || [[{tableSchema}.{tableName}|{tableName}]] || {description}");
                }
                AppendLine("|}");
            }

            return _wiki.ToString();
        }

        /// <summary>
        /// Gets the table values in wiki format.
        /// </summary>
        internal static async Task<string> GetTableValuesAsync(string sql, string connectionString)
        {
            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
            if (dt == null || dt.Rows.Count == 0)
            {
                return "''No data found.''";
            }
            return DataTableToDoc(dt);
        }

        /// <summary>
        /// Gets the table values async.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Task.</returns>
        internal static string DataTableToDoc(DataTable data)
        {
            var sb = new StringBuilder();

            sb.AppendLine("== Values ==");

            if (data != null && data.Rows.Count > 0)
            {
                var headers = new StringBuilder("{| class=\"wikitable\"\n|-\n");
                foreach (DataColumn col in data.Columns)
                {
                    headers.Append($"! {col.ColumnName} ");
                }
                headers.Append("\n");

                sb.Append(headers);

                foreach (DataRow dr in data.Rows)
                {
                    sb.Append("|-\n");
                    foreach (DataColumn col in data.Columns)
                    {
                        sb.Append($"| {dr[col]?.ToString() ?? ""} ");
                    }
                    sb.Append("\n");
                }
                sb.Append("|}");
            }
            else
            {
                sb.AppendLine("''No data found.''");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts tabular text data (tab-separated, first line is header) to a wiki table.
        /// </summary>
        internal string TextToTable(string metaData)
        {
            _wiki.Clear();

            if (string.IsNullOrWhiteSpace(metaData))
                return string.Empty;

            metaData = metaData.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
            var lines = metaData.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return string.Empty;

            var headers = lines[0].Split('\t');
            var headerLine = new StringBuilder("{| class=\"wikitable\"\n|-\n");
            foreach (var header in headers)
            {
                headerLine.Append($"! {header.Trim()} ");
            }
            headerLine.Append("\n");
            AppendLine(headerLine.ToString());

            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split('\t');
                var rowLine = new StringBuilder("|-\n");
                foreach (var col in columns)
                {
                    rowLine.Append($"| {col.Trim()} ");
                }
                rowLine.Append("\n");
                AppendLine(rowLine.ToString());
            }
            AppendLine("|}");

            return _wiki.ToString();
        }

        /// <summary>
        /// Gets the columns body in wiki format.
        /// </summary>
        private static string GetColumnsBody(List<DBColumn> columns)
        {
            var sb = new StringBuilder();
            if (columns.Count > 0)
            {
                sb.AppendLine("{| class=\"wikitable\"");
                sb.AppendLine("! Ord !! Name !! Data Type !! Description");
                foreach (DBColumn col in columns)
                {
                    string ord = col.Ord ?? string.Empty;
                    string colName = col.ColumnName;
                    if (string.IsNullOrEmpty(colName)) colName = " ";
                    sb.AppendLine("|-");
                    sb.AppendLine($"| {ord} || <code>{colName}</code> || {col.DataType} || {col.Description}");
                }
                sb.AppendLine("|}");
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return "''No columns found''";
            }
        }

        /// <summary>
        /// Gets the constraints body in wiki format.
        /// </summary>
        private static string GetConstraintsBody(List<ConstraintItem> constraints)
        {
            if (constraints == null || constraints.Count == 0)
            {
                return "''No constraints found''";
            }

            var sb = new StringBuilder();
            sb.AppendLine("{| class=\"wikitable\"");
            sb.AppendLine("! Constraint Name !! Type !! Column");
            foreach (var constraint in constraints)
            {
                sb.AppendLine("|-");
                sb.AppendLine($"| <code>{constraint.Name}</code> || {constraint.Type} || {constraint.Column.QuotedName() ?? string.Empty}");
            }
            sb.AppendLine("|}");
            return sb.ToString();
        }

        /// <summary>
        /// Gets the indexes body in wiki format.
        /// </summary>
        private static string GetIndexesBody(List<IndexItem> indexes)
        {
            var sb = new StringBuilder();

            if (indexes != null && indexes.Count > 0)
            {
                sb.AppendLine("{| class=\"wikitable\"");
                sb.AppendLine("! Index Name !! Type !! Columns !! Unique");
                foreach (var index in indexes)
                {
                    string idxName = index.Name;
                    string type = index.Type;
                    string columns = index.Columns;
                    string unique = index.IsUnique ? "Yes" : "No";
                    sb.AppendLine("|-");
                    sb.AppendLine($"| <code>{idxName}</code> || {type} || {columns} || {unique}");
                }
                sb.AppendLine("|}");
            }
            else
            {
                sb.AppendLine("''No indexes found''");
            }

            return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
        }

        /// <summary>
        /// Converts object type to string.
        /// </summary>
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
        /// Appends a line to the wiki builder.
        /// </summary>
        private void AppendLine(string text) => _wiki.AppendLine(text);
    }
}