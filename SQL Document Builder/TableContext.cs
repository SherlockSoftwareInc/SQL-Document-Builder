using System.Collections.Generic;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents the context for a table, including database description, schema,
    /// table name, optional table description, and a list of column contexts.
    /// </summary>
    public class TableContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableContext"/> class.
        /// </summary>
        public TableContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableContext"/> class.
        /// </summary>
        /// <param name="dBColumns">The d b columns.</param>
        public TableContext(List<DBColumn> dBColumns)
        {
            Columns.Clear();
            foreach (var dbColumn in dBColumns)
            {
                Columns.Add(new ColumnContext()
                {
                    Ord = dbColumn.Ord ?? string.Empty,
                    ColumnName = dbColumn.ColumnName,
                    DataType = dbColumn.DataType,
                    Description = dbColumn.Description
                });
            }
        }

        /// <summary>
        /// The schema of the table.
        /// </summary>
        public string TableSchema { get; set; } = string.Empty;

        /// <summary>
        /// The name of the table.
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// The description of the table, if available.
        /// </summary>
        public string? TableDescription { get; set; }

        /// <summary>
        /// The list of columns in the table.
        /// </summary>
        public List<ColumnContext> Columns { get; set; } = new();
    }
}