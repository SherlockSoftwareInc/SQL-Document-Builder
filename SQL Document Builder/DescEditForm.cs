using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class DescEditForm : Form
    {
        private bool _descChanged = false;

        public DescEditForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Object name to open
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectName? TableName { get; set; }

        /// <summary>
        /// Handles column view selected column change event:
        ///     1. Save changes
        ///     2. Show selected column name and description in the edit panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnDefView1_SelectedColumnChanged(object sender, EventArgs e)
        {
            SaveChange();

            string? selectedColumn = columnView.SelectedColumn;
            if (selectedColumn != null)
            {
                if (selectedColumn.Length == 0)
                {
                    descTextBox.Text = string.Empty;
                }
                else
                {
                    titleLabel.Text = "Column:";
                    columnNameLabel.Text = selectedColumn;
                    descTextBox.Text = columnView.ColumnDescription(selectedColumn);
                    _descChanged = false;
                }
            }
        }

        /// <summary>
        /// Handles form closing event: Ensure change has been saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveChange();
        }

        /// <summary>
        /// Handles form load event: Open the object in the column view control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescEditForm_Load(object sender, EventArgs e)
        {
            columnView.ConnectionString = Properties.Settings.Default.dbConnectionString;
            columnView.Open(TableName);
        }

        /// <summary>
        /// Handles description text box text change event: set change indicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescTextBox_TextChanged(object sender, EventArgs e)
        {
            _descChanged = true;
        }

        /// <summary>
        /// Handles description text box validated event: Save the change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescTextBox_Validated(object sender, EventArgs e)
        {
            SaveChange();
        }

        /// <summary>
        /// Save the change
        /// </summary>
        private void SaveChange()
        {
            if (_descChanged && columnView?.TableName?.Length > 0)
            {
                if (columnNameLabel.Text.Length > 0)
                {
                    columnView.UpdateColumnDesc(columnNameLabel.Text, descTextBox.Text, columnView.TableType == ObjectName.ObjectTypeEnums.View);
                }
                else
                {
                    columnView.UpdateTableDescription(descTextBox.Text);
                }
                _descChanged = false;
            }
        }

        private void ColumnView_TableDescSelected(object sender, EventArgs e)
        {
            titleLabel.Text = (columnView.TableType == ObjectName.ObjectTypeEnums.View ? "View: " : "Table: ") + columnView.TableFullName;
            columnNameLabel.Text = string.Empty;
            descTextBox.Text = columnView.TableDescription;
            _descChanged = false;
        }
    }
}
