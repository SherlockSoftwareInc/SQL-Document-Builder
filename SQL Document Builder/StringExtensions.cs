using System;

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

        public static string QuoteString(this string value)
        {
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                return value;
            }
            else
            {
                return "[" + value + "]";
            }
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
                return value.Substring(1, value.Length - 2);
            }
            else
            {
                return value;
            }
        }
    }
}