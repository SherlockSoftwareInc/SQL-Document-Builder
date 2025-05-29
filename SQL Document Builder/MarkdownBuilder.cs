using System;
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
        /// Appends the line.
        /// </summary>
        /// <param name="text">The text.</param>
        private void AppendLine(string text) => _md.AppendLine(text);

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
        /// Builds the stored procedure list async.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> BuildSPListAsync(string schemaName, string connectionString, IProgress<int> progress)
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
        /// Gets the table def.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> GetTableDef(ObjectName objectName, string connectionString)
        {
            _md.Clear();

            // Title
            if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
                AppendLine($"# TABLE NAME: `{objectName.Schema}.{objectName.Name}`");
            else
                AppendLine($"# VIEW NAME: `{objectName.Schema}.{objectName.Name}`");

            // Description
            var objectDesc = await DatabaseHelper.GetTableDescriptionAsync(objectName, connectionString);
            if (!string.IsNullOrWhiteSpace(objectDesc))
                AppendLine($"\n> {objectDesc}\n");

            // Table structure
            AppendLine("## Table Structure");
            AppendLine("| Col ID | Name | Data Type | Description |");
            AppendLine("|--------|------|-----------|-------------|");

            string sql = $"SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = N'{objectName.Schema}' AND TABLE_NAME = N'{objectName.Name}' ORDER BY ORDINAL_POSITION";
            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string colID = dr["ORDINAL_POSITION"].ToString() ?? "";
                    string colName = dr["COLUMN_NAME"].ToString() ?? "";
                    string dataType = dr["DATA_TYPE"].ToString() ?? "";
                    if (dr["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                        dataType += $"({dr["CHARACTER_MAXIMUM_LENGTH"]})";
                    string colDesc = await DatabaseHelper.GetColumnDescriptionAsync(objectName, colName, connectionString);
                    AppendLine($"| {colID} | `{colName}` | {dataType} | {colDesc} |");
                }
            }

            // Additional sections (ETL, variables, etc.) can be added here as needed.

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

            AppendLine($"## Table Values for `{fullName}`\n");

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
        /// Texts the to table.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        /// <returns>A string.</returns>
        /// <summary>
        /// Converts tabular text data (tab-separated, first line is header) to a Markdown table.
        /// </summary>
        /// <param name="metaData">The tabular text data.</param>
        /// <returns>A Markdown table string.</returns>
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
    }
}