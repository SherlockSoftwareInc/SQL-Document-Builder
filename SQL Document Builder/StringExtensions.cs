using System;
using System.Collections.Generic;

namespace SQL_Document_Builder
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a Pascal case string to normal string with space between each word
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertPascalName(this String str)
        {
            string result = str;

            if (str.Length > 0)
            {
                if (!str.Contains(' '))
                {
                    var chars = str.ToCharArray();
                    if (char.IsUpper(chars[0]))     // a pascal case string must start with a upper case char
                    {
                        bool isUpper = true;
                        result = chars[0].ToString();

                        for (int i = 1; i < chars.Length; i++)
                        {
                            char ch = chars[i];
                            if (ch == '_')
                            {
                                result += " ";
                                isUpper = true;
                            }
                            else
                            {
                                if (char.IsUpper(ch))
                                {
                                    if (isUpper)        // previous char is an upper case
                                        result += ch.ToString();
                                    else
                                    {
                                        // insert a space between word
                                        result += (" " + ch.ToString());
                                    }
                                    isUpper = true;
                                }
                                else
                                {
                                    result += ch.ToString();
                                    isUpper = char.IsUpper(ch);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if the string is a numeric string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(this String str)
        {
            if (str.Length == 0)
                return false;

            var chArray = str.ToCharArray();
            int startIndex = 0;
            if (chArray[0] == '-')
                startIndex = 1;
            bool hasDecimal = false;
            for (int i = startIndex; i < chArray.Length; i++)
            {
                char ch = chArray[i];
                if (ch == '.')
                {
                    if (hasDecimal)
                        return false;
                    hasDecimal = true;
                }
                else
                {
                    if (!char.IsDigit(ch))
                        return false;
                }
            }

            //return str.All(Char.IsNumber);
            return true;
        }

        /// <summary>
        /// Quote the database object name if needed
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string QuotedName(this string value)
        {
            if (value.Length == 0) return value;
            if (value[0] == '[' && value[^1] == ']') return value;

            if (value.Contains(' ')) return "[" + value + "]";

            var keywords = new List<string>()  { "ALTER","CREATE",
                "DROP","SELECT","INSERT","UPDATE","DELETE","MERGE",
                "TRUNCATE","DISABLE","ENABLE","EXECUTE","BULK","GRANT",
                "DENY","REVOKE","GO","ADD","BEGIN","COMMIT",
                "ROLLBACK","DUMP","BACKUP","RESTORE","LOAD","CHECKPOINT",
                "WHILE","IF","BREAK","CONTINUE","GOTO","SET",
                "DECLARE","PRINT","FETCH","OPEN","CLOSE","DEALLOCATE",
                "WITH","DBCC","KILL","MOVE","GET","RECEIVE",
                "SEND","WAITFOR","READTEXT","UPDATETEXT","WRITETEXT","USE",
                "SHUTDOWN","RETURN","REVERT","ALL","AND","ANY",
                "AS","ASC","AUTHORIZATION","BETWEEN","BROWSE","BY",
                "CASCADE","CASE","CHECK","CLUSTERED","COLLATE",
                "COLUMN","COMMITTED","COMPUTE","CONSTRAINT","CROSS","CURRENT",
                "CURSOR","DATABASE","DEFAULT","DESC","DISK","DISTINCT",
                "DISTRIBUTED","DOUBLE","ELSE","END","ESCAPE","EXCEPT",
                "EXISTS","EXTERNAL","FILE","FILLFACTOR","FOR","FOREIGN",
                "FROM","FULL","FUNCTION","GROUP","HAVING","HOLDLOCK",
                "IDENTITY","IDENTITY_INSERT","IDENTITYCOL","IN","INDEX","INNER",
                "INTERSECT","INTO","IS","JOIN","KEY","LEFT",
                "LIKE","LINENO","NOCHECK","NONCLUSTERED","NOT",
                "NULL","OFF","OFFSETS","ON","OPTION","OR",
                "ORDER","OUTER","OVER","PERCENT","PIVOT","PLAN",
                "PRECISION","PRIMARY","PROCEDURE","PUBLIC","READ","REPEATABLE",
                "RECONFIGURE","REFERENCES","REPLICATION","RETURNS","RIGHT","ROWCOUNT",
                "ROWGUIDCOL","RULE","SAVE","SCHEMA","SETUSER","SOME",
                "STATISTICS","TABLE","TABLESAMPLE","TEXTSIZE","THEN","TOP",
                "TRANSACTION","TRIGGER","UNION","UNIQUE","UNPIVOT","VALUES",
                "VARYING","VIEW","WHEN","WHERE","WITHIN"};
            if (keywords.Contains(value.ToUpper()))
            {
                return string.Format("[{0}]", value);
            }

            return value;
        }

        /// <summary>
        /// Remove quotation from a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveQuote(this string value)
        {
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                return value[1..^1];
            }
            else
            {
                return value;
            }
        }
    }
}