using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The JSON builder.
    /// </summary>
    internal class JsonBuilder
    {
        private readonly StringBuilder _json = new();

        /// <summary>
        /// Gets the function procedure def.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetFunctionProcedureDef(ObjectName objectName, string connectionString)
        {
            var func = new DBObject();
            await func.OpenAsync(objectName, connectionString);

            var obj = new
            {
                ObjectName = objectName.Name,
                ObjectSchema = objectName.Schema,
                ObjectType = ObjectTypeToString(objectName.ObjectType),
                func.Description,
                func.Definition,
                Parameters = func.Parameters?.ConvertAll(dr => new
                {
                    dr.Ord,
                    dr.Name,
                    dr.DataType,
                    dr.Mode,
                    dr.Description
                })
            };

            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Gets the table view def.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="templateBody">The template body.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetTableDef(ObjectName objectName, string connectionString)
        {
            var tableView = new DBObject();
            await tableView.OpenAsync(objectName, connectionString);

            var obj = new
            {
                ObjectName = objectName.Name,
                ObjectSchema = objectName.Schema,
                ObjectType = ObjectTypeToString(objectName.ObjectType),
                Description = string.IsNullOrEmpty(tableView.Description) ? " " : tableView.Description,
                Columns = tableView.Columns?.ConvertAll(col => new
                {
                    OrdinalPosition = col.OrdinalPosition,
                    col.ColumnName,
                    col.DataType,
                    col.Description
                }),
                Indexes = tableView.Indexes?.ConvertAll(idx => new
                {
                    idx.Name,
                    idx.Type,
                    idx.Columns,
                    idx.IsUnique
                }),
                Constraints = tableView.Constraints?.ConvertAll(constraint => new
                {
                    constraint.Name,
                    constraint.Type,
                    Column = constraint.Column?.QuotedName()
                })
            };

            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Gets the view def.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetViewDef(ObjectName objectName, string connectionString)
        {
            var tableView = new DBObject();
            await tableView.OpenAsync(objectName, connectionString);

            var obj = new
            {
                ObjectName = objectName.Name,
                ObjectSchema = objectName.Schema,
                ObjectType = ObjectTypeToString(objectName.ObjectType),
                Description = string.IsNullOrEmpty(tableView.Description) ? " " : tableView.Description,
                tableView.Definition,
                Columns = tableView.Columns?.ConvertAll(col => new
                {
                    OrdinalPosition = col.OrdinalPosition,
                    col.ColumnName,
                    col.DataType,
                    col.Description
                }),
                Indexes = tableView.Indexes?.ConvertAll(idx => new
                {
                    idx.Name,
                    idx.Type,
                    idx.Columns,
                    idx.IsUnique
                })
            };

            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Builds the object list.
        /// </summary>
        /// <param name="objectList">The object list.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A Task.</returns>
        internal async Task<string> BuildObjectList(List<ObjectName> objectList, string connectionString, IProgress<int> progress)
        {
            var list = new List<object>();

            for (int i = 0; i < objectList.Count; i++)
            {
                int percentComplete = (i * 100) / objectList.Count;
                if (percentComplete > 0 && percentComplete % 2 == 0)
                {
                    progress.Report(percentComplete + 1);
                }

                ObjectName dr = objectList[i];
                string description = await DatabaseHelper.GetTableDescriptionAsync(dr, connectionString);
                list.Add(new
                {
                    Schema = dr.Schema,
                    Name = dr.Name,
                    Description = description
                });
            }

            return JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Gets the table values async.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A Task.</returns>
        internal static async Task<string> GetTableValuesAsync(string sql, string connectionString)
        {
            DataTable? dt = await DatabaseHelper.GetDataTableAsync(sql, connectionString);
            if (dt == null || dt.Rows.Count == 0)
            {
                return "[]";
            }
            return DataTableToJson(dt);
        }

        /// <summary>
        /// Data the table to json.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns>A string.</returns>
        internal static string DataTableToJson(DataTable dt)
        {
            var rows = new List<Dictionary<string, object?>>();
            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object?>();
                foreach (DataColumn col in dt.Columns)
                {
                    row[col.ColumnName] = dr[col];
                }
                rows.Add(row);
            }
            return JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Texts the to table.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        /// <returns>A string.</returns>
        internal string TextToTable(string metaData)
        {
            if (string.IsNullOrWhiteSpace(metaData))
                return "[]";

            metaData = metaData.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
            var lines = metaData.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return "[]";

            var headers = lines[0].Split('\t');
            var data = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split('\t');
                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length && j < columns.Length; j++)
                {
                    row[headers[j].Trim()] = columns[j].Trim();
                }
                data.Add(row);
            }

            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Objects the type to string.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>A string.</returns>
        private static string ObjectTypeToString(ObjectName.ObjectTypeEnums objectType)
        {
            return objectType switch
            {
                ObjectName.ObjectTypeEnums.Table => "Table",
                ObjectName.ObjectTypeEnums.View => "View",
                ObjectName.ObjectTypeEnums.Function => "Function",
                ObjectName.ObjectTypeEnums.StoredProcedure => "Stored Procedure",
                _ => "Unknown"
            };
        }
    }
}