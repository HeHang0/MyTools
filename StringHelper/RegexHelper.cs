using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringHelper
{
    public class RegexHelper
    {
        /// <summary>
        /// 判断输入字符串与正则表达式是否匹配
        /// </summary>
        /// <param name="input">输入项</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        public static bool IsMatch(string input, string pattern)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
            {
                return false;
            }
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 从给定字符串中找出第一个匹配项
        /// </summary>
        /// <param name="input">输入项</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>匹配的字符串</returns>
        public static string Match(string input, string pattern)
        {
            return Regex.Match(input, pattern).Value;
        }

        /// <summary>
        /// 从给定字符串中找出所有匹配项及位置
        /// </summary>
        /// <param name="input">输入项</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        public static Dictionary<int,string> Matches(string input, string pattern)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
            MatchCollection mc = Regex.Matches(input, pattern);
            foreach (Match item in mc)
            {
                list.Add(item.Index, item.Value);                
            }
            return list;
        }

        /// <summary>
        /// 从给定字符串中找出第一个分组匹配项
        /// </summary>
        /// <param name="input">输入项</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>匹配的字符串</returns>
        public static string MatchGroup(string input, string pattern)
        {
            if (!pattern.Contains("(") || !pattern.Contains(")"))
            {
                return "";
            }
            Match match = Regex.Match(input, pattern);
            string result = "";
            if (match.Groups.Count > 1)
            {
                result = match.Groups[1].Value;
            }
            return result;
        }

        /// <summary>
        /// 从给定字符串中找出所有分组匹配项及位置
        /// </summary>
        /// <param name="input">输入项</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        public static List<List<string>> MatchesGroups(string input, string pattern)
        {
            List<List<string>> list = new List<List<string>>();
            MatchCollection mc = Regex.Matches(input, pattern);
            foreach (Match item in mc)
            {
                List<string> groups = new List<string>();
                for (int i = 1; i < item.Groups.Count; i++)
                {
                    groups.Add(item.Groups[i].Value);
                }
                list.Add(groups);
            }
            return list;
        }

        public static string Replace(string input, string pattern, string replaceMent)
        {
            return Regex.Replace(input, pattern, replaceMent);
        }
    }
}
