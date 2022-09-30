using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System;
using System.Windows.Forms;
using System.Xml;

namespace SQL_Document_Builder
{
    public class SQLDatabaseConnectionItem
    {
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
            GUID = Guid.NewGuid().ToString();
        }

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
                    switch (elementName)
                    {
                        case "dbms":
                            this.DBMSType = int.Parse(elementValue);
                            break;

                        case "name":
                            this.Name = elementValue;
                            break;

                        case "server":
                            this.ServerName = elementValue;
                            break;

                        case "database":
                            this.Database = elementValue;
                            break;

                        case "user":
                            this.UserName = elementValue;
                            break;

                        case "connectionstring":
                            this.ConnectionString = ParseSecureConnectionString(elementValue);
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

                        case "rememberpwd":
                            this.RememberPassword = string.Compare(elementValue, "true", true) == 0;
                            break;

                        case "encryptconnection":
                            this.EncryptConnection = string.Compare(elementValue, "true", true) == 0;
                            break;

                        case "trustservercertificate":
                            this.TrustServerCertificate = string.Compare(elementValue, "true", true) == 0;
                            break;

                        case "pwd":
                            this.Password = ParsePwd(elementValue);
                            break;

                        case "customconnection":
                            this.IsCustom = true;
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
        /// Gets or sets a value indicating whether to use encrypted connections
        /// </summary>
        public bool EncryptConnection { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to trust the server certificate
        /// </summary>
        public bool TrustServerCertificate { get; set; } = true;

        public short AuthenticationType { get; set; }

        public string? ConnectionString { get; set; }

        public string? Database { get; set; }

        /// <summary>
        /// DBMS type for future use.
        /// Currently 0 is used stand for MS SQL server
        /// </summary>
        public int DBMSType { get; set; } = 0;

        public string? GUID { get; private set; }

        public bool IsCustom { get; set; }

        public string? Name { get; set; }

        public string? Password { get; set; }

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

        public string? ServerName { get; set; }

        public string? UserName { get; set; }

        public void BuildConnectionString()
        {
            //bool integratedSecurity = (AuthenticationType == 0);
            //var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
            //{
            //    DataSource = ServerName,
            //    InitialCatalog = Database,
            //    IntegratedSecurity = integratedSecurity
            //};
            //if (!integratedSecurity)
            //{
            //    builder.UserID = UserName;
            //    builder.Password = Password;
            //}
            //SqlConnectionStringBuilder builder = new()
            //{
            //    //Driver = "Sql Driver 17 for SQL Server"
            //};

            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
            {
                DataSource = ServerName,
                InitialCatalog = Database,
                //IntegratedSecurity = true,
                Encrypt = true,
                TrustServerCertificate = true,
            };

            //builder.Add("Server", "phsa-csbc-pcr-prod-sql-server.database.windows.net");
            //builder.Add("Database", "pcr_analytic");
            //builder.Add("Authentication", "ActiveDirectoryInteractive");
            //builder.Add("UID", UserName);
            //builder.Add("PWD", Password);
            //builder.Add("Server", ServerName);
            //builder.Add("Database", Database);
            switch (AuthenticationType)
            {
                case 0:     //Windows Authentication
                    builder.IntegratedSecurity = true;
                    //builder.Add("Trusted_Connection", "yes");
                    break;

                case 1:     //SQL Server Authentication
                    break;

                case 2:     //Active Directory - Interactive
                    builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryInteractive;
                    //builder.Add("Authentication", "ActiveDirectoryInteractive");
                    break;

                case 3:     //Active Directory - Integrated
                    builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                    //builder.Add("Authentication", "ActiveDirectoryIntegrated");
                    break;

                case 4:     //Active Directory - Password
                    builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
                    //builder.Add("Authentication", "ActiveDirectoryPassword");
                    break;

                case 5:     //Active Directory -Service Principal
                    builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryServicePrincipal;
                    //builder.Add("Authentication", "ActiveDirectoryServicePrincipal");
                    break;

                default:
                    break;
            }

            if (UserName?.Trim().Length > 0)
                builder.UserID = UserName;
            //builder.Add("UID", UserName);
            if (Password?.Trim().Length > 0)
                builder.Password = Password;
            //builder.Add("PWD", Password);

            ConnectionString = builder.ConnectionString;
        }

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

        public override int GetHashCode()
        {
            if (GUID != null)
                return GUID.GetHashCode();
            else
                return 0;
        }

        public string? Login()
        {
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
                    //bool integratedSecurity = (dlg.Authentication == 0);
                    //var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
                    //{
                    //    DataSource = dlg.ServerName,
                    //    InitialCatalog = dlg.DatabaseName,
                    //    IntegratedSecurity = integratedSecurity
                    //};
                    //if (!integratedSecurity)
                    //{
                    //    builder.UserID = dlg.UserName;
                    //    builder.Password = dlg.Password;
                    //}
                    //SqlConnectionStringBuilder builder = new()
                    //{
                    //    //Driver = "Sql Driver 17 for SQL Server"
                    //};
                    //builder.Add("Server", "phsa-csbc-pcr-prod-sql-server.database.windows.net");
                    //builder.Add("Database", "pcr_analytic");
                    //builder.Add("Authentication", "ActiveDirectoryInteractive");
                    //builder.Add("UID", dlg.UserName);
                    //builder.Add("PWD", dlg.Password);
                    //builder.Add("Server", dlg.ServerName);
                    //builder.Add("Database", dlg.DatabaseName);
                    //switch (dlg.Authentication)
                    //{
                    //    case 0:     //Windows Authentication
                    //        builder.Add("Trusted_Connection", "yes");
                    //        break;
                    //    case 1:     //SQL Server Authentication
                    //        break;
                    //    case 2:     //Active Directory - Interactive
                    //        builder.Add("Authentication", "ActiveDirectoryInteractive");
                    //        break;
                    //    case 3:     //Active Directory - Integrated
                    //        builder.Add("Authentication", "ActiveDirectoryIntegrated");
                    //        break;
                    //    case 4:     //Active Directory - Password
                    //        builder.Add("Authentication", "ActiveDirectoryPassword");
                    //        break;
                    //    case 5:     //Active Directory -Service Principal
                    //        builder.Add("Authentication", "ActiveDirectoryServicePrincipal");
                    //        break;
                    //    default:
                    //        break;
                    //}

                    //if (dlg.UserName?.Trim().Length > 0)
                    //    builder.Add("UID", dlg.UserName);
                    //if (dlg.Password?.Trim().Length > 0)
                    //    builder.Add("PWD", dlg.Password);

                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
                    {
                        DataSource = dlg.ServerName,
                        InitialCatalog = dlg.DatabaseName,
                        Encrypt = true,
                        TrustServerCertificate = true,
                    };

                    switch (AuthenticationType)
                    {
                        case 0:     //Windows Authentication
                            builder.IntegratedSecurity = true;
                            break;

                        case 1:     //SQL Server Authentication
                            break;

                        case 2:     //Active Directory - Interactive
                            builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryInteractive;
                            break;

                        case 3:     //Active Directory - Integrated
                            builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryIntegrated;
                            break;

                        case 4:     //Active Directory - Password
                            builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
                            break;

                        case 5:     //Active Directory -Service Principal
                            builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryServicePrincipal;
                            break;

                        default:
                            break;
                    }

                    if (UserName?.Trim().Length > 0)
                        builder.UserID = dlg.UserName;
                    if (Password?.Trim().Length > 0)
                        builder.Password = dlg.Password;

                    ConnectionString = builder.ConnectionString;
                }
            }
            return ConnectionString;
        }

        public bool RequireMannualLogin()
        {
            bool result;

            if (AuthenticationType == 1)
            {
                result = true;
            }
            else
            {
                result = (ConnectionString?.Length == 0);
            }

            return result;
        }

        public override string? ToString()
        {
            return Name;
        }

        //public void Write(XmlWriter writer)
        //{
        //    writer.WriteStartElement("ConnectionItem");

        //    writer.WriteStartElement("DBMS");
        //    writer.WriteValue(DBMSType);
        //    writer.WriteEndElement();

        //    writer.WriteStartElement("Name");
        //    writer.WriteValue(Name);
        //    writer.WriteEndElement();

        //    if (IsCustom)
        //    {
        //        if (ConnectionString != null)
        //        {
        //            writer.WriteStartElement("CustomConnection");
        //            writer.WriteValue("True");
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("ConnectionString");
        //            writer.WriteValue(BuildSecureConnectionString(ConnectionString));
        //            writer.WriteEndElement();
        //        }
        //    }
        //    else
        //    {
        //        writer.WriteStartElement("Server");
        //        writer.WriteValue(ServerName);
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("Database");
        //        writer.WriteValue(Database);
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("Authentication");
        //        writer.WriteValue(AuthenticationType.ToString());
        //        writer.WriteEndElement();

        //        if (AuthenticationType == 1)
        //        {
        //            writer.WriteStartElement("User");
        //            writer.WriteValue(UserName);
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("RememberPwd");
        //            writer.WriteValue(RememberPassword.ToString());
        //            writer.WriteEndElement();

        //            if (RememberPassword && Password != null && ConnectionString != null)
        //            {
        //                writer.WriteStartElement("Pwd");
        //                writer.WriteValue(EncryptPwd(Password));
        //                writer.WriteEndElement();

        //                BuildConnectionString();
        //                writer.WriteStartElement("ConnectionString");
        //                writer.WriteValue(BuildSecureConnectionString(ConnectionString));
        //                writer.WriteEndElement();
        //            }
        //        }
        //        else
        //        {
        //            if (ConnectionString != null)
        //            {
        //                writer.WriteStartElement("ConnectionString");
        //                writer.WriteValue(BuildSecureConnectionString(ConnectionString));
        //                writer.WriteEndElement();
        //            }
        //        }
        //    }

        //    writer.WriteEndElement();
        //}


        /// <summary>
        /// Build xml string
        /// </summary>
        /// <param name="writer"></param>
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("ConnectionItem");

            writer.WriteStartElement("DBMS");
            writer.WriteValue(DBMSType.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Name");
            writer.WriteValue(Name);
            writer.WriteEndElement();

            if (IsCustom)
            {
                writer.WriteStartElement("CustomConnection");
                writer.WriteValue("True");
                writer.WriteEndElement();

                writer.WriteStartElement("ConnectionString");
                writer.WriteValue(BuildSecureConnectionString(ConnectionString));
                writer.WriteEndElement();
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
                        writer.WriteStartElement("ConnectionString");
                        writer.WriteValue(BuildSecureConnectionString(ConnectionString));
                        writer.WriteEndElement();
                    }
                }
                else if (AuthenticationType == 4 || AuthenticationType == 5)
                {
                    writer.WriteStartElement("User");
                    writer.WriteValue(UserName);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("ConnectionString");
                writer.WriteValue(BuildSecureConnectionString(ConnectionString));
                writer.WriteEndElement();

                writer.WriteStartElement("EncryptConnection");
                writer.WriteValue(EncryptConnection.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("TrustServerCertificate");
                writer.WriteValue(TrustServerCertificate.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

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