using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Muyan.Search
{
    public static class StringExtension
    {
        /// <summary>
        /// 移除指定字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        internal static string ClearChar(this string source, IEnumerable<char> chars)
        {
            return string.IsNullOrEmpty(source)
                ? string.Empty
                : new string(source.Where(t => !chars.Contains(t)).ToArray());
        }

        /// <summary>
        /// 移除HTML标签
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string ClearHtml(this string source)
        {
            string result = Regex.Replace(source, "<[^>]+>", "");
            return Regex.Replace(result, "&[^;]+;", "");
        }


        /// <summary>
        /// 移除XML标签
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string ClearXml(this string source)
        {
            return Regex.Replace(source, "<[^>]+>", "");
        }

        internal static string ClearCsv(this string source)
        {
            return source;
        }

        /// <summary>
        /// 移除JSON标签
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string ClearJson(this string source)
        {
            return source;
        }

        internal static Dictionary<string, string> ConvertJsonStringToDictionary(this string source)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            
            return dic;
        }
    }
}
