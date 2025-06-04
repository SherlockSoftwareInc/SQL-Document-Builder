using System.Data;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The constraint item.
    /// </summary>
    internal class ConstraintItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintItem"/> class.
        /// </summary>
        /// <param name="name">The constraint name.</param>
        /// <param name="type">The constraint type.</param>
        /// <param name="columns">The columns involved in the constraint.</param>
        public ConstraintItem(string name, string type, string column)
        {
            Name = name;
            Type = type;
            Column = column;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintItem"/> class from a DataRow.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        public ConstraintItem(DataRow dataRow)
        {
            Name = dataRow["ConstraintName"]?.ToString() ?? string.Empty;
            Type = dataRow["ConstraintType"]?.ToString() ?? string.Empty;
            Column = dataRow["ColumnName"]?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the constraint name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the constraint type (e.g., PRIMARY KEY, FOREIGN KEY, CHECK).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the columns involved in the constraint.
        /// </summary>
        public string Column { get; set; } = string.Empty;

    }
}