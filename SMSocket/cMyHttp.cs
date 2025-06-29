using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using SMSocket.HttpSocket;
using SoukeyResource;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;

namespace SMSocket
{
    /// <summary>
    /// 网络矿工网页请求类
    /// </summary>
    public class cMyHttp
    {
        private string m_DefaultHeader = string.Empty;

        public cMyHttp()
        {
            this.m_AutoRedirect = true;
            this.m_IsProxy = false;
            this.m_Proxy = null;
            this.m_WebCode = cGlobalParas.WebCode.auto;
            this.m_IsUrlCode = false;
            this.m_UrlCode = cGlobalParas.WebCode.auto;
            this.m_IsCustomHeader = false;
            this.m_DefaultHeader = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0\r\n\r\n";
            this.m_cookie = string.Empty ;
            this.m_ResponseCookie = string.Empty;
        }

        ~cMyHttp()
        {
        }

        #region 属性
        private bool m_AutoRedirect;
        /// <summary>
        /// 该值指示请求是否应跟随重定向响应。
        /// </summary>
        public bool AutoRedirect
        {
            get { return m_AutoRedirect; }
            set { m_AutoRedirect = value; }
        }

        private bool m_IsProxy;
        /// <summary>
        /// 是否代理
        /// </summary>
        public bool IsProxy
        {
            get { return m_IsProxy; }
            set { m_IsProxy = value; }
        }

        private WebProxy m_Proxy;
        public WebProxy Proxy
        {
            get { return m_Proxy; }
            set { m_Proxy = value; }
        }

        private cGlobalParas.WebCode  m_WebCode;
        public cGlobalParas.WebCode WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private bool m_IsUrlCode;
        public bool IsUrlCode
        {
            get { return m_IsUrlCode; }
            set { m_IsUrlCode = value; }
        }

        private cGlobalParas.WebCode m_UrlCode;
        public cGlobalParas.WebCode UrlCode
        {
            get { return m_UrlCode; }
            set { m_UrlCode = value; }
        }

        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private string m_CustomHeader;
        public string CustomHeader
        {
            get { return m_CustomHeader; }
            set { m_CustomHeader = value; }
        }

        private string m_ResponseHeader;
        public string ResponseHeader
        {
            get { return m_ResponseHeader; }
        }

        private string m_ResponseCookie;
        public string ResponseCookie
        {
            get { return m_ResponseCookie; }
        }

        private string m_ResponseState;
        public string ResponseState
        {
            get { return m_ResponseState; }
        }

        private string m_cookie;
        public string cookie
        {
            get { return m_cookie; }
            set { m_cookie = value; }
        }
        #endregion

        MyWebRequest request = null;
        public string GetHtml(string url)
        {
            Stopwatch sw = new Stopwatch();

            string webSource = string.Empty;
            //request.

            request = MyWebRequest.Create(new Uri(url), request, true);

            bool isPost = false;
            string postData = string.Empty;

            if (this.IsCustomHeader == true && isPost == true)
                request.Header = this.CustomHeader;
            else
                request.Header = this.m_DefaultHeader;

            if (Regex.IsMatch(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
            {
                Match s = Regex.Match(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
                postData = s.Groups[0].Value.ToString();
                isPost = true;

                request.Header = request.Header.Substring(0, request.Header.Length - 2) + postData + "\r\n\r\n";
            }

            if (isPost == false)
                request.Method = "GET";
            else
                request.Method = "POST";
             

            request.wCode = (SMSocket.HttpSocket.webCode )((int)this.WebCode);
            request.uCode = (SMSocket.HttpSocket.webCode )((int)this.UrlCode);

            if (this.m_IsProxy == true)
            {
                request.proxyType = SMSocket.HttpSocket.ProxyType.HttpProxy;
                request.IsProxy = true;

                request.httpProxyAddress = this.Proxy.Address.AbsoluteUri;
                request.httpProxyPort = this.Proxy.Address.Port;
            }
            else
                request.IsProxy = false;


            //先获取头部信息
            request.GetResponse();

            Console.WriteLine("获取源码：" + sw.ElapsedMilliseconds.ToString());

            if (request.response.ResponseState == "404")
            {
                //如果是404错误，则不用再请求源码，直接返回
                request = null;

                throw (new ApplicationException("404错误"));
            }

            if ((this.AutoRedirect ==true && request.response.ResponseState == "301")
                || (this.AutoRedirect == true && request.response.ResponseState == "302"))
            {
                webSource= GetHtml(request.RequestUri.AbsoluteUri);
                request = null;
                return webSource;
            }

            webSource = request.response.TextViews;
            this.m_ResponseHeader = request.response.Header;
            this.m_ResponseState = request.response.ResponseState;

            sw.Reset();
            sw.Start();

            #region 更新cookie
            if (Regex.IsMatch(this.m_ResponseHeader, @"(?<=cookie:)[\S\s]*(?=\r\n)", RegexOptions.IgnoreCase))
            {
                //先把旧cookie添加到一个hashtable中
                string[] oldCookies = this.cookie.Split(';');
                Hashtable cookieTable = new Hashtable();
                for (int j = 0; j < oldCookies.Length; j++)
                {
                    if (oldCookies[j].ToString() != "")
                    {
                        string label = oldCookies[j].Substring(0, oldCookies[j].IndexOf("="));
                        string value = oldCookies[j].Substring(oldCookies[j].IndexOf("=") + 1, oldCookies[j].Length - oldCookies[j].IndexOf("=") - 1);
                        cookieTable.Add(label.Trim(), value.Trim());
                    }
                }

                //开始处理新的cookie
                Regex re = new Regex(@"(?<=cookie:)[\S\s]+?(?=\r\n)", RegexOptions.IgnoreCase);
                MatchCollection mc = re.Matches(this.m_ResponseHeader);
                foreach (Match ma in mc)
                {
                    string nCookie = ma.Value.ToString();
                    if (nCookie.IndexOf("expires", StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        //需判断过期时间
                        string[] newCookies = nCookie.Split(';');
                        for (int i = 0; i < newCookies.Length; i++)
                        {
                            string label = newCookies[i].Substring(0, newCookies[i].IndexOf("=")).Trim();
                            string value = newCookies[i].Substring(newCookies[i].IndexOf("=") + 1, newCookies[i].Length - newCookies[i].IndexOf("=") - 1).Trim();
                            if (label.StartsWith("expires", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string estrDate = value;
                                try
                                {
                                    DateTime eDate = DateTime.Parse(estrDate);
                                    if (eDate > System.DateTime.Now)
                                    {
                                        label = nCookie.Substring(0, nCookie.IndexOf("=")).Trim();
                                        value = nCookie.Substring(nCookie.IndexOf("=") + 1, nCookie.IndexOf(";") - nCookie.IndexOf("=") - 1).Trim();

                                        if (cookieTable.ContainsKey(label))
                                        {
                                            cookieTable.Remove(label);
                                            cookieTable.Add(label, value);
                                        }
                                        else
                                        {
                                            cookieTable.Add(label, value);
                                        }
                                        break;
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        string label = nCookie.Substring(0, nCookie.IndexOf("=")).Trim();
                        string value = string.Empty;

                        if (nCookie.IndexOf(";") > 0)
                            value = nCookie.Substring(nCookie.IndexOf("=") + 1, nCookie.IndexOf(";") - nCookie.IndexOf("=") - 1).Trim();
                        else
                            value = nCookie.Substring(nCookie.IndexOf("=") + 1, nCookie.Length - nCookie.IndexOf("=") - 1).Trim();

                        if (cookieTable.ContainsKey(label))
                        {
                            cookieTable.Remove(label);
                            cookieTable.Add(label, value);
                        }
                        else
                        {
                            cookieTable.Add(label, value);
                        }
                    }

                }

                //将cookie转换成字符串
                string rCookie = "";
                foreach (DictionaryEntry de in cookieTable)
                {
                    rCookie += de.Key + "=" + de.Value + ";";
                }
                if (rCookie != "")
                    rCookie = rCookie.Substring(0, rCookie.Length - 1);

                this.m_ResponseCookie = rCookie;
            }

            #endregion

            sw.Stop();
            Console.WriteLine("更新cookie：" + sw.ElapsedMilliseconds.ToString());
            //request = null;

            return webSource;
        }

        public void DownloadFile(string url,string fileName)
        {
        }

        public void UploadFile(string fileName)
        {
        }
    }
}
