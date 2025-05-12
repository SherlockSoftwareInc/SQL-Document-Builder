using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The DB objects select form.
    /// </summary>
    public partial class DBObjectsSelectForm : Form
    {
        private readonly List<DatabaseObject> _selectedObjects = [];
        private List<DatabaseObject>? _selectableObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBObjectsSelectForm"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public DBObjectsSelectForm(string connectionString)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the selected database objects.
        /// </summary>
        public List<DatabaseObject>? SelectedObjects
        {
            get
            {
                // build the list of selected objects from the selected list box
                var selectedObjects = new List<DatabaseObject>();
                foreach (DatabaseObject item in selectableListBox.Items)
                {
                    selectedObjects.Add(item);
                }
                return selectedObjects;
            }
        }

        /// <summary>
        /// Gets the stored procedures async.
        /// </summary>
        /// <returns>A Task.</returns>
        private static async Task<List<DatabaseObject>> GetStoredProceduresAsync()
        {
            var objects = new List<DatabaseObject>();

            var query = @"SELECT
    s.name AS SchemaName,
    p.name AS SPName
FROM sys.procedures p
JOIN sys.schemas s ON p.schema_id = s.schema_id
ORDER BY s.name, p.name;";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new DatabaseObject
                {
                    SchemaName = reader["SchemaName"].ToString(),
                    ObjectName = reader["SPName"].ToString(),
                    ObjectType = "SP"
                });
            }

            return objects;
        }

        /// <summary>
        /// Gets the tables async.
        /// </summary>
        /// <returns>A Task.</returns>
        private static async Task<List<DatabaseObject>> GetTablesAsync()
        {
            var objects = new List<DatabaseObject>();

            var query = @"
                SELECT
                    TABLE_SCHEMA AS SchemaName,
                    TABLE_NAME AS ObjectName
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new DatabaseObject
                {
                    SchemaName = reader["SchemaName"].ToString(),
                    ObjectName = reader["ObjectName"].ToString(),
                    ObjectType = "Table"
                });
            }

            return objects;
        }

        /// <summary>
        /// Gets the views async.
        /// </summary>
        /// <returns>A Task.</returns>
        private static async Task<List<DatabaseObject>> GetViewsAsync()
        {
            var objects = new List<DatabaseObject>();

            var query = @"
                SELECT
                    TABLE_SCHEMA AS SchemaName,
                    TABLE_NAME AS ObjectName
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'View'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new DatabaseObject
                {
                    SchemaName = reader["SchemaName"].ToString(),
                    ObjectName = reader["ObjectName"].ToString(),
                    ObjectType = "View"
                });
            }

            return objects;
        }

        /// <summary>
        /// Handles the click event of the add button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddAllButton_Click(object sender, EventArgs e)
        {
            // move all items from the selectable list box to the selected list box
            foreach (DatabaseObject item in selectableListBox.Items)
            {
                if (!IsItemInSelectedList(item))
                {
                    selectedListBox.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds the button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            // move the selected item from the selectable list box to the selected list box
            if (selectableListBox.SelectedItem is DatabaseObject selectedItem)
            {
                if (!IsItemInSelectedList(selectedItem))
                {
                    selectedListBox.Items.Add(selectedItem);
                }
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
        /// Gets the database objects async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<List<DatabaseObject>> GetDatabaseObjectsAsync()
        {
            var objects = objectTypeComboBox.SelectedItem switch
            {
                "Table" => await GetTablesAsync(),
                "View" => await GetViewsAsync(),
                "Stored Procedure" => await GetStoredProceduresAsync(),
                "Function" => await GetFunctionsAsync(),
                _ => throw new NotSupportedException("Unsupported object type."),
            };
            return objects;
        }

        /// <summary>
        /// Gets the functions async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task<List<DatabaseObject>> GetFunctionsAsync()
        {
            var objects = new List<DatabaseObject>();

            var query = @"SELECT
    s.name AS SchemaName,
    o.name AS FunctionName,
    o.type_desc AS FunctionType
FROM sys.objects o
JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE o.type IN ('FN', 'IF', 'TF')
ORDER BY s.name, o.name;";

            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(new DatabaseObject
                {
                    SchemaName = reader["SchemaName"].ToString(),
                    ObjectName = reader["FunctionName"].ToString(),
                    ObjectType = reader["FunctionType"].ToString()
                });
            }

            return objects;
        }

        /// <summary>
        /// Are the item in selected list.
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        /// <returns>A bool.</returns>
        private bool IsItemInSelectedList(DatabaseObject selectedItem)
        {
            // go through the selected list box items
            foreach (DatabaseObject item in selectedListBox.Items)
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
        /// <param name="e">The e.</param>
        private async void ObjectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the database objects based on the selected object type
            _selectableObjects = await GetDatabaseObjectsAsync();

            // populate the selectable objects list box
            PopulateSelectableObjects();
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
        /// Populates the schemas asynchronously.
        /// </summary>
        private async Task PopulateSchemasAsync()
        {
            // Clear existing items in the schema combo box
            schemaComboBox.Items.Clear();
            schemaComboBox.Items.Add("(All)");

            var query = "SELECT name FROM sys.schemas ORDER BY name";
            using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            using var command = new SqlCommand(query, connection);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    schemaComboBox.Items.Add(reader["name"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schemas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await connection.CloseAsync();
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
                if (selectedSchema == "(All)" || obj.SchemaName.Equals(selectedSchema, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if the object name contains the filter text
                    if (string.IsNullOrEmpty(filter) || obj.ObjectName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Add the object to the selectable list box
                        selectableListBox.Items.Add(obj);
                    }
                    else if (obj.ObjectName.Contains(filter, StringComparison.OrdinalIgnoreCase))
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
            if (selectedListBox.SelectedItem is DatabaseObject selectedItem)
            {
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