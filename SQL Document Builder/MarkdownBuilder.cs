using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The markdown builder.
    /// </summary>
    internal class MarkdownBuilder
    {
        private readonly StringBuilder _md = new();

        /// <summary>
        /// Gets the function definition.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetFunctionProcedureDef(ObjectName objectName, string connectionString, string templateBody)
        {
            string doc = templateBody;

            // open the database oobjects
            var func = new DBObject();
            await func.OpenAsync(objectName, connectionString);

            // Replace placeholders with actual values
            doc = doc.Replace("[ObjectName]", objectName.Name);
            doc = doc.Replace("[ObjectSchema]", objectName.Schema);
            doc = doc.Replace("[ObjectFullName]", objectName.FullName);
            doc = doc.Replace("[ObjectType]", ObjectTypeToString(objectName.ObjectType));

            doc = doc.Replace("[Description]", func.Description);
            doc = doc.Replace("[Definition]", func.Definition);

            var sb = new StringBuilder();
            var paramDt = func.Parameters;
            if (paramDt != null && paramDt.Count > 0)
            {
                sb.AppendLine("| Ord | Name | Data Type | Direction | Description |");
                sb.AppendLine("|-----|------|-----------|-----------|-------------|");
                foreach (DBParameter dr in paramDt)
                {
                    string ord = dr.Ord;
                    string name = dr.Name;
                    string dataType = dr.DataType;
                    string direction = dr.Mode;
                    string description = dr.Description ?? string.Empty;
                    if (name.Length == 0) name = " ";
                    sb.AppendLine($"| {ord} | `{name}` | {dataType} | {direction} | {description} |");
                }
                doc = doc.Replace("[Parameters]", sb.ToString());
            }
            else
            {
                doc = doc.Replace("[Parameters]", "| - | _No parameters_ |  |  |  |  |\n");
            }

            //doc = doc.Replace("[Relationships]", objectName.Name);

            return doc;
        }

        /// <summary>
        /// Gets the table definition
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetTableViewDef(ObjectName objectName, string connectionString, string templateBody)
        {
            string doc = templateBody;

            // open the database oobjects
            var tableView = new DBObject();
            await tableView.OpenAsync(objectName, connectionString);

            // Replace placeholders with actual values
            doc = doc.Replace("[ObjectName]", objectName.Name);
            doc = doc.Replace("[ObjectSchema]", objectName.Schema);
            doc = doc.Replace("[ObjectFullName]", objectName.FullName);
            doc = doc.Replace("[ObjectType]", ObjectTypeToString(objectName.ObjectType));

            doc = doc.Replace("[Description]", tableView.Description.Length == 0 ? " " : tableView.Description);
            doc = doc.Replace("[Definition]", tableView.Definition);

            // Build the columns table
            if (doc.Contains("[Columns]"))
            {
                var columnBody = GetColumnsBody(tableView.Columns);
                doc = doc.Replace("[Columns]", columnBody);
            }

            // Build the indexes table
            if (doc.Contains("[Indexes]"))
            {
                var indexBody = GetIndexesBody(tableView.Indexes);
                doc = doc.Replace("[Indexes]", indexBody);
            }

            // Build the constraints table
            if (doc.Contains("[Constraints]"))
            {
                var constraintBody = GetConstraintsBody(tableView.Constraints);
                doc = doc.Replace("[Constraints]", constraintBody);
            }

            /*
            sb.Clear();
            var indexDt = tableView.Indexes;
            if (indexDt != null && indexDt.Count > 0)
            {
                sb.AppendLine("| Index Name | Type | Columns | Unique | Description |");
                sb.AppendLine("|------------|------|---------|--------|-------------|");
                foreach (DBIndex idx in indexDt)
                {
                    string idxName = idx.Name;
                    string type = idx.Type;
                    string columns = idx.Columns;
                    string unique = idx.IsUnique ? "Yes" : "No";
                    string description = idx.Description ?? string.Empty;
                    sb.AppendLine($"| `{idxName}` | {type} | {columns} | {unique} | {description} |");
                }
                doc = doc.Replace("[Indexes]", sb.ToString());
            }
            else
            {
                doc = doc.Replace("[Indexes]", "_No indexes found_\n");
            }
            */

            //doc = doc.Replace("[Triggers]", objectName.Name);
            //doc = doc.Replace("[Relationships]", objectName.Name);

            /*
            _md.Clear();

            // Title
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
                AppendLine($"# TABLE NAME: `{objectName.Schema}.{objectName.Name}`");
            else
                AppendLine($"# VIEW NAME: `{objectName.Schema}.{objectName.Name}`");

            var tableView = new DBObject();
            // Open the database object
            await tableView.OpenAsync(objectName, connectionString);

            // Description
            if (!string.IsNullOrWhiteSpace(tableView.Description))
                AppendLine($"\n> {tableView.Description}\n");

            // Table structure
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
                AppendLine("## Table Structure");
            else
                AppendLine("## View Structure");
            AppendLine("| Ord | Name | Data Type | Description |");
            AppendLine("|--------|------|-----------|-------------|");

            foreach (DBColumn col in tableView.Columns) // Fix: Use DBColumn instead of DataColumn
            {
                string ord = col.Ord ?? string.Empty; // Fix: Access Ord from DBColumn
                string colName = col.ColumnName;
                if (string.IsNullOrEmpty(colName)) colName = " ";
                AppendLine($"| {ord} | `{colName}` | {col.DataType} | {col.Description} |");
            }

            // Add view definition if it's a view
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
            {
                AppendLine("\n## View Definition\n");
                AppendLine("```sql");

                if (string.IsNullOrEmpty(tableView.Definition))
                {
                    AppendLine("-- Definition not found --");
                }
                else
                {
                    AppendLine(tableView.Definition.Trim());
                }
                AppendLine("```");
            }

            return _md.ToString();
            */

            return doc;
        }

        /// <summary>
        /// Gets the constraints body.
        /// </summary>
        /// <param name="constraints">The constraints.</param>
        /// <returns>A string.</returns>
        private static string GetConstraintsBody(List<ConstraintItem> constraints)
        {
            if (constraints == null || constraints.Count == 0)
            {
                return "_No constraints found_";
            }

            var sb = new StringBuilder();
            sb.AppendLine("| Constraint Name | Type | Column |");
            sb.AppendLine("|------------------|------|-------------|");

            foreach (var constraint in constraints)
            {
                sb.AppendLine($"| `{constraint.Name}` | {constraint.Type} | {constraint.Column.QuotedName() ?? string.Empty} |");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the columns body.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A string.</returns>
        private static string GetColumnsBody(List<DBColumn> columns)
        {
            var sb = new StringBuilder();
            if (columns.Count > 0)
            {
                sb.AppendLine("| Ord | Name | Data Type | Description |");
                sb.AppendLine("|--------|------|-----------|-------------|");

                foreach (DBColumn col in columns) // Fix: Use DBColumn instead of DataColumn
                {
                    string ord = col.Ord ?? string.Empty; // Fix: Access Ord from DBColumn
                    string colName = col.ColumnName;
                    if (string.IsNullOrEmpty(colName)) colName = " ";
                    sb.AppendLine($"| {ord} | `{colName}` | {col.DataType} | {col.Description} |");
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                return "_No columns found_";
            }
        }

        


        /// <summary>
        /// Gets the indexes body.
        /// </summary>
        /// <param name="indexes">The indexes.</param>
        /// <returns>An object.</returns>
        private static string GetIndexesBody(List<IndexItem> indexes)
        {
            // Create a StringBuilder to build the markdown table
            var sb = new StringBuilder();

            // Check if there are any indexes
            if (indexes != null && indexes.Count > 0)
            {
                sb.AppendLine("| Index Name | Type | Columns | Unique |");
                sb.AppendLine("|------------|------|---------|--------|");

                // Iterate through each index and append its details to the table
                foreach (var index in indexes)
                {
                    string idxName = index.Name;
                    string type = index.Type;
                    string columns = index.Columns;
                    string unique = index.IsUnique ? "Yes" : "No";
                    sb.AppendLine($"| `{idxName}` | {type} | {columns} | {unique} |");
                }
            }
            else
            {
                sb.AppendLine("_No indexes found_");
            }

            // Return the built markdown table as an object
            return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
        }


        /// <summary>
        /// Builds the function list.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> BuildFunctionListAsync(string schemaName, string connectionString, IProgress<int> progress)
        {
            _md.Clear();

            string sql = "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' ORDER BY ROUTINE_NAME";

            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                AppendLine("| Schema | Function | Description |");
                AppendLine("|--------|----------|-------------|");

                for (int i = 0; i < dt?.Rows.Count; i++)
                {
                    int percentComplete = (i * 100) / dt.Rows.Count;
                    if (percentComplete > 0 && percentComplete % 2 == 0)
                    {
                        progress.Report(percentComplete + 1);
                    }

                    DataRow dr = dt.Rows[i];

                    string schema = (string)dr[0];
                    bool generate = true;
                    if (schemaName.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string fnName = (string)dr[1];
                        AppendLine($"| {schema} | `{fnName}` |  |");
                    }
                }
            }

            return _md.ToString();
        }

        /// <summary>
        /// Builds the stored procedure list async.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> BuildProcedureListAsync(string schemaName, string connectionString, IProgress<int> progress)
        {
            _md.Clear();

            string sql = "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_NAME";

            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                AppendLine("| Schema | Stored Procedure | Description |");
                AppendLine("|--------|------------------|-------------|");

                for (int i = 0; i < dt?.Rows.Count; i++)
                {
                    int percentComplete = (i * 100) / dt.Rows.Count;
                    if (percentComplete > 0 && percentComplete % 2 == 0)
                    {
                        progress.Report(percentComplete + 1);
                    }

                    DataRow dr = dt.Rows[i];

                    string schema = (string)dr[0];
                    bool generate = true;
                    if (schemaName.Length > 0 && !schemaName.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        generate = false;
                    }
                    if (generate)
                    {
                        string spName = (string)dr[1];
                        AppendLine($"| {schema} | `{spName}` |  |");
                    }
                }
            }

            return _md.ToString();
        }

        /// <summary>
        /// Builds the table list.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> BuildTableList(string schemaName, string connectionString, IProgress<int> progress)
        {
            _md.Clear();

            string sql = schemaName.Length == 0
                ? "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables ORDER BY table_schema, table_name"
                : $"SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables WHERE SCHEMA_NAME(schema_id) = N'{schemaName}' ORDER BY table_schema, table_name";

            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                AppendLine("| Schema | Table Name | Description |");
                AppendLine("|--------|------------|-------------|");

                for (int i = 0; i < dt?.Rows.Count; i++)
                {
                    int percentComplete = (i * 100) / dt.Rows.Count;
                    if (percentComplete > 0 && percentComplete % 2 == 0)
                    {
                        progress.Report(percentComplete + 1);
                    }

                    DataRow dr = dt.Rows[i];
                    string tableSchema = (string)dr[0];
                    string tableName = (string)dr[1];
                    string description = await DatabaseHelper.GetTableDescriptionAsync(
                        new ObjectName() { Schema = tableSchema, Name = tableName }
                        , connectionString);
                    AppendLine($"| {tableSchema} | `{tableName}` | {description} |");
                }
            }

            return _md.ToString();
        }

        /// <summary>
        /// Builds the view list async.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> BuildViewListAsync(string schemaName, string connectionString, IProgress<int> progress)
        {
            _md.Clear();

            string sql = schemaName.Length == 0
                ? "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views ORDER BY table_schema, table_name"
                : $"SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views WHERE SCHEMA_NAME(schema_id) = N'{schemaName}' ORDER BY table_schema, table_name";

            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt?.Rows.Count > 0)
            {
                AppendLine("| Schema | View Name | Description |");
                AppendLine("|--------|-----------|-------------|");

                for (int i = 0; i < dt?.Rows.Count; i++)
                {
                    int percentComplete = (i * 100) / dt.Rows.Count;
                    if (percentComplete > 0 && percentComplete % 2 == 0)
                    {
                        progress.Report(percentComplete + 1);
                    }

                    DataRow dr = dt.Rows[i];
                    string tableSchema = (string)dr[0];
                    string viewName = (string)dr[1];
                    string description = await DatabaseHelper.GetTableDescriptionAsync(
                        new ObjectName() { Schema = tableSchema, Name = viewName, ObjectType = ObjectName.ObjectTypeEnums.View }
                        , connectionString);
                    AppendLine($"| {tableSchema} | `{viewName}` | {description} |");
                }
            }

            return _md.ToString();
        }

        /// <summary>
        /// Gets the stored procedure def.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> GetStoredProcedureDef(ObjectName objectName, string connectionString)
        {
            _md.Clear();

            // Title
            AppendLine($"# Stored Procedure: `{objectName.FullName}`");

            // open the database oobjects
            var proc = new DBObject();
            await proc.OpenAsync(objectName, connectionString);

            // Description
            if (!string.IsNullOrWhiteSpace(proc.Description))
            {
                AppendLine("");
                AppendLine("## Description");
                AppendLine(proc.Description);
            }

            // SQL Code
            AppendLine("");
            AppendLine("## SQL Code");
            AppendLine("```sql");

            if (!string.IsNullOrWhiteSpace(proc.Definition))
            {
                AppendLine(proc.Definition.Trim());
            }
            else
            {
                AppendLine("-- Definition not found --");
            }

            AppendLine("```");

            return _md.ToString();
        }

        /// <summary>
        /// Gets the table values async.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> GetTableValuesAsync(string fullName, string connectionString)
        {
            _md.Clear();

            AppendLine($"## Table Values\n");

            string sql = $"SELECT * FROM {fullName}";
            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt != null && dt.Rows.Count > 0)
            {
                // Header
                var headers = new StringBuilder("|");
                var separators = new StringBuilder("|");
                foreach (DataColumn col in dt.Columns)
                {
                    headers.Append($" {col.ColumnName} |");
                    separators.Append("------------|");
                }
                AppendLine(headers.ToString());
                AppendLine(separators.ToString());

                // Rows
                foreach (DataRow dr in dt.Rows)
                {
                    var row = new StringBuilder("|");
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Append($" {dr[col]?.ToString() ?? ""} |");
                    }
                    AppendLine(row.ToString());
                }
            }
            else
            {
                AppendLine("_No data found._");
            }

            return _md.ToString();
        }

        /// <summary>
        /// Converts tabular text data (tab-separated, first line is header) to a Markdown table.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        /// <returns>A string.</returns>
        internal string TextToTable(string metaData)
        {
            _md.Clear();

            if (string.IsNullOrWhiteSpace(metaData))
                return string.Empty;

            // Normalize line endings and split into lines
            metaData = metaData.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
            var lines = metaData.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return string.Empty;

            // Header
            var headers = lines[0].Split('\t');
            var headerLine = new StringBuilder("|");
            var separatorLine = new StringBuilder("|");
            foreach (var header in headers)
            {
                headerLine.Append($" {header.Trim()} |");
                separatorLine.Append(" --- |");
            }
            AppendLine(headerLine.ToString());
            AppendLine(separatorLine.ToString());

            // Rows
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split('\t');
                var rowLine = new StringBuilder("|");
                foreach (var col in columns)
                {
                    rowLine.Append($" {col.Trim()} |");
                }
                AppendLine(rowLine.ToString());
            }

            return _md.ToString();
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
        /// Appends the line.
        /// </summary>
        /// <param name="text">The text.</param>
        private void AppendLine(string text) => _md.AppendLine(text);
    }
}