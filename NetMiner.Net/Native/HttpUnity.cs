using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Net.Common;
using NetMiner.Net.Interface;
using NetMiner.Resource;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using NetMiner;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;

namespace NetMiner.Net.Native
{
    /// <summary>
    /// 网络矿工基础http通讯类，默认使用HTTP 1.1协议
    /// </summary>
    public class HttpUnity:IDisposable
    {
        internal static string sMakeCertParamsEE = "-pe -ss my -n \"CN={0}{1}\" -sky exchange -in {2} -is my -eku 1.3.6.1.5.5.7.3.1 -cy end -a sha1 -m 120";
        internal static string sMakeCertParamsRoot = "-r -ss my -n \"CN={0}{1}\" -sky signature -eku 1.3.6.1.5.5.7.3.1 -h 1 -cy authority -a sha1 -m 120";
        internal static string sMakeCertSubjectO = ", O=DO_NOT_TRUST, OU=Created by http://www.netminer.cn";
        internal static string sMakeCertRootCN = "DO_NOT_TRUST_NetMinerRoot";
        private static ICertificateProvider oCertProvider = null;
        private X509Certificate2 x509c = null;

        private string m_workPath = string.Empty;

        /// <summary>
        /// 请求网页数据的缓存，保留最近请求的5个网页数据，防止重复请求。目的是为了降低采集引擎的负责程度。
        /// </summary>
        public static Queue<eResponse> g_CacheResponse = new Queue<eResponse>(5);

        /// <summary>
        /// 传入workpath的目的是为了要找到创建证书的makecert文件，不能用相对路径，如果是在
        /// 采集引擎，相对路径则对应到了system32下了。
        /// </summary>
        /// <param name="workPath"></param>
        public HttpUnity(string workPath)
        {
            m_workPath=workPath;
        }

        ~HttpUnity()
        {

        }

        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 Download 的当前实例使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {

                   
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }

        #endregion

        /// <summary>
        /// 获取指定网址源码
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="webCode">网页编码</param>
        /// <param name="cookie">网页cookie</param>
        /// <param name="startPos">获取网页源码的起始位置</param>
        /// <param name="endPos">获取网页源码的终止位置</param>
        /// <param name="IsCutnr">是否截取回车换行符，默认为true，截取</param>
        /// <param name="IsAjax">是否为Ajax页面</param>
        /// <returns></returns>
        public eResponse RequestUri(Uri rUri, eRequest request, cGlobalParas.RequestMethod Method,WebProxy wProxy)
        {

            eResponse response = GetCache(rUri);
            if (response != null)
                return response;

            response = new Common.eResponse();

            bool isSSL = false;
            if (rUri.Scheme == Uri.UriSchemeHttps)
            {
                isSSL = true;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            }

            

            //判断网页编码
            string PostPara = "";


            HttpWebRequest wReq=(System.Net.HttpWebRequest)WebRequest.Create(rUri);
            wReq.ProtocolVersion =HttpVersion.Version11;

            string qUrl = string.Empty;

            if (request.AllAllowAutoRedirect == true)
                wReq.AllowAutoRedirect = true;
            else
                wReq.AllowAutoRedirect = false;

            //判断是否需要代理
            if (wProxy !=null)
            {
                wReq.Proxy = wProxy;
            }

            #region cookie
            wReq.CookieContainer = request.Cookie;

            #endregion

            #region 通讯header

            wReq.Referer = request.referUrl;

           
                foreach (DictionaryEntry de in request.rHeaders)
                {
                    switch (de.Key.ToString())
                    {

                        case "Accept":
                            wReq.Accept = de.Value.ToString();
                            break;
                        case "User-Agent":
                            wReq.UserAgent = de.Value.ToString();
                        break;
                        case "Connection":
                            wReq.KeepAlive = true;
                            break;
                        case "Content-Type":
                            wReq.ContentType = de.Value.ToString();
                        break;
                        case "Referer":
                            wReq.Referer = de.Value.ToString();
                        break;
                        case "Host":
                            break;
                        default:
                            if (de.Value.ToString().StartsWith("{Cookie"))
                            {
                                //string hValue = de.Value.ToString();
                                //hValue = hValue.Substring(8, hValue.Length - 9).ToLower();

                                //CookieCollection cc = CookieCon.GetCookies(rUri);

                                //for (int ii = 0; ii < cc.Count; ii++)
                                //{
                                //    if (cc[ii].Name.ToLower() == hValue)
                                //    {
                                //        wReq.Headers.Add(de.Key.ToString(), cc[ii].Value);
                                //    }
                                //}
                            }
                            else
                                wReq.Headers.Add(de.Key.ToString(), de.Value.ToString());
                            break;
                    }

                }

               
            if (wReq.UserAgent == null)
                    wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";

            //else
            //{

            //    wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
            //    //wReq.Headers.Add("Accept-Language", "zh-cn,en-us;");
            //    //wReq.KeepAlive = true;
            //    //wReq.Headers.Add("Accept-Encoding", "gzip, deflate");
    
            //}

            #endregion

            #region POST数据

            //判断是否含有POST参数
            if (Method==cGlobalParas.RequestMethod.Post)
            {
                wReq.Method = "POST";

                System.IO.Stream reqStream = wReq.GetRequestStream();
                reqStream.Write(request.Body, 0, request.Body.Length);
                reqStream.Close();

            }
            else
            {
                wReq.Method = "GET";
            }

            #endregion

            //设置页面超时时间为12秒
            wReq.Timeout = 30000;

            #region 判断是否为https 如果是则需要加载证书
            if (isSSL==true)
            {
                #region 加载证书 从本地的证书管理器中查找
                //挂接验证服务端证书的回调

                bool isCreate = false;

            FindCert:

                //Create new X509 store called teststore from the local certificate store.
                X509Store _store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

                _store.Open(OpenFlags.ReadOnly);

                //获取本地主机名称作为证书查找的参数
                string findValue =rUri.Host;
                if (findValue.StartsWith("www", StringComparison.CurrentCultureIgnoreCase))
                {
                    findValue = findValue.Substring(4, findValue.Length - 4);
                }

                string[] dots = findValue.Split('.');
                X509Certificate2Collection _certsCollection = null;
                for (int dotIndex = 0; dotIndex < dots.Length; dotIndex++)
                {
                    findValue = dots[dotIndex];
                    if (findValue.ToLower() != "com" && findValue.ToLower() != "cn" && findValue.ToLower() != "net"
                        && findValue.ToLower() != "org" && findValue.ToLower() != "gov")
                    {
                        _certsCollection = _store.Certificates.Find(X509FindType.FindByIssuerName, findValue, false);

                        if (_certsCollection.Count == 0)
                            _certsCollection = _store.Certificates.Find(X509FindType.FindBySubjectName, findValue, false);

                        if (_certsCollection.Count > 0)
                            break;
                    }
                }

                if (_certsCollection.Count > 0)
                {
                    //x509c = _certsCollection[0];
                    wReq.ClientCertificates.AddRange(_certsCollection);
                }
                else
                {
                    //开始进行手工创建
                    if (isCreate == false)
                    {
                        CreateCert(rUri.Host, false);
                        isCreate = true;
                        goto FindCert;
                    }

                    if (x509c == null)
                    {
                        throw new Exception("无效的证书，请确保证书存在且有效！");
                    }

                    wReq.ClientCertificates.Add(x509c);

                }

                #endregion

            }
            #endregion


            #region  开始请求代码
            System.Net.HttpWebResponse wResp = null;
            System.IO.Stream respStream = null;

            try
            {
                wResp = (System.Net.HttpWebResponse)wReq.GetResponse();
                respStream = wResp.GetResponseStream();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            string strWebData = "";

            string RedirectUrl = "";

            string ResponseContentType = wResp.ContentType;

            if (wResp.StatusCode.ToString() == "302")
            {
                RedirectUrl = wResp.Headers["Location"].ToString();
            }

            //获取返回的cookie
            if (wResp.Cookies.Count > 0 )
            {
                CookieCollection wCookie = wResp.Cookies;   //得到新的cookie：注意这里没考虑cookie合并的情况  

                response.eHeaders.Add("Set-Cookie", wCookie.ToString());

            }

            #region 请求数据的编码、压缩格式转换

            Encoding wCode = Encoding.UTF8;

            bool IsAutoCharset = false;
            switch (request.WebCode)
            {
                case cGlobalParas.WebCode.auto:
                    try
                    {
                        string autoCode = string.Empty;
                        if (string.IsNullOrEmpty(wResp.CharacterSet))
                        {
                            IsAutoCharset = true;
                            wCode = Encoding.Default;
                        }
                        else
                        {
                            autoCode = wResp.CharacterSet;

                            if (autoCode.ToLower() == "iso-8859-1")
                            {
                                IsAutoCharset = true;
                                wCode = Encoding.Default;
                            }
                            else
                                wCode = System.Text.Encoding.GetEncoding(autoCode);
                        }
                    }
                    catch
                    {
                        wCode = Encoding.Default;
                    }

                    break;
                case cGlobalParas.WebCode.gb2312:
                    wCode = Encoding.GetEncoding("gb2312");
                    break;
                case cGlobalParas.WebCode.gbk:
                    wCode = Encoding.GetEncoding("gbk");
                    break;
                case cGlobalParas.WebCode.utf8:
                    wCode = Encoding.UTF8;
                    break;

                case cGlobalParas.WebCode.big5:
                    wCode = Encoding.GetEncoding("big5");

                    break;

                case cGlobalParas.WebCode.Shift_JIS:
                    wCode = Encoding.GetEncoding("Shift_JIS");
                    break;
                default:
                    wCode = Encoding.UTF8;
                    break;
            }


            if (wResp.ContentEncoding == "gzip")
            {
                if (IsAutoCharset == true)
                {
                    GZipStream myGZip = new GZipStream(respStream, CompressionMode.Decompress);

                    //定义一个字节数组
                    byte[] buffer = new byte[0x400];

                    //定义一个流，将数据读出来
                    MemoryStream mReader = new MemoryStream();
                    for (int i = myGZip.Read(buffer, 0, buffer.Length); i > 0; i = myGZip.Read(buffer, 0, buffer.Length))
                    {
                        mReader.Write(buffer, 0, i);
                    }
                    myGZip.Close();

                    string strChar = Encoding.ASCII.GetString(mReader.ToArray());
                    Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.ToString();
                    webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                    if (webCharSet != "")
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    else
                        wCode = Encoding.Default;

                    mReader.Seek((long)0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(mReader, wCode);
                    strWebData = reader.ReadToEnd();

                    mReader.Close();
                    mReader.Dispose();
                    reader.Close();
                    reader.Dispose();

                }
                else
                {
                    GZipStream myGZip = new GZipStream(respStream, CompressionMode.Decompress);
                    System.IO.StreamReader reader;
                    reader = new System.IO.StreamReader(myGZip, wCode);
                    strWebData = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                    myGZip.Close();
                }

            }
            else if (wResp.ContentEncoding.StartsWith("deflate"))
            {
                if (IsAutoCharset == true)
                {
                    DeflateStream myDeflate = new DeflateStream(respStream, CompressionMode.Decompress);

                    //定义一个字节数组
                    byte[] buffer = new byte[0x400];

                    //定义一个流，将数据读出来
                    MemoryStream mReader = new MemoryStream();
                    for (int i = myDeflate.Read(buffer, 0, buffer.Length); i > 0; i = myDeflate.Read(buffer, 0, buffer.Length))
                    {
                        mReader.Write(buffer, 0, i);
                    }
                    myDeflate.Close();

                    string strChar = Encoding.ASCII.GetString(mReader.ToArray());
                    Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.ToString();
                    webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                    if (webCharSet != "")
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    else
                        wCode = Encoding.Default;

                    mReader.Seek((long)0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(mReader, wCode);
                    strWebData = reader.ReadToEnd();

                    mReader.Close();
                    mReader.Dispose();
                    reader.Close();
                    reader.Dispose();
                }
                else
                {
                    DeflateStream myDeflate = new DeflateStream(respStream, CompressionMode.Decompress);
                    System.IO.StreamReader reader;
                    reader = new System.IO.StreamReader(myDeflate, wCode);
                    strWebData = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }
            else
            {
                if (IsAutoCharset == true)
                {
                    //定义一个字节数组
                    byte[] buffer = new byte[0x400];

                    //定义一个流，将数据读出来
                    MemoryStream mReader = new MemoryStream();
                    for (int i = respStream.Read(buffer, 0, buffer.Length); i > 0; i = respStream.Read(buffer, 0, buffer.Length))
                    {
                        mReader.Write(buffer, 0, i);
                    }

                    string strChar = Encoding.ASCII.GetString(mReader.ToArray());
                    Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.ToString();
                    webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                    if (webCharSet != "")
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    else
                        wCode = Encoding.Default;

                    mReader.Seek((long)0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(mReader, wCode);
                    strWebData = reader.ReadToEnd();

                    mReader.Close();
                    mReader.Dispose();
                    reader.Close();
                    reader.Dispose();
                }
                else
                {
                    System.IO.StreamReader reader;
                    reader = new System.IO.StreamReader(respStream, wCode);
                    strWebData = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }

            respStream.Close();
            respStream.Dispose();
           

            #endregion

            #endregion
            
            response.StatusCode = wResp.StatusCode;
            response.cookie = wResp.Cookies;
            response.rUri = rUri;
            response.Body = strWebData;

            wResp.Close();
            wResp = null;
            wReq = null;

            //压入缓存队列
            EnqueueResponse(response);

            return response;

        }

        private void EnqueueResponse(eResponse response)
        {
            if (g_CacheResponse.Count >= 5)
                g_CacheResponse.Dequeue();
            response.rTime = System.DateTime.Now;

            if (response!=null)
                g_CacheResponse.Enqueue(response);
        }

        public void DownloadFile()
        {

        }

        private eResponse GetCache(Uri uri)
        {
            eResponse response = (from a in g_CacheResponse
                                  where a.rUri.Equals(uri) && a.rTime>System.DateTime.Now.AddSeconds(-30)
                                  select a).FirstOrDefault();

            return response;

        }

        public bool RemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;

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

        #region 证书的处理
        /// <summary>
        /// 产生证书
        /// </summary>
        /// <param name="url"></param>
        //private bool CreateCert(string url)
        //{
        //    string mFile = this.m_workPath + "\\components\\makecert.exe";
        //    if (!File.Exists (mFile))
        //    {
        //        return false;
        //    }

        //    Uri u=new Uri (url);
        //    string hUrl=u.Host;
        //    string webSource = cTool.GetHtmlSource(hUrl + ":443", false);

        //    return true;
        //}

        private static readonly object oEECertCreationLock = new object();
        private static readonly object oRootCertCreationLock = new object();
        private bool CreateCert(string sHostname, bool isRoot)
        {
            if (oCertProvider == null)
                oCertProvider = LoadOverrideCertProvider();

            if (!isRoot && !rootCertExists())
            {
                lock (oRootCertCreationLock)
                {
                    if ((FindCert(sMakeCertRootCN, false) == null) && !createRootCert())
                    {
                        //FiddlerApplication.DoNotifyUser("Creation of the root certificate was not successful.", "Certificate Error");
                        return false;
                    }
                }
            }

            if (sHostname.IndexOfAny(new char[] { '"', '\r', '\n' }) == -1)
            {
                int num;
                string str3;
                string path = this.m_workPath + "components\\MakeCert.exe";
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path + " 文件未找到！");
                }
                string sParams = string.Format(isRoot ? sMakeCertParamsRoot : sMakeCertParamsEE, sHostname, sMakeCertSubjectO, sMakeCertRootCN);

                lock (oEECertCreationLock)
                {

                    str3 = GetExecutableOutput(path, sParams, out num);

                    if (num == 0)
                    {
                        Thread.Sleep(150);
                    }

                }
                if (num == 0)
                {
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// 执行命令，在此执行建立证书的命令，并获取执行结果
        /// </summary>
        /// <param name="sExecute"></param>
        /// <param name="sParams"></param>
        /// <param name="iExitCode"></param>
        /// <returns></returns>
        private string GetExecutableOutput(string sExecute, string sParams, out int iExitCode)
        {
            iExitCode = -999;
            StringBuilder builder = new StringBuilder();
            builder.Append("Results from " + sExecute + " " + sParams + "\r\n\r\n");
            try
            {
                string str;
                Process process = new Process
                {
                    StartInfo = { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = false, CreateNoWindow = true, FileName = sExecute, Arguments = sParams }
                };
                process.Start();
                while ((str = process.StandardOutput.ReadLine()) != null)
                {
                    str = str.TrimEnd(new char[0]);
                    if (str != string.Empty)
                    {
                        builder.Append(str + "\r\n");
                    }
                }
                iExitCode = process.ExitCode;
                process.Dispose();
            }
            catch (Exception exception)
            {
                builder.Append("Exception thrown: " + exception.ToString() + "\r\n" + exception.StackTrace.ToString());
            }
            builder.Append("-------------------------------------------\r\n");
            return builder.ToString();
        }

        //创建根证书，是一个网络矿工的根证书
        private bool createRootCert()
        {
            if (oCertProvider != null)
            {
                return oCertProvider.CreateRootCertificate();
            }
            return CreateCert(sMakeCertRootCN, true);
        }

        private bool rootCertExists()
        {
            try
            {
                X509Certificate2 rootCertificate = GetRootCertificate();
                return (null != rootCertificate);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private X509Certificate2 GetRootCertificate()
        {
            return ((oCertProvider != null) ? oCertProvider.GetRootCertificate() : FindCert(sMakeCertRootCN, false));
        }

        private X509Certificate2 FindCert(string sHostname, bool allowCreate)
        {
            if (oCertProvider != null)
            {
                return oCertProvider.GetCertificateForHost(sHostname);
            }
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            string b = string.Format("CN={0}{1}", sHostname, sMakeCertSubjectO);
            X509Certificate2Enumerator enumerator = store.Certificates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                X509Certificate2 current = enumerator.Current;
                if (string.Equals(current.Subject, b, StringComparison.OrdinalIgnoreCase))
                {
                    store.Close();
                    return current;
                }
            }
            store.Close();
            if (!allowCreate)
            {
                return null;
            }
            bool flag = CreateCert(sHostname, false);
            X509Certificate2 certificate2 = FindCert(sHostname, false);
            if (certificate2 == null)
            {
                //FiddlerApplication.Log.LogFormat("!Fiddler.CertMaker> Tried to create cert for {0}, got {1}, but can't find it from thread {2}!", new object[] { sHostname, flag.ToString(), Thread.CurrentThread.ManagedThreadId });
            }
            return certificate2;
        }

        private static ICertificateProvider LoadOverrideCertProvider()
        {
            //string stringPref = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.assembly", CONFIG.GetPath("App") + "CertMaker.dll");
            //if (File.Exists(stringPref))
            //{
            //    Assembly assembly;
            //    try
            //    {
            //        assembly = Assembly.LoadFrom(stringPref);
            //        if (!Utilities.FiddlerMeetsVersionRequirement(assembly, "Certificate Maker"))
            //        {
            //            FiddlerApplication.Log.LogFormat("Assembly {0} did not specify a RequiredVersionAttribute. Aborting load of Certificate Generation module.", new object[] { assembly.CodeBase });
            //            return null;
            //        }
            //    }
            //    catch (Exception exception)
            //    {
            //        FiddlerApplication.LogAddonException(exception, "Failed to load CertMaker" + stringPref);
            //        return null;
            //    }
            //    foreach (Type type in assembly.GetExportedTypes())
            //    {
            //        if ((!type.IsAbstract && type.IsPublic) && (type.IsClass && typeof(ICertificateProvider).IsAssignableFrom(type)))
            //        {
            //            try
            //            {
            //                return (ICertificateProvider)Activator.CreateInstance(type);
            //            }
            //            catch (Exception exception2)
            //            {
            //                FiddlerApplication.DoNotifyUser(string.Format("[Fiddler] Failure loading {0} CertMaker from {1}: {2}\n\n{3}\n\n{4}", new object[] { type.Name, assembly.CodeBase, exception2.Message, exception2.StackTrace, exception2.InnerException }), "Load Error");
            //            }
            //        }
            //    }
            //}
            return null;
        }

        #endregion
    }

    public interface ICertificateProvider
    {
        bool ClearCertificateCache();
        bool CreateRootCertificate();
        X509Certificate2 GetCertificateForHost(string sHostname);
        X509Certificate2 GetRootCertificate();
        bool rootCertIsTrusted(out bool bUserTrusted, out bool bMachineTrusted);
        bool TrustRootCertificate();
    }
}
