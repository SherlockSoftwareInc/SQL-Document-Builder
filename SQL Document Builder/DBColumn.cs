using System;

namespace SQL_Document_Builder
{
    internal class DBColumn
    {
        public DBColumn(System.Data.SqlClient.SqlDataReader dr)
        {
            ColID = (int)dr["ORDINAL_POSITION"];
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

        public int ColID { get; set; }
        public bool Nullable { get; set; }
        public string ColumnName { get; set; } = string.Empty;
        public string ColumnType { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}