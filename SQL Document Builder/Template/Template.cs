using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template.
    /// </summary>
    internal class Template
    {
        private readonly List<TemplateItem> _templates = [];
        private string _fileName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        public Template(string documentType)
        {
            // Validate the document type
            if (string.IsNullOrWhiteSpace(documentType))
            {
                throw new ArgumentException("Document type cannot be null or empty.", nameof(documentType));
            }

            DocumentType = documentType;

            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            _fileName = Path.Combine(dataPath, $"{documentType}.json");
        }

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        public string DocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Returns Template items
        /// </summary>
        public List<TemplateItem> TemplateLists
        {
            get
            {
                return _templates;
            }
        }

        /// <summary>
        /// Load Template items from the application data file
        /// </summary>
        public bool Load()
        {
            if (!string.IsNullOrWhiteSpace(_fileName))
            {
                // if the json file does not exists, save the templates to json
                if (File.Exists(_fileName))
                {
                    // Load the JSON file into TemplateLists
                    string json = File.ReadAllText(_fileName);
                    TemplateLists.Clear();
                    var templatesFromJson = System.Text.Json.JsonSerializer.Deserialize<List<TemplateItem>>(json);
                    if (templatesFromJson != null)
                    {
                        _templates.AddRange(templatesFromJson);
                    }
                }
                else
                {
                    return false; // File does not exist, return false
                }
            }

            // initialize the templates if they are not loaded
            if (_templates.Count != 7)
            {
                // if table template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.Table))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.Table));
                }

                // if view template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.View))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.View));
                }

                // if function template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.Function))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.Function));
                }

                // if stored procedure template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.StoredProcedure))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.StoredProcedure));
                }

                // if trigger template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.Trigger))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.Trigger));
                }

                // if object list template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.ObjectList))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.ObjectList));
                }

                // if data table template is not loaded, initialize it
                if (_templates.All(template => template.ObjectType != TemplateItem.ObjectTypeEnums.DataTable))
                {
                    _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.DataTable));
                }
            }
            return true; // Successfully loaded templates
        }

        /// <summary>
        /// Saves the.
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(_fileName))
            {
                // Save the TemplateLists to the JSON file
                string json = System.Text.Json.JsonSerializer.Serialize(_templates, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_fileName, json);
            }
        }

        /// <summary>
        /// Gets the template item.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>A TemplateItem.</returns>
        internal TemplateItem GetTemplateItem(TemplateItem.ObjectTypeEnums objectType)
        {
            // Check if the templates list is initialized
            if (_templates == null)
            {
                throw new InvalidOperationException("Templates not initialized.");
            }

            // Find the template item that matches the specified object type
            var matchedTemplateItem = _templates.FirstOrDefault(template => template.ObjectType == objectType);

            return matchedTemplateItem ?? new TemplateItem(objectType);
        }

        /// <summary>
        /// Initializes the.
        /// </summary>
        internal void Initialize()
        {
            // Initialize the templates list with default templates if needed
            if (_templates.Count == 0)
            {
                // add template items for all object types
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.Table));
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.View));
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.Function));
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.StoredProcedure));
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.Trigger));
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.ObjectList));
                _templates.Add(new TemplateItem(TemplateItem.ObjectTypeEnums.DataTable));
            }
        }

        /// <summary>
        /// Resets the.
        /// </summary>
        /// <param name="markdownTemplate">The markdown template.</param>
        internal bool Reset(string markdownTemplate)
        {
            // Load the JSON file into TemplateLists
            TemplateLists.Clear();
            try
            {
                var templatesFromJson = System.Text.Json.JsonSerializer.Deserialize<List<TemplateItem>>(markdownTemplate);
                if (templatesFromJson != null)
                {
                    _templates.AddRange(templatesFromJson);
                    return true; // Successfully reset templates
                }
            }
            catch (Exception)
            {
                Load();
            }
            return false; // Failed to reset templates
        }
    }
}