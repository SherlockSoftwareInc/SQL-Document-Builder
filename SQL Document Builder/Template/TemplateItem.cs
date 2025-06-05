using System;
using System.Xml;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template item.
    /// </summary>
    internal class TemplateItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateItem"/> class.
        /// </summary>
        public TemplateItem()
        {
            DocumentType = DocumentTypeEnums.Markdown;
            ObjectType = ObjectTypeEnums.Table;
            Body = string.Empty;
        }

        /// <summary>
        /// The document type enums.
        /// </summary>
        public enum DocumentTypeEnums
        {
            Markdown,
            SharePoint,
            Wiki
        }

        /// <summary>
        /// The object type enums.
        /// </summary>
        public enum ObjectTypeEnums
        {
            Table,
            View,
            Function,
            StoredProcedure,
            Trigger
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateItem"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public TemplateItem(XmlNode xNode)
            : this()
        {
            if (xNode.HasChildNodes)
            {
                foreach (XmlNode node in xNode.ChildNodes)
                {
                    string elementName = node.Name.ToLower();
                    string elementValue = node.InnerText;

                    switch (elementName)
                    {
                        case "documenttype":
                            if (elementValue.Length > 0 && Enum.TryParse(typeof(DocumentTypeEnums), elementValue, true, out var docType))
                                this.DocumentType = (DocumentTypeEnums)docType;
                            break;

                        case "objecttype":
                            if (elementValue.Length > 0 && Enum.TryParse(typeof(ObjectTypeEnums), elementValue, true, out var objType))
                                this.ObjectType = (ObjectTypeEnums)objType;
                            break;

                        case "body":
                            this.Body = elementValue;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        public DocumentTypeEnums DocumentType { get; set; } = DocumentTypeEnums.Markdown;

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public ObjectTypeEnums ObjectType { get; set; } = ObjectTypeEnums.Table;

        /// <summary>
        /// Gets or sets the template body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Writes the template item.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("TemplateItem");

            writer.WriteElementString("DocumentType", this.DocumentType.ToString());
            writer.WriteElementString("ObjectType", this.ObjectType.ToString());
            writer.WriteElementString("Body", this.Body);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets the table template.
        /// </summary>
        private static string TableTemplate => $@"# Table: `[ObjectName]`

**Description:**
[ObjectDescription]
**Columns:**

| Column Name        | Data Type     | Length/Precision/Scale | Nullable | Primary Key | Foreign Key (References) | Default Value | Constraints | Description                                  |
|--------------------|---------------|------------------------|----------|-------------|--------------------------|---------------|-------------|----------------------------------------------|
{{{{Repeatedly Insert ""Column Definition Sub-Template"" Here for Each Column}}}}
{{{{Example using the sub-template structure:}}}}

**Indexes:**

| Index Name         | Type      | Columns                               | Unique | Description                                     |
|--------------------|-----------|---------------------------------------|--------|-------------------------------------------------|
| `[Index1Name]`     | `[Type]`  | `[ColumnA]`, `[ColumnB]` (ASC/DESC) | Yes/No | [Purpose of the index]                        |
| ...                | ...       | ...                                   | ...    | ...                                             |

**Constraints (Table-Level):**

| Constraint Name    | Type (PRIMARY KEY, FOREIGN KEY, UNIQUE, CHECK) | Columns / Definition                                  |
|--------------------|----------------------------------------------|-------------------------------------------------------|
| `PK_[TableName]`   | `PRIMARY KEY`                                | `([Column1], [Column2])`                              |
| `FK_[ThisTable]_[RelatedTable]` | `FOREIGN KEY`                             | `([ForeignKeyColumn]) REFERENCES [RelatedTable]([RelatedTablePK])` |
| `UQ_[TableName]_[ColumnName]` | `UNIQUE`                                  | `([UniqueColumn])`                                    |
| `CHK_[TableName]_[Condition]` | `CHECK`                                   | `([LogicalExpression])`                               |
| ...                | ...                                          | ...                                                   |

**Triggers:**

| Trigger Name       | Event (INSERT, UPDATE, DELETE) | Timing (BEFORE, AFTER, INSTEAD OF) | Description                               |
|--------------------|--------------------------------|------------------------------------|-------------------------------------------|
| `[Trigger1Name]`   | `[Event]`                      | `[Timing]`                         | [Purpose and action of the trigger]       |
| ...                | ...                            | ...                                | ...                                       |

**Relationships:**
- **Related To (FKs in this table):**
    - `[ThisTable].[ForeignKeyColumn]` references `[OtherTable].[PrimaryKeyColumn]` ([Description of relationship])
- **Referenced By (FKs in other tables pointing to this table):**
    - `[ReferencingTable].[ForeignKeyColumn]` references `[ThisTable].[PrimaryKeyColumn]` ([Description of relationship])

**Notes:**
[Any additional notes, usage examples, or important considerations for this table.]
---";

        private static string ColumnDefinitionSubTemplate => $@"| `[ColumnName]` | `[DataType]` | `[Length/Precision/Scale]` | `[Nullable]` | `[PrimaryKey]` | `[ForeignKeyReferences]` | `[DefaultValue]` | `[Constraints]` | `[Description]` |";

        private static string IndexDefinitionSubTemplate => $@"| `[IndexName]` | `[Type]` | `[Columns]` | `[Unique]` | `[Description]` |";

        private static string ConstraintDefinitionSubTemplate => $@"| `[ConstraintName]` | `[Type]` | `[Columns/Definition]` |";

        private static string TriggerDefinitionSubTemplate => $@"| `[TriggerName]` | `[Event]` | `[Timing]` | `[Description]` |";

        private static string RelationshipDefinitionSubTemplate => $@"- `[ThisTable].[ForeignKeyColumn]` references `[OtherTable].[PrimaryKeyColumn]` ([Description of relationship])";

        private static string NotesSubTemplate => $@"[Any additional notes, usage examples, or important considerations for this table.]";

        private static string ViewTemplate => $@"# View: `[ObjectName]`

**Description:**
[ObjectDescription]
**Dependencies:**
[List of base tables, other views, or functions that this view depends on.]
- `[SchemaName].[TableNameOrViewName1]`
- `[SchemaName].[FunctionName1]`
- ...

**Columns:**

| Column Name        | Data Type     | Length/Precision/Scale | Nullable | Source / Derivation                       | Description                                  |
|--------------------|---------------|------------------------|----------|-------------------------------------------|----------------------------------------------|
{{{{Repeatedly Insert ""View Column Definition Sub-Template"" Here for Each Column}}}}
{{{{Example using the sub-template structure:}}}}
| `UserFullName`     | `VARCHAR`     | `511`                  | No       | `Users.FirstName + ' ' + Users.LastName`  | Concatenated full name of the user.          |
| `UserEmail`        | `VARCHAR`     | `255`                  | No       | `Users.Email`                             | User's primary email address.                |
| `TotalOrderAmount` | `DECIMAL`     | `12,2`                 | Yes      | `SUM(Orders.OrderValue)` (Grouped by User)| Total monetary value of all orders for the user. Can be NULL if no orders. |
| `LastOrderDate`    | `DATETIME`    | N/A                    | Yes      | `MAX(Orders.OrderDate)` (Grouped by User) | The date of the most recent order placed by the user. |
| `IsActive`         | `BOOLEAN`     | N/A                    | No       | `UserStatus.IsActiveFlag`                 | Indicates if the user's account is currently active. |

**SQL Definition:**
```sql
[Paste the complete CREATE VIEW SQL statement here]
";
    }
}