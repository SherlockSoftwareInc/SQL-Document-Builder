using System;
using System.Data;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The d b column.
    /// </summary>
    public class DBColumn
    {
        public DBColumn()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBColumn"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public DBColumn(DataRow dr)
        {
            Ord = Convert.ToString(dr["ORDINAL_POSITION"]);
            ColumnName = (string)dr["COLUMN_NAME"];
            string dtType = dr["DATA_TYPE"] == DBNull.Value ? string.Empty : (string)dr["DATA_TYPE"];

            // Types that should not have any parameters
            if (string.Compare(dtType, "text", true) == 0 ||
                string.Compare(dtType, "ntext", true) == 0 ||
                string.Compare(dtType, "image", true) == 0 ||
                string.Compare(dtType, "xml", true) == 0 ||
                string.Compare(dtType, "int", true) == 0 ||
                string.Compare(dtType, "bigint", true) == 0 ||
                string.Compare(dtType, "smallint", true) == 0 ||
                string.Compare(dtType, "tinyint", true) == 0 ||
                string.Compare(dtType, "bit", true) == 0 ||
                string.Compare(dtType, "money", true) == 0 ||
                string.Compare(dtType, "smallmoney", true) == 0 ||
                string.Compare(dtType, "real", true) == 0 ||
                string.Compare(dtType, "date", true) == 0 ||
                string.Compare(dtType, "datetime", true) == 0 ||
                string.Compare(dtType, "smalldatetime", true) == 0 ||
                string.Compare(dtType, "timestamp", true) == 0 ||
                string.Compare(dtType, "uniqueidentifier", true) == 0 ||
                string.Compare(dtType, "geography", true) == 0 ||
                string.Compare(dtType, "geometry", true) == 0 ||
                string.Compare(dtType, "hierarchyid", true) == 0 ||
                string.Compare(dtType, "sql_variant", true) == 0)
            {
                DataType = dtType;
            }
            // Decimal and numeric types with precision and scale
            else if (string.Compare(dtType, "decimal", true) == 0 ||
                     string.Compare(dtType, "numeric", true) == 0)
            {
                string? precision = dr["NUMERIC_PRECISION"]?.ToString();
                string? scale = dr.Table.Columns.Contains("NUMERIC_SCALE") ? dr["NUMERIC_SCALE"]?.ToString() : null;
                if (!string.IsNullOrEmpty(precision))
                {
                    DataType = string.Format("{0}({1},{2})", dtType, precision, scale ?? "0");
                }
                else
                {
                    DataType = dtType;
                }
            }
            // Float type with optional precision (float without precision = float(53))
            else if (string.Compare(dtType, "float", true) == 0)
            {
                string? precision = dr["NUMERIC_PRECISION"]?.ToString();
                if (!string.IsNullOrEmpty(precision) && precision != "53")
                {
                    DataType = string.Format("{0}({1})", dtType, precision);
                }
                else
                {
                    DataType = dtType;
                }
            }
            // Time types with fractional seconds precision (datetime2, datetimeoffset, time)
            else if (string.Compare(dtType, "datetime2", true) == 0 ||
                     string.Compare(dtType, "datetimeoffset", true) == 0 ||
                     string.Compare(dtType, "time", true) == 0)
            {
                string? scale = dr.Table.Columns.Contains("DATETIME_PRECISION") ? dr["DATETIME_PRECISION"]?.ToString() : null;
                if (!string.IsNullOrEmpty(scale) && scale != "7")
                {
                    DataType = string.Format("{0}({1})", dtType, scale);
                }
                else
                {
                    DataType = dtType;
                }
            }
            // Character and binary types with length (char, varchar, nchar, nvarchar, binary, varbinary)
            else
            {
                string? strMaxLength = dr["CHARACTER_MAXIMUM_LENGTH"]?.ToString();
                // For binary types, use CHARACTER_OCTET_LENGTH if CHARACTER_MAXIMUM_LENGTH is empty
                if (string.IsNullOrEmpty(strMaxLength) &&
                    (string.Compare(dtType, "binary", true) == 0 ||
                     string.Compare(dtType, "varbinary", true) == 0))
                {
                    strMaxLength = dr.Table.Columns.Contains("CHARACTER_OCTET_LENGTH") ? dr["CHARACTER_OCTET_LENGTH"]?.ToString() : null;
                }

                if (!string.IsNullOrEmpty(strMaxLength) && strMaxLength.IsNumeric())
                {
                    int maxLength = Int32.Parse(strMaxLength);
                    if (maxLength > 0)
                    {
                        DataType = string.Format("{0}({1})", dtType, strMaxLength);
                    }
                    else if (maxLength == -1)
                    {
                        DataType = string.Format("{0}(MAX)", dtType);
                    }
                    else
                    {
                        DataType = dtType;
                    }
                }
                else
                {
                    DataType = dtType;
                }
            }

            Nullable = (dr["IS_NULLABLE"].ToString() == "YES");
        }

        /// <summary>
        /// Gets the Ordinal position without primary key and index flag.
        /// </summary>
        /// <returns>A string.</returns>
        internal string OrdinalPosition =>
            Ord.Replace("🔢", string.Empty).Replace("🗝", string.Empty);

        /// <summary>
        /// Gets or sets column sequence number
        /// </summary>
        public string Ord { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets column name
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets column data type
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicates whether column nullable or not
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Gets or set column description
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}