using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.IO.Compression;
using NetMiner.Resource;

namespace NetMiner.Net.Socket
{
    /// <summary>
    /// 网络矿工网页请求类
    /// </summary>
    public class HttpUnity
    {
        
        public static string GetHtml(string url, cGlobalParas.RequestMethod m, string postData, cGlobalParas.WebCode pCode,
            ref string cookie, string headers, cGlobalParas.ProxyType proxyType, string proxyAddress, int proxyPort, out string repHeaders)
        {
            HttpWebRequest request = null;
            request = HttpWebRequest.Create(new Uri(url), request, true);
            request.Cookie = cookie;

            string[] headerArray = Regex.Split(headers, "\r\n");

            for (int i = 0; i < headerArray.Length; i++)
            {
                if (!request.Headers.ContainsKey(headerArray[i].Substring(0, headerArray[i].IndexOf(":"))))

                    request.Headers.Add(headerArray[i].Substring(0, headerArray[i].IndexOf(":")),
                        headerArray[i].Substring(headerArray[i].IndexOf(":") + 1, headerArray[i].Length - headerArray[i].IndexOf(":") - 1));
            }
            request.method = m;

            if (m ==cGlobalParas.RequestMethod.Post)
            {
                string PostPara = postData;
                byte[] pPara = Encoding.ASCII.GetBytes(PostPara);

                request.ContentLength = pPara.Length;
                request.postData = pPara;
                request.method = cGlobalParas.RequestMethod.Post;
            }
            else
                request.method = cGlobalParas.RequestMethod.Get;


            request.proxyType = proxyType;
            request.ProxyAddress = proxyAddress;
            request.ProxyPort = proxyPort;

            //设置页面超时时间为12秒
            request.Timeout = 30000;

            #region 判断是否为https 如果是则需要加载证书
            if (request.RequestUri.Scheme == "https")
            {
                #region 加载证书
                //挂接验证服务端证书的回调
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

                //Create new X509 store called teststore from the local certificate store.
                X509Store _store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                X509Certificate2 x509c = null;

                _store.Open(OpenFlags.ReadWrite);

                //获取本地主机名称作为证书查找的参数
                string findValue = (new Uri(url)).Host;
                if (findValue.StartsWith("www", StringComparison.CurrentCultureIgnoreCase))
                {
                    findValue = findValue.Substring(4, findValue.Length - 4);
                }

                findValue = findValue.Substring(0, findValue.IndexOf("."));
                X509Certificate2Collection _certsCollection = _store.Certificates.Find(X509FindType.FindByIssuerName, findValue, false);

                if (_certsCollection.Count == 0)
                    _certsCollection = _store.Certificates.Find(X509FindType.FindBySubjectName, findValue, false);

                if (_certsCollection.Count > 0)
                {
                    x509c = _certsCollection[0];
                }
                else
                {
                    throw new NetCredentialException("未找到采集网站所需的证书信息，采集任务被迫停止！");
                }

                if (x509c == null)
                {
                    throw new NetCredentialException("无效的证书，请确保证书存在且有效！");
                }

                request.Credentials.Add(x509c);

                #endregion
            }
            #endregion

            #region  开始请求代码
            HttpWebResponse wResp = null;

            Stream stream;
            string webSource = string.Empty;
            try
            {
                wResp = request.GetResponse();
                stream = wResp.repStream;
                Encoding webCode = Encoding.Default;
                bool isAutoEncoding = false;

                if (pCode == cGlobalParas.WebCode.auto && wResp.CharacterSet != "")
                {
                    if (wResp.CharacterSet.ToLower() == "utf8")
                        webCode = Encoding.UTF8;
                    else
                        webCode = Encoding.GetEncoding(wResp.CharacterSet);
                }
                else
                {
                    switch (pCode)
                    {
                        case cGlobalParas.WebCode.auto:
                            webCode = Encoding.Default;
                            isAutoEncoding = true;
                            break;
                        case cGlobalParas.WebCode.big5:
                            webCode = Encoding.GetEncoding("big5");
                            break;
                        case cGlobalParas.WebCode.gb2312:
                            webCode = Encoding.GetEncoding("gb2312");
                            break;
                        case cGlobalParas.WebCode.gbk:
                            webCode = Encoding.GetEncoding("gbk");
                            break;
                        case cGlobalParas.WebCode.NoCoding:
                            webCode = Encoding.Default;
                            break;
                        case cGlobalParas.WebCode.utf8:
                            webCode = Encoding.UTF8;
                            break;
                    }
                }

                webSource = ConvertSource(stream, isAutoEncoding, wResp.CompressEncoding, webCode, wResp.repHeader);
            }
            catch (System.Exception ex)
            {
                throw new NetException("http请求出错，错误信息：" + ex.Message);
            }

            cookie = wResp.repCookie;
            repHeaders = wResp.repHeader;

            #endregion

            return webSource;
        }

        internal static bool RemoteCertificateValidationCallback(Object sender,
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
                    throw new NetCredentialException(errMsg);
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
                    throw new NetCredentialException(errMsg);
                }
                string errorMsg = "证书验证失败{0}" + sslPolicyErrors;
                Console.WriteLine(errorMsg);
                throw new NetCredentialException(errorMsg);
            }
            #endregion
        }


        internal static string ConvertSource(Stream stream, bool isAutoEncoding, string cEncoding, Encoding eCode, string repHeader)
        {
            Encoding wCode = Encoding.Default;
            string strWebData = "";
            string webCode = string.Empty;
            wCode = eCode;

            try
            {

                if (cEncoding == null)
                {
                    strWebData = ConvertCommonSource(stream, isAutoEncoding, repHeader, eCode);
                }
                else if (cEncoding.ToLower().IndexOf("gzip") > -1)
                {
                    if (isAutoEncoding)
                    {
                        #region GZIP压缩
                        GZipStream myGZip = new GZipStream(stream, CompressionMode.Decompress);

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
                        if (webCode == "auto")
                        {
                            Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string webCharSet = charSetMatch.ToString();
                            if (webCharSet != "")
                                wCode = System.Text.Encoding.GetEncoding(webCharSet);
                            else
                                wCode = Encoding.Default;
                        }
                        else
                        {
                            wCode = Encoding.GetEncoding(webCode);
                        }

                        mReader.Seek((long)0, SeekOrigin.Begin);
                        StreamReader reader = new StreamReader(mReader, wCode);
                        strWebData = reader.ReadToEnd();

                        mReader.Close();
                        mReader.Dispose();
                        reader.Close();
                        reader.Dispose();
                        #endregion
                    }
                    else
                    {

                        GZipStream myGZip = new GZipStream(stream, CompressionMode.Decompress);
                        System.IO.StreamReader reader;
                        reader = new System.IO.StreamReader(myGZip, wCode);
                        strWebData = reader.ReadToEnd();

                        reader.Close();
                        reader.Dispose();
                        myGZip.Close();
                    }

                }
                else if (cEncoding.ToLower().IndexOf("deflate") > -1)
                {
                    if (isAutoEncoding)
                    {
                        #region diflate压缩
                        DeflateStream myDeflate = new DeflateStream(stream, CompressionMode.Decompress);

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

                        if (webCode == "")
                        {
                            Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string webCharSet = charSetMatch.ToString();
                            if (webCharSet != "")
                                wCode = System.Text.Encoding.GetEncoding(webCharSet);
                            else
                                wCode = Encoding.Default;
                        }
                        else
                        {
                            wCode = Encoding.GetEncoding(webCode);
                        }

                        mReader.Seek((long)0, SeekOrigin.Begin);
                        StreamReader reader = new StreamReader(mReader, wCode);
                        strWebData = reader.ReadToEnd();

                        mReader.Close();
                        mReader.Dispose();
                        reader.Close();
                        reader.Dispose();
                        #endregion
                    }
                    else
                    {
                        DeflateStream myDeflate = new DeflateStream(stream, CompressionMode.Decompress);
                        System.IO.StreamReader reader;
                        reader = new System.IO.StreamReader(myDeflate, wCode);
                        strWebData = reader.ReadToEnd();
                        reader.Close();
                        reader.Dispose();
                    }
                }
                else
                {
                    strWebData = ConvertCommonSource(stream, isAutoEncoding, repHeader, eCode);
                }

            }
            catch
            {
            }



            return strWebData;
        }

        internal static string ConvertCommonSource(Stream stream, bool isAutoEncoding, string repHeader, Encoding webCode)
        {
            string strWebData = string.Empty;

            if (isAutoEncoding)
            {
                //定义一个字节数组
                byte[] buffer = new byte[0x400];

                //定义一个流，将数据读出来
                MemoryStream mReader = new MemoryStream();
                for (int i = stream.Read(buffer, 0, buffer.Length); i > 0; i = stream.Read(buffer, 0, buffer.Length))
                {
                    mReader.Write(buffer, 0, i);
                }

                string strChar = Encoding.ASCII.GetString(mReader.ToArray());
                Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string webCharSet = charSetMatch.ToString();
                webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                if (webCharSet != "")
                    webCode = System.Text.Encoding.GetEncoding(webCharSet);
                else
                    webCode = Encoding.Default;

                mReader.Seek((long)0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(mReader, webCode);
                strWebData = reader.ReadToEnd();
            }
            else
            {
                StreamReader reader = new StreamReader(stream, webCode);
                strWebData = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
            }

            return strWebData;
        }
    }
}
