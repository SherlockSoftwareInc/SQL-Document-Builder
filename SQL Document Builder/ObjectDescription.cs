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
            bool spaceAdded = false;
            var sb = new StringBuilder();

            var dbTable = new DBObject();
            if (await dbTable.OpenAsync(objectName, connection))
            {
                var tableDesc = dbTable.Description;
                string level1type = objectName.ObjectType switch
                {
                    ObjectTypeEnums.Table => "TABLE",
                    ObjectTypeEnums.View => "VIEW",
                    ObjectTypeEnums.StoredProcedure => "PROCEDURE",
                    ObjectTypeEnums.Function => "FUNCTION",
                    ObjectTypeEnums.Trigger => "TRIGGER",
                    ObjectTypeEnums.Synonym => "SYNONYM",
                    _ => throw new InvalidOperationException("Unsupported object type for description update.")
                };

                if (tableDesc.Length > 0)
                {
                    //sb.AppendLine();
                    spaceAdded = true;

                    if (useExtendedProperties)
                    {
                        // Use sp_addextendedproperty for table description
                        sb.AppendLine($"EXEC sp_addextendedproperty " +
                                      $"@name = N'MS_Description', " +
                                      $"@value = N'{tableDesc.Replace("'", "''")}', " +
                                      $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                      $"@level1type = N'{level1type}', @level1name = N'{objectName.Name}';");
                    }
                    else
                    {
                        // Use the default stored procedure for table description
                        sb.AppendLine($"EXEC usp_addupdateextendedproperty " +
                                      $"@name = N'MS_Description', " +
                                      $"@value = N'{tableDesc.Replace("'", "''")}', " +
                                      $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                      $"@level1type = N'{level1type}', @level1name = N'{objectName.Name}';");

                        //sb.AppendLine($"EXEC usp_AddObjectDescription '{objectName.Schema}.{objectName.Name}', N'{tableDesc.Replace("'", "''")}';");
                    }
                }

                if (objectName.ObjectType == ObjectTypeEnums.Table ||
                   objectName.ObjectType == ObjectTypeEnums.View)
                {
                    foreach (var column in dbTable.Columns)
                    {
                        var colDesc = column.Description.Replace("'", "''");
                        if (colDesc.Length > 0)
                        {
                            if (!spaceAdded)
                            {
                                //sb.AppendLine();
                                spaceAdded = true;
                            }

                            if (useExtendedProperties)
                            {
                                // Use sp_addextendedproperty for column description
                                sb.AppendLine($"EXEC sp_addextendedproperty " +
                                              $"@name = N'MS_Description', " +
                                              $"@value = N'{colDesc}', " +
                                              $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                              $"@level1type = N'{level1type}', @level1name = N'{objectName.Name}', " +
                                              $"@level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
                            }
                            else
                            {
                                // Use the default stored procedure for column description
                                sb.AppendLine($"EXEC usp_addupdateextendedproperty " +
                                              $"@name = N'MS_Description', " +
                                              $"@value = N'{colDesc}', " +
                                              $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                              $"@level1type = N'{level1type}', @level1name = N'{objectName.Name}', " +
                                              $"@level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
                                //sb.AppendLine($"EXEC usp_AddColumnDescription '{objectName.Schema}.{objectName.Name}', '{column.ColumnName}', N'{colDesc.Replace("'", "''")}';");
                            }
                        }
                    }
                }
                else
                {
                    foreach (var parameter in dbTable.Parameters)
                    {
                        var paraDesc = parameter.Description.Replace("'", "''");
                        if (paraDesc.Length > 0)
                        {
                            if (!spaceAdded)
                            {
                                //sb.AppendLine();
                                spaceAdded = true;
                            }

                            if (useExtendedProperties)
                            {
                                // Use sp_addextendedproperty for column description
                                sb.AppendLine($"EXEC sp_addextendedproperty " +
                                              $"@name = N'MS_Description', " +
                                              $"@value = N'{paraDesc}', " +
                                              $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                              $"@level1type = N'{level1type}', @level1name = N'{objectName.Name}', " +
                                              $"@level2type = N'PARAMETER', @level2name = N'{parameter.Name}';");
                            }
                            else
                            {
                                // Use the default stored procedure for column description
                                sb.AppendLine($"EXEC usp_addupdateextendedproperty " +
                                              $"@name = N'MS_Description', " +
                                              $"@value = N'{paraDesc}', " +
                                              $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                              $"@level1type = N'{level1type}', @level1name = N'{objectName.Name}', " +
                                              $"@level2type = N'PARAMETER', @level2name = N'{parameter.Name}';");
                            }
                        }
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
    }
}