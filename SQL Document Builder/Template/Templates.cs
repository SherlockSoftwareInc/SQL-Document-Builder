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
        public List<Template> TemplateLists => _templates;

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
                // check if the file is start with "<root>", if so, clear the file
                string firstLine = File.ReadLines(filePath).FirstOrDefault() ?? string.Empty;
                if (firstLine.StartsWith("<root>", StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllText(filePath, string.Empty); // Clear the file
                }

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
                    AddDocumentType(templateName);
                }
            }
        }

        /// <summary>
        /// Resets the template.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>A bool.</returns>
        public bool ResetTemplate(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return false; // Return false if documentType is null or empty
            }

            // Find the template based on documentType
            var template = _templates.FirstOrDefault(t => t.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase));
            if (template == null)
            {
                return false; // Template not found
            }

            // Reset the template
            if (template.Reset(documentType))
            {
                template.Save();
                return true; // Reset successful
            }
            return false; // Reset failed
        }

        /// <summary>
        /// Adds the document type.
        /// </summary>
        /// <param name="newDocumentType">The new document type.</param>
        internal void AddDocumentType(string newDocumentType)
        {
            // Check if the document type already exists
            if (_templates.Any(t => t.DocumentType.Equals(newDocumentType, StringComparison.OrdinalIgnoreCase)))
            {
                return; // Document type already exists, no need to add
            }

            // Create a new Template object with the provided document type
            var template = new Template(newDocumentType);
            if (!template.Load())
            {
                if (!string.IsNullOrEmpty(newDocumentType))
                {
                    template.Reset(newDocumentType);
                }
                else
                {
                    template.Initialize();
                }
                template.Save();
            }

            _templates.Add(template);

            // add the new document type to the file with FilePath()
            string filePath = FilePath();
            bool existsInFile = false;
            if (File.Exists(filePath))
            {
                // Read all lines and check if the document type already exists (case-insensitive)
                existsInFile = File.ReadLines(filePath)
                    .Any(line => line.Equals(newDocumentType, StringComparison.OrdinalIgnoreCase));
            }

            if (!existsInFile)
            {
                using StreamWriter sw = new(filePath, true);
                sw.WriteLine(newDocumentType); // Append the new document type to the file
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
    }
}