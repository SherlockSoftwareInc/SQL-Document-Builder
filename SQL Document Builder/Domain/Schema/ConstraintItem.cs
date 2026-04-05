using System.Data;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The constraint item.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConstraintItem"/> class from a DataRow.
    /// </remarks>
    /// <param name="dataRow">The data row.</param>
    internal class ConstraintItem(DataRow dataRow)
    {

        /// <summary>
        /// Gets or sets the constraint name.
        /// </summary>
        public string Name { get; set; } = dataRow["ConstraintName"]?.ToString() ?? string.Empty;

        /// <summary>
        /// Gets or sets the constraint type (e.g., PRIMARY KEY, FOREIGN KEY, CHECK).
        /// </summary>
        public string Type { get; set; } = dataRow["ConstraintType"]?.ToString() ?? string.Empty;

        /// <summary>
        /// Gets or sets the columns involved in the constraint.
        /// </summary>
        public string Column { get; set; } = dataRow["ColumnName"]?.ToString() ?? string.Empty;

    }
}