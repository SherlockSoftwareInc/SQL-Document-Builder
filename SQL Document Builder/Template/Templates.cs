using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The templates.
    /// </summary>
    internal class Templates
    {
        private readonly List<Template> _templates = [];

        /// <summary>
        /// Returns Template items
        /// </summary>
        public List<Template> TemplateLists
        {
            get
            {
                return _templates;
            }
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>A Template.</returns>
        public Template? GetTemplate(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return null; // Return null if documentType is null or empty
            }

            // Find the template based on documentType
            var template = _templates.FirstOrDefault(t => t.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase));
            if (template == null)
            {
                // If not found, create a new one
                template = new Template(documentType);
                _templates.Add(template);
            }
            return template;
        }

        /// <summary>
        /// Load Template items from the application data file
        /// </summary>
        public void Load()
        {
            var templatelist = new List<string> { "SharePoint", "Markdown", "WIKI" };

            // open the Templates.dat file
            string filePath = FilePath();
            if (File.Exists(filePath))
            {
                // Load the templates from the file
                string[] lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !templatelist.Contains(line, StringComparer.OrdinalIgnoreCase))
                    {
                        templatelist.Add(line);
                    }
                }
            }

            // Initialize the templates list with default templates if needed
            if (_templates.Count == 0)
            {
                foreach (var templateName in templatelist)
                {
                    var template = new Template(templateName);
                    if (template.Load())
                        _templates.Add(template);
                }
            }

            // if the json file does not exists, save the templates to json
            //if (!File.Exists(JsonFilePath))
            //{
            //    // Existing logic for handling file absence
            //}
            //else
            //{
            //    foreach (var template in _templates)
            //    {
            //        template.Load(); // Load each template from its file
            //    }
            //}
        }

        /// <summary>
        /// Returns the local where to store the Templates data
        /// </summary>
        /// <returns></returns>
        private static string FilePath()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            return Path.Combine(dataPath, "Templates.dat");
        }

        /// <summary>
        /// Gets the json file path.
        /// </summary>
        private static string JsonFilePath
        {
            get
            {
                string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
                dataPath = Path.Combine(dataPath, "SQLDocBuilder");
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }

                return Path.Combine(dataPath, "Templates1.json");
            }
        }

        /// <summary>
        /// Saves the.
        /// </summary>
        internal void Save()
        {
            // save for each template in the list
            foreach (var template in _templates)
            {
                template.Save();
            }
        }

        /// <summary>
        /// Adds the document type.
        /// </summary>
        /// <param name="newDocumentType">The new document type.</param>
        internal void AddDocumentType(string newDocumentType)
        {
            // Create a new Template object with the provided document type
            var template = new Template(newDocumentType);
            template.Initialize();
            template.Save(); // Save the new template
            _templates.Add(template);

            // add the new document type to the file with FilePath() 
            string filePath = FilePath();
            using StreamWriter sw = new(filePath, true);
            sw.WriteLine(newDocumentType); // Append the new document type to the file
        }
    }
}