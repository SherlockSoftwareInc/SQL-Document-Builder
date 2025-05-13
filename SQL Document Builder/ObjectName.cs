using System;

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
        public ObjectName(ObjectTypeEnums objectType, string? objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                ObjectType = ObjectTypeEnums.None;
                Schema = string.Empty;
                Name = string.Empty;
                return;
            }

            if (objectName.Contains('.'))
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
        public ObjectName(ObjectTypeEnums objectType, string? schemaName, string? tableName)
        {
            Schema = schemaName ?? "";
            Name = tableName ?? "";
            if (string.IsNullOrEmpty(Name))
            {
                ObjectType = ObjectTypeEnums.None;
            }
            else
            {
                ObjectType = objectType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectName"/> class.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        public ObjectName(string? schemaName, string? tableName)
        {
            ObjectType = ObjectTypeEnums.None;
            Schema = schemaName ?? "";
            Name = tableName ?? "";
        }

        /// <summary>
        /// The object type enums.
        /// </summary>
        public enum ObjectTypeEnums
        {
            None,
            Table,
            View,
            StoredProcedure,
            Function
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
                    return $"[{_schema}].[{_name}]";
                }
                if (_name.Length > 0)
                {
                    return $"[{_name}]";
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
        /// Equals the.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A bool.</returns>
        public override bool Equals(object? obj)
        {
            // Check if the object is null
            if (obj == null)
                return false;

            // Check if the object is of the same type
            if (obj is not ObjectName other)
                return false;

            // Compare the properties
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Schema, other.Schema, StringComparison.OrdinalIgnoreCase) &&
                   ObjectType == other.ObjectType;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>An int.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Schema, ObjectType);
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
        /// Removes surrounding quotes or brackets from the object name.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>The text without surrounding quotes or brackets.</returns>
        private static string RemoveQuota(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 3)
                return text;

            // Check for surrounding brackets [ ] or single quotes ' '
            if ((text.StartsWith('[') && text.EndsWith(']')) || (text.StartsWith('\'') && text.EndsWith('\'')))
            {
                return text[1..^1];
            }

            return text;
        }
    }
}