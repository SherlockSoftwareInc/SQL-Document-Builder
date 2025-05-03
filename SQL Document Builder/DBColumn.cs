using System;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The d b column.
    /// </summary>
    internal class DBColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DBColumn"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public DBColumn(Microsoft.Data.SqlClient.SqlDataReader dr)
        {
            ColID = Convert.ToString(dr["ORDINAL_POSITION"]);
            ColumnName = (string)dr["COLUMN_NAME"];
            string dtType = dr["DATA_TYPE"] == DBNull.Value ? string.Empty : (string)dr["DATA_TYPE"];
            string? strMaxLength = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
            if (strMaxLength?.Length > 0)
            {
                if (string.Compare(dtType, "text", true) == 0 ||
                    string.Compare(dtType, "ntext", true) == 0 ||
                    string.Compare(dtType, "image", true) == 0 ||
                    string.Compare(dtType, "xml", true) == 0)
                {
                    DataType = dtType;
                }
                else
                {
                    if (strMaxLength.IsNumeric())
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
                    }
                    else
                    {
                        DataType = dtType;
                    }
                }
            }
            else
            {
                DataType = dtType;
            }

            Nullable = (dr["IS_NULLABLE"].ToString() == "YES");
        }

        /// <summary>
        /// Gets or sets column sequence number
        /// </summary>
        public string? ColID { get; set; } = string.Empty;

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