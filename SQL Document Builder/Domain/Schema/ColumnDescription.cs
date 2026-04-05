namespace SQL_Document_Builder
{
    /// <summary>
    /// Contains description details for a single column, including order, name,
    /// description, and optional reference information.
    /// </summary>
    public class ColumnDescription
    {
        /// <summary>
        /// The ordinal position of the column in the table.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description of the column.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Reference information for the column, if applicable.
        /// </summary>
        public string? Reference { get; set; }
    }
}