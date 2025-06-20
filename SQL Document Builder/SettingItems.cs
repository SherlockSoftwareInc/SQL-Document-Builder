﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SQL_Document_Builder
{
    internal class SettingItems
    {
        private readonly List<SettingItem> _items = new();

        /// <summary>
        /// Returns connection items
        /// </summary>
        public List<SettingItem> Items
        {
            get
            {
                return _items;
            }
        }

        public string Settings
        {
            get
            {
                var sb = new StringBuilder();
                var settings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, NewLineOnAttributes = false };
                using (var writer = XmlWriter.Create(sb, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("root");

                    foreach (var item in _items)
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
            set { ParseXML(value); }
        }

        /// <summary>
        /// Add a new setting item
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="headerText"></param>
        /// <param name="footerText"></param>
        public void Add(string schemaName,
            string headerText,
            string footerText)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SchemaName == schemaName)
                {
                    _items[i].HeaderText = headerText;
                    _items[i].FooterText = footerText;
                    return;
                }
            }

            var connItem = new SettingItem()
            {
                SchemaName = schemaName,
                HeaderText = headerText,
                FooterText = footerText
            };
            _items.Add(connItem);
        }

        /// <summary>
        /// Gets settings for a specific schema
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public SettingItem? GetSchemaSetting(string schemaName)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SchemaName == schemaName)
                {
                    return _items[i];
                }
            }

            string headText = string.Empty;
            string footerText = string.Empty;

            switch (schemaName.ToLower())
            {
                case "dbo":
                    footerText = "<hr/>\r\n<div>Back to[[Data warehouse tables]]</div>\r\n<div>Back to [[Home]]</div>";
                    break;

                case "af":
                    footerText = "<hr/>\r\n<div>Back to[[AF Database tables]]</div>\r\n<div>Back to[[Data warehouse tables]]</div>\r\n<div>Bac to [[Home]]</div>";
                    break;

                case "bccr":
                    footerText = "<hr/>\r\n<div>Back to[[BCCR Database tables]]</div>\r\n<div>Back to[[Data warehouse tables]]</div>\r\n<div>Bac to [[Home]]</div>";
                    break;

                case "dih":
                    footerText = "<hr/>\r\n<div>Back to[[APPROACH (HeartIS) database tables]]</div>\r\n<div>Bac to [[Home]]</div>";
                    break;

                case "joint":
                    footerText = "<hr/>\r\n<div>Back to[[JOINT database tables]]</div>\r\n<div>Bac to [[Home]]</div>";
                    break;

                case "pcr":
                case "pcrl1":
                    footerText = "<hr/>\r\n<div>Back to [[PCR database tables (CVI.Source)]]</div>\r\n<div>Back to [[Data warehouse tables]]</div>";
                    break;

                case "wlv":
                    footerText = "<hr/>\r\n<div>Back to[[WLV database tables]]</div>\r\n<div>Bac to [[Home]]</div>";
                    break;

                default:
                    break;
            }

            if (headText.Length > 0 || footerText.Length > 0)
            {
                Add(schemaName, headText, footerText);
                return GetSchemaSetting(schemaName);
            }

            return null;
        }

        /// <summary>
        /// Remove a connection item at specified location
        /// </summary>
        /// <param name="index">index in the connection list to remove</param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _items.Count)
                _items.RemoveAt(index);
        }

        /// <summary>
        /// Parse connections from a xml string
        /// </summary>
        /// <param name="values">xml document body</param>
        private void ParseXML(string values)
        {
            if (values.Length < 10) return;
            try
            {
                _items.Clear();

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
                                if (string.Compare(sNodeName, "SettingItem", true) == 0)
                                {
                                    var connectionItem = new SettingItem(node);
                                    if (connectionItem?.SchemaName.Length > 0)
                                        _items.Add(connectionItem);
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