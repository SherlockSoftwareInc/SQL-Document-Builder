using System;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The date range selector.
    /// </summary>
    public partial class DateRangeSelector : Form
    {
        private DateTime endDate;

        private DateTime startDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRangeSelector"/> class.
        /// </summary>
        public DateRangeSelector()
        {
            InitializeComponent();
            if (Properties.Settings.Default.DarkMode) _ = new DarkMode(this);
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate.Date.AddDays(1).AddTicks(-1); }
            set { endDate = value.Date; }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate.Date; }
            set { startDate = value.Date; }
        }

        /// <summary>
        /// Handles the click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the click event of the OkButton control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;

            if (todayRadioButton.Checked)
            {
                StartDate = today;
                EndDate = today;
            }
            else if (twoDaysRadioButton.Checked)
            {
                StartDate = today.AddDays(-1);
                EndDate = today;
            }
            else if (threeDaysRadioButton.Checked)
            {
                StartDate = today.AddDays(-2);
                EndDate = today;
            }
            else if (sevenDaysRadioButton.Checked)
            {
                StartDate = today.AddDays(-6);
                EndDate = today;
            }
            else if (monthRadioButton.Checked)
            {
                StartDate = today.AddDays(-29);
                EndDate = today;
            }
            else if (specifyDateRadioButton.Checked)
            {
                StartDate = startDateDateTimePicker.Value.Date;
                EndDate = endDateDateTimePicker.Value.Date;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles Specify date check box checked changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SpecifyDate_CheckedChanged(object sender, EventArgs e)
        {
            specifyDateGroupBox.Enabled = specifyDateRadioButton.Checked;
        }
    }
}