using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace HtmlExtract.Utility
{
    public class HtmlHelper
    {
        /// <summary>
        /// 清洗标签
        /// </summary>
        /// <param name="htmlSource"></param>
        /// <returns></returns>
        public static string HtmlCleanTag(string htmlSource)
        {
            string pattern = @"(?s)<script.*?</script[^>]*>";
            string pattern2 = @"(?s)<!--.*?-->";
            string pattern3 = @"(?s)<iframe.*?</iframe[^>]*>";
            string pattern4 = @"(?s)(?s)<style.*?</style[^>]*>";
            //string pattern5 = @"(?is)(<form[^>]*>(.*?)</form>)";
            htmlSource=Regex.Replace(htmlSource,pattern,string.Empty,RegexOptions.IgnoreCase);
            htmlSource = Regex.Replace(htmlSource, pattern2, string.Empty, RegexOptions.IgnoreCase);
            htmlSource = Regex.Replace(htmlSource, pattern3, string.Empty, RegexOptions.IgnoreCase);
            htmlSource = Regex.Replace(htmlSource, pattern4, string.Empty, RegexOptions.IgnoreCase);
            //htmlSource = Regex.Replace(htmlSource, pattern5, string.Empty, RegexOptions.IgnoreCase);
            
            return htmlSource;
        }
        /// <summary>
        /// 清洗标签(Wap)
        /// </summary>
        /// <param name="htmlSource"></param>
        /// <returns></returns>
        public static string HtmlCleanWapTag(string htmlSource)
        {
            //string pattern = @"(?s)<script.*?</script>";
            //string pattern2 = @"(?s)<!--.*?-->";
            //string pattern3 = @"(?s)<iframe.*?</iframe>";
            //string pattern4 = @"(?s)(?s)<style.*?</style>";
            string pattern5 = @"(?is)(<form[^>]*>(.*?)</form>)";
            string pattern6 = @"(?is)(<object[^>]*>(.*?)</object>)";
            string pattern7 = @"(?i)(<embed[^>]*/>)";
            //htmlSource = Regex.Replace(htmlSource, pattern, string.Empty, RegexOptions.IgnoreCase);
            //htmlSource = Regex.Replace(htmlSource, pattern2, string.Empty, RegexOptions.IgnoreCase);
            //htmlSource = Regex.Replace(htmlSource, pattern3, string.Empty, RegexOptions.IgnoreCase);
            //htmlSource = Regex.Replace(htmlSource, pattern4, string.Empty, RegexOptions.IgnoreCase);
            htmlSource = Regex.Replace(htmlSource, pattern5, string.Empty, RegexOptions.IgnoreCase);
            htmlSource = Regex.Replace(htmlSource, pattern6, string.Empty, RegexOptions.IgnoreCase);
            htmlSource = Regex.Replace(htmlSource, pattern7, string.Empty, RegexOptions.IgnoreCase);

            return htmlSource;
        }
        /// <summary>
        /// 格式化Html
        /// </summary>
        /// <param name="htmlSource"></param>
        public static string HtmlFormat(string htmlSource)
        {
            if(string.IsNullOrEmpty(htmlSource))
            {
                return htmlSource;
            }
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionOutputAsXml = true;
            htmlDoc.LoadHtml(htmlSource);
            StringBuilder sbXml = new StringBuilder();
            StringWriter sw = new StringWriter(sbXml);
            htmlDoc.Save(sw);

            return sbXml.ToString();
        }
        /// <summary>
        /// 获取标签数
        /// </summary>
        /// <param name="htmlSource"></param>
        /// <returns></returns>
        public static int GetTagCount(string htmlSource)
        {
            if (string.IsNullOrEmpty(htmlSource))
            {
                return 0;
            }
            string pattern = @"<[^>]+>";
            MatchCollection mc = Regex.Matches(htmlSource, pattern);
            return mc.Count;
        }

        /// <summary>
        /// 获取链接数量及链接内的文字数
        /// </summary>
        /// <param name="htmlSource"></param>
        /// <returns></returns>
        public static ArrayList GetLinkInfo(string htmlSource)
        {
            ArrayList result = new ArrayList();
            int linkCount = 0;//链接数
            int linkCharsCount = 0;//链接文字数
            if (string.IsNullOrEmpty(htmlSource))
            {
                result.Add(linkCount);
                result.Add(linkCharsCount);
                return result;
            }

            string pattern = @"(?is)(<a\s[^>]+>(?<linkName>.*?)<\/a>)";
            MatchCollection mc = Regex.Matches(htmlSource, pattern);
            //linkCount = mc.Count;
            foreach (Match match in mc)
            {
                GroupCollection gc = match.Groups;
                //string linkName = Regex.Replace(gc["linkName"].ToString(), @"<(?!img)[^>]+>", "",RegexOptions.IgnoreCase);
                string linkName = Regex.Replace(gc["linkName"].ToString(), @"<[^>]+>", "", RegexOptions.IgnoreCase).Trim();
                //linkName = linkName.Replace("&amp;nbsp;", "");
                linkName = linkName.Replace("&amp;", "&");
                linkName = Regex.Replace(linkName, @"&[a-z]{2,8};", "", RegexOptions.IgnoreCase);
                if (linkName.Length != 0)
                {
                    linkCount += 1;
                    linkCharsCount += linkName.Length;
                }
            }
            result.Add(linkCount);
            result.Add(linkCharsCount);
            return result;
        }

        /// <summary>
        /// 获取标点符号数
        /// </summary>
        /// <param name="htmlSource"></param>
        /// <returns></returns>
        public static int GetPunctuationCount(string htmlSource)
        {
            if (string.IsNullOrEmpty(htmlSource))
            {
                return 0;
            }
            string pattern = "[，|。|,|.|、|?|&#8217;|&#8221;]+";
            MatchCollection mc = Regex.Matches(htmlSource, pattern);
            return mc.Count;
        }

        /// <summary>
        /// 移除特殊字符
        /// </summary>
        /// <param name="htmlSource"></param>
        /// <returns></returns>
        public static string RemoveSpecialChars(string htmlSource)
        {
            if (string.IsNullOrEmpty(htmlSource))
            {
                return string.Empty;
            }
            htmlSource = Regex.Replace(htmlSource, @"\s", "");
            //htmlSource = Regex.Replace(htmlSource, @"&amp;nbsp;", "");
            htmlSource = htmlSource.Replace("&amp;", "&");
            htmlSource = Regex.Replace(htmlSource, @"&[a-z]{2,8};", "", RegexOptions.IgnoreCase);
            return htmlSource;

        }

        /// <summary>
        /// 从一段网页源码中获取正文
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMainContent(string htmlSource)
        {
            string reg1 = @"<(p|br)[^<]*>";
            string reg2 =
                @"(\[([^=]*)(=[^\]]*)?\][\s\S]*?\[/\1\])|(?<lj>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|(?<Style><style[\s\S]+?/style>)|(?<select><select[\s\S]+?/select>)|(?<Script><script[\s\S]*?/script>)|(?<Explein><\!\-\-[\s\S]*?\-\->)|(?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|(?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|(?<Other>&[a-zA-Z]+;)|(?<Other2>\#[a-z0-9]{6})|(?<Space>\s+)|(\&\#\d+\;)";
            //把p和br替换成特殊的占位符[p][br]
            htmlSource = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(htmlSource, "[$1]");

            //去掉HTML标签，保留汉字
            htmlSource = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(htmlSource, "");

            //把特殊占维护替换成回车和换行
            htmlSource = new Regex("\\[p]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(htmlSource, "\r\n　　");
            htmlSource = new Regex("\\[br]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(htmlSource, "\r\n");
            return htmlSource;
        }
    }
}
