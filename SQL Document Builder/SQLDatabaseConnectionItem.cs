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
            AuthenticationType = 1;
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
                            if (short.TryParse(elementValue, out short value))
                            {
                                if (value >= 0 && value < 7)
                                    this.AuthenticationType = value;
                                else
                                    this.AuthenticationType = 0;
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
        public short AuthenticationType { get; set; }

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
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
            {
                DataSource = ServerName,
                InitialCatalog = Database,
                Encrypt = EncryptConnection,
                TrustServerCertificate = TrustServerCertificate
            };

            switch (AuthenticationType)
            {
                case 1: //SQL Server authentication
                    builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.SqlPassword;
                    break;

                case 2: //Active Directory Password Authentication
                    builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryPassword;
                    break;

                case 3: //Active Directory Integrated Authentication
                    builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                    break;

                case 4: //Active Directory Interactive Authentication
                    builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryInteractive;
                    break;

                case 5: //Service Principal Authentication
                    builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryServicePrincipal;
                    break;

                case 6: //Managed Service Identity Authentication
                    builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow;
                    break;

                default:    //Windows Authentication
                    builder.IntegratedSecurity = true;
                    builder.TrustServerCertificate = true;
                    break;
            }

            if (UserName?.Length > 0) builder.UserID = UserName;
            if (Password?.Length > 0) builder.Password = Password;

            ConnectionString = builder.ConnectionString;
        }

        /// <summary>
        /// Checks if the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A bool.</returns>
        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                SQLDatabaseConnectionItem p = (SQLDatabaseConnectionItem)obj;
                return (GUID == p.GUID);
            }
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>An int.</returns>
        public override int GetHashCode()
        {
            if (GUID != null)
                return GUID.GetHashCode();
            else
                return 0;
        }

        /// <summary>
        /// Performs the login operation.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task<string?> Login()
        {
            string connectionString = string.Empty;

            using (var dlg = new SQLServerLoginDialog()
            {
                ServerName = ServerName,
                DatabaseName = Database,
                UserName = UserName,
                Authentication = AuthenticationType
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
                    {
                        DataSource = dlg.ServerName,
                        InitialCatalog = dlg.DatabaseName,
                        Encrypt = true,
                        TrustServerCertificate = true,
                    };

                    switch (dlg.Authentication)
                    {
                        case 1: //SQL Server authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.SqlPassword;
                            break;

                        case 2: //Active Directory Password Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryPassword;
                            break;

                        case 3: //Active Directory Integrated Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                            break;

                        case 4: //Active Directory Interactive Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryInteractive;
                            break;

                        case 5: //Service Principal Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryServicePrincipal;
                            break;

                        case 6: //Managed Service Identity Authentication
                            builder.Authentication = Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow;
                            break;

                        default:    //Windows Authentication
                            builder.IntegratedSecurity = true;
                            builder.TrustServerCertificate = true;
                            break;
                    }

                    if (UserName?.Trim().Length > 0)
                        builder.UserID = dlg.UserName;
                    if (Password?.Trim().Length > 0)
                        builder.Password = dlg.Password;

                    connectionString = builder.ConnectionString;
                }
            }

            bool testResult = await DatabaseHelper.TestConnection(connectionString);
            if (!testResult)
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
        public override string? ToString()
        {
            return Name;
        }

        /// <summary>
        /// Build xml string
        /// </summary>
        /// <param name="writer"></param>
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("ConnectionItem");

            writer.WriteStartElement("Name");
            writer.WriteValue(Name);
            writer.WriteEndElement();

            if (IsCustom)
            {
                writer.WriteStartElement("CustomConnection");
                writer.WriteValue("True");
                writer.WriteEndElement();

                if (ConnectionString?.Length > 0)
                {
                    writer.WriteStartElement("ConnectionString");
                    writer.WriteValue(BuildSecureConnectionString(ConnectionString));
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteStartElement("Server");
                writer.WriteValue(ServerName);
                writer.WriteEndElement();

                writer.WriteStartElement("Database");
                writer.WriteValue(Database);
                writer.WriteEndElement();

                writer.WriteStartElement("Authentication");
                writer.WriteValue(AuthenticationType.ToString());
                writer.WriteEndElement();

                if (AuthenticationType == 1 || AuthenticationType == 2 || AuthenticationType == 6)
                {
                    writer.WriteStartElement("User");
                    writer.WriteValue(UserName);
                    writer.WriteEndElement();

                    writer.WriteStartElement("RememberPwd");
                    writer.WriteValue(RememberPassword.ToString());
                    writer.WriteEndElement();

                    if (RememberPassword)
                    {
                        writer.WriteStartElement("Pwd");
                        writer.WriteValue(EncryptPwd(Password));
                        writer.WriteEndElement();

                        BuildConnectionString();
                    }
                }
                else if (AuthenticationType == 4 || AuthenticationType == 5)
                {
                    writer.WriteStartElement("User");
                    writer.WriteValue(UserName);
                    writer.WriteEndElement();
                }

                if (ConnectionString?.Length > 0)
                {
                    writer.WriteStartElement("ConnectionString");
                    writer.WriteValue(BuildSecureConnectionString(ConnectionString));
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("EncryptConnection");
                writer.WriteValue(EncryptConnection.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("TrustServerCertificate");
                writer.WriteValue(TrustServerCertificate.ToString());
                writer.WriteEndElement();
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