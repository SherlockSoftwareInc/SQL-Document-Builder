namespace SQL_Document_Builder
{
    /// <summary>
    /// The index item.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="IndexItem"/> class.
    /// </remarks>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="columns">The columns.</param>
    /// <param name="isUnique">If true, is unique.</param>
    internal class IndexItem(string name, string type, string columns, bool isUnique)
    {
        /// <summary>
        /// Gets or sets the columns included in the index.
        /// </summary>
        public string Columns { get; set; } = columns;

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique.
        /// </summary>
        public bool IsUnique { get; set; } = isUnique;

        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// Gets or sets the index type (e.g., CLUSTERED, NONCLUSTERED).
        /// </summary>
        public string Type { get; set; } = type;
    }
}