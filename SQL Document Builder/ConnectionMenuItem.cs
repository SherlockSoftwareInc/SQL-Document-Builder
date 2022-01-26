using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// A private class for connection menu item
    /// </summary>
    internal class ConnectionMenuItem : ToolStripMenuItem
    {
        public ConnectionMenuItem(SQLDatabaseConnectionItem connectionItem)
            : base(connectionItem.Name)
        {
            Connection = connectionItem;
        }

        public SQLDatabaseConnectionItem Connection { get; set; }

        public string ConnectionName
        {
            get { return Connection.Name; }
        }

        public override string ToString()
        {
            return Connection != null ? Connection.Name : string.Empty;
        }
    }
}