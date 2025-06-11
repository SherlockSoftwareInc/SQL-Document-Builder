namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The column template.
    /// </summary>
    public class DataTableTemplate
    {
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the column row.
        /// </summary>
        public string DataRow { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the head cell.
        /// </summary>
        public string HeaderCell { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cell.
        /// </summary>
        public string Cell { get; set; } = string.Empty;
    }
}