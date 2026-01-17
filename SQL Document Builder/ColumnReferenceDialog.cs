using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Dialog for selecting a database object (table, view, etc.) and referencing its columns.
    /// </summary>
    public partial class ColumnReferenceDialog : Form
    {
        // Holds all objects fetched for search functionality
        private readonly List<ObjectName> _allObjects = new();

        // Indicates if the dialog is initializing to prevent unwanted event triggers
        private bool _init = false;

        // The list of objects currently displayed (filtered by type/schema/search)
        private List<ObjectName> _objects = new();

        // The currently selected object in the dialog
        private ObjectName? _selectedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnReferenceDialog"/> class and wires up event handlers.
        /// </summary>
        public ColumnReferenceDialog()
        {
            InitializeComponent();
            // Wire up event handlers for UI controls
            schemaComboBox.SelectedIndexChanged += SchemaComboBox_SelectedIndexChanged;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchButton.Click += SearchButton_Click;
            clearSerachButton.Click += ClearSerachButton_Click;
            objectsListBox.SelectedIndexChanged += ObjectsListBox_SelectedIndexChanged;
            okButton.Click += OkButton_Click;
            // Cancel button event handler (if present)
            if (cancelButton != null)
                cancelButton.Click += CancelButton_Click;
        }

        /// <summary>
        /// Gets or sets the database connection item for fetching objects.
        /// </summary>
        public DatabaseConnectionItem? Connection { get; set; }

        /// <summary>
        /// Gets the object selected by the user.
        /// </summary>
        public ObjectName? SelectedObject => _selectedObject;

        /// <summary>
        /// Handles the Cancel button click event. Closes the dialog with Cancel result.
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the Clear Search button click event. Clears the search box and focuses it.
        /// </summary>
        private void ClearSerachButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
            searchTextBox.Focus();
        }

        /// <summary>
        /// Handles the dialog load event. Initializes object type combo box and triggers initial population.
        /// </summary>
        private void ColumnReferenceDialog_Load(object sender, EventArgs e)
        {
            _init = true;
            // Only load tables, no object type selection
            _ = LoadTablesAsync();
            // set focus to search box
            searchTextBox.Focus();
        }

        private async Task LoadTablesAsync()
        {
            if (Connection == null) return;
            string connectionString = Connection.ConnectionString;
            try
            {
                _objects = await SQLDatabaseHelper.GetDatabaseObjectsAsync(ObjectName.ObjectTypeEnums.Table, connectionString) ?? new List<ObjectName>();
                PopulateSchema();
                if (schemaComboBox.Items.Count > 0)
                {
                    schemaComboBox.SelectedIndex = 0;
                }
                if (objectsListBox.Items.Count > 0 && objectsListBox.SelectedItem == null)
                {
                    objectsListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tables: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _init = false;
        }

        /// <summary>
        /// Returns a list of unique schemas from the current object list.
        /// </summary>
        /// <returns>List of schema names.</returns>
        private List<string> GetTableSchemas()
        {
            var dtSchemas = new List<string>();
            foreach (var obj in _objects)
            {
                if (!dtSchemas.Contains(obj.Schema))
                {
                    dtSchemas.Add(obj.Schema);
                }
            }
            return dtSchemas;
        }

        /// <summary>
        /// Handles the selection change in the objects list box. Updates the selected object.
        /// </summary>
        private void ObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem is ObjectName selectedObj)
            {
                _selectedObject = selectedObj;
            }
        }

        /// <summary>
        /// Handles the OK button click event. Sets the selected object and closes the dialog.
        /// </summary>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (objectsListBox.SelectedItem is ObjectName selectedObj)
            {
                _selectedObject = selectedObj;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Populates the objects list box based on the current schema and search criteria.
        /// </summary>
        private async Task PopulateAsync()
        {
            string schemaName = string.Empty;
            if (schemaComboBox.SelectedIndex > 0)
                schemaName = schemaComboBox.Text;
            objectsListBox.Items.Clear();
            string searchFor = searchTextBox.Text.Trim();
            bool useExactMatch = false;
            if (searchFor.IndexOf('.') > 0)
            {
                if (ObjectName.TryParse(searchFor, out ObjectName objName))
                {
                    schemaName = objName.Schema;
                    searchFor = objName.Name;
                    useExactMatch = true;
                }
            }
            if ((searchFor.StartsWith('[') && searchFor.EndsWith(']')) || (searchFor.StartsWith("'") && searchFor.EndsWith("'")))
            {
                useExactMatch = true;
            }
            searchFor = searchFor.Trim('[', ']', '\'');
            foreach (var obj in _objects)
            {
                if (string.IsNullOrEmpty(searchFor))
                {
                    if (string.IsNullOrEmpty(schemaName))
                    {
                        objectsListBox.Items.Add(new ObjectName(obj.ObjectType, obj.Schema, obj.Name));
                    }
                    else if (schemaName.Equals(obj.Schema, StringComparison.CurrentCultureIgnoreCase))
                    {
                        objectsListBox.Items.Add(new ObjectName(obj.ObjectType, obj.Schema, obj.Name));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(schemaName))
                    {
                        if (useExactMatch)
                        {
                            if (obj.Name.Equals(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                objectsListBox.Items.Add(new ObjectName(obj.ObjectType, obj.Schema, obj.Name));
                        }
                        else if (obj.Name.Contains(searchFor, StringComparison.CurrentCultureIgnoreCase))
                        {
                            objectsListBox.Items.Add(new ObjectName(obj.ObjectType, obj.Schema, obj.Name));
                        }
                    }
                    else
                    {
                        if (useExactMatch)
                        {
                            if (obj.Schema.Equals(schemaName, StringComparison.CurrentCultureIgnoreCase) &&
                                obj.Name.Equals(searchFor, StringComparison.CurrentCultureIgnoreCase))
                                objectsListBox.Items.Add(new ObjectName(obj.ObjectType, obj.Schema, obj.Name));
                        }
                        else if (schemaName.Equals(obj.Schema, StringComparison.CurrentCultureIgnoreCase) &&
                                 obj.Name.Contains(searchFor, StringComparison.CurrentCultureIgnoreCase))
                        {
                            objectsListBox.Items.Add(new ObjectName(obj.ObjectType, obj.Schema, obj.Name));
                        }
                    }
                }
            }
            if (objectsListBox.Items.Count > 0)
            {
                if (_selectedObject != null)
                {
                    for (int i = 0; i < objectsListBox.Items.Count; i++)
                    {
                        if (objectsListBox.Items[i] is ObjectName obj && obj.Equals(_selectedObject))
                        {
                            objectsListBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    objectsListBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Populates the schema combo box with available schemas from the current object list.
        /// </summary>
        private void PopulateSchema()
        {
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");
            var schemas = GetTableSchemas();
            schemas.Sort();
            foreach (var item in schemas)
            {
                schemaComboBox.Items.Add(item);
            }
        }

        /// <summary>
        /// Handles the schema combo box selection change. Populates the object list for the selected schema.
        /// </summary>
        private async void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await PopulateAsync();
            if (objectsListBox.Items.Count > 0 && objectsListBox.SelectedItem == null)
            {
                objectsListBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the Search button click event. Attempts to find and select an exact object match.
        /// </summary>
        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchTextBox.Text))
            {
                if (ObjectName.TryParse(searchTextBox.Text.Trim(), out ObjectName searchObj))
                {
                    string schema = searchObj.Schema.Trim('[', ']');
                    string name = searchObj.Name.Trim('[', ']');
                    var matches = _allObjects
                        .Where(obj =>
                            obj.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase) &&
                            obj.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                            obj.ObjectType == ObjectName.ObjectTypeEnums.Table)
                        .ToList();
                    if (matches.Count == 1)
                    {
                        var matched = matches[0];
                        objectsListBox.Items.Clear();
                        objectsListBox.Items.Add(matched);
                        objectsListBox.SelectedIndex = 0;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the search text box text changed event. Triggers async population of the object list.
        /// </summary>
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            _ = PopulateAsync();
        }
    }
}