using System;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
//using DevTools.Text;

namespace HtmlExtract.Utility
{
    public class WebRequestHelper
    {
        #region 常规操作
        /// <summary>
        /// 获取请求内容
        /// </summary>
        /// <param name="mReq"></param>
        /// <returns></returns>
        public static string GetPost(RequestArgs mReq)
        {
            string sHtml = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(mReq.Url);
            req.Method = mReq.Method;
            req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/msword, */*";
            //req.ContentType = "application/x-www-form-urlencoded";
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:x.x.x) Gecko/20041107 Firefox/x.x";//用户代理头

            if (mReq.Cookie != string.Empty)
                req.Headers.Add(HttpRequestHeader.Cookie, mReq.Cookie);
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            req.Headers.Add(HttpRequestHeader.AcceptCharset, "gb2312,utf-8;q=0.7,*;q=0.7");
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn");
            req.KeepAlive = true;

            if (mReq.RefererUrl != "")
            {
                req.Referer = mReq.RefererUrl;
            }
            req.AllowAutoRedirect = mReq.Redirect;
            req.AllowAutoRedirect = true;
            mReq.Policy = true;
            if (mReq.Policy == true)
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            if (mReq.Method == "POST")
            {
                StreamWriter sw = null;
                sw = new StreamWriter(req.GetRequestStream());
                sw.Write(mReq.PostData);
                sw.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            #region 获取编码
            Encoding encoding = Encoding.UTF8;

            if (string.IsNullOrEmpty(mReq.Encode) || mReq.Encode.ToLower() == "auto")
            {
                mReq.Encode = GetCharset(resp, "");
            }
            mReq.Encode = string.IsNullOrEmpty(mReq.Encode) ? "" : mReq.Encode.ToUpper();

            switch (mReq.Encode)
            {
                case "UTF-8":
                    encoding = Encoding.UTF8;
                    break;
                case "GB2312":
                    encoding = Encoding.GetEncoding("gb2312");
                    break;
                case "BIG5":
                    encoding = Encoding.GetEncoding("BIG5");
                    break;
                default:
                    encoding = Encoding.Default;
                    break;
            }
            #endregion

            byte[] buffer = HttpDecompress(resp);
            try
            {
                sHtml = encoding.GetString(buffer);

                //检查页面编码
                string codePattern = @"(?i)(charset\b\s*=\s*(?<charset>[^""]*))";
                string charset = Regex.Match(sHtml, codePattern).Groups["charset"].Value;
                if (!string.IsNullOrEmpty(charset) && mReq.Encode != charset.ToUpper())
                {
                    sHtml = Encoding.GetEncoding(charset).GetString(buffer);
                }

            }
            catch (ArgumentException)
            {
                //指定的编码不可识别
                sHtml = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);
            }


            string sHeader = "";
            if (mReq.GetHeaders == true)
            {
                foreach (string header in resp.Headers)
                    sHeader += header + ":" + resp.Headers[header] + "\r\n";
            }
            sHtml = sHeader + sHtml;
            req.Abort();
            resp.Close();
            return sHtml;
        }

        /// <summary>
        /// 获取编码
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="sCoding"></param>
        /// <returns></returns>
        public static string GetCharset(HttpWebResponse resp, string sCoding)
        {
            string charset = "";
            string ht = resp.GetResponseHeader("Content-Type");

            string regCharSet = "[\\s\\S]*charset=(?<charset>[\\S]*)";
            charset = RegexHelper.GetText(ht, regCharSet, "charset");

            if (string.IsNullOrEmpty(charset) && !string.IsNullOrEmpty(sCoding))
            {
                charset = sCoding.ToLower();
            }
            return charset;
        }

        /// <summary>
        /// 获取头信息
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetHead(string strUrl)
        {
            string sHtml = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/msword, */*";
            req.ContentType = "application/x-www-form-urlencoded";
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; Maxthon; .NET CLR 1.1.4322;)";

            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn");
            req.KeepAlive = true;
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            foreach (string header in resp.Headers)
            {
                sHtml += header + ":" + resp.Headers[header] + "\r\n";
            }
            req.Abort();
            resp.Close();
            return sHtml;
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sCoding"></param>
        /// <returns></returns>
        public static string GetHtmlByUrl(string sUrl, string sCoding)
        {
            string content = "";
            try
            {
                HttpWebRequest resq = GetRequest(sUrl, "", "");
                HttpWebResponse resp = (HttpWebResponse)resq.GetResponse();
                string ht = resp.GetResponseHeader("Content-Type");
                byte[] buffer = HttpDecompress(resp);

                string charset = "";
                if (sCoding == null || sCoding == "" || sCoding.ToLower() == "auto")
                {   
                    //如果不指定编码，那么系统代为指定
                    //首先，从返回头信息中寻找                 
                    string regCharSet = "[\\s\\S]*charset=(?<charset>[\\S]*)";
                    charset = RegexHelper.GetText(ht, regCharSet, "charset");

                    if (charset == "")
                    {
                        //找不到，则在文件信息本身中查找
                        content = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);
                        regCharSet = "(<meta[^>]*charset=(?<charset>[^>'\"]*)[\\s\\S]*?>)|(xml[^>]+encoding=(\"|')*(?<charset>[^>'\"]*)[\\s\\S]*?>)";
                        charset = RegexHelper.GetText(content, regCharSet, "charset");

                        if (charset.ToLower() == "gb2312" || charset == "")
                        {   //只能按"gb2312"来获取 
                            return content;
                        }
                    }
                }
                else
                {
                    charset = sCoding.ToLower();
                }
                try
                {
                    content = System.Text.Encoding.GetEncoding(charset).GetString(buffer);
                }
                catch (ArgumentException)
                {//指定的编码不可识别
                    content = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);
                }
            }
            catch
            {
                content = "";
            }
            return content;
        }
        #endregion  常规操作

        #region 辅助函数
        /// <summary>
        /// 构造Request
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sReferer"></param>
        /// <param name="sCookie"></param>
        /// <returns></returns>
        public static HttpWebRequest GetRequest(string sUrl, string sReferer, string sCookie)
        {
            try
            {
                Uri uri = new Uri(sUrl);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/msword, application/vnd.ms-excel, application/x-shockwave-flash, application/vnd.ms-powerpoint, */*";
                req.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn");
                if (sReferer != "")
                    req.Referer = sReferer;
                else
                    req.Referer = "http://" + uri.Host;
                req.Headers.Add("UA-CPU:x86");
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1;)";
                req.KeepAlive = true;
                if (sCookie != "")
                    req.Headers.Add(HttpRequestHeader.Cookie, sCookie);
                req.Timeout = 10000;
                return req;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解压数据，返回内容字符串
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static string HttpDecompress(HttpWebResponse resp, Encoding encoding)
        {
            if (resp.StatusCode != HttpStatusCode.OK)
                return "";
            string sReturn = "";

            byte[] deBuffer = HttpDecompress(resp);

            if (deBuffer != null)
            {
                sReturn = encoding.GetString(deBuffer, 0, deBuffer.Length);
            }
            return sReturn;
        }

        /// <summary>
        /// 解压数据，返回字节流
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private static byte[] HttpDecompress(HttpWebResponse resp)
        {
            int offset = 0;
            byte[] deBuffer = null;
            if (resp == null)
                return null;
            Stream s = null;
            try
            {
                s = resp.GetResponseStream();
                byte[] zipBuffer = new byte[1024 * 100];
                int bytesRead = 0;
                while (true)
                {
                    bytesRead = s.Read(zipBuffer, offset, zipBuffer.Length - offset);
                    if (bytesRead <= 0)
                        break;
                    offset += bytesRead;
                }
                if (resp.ContentEncoding == "gzip")
                {
                    MemoryStream ms = new MemoryStream(zipBuffer, 0, offset, true);
                    GZipStream zipStream = new GZipStream(ms, CompressionMode.Decompress, true);
                    ms.Seek(0, SeekOrigin.Begin);
                    deBuffer = new byte[1024 * 1024];
                    offset = 0;
                    while (true)
                    {
                        bytesRead = zipStream.Read(deBuffer, offset, 1024);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        offset += bytesRead;
                    }
                    zipStream.Close();
                }
                else if (resp.ContentEncoding == "deflate")
                {
                    MemoryStream ms = new MemoryStream(zipBuffer, 0, offset, true);
                    DeflateStream deflateStream = new DeflateStream(ms, CompressionMode.Decompress, true);
                    ms.Seek(0, SeekOrigin.Begin);
                    deBuffer = new byte[1024 * 1024];
                    offset = 0;
                    while (true)
                    {
                        bytesRead = deflateStream.Read(deBuffer, offset, 1024);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        offset += bytesRead;
                    }
                    deflateStream.Close();
                }
                else
                {
                    deBuffer = zipBuffer;
                }
                s.Close();
                resp.Close();
            }
            catch
            {
                if (s != null)
                    s.Close();
                return null;
            }
            if (offset > 0)
            {
                MemoryStream ms = new MemoryStream(deBuffer, 0, offset);
                deBuffer = ms.ToArray();
                ms.Close();
                return deBuffer;
            }
            else
                return null;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        #endregion 辅助函数
    }

    /// <summary>
    /// 请求实体
    /// </summary>
    public class RequestArgs
    {
        #region 字段
        private string _url = "";
        private string _refererUrl = "";
        private string _postData = "";
        private string _cookie = "";
        private string _encode = "Auto";
        private string _method = "GET";
        private string _accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/msword, */*";
        private bool _policy = false;
        private bool _redirect = false;
        private bool _getHeaders = false;
        private int _timeOut = 5000;
        //private List<CookieObj> _listCookie = new List<CookieObj>();
        #endregion 字段

        #region 属性
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        public string RefererUrl
        {
            get { return _refererUrl; }
            set { _refererUrl = value; }
        }
        public string PostData
        {
            get { return _postData; }
            set { _postData = value; }
        }
        /// <summary>
        /// 样例:key1=value1;key2=value2;
        /// </summary>
        public string Cookie
        {
            get
            {
                return _cookie;
            }
            set { _cookie = value; }
        }
        public string Encode
        {
            get { return _encode; }
            set { _encode = value; }
        }
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }
        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }
        public bool Policy
        {
            get { return _policy; }
            set { _policy = value; }
        }
        public bool Redirect
        {
            get { return _redirect; }
            set { _redirect = value; }
        }
        public bool GetHeaders
        {
            get { return _getHeaders; }
            set { _getHeaders = value; }
        }
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
        #endregion 属性
    }
}