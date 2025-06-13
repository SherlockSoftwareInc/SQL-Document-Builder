using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The DB object def panel.
    /// </summary>
    public partial class DBObjectDefPanel : UserControl
    {
        /// <summary>
        /// The desc changed.
        /// </summary>
        private bool _descChanged = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBObjectDefPanel"/> class.
        /// </summary>
        public DBObjectDefPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Object name to open
        /// </summary>
        private ObjectName? TableName { get; set; }

        /// <summary>
        /// Perform copy
        /// </summary>
        public void Copy()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl?.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)focusedControl;
                textBox.Copy();
            }
            else if (focusedControl?.GetType() == typeof(ColumnDefView))
            {
                ColumnDefView defView = (ColumnDefView)focusedControl;
                defView.Copy();
            }
        }

        /// <summary>
        /// Perform cut
        /// </summary>
        public void Cut()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is ColumnDefView defView)
            {
                defView.Cut();
                return;
            }
            else if (focusedControl is TextBox textBox)
            {
                textBox.Cut();
                return;
            }
        }

        /// <summary>
        /// Open a database object
        /// </summary>
        /// <param name="tableName">The table name.</param>
        public async Task OpenAsync(ObjectName? tableName, string connectionString)
        {
            await SaveChangeAsync();
            TableName = tableName;
            columnView.ConnectionString = connectionString;
            await columnView.OpenAsync(TableName, connectionString);
        }

        /// <summary>
        /// Perform paste
        /// </summary>
        public void Paste()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is ColumnDefView defView)
            {
                defView.Paste();
                return;
            }
            else if (focusedControl is TextBox textBox)
            {
                textBox.Paste();
                return;
            }
        }

        /// <summary>
        /// Perform Select all.
        /// </summary>
        public void SelectAll()
        {
            Control? focusedControl = GetFocusedControl(this);
            if (focusedControl == null) return;

            if (focusedControl is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (focusedControl is ColumnDefView defView)
            {
                defView.SelectAll();
            }
        }

        /// <summary>
        /// Recursively gets the currently focused control within a container.
        /// </summary>
        /// <param name="container">The container control.</param>
        /// <returns>The focused control, or null if none is focused.</returns>
        private static Control? GetFocusedControl(Control container)
        {
            if (container == null)
                return null;

            if (container.Focused)
                return container;

            foreach (Control child in container.Controls)
            {
                Control? focused = GetFocusedControl(child);
                if (focused != null)
                    return focused;
            }

            return null;
        }

        /// <summary>
        /// Columns the def view1 selected column changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void ColumnDefView1_SelectedColumnChanged(object sender, EventArgs e)
        {
            await SaveChangeAsync();

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
        /// Columns the view table desc selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void ColumnView_TableDescSelected(object sender, EventArgs e)
        {
            titleLabel.Text = (columnView.TableType == ObjectName.ObjectTypeEnums.View ? "View: " : "Table: ") + columnView.TableFullName;
            columnNameLabel.Text = string.Empty;
            descTextBox.Text = columnView.TableDescription;
            _descChanged = false;
        }

        /// <summary>
        /// DB the object def panel size changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void DBObjectDefPanel_SizeChanged(object sender, EventArgs e)
        {
            if (panel1.Width > 34) descTextBox.Width = panel1.Width - 24;
        }

        /// <summary>
        /// Descs the edit form form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void DescEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await SaveChangeAsync();
        }

        /// <summary>
        /// Descs the text box text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private void DescTextBox_TextChanged(object sender, EventArgs e)
        {
            _descChanged = true;
        }

        /// <summary>
        /// Descs the text box validated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The E.</param>
        private async void DescTextBox_Validated(object sender, EventArgs e)
        {
            await SaveChangeAsync();
            //columnView.Open(TableName);
        }

        /// <summary>
        /// Save the changes.
        /// </summary>
        private async Task SaveChangeAsync()
        {
            if (_descChanged && columnView?.TableName?.Length > 0)
            {
                if (columnNameLabel.Text.Length > 0)
                {
                    await columnView.UpdateColumnDescAsync(columnNameLabel.Text, descTextBox.Text);
                }
                else
                {
                    await columnView.UpdateTableDescriptionAsync(descTextBox.Text);
                }
                _descChanged = false;
            }
        }
    }
}