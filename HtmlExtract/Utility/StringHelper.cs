using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlExtract.Utility
{
    /// <summary>
    /// 字符串辅助类
    /// Created By Xuzhibin
    /// Created On 2009.11.10
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// 字符串转换为UTF8
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeUTF8(string str)
        {
            return System.Web.HttpUtility.UrlEncode(str, System.Text.Encoding.UTF8);
        }

        
        /// <summary>
        /// Performs a case-insensitive comparison of two passed strings.
        /// </summary>
        /// <param name="stringA">The string to compare with the second parameter</param>
        /// <param name="stringB">The string to compare with the first parameter</param>
        /// <returns>
        /// 	<c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(string stringA, string stringB)
        {
            return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Evaluates an array of strings to determine if at least one item is a match
        /// </summary>
        /// <param name="stringA">The base comparison string.</param>
        /// <param name="matchStrings">The match strings.</param>
        /// <returns></returns>
        public static bool MatchesOne(string stringA, params string[] matchStrings)
        {
            for (int i = 0; i < matchStrings.Length; i++)
            {
                if (IsMatch(stringA, matchStrings[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Performs a case-insensitive comparison of two passed strings, 
        /// with an option to trim the strings before comparison.
        /// </summary>
        /// <param name="stringA">The string to compare with the second parameter</param>
        /// <param name="stringB">The string to compare with the first parameter</param>
        /// <param name="trimStrings">if set to <c>true</c> strings will be trimmed before comparison.</param>
        /// <returns>
        /// 	<c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(string stringA, string stringB, bool trimStrings)
        {
            if (trimStrings)
                return String.Equals(stringA.Trim(), stringB.Trim(), StringComparison.InvariantCultureIgnoreCase);

            return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the passed string matches the passed RegEx pattern.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="matchPattern">The RegEx match pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the string matches the pattern; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRegexMatch(string inputString, string matchPattern)
        {
            return Regex.IsMatch(inputString, matchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }
        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        public static string FastReplace(string original, string pattern, string replacement)
        {
            return FastReplace(original, pattern, replacement, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static string FastReplace(string original, string pattern, string replacement, StringComparison comparisonType)
        {
            if (original == null)
                return null;

            if (String.IsNullOrEmpty(pattern))
                return original;

            int lenPattern = pattern.Length;
            int idxPattern = -1;
            int idxLast = 0;

            StringBuilder result = new StringBuilder();

            while (true)
            {
                idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);

                if (idxPattern < 0)
                {
                    result.Append(original, idxLast, original.Length - idxLast);
                    break;
                }

                result.Append(original, idxLast, idxPattern - idxLast);
                result.Append(replacement);

                idxLast = idxPattern + lenPattern;
            }

            return result.ToString();
        }
        /// <summary>
        /// Strips the text.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="stripString">The strip string.</param>
        /// <returns></returns>
        public static string StripText(string inputString, string stripString)
        {
            if (!String.IsNullOrEmpty(stripString))
            {
                string[] replace = stripString.Split(new char[] { ',' });
                for (int i = 0; i < replace.Length; i++)
                {
                    if (!String.IsNullOrEmpty(inputString))
                        inputString = Regex.Replace(inputString, replace[i], String.Empty);
                }
            }
            return inputString;
        }
        /// <summary>
        /// Finds a match in word using comma separted list.
        /// </summary>
        /// <param name="word">The string to check against.</param>
        /// <param name="list">A comma separted list of values to find.</param>
        /// <returns>
        /// true if a match is found or list is empty, otherwise false.
        /// </returns>
        public static bool StartsWith(string word, string list)
        {
            if (string.IsNullOrEmpty(list))
                return true;

            string[] find = Split(list);

            foreach (string f in find)
            {
                if (word.StartsWith(f, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// A custom split method
        /// </summary>
        /// <param name="list">A list of values separated by either ", " or ","</param>
        /// <returns></returns>
        public static string[] Split(string list)
        {
            string[] find;
            try
            {
                find = list.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                find = new string[] { String.Empty };
            }
            return find;
        }
        /// <summary>
        /// Checks the length of the string.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string CheckStringLength(string stringToCheck, int maxLength)
        {
            string checkedString;

            if (stringToCheck.Length <= maxLength)
                return stringToCheck;

            // If the string to check is longer than maxLength 
            // and has no whitespace we need to trim it down.
            if ((stringToCheck.Length > maxLength) && (stringToCheck.IndexOf(" ") == -1))
                checkedString = String.Concat(stringToCheck.Substring(0, maxLength), "...");
            else if (stringToCheck.Length > 0)
                checkedString = String.Concat(stringToCheck.Substring(0, maxLength), "...");
            else
                checkedString = stringToCheck;

            return checkedString;
        }
        /// <summary>
        /// Gets the random string.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, false));
            builder.Append(RandomInt(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        /// <summary>
        /// Randoms the string.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="lowerCase">if set to <c>true</c> [lower case].</param>
        /// <returns></returns>
        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(26 * random.NextDouble() + 65));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        /// <summary>
        /// Randoms the int.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        private static int RandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        /// <summary>
        /// 根据指定字符，截取相应字符串
        /// </summary>
        /// <param name="sOrg"></param>
        /// <param name="sLast"></param>
        /// <returns></returns>
        public static string GetStrByLast(string sOrg, string sLast)
        {
            int iLast = sOrg.LastIndexOf(sLast);
            if (iLast > 0)
                return sOrg.Substring(iLast + 1);
            else
                return sOrg;
        }
        public static string GetPreStrByLast(string sOrg, string sLast)
        {
            int iLast = sOrg.LastIndexOf(sLast);
            if (iLast > 0)
                return sOrg.Substring(0, iLast);
            else
                return sOrg;
        }
        /// <summary> 
        /// 检测含有中文字符串的实际长度
        /// </summary> 
        /// <param name="str">字符串</param> 
        public static int GetLength(string str)
        {
            System.Text.ASCIIEncoding n = new System.Text.ASCIIEncoding();
            byte[] b = n.GetBytes(str);
            int l = 0; // l 为字符串之实际长度 
            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] == 63) //判断是否为汉字或全脚符号 
                {
                    l++;
                }
                l++;
            }
            return l;
        }
        //截取长度,num是英文字母的总数，一个中文算两个英文
        public static string GetLetter(string str, int iNum, bool bAddDot)
        {
            string sContent = "";
            int iTmp = iNum;
            if (str == null)
                return sContent;
            else
                sContent = str;
            if (sContent.Length > 0)
            {
                if (iTmp > 0)
                {
                    if (sContent.Length * 2 > iTmp) //说明字符串的长度可能大于iNum,否则显示全部
                    {
                        char[] arrC;
                        if (sContent.Length >= iTmp) //防止因为中文的原因使ToCharArray溢出
                        {
                            arrC = str.ToCharArray(0, iTmp);
                        }
                        else
                        {
                            arrC = str.ToCharArray(0, sContent.Length);
                        }
                        int k = 0;
                        int i = 0;
                        int iLength = 0;
                        foreach (char ch in arrC)
                        {
                            iLength++;
                            if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                            {
                                i += 2;
                            }
                            else
                            {
                                k = (int)ch;
                                if (k < 0)
                                {
                                    k = 65536;
                                }
                                if (k > 255)
                                {
                                    i += 2;
                                }
                                else
                                {
                                    if (k >= (int)'A' && k <= (int)'Z')
                                        i++;
                                    i++;
                                }
                            }
                            if (i >= iTmp)
                                break;
                        }
                        if (bAddDot)
                            sContent = sContent.Substring(0, iLength - 2) + "...";
                        else
                            sContent = sContent.Substring(0, iLength);
                    }
                }
            }
            return sContent;
        }

        public static string[] Split(string s, char c, int count)
        {
            string[] ss = s.Split(new[] { c }, count);
            if (ss.Length == count)
            {
                return ss;
            }
            var l = new List<string>(ss);
            for (int i = ss.Length; i < count; i++)
            {
                l.Add("");
            }
            return l.ToArray();
        }

        public static string MultiLineAddPrefix(string Source)
        {
            return MultiLineAddPrefix(Source, "\t");
        }

        public static string MultiLineAddPrefix(string Source, string Prefix)
        {
            return MultiLineAddPrefix(Source, Prefix, '\n');
        }

        public static string MultiLineAddPrefix(string Source, string Prefix, char SplitBy)
        {
            var sb = new StringBuilder();
            string[] ss = Source.Split(SplitBy);
            foreach (string s in ss)
            {
                sb.Append(Prefix);
                sb.Append(s);
                sb.Append(SplitBy);
            }
            return sb.ToString();
        }

        public static string GetStringLeft(string s, int n)
        {
            if (s.Length > n)
            {
                return s.Substring(0, s.Length - n);
            }
            throw new ArgumentOutOfRangeException();
        }

        #region Base64 Util

        /// <summary>
        /// 把字符串进行BASE64编码
        /// </summary>
        /// <param name="inputString">原字符串</param>
        /// <param name="encodingName">编码格式名</param>
        /// <returns>编码后的字符串</returns>
        public static string ToBase64(string inputString, string encodingName)
        {
            return Convert.ToBase64String(Encoding.GetEncoding(encodingName).GetBytes(inputString));
        }

        /// <summary>
        /// 对BASE64字符串进行解码
        /// </summary>
        /// <param name="base64String">待解码的字符串</param>
        /// <param name="encodingName">编码格式名</param>
        /// <returns>解码后的字符串</returns>
        public static string FromBase64(string base64String, string encodingName)
        {
            return Encoding.GetEncoding(encodingName).GetString(Convert.FromBase64String(base64String));
        }

        #endregion Base64 Util

        #region HTML相关操作
        /// <summary>
        /// 清除Html标记
        /// </summary>
        /// <param name="sHtml"></param>
        /// <returns></returns>
        public static string ClearHtml(string Htmlstring)
        {
            //删除脚本

            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML

            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");

            Htmlstring.Replace(">", "");

            Htmlstring.Replace("\r\n", "");

            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        
        public static string ClearTag(string sHtml)
        {
            if (sHtml == "")
                return "";
            sHtml = Regex.Replace(sHtml, @"<iframe[\s\S]+?</iframe>", "", RegexOptions.IgnoreCase);
            sHtml = Regex.Replace(sHtml, @"<script[\s\S]+?</script>", "", RegexOptions.IgnoreCase);
            sHtml = Regex.Replace(sHtml, @"(<[^>\s]*\b(\w)+\b[^>]*>)|([\s]+)|(<>)|(&nbsp;)", "", RegexOptions.IgnoreCase);
            sHtml = sHtml.Replace("\"", "").Replace("<", "").Replace(">", "");
            return sHtml;
        }
        public static string ClearTag(string sHtml, string sRegex)
        {
            string sTemp = sHtml;
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return re.Replace(sHtml, "");
        }

        public static string ConvertToJS(string sHtml)
        {
            StringBuilder sText = new StringBuilder();
            Regex re;
            re = new Regex(@"\r\n", RegexOptions.IgnoreCase);
            string[] strArray = re.Split(sHtml);
            foreach (string strLine in strArray)
            {
                sText.Append("document.writeln(\"" + strLine.Replace("\"", "\\\"") + "\");\r\n");
            }
            return sText.ToString();
        }
        public static string ReplaceNbsp(string str)
        {
            string sContent = str;
            if (sContent.Length > 0)
            {
                sContent = sContent.Replace(" ", "");
                sContent = sContent.Replace("&nbsp;", "");
                sContent = "&nbsp;&nbsp;&nbsp;&nbsp;" + sContent;
            }
            return sContent;
        }
        public static string StringToHtml(string str)
        {
            string sContent = str;
            if (sContent.Length > 0)
            {
                char csCr = (char)13;
                sContent = sContent.Replace(csCr.ToString(), "<br>");
                sContent = sContent.Replace(" ", "&nbsp;");
                sContent = sContent.Replace("　", "&nbsp;&nbsp;");
            }
            return sContent;
        }
        public static string JsToHtml(string strJS)
        {
            string sReturn = strJS.Replace("document.writeln(\"", "");
            sReturn = sReturn.Replace("document.write(\"", "");
            sReturn = sReturn.Replace("document.write('", "");
            sReturn = RegexHelper.Replace(sReturn, @"(?<backslash>\\)[^\\]", "", "backslash");
            sReturn = sReturn.Replace(@"\\", @"\");
            sReturn = sReturn.Replace("/\\\\\\", "\\");
            sReturn = sReturn.Replace("/\\\\\\'", "\\'");
            sReturn = sReturn.Replace("/\\\\\\//", "\\/");
            sReturn = sReturn.Replace("\");", "");
            sReturn = sReturn.Replace("\")", "");
            sReturn = sReturn.Replace("');", "");
            return sReturn;
        }


        //截取长度并转换为HTML
        public static string AcquireAssignString(string str, int num)
        {
            string sContent = str;
            sContent = GetLetter(sContent, num, false);
            sContent = StringToHtml(sContent);
            return sContent;
        }
        /// <summary>
        /// 获取一个Html标签里的所有属性键值对,属性值必须用双引号""包括
        /// Key的值默认转换成小写
        /// </summary>
        /// <param name="sHtml"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetProperties(string sHtml)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Regex re = new Regex(@"\s+(?<Key>[^\s]+?)\s*=\s*""(?<Value>[\s\S]*?)(?<![\\])""");
            MatchCollection mc = re.Matches(sHtml);
            string sKey = null;
            foreach (Match m in mc)
            {
                sKey = m.Groups["Key"].Value.ToLower();
                if (!dic.ContainsKey(sKey))
                    dic.Add(m.Groups["Key"].Value.ToLower(), m.Groups["Value"].Value.Replace("\\\"", "\""));
            }
            return dic;
        }
        #endregion HTML相关操作
        #region Regex
        /// <summary>
        /// 替换指定内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sReplace">替换值</param>
        /// <param name="iGroupIndex">分组序号, 0代表不分组</param>
        public static string Replace(string sInput, string sRegex, string sReplace, int iGroupIndex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (iGroupIndex > 0)
                    sInput = sInput.Replace(mc.Groups[iGroupIndex].Value, sReplace);
                else
                    sInput = sInput.Replace(mc.Value, sReplace);
            }
            return sInput;
        }
        #endregion
    }
}
