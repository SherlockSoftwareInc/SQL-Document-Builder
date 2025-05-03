using System;
using System.Data;
using System.Text;

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
        internal string GetInsertStatement()
        {
            var sb = new StringBuilder();

            // Add drop table statement
            sb.AppendLine($"IF OBJECT_ID('[YourTableName]', 'U') IS NOT NULL");
            sb.AppendLine($"\tDROP TABLE [YourTableName];");
            sb.AppendLine($"GO");

            // Generate the CREATE TABLE statement
            sb.AppendLine("CREATE TABLE YourTableName (");
            for (var i = 0; i < Data.Columns.Count; i++)
            {
                var column = Data.Columns[i];
                sb.Append($"    [{column.ColumnName}] ");

                // Infer SQL data type from DataColumn.DataType
                sb.Append(column.DataType == typeof(int) ? "INT" :
                          column.DataType == typeof(decimal) ? "DECIMAL" :
                          column.DataType == typeof(DateTime) ? "DATETIME" :
                          "NVARCHAR(MAX)");

                if (i < Data.Columns.Count - 1)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    sb.AppendLine();
                }
            }
            sb.AppendLine(");");
            sb.AppendLine();

            // Generate the INSERT statements in batches of 20 rows
            const int batchSize = 20;

            for (int batchStart = 0; batchStart < Data.Rows.Count; batchStart += batchSize)
            {
                var batchEnd = Math.Min(batchStart + batchSize, Data.Rows.Count);

                sb.Append("INSERT INTO YourTableName (");
                for (var i = 0; i < Data.Columns.Count; i++)
                {
                    sb.Append($"[{Data.Columns[i].ColumnName}]");
                    if (i < Data.Columns.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.AppendLine(") VALUES");

                for (int rowIndex = batchStart; rowIndex < batchEnd; rowIndex++)
                {
                    var row = Data.Rows[rowIndex];
                    sb.Append('(');
                    for (var i = 0; i < Data.Columns.Count; i++)
                    {
                        // Escape single quotes in the value
                        var value = row[i].ToString().Replace("'", "''");
                        sb.Append($"'{value}'");
                        if (i < Data.Columns.Count - 1)
                        {
                            sb.Append(", ");
                        }
                    }
                    sb.Append(')');

                    if (rowIndex < batchEnd - 1)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        sb.AppendLine(";");
                    }
                }
            }
            sb.AppendLine($"GO");
            sb.AppendLine($"SELECT * FROM YourTableName");

            return sb.ToString();
        }
    }
}