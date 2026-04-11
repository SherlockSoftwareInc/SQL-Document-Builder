using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The excel data helper.
    /// </summary>
    internal class ExcelDataHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelDataHelper"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        internal ExcelDataHelper(DataTable data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public DataTable Data { get; }

        /// <summary>
        /// Exports the descriptions to CSV.
        /// </summary>
        /// <param name="selectedObjects">The selected objects.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="connection">The connection.</param>
        internal static async Task ExportDescriptionsToCsv(List<ObjectName> selectedObjects, string fileName, DatabaseConnectionItem? connection)
        {
            if (connection == null || selectedObjects?.Count == 0)
            {
                return;
            }

            var lines = new List<string>
            {
                "Level0Type,Level0Name,Level1Type,Level1Name,Level2Type,Level2Name,Value"
            };

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                // Output the object descriptions
                await OutputObjectDescription(selectedObjects[i], lines, connection);
            }

            await File.WriteAllLinesAsync(fileName, lines, Encoding.UTF8);
        }

        /// <summary>
        /// Outputs the object description.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="lines">The CSV output lines.</param>
        /// <param name="connection">The connection.</param>
        private static async Task OutputObjectDescription(ObjectName objectName, List<string> lines, DatabaseConnectionItem connection)
        {
            // open the object
            var table = new DBObject();
            if (!await table.OpenAsync(objectName, connection))
            {
                return; // If the object cannot be opened, skip it
            }

            string level1Type = objectName.ObjectType switch
            {
                ObjectTypeEnums.Table => "TABLE",
                ObjectTypeEnums.View => "VIEW",
                ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                ObjectTypeEnums.Function => "FUNCTION",
                ObjectTypeEnums.Synonym => "SYNONYM",
                _ => ""
            };

            // Set the values for the row based on the objectName description
            lines.Add(ToCsvLine("SCHEMA", objectName.Schema, level1Type, objectName.Name, "", "", table.Description ?? ""));

            // output the columns for table or view
            if (objectName.ObjectType == ObjectTypeEnums.Table || objectName.ObjectType == ObjectTypeEnums.View)
            {
                // Loop through columns
                for (int r = 0; r < table.Columns.Count; r++)
                {
                    var col = table.Columns[r];

                    lines.Add(ToCsvLine("SCHEMA", objectName.Schema, level1Type, objectName.Name, "COLUMN", col.ColumnName, col.Description ?? ""));
                }
            }
            else if (objectName.ObjectType == ObjectTypeEnums.StoredProcedure || objectName.ObjectType == ObjectTypeEnums.Function)
            {
                // loop through parameters
                for (int r = 0; r < table.Parameters.Count; r++)
                {
                    var param = table.Parameters[r];

                    lines.Add(ToCsvLine("SCHEMA", objectName.Schema, level1Type, objectName.Name, "PARAMETER", param.Name, param.Description ?? ""));
                }
            }
        }

        private static string ToCsvLine(params string[] fields)
        {
            return string.Join(",", fields.Select(EscapeCsv));
        }

        private static string EscapeCsv(string? value)
        {
            value ??= string.Empty;
            if (value.Contains('"'))
            {
                value = value.Replace("\"", "\"\"");
            }

            if (value.Contains(',') || value.Contains('"') || value.Contains('\r') || value.Contains('\n'))
            {
                value = $"\"{value}\"";
            }

            return value;
        }

        /// <summary>
        /// Gets the description statement.
        /// </summary>
        /// <returns>A string.</returns>
        internal string GetDescriptionStatement(DatabaseConnectionItem? connection)
        {
            var descriptionStatements = new StringBuilder();
            var dbmsType = GetEffectiveDbmsType(connection);

            // Iterate through each row in the DataTable
            foreach (DataRow row in Data.Rows)
            {
                string value = row["Value"].ToString();
                if (string.IsNullOrEmpty(value))
                {
                    continue; // Skip rows with empty values
                }
                value = value.Replace("'", "''").TrimEnd('\r','\n'); // Escape single quotes

                string level0type = row["Level0Type"].ToString();
                string level0name = row["Level0Name"].ToString();
                string level1type = row["Level1Type"].ToString();
                string level1name = row["Level1Name"].ToString();
                string level2type = row["Level2Type"].ToString();
                string level2name = row["Level2Name"].ToString();

                if (dbmsType == DBMSTypeEnums.SQLServer)
                {
                    // Check if Level2Type is null or empty
                    if (string.IsNullOrEmpty(level2type))
                    {
                        descriptionStatements.AppendLine($"EXEC usp_addupdateextendedproperty @name = N'MS_Description', @value = N'{value}', @level0type = N'{level0type}', @level0name = N'{level0name}', @level1type = N'{level1type}', @level1name = N'{level1name}';");
                    }
                    else
                    {
                        descriptionStatements.AppendLine($"EXEC usp_addupdateextendedproperty @name = N'MS_Description', @value = N'{value}', @level0type = N'{level0type}', @level0name = N'{level0name}', @level1type = N'{level1type}', @level1name = N'{level1name}', @level2type = N'{level2type}', @level2name = N'{level2name}';");
                    }
                }
                else if (dbmsType == DBMSTypeEnums.PostgreSQL || dbmsType == DBMSTypeEnums.Oracle)
                {
                    if (string.IsNullOrEmpty(level2type))
                    {
                        var objectKind = level1type.Equals("VIEW", StringComparison.OrdinalIgnoreCase) ? "VIEW" : "TABLE";
                        descriptionStatements.AppendLine($"COMMENT ON {objectKind} {QuoteName(level0name, dbmsType)}.{QuoteName(level1name, dbmsType)} IS '{value}';");
                    }
                    else if (level2type.Equals("COLUMN", StringComparison.OrdinalIgnoreCase))
                    {
                        descriptionStatements.AppendLine($"COMMENT ON COLUMN {QuoteName(level0name, dbmsType)}.{QuoteName(level1name, dbmsType)}.{QuoteName(level2name, dbmsType)} IS '{value}';");
                    }
                }
                else if ((dbmsType == DBMSTypeEnums.MySQL || dbmsType == DBMSTypeEnums.MariaDB) &&
                         string.IsNullOrEmpty(level2type) &&
                         level1type.Equals("TABLE", StringComparison.OrdinalIgnoreCase))
                {
                    descriptionStatements.AppendLine($"ALTER TABLE {QuoteName(level0name, dbmsType)}.{QuoteName(level1name, dbmsType)} COMMENT = '{value}';");
                }
            }

            return descriptionStatements.ToString();
        }

        private static DBMSTypeEnums GetEffectiveDbmsType(DatabaseConnectionItem? connection)
        {
            if (connection == null)
            {
                return DBMSTypeEnums.Other;
            }

            if (connection.ConnectionType.Equals("SQL Server", StringComparison.OrdinalIgnoreCase))
            {
                return DBMSTypeEnums.SQLServer;
            }

            if (connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase) &&
                connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                return DBMSTypeEnums.Other;
            }

            return connection.DBMSType;
        }

        private static string QuoteName(string? name, DBMSTypeEnums dbmsType)
        {
            string value = name ?? string.Empty;

            return dbmsType switch
            {
                DBMSTypeEnums.MySQL or DBMSTypeEnums.MariaDB => $"`{value.Replace("`", "``")}`",
                DBMSTypeEnums.PostgreSQL or DBMSTypeEnums.Oracle => $"\"{value.Replace("\"", "\"\"")}\"",
                _ => $"[{value.Replace("]", "]]" )}]"
            };
        }

        /// <summary>
        /// Gets the insert statement from the DataTable.
        /// </summary>
        /// <returns>A string.</returns>
        internal string GetInsertStatement(string tableName, bool nullForBlank, DatabaseConnectionItem? connection = null)
        {
            var sb = new StringBuilder();
            var dbmsType = GetEffectiveDbmsType(connection);
            var isSqlServer = dbmsType == DBMSTypeEnums.SQLServer;

            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = "YourTableName";
            }

            // convert the table name to ObjectName
            // split the table name into schema and table name
            var schemaName = string.Empty;
            var parts = tableName.Split('.');
            if (parts.Length == 2)
            {
                schemaName = parts[0];
                tableName = parts[1];
            }
            else if (parts.Length > 2)
            {
                schemaName = parts[0];
                tableName = string.Join(".", parts.Skip(1));
            }
            var objectName = new ObjectName(ObjectTypeEnums.Table, schemaName, tableName);
            var quotedTableName = string.IsNullOrWhiteSpace(schemaName)
                ? QuoteName(tableName, dbmsType)
                : $"{QuoteName(schemaName, dbmsType)}.{QuoteName(tableName, dbmsType)}";

            // Infer types for each column using up to 1000 rows
            var inferredTypes = new Type[Data.Columns.Count];
            for (int i = 0; i < Data.Columns.Count; i++)
            {
                inferredTypes[i] = InferColumnType(i, 1000);
            }

            if (isSqlServer && Properties.Settings.Default.AddDropStatement)
            {
                sb.AppendLine($"IF OBJECT_ID(N'{objectName.FullName}', 'U') IS NOT NULL");
                sb.AppendLine($"\tDROP TABLE {objectName.FullName};");
                sb.AppendLine("GO");
            }

            // CREATE TABLE statement using inferred types (SQL Server only)
            if (isSqlServer)
            {
                sb.AppendLine($"CREATE TABLE {objectName.FullName} (");
                for (var i = 0; i < Data.Columns.Count; i++)
                {
                    var column = Data.Columns[i];
                    var type = inferredTypes[i];
                    sb.Append($"    [{column.ColumnName}] ");

                    string sqlType = type == typeof(int) ? "INT"
                        : type == typeof(long) ? "BIGINT"
                        : type == typeof(short) ? "SMALLINT"
                        : type == typeof(byte) ? "TINYINT"
                        : type == typeof(bool) ? "BIT"
                        : type == typeof(decimal) ? "DECIMAL(18,4)"
                        : type == typeof(double) ? "FLOAT"
                        : type == typeof(float) ? "REAL"
                        : type == typeof(DateTime) ? "DATETIME"
                        : type == typeof(Guid) ? "UNIQUEIDENTIFIER"
                        : $"NVARCHAR({(column.MaxLength > 0 ? column.MaxLength.ToString() : "255")})";

                    sb.Append(sqlType);

                    if (i < Data.Columns.Count - 1)
                        sb.AppendLine(",");
                    else
                        sb.AppendLine();
                }
                sb.AppendLine(");");
                sb.AppendLine("GO");
            }

            // Batch size with safe fallback
            int batchSize = 20;
            try
            {
                batchSize = Properties.Settings.Default.InsertBatchRows;
                if (batchSize <= 0) batchSize = 20;
            }
            catch { /* Use default if settings not available */ }

            for (int batchStart = 0; batchStart < Data.Rows.Count; batchStart += batchSize)
            {
                var batchEnd = Math.Min(batchStart + batchSize, Data.Rows.Count);

                sb.Append($"INSERT INTO {quotedTableName} (");
                for (var i = 0; i < Data.Columns.Count; i++)
                {
                    sb.Append(QuoteName(Data.Columns[i].ColumnName, dbmsType));
                    if (i < Data.Columns.Count - 1)
                        sb.Append(", ");
                }
                sb.AppendLine(") VALUES");

                for (int rowIndex = batchStart; rowIndex < batchEnd; rowIndex++)
                {
                    var row = Data.Rows[rowIndex];
                    sb.Append("\t(");
                    for (var i = 0; i < Data.Columns.Count; i++)
                    {
                        var type = inferredTypes[i];
                        var valueObj = row[i];
                        var valueStr = valueObj?.ToString();

                        // Only treat as NULL if DBNull or (string and nullForBlank and valueStr is exactly empty string)
                        if (valueObj == DBNull.Value || (type == typeof(string) && nullForBlank && valueStr == string.Empty))
                        {
                            sb.Append("NULL");
                        }
                        else if (type == typeof(DateTime))
                        {
                            if (DateTime.TryParse(valueStr, out var dt))
                                sb.Append($"'{dt:yyyy-MM-dd HH:mm:ss}'");
                            else
                                sb.Append("NULL");
                        }
                        else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
                        {
                            if (decimal.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
                                sb.Append(Convert.ToString(dec, CultureInfo.InvariantCulture));
                            else
                                sb.Append("NULL");
                        }
                        else if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                        {
                            if (long.TryParse(valueStr, out var lval))
                                sb.Append(lval);
                            else
                                sb.Append("NULL");
                        }
                        else if (type == typeof(bool))
                        {
                            if (bool.TryParse(valueStr, out var bval))
                                sb.Append(dbmsType == DBMSTypeEnums.PostgreSQL ? (bval ? "TRUE" : "FALSE") : (bval ? "1" : "0"));
                            else
                                sb.Append("NULL");
                        }
                        else if (type == typeof(Guid))
                        {
                            if (Guid.TryParse(valueStr, out var guidVal))
                                sb.Append($"'{guidVal}'");
                            else
                                sb.Append("NULL");
                        }
                        else
                        {
                            // if valueStr is 'null', treat it as NULL
                            if (valueStr!.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
                            {
                                sb.Append("NULL");
                            }
                            else
                            {
                                var value = valueStr?.Replace("'", "''");
                                sb.Append(isSqlServer ? $"N'{value}'" : $"'{value}'");
                            }
                        }

                        if (i < Data.Columns.Count - 1)
                            sb.Append(", ");
                    }
                    sb.Append(')');
                    if (rowIndex < batchEnd - 1)
                        sb.AppendLine(",");
                    else
                        sb.AppendLine(";");
                }
            }
            if (isSqlServer)
            {
                sb.AppendLine("GO");
            }
            //sb.AppendLine($"SELECT * FROM {objectName.FullName}");

            return sb.ToString();
        }

        /// <summary>
        /// Infers the column type.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <returns>A Type.</returns>
        private Type InferColumnType(int columnIndex, int maxRows = 1000)
        {
            int rowCount = Math.Min(Data.Rows.Count, maxRows);
            bool isInt = true, isLong = true, isDecimal = true, isDateTime = true, isBool = true, isGuid = true;
            bool foundValue = false;
            for (int i = 0; i < rowCount; i++)
            {
                var value = Data.Rows[i][columnIndex]?.ToString();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                foundValue = true;

                // If value is all digits and starts with '0' and length > 1, treat as string
                if (value.Length > 1 && value[0] == '0' && value.All(char.IsDigit))
                {
                    // This column must be string
                    isInt = isLong = isDecimal = isDateTime = isBool = isGuid = false;
                    break;
                }

                int intVal;
                long longVal;
                decimal decVal;
                DateTime dtVal;
                bool boolVal;
                Guid guidVal;

                if (isInt && !int.TryParse(value, out intVal))
                    isInt = false;
                if (isLong && !long.TryParse(value, out longVal))
                    isLong = false;
                if (isDecimal && !decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decVal))
                    isDecimal = false;
                if (isDateTime && !DateTime.TryParse(value, out dtVal))
                    isDateTime = false;
                if (isBool && !bool.TryParse(value, out boolVal))
                    isBool = false;
                if (isGuid && !Guid.TryParse(value, out guidVal))
                    isGuid = false;
            }

            if (!foundValue)
                return typeof(string);

            if (isInt) return typeof(int);
            if (isLong) return typeof(long);
            if (isDecimal) return typeof(decimal);
            if (isBool) return typeof(bool);
            if (isDateTime) return typeof(DateTime);
            if (isGuid) return typeof(Guid);
            return typeof(string);
        }
    }
}