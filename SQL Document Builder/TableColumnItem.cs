using System;
using Microsoft.Data.SqlClient;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The table column item.
    /// </summary>
    internal class TableColumnItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnItem"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        public TableColumnItem(DBColumn column)
        {
            ColID = column.ColID;
            ColumnName = column.ColumnName;
            DataType = column.DataType;
            Nullable = column.Nullable;
            Description = column.Description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnItem"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public TableColumnItem(SqlDataReader dr)
        {
            this.ColID = ((int)dr["ORDINAL_POSITION"]).ToString();
            this.ColumnName = dr["COLUMN_NAME"].ToString();
            string? dtType = dr["DATA_TYPE"].ToString();
            this.DataType = dtType;
            string? strMaxLength = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
            if (strMaxLength?.Length > 0)
            {
                if (string.Compare(dtType, "text", true) == 0 ||
                    string.Compare(dtType, "ntext", true) == 0 ||
                    string.Compare(dtType, "image", true) == 0 ||
                    string.Compare(dtType, "xml", true) == 0)
                {
                    this.DataType = dtType;
                }
                else
                {
                    if (strMaxLength.IsNumeric())
                    {
                        int maxLength = Int32.Parse(strMaxLength);
                        if (maxLength > 0)
                        {
                            this.DataType = string.Format("{0}({1})", dtType, strMaxLength);
                        }
                    }
                    else
                    {
                        this.DataType = dtType;
                    }
                }
            }
            else
            {
                this.DataType = dtType;
            }

            this.Nullable = (dr["IS_NULLABLE"].ToString() == "YES");
        }

        /// <summary>
        /// Gets or sets the col i d.
        /// </summary>
        public string? ColID { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public string? DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether nullable.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string? Description { get; set; }
    }
}