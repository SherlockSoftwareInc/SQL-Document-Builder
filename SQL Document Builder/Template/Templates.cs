using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The templates.
    /// </summary>
    internal class Templates
    {
        private readonly List<TemplateItem> _templates = new();
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
        /// <param name="name">The name.</param>
        /// <param name="body">The body.</param>
        /// <returns>A TemplateItem? .</returns>
        public TemplateItem? Add(string name,
            string body)
        {
            var connItem = new TemplateItem()
            {
                Name = name,
                TemplateBody = body
            };
            _templates.Add(connItem);
            return connItem;
        }

        /// <summary>
        /// Load Template items from the application data file
        /// </summary>
        public void Load()
        {
            string fileName = FilePath();

            if (File.Exists(fileName))
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
        /// Adds a Template item to the list
        /// </summary>
        /// <param name="Template">The Template.</param>
        internal void Add(TemplateItem Template)
        {
            // add a Template item to the list
            if (Template != null && Template.Name != null && Template.Name.Length > 0)
            {
                _templates.Add(Template);
            }
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
                                    var TemplateItem = new TemplateItem(node);
                                    if (TemplateItem?.Name?.Length > 0)
                                        _templates.Add(TemplateItem);
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
    }
}