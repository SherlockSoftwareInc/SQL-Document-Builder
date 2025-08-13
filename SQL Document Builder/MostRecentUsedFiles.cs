using System;
using System.Collections.Generic;
using System.IO;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The most recent used files class.
    /// </summary>
    internal class MostRecentUsedFiles
    {
        private readonly List<string> _mruList = [];  // recent used file list

        /// <summary>
        /// Returns file list
        /// </summary>
        public string[]? Files => _mruList?.ToArray();

        /// <summary>
        /// Save recent used files
        /// </summary>
        /// <param name="strPath"></param>
        public void AddFile(string strPath)
        {
            if (!string.IsNullOrEmpty(strPath))
            {
                _mruList.Remove(strPath);
                _mruList.Insert(0, strPath);

                Save();
            }
        }

        /// <summary>
        /// Load recent used file list
        /// </summary>
        public int Load()
        {
            _mruList.Clear();

            try
            {
                var mruFileName = MRUFileName();
                if (File.Exists(mruFileName))
                {
                    StreamReader srStream = new(mruFileName);

                    string strLine = "";

                    while ((InlineAssignHelper(ref strLine, srStream.ReadLine())) != null)
                    {
                        if (File.Exists(strLine))
                        {
                            _mruList.Add(strLine);
                        }
                    }

                    srStream.Close();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Error:" + ex.Message);
            }

            return _mruList.Count;
        }

        /// <summary>
        /// Inlines the assign helper.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        /// <returns>A T.</returns>
        private static T InlineAssignHelper<T>(ref T target, T value)
        {
            target = value;
            return value;
        }

        /// <summary>
        /// Returns the local where to store the mru list
        /// </summary>
        /// <returns></returns>
        private static string MRUFileName()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sherlock Software Inc");
            dataPath = Path.Combine(dataPath, "SQLDocBuilder");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            return Path.Combine(dataPath, "mrufiles.txt");
        }

        /// <summary>
        /// Removes the.
        /// </summary>
        /// <param name="oldFullPath">The old full path.</param>
        internal void Remove(string oldFullPath)
        {
            // Check if the oldFullPath exists in the _mruList
            if (_mruList.Contains(oldFullPath))
            {
                // Remove the oldFullPath from the list
                _mruList.Remove(oldFullPath);

                Save();
            }
        }

        /// <summary>
        /// Saves the.
        /// </summary>
        private void Save()
        {
            File.WriteAllLines(MRUFileName(), _mruList);
        }

    }
}