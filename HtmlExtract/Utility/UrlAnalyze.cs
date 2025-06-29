using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace HtmlExtract.Utility
{
    /// <summary>
    /// 对URL进行分析
    /// </summary>
    public class UrlAnalyze
    {

        private string  RegexImgTag=@"(?i)(?<=src\s*=\s*(['""]?)\s*)[^'""\s>]+(?:\.*)(?=\s*\1)";
        private string  RegexATag=@"(?i)(?<=href\s*=\s*(['""]?)\s*)[^'""\s>]+(?:\.*)(?=\s*\1)";
        private string  _currentPage=string.Empty;
        private string  _htmlSource=string.Empty;

        public UrlAnalyze(string currentPage,string htmlSource)
        {
            _currentPage=currentPage;
            _htmlSource=htmlSource;
        }
        /// <summary>
        /// 当前页URL
        /// </summary>
        public string CurrentPage
        {
            get{return _currentPage;}
            set{_currentPage=value;}
        }
        /// <summary>
        /// Html内容
        /// </summary>
        public string HtmlSource
        {
            get{return _htmlSource;}
            set{_htmlSource=value;}
        }

        /// <summary>
        /// 源码URL转换
        /// </summary>
        /// <returns></returns>
        public string UrlTransform()
        {
            if(string.IsNullOrEmpty(HtmlSource))
            {
                return string.Empty;
            }
            HtmlSource=Regex.Replace(HtmlSource, RegexImgTag, new MatchEvaluator(ReplacePath));
            HtmlSource=Regex.Replace(HtmlSource, RegexATag, new MatchEvaluator(ReplacePath));
            return HtmlSource;
        }

        /// <summary>
        /// 替换URL路径
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public string ReplacePath(Match match)
        {
            string preUrl = ReplacePath(CurrentPage, match.Value);
            return preUrl;
        }

        /// <summary>
        /// 替换URL路径
        /// </summary>
        /// <param name="path">URL路径</param>
        /// <returns></returns>
        public string ReplacePath(string currentPage, string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            path = Regex.Replace(path, @"\s", "%20");
            path = path.Replace("&amp;", "&");
            string preUrl = currentPage;
            if (path.Length < 4 || string.Compare(path.Substring(0, 4), "http", true) != 0)
            {
                if (path.Length >= 1 && path.Substring(0, 1) == "/")
                {
                    Uri uri = new Uri(preUrl);
                    preUrl = uri.Scheme + "://" + uri.Host + path;
                }
                else if (path.Length >= 2 && path.Substring(0, 2) == "./")
                {
                    preUrl = Regex.Match(preUrl, ".*/").Groups[0].Value;
                    preUrl = (preUrl + path).Replace("./", "");
                }
                else if (path.Length >= 2 && path.Substring(0, 2) == "~/")
                {
                    Uri uri = new Uri(preUrl);
                    preUrl = uri.Scheme + "://" + uri.Host + path.Replace("~", "");
                }
                else if (path.Length >= 3 && path.Substring(0, 3) == "../")
                {
                    preUrl = Regex.Match(preUrl, ".*/").Groups[0].Value;
                    Uri uri = new Uri(preUrl);
                    //preUrl = UrlSubstring(preUrl, path);
                    int lastIndex = uri.AbsoluteUri.LastIndexOf(uri.AbsolutePath);
                    preUrl = UrlSubstring(uri.AbsoluteUri.Substring(0,lastIndex), preUrl, path);
                }
                else
                {
                    preUrl = Regex.Match(preUrl, ".*/").Groups[0].Value.ToString() + path;
                }
            }
            else
            {
                preUrl = path;
            }
            return preUrl;
        }

        private string UrlSubstring(string domain, string preUrl, string path)
        {
            string urlPath = preUrl;
            int lastIndex = urlPath.LastIndexOf("/");
            if (preUrl.Substring(lastIndex) == "/")
                preUrl = preUrl.Substring(0, lastIndex);
            if (path.Length >= 3 && path.Substring(0, 3) == "../")
            {
                if (domain == preUrl)
                {
                    return domain + "/" + path.Replace("../", "");
                }
                else
                {
                    preUrl = preUrl.Substring(0, preUrl.LastIndexOf("/"));
                    path = path.Substring(path.IndexOf("/") + 1);
                    return UrlSubstring(domain, preUrl, path);
                }
            }
            return urlPath + "/" + path;
        }

    }
}

