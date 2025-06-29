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

///socket请求及响应类
namespace  NetMinerHttpHelp.Net
{
    public class HttpWebResponse
    {
        public static Hashtable g_Domain = new Hashtable();

        public ManualResetEvent wait = new ManualResetEvent(false);
        #region 属性
        public Uri ResponseUri;
        public string ResponseState;
        private int ResponseStateCode;
        public int ContentLength;
        public WebHeaderCollection Headers;     //响应头部信息结合
        /// <summary>
        /// 响应头信息
        /// </summary>
        public string repHeader = string.Empty;
        public string repCookie = string.Empty;

        /// <summary>
        /// 响应流信息
        /// </summary>
        public Stream repStream;

        private string reqHeader = string.Empty;   //请求header信息
        internal Socket socket;
        public bool KeepAlive;                  //
        internal string HTTPVersion;              //HTTP协议版本

        /// <summary>
        /// 请求响应的流数据
        /// </summary>
        private MemoryStream m_responseStreamData;

        private enCodingCode m_wCode;
        public string CharacterSet
        {
            get 
            {
                if (m_wCode == enCodingCode.auto)
                    return "";
                else
                    return m_wCode.ToString();
            }
        }
        private HttpWebRequest m_request;

        private bool m_isClose;
        private SslStream sStream;                 //接收SSL的流
        private byte[] HeaderBytes;

        private string m_CompressEncoding;
        public string CompressEncoding
        {
            get { return m_CompressEncoding; }
        }
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
        /// <summary>
        /// 主体信息偏移量
        /// </summary>
        private int iEntityBodyOffset;

        private bool m_isReceiveCompleted;

        private bool isParseHeader = false;

        private const int m_RecvBufferLength = 0x8000;

        #endregion

        #region 定义回调事件
        internal event ReceiveCompleteHandler OnReceiveComplete;
        #endregion

        /// <summary>
        /// 标记当前socked是否连接
        /// </summary>
        private bool m_isConnected;

        public HttpWebResponse (HttpWebRequest request)
        {
            IniData();

            m_request = request;

        }

        ~HttpWebResponse()
        {
        }

        /// <summary>
        /// 重新初始化数据
        /// </summary>
        private void IniData()
        {
            Headers = null;
            repHeader = string.Empty;                   //响应头部信息字符串
            m_isClose = false;
            sStream = null;
            HeaderBytes = null;
            m_CompressEncoding = string.Empty;
            m_isChunked = false;
            RecvBuffer = null;
            ResponseBytesList = new List<byte>();
            m_recvOffset = 0;
            m_headerOffset = 0;

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
        public void Connect(HttpWebRequest request)
        {
            ResponseUri = request.RequestUri;

            socket = new Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP=null;

            if (request.proxyType == ProxyType.HttpProxy)
            {
                remoteEP = new IPEndPoint(GetDomainIP(request.ProxyAddress.Trim()), request.ProxyPort);
            }
            else if (request.proxyType == ProxyType.None)
            {
                remoteEP = new IPEndPoint(GetDomainIP(ResponseUri.Host.Trim()), ResponseUri.Port);
            }
            else if (request.proxyType == ProxyType.SystemProxy)
            {
                IWebProxy wProxy = System.Net.HttpWebRequest.GetSystemWebProxy();
                Uri proxyUri = wProxy.GetProxy(ResponseUri);
                remoteEP = new IPEndPoint(GetDomainIP(proxyUri.Host), proxyUri.Port);
                if (proxyUri == ResponseUri)
                {
                    request.ProxyAddress = "";
                    request.ProxyPort = 80;
                }
                else
                {
                    request.ProxyAddress = proxyUri.Host;
                    request.ProxyPort = proxyUri.Port;
                }
            }
            else if (request.proxyType == ProxyType.Socket5)
            {
                //链接Socket5服务器
                remoteEP = new IPEndPoint(GetDomainIP(request.ProxyAddress.Trim()), request.ProxyPort);
            }

            socket.Connect(remoteEP);

            if (request.proxyType == ProxyType.Socket5)
            {
                //进行验证握手

                byte[] tmpB = new byte[3];
                tmpB[0]=0x05;
                tmpB[1]=0x01;
                tmpB[2]=0x00;
                socket.Send(tmpB);

                //接受验证的响应信息
                byte[] rFlag=new  byte[2];
                socket.Receive(rFlag);

                if (rFlag[1] != 0)
                {
                    throw new NetException("连接Socket5代理服务器失败！");
                }

                //验证成功，开始链接远程服务器
                tmpB = new byte[10];
                tmpB[0] = 0x05;
                tmpB[1] = 0x01;
                tmpB[2] = 0x00;
                tmpB[3] = 0x01;

                IPEndPoint remoteScoketEP = new IPEndPoint(GetDomainIP(ResponseUri.Host.Trim()), ResponseUri.Port);

                Array.Copy(remoteScoketEP.Address.GetAddressBytes(), 0, tmpB, 4, 4);
                Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(remoteScoketEP.Port)), 2, tmpB, 8, 2);

                socket.Send(tmpB);

                rFlag = new byte[10];
                socket.Receive(rFlag);
                if (rFlag[1] != 0)
                {
                    throw new NetException("连接Socket5代理服务器失败！");
                }
            }


        }

        /// <summary>
        /// 获取域名的IP地址
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private IPAddress GetDomainIP(string domain)
        {
            bool isExist = false;
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
            IPAddress ipAdd = null;
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
        public void SendRequest(HttpWebRequest request)
        {
            IniData();

            //确定当前的链接状态
            bool blockingState = socket.Blocking;
            byte[] tmpB = new byte[0];
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

            SendRequestContinue(request);

        }
        /// <summary>
        /// 发送请求实际方法，如果检测到是HTTPS请求，则在此处理证书问题
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uCode"></param>
        public void SendRequestContinue(HttpWebRequest request)
        {
            ResponseUri = request.RequestUri;

            if (!request.Headers.ContainsKey("User-Agent"))
            {
                request.Headers.Add("User-Agent", "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            }

            if (request.method == Method.POST && !request.Headers.ContainsKey("Content-Type"))
            {
                request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            }

            if (request.method == Method.POST && !request.Headers.ContainsKey("Content-Length"))
            {
                request.Headers.Add("Content-Length", request.ContentLength);
            }
         
            string aUrl = "";
            if (request.proxyType == ProxyType.HttpProxy)
            {
                aUrl = ResponseUri.AbsoluteUri;
            }
            else if (request.proxyType == ProxyType.None)
            {
                aUrl = ResponseUri.PathAndQuery;
            }
            else if (request.proxyType == ProxyType.Socket5)
            {
                aUrl = ResponseUri.AbsoluteUri;
            }
            else if (request.proxyType == ProxyType.SystemProxy)
            {
                if (request.ProxyAddress == "")
                {
                    aUrl = ResponseUri.PathAndQuery;
                }
                else
                {
                    aUrl = ResponseUri.AbsoluteUri;
                }
            }

         
            reqHeader = request.method.ToString () + " " + aUrl + " HTTP/1.1\r\n"
                + GetReqHeader(request) + "\r\n";


            byte[] sBytes = null;
            byte[] sHeaderBytes = null;

            sHeaderBytes = Encoding.ASCII.GetBytes(reqHeader);

            if (request.method == Method.POST)
            {
                if (request.postData == null)
                {
                    sBytes = new byte[sHeaderBytes.Length];
                    Array.Copy(sHeaderBytes, sBytes, sHeaderBytes.Length);
                }
                else
                {
                    sBytes = new byte[sHeaderBytes.Length + request.postData.Length];
                    Array.Copy(sHeaderBytes, sBytes, sHeaderBytes.Length);
                    Array.Copy(request.postData, 0, sBytes, sHeaderBytes.Length, request.postData.Length);
                }
            }
            else
            {
                sBytes = new byte[sHeaderBytes.Length];
                Array.Copy(sHeaderBytes, sBytes, sHeaderBytes.Length);
            }


            if (request.m_IsSsl == true)
            {
                if (request.Credentials == null || request.Credentials.Count == 0)
                {
                    throw new NetCredentialException("未提供证书信息！"); 
                }

                string findValue = request.RequestUri.Host;
                if (findValue.StartsWith("www", StringComparison.CurrentCultureIgnoreCase))
                {
                    findValue = findValue.Substring(4, findValue.Length - 4);

                }

                if (!this.Validate(findValue, request.Credentials[0]))
                {
                    throw new NetCredentialException("证书验证失败！"); 
                }
                this.sStream.Write(sBytes, 0, sBytes.Length);
            }
            else
            {
                socket.Send(sBytes);
            }

        }

        private string GetReqHeader(HttpWebRequest request)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry de in request.Headers)
            {
                sb.Append(de.Key + ":" + de.Value + "\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 发送请求，此方式用于进行文件上传
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uCode"></param>
        /// <param name="fName"></param>
        public void SendRequest(HttpWebRequest request, string fName)
        {
            ResponseUri = request.RequestUri;

            reqHeader = request.method.ToString() + " " + ResponseUri.PathAndQuery + " HTTP/1.1\r\n"
                + GetReqHeader(request) + "\r\n";

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

            string header = reqHeader;
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
                        startHeader.Append(headers[i] + "\r\n\r\n");
                        isStart = false;
                    }
                    else
                    {
                        endHeader.Append(headers[i] + "\r\n");
                    }
                }
                else
                {
                    if (isStart == true)
                    {
                        startHeader.Append(headers[i] + "\r\n");
                    }
                    else
                    {
                        endHeader.Append(headers[i] + "\r\n");
                    }
                }

            }
   
            hdHead = Encoding.ASCII.GetBytes(startHeader.ToString());
            hdEnd = Encoding.ASCII.GetBytes(endHeader.ToString());

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
        public void Receive(HttpWebRequest request)
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

                //this.TextViews = ConvertSource(this.m_CompressEncoding, this.m_wCode);

                if (m_isChunked == true)
                {
                    m_responseStreamData = cUtilities.ClearChunkedFlag(m_responseStreamData, iEntityBodyOffset);
                }
                else
                    m_responseStreamData.Seek(iEntityBodyOffset, SeekOrigin.Begin);

                
            }

            repStream = m_responseStreamData;
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

            if (!isParseHeader)
            {
                ParseHeader(so.buffer, read);
            }

            m_recvOffset += read;
            this.m_responseStreamData.Write(so.buffer, 0, read);

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
        private void ReceiveHeader(HttpWebRequest request)
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

                strHeader.Append(Encoding.ASCII.GetString(bytes, 0, 1));

                hList.Add(bytes[0]);

                if (bytes[0] == '\n' && strHeader.ToString().EndsWith("\r\n\r\n"))
                    break;

            }

            this.HeaderBytes = new byte[hList.Count];
            HeaderBytes = hList.ToArray();

            ParseHeader(HeaderBytes, HeaderBytes.Length);


            #endregion

        }


        /// <summary>
        /// 接受主体数据，当前仅用于SSL的请求接受，
        /// </summary>
        /// <param name="request"></param>
        private void ReceiveData(HttpWebRequest request)
        {

            m_recvOffset = 0;

            if (request.m_IsSsl == true)
            {
                if (m_isChunked == true)
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
                ////开始初始化接收的数据缓存
                /////此为异步接受事件处理，当前未用
                //#region  接受正常数据，仅用于接收非SSL请求，采用异步事件模式 当前未采用
                //SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                //e.AcceptSocket = this.socket;
                //RecvBuffer = new byte[m_RecvBufferLength];

                //e.SetBuffer(RecvBuffer, 0, m_RecvBufferLength);
                //e.Completed += new EventHandler<SocketAsyncEventArgs>(on_Completed);

                //bool isS = socket.ReceiveAsync(e);
                //if (!isS)
                //{
                //    onCompleted(e);
                //}
                //#endregion

            }

        }

        #region 处理各种类型的数据接收，包括SSL 及 CHUNKED
        /// <summary>
        /// 用于接收SSL请求响应数据
        /// </summary>
        /// <param name="ar"></param>
        private void ProcessSSL(IAsyncResult ar)
        {

            int rLen = this.sStream.EndRead(ar);

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
        /// 接收数据完成的事件处理，当接收的数据大于1MB时，将通过流进行数据返回
        /// </summary>
        private void CloseReceiveData()
        {
            this.m_responseStreamData.Write(RecvBuffer, 0, RecvBuffer.Length);

            wait.Set();

            //触发接收完成事件
            //if (OnReceiveComplete != null)
            //    OnReceiveComplete();
        }
        #endregion

        /// <summary>
        /// 关闭socket
        /// </summary>
        internal void Close()
        {
            socket.Close();
        }

        #region 证书处理部分
        private bool Validate(string serverName, X509Certificate2 x509)
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

                throw new NetCredentialException(strMsg, e.InnerException);
            }
            catch (Exception ex)
            {
                throw new NetCredentialException(ex.Message);
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

        private int i1 = 0;
        /// <summary>
        /// 判断是否接受完毕
        /// </summary>
        /// <returns></returns>
        private bool isCompleted()
        {
            i1++;

            bool isOver = false;

            if (this.m_isChunked == true)
            {
                long num;
                if (this._lngLastChunkInfoOffset < this.iEntityBodyOffset)
                {
                    this._lngLastChunkInfoOffset = this.iEntityBodyOffset;
                }
                isOver = cUtilities.IsChunkedBodyComplete(this.m_responseStreamData, this._lngLastChunkInfoOffset, out this._lngLastChunkInfoOffset, out num);
            }
            if (this.Headers["Content-Length"] != null && ("0" != this.Headers["Content-Length"].Trim()))
            {
                long num2;
                if (long.TryParse(this.Headers["Content-Length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2) && (num2 >= 0L))
                {
                    isOver = (this.m_recvOffset >= (this.m_headerOffset + num2 + 1));
                }
                else
                    isOver = true;
            }
            if (this.m_isClose == true)
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
        private bool ParseHeader(byte[] bytes, long bLen)
        {
            if (bLen >= 4)
            {
                HTTPHeaderParseWarnings hHeaderWarnings;
                if (!cUtilities.FindEndOfHeaders(bytes, ref m_headerOffset, bLen, out hHeaderWarnings))
                    return false;

                this.iEntityBodyOffset = this.m_headerOffset + 1;

                if (hHeaderWarnings == HTTPHeaderParseWarnings.EndedWithLFCRLF)
                {

                }
                if (hHeaderWarnings == HTTPHeaderParseWarnings.EndedWithLFLF)
                {

                }
                this.repHeader = Encoding.ASCII.GetString(bytes, 0, m_headerOffset).Trim();
                string[] sHeaderLines = this.repHeader.Replace("\r\n", "\n").Split('\n');

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
                        this.ResponseStateCode = int.Parse(sHeaderLines[0].Substring(0, sHeaderLines[0].IndexOf(' ')).Trim());
                    }

                    #region 在此可以处理根据相应的状态进行的各类信息

                    #endregion

                    for (int i = 1; i < sHeaderLines.Length; i++)
                    {
                        string[] strItem = sHeaderLines[i].Split(new char[] { ':' }, 2);
                         try
                            {
                                if (strItem.Length > 0)
                                {
                           
                                        Headers[strItem[0].Trim()] = strItem[1].Trim();
                           
                                }
                            }
                         catch { }
                    }

                    if (Headers["Transfer-Encoding"] == "chunked")
                        m_isChunked = true;
                    else
                        m_isChunked = false;

                    m_CompressEncoding = Headers["Content-Encoding"];

                    if (Headers["Content-Type"] != null)
                    {
                        Match reg = Regex.Match(Headers["Content-Type"], "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        m_wCode = cUtilities.ConvertCode(reg.ToString());
                    }

                    if (Headers["Content-Length"] != null)
                        ContentLength = int.Parse(Headers["Content-Length"]);
                    KeepAlive = (Headers["Connection"] != null && Headers["Connection"].ToLower() == "keep-alive") ||
                                (Headers["Proxy-Connection"] != null && Headers["Proxy-Connection"].ToLower() == "keep-alive");

                    if ((Headers["Connection"] != null && Headers["Connection"].ToLower() == "close") ||
                        (Headers["Proxy-Connection"] != null && Headers["Proxy-Connection"] == "close"))
                        m_isClose = true;
                    else
                        m_isClose = false;
        
                }

                //解析Cookie
                ParseCookie(this.repHeader);

                if (this.ResponseStateCode == 302 || this.ResponseStateCode==301)
                {
                    try { ResponseUri = new Uri(Headers["Location"]); }
                    catch { ResponseUri = new Uri(ResponseUri, Headers["Location"]); }
                }

                isParseHeader = true;
               
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// 解析返回来的Cookie
        /// </summary>
        /// <param name="cookie"></param>
        private void ParseCookie(string strheader)
        {
            Hashtable cookieTable = new Hashtable();

            //开始处理新的cookie
            Regex re = new Regex(@"(?<=cookie:)[\S\s]+?(?=\r\n)", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(strheader + "\r\n");
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
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry de in cookieTable)
            {
                sb.Append(de.Key + "=" + de.Value + ";");
            }

            this.repCookie = sb.ToString();
            
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
                DownloadSpeed = (m_DownloadCount / 1024) / sSpan.Milliseconds * 1000;

                m_LastTime = DateTime.Now;
                m_DownloadCount = 0;
            }
            return DownloadSpeed;
        }

    }

}
