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
                            if (elementValue == "1")
                                this.AuthenticationType = 1;
                            else
                                this.AuthenticationType = 0;
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

                        case "customconnection":
                            this.IsCustom = true;
                            break;

                        default:
                            break;
                    }
                }

                if (this.ConnectionString.Length == 0 && this.AuthenticationType == 0)
                {
                    BuildConnectionString();
                }
            }
        }

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
            bool integratedSecurity = (AuthenticationType == 0);
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
            {
                DataSource = ServerName,
                InitialCatalog = Database,
                IntegratedSecurity = integratedSecurity
            };
            if (!integratedSecurity)
            {
                builder.UserID = UserName;
                builder.Password = Password;
            }

            ConnectionString = builder.ConnectionString;
        }

        public override bool Equals(object obj)
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
            if(GUID!=null)
                return GUID.GetHashCode();
            else
                return 0;
        }

        public string Login()
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
                    bool integratedSecurity = (dlg.Authentication == 0);
                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
                    {
                        DataSource = dlg.ServerName,
                        InitialCatalog = dlg.DatabaseName,
                        IntegratedSecurity = integratedSecurity
                    };
                    if (!integratedSecurity)
                    {
                        builder.UserID = dlg.UserName;
                        builder.Password = dlg.Password;
                    }

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
                result = (ConnectionString.Length == 0);
            }

            return result;
        }

        public override string ToString()
        {
            return Name;
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("ConnectionItem");

            writer.WriteStartElement("DBMS");
            writer.WriteValue(DBMSType);
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

                if (AuthenticationType == 1)
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
                else
                {
                    writer.WriteStartElement("ConnectionString");
                    writer.WriteValue(BuildSecureConnectionString(ConnectionString));
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        private string BuildSecureConnectionString(string connectionString)
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

        private string EncryptPwd(string pwd)
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

        private string ParsePwd(string pwd)
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

        private string ParseSecureConnectionString(string connString)
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