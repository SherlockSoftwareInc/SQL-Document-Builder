using System;
using System.Windows.Forms;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template node.
    /// </summary>
    internal class TemplateNode : TreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateNode"/> class.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        public TemplateNode(TemplateItem templateItem)
        {
            Template = templateItem ?? throw new ArgumentNullException(nameof(templateItem));

            switch (templateItem.ObjectType)
            {
                case TemplateItem.ObjectTypeEnums.Table:
                    Text = "Table";
                    PopulateTableNodes(templateItem);
                    break;

                case TemplateItem.ObjectTypeEnums.View:
                    Text = "View";
                    PopulateViewNodes(templateItem);
                    break;

                case TemplateItem.ObjectTypeEnums.StoredProcedure:
                    Text = "Stored Procedure";
                    PopulateProcedureNodes(templateItem);
                    break;

                case TemplateItem.ObjectTypeEnums.Function:
                    Text = "Function";
                    PopulateFunctionNodes(templateItem);
                    break;

                case TemplateItem.ObjectTypeEnums.Trigger:
                    Text = "Trigger";
                    PopulateTriggerNodes(templateItem);
                    break;

                case TemplateItem.ObjectTypeEnums.ObjectList:
                    Text = "Object List";
                    PopulateObjectItemNodes(templateItem);
                    break;

                case TemplateItem.ObjectTypeEnums.DataTable:
                    Text = "Data Table";
                    PopulateDataTableRowNodes(templateItem);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(templateItem.ObjectType), "Unknown template object type.");
            }
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        public TemplateItem Template { get; set; }

        /// <summary>
        /// Gets or sets the template body.
        /// </summary>
        public string TemplateBody
        {
            get => Template.Body ?? string.Empty;
            set => Template.Body = value ?? string.Empty;
        }

        /// <summary>
        /// Tos the string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return (Template.ObjectType) switch
            {
                TemplateItem.ObjectTypeEnums.Table => "Table",
                TemplateItem.ObjectTypeEnums.View => "View",
                TemplateItem.ObjectTypeEnums.StoredProcedure => "Stored Procedure",
                TemplateItem.ObjectTypeEnums.Function => "Function",
                TemplateItem.ObjectTypeEnums.Trigger => "Trigger",
                TemplateItem.ObjectTypeEnums.ObjectList => "Object List",
                TemplateItem.ObjectTypeEnums.DataTable => "Data Table",
                _ => base.ToString(),
            };
        }

        /// <summary>
        /// Populates the data table row nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateDataTableRowNodes(TemplateItem templateItem)
        {
            // add the table row node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.DataTableRow));
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.DataTableHeaderCell));
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.DataTableCell));
        }

        /// <summary>
        /// Populates the function nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateFunctionNodes(TemplateItem templateItem)
        {
            // add the parameters node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Parameters));
        }

        /// <summary>
        /// Populates the trigger nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateObjectItemNodes(TemplateItem templateItem)
        {
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.ObjectListRow));
        }

        /// <summary>
        /// Populates the procedure nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateProcedureNodes(TemplateItem templateItem)
        {
            // add the parameters node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Parameters));
        }

        /// <summary>
        /// Populates the sub-nodes for the table nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateTableNodes(TemplateItem templateItem)
        {
            // add the columns node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Columns));

            // add the indexes node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Indexes));

            // add the constraints node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Constraints));
        }

        /// <summary>
        /// Populates the trigger nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateTriggerNodes(TemplateItem templateItem)
        {
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Triggers));
        }

        /// <summary>
        /// Populates the view nodes.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        private void PopulateViewNodes(TemplateItem templateItem)
        {
            // add the columns node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Columns));

            // add the indexes node
            this.Nodes.Add(new TemplateElementNode(templateItem, TemplateElementNode.TemplateElementType.Indexes));
        }
    }
}