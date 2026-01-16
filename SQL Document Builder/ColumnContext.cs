namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents the context for a single column, including its name, data type,
    /// optional description, and reference information.
    /// </summary>
    public class ColumnContext
    {
        /// <summary>
        /// Gets or sets column sequence number
        /// </summary>
        public string Ord { get; set; } = string.Empty;

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// The data type of the column.
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// The description of the column, if available.
        /// </summary>
        public string? Description { get; set; }

    }
}