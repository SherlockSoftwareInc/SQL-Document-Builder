using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        //private SQLDatabaseConnectionItem? _selectedConnection;
        private readonly string? _server = string.Empty;

        private readonly System.Text.StringBuilder _script = new();

        /// <summary>
        /// Builds the table list async.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        public async Task<string> BuildTableListAsync(string schemaName, string connectionString, IProgress<int> progress)
        {
            _script.Clear();

            try
            {
                string sql = schemaName.Length == 0
                    ? "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables ORDER BY table_schema, table_name"
                    : $"SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.tables WHERE SCHEMA_NAME(schema_id) = N'{schemaName}' ORDER BY table_schema, table_name";

                DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

                if (dt?.Rows.Count > 0)
                {
                    AppendLine("""
<table class="wikitable" style="margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;">
<tbody>
    <tr>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Schema</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Table Name</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Description</th>
    </tr>
""");

                    // Optionally, batch-fetch all descriptions here for better performance
                    // For now, process sequentially
                    int lastPercent = -1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int percentComplete = (i * 100) / dt.Rows.Count;
                        if (percentComplete != lastPercent && percentComplete % 2 == 0)
                        {
                            progress.Report(percentComplete + 1);
                            lastPercent = percentComplete;
                        }

                        DataRow dr = dt.Rows[i];
                        string tableSchema = dr["table_schema"]?.ToString() ?? string.Empty;
                        string tableName = dr["table_name"]?.ToString() ?? string.Empty;

                        string description = await DatabaseHelper.GetTableDescriptionAsync(
                            new ObjectName { Schema = tableSchema, Name = tableName }
                            , connectionString
                        );

                        AppendLine($"""
    <tr>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">{tableSchema}</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">[[{tableSchema}.{tableName}|{tableName}]]</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">{description}</td>
    </tr>
""");
                    }
                    AppendLine("</tbody>");
                    AppendLine("</table>");
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return _script.ToString();
        }

        /// <summary>
        /// Scan user view in the database and generate the creation script for them
        /// </summary>
        public async Task<string> BuildViewListAsync(string schemaName, string connectionString, IProgress<int> progress)
        {
            _script.Clear();

            try
            {
                string sql = schemaName.Length == 0
                    ? "SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views ORDER BY table_schema, table_name"
                    : $"SELECT SCHEMA_NAME(schema_id) AS table_schema, name AS table_name FROM sys.views WHERE SCHEMA_NAME(schema_id) = N'{schemaName}' ORDER BY table_schema, table_name";

                DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

                if (dt?.Rows.Count > 0)
                {
                    AppendLine("""
<table class="wikitable" style="margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;">
<tbody>
    <tr>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Schema</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">View Name</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Description</th>
    </tr>
""");

                    int lastPercent = -1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int percentComplete = (i * 100) / dt.Rows.Count;
                        if (percentComplete != lastPercent && percentComplete % 2 == 0)
                        {
                            progress.Report(percentComplete + 1);
                            lastPercent = percentComplete;
                        }

                        DataRow dr = dt.Rows[i];
                        string tableSchema = dr["table_schema"]?.ToString() ?? string.Empty;
                        string tableName = dr["table_name"]?.ToString() ?? string.Empty;

                        string description = await DatabaseHelper.GetTableDescriptionAsync(
                            new ObjectName
                            {
                                Schema = tableSchema,
                                Name = tableName,
                                ObjectType = ObjectName.ObjectTypeEnums.View
                            }
                            , connectionString
                        );

                        AppendLine($"""
    <tr>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">{tableSchema}</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">[[{tableSchema}.{tableName}|{tableName}]]</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">{description}</td>
    </tr>
""");
                    }
                    AppendLine("</tbody>");
                    AppendLine("</table>");
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return _script.ToString();
        }

        /// <summary>
        /// Scan user view in the database and generate the creation script for them
        /// </summary>
        public async Task<string> BuildSPList(string schemaName, string connectionString)
        {
            _script.Clear();

            try
            {
                string sql = schemaName.Length == 0
                    ? "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_SCHEMA, ROUTINE_NAME"
                    : $"SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'PROCEDURE' AND ROUTINE_SCHEMA = N'{schemaName}' AND LEFT(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_') ORDER BY ROUTINE_SCHEMA, ROUTINE_NAME";

                var dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

                if (dt?.Rows.Count > 0)
                {
                    AppendLine("""
<table class="wikitable" style="margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;">
<tbody>
    <tr>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Schema</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Stored Procedure</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Description</th>
    </tr>
""");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string routineSchema = dt.Rows[i]["ROUTINE_SCHEMA"]?.ToString() ?? string.Empty;
                        string routineName = dt.Rows[i]["ROUTINE_NAME"]?.ToString() ?? string.Empty;

                        AppendLine($"""
    <tr>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">{routineSchema}</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">[[{routineSchema}.{routineName}|{routineName}]]</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"></td>
    </tr>
""");
                    }
                    AppendLine("</tbody>");
                    AppendLine("</table>");
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return _script.ToString();
        }

        /// <summary>
        /// Scan user view in the database and generate the creation script for them
        /// </summary>
        /// <summary>
        /// Scan user functions in the database and generate the creation script for them
        /// </summary>
        public async Task<string> BuildFunctionList(string schemaName, string connectionString)
        {
            _script.Clear();

            try
            {
                string sql = schemaName.Length == 0
                    ? "SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' ORDER BY ROUTINE_SCHEMA, ROUTINE_NAME"
                    : $"SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND ROUTINE_SCHEMA = N'{schemaName}' ORDER BY ROUTINE_SCHEMA, ROUTINE_NAME";

                DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

                if (dt?.Rows.Count > 0)
                {
                    AppendLine("""
<table class="wikitable" style="margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;">
<tbody>
    <tr>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Schema</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Function</th>
        <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Description</th>
    </tr>
""");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string routineSchema = dt.Rows[i]["ROUTINE_SCHEMA"]?.ToString() ?? string.Empty;
                        string routineName = dt.Rows[i]["ROUTINE_NAME"]?.ToString() ?? string.Empty;

                        AppendLine($"""
    <tr>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">{routineSchema}</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;">[[{routineSchema}.{routineName}|{routineName}]]</td>
        <td style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"></td>
    </tr>
""");
                    }
                    AppendLine("</tbody>");
                    AppendLine("</table>");
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return _script.ToString();
        }

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
        /// Append text to the bottom of the text box
        /// </summary>
        /// <param name="text"></param>
        private void AppendLine(string text)
        {
            //sqlTextBox.AppendText(text + Environment.NewLine);
            _script.AppendLine(text);
        }

        /// <summary>
        /// Generate contect for wiki of the give table
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        public async Task<string> GetTableViewDef(ObjectName objectName, string connectionString)
        {
            _script.Clear();
            AppendLine($"<h1>{(objectName.ObjectType == ObjectName.ObjectTypeEnums.Table ? "TABLE" : "VIEW")} NAME: {objectName.Schema}.{objectName.Name}</h1>");
            var objectDesc = await DatabaseHelper.GetTableDescriptionAsync(objectName, connectionString);
            if (objectDesc.Length > 0)
            {
                AppendLine("<p>" + objectDesc + "</p>");
            }

            AppendLine("""
<div>
    <table class="wikitable" style="margin: 1em 0px; border: 1px solid #a2a9b1; color: #202122; font-family: sans-serif; font-size: 14px; background-color: #f8f9fa;">
    <tbody>
        <tr>
            <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Col ID</th>
            <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Name</th>
            <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Data Type</th>
            <th style="padding: 0.2em 0.4em; border: 1px solid #a2a9b1; text-align: center; background-color: #eaecf0;">Description</th>
        </tr>
""");

            await GetTableDefinition(objectName, connectionString);

            AppendLine("\t</tbody>");
            AppendLine("\t</table>");
            AppendLine("</div>");

            return _script.ToString();
        }

        /// <summary>
        /// Get table structure for wiki
        /// </summary>
        /// <param name="TableSchema">Schame name</param>
        /// <param name="TableName">Table name</param>
        /// <returns></returns>
        private async Task GetTableDefinition(ObjectName objectName, string connectionString)
        {
            try
            {
                string sql = $@"
SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = N'{objectName.Schema}' AND TABLE_NAME = N'{objectName.Name}'
ORDER BY ORDINAL_POSITION";

                DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);

                if (dt?.Rows.Count > 0)
                {
                    // Fetch all column descriptions in parallel
                    var descTasks = new Task<string>[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string colName = dt.Rows[i]["COLUMN_NAME"]?.ToString() ?? string.Empty;
                        descTasks[i] = DatabaseHelper.GetColumnDescriptionAsync(objectName, colName, connectionString);
                    }
                    var descriptions = await Task.WhenAll(descTasks);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        string ord = dr["ORDINAL_POSITION"]?.ToString() ?? string.Empty;
                        string colName = dr["COLUMN_NAME"]?.ToString() ?? string.Empty;
                        string dataType = dr["DATA_TYPE"]?.ToString() ?? string.Empty;
                        if (dr["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value && dr["CHARACTER_MAXIMUM_LENGTH"] != null)
                        {
                            dataType += $"({dr["CHARACTER_MAXIMUM_LENGTH"]})";
                        }
                        string colDesc = descriptions[i];

                        AppendLine($@"        <tr>
            <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{ord}</td>
            <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{colName}</td>
            <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{dataType}</td>
            <td style=""padding: 0.2em 0.4em; border: 1px solid #a2a9b1;"">{colDesc}</td>
        </tr>");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Build value list of the given table for wiki
        /// </summary>
        /// <param name="tableName"></param>
        public async Task<string> GetTableValuesAsync(string tableName, string connectionString)
        {
            _script.Clear();
            AppendLine("<div>");
            AppendLine("<h2>Table Values:</h2>");
            try
            {
                string sql = string.Format("SELECT * FROM {0}", tableName);
                AppendLine(await Common.QueryDataToHTMLTableAsync(sql, connectionString));
            }
            catch (Exception ex)
            {
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            AppendLine("</div>");

            return _script.ToString();
        }
    }
}