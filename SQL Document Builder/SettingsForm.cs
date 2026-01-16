using System;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// Represents the settings dialog for configuring AI endpoint, model, API key, and language preferences.
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// List of major supported languages for the application.
        /// </summary>
        private static readonly string[] MajorLanguages = new[]
        {
            "English",
            "Arabic",
            "Chinese (Simplified)",
            "Chinese (Traditional)",
            "Czech",
            "Dutch",
            "French",
            "German",
            "Greek",
            "Hebrew",
            "Hindi",
            "Hungarian",
            "Indonesian",
            "Italian",
            "Japanese",
            "Korean",
            "Persian (Farsi)",
            "Polish",
            "Portuguese",
            "Romanian",
            "Russian",
            "Spanish",
            "Swedish",
            "Thai",
            "Turkish",
            "Ukrainian",
            "Vietnamese"
        };

/*
        private static readonly string[] MajorLanguages = new[]
        {
            "English",
            "العربية",
            "简体中文",
            "繁體中文",
            "Čeština",
            "Nederlands",
            "Français",
            "Deutsch",
            "Ελληνικά",
            "עברית",
            "हिन्दी",
            "Magyar",
            "Bahasa Indonesia",
            "Italiano",
            "日本語",
            "한국어",
            "فارسی",
            "Polski",
            "Português",
            "Română",
            "Русский",
            "Español",
            "Svenska",
            "ไทย",
            "Türkçe",
            "Українська",
            "Tiếng Việt"
        };
*/

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsForm"/> class.
        /// Loads saved settings and populates the language selection.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();

            // Load settings from application properties
            textBoxEndpoint.Text = Properties.Settings.Default.AIEndpoint;
            textBoxModel.Text = Properties.Settings.Default.AIModel;
            textBoxApiKey.Text = Properties.Settings.Default.AIApiKey;

            // Populate language combo box with supported languages
            comboBoxLanguage.Items.AddRange(MajorLanguages);
            var savedLanguage = Properties.Settings.Default.AILanguage;
            if (!string.IsNullOrEmpty(savedLanguage) && comboBoxLanguage.Items.Contains(savedLanguage))
            {
                comboBoxLanguage.SelectedItem = savedLanguage;
            }
            else
            {
                comboBoxLanguage.SelectedIndex = 0; // Default to first language
            }
        }

        /// <summary>
        /// Handles the Save button click event.
        /// Saves the current settings to application properties and closes the form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            // Save settings to application properties
            Properties.Settings.Default.AIEndpoint = textBoxEndpoint.Text;
            Properties.Settings.Default.AIModel = textBoxModel.Text;
            Properties.Settings.Default.AIApiKey = textBoxApiKey.Text;
            Properties.Settings.Default.AILanguage = comboBoxLanguage.SelectedItem?.ToString() ?? "";
            Properties.Settings.Default.Save();
            Close();
        }

        /// <summary>
        /// Handles the Cancel button click event.
        /// Closes the form without saving changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}