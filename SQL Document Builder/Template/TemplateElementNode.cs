using System;
using System.Windows.Forms;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The column template node.
    /// </summary>
    internal class TemplateElementNode : TreeNode
    {
        private TemplateElementType _elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateElementNode"/> class.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        public TemplateElementNode(TemplateItem templateItem, TemplateElementType elementType)
        {
            Template = templateItem ?? throw new ArgumentNullException(nameof(templateItem));
            _elementType = elementType;

            switch (elementType)
            {
                case TemplateElementType.Columns:
                    Text = "Columns";
                    ToolTipText = "Template for columns";

                    // add the column row template node
                    Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.ColumnRow));
                    break;

                case TemplateElementType.ColumnRow:
                    Text = "Column Item";
                    ToolTipText = "Template for each column row";
                    break;

                case TemplateElementType.Indexes:
                    Text = "Indexes";
                    ToolTipText = "Indexes template";

                    // add the index row template node
                    Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.IndexRow));
                    break;

                case TemplateElementType.IndexRow:
                    Text = "Index Item";
                    ToolTipText = "Template for each index row";
                    break;

                case TemplateElementType.Constraints:
                    Text = "Constraints";
                    ToolTipText = "Constraints template";

                    // add the constraint row template node
                    Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.ConstraintRow));
                    break;

                case TemplateElementType.ConstraintRow:
                    Text = "Constraint Item";
                    ToolTipText = "Template for each constraint row";
                    break;

                case TemplateElementType.Parameters:
                    Text = "Parameters";
                    ToolTipText = "Parameters template";

                    // add the parameter row template node
                    Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.ParameterRow));
                    break;

                case TemplateElementType.ParameterRow:
                    Text = "Parameter Item";
                    ToolTipText = "Template for each parameter row";
                    break;

                case TemplateElementType.Triggers:
                    Text = "Triggers";
                    ToolTipText = "Triggers template";
                    break;

                case TemplateElementType.ObjectList:
                    Text = "Object List";
                    ToolTipText = "Object list template (e.g., for views)";
                    // add the object row template node
                    Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.ObjectListRow));
                    break;

                case TemplateElementType.ObjectListRow:
                    Text = "Object Item";
                    ToolTipText = "Template for each object item";
                    break;

                case TemplateElementType.DataTable:
                    Text = "Data Table";
                    ToolTipText = "Data table template (e.g., for value list of data table/view or query results )";
                    break;

                case TemplateElementType.DataTableRow:
                    Text = "Data Table Row";
                    ToolTipText = "Template for each data table row item (e.g., for value list of data table/view or query results)";
                    break;

                case TemplateElementType.DataTableHeaderCell:
                    Text = "Data Table Header Cell";
                    ToolTipText = "Template for each data table header cell (e.g., for value list of data table/view or query results)";
                    break;

                case TemplateElementType.DataTableCell:
                    Text = "Data Table Cell";
                    ToolTipText = "Template for each data table cell (e.g., for value list of data table/view or query results)";
                    break;

                case TemplateElementType.Relationships:
                    Text = "Relationships";
                    ToolTipText = "Relationships template";
                    // add the relationship row template node
                    Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.RelationshipRow));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(elementType), elementType, null);
            }
        }

        /// <summary>
        /// The template element type.
        /// </summary>
        public enum TemplateElementType
        {
            Columns,
            ColumnRow,
            Indexes,
            IndexRow,
            Constraints,
            ConstraintRow,
            Parameters,
            ParameterRow,
            Triggers,
            ObjectList,
            ObjectListRow,
            DataTable,
            DataTableRow,
            DataTableHeaderCell,
            DataTableCell,
            Relationships,
            RelationshipRow
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string ElementBody
        {
            get
            {
                return _elementType switch
                {
                    TemplateElementType.Columns => Template.Columns?.Body ?? "",
                    TemplateElementType.ColumnRow => Template.Columns?.ColumnRow ?? "",
                    TemplateElementType.Indexes => Template.Indexes?.Body ?? "",
                    TemplateElementType.IndexRow => Template.Indexes?.IndexRow ?? "",
                    TemplateElementType.Constraints => Template.Constraints?.Body ?? "",
                    TemplateElementType.ConstraintRow => Template.Constraints?.ConstraintRow ?? "",
                    TemplateElementType.Parameters => Template.Parameters?.Body ?? "",
                    TemplateElementType.ParameterRow => Template.Parameters?.ParameterRow ?? "",
                    TemplateElementType.ObjectList => Template.ObjectLists?.Body ?? "",
                    TemplateElementType.ObjectListRow => Template.ObjectLists?.ObjectRow ?? "",
                    TemplateElementType.DataTable => Template.DataTable.Body ?? "",
                    TemplateElementType.DataTableRow => Template.DataTable.DataRow ?? "",
                    TemplateElementType.DataTableHeaderCell => Template.DataTable.HeaderCell ?? "",
                    TemplateElementType.DataTableCell => Template.DataTable.Cell ?? "",
                    TemplateElementType.Triggers => Template.Triggers ?? "",
                    TemplateElementType.Relationships => Template.Relationships?.Body ?? "",
                    TemplateElementType.RelationshipRow => Template.Relationships?.RelationshipRow ?? "",
                    _ => "",
                };
            }
            set
            {
                if (Template.Columns == null)
                    Template.Columns = new ColumnTemplate();

                switch (_elementType)
                {
                    case TemplateElementType.Columns:
                        if (Template.Columns != null)
                            Template.Columns.Body = value ?? "";
                        break;

                    case TemplateElementType.ColumnRow:
                        if (Template.Columns != null)
                            Template.Columns.ColumnRow = value ?? "";
                        break;

                    case TemplateElementType.Indexes:
                        if (Template.Indexes != null)
                            Template.Indexes.Body = value ?? "";
                        break;

                    case TemplateElementType.IndexRow:
                        if (Template.Indexes != null)
                            Template.Indexes.IndexRow = value ?? "";
                        break;

                    case TemplateElementType.Constraints:
                        if (Template.Constraints != null)
                            Template.Constraints.Body = value ?? "";
                        break;

                    case TemplateElementType.ConstraintRow:
                        if (Template.Constraints != null)
                            Template.Constraints.ConstraintRow = value ?? "";
                        break;

                    case TemplateElementType.Parameters:
                        if (Template.Parameters != null)
                            Template.Parameters.Body = value ?? "";
                        break;

                    case TemplateElementType.ParameterRow:
                        if (Template.Parameters != null)
                            Template.Parameters.ParameterRow = value ?? "";
                        break;

                    case TemplateElementType.ObjectList:
                        if (Template.ObjectLists != null)
                            Template.ObjectLists.Body = value ?? "";
                        break;

                    case TemplateElementType.ObjectListRow:
                        if (Template.ObjectLists != null)
                            Template.ObjectLists.ObjectRow = value ?? "";
                        break;

                    case TemplateElementType.DataTable:
                        if (Template.DataTable != null)
                            Template.DataTable.Body = value ?? "";
                        break;

                    case TemplateElementType.DataTableRow:
                        if (Template.DataTable != null)
                            Template.DataTable.DataRow = value ?? "";
                        break;

                    case TemplateElementType.DataTableHeaderCell:
                        if (Template.DataTable != null)
                            Template.DataTable.HeaderCell = value ?? "";
                        break;

                    case TemplateElementType.DataTableCell:
                        if (Template.DataTable != null)
                            Template.DataTable.Cell = value ?? "";
                        break;

                    case TemplateElementType.Triggers:
                        Template.Triggers = value ?? "";
                        break;

                    case TemplateElementType.Relationships:
                        if (Template.Relationships != null)
                            Template.Relationships.Body = value ?? "";
                        break;

                    case TemplateElementType.RelationshipRow:
                        if (Template.Relationships != null)
                            Template.Relationships.RelationshipRow = value ?? "";
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        public TemplateItem Template { get; private set; }
    }
}