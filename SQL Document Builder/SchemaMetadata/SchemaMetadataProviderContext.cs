using System;

namespace SQL_Document_Builder.SchemaMetadata
{
    internal static class SchemaMetadataProviderContext
    {
        private static ISchemaMetadataProvider _current = CreateDefaultProvider();

        public static ISchemaMetadataProvider Current
        {
            get => _current;
            set => _current = value ?? new SchemaProviderCoreMetadataProvider();
        }

        internal static void ResetToDefaultProvider()
        {
            _current = CreateDefaultProvider();
        }

        private static ISchemaMetadataProvider CreateDefaultProvider()
        {
            var useSchemaProviderCore = true;
            var settingValue = Environment.GetEnvironmentVariable("SQLDOCBUILDER_USE_SCHEMA_PROVIDER_CORE_METADATA");
            if (!string.IsNullOrWhiteSpace(settingValue) && bool.TryParse(settingValue, out var parsed))
            {
                useSchemaProviderCore = parsed;
            }

            return useSchemaProviderCore
                ? new SchemaProviderCoreMetadataProvider()
                : new LegacySqlMetadataProvider();
        }
    }
}
