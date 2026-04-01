namespace SQL_Document_Builder.SchemaMetadata
{
    internal static class SchemaMetadataProviderContext
    {
        private static ISchemaMetadataProvider _current = new LegacySqlMetadataProvider();

        public static ISchemaMetadataProvider Current
        {
            get => _current;
            set => _current = value ?? new LegacySqlMetadataProvider();
        }
    }
}
