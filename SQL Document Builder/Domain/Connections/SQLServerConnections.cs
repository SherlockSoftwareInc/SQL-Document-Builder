using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SQL_Document_Builder
{
    /// <summary>
    /// All database connections saved in an application data file in xml format.
    /// The class manages load, save, add or remove connection items.
    /// </summary>
    public class SQLServerConnections
    {
        private readonly List<DatabaseConnectionItem> _connections = new List<DatabaseConnectionItem>();
        private string _tmpFile = "";   // a temporary file to save the changes during the editing

        /// <summary>
        /// Gets all available ODBC DSNs
        /// </summary>
        public List<DatabaseConnectionItem> AvailableDSNs { get; } = new List<DatabaseConnectionItem>();

        /// <summary>
        /// Returns connection items
        /// </summary>
        public List<DatabaseConnectionItem> Connections
        {
            get
            {
                return _connections;
            }
        }

        /// <summary>
        /// Add a new ODBC connection item
        /// </summary>
        /// <param name="dsn"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="rememberPassword"></param>
        /// <param name="requireManualLogin"></param>
        /// <returns></returns>
        public DatabaseConnectionItem AddOdbcConnection(DatabaseConnectionItem connection, string dsn, string userName, string password, bool rememberPassword, bool requireManualLogin, string endpoint, string apikey)
        {
            if (!IsOdbcConnectionItemExists(dsn))
            {
                var connItem = new DatabaseConnectionItem()
                {
                    ConnectionType = "ODBC",
                    Name = dsn,
                    UserName = userName,
                    Password = password,
                    RememberPassword = rememberPassword,
                    RequireManualLogin = requireManualLogin,
                    Endpoint = endpoint,
                    APIKey = apikey,
                    DBMSType = connection?.DBMSType ?? DBMSTypeEnums.Other
                };
                connItem.BuildConnectionString();
                _connections.Add(connItem);
                return connItem;
            }
            return null;
        }

        /// <summary>
        /// Checks whether an ODBC connection item exists or not
        /// </summary>
        /// <param name="dsn"></param>
        /// <returns></returns>
        private bool IsOdbcConnectionItemExists(string dsn)
        {
            for (int i = 0; i < _connections.Count; i++)
            {
                if (string.Equals(_connections[i].ConnectionType, "ODBC", StringComparison.CurrentCultureIgnoreCase) &&
                    string.Equals(_connections[i].Name, dsn, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
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
        /// <param name="rememberPassword">remember password indicator</param>
        public DatabaseConnectionItem AddSqlConnection(string name,
            string server,
            string database,
            short authenticationType,
            string userName,
            string password,
            bool rememberPassword,
            bool encryptConnection,
            bool trustServerCertificate,
            string endpoint,
            string apiKey)
        {
            if (!IsSQLConnectionItemExists(server, database))
            {
                var connItem = new DatabaseConnectionItem()
                {
                    ConnectionType = "SQL Server",
                    Name = name,
                    ServerName = server,
                    Database = database,
                    AuthenticationType = (AuthenticationMethod)authenticationType,
                    UserName = userName,
                    Password = password,
                    RememberPassword = rememberPassword,
                    EncryptConnection = encryptConnection,
                    TrustServerCertificate = trustServerCertificate,
                    Endpoint = endpoint,
                    APIKey = apiKey
                };
                connItem.BuildConnectionString();
                _connections.Add(connItem);
                return connItem;
            }
            return null;
        }

        /// <summary>
        /// Checks whether the connection item has already exists
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        private bool IsSQLConnectionItemExists(string server, string database)
        {
            for (int i = 0; i < _connections.Count; i++)
            {
                if (string.Equals(_connections[i].ConnectionType, "SQL Server", StringComparison.CurrentCultureIgnoreCase) &&
                    _connections[i].ServerName.Equals(server, StringComparison.CurrentCultureIgnoreCase) &&
                    _connections[i].Database.Equals(database, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new ODBC connection item
        /// </summary>
        /// <param name="dsnItem"></param>
        public void Add(DatabaseConnectionItem dsnItem)
        {
            var connItem = new DatabaseConnectionItem
            {
                ConnectionID = dsnItem.ConnectionID,
                DBMSType = dsnItem.DBMSType,
                ConnectionType = dsnItem.ConnectionType,
                Name = dsnItem.Name,
                ServerName = dsnItem.ServerName,
                Database = dsnItem.Database,
                AuthenticationType = dsnItem.AuthenticationType,
                UserName = dsnItem.UserName,
                Password = dsnItem.Password,
                RememberPassword = dsnItem.RememberPassword,
                EncryptConnection = dsnItem.EncryptConnection,
                TrustServerCertificate = dsnItem.TrustServerCertificate,
                IsCustom = dsnItem.IsCustom,
                DatabaseDescription = dsnItem.DatabaseDescription,
                RequireManualLogin = dsnItem.RequireManualLogin,
                Endpoint = dsnItem.Endpoint,
                APIKey = dsnItem.APIKey
            };
            connItem.BuildConnectionString();
            Connections.Add(connItem);
        }

        /// <summary>
        /// Get available ODBC DSNs
        /// </summary>
        public void GetAllDSNs()
        {
            AvailableDSNs.Clear();
            var dsn = new ODBCDNS();

            foreach (var item in dsn.SystemDsns)
            {
                AvailableDSNs.Add(new DatabaseConnectionItem
                {
                    ConnectionType = "ODBC",
                    Name = item,
                    DBMSType = DBMSTypeEnums.Other
                });
            }
            foreach (var item in dsn.UserDsns)
            {
                AvailableDSNs.Add(new DatabaseConnectionItem
                {
                    ConnectionType = "ODBC",
                    Name = item,
                    DBMSType = DBMSTypeEnums.Other
                });
            }
        }

        /// <summary>
        /// Load connection items from the application data file
        /// </summary>
        public void Load()
        {
            GetAllDSNs();

            string fileName = FilePath();

            if (!File.Exists(fileName))
            {
                // copy previous connections setting file
                var previousVersion = fileName.Replace("Connections2.dat", "Connections.dat");
                if (File.Exists(previousVersion))
                {
                    File.Copy(previousVersion, fileName, true);
                }
            }

            if (File.Exists(fileName))
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        string strFirstLine = streamReader.ReadLine();
                        string strXML = streamReader.ReadToEnd();
                        ParseXML(strFirstLine + "\r\n" + strXML);
                    }
                }
            }
            _tmpFile = Path.GetTempFileName();
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
        /// Returns the local where to store the connections data
        /// </summary>
        /// <returns></returns>
        private string FilePath()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "Octofy");
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            return Path.Combine(dataPath, "Connections2.dat");
        }

        /// <summary>
        /// Checks if the specified dsn still available on the computer
        /// </summary>
        /// <param name="dsn"></param>
        /// <returns></returns>
        private DatabaseConnectionItem IsDsnAvailable(string dsn)
        {
            return AvailableDSNs.Find(i =>
                string.Equals(i.Name, dsn, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(i.ConnectionType, "ODBC", StringComparison.OrdinalIgnoreCase));
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
                oDoc.LoadXml(values);

                if (oDoc != null)
                {
                    if (oDoc.DocumentElement.HasChildNodes)
                    {
                        foreach (XmlNode node in oDoc.DocumentElement.ChildNodes)
                        {
                            if (node is XmlElement)
                            {
                                string sNodeName = node.Name;
                                if (string.Compare(sNodeName, "ConnectionItem", true) == 0)
                                {
                                    var connectionItem = ParseConnectionItem(node);
                                    if (!string.IsNullOrEmpty(connectionItem.Name))
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
                    WriteConnectionItem(writer, item);
                }

                writer.WriteEndElement();    //end sf:root
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            using (var fileWriter = new StreamWriter(fileName))
            {
                fileWriter.Write(sb.ToString());
                fileWriter.Flush();
                fileWriter.Close();
            }
        }

        private static DatabaseConnectionItem ParseConnectionItem(XmlNode node)
        {
            var item = new DatabaseConnectionItem();

            foreach (XmlNode child in node.ChildNodes)
            {
                string value = child.InnerText ?? string.Empty;
                switch (child.Name)
                {
                    case "Name":
                        item.Name = value;
                        break;
                    case "ServerName":
                        item.ServerName = value;
                        break;
                    case "Database":
                        item.Database = value;
                        break;
                    case "ConnectionType":
                        item.ConnectionType = value;
                        break;
                    case "AuthenticationType":
                        if (short.TryParse(value, out short authValue))
                        {
                            item.AuthenticationType = (AuthenticationMethod)authValue;
                        }
                        break;
                    case "UserName":
                        item.UserName = value;
                        break;
                    case "EncrypedPassword":
                        item.EncrypedPassword = value;
                        break;
                    case "RememberPassword":
                        item.RememberPassword = bool.TryParse(value, out bool remember) && remember;
                        break;
                    case "EncryptConnection":
                        item.EncryptConnection = bool.TryParse(value, out bool encrypt) && encrypt;
                        break;
                    case "TrustServerCertificate":
                        item.TrustServerCertificate = bool.TryParse(value, out bool trust) && trust;
                        break;
                    case "DatabaseDescription":
                        item.DatabaseDescription = value;
                        break;
                    case "RequireManualLogin":
                        item.RequireManualLogin = bool.TryParse(value, out bool manualLogin) && manualLogin;
                        break;
                    case "Endpoint":
                        item.Endpoint = value;
                        break;
                    case "APIKey":
                        item.APIKey = value;
                        break;
                }
            }

            item.BuildConnectionString();
            return item;
        }

        private static void WriteConnectionItem(XmlWriter writer, DatabaseConnectionItem item)
        {
            writer.WriteStartElement("ConnectionItem");
            writer.WriteElementString("Name", item.Name ?? string.Empty);
            writer.WriteElementString("ServerName", item.ServerName ?? string.Empty);
            writer.WriteElementString("Database", item.Database ?? string.Empty);
            writer.WriteElementString("ConnectionType", item.ConnectionType ?? string.Empty);
            writer.WriteElementString("AuthenticationType", ((short)item.AuthenticationType).ToString());
            writer.WriteElementString("UserName", item.UserName ?? string.Empty);
            writer.WriteElementString("EncrypedPassword", item.EncrypedPassword ?? string.Empty);
            writer.WriteElementString("RememberPassword", item.RememberPassword.ToString());
            writer.WriteElementString("EncryptConnection", item.EncryptConnection.ToString());
            writer.WriteElementString("TrustServerCertificate", item.TrustServerCertificate.ToString());
            writer.WriteElementString("DatabaseDescription", item.DatabaseDescription ?? string.Empty);
            writer.WriteElementString("RequireManualLogin", item.RequireManualLogin.ToString());
            writer.WriteElementString("Endpoint", item.Endpoint ?? string.Empty);
            writer.WriteElementString("APIKey", item.APIKey ?? string.Empty);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Moves the up.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void MoveUp(DatabaseConnectionItem item)
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
        /// Moves the down.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void MoveDown(DatabaseConnectionItem item)
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
        /// Gets the connection item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A DatabaseConnectionItem.</returns>
        internal DatabaseConnectionItem GetConnectionItem(string name)
        {
            // find the connection item by name
            foreach (var item in _connections)
            {
                if (item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return item;
            }
            return null;
        }
    }
}