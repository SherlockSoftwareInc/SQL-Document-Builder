using System.Text.Json.Serialization;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The column template.
    /// </summary>
    public class IndexTemplate
    {
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the column row.
        /// </summary>
        [JsonPropertyName("IndexRow")]
        public string IndexRow { get; set; } = string.Empty; // Will map "IndexRow" from JSON to "Row" in C#
    }
}