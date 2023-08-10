using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SQL_Document_Builder
{
    internal class Templates
    {
        private readonly List<TemplateItem> _templates = new();

        /// <summary>
        /// Returns template items
        /// </summary>
        public List<TemplateItem> TemplateItems
        {
            get
            {
                return _templates;
            }
        }

        public string TemplatesValue
        {
            get
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

                return sb.ToString();
            }
            set
            {
                _templates.Clear();
                if (value.Length > 0)
                {
                    ParseXML(value);
                }
            }
        }

        /// <summary>
        /// Add a new template item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="footer"></param>
        public void Add(string name,
            string header,
            string body,
            string footer)
        {
            if (name.Length > 0)
            {
                var item = new TemplateItem()
                {
                    TemplateName = name,
                    Header = header,
                    Body = body,
                    Footer = footer
                };
                _templates.Add(item);
            }
        }

        /// <summary>
        /// Remove a template item at specified location
        /// </summary>
        /// <param name="index">index in the template list to remove</param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _templates.Count)
                _templates.RemoveAt(index);
        }

        /// <summary>
        /// Parse templates from a xml string
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
                                if (sNodeName.Equals("TempleteItem", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var templateItem = new TemplateItem(node);
                                    if (templateItem?.TemplateName.Length > 0)
                                        _templates.Add(templateItem);
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
    }
}