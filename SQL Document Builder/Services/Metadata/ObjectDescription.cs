using System;
using System.Collections.Generic;
using System.Data;
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
        public static async Task<string> BuildObjectDescription(ObjectName objectName, DBSchema? schemaCache, bool useExtendedProperties)
        {
            if (objectName == null || objectName.IsEmpty() || schemaCache == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var dbmsType = GetEffectiveDbmsType(schemaCache);

            var objectDescription = await schemaCache.GetObjectDescriptionAsync(objectName);
            AppendObjectDescriptionScript(sb, objectName, dbmsType, objectDescription, useExtendedProperties);

            if (objectName.ObjectType == ObjectTypeEnums.Table ||
                objectName.ObjectType == ObjectTypeEnums.View)
            {
                var columns = schemaCache.GetCachedColumns(objectName);
                if (columns.Count == 0)
                {
                    columns = await schemaCache.GetColumnsAsync(objectName);
                }

                foreach (var column in columns)
                {
                    var columnDescription = await schemaCache.GetLevel2DescriptionAsync(objectName, column.ColumnName);
                    AppendColumnDescriptionScript(sb, objectName, dbmsType, column, columnDescription, useExtendedProperties);
                }
            }
            else
            {
                var parameterNames = GetParameterNames(await schemaCache.GetObjectParametersAsync(objectName));
                foreach (var parameterName in parameterNames)
                {
                    var parameterDescription = await schemaCache.GetLevel2DescriptionAsync(objectName, parameterName);
                    AppendParameterDescriptionScript(sb, objectName, dbmsType, parameterName, parameterDescription, useExtendedProperties);
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

        private static DBMSTypeEnums GetEffectiveDbmsType(DBSchema? schemaCache)
        {
            if (schemaCache?.Connection == null)
            {
                return DBMSTypeEnums.Other;
            }

            return schemaCache.Connection.DBMSType;
        }

        private static IEnumerable<string> GetParameterNames(DataTable? parameters)
        {
            if (parameters == null || parameters.Rows.Count == 0)
            {
                return [];
            }

            var names = new List<string>();
            foreach (DataRow row in parameters.Rows)
            {
                var parameterName = row.Table.Columns.Contains("PARAMETER_NAME")
                    ? row["PARAMETER_NAME"]?.ToString()
                    : row.Table.Columns.Contains("ParameterName")
                        ? row["ParameterName"]?.ToString()
                        : string.Empty;

                if (!string.IsNullOrWhiteSpace(parameterName))
                {
                    names.Add(parameterName);
                }
            }

            return names;
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

        private static void AppendColumnDescriptionScript(StringBuilder sb, ObjectName objectName, DBMSTypeEnums dbmsType, DBColumn? column, string? description, bool useExtendedProperties)
        {
            if (column == null || string.IsNullOrWhiteSpace(column.ColumnName) || string.IsNullOrWhiteSpace(description))
            {
                return;
            }

            var columnName = column.ColumnName;
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

                case DBMSTypeEnums.MySQL:
                case DBMSTypeEnums.MariaDB:
                    if (!string.IsNullOrWhiteSpace(column.DataType))
                    {
                        var nullability = column.Nullable ? "NULL" : "NOT NULL";
                        sb.AppendLine($"ALTER TABLE {GetQualifiedName(objectName, dbmsType)} MODIFY COLUMN {QuoteIdentifier(columnName, dbmsType)} {column.DataType} {nullability} COMMENT '{escapedDescription}';");
                    }
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