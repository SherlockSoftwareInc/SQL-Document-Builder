using System;
using System.Collections.Generic;
using System.IO;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The my file operation logs.
    /// </summary>
    internal class MyFileOperationLogs
    {
        /// <summary>
        /// Adds a log entry.
        /// </summary>
        /// <param name="fileActionType">The file action type.</param>
        /// <param name="fileName">The file name.</param>
        public static void AddLog(MyFileLogItem.FileOperationTypeEnums fileActionType, string fileName)
        {
            List<MyFileLogItem> myFileLogItems = Load();

            MyFileLogItem logItem = new()
            {
                LogTime = DateTime.Now,
                LogType = fileActionType,
                FileName = fileName
            };

            myFileLogItems.Add(logItem);

            // save the log item to the file
            try
            {
                using StreamWriter swStream = new(LogFileName(), true);
                string jsonString = System.Text.Json.JsonSerializer.Serialize(logItem);
                swStream.WriteLine(jsonString);
            }
            catch (Exception)
            {
                // Handle exceptions as needed
            }
        }

        /// <summary>
        /// Gets all log entries.
        /// </summary>
        /// <returns>A list of log items.</returns>
        public static List<MyFileLogItem> GetAllLogs()
        {
            return Load();
        }

        /// <summary>
        /// Logs the file name.
        /// </summary>
        /// <returns>A string.</returns>
        private static string LogFileName()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            return Path.Combine(dataPath, "myfilelogs.json");
        }

        /// <summary>
        /// Loads the logs
        /// </summary>
        /// <returns>A list of log items.</returns>
        private static List<MyFileLogItem> Load()
        {
            List<MyFileLogItem> fileLogs = [];
            try
            {
                var logFileName = LogFileName();
                if (File.Exists(logFileName))
                {
                    using StreamReader srStream = new(logFileName);
                    string strLine;
                    while ((strLine = srStream.ReadLine()) != null)
                    {
                        var logItem = System.Text.Json.JsonSerializer.Deserialize<MyFileLogItem>(strLine);
                        if (logItem != null)
                        {
                            fileLogs.Add(logItem);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions as needed
            }
            return fileLogs;
        }
    }
}