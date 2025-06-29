using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using NetMiner.Resource;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Xml;

//支持下载和上传速率控制的http访问类
namespace NetMiner.Gather.MyHttp
{
    public class cAsyncGetWebPage
    {
        //#region 属性
        //private string m_WebpageSource = string.Empty;

        //private double m_UploadSpeed;
        //public double UploadSpeed
        //{
        //    get { return m_UploadSpeed; }
        //    set { m_UploadSpeed = value; }
        //}

        //private double m_DownloadSpeed;
        //public double DownloadSpeed
        //{
        //    get { return m_DownloadSpeed; }
        //    set { m_DownloadSpeed = value; }
        //}

        //private bool m_IsUrlAutoRedirect;
        //public bool IsUrlAutoRedirect
        //{
        //    get { return m_IsUrlAutoRedirect; }
        //    set { m_IsUrlAutoRedirect = value; }
        //}

        //private bool m_IsProxy;
        //public bool IsProxy
        //{
        //    get { return m_IsProxy; }
        //    set { m_IsProxy = value; }
        //}

        //private WebProxy m_wProxy;
        //public WebProxy wProxy
        //{
        //    get { return m_wProxy; }
        //    set { m_wProxy = value; }
        //}

        //private bool m_IsCustomHeader;
        //public bool IsCustomHeader
        //{
        //    get { return m_IsCustomHeader; }
        //    set { m_IsCustomHeader = value; }
        //}

        //private List<eHeader> m_Headers;
        //public List<eHeader> Headers
        //{
        //    get { return m_Headers; }
        //    set { m_Headers = value; }
        //}
        //#endregion

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DefaultTimeout = 2 * 60 * 1000; // 2 minutes timeout

        //控制访问超时
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }
        //public string GetHtml(string url, cGlobalParas.WebCode webCode, bool isUrlCode, cGlobalParas.WebCode urlCode,
        //           ref string cookie, string startPos, string endPos, bool IsCutnr, bool IsAjax)
        //{

        //    if (isUrlCode == true)
        //        url = cTool.UrlEncode(url, urlCode);

        //    if (Regex.IsMatch(url, "&amp;"))
        //    {
        //        url = url.Replace("&amp;", "&");
        //    }

        //    if (Regex.IsMatch(url, "\\{"))
        //    {
        //        url = url.Replace("\\{", "{");
        //        url = url.Replace("\\}", "}");
        //    }

        //    //判断网页编码
        //    Encoding wCode;
        //    string PostPara = "";

        //    CookieContainer CookieCon = new CookieContainer();

        //    HttpWebRequest wReq;

        //    Uri m_Uri;

        //    try
        //    {
        //        m_Uri = new Uri(url);
        //    }
        //    catch (System.Exception)
        //    {
        //        return "";
        //    }


        //    if (Regex.IsMatch(url, @"<POST[^>]*>[\S\s]*</POST>", RegexOptions.IgnoreCase))
        //    {
        //        wReq = (HttpWebRequest)WebRequest.Create(@url.Substring(0, url.IndexOf("<POST")));
        //    }
        //    else
        //    {
        //        wReq = (HttpWebRequest)WebRequest.Create(@url);
        //    }

        //    //允许重定向
        //    wReq.AllowAutoRedirect = this.IsUrlAutoRedirect;

        //    //判断是否需要代理
        //    if (m_IsProxy == true)
        //        wReq.Proxy = m_wProxy;

        //    #region cookie

        //    //判断是否有cookie

        //    if (cookie.Trim() != "")
        //    {
        //        CookieCollection cl = new CookieCollection();

        //        foreach (string sc in cookie.Split(';'))
        //        {
        //            string ss = sc.Trim();


        //            string s1 = ss.Substring(0, ss.IndexOf("="));
        //            string s2 = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

        //            if (s2.IndexOf(",") > 0 || s2.IndexOf(";") > 0)
        //            {
        //                s2 = s2.Replace(",", "%2c");
        //                s2 = s2.Replace(";", "%3b");
        //            }

        //            cl.Add(new Cookie(s1, s2, "/"));
        //        }


        //        CookieCon.Add(m_Uri, cl);
        //        wReq.CookieContainer = CookieCon;
        //    }
        //    else
        //    {
        //        wReq.CookieContainer = CookieCon;
        //    }

        //    #endregion

        //    #region 通讯header

        //    if (this.IsCustomHeader == true)
        //    {
        //        for (int i = 0; i < this.Headers.Count; i++)
        //        {
        //            switch (this.Headers[i].Label)
        //            {

        //                case "Accept":
        //                    wReq.Accept = this.Headers[i].LabelValue;
        //                    break;
        //                case "User-Agent":
        //                    wReq.UserAgent = this.Headers[i].LabelValue;
        //                    break;
        //                case "Connection":
        //                    wReq.KeepAlive = true;
        //                    break;
        //                case "Content-Type":
        //                    wReq.ContentType = this.Headers[i].LabelValue;
        //                    break;
        //                case "Referer":
        //                    wReq.Referer = this.Headers[i].LabelValue;
        //                    break;
        //                case "Host":
        //                    break;
        //                default:
        //                    if (this.Headers[i].LabelValue.StartsWith("{Cookie"))
        //                    {
        //                        string hValue = this.Headers[i].LabelValue;
        //                        hValue = hValue.Substring(8, hValue.Length - 9).ToLower();

        //                        CookieCollection cc = CookieCon.GetCookies(m_Uri);

        //                        for (int ii = 0; ii < cc.Count; ii++)
        //                        {
        //                            if (cc[ii].Name.ToLower() == hValue)
        //                            {
        //                                wReq.Headers.Add(this.Headers[i].Label, cc[ii].Value);
        //                            }
        //                        }
        //                    }
        //                    else
        //                        wReq.Headers.Add(this.Headers[i].Label, this.Headers[i].LabelValue);
        //                    break;
        //            }

        //        }
        //    }
        //    else
        //    {

        //        wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
        //        wReq.Headers.Add("Accept-Language", "zh-cn,en-us;");
        //        wReq.KeepAlive = true;
        //        wReq.Headers.Add("Accept-Encoding", "gzip, deflate");

        //        Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);

        //        string url1 = a.Groups[0].Value.ToString();
        //        wReq.Referer = url1;
        //    }

        //    #endregion

        //    #region 判断是否为https 如果是则需要加载证书
        //    if (wReq.RequestUri.Scheme == "https")
        //    {
        //        #region 加载证书
        //        //挂接验证服务端证书的回调
        //        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

        //        //Create new X509 store called teststore from the local certificate store.
        //        X509Store _store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        //        _store.Open(OpenFlags.ReadWrite);

        //        //获取本地主机名称作为证书查找的参数
        //        string findValue = m_Uri.Host;
        //        if (findValue.StartsWith("www", StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            findValue = findValue.Substring(4, findValue.Length - 4);

        //        }

        //        findValue = findValue.Substring(0, findValue.IndexOf("."));
        //        X509Certificate2Collection _certsCollection = _store.Certificates.Find(X509FindType.FindByIssuerName, findValue, false);

        //        if (_certsCollection.Count == 0)
        //            _certsCollection = _store.Certificates.Find(X509FindType.FindBySubjectName, findValue, false);

        //        X509Certificate2 x509c = null;



        //        if (_certsCollection.Count > 0)
        //        {

        //            x509c = _certsCollection[0];
        //            wReq.ClientCertificates.Add(x509c);

        //        }
        //        else
        //        {
        //            throw new Exception("无效的证书，请确保证书存在且有效！");
        //        }

        //        #endregion
        //    }
        //    #endregion

        //    #region POST数据

        //    //判断是否含有POST参数
        //    if (Regex.IsMatch(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
        //    {

        //        Match s = Regex.Match(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
        //        PostPara = s.Groups[0].Value.ToString();
        //        byte[] pPara;

        //        s = Regex.Match(url, @"(?<=<POST)[^>]*(?=>)", RegexOptions.IgnoreCase);
        //        string postCode = s.Groups[0].Value.ToString();

        //        if (postCode != "")
        //            postCode = postCode.Substring(1, postCode.Length - 1).ToLower();

        //        if (postCode == "" || postCode == "ascii")
        //            pPara = Encoding.ASCII.GetBytes(PostPara);
        //        else if (postCode == "utf8")
        //            pPara = Encoding.UTF8.GetBytes(PostPara);
        //        else
        //            pPara = Encoding.GetEncoding(postCode).GetBytes(PostPara);


        //        if (wReq.ContentType == "")
        //            wReq.ContentType = "application/x-www-form-urlencoded";

        //        wReq.ContentLength = pPara.Length;

        //        wReq.Method = "POST";

        //        System.IO.Stream reqStream = wReq.GetRequestStream();
        //        reqStream.Write(pPara, 0, pPara.Length);
        //        reqStream.Close();

        //    }
        //    else
        //    {
        //        wReq.Method = "GET";

        //    }

        //    #endregion

        //    //设置页面超时时间为12秒
        //    wReq.Timeout = 30000;

        //    //从这里开始进行异步请求
        //    RequestState myRequestState = new RequestState();
        //    myRequestState.request = wReq;
        //    IAsyncResult result =
        //        (IAsyncResult)wReq.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

        //    ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), wReq, DefaultTimeout, true);

        //    allDone.WaitOne();

        //    // Release the HttpWebResponse resource.
        //    myRequestState.response.Close();

        //    return this.m_WebpageSource;

        //}

        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                
            }
            catch (System.Exception  e)
            {
                throw e;
                
            }
            allDone.Set();
        }

        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                
                if (read > 0)
                {
                    myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }
                else
                {
                    if (myRequestState.requestData.Length > 1)
                    {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        
                    }
                    responseStream.Close();
                }

            }
            catch (System.Exception   e)
            {
                throw e;
            }
            allDone.Set();

        }


        #region SSL的处理
        public bool RemoteCertificateValidationCallback(Object sender,
         X509Certificate certificate,
         X509Chain chain,
         SslPolicyErrors sslPolicyErrors)
        {
            //return true;

            #region Validated Message
            //如果没有错就表示验证成功
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
            {
                if ((SslPolicyErrors.RemoteCertificateNameMismatch & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    string errMsg = "证书名称不匹配{0}" + sslPolicyErrors;
                    Console.WriteLine(errMsg);
                    throw new AuthenticationException(errMsg);
                }

                if ((SslPolicyErrors.RemoteCertificateChainErrors & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    string msg = "";
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        msg += "status code ={0} " + status.Status;
                        msg += "Status info = " + status.StatusInformation + " ";
                    }
                    string errMsg = "证书链错误{0}" + msg;
                    Console.WriteLine(errMsg);
                    throw new AuthenticationException(errMsg);
                }
                string errorMsg = "证书验证失败{0}" + sslPolicyErrors;
                Console.WriteLine(errorMsg);
                throw new AuthenticationException(errorMsg);
            }
            #endregion
        }

        public static bool CheckStatusOfResponse(string strResponseXml)
        {
            XmlDocument doc = new XmlDocument();
            bool flag = false;
            string id = string.Empty;
            try
            {
                doc.LoadXml(strResponseXml);
                if (doc.DocumentElement.Attributes[0].Value == "fail")
                {
                    //获取错误编码
                    string errCode = doc.DocumentElement.FirstChild.Attributes[0].Value;

                    //获取错误信息
                    string errMsg = doc.DocumentElement.FirstChild.Attributes[1].Value;

                    //抛出异常信息
                    string[] errMsgParameters = new string[] { errCode, errMsg };
                    string exceptionMsg = string.Format("Error Code: {0}, Error Message: {1}", errMsgParameters);
                    throw new Exception(exceptionMsg);
                }
                else
                {
                    flag = true;
                }

                return flag;
            }
            catch (XmlException ex)
            {
                throw new XmlException(ex.Message, ex.InnerException);
            }
        }
        #endregion
    }

    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }
}
