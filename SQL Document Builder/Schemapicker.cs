
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The schemapicker.
    /// </summary>
    public partial class Schemapicker : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Schemapicker"/> class.
        /// </summary>
        public Schemapicker()
        {
            InitializeComponent();
            _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Schema { get; set; }

        /// <summary>
        /// Handles the click event of the cancel button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the click event of the ok button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (schemaListBox.SelectedIndex >= 0)
            {
                if (schemaListBox.SelectedIndex == 0)
                    Schema = string.Empty;
                else
                    Schema = schemaListBox.Items[schemaListBox.SelectedIndex].ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Populates the schemas.
        /// </summary>
        private async void PopulateSchemas()
        {
            schemaListBox.Items.Clear();
            schemaListBox.Items.Add("(All)");

            var schemas = await SQLDatabaseHelper.GetSchemasAsync(ConnectionString);

            // Add schemas to the combo box
            foreach (var schema in schemas)
            {
                schemaListBox.Items.Add(schema);
            }
        }

        /// <summary>
        /// Handles the load event of the schema picker form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Schemapicker_Load(object sender, EventArgs e)
        {
            PopulateSchemas();
            if (schemaListBox.Items.Count > 0) schemaListBox.SelectedIndex = 0;
        }
    }
}