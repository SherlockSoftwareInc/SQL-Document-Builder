namespace SQL_Document_Builder.Template
{
    /// <summary>
    /// The template item.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TemplateItem"/> class.
    /// </remarks>
    public partial class TemplateItem(TemplateItem.ObjectTypeEnums objectType)
    {

        /// <summary>
        /// The object type enums.
        /// </summary>
        public enum ObjectTypeEnums
        {
            Table,
            View,
            Function,
            StoredProcedure,
            Trigger,
            ObjectList,
            DataTable
        }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public ObjectTypeEnums ObjectType { get; set; } = objectType;

        /// <summary>
        /// Gets or sets the template body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public ColumnTemplate Columns { get; set; } = new ColumnTemplate();

        /// <summary>
        /// Gets the constraints.
        /// </summary>
        public ConstraintTemplate Constraints { get; set; } = new ConstraintTemplate();

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        public IndexTemplate Indexes { get; set; } = new IndexTemplate();

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public ParameterTemplate Parameters { get; set; } = new ParameterTemplate();

        /// <summary>
        /// Gets or sets the object lists.
        /// </summary>
        public ObjectListTemplate ObjectLists { get; set; } = new ObjectListTemplate();

        /// <summary>
        /// Gets or sets the data table.
        /// </summary>
        public DataTableTemplate DataTable { get; set; } = new DataTableTemplate();

    }

}