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
        ///// <summary>
        ///// Builds the descriptions for all objects in a specified schame.
        ///// </summary>
        ///// <param name="objType">The obj type.</param>
        ///// <param name="schemaName">The schema name.</param>
        ///// <param name="progress">The progress.</param>
        ///// <returns>A string.</returns>
        //public static async Task<string> BuildObjectDescriptionsAsync(ObjectName.ObjectTypeEnums objType, string schemaName, IProgress<int> progress)
        //{
        //    var sb = new StringBuilder();

        //    var tables = Common.GetObjectList(objType);
        //    for (int i = 0; i < tables.Count; i++)
        //    {
        //        int percentComplete = (i * 100) / tables.Count;
        //        if (percentComplete > 0 && percentComplete % 2 == 0)
        //            progress.Report(percentComplete + 1);

        //        var table = tables[i];
        //        bool spaceAdded = false;

        //        bool generate = true;
        //        if (schemaName.Length > 0 && !schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            generate = false;
        //        }
        //        if (generate)
        //        {
        //            var dbTable = new DBObject();
        //            if (await dbTable.OpenAsync(table, Properties.Settings.Default.dbConnectionString))
        //            {
        //                var tableDesc = dbTable.Description;
        //                if (tableDesc.Length > 0)
        //                {
        //                    //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
        //                    sb.AppendLine();
        //                    spaceAdded = true;
        //                    sb.AppendLine(string.Format("EXEC usp_AddObjectDescription '{0}.{1}', N'{2}';", table.Schema, table.Name, tableDesc.Replace("'", "''")));
        //                }
        //            }

        //            foreach (var column in dbTable.Columns)
        //            {
        //                var colDesc = column.Description;
        //                if (colDesc.Length > 0)
        //                {
        //                    if (!spaceAdded)
        //                    {
        //                        //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
        //                        sb.AppendLine();
        //                        spaceAdded = true;
        //                    }
        //                    sb.AppendLine(string.Format("EXEC usp_AddColumnDescription '{0}.{1}', '{2}', N'{3}';", table.Schema, table.Name, column.ColumnName, colDesc.Replace("'", "''")));
        //                }
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}

        /// <summary>
        /// Builds the object description.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <returns>A string.</returns>
        public static async Task<string> BuildObjectDescription(ObjectName objectName, bool useExtendedProperties)
        {
            bool spaceAdded = false;
            var sb = new StringBuilder();

            var dbTable = new DBObject();
            if (await dbTable.OpenAsync(objectName, Properties.Settings.Default.dbConnectionString))
            {
                var tableDesc = dbTable.Description;
                if (tableDesc.Length > 0)
                {
                    //sb.AppendLine();
                    spaceAdded = true;

                    if (useExtendedProperties)
                    {
                        // Use sp_addextendedproperty for table description
                        sb.AppendLine($"EXECUTE sp_addextendedproperty " +
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
                        sb.AppendLine($"EXECUTE sp_addextendedproperty " +
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

        /// <summary>
        /// Builds the object description asynchronously.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="useExtendedProperties">Whether to use extended properties for descriptions.</param>
        /// <returns>A Task<string> containing the generated script.</returns>
        public static async Task<string> BuildObjectDescriptionAsync(ObjectName objectName, bool useExtendedProperties)
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
            if (await dbTable.OpenAsync(objectName, Properties.Settings.Default.dbConnectionString))
            {
                var tableDesc = dbTable.Description;
                if (!string.IsNullOrEmpty(tableDesc))
                {
                    spaceAdded = true;

                    if (useExtendedProperties)
                    {
                        // Use sp_addextendedproperty for table description
                        sb.AppendLine($"EXECUTE sp_addextendedproperty " +
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
                        sb.AppendLine($"EXECUTE sp_addextendedproperty " +
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