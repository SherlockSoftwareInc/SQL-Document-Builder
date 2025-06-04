using DarkModeForms;
using System;
using System.Windows.Forms;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template editor.
    /// </summary>
    public partial class TemplateEditor : Form
    {
        private readonly Templates _templates = new();
        private TemplateItem.DocumentTypeEnums _documentType = TemplateItem.DocumentTypeEnums.Markdown;
        private bool _init = false;
        private TemplateItem.ObjectTypeEnums _objectType = TemplateItem.ObjectTypeEnums.Table;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateEditor"/> class.
        /// </summary>
        public TemplateEditor()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
            _templates.Load();
        }

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        internal TemplateItem.DocumentTypeEnums DocumentType { private get; set; } = TemplateItem.DocumentTypeEnums.Markdown;

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        internal TemplateItem.ObjectTypeEnums ObjectType { private get; set; } = TemplateItem.ObjectTypeEnums.Table;

        /// <summary>
        /// Changes the template.
        /// </summary>
        private void ChangeTemplate()
        {
            // save the current template
            if (templateTextBox.Modified && templateTextBox.Text.Length > 1)
            {
                SaveTemplate();
            }

            // get the selected document type and object type using Enum.TryParse
            if (Enum.TryParse(docTypeToolStripComboBox.Text, out TemplateItem.DocumentTypeEnums docType))
                _documentType = docType;
            if (Enum.TryParse(objTypeToolStripComboBox.Text, out TemplateItem.ObjectTypeEnums objType))
                _objectType = objType;

            templateTextBox.Text = _templates?.GetTemplate(_documentType, _objectType)?.Body;
            templateTextBox.SetSavePoint();
            templateTextBox.EmptyUndoBuffer();
        }

        /// <summary>
        /// Handles the Click event of the ColumnsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Columns~");
        }

        /// <summary>
        /// Handles the Click event of the ForeignKeysToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ConstraintsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Constraints~");
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
            SaveTemplate();
            Close();
        }

        /// <summary>
        /// Handles the Click event of the IndexesToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void IndexesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Indexes~");
        }

        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void InsertText(string text)
        {
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
        /// Handles the Click event of the ObjectDefinitionToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjectDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Definition~");
        }

        /// <summary>
        /// handles the Click event of the ObjectDescriptionToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjectDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Description~");
        }

        /// <summary>
        /// Handles the Click event of the ObjectFullNameToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjectFullNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~ObjectFullName~");
        }

        /// <summary>
        /// Handles the Click event of the ObjectNameToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjectNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~ObjectName~");
        }

        /// <summary>
        /// Handles the Click event of the ObjectSchemaToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjectSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~ObjectSchema~");
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ObjTypeToolStripComboBox control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ObjTypeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTemplate();

            // enable/disable the ObjectType ToolStripComboBox based on the selected DocumentType
            if (_init) return; // Prevents re-entrancy during form load

            objectDefinitionToolStripMenuItem.Enabled = false;
            columnsToolStripMenuItem.Enabled = false;
            constraintsToolStripMenuItem.Enabled = false;
            indexesToolStripMenuItem.Enabled = false;
            relationshipsToolStripMenuItem.Enabled = false;
            parametersToolStripMenuItem.Enabled = false;

            if (objTypeToolStripComboBox.SelectedItem is TemplateItem.ObjectTypeEnums objType)
            {
                switch (objType)
                {
                    case TemplateItem.ObjectTypeEnums.Table:
                        columnsToolStripMenuItem.Enabled = true;
                        constraintsToolStripMenuItem.Enabled = true;
                        indexesToolStripMenuItem.Enabled = true;
                        relationshipsToolStripMenuItem.Enabled = true;
                        break;

                    case TemplateItem.ObjectTypeEnums.View:
                        objectDefinitionToolStripMenuItem.Enabled = true;
                        columnsToolStripMenuItem.Enabled = true;
                        indexesToolStripMenuItem.Enabled = true;
                        relationshipsToolStripMenuItem.Enabled = true;
                        break;

                    case TemplateItem.ObjectTypeEnums.Function:
                    case TemplateItem.ObjectTypeEnums.StoredProcedure:
                        objectDefinitionToolStripMenuItem.Enabled = true;
                        parametersToolStripMenuItem.Enabled = true;
                        break;

                    case TemplateItem.ObjectTypeEnums.Trigger:
                        objectDefinitionToolStripMenuItem.Enabled = true;
                        break;

                    default:
                        objectDefinitionToolStripMenuItem.Enabled = true;
                        columnsToolStripMenuItem.Enabled = true;
                        constraintsToolStripMenuItem.Enabled = true;
                        indexesToolStripMenuItem.Enabled = true;
                        relationshipsToolStripMenuItem.Enabled = true;
                        parametersToolStripMenuItem.Enabled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ParametersToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Parameters~");
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
        /// Handles the Click event of the UndoToolStripMenuItem control:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateTextBox.Redo();
        }

        /// <summary>
        /// Handles the Click event of the RelationshipsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RelationshipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Relationships~");
        }

        /// <summary>
        /// Saves the template.
        /// </summary>
        private void SaveTemplate()
        {
            if (templateTextBox.Modified)
            {
                _templates?.UpdateTemplate(_documentType, _objectType, templateTextBox.Text);
            }
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
            foreach (var value in Enum.GetValues(typeof(TemplateItem.DocumentTypeEnums)))
            {
                docTypeToolStripComboBox.Items.Add(value);
            }
            // Set selected item using DocumentType property
            docTypeToolStripComboBox.SelectedItem = DocumentType;
            _documentType = DocumentType;

            // Populate ObjectType ToolStripComboBox
            objTypeToolStripComboBox.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(TemplateItem.ObjectTypeEnums)))
            {
                objTypeToolStripComboBox.Items.Add(value);
            }

            _init = false;
            // Set selected item using ObjectType property
            objTypeToolStripComboBox.SelectedItem = ObjectType;
            _objectType = ObjectType;
        }

        /// <summary>
        /// Handles the Click event of the TriggersToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TriggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertText("~Triggers~");
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

        private void objTypeToolStripComboBox_Click(object sender, EventArgs e)
        {
        }
    }
}