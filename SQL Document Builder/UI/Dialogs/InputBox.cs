using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The input box.
    /// </summary>
    public partial class InputBox : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputBox"/> class.
        /// </summary>
        public InputBox()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Default { get; set; }

        /// <summary>
        /// Gets or sets the input text.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string InputText { get; set; } = "";

        /// <summary>
        /// Gets or sets the prompt.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Prompt { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the max length of the input text.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxLength { get => inputTextBox.MaxLength; set => inputTextBox.MaxLength = value < 32767 ? value : 32767; }

        /// <summary>
        /// Gets or sets whether the input box is multiline.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Multiline
        {
            get => inputTextBox.Multiline;
            set
            {
                inputTextBox.Multiline = value;
                if (value)
                {
                    inputTextBox.AcceptsReturn = true;
                    inputTextBox.AcceptsTab = true;
                    this.AcceptButton = null; // Disable AcceptButton so Enter inserts newline
                    inputTextBox.KeyDown -= InputTextBox_KeyDown;
                    inputTextBox.KeyDown += InputTextBox_KeyDown;
                }
                else
                {
                    inputTextBox.AcceptsReturn = false;
                    inputTextBox.AcceptsTab = false;
                    this.AcceptButton = okButton;
                    inputTextBox.KeyDown -= InputTextBox_KeyDown;
                }
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputTextBox.Multiline)
            {
                if (e.KeyCode == Keys.Tab && !e.Control && !e.Alt)
                {
                    int selStart = inputTextBox.SelectionStart;
                    inputTextBox.Text = inputTextBox.Text.Insert(selStart, "\t");
                    inputTextBox.SelectionStart = selStart + 1;
                    e.SuppressKeyPress = true;
                    e.Handled = true;
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
        /// Inputs the box_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InputBox_Load(object sender, EventArgs e)
        {
            this.Text = Title;
            captionLabel.Text = Prompt;
            inputTextBox.Text = Default;

            // Adjust height if multiline
            if (inputTextBox.Multiline)
            {
                var currentHeight = inputTextBox.Height;
                inputTextBox.Height = currentHeight * 5;

                this.Height += currentHeight * 4;
            }
        }

        /// <summary>
        /// OS the k button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            InputText = inputTextBox.Text;
            Close();
        }
    }
}