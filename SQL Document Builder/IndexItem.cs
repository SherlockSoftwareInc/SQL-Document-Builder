using System;
using System.Linq;

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
        /// Gets the quoted columns.
        /// </summary>
        public string QuotedColumns
        {
            get
            {
                // break the columns into column list
                var columnList = Columns.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // join the column list with square brackets and add quotes
                return string.Join(", ", columnList.Select(c => $"[{c.Trim()}]"));
            }
        }

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