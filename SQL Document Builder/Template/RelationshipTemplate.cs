using System.Text.Json.Serialization;

namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The relationship template.
    /// </summary>
    public class RelationshipTemplate
    {
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the relationship row.
        /// </summary>
        [JsonPropertyName("RelationshipRow")]
        public string RelationshipRow { get; set; } = string.Empty;
    }
}