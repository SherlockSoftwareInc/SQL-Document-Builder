using System;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The my file log item.
    /// </summary>
    internal class MyFileLogItem
    {
        /// <summary>
        /// The action type enums.
        /// </summary>
        public enum FileOperationTypeEnums
        {
            None = 0,
            /// <summary>
            /// The create log type.
            /// </summary>
            Create,

            /// <summary>
            /// The modify log type.
            /// </summary>
            Modify,

            /// <summary>
            /// The move log type.
            /// </summary>
            Move,
        }

        /// <summary>
        /// Gets or sets the log time.
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// Gets or sets the operation type.
        /// </summary>
        public FileOperationTypeEnums LogType { get; set; } = FileOperationTypeEnums.Create;

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; } = string.Empty;
    }
}