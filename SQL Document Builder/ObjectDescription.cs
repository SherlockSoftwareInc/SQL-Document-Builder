using System;
using System.Text;

namespace SQL_Document_Builder
{
    internal class ObjectDescription
    {
        public static string BuildTableDescriptions(string schemaName)
        {
            var sb = new StringBuilder();

            var tables = Common.GetTableList();
            foreach (var table in tables)
            {
                bool generate = true;
                if (schemaName.Length > 0 && !schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
                {
                    generate = false;
                }
                if (generate)
                {
                    var tableDesc = Common.GetTableDescription(table);
                    if (tableDesc?.Length > 0)
                    {
                        sb.Append(string.Format("EXEC ADMIN.usp_AddObjectDescription '{0}.{1}', N'{2}';\r\n", table.Schema, table.Name, tableDesc.Replace("'", "''")));
                    }
                    var columnsDesc = BuildTableColumnsDescriptions(table);
                    if (columnsDesc?.Length > 0)
                    {
                        sb.AppendLine(columnsDesc);
                    }
                }
            }
            return sb.ToString();
        }

        public static string BuildViewDescriptions(string schemaName, IProgress<int> progress)
        {
            var sb = new StringBuilder();

            var tables = Common.GetViewList();
            for (int i = 0; i < tables.Count; i++)
            {
                var table = tables[i];

                bool generate = true;
                if (schemaName.Length > 0 && !schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
                {
                    generate = false;
                }
                if (generate)
                {
                    var obj = new DBObject();
                    obj.Open(table.Schema, table.Name, ObjectName.ObjectTypeEnums.View, Properties.Settings.Default.dbConnectionString);
                    sb.AppendLine(obj.DescriptionScript());
                    //var tableDesc = Common.GetTableDescription(table);
                    //if (tableDesc?.Length > 0)
                    //{
                    //    sb.Append(string.Format("EXEC ADMIN.usp_AddObjectDescription '{0}.{1}', N'{2}';\r\n", table.Schema, table.Name, tableDesc.Replace("'", "''")));
                    //}
                    //var columnsDesc = BuildTableColumnsDescriptions(table);
                    //if (columnsDesc?.Length > 0)
                    //{
                    //    sb.AppendLine(columnsDesc);
                    //}
                }

                var percentComplete = (i * 100) / tables.Count;
                progress.Report(percentComplete);

            }
            return sb.ToString();
        }

        private static string BuildTableColumnsDescriptions(ObjectName tableName)
        {
            var sb = new StringBuilder();
            var columns = Common.GetTableColumns(tableName);
            foreach (var column in columns)
            {
                var columnDesc = Common.GetColumnDescription(tableName, column);
                if (columnDesc?.Length > 0)
                {
                    sb.Append(string.Format("EXEC ADMIN.usp_AddColumnDescription '{0}.{1}', '{2}', N'{3}';\r\n", tableName.Schema, tableName.Name, column, columnDesc.Replace("'", "''")));
                }
            }
            return sb.ToString();
        }
    }
}