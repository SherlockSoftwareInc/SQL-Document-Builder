namespace SQL_Document_Builder
{
    public class ObjectName
    {
        private string _name = "";

        private string _schema = "";

        public ObjectName()
        {
            Schema = string.Empty;
            Name = string.Empty;
        }

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

        public enum ObjectTypeEnums
        {
            None,
            Table,
            View
        }
        public string Name
        {
            get { return _name; }
            set { _name = RemoveQuota(value); }
        }

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

        public ObjectTypeEnums ObjectType { get; set; }

        public string Schema
        {
            get { return _schema; }
            set { _schema = RemoveQuota(value); }
        }

        

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