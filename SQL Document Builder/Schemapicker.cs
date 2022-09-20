using System;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class Schemapicker : Form
    {
        public Schemapicker()
        {
            InitializeComponent();
        }

        public string? Schema { get; set; }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

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

        private void PopulateSchemas()
        {
            schemaListBox.Items.Clear();
            schemaListBox.Items.Add("(All)");

            try
            {
                using var conn = new OdbcConnection(Properties.Settings.Default.dbConnectionString);
                conn.Open();
                DataTable schemaTable = conn.GetSchema(System.Data.OleDb.OleDbMetaDataCollectionNames.Tables);
                if (schemaTable.Rows.Count > 0)
                {
                    var dtSchemas = schemaTable.DefaultView.ToTable(true, "TABLE_SCHEM");
                    foreach (DataRow dr in dtSchemas.Rows)
                    {
                        schemaListBox.Items.Add((string)dr[0]);
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Schemapicker_Load(object sender, EventArgs e)
        {
            PopulateSchemas();
            if (schemaListBox.Items.Count > 0) schemaListBox.SelectedIndex = 0;
        }
    }
}