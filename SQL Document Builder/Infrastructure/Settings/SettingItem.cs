using System.Xml;

namespace SQL_Document_Builder
{
    internal class SettingItem
    {
        public SettingItem()
        {
        }

        public SettingItem(XmlNode xNode)
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
                        case "headertext":
                            this.HeaderText = elementValue;
                            break;

                        case "footertext":
                            this.FooterText = elementValue;
                            break;

                        case "schema":
                            this.SchemaName = elementValue;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public string FooterText { get; set; } = string.Empty;
        public string HeaderText { get; set; } = string.Empty;
        public string SchemaName { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                SettingItem p = (SettingItem)obj;
                return (SchemaName == p.SchemaName);
            }
        }

        public override int GetHashCode()
        {
            if (SchemaName != null)
                return SchemaName.GetHashCode();
            else
                return 0;
        }

        public override string? ToString()
        {
            return SchemaName;
        }

        /// <summary>
        /// Build xml string
        /// </summary>
        /// <param name="writer"></param>
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("SettingItem");

            writer.WriteStartElement("SchemaName");
            writer.WriteValue(SchemaName);
            writer.WriteEndElement();

            writer.WriteStartElement("HeaderText");
            writer.WriteValue(HeaderText);
            writer.WriteEndElement();

            writer.WriteStartElement("FooterName");
            writer.WriteValue(FooterText);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}