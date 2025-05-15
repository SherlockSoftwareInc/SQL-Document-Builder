using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SQL_Document_Builder
{    /// <summary>
     /// All database connections saved in an application data file in xml format.
     /// The class manages load, save, add or remove connection items.
     /// </summary>
    public class SQLServerConnections
    {
        private readonly List<SQLDatabaseConnectionItem> _connections = new();
        private string _tmpFile = "";   // a temporary file to save the changes during the editing

        /// <summary>
        /// Returns connection items
        /// </summary>
        public List<SQLDatabaseConnectionItem> Connections
        {
            get
            {
                return _connections;
            }
        }

        /// <summary>
        /// Add a new connection item
        /// </summary>
        /// <param name="name">connection item name</param>
        /// <param name="server">database server name</param>
        /// <param name="database">database name</param>
        /// <param name="authenticationType">authentication type</param>
        /// <param name="userName">user name</param>
        /// <param name="password">password</param>
        /// <param name="connectionString">connection string</param>
        /// <param name="rememberPassword">remember password indicator</param>
        public SQLDatabaseConnectionItem? Add(string? name,
            string? server,
            string? database,
            SqlAuthenticationMethod authenticationType,
            string? userName,
            string? password,
            string? connectionString,
            bool rememberPassword)
        {
            if (server != null && database != null)
            {
                var connItem = new SQLDatabaseConnectionItem()
                {
                    Name = name,
                    ServerName = server,
                    Database = database,
                    AuthenticationType = authenticationType,
                    UserName = userName,
                    Password = password,
                    ConnectionString = connectionString,
                    RememberPassword = rememberPassword
                };
                connItem.BuildConnectionString();
                _connections.Add(connItem);
                return connItem;
            }
            return null;
        }

        /// <summary>
        /// Load connection items from the application data file
        /// </summary>
        public void Load()
        {
            string fileName = FilePath();

            if (!File.Exists(fileName)) CopyOctofySetting(fileName);

            if (File.Exists(fileName))
            {
                using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using var streamReader = new StreamReader(fileStream);
                if (streamReader != null)
                {
                    string? strFirstLine = streamReader.ReadLine();
                    string? strXML = streamReader.ReadToEnd();
                    ParseXML(strFirstLine + "\r\n" + strXML);
                }
            }
            _tmpFile = Path.GetTempFileName();
        }

        /// <summary>
        /// Copies the connection settings from octofy.
        /// </summary>
        private static void CopyOctofySetting(string targetFile)
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "Octofy");
            if (Directory.Exists(dataPath))
            {
                var source = Path.Combine(dataPath, "Connections2.dat");
                // copy the file to the new location if it exists
                if (File.Exists(source))
                {
                    File.Copy(source, targetFile, true);
                }
            }
        }

        /// <summary>
        /// Remove a connection item at specified location
        /// </summary>
        /// <param name="index">index in the connection list to remove</param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _connections.Count)
                _connections.RemoveAt(index);
        }

        /// <summary>
        /// Save connection settings to file
        /// </summary>
        public void Save()
        {
            Save(FilePath());
        }

        /// <summary>
        /// Save to a temp file for editing
        /// </summary>
        public void SaveTemp()
        {
            Save(_tmpFile);
        }

        /// <summary>
        /// Adds a connection item to the list
        /// </summary>
        /// <param name="connection">The connection.</param>
        internal void Add(SQLDatabaseConnectionItem connection)
        {
            // add a connection item to the list
            if (connection != null && connection.Name != null && connection.Name.Length > 0)
            {
                _connections.Add(connection);
            }
        }

        /// <summary>
        /// Moves the down.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void MoveDown(SQLDatabaseConnectionItem item)
        {
            // move selected item down
            int index = _connections.IndexOf(item);
            if (index >= 0 && index < _connections.Count - 1)
            {
                _connections.RemoveAt(index);
                _connections.Insert(index + 1, item);
            }
        }

        /// <summary>
        /// Moves the up.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void MoveUp(SQLDatabaseConnectionItem item)
        {
            // move selected item up
            int index = _connections.IndexOf(item);
            if (index > 0)
            {
                _connections.RemoveAt(index);
                _connections.Insert(index - 1, item);
            }
        }

        /// <summary>
        /// Returns the local where to store the connections data
        /// </summary>
        /// <returns></returns>
        private static string FilePath()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            return Path.Combine(dataPath, "Connections.dat");
        }

        /// <summary>
        /// Parse connections from a xml string
        /// </summary>
        /// <param name="values">xml document body</param>
        private void ParseXML(string values)
        {
            try
            {
                _connections.Clear();

                var oDoc = new XmlDocument();
                if (oDoc != null)
                {
                    oDoc.LoadXml(values);
                    if (oDoc.DocumentElement.HasChildNodes)
                    {
                        foreach (XmlNode node in oDoc.DocumentElement.ChildNodes)
                        {
                            if (node is XmlElement)
                            {
                                string sNodeName = node.Name;
                                if (string.Compare(sNodeName, "ConnectionItem", true) == 0)
                                {
                                    var connectionItem = new SQLDatabaseConnectionItem(node);
                                    if (connectionItem?.Name?.Length > 0 && !connectionItem.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase))
                                        _connections.Add(connectionItem);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save the settings to the application data file
        /// </summary>
        /// <param name="fileName"></param>
        private void Save(string fileName)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, NewLineOnAttributes = false };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("root");

                foreach (var item in _connections)
                {
                    item.Write(writer);
                }

                writer.WriteEndElement();    //end sf:root
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            using var fileWriter = new StreamWriter(fileName);
            fileWriter.Write(sb.ToString());
            fileWriter.Flush();
            fileWriter.Close();
        }
    }
}