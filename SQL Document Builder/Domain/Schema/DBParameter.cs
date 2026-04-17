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
            Name = Convert.ToString(dr["PARAMETER_NAME"]) ?? string.Empty;
            DataType = Convert.ToString(dr["DATA_TYPE"]) ?? string.Empty;
            Mode = Convert.ToString(dr["PARAMETER_MODE"]) ?? string.Empty;
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