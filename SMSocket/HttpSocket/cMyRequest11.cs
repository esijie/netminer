using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using SoukeyResource;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;

///重构的resquest类，调用socket进行通讯，封装之后，简化客户端的调用
namespace SMSocket.HttpSocket
{
  

    public class cMyRequest11
    {
        public static decimal CurrentDownRate;
        public static decimal CurrentUploadRate;
        
        //定义一个现成，计算当前的下载速率
        private System.Threading.Timer m_DownloadWatch;

        //判断当前是否限速
        private bool m_LimitSpeed;
        private long m_DownByteCount;
        private long m_UploadByteCount;
        
        private int m_LastTime;

        /// <summary>
        /// 缓存 线程同步锁
        /// </summary>
        private readonly Object m_mstreamLock = new Object();

        /// <summary>
        /// 下载数据缓存
        /// </summary>
        private MemoryStream m_mstream;

        /// <summary>
        /// 下载文件
        /// </summary>
        private FileStream m_fs;

        private Thread[] m_DownThread;

        public cMyRequest11(Uri u)
        {
            
            m_uri = u;

            this.MaxDownRate = 0;

            m_AllowAutoRedirect = true;
            m_wCode = webCode.auto;

            m_LastTime = System.Environment.TickCount;

            m_DownloadWatch = new System.Threading.Timer(new System.Threading.TimerCallback(m_DownloadWatch_CallBack), null, 0, 500);
        }

        public cMyRequest11()
        {

            this.MaxDownRate = 0;

            m_AllowAutoRedirect = true;
            m_wCode = webCode.auto;

            m_LastTime = System.Environment.TickCount;

            m_DownloadWatch = new System.Threading.Timer(new System.Threading.TimerCallback(m_DownloadWatch_CallBack), null, 0, 500);
        }

        ~cMyRequest11()
        {
            if (m_DownloadWatch != null)
            {
                m_DownloadWatch.Dispose();
            }
        }

        #region 下载速率控制
        //监控当前的下载速率
        private void m_DownloadWatch_CallBack(object State)
        {
            // 在此检查速度限制
            UpdateSpeed();

            if (CurrentDownRate > m_MaxDownRate && m_MaxDownRate>0)
            {
                m_LimitSpeed = true;
            }
            else
            {
                m_LimitSpeed = false;
            }
        }

        private void UpdateSpeed()
        {
            if (m_LastTime != System.Environment.TickCount)
            {
                int tnow = System.Environment.TickCount;
                long dcount = this.m_DownByteCount;
                if (dcount > 0)
                {
                    long cc = dcount;
                }
                this.m_DownByteCount=0;
                int time = tnow - m_LastTime;
                m_LastTime = tnow;

                CurrentDownRate = ((decimal)(dcount *1000 ) / (decimal)(time*1024));
            }
        }

        #endregion

        #region 属性
        private Uri m_uri;
        public Uri uri
        {
            get { return m_uri; }
            set { m_uri = value; }
        }

        private bool m_AllowAutoRedirect;
        public bool AllowAutoRedirect
        {
            get { return m_AllowAutoRedirect; }
            set { m_AllowAutoRedirect = value; }
        }

        private string m_ProxyAddress;
        public string ProxyAddress
        {
            get { return m_ProxyAddress; }
            set { m_ProxyAddress = value; }
        }

        private int m_ProxyPort;
        public int ProxyPort
        {
            get { return m_ProxyPort; }
            set { m_ProxyPort = value; }
        }

        private decimal m_MaxDownRate;
        public decimal MaxDownRate
        {
            get { return m_MaxDownRate; }
            set { m_MaxDownRate = value; }
        }

        private string m_RequestHeaders;
        public string RequestHeaders
        {
            get { return m_RequestHeaders; }
            set { m_RequestHeaders = value; }
        }

        private method m_Method;
        public method Method
        {
            get { return m_Method; }
            set { m_Method = value; }
        }

        private string m_ResponseText;
        public string ResponseText
        {
            get { return m_ResponseText; }
            set { m_ResponseText = value; }
        }

        private string m_ResponseCookie;
        public string ResponseCookie
        {
            get { return m_ResponseCookie; }
            set { m_ResponseCookie = value; }
        }

        private string m_ResponseHeaders;
        public string ResponseHeaders
        {
            get { return m_ResponseHeaders; }
            set { m_ResponseHeaders = value; }
        }

        private webCode m_wCode;
        public webCode wCode
        {
            get{return m_wCode ;}
            set{m_wCode=value;}
        }

        #endregion

        #region 公共方法

        public void Reset()
        {
            this.uri = null;
            this.m_AllowAutoRedirect = true;
            this.wCode = webCode.auto;
            this.ProxyAddress = string.Empty;
            this.ProxyPort = 0;
            this.MaxDownRate = 0;
            this.RequestHeaders = string.Empty;
            this.ResponseText = string.Empty;
            this.ResponseCookie = string.Empty;
            this.ResponseHeaders = string.Empty;

        }

        public void GetResponse()
        {
            this.ResponseText = string.Empty;
            this.m_ResponseHeaders = string.Empty;

            string strheaders = this.m_RequestHeaders;

            if (strheaders == "")
            {
                strheaders = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n\r\n";
            }

            #region 先将旧的cookie提取出来
            string c1 = "";
            string[] inHeaders = Regex.Split(strheaders, "\r\n");
            for (int i = 0; i < inHeaders.Length; i++)
            {
                if (inHeaders[i] != "")
                {
                    if (inHeaders[i].Trim().StartsWith("cookie", StringComparison.CurrentCultureIgnoreCase))
                    {
                        c1 += inHeaders[i].Substring(inHeaders[i].IndexOf(":") + 1, inHeaders[i].Length
                            - inHeaders[i].IndexOf(":") - 1);
                    }
                }
            }

            string[] oldCookie = c1.Split(';');


            //先把旧cookie添加到一个hashtable中
            Hashtable cookieTable = new Hashtable();
            for (int j = 0; j < oldCookie.Length; j++)
            {
                if (oldCookie[j].ToString() != "")
                {
                    string label = oldCookie[j].Substring(0, oldCookie[j].IndexOf("="));
                    string value = oldCookie[j].Substring(oldCookie[j].IndexOf("=") + 1, oldCookie[j].Length - oldCookie[j].IndexOf("=") - 1);
                    cookieTable.Add(label, value);
                }
            }
            #endregion

            //#region 创建一个http request，同时判断是否为http代理请求
            //Uri u = m_uri;
            //MyWebRequest request = null;
            //if (this.ProxyAddress != "" && this.ProxyAddress !=null)
            //{

            //    request = new MyWebRequest (u, true, ProxyType.HttpProxy);
            //    request.httpProxyAddress = this.ProxyAddress;
            //    request.httpProxyPort = this.ProxyPort;

            //}
            //else
            //{
            //    request = new MyWebRequest (u, false, ProxyType.None);
            //}
            //#endregion

            //if (this.m_Method == method.GET)
            //    request.Method = "GET";
            //else if (this.m_Method == method.POST)
            //    request.Method = "POST";
            //else
            //    return ;

            //request.Header = strheaders;

            ////request.Timeout = 30;

            //MyWebResponse response=null;
            

            //request.GetResponse(this.wCode);
            //this.ResponseHeaders = response.Header;

            ////获取cookie
            //string cEncoding = "utf-8";
            //string webCode = "utf-8";
            //bool isChunked = false;

            //string strCookie = "";

            //#region 根据响应的头信息进行相应的处理

            //if (response.Header != null)
            //{

            //    #region 根据response获取新cookie信息
            //    string[] headers = Regex.Split(response.Header, "\r\n");
            //    for (int i = 0; i < headers.Length; i++)
            //    {

            //        if (headers[i].IndexOf("cookie", StringComparison.CurrentCultureIgnoreCase) > -1)
            //        {
            //            string tmpStr1 = headers[i];
            //            if (tmpStr1.IndexOf(";") < 0)
            //                tmpStr1 += ";";
            //            strCookie += tmpStr1.Substring(tmpStr1.IndexOf(":") + 1, tmpStr1.IndexOf(";") - tmpStr1.IndexOf(":"));

            //        }
            //    }
            //    if (strCookie.EndsWith(";"))
            //    {
            //        strCookie = strCookie.Substring(0, strCookie.Length - 1);
            //    }
            //    #endregion

            //    #region 提取返回的cookie信息，进行cookie合并

            //    string[] newCookie = strCookie.Split(';');

            //    for (int i = 0; i < newCookie.Length; i++)
            //    {
            //        if (newCookie[i].ToString() != "")
            //        {
            //            string label = newCookie[i].Substring(0, newCookie[i].IndexOf("="));
            //            string value = newCookie[i].Substring(newCookie[i].IndexOf("=") + 1, newCookie[i].Length - newCookie[i].IndexOf("=") - 1);
            //            if (cookieTable.ContainsKey(label))
            //            {
            //                cookieTable.Remove(label);
            //                cookieTable.Add(label, value);
            //            }
            //            else
            //            {
            //                cookieTable.Add(label, value);
            //            }
            //        }
            //    }

            //    //将cookie转换成字符串
            //    string rCookie = "";
            //    foreach (DictionaryEntry de in cookieTable)
            //    {
            //        rCookie += de.Key + "=" + de.Value + ";";
            //    }
            //    if (rCookie != "")
            //        rCookie = rCookie.Substring(0, rCookie.Length - 1);

            //    this.ResponseCookie = rCookie;
            //    #endregion

            //    #region 处理302跳转
            //    if (response.Header.StartsWith("HTTP/1.1 302") && AllowAutoRedirect == true ||
            //        response.Header.StartsWith("HTTP/1.1 301") && AllowAutoRedirect == true)
            //    {
            //        int start = response.Header.ToUpper().IndexOf("LOCATION");

            //        if (start > 0)
            //        {
            //            string temp = response.Header.Substring(start, response.Header.Length - start);
            //            string[] sArry = Regex.Split(temp, "\r\n");
            //            string rUrl = sArry[0].Remove(0, 10);

            //            response.Close();
            //            request = null;

            //            this.Method = method.GET;
            //            GetResponse();
            //        }

            //    }
            //    #endregion

            //    //根据头信息，判断编码
            //    for (int i = 0; i < headers.Length; i++)
            //    {
            //        string ss = headers[i].ToString();
            //        if (ss.StartsWith("Content-Encoding"))
            //        {
            //            cEncoding = ss.Substring(17, ss.Length - 17);

            //        }
            //        if (ss.StartsWith("Content-Type"))
            //        {
            //            Match charSetMatch = Regex.Match(ss, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //            webCode = charSetMatch.ToString();
            //        }
            //        if (ss.StartsWith("Transfer-Encoding"))
            //        {
            //            string s1 = ss.Substring(18, ss.Length - 18);
            //            if (s1.IndexOf("chunked") > -1)
            //            {
            //                isChunked = true;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    this.ResponseCookie = c1;
            //}

            //#endregion 


            //#region 接收源码

            //if (request.m_IsSsl == true)
            //{
            //    #region 处理SSL的字节接收

            //    byte[] RecvBuffer = new byte[1024];
            //    int nBytes=0, nTotalBytes = 0;

            //    byte[] sourceByte = null;

            //    if (isChunked == true)
            //    {
            //        byte[] b1 = new byte[1];
            //        string sLen = "";
            //        List<byte> ResponseBytesList = new List<byte>();

            //        while (true)
            //        {
            //            int b1Len = 0;
            //            if (response.sStream.CanRead)
            //            {
            //                b1Len = response.sStream.Read(b1, 0, 1);//返回实际接收内容的字节数,循环读取，直到接收完所有数据 
            //            }

            //            if (b1Len <= 0)
            //                break;

            //            m_DownByteCount += b1Len;

            //            //控制速率和监测速率
            //            while (m_LimitSpeed)
            //            {
            //                Thread.Sleep(100);
            //            }


            //            sLen += Encoding.ASCII.GetString(b1, 0, 1);

            //            if (b1[0] == '\n' && sLen.EndsWith("0\r\n\r\n"))
            //                break;

            //            if (b1[0] == '\n' && sLen.EndsWith("\r\n"))
            //            {
            //                if (sLen == "0\r\n")
            //                {
            //                    //表示有可能是结束符
            //                }
            //                else
            //                {
            //                    sLen = sLen.Substring(0, sLen.Length - 2);

            //                    if (sLen != "")
            //                    {
            //                        int iLen = Convert.ToInt32(sLen, 16);

            //                        byte[] tByte = new byte[iLen];
            //                        for (int ti = 0; ti < iLen; ti++)
            //                        {
            //                            byte[] b2 = new byte[1];
            //                            response.sStream.Read(b2, 0, 1);
            //                            tByte[ti] = b2[0];
            //                        }


            //                        if (tByte == null)
            //                            break;

            //                        ResponseBytesList.AddRange(tByte);
            //                        sLen = "";
            //                    }
            //                }
            //            }


            //        }

            //        sourceByte = new byte[ResponseBytesList.Count];
            //        sourceByte = ResponseBytesList.ToArray();

            //    }
            //    else
            //    {
            //        int cLen = response.ContentLength;
            //        sourceByte = new byte[cLen];
            //        int iLen = 1024;
            //        if (iLen > cLen)
            //        {
            //            iLen = cLen;
            //            RecvBuffer = new byte[iLen];
            //        }

            //        while (true)
            //        {
            //            if (response.sStream.CanRead)
            //            {
            //                nBytes = response.sStream.Read(RecvBuffer, 0, iLen);//返回实际接收内容的字节数,循环读取，直到接收完所有数据 
            //            }

            //            if (nBytes <= 0)
            //                break;

            //            this.m_DownByteCount += nBytes;

            //            //控制速率和监测速率
            //            while (m_LimitSpeed)
            //            {
            //                Thread.Sleep(100);
            //            }


            //            try
            //            {
            //                RecvBuffer.CopyTo(sourceByte, nTotalBytes);
            //            }
            //            catch
            //            {
            //                break;
            //            }

            //            nTotalBytes += nBytes;

            //            if (nTotalBytes >= response.ContentLength && response.ContentLength > 0)
            //                break;

            //            if (cLen - nTotalBytes < iLen)
            //            {
            //                iLen = cLen - nTotalBytes;
            //                RecvBuffer = new byte[iLen];
            //            }

            //        }
            //    }
            //    this.ResponseText += ConvertSource(sourceByte, cEncoding, webCode, false);

            //    #endregion

            //}
            //else
            //{
            //    #region 处理普通页面的数据接收

            //    byte[] RecvBuffer = new byte[1024];
            //    int nBytes, nTotalBytes = 0;

            //    byte[] sourceByte = null;

            //    if (isChunked == true)
            //    {
            //        byte[] b1 = new byte[1];
            //        string sLen = "";
            //        List<byte> ResponseBytesList = new List<byte>();

            //        while (response.socket.Receive(b1, 0, 1, System.Net.Sockets.SocketFlags.None) > 0)
            //        {
            //            sLen += Encoding.ASCII.GetString(b1, 0, 1);

            //            if (b1[0] == '\n' && sLen.EndsWith("0\r\n\r\n"))
            //                break;

            //            this.m_DownByteCount += 1;

            //            //控制速率和监测速率
            //            while (m_LimitSpeed)
            //            {
            //                Thread.Sleep(100);
            //            }

            //            if (b1[0] == '\n' && sLen.EndsWith("\r\n"))
            //            {
            //                if (sLen == "0\r\n")
            //                {
            //                    //表示有可能是结束符
            //                }
            //                else
            //                {
            //                    sLen = sLen.Substring(0, sLen.Length - 2);

            //                    if (sLen != "")
            //                    {
            //                        int iLen = Convert.ToInt32(sLen, 16);

            //                        byte[] tByte = new byte[iLen];
            //                        for (int ti = 0; ti < iLen; ti++)
            //                        {
            //                            byte[] b2 = new byte[1];
            //                            response.socket.Receive(b2, 0, 1, System.Net.Sockets.SocketFlags.None);
            //                            tByte[ti] = b2[0];
            //                        }


            //                        if (tByte == null)
            //                            break;

            //                        ResponseBytesList.AddRange(tByte);
            //                        sLen = "";
            //                    }
            //                }
            //            }


            //        }

            //        sourceByte = new byte[ResponseBytesList.Count];
            //        sourceByte = ResponseBytesList.ToArray();

            //    }
            //    else
            //    {
            //        int cLen = response.ContentLength;
            //        sourceByte = new byte[cLen];
            //        int iLen = 1024;
            //        if (iLen > cLen)
            //        {
            //            iLen = cLen;
            //            RecvBuffer = new byte[iLen];
            //        }

            //        while ((nBytes = response.socket.Receive(RecvBuffer, 0, iLen, System.Net.Sockets.SocketFlags.None)) > 0)
            //        {
            //            //System.Threading.Thread.Sleep(500);

            //            try
            //            {
            //                RecvBuffer.CopyTo(sourceByte, nTotalBytes);
            //            }
            //            catch
            //            {
            //                break;
            //            }

            //            nTotalBytes += nBytes;

            //            if (nTotalBytes >= response.ContentLength && response.ContentLength > 0)
            //                break;

            //            this.m_DownByteCount += nBytes;

            //            //控制速率和监测速率
            //            while (m_LimitSpeed)
            //            {
            //                Thread.Sleep(100);
            //            }

            //            if (cLen - nTotalBytes < iLen)
            //            {
            //                iLen = cLen - nTotalBytes;
            //                RecvBuffer = new byte[iLen];
            //            }

            //        }
            //    }
            //    this.ResponseText += ConvertSource(sourceByte, cEncoding, webCode, false);
                
            //    #endregion

            //}
           
            //#endregion

            //response.Close();
            //request = null;

            
        }

        public void UploadFile(string url, string strHeader, cGlobalParas.WebCode uCode, string FileName)
        {
            //MyWebRequest request = null;
            //request = new MyWebRequest (new Uri(url),  false, ProxyType.None);
            //request.Method = "POST";
            //request.Header = strHeader;

            //MyWebResponse response = request.GetResponse(uCode, FileName);

            ////获取cookie
            //string cEncoding = "utf-8";
            //string webCode = "utf-8";
            //bool isChunked = false;

            //string strCookie = "";
            //if (response.Header != null)
            //{
            //    string[] headers = Regex.Split(response.Header, "\r\n");
            //    for (int i = 0; i < headers.Length; i++)
            //    {

            //        if (headers[i].IndexOf("cookie", StringComparison.CurrentCultureIgnoreCase) > 0)
            //        {
            //            strCookie += headers[i].Substring(headers[i].IndexOf(":") + 1, headers[i].IndexOf(";") - headers[i].IndexOf(":"));
            //        }
            //    }
            //    if (strCookie.EndsWith(";"))
            //    {
            //        strCookie = strCookie.Substring(0, strCookie.Length - 1);
            //    }

            //    //开始接收源码信息
            //    for (int i = 0; i < headers.Length; i++)
            //    {
            //        string ss = headers[i].ToString();
            //        if (ss.StartsWith("Content-Encoding"))
            //        {
            //            cEncoding = ss.Substring(17, ss.Length - 17);

            //        }
            //        if (ss.StartsWith("Content-Type"))
            //        {
            //            Match charSetMatch = Regex.Match(ss, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //            webCode = charSetMatch.ToString();
            //        }
            //        if (ss.StartsWith("Transfer-Encoding"))
            //        {
            //            string s1 = ss.Substring(18, ss.Length - 18);
            //            if (s1.IndexOf("chunked") > -1)
            //            {
            //                isChunked = true;
            //            }
            //        }
            //    }
            //}
            //byte[] RecvBuffer = new byte[response.socket.ReceiveBufferSize];
            ////response.socket.re
            //response.socket.Receive(RecvBuffer, 0, response.socket.ReceiveBufferSize, System.Net.Sockets.SocketFlags.None);


            //string webSource = ConvertSource(RecvBuffer, cEncoding, webCode, isChunked);

            //response.Close();
            //request = null;

        }

        public void DownloadFileAsyn(string saveFile, string url, string header,int downThread)
        {

            //StringBuilder strheaders = new StringBuilder(header); ;

            //if (strheaders.Length==0)
            //{
            //    strheaders.Append ("User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n\r\n");
            //}

            ////处理头部信息
            //bool isContentType = false;
            //string[] headers=header.Split ('\r');
            //for (int i = 0; i < headers.Length; i++)
            //{
            //    if (headers[i].StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        isContentType = true;
            //        break;
            //    }
            //}
            //if (isContentType == false)
            //{
            //    strheaders.Insert(0, "Content-Type: application/octet-stream\r\n");
            //}

            //#region 创建一个http request，同时判断是否为http代理请求
            //Uri u = new Uri(url);
            //MyWebRequest request = null;
            //if (this.ProxyAddress != "" && this.ProxyAddress != null)
            //{

            //    request = new MyWebRequest (u,true, ProxyType.HttpProxy);
            //    request.httpProxyAddress = this.ProxyAddress;
            //    request.httpProxyPort = this.ProxyPort;

            //}
            //else
            //{
            //    request =  new MyWebRequest (u, false, ProxyType.None);
            //}
            //#endregion
          
            //request.Method = "GET";
            //request.Header = strheaders.ToString ();

            
            //MyWebResponse response = null;
            //request.GetResponse(this.wCode);
            //this.ResponseHeaders = response.Header;

          
            //#region 根据响应的头信息进行相应的处理
            //if (response.Header != null)
            //{
            //    #region 处理302跳转
            //    if (response.Header.StartsWith("HTTP/1.1 302") && AllowAutoRedirect == true ||
            //        response.Header.StartsWith("HTTP/1.1 301") && AllowAutoRedirect == true)
            //    {
            //        int start = response.Header.ToUpper().IndexOf("LOCATION");

            //        if (start > 0)
            //        {
            //            string temp = response.Header.Substring(start, response.Header.Length - start);
            //            string[] sArry = Regex.Split(temp, "\r\n");
            //            string rUrl = sArry[0].Remove(0, 10);

            //            response.Close();
            //            request = null;

            //            this.Method = method.GET;
            //            DownloadFileAsyn(saveFile, rUrl, header, downThread);
            //        }

            //    }
            //    #endregion
            //}
            //#endregion


            ////开始对下载的文件进行分块处理
            ////SplitDownData(response.socket, response.ContentLength, 2);
            //m_mstream = new MemoryStream();

            //#region 同步接收下载数据
            ////byte[] RecvBuffer = new byte[1024];
            ////response.socket.BeginReceive (RecvBuffer ,SocketFlags.None ,new IAsyncResult (
          

            //#endregion

            //#region 异步接收下载数据
            //m_fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.Write);

            //byte[] RecvBuffer = new byte[1024];

            //SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            //e.Completed += new EventHandler<SocketAsyncEventArgs>(this.on_rComplete);
            //e.UserToken = response;
            //e.AcceptSocket = response.socket;
            //e.SetBuffer(RecvBuffer, 0, 1024);
            //bool isS = response.socket.ReceiveAsync(e);
            //if (!isS)
            //    this.on_rComplete(this, e);

            //#endregion

            //response.Close();
            //request = null;

        }

        public void GetResponseAsyn()
        {
            //this.ResponseText = string.Empty;
            //this.m_ResponseHeaders = string.Empty;

            //string strheaders = this.m_RequestHeaders;

            //if (strheaders == "")
            //{
            //    strheaders = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n\r\n";
            //}

 
            //#region 创建一个http request，同时判断是否为http代理请求
            //Uri u = m_uri;
            //MyWebRequest request = null;
            //request = new MyWebRequest(u, false, ProxyType.None);
            
            //#endregion

            //if (this.m_Method == method.GET)
            //    request.Method = "GET";
            //else if (this.m_Method == method.POST)
            //    request.Method = "POST";
            //else
            //    return;

            //request.Header = strheaders;

            ////request.Timeout = 30;

            //MyWebResponse response = null;
            

            

        }
        #endregion


        #region 异步接收数据方法
        private delegate void delegateReceive(Socket socket,long ContentLength);
        /// <summary>
        /// 代理异步执行接收数据
        /// </summary>
        private void ReceiveSocketData(Socket socket,long ContentLength)
        {
            byte[] RecvBuffer = new byte[1024];
            int nBytes = 0;
            int nTotalBytes = 0;

            while ((nBytes = socket.Receive(RecvBuffer, 0, 1024, System.Net.Sockets.SocketFlags.None)) > 0)
            {
                m_mstream.Write(RecvBuffer, 0, nBytes);

                nTotalBytes += nBytes;

                this.m_DownByteCount += nBytes;

                //控制速率和监测速率
                while (m_LimitSpeed)
                {
                    Thread.Sleep(100);
                }


                if (nTotalBytes >= (ContentLength))
                    break;
            }
        }

        private void on_rComplete(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                MyWebResponse response = e.UserToken as MyWebResponse;

                if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                {
                    m_mstream.Write(e.Buffer, 0, e.BytesTransferred);

                    this.m_DownByteCount += e.BytesTransferred;

                    //控制速率和监测速率
                    while (m_LimitSpeed)
                    {
                        Thread.Sleep(100);
                    }

                    bool isS = response.socket.ReceiveAsync(e);
                    if (!isS)
                        this.on_rComplete(this, e);


                }
                else
                {
                    CloseSocket(e);
                }
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
                    
        }

        private void CloseSocket(SocketAsyncEventArgs e)
        {
            WriteToFile();
            MyWebResponse resposner = (MyWebResponse)e.UserToken;
            resposner.Close();
            resposner = null;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将字节数转换成字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="cEncoding"></param>
        /// <param name="webCode"></param>
        /// <param name="isChunked"></param>
        /// <returns></returns>
        private string ConvertSource(byte[] bytes, string cEncoding, string webCode, bool isChunked)
        {
            Encoding wCode;
            string strWebData = "";

            try
            {

                Stream respStream = new MemoryStream(bytes);

                if (cEncoding.ToLower().IndexOf("gzip") > -1)
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


                }
                else if (cEncoding.ToLower().IndexOf("deflate") > -1)
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

                }
                else
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

                    if (webCode == "")
                    {
                        Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string webCharSet = charSetMatch.ToString();
                        webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
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

                }
            }
            catch
            {
            }

            return strWebData;
        }

        /// <summary>
        /// 对需要下载的数据进行分块,并开始下载数据
        /// </summary>
        /// <param name="fileLength"></param>
        //private void SplitDownData(Socket socket,long fileLength,int downThread)
        //{
        //    //判断每块的大小
        //    int blockLen = (int)(fileLength / downThread);
        //    if ((fileLength % downThread) > 0)
        //    {
        //        downThread++;
        //    }

        //    //开始设置分块数量
        //    cDownBlockData[] dDatas = new cDownBlockData[downThread];
        //    m_mstream = new MemoryStream();

        //    m_DownThread = new Thread[downThread];

        //    long fileOffset = 0;
        //    for (int i = 0; i < downThread; i++)
        //    {
        //        dDatas[i] = new cDownBlockData();

        //        dDatas[i].BeginIndex = fileOffset;
        //        dDatas[i].CurIndex = fileOffset;
        //        fileOffset +=blockLen;
        //        if (fileOffset > fileLength)
        //            fileOffset = fileLength;
        //        dDatas[i].EndIndex = fileOffset;

        //        m_DownThread[i] = new Thread(new ThreadStart(delegate { this.DownloadData(socket, dDatas[i]); }));
        //        m_DownThread[i].IsBackground =true ;
        //        m_DownThread[i].Start ();
        //    }
        
        //}

        #endregion

        #region 多线程接收数据
        //private void DownloadData(Socket socket, cDownBlockData bData)
        //{
        //    byte[] RecvBuffer = new byte[1024];
        //    int nBytes = 0;
        //    int nTotalBytes = 0;

        //    long startIndex = bData.BeginIndex;
        //    long endIndex = bData.EndIndex;

        //    while ((nBytes = socket.Receive(RecvBuffer, 0, 1024, System.Net.Sockets.SocketFlags.Partial)) > 0)
        //    {

        //        nTotalBytes += nBytes;

        //        this.m_DownByteCount += nBytes;

        //        //控制速率和监测速率
        //        while (m_LimitSpeed)
        //        {
        //            Thread.Sleep(100);
        //        }

        //        if (nTotalBytes >= (bData.BeginIndex + bData.EndIndex))
        //            break;
        //    }
        
        //}


        /// <summary>
        /// 写入内存缓冲区
        /// [在子线程内触发事件]
        /// </summary>
        /// <param name="buffer">数据源数组</param>
        /// <param name="offset">buffer 中的字节偏移量，从此处开始写入</param>
        /// <param name="count">写入的字节数</param>
        public void WriteToMemory(byte[] buffer, int offset, int count)
        {
            lock (m_mstreamLock)
            {
                m_mstream.Write(buffer, offset, count);
            }
          
        }

        /// <summary>
        /// 将内存中的缓冲数据写入文件
        /// [在主线程触发事件]
        /// </summary>
        /// <param name="fs">文件流对象</param>
        public void WriteToFile()
        {
            // 注意：停止任务时，FileStream已关闭，但可能会与全局保存冲突
            if (m_mstream.Length > 0 )
            {
                MemoryStream tmpStream;
                lock (m_mstreamLock)
                {
                    tmpStream = m_mstream;
                    m_mstream = new MemoryStream();
                }
                long len = tmpStream.Length;
                if (len > 0)
                {

                    m_fs.Write(tmpStream.ToArray(), 0, (int)len);
                   
                    tmpStream.Close();

                  
                }
            }

            m_fs.Close ();
            m_fs.Dispose ();
        }
        #endregion

    }
}
