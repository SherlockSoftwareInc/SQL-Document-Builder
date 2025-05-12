using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// A private class for connection menu item
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConnectionMenuItem"/> class.
    /// </remarks>
    /// <param name="connectionItem">The connection item.</param>
    internal class ConnectionMenuItem(SQLDatabaseConnectionItem connectionItem) : ToolStripMenuItem(connectionItem.Name)
    {

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SQLDatabaseConnectionItem Connection { get; set; } = connectionItem;

        /// <summary>
        /// Gets the connection name.
        /// </summary>
        public string? ConnectionName
        {
            get { return Connection.Name; }
        }

        /// <summary>
        /// Tos the string.
        /// </summary>
        /// <returns>A string? .</returns>
        public override string? ToString()
        {
            return Connection != null ? Connection.Name : string.Empty;
        }
    }
}