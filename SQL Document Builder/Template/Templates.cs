using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The templates.
    /// </summary>
    internal class Templates
    {
        private readonly List<TemplateItem> _templates = [];
        private string _tmpFile = "";   // a temporary file to save the changes during the editing

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
        /// Add a new Template item
        /// </summary>
        /// <param name="docType">The doc type.</param>
        /// <param name="objectType">The object type.</param>
        /// <param name="body">The body.</param>
        /// <returns>A TemplateItem? .</returns>
        internal TemplateItem Add(TemplateItem.DocumentTypeEnums docType, TemplateItem.ObjectTypeEnums objectType, string body)
        {
            // Assuming _templates is a list of TemplateItem that contains templates
            var templateItem = TemplateLists.FirstOrDefault(t => t.DocumentType == docType && t.ObjectType == objectType);
            if (templateItem != null)
            {
                // If the template already exists, return it
                return templateItem;
            }

            templateItem = new TemplateItem()
            {
                DocumentType = docType,
                ObjectType = objectType,
                Body = body
            };
            _templates.Add(templateItem);
            return templateItem;
        }

        /// <summary>
        /// Load Template items from the application data file
        /// </summary>
        public void Load()
        {
            string fileName = FilePath();

            // if the file does not exist, use the default templates
            if (!File.Exists(fileName))
            {
                ParseXML(default_templates);
                Save();
            }
            else
            {
                using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using var streamReader = new StreamReader(fileStream);
                if (streamReader != null)
                {
                    string? strFirstLine = streamReader.ReadLine();
                    string? strXML = streamReader.ReadToEnd();
                    ParseXML(strFirstLine + "\r\n" + strXML);
                }
            }
            _tmpFile = Path.GetTempFileName();
        }

        /// <summary>
        /// Remove a Template item at specified location
        /// </summary>
        /// <param name="index">index in the Template list to remove</param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _templates.Count)
                _templates.RemoveAt(index);
        }

        /// <summary>
        /// Save Template settings to file
        /// </summary>
        public void Save()
        {
            Save(FilePath());
        }

        /// <summary>
        /// Save to a temp file for editing
        /// </summary>
        public void SaveTemp()
        {
            Save(_tmpFile);
        }

        /// <summary>
        /// Moves the down.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void MoveDown(TemplateItem item)
        {
            // move selected item down
            int index = _templates.IndexOf(item);
            if (index >= 0 && index < _templates.Count - 1)
            {
                _templates.RemoveAt(index);
                _templates.Insert(index + 1, item);
            }
        }

        /// <summary>
        /// Moves the up.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void MoveUp(TemplateItem item)
        {
            // move selected item up
            int index = _templates.IndexOf(item);
            if (index > 0)
            {
                _templates.RemoveAt(index);
                _templates.Insert(index - 1, item);
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

        /// <summary>
        /// Parse Templates from a xml string
        /// </summary>
        /// <param name="values">xml document body</param>
        private void ParseXML(string values)
        {
            try
            {
                _templates.Clear();

                var oDoc = new XmlDocument();
                if (oDoc != null)
                {
                    oDoc.LoadXml(values);
                    if (oDoc.DocumentElement.HasChildNodes)
                    {
                        foreach (XmlNode node in oDoc.DocumentElement.ChildNodes)
                        {
                            if (node is XmlElement)
                            {
                                string sNodeName = node.Name;
                                if (string.Compare(sNodeName, "TemplateItem", true) == 0)
                                {
                                    var item = new TemplateItem(node);
                                    _templates.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save the settings to the application data file
        /// </summary>
        /// <param name="fileName"></param>
        private void Save(string fileName)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, NewLineOnAttributes = false };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("root");

                foreach (var item in _templates)
                {
                    item.Write(writer);
                }

                writer.WriteEndElement();    //end sf:root
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            using var fileWriter = new StreamWriter(fileName);
            fileWriter.Write(sb.ToString());
            fileWriter.Flush();
            fileWriter.Close();
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>A string.</returns>
        internal TemplateItem GetTemplate(TemplateItem.DocumentTypeEnums documentType, TemplateItem.ObjectTypeEnums objectType)
        {
            // Assuming _templates is a list of TemplateItem that contains templates
            var templateItem = TemplateLists.FirstOrDefault(t => t.DocumentType == documentType && t.ObjectType == objectType);
            if (templateItem != null)
            {
                // If the template already exists, return it
                return templateItem;
            }

            templateItem = new TemplateItem()
            {
                DocumentType = documentType,
                ObjectType = objectType,
                Body = string.Empty
            };
            _templates.Add(templateItem);
            return templateItem;
        }

        /// <summary>
        /// Updates the template.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="objectType">The object type.</param>
        /// <param name="text">The text.</param>
        internal void UpdateTemplate(TemplateItem.DocumentTypeEnums documentType, TemplateItem.ObjectTypeEnums objectType, string text)
        {
            // Find the template based on documentType and objectType
            var template = GetTemplate(documentType, objectType);
            if (template != null)
            {
                // Update the template text
                template.Body = text;
                Save(); // Save the changes to the file
            }
        }

        private const string default_templates = @"<root>
  <TemplateItem>
    <DocumentType>Markdown</DocumentType>
    <ObjectType>Table</ObjectType>
    <Body># Table: `[ObjectFullName]`

&gt; [Description]

**Columns:**
[Columns]

**Indexes:**
[Indexes]

**Constraints:**
[Constraints]
---
</Body>
  </TemplateItem>
  <TemplateItem>
    <DocumentType>Markdown</DocumentType>
    <ObjectType>View</ObjectType>
    <Body># View: `[ObjectFullName]`

&gt; [Description]

**Columns:**
[Columns]

**Indexes:**
[Indexes]

**SQL Definition:**
```sql
[Definition]
```</Body>
  </TemplateItem>
  <TemplateItem>
    <DocumentType>Markdown</DocumentType>
    <ObjectType>StoredProcedure</ObjectType>
    <Body># Stored Procedure: `[ObjectFullName]`

&gt; [Description]

## Parameters
[Parameters]

## SQL Code
```sql
[Definition]
```
</Body>
  </TemplateItem>
  <TemplateItem>
    <DocumentType>Markdown</DocumentType>
    <ObjectType>Function</ObjectType>
    <Body># Function: `[ObjectFullName]`

&gt; [Description]

## Parameters
[Parameters]

## SQL Code
```sql
[Definition]
```
</Body>
  </TemplateItem>
  <TemplateItem>
    <DocumentType>SharePoint</DocumentType>
    <ObjectType>Table</ObjectType>
    <Body>&lt;h1&gt;TABLE NAME: [ObjectFullName]&lt;/h1&gt;
&lt;p&gt;[Description]&lt;/p&gt;
&lt;div&gt;
    [Columns]
&lt;/div&gt;
</Body>
  </TemplateItem>
  <TemplateItem>
    <DocumentType>Markdown</DocumentType>
    <ObjectType>Trigger</ObjectType>
    <Body># Trigger: `[ObjectFullName]`

&gt; [Description]

## Parameters
[Parameters]

## Trigger SQL Code
```sql
[Definition]
```
</Body>
  </TemplateItem>
</root>";

    }
}