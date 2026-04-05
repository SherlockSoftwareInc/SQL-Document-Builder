using SQL_Document_Builder.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SQL_Document_Builder.DatabaseAccess;
using SQL_Document_Builder.SchemaMetadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The document builder.
    /// </summary>
    internal class DocumentBuilder
    {
        private sealed class ObjectMetadataSnapshot
        {
            public string Description { get; set; } = string.Empty;
            public string Definition { get; set; } = string.Empty;
            public List<DBColumn> Columns { get; } = [];
            public List<IndexItem> Indexes { get; } = [];
            public List<ConstraintItem> Constraints { get; } = [];
            public List<DBParameter> Parameters { get; } = [];
            public string SynonymBaseObjectName { get; set; } = string.Empty;
            public string SynonymBaseObjectType { get; set; } = string.Empty;
        }

        private static readonly ISchemaMetadataProvider MetadataProvider = new SchemaProviderCoreMetadataProvider();

        /// <summary>
        /// Builds the table list.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> BuildObjectList(List<ObjectName> objectList, DatabaseConnectionItem? connection, TemplateItem template, IProgress<int> progress, DBSchema? schemaCache = null)
        {
            if (objectList == null || objectList.Count == 0 || connection == null || template == null)
            {
                return string.Empty;
            }

            if (connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                string doc = template.Body;
                string objectItemTemplate = template.ObjectLists.ObjectRow;

                var sb = new StringBuilder();

                if (objectList.Count > 0)
                {
                    for (int i = 0; i < objectList.Count; i++)
                    {
                        int percentComplete = (i * 100) / objectList.Count;
                        if (percentComplete > 0 && percentComplete % 2 == 0)
                        {
                            progress.Report(percentComplete + 1);
                        }

                        bool useQuotedId = Properties.Settings.Default.UseQuotedIdentifier;
                        ObjectName dr = objectList[i];
                        string tableSchema = useQuotedId ? dr.Schema.QuotedName() : dr.Schema.RemoveQuote();
                        string tableName = useQuotedId ? dr.Name.QuotedName() : dr.Name.RemoveQuote();
                        string description = schemaCache != null
                            ? await schemaCache.GetObjectDescriptionAsync(dr)
                            : await MetadataProvider.GetObjectDescriptionAsync(dr, connection.ConnectionString);
                        var objectName = new ObjectName(dr.ObjectType, dr.Schema, dr.Name);
                        string fullName = useQuotedId ? objectName.FullName : objectName.FullNameNoQuote;

                        string objItemDoc = objectItemTemplate
                            .Replace("~ObjectName~", tableName)
                            .Replace("~ObjectSchema~", tableSchema)
                            .Replace("~ObjectFullName~", fullName)
                            .Replace("~ObjectType~", ObjectTypeToString(dr.ObjectType))
                            .Replace("~Description~", description);

                        sb.AppendLine(objItemDoc);
                    }
                }

                return doc.Replace("~ObjectItem~", sb.ToString());
            }

            // reserved for future use
            return string.Empty;

        }

        /// <summary>
        /// Convert the data table to markdown document.
        /// </summary>
        /// <param name="dt">The data table</param>
        /// <returns>A string.</returns>
        internal static string DataTableToDoc(DataTable dt)
        {
            var sb = new StringBuilder();

            // Header
            var headers = new StringBuilder("|");
            var separators = new StringBuilder("|");
            foreach (DataColumn col in dt.Columns)
            {
                headers.Append($" {col.ColumnName} |");
                separators.Append("------------|");
            }
            sb.AppendLine(headers.ToString());
            sb.AppendLine(separators.ToString());

            // Rows
            foreach (DataRow dr in dt.Rows)
            {
                var row = new StringBuilder("|");
                foreach (DataColumn col in dt.Columns)
                {
                    row.Append($" {dr[col]?.ToString() ?? ""} |");
                }
                sb.AppendLine(row.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Data the table to doc.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="tableTemplate">The table template.</param>
        /// <param name="rowTemplate">The row template.</param>
        /// <param name="cellTemplate">The cell template.</param>
        /// <returns>A string.</returns>
        internal static string DataTableToTemplateDoc(DataTable dt, Template.TemplateItem template)
        {
            if (dt == null || dt.Columns.Count == 0)
                return "";

            string tableTemplate = template.Body;
            string headerCellTemplate = template.DataTable.HeaderCell;
            string rowTemplate = template.DataTable.DataRow;
            string cellTemplate = template.DataTable.Cell;

            bool isHtml = tableTemplate.Contains("<table", StringComparison.OrdinalIgnoreCase);
            bool includeAlign = tableTemplate.Contains("~Align~");

            // === Build header ===
            var headerParts = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                string headerValue = col.ColumnName;
                if (isHtml)
                    headerValue = System.Net.WebUtility.HtmlEncode(headerValue);
                headerParts.Add(headerCellTemplate.Replace("~HeaderCell~", headerValue));
            }
            string header = string.Join(isHtml ? Environment.NewLine : " | ", headerParts);

            // === Build alignment row (for Markdown only) ===
            string align = string.Empty;
            if (includeAlign)
            {
                // You can customize alignment here: center by default
                var aligns = Enumerable.Repeat(":---:", dt.Columns.Count);
                align = string.Join(" | ", aligns);
                // Replace ~Align~ in the template body now, before inserting rows/cells
                tableTemplate = tableTemplate.Replace("~Align~", align);
            }

            // === Build rows ===
            var allRows = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                var cellParts = new List<string>();
                foreach (DataColumn col in dt.Columns)
                {
                    string value = dr[col]?.ToString() ?? "";
                    if (isHtml)
                        value = System.Net.WebUtility.HtmlEncode(value);
                    cellParts.Add(cellTemplate.Replace("~Cell~", value));
                }

                string joinedCells = string.Join(isHtml ? Environment.NewLine : " | ", cellParts);
                string row = rowTemplate.Replace("~Row~", joinedCells);
                allRows.Add(row);
            }

            string rows = string.Join(Environment.NewLine, allRows);

            // === Final replacement ===
            string result = tableTemplate
                .Replace("~Header~", header)
                .Replace("~Rows~", rows);

            return result;
        }

        /// <summary>
        /// Gets the table definition
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetObjectDef(ObjectName objectName, DatabaseConnectionItem? connection, TemplateItem template, TemplateItem? dataTemplate, DBSchema? schemaCache = null)
        {
            if (objectName == null || connection == null || template == null)
            {
                return string.Empty;
            }

            if (connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                string doc = template.Body;

                var objectMetadata = await LoadObjectMetadataAsync(objectName, connection, schemaCache);

                bool useQuotedId = Properties.Settings.Default.UseQuotedIdentifier;

                // Replace placeholders with actual values
                doc = doc.Replace("~ObjectName~", useQuotedId? objectName.Name : objectName.Name.RemoveQuote());
                doc = doc.Replace("~ObjectSchema~", useQuotedId? objectName.Schema : objectName.Schema.RemoveQuote());
                doc = doc.Replace("~ObjectFullName~", useQuotedId ? objectName.FullName : objectName.FullNameNoQuote);
                doc = doc.Replace("~ObjectType~", ObjectTypeToString(objectName.ObjectType));
                doc = doc.Replace("~TriggerName~", useQuotedId ? objectName.Name : objectName.Name.RemoveQuote());

                //doc = doc.Replace("~Description~", tableView.Description.Length == 0 ? " " : tableView.Description);
                doc = ProcessSection(doc, "Description", "~Description~", objectMetadata.Description.Length == 0 ? " " : objectMetadata.Description);

                //doc = doc.Replace("~Definition~", tableView.Definition);
                doc = ProcessSection(doc, "Definition", "~Definition~", objectMetadata.Definition);

                // Build the columns table
                if (doc.Contains("~Columns~"))
                {
                    string columnsDoc = template.Columns.Body;
                    if (columnsDoc.Contains("~ColumnItem~", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string columnItemTemplate = template.Columns.ColumnRow;
                        var columnsBody = GetColumnsBody(objectMetadata.Columns, columnItemTemplate);
                        columnsDoc = columnsDoc.Replace("~ColumnItem~", columnsBody);
                    }
                    //doc = doc.Replace("~Columns~", columnsDoc);
                    doc = ProcessSection(doc, "Columns", "~Columns~", columnsDoc);
                }

                // Build the indexes table
                if (doc.Contains("~Indexes~"))
                {
                    string indexesDoc = template.Indexes.Body;
                    if (indexesDoc.Contains("~IndexItem~", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string indexItemTemplate = template.Indexes.IndexRow;
                        var indexesBody = GetIndexesBody(objectMetadata.Indexes, indexItemTemplate);
                        if(string.IsNullOrEmpty(indexesBody))
                        {
                            indexesDoc = string.Empty;
                        }
                        else
                        {
                            indexesDoc = indexesDoc.Replace("~IndexItem~", indexesBody);
                        }
                    }
                    //doc = doc.Replace("~Indexes~", indexesDoc);
                    doc = ProcessSection(doc, "Indexes", "~Indexes~", indexesDoc);
                }

                // Build the constraints table
                if (doc.Contains("~Constraints~"))
                {
                    string constraintDoc = template.Constraints.Body;
                    if (constraintDoc.Contains("~ConstraintItem~", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string constraintItemTemplate = template.Constraints.ConstraintRow;
                        var constraintsBody = GetConstraintsBody(objectMetadata.Constraints, constraintItemTemplate);
                        if (string.IsNullOrEmpty(constraintsBody))
                        {
                            constraintDoc = string.Empty;
                        }
                        else
                        {
                            constraintDoc = constraintDoc.Replace("~ConstraintItem~", constraintsBody);
                        }
                    }
                    //doc = doc.Replace("~Constraints~", constraintDoc);
                    doc = ProcessSection(doc, "Constraints", "~Constraints~", constraintDoc);
                }

                // Build the parameters table
                if (doc.Contains("~Parameters~"))
                {
                    string parameterDoc = template.Parameters.Body;
                    if (parameterDoc.Contains("~ParameterItem~", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string parameterItemTemplate = template.Parameters.ParameterRow;
                        var parametersBody = GetParametersBody(objectMetadata.Parameters, parameterItemTemplate);
                        if (string.IsNullOrEmpty(parametersBody))
                        {
                            parameterDoc = string.Empty;
                        }
                        else
                        {
                            // Replace the placeholder with the actual parameters body
                            parameterDoc = parameterDoc.Replace("~ParameterItem~", parametersBody);
                        }
                    }
                    //doc = doc.Replace("~Parameters~", parameterDoc);
                    doc = ProcessSection(doc, "Parameters", "~Parameters~", parameterDoc);
                }

                // synonyms
                if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Synonym)
                {
                    doc = doc.Replace("~BaseObjectName~", objectMetadata.SynonymBaseObjectName);
                    doc = doc.Replace("~BaseObjectType~", objectMetadata.SynonymBaseObjectType);
                }

                // table values
                if (doc.Contains("~TableValues~") && dataTemplate != null)
                {
                    string tableValuesDoc = string.Empty;
                    string sql = $"SELECT * FROM {objectName.FullName}";
                    // Check if the SQL statement is a valid SELECT statement
                    var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                    if (!await provider.IsValidSelectStatementAsync(sql, connection.ConnectionString))
                    {
                        tableValuesDoc = "Cannot generate the value list because the object contains columns with unsupported data types.";
                    }
                    else
                    {
                        tableValuesDoc = await GetTableValuesAsync(sql, connection, dataTemplate);
                    }

                    doc = ProcessSection(doc, "TableValues", "~TableValues~", tableValuesDoc);
                }

                // triggers
                if (doc.Contains("~Triggers~") && !string.IsNullOrEmpty( template.Triggers ))
                {
                    string triggersDoc = await GetObjectTriggersAsync(objectName, connection, template.Triggers?? "", schemaCache);
                    doc = ProcessSection(doc, "Triggers", "~Triggers~", triggersDoc);
                }

                // trigger type
                if (doc.Contains("~TriggerType~"))
                {
                    string triggerType = await GetTriggerTypeAsync(objectName, connection);
                    if (string.IsNullOrEmpty(triggerType))
                    {
                        triggerType = "UNKNOWN";
                    }
                    doc = doc.Replace("~TriggerType~", triggerType);
                }

                // relationships
                if (doc.Contains("~Relationships~"))
                {
                    string relationshipsDoc = template.Relationships.Body;
                    if (relationshipsDoc.Contains("~RelationshipItem~", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string relationItemTemplate = template.Relationships.RelationshipRow;
                        var relationBody = await GetRelationshipsBody(objectName, relationItemTemplate, connection);
                        if (string.IsNullOrEmpty(relationBody))
                        {
                            relationshipsDoc = string.Empty;
                        }
                        else
                        {
                            relationshipsDoc = relationshipsDoc.Replace("~RelationshipItem~", relationBody);
                        }
                    }
                    doc = ProcessSection(doc, "Relationships", "~Relationships~", relationshipsDoc);
                }

                return doc;
            }

            // reserved for future use
            return string.Empty;

        }

        private static async Task<ObjectMetadataSnapshot> LoadObjectMetadataAsync(ObjectName objectName, DatabaseConnectionItem connection, DBSchema? schemaCache)
        {
            var snapshot = new ObjectMetadataSnapshot
            {
                Description = schemaCache != null
                    ? await schemaCache.GetObjectDescriptionAsync(objectName)
                    : await MetadataProvider.GetObjectDescriptionAsync(objectName, connection.ConnectionString)
            };

            switch (objectName.ObjectType)
            {
                case ObjectName.ObjectTypeEnums.Table:
                case ObjectName.ObjectTypeEnums.View:
                {
                    var cachedColumns = schemaCache?.GetCachedColumns(objectName) ?? [];
                    if (cachedColumns.Count > 0)
                    {
                        snapshot.Columns.AddRange(cachedColumns);
                    }
                    else
                    {
                        var dtColumns = await MetadataProvider.GetObjectColumnsAsync(objectName, connection.ConnectionString);
                        if (dtColumns != null)
                        {
                            foreach (DataRow row in dtColumns.Rows)
                            {
                                snapshot.Columns.Add(new DBColumn(row));
                            }
                        }
                    }

                    foreach (var column in snapshot.Columns)
                    {
                        column.Description = schemaCache != null
                            ? await schemaCache.GetLevel2DescriptionAsync(objectName, column.ColumnName)
                            : await MetadataProvider.GetColumnDescriptionAsync(objectName, column.ColumnName, connection.ConnectionString);
                    }

                    if (objectName.ObjectType == ObjectName.ObjectTypeEnums.Table)
                    {
                        var primaryKeyColumns = await MetadataProvider.GetPrimaryKeyColumnsAsync(objectName, connection.ConnectionString);
                        foreach (var primaryKeyColumn in primaryKeyColumns)
                        {
                            var column = snapshot.Columns.Find(c => c.ColumnName.Equals(primaryKeyColumn, StringComparison.OrdinalIgnoreCase));
                            if (column != null && !column.Ord.Contains("🗝", StringComparison.Ordinal))
                            {
                                column.Ord += "🗝";
                            }
                        }

                        var dtConstraints = await MetadataProvider.GetObjectConstraintsAsync(objectName, connection.ConnectionString);
                        if (dtConstraints != null)
                        {
                            foreach (DataRow row in dtConstraints.Rows)
                            {
                                snapshot.Constraints.Add(new ConstraintItem(row));
                            }
                        }
                    }

                    var dtIndexes = await MetadataProvider.GetObjectIndexesAsync(objectName, connection.ConnectionString);
                    if (dtIndexes != null && dtIndexes.Rows.Count > 0)
                    {
                        var indexGroups = dtIndexes.AsEnumerable()
                            .GroupBy(row => row["IndexName"]?.ToString() ?? string.Empty);

                        foreach (var group in indexGroups)
                        {
                            var firstRow = group.First();
                            string indexName = firstRow["IndexName"]?.ToString() ?? string.Empty;
                            string type = firstRow["Type"]?.ToString() ?? string.Empty;
                            bool isUnique = firstRow["IsUnique"] != DBNull.Value && Convert.ToBoolean(firstRow["IsUnique"]);
                            string columns = string.Join(", ", group.Select(r => $"{r["ColumnName"]}"));

                            snapshot.Indexes.Add(new IndexItem(indexName, type, columns, isUnique));

                            foreach (var row in group)
                            {
                                var columnName = row["ColumnName"]?.ToString();
                                if (string.IsNullOrWhiteSpace(columnName))
                                {
                                    continue;
                                }

                                var column = snapshot.Columns.Find(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                                if (column != null && !column.Ord.Contains("🔢", StringComparison.Ordinal))
                                {
                                    column.Ord += "🔢";
                                }
                            }
                        }
                    }

                    if (objectName.ObjectType == ObjectName.ObjectTypeEnums.View)
                    {
                        snapshot.Definition = await MetadataProvider.GetObjectDefinitionAsync(objectName, connection.ConnectionString);
                    }

                    break;
                }

                case ObjectName.ObjectTypeEnums.StoredProcedure:
                case ObjectName.ObjectTypeEnums.Function:
                {
                    snapshot.Definition = await MetadataProvider.GetObjectDefinitionAsync(objectName, connection.ConnectionString);

                    var dtParameters = await MetadataProvider.GetObjectParametersAsync(objectName, connection.ConnectionString);
                    if (dtParameters != null)
                    {
                        foreach (DataRow row in dtParameters.Rows)
                        {
                            var parameter = new DBParameter(row)
                            {
                                Description = schemaCache != null
                                    ? await schemaCache.GetLevel2DescriptionAsync(objectName, row["PARAMETER_NAME"]?.ToString() ?? string.Empty)
                                    : await MetadataProvider.GetColumnDescriptionAsync(objectName, row["PARAMETER_NAME"]?.ToString() ?? string.Empty, connection.ConnectionString)
                            };
                            snapshot.Parameters.Add(parameter);
                        }
                    }

                    break;
                }

                case ObjectName.ObjectTypeEnums.Trigger:
                    snapshot.Definition = await MetadataProvider.GetObjectDefinitionAsync(objectName, connection.ConnectionString);
                    break;

                case ObjectName.ObjectTypeEnums.Synonym:
                {
                    snapshot.Definition = await MetadataProvider.GetSynonymBaseObjectAsync(objectName, connection.ConnectionString);
                    var synonymInfo = await MetadataProvider.GetSynonymInfoAsync(objectName, connection.ConnectionString);
                    if (synonymInfo != null && synonymInfo.Rows.Count > 0)
                    {
                        var row = synonymInfo.Rows[0];
                        snapshot.SynonymBaseObjectName = row["BaseObjectName"]?.ToString() ?? string.Empty;
                        snapshot.SynonymBaseObjectType = row["BaseObjectType"]?.ToString() ?? string.Empty;
                    }

                    break;
                }
            }

            return snapshot;
        }

        /// <summary>
        /// Gets the relationships body.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="relationItemTemplate">The relation item template.</param>
        /// <returns>A string.</returns>
        private static async Task<string> GetRelationshipsBody(ObjectName objectName, string relationItemTemplate, DatabaseConnectionItem connection)
        {
            if(connection.DBMSType != DBMSTypeEnums.SQLServer || objectName == null || string.IsNullOrEmpty(relationItemTemplate))
            {
                return string.Empty;
            }

            var dt = await MetadataProvider.GetObjectRelationshipsAsync(objectName, connection.ConnectionString);

            if(dt?.Rows.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    string foreignKeyName = dr["ForeignKeyName"].ToString() ?? string.Empty;
                    string fromSchema = dr["FromSchema"].ToString() ?? string.Empty;
                    string fromTable = dr["FromTable"].ToString() ?? string.Empty;
                    string fromColumn = dr["FromColumn"].ToString() ?? string.Empty;
                    string toSchema = dr["ToSchema"].ToString() ?? string.Empty;
                    string toTable = dr["ToTable"].ToString() ?? string.Empty;
                    string toColumn = dr["ToColumn"].ToString() ?? string.Empty;
                    string onDelete = dr["OnDelete"].ToString() ?? "NO ACTION";
                    string onUpdate = dr["OnUpdate"].ToString() ?? "NO ACTION";
                    string relationDoc = relationItemTemplate
                        .Replace("~ForeignKeyName~", foreignKeyName)
                        .Replace("~FromSchema~", fromSchema)
                        .Replace("~FromTable~", fromTable)
                        .Replace("~FromColumn~", fromColumn)
                        .Replace("~ToSchema~", toSchema)
                        .Replace("~ToTable~", toTable)
                        .Replace("~ToColumn~", toColumn)
                        .Replace("~OnDelete~", onDelete)
                        .Replace("~OnUpdate~", onUpdate);
                    sb.AppendLine(relationDoc);
                }
                return sb.ToString();

            }
            return string.Empty;
        }

        /// <summary>
        /// Processes a template section: removes or replaces section with markers, or simply replaces placeholder if no markers exist.
        /// </summary>
        /// <param name="doc">The document template</param>
        /// <param name="sectionName">The section name (e.g., Triggers, Indexes)</param>
        /// <param name="placeholder">The placeholder (e.g., ~Triggers~)</param>
        /// <param name="content">The content to insert</param>
        /// <returns>The processed document</returns>
        public static string ProcessSection(string doc, string sectionName, string placeholder, string content)
        {
            string pattern = $@"<!-- SECTION:{Regex.Escape(sectionName)} -->(.*?)<!-- ENDSECTION:{Regex.Escape(sectionName)} -->";

            if (Regex.IsMatch(doc, pattern, RegexOptions.Singleline))
            {
                // Section markers found — process as section
                if (string.IsNullOrWhiteSpace(content))
                {
                    // Remove whole section
                    return Regex.Replace(
                        doc,
                        pattern,
                        string.Empty,
                        RegexOptions.Singleline);
                }
                else
                {
                    // Replace placeholder inside section and remove markers
                    return Regex.Replace(
                        doc,
                        pattern,
                        match =>
                        {
                            string sectionBody = match.Groups[1].Value;
                            sectionBody = sectionBody.Replace(placeholder, content);
                            return sectionBody.Trim();
                        },
                        RegexOptions.Singleline);
                }
            }
            else
            {
                // No section markers — simple replace
                if (string.IsNullOrWhiteSpace(content))
                {
                    return doc.Replace(placeholder, string.Empty);
                }
                else
                {
                    return doc.Replace(placeholder, content);
                }
            }
        }

        /// <summary>
        /// Gets the triggers async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="template">The template.</param>
        /// <returns>A Task.</returns>
        private static async Task<string> GetObjectTriggersAsync(ObjectName objectName, DatabaseConnectionItem? connection, string templateBody, DBSchema? schemaCache)
        {
            if (objectName == null || connection == null || string.IsNullOrEmpty(templateBody))
            {
                return string.Empty;
            }

            if (connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                StringBuilder sb = new();

                var allTriggers = await MetadataProvider.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Trigger, connection.ConnectionString);
                if (allTriggers.Count == 0)
                    return "";

                foreach (var triggerObject in allTriggers)
                {
                    var triggerInfo = await MetadataProvider.GetTriggerInfoAsync(triggerObject, connection.ConnectionString);
                    if (triggerInfo == null || triggerInfo.Rows.Count == 0)
                    {
                        continue;
                    }

                    var infoRow = triggerInfo.Rows[0];
                    var parentSchema = infoRow["ParentObjectSchema"]?.ToString() ?? string.Empty;
                    var parentName = infoRow["ParentObjectName"]?.ToString() ?? string.Empty;

                    if (!parentSchema.Equals(objectName.Schema, StringComparison.OrdinalIgnoreCase) ||
                        !parentName.Equals(objectName.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string triggerName = triggerObject.Name;
                    string definition = await MetadataProvider.GetObjectDefinitionAsync(triggerObject, connection.ConnectionString);
                    string triggerType = infoRow["TriggerType"]?.ToString() ?? "UNKNOWN";

                    string triggerDoc = templateBody;
                    triggerDoc = triggerDoc
                        .Replace("~TriggerName~", triggerName)
                        .Replace("~TriggerType~", triggerType)
                        .Replace("~Definition~", definition);

                    // if has description, replace the placeholder
                    if (triggerDoc.Contains("~Description~"))
                    {
                        // get trigger description
                        string description = schemaCache != null
                            ? await schemaCache.GetObjectDescriptionAsync(triggerObject)
                            : await MetadataProvider.GetObjectDescriptionAsync(triggerObject, connection.ConnectionString);

                        triggerDoc = ProcessSection(triggerDoc, "Description", "~Description~", description.Length == 0 ? " " : description);
                    }

                    sb.AppendLine(triggerDoc);
                }

                return sb.ToString();
            }

            // reserved for future use
            return string.Empty;
        }

        /// <summary>
        /// Gets the trigger type async.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>A Task.</returns>
        private static async Task<string> GetTriggerTypeAsync(ObjectName objectName, DatabaseConnectionItem? connection)
        {
            if (objectName == null || connection == null)
            {
                return string.Empty;
            }

            if (connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                var triggerInfo = await MetadataProvider.GetTriggerInfoAsync(objectName, connection.ConnectionString);
                if (triggerInfo == null || triggerInfo.Rows.Count == 0)
                    return string.Empty;

                var triggerType = triggerInfo.Rows[0]["TriggerType"]?.ToString();
                if (string.IsNullOrEmpty(triggerType))
                {
                    return string.Empty;
                }

                return triggerType.Equals("INSTEAD OF", StringComparison.OrdinalIgnoreCase)
                    ? "INSTEAD OF Trigger"
                    : "AFTER Trigger";
            }

            // reserved for future use
            return string.Empty;
        }

        /// <summary>
        /// Gets the table values async.
        /// </summary>
        /// <param name="sql">The full name.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetTableValuesAsync(string sql, DatabaseConnectionItem? connection, TemplateItem template)
        {
            if (string.IsNullOrWhiteSpace(sql) || connection == null)
            {
                return string.Empty;
            }

            if (connection.DBMSType == DBMSTypeEnums.SQLServer)
            {
                var provider = DatabaseAccessProviderFactory.GetProvider(connection);
                DataTable? dt = await provider.GetDataTableAsync(sql, connection?.ConnectionString ?? string.Empty);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return string.Empty;
                }
                return DataTableToTemplateDoc(dt, template);
            }

            // reserved for future use
            return string.Empty;
        }

        /// <summary>
        /// Converts tabular text data (tab-separated, first line is header) to a Markdown table.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        /// <returns>A string.</returns>
        internal static string TextToTable(string metaData, TemplateItem template)
        {
            if (string.IsNullOrWhiteSpace(metaData))
                return string.Empty;

            // Normalize line endings and split into lines
            metaData = metaData.TrimEnd('\r', '\n');
            metaData = metaData.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
            var lines = metaData.Split('\n'); // Do NOT remove empty entries

            if (lines.Length == 0)
                return string.Empty;

            // convert the metadata to DataTable with the first line as header, tab as delimiter
            DataTable data = new();
            string[] headers = lines[0].Split('\t');
            foreach (var header in headers)
            {
                data.Columns.Add(header.Trim(), typeof(string));
            }
            for (int i = 1; i < lines.Length; i++)
            {
                // Always split with StringSplitOptions.None to preserve empty columns
                var columns = lines[i].Split('\t');
                // Pad columns to match headers length
                if (columns.Length < headers.Length)
                {
                    Array.Resize(ref columns, headers.Length);
                }
                DataRow row = data.NewRow();
                for (int j = 0; j < headers.Length; j++)
                {
                    row[j] = (columns[j] ?? string.Empty).Trim();
                }
                data.Rows.Add(row);
            }

            return DataTableToTemplateDoc(data, template);
        }

        /// <summary>
        /// Gets the columns body.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>A string.</returns>
        private static string GetColumnsBody(List<DBColumn> columns, string columnTemplate)
        {
            var sb = new StringBuilder();
            if (columns.Count > 0)
            {
                foreach (DBColumn col in columns) // Fix: Use DBColumn instead of DataColumn
                {
                    string ord = col.Ord ?? string.Empty; // Fix: Access Ord from DBColumn
                    string colName = col.ColumnName;
                    if (string.IsNullOrEmpty(colName)) colName = " ";
                    string nullable = col.Nullable ? "Yes" : "No";

                    string colDoc = columnTemplate
                        .Replace("~ColumnOrd~", ord)
                        .Replace("~ColumnName~", colName)
                        .Replace("~ColumnDataType~", col.DataType)
                        .Replace("~ColumnNullable~", nullable)
                        .Replace("~ColumnDescription~", col.Description ?? string.Empty);

                    sb.AppendLine(colDoc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                //return columnTemplate
                //        .Replace("~ColumnOrd~", "")
                //        .Replace("~ColumnName~", "_No column found_")
                //        .Replace("~ColumnDataType~", "")
                //        .Replace("~ColumnNullable~", "")
                //        .Replace("~ColumnDescription~", "");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the constraints body.
        /// </summary>
        /// <param name="constraints">The constraints.</param>
        /// <returns>A string.</returns>
        private static string GetConstraintsBody(List<ConstraintItem> constraints, string strTemplate)
        {
            var sb = new StringBuilder();
            if (constraints.Count > 0)
            {
                foreach (var constraint in constraints)
                {
                    string colDoc = strTemplate
                        .Replace("~ConstraintName~", constraint.Name)
                        .Replace("~ConstraintType~", constraint.Type)
                        .Replace("~ConstraintColumn~", constraint.Column);

                    sb.AppendLine(colDoc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                //return strTemplate
                //        .Replace("~ConstraintName~", "_No constraint found_")
                //        .Replace("~ConstraintType~", "")
                //        .Replace("~ConstraintColumn~", "");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the indexes body.
        /// </summary>
        /// <param name="indexes">The indexes.</param>
        /// <returns>An object.</returns>
        private static string GetIndexesBody(List<IndexItem> indexes, string indexTemplate)
        {
            var sb = new StringBuilder();
            if (indexes.Count > 0)
            {
                foreach (var index in indexes)
                {
                    string column = index.Columns;
                    if(Properties.Settings.Default.UseQuotedIdentifier)
                    {
                        column = index.QuotedColumns;
                    }

                    string colDoc = indexTemplate
                        .Replace("~IndexName~", index.Name)
                        .Replace("~IndexType~", index.Type)
                        .Replace("~IndexColumns~", column)
                        .Replace("~UniqueIndex~", index.IsUnique ? "Yes" : "No");

                    sb.AppendLine(colDoc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                //return indexTemplate
                //        .Replace("~IndexName~", "_No index found_")
                //        .Replace("~IndexType~", "")
                //        .Replace("~IndexColumns~", "")
                //        .Replace("~UniqueIndex~", "");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the parameters body.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        /// <param name="parameterTemplate">The parameter template.</param>
        /// <returns>A string.</returns>
        private static string GetParametersBody(List<DBParameter> parameters, string parameterTemplate)
        {
            var sb = new StringBuilder();
            if (parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    string doc = parameterTemplate
                        .Replace("~ParameterOrd~", item.Ord)
                        .Replace("~ParameterName~", item.Name)
                        .Replace("~ParameterDataType~", item.DataType)
                        .Replace("~ParameterDirection~", item.Mode)
                        .Replace("~ParameterDescription~", item.Description ?? string.Empty);

                    sb.AppendLine(doc);
                }
                return sb.ToString().TrimEnd('\r', '\n', '\t', ' ');
            }
            else
            {
                //return parameterTemplate
                //        .Replace("~ParameterOrd~", "")
                //        .Replace("~ParameterName~", "_No parameter found_")
                //        .Replace("~ParameterDataType~", "")
                //        .Replace("~ParameterDirection~", "")
                //        .Replace("~ParameterDescription~", "");
                return string.Empty;
            }
        }

        /// <summary>
        /// Objects the type to string.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>A string.</returns>
        private static string ObjectTypeToString(ObjectName.ObjectTypeEnums objectType)
        {
            return objectType switch
            {
                ObjectName.ObjectTypeEnums.Table => "Table",
                ObjectName.ObjectTypeEnums.View => "View",
                ObjectName.ObjectTypeEnums.Function => "Function",
                ObjectName.ObjectTypeEnums.StoredProcedure => "Stored Procedure",
                ObjectName.ObjectTypeEnums.Trigger => "Trigger",
                ObjectName.ObjectTypeEnums.Synonym => "Synonym",
                _ => "Unknown"
            };
        }
    }
}