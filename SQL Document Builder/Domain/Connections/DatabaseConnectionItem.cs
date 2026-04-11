using SQL_Document_Builder.DatabaseAccess;
using System;
using System.Data.Odbc;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The authentication method.
    ///     The authentication method 0 to 10 are borrowed from Microsoft.Data.SqlClient.SqlAuthenticationMethod enumeration.
    ///     Will reserve the values for other DBMS authentication methos.
    /// </summary>
    public enum AuthenticationMethod
    {
        NotSpecified = 0,
        SqlPassword,
        ActiveDirectoryPassword,
        ActiveDirectoryIntegrated,
        ActiveDirectoryInteractive,
        ActiveDirectoryServicePrincipal,
        ActiveDirectoryDeviceCodeFlow,
        ActiveDirectoryManagedIdentity,
        ActiveDirectoryMSI,
        ActiveDirectoryDefault,
        ActiveDirectoryWorkloadIdentity
    }

    /// <summary>
    /// The DBMS type enums.
    /// </summary>
    public enum DBMSTypeEnums
    {
        SQLServer = 0,
        MySQL = 1,
        PostgreSQL = 2,
        Oracle = 3,
        MongoDB = 4,
        SQLite = 5,
        Redis = 6,
        Cassandra = 7,
        MariaDB = 8,
        Other = 9
    }

    /// <summary>
    /// The sql database connection item.
    /// </summary>
    public class DatabaseConnectionItem
    {
        private string? _dsn;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectionItem"/> class.
        /// </summary>
        public DatabaseConnectionItem()
        {
            DBMSType = DBMSTypeEnums.SQLServer;
            Name = "";
            ServerName = "";
            Database = "";
            UserName = "";
            Password = "";
            AuthenticationType = AuthenticationMethod.ActiveDirectoryIntegrated;
            ConnectionString = "";
            ConnectionType = "SQL Server";
            DSN = "";
            ConnectionID = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets optional API key metadata.
        /// </summary>
        public string? APIKey { get; set; }

        /// <summary>
        /// Gets or sets the authentication type.
        /// </summary>
        public AuthenticationMethod AuthenticationType { get; set; }

        /// <summary>
        /// Gets the guid for a connection item.
        /// </summary>
        public Guid ConnectionID { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [JsonIgnore]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection type.
        /// </summary>
        public string ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// Gets or sets the description for the database connection.
        /// </summary>
        public string? DatabaseDescription { get; set; }

        /// <summary>
        /// DBMS type for future use.
        /// Currently 0 is used stand for MS SQL server
        /// </summary>
        public DBMSTypeEnums DBMSType { get; set; } = 0;

        /// <summary>
        /// Gets or sets ODBC driver display name.
        /// </summary>
        public string? Driver { get; set; }

        /// <summary>
        /// Gets or sets DSN name for ODBC connections.
        /// </summary>
        public string? DSN
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_dsn))
                {
                    return _dsn;
                }

                if (!string.IsNullOrWhiteSpace(ServerName) && ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase))
                {
                    return ServerName;
                }

                if (!string.IsNullOrWhiteSpace(Name) && ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase))
                {
                    return Name;
                }

                return string.Empty;
            }
            set => _dsn = value;
        }

        /// <summary>
        /// Gets or sets the encryped password.
        /// </summary>
        public string EncrypedPassword
        {
            get => EncryptPwd(Password);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Password = string.Empty;
                }
                else
                {
                    Password = ParsePwd(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use encrypted connections
        /// </summary>
        public bool EncryptConnection { get; set; } = true;

        /// <summary>
        /// Gets or sets optional endpoint metadata.
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// Gets or sets optional file picker filter for DBQ-based ODBC sources.
        /// </summary>
        public string? FileFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is custom.
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        /// Gets a value indicating whether the database is SQL Server.
        /// </summary>
        public bool IsSQLServer { get => DBMSType == DBMSTypeEnums.SQLServer; }

        /// <summary>
        /// Gets or sets a value indicating whether login has succeeded for current runtime session.
        /// </summary>
        [JsonIgnore]
        public bool LoginSucceed { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [JsonIgnore]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember password.
        /// </summary>
        public bool RememberPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether manual login is required.
        /// </summary>
        public bool RequireManualLogin { get; set; }

        /// <summary>
        /// Returns server and database name
        /// </summary>
        [JsonIgnore]
        public string? ServerDatabaseName
        {
            get
            {
                if (Database?.Length > 0)
                {
                    return string.Format("{0}.{1}", ServerName, Database);
                }
                else
                { return string.Empty; }
            }
        }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string? ServerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trust the server certificate
        /// </summary>
        public bool TrustServerCertificate { get; set; } = true;

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        public void BuildConnectionString()
        {
            if (ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(_dsn) && !string.IsNullOrWhiteSpace(ServerName))
                {
                    _dsn = ServerName;
                }

                if (string.IsNullOrWhiteSpace(_dsn) && !string.IsNullOrWhiteSpace(Name))
                {
                    _dsn = Name;
                }

                if (string.IsNullOrWhiteSpace(_dsn))
                {
                    ConnectionString = string.Empty;
                    return;
                }

                var odbcBuilder = new OdbcConnectionStringBuilder
                {
                    Dsn = _dsn
                };

                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    odbcBuilder["Uid"] = UserName;
                }

                if (!string.IsNullOrWhiteSpace(Password))
                {
                    odbcBuilder["Pwd"] = Password;
                }

                ServerName = _dsn;
                ConnectionString = odbcBuilder.ConnectionString;

                return;
            }

            if (string.IsNullOrEmpty(ServerName) || string.IsNullOrEmpty(Database))
            {
                ConnectionString = string.Empty;
                return;
            }

            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = Database,
                Encrypt = EncryptConnection,
                TrustServerCertificate = TrustServerCertificate,
                Authentication = (Microsoft.Data.SqlClient.SqlAuthenticationMethod)AuthenticationType
            };

            if (!string.IsNullOrEmpty(UserName)) builder.UserID = UserName;
            if (!string.IsNullOrEmpty(Password)) builder.Password = Password;

            ConnectionString = builder.ConnectionString;
        }

        /// <summary>
        /// Checks if the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A bool.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not DatabaseConnectionItem p)
                return false;
            return ConnectionID != null && ConnectionID.Equals(p.ConnectionID);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>An int.</returns>
        public override int GetHashCode()
        {
            return ConnectionID.ToString().GetHashCode();
        }

        /// <summary>
        /// Performs the login operation.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task<string?> Login()
        {
            if (ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase))
            {
                string dsn = DSN ?? string.Empty;
                if (string.IsNullOrWhiteSpace(dsn))
                {
                    return string.Empty;
                }

                string userName = UserName ?? string.Empty;
                string password = Password ?? string.Empty;

                if (RequireManualLogin)
                {
                    using var loginDialog = new ODBCLoginDialog
                    {
                        DSN = dsn,
                        UserName = userName,
                        Password = password
                    };

                    if (loginDialog.ShowDialog() != DialogResult.OK)
                    {
                        return string.Empty;
                    }

                    userName = loginDialog.UserName ?? string.Empty;
                    password = loginDialog.Password ?? string.Empty;
                }

                var odbcBuilder = new OdbcConnectionStringBuilder
                {
                    Dsn = dsn
                };

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    odbcBuilder["Uid"] = userName;
                }

                if (!string.IsNullOrWhiteSpace(password))
                {
                    odbcBuilder["Pwd"] = password;
                }

                string odbcConnectionString = odbcBuilder.ConnectionString;
                bool canConnect = await Task.Run(() => ODBCDataSource.TestConnection(odbcConnectionString));
                if (!canConnect)
                {
                    return string.Empty;
                }

                UserName = userName;
                Password = RememberPassword ? password : string.Empty;
                _dsn = dsn;
                ServerName = dsn;
                ConnectionString = odbcConnectionString;
                return ConnectionString;
            }

            string connectionString = string.Empty;

            using (var dlg = new SQLServerLoginDialog
            {
                ServerName = ServerName,
                DatabaseName = Database,
                UserName = UserName,
                Authentication = AuthenticationType
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
                    {
                        DataSource = dlg.ServerName,
                        InitialCatalog = dlg.DatabaseName,
                        Encrypt = true,
                        TrustServerCertificate = true,
                        Authentication = (Microsoft.Data.SqlClient.SqlAuthenticationMethod)dlg.Authentication
                    };

                    if (!string.IsNullOrEmpty(dlg.UserName))
                        builder.UserID = dlg.UserName;
                    if (!string.IsNullOrEmpty(dlg.Password))
                        builder.Password = dlg.Password;

                    connectionString = builder.ConnectionString;
                }
            }

            var provider = DatabaseAccessProviderFactory.GetProvider(this);
            if (!await provider.TestConnectionAsync(connectionString))
            {
                return string.Empty;
            }
            ConnectionString = connectionString;
            return ConnectionString;
        }

        /// <summary>
        /// Overrides the ToString method to return the name of the connection item.
        /// </summary>
        /// <returns>A string? .</returns>
        public override string? ToString() => Name ?? string.Empty;

        /// <summary>
        /// Builds the secure connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A string.</returns>
        private static string BuildSecureConnectionString(string connectionString)
        {
            if (connectionString.Length > 0)
                try
                {
                    var wrapper = new Encryption("ATiBRZWdFeDYMvqqJaxrZ1KG9eq4mgkDc5kKVWuYdXjk0YxPwvYnc17aajVGDJGB");
                    string cipherText = wrapper.EncryptData(connectionString);
                    return cipherText;
                }
                catch (FormatException)
                {
                    //do nothing here, just ignore
                }
            return "";
        }

        /// <summary>
        /// Encrypts the password.
        /// </summary>
        /// <param name="pwd">The pwd.</param>
        /// <returns>A string.</returns>
        private static string EncryptPwd(string? pwd)
        {
            if (!string.IsNullOrEmpty(pwd))
            {
                try
                {
                    var wrapper = new Encryption("qECqeNZ3pBAVUuT3LCZCvHbx9BPTey7rFuNNF6JBafBSVRMMX2XTHVfwZjMk4qSj");
                    string cipherText = wrapper.EncryptData(pwd);
                    return cipherText;
                }
                catch (FormatException)
                {
                    //do nothing here, just ignore
                }
            }
            return "";
        }

        /// <summary>
        /// Parses the password.
        /// </summary>
        /// <param name="pwd">The pwd.</param>
        /// <returns>A string.</returns>
        private static string ParsePwd(string pwd)
        {
            if (pwd.Length > 1)
            {
                try
                {
                    var wrapper = new Encryption("qECqeNZ3pBAVUuT3LCZCvHbx9BPTey7rFuNNF6JBafBSVRMMX2XTHVfwZjMk4qSj");
                    return wrapper.DecryptData(pwd);
                }
                catch (FormatException)
                {
                    //do nothing here, just ignore
                }
            }
            return "";
        }

        /// <summary>
        /// Parses the secure connection string.
        /// </summary>
        /// <param name="connString">The conn string.</param>
        /// <returns>A string.</returns>
        private static string ParseSecureConnectionString(string connString)
        {
            if (connString.Length > 0)
                try
                {
                    var wrapper = new Encryption("ATiBRZWdFeDYMvqqJaxrZ1KG9eq4mgkDc5kKVWuYdXjk0YxPwvYnc17aajVGDJGB");
                    return wrapper.DecryptData(connString);
                }
                catch (FormatException)
                {
                    //do nothing here, just ignore
                }
            return "";
        }
    }
}