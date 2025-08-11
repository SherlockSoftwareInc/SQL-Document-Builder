using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The recent files manage form.
    /// </summary>
    public partial class RecentFilesManageForm : Form
    {
        private List<FileInfo> recentFiles = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentFilesManageForm"/> class.
        /// </summary>
        public RecentFilesManageForm()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets the file to open.
        /// </summary>
        public string? FileToOpen { get; private set; } = string.Empty;

        /// <summary>
        /// Handles the Click event of the closeToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the deleteFileToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DeleteFileToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedFile = GetSelectedFile();

            if (selectedFile == null)
                return;

            // Show confirmation dialog
            var result = MessageBox.Show(
                $"Are you sure you want to delete the file:\n\n{selectedFile}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            try
            {
                MostRecentUsedFiles mruFiles = new();
                mruFiles.Load();
                mruFiles.Remove(selectedFile);

                // Load the recent files into the local variable
                LoadMRUFiles();

                // Refresh the display
                PopulateFiles();

                MessageBox.Show("File deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Files the data grid view_ cell double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void FilesDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedFile = GetSelectedFile();
            if (selectedFile != null)
            {
                // perform the click action as if the open button was clicked
                openToolStripButton.PerformClick();
            }
        }

        /// <summary>
        /// Gets the selected file.
        /// </summary>
        /// <returns>A string? .</returns>
        private string? GetSelectedFile()
        {
            // get the file name and path from the first selected row
            if (filesDataGridView.SelectedCells.Count > 0)
            {
                int selectedRowIndex = filesDataGridView.SelectedCells[0].RowIndex;
                if (selectedRowIndex >= 0 && selectedRowIndex < recentFiles.Count)
                {
                    var selectedRow = filesDataGridView.Rows[selectedRowIndex];
                    var fileName = selectedRow.Cells["File Name"].Value?.ToString() ?? string.Empty;
                    var filePath = selectedRow.Cells["Path"].Value?.ToString() ?? string.Empty;
                    var fileExtension = selectedRow.Cells["Extension"].Value?.ToString() ?? string.Empty;

                    return System.IO.Path.Combine(filePath, fileName + fileExtension);
                }
            }
            return null;
        }

        /// <summary>
        /// Loads the m r u files.
        /// </summary>
        private void LoadMRUFiles()
        {
            recentFiles.Clear();
            MostRecentUsedFiles mruFiles = new();
            mruFiles.Load();
            foreach (string filePath in mruFiles.Files)
            {
                FileInfo fi = new()
                {
                    FileName = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    FilePath = System.IO.Path.GetDirectoryName(filePath) ?? string.Empty,
                    FileExtension = System.IO.Path.GetExtension(filePath),
                    LastUpdateTime = System.IO.File.Exists(filePath)
                        ? System.IO.File.GetLastWriteTime(filePath)
                        : DateTime.MinValue
                };
                recentFiles.Add(fi);
            }
        }

        /// <summary>
        /// Handles the Click event of the moveFileToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void MoveFileToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedFile = GetSelectedFile();

            if (selectedFile == null)
                return;

            // Show a folder browser dialog to select the new location
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the destination folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Use the file name from the selected file
                    string fileName = System.IO.Path.GetFileName(selectedFile);
                    string newFullPath = System.IO.Path.Combine(folderDialog.SelectedPath, fileName);

                    // Check if file already exists in the target folder
                    if (System.IO.File.Exists(newFullPath))
                    {
                        MessageBox.Show(
                            $"A file with the same name already exists in the selected folder:\n\n{newFullPath}",
                            "File Exists",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        // Move the file
                        System.IO.File.Move(selectedFile, newFullPath);

                        // Update MRU: remove old, add new
                        MostRecentUsedFiles mruFiles = new();
                        mruFiles.Load();

                        // Remove the old path
                        mruFiles.Remove(selectedFile);

                        // Add the new path
                        mruFiles.AddFile(newFullPath);

                        // Reload the recent files and repopulate
                        LoadMRUFiles();

                        // Refresh the display
                        PopulateFiles();

                        MessageBox.Show("File moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error moving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the openToolStripButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            FileToOpen = GetSelectedFile();
            if(!string.IsNullOrEmpty(FileToOpen))
            {
                // close the form
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Populates the files.
        /// </summary>
        private void PopulateFiles()
        {
            // Get the search text and trim whitespace
            string searchText = searchToolStripTextBox.Text?.Trim() ?? string.Empty;

            // Filter the recent files if search text is not empty
            IEnumerable<FileInfo> filteredFiles = recentFiles;
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredFiles = recentFiles.Where(f =>
                    f.FileName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            // Prepare a DataTable for the DataGridView
            var table = new DataTable();
            table.Columns.Add("File Name");
            table.Columns.Add("Path");
            table.Columns.Add("Extension");
            table.Columns.Add("Last Update Time");

            foreach (var file in filteredFiles)
            {
                table.Rows.Add(
                    file.FileName,
                    file.FilePath,
                    file.FileExtension,
                    file.LastUpdateTime == DateTime.MinValue ? "" : file.LastUpdateTime.ToString("g")
                );
            }

            // Sort the DataTable by "Name"
            DataView view = table.DefaultView;
            filesDataGridView.DataSource = view.ToTable();

            // resize the columns to fit the content
            filesDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Calculate usable width for the second column
            int formClientWidth = this.ClientSize.Width;
            int dgvLeft = filesDataGridView.Left;
            int dgvRightMargin = this.ClientSize.Width - (filesDataGridView.Left + filesDataGridView.Width);
            int availableWidth = formClientWidth - dgvLeft - dgvRightMargin;

            int firstColWidth = filesDataGridView.Columns[0].Width;
            int lastColWidth = filesDataGridView.Columns[2].Width;
            int lastUpdateTimeColWidth = filesDataGridView.Columns[3].Width;
            int rowHeaderWidth = filesDataGridView.RowHeadersWidth;

            // Check for visible vertical scrollbar and get its width
            int vScrollBarWidth = 0;
            foreach (Control ctrl in filesDataGridView.Controls)
            {
                if (ctrl is VScrollBar vScrollBar && vScrollBar.Visible)
                {
                    vScrollBarWidth = vScrollBar.Width;
                    break;
                }
            }

            // Subtract the width of the "Last Update Time" column as well
            int usableWidth = availableWidth - firstColWidth - lastColWidth - lastUpdateTimeColWidth - rowHeaderWidth - vScrollBarWidth - 6;

            // Set a minimum width for usability
            usableWidth = Math.Max(50, usableWidth);

            filesDataGridView.Columns[1].Width = usableWidth;
        }

        /// <summary>
        /// Handles the Load event of the RecentFilesManageForm control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RecentFilesManageForm_Load(object sender, EventArgs e)
        {
            // set the window status to maximized
            this.WindowState = FormWindowState.Maximized;

            // Load the recent files into the local variable
            LoadMRUFiles();

            // Populate the listbox with the recent files
            PopulateFiles();
        }

        /// <summary>
        /// Handles the TextChanged event of the searchToolStripTextBox control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateFiles();
        }

        /// <summary>
        /// The file info.
        /// </summary>
        private class FileInfo
        {
            /// <summary>
            /// Gets or sets the file extension.
            /// </summary>
            public string FileExtension { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            public string FileName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the file path.
            /// </summary>
            public string FilePath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the last update time of the file.
            /// </summary>
            public DateTime LastUpdateTime { get; set; }
        }
    }
}