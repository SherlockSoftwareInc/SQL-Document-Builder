using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
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
        /// Gets the insert statement from the DataTable.
        /// </summary>
        /// <returns>A string.</returns>
        internal string GetInsertStatement(string tableName, bool nullForBlank)
        {
            var sb = new StringBuilder();

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

            // Infer types for each column using up to 1000 rows
            var inferredTypes = new Type[Data.Columns.Count];
            for (int i = 0; i < Data.Columns.Count; i++)
            {
                inferredTypes[i] = InferColumnType(i, 1000);
            }

            if (Properties.Settings.Default.AddDropStatement)
            {
                sb.AppendLine($"IF OBJECT_ID('{objectName.FullName}', 'U') IS NOT NULL");
                sb.AppendLine($"\tDROP TABLE {objectName.FullName};");
                sb.AppendLine("GO");
            }

            // CREATE TABLE statement using inferred types
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

                sb.Append($"INSERT INTO {objectName.FullName} (");
                for (var i = 0; i < Data.Columns.Count; i++)
                {
                    sb.Append($"[{Data.Columns[i].ColumnName}]");
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
                        else if (type == typeof(string))
                        {
                            var value = valueStr?.Replace("'", "''");
                            sb.Append($"N'{value}'");
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
                                sb.Append(bval ? "1" : "0");
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
                            var value = valueStr?.Replace("'", "''");
                            sb.Append($"N'{value}'");
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
            sb.AppendLine("GO");
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