using System;
using System.Windows.Forms;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template tree.
    /// </summary>
    internal class TemplateTree : TreeView
    {
        private TreeNode? _root;
        private Template? _template;
        private string? _documentType;

        /// <summary>
        /// Opens the.
        /// </summary>
        /// <param name="templates">The templates.</param>
        /// <param name="documentType">The document type.</param>
        internal void Open(Template template, string documentType)
        {
            Nodes.Clear();
            _template = template;
            _documentType = documentType;

            _root = new TreeNode(documentType);
            Nodes.Add(_root);

            Populate();

            // Expand the root node
            _root.ExpandAll();
        }

        /// <summary>
        /// Populates the.
        /// </summary>
        private void Populate()
        {
            if (_template == null || _root == null)
            {
                return;
            }

            foreach (var templateItem in _template.TemplateLists)
            {
                var node = new TemplateNode(templateItem)
                {
                    Text = templateItem.ObjectType switch
                    {
                        TemplateItem.ObjectTypeEnums.Table => "Table",
                        TemplateItem.ObjectTypeEnums.View => "View",
                        TemplateItem.ObjectTypeEnums.StoredProcedure => "Stored Procedure",
                        TemplateItem.ObjectTypeEnums.Function => "Function",
                        TemplateItem.ObjectTypeEnums.Trigger => "Trigger",
                        TemplateItem.ObjectTypeEnums.ObjectList => "Object List",
                        TemplateItem.ObjectTypeEnums.DataTable => "Data Table",
                        TemplateItem.ObjectTypeEnums.Synonym => "Synonym",
                        _ => throw new ArgumentOutOfRangeException(nameof(templateItem.ObjectType), "Unknown template object type.")
                    },
                    ImageIndex = 0,
                    SelectedImageIndex = 0
                };
                _root.Nodes.Add(node);
            }
        }
    }
}