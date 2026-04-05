using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            useIfExistsCheckBox.Checked = Properties.Settings.Default.UseIfExist;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseIfExist=useIfExistsCheckBox.Checked ;
            Close();
        }
        
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
