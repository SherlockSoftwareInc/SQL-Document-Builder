using System;
using System.Data;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The index item.
    /// </summary>
    internal class IndexItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="isUnique">If true, is unique.</param>
        public IndexItem(string name, string type, string columns, bool isUnique)
        {
            Name = name;
            Type = type;
            Columns = columns;
            IsUnique = isUnique;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexItem"/> class.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        public IndexItem(DataRow dataRow)
        {
            Name = dataRow["IndexName"]?.ToString() ?? string.Empty;
            Type = dataRow["Type"]?.ToString() ?? string.Empty;
            Columns = dataRow["Columns"]?.ToString() ?? string.Empty;
            IsUnique = dataRow.Table.Columns.Contains("IsUnique") && dataRow["IsUnique"] != DBNull.Value
                ? Convert.ToBoolean(dataRow["IsUnique"])
                : false;
        }

        /// <summary>
        /// Gets or sets the columns included in the index.
        /// </summary>
        public string Columns { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the index type (e.g., CLUSTERED, NONCLUSTERED).
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }
}