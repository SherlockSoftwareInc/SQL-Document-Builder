using System;
using System.Data;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The d b parameter.
    /// </summary>
    internal class DBParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DBParameter"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public DBParameter(DataRow dr)
        {
            if (dr == null)
            {
                throw new ArgumentNullException(nameof(dr), "DataRow cannot be null");
            }
            Ord = dr["ORDINAL_POSITION"] == DBNull.Value ? string.Empty : Convert.ToString(dr["ORDINAL_POSITION"]);
            Name = (string)dr["PARAMETER_NAME"];
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
            }
            else
            {
                DataType = dtType;
            }
            Mode = dr["PARAMETER_MODE"] == DBNull.Value ? string.Empty : (string)dr["PARAMETER_MODE"];
        }

        /// <summary>
        /// Gets or sets column sequence number
        /// </summary>
        public string Ord { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets column name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets column data type
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        public string Mode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or set column description
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}