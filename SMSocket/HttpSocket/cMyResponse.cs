using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Net.Security;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO.Compression;
using System.Collections;
using System.Globalization;
using System.Diagnostics;

///socket请求及响应类
namespace SMSocket.HttpSocket
{
    public class MyWebResponse
    {
        internal static Hashtable g_Domain = new Hashtable();

        public ManualResetEvent wait = new ManualResetEvent(false);
        #region 属性
        public Uri ResponseUri;
        public string ResponseState;
        private int ResponseStateCode;
        public int ContentLength;
        public WebHeaderCollection Headers;     //响应头部信息结合
        public string Header=string.Empty ;                   //响应头部信息字符串
        public string TextViews;
        internal Socket socket;
        public bool KeepAlive;                  //
        public string HTTPVersion;              //HTTP协议版本

        /// <summary>
        /// 请求响应的流数据
        /// </summary>
        private MemoryStream m_responseStreamData;

        private webCode m_wCode;
        
        private webCode m_uCode;
        private MyWebRequest m_request;

        private bool m_isClose;
        private SslStream sStream;                 //接收SSL的流
        private byte[] HeaderBytes;
        
        private string m_CompressEncoding;
        private bool m_isChunked;                  //记录当前是否为Chunked接收
        public bool isChunked
        {
            get { return m_isChunked; }
        }

        private byte[] RecvBuffer;
     
        /// <summary>
        /// 用于接收SSL的请求响应
        /// </summary>
        private List<byte> ResponseBytesList;

        //接收自己的偏移量
        /// <summary>
        /// 接收数据偏移量
        /// </summary>
        private long m_recvOffset;     
        /// <summary>
        /// 头部信息偏移量
        /// </summary>
        private int m_headerOffset;                     
        private long _lngLastChunkInfoOffset;
        private long _lngLeakedOffset;
        private int iEntityBodyOffset;

        private bool m_isReceiveCompleted;
        public bool isReceiveCompleted
        {
            get { return m_isReceiveCompleted; }
        }

        private bool isParseHeader = false;

        private const int m_RecvBufferLength = 0x8000;

        #endregion

        #region 定义回调事件
        public event ReceiveCompleteHandler OnReceiveComplete;
        #endregion

        /// <summary>
        /// 标记当前socked是否连接
        /// </summary>
        private bool m_isConnected;

        public MyWebResponse(MyWebRequest request,webCode wCode,webCode uCode)
        {
            IniData();

            m_wCode = wCode;
            m_uCode = uCode;
            m_request = request;

        }

        ~MyWebResponse()
        {
        }

        /// <summary>
        /// 重新初始化数据
        /// </summary>
        private void IniData()
        {
            Headers = null;
            Header=string.Empty ;                   //响应头部信息字符串
            TextViews = string.Empty;
            m_isClose = false;
            sStream = null;
            HeaderBytes=null;
            m_CompressEncoding = string.Empty;
            m_isChunked = false;
            RecvBuffer = null;
            ResponseBytesList = new List<byte>();
            m_recvOffset=0;
            m_headerOffset=0;

            m_isConnected = false;
            this.m_responseStreamData = new MemoryStream(0x8000);
            this._lngLastChunkInfoOffset = -1L;
            this._lngLeakedOffset = 0;
            this.iEntityBodyOffset = 0;

            wait = new ManualResetEvent(false);

            isParseHeader = false;

            m_LastTime = DateTime.Now;

            m_isReceiveCompleted = false;
        }

        /// <summary>
        /// 连接远程服务器
        /// </summary>
        /// <param name="request"></param>
        public void Connect(MyWebRequest request)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ResponseUri = request.RequestUri;

            socket = new Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP;

            if (request.IsProxy == true)
            {
                remoteEP = new IPEndPoint(GetDomainIP(request.httpProxyAddress.Trim ()), request.httpProxyPort);
            }
            else
            {
                remoteEP = new IPEndPoint(GetDomainIP(ResponseUri.Host.Trim ()), ResponseUri.Port);
            }
           
            socket.Connect(remoteEP);
            
            sw.Stop();
            Console.WriteLine("获取IP：" + sw.ElapsedMilliseconds.ToString());

        }

        /// <summary>
        /// 获取域名的IP地址
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private IPAddress GetDomainIP(string domain)
        {
            bool isExist=false ;
            string ip = string.Empty;

            foreach (DictionaryEntry de in g_Domain)
            {
                if (domain == de.Key.ToString())
                {
                    ip = de.Value.ToString();
                    isExist = true;
                    break;
                }
            }
            IPAddress ipAdd=null;
            if (isExist == true)
                ipAdd = IPAddress.Parse(ip);
            else
            {
                ipAdd = Dns.GetHostAddresses(domain)[0];
                g_Domain.Add(domain, ipAdd.ToString());
            }

            return ipAdd;
        }

        /// <summary>
        /// 发送请求，外部调用接口
        /// </summary>
        /// <param name="request"></param>
        public void SendRequest(MyWebRequest request)
        {
            IniData();

            //确定当前的链接状态
            bool blockingState = socket.Blocking;
            byte[] tmpB=new byte[0];
            try
            {
                socket.Blocking = false;
                socket.Send(tmpB);
            }
            catch (SocketException e)
            {
                if (e.NativeErrorCode.Equals(10035))
                    Console.WriteLine("Still Connected, but the Send would block");
                else
                {
                    Connect(request);
                }
            }
            finally
            {
                socket.Blocking = blockingState;
            }

            SendRequest(request, m_uCode);
         
        }
        /// <summary>
        /// 发送请求实际方法，如果检测到是HTTPS请求，则在此处理证书问题
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uCode"></param>
        public void SendRequest(MyWebRequest request, webCode uCode)
        {
            ResponseUri = request.RequestUri;

            string[] strHeaders = Regex.Split(request.Header, "\r\n");

            //判断host是否存在，如果不存在，则加入
            bool isHost = false;
            for (int i = 0; i < strHeaders.Length; i++)
            {
                if (strHeaders[i].StartsWith("Host", StringComparison.CurrentCultureIgnoreCase))
                {
                    isHost = true;
                    break;
                }
            }

            string aUrl = "";
            if (request.IsProxy == true)
            {
                aUrl = ResponseUri.AbsoluteUri;
            }
            else
            {
                aUrl = ResponseUri.PathAndQuery;
            }
            if (isHost == false)
            {
                if (request.Header == "")
                {
                    request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n"
                        + "Host:" + request.Headers["Host"].ToString() + "\r\n\r\n";
                }
                else
                {
                    request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n"
                        + "Host:" + request.Headers["Host"].ToString() + "\r\n"
                        + request.Header;
                }
            }
            else
            {
                request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n" + request.Header;
            }

            byte[] sBytes = null;

            switch (uCode)
            {
                case webCode.auto:
                    sBytes = Encoding.ASCII.GetBytes(request.Header);
                    break;
                case webCode.big5:
                    sBytes = Encoding.GetEncoding("big5").GetBytes(request.Header);
                    break;
                case webCode.gb2312:
                    sBytes = Encoding.GetEncoding("gb2312").GetBytes(request.Header);
                    break;
                case webCode.gbk:
                    sBytes = Encoding.GetEncoding("gbk").GetBytes(request.Header);
                    break;
                case webCode.NoCoding:
                    sBytes = Encoding.ASCII.GetBytes(request.Header);
                    break;
                case webCode.utf8:
                    sBytes = Encoding.UTF8.GetBytes(request.Header);
                    break;
            }

            if (request.m_IsSsl == true)
            {

                string findValue = request.RequestUri.Host;
                if (findValue.StartsWith("www", StringComparison.CurrentCultureIgnoreCase))
                {
                    findValue = findValue.Substring(4, findValue.Length - 4);

                }

                X509Certificate2 x509 = GetCert(findValue);
                X509Certificate2Collection x509c = new X509Certificate2Collection(x509);

                if (!this.Validate(findValue, x509))
                {
                    return;
                }
                this.sStream.Write(sBytes, 0, sBytes.Length);
            }
            else
            {
                socket.Send(sBytes);
            }
                


        }

        /// <summary>
        /// 发送请求，此方式用于进行文件上传
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uCode"></param>
        /// <param name="fName"></param>
        public void SendRequest(MyWebRequest request, webCode uCode, string fName)
        {
            ResponseUri = request.RequestUri;

            request.Header = request.Method + " " + ResponseUri.PathAndQuery + " HTTP/1.1\r\n" + request.Header;

            //将文件用二进制读出来
            FileStream fs = new FileStream(fName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int fLength = (int)fs.Length;
            byte[] filebyte = new byte[fLength];
            filebyte = br.ReadBytes(fLength);
            br.Close();
            br = null;
            fs.Close();
            fs.Dispose();

            byte[] hdHead = null;
            byte[] hdEnd = null;

            string header = request.Header;
            string[] headers = Regex.Split(header, "\r\n");
            StringBuilder startHeader = new StringBuilder();

            StringBuilder endHeader = new StringBuilder();

            bool isStart = true;
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].StartsWith("Content-Type") && headers[i].IndexOf("--") == -1)
                {
                    if (isStart == true)
                    {
                        startHeader.Append ( headers[i] + "\r\n\r\n");
                        isStart = false;
                    }
                    else
                    {
                        endHeader.Append ( headers[i] + "\r\n");
                    }
                }
                else
                {
                    if (isStart == true)
                    {
                        startHeader.Append (headers[i] + "\r\n");
                    }
                    else
                    {
                        endHeader.Append ( headers[i] + "\r\n");
                    }
                }

            }

            switch (uCode)
            {
                case webCode.auto:
                    hdHead = Encoding.ASCII.GetBytes(startHeader.ToString ());
                    hdEnd = Encoding.ASCII.GetBytes(endHeader.ToString ());
                    break;
                case webCode.big5:
                    hdHead = Encoding.GetEncoding("big5").GetBytes(startHeader.ToString());
                    hdEnd = Encoding.GetEncoding("big5").GetBytes(endHeader.ToString());
                    break;
                case webCode.gb2312:
                    hdHead = Encoding.GetEncoding("gb2312").GetBytes(startHeader.ToString());
                    hdEnd = Encoding.GetEncoding("gb2312").GetBytes(endHeader.ToString());
                    break;
                case webCode.gbk:
                    hdHead = Encoding.GetEncoding("gbk").GetBytes(startHeader.ToString());
                    hdEnd = Encoding.GetEncoding("gbk").GetBytes(endHeader.ToString());
                    break;
                case webCode.NoCoding:
                    hdHead = Encoding.ASCII.GetBytes(startHeader.ToString());
                    hdEnd = Encoding.ASCII.GetBytes(endHeader.ToString());
                    break;
                case webCode.utf8:
                    hdHead = Encoding.UTF8.GetBytes(startHeader.ToString());
                    hdEnd = Encoding.UTF8.GetBytes(endHeader.ToString());
                    break;
            }

            byte[] sendhd = new byte[hdHead.Length + hdEnd.Length + filebyte.Length];
            hdHead.CopyTo(sendhd, 0);
            filebyte.CopyTo(sendhd, hdHead.Length);
            hdEnd.CopyTo(sendhd, hdHead.Length + filebyte.Length);
            socket.Send(sendhd);
        }

        /// <summary>
        /// 设置默认的超时时间
        /// </summary>
        /// <param name="Timeout"></param>
        public void SetTimeout(int Timeout)
        {
            socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Timeout * 1000);
            socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout * 1000);
        }

        /// <summary>
        /// 接收请求响应数据
        /// </summary>
        /// <param name="request"></param>
        public void Receive(MyWebRequest request)
        {
            if (request.m_IsSsl == true)
            {
                ReceiveHeader(request);

                ReceiveData(request);

            }
            else
            {
                //开始接受头信息
                SocketStateObject so2 = new SocketStateObject(socket);
                IAsyncResult ia = socket.BeginReceive(so2.buffer, 0, SocketStateObject.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(AsynchReadCallback), so2);
                wait.WaitOne();
                this.TextViews = ConvertSource(this.m_CompressEncoding, this.m_wCode.ToString());
            }
        }

        /// <summary>
        /// 异步接受响应数据，主要用于非SSL请求
        /// </summary>
        /// <param name="ar"></param>
        private void AsynchReadCallback(System.IAsyncResult ar)
        {

            SocketStateObject so = (SocketStateObject)ar.AsyncState;
            Socket s = so.WorkSocket;
            if (s == null || !s.Connected)
            {
                wait.Set();
                return;
            }

            int read = s.EndReceive(ar);

            if (!isParseHeader )
            {
                ParseHeader(so.buffer, read);
            }

            m_recvOffset += read;
            this.m_responseStreamData.Write(so.buffer, 0,read);

            try
            {
                if (isCompleted())
                {
                    wait.Set();
                    return;
                }
                else
                {
                     s.BeginReceive(so.buffer, 0, SocketStateObject.BUFFER_SIZE, 0, new AsyncCallback(AsynchReadCallback), so);
                }

            }
            catch { wait.Set(); }

        }

        /// <summary>
        /// 接收头部数据，同步接收数据
        /// </summary>
        /// <param name="request"></param>
        private void ReceiveHeader(MyWebRequest request)
        {
           
            StringBuilder strHeader = new StringBuilder();

            #region 接收头部信息
            
            byte[] bytes = new byte[1];
            List<byte> hList = new List<byte>();

            while (true)
            {
                int bLen = 0;
                if (this.sStream.CanRead)
                {
                    bLen = this.sStream.Read(bytes, 0, 1);
                }
                if (bLen <= 0)
                    break;

                strHeader.Append ( Encoding.ASCII.GetString(bytes, 0, 1));

                hList.Add(bytes[0]);

                if (bytes[0] == '\n' && strHeader.ToString ().EndsWith("\r\n\r\n"))
                    break;

            }

            this.HeaderBytes = new byte[hList.Count];
            HeaderBytes = hList.ToArray();

            ParseHeader(HeaderBytes, HeaderBytes.Length );
            
      
            #endregion

            #region 头部的处理 被注销
            //Header = strHeader.ToString();

            //#region 解析头部数据并加载 同时根据头部数据进行相关信息的判断
            //MatchCollection matches = new Regex("[^\r\n]+").Matches(Header.TrimEnd('\r', '\n'));
            //for (int n = 1; n < matches.Count; n++)
            //{
            //    string[] strItem = matches[n].Value.Split(new char[] { ':' }, 2);
            //    if (strItem.Length > 0)
            //        Headers[strItem[0].Trim()] = strItem[1].Trim();
            //}
            //#endregion

            //#region 判断是否为301 302 跳转
            ////处理301及302跳转
            //string firstHeader = matches[0].Value.ToString();
            //reg = Regex.Match(firstHeader, @"\s\d+\s", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //this.ResponseState = reg.ToString().Trim ();

            //if (this.ResponseState == "301" || this.ResponseState == "302")
            //{
            //    if (Headers["Location"] != null)
            //    {
            //        try
            //        {
            //            request.RequestUri = new Uri(Headers["Location"]);
            //            ResponseUri = new Uri(Headers["Location"]);
            //        }
            //        catch
            //        {
            //            ResponseUri = new Uri(ResponseUri, Headers["Location"]);
            //        }

            //    }
               
            //}
         

            //#endregion

            //if (Headers["Transfer-Encoding"] == "chunked")
            //    m_isChunked = true ;
            //else
            //    m_isChunked = false;

            //m_CompressEncoding = Headers["Content-Encoding"];

            //ContentType = Headers["Content-Type"];
            //if (ContentType != null)
            //{
            //    reg = Regex.Match(ContentType, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //    m_wCode = reg.ToString();
            //}

            //if (Headers["Content-Length"] != null)
            //    ContentLength = int.Parse(Headers["Content-Length"]);
            //KeepAlive = (Headers["Connection"] != null && Headers["Connection"].ToLower() == "keep-alive") ||
            //            (Headers["Proxy-Connection"] != null && Headers["Proxy-Connection"].ToLower() == "keep-alive");

            //if (Headers["Connection"] != null && Headers["Connection"].ToLower() == "close")
            //    isClose = true;
            //else
            //    isClose = false;
            #endregion
        }

        /// <summary>
        /// 接受主体数据，当前仅用于SSL的请求接受，
        /// </summary>
        /// <param name="request"></param>
        private void ReceiveData(MyWebRequest request)
        {

            m_recvOffset = 0;

            if (request.m_IsSsl== true)
            {
                if (m_isChunked==true )
                    ResponseBytesList = new List<byte>();
                else
                    RecvBuffer = new byte[this.ContentLength];

                byte[] buffer = new byte[m_RecvBufferLength];
                if (this.sStream.CanRead)
                    this.sStream.BeginRead(buffer, 0, m_RecvBufferLength, new AsyncCallback(this.ProcessSSL),
                        buffer);
                wait.WaitOne();

            }
            else
            {
                //开始初始化接收的数据缓存
                ///此为异步接受事件处理，当前未用
                #region  接受正常数据，仅用于接收非SSL请求，采用异步事件模式 当前未采用
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.AcceptSocket = this.socket;
                RecvBuffer = new byte[m_RecvBufferLength];

                e.SetBuffer(RecvBuffer, 0, m_RecvBufferLength);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(on_Completed);

                bool isS = socket.ReceiveAsync(e);
                if (!isS)
                {
                    onCompleted(e);
                }
                #endregion

            }

        }

        #region 处理各种类型的数据接收，包括SSL 及 CHUNKED
        /// <summary>
        /// 用于接收SSL请求响应数据
        /// </summary>
        /// <param name="ar"></param>
        private void ProcessSSL(IAsyncResult ar)
        {

            int rLen=this.sStream.EndRead(ar);

            if (m_isChunked == true)
            {
                byte[] buffer = new byte[rLen];
                Array.Copy((byte[])ar.AsyncState, buffer, rLen);

                string tempStr = Encoding.ASCII.GetString(buffer, buffer.Length - 5, 5);

                //判断是否接受完毕
                if (tempStr == "0\r\n\r\n")
                {
                    ResponseBytesList.AddRange(buffer);

                    CloseReceiveData();
                    return;
                }

                ResponseBytesList.AddRange(buffer);
            }
            else
            {
                Array.Copy((byte[])ar.AsyncState, 0, RecvBuffer, m_recvOffset, rLen);

                m_recvOffset += rLen;

                //每次接收数据判断是否结束
                if (m_recvOffset >= this.ContentLength && this.ContentLength > 0)
                {
                    CloseReceiveData();
                    return;
                }
            }
            //继续读取数据
            byte[] bytes = new byte[m_RecvBufferLength];
            this.sStream.BeginRead(bytes, 0, m_RecvBufferLength, new AsyncCallback(this.ProcessSSL),
                    bytes);
        }

        /// <summary>
        /// 接收Chunked信息，对于Chunked需要处理两种格式：
        /// 1、真实的网页数据；2、Chunked格式数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bLen"></param>
        //private void ProcessChunked(byte[] bytes, int bLen)
        //{
        //    string tempStr;
        //    if (bytes.Length >5)
        //        tempStr = Encoding.ASCII.GetString(bytes, bLen - 5, 5);
        //    else
        //        tempStr = Encoding.ASCII.GetString(bytes, 0,bLen);

        //    byte[] bs = new byte[bLen];
        //    Array.Copy(bytes, 0, bs, 0, bLen);
        //    //判断是否接受完毕
        //    if (tempStr=="0\r\n\r\n")
        //    {
                
        //        ResponseBytesList.AddRange(bs);

        //        CloseReceiveData();
        //        return;
        //    }

        //    ResponseBytesList.AddRange(bs);
        //}
 
        //private void ProcessByte(byte[] bytes, int bLen)
        //{
        //    Array.Copy(bytes, 0, RecvBuffer, m_recvOffset, bLen);
        //    //bytes.CopyTo(RecvBuffer, recvOffset);

        //    m_recvOffset += bLen;
            
        //    //每次接收数据判断是否结束
        //    if (m_recvOffset >= this.ContentLength && this.ContentLength > 0)
        //    {
        //        CloseReceiveData();
        //    }
        //}

        //private void ProcessCloseByte(byte[] bytes, int bLen)
        //{
        //    ResponseBytesList.AddRange(bytes);
        //}

        /// <summary>
        /// 将字节数转换成字符串
        /// </summary>
        /// <param name="bytes">指定转换字符串的缓存</param>
        /// <param name="cEncoding">网页压缩编码</param>
        /// <param name="webCode">网页编码</param>
        /// <returns></returns>
        private string ConvertSource(string cEncoding, string webCode)
        {
            Encoding wCode;
            string strWebData = "";

            try
            {
                
                if (cEncoding == null)
                {
                    strWebData = ConvertCommonSource(webCode);
                }
                else if (cEncoding.ToLower().IndexOf("gzip") > -1)
                {
                    Stream respStream = this.m_responseStreamData;


                    #region GZIP压缩
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
                    #endregion

                }
                else if (cEncoding.ToLower().IndexOf("deflate") > -1)
                {
                    Stream respStream = this.m_responseStreamData;


                    #region diflate压缩
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
                    #endregion
                }
                else
                {
                    strWebData = ConvertCommonSource(webCode);
                }
            
            }
            catch
            {
            }



            return strWebData;
        }

        private string ConvertCommonSource(string webCode)
        {
            Encoding wCode;

            string strChar = Encoding.ASCII.GetString(this.m_responseStreamData.ToArray());

            if (webCode == "" || webCode=="auto")
            {
                Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string webCharSet = charSetMatch.ToString();
                webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                if (webCharSet != "")
                    wCode = System.Text.Encoding.GetEncoding(webCharSet);
                else
                    wCode = Encoding.Default;
            }
            else if (webCode.ToLower() == "utf8")
            {
                wCode = Encoding.UTF8;
            }
            else
            {
                wCode = Encoding.GetEncoding(webCode);
            }

            m_responseStreamData.Seek((long)0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(m_responseStreamData, wCode);
            string strWebData = reader.ReadToEnd();

          
            reader.Close();
            reader.Dispose();

            return strWebData;
        }

        /// <summary>
        /// 接收数据完成的事件处理，当接收的数据大于1MB时，将通过流进行数据返回
        /// </summary>
        private void CloseReceiveData()
        {
            this.m_responseStreamData.Write(RecvBuffer, 0, RecvBuffer.Length);

            if (m_isChunked == true)
            {
                
                string hSource= ConvertSource( this.m_CompressEncoding, this.m_wCode.ToString ());
                this.TextViews = Regex.Replace(hSource,"[a-f0-9]+?\r\n","",RegexOptions.IgnoreCase | RegexOptions.Multiline );
                
            }
            else if (m_isClose == true)
            {
              
                this.TextViews = ConvertSource(this.m_CompressEncoding, this.m_wCode.ToString());
            }
            else if (m_isChunked == false)
            {
                this.TextViews = ConvertSource( this.m_CompressEncoding, this.m_wCode.ToString());
            }

            if (this.TextViews.Trim() == "")
            {
                string ss = string.Empty;

            }

            wait.Set();

            //触发接收完成事件
            //if (OnReceiveComplete != null)
            //    OnReceiveComplete();
        }
        #endregion

        private readonly object m_CloseLock = new object();
        public void Close()
        {
            lock (m_CloseLock)
            {
                if (m_isReceiveCompleted == false)
                {
                    //处理源码
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    CloseReceiveData();
                    sw.Stop();

                    Console.WriteLine("解析源码：" + sw.ElapsedMilliseconds.ToString());

                    m_isReceiveCompleted = true;

                    //socket.Shutdown(SocketShutdown.Both);
                    //socket.Close();
                    this.m_isConnected = false;

                    
                }
            }
        }

        #region 证书处理部分
        private bool Validate(string serverName,X509Certificate2 x509)
        {

            this.sStream = new SslStream(new NetworkStream(socket, FileAccess.ReadWrite, true),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );

            //this.sStream = new SslStream(new NetworkStream(socket, FileAccess.ReadWrite, true), false);

            try
            {
                this.sStream.AuthenticateAsClient(serverName, new X509Certificate2Collection(x509), SslProtocols.Tls, false);
            }
            catch (AuthenticationException e)
            {
                string strMsg = string.Format("SSL验证失败!\r\nException: {0}", e.Message);
                if (e.InnerException != null)
                {
                    strMsg = string.Format("{1}\r\nInner exception: {0}", e.InnerException.Message, strMsg);
                }

                this.sStream.Close();

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
            return true;
        }

        // 验证证书
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                return false;
            }
            return true;
        }

        //查找证书
        private X509Certificate2 GetCert(string host)
        {
            X509Certificate2 x509c = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

            //Create new X509 store called teststore from the local certificate store.
            X509Store _store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            _store.Open(OpenFlags.ReadWrite);

            //获取本地主机名称作为证书查找的参数
            string findValue = host;
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
                
            }

            if (x509c == null)
            {
                throw new Exception("无效的证书，请确保证书存在且有效！");
            }

            return x509c;

        }

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

        #endregion

        private void on_Completed(object sender, SocketAsyncEventArgs e)
        {
            onCompleted(e);
        }

        Stopwatch gsw = new Stopwatch();

        private void onCompleted(SocketAsyncEventArgs e)
        {
            gsw.Reset();
            gsw.Start();
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:


                    if (e.SocketError == SocketError.Success)
                    {
                        //wait.Set();
                        this.m_isConnected = true;
                       
                        //连接成功后开始发送数据,发送数据为同步发送
                        SendRequest(this.m_request, this.m_uCode);
                      
                        //发送成功后，开始接收数据
                        //ReceiveHeader(this.m_request);

                        //ReceiveData(this.m_request);
                        //this.Close();
                    }
                    else
                    {
                        //wait.Set();
                        this.m_isConnected = false ;
                        //if (OnConnected != null)
                        //    OnConnected(false);
                    }
                    break;
                case SocketAsyncOperation.Send:
                    if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                    {

                        ReceiveData(this.m_request);

                        
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                    {

                        if (On_Receive(e.Buffer, e.BytesTransferred))
                        {
                            e.SetBuffer(0, m_RecvBufferLength);
                            try
                            {
                                if (!socket.ReceiveAsync(e))
                                    onCompleted(e);
                            }
                            catch
                            {
                                if (OnReceiveComplete != null)
                                    OnReceiveComplete();

                                //OnConnected(false);
                            }
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        this.Close();

                        //if (m_isClose == true)
                        //    CloseReceiveData();

                        //if (OnReceiveComplete != null)
                        //    OnReceiveComplete();

                        //if (OnConnected != null)
                        //    OnConnected(false);

                    }
                    break;
            }
            gsw.Stop();

            Console.WriteLine("每次响应:" + gsw.ElapsedMilliseconds.ToString() + " " + e.LastOperation.ToString () );
        }

        private readonly object dataSync = new object();

        //只负责接收数据，接收数据完毕后，统一处理源码问题
        private bool On_Receive(byte[] buffer,int bLen)
        {
           
            Stopwatch sw = new Stopwatch();
            sw.Start();

            m_recvOffset += bLen;

            if (this.Headers == null)
            {
                //处理头部数据
                if (!ParseHeader(buffer, bLen))
                {
                    return false;
                }
           
            }

            bool isS = false;

            this.m_responseStreamData.Write(buffer, 0, bLen);

            if (isCompleted())
            {
                isS= false;
            }
            else
            isS= true;

            sw.Stop();
            Console.WriteLine("on_receive:" + sw.ElapsedMilliseconds.ToString());
     
            return isS;
        }

        private int i1 = 0;
        /// <summary>
        /// 判断是否接受完毕
        /// </summary>
        /// <returns></returns>
        private bool isCompleted()
        {
            i1++;
            
            bool isOver = false;

            if (this.m_isChunked ==true )
            {
                long num;
                if (this._lngLastChunkInfoOffset < this.iEntityBodyOffset)
                {
                    this._lngLastChunkInfoOffset = this.iEntityBodyOffset;
                }
                isOver= cUtilities.IsChunkedBodyComplete(this.m_responseStreamData, this._lngLastChunkInfoOffset, out this._lngLastChunkInfoOffset, out num);
            }
            if (this.Headers["Content-Length"] != null && ("0" != this.Headers["Content-Length"].Trim()))
            {
                long num2;
                if (long.TryParse(this.Headers["Content-Length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2) && (num2 >= 0L))
                {
                    isOver = (this.m_recvOffset >= (this.m_headerOffset + num2+1));
                }
                else
                    isOver = true;
            }
            if (this.m_isClose==true )
            {
            }
            
            return isOver;
        }

        

        private void ReleaseStreamedChunkedData()
        {
            long num;
            if (this.iEntityBodyOffset > this._lngLastChunkInfoOffset)
            {
                this._lngLastChunkInfoOffset = this.iEntityBodyOffset;
            }
            cUtilities.IsChunkedBodyComplete(this.m_responseStreamData, this._lngLastChunkInfoOffset, out this._lngLastChunkInfoOffset, out num);
            int capacity = (int)(this.m_responseStreamData.Length - this._lngLastChunkInfoOffset);
            MemoryStream stream = new MemoryStream(capacity);
            stream.Write(this.m_responseStreamData.GetBuffer(), (int)this._lngLastChunkInfoOffset, capacity);
            this.m_responseStreamData = stream;
            this._lngLeakedOffset = capacity;
            this._lngLastChunkInfoOffset = 0L;
            this.iEntityBodyOffset = 0;
        }

        private void ReleaseStreamedData()
        {
            this.m_responseStreamData = new MemoryStream();
            this._lngLeakedOffset = 0L;
            if (this.iEntityBodyOffset > 0)
            {
                this.m_recvOffset -= this.iEntityBodyOffset;
                this.iEntityBodyOffset = 0;
            }
        }

        /// <summary>
        /// 解析头部数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bLen"></param>
        /// <returns></returns>
        private bool ParseHeader(byte[] bytes,long bLen)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (bLen >= 4)
            {
                HTTPHeaderParseWarnings hHeaderWarnings;
                if (!cUtilities.FindEndOfHeaders(bytes, ref m_headerOffset, bLen, out hHeaderWarnings))
                    return false;

                this.iEntityBodyOffset = this.m_headerOffset + 1;

                if (hHeaderWarnings == HTTPHeaderParseWarnings.EndedWithLFCRLF)
                {

                }
                if (hHeaderWarnings ==HTTPHeaderParseWarnings.EndedWithLFLF)
                {

                }
                this.Header = Encoding.ASCII.GetString(bytes, 0, m_headerOffset).Trim();
                string[] sHeaderLines = this.Header.Replace("\r\n", "\n").Split('\n');

                this.Headers = new WebHeaderCollection();
                if (sHeaderLines.Length > 0)
                {
                    //提取响应的状态及http协议版本
                    int index = sHeaderLines[0].IndexOf(' ');
                    if (index > 0)
                    {
                        this.HTTPVersion = sHeaderLines[0].Substring(0, index).ToUpper();
                        sHeaderLines[0] = sHeaderLines[0].Substring(index + 1).Trim();
                        this.ResponseState = sHeaderLines[0];
                        this.ResponseStateCode = int.Parse (sHeaderLines[0].Substring(0, sHeaderLines[0].IndexOf(' ')).Trim());
                    }

                    #region 在此可以处理根据相应的状态进行的各类信息

                    #endregion

                    for (int i = 1; i < sHeaderLines.Length; i++)
                    {
                        string[] strItem = sHeaderLines[i].Split(new char[] { ':' }, 2);
                        if (strItem.Length > 0)
                            Headers[strItem[0].Trim()] = strItem[1].Trim();
                    }

                    if (Headers["Transfer-Encoding"] == "chunked")
                        m_isChunked = true;
                    else
                        m_isChunked = false;

                    m_CompressEncoding = Headers["Content-Encoding"];

                    if (Headers["Content-Type"] != null)
                    {
                        Match reg = Regex.Match(Headers["Content-Type"], "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        m_wCode =cUtilities.ConvertCode(reg.ToString());
                    }

                    if (Headers["Content-Length"] != null)
                        ContentLength = int.Parse(Headers["Content-Length"]);
                    KeepAlive = (Headers["Connection"] != null && Headers["Connection"].ToLower() == "keep-alive") ||
                                (Headers["Proxy-Connection"] != null && Headers["Proxy-Connection"].ToLower() == "keep-alive");

                    if ((Headers["Connection"] != null && Headers["Connection"].ToLower() == "close") ||
                        (Headers["Proxy-Connection"]!=null && Headers["Proxy-Connection"]=="close"))
                        m_isClose = true;
                    else
                        m_isClose = false;
                }

                sw.Stop();
                Console.WriteLine("解析头部:"  + sw.ElapsedMilliseconds.ToString());
                isParseHeader = true;
                return true;
            }
            else
                return false;

        }

        //计算下载的速率
        public static Single m_DownloadCount;
        public static DateTime m_LastTime;
        public static Single UploadSpeedLimit = 0;
        public static Single DownloadSpeedLimit = 0;
        public static Single UploadSpeed;
        public static Single DownloadSpeed;

        /// <summary>
        /// 计算当前的下载速率
        /// </summary>
        /// <returns></returns>
        private Single CalcSpeed()
        {
            TimeSpan sSpan = DateTime.Now - m_LastTime;

            if (sSpan.Milliseconds > 0)
            {
                DownloadSpeed = (m_DownloadCount/1024) / sSpan.Milliseconds * 1000 ;

                m_LastTime = DateTime.Now;
                m_DownloadCount = 0;
            }
            return DownloadSpeed;
        }

    }

}
