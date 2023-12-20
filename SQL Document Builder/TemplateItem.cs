using System.Xml;

namespace SQL_Document_Builder
{
    internal class TemplateItem
    {
        public TemplateItem()
        {
            TemplateName = string.Empty;
            Header = string.Empty;
            Body = string.Empty;
            Footer = string.Empty;
        }

        public TemplateItem(XmlNode xNode)
            : this()
        {
            if (xNode.HasChildNodes)
            {
                foreach (XmlNode node in xNode.ChildNodes)
                {
                    string elementName = node.Name.ToLower();
                    string elementValue = node.InnerText;
                    switch (elementName)
                    {
                        case "name":
                            TemplateName = elementValue;
                            break;

                        case "header":
                            Header = elementValue;
                            break;

                        case "body":
                            Body = elementValue;
                            break;

                        case "footer":
                            Footer = elementValue;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets template body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets template footer
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets templete header
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets template name
        /// </summary>
        public string TemplateName { get; set; }
        
        /// <summary>
        /// Build xml string
        /// </summary>
        /// <param name="writer"></param>
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("TemplateItem");

            writer.WriteStartElement("Name");
            writer.WriteValue(TemplateName);
            writer.WriteEndElement();

            writer.WriteStartElement("Header");
            writer.WriteValue(Header);
            writer.WriteEndElement();

            writer.WriteStartElement("Body");
            writer.WriteValue(Body);
            writer.WriteEndElement();

            writer.WriteStartElement("Footer");
            writer.WriteValue(Footer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return TemplateName;
        }
    }
}