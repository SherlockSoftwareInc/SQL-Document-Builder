using System;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public string? Default { get; set; }
        public string? InputText { get; set; } = "";
        public string? Prompt { get; set; }
        public string? Title { get; set; }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            this.Text = Title;
            captionLabel.Text = Prompt;
            textBox1.Text = Default;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            InputText = textBox1.Text;
            Close();
        }
    }
}