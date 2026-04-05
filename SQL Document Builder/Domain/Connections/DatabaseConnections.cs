using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SQL_Document_Builder
{    /// <summary>
     /// All database connections saved in an application data file in xml format.
     /// The class manages load, save, add or remove connection items.
     /// </summary>
    public class DatabaseConnections
    {
        private readonly List<DatabaseConnectionItem> _connections = new();

        /// <summary>
        /// Returns connection items
        /// </summary>
        public List<DatabaseConnectionItem> Connections => _connections;

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
        public DatabaseConnectionItem? Add(string? name,
            string? server,
            string? database,
            AuthenticationMethod authenticationType,
            string? userName,
            string? password,
            string? connectionString,
            bool rememberPassword)
        {
            if (server != null && database != null)
            {
                var connItem = new DatabaseConnectionItem()
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

            if (File.Exists(fileName))
            {
                try
                {
                    // Load the JSON file into TemplateLists
                    string json = File.ReadAllText(fileName);
                    _connections.Clear();
                    var templatesFromJson = System.Text.Json.JsonSerializer.Deserialize<List<DatabaseConnectionItem>>(json);
                    if (templatesFromJson != null)
                    {
                        _connections.AddRange(templatesFromJson);
                    }

                    // rebuild connection strings for each item
                    foreach (var item in _connections)
                    {
                        item.BuildConnectionString();
                    }
                }
                catch (Exception ex)
                {
                    // show error message if the file cannot be loaded
                    MessageBox.Show($"Error loading connections from file: {ex.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            var tmpFile = Path.Combine(dataPath, "TmpConnections.json");
            Save(tmpFile);
        }

        /// <summary>
        /// Adds a connection item to the list
        /// </summary>
        /// <param name="connection">The connection.</param>
        internal void Add(DatabaseConnectionItem connection)
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

            return Path.Combine(dataPath, "Connections.json");
        }

        /// <summary>
        /// Save the settings to the application data file
        /// </summary>
        /// <param name="fileName"></param>
        private void Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(_connections, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                // show error message if the file cannot be saved
                MessageBox.Show($"Error saving connections to file: {ex.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}