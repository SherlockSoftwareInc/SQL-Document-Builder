using System;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents a database object.
    /// </summary>
    public class DatabaseObject
    {
        /// <summary>
        /// Gets or sets the object name.
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Tos the string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return $"{SchemaName}.{ObjectName}";
        }

        /// <summary>
        /// Equals the.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A bool.</returns>
        public override bool Equals(object? obj)
        {
            // returns true if the object is the same type and has the same name and schema
            if (obj is DatabaseObject other)
            {
                return ObjectName.Equals(other.ObjectName, StringComparison.OrdinalIgnoreCase) &&
                       SchemaName.Equals(other.SchemaName, StringComparison.OrdinalIgnoreCase) &&
                       ObjectType.Equals(other.ObjectType, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}