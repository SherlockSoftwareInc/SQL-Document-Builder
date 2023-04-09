using System;
using System.Data.SqlClient;

namespace SQL_Document_Builder
{
    internal class TableColumnItem
    {
        public TableColumnItem(DBColumn column)
        {
            ColID= column.ColID;
            ColumnName= column.ColumnName;
            DataType = column.DataType;
            Nullable = column.Nullable;
            Description = column.Description;
        }

        public TableColumnItem(SqlDataReader dr)
        {
            this.ColID = (int)dr["ORDINAL_POSITION"];
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

        public int ColID { get; set; }
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
        public bool Nullable { get; set; }
        public string? Description { get; set; }
    }
}