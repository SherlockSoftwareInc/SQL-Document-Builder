using System;
using System.Data.Odbc;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace SQL_Document_Builder.Tests
{
    public class TableBuilderFormIntegrationTests
    {
        [Fact]
        public async Task GetObjectCreateScriptAsync_MySqlOdbc_ReturnsCreateStatement_ForOctofyApiKey()
        {
            Environment.SetEnvironmentVariable("SQLDOCBUILDER_USE_SCHEMA_PROVIDER_CORE_METADATA", "true");
            ResetSchemaMetadataProviderContext();

            var connection = CreateMySqlWikiConnection();

            Assert.False(string.IsNullOrWhiteSpace(connection.ConnectionString));

            // Integration prerequisite: DSN and credentials must be available on the machine.
            if (!ODBCDataSource.TestConnection(connection.ConnectionString))
            {
                return;
            }

            var objectName = new ObjectName(ObjectName.ObjectTypeEnums.Table, "octofy", "api_key");
            var form = (TableBuilderForm)RuntimeHelpers.GetUninitializedObject(typeof(TableBuilderForm));

            var method = typeof(TableBuilderForm).GetMethod(
                "GetObjectCreateScriptAsync",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);

            var task = (Task<string>?)method!.Invoke(form, [objectName, connection]);
            Assert.NotNull(task);

            var script = await task!;

            Assert.False(string.IsNullOrWhiteSpace(script));
            Assert.Contains("CREATE", script, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task BuildObjectDescription_MySqlOdbc_GeneratesColumnDescription_ForOctofyUsers()
        {
            Environment.SetEnvironmentVariable("SQLDOCBUILDER_USE_SCHEMA_PROVIDER_CORE_METADATA", "true");
            ResetSchemaMetadataProviderContext();

            var connection = CreateMySqlWikiConnection();
            Assert.False(string.IsNullOrWhiteSpace(connection.ConnectionString));

            if (!ODBCDataSource.TestConnection(connection.ConnectionString))
            {
                return;
            }

            var schema = new DBSchema();
            if (!await schema.OpenAsync(connection, loadColumns: true))
            {
                return;
            }

            var objectName = new ObjectName(ObjectName.ObjectTypeEnums.Table, "octofy", "users");
            var columns = await schema.GetColumnsAsync(objectName);

            var hasAnyColumnDescription = false;
            foreach (var column in columns)
            {
                var description = await schema.GetLevel2DescriptionAsync(objectName, column.ColumnName);
                if (!string.IsNullOrWhiteSpace(description))
                {
                    hasAnyColumnDescription = true;
                    break;
                }
            }

            if (!hasAnyColumnDescription)
            {
                return;
            }

            var assembly = typeof(DatabaseConnectionItem).Assembly;
            var objectDescriptionType = assembly.GetType("SQL_Document_Builder.ObjectDescription");
            var method = objectDescriptionType?.GetMethod("BuildObjectDescription", BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(method);

            var task = (Task<string>?)method!.Invoke(null, [objectName, schema, false]);
            Assert.NotNull(task);

            var script = await task!;

            Assert.Contains("MODIFY COLUMN", script, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("COMMENT", script, StringComparison.OrdinalIgnoreCase);
        }

        private static DatabaseConnectionItem CreateMySqlWikiConnection()
        {
            var connection = new DatabaseConnectionItem
            {
                ConnectionType = "ODBC",
                DBMSType = DBMSTypeEnums.MySQL,
                Name = "wiki (?????)",
                DSN = "wiki",
                UserName = "root",
                RememberPassword = true,
                RequireManualLogin = true
            };

            connection.EncrypedPassword = "zGs/q3pEMRNPEsBCm/nu7w==";
            connection.BuildConnectionString();

            var odbcBuilder = new OdbcConnectionStringBuilder
            {
                Dsn = "wiki",
                Driver = "MySQL ODBC 8.0 Unicode Driver"
            };
            if (!string.IsNullOrWhiteSpace(connection.UserName))
            {
                odbcBuilder["Uid"] = connection.UserName;
            }
            if (!string.IsNullOrWhiteSpace(connection.Password))
            {
                odbcBuilder["Pwd"] = connection.Password;
            }

            connection.ConnectionString = odbcBuilder.ConnectionString;
            return connection;
        }

        private static void ResetSchemaMetadataProviderContext()
        {
            var assembly = typeof(DatabaseConnectionItem).Assembly;
            var contextType = assembly.GetType("SQL_Document_Builder.SchemaMetadata.SchemaMetadataProviderContext");
            var resetMethod = contextType?.GetMethod("ResetToDefaultProvider", BindingFlags.NonPublic | BindingFlags.Static);
            resetMethod?.Invoke(null, null);
        }
    }
}
