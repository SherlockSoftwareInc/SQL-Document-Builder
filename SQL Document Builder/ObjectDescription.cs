using System;
using System.Text;

namespace SQL_Document_Builder
{
    internal class ObjectDescription
    {
        public static string BuildObjectDescriptions(ObjectName.ObjectTypeEnums objType, string schemaName, IProgress<int> progress)
        {
            var sb = new StringBuilder();

            var tables = Common.GetObjectList(objType);
            for (int i = 0; i < tables.Count; i++)
            {
                int percentComplete = (i * 100) / tables.Count;
                if (percentComplete > 0 && percentComplete % 2 == 0)
                    progress.Report(percentComplete + 1);

                var table = tables[i];
                bool spaceAdded = false;

                bool generate = true;
                if (schemaName.Length > 0 && !schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
                {
                    generate = false;
                }
                if (generate)
                {
                    var dbTable = new DBObject();
                    if (dbTable.Open(table, Properties.Settings.Default.dbConnectionString))
                    {
                        var tableDesc = dbTable.Description;
                        if (tableDesc.Length > 0)
                        {
                            //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
                            sb.AppendLine();
                            spaceAdded = true;
                            sb.AppendLine(string.Format("EXEC ADMIN.usp_AddObjectDescription '{0}.{1}', N'{2}';", table.Schema, table.Name, tableDesc.Replace("'", "''")));
                        }
                    }

                    foreach (var column in dbTable.Columns)
                    {
                        var colDesc = column.Description;
                        if (colDesc.Length > 0)
                        {
                            if (!spaceAdded)
                            {
                                //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
                                sb.AppendLine();
                                spaceAdded = true;
                            }
                            sb.AppendLine(string.Format("EXEC ADMIN.usp_AddColumnDescription '{0}.{1}', '{2}', N'{3}';", table.Schema, table.Name, column.ColumnName, colDesc.Replace("'", "''")));
                        }
                    }
                }
            }
            return sb.ToString();
        }

        //public static string BuildTableDescriptions(string schemaName, IProgress<int> progress)
        //{
        //    var sb = new StringBuilder();

        //    var tables = Common.GetTableList();
        //    for (int i = 0; i < tables.Count; i++)
        //    {
        //        int percentComplete = (i * 100) / tables.Count;
        //        if (percentComplete > 0 && percentComplete % 2 == 0)
        //            progress.Report(percentComplete + 1);

        //        var table = tables[i];
        //        bool spaceAdded = false;

        //        bool generate = true;
        //        if (schemaName.Length > 0 && !schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            generate = false;
        //        }
        //        if (generate)
        //        {
        //            var dbTable = new DBObject();
        //            if (dbTable.Open(table, Properties.Settings.Default.dbConnectionString))
        //            {
        //                var tableDesc = dbTable.Description;
        //                if (tableDesc.Length > 0)
        //                {
        //                    //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
        //                    sb.AppendLine();
        //                    spaceAdded = true;
        //                    sb.AppendLine(string.Format("EXEC ADMIN.usp_AddObjectDescription '{0}.{1}', N'{2}';", table.Schema, table.Name, tableDesc.Replace("'", "''")));
        //                }
        //            }

        //            foreach (var column in dbTable.Columns)
        //            {
        //                var colDesc = column.Description;
        //                if (colDesc.Length > 0)
        //                {
        //                    if (!spaceAdded)
        //                    {
        //                        //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
        //                        sb.AppendLine();
        //                        spaceAdded = true;
        //                    }
        //                    sb.AppendLine(string.Format("EXEC ADMIN.usp_AddColumnDescription '{0}.{1}', '{2}', N'{3}';", table.Schema, table.Name, column.ColumnName, colDesc.Replace("'", "''")));
        //                }
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}

        //public static string BuildViewDescriptions(string schemaName, IProgress<int> progress)
        //{
        //    var sb = new StringBuilder();

        //    var tables = Common.GetViewList();
        //    for (int i = 0; i < tables.Count; i++)
        //    {
        //        int percentComplete = (i * 100) / tables.Count;
        //        if (percentComplete > 0 && percentComplete % 2 == 0)
        //            progress.Report(percentComplete + 1);

        //        var table = tables[i];
        //        bool spaceAdded = false;

        //        bool generate = true;
        //        if (schemaName.Length > 0 && !schemaName.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            generate = false;
        //        }
        //        if (generate)
        //        {
        //            var dbTable = new DBObject();
        //            if (dbTable.Open(table, Properties.Settings.Default.dbConnectionString))
        //            {
        //                var tableDesc = dbTable.Description;
        //                if (tableDesc.Length > 0)
        //                {
        //                    //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
        //                    sb.AppendLine();
        //                    spaceAdded = true;
        //                    sb.AppendLine(string.Format("EXEC ADMIN.usp_AddObjectDescription '{0}.{1}', N'{2}';", table.Schema, table.Name, tableDesc.Replace("'", "''")));
        //                }
        //            }

        //            foreach (var column in dbTable.Columns)
        //            {
        //                var colDesc = column.Description;
        //                if (colDesc.Length > 0)
        //                {
        //                    if (!spaceAdded)
        //                    {
        //                        //sb.AppendLine(string.Format("-- VIEW: {0}.{1}", table.Schema, table.Name));
        //                        sb.AppendLine();
        //                        spaceAdded = true;
        //                    }
        //                    sb.AppendLine(string.Format("EXEC ADMIN.usp_AddColumnDescription '{0}.{1}', '{2}', N'{3}';", table.Schema, table.Name, column.ColumnName, colDesc.Replace("'", "''")));
        //                }
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
}