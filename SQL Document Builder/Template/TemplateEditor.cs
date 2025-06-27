using NPOI.OpenXmlFormats.Vml.Spreadsheet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template editor.
    /// </summary>
    public partial class TemplateEditor : Form
    {
        private readonly Templates _templates = new();
        private Template? _activeTemplate;
        private string _documentType = string.Empty;
        private bool _init = false;
        private TreeNode? _selectedNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateEditor"/> class.
        /// </summary>
        public TemplateEditor()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);
            _templates.Load();
        }

        /// <summary>
        /// Gets or sets the current doc type.
        /// </summary>
        public string CurrentDocType { get; set; } = string.Empty;

        ///// <summary>
        ///// Gets or sets the document type.
        ///// </summary>
        //internal TemplateItem.DocumentTypeEnums DocumentType { private get; set; } = TemplateItem.DocumentTypeEnums.Markdown;

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal TemplateItem.ObjectTypeEnums ObjectType { private get; set; } = TemplateItem.ObjectTypeEnums.Table;

        /// <summary>
        /// Changes the template.
        /// </summary>
        private void ChangeTemplate()
        {
            if (string.IsNullOrWhiteSpace(docTypeToolStripComboBox.Text))
            {
                return; // Prevents changing template if no document type or object type is selected
            }

            // save the current template
            if (templateTextBox.Modified && templateTextBox.Text.Length > 1)
            {
                SaveTemplate();
            }

            // get the selected document type and object type using Enum.TryParse
            _documentType = docTypeToolStripComboBox.Text;
            _activeTemplate = _templates.GetTemplate(_documentType);

            if (_activeTemplate != null)
            {
                templateTreeView.Open(_activeTemplate, _documentType);
            }
            else
            {
                // clear the template tree view if no template is found
                templateTreeView.Nodes.Clear();
                templateTextBox.Text = string.Empty;
                templateTextBox.Visible = false;
            }

            // select the root node in the template tree view
            templateTreeView.SelectedNode = templateTreeView.Nodes[0];
            templateTreeView.Focus();
        }

        /// <summary>
        /// Handles the Click event of the CopyToolStripMenuItem control:
        /// Copies the selected text in the template text box to the clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateTextBox.Copy();
        }

        /// <summary>
        /// Handles the Click event of the CutToolStripMenuItem control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateTextBox.Cut();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the docTypeToolStripComboBox control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DocTypeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // change the sqltext document type based on the selected item
            templateTextBox.DocumentType = docTypeToolStripComboBox.Text.ToLower() switch
            {
                "markdown" => SqlEditBox.DocumentTypeEnums.Markdown,
                "sharepoint" => SqlEditBox.DocumentTypeEnums.Html,
                "wiki" => SqlEditBox.DocumentTypeEnums.Wiki,
                _ => SqlEditBox.DocumentTypeEnums.empty,
            };
            ChangeTemplate();
        }

        /// <summary>
        /// Handles the click event of the ExitToolStripButton
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExitToolStripButton_Click(object sender, EventArgs e)
        {
            // Save the template before closing
            if (templateTextBox.Modified && _selectedNode != null)
            {
                if (_selectedNode is TemplateNode templateNode)
                {
                    // Set the template text box with the selected template body
                    templateNode.TemplateBody = templateTextBox.Text;
                }
                else if (_selectedNode is TemplateElementNode templateElementNode)
                {
                    // Set the template text box with the selected template element body
                    templateElementNode.ElementBody = templateTextBox.Text;
                }
            }

            SaveTemplate();
            Close();
        }

        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void InsertText(string text)
        {
            // retur if the templateTextBox is not visible
            if (!templateTextBox.Visible) return;

            // Insert the object name at the current cursor position in the template text box
            if (templateTextBox.SelectionStart != templateTextBox.SelectionEnd)
            {
                templateTextBox.ReplaceSelection(text);
            }
            else
            {
                templateTextBox.InsertText(templateTextBox.CurrentPosition, text);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ObjTypeToolStripComboBox control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjTypeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTemplate();
        }

        /// <summary>
        /// Handles the Click event of the PasteToolStripMenuItem control:
        /// Pastes text from the clipboard into the template text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateTextBox.Paste();
        }

        /// <summary>
        /// Populates the keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        private void PopulateKeywords((string Keyword, string Description)[] keywords)
        {
            // Assume you have a ToolStripMenuItem named keywordsToolStripMenuItem for the Insert menu
            insertToolStripMenuItem.DropDownItems.Clear();

            foreach (var (keyword, description) in keywords)
            {
                var item = new ToolStripMenuItem($"{keyword}  —  {description}")
                {
                    Tag = keyword
                };
                item.Click += (s, e) => InsertText(keyword);
                insertToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Handles the Click event of the UndoToolStripMenuItem control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateTextBox.Redo();
        }

        /// <summary>
        /// Handles the Click event of the ResetToDefaultToolStripMenuItem control:
        /// Resets the template to the default settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ResetToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_activeTemplate == null || _templates == null)
            {
                return; // Prevents resetting if no active template or templates are loaded
            }

            _init = true;

            bool resetReset = false;
            if (docTypeToolStripComboBox.Text.Equals("Markdown", StringComparison.CurrentCultureIgnoreCase) &&
                _activeTemplate?.DocumentType?.Equals("Markdown", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                // Reset the active template to the default markdown template
                resetReset = _templates.ResetTemplate("Markdown");
            }
            else if (docTypeToolStripComboBox.Text.Equals("SharePoint", StringComparison.CurrentCultureIgnoreCase) &&
                            _activeTemplate?.DocumentType?.Equals("SharePoint", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                // Reset the active template to the default SharePoint template
                resetReset = _templates.ResetTemplate("SharePoint");
            }
            else if (docTypeToolStripComboBox.Text.Equals("Wiki", StringComparison.CurrentCultureIgnoreCase) &&
                            _activeTemplate?.DocumentType?.Equals("wiki", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                // Reset the active template to the default markdown template
                resetReset = _templates.ResetTemplate("Wiki");
            }
            else
            {
                // show a message that the document type is not supported for reset
                MessageBox.Show("The selected document type is not supported for reset.", "Unsupported Document Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (resetReset)
            {
                MessageBox.Show("Template has been reset to the default.", "Reset Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (_activeTemplate != null)
                {
                    templateTreeView.Open(_activeTemplate, _documentType);
                }
                // select the root node in the template tree view
                templateTreeView.SelectedNode = templateTreeView.Nodes[0];
            }
            else
            {
                // Show a message that the reset failed
                MessageBox.Show("Failed to reset the template.", "Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the template.
        /// </summary>
        private void SaveTemplate()
        {
            _templates?.Save();
            templateTextBox.SetSavePoint();
            templateTextBox.EmptyUndoBuffer();
        }

        /// <summary>
        /// Handles the Click event of the SelectAllToolStripMenuItem control:
        /// Selects all text in the template text box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Select all text in the template text box
            templateTextBox.SelectAll();
        }

        /// <summary>
        /// Handles the Load event of the TemplateEditor form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TemplateEditor_Load(object sender, EventArgs e)
        {
            _init = true;

            // Populate DocumentType ToolStripComboBox
            docTypeToolStripComboBox.Items.Clear();

            // list document types in the templates
            foreach (var template in _templates.TemplateLists)
            {
                docTypeToolStripComboBox.Items.Add(template.DocumentType);
            }

            _init = false;

            int selectedIndex = docTypeToolStripComboBox.Items.IndexOf(CurrentDocType);
            if (selectedIndex < 0)
            {
                // If the current document type is not found, default to the first item
                selectedIndex = 0;
            }

            if (docTypeToolStripComboBox.Items.Count > 0)
            {
                docTypeToolStripComboBox.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Templates the tree view_ after select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TemplateTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TemplateNode templateNode)
            {
                // Set the template text box with the selected template body
                templateTextBox.Text = templateNode.Template?.Body ?? string.Empty;
                templateTextBox.SetSavePoint();
                templateTextBox.EmptyUndoBuffer();
                templateTextBox.Visible = true;
            }
            else if (e.Node is TemplateElementNode templateElementNode)
            {
                // Set the template text box with the selected template element body
                templateTextBox.Text = templateElementNode.ElementBody;
                templateTextBox.SetSavePoint();
                templateTextBox.EmptyUndoBuffer();
                templateTextBox.Visible = true;
            }
            else
            {
                // Clear the template text box if no valid node is selected
                templateTextBox.Text = string.Empty;
                templateTextBox.Visible = false;
            }
            _selectedNode = e.Node;

            // populate the keywords menu items based on the selected template
            var keywords = e.Node?.Text.ToUpper() switch
            {
                "TABLE" =>
                [
                    ("~Columns~", "Column definition section"),
                    ("~Constraints~", "Constraints section"),
                    ("~Description~", "Object description"),
                    ("~Indexes~", "Index section"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                    ("~ObjectType~", "Object type"),
                    ("~Relationships~", "Object relationships"),
                    ("~TableValues~", "Table values"),
                    ("~Triggers~", "Table triggers"),
                    ("<!-- SECTION:Columns --><!-- ENDSECTION:Columns -->", "Columns spacehold"),
                    ("<!-- SECTION:Constraints --><!-- ENDSECTION:Constraints -->", "Constraints spacehold"),
                    ("<!-- SECTION:Description --><!-- ENDSECTION:Description -->", "Description spacehold"),
                    ("<!-- SECTION:Indexes --><!-- ENDSECTION:Indexes -->", "Indexes spacehold"),
                    ("<!-- SECTION:Relationships --><!-- ENDSECTION:Relationships -->", "Relationships spacehold"),
                    ("<!-- SECTION:TableValues --><!-- ENDSECTION:TableValues -->", "Table Values spacehold"),
                    ("<!-- SECTION:Triggers --><!-- ENDSECTION:Triggers -->", "Triggers spacehold"),
                ],
                "COLUMNS" =>
                [
                    ("~ColumnItem~", "Column row"),
                ],
                "COLUMN ITEM" =>
                [
                    ("~ColumnDataType~", "Column data type"),
                    ("~ColumnDescription~", "Column description"),
                    ("~ColumnName~", "Column name"),
                    ("~ColumnNullable~", "Column nullable"),
                    ("~ColumnOrd~", "Column ordinal position"),
                ],
                "INDEXES" =>
                [
                    ("~IndexItem~", "Index items"),
                ],
                "INDEX ITEM" =>
                [
                    ("~IndexColumns~", "Index columns"),
                    ("~IndexName~", "Index name"),
                    ("~IndexType~", "Index type"),
                    ("~UniqueIndex~", "Unique index indicator"),
                ],
                "CONSTRAINTS" =>
                [
                    ("~ConstraintItem~", "Constraint item"),
                ],
                "CONSTRAINT ITEM" =>
                [
                    ("~ConstraintColumn~", "Constraint column"),
                    ("~ConstraintName~", "Constraint name"),
                    ("~ConstraintType~", "Constraint type"),
                ],
                "VIEW" =>
                [
                    ("~Columns~", "Column definition section"),
                    ("~Definition~", "Object definition (source code)"),
                    ("~Description~", "Object description"),
                    ("~Indexes~", "Index section"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                    ("~ObjectType~", "Object type"),
                    ("~TableValues~", "Table values"),
                    ("~Triggers~", "Table triggers"),
                    ("<!-- SECTION:Columns --><!-- ENDSECTION:Columns -->", "Columns spacehold"),
                    ("<!-- SECTION:Description --><!-- ENDSECTION:Description -->", "Description spacehold"),
                    ("<!-- SECTION:Indexes --><!-- ENDSECTION:Indexes -->", "Indexes spacehold"),
                    ("<!-- SECTION:TableValues --><!-- ENDSECTION:TableValues -->", "Table Values spacehold"),
                    ("<!-- SECTION:Triggers --><!-- ENDSECTION:Triggers -->", "Triggers spacehold"),
                ],
                "FUNCTION" or "STORED PROCEDURE" =>
                [
                    ("~Definition~", "Object definition (source code)"),
                    ("~Description~", "Object description"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                    ("~ObjectType~", "Object type"),
                    ("~Parameters~", "Parameter section"),
                    ("<!-- SECTION:Description --><!-- ENDSECTION:Description -->", "Description spacehold"),
                    ("<!-- SECTION:Parameters --><!-- ENDSECTION:Parameters -->", "Parameters spacehold"),
                ],
                "PARAMETERS" =>
                [
                    ("~ParameterItem~", "Parameter items"),
                ],
                "PARAMETER ITEM" =>
                [
                    ("~ParameterDataType~", "Parameter data type"),
                    ("~ParameterDescription~", "Parameter description"),
                    ("~ParameterDirection~", "Parameter direction"),
                    ("~ParameterOrd~", "Parameter ordinal position"),
                    ("~ParameterName~", "Parameter name"),
                ],
                "TRIGGER" =>
                [
                    ("~Definition~", "Object definition (source code)"),
                    ("~Description~", "Object description"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                    ("~ObjectType~", "Object type"),
                    ("~TriggerType~", "Trigger type"),
                ],
                "TRIGGERS" =>
                [
                    ("~Definition~", "Object definition (source code)"),
                    ("~Description~", "Object description"),
                    ("~TriggerName~", "Trigger name"),
                    ("~TriggerType~", "Trigger type"),
                ],
                "OBJECT LIST" =>
                [
                    ("~ObjectItem~", "Object list item"),
                ],
                "OBJECT ITEM" =>
                [
                    ("~Description~", "Object description"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                    ("~ObjectType~", "Object type"),
                ],
                "DATA TABLE" =>
                [
                    ("~Align~", "Alignment"),
                    ("~Header~", "Data table header section"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                    ("~Rows~", "Data rows sections"),
                ],
                "DATA TABLE ROW" =>
                [
                    ("~Row~", "Data row"),
                ],
                "DATA TABLE HEADER CELL" =>
                [
                    ("~HeaderCell~", "Data table header cell"),
                ],
                "DATA TABLE CELL" =>
                [
                    ("~Cell~", "Table cell"),
                ],
                "SYNONYM" =>
                [
                    ("~BaseObjectName~", "Base object name"),
                    ("~BaseObjectType~", "Base object type"),
                    ("~ObjectFullName~", "Object full name"),
                    ("~ObjectName~", "Object name"),
                    ("~ObjectSchema~", "Object schema"),
                ],
                "RELATIONSHIPS" =>
                [
                    ("~RelationshipItem~", "Relationship item"),
                ],
                "RELATIONSHIP ITEM" =>
                [
                    ("~ForeignKeyName~", "Foreign key name"),
                    ("~FromSchema~", "Child object schema"),
                    ("~FromTable~", "Child object table"),
                    ("~FromColumn~", "Child object column"),
                    ("~ToSchema~", "Parent object schema"),
                    ("~ToTable~", "Parent object table"),
                    ("~ToColumn~", "Parent object column"),
                    ("~OnDelete~", "On delete action"),
                    ("~OnUpdate~", "On update action"),
                ],
                _ => Array.Empty<(string Keyword, string Description)>() // Default case if no specific keywords are defined
            };
            PopulateKeywords(keywords);
        }

        /// <summary>
        /// Templates the tree view_ before select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TemplateTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            // save the current template before changing selection if it has been modified
            if (templateTextBox.Modified && _selectedNode != null)
            {
                if (_selectedNode is TemplateNode templateNode)
                {
                    // Set the template text box with the selected template body
                    templateNode.TemplateBody = templateTextBox.Text;
                    //_templates.UpdateTemplate(templateNode.Template.DocumentType, templateNode.Template.ObjectType, templateTextBox.Text);
                }
                else if (_selectedNode is TemplateElementNode templateElementNode)
                {
                    // Set the template text box with the selected template element body
                    templateElementNode.ElementBody = templateTextBox.Text;
                    //_templates.UpdateTemplateElement(templateElementNode.Template, templateElementNode.ElementType, templateTextBox.Text);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the UndoToolStripMenuItem control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateTextBox.Undo();
        }

        /// <summary>
        /// Adds the document type tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddDocumentTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // input the new document type name
            string newDocumentType = Common.InputBox("Enter the new document type name:", "Add Document Type", string.Empty);
            // Validate the new document type name
            if (string.IsNullOrWhiteSpace(newDocumentType))
            {
                return;
            }
            // the length of new document type name should be between 3 and 25 characters
            if (newDocumentType.Length < 3 || newDocumentType.Length > 25)
            {
                MessageBox.Show("The document type name must be between 3 and 25 characters long.", "Invalid Document Type Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Check if the document type already exists
            if (_templates.TemplateLists.Any(t => t.DocumentType.Equals(newDocumentType, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("The document type already exists.", "Document Type Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Add the new document type to the templates
            _templates.AddDocumentType(newDocumentType);

            // add the new document type to the combo box
            docTypeToolStripComboBox.Items.Add(newDocumentType);
            docTypeToolStripComboBox.SelectedItem = newDocumentType; // Select the newly added document type
        }

        /// <summary>
        /// Handles the Click event of the ResetSelectedNodeToolStripMenuItem control:
        /// Resets the selected node in the template tree view to its default state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ResetSelectedNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if a node is selected
            if (_selectedNode == null)
            {
                MessageBox.Show("No node selected to reset.", "Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the selected node is the root node
            if (_selectedNode != null && _selectedNode.Parent == null)
            {
                // Reset the entire template
                if (_activeTemplate != null && _templates.ResetTemplate(_activeTemplate.DocumentType))
                {
                    resetToDefaultToolStripMenuItem.PerformClick();
                }
            }
            else
            {
                // Load the default template for the current document type
                var defaultTemplate = new Template(_documentType);
                if (defaultTemplate == null)
                {
                    MessageBox.Show("Default template not found.", "Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!defaultTemplate.Reset(_documentType))
                {
                    defaultTemplate.Load();
                }

                // Find the object type of the selected node
                TemplateItem.ObjectTypeEnums objectType = TemplateItem.ObjectTypeEnums.Table;
                bool typeFound = false;
                if (_selectedNode is TemplateNode templateNode)
                {
                    if (templateNode.Template != null)
                    {
                        objectType = templateNode.Template.ObjectType;
                        typeFound = true;
                    }
                }
                else if (_selectedNode is TemplateElementNode templateElementNode)
                {
                    if (templateElementNode.Template != null)
                    {
                        objectType = templateElementNode.Template.ObjectType;
                        typeFound = true;
                    }
                }

                if (!typeFound)
                {
                    MessageBox.Show("Selected node does not have a valid template object type.", "Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Find the corresponding TemplateItem by object type
                var defaultTemplateItem = defaultTemplate.GetTemplateItem(objectType);

                if (_selectedNode is TemplateNode)
                {
                    // For TemplateNode, set the main Body
                    templateTextBox.Text = defaultTemplateItem?.Body ?? string.Empty;
                }
                else if (_selectedNode is TemplateElementNode templateElementNode)
                {
                    // For TemplateElementNode, set the corresponding ElementBody
                    string elementBody = string.Empty;
                    switch (_selectedNode.Text.ToUpperInvariant())
                    {
                        case "COLUMNS":
                            elementBody = defaultTemplateItem?.Columns?.Body ?? string.Empty;
                            break;
                        case "COLUMN ITEM":
                            elementBody = defaultTemplateItem?.Columns?.ColumnRow ?? string.Empty;
                            break;
                        case "CONSTRAINTS":
                            elementBody = defaultTemplateItem?.Constraints?.Body ?? string.Empty;
                            break;
                        case "CONSTRAINT ITEM":
                            elementBody = defaultTemplateItem?.Constraints?.ConstraintRow ?? string.Empty;
                            break;
                        case "INDEXES":
                            elementBody = defaultTemplateItem?.Indexes?.Body ?? string.Empty;
                            break;
                        case "INDEX ITEM":
                            elementBody = defaultTemplateItem?.Indexes?.IndexRow ?? string.Empty;
                            break;
                        case "PARAMETERS":
                            elementBody = defaultTemplateItem?.Parameters?.Body ?? string.Empty;
                            break;
                        case "PARAMETER ITEM":
                            elementBody = defaultTemplateItem?.Parameters?.ParameterRow ?? string.Empty;
                            break;
                        case "OBJECT LIST":
                            elementBody = defaultTemplateItem?.ObjectLists?.Body ?? string.Empty;
                            break;
                        case "OBJECT ITEM":
                            elementBody = defaultTemplateItem?.ObjectLists?.ObjectRow ?? string.Empty;
                            break;
                        case "DATA TABLE":
                            elementBody = defaultTemplateItem?.DataTable?.Body ?? string.Empty;
                            break;
                        case "DATA TABLE ROW":
                            elementBody = defaultTemplateItem?.DataTable?.DataRow ?? string.Empty;
                            break;
                        case "DATA TABLE HEADER CELL":
                            elementBody = defaultTemplateItem?.DataTable?.HeaderCell ?? string.Empty;
                            break;
                        case "DATA TABLE CELL":
                            elementBody = defaultTemplateItem?.DataTable?.Cell ?? string.Empty;
                            break;
                        case "TRIGGERS":
                            elementBody = defaultTemplateItem?.Triggers ?? string.Empty;
                            break;
                        case "RELATIONSHIPS":
                            elementBody = defaultTemplateItem?.Relationships?.Body ?? string.Empty;
                            break;
                        case "RELATIONSHIP ITEM":
                            elementBody = defaultTemplateItem?.Relationships?.RelationshipRow ?? string.Empty;
                            break;
                        default:
                            elementBody = string.Empty;
                            break;
                    }
                    templateTextBox.Text = elementBody;
                }
            }
        }
    }
}