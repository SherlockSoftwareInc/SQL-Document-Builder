using DarkModeForms;
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
            _ = new DarkModeCS(this);
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
        public int MaxLength { get => textBox1.MaxLength; set => textBox1.MaxLength = value < 32767 ? value : 32767; }

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
            textBox1.Text = Default;
        }

        /// <summary>
        /// OS the k button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            InputText = textBox1.Text;
            Close();
        }
    }
}