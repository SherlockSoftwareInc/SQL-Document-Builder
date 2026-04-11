using System;
using System;
using System.Text;
using System.Threading.Tasks;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The object description.
    /// </summary>
    internal class ObjectDescription
    {
        /// <summary>
        /// Builds the object description.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <returns>A string.</returns>
        public static async Task<string> BuildObjectDescription(ObjectName objectName, DatabaseConnectionItem? connection, bool useExtendedProperties)
        {
            var sb = new StringBuilder();
            var dbmsType = GetEffectiveDbmsType(connection);

            var dbTable = new DBObject();
            if (await dbTable.OpenAsync(objectName, connection))
            {
                AppendObjectDescriptionScript(sb, objectName, dbmsType, dbTable.Description, useExtendedProperties);

                if (objectName.ObjectType == ObjectTypeEnums.Table ||
                   objectName.ObjectType == ObjectTypeEnums.View)
                {
                    foreach (var column in dbTable.Columns)
                    {
                        AppendColumnDescriptionScript(sb, objectName, dbmsType, column.ColumnName, column.Description, useExtendedProperties);
                    }
                }
                else
                {
                    foreach (var parameter in dbTable.Parameters)
                    {
                        AppendParameterDescriptionScript(sb, objectName, dbmsType, parameter.Name, parameter.Description, useExtendedProperties);
                    }
                }
            }

            var script = sb.ToString();

            if (script.Length > 0)
            {
                // Remove the last line break
                script = script.TrimEnd('\r', '\n', ' ', '\t') + Environment.NewLine;
            }

            return script;
        }

        private static DBMSTypeEnums GetEffectiveDbmsType(DatabaseConnectionItem? connection)
        {
            if (connection == null)
            {
                return DBMSTypeEnums.Other;
            }

            if (connection.ConnectionType.Equals("SQL Server", StringComparison.OrdinalIgnoreCase))
            {
                return DBMSTypeEnums.SQLServer;
            }

            if (connection.ConnectionType.Equals("ODBC", StringComparison.OrdinalIgnoreCase) &&
                connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                return DBMSTypeEnums.Other;
            }

            return connection.DBMSType;
        }

        private static void AppendObjectDescriptionScript(StringBuilder sb, ObjectName objectName, DBMSTypeEnums dbmsType, string? description, bool useExtendedProperties)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return;
            }

            var escapedDescription = EscapeSqlLiteral(description);

            switch (dbmsType)
            {
                case DBMSTypeEnums.SQLServer:
                    var level1type = GetSqlServerLevel1Type(objectName.ObjectType);
                    var sqlServerProcedure = useExtendedProperties ? "sp_addextendedproperty" : "usp_addupdateextendedproperty";
                    sb.AppendLine($"EXEC {sqlServerProcedure} " +
                                  $"@name = N'MS_Description', " +
                                  $"@value = N'{escapedDescription}', " +
                                  $"@level0type = N'SCHEMA', @level0name = N'{EscapeSqlLiteral(objectName.Schema)}', " +
                                  $"@level1type = N'{level1type}', @level1name = N'{EscapeSqlLiteral(objectName.Name)}';");
                    break;

                case DBMSTypeEnums.PostgreSQL:
                    if (objectName.ObjectType is ObjectTypeEnums.Table or ObjectTypeEnums.View)
                    {
                        var objectKind = objectName.ObjectType == ObjectTypeEnums.View ? "VIEW" : "TABLE";
                        sb.AppendLine($"COMMENT ON {objectKind} {GetQualifiedName(objectName, dbmsType)} IS '{escapedDescription}';");
                    }
                    break;

                case DBMSTypeEnums.Oracle:
                    if (objectName.ObjectType is ObjectTypeEnums.Table or ObjectTypeEnums.View)
                    {
                        sb.AppendLine($"COMMENT ON TABLE {GetQualifiedName(objectName, dbmsType)} IS '{escapedDescription}';");
                    }
                    break;

                case DBMSTypeEnums.MySQL:
                case DBMSTypeEnums.MariaDB:
                    if (objectName.ObjectType == ObjectTypeEnums.Table)
                    {
                        sb.AppendLine($"ALTER TABLE {GetQualifiedName(objectName, dbmsType)} COMMENT = '{escapedDescription}';");
                    }
                    break;
            }
        }

        private static void AppendColumnDescriptionScript(StringBuilder sb, ObjectName objectName, DBMSTypeEnums dbmsType, string? columnName, string? description, bool useExtendedProperties)
        {
            if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(description))
            {
                return;
            }

            var escapedDescription = EscapeSqlLiteral(description);
            switch (dbmsType)
            {
                case DBMSTypeEnums.SQLServer:
                    var level1type = GetSqlServerLevel1Type(objectName.ObjectType);
                    var sqlServerProcedure = useExtendedProperties ? "sp_addextendedproperty" : "usp_addupdateextendedproperty";
                    sb.AppendLine($"EXEC {sqlServerProcedure} " +
                                  $"@name = N'MS_Description', " +
                                  $"@value = N'{escapedDescription}', " +
                                  $"@level0type = N'SCHEMA', @level0name = N'{EscapeSqlLiteral(objectName.Schema)}', " +
                                  $"@level1type = N'{level1type}', @level1name = N'{EscapeSqlLiteral(objectName.Name)}', " +
                                  $"@level2type = N'COLUMN', @level2name = N'{EscapeSqlLiteral(columnName)}';");
                    break;

                case DBMSTypeEnums.PostgreSQL:
                case DBMSTypeEnums.Oracle:
                    sb.AppendLine($"COMMENT ON COLUMN {GetQualifiedName(objectName, dbmsType)}.{QuoteIdentifier(columnName, dbmsType)} IS '{escapedDescription}';");
                    break;
            }
        }

        private static void AppendParameterDescriptionScript(StringBuilder sb, ObjectName objectName, DBMSTypeEnums dbmsType, string? parameterName, string? description, bool useExtendedProperties)
        {
            if (string.IsNullOrWhiteSpace(parameterName) || string.IsNullOrWhiteSpace(description))
            {
                return;
            }

            if (dbmsType != DBMSTypeEnums.SQLServer)
            {
                return;
            }

            var level1type = GetSqlServerLevel1Type(objectName.ObjectType);
            var escapedDescription = EscapeSqlLiteral(description);
            var sqlServerProcedure = useExtendedProperties ? "sp_addextendedproperty" : "usp_addupdateextendedproperty";

            sb.AppendLine($"EXEC {sqlServerProcedure} " +
                          $"@name = N'MS_Description', " +
                          $"@value = N'{escapedDescription}', " +
                          $"@level0type = N'SCHEMA', @level0name = N'{EscapeSqlLiteral(objectName.Schema)}', " +
                          $"@level1type = N'{level1type}', @level1name = N'{EscapeSqlLiteral(objectName.Name)}', " +
                          $"@level2type = N'PARAMETER', @level2name = N'{EscapeSqlLiteral(parameterName)}';");
        }

        private static string GetSqlServerLevel1Type(ObjectTypeEnums objectType)
        {
            return objectType switch
            {
                ObjectTypeEnums.Table => "TABLE",
                ObjectTypeEnums.View => "VIEW",
                ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                ObjectTypeEnums.Function => "FUNCTION",
                ObjectTypeEnums.Trigger => "TRIGGER",
                ObjectTypeEnums.Synonym => "SYNONYM",
                _ => throw new InvalidOperationException("Unsupported object type for description update.")
            };
        }

        private static string GetQualifiedName(ObjectName objectName, DBMSTypeEnums dbmsType)
        {
            var quotedName = QuoteIdentifier(objectName.Name, dbmsType);
            if (string.IsNullOrWhiteSpace(objectName.Schema))
            {
                return quotedName;
            }

            return $"{QuoteIdentifier(objectName.Schema, dbmsType)}.{quotedName}";
        }

        private static string QuoteIdentifier(string? identifier, DBMSTypeEnums dbmsType)
        {
            var value = identifier ?? string.Empty;
            return dbmsType switch
            {
                DBMSTypeEnums.MySQL or DBMSTypeEnums.MariaDB => $"`{value.Replace("`", "``")}`",
                DBMSTypeEnums.PostgreSQL or DBMSTypeEnums.Oracle => $"\"{value.Replace("\"", "\"\"")}\"",
                _ => $"[{value.Replace("]", "]]" )}]"
            };
        }

        private static string EscapeSqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}