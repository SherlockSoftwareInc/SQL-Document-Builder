using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The sql database connection item.
    /// </summary>
    public class SQLDatabaseConnectionItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLDatabaseConnectionItem"/> class.
        /// </summary>
        public SQLDatabaseConnectionItem()
        {
            DBMSType = 0;
            Name = "";
            ServerName = "";
            Database = "";
            UserName = "";
            Password = "";
            AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryIntegrated;
            ConnectionString = "";
            ConnectionType = "SQL Server";
            GUID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLDatabaseConnectionItem"/> class.
        /// </summary>
        /// <param name="xNode">The x node.</param>
        public SQLDatabaseConnectionItem(XmlNode xNode)
            : this()
        {
            if (xNode.HasChildNodes)
            {
                this.IsCustom = false;
                foreach (XmlNode node in xNode.ChildNodes)
                {
                    string elementName = node.Name.ToLower();
                    string elementValue = node.InnerText;
                    ConnectionType = "SQL Server";

                    switch (elementName)
                    {
                        case "connectiontype":
                            if (elementValue.Length > 0)
                                ConnectionType = elementValue;
                            break;

                        case "authentication":
                            switch (elementValue)
                            {
                                case "1":
                                    this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryDefault;
                                    break;

                                case "2":
                                    this.AuthenticationType = SqlAuthenticationMethod.SqlPassword;
                                    break;

                                case "3":
                                    this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                                    //this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryPassword;
                                    break;

                                case "4":
                                    this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                                    break;

                                case "5":
                                    this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryInteractive;
                                    break;

                                case "6":
                                    this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryServicePrincipal;
                                    break;

                                case "7":
                                    this.AuthenticationType = SqlAuthenticationMethod.ActiveDirectoryManagedIdentity;
                                    break;

                                default:
                                    if (SqlAuthenticationMethod.TryParse(elementValue, out SqlAuthenticationMethod value))
                                    {
                                        this.AuthenticationType = value;
                                    }
                                    break;
                            }

                            break;

                        case "connectionstring":
                            this.ConnectionString = ParseSecureConnectionString(elementValue);
                            break;

                        case "customconnection":
                            this.IsCustom = true;
                            break;

                        case "database":
                            this.Database = elementValue;
                            break;

                        case "encryptconnection":
                            EncryptConnection = elementValue.Equals("true", StringComparison.CurrentCultureIgnoreCase);
                            break;

                        case "name":
                            this.Name = elementValue;
                            break;

                        case "server":
                            this.ServerName = elementValue;
                            break;

                        case "TrustServerCertificate":
                            TrustServerCertificate = elementValue.Equals("true", StringComparison.CurrentCultureIgnoreCase);
                            break;

                        case "user":
                            this.UserName = elementValue;
                            break;

                        case "rememberpwd":
                            if (string.Compare(elementValue, "true", true) == 0)
                                this.RememberPassword = true;
                            else
                                this.RememberPassword = false;
                            break;

                        case "pwd":
                            this.Password = ParsePwd(elementValue);
                            break;

                        default:
                            break;
                    }
                }

                if (this.ConnectionString?.Length == 0 && this.AuthenticationType == 0)
                {
                    BuildConnectionString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the connection type.
        /// </summary>
        public string ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use encrypted connections
        /// </summary>
        public bool EncryptConnection { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to trust the server certificate
        /// </summary>
        public bool TrustServerCertificate { get; set; } = true;

        /// <summary>
        /// Gets or sets the authentication type.
        /// </summary>
        public SqlAuthenticationMethod AuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// DBMS type for future use.
        /// Currently 0 is used stand for MS SQL server
        /// </summary>
        public int DBMSType { get; set; } = 0;

        /// <summary>
        /// Gets the g u i d.
        /// </summary>
        public string? GUID { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is custom.
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember password.
        /// </summary>
        public bool RememberPassword { get; set; }

        /// <summary>
        /// Returns server and database name
        /// </summary>
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
        /// Gets or sets the user name.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        public void BuildConnectionString()
        {
            if (string.IsNullOrEmpty(ServerName) || string.IsNullOrEmpty(Database))
            {
                ConnectionString = string.Empty;
                return;
            }

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = Database,
                Encrypt = EncryptConnection,
                TrustServerCertificate = TrustServerCertificate,
                Authentication = AuthenticationType
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
            if (obj is not SQLDatabaseConnectionItem p || string.IsNullOrEmpty(GUID))
                return false;
            return GUID == p.GUID;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>An int.</returns>
        public override int GetHashCode() => GUID?.GetHashCode() ?? 0;

        /// <summary>
        /// Performs the login operation.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task<string?> Login()
        {
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
                    var builder = new SqlConnectionStringBuilder
                    {
                        DataSource = dlg.ServerName,
                        InitialCatalog = dlg.DatabaseName,
                        Encrypt = true,
                        TrustServerCertificate = true,
                        Authentication = dlg.Authentication
                    };

                    if (!string.IsNullOrEmpty(dlg.UserName))
                        builder.UserID = dlg.UserName;
                    if (!string.IsNullOrEmpty(dlg.Password))
                        builder.Password = dlg.Password;

                    connectionString = builder.ConnectionString;
                }
            }

            if (!await DatabaseHelper.TestConnectionAsync(connectionString))
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
        /// Build xml string
        /// </summary>
        /// <param name="writer"></param>
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("ConnectionItem");

            writer.WriteElementString("Name", Name ?? string.Empty);

            if (IsCustom)
            {
                writer.WriteElementString("CustomConnection", "True");
                if (!string.IsNullOrEmpty(ConnectionString))
                    writer.WriteElementString("ConnectionString", BuildSecureConnectionString(ConnectionString));
            }
            else
            {
                writer.WriteElementString("Server", ServerName ?? string.Empty);
                writer.WriteElementString("Database", Database ?? string.Empty);
                writer.WriteElementString("Authentication", AuthenticationType.ToString());

                if (AuthenticationType == SqlAuthenticationMethod.SqlPassword ||
                    AuthenticationType == SqlAuthenticationMethod.ActiveDirectoryPassword)
                {
                    writer.WriteElementString("User", UserName ?? string.Empty);
                    writer.WriteElementString("RememberPwd", RememberPassword.ToString());
                    if (RememberPassword)
                    {
                        writer.WriteElementString("Pwd", EncryptPwd(Password ?? string.Empty));
                        BuildConnectionString();
                    }
                }
                else if (AuthenticationType == SqlAuthenticationMethod.ActiveDirectoryIntegrated ||
                         AuthenticationType == SqlAuthenticationMethod.ActiveDirectoryInteractive ||
                         AuthenticationType == SqlAuthenticationMethod.ActiveDirectoryManagedIdentity ||
                         AuthenticationType == SqlAuthenticationMethod.ActiveDirectoryServicePrincipal)
                {
                    if (!string.IsNullOrEmpty(UserName))
                        writer.WriteElementString("User", UserName);
                }

                if (!string.IsNullOrEmpty(ConnectionString))
                    writer.WriteElementString("ConnectionString", BuildSecureConnectionString(ConnectionString));

                writer.WriteElementString("EncryptConnection", EncryptConnection.ToString());
                writer.WriteElementString("TrustServerCertificate", TrustServerCertificate.ToString());
            }

            writer.WriteEndElement();
        }

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
        private static string EncryptPwd(string pwd)
        {
            if (pwd != null)
            {
                if (pwd.Length > 1)
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