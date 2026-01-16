using System.Collections.Generic;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Contains detailed information about a table, including schema, name,
    /// description, and a list of column descriptions.
    /// </summary>
    public class TableInfo1
    {
        /// <summary>
        /// The schema of the table.
        /// </summary>
        public string Schema { get; set; } = string.Empty;

        /// <summary>
        /// The name of the table.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description of the table.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The list of column descriptions for the table.
        /// </summary>
        public List<ColumnDescription> Columns { get; set; } = new();
    }
}