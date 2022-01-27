namespace SQL_Document_Builder
{
    public class DBTableName
    {
        private string _alias;

        public DBTableName()
        {
            Catelog = string.Empty;
            Schema = string.Empty;
            Name = string.Empty;
            Alias = string.Empty;
        }

        public DBTableName(string fullName)
        {
            FullName = fullName;
        }

        public DBTableName(string schema, string tableName)
        {
            Catelog = string.Empty;
            Schema = schema;
            Name = tableName;
            Alias = string.Empty;
        }

        public DBTableName(string catelog, string schema, string tableName)
        {
            Catelog = catelog;
            Schema = schema;
            Name = tableName;
            Alias = string.Empty;
        }

        /// <summary>
        /// Gets or sets object alias
        /// </summary>
        public string Alias
        {
            get
            {
                if (_alias?.Length > 0)
                {
                    return _alias;
                }
                else
                {
                    return Name;
                }
            }
            set
            {
                if (value.IndexOf(".") > 0)
                {
                    string[] tableElements = value.Split('.');
                    _alias = tableElements[tableElements.Length - 1];
                }
                else
                {
                    _alias = value;
                }
            }
        }

        /// <summary>
        /// Gets or set object ctelog
        /// </summary>
        public string Catelog { get; set; } = "";

        /// <summary>
        /// Gets or sets object full name
        /// </summary>
        public string FullName
        {
            get
            {
                if (Catelog?.Length > 0)
                {
                    return string.Format("{0}.{1}.{2}", QuotedName(Catelog), QuotedName(Schema), QuotedName(Name));
                }
                else if (Schema?.Length > 0)
                {
                    return string.Format("{0}.{1}", QuotedName(Schema), QuotedName(Name));
                }
                else
                {
                    return QuotedName(Name);
                }
            }
            set
            {
                Catelog = "";
                Schema = "";
                Name = "";
                //Parse table name
                string[] tableElements = value.Split('.');
                if (tableElements.Length == 1)
                {
                    Name = value.RemoveQuote();
                }
                else if (tableElements.Length == 2)
                {
                    Schema = tableElements[0].RemoveQuote();
                    Name = tableElements[1].RemoveQuote();
                }
                else if (tableElements.Length == 3)
                {
                    Catelog = tableElements[0].RemoveQuote();
                    Schema = tableElements[1].RemoveQuote();
                    Name = tableElements[2].RemoveQuote();
                }
            }
        }

        /// <summary>
        /// Get table full name without quotation
        /// </summary>
        public string FullNameNoQuote
        {
            get
            {
                if (Catelog?.Length > 0)
                {
                    return string.Format("{0}.{1}.{2}", Catelog, Schema, Name);
                }
                else if (Schema?.Length > 0)
                {
                    return string.Format("{0}.{1}", Schema, Name);
                }
                else
                {
                    return Name;
                }
            }
        }

        /// <summary>
        /// Gets or sets object name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets object schame
        /// </summary>
        public string Schema { get; set; } = "";

        //public string TableNameA { get { return string.Format("{0}.{1}", QuotedName(Alias), QuotedName(Name)); } }

        /// <summary>
        /// Return a copy of the class
        /// </summary>
        /// <returns></returns>
        public DBTableName Copy()
        {
            DBTableName result = new DBTableName()
            {
                Catelog = this.Catelog,
                Schema = this.Schema,
                Name = this.Name,
                Alias = this.Alias
            };
            return result;
        }

        /// <summary>
        /// Checks if the given table name the same as this
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj != null)
            {
                if (obj is DBTableName item)
                {
                    if (item.Catelog == this.Catelog && item.Schema == this.Schema && item.Name == this.Name)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        /// <summary>
        /// Returns if this is an empty object
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return Name?.Length == 0;
        }

        /// <summary>
        /// Return a name with quotation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string QuotedName(string value)
        {
            if (value.Length > 0)
            {
                if (value.StartsWith("[") || value.EndsWith("]"))
                {
                    return value;
                }
                else
                {
                    return string.Format("[{0}]", value);
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}