namespace SQL_Document_Builder
{
    /// <summary>
    /// The object name.
    /// </summary>
    public class ObjectName
    {
        private string _name = "";

        private string _schema = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectName"/> class.
        /// </summary>
        public ObjectName()
        {
            Schema = string.Empty;
            Name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectName"/> class.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="objectName">The object name.</param>
        public ObjectName(ObjectTypeEnums objectType, string objectName)
        {
            if (objectName.IndexOf('.') >= 0)
            {
                var names = objectName.Split('.');
                Schema = names[0];
                Name = names[1];
                ObjectType = objectType;
            }
            else
            {
                ObjectType = ObjectTypeEnums.None;
                Schema = string.Empty;
                Name = string.Empty;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectName"/> class.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        public ObjectName(ObjectTypeEnums objectType, string schemaName, string tableName)
        {
            ObjectType = objectType;
            Schema = schemaName;
            Name = tableName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectName"/> class.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        public ObjectName(string schemaName, string tableName)
        {
            ObjectType = ObjectTypeEnums.None;
            Schema = schemaName;
            Name = tableName;
        }

        /// <summary>
        /// The object type enums.
        /// </summary>
        public enum ObjectTypeEnums
        {
            None,
            Table,
            View
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName
        {
            get
            {
                if (_schema.Length > 0)
                {
                    return string.Format("[{0}].[{1}]", _schema, _name);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = RemoveQuota(value); }
        }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public ObjectTypeEnums ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        public string Schema
        {
            get { return _schema; }
            set { _schema = RemoveQuota(value); }
        }

        /// <summary>
        /// Are the empty.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool IsEmpty()
        {
            return _name.Length == 0 || _schema.Length == 0;
        }

        /// <summary>
        /// Tos the string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            if (Schema.Length > 0)
            {
                return string.Format("{0}.{1}", Schema, Name);
            }
            return string.Empty;
        }

        /// <summary>
        /// Remove quota from the object name
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveQuota(string text)
        {
            if (text.StartsWith("[") && text.EndsWith("]"))
            {
                return text.Substring(1, text.Length - 2);
            }

            if (text.StartsWith("'") && text.EndsWith("'"))
            {
                return text.Substring(1, text.Length - 2);
            }

            return text;
        }
    }
}