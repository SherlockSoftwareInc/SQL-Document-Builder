using System;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<string> BuildObjectDescription(ObjectName objectName, string connectionString, bool useExtendedProperties)
        {
            bool spaceAdded = false;
            var sb = new StringBuilder();

            var dbTable = new DBObject();
            if (await dbTable.OpenAsync(objectName, connectionString))
            {
                var tableDesc = dbTable.Description;
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
                                      $"@level1type = N'TABLE', @level1name = N'{objectName.Name}';");
                    }
                    else
                    {
                        // Use the default stored procedure for table description
                        sb.AppendLine($"EXEC usp_AddObjectDescription '{objectName.Schema}.{objectName.Name}', N'{tableDesc.Replace("'", "''")}';");
                    }
                }
            }

            foreach (var column in dbTable.Columns)
            {
                var colDesc = column.Description;
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
                                      $"@value = N'{colDesc.Replace("'", "''")}', " +
                                      $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                      $"@level1type = N'TABLE', @level1name = N'{objectName.Name}', " +
                                      $"@level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
                    }
                    else
                    {
                        // Use the default stored procedure for column description
                        sb.AppendLine($"EXEC usp_AddColumnDescription '{objectName.Schema}.{objectName.Name}', '{column.ColumnName}', N'{colDesc.Replace("'", "''")}';");
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

        /// <summary>
        /// Builds the object description asynchronously.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="useExtendedProperties">Whether to use extended properties for descriptions.</param>
        /// <returns>A Task<string> containing the generated script.</returns>
        public static async Task<string> BuildObjectDescriptionAsync(ObjectName objectName, string connectionString, bool useExtendedProperties)
        {
            if (objectName == null)
            {
                throw new ArgumentNullException(nameof(objectName), "The object name cannot be null.");
            }

            if (objectName.IsEmpty())
            {
                throw new InvalidOperationException("The object name must have a valid schema and name.");
            }

            bool spaceAdded = false;
            var sb = new StringBuilder();

            var dbTable = new DBObject();
            if (await dbTable.OpenAsync(objectName, connectionString))
            {
                var tableDesc = dbTable.Description;
                if (!string.IsNullOrEmpty(tableDesc))
                {
                    spaceAdded = true;

                    if (useExtendedProperties)
                    {
                        // Use sp_addextendedproperty for table description
                        sb.AppendLine($"EXEC sp_addextendedproperty " +
                                      $"@name = N'MS_Description', " +
                                      $"@value = N'{tableDesc.Replace("'", "''")}', " +
                                      $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                      $"@level1type = N'TABLE', @level1name = N'{objectName.Name}';");
                    }
                    else
                    {
                        // Use the default stored procedure for table description
                        sb.AppendLine($"EXEC usp_AddObjectDescription '{objectName.Schema}.{objectName.Name}', N'{tableDesc.Replace("'", "''")}';");
                    }
                }
            }

            foreach (var column in dbTable.Columns)
            {
                var colDesc = column.Description;
                if (!string.IsNullOrEmpty(colDesc))
                {
                    if (!spaceAdded)
                    {
                        spaceAdded = true;
                    }

                    if (useExtendedProperties)
                    {
                        // Use sp_addextendedproperty for column description
                        sb.AppendLine($"EXEC sp_addextendedproperty " +
                                      $"@name = N'MS_Description', " +
                                      $"@value = N'{colDesc.Replace("'", "''")}', " +
                                      $"@level0type = N'SCHEMA', @level0name = N'{objectName.Schema}', " +
                                      $"@level1type = N'TABLE', @level1name = N'{objectName.Name}', " +
                                      $"@level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
                    }
                    else
                    {
                        // Use the default stored procedure for column description
                        sb.AppendLine($"EXEC usp_AddColumnDescription '{objectName.Schema}.{objectName.Name}', '{column.ColumnName}', N'{colDesc.Replace("'", "''")}';");
                    }
                }
            }

            var script = sb.ToString();

            if (script.Length > 0)
            {
                // Remove the last line break
                script = script.TrimEnd('\r', '\n', ' ', '\t');
            }

            return script;
        }
    }
}