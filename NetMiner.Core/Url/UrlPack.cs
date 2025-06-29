using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Net.Common;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using System.Text.RegularExpressions;
using NetMiner.Common;
using System.Net;

namespace NetMiner.Core.Url
{
    public class UrlPack
    {
        /// <summary>
        /// 根据传入的参数合成一个request请求类
        /// </summary>
        /// <param name="url"></param>
        /// <param name="webCode"></param>
        /// <param name="isUrlCode"></param>
        /// <param name="isTwoUrlCode"></param>
        /// <param name="urlCode"></param>
        /// <param name="isCustomHeader"></param>
        /// <param name="header"></param>
        /// <param name="referUrl"></param>
        /// <param name="IsUrlAutoRedirect"></param>
        /// <returns></returns>
        public static eRequest GetRequest(string url, string cookie,cGlobalParas.WebCode webCode,bool isUrlCode,
            bool isTwoUrlCode, cGlobalParas.WebCode urlCode,string HeaderFlag, List<eHeader> header,string referUrl ,bool IsUrlAutoRedirect)
        {
            //先处理url中是否存在base64编码的问题，如果有，先解码
            if (Regex.IsMatch(url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = ToolUtil.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                url = Regex.Replace(url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            //先对Url进行编码处理
            if (isUrlCode == true)
            {
                if (isTwoUrlCode == true)
                {
                    url = ToolUtil.UrlEncode(url, urlCode);
                    referUrl = ToolUtil.UrlEncode(referUrl, urlCode);

                    url = ToolUtil.UrlTwoEncode(url, urlCode);
                    referUrl = ToolUtil.UrlTwoEncode(referUrl, urlCode);
                }
                else
                {
                    url = ToolUtil.UrlEncode(url, urlCode);
                    referUrl = ToolUtil.UrlEncode(referUrl, urlCode);
                }
            }

            //对Url进行
            if (Regex.IsMatch(url, "&amp;"))
            {
                url = url.Replace("&amp;", "&");
            }

            //这个代表的是参数，部分网站请求的post数据中，包含此符号
            if (Regex.IsMatch(url, "\\{"))
            {
                url = url.Replace("\\{", "{");
                url = url.Replace("\\}", "}");
            }


            eRequest request = new eRequest();
            request.AllAllowAutoRedirect = IsUrlAutoRedirect;
            request.referUrl = referUrl;
            request.WebCode = webCode;

            string qUrl = string.Empty;
            if (Regex.IsMatch(url, @"<POST[^>]*>[\S\s]*</POST>", RegexOptions.IgnoreCase))
            {
                qUrl = url.Substring(0, url.IndexOf("<POST"));
                qUrl = Regex.Replace(qUrl, " ", "%20");

                Match s = Regex.Match(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
                string PostPara = s.Groups[0].Value.ToString();
                byte[] pPara;

                s = Regex.Match(url, @"(?<=<POST)[^>]*(?=>)", RegexOptions.IgnoreCase);
                string postCode = s.Groups[0].Value.ToString();

                if (postCode != "")
                    postCode = postCode.Substring(1, postCode.Length - 1).ToLower();

                if (postCode == "" || postCode == "ascii")
                    pPara = Encoding.ASCII.GetBytes(PostPara);
                else if (postCode == "utf8")
                    pPara = Encoding.UTF8.GetBytes(PostPara);
                else
                    pPara = Encoding.GetEncoding(postCode).GetBytes(PostPara);


                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = pPara.Length;
                request.Method = cGlobalParas.RequestMethod.Post;
                request.Body = pPara;
            }
            else
            {
                qUrl = url;
                qUrl = Regex.Replace(qUrl, " ", "%20");
                request.Method = cGlobalParas.RequestMethod.Get;
            }

            request.uri = new Uri(qUrl);

            #region cookie

            //判断是否有cookie
            CookieContainer CookieCon = new CookieContainer();
            if (cookie.Trim() != "")
            {
                try
                {
                    CookieCollection cl = new CookieCollection();

                    foreach (string sc in cookie.Split(';'))
                    {
                        string ss = sc.Trim();

                        if (!string.IsNullOrEmpty(ss))
                        {
                            string s1 = ss.Substring(0, ss.IndexOf("="));
                            string s2 = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

                            if (s2.IndexOf(",") > -1 || s2.IndexOf(";") > -1)
                            {
                                s2 = s2.Replace(",", "%2c");
                                s2 = s2.Replace(";", "%3b");
                            }

                            cl.Add(new Cookie(s1, s2, "/"));
                        }
                    }

                    if (cl.Count > 20)
                        CookieCon.PerDomainCapacity = cl.Count;

                    CookieCon.Add(request.uri, cl);
                    request.Cookie = CookieCon;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }


            #endregion

            for (int i = 0; i < header.Count; i++)
            {
                if (header[i].Range == "[All]" || header[i].Range == HeaderFlag)
                {
                    request.rHeaders.Add(header[i].Label, header[i].LabelValue);
                }
            }

            return request;
        }
    }
}
