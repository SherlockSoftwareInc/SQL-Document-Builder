using DarkModeForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SQL_Document_Builder.ObjectName;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The DB objects select form.
    /// </summary>
    public partial class DBObjectsSelectForm : Form
    {
        private List<ObjectName>? _selectableObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBObjectsSelectForm"/> class.
        /// </summary>
        public DBObjectsSelectForm()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
        }

        /// <summary>
        /// Gets the selected database objects.
        /// </summary>
        public List<ObjectName>? SelectedObjects
        {
            get
            {
                // build the list of selected objects from the selected list box
                var selectedObjects = new List<ObjectName>();
                foreach (ObjectName item in selectedListBox.Items)
                {
                    selectedObjects.Add(item);
                }
                return selectedObjects;
            }
        }

        /// <summary>
        /// Handles the click event of the add button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddAllButton_Click(object sender, EventArgs e)
        {
            // move all items from the selectable list box to the selected list box
            foreach (ObjectName item in selectableListBox.Items)
            {
                if (!IsItemInSelectedList(item))
                {
                    selectedListBox.Items.Add(item);
                }
            }
            selectableListBox.Items.Clear();
        }

        /// <summary>
        /// Adds the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            // move the selected item from the selectable list box to the selected list box
            if (selectableListBox.SelectedItem is ObjectName selectedItem)
            {
                if (!IsItemInSelectedList(selectedItem))
                {
                    selectedListBox.Items.Add(selectedItem);
                }
                selectableListBox.Items.Remove(selectedItem);
            }
        }

        /// <summary>
        /// Cancels the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Clears the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            // clear the filter text box
            filterTextBox.Text = string.Empty;
            filterTextBox.Focus();
        }

        /// <summary>
        /// Handles the click event of the copy button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CopyButton_Click(object sender, EventArgs e)
        {
            // Copy the Selected Items To Clipboard
            if (selectedListBox.Items.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in selectedListBox.Items)
                {
                    sb.AppendLine(item.ToString());
                }
                Clipboard.SetText(sb.ToString());
            }
        }

        /// <summary>
        /// Handles the load event of the form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void DBObjectsSelectForm_Load(object sender, EventArgs e)
        {
            try
            {
                await PopulateSchemasAsync();
                objectTypeComboBox.SelectedIndex = 0; // Set default selection

                //_selectableObjects = await GetDatabaseObjectsAsync();

                //PopulateSelectableObjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading database objects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the click event of the select all button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectableObjects != null)
            {
                // populate the selectable objects list box
                PopulateSelectableObjects();
            }
        }

        /// <summary>
        /// Are the item in selected list.
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        /// <returns>A bool.</returns>
        private bool IsItemInSelectedList(ObjectName selectedItem)
        {
            // go through the selected list box items
            foreach (ObjectName item in selectedListBox.Items)
            {
                // check if the item is already in the selected list box
                if (item.Equals(selectedItem))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the selected index changed event of the object type combo box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void ObjectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Determine the selected object type
                var selectedObjectType = objectTypeComboBox.SelectedItem?.ToString() switch
                {
                    "Table" => ObjectTypeEnums.Table,
                    "View" => ObjectTypeEnums.View,
                    "Stored Procedure" => ObjectTypeEnums.StoredProcedure,
                    "Function" => ObjectTypeEnums.Function,
                    _ => ObjectTypeEnums.None
                };

                // If no valid object type is selected, clear the selectable objects and return
                if (selectedObjectType == ObjectTypeEnums.None)
                {
                    //_selectableObjects = null;
                    //PopulateSelectableObjects();
                    return;
                }

                // Retrieve the database objects asynchronously
                _selectableObjects = await DatabaseHelper.GetDatabaseObjectsAsync(selectedObjectType);

                // Populate the selectable objects list box
                PopulateSelectableObjects();
            }
            catch (Exception ex)
            {
                // Display an error message if something goes wrong
                MessageBox.Show($"Error loading database objects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Oks the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Pastes the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PasteButton_Click(object sender, EventArgs e)
        {
            // Paste the items from the clipboard to the selected list box
            string clipboardText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clipboardText))
            {
                string[] items = clipboardText.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

                // set the schema combo box to (All)
                schemaComboBox.SelectedIndex = 0;

                foreach (string item in items)
                {
                    var name = new ObjectName(ObjectTypeEnums.None, item.Trim());

                    // find the item in the selectable list box
                    foreach (ObjectName selectableItem in selectableListBox.Items)
                    {
                        // check if the item is already in the selectable list box
                        if (selectableItem.Schema.Equals(name.Schema, StringComparison.OrdinalIgnoreCase) &&
                            selectableItem.Name.Equals(name.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            selectableListBox.SelectedItem = selectableItem;
                            addButton.PerformClick();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populates the schemas asynchronously.
        /// </summary>
        private async Task PopulateSchemasAsync()
        {
            // Clear existing items in the schema combo box
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");

            var schemas = await DatabaseHelper.GetSchemasAsync();

            // Add schemas to the combo box
            foreach (var schema in schemas)
            {
                schemaComboBox.Items.Add(schema);
            }

            schemaComboBox.SelectedIndex = 0; // Set default selection
        }

        /// <summary>
        /// Populates the selectable objects.
        /// </summary>
        private void PopulateSelectableObjects()
        {
            // Clear existing items in the selectable list box
            selectableListBox.Items.Clear();

            // check the _selectableObjects
            if (_selectableObjects == null || _selectableObjects.Count == 0)
            {
                return;
            }

            // Get the selected schema
            var selectedSchema = schemaComboBox.SelectedItem?.ToString() ?? string.Empty;

            // get the filter
            string filter = filterTextBox.Text.Trim();

            // Filter the objects based on the selected schema
            foreach (var obj in _selectableObjects)
            {
                if (selectedSchema == "(All)" || obj.Schema.Equals(selectedSchema, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if the object name contains the filter text
                    if (string.IsNullOrEmpty(filter) || obj.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Add the object to the selectable list box
                        selectableListBox.Items.Add(obj);
                    }
                    else if (obj.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        // Add the object to the selectable list box
                        selectableListBox.Items.Add(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the all button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RemoveAllButton_Click(object sender, EventArgs e)
        {
            // add all items from the selected list box to the selectable list box if they are not already there
            foreach (ObjectName item in selectedListBox.Items)
            {
                if (!selectableListBox.Items.Contains(item))
                {
                    selectableListBox.Items.Add(item);
                }
            }

            // move all items from the selected list box to the selectable list box
            selectedListBox.Items.Clear();
        }

        /// <summary>
        /// Removes the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            // move the selected item from the selected list box
            if (selectedListBox.SelectedItem is ObjectName selectedItem)
            {
                // add the item to the selectable list box
                selectableListBox.Items.Add(selectedItem);

                // remove the item from the selected list box
                selectedListBox.Items.Remove(selectedItem);
            }
        }

        /// <summary>
        /// Handles the selected index changed event of the schema combo box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectableObjects != null)
            {
                // populate the selectable objects list box
                PopulateSelectableObjects();
            }
        }

        /// <summary>
        /// Handles the double click event of the selectable list box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectableListBox_DoubleClick(object sender, EventArgs e)
        {
            addButton.PerformClick();
        }

        /// <summary>
        /// Handles the double click event of the selected list box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectedListBox_DoubleClick(object sender, EventArgs e)
        {
            removeButton.PerformClick();
        }
    }
}