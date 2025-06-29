using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlExtract.Utility
{
    public class RegexHelper
    {
        #region BaseMethod
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        public static bool IsMatch(string sInput, string sRegex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            return mc.Success;
        }
        /// <summary>
        /// 多个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="iGroupIndex">第几个分组, 从1开始, 0代表不分组</param>
        public static List<string> GetList(string sInput, string sRegex, int iGroupIndex)
        {
            List<string> list = new List<string>();
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (iGroupIndex > 0)
                    list.Add(mc.Groups[iGroupIndex].Value);
                else
                    list.Add(mc.Value);
            }
            return list;
        }
        /// <summary>
        /// 多个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sGroupName">分组名, ""代表不分组</param>
        public static List<string> GetList(string sInput, string sRegex, string sGroupName)
        {
            List<string> list = new List<string>();
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (sGroupName != "")
                    list.Add(mc.Groups[sGroupName].Value);
                else
                    list.Add(mc.Value);
            }
            return list;
        }
        /// <summary>
        /// 单个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="iGroupIndex">分组序号, 从1开始, 0不分组</param>
        public static string GetText(string sInput, string sRegex, int iGroupIndex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            if (mc.Success)
            {
                if (iGroupIndex > 0)
                    return mc.Groups[iGroupIndex].Value;
                else
                    return mc.Value;
            }
            else
                return "";
        }
        /// <summary>
        /// 单个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sGroupName">分组名, ""代表不分组</param>
        public static string GetText(string sInput, string sRegex, string sGroupName)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            if (mc.Success)
            {
                if (sGroupName != "")
                    return mc.Groups[sGroupName].Value;
                else
                    return mc.Value;
            }
            else
                return "";
        }
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
        /// <summary>
        /// 替换指定内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sReplace">替换值</param>
        /// <param name="sGroupName">分组名, "" 代表不分组</param>
        public static string Replace(string sInput, string sRegex, string sReplace, string sGroupName)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (sGroupName != "")
                    sInput = sInput.Replace(mc.Groups[sGroupName].Value, sReplace);
                else
                    sInput = sInput.Replace(mc.Value, sReplace);
            }
            return sInput;
        }
        /// <summary>
        /// 分割
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="iStrLen">最小保留字符串长度</param>
        public static List<string> Split(string sInput, string sRegex, int iStrLen)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            string[] sArray = re.Split(sInput);
            List<string> list = new List<string>();
            list.Clear();
            foreach (string s in sArray)
            {
                if (s.Trim().Length < iStrLen)
                    continue;
                list.Add(s.Trim());
            }
            return list;
        }
        #endregion BaseMethod

        #region 获得特定内容
        /// <summary>
        /// 多个链接
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static List<string> GetLinks(string sInput)
        {
            return GetList(sInput, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href");
        }
        /// <summary>
        /// 单个链接
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static string GetLink(string sInput)
        {
            return GetText(sInput, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href");
        }
        /// <summary>
        /// 图片标签
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static List<string> GetImgTag(string sInput)
        {
            return GetList(sInput, "<img[^>]+src=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "");
        }
        /// <summary>
        /// 图片地址
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static string GetImgSrc(string sInput)
        {
            return GetText(sInput, "<img[^>]+src=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src");
        }
        /// <summary>
        /// 根据URL获得域名
        /// </summary>
        /// <param name="sUrl">输入内容</param>
        public static string GetDomain(string sInput)
        {
            return GetText(sInput, @"http(s)?://([\w-]+\.)+(\w){2,}", 0);
        }
        /// <summary>
        /// 根据URL获得Host,不带协议名
        /// </summary>
        /// <param name="sUrl">输入内容</param>
        public static string GetHost(string sInput)
        {
            return GetText(sInput, @"(http(s)?://)?(?<Host>([\w-]+\.)+(\w){2,})", "Host");
        }
        /// <summary>
        /// 根据相对路径得到绝对路径
        /// </summary>
        /// <param name="sInput">原始网站地址</param>
        /// <param name="sRelativeUrl">相对链接地址</param>
        public static string GetUrl(string sInput, string sRelativeUrl)
        {
            string sReturnUrl = "";
            string sUrl1 = sInput.Trim();
            if (sRelativeUrl.ToLower().StartsWith("http") || sRelativeUrl.ToLower().StartsWith("https"))
            {
                sReturnUrl = sRelativeUrl.Trim();
            }
            else if (sRelativeUrl.StartsWith("/"))
            {
                sReturnUrl = GetDomain(sInput) + sRelativeUrl;
            }
            else if (sRelativeUrl.StartsWith("../"))
            {
                while (sRelativeUrl.IndexOf("../") >= 0)
                {
                    sUrl1 = StringHelper.GetPreStrByLast(sUrl1, "/");
                    sRelativeUrl = sRelativeUrl.Substring(3);
                }
                sReturnUrl = sUrl1 + "/" + sRelativeUrl.Trim();
            }
            else if (sRelativeUrl.Trim() != "")
            {
                sReturnUrl = StringHelper.GetPreStrByLast(sInput, "/") + "/" + sRelativeUrl;
            }
            else
            {
                sRelativeUrl = sInput;
            }
            return sReturnUrl;
        }
        #endregion 获得特定内容
    }
}
